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
    public event Action<int> MovingAnimationFinished;
    public event Action GameWon;
    public event Action GameLost;

    public override void _Ready()
    {
        if (_cellScene == null)
            GD.PrintErr("Missing cell packed scene in board scene.");

        _sprite = GetNode<Sprite2D>("Sprite2D");
        
        Initialize();
    }

    private Vector2 DetermineBoardSize()
    {
        var size = _sprite.GetRect().Size;

        _boardWidth = size.X;
        _boardHeight = size.Y;

        return size;
    }

    public void Initialize()
    {
        _ = DetermineBoardSize();
        var viewportCenter = GetViewportRect().Size / 2f;
        this.Position = new Vector2(viewportCenter.X / 1.8f, viewportCenter.Y);
        
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
            var possibleCells = _backgroundCells.Keys.Where(cp => !_playingCells.ContainsKey(cp)).ToList();
            var randomCellPosition =  possibleCells[random.Next(0, possibleCells.Count)];
            SpawnNumber(randomCellPosition, value);
        }
    }

    private void AddNumber()
    {
        var numberRandomness = new Random().Next(0, 10);
        var possibleCells = _backgroundCells.Keys.Where(cp => !_playingCells.ContainsKey(cp)).ToList();
        var randomCellPosition = possibleCells[new Random().Next(0, possibleCells.Count)];
        SpawnNumber(randomCellPosition, numberRandomness > 9 ? 4 : 2);
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

    public bool Move(MoveDirection moveDirection)
    {
        var cellsBefore = new Dictionary<CellPosition, Cell>(_playingCells);
        
        var cellsToMove = _playingCells.Where(c => !c.Value.Empty).ToList();
        
        var result = CalculateMoves(moveDirection, cellsToMove);

        if (result.newPositions.Count == 0 || !cellsBefore.Except(result.newPositions).Any())
            return false;

        Tween lastTween = null;
        foreach (var cell in result.newPositions)
        {
            var tween = GetTree().CreateTween();
            tween.TweenProperty(cell.Value, "position", _backgroundCells[cell.Key].Position, 0.1f);
            lastTween = tween;
        }

        _playingCells = result.newPositions;
        lastTween!.Finished += () => MovingAnimationFinished?.Invoke(result.pointsToAdd);

        AddNumber();
        
        foreach (var cell in _playingCells.Values)
            cell.ResetUppedProperty();
        
        return true;
    }

    private (Dictionary<CellPosition, Cell> newPositions, int pointsToAdd) CalculateMoves(MoveDirection moveDirection, List<KeyValuePair<CellPosition, Cell>> cellsToMove)
    {
        var result = new Dictionary<CellPosition, Cell>();
        var pointsToAdd = 0;

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
                            if (value.Value != cell.Value.Value || value.HasAlreadyBeenUpped)
                            {
                                result.Add(new CellPosition(i - 1, cell.Key.Column), cell.Value);
                                _playingCells.Remove(cell.Key);
                                _playingCells.Add(new CellPosition(i - 1, cell.Key.Column), cell.Value);
                            }
                            else if (value.Value == cell.Value.Value)
                            {
                                int points = value.Value * 2;
                                value.IncrementValueTo(points);
                                pointsToAdd += points;
                                
                                _playingCells.Remove(cell.Key);
                                cell.Value.QueueFree();
                            }
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
                            if (value.Value != cell.Value.Value || value.HasAlreadyBeenUpped)
                            {
                                result.Add(new CellPosition(i + 1, cell.Key.Column), cell.Value);
                                _playingCells.Remove(cell.Key);
                                _playingCells.Add(new CellPosition(i + 1, cell.Key.Column), cell.Value);
                            }
                            else if (value.Value == cell.Value.Value)
                            {
                                int points = value.Value * 2;
                                value.IncrementValueTo(points);
                                pointsToAdd += points;
                                
                                _playingCells.Remove(cell.Key);
                                cell.Value.QueueFree();
                            }
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
                            if (value.Value != cell.Value.Value || value.HasAlreadyBeenUpped)
                            {
                                result.Add(new CellPosition(cell.Key.Row, i + 1), cell.Value);
                                _playingCells.Remove(cell.Key);
                                _playingCells.Add(new CellPosition(cell.Key.Row, i + 1), cell.Value);
                            }
                            else if (value.Value == cell.Value.Value)
                            {
                                int points = value.Value * 2;
                                value.IncrementValueTo(points);
                                pointsToAdd += points;

                                _playingCells.Remove(cell.Key);
                                cell.Value.QueueFree();
                            }
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
                            if (value.Value != cell.Value.Value || value.HasAlreadyBeenUpped)
                            {
                                result.Add(new CellPosition(cell.Key.Row, i - 1), cell.Value);
                                _playingCells.Remove(cell.Key);
                                _playingCells.Add(new CellPosition(cell.Key.Row, i - 1), cell.Value);
                            }
                            else if (value.Value == cell.Value.Value)
                            {
                                int points = value.Value * 2;
                                value.IncrementValueTo(points);
                                pointsToAdd += points;

                                _playingCells.Remove(cell.Key);
                                cell.Value.QueueFree();
                            }
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
        
        return (result, pointsToAdd);
    }

    public void CheckIfWin()
    {
        bool isWon = _playingCells.Values.Any(c => c.Value == 2048);
        if (isWon)
            GameWon?.Invoke();
    }

    public void CheckIfNoMoreMoves()
    {
        bool boardFull = _playingCells.Count == 16
            && _playingCells.Values.All(c => !c.Empty);

        if (!boardFull)
            return;

        if (!AnyAdjacentMergePossible())
            GameLost?.Invoke();
    }

    private bool AnyAdjacentMergePossible()
    {
        foreach (var (pos, cell) in _playingCells)
        {
            int value = cell.Value;

            if (_playingCells.TryGetValue(new CellPosition(pos.Row, pos.Column + 1), out var rightVal) && rightVal.Value == value)
                return true;
            if (_playingCells.TryGetValue(new CellPosition(pos.Row - 1, pos.Column), out var downVal) && downVal.Value == value)
                return true;
        }
        return false;
    }
}