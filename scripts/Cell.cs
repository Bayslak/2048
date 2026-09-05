using Godot;
using Empty.scripts;

public partial class Cell : Node2D
{
    [Export] private Sprite2D _sprite;
    [Export] private Label _label;
    [Export] private Area2D _area;

    [Export] private bool _empty = true;
    
    private Vector2 _startingScale;
    private bool _isHovered = false;
    
    public override void _Ready()
    {
        _sprite =  GetNode<Sprite2D>("Sprite2D");
        _label = GetNode<Label>("Label");
        _area = GetNode<Area2D>("Area2D");

        _startingScale = this.Scale;
        
        Initialize();

        _area.MouseEntered += HandleMouseEnteredEvent;
        _area.MouseExited += HandleMouseExitedEvent;
    }

    public override void _ExitTree()
    {
        _area.MouseEntered -= HandleMouseEnteredEvent;
        _area.MouseExited -= HandleMouseExitedEvent;
    }

    private void Initialize()
    {
        if (!_empty)
            return;
        
        _label.Text = string.Empty;
        _sprite.SelfModulate = CellConstants.EMPTY;
    }
    
    private void HandleMouseEnteredEvent()
    {
        if (_isHovered)
            return;
        
        _isHovered = true;
        this.Scale *= 1.1f;
    }
    
    private void HandleMouseExitedEvent()
    {
        if (!_isHovered)
            return;
        
        _isHovered = false;
        this.Scale = _startingScale;
    }
}

public record CellPosition(int Row, int Column);