using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LoveLetter.Game
{
    public class GameLogic
    {
        /// <summary>
        /// Opening point to start the program
        /// </summary>
        public void StartProgram()
        {
            DisplayStart();
            StartDirection(UserIntInput(1, 2));
        }

        /// <summary>
        /// Directs the code to either load or start a new game
        /// </summary>
        /// <param name="userInput">What the user picks to do</param>
        private void StartDirection(int userInput)
        {
            switch (userInput)
            {
                case 1:
                    DisplayLoad("Starting new game");
                    NewGame();
                    break;
                case 2:
                    DisplayLoad("Loading all save files");
                    LoadGame();
                    break;
            }
        }

        /// <summary>
        /// Directs the user to what there final direction is, if the want to restart the application or just leave it
        /// </summary>
        /// <param name="userInput">What the user inputs</param>
        private void EndDirection(int userInput)
        {
            switch (userInput)
            {
                case 1:
                    DisplayLoad("Starting application again.");
                    StartProgram();
                    break;
                case 2:
                    DisplayClosingApplication();                    
                    break;
            }
        }

        /// <summary>
        /// Directs the user to there desired outcome to either discard a card or save and exit the game
        /// </summary>
        /// <param name="gameData">Master game data for the game</param>
        /// <param name="userInput">Users direction</param>
        private void InGameQuestion(GameData gameData,int userInput)
        {
            switch (userInput)
            {
                case 1:
                    DiscardCard(gameData, 0);
                    break;
                case 2:
                    DiscardCard(gameData, 1);
                    break;
                case 3:
                    gameData.Save(DisplaySavingGame());
                    DisplayClosingApplication();
                    break;
                case 4:
                    DisplayClosingApplication();
                    break;
            }
        }

        /// <summary>
        /// Discards the user picked card and uses its ability
        /// </summary>
        /// <param name="gameData">Master game data for the game</param>
        /// <param name="index">Index so the code knows which card to discard</param>
        private void DiscardCard(GameData gameData, int index)
        {
            //Holds the name of the card discarded
            string search = gameData.AllPlayers[true][gameData.CurrentPlayerIndex].PlayersHand[index].CardType.ToString();

            //Discards the card
            gameData.DiscardPlayerCard(gameData.CurrentPlayerIndex, index);

            //Finds the ability to call
            switch (search)
            {
                case "Guard":
                    AbilityGuard(gameData);
                    break;
                case "Priest":
                    AbilityPriest(gameData);
                    break;
                case "Baron":
                    AbilityBaron(gameData);
                    break;
                case "Handmaid":
                    DisplayHandmaidDiscard();
                    break;
                case "Prince":
                    AbilityPrince(gameData);
                    break;
                case "King":
                    AbilityKing(gameData);
                    break;
                case "Countess":
                    DisplayCountessDiscard();
                    break;
                case "Princess":
                    AbilityPrincess(gameData);
                    break;
            }
        }

        /// <summary>
        /// Starts a new game up
        /// </summary>
        private void NewGame()
        {
            // Sets up game data as an object and starts a new set of game data
            var gameData = new GameData();
            gameData.StartNewGameData();

            DisplayNewGame(gameData);
            NextRound(gameData);
        }

        /// <summary>
        /// Loads game data if there is any and then lets the user pick which file they want to loadH
        /// </summary>
        private void LoadGame()
        {
            //Gets load data
            var gameData = new GameData();
            DisplayLoadGame(gameData);

            InRound(gameData);
        }

        /// <summary>
        /// Brings players back, clears and makes a new deck and gives all players a card each to start the next round
        /// </summary>
        /// <param name="gameData">Master game data for the game</param>
        private void NextRound(GameData gameData)
        {
            //First checks for a winner if one
            CheckForGameWinner(gameData);

            //Undoes all discarded cards and players
            gameData.BringPlayersBackInGame();
            gameData.ClearPlayersCards();

            // Stars the next round up for play
            gameData.NextRound();
            gameData.CreateNewDeck();
            gameData.RestartPlayerIndex();

            //Organises and hands out starting cards
            DiscardThreeCards(gameData);
            GiveCardsToAllPlayers(gameData);

            gameData.GivePlayerCard(gameData.CurrentPlayerIndex);
            InRound(gameData);
        }

        /// <summary>
        /// All logic that's done in round is done in here
        /// </summary>
        /// <param name="gameData">Master game data for the game</param>
        private void InRound(GameData gameData)
        {
            DisplayInRoundNewPlayer(gameData);

            gameData.AllPlayers[true][gameData.CurrentPlayerIndex].RemoveImmune();

            CheckCountessAbility(gameData);

            //auto saves the game
            gameData.Save();

            DisplayInRoundMain(gameData);
            InGameQuestion(gameData, UserIntInput(1,4));

            CheckIfRoundOver(gameData);
        }

        /// <summary>
        /// Ends game once a player has be found a winner
        /// </summary>
        /// <param name="names">List of names that have won the game</param>
        private void EndGame(List<string> names)
        {
            if (names.Count == 0) return;
            DisplayGameWinners(names);
            EndDirection(UserIntInput(1, 2));
        }

        /// <summary>
        ///Goes to the next player in the game and starts there round
        /// </summary>
        /// <param name="gameData">Master game data for the game</param>
        private void NextPlayer(GameData gameData)
        {
            gameData.NextPlayer();
            gameData.GivePlayerCard(gameData.CurrentPlayerIndex);
            InRound(gameData);
        }

        /// <summary>
        /// Uses an upper and lower bound and ask the user to input a number between these numbers
        /// </summary>
        /// <param name="lowerNumber">Lower number that can be returned</param>
        /// <param name="higherNumber">Higher number that can be returned</param>
        /// <returns>Returns the users input turned into an int</returns>
        private static int UserIntInput(int lowerNumber, int higherNumber)
        {
            //Holds the input just so it can be checked in range
            int output;

            //Repeats the question until the user puts in a valid answer
            while (true)
            {
                // Asks question for what inputs the program is looking for
                Console.Write($"Please input a number between {lowerNumber} and {higherNumber}: ");

                //Checks if the users input is a number
                if (int.TryParse(Console.ReadLine(), out output))
                {
                    //Checks if the users inputs is in the range
                    if (output >= lowerNumber && output <= higherNumber)
                    {
                        break;
                    }
                    Console.WriteLine("Number is out of specified range");
                }
                else
                {
                    Console.WriteLine("Input was not a number.");
                }
            }

            // Returns if all checks pass
            return output;
        }

        /// <summary>
        /// Checks if there are 2 players and if so removes 3 cards
        /// </summary>
        /// <param name="gameData">Master game data for the game</param>
        private static void DiscardThreeCards(GameData gameData)
        {
            if (gameData.AllPlayers[true].Count != 2) return;
            gameData.DiscardCard(0);
            gameData.DiscardCard(0);
            gameData.DiscardCard(0);
        }

        /// <summary>
        /// Gives 1 card to each player
        /// </summary>
        /// <param name="gameData">Master game data for the game</param>
        private static void GiveCardsToAllPlayers(GameData gameData)
        {
            for (var index = 0; index < gameData.AllPlayers[true].Count; index++)
            {
                gameData.GivePlayerCard(index);
            }
        }

        /// <summary>
        /// Checks if there is a winner to the game based on there tokens, if so runs the EndGame method
        /// </summary>
        /// <param name="gameData">Master game data for the game</param>
        private void CheckForGameWinner(GameData gameData)
        {
            //Finds the amount of tokens to win
            var tokenWin = TokensWin(gameData.AllPlayers[true].Count + gameData.AllPlayers[false].Count);
            foreach (var player in gameData.AllPlayers[true])
            {
                var names = new List<string>();
                if (player.PlayerTokens == tokenWin)
                {
                    names.Add(player.PlayersName);
                }

                EndGame(names);
            }
        }

        /// <summary>
        /// Checks if the round is over, if so will find who won the game
        /// </summary>
        /// <param name="gameData">Master game data for the game</param>
        private void CheckIfRoundOver(GameData gameData)
        {
            if(gameData.AllPlayers[true].Count == 1 || gameData.CardDecks[true].Count == 0)
            {
                CheckForRoundWinner(gameData);
            }
            NextPlayer(gameData);
        }

        /// <summary>
        /// Finds and displays all winners of a round and starts the next round
        /// </summary>
        /// <param name="gameData"></param>
        private void CheckForRoundWinner(GameData gameData)
        {
            var highestCard = FindHighestCard(gameData);
            var names = new List<string>();
            for (var index = 0; index < gameData.AllPlayers[true].Count; index++)
            {
                var player = gameData.AllPlayers[true][index];
                if (player.PlayersHand[0].CardValue != highestCard) continue;
                
                player.AddToken();
                names.Add(player.PlayersName);
            }

            DisplayRoundWinners(names);

            NextRound(gameData);
        }

        /// <summary>
        /// Finds the highest card value to give out tokens to
        /// </summary>
        /// <param name="gameData">Master game data for the game</param>
        /// <returns></returns>
        private static int FindHighestCard(GameData gameData)
        {
            return gameData.AllPlayers[true].Select(player => player.PlayersHand[0].CardValue).Prepend(0).Max();
        }

        /// <summary>
        /// Finds the amount of tokens is needed to be a winner at the game
        /// </summary>
        /// <param name="players">Amount of players in the game</param>
        /// <returns></returns>
        private static int TokensWin(int players)
        {
            return players switch
            {
                2 => 7,
                3 => 5,
                4 => 4,
                _ => 0,
            };
        }

        /// <summary>
        /// Checks if the user has both the countess card and the king/prince card. If they do then the countess card is removed
        /// </summary>
        /// <param name="gameData">Master game data for the game</param>
        private void CheckCountessAbility(GameData gameData)
        {
            if (gameData.AllPlayers[true][gameData.CurrentPlayerIndex].PlayersHand[0].CardType.ToString() !=
                "Countess" &&
                gameData.AllPlayers[true][gameData.CurrentPlayerIndex].PlayersHand[1].CardType.ToString() !=
                "Countess") return;
            
            if (gameData.AllPlayers[true][gameData.CurrentPlayerIndex].PlayersHand[0].CardType.ToString() == "King" ||
                gameData.AllPlayers[true][gameData.CurrentPlayerIndex].PlayersHand[0].CardType.ToString() == "Prince")
            {
                DisplayCountessAbility(gameData, 1);
                gameData.DiscardPlayerCard(gameData.CurrentPlayerIndex, 1);
                NextPlayer(gameData);
            }

            if (gameData.AllPlayers[true][gameData.CurrentPlayerIndex].PlayersHand[1].CardType.ToString() != "King" &&
                gameData.AllPlayers[true][gameData.CurrentPlayerIndex].PlayersHand[1].CardType.ToString() !=
                "Prince") return;
            
            DisplayCountessAbility(gameData, 0);
            gameData.DiscardPlayerCard(gameData.CurrentPlayerIndex, 0);
            NextPlayer(gameData);
        }

        /// <summary>
        /// Uses the guard ability
        /// </summary>
        /// <param name="gameData">Master game data for the game</param>
        private void AbilityGuard(GameData gameData)
        {
            DisplayGuardAbility();

            //Displays and asks user for play
            DisplayListPlayers(gameData, false);
            var playerIndex = PickPlayer(gameData, false);

            //Displays and asks user for card
            DisplayCards();
            var card = PickedCard(UserIntInput(1, 7));

            if (IsImmune(gameData, playerIndex)) return;
            if (gameData.AllPlayers[true][playerIndex].PlayersHand[0].CardType.ToString() == card)
            {
                DisplayGuardRight(gameData, playerIndex);
                gameData.DiscardPlayer(playerIndex);
            }
            else
            {
                DisplayGuardWrong(gameData,playerIndex);
            }
        }

        /// <summary>
        /// Asks which player they want to see the cards of and shows it to them if they are not immune
        /// </summary>
        /// <param name="gameData">Master gam data for the game</param>
        private void AbilityPriest(GameData gameData)
        {

            DisplayPriestAbility();

            DisplayListPlayers(gameData, false);
            var playerIndex = PickPlayer(gameData, false);

            if (IsImmune(gameData,playerIndex) == false)
            {
                DisplayPriestShowCards(gameData, playerIndex);
            }
        }

        /// <summary>
        /// Asks current player to pick another player to compare hands with, one with lover hand is removed from the game
        /// </summary>
        /// <param name="gameData">Master game data for the game</param>
        private void AbilityBaron(GameData gameData)
        {
            DisplayBaronAbility();

            DisplayListPlayers(gameData, false);

            var playerIndex = PickPlayer(gameData, false);
            
            if (IsImmune(gameData, playerIndex)) return;
            if (gameData.AllPlayers[true][gameData.CurrentPlayerIndex].PlayersHand[0].CardValue > gameData.AllPlayers[true][playerIndex].PlayersHand[0].CardValue)
            {
                DisplayBaronWin(gameData, playerIndex);
                gameData.DiscardPlayer(playerIndex);
            }
            else if (gameData.AllPlayers[true][gameData.CurrentPlayerIndex].PlayersHand[0].CardValue < gameData.AllPlayers[true][playerIndex].PlayersHand[0].CardValue)
            {
                DisplayBaronLose(gameData, playerIndex);
                gameData.DiscardPlayer(gameData.CurrentPlayerIndex);
            }
            else
            {
                DisplayBaronTie(gameData, playerIndex);
            }
        }

        /// <summary>
        /// Takes any player and discards there card and replaces it, if the card discarded was the princess then the player will be removed
        /// </summary>
        /// <param name="gameData">Master game data for the game</param>
        private void AbilityPrince(GameData gameData)
        {
            DisplayPrinceAbility();

            DisplayListPlayers(gameData, true);

            var playerIndex = PickPlayer(gameData, true);
            if (IsImmune(gameData, playerIndex)) return;
            if (gameData.AllPlayers[true][playerIndex].PlayersHand[0].CardType.ToString() != "Princess")
            {
                //Stores name of card to be used in display
                var cardNameHold = gameData.AllPlayers[true][playerIndex].PlayersHand[0].CardType.ToString();

                //Discards and gets the player a new card
                gameData.DiscardPlayerCard(playerIndex, 0);
                gameData.GivePlayerCard(playerIndex);

                //To find what display is needed to been shown
                if (gameData.CurrentPlayerIndex == playerIndex)
                {
                    DisplayPrinceDiscardSelf(gameData, cardNameHold, playerIndex);
                }
                else
                {
                    DisplayPrinceDiscardOther(gameData, cardNameHold, playerIndex);
                }
            }
            else
            {
                //To find what display is needed to been shown
                if (gameData.CurrentPlayerIndex == playerIndex)
                {
                    DisplayPrinceRemoveSelf(gameData, playerIndex);
                }
                else
                {
                    DisplayPrinceRemoveOther(gameData, playerIndex);
                }
                gameData.DiscardPlayer(playerIndex);
            }
        }

        /// <summary>
        /// Asks the user which player they want to swap players with and will swap hands if possible
        /// </summary>
        /// <param name="gameData">Master game data for the game</param>
        private void AbilityKing(GameData gameData)
        {
            DisplayKingAbility();

            DisplayListPlayers(gameData, false);
            var playerIndex = PickPlayer(gameData, false);

            if (gameData.AllPlayers[true][playerIndex].IsImmune) return;
            DisplayKingCardSwap(gameData, playerIndex);
            gameData.SwapPlayersHands(gameData.CurrentPlayerIndex, playerIndex);
        }

        /// <summary>
        /// Uses the ability of the princess card, removing player from the game
        /// </summary>
        /// <param name="gameData">Master game data for the game</param>
        private void AbilityPrincess(GameData gameData)
        {
            DisplayPrincessAbility();
            gameData.DiscardPlayer(gameData.CurrentPlayerIndex);
        }

        /// <summary>
        /// Checks if the player picked is immune to card ability
        /// </summary>
        /// <param name="gameData">Master game data for the game</param>
        /// <param name="playerIndex">Index of player to check</param>
        /// <returns></returns>
        private static bool IsImmune(GameData gameData, int playerIndex)
        {
            if(gameData.AllPlayers[true][playerIndex].IsImmune)
            {
                DisplayImmunePlayerPicked(gameData, playerIndex);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Takes in the users input for what player they pick and returns the index for where that player is
        /// </summary>
        /// <param name="gameData">Master game data for the game</param>
        /// <param name="self">Should the code take the current user as an input</param>
        /// <returns></returns>
        private static int PickPlayer(GameData gameData, bool self)
        {
            var choiceIndex = self ? new int[gameData.AllPlayers[true].Count] : new int[gameData.AllPlayers[true].Count - 1];
            
            var count = 0;
            for (var index = 0; index < gameData.AllPlayers[true].Count; index++)
            {
                if (index == gameData.CurrentPlayerIndex && !self) continue;
                choiceIndex[count] = index;
                count += 1;
            }

            return choiceIndex[(UserIntInput(1, choiceIndex.GetLength(0))) - 1];
        }

        /// <summary>
        /// Takes a users input and returns a sting for what card the user picked
        /// </summary>
        /// <param name="userInput">The users input</param>
        /// <returns></returns>
        private static string PickedCard(int userInput)
        {
            return userInput switch
            {
                1 => "Priest",
                2 => "Baron",
                3 => "Handmaid",
                4 => "Prince",
                5 => "King",
                6 => "Countess",
                7 => "Princess",
                _ => "Out",
            };
        }

        /// <summary>
        /// Displays the loading in text
        /// </summary>
        /// <param name="text">Text for loading or starting</param>
        private static void DisplayLoad(string text)
        {
            DisplayLogo();
            Console.WriteLine($"{text} \n" +
                "Push any key to continue");
            Console.ReadKey();
        }

        /// <summary>
        /// Displays the logo
        /// </summary>
        private static void DisplayLogo()
        {
            Console.Clear();
            Console.WriteLine(
                " ______________________________________________________ \n" +
                "|  _                      _          _   _             |\n" +
                "| | |                    | |        | | | |            |\n" +
                "| | |     _____   _____  | |     ___| |_| |_ ___ _ __  |\n" +
                "| | |    / _ \\ \\ / / _ \\ | |    / _ \\ __| __/ _ \\ '__| |\n" +
                "| | |___| (_) \\ V /  __/ | |___|  __/ |_| ||  __/ |    |\n" +
                "| |______\\___/ \\_/ \\___| |______\\___|\\__|\\__\\___|_|    |\n" +
                "|______________________________________________________|\n\n");
        }

        /// <summary>
        /// Displays the new game text and displays new game options with logo
        /// </summary>
        private static void DisplayStart()
        {
            DisplayLogo();
            Console.WriteLine(
                "Welcome To Love Letter! A 2-4 player game full of deceit, sacrifice, betrayal and most importantly love.\n\n" +
                "1. To start a new game\n" +
                "2. To load a save\n"
            );
        }

        /// <summary>
        /// Displays a question if the user wants to see the rules, and also displays the rules if they do
        /// </summary>
        private void DisplayRulesQuestion()
        {
            DisplayLogo();
            Console.WriteLine(
                "Starting a new game, would you like to read the rules if you are new to the game?\n\n" +
                "1. Yes\n" +
                "2. No\n");
            if (UserIntInput(1, 2) == 1)
            {
                DisplayRules();
            }
        }

        /// <summary>
        /// Displays a question for how many players will be playing and add these players to game
        /// </summary>
        /// <param name="gameData">Master game data for the game</param>
        private static void DisplayPlayersQuestion(GameData gameData)
        {
            DisplayLogo();
            Console.WriteLine("How many players will be playing in this game ? Between 2 - 4 players can play in game.");
            gameData.CreatePlayers(UserIntInput(2, 4));
        }

        /// <summary>
        /// Displays all information present in a creating a new game
        /// </summary>
        /// <param name="gameData">Master game data for the game</param>
        private void DisplayNewGame(GameData gameData)
        {
            DisplayRulesQuestion();
            DisplayPlayersQuestion(gameData);
            DisplayLogo();
            Console.WriteLine(
                "All players have been given a card each, player order will be done by order of creation, when a new round starts the player oder will highest player with tokens go first and one with least will go last\n\n" +
                "Push any button to get into game.");
            Console.ReadKey();
        }

        /// <summary>
        /// Displays the loading scrip for the game
        /// </summary>
        /// <param name="gameData">Master game data for the game</param>
        private void DisplayLoadGame(GameData gameData)
        {
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.xml");
            DisplayLoadFail(files.GetLength(0));
            DisplayAllFiles(files, gameData);
            DisplayLoad("Game Loaded!");
        }

        /// <summary>
        /// Displays an error if there are no files saved, returns user to the start of the program
        /// </summary>
        /// <param name="filesCount">Amount of valid files</param>
        private void DisplayLoadFail(int filesCount)
        {
            if (filesCount != 0) return;
            DisplayLogo();
            Console.WriteLine(
                "No saves have been found, restarting application.\n" + 
                "Push any button to continue.");
            Console.ReadKey();
            StartProgram();
        }

        /// <summary>
        /// Asks user for the name they want to name the save file
        /// </summary>
        /// <returns>The name the users picks</returns>
        private static string DisplaySavingGame()
        {
            DisplayLogo();
            Console.WriteLine("Saving game, what name do you want to give the save file?: ");
            return Console.ReadLine();
        }

        /// <summary>
        /// Displays closing text and closes application
        /// </summary>
        private static void DisplayClosingApplication()
        {
            DisplayLogo();
            Console.WriteLine("Closing program.\n"+
            "Push any button to leave.");
            Console.ReadKey();
            Environment.Exit(1);
        }

        /// <summary>
        /// Displays all files and lets the user pick the one they want to load
        /// </summary>
        /// <param name="files">All valid files</param>
        /// <param name="gameData">Master game data for game</param>
        private static void DisplayAllFiles(string[] files, GameData gameData)
        {
            DisplayLogo();
            for (var index = 0; index < files.GetLength(0); index++)
            {
                Console.WriteLine((index + 1) + ". " + files[index]);
            }

            Console.WriteLine("Select a file to load");
            gameData.Load(files[(UserIntInput(1, files.GetLength(0))) - 1]);
        }

        /// <summary>
        /// Displays the change to a different play, and informs the player if they are no longer immune
        /// </summary>
        /// <param name="gameData">Master game data for the game</param>
        private static void DisplayInRoundNewPlayer(GameData gameData)
        {
            DisplayLogo();
            if (gameData.AllPlayers[true][gameData.CurrentPlayerIndex].IsImmune)
            {
                Console.WriteLine("You are no longer immune to card ability's.\n");
            }

            Console.WriteLine($"It is currently {gameData.AllPlayers[true][gameData.CurrentPlayerIndex].PlayersName} turn to play, please hand over controls to them. Please push any key to continue.");
            Console.ReadKey();
        }

        /// <summary>
        /// Displays all relevant information that the user will need to know in round
        /// </summary>
        /// <param name="gameData">Master game data for the game</param>
        private void DisplayInRoundMain(GameData gameData)
        {
            DisplayLogo();
            Console.WriteLine($"It is your turn to play {gameData.AllPlayers[true][gameData.CurrentPlayerIndex].PlayersName} " +
                $"in round {gameData.CurrentRound}, there are {gameData.CardDecks[true].Count} cards left in deck, " +
                $"you currently have {gameData.AllPlayers[true][gameData.CurrentPlayerIndex].PlayerTokens} tokens, you started with" +
                $" {gameData.AllPlayers[true][gameData.CurrentPlayerIndex].PlayersHand[0].CardType} and pick up the " +
                $"{gameData.AllPlayers[true][gameData.CurrentPlayerIndex].PlayersHand[1].CardType}, pick a card to discard.\n");
            DisplayPlayersInGame(gameData);
            DisplayDiscardedCards(gameData);
            DisplayImmunePlayers(gameData);
            DisplayInRoundQuestion(gameData);
        }

        /// <summary>
        /// Displays question for player for what they want to do
        /// </summary>
        /// <param name="gameData">Master game data for the game</param>
        private static void DisplayInRoundQuestion(GameData gameData)
        {
            Console.WriteLine(
                "What would you like to do?\n\n" +
                $"1. Discard {gameData.AllPlayers[true][gameData.CurrentPlayerIndex].PlayersHand[0].CardType} {gameData.AllPlayers[true][gameData.CurrentPlayerIndex].PlayersHand[0].CardValue} - description - {gameData.AllPlayers[true][gameData.CurrentPlayerIndex].PlayersHand[0].CardDescription}\n" +
                $"2. Discard {gameData.AllPlayers[true][gameData.CurrentPlayerIndex].PlayersHand[1].CardType} {gameData.AllPlayers[true][gameData.CurrentPlayerIndex].PlayersHand[1].CardValue} - description - {gameData.AllPlayers[true][gameData.CurrentPlayerIndex].PlayersHand[1].CardDescription}\n" +
                "3. Save and Quit.\n"+
                "4. Just Quit\n");
        }

        /// <summary>
        /// Displays all players that are in game
        /// </summary>
        /// <param name="gameData">Master game data for the game</param>
        private static void DisplayPlayersInGame(GameData gameData)
        {
            Console.Write("These players are still in game: ");
            foreach (var players in gameData.AllPlayers[true])
            {
                Console.Write(players.PlayersName + " ");
            }
            Console.WriteLine("\n");
        }

        /// <summary>
        /// Displays all cards that have been discarded
        /// </summary>
        /// <param name="gameData">Master game data for the game</param>
        private static void DisplayDiscardedCards(GameData gameData)
        {
            Console.Write("Cards that have been discarded: ");
            foreach (var cards in gameData.CardDecks[false])
            {
                Console.Write(cards.CardType.ToString() + " ");
            }
            Console.WriteLine("\n");
        }

        /// <summary>
        /// Displays all players that are Immune to card ability's 
        /// </summary>
        /// <param name="gameData">Master game data for the game</param>
        private static void DisplayImmunePlayers(GameData gameData)
        {
            foreach (var players in gameData.AllPlayers[true].Where(players => players.IsImmune))
            {
                Console.WriteLine("{0} is immune to all card affects", players.PlayersName);
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Tells the user the player they pick is immune to all ability's and that nothing will happen
        /// </summary>
        /// <param name="gameData">Master game data for the game</param>
        /// <param name="playerIndex">The player index of the immune player picked</param>
        private static void DisplayImmunePlayerPicked(GameData gameData, int playerIndex)
        {
            DisplayLogo();
            Console.WriteLine($"The player {gameData.AllPlayers[true][playerIndex].PlayersName} is immune to all ability's. So nothing will happen. Push any key to continue.");
            Console.ReadKey();
        }

        /// <summary>
        /// Displays to the player that they discarded the Guard card and what will happen
        /// </summary>
        private static void DisplayGuardAbility()
        {
            DisplayLogo();
            Console.WriteLine("You discarded the Guard card, please pick a play and what card you think that player has. Push any key to continue.");
            Console.ReadKey();
        }

        /// <summary>
        /// Displays if the player is right with the guard ability
        /// </summary>
        /// <param name="gameData">Master game data for the game</param>
        /// <param name="playerIndex">Player checked</param>
        private static void DisplayGuardRight(GameData gameData, int playerIndex)
        {
            DisplayLogo();
            Console.WriteLine($"You where correct, {gameData.AllPlayers[true][playerIndex].PlayersName} did have the card {gameData.AllPlayers[true][playerIndex].PlayersHand[0].CardType}. They will now be removed form this round! Push any key to continue.");
            Console.ReadKey();
        }

        /// <summary>
        /// Displays if the player is wrong with the guard ability
        /// </summary>
        /// <param name="gameData">Master game data for the game</param>
        /// <param name="playerIndex">Player checked</param>
        private static void DisplayGuardWrong(GameData gameData, int playerIndex)
        {
            DisplayLogo();
            Console.WriteLine($"You where wrong, {gameData.AllPlayers[true][playerIndex].PlayersName} not have that card. Nothing will happen. Push any key to continue.");
            Console.ReadKey();
        }

        /// <summary>
        /// Displays what the priest ability dose
        /// </summary>
        private static void DisplayPriestAbility()
        {
            DisplayLogo();
            Console.WriteLine("You discarded the priest, pick a player to see there cards:\n");
        }

        /// <summary>
        /// Displays a others players card to the current player
        /// </summary>
        /// <param name="gameData">Master game data for the game</param>
        /// <param name="playerIndex">Players card being shown</param>
        private static void DisplayPriestShowCards(GameData gameData, int playerIndex)
        {
            DisplayLogo();
            Console.WriteLine($"Player {gameData.AllPlayers[true][playerIndex].PlayersName} has the card {gameData.AllPlayers[true][playerIndex].PlayersHand[0].CardType} in there hand. Push any button to continue.");
            Console.ReadKey();
        }

        /// <summary>
        /// Displays Baron ability
        /// </summary>
        private static void DisplayBaronAbility()
        {
            DisplayLogo();
            Console.WriteLine("You discarded the baron, pick a player to compare hands with:");
            Console.ReadKey();
        }

        /// <summary>
        /// Displays if you win the baron check
        /// </summary>
        /// <param name="gameData">Master game data for the game</param>
        /// <param name="playerIndex">Index of the other player being compared to</param>
        private static void DisplayBaronWin(GameData gameData, int playerIndex)
        {
            DisplayLogo();
            Console.WriteLine("Your card {0} {1} was higher than players {2} card {3} {4}, {2} will be removed from the game! Push any button to continue.", gameData.AllPlayers[true][gameData.CurrentPlayerIndex].PlayersHand[0].CardType.ToString(),
                gameData.AllPlayers[true][gameData.CurrentPlayerIndex].PlayersHand[0].CardType,
                gameData.AllPlayers[true][playerIndex].PlayersName,
                gameData.AllPlayers[true][playerIndex].PlayersHand[0].CardType.ToString(),
                gameData.AllPlayers[true][playerIndex].PlayersHand[0].CardValue);
            Console.ReadKey();
        }

        /// <summary>
        /// Displays if you lose the baron check
        /// </summary>
        /// <param name="gameData">Master game data for the game</param>
        /// <param name="playerIndex">Index of the other player bing compared to</param>
        private static void DisplayBaronLose(GameData gameData, int playerIndex)
        {
            DisplayLogo();
            Console.WriteLine("Your card {0} {1} was lower than players {2} card {3} {4}, you will be removed from the game. Push any button to continue.", gameData.AllPlayers[true][gameData.CurrentPlayerIndex].PlayersHand[0].CardType.ToString(),
                gameData.AllPlayers[true][gameData.CurrentPlayerIndex].PlayersHand[0].CardValue,
                gameData.AllPlayers[true][playerIndex].PlayersName,
                gameData.AllPlayers[true][playerIndex].PlayersHand[0].CardType.ToString(),
                gameData.AllPlayers[true][playerIndex].PlayersHand[0].CardValue);
            Console.ReadKey();
        }

        /// <summary>
        /// Displays the baron check tie
        /// </summary>
        /// <param name="gameData">Master game data for the game</param>
        /// <param name="playerIndex">Index of other player being compared to</param>
        private static void DisplayBaronTie(GameData gameData, int playerIndex)
        {
            Console.Clear();
            DisplayLogo();
            Console.WriteLine("Your card {0} {1} was equal to players {2} card {3} {4}, nothing will happen. Push any button to continue.", gameData.AllPlayers[true][gameData.CurrentPlayerIndex].PlayersHand[0].CardType.ToString(),
                gameData.AllPlayers[true][gameData.CurrentPlayerIndex].PlayersHand[0].CardValue,
                gameData.AllPlayers[true][playerIndex].PlayersName,
                gameData.AllPlayers[true][playerIndex].PlayersHand[0].CardType.ToString(),
                gameData.AllPlayers[true][playerIndex].PlayersHand[0].CardValue);
            Console.ReadKey();
        }

        /// <summary>
        /// Displays message for discarding the handmaid
        /// </summary>
        private static void DisplayHandmaidDiscard()
        {
            DisplayLogo();
            Console.WriteLine("You discarded the Handmaid card, you will now be immune to card ability's until its your turn again. Push any button to continue.");
            Console.ReadKey();
        }

        /// <summary>
        /// Displays princes ability
        /// </summary>
        private static void DisplayPrinceAbility()
        {
            DisplayLogo();
            Console.WriteLine("You discarded the Prince, pick a player, including yourself, to discard there current hand and pick up another card.");
            Console.ReadKey();
        }

        /// <summary>
        /// Shows information of the card the player discarded and gained
        /// </summary>
        /// <param name="gameData">Master game data for the game</param>
        /// <param name="card">Name of the card discarded</param>
        /// <param name="playerIndex">Player being affected by the card affect</param>
        private static void DisplayPrinceDiscardSelf(GameData gameData, string card, int playerIndex)
        {
            DisplayLogo();
            Console.WriteLine($"You discarded the {card}, you now have picked up the {gameData.AllPlayers[true][playerIndex].PlayersHand[0].CardType}");
            Console.ReadKey();
        }

        /// <summary>
        /// Shows information of the player that had a card removed and what card was removed
        /// </summary>
        /// <param name="gameData">Master game data for the game</param>
        /// <param name="card">Name of the card discarded</param>
        /// <param name="playerIndex">Player being affected by the card affect</param>
        private static void DisplayPrinceDiscardOther(GameData gameData, string card,int playerIndex)
        {
            DisplayLogo();
            Console.WriteLine($"You discarded {gameData.AllPlayers[true][playerIndex].PlayersName}'s card which was {card} and now they have picked up a new card.");
            Console.ReadKey();
        }

        /// <summary>
        /// Displays information if the user removes themself by discarding the princess
        /// </summary>
        /// <param name="gameData">Master game data for the game</param>
        /// <param name="playerIndex">Player being affected by the card affect</param>
        private void DisplayPrinceRemoveSelf(GameData gameData,int playerIndex)
        {
            DisplayLogo();
            Console.WriteLine($"You discarded the {gameData.AllPlayers[true][playerIndex].PlayersHand[0].CardType}... and now because of that you will now be removed from the game!");
            Console.ReadKey();
        }

        /// <summary>
        /// Displays information if the user makes another player discard the princess
        /// </summary>
        /// <param name="gameData">Master game data for the game</param>
        /// <param name="playerIndex">Player being affected by the card affect</param>
        private static void DisplayPrinceRemoveOther(GameData gameData, int playerIndex)
        {
            DisplayLogo();
            Console.WriteLine(
                $"You discarded players {gameData.AllPlayers[true][playerIndex].PlayersName} card and they had a {gameData.AllPlayers[true][playerIndex].PlayersHand[0].CardType} so they will be removed from the game!");
            Console.ReadKey();
        }

        /// <summary>
        /// Displays the kings Ability
        /// </summary>
        private static void DisplayKingAbility()
        {
            DisplayLogo();
            Console.WriteLine("Chose a player to swap hands with. Push any key to continue");
            Console.ReadKey();
        }

        /// <summary>
        /// Displays who you swapped cards with, what card you are getting rid of and what card you are getting
        /// </summary>
        /// <param name="gameData">Master game data for the game</param>
        /// <param name="playerIndex">The play that is swapping hands with the current player</param>
        private static void DisplayKingCardSwap(GameData gameData, int playerIndex)
        {
            DisplayLogo();
            Console.WriteLine($"You and {gameData.AllPlayers[true][playerIndex].PlayersName} will swap cards, you have given then the {gameData.AllPlayers[true][gameData.CurrentPlayerIndex].PlayersHand[0].CardType} card, and you have now gained the {gameData.AllPlayers[true][playerIndex].PlayersHand[0].CardType} card.\n"+
                "Push any key to continue.\n");
            Console.ReadKey();
        }

        /// <summary>
        /// Displays activation of the Countess
        /// </summary>
        /// <param name="gameData">Master game data for the game</param>
        /// <param name="cardIndex">Card index for reference card</param>
        private static void DisplayCountessAbility(GameData gameData, int cardIndex)
        {
            DisplayLogo();
            Console.WriteLine($"You have the Countess in and you also have the {gameData.AllPlayers[true][gameData.CurrentPlayerIndex].PlayersHand[cardIndex].CardType}, you are forced to remove the Countess and let the next player play.Push any key to continue.");
            Console.ReadKey();
        }

        /// <summary>
        /// Displays message for discarding the Countess
        /// </summary>
        private static void DisplayCountessDiscard()
        {
            DisplayLogo();
            Console.WriteLine("The Countess ability will not be used and the card will be discarded normally. Push any key to continue.");
            Console.ReadKey();
        }

        /// <summary>
        /// Displays the princess ability
        /// </summary>
        private void DisplayPrincessAbility()
        {
            DisplayLogo();
            Console.WriteLine("Congratulations, you played yourself, you weren't suppose to remove this card but you did, now you will be removed from this round!!");
            Console.ReadKey();
        }

        /// <summary>
        /// Displays a list of players to chose from
        /// </summary>
        /// <param name="gameData">Master game data for the game</param>
        /// <param name="self">Asks if the current player should be displayed</param>
        private static void DisplayListPlayers(GameData gameData, bool self)
        {
            DisplayLogo();
            Console.WriteLine("Please pick a player to inflict your card ability on\n");

            var count = 0;
            for (var index = 0; index < gameData.AllPlayers[true].Count; index++)
            {
                if (index == gameData.CurrentPlayerIndex && self != true) continue;
                count += 1;
                Console.WriteLine($"{count}. {gameData.AllPlayers[true][index].PlayersName}");
            }
        }

        /// <summary>
        /// Displays a list of cards to pick for the card ability
        /// </summary>
        private static void DisplayCards()
        {
            DisplayLogo();
            Console.WriteLine(
                "What value do you think that player has?\n\n"+
                "1. Priest\n" +
                "2. Baron\n"+
                "3. Handmaid\n" +
                "4. Prince\n"+
                "5. King\n" +
                "6. Countess\n" +
                "7. Princess\n"
            );
        }

        /// <summary>
        /// Displays winners names and who is getting tokens
        /// </summary>
        /// <param name="names">List of round winner names</param>
        private static void DisplayRoundWinners(IEnumerable<string> names)
        {
            DisplayLogo();
            Console.WriteLine("The round has ended, tokens will be given out to all players with the highest card value.\n");
            foreach(var name in names)
            {
                Console.WriteLine($"Congratulations {name} you gained a token!\n");
            }
            Console.WriteLine("Push any key to continue.");
            Console.ReadKey();
        }

        /// <summary>
        /// Displays winners for the game and ask if the user wants to restart the application
        /// </summary>
        /// <param name="names">List of winners</param>
        private static void DisplayGameWinners(IEnumerable<string> names)
        {
            DisplayLogo();
            Console.WriteLine("The game has ended, thank you all for playing.\n");
            foreach (var name in names)
            {
                Console.WriteLine($"Congratulations {name} you won the game!\n");
            }
            Console.WriteLine(
                "Would you like to restart the application to play a new game?\n"+
                "1. Yes\n"+
                "2. No\n");
        }

        /// <summary>
        /// Displays all the rules in the game
        /// </summary>
        private static void DisplayRules()
        {
            DisplayLogo();
            Console.WriteLine(
            "Object of the Game\n\n" +
            "In the wake of the arrest of Queen Marianna for high treason, none was more\n" +
            "heartbroken than her daughter, Princess Annette. Suitors throughout the City-\n" +
            "State of Tempest sought to ease Annette’s sorrow by courting her, seeking to\n" +
            "bring some joy into her life.\n\n" +
            "You are one of these suitors, trying to get your love letter to the Princess.\n" +
            "Unfortunately, she has locked herself in the palace, so you must rely on \n" +
            "intermediaries to carry your message\n\n"+
            "During the game, you hold one secret card in your hand. This is who currently\n" +
            "carries your message of love for the Princess. Make sure that the person closest\n" +
            "to the Princess holds your love letter at the end of the day, so it reaches her first!\n\n" +
            "Components\n\n" +
            "A set of 16 cards consisting of:\n" +
            "1 Princess, 1 Countess, 1 King, 2 Prince, 2 Handmaid, 2 Baron, 2 Priest, 5 Guards\n" +
            "A set of Tokens of Affection\n\n" +
            "Setup\n\n" +
            "Shuffle the set of 16 cards face-down so no player can see then and then set aside face down.\n" +
            "If playing a 2 player game, take 3 more cards from the deck and discard them face up so\n" +
            "players can examine during game.\n" +
            "Each player draws 1 card from the deck. This is the players hand and is meant to be kept secret\n" +
            "from the other players.\n" +
            "The Player that won the last round of game gets to go first (if tied, the youngest player will\n" +
            "go first)\n\n" +
            "How to Play\n\n" +
            "Love Letter is played in a series of rounds. Each round represents one day. At the end of\n"+
            "each round, one player’s letter reaches Princess Annette, and she reads it. When she reads\n"+
            "enough letters from one suitor, she becomes enamored and grants that suitor permission to \n"+
            "court her. That player wins the Princess’ heart and the game.\n\n" +
            "Turns\n\n"+
            "On your turn, take one card from the top of the deck and add it to your hand. Then chose\n"+
            "one card to discard face up. Apply the effect of the card when discarded. Even if its bad\n"+
            "for you. Some cards have affects once in hand such as the Countess. After discarding a card\n"+
            "the next players turn starts.\n\n"+
            "Out of the Round\n\n"+
            "If a card affect knocks a player out of the round, the player discards all cards in hand face\n"+
            "up(No affects will be applied) and will not play anymore until the start of the next round\n\n"+
            "Honesty\n\n"+
            "This is a console app game, which means you can pass to a different player and peak at there\n"+
            "cards, please play honestly and do not try and cheat in any way.\n\n"+
            "Choosing a Player\n\n"+
            "If you discard a card with an effect that requires you to choose a player that cannot be \n"+
            "chosen due to another card effect (Handmaid), your card is discarded without effect.\n\n"+
            "End of a Round\n\n"+
            "A round ends if the deck is empty at the end of a player’s turn. The royal residence\n" +
            "closes for the evening, the person closest to the Princess delivers the love letter, and\n"+
            "Princess Annette retires to her chambers to read it. All players still in the round reveal\n"+
            "their hands. The player with the highest number in their hand wins the round. In case of a\n"+
            "tie, players add the numbers on the cards in their discard pile. The highest total wins. \n"+
            "If there is still a tie, then all tied players are considered to have won the round.\n"+
            "A round also ends if all players but one are out of the round, in which case the remaining\n"+
            "player wins.\n"+
            "After a round ends, the winner (or winners, if there was a tie at the end of the round)\n"+
            "receives a Token of Affection. Shuffle all 16 cards together, and play a new round\n"+
            "following all of the setup rules above. The winner of the previous round goes first,\n"+
            "because the Princess speaks kindly of him or her at breakfast. If there was more than\n"+
            "one winner from the previous round as a result of a tie, then whichever of the tied\n"+
            "players was most recently on a date goes first\n\n"+
            "How to Win\n\n" +
            "The winner is decide on the winning number of Tokens in hand. The amount of tokens to win\n"+
            "is based on player amount\n"+
            "2 Players = 7 tokens\n"+
            "3 Players = 5 tokens\n"+
            "4 Players = 4 tokens\n\n"+
            "Card Affects\n\n"+
            "8 Princess:\n"+
            "If the Princess is discarded while in your hand, you are knocked out of the round. If the\n"+
            "Princess was discarded by a card effect, any remaining effects of that card do not apply\n"+
            "(you do not draw a card from the Prince, for example)\n\n"+
            "7 Countess:\n"+
            "If you ever have the Countess and either the King or Prince in your hand, you must discard\n"+
            "the Countess. You do not have to reveal the other card in your hand. Of course, you can\n"+
            "also discard the Countess even if you do not have a royal family member in your hand.\n\n"+
            "6 King:\n"+
            "When the King is discarded, swap hands with another player of choice, the player can not\n"+
            "chose a player who it out of the round.\n\n"+
            "5 Prince:\n"+
            "When the Prince is discarded, chose any player(Including yourself) and discard there hand\n"+
            "and take a new card from the deck. No cards affect will be used when this card is used\n"+
            "(Unless if the Princess, that cards effect will still take place)\n\n"+
            "4 Handmaid:\n"+
            "When the Handmaid is discarded, you will be immune to the effects from other players until\n"+
            "until its your turn next. If all players other than the player whose turn it is are protected\n"+
            "by the Handmaid, the player must choose him or herself for a card’s effects, if possible.\n\n"+
            "3 Baron:\n"+
            "When the Baron is discarded, choose another player still in round and compare hands, the\n"+
            "player with the low value card is knocked out of the round. If tie, nothing happens\n\n"+
            "2 Priest:\n"+
            "When Priest is discarded, look at another players hand. Keep this information secret\n\n"+
            "1 Guard:\n"+
            "When the Guard is discarded, chose a player and a card value (Other than 1) and ask if\n"+
            "the player dose have that value card, that player is knocked out of the round, if the\n"+
            "if the player doesn't have that card or all players cannot be chosen in the round\n"+
            "(Handmaid) then just discard the card with no affect.\n\n"+
            "Push any key to continue to game."
        );
            Console.ReadKey();
        }
    }
}
