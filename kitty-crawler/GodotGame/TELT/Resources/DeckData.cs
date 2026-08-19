using Godot;
using Godot.Collections;

namespace KittyCrawler.TELT;

[GlobalClass]
public partial class DeckData : Resource
{
    [Export] public Array<CardData> Deck { get; set; } = new();
}
