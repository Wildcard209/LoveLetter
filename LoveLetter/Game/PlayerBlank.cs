using System.Collections.Generic;

namespace LoveLetter.Game
{
    internal class PlayerBlank
    {
        // Creates the players name string as a public readonly
        public readonly string PlayersName;

        // Creates the variables player tokens, is immune and players hand to get; private set;
        public int PlayerTokens { get; private set; }
        public bool IsImmune { get; private set; }
        public List<CardBlank> PlayersHand { get; } = new List<CardBlank>();

        /// <summary>
        /// Constructor for a new player
        /// </summary>
        /// <param name="playerName">Takes in the players name, nothing else is needed for a new player</param>
        public PlayerBlank(string playerName)
        {
            PlayersName = playerName;
        }

        /// <summary>
        /// Constructor to load in a player from save.
        /// </summary>
        /// <param name="playerName">Takes players name from save</param>
        /// <param name="playerTokens">Takes players tokens from save</param>
        /// <param name="isImmune">Takes if player is Immune from save</param>
        public PlayerBlank(string playerName, int playerTokens, bool isImmune)
        {
            PlayersName = playerName;
            PlayerTokens = playerTokens;
            IsImmune = isImmune;
        }

        /// <summary>
        /// Adds 1 token to the player
        /// </summary>
        public void AddToken()
        {
            PlayerTokens += 1;
        }
        
        /// <summary>
        /// Adds 1 card to the players hand, a CardBlank is needed to be given to this method so the card can be given
        /// </summary>
        /// <param name="card"></param>
        public void GiveCard(CardBlank card)
        {
            PlayersHand.Add(card);
        }

        /// <summary>
        /// Takes a players card at a certain index
        /// </summary>
        /// <param name="index">A number representing the index of which card will be removed</param>
        /// <returns></returns>
        public CardBlank RemoveCard(int index)
        {
            var hold = PlayersHand[index];
            AddImmune(hold);
            PlayersHand.Remove(hold);
            return hold;
        }

        /// <summary>
        /// Destroys all cards in players hand, only use to end game and destroy all cards
        /// </summary>
        public void ClearHand()
        {
            PlayersHand.Clear();
        }

        /// <summary>
        /// Takes a card and see if its a Handmaid, if it is it sets IsImmune to ture
        /// </summary>
        /// <param name="card">CardBlank to check if its a handmaid</param>
        private void AddImmune(CardBlank card)
        {
            if(card.CardType == CardBlank.CardTypes.Handmaid)
            {
                IsImmune = true;
            }
        }

        /// <summary>
        /// Removes immunity from the player
        /// </summary>
        public void RemoveImmune()
        {
            IsImmune = false;
        }
    }
}
