using Godot;
using System;

public partial class AudioSettings : TabBar
{
    private readonly PackedScene audioSettingsScene = GD.Load<PackedScene>("res://scenes/core/VolumeButton.tscn");

    private VBoxContainer audioList;

    public override void _Ready()
    {
        audioList = GetNode<VBoxContainer>("VBoxContainer/ScrollContainer/AudioActionList");

        PopulateAudioList();
    }

    private void PopulateAudioList()
    {
        CreateVolumeRow("Master Volume", "Master");
        CreateVolumeRow("Music", "Music");
        CreateVolumeRow("Sound Effects", "SFX");
        CreateVolumeRow("Footsteps", "Footsteps");
    }

    private void CreateVolumeRow(string label, string busName)
    {
        Node row = audioSettingsScene.Instantiate();

        Label nameLabel = row.FindChild("LabelName") as Label;
        Label volumeLabel = row.FindChild("LabelVolume") as Label;
        Button minusButton = row.FindChild("MinusButton") as Button;
        Button plusButton = row.FindChild("PlusButton") as Button;

        GD.Print($"row null: {row == null}");
        GD.Print($"nameLabel null: {nameLabel == null}");
        GD.Print($"volumeLabel null: {volumeLabel == null}");
        GD.Print($"minusButton null: {minusButton == null}");
        GD.Print($"plusButton null: {plusButton == null}");
        GD.Print($"AudioManager.Instance null: {AudioManager.Instance == null}");

        nameLabel.Text = label;

        int volume = AudioManager.Instance.GetVolume(busName);
        volumeLabel.Text = $"{volume}%";

        minusButton.Pressed += () =>
        {
            AudioManager.Instance.DecreaseVolume(busName);

            volumeLabel.Text =
                $"{AudioManager.Instance.GetVolume(busName)}%";
        };

        plusButton.Pressed += () =>
        {
            AudioManager.Instance.IncreaseVolume(busName);

            volumeLabel.Text =
                $"{AudioManager.Instance.GetVolume(busName)}%";
        };

        audioList.AddChild(row);
    }
}
