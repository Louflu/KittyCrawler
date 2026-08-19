using Godot;
using System;

public partial class KeybindSettings : TabBar
{
    private readonly PackedScene inputButtonScene = GD.Load<PackedScene>("res://scenes/core/InputButton.tscn");

    private VBoxContainer actionList;

    private bool isRemapping = false;
    private StringName actionToRemap = null;
    private Button remappingButton = null;

    public override void _Ready()
	{
       actionList = GetNode<VBoxContainer>("VBoxContainer/ScrollContainer/ActionList");

       PopulateActionList();
    }

    private readonly StringName[] remappableActions =
    {
        "up",
        "left",
        "down",
        "right",
        "interact",
        "escape",
    };

    private void PopulateActionList()
    {
        InputMap.LoadFromProjectSettings();

        foreach (Node item in actionList.GetChildren())
        {
            item.QueueFree();
        }

        foreach (StringName action in remappableActions)
        {
            Node button = inputButtonScene.Instantiate();

            Label actionLabel = button.FindChild("LabelAction") as Label;
            Label inputLabel = button.FindChild("LabelInput") as Label;

            actionLabel.Text = action.ToString();

            var events = InputMap.ActionGetEvents(action);

            if (events.Count > 0)
            {
                InputEvent inputEvent = events[0];

                if (inputEvent is InputEventKey keyEvent)
                {
                    inputLabel.Text = OS.GetKeycodeString(keyEvent.PhysicalKeycode);
                }
                else
                {
                    inputLabel.Text = inputEvent.AsText();
                }
            }
            else
            {
                inputLabel.Text = "Unassigned";
            }

            actionList.AddChild(button);
        }
    }
}
