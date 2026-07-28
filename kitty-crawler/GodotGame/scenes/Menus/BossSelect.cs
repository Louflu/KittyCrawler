using Godot;
using System;

public partial class BossSelect : Control
{
    [Signal]
    public delegate void BossSelectedEventHandler(string bossName);

    [Export]
    public float HoverScale = 1.1f;

    [Export]
    public float AnimationSpeed = 0.1f;

    public override void _Ready()
	{
        foreach (Node node in GetTree().GetNodesInGroup("BossButton"))
        {
            if (node is TextureButton button)
            {
                SetupButton(button);
            }
        }
    }

    private void SetupButton(TextureButton button)
    {
        button.PivotOffset = button.Size / 2;

        button.MouseEntered += () =>
        {
            HoverButton(button);
        };

        button.MouseExited += () =>
        {
            UnhoverButton(button);
        }; 
    }

    private void HoverButton(TextureButton button)
    {
        Tween tween = CreateTween();

        tween.TweenProperty(button, "scale", Vector2.One * HoverScale, AnimationSpeed)
             .SetTrans(Tween.TransitionType.Back)
             .SetEase(Tween.EaseType.Out);
    }

    private void UnhoverButton(TextureButton button)
    {
        Tween tween = CreateTween();

        tween.TweenProperty(button, "scale", Vector2.One, AnimationSpeed)
             .SetTrans(Tween.TransitionType.Quad)
             .SetEase(Tween.EaseType.Out);
    }

    public void OnMioButtonPressed()
    {
        GD.Print("Mio button pressed");
        EmitSignal("BossSelected", "Mio");
    }

    public void OnMedusaButtonPressed()
    {
        GD.Print("Medusa button pressed");
        EmitSignal("BossSelected", "Medusa");
    }
}
