using Godot;
using System.Collections.Generic;

namespace KittyCrawler.TELT;

public partial class PlayerData : Node
{
    public string PlayerName { get; set; } = "Player";
    public int TotalDamageReceived { get; set; } = 0;

    private List<CardData> _deck = new();
    private List<CardData> _hand = new();
    private List<CardData> _discardPile = new();

    // ── Deck ──────────────────────────────────────────────────────────
    public void SetDeck(List<CardData> deck)
    {
        _deck = new List<CardData>(deck);
    }

    public void ShuffleDeck()
    {
        var rng = new RandomNumberGenerator();
        rng.Randomize();
        for (int i = _deck.Count - 1; i > 0; i--)
        {
            int j = (int)(rng.Randi() % (uint)(i + 1));
            (_deck[i], _deck[j]) = (_deck[j], _deck[i]);
        }
    }

    // ── Trekking ──────────────────────────────────────────────────────
    public CardData LastDrawnCard { get; private set; } = null;

    private List<CardData> _newlyDrawnCards = new();

    public List<CardData> NewlyDrawnCards => new(_newlyDrawnCards);

    public bool TryDrawCard()
    {
        if (_deck.Count == 0) return false;

        var card = _deck[0];
        _deck.RemoveAt(0);
        _hand.Add(card);
        LastDrawnCard = card;
        _newlyDrawnCards.Add(card);
        return true;
    }

    public void ClearLastDrawnCard()
    {
        LastDrawnCard = null;
        _newlyDrawnCards.Clear();
    }

    public void DrawCards(int amount)
    {
        for (int i = 0; i < amount; i++)
            TryDrawCard();
    }

    // ── Hånd ──────────────────────────────────────────────────────────
    public List<CardData> GetHand() => new(_hand);
    public int HandCount => _hand.Count;
    public bool HasCardsInHand => _hand.Count > 0;

    public bool TryPlayCard(CardData card)
    {
        return _hand.Remove(card);
    }

    // ── Discard ───────────────────────────────────────────────────────
    public void DiscardCard(CardData card)
    {
        _hand.Remove(card);
        _discardPile.Add(card);
    }

    public void DiscardHand()
    {
        _discardPile.AddRange(_hand);
        _hand.Clear();
    }

    // ── Opprydding mellom matcher ─────────────────────────────────────
    public void CollectBattlemapCards(List<CardData> cardsFromBattlemap)
    {
        foreach (var card in cardsFromBattlemap)
        {
            card.ResetCurrentDamage();
            card.IsPoisoned = false;
            card.IsEnraged = false;
        }

        _discardPile.AddRange(cardsFromBattlemap);
    }

    // ── Damage ────────────────────────────────────────────────────────
    public void ReceiveDamage(int amount)
    {
        TotalDamageReceived += amount;
    }

    // ── Debug ─────────────────────────────────────────────────────────
    public void PrintState()
    {
        GD.Print(
            $"[{PlayerName}] Deck: {_deck.Count} | Hånd: {_hand.Count} | Discard: {_discardPile.Count} | Damage: {TotalDamageReceived}");
    }

    public int DeckCount => _deck.Count;
    public List<CardData> GetDiscardPile() => new(_discardPile);


    // ══════════════════════════════════════════════════════════════════
    //  Progresjon — delegerer all lagring til WorldStateManager
    // ══════════════════════════════════════════════════════════════════
    private static WorldStateManager World => WorldStateManager.Instance;

    public static List<string> OwnedCards =>
        World != null ? new List<string>(World.CardsOwned) : new();

    public static List<string> SavedDeck =>
        World != null ? new List<string>(World.Deck) : new();

    public static int TotalDamageDealt => World?.Score ?? 0;

    // ── Kortsamling ───────────────────────────────────────────────────
    public static void AddCardToInventory(string cardPath)
    {
        if (World == null) { GD.PrintErr("[PlayerData] WorldStateManager mangler."); return; }
        World.OnCardAdded(cardPath);
    }

    public static bool HasCardInInventory(string cardPath) =>
        World?.CardsOwned.Contains(cardPath) ?? false;

    // ── Deck ──────────────────────────────────────────────────────────
    public static void SaveDeck(List<string> deck)
    {
        if (World == null) { GD.PrintErr("[PlayerData] WorldStateManager mangler."); return; }
        World.Deck = new Godot.Collections.Array<string>(deck);
        World.DeckHasChanged = true;
        World.SaveGame();
    }

    // ── NPC-progresjon ────────────────────────────────────────────────
    public static bool HasDefeatedNpc(string npcId) =>
        World?.BossesWon.Contains(npcId) ?? false;

    public static void DefeatNpc(string npcId, int damageDealt)
    {
        if (World == null) return;
        if (World.BossesWon.Contains(npcId)) return;

        World.OnNpcDefeated(npcId);
        World.OnScoreUpdated(damageDealt);
    }

    public static void AddDamageDealt(int amount) => World?.OnScoreUpdated(amount);

    // ── Belønningskort ────────────────────────────────────────────────
    public static bool HasReceivedCard(string npcId) =>
        World?.ReceivedCards.Contains(npcId) ?? false;

    public static void GiveRewardCard(string npcId, string cardPath)
    {
        if (World == null) return;
        if (World.ReceivedCards.Contains(npcId)) return;

        World.ReceivedCards.Add(npcId);
        if (!string.IsNullOrEmpty(cardPath) && !World.CardsOwned.Contains(cardPath))
            World.CardsOwned.Add(cardPath);

        World.SaveGame();
    }

    // ── Reset ─────────────────────────────────────────────────────────
    public static void ResetSessionDamage()
    {
        if (World != null) World.Score = 0;
    }

    public static void ResetForNewGame()
    {
        World?.WorldStateReset();
        World?.SaveGame();
    }
}
