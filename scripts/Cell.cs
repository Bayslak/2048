using Godot;
using Empty.scripts;

public partial class Cell : Node2D
{
    [Export] private Sprite2D _sprite;
    [Export] private Label _label;

    [Export] private bool _empty = true;
    private Vector2 _startingScale;
    
    public override void _Ready()
    {
        _sprite =  GetNode<Sprite2D>("Sprite2D");
        _label = GetNode<Label>("Label");

        _startingScale = this.Scale;
    }

    public void Initialize()
    {
        _label.Text = string.Empty;
        _sprite.SelfModulate = CellConstants.EMPTY;

        this.Visible = false;
        this.Scale = Vector2.Zero;
    }

    public void AnimateStart(float delay)
    {
        this.Visible = true;
        var tween = GetTree().CreateTween();
        tween.TweenProperty(this, "scale", _startingScale, 0.2f).SetDelay(delay);
    }

    public void SetValue(int value)
    {
        _label.Text = value.ToString();
        _sprite.SelfModulate = CellConstants.NUMBERS_TO_COLORS[value];
    }
}

public record CellPosition(int Row, int Column);