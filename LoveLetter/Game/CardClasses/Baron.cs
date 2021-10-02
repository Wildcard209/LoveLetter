namespace LoveLetter.Game.CardClasses
{
    internal class Baron : CardBlank
    {
        /// <summary>
        /// Fully constructs the Baron class with all three values set.
        /// </summary>
        public Baron()
        {
            CardType = CardTypes.Baron;
            CardValue = 3;
            CardDescription = "You will choose another player and privately compare hands. The player with the lower-strength hand is eliminated from the round.";
        }
    }
}
