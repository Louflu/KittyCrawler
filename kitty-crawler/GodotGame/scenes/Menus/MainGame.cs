using Game.Core;
using Godot;
using System;

public partial class MainGame : Node2D
{
    private MainMenu mainMenu;
    private BossSelect bossSelect;
    private LevelTransition _levelTransition;
    // private GameTimerManager _gameTimer;
    // private PauseMenu pauseMenu;
    // private GameOver endMenu;
    // private const string LeaderboardScenePath = "res://scenes/MainMenu/Leaderboard/Leaderboard.tscn";
    private const string GameScenePath = "res://TELT/Scenes/TeltBattle.tscn";

    public override void _Ready()
    {
        mainMenu = GetNodeOrNull<MainMenu>("UI/MainMenu");
        _levelTransition = GetNodeOrNull<LevelTransition>("UI/LevelTransition");
        bossSelect = GetNodeOrNull<BossSelect>("UI/BossSelect");

        mainMenu.StartGameRequested += StartGame;
        mainMenu.LeaderboardRequested += Leaderboard;
        mainMenu.DeckEditorRequested += DeckEditor;
        bossSelect.BossSelected += OnBossSelected;

        bossSelect.Hide();
        mainMenu.Show();

        // Må implementeres i Telt ved spillerunde slutt -> en knapp for retur til main menu
        //SceneManager.Instance.MainMenuLoaded += OnMainMenuReturnPressed;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);
    }

    // START GAME
    public void StartGame(bool newGame)
    {
        GD.Print("Starting game. New game: " + newGame);

        mainMenu?.Hide();
        bossSelect?.Show();
        if (newGame)
        {
            WorldStateManager.Instance.GameEnded = false;

            WorldStateManager.Instance.WorldStateReset();

            Globals.InitializeStartingCards(); // or set deck from player edited deck 


            //GD.Print("Starting new Telt Game");
            // ikke implementert enda
            // _gameTimer.StartTimer();
        }
        else
        {
            GD.Print("Loading saved game");
            // _gameTimer.ContinueTimer();
        }
    }

    // LEADERBOARD
    public void Leaderboard()
    {
    }

    // DECK EDITOR
    public void DeckEditor() { }

    // TUTORIAL
    public void StartTutorialScene() { }

    public void OnBossSelected(string bossName)
    {
        _levelTransition.ScenePath = GameScenePath;
        _levelTransition.TriggerTransition();
    }

}
