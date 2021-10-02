using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace LoveLetter.Game
{
    internal class GameData
    {
        /// <summary>
        /// Number to represent who is currently playing
        /// </summary>
        public int CurrentPlayerIndex { get; private set; }

        /// <summary>
        /// Number to show which round the game is in
        /// </summary>
        public int CurrentRound { get; private set; }

        /// <summary>
        /// A dictionary that represents all the cards in 2 decks, true means the card is still in play and false means its been discarded
        /// </summary>
        public Dictionary<bool, List<CardBlank>> CardDecks { get;} = new Dictionary<bool, List<CardBlank>>();

        /// <summary>
        /// A dictionary that represents all players in the game, true means the player is still in play for the round, false means they have been removed from the round
        /// </summary>
        public Dictionary<bool, List<PlayerBlank>> AllPlayers { get;} = new Dictionary<bool, List<PlayerBlank>>();

        private readonly Random _random = new Random();

        /// <summary>
        /// Fills the all dictionary's with both the true and false lists
        /// </summary>
        public void StartNewGameData()
        {
            //fills CardDecks
            CardDecks.Add(true, new List<CardBlank>());
            CardDecks.Add(false, new List<CardBlank>());

            //fills AllPlayers
            AllPlayers.Add(true, new List<PlayerBlank>());
            AllPlayers.Add(false, new List<PlayerBlank>());
        }

        /// <summary>
        /// Sets the CurrentPlayerIndex to the next player, if at the end of the index it will loop back to the first player
        /// </summary>
        public void NextPlayer()
        {
            if(CurrentPlayerIndex == AllPlayers[true].Count - 1)
            {
                CurrentPlayerIndex = 0;
            }
            else
            {
                CurrentPlayerIndex += 1;
            }
        }
        
        /// <summary>
        /// Restarts the playerIndex
        /// </summary>
        public void RestartPlayerIndex()
        {
            CurrentPlayerIndex = 0;
        }

        /// <summary>
        /// Sets the current round counter to +1 of itself
        /// </summary>
        public void NextRound()
        {
            CurrentRound += 1;
        }

        /// <summary>
        /// Creates all players that will be used in the game
        /// </summary>
        /// <param name="playersAmount">The amount of players playing</param>
        public void CreatePlayers(int playersAmount)
        {
            for (var index = 0; index < playersAmount; index++)
            {
                //asks for players name so it can create the new objects
                Console.Write($"Player {index + 1} what is your name? note this name will be used to refer to you, please do not use the same name as another player: ");
                AllPlayers[true].Add(new PlayerBlank(Console.ReadLine()));
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Creates a brand new deck
        /// </summary>
        public void CreateNewDeck()
        {
            ClearDeck();

            //Small problem with this code where it creates 1 extra guard and 1 less princess, so the cardDistribution had to be changed to quick fix it
            int[] cardDistribution = { 4, 2, 2, 2, 2, 1, 1, 2 };
            for (var card = 0; card < 16; card++)
            {
                var pickedCard = _random.Next(0, 16 - card);
                int[] locateCard = { 0, 0 };

                foreach (var missingCard in cardDistribution)
                {
                    locateCard[1] += missingCard;

                    if (pickedCard <= locateCard[1])
                    {
                        CardDecks[true].Add(FindCard(locateCard[0]));
                        cardDistribution[locateCard[0]] -= 1;
                        break;
                    }

                    locateCard[0] += 1;
                }
            }
            // Removes the first card from the deck to follow the rules
            CardDecks[true].RemoveAt(0);
        }

        /// <summary>
        /// Takes a number and returns a card
        /// </summary>
        /// <param name="cardNumber">The number of card you want to find</param>
        /// <returns>The card that is asked for</returns>
        private static CardBlank FindCard(int cardNumber)
        {
            CardBlank hold = cardNumber switch
            {
                0 => new CardClasses.Guard(),
                1 => new CardClasses.Priest(),
                2 => new CardClasses.Baron(),
                3 => new CardClasses.Handmaid(),
                4 => new CardClasses.Prince(),
                5 => new CardClasses.King(),
                6 => new CardClasses.Countess(),
                7 => new CardClasses.Princess(),
                _ => null
            };
            return hold;
        }

        /// <summary>
        /// Gives a player a card from the active deck
        /// </summary>
        /// <param name="index">Player index that will get a card</param>
        public void GivePlayerCard(int index)
        {
            var hold = CardDecks[true][0];
            CardDecks[true].Remove(hold);
            AllPlayers[true][index].GiveCard(hold);
        }

        /// <summary>
        /// Swaps two players hand, only really used for the King ability
        /// </summary>
        /// <param name="indexUser">User who called it index</param>
        /// <param name="indexOther">User affected index</param>
        public void SwapPlayersHands(int indexUser, int indexOther)
        {
            var holdOne = AllPlayers[true][indexUser].RemoveCard(0);
            var holdTwo = AllPlayers[true][indexOther].RemoveCard(0);
            AllPlayers[true][indexOther].GiveCard(holdOne);
            AllPlayers[true][indexUser].GiveCard(holdTwo);
        }

        /// <summary>
        /// Discard a card from active deck to inactive deck
        /// </summary>
        /// <param name="index">card index for what card should be discarded</param>
        public void DiscardCard(int index)
        {
            var holdCard = CardDecks[true][index];
            CardDecks[true].Remove(holdCard);
            CardDecks[false].Add(holdCard);
        }

        /// <summary>
        /// Removes one card in the players hand and puts it to the discard deck
        /// </summary>
        /// <param name="indexPlayer">Player index</param>
        /// <param name="indexCard">Card index in players hand</param>
        public void DiscardPlayerCard(int indexPlayer, int indexCard)
        {
            CardBlank holdCard = AllPlayers[true][indexPlayer].RemoveCard(indexCard);
            CardDecks[false].Add(holdCard);
        }

        /// <summary>
        /// Discards player with all there cards, as well as makes sure the removed player dose not mess with playerIndex
        /// </summary>
        /// <param name="index">Index of player that will be removed</param>
        public void DiscardPlayer(int index)
        {
            //Checks if the player removed is the current player, and sorts out the index so the code doesn't skip or crash
            if (index == CurrentPlayerIndex)
            {
                CurrentPlayerIndex -= 1;
                if (CurrentPlayerIndex == -1)
                {
                    CurrentPlayerIndex = AllPlayers[true].Count - 2;
                }
            }

            PlayerBlank hold = AllPlayers[true][index];

            //Discard all cards first before discarding player
            for (int i = 0; i < AllPlayers[true][index].PlayersHand.Count; i++)
            {
                DiscardPlayerCard(index, 0);
            }

            AllPlayers[true].Remove(hold);
            AllPlayers[false].Add(hold);
        }

        /// <summary>
        /// Brings all players back into active play
        /// </summary>
        public void BringPlayersBackInGame()
        {
            var holdPlayers = new List<PlayerBlank>();
            holdPlayers.AddRange(AllPlayers[true]);
            holdPlayers.AddRange(AllPlayers[false]);

            //Orders all players from highest tokens amount to lower
            holdPlayers = holdPlayers.OrderBy(i => i.PlayerTokens).ToList();

            ClearPlayers();
            AllPlayers[true] = holdPlayers;
        }

        /// <summary>
        /// Clears all the cards in the deck
        /// </summary>
        private void ClearDeck()
        {
            CardDecks[true].Clear();
            CardDecks[false].Clear();
        }

        /// <summary>
        /// Clears all players in game
        /// </summary>
        private void ClearPlayers()
        {
            AllPlayers[true].Clear();
            AllPlayers[false].Clear();
        }

        /// <summary>
        /// Clears all cards from all player
        /// </summary>
        public void ClearPlayersCards()
        {
            for (var index = 0; index < AllPlayers[true].Count; index++)
            {
                AllPlayers[true][index].ClearHand();
            }
        }

        /// <summary>
        /// Completely removes all information in the class
        /// </summary>
        private void ClearAll()
        {
            CurrentPlayerIndex = 0;
            CurrentRound = 0;
            CardDecks.Clear();
            AllPlayers.Clear();
        }

        /// <summary>
        /// Saves the game under the filename auto save
        /// </summary>
        public void Save()
        {
            ToXml("auto save");
        }

        /// <summary>
        /// Saves the game to what the user picks
        /// </summary>
        /// <param name="name">Name of file</param>
        public void Save(string name)
        {
            ToXml(name);
        }

        /// <summary>
        /// Takes the filename and saves everything in the GameData class as is
        /// </summary>
        /// <param name="fileName">Name of the file it will be saved as</param>
        private void ToXml(string fileName)
        {
            var doc = new XDocument(new XElement("GameData",
                                           new XElement("CurrentPlayerIndex", CurrentPlayerIndex.ToString()),
                                           new XElement("CurrentRound", CurrentRound.ToString()),
                                           new XElement("CardDecks",
                                           from key in CardDecks
                                           select new XElement(key.Key.ToString(),
                                           from classes in key.Value
                                           select new XElement("CardType", classes.CardType.ToString()))),
                                           new XElement("Players",
                                           from key in AllPlayers
                                           select new XElement(key.Key.ToString(),
                                           from players in key.Value
                                           select new XElement("Player",
                                           new XElement("PlayerName", players.PlayersName),
                                           new XElement("PlayerTokens", players.PlayerTokens.ToString()),
                                           new XElement("IsImmune", players.IsImmune.ToString()),
                                           new XElement("PlayersHand",
                                           from card in players.PlayersHand
                                           select new XElement("CardType", card.CardType.ToString())))))));
            doc.Save(Directory.GetCurrentDirectory() + "//" + fileName + ".xml");
        }

        /// <summary>
        /// Reads an xml file and loads its data
        /// </summary>
        /// <param name="fileName">Filename that will be loaded</param>
        public void Load(string fileName)
        {
            ClearAll();
            StartNewGameData();
            var doc = XElement.Load(fileName);

            CurrentPlayerIndex = int.Parse(doc.Element("CurrentPlayerIndex")?.Value ?? string.Empty);
            CurrentRound = int.Parse(doc.Element("CurrentRound")?.Value ?? string.Empty);

            LoadDeck(doc.Descendants("CardDecks").Elements("True").Elements("CardID").AsEnumerable(), true);
            LoadDeck(doc.Descendants("CardDecks").Elements("False").Elements("CardID").AsEnumerable(), false);
            LoadPlayers(doc.Descendants("Players").Elements("True").Elements("Player").AsEnumerable(), true);
            LoadPlayers(doc.Descendants("Players").Elements("False").Elements("Player").AsEnumerable(), false);
        }

        /// <summary>
        /// Loads all the cards into one deck
        /// </summary>
        /// <param name="cards">The cards that will be loaded in</param>
        /// <param name="active">The active/inactive deck they will be put in</param>
        private void LoadDeck(IEnumerable<XElement> cards, bool active)
        {
            foreach (var card in cards)
            {
                CardDecks[active].Add(FindCard((int)Enum.Parse(typeof(CardBlank.CardTypes), card.Value)));
            }
        }

        /// <summary>
        /// Loads all the players into the game with cards
        /// </summary>
        /// <param name="players">Loads all the players that are given</param>
        /// <param name="active">Adds the players to either the active/inactive</param>
        private void LoadPlayers(IEnumerable<XElement> players, bool active)
        {
            foreach (var player in players)
            {
                //Gets all values held in each individual player
                var playerName = player.Element("PlayerName")?.Value;
                var playerTokens = int.Parse(player.Element("PlayerTokens")?.Value ?? string.Empty);
                var isImmune = bool.Parse(player.Element("IsImmune")?.Value ?? string.Empty);
                var playersHand = player.Descendants("PlayersHand").Elements("CardType");

                //Creates a new player setting all the preset data
                AllPlayers[active].Add(new PlayerBlank(playerName, playerTokens, isImmune));

                //Adds all the cards back to players hand
                foreach (var card in playersHand)
                {
                    AllPlayers[active][AllPlayers[active].Count - 1].GiveCard(FindCard((int)Enum.Parse(typeof(CardBlank.CardTypes), card.Value)));
                }
            }
        }
    }
}
