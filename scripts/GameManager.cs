using Empty.scripts;
using Godot;

public partial class GameManager : Node2D
{
    [Export] private Board _board;
    [Export] private ScoreUi _scoreBoard;

    private bool _waitingToStart = true;
    [Export] private bool _canListenToInput;

    private int _points = 0;

    private const string SavePath = "user://save.json";
    private int _bestScore = 0;
    
    public override void _Ready()
    {
        _board = GetParent().GetNode<Board>("board");
        _scoreBoard = GetParent().GetNode<ScoreUi>("score_ui");

        _board.StartingAnimationFinished += HandleStartingAnimationFinished;
        _board.MovingAnimationFinished += HandleMovingAnimationFinished;
        _board.GameWon += HandleGameWon;
        _board.GameLost += HandleGameLost;
        
        RenderingServer.SetDefaultClearColor(Constants.BACKGROUND);

        _scoreBoard.RestartButtonPressed += HandleRestartEvent;
    }

    public override void _ExitTree()
    {
        _board.StartingAnimationFinished -= HandleStartingAnimationFinished;
        _board.MovingAnimationFinished -= HandleMovingAnimationFinished;
        _board.GameWon -= HandleGameWon;
        _board.GameLost -= HandleGameLost;
        
        _scoreBoard.RestartButtonPressed -= HandleRestartEvent;
    }

    public override void _Process(double delta)
    {
        if (Input.IsKeyPressed(Key.Enter) && _waitingToStart)
        {
            _waitingToStart = false;
            _board.AnimateStart();
            _scoreBoard.AnimateStart();
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
            if(_board.Move(MoveDirection.Left))
                _canListenToInput = false;
        } 
        else if (Input.IsActionJustPressed("right"))
        {
            if(_board.Move(MoveDirection.Right))
                _canListenToInput = false;
        }
        else if (Input.IsActionJustPressed("up"))
        {
            if(_board.Move(MoveDirection.Up))
                _canListenToInput = false;
        }
        else if (Input.IsActionJustPressed("down"))
        {
            if(_board.Move(MoveDirection.Down))
                _canListenToInput = false;
        }
    }

    private void HandleStartingAnimationFinished()
    {
        StartGame();
    }
    
    private void HandleMovingAnimationFinished(int pointsToAdd)
    {
        _canListenToInput = true;

        if (pointsToAdd > 0)
        {
            _points += pointsToAdd;
            _scoreBoard.UpdatePoints(_points);
        }
        
        _board.CheckIfWin();
        _board.CheckIfNoMoreMoves();
    }

    private void HandleGameWon()
    {
        _scoreBoard.UpdateGameResultLabel("Game won!");
        _canListenToInput = false;

        CheckBestScore();
    }

    private void HandleGameLost()
    {
        _scoreBoard.UpdateGameResultLabel("Game lost!");
        _canListenToInput = false;

        CheckBestScore();
    }

    private void StartGame()
    {
        _bestScore = LoadBest();
        _scoreBoard.UpdateBestScore(_bestScore);
        
        _board.SpawnStartingNumbers(2, 2);
        _canListenToInput = true;
        _scoreBoard.UpdatePoints(_points);
    }

    private void HandleRestartEvent()
    {
        _board.Reset();
        _board.SpawnStartingNumbers(2, 2);
        _canListenToInput = true;
        
        _points = 0;
        _scoreBoard.UpdatePoints(_points);
        _scoreBoard.ResetGameResult();
    }

    #region SAVE
    
    private void SaveBest(int best)
    {
        var data = new Godot.Collections.Dictionary { { "best", best } };
        using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Write);
        if (file == null)
        {
            GD.PrintErr($"Save failed: {FileAccess.GetOpenError()}");
            return;
        }
        file.StoreString(Json.Stringify(data));
    }

    private int LoadBest()
    {
        if (!FileAccess.FileExists(SavePath))
            return 0;

        using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Read);
        if (file == null)
            return 0;

        var parsed = Json.ParseString(file.GetAsText());
        if (parsed.VariantType != Variant.Type.Dictionary)
            return 0;

        return (int)parsed.AsGodotDictionary()["best"];
    }

    private void CheckBestScore()
    {
        if (_points <= _bestScore)
            return;
            
        _bestScore = _points;
        _scoreBoard.UpdateBestScore(_bestScore);
        SaveBest(_bestScore);
    }
    
    #endregion
}

public enum MoveDirection
{
    Up,
    Down,
    Left,
    Right
}