using System;
using Godot;

public partial class ScoreUi : Control
{
    [Export] private VBoxContainer _scoreBoxContainer;
    [Export] private Label _startingLabel; 
    
    [Export] private VBoxContainer _pointsContainer;
    [Export] private Label _pointsLabel;
    [Export] private Label _bestScoreLabel;
    [Export] private Button _restartButton;
    [Export] private Label _gameResultLabel;
    
    [Export] private Button _quitButton;
    
    public event Action RestartButtonPressed;

    public override void _Ready()
    {
        _scoreBoxContainer = GetNode<VBoxContainer>("v_container");
        _startingLabel = GetNode<Label>("v_container/start_game_label");
        
        _pointsContainer = GetNode<VBoxContainer>("v_container/v_points_container");
        _pointsLabel = GetNode<Label>("v_container/v_points_container/h_points_container/points");
        _bestScoreLabel = GetNode<Label>("v_container/v_points_container/h_best_points_container/best_points");
        _gameResultLabel = GetNode<Label>("v_container/v_points_container/game_result");
        
        _restartButton = GetNode<Button>("v_container/v_points_container/try_again_b");
        _restartButton.Pressed += HandleRestartButtonPressed;

        _quitButton = GetNode<Button>("quit_b");
        _quitButton.Pressed += HandleQuitButtonPressed;
    }

    public override void _ExitTree()
    {
        _restartButton.Pressed -= HandleRestartButtonPressed;
        _quitButton.Pressed -= HandleQuitButtonPressed;
    }

    private void HandleQuitButtonPressed()
    {
        GetTree().Quit();
    }

    private void HandleRestartButtonPressed()
    {
        RestartButtonPressed?.Invoke();
        _restartButton.ReleaseFocus();
    }

    public void AnimateStart()
    {
        DissolveLabel(_startingLabel);
        MakeScoresAppear(_pointsContainer);
    }

    private void MakeScoresAppear(VBoxContainer pointsContainer)
    {
        var tween = GetTree().CreateTween();
        tween.TweenProperty(pointsContainer, "visible", true, 0.1f);
    }

    private void DissolveLabel(Label startingLabel)
    {
        var tween = GetTree().CreateTween();
        tween.TweenProperty(startingLabel, "visible", false, 0.1f);
    }

    public void UpdatePoints(int points)
    {
        _pointsLabel.Text = points.ToString();

        _pointsLabel.PivotOffset = _pointsLabel.Size / 2f;

        var tween = GetTree().CreateTween();
        tween.TweenProperty(_pointsLabel, "scale", new Vector2(1.2f, 1.2f), 0.1f);
        tween.TweenProperty(_pointsLabel, "scale", Vector2.One, 0.1f);
    }

    public void UpdateBestScore(int bestScore) => _bestScoreLabel.Text  = bestScore.ToString();

    public void UpdateGameResultLabel(string label)
    {
        _gameResultLabel.Text = label;
        _gameResultLabel.Visible = true;
    }

    public void ResetGameResult()
    {
        if (!_gameResultLabel.Visible)
            return;
        
        _gameResultLabel.Text = string.Empty;
        _gameResultLabel.Visible = false;
    }
}
