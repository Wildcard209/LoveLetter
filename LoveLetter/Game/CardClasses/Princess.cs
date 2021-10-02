namespace LoveLetter.Game.CardClasses
{
    internal class Princess : CardBlank
    {
        /// <summary>
        /// Fully constructs the Prince class with all three values set.
        /// </summary>
        public Princess()
        {
            CardType = CardTypes.Princess;
            CardValue = 8;
            CardDescription = "If you discard this card for any reason, you are eliminated from the round.";
        }
    }
}
