namespace LoveLetter.Game.CardClasses
{
    internal class King : CardBlank
    {
        /// <summary>
        /// Fully constructs the King class with all three values set.
        /// </summary>
        public King()
        {
            CardType = CardTypes.King;
            CardValue = 6;
            CardDescription = "You trades hands with any other player.";
        }
    }
}
