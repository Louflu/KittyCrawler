using Game.Core;
using Godot;
using System;

public partial class MainGame : Node2D
{
    private MainMenu mainMenu;
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
       
        mainMenu.StartGameRequested += StartGame;
        mainMenu.LeaderboardRequested += Leaderboard;
        mainMenu.DeckEditorRequested += DeckEditor;

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

        if (newGame)
        {
            WorldStateManager.Instance.GameEnded = false;

            WorldStateManager.Instance.WorldStateReset();

            Globals.InitializeStartingCards();

            _levelTransition.ScenePath = GameScenePath;
            _levelTransition.TriggerTransition();

            GD.Print("Starting new Telt Game");
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


}
