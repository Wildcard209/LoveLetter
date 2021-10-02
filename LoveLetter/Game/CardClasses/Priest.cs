namespace LoveLetter.Game.CardClasses
{
    internal class Priest : CardBlank
    {
        /// <summary>
        /// Fully constructs the Priest class with all three values set.
        /// </summary>
        public Priest()
        {
            CardType = CardTypes.Priest;
            CardValue = 2;
            CardDescription = "Lets you see another players card";
        }
    }
}
