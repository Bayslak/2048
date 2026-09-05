using System;
using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class Board : Node2D
{
    [Export] private PackedScene _cellScene;

    private Sprite2D _sprite;
    private double _boardWidth,  _boardHeight;
    
    private int _rows = 4, _columns = 4;
    private Vector2 _cellSize;

    private readonly Dictionary<CellPosition, Cell> _backgroundCells = new Dictionary<CellPosition, Cell>();
    private Dictionary<CellPosition, Cell> _playingCells = new Dictionary<CellPosition, Cell>();

    private float _timeBetweenSpawns = 0.08f;
    public event Action StartingAnimationFinished;
    public bool IsMoving = false;

    public override void _Ready()
    {
        if (_cellScene == null)
            GD.PrintErr("Missing cell packed scene in board scene.");

        _sprite = GetNode<Sprite2D>("Sprite2D");
        
        Initialize();
    }

    private void DetermineBoardSize()
    {
        var size = _sprite.GetRect().Size;

        _boardWidth = size.X;
        _boardHeight = size.Y;
    }

    public void Initialize()
    {
        DetermineBoardSize();
        
        _cellSize = new Vector2((float)_boardWidth / _columns, (float)_boardHeight / _rows);
        var startingPosition = new Vector2(-((float)_boardWidth / 2) + _cellSize.X / 2,
            (float)_boardHeight / 2 - _cellSize.Y / 2);

        InstantiateCells(startingPosition);
    }

    private void InstantiateCells(Vector2 startingPosition)
    {
        for (int i = 0; i < _columns; i++)
        {
            for (int y = 0; y < _rows; y++)
            {
                var cell = _cellScene.Instantiate<Cell>();
                cell.Name = i + "_" + y;

                var cellPosition = new CellPosition(y, i);
                AddChild(cell);

                cell.Position = startingPosition;
                cell.Position += new Vector2(_cellSize.X * i, 0);
                cell.Position -= new Vector2(0, _cellSize.Y * y);
                
                _backgroundCells.Add(cellPosition, cell);

                cell.Initialize();
            }
        }
    }

    public void AnimateStart()
    {
        var cells = _backgroundCells.Values.ToList();
        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].AnimateStart();
        }
        
        StartingAnimationFinished?.Invoke();
    }

    public void SpawnStartingNumbers(int howMany, int value)
    {
        var random = new Random();
        for (int i = 0; i < howMany; i++)
        {
            var randomRow = random.Next(_rows);
            var randomColumn = random.Next(_columns);
            
            SpawnNumber(new CellPosition(randomRow, randomColumn), value);
        }
    }

    private void SpawnNumber(CellPosition cellPosition, int value)
    {
        var cell = _cellScene.Instantiate<Cell>();
        cell.Name = _playingCells.Values.Count + 1 + "_cell";
        AddChild(cell);
        
        cell.Position = _backgroundCells[cellPosition].Position;
        cell.Initialize();
        
        cell.SetValue(value);
        cell.AnimateStart();
        
        _playingCells.Add(cellPosition, cell);
    }

    public void Move(MoveDirection moveDirection)
    {
        IsMoving = true;
        
        var cellsToMove = _playingCells.Where(c => !c.Value.Empty).ToList();
        
        // i need to understand next position for each cell that can move, if they can move
        var result = CalculatePossibleMoves(moveDirection, cellsToMove);

        foreach (var cell in result)
        {
            var tween = GetTree().CreateTween();
            tween.TweenProperty(cell.Value, "position", _backgroundCells[cell.Key].Position, 0.1f);
        }

        _playingCells = result;

        IsMoving = false;
    }

    private Dictionary<CellPosition, Cell> CalculatePossibleMoves(MoveDirection moveDirection, List<KeyValuePair<CellPosition, Cell>> cellsToMove)
    {
        var result = new Dictionary<CellPosition, Cell>();

        switch (moveDirection)
        {
            case MoveDirection.Up:
                var orderByDescendingRow = cellsToMove.OrderByDescending(kvp => kvp.Key.Row);
                foreach (var cell in orderByDescendingRow)
                {
                    if (cell.Key.Row > 2)
                    {
                        result.Add(cell.Key, cell.Value);
                        continue;
                    }

                    for (int i = cell.Key.Row + 1; i < _rows; i++)
                    {
                        _playingCells.TryGetValue(new CellPosition(i, cell.Key.Column), out var value);

                        if (value is { Empty: false })
                        {
                            result.Add(new CellPosition(i - 1, cell.Key.Column), cell.Value);
                            _playingCells.Remove(cell.Key);
                            _playingCells.Add(new CellPosition(i - 1, cell.Key.Column), cell.Value);
                            break;
                        }

                        if (i == _rows - 1)
                        {
                            result.Add(new CellPosition(i, cell.Key.Column), cell.Value);
                            _playingCells.Remove(cell.Key);
                            _playingCells.Add(new CellPosition(i, cell.Key.Column), cell.Value);
                        }
                    }
                }
                break;
            case MoveDirection.Down:
                var orderedByAscendingRow = cellsToMove.OrderBy(kvp => kvp.Key.Row);
                foreach (var cell in orderedByAscendingRow)
                {
                    if (cell.Key.Row < 1)
                    {
                        result.Add(cell.Key, cell.Value);
                        continue;
                    }

                    for (int i = cell.Key.Row - 1; i >= 0; i--)
                    {
                        _playingCells.TryGetValue(new CellPosition(i, cell.Key.Column), out var value);

                        if (value is { Empty: false })
                        {
                            result.Add(new CellPosition(i + 1, cell.Key.Column), cell.Value);
                            _playingCells.Remove(cell.Key);
                            _playingCells.Add(new CellPosition(i + 1, cell.Key.Column), cell.Value);
                            break;
                        }

                        if (i == 0)
                        {
                            result.Add(new CellPosition(i, cell.Key.Column), cell.Value);
                            _playingCells.Remove(cell.Key);
                            _playingCells.Add(new CellPosition(i, cell.Key.Column), cell.Value);
                        }
                    }
                }
                break;
            case MoveDirection.Left:
                var orderedByAscendingColumn = cellsToMove.OrderBy(kvp => kvp.Key.Column);
                foreach (var cell in orderedByAscendingColumn)
                {
                    if (cell.Key.Column < 1)
                    {
                        result.Add(cell.Key, cell.Value);
                        continue;
                    }

                    for (int i = cell.Key.Column - 1; i >= 0; i--)
                    {
                        _playingCells.TryGetValue(new CellPosition(cell.Key.Row, i), out var value);

                        if (value is { Empty: false })
                        {
                            result.Add(new CellPosition(cell.Key.Row, i + 1), cell.Value);
                            _playingCells.Remove(cell.Key);
                            _playingCells.Add(new CellPosition(cell.Key.Row, i + 1), cell.Value);
                            break;
                        }

                        if (i == 0)
                        {
                            result.Add(new CellPosition(cell.Key.Row, i), cell.Value);
                            _playingCells.Remove(cell.Key);
                            _playingCells.Add(new CellPosition(cell.Key.Row, i), cell.Value);
                        }
                    }
                }
                break;
            case MoveDirection.Right:
                var orderedByDescendingColumn = cellsToMove.OrderByDescending(kvp => kvp.Key.Column);
                foreach (var cell in orderedByDescendingColumn)
                {
                    if (cell.Key.Column > 2)
                    {
                        result.Add(cell.Key, cell.Value);
                        continue;
                    }

                    for (int i = cell.Key.Column + 1; i < _columns; i++)
                    {
                        _playingCells.TryGetValue(new CellPosition(cell.Key.Row, i), out var value);

                        if (value is { Empty: false })
                        {
                            _playingCells.Remove(cell.Key);
                            result.Add(new CellPosition(cell.Key.Row, i - 1), cell.Value);
                            _playingCells.Add(new CellPosition(cell.Key.Row, i - 1), cell.Value);
                            break;
                        }

                        if (i == _columns - 1)
                        {
                            result.Add(new CellPosition(cell.Key.Row, i), cell.Value);
                            _playingCells.Remove(cell.Key);
                            _playingCells.Add(new CellPosition(cell.Key.Row, i), cell.Value);
                        }
                    }
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(moveDirection), moveDirection, null);
        }
        
        return result;
    }
}