namespace LoveLetter.Game.CardClasses
{
    internal class Handmaid : CardBlank
    {
        /// <summary>
        /// Fully constructs the Handmaid class with all three values set.
        /// </summary>
        public Handmaid()
        {
            CardType = CardTypes.Handmaid;
            CardValue = 4;
            CardDescription = "You will not be affected by any other player's card until your next turn.";
        }
    }
}
