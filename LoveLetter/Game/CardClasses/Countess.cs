namespace LoveLetter.Game.CardClasses
{
    internal class Countess : CardBlank
    {
        /// <summary>
        /// Fully constructs the Countess class with all three values set.
        /// </summary>
        public Countess()
        {
            CardType = CardTypes.Countess;
            CardValue = 7;
            CardDescription = "If you hold both this card and either the King or Prince card, this card must be played immediately. Else nothing happens.";
        }
    }
}
