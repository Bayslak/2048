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

        _board.StartingAnimationFinished += HandleStartingAnimationFinished;
    }

    public override void _ExitTree()
    {
        _board.StartingAnimationFinished -= HandleStartingAnimationFinished;
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

        if (_waitingToStart)
            return;

        if(!_board.IsMoving)
            HandleMovements();
    }

    private void HandleMovements()
    {
        if (Input.IsActionJustPressed("left"))
        {
            _board.Move(MoveDirection.Left);
        } 
        else if (Input.IsActionJustPressed("right"))
        {
            _board.Move(MoveDirection.Right);
        }
        else if (Input.IsActionJustPressed("up"))
        {
            _board.Move(MoveDirection.Up);
        }
        else if (Input.IsActionJustPressed("down"))
        {
            _board.Move(MoveDirection.Down);
        }
    }

    private void HandleStartingAnimationFinished()
    {
        StartGame();
    }

    private void StartGame()
    {
        _board.SpawnStartingNumbers(2, 2);
    }
}

public enum MoveDirection
{
    Up,
    Down,
    Left,
    Right
}