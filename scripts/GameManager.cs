using Godot;

public partial class GameManager : Node2D
{
    [Export] private Label _startLabel;
    [Export] private Board _board;

    private bool _waitingToStart = true;
    [Export] private bool _canListenToInput;
    
    public override void _Ready()
    {
        _startLabel = GetParent().GetNode<Label>("start_label");
        _board = GetParent().GetNode<Board>("board");

        _board.StartingAnimationFinished += HandleStartingAnimationFinished;
        _board.MovingAnimationFinished += HandleMovingAnimationFinished;
    }

    public override void _ExitTree()
    {
        _board.StartingAnimationFinished -= HandleStartingAnimationFinished;
        _board.MovingAnimationFinished -= HandleMovingAnimationFinished;
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

        if(_canListenToInput)
            HandleMovements();
    }

    private void HandleMovements()
    {
        if (Input.IsActionJustPressed("left"))
        {
            _board.Move(MoveDirection.Left);
            _canListenToInput = false;
        } 
        else if (Input.IsActionJustPressed("right"))
        {
            _board.Move(MoveDirection.Right);
            _canListenToInput = false;
        }
        else if (Input.IsActionJustPressed("up"))
        {
            _board.Move(MoveDirection.Up);
            _canListenToInput = false;
        }
        else if (Input.IsActionJustPressed("down"))
        {
            _board.Move(MoveDirection.Down);
            _canListenToInput = false;
        }
        else
        {
            _canListenToInput = true;
        }
    }

    private void HandleStartingAnimationFinished()
    {
        StartGame();
    }
    
    private void HandleMovingAnimationFinished()
    {
        _canListenToInput = true;
    }

    private void StartGame()
    {
        _board.SpawnStartingNumbers(2, 2);
        _canListenToInput = true;
    }
}

public enum MoveDirection
{
    Up,
    Down,
    Left,
    Right
}