/// <summary>
/// This is the code for the love letter assinemnt for Object Oriented Programming and Principles (400069_A20_T2)
/// 
/// All code will be using the Microsoft .NET recommended naming guidelines
/// This means Namespace, Type, Interface, Method, Property, Event, Field and Enum value will all use Pascal and Parameter will be using Camel
/// Word Choices will be easily readable and will favor readability over brevity, words will also avoid using nonalphanumeric characters unless under private identifires 
/// Aything privat or part of a class will use an underscore to quicky identyfi it
/// </summary>
namespace LoveLetter
{
    static class Program
    {
        /// <summary>
        /// Starting point for all the code, sets up introduction to the code and pushes the user to start a new game or load a game
        /// </summary>
        private static void Main()
        {
            var game = new Game.GameLogic();
            game.StartProgram();
        }

    }
 
}

