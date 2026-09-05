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

    private readonly Dictionary<CellPosition, Cell> _cells = [];

    private float _timeBetweenSpawns = 0.08f;
    public event Action StartingAnimationFinished;

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

                var cellPosition = new CellPosition(i, y);
                AddChild(cell);

                cell.Position = startingPosition;
                cell.Position += new Vector2(_cellSize.X * i, 0);
                cell.Position -= new Vector2(0, _cellSize.Y * y);
                
                _cells.Add(cellPosition, cell);

                cell.Initialize();
            }
        }
    }

    public void AnimateStart()
    {
        var cells = _cells.Values.ToList();
        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].AnimateStart(i * _timeBetweenSpawns);
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
        var cell =  _cells[cellPosition];
        cell.SetValue(value);
    }
}
