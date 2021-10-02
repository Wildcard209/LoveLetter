namespace LoveLetter.Game.CardClasses
{
    internal class Guard : CardBlank
    {
        /// <summary>
        /// Fully constructs the Guard class with all three values set.
        /// </summary>
        public Guard()
        {
            CardType = CardTypes.Guard;
            CardValue = 1;
            CardDescription = "pick another player and guess there card type they have in hand(besides Guard), if you are correct the other player is removed from the game, otherwise nothing happens.";
        }
    }
}
