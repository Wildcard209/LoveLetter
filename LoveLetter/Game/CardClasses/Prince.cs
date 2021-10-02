namespace LoveLetter.Game.CardClasses
{
    internal class Prince : CardBlank
    {
        /// <summary>
        /// Fully constructs the Prince class with all three values set.
        /// </summary>
        public Prince()
        {
            CardType = CardTypes.Prince;
            CardValue = 5;
            CardDescription = "You can choose any player (including yourself) to discard their hand and draw a new one. If the discarded card is the Princess, the discarding player is eliminated.";
        }
    }
}
