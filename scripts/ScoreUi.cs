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
    
    [Export] private Button _quitButton;
    
    public event Action RestartButtonPressed;

    public override void _Ready()
    {
        _scoreBoxContainer = GetNode<VBoxContainer>("v_container");
        _startingLabel = GetNode<Label>("v_container/start_game_label");
        
        _pointsContainer = GetNode<VBoxContainer>("v_container/v_points_container");
        _pointsLabel = GetNode<Label>("v_container/v_points_container/points");
        _bestScoreLabel = GetNode<Label>("v_container/v_points_container/best_points");
        
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

    public void UpdatePoints(int points) => _pointsLabel.Text = $"Score: " + points;

    public void UpdateBestScore(int bestScore) => _bestScoreLabel.Text  = $"Best: {bestScore}";
}
