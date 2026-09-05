using Godot;

public partial class GameManager : Node2D
{
    [Export] private Label _startLabel;
    [Export] private Board _board;

    private bool _waitingToStart = true;
    
    public override void _Ready()
    {
        _startLabel = GetParent().GetNode<Label>("start_label");
        _board = GetParent().GetNode<Board>("board");
    }

    public override void _Process(double delta)
    {
        if (Input.IsKeyPressed(Key.Enter) && _waitingToStart)
        {
            _waitingToStart = false;
            
            var tween = GetTree().CreateTween();
            tween.TweenProperty(_startLabel, "modulate", new Color(1, 1, 1, 0), 0.2);
            
            _board.AnimateStart();
        }
    }
}