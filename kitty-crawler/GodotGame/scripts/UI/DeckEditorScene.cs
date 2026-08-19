using Godot;
using System.Collections.Generic;
using System.Linq;
using KittyCrawler.TELT;

namespace KittyCrawler;

public partial class DeckEditorScene : Control
{
    [Export] private GridContainer _deckGrid;
    [Export] private GridContainer _inventoryGrid;
    [Export] private Label _deckCountLabel;
    [Export] private Button _saveDeckButton;
    [Export] private Button _closeButton;
    [Export] private PackedScene _cardButtonVisualScene;
    [Export] private Control _previewContainer;
    [Export] private PackedScene _cardVisualScene;
    [Export] private Label _feedbackLabel;

    // Settes av scenen som åpner deck editoren
    public static string ReturnScenePath { get; set; } = "";

    private CardVisual _previewCard = null;

    private List<string> _currentDeck = new();
    private List<string> _inventory = new();

    public override void _Ready()
    {
        _saveDeckButton.Pressed += OnSaveDeckPressed;
        _closeButton.Pressed += OnClosePressed;
        _feedbackLabel.Visible = false;

        LoadDeckAndInventory();
        RefreshUI();
    }

    private void LoadDeckAndInventory()
    {
        _currentDeck = new List<string>(PlayerData.SavedDeck);

        // Tell opp kopier i deck, vis resterende fra OwnedCards
        var ownedCards = new List<string>(PlayerData.OwnedCards);
        var deckCopy = new List<string>(_currentDeck);

        _inventory = new List<string>();
        foreach (var path in ownedCards)
        {
            int idx = deckCopy.IndexOf(path);
            if (idx >= 0)
                deckCopy.RemoveAt(idx); // "brukt" én kopi mot decket
            else
                _inventory.Add(path);   // resterende havner i inventory
        }
    }

    private void RefreshUI()
    {
        foreach (Node child in _deckGrid.GetChildren())
            child.QueueFree();
        foreach (Node child in _inventoryGrid.GetChildren())
            child.QueueFree();

        foreach (var cardPath in _currentDeck)
            AddCardEntry(_deckGrid, cardPath, true);

        foreach (var cardPath in _inventory)
            AddCardEntry(_inventoryGrid, cardPath, false);

        _deckCountLabel.Text = $"{_currentDeck.Count}/25";
    }

    private void AddCardEntry(GridContainer grid, string cardPath, bool isInDeck)
    {
        var cardData = GD.Load<CardData>(cardPath);
        if (cardData == null) return;

        var button = _cardButtonVisualScene.Instantiate<CardButtonVisual>();
        button.CustomMinimumSize = new Vector2(128, 40);
        grid.AddChild(button);
        button.Setup(cardData);

        // Hover preview
        button.MouseEntered += () => ShowPreview(cardData);
        button.MouseExited  += () => HidePreview();

        if (isInDeck)
            button.Pressed += () => MoveToInventory(cardPath);
        else
            button.Pressed += () => MoveToDeck(cardPath);
    }

    private void ShowPreview(CardData cardData)
    {
        HidePreview();
        _previewCard = _cardVisualScene.Instantiate<CardVisual>();
        _previewContainer.AddChild(_previewCard);
        _previewCard.Setup(cardData);
        _previewCard.SetStatic();
    }

    private void HidePreview()
    {
        if (_previewCard != null)
        {
            _previewCard.QueueFree();
            _previewCard = null;
        }
    }

    private void MoveToDeck(string cardPath)
    {
        if (_currentDeck.Count >= 25) return;

        var cardData = GD.Load<CardData>(cardPath);
        if (cardData == null) return;

        int maxCopies = cardData.CardRarity switch
        {
            CardData.Rarity.Common   => 3,
            CardData.Rarity.Uncommon => 2,
            CardData.Rarity.Rare     => 1,
            _ => 1
        };

        int copiesInDeck = _currentDeck.Count(c => c == cardPath);
        if (copiesInDeck >= maxCopies) return;

        _inventory.Remove(cardPath);
        _currentDeck.Add(cardPath);
        RefreshUI();
    }

    private void MoveToInventory(string cardPath)
    {
        int index = _currentDeck.IndexOf(cardPath);
        if (index >= 0)
            _currentDeck.RemoveAt(index); // ← fjern kun én instans
        _inventory.Add(cardPath);
        RefreshUI();
    }

    private void OnSaveDeckPressed()
    {
        if (_currentDeck.Count < 25)
        {
            ShowFeedback("Unable to save deck", Colors.Red);
            return;
        }

        PlayerData.SaveDeck(_currentDeck);
        ShowFeedback("Deck saved!", Colors.Green);
    }

    private async void ShowFeedback(string message, Color color)
    {
        _feedbackLabel.Text = message;
        _feedbackLabel.AddThemeColorOverride("font_color", color);
        _feedbackLabel.Visible = true;
        await ToSignal(GetTree().CreateTimer(2f), "timeout");
        if (IsInstanceValid(_feedbackLabel))
            _feedbackLabel.Visible = false;
    }

    private async void OnClosePressed()
    {
        HidePreview();

        if (string.IsNullOrEmpty(ReturnScenePath))
        {
            GD.PrintErr("[DeckEditor] Ingen ReturnScenePath satt!");
            return;
        }

        await SceneManager.Instance.ChangeSceneAsync(ReturnScenePath, TransitionType.Generic);
    }
}
