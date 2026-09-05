using Godot;
using System;
using System.Collections.Generic;

public partial class Board : Node2D
{
    [Export] private PackedScene _cellScene;

    private Sprite2D _sprite;
    private double _boardWidth,  _boardHeight;
    
    private int _rows = 4, _columns = 4;
    private Vector2 _cellSize;

    private readonly Dictionary<CellPosition, Cell> _cells = [];

    public override void _Ready()
    {
        if (_cellScene == null)
            GD.PrintErr("Missing cell packed scene in board scene.");

        _sprite = GetNode<Sprite2D>("Sprite2D");
        
        Initialize();
        
        GD.Print("All good.");
    }

    private void DetermineBoardSize()
    {
        var size = _sprite.GetRect().Size;

        _boardWidth = size.X;
        _boardHeight = size.Y;
    }

    private void Initialize()
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
            }
        }
    }
}
