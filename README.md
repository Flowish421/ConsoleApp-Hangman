Description Hangman is a word guessing game where the player needs to guess a hidden word by guessing letters. Each incorrect guess will reduce the number of remaining attempts. If the player guesses the word before running out of attempts, they win!

This implementation makes use of the following:

Spectre.Console for colorful, styled text. LINQ for checking word completion. JSON for saving game history. The game allows you to:

Guess letters. See the progress of the word. Save the game history in a JSON file. Features Styled Console Output: Uses Spectre.Console to make the game more visually appealing with colorized text. Game History: Saves the guesses and remaining attempts in a game_history.json file for later analysis. Input Validation: Ensures the user inputs only a single letter at a time. User-Friendly Navigation: Game is controlled through keyboard input. Object-Oriented Design: The code is structured using object-oriented principles for easy extensibility. Installation To run the game, you will need: Packs needs to be installed.
.NET Core SDK installed on your computer. Spectre.Console NuGet package.
