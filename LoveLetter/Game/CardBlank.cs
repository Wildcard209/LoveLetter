namespace LoveLetter.Game
{
    public class CardBlank
    {
        /// <summary>
        /// Creates a blank constructor for the CardBlank, this will not be used and is only to help the child class construct
        /// </summary>
        protected CardBlank() { }

        // Creates blank variables that are get; protected set; so the child class can define the blank
        public int CardValue { get; protected set; }
        public string CardDescription { get; protected set; }
        public CardTypes CardType { get; protected set; }

        /// <summary>
        /// Declares the enum cardTypes, this is used in the logic a lot so it needs to be public
        /// </summary>
        public enum CardTypes
        {
            Guard,
            Priest,
            Baron,
            Handmaid,
            Prince,
            King,
            Countess,
            Princess
        }
    }
}
