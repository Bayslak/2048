using Godot;
using System;
using Empty.scripts;

public partial class Cell : Node2D
{
    [Export] private Sprite2D _sprite;
    [Export] private Label _label;

    [Export] private bool _empty = true;
    
    public override void _Ready()
    {
        _sprite =  GetNode<Sprite2D>("Sprite2D");
        _label = GetNode<Label>("Label");

        Initialize();
    }

    private void Initialize()
    {
        if (!_empty)
            return;
        
        _label.Text = string.Empty;
        _sprite.SelfModulate = CellConstants.EMPTY;
    }
}

public record CellPosition(int Row, int Column);
