using Godot;
using Empty.scripts;

public partial class Cell : Node2D
{
    [Export] private Sprite2D _sprite;
    [Export] private Label _label;
    public int Value => int.Parse(_label.Text);
    public bool HasAlreadyBeenUpped = false;

    [Export] public bool Empty = true;
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
        _sprite.SelfModulate = Constants.EMPTY;

        this.Visible = false;
        this.Scale = Vector2.Zero;
    }

    public void AnimateStart()
    {
        this.Visible = true;
        var tween = GetTree().CreateTween();
        tween.TweenProperty(this, "scale", _startingScale, 0.2f);
    }

    public void SetValue(int value)
    {
        _label.Text = value.ToString();
        _label.AddThemeFontSizeOverride("font_size", Constants.NUMBERS_TO_TEXT_SIZE[value]);
        
        _sprite.SelfModulate = Constants.NUMBERS_TO_COLORS[value];
        
        Empty = false;
    }

    public void IncrementValueTo(int value)
    {
        this.Scale = Vector2.Zero;
        _label.Text = value.ToString();
        _label.AddThemeFontSizeOverride("font_size", Constants.NUMBERS_TO_TEXT_SIZE[value]);
        
        _sprite.SelfModulate = Constants.NUMBERS_TO_COLORS[value];
        Empty = false;
        
        var tween = GetTree().CreateTween();
        tween.TweenProperty(this, "scale", _startingScale, 0.2f);
        
        HasAlreadyBeenUpped = true;
    }
    
    public void ResetUppedProperty() =>  HasAlreadyBeenUpped = false;
}

public record CellPosition(int Row, int Column);