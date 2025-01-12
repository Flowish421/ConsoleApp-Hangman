using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Spectre.Console;

// Definierar ett interface för ett spel
public interface IGame
{
    void Play(); // Alla spel måste ha en Play-metod som startar spelet
}

// Hangman-klassen implementerar IGame-gränssnittet
public class Hangman : IGame
{
    private readonly string wordToGuess; // Ordet som ska gissas
    private readonly HashSet<char> guessedLetters; // En uppsättning av gissade bokstäver
    private int remainingAttempts; // Antal återstående försök
    private readonly List<string> guessHistory; // Lista för att spara gissade bokstäver
    private readonly IGameHistoryService gameHistoryService; // Tjänst för att spara spelhistorik

    // Konstruktorn initialiserar spelet med ett ord, antal försök och en historiktjänst
    public Hangman(string word, int attempts, IGameHistoryService historyService)
    {
        wordToGuess = word.ToUpper(); // Omvandlar ordet till stora bokstäver
        guessedLetters = new HashSet<char>(); // Skapar en ny uppsättning för gissade bokstäver
        remainingAttempts = attempts; // Sätter antal försök
        guessHistory = new List<string>(); // Skapar en lista för att spara gissade bokstäver
        this.gameHistoryService = historyService; // Sparar tjänsten för historik
    }

    // Spelmetod som kör själva spelet
    public void Play()
    {
        while (remainingAttempts > 0 && !isWordGuessed()) // Så länge vi har försök och ordet inte är gissat
        {
            displayGameStatus(); // Visar spelets status

            char guessedLetter = getUserGuess(); // Hämtar spelarens gissning

            if (guessedLetter == '\0') continue; // Om gissningen är ogiltig, fortsätt till nästa varv

            processGuess(guessedLetter); // Behandlar gissningen
        }

        endGame(); // Slutar spelet när vi är klara
    }

    // Visar status för spelet (ordet och antal försök)
    private void displayGameStatus()
    {
        Console.Clear(); // Rensar skärmen
        displayHangman(); // Visar aktuell hangman-stadium
        displayWord(); // Visar ordet med gissade bokstäver
        Console.WriteLine($"\nRemaining attempts: {remainingAttempts}"); // Visar antal återstående försök
    }

    // Visar olika hangman-stadier beroende på återstående försök
    private void displayHangman()
    {
        string[] hangmanStages = new string[] // Array med olika bilder för varje hangman-stadium
        {
            @"  _______
 |/      |
 |      
 |      
 |      
 |      
 |______",

            @"  _______
 |/      |
 |      O
 |      
 |      
 |      
 |______",

            @"  _______
 |/      |
 |      O
 |      |
 |      
 |      
 |______",

            @"  _______
 |/      |
 |      O
 |     /|
 |      
 |      
 |______",

            @"  _______
 |/      |
 |      O
 |     /|\
 |      
 |      
 |______",

            @"  _______
 |/      |
 |      O
 |     /|\
 |     / 
 |      
 |______",

            @"  _______
 |/      |
 |      O
 |     /|\
 |     / \
 |      
 |______"
        };

        Console.WriteLine(hangmanStages[6 - remainingAttempts]); // Visar motsvarande hangman-stadium beroende på försök
    }

    // Hämtar användarens gissning och validerar den
    private char getUserGuess()
    {
        Console.Write("Guess a letter: "); // Ber användaren gissa en bokstav
        string input = Console.ReadLine()?.ToUpper(); // Läser in användarens gissning och gör den till stora bokstäver

        // Kontrollera att input är en enda bokstav
        if (string.IsNullOrEmpty(input) || input.Length != 1 || !char.IsLetter(input[0]))
        {
            Console.WriteLine("Invalid input! Please enter a single letter.");
            Console.ReadKey(); // Väntar på att användaren trycker en tangent
            return '\0'; // Ogiltig input, returnerar nulltecken
        }

        char guessedLetter = input[0]; // Hämtar den första bokstaven från input

        // Kontrollera om bokstaven redan har gissats
        if (guessedLetters.Contains(guessedLetter))
        {
            Console.WriteLine($"You have already guessed the letter '{guessedLetter}'. Try again.");
            Console.ReadKey(); // Väntar på användarens tangenttryckning
            return '\0'; // Om bokstaven redan är gissad, returnerar nulltecken
        }

        return guessedLetter; // Returnerar den gissade bokstaven
    }

    // Bearbetar spelarens gissning
    private void processGuess(char guessedLetter)
    {
        guessedLetters.Add(guessedLetter); // Lägger till den gissade bokstaven i uppsättningen

        // Om bokstaven finns i ordet
        if (wordToGuess.Contains(guessedLetter))
        {
            Console.WriteLine($"Good job! The letter '{guessedLetter}' is in the word.");
            AnsiConsole.Markup($"[green]Good job! The letter '{guessedLetter}' is in the word.[/]"); // Grön text i konsolen
        }
        else
        {
            Console.WriteLine($"The letter '{guessedLetter}' is not in the word.");
            remainingAttempts--; // Minskar antalet försök om bokstaven inte finns
        }

        Console.ReadKey(); // Väntar på användarens tangenttryckning
    }

    // Visar ordet med gissade bokstäver, eller understreck för ogissade
    private void displayWord()
    {
        foreach (var letter in wordToGuess)
        {
            // Visa gissade bokstäver, annars visa ett understreck
            if (guessedLetters.Contains(letter))
            {
                AnsiConsole.Markup("[green]" + letter + " [/]"); // Grön färg för gissade bokstäver
            }
            else
            {
                AnsiConsole.Markup("[red]_ [/]"); // Röd färg för ogissade bokstäver
            }
        }
        Console.WriteLine(); // Ny rad efter att ha visat ordet
    }

    // Kollar om hela ordet har gissats korrekt
    private bool isWordGuessed()
    {
        return wordToGuess.All(letter => guessedLetters.Contains(letter)); // Kollar om alla bokstäver är gissade
    }

    // Avslutar spelet och visar resultatet
    private void endGame()
    {
        Console.Clear();
        if (isWordGuessed()) // Om ordet är gissat korrekt
        {
            Console.WriteLine($"Congratulations! You've guessed the word: {wordToGuess}");
        }
        else // Om spelet är slut utan att ordet har gissats
        {
            Console.WriteLine($"Game Over. The word was: {wordToGuess}");
            AnsiConsole.Markup("[bold red]Game Over![/]"); // Röd text för att indikera att spelet är slut
        }

        // Sparar historik efter att spelet avslutats
        gameHistoryService.saveHistory(wordToGuess, guessHistory, remainingAttempts);
    }
}

// Interface för en tjänst som sparar historik
public interface IGameHistoryService
{
    void saveHistory(string wordToGuess, List<string> guessHistory, int remainingAttempts);
}

// Implementering av GameHistoryService för att spara spelets historik till en fil
public class GameHistoryService : IGameHistoryService
{
    public void saveHistory(string wordToGuess, List<string> guessHistory, int remainingAttempts)
    {
        var history = new { Word = wordToGuess, Guesses = guessHistory, AttemptsLeft = remainingAttempts }; // Skapar ett objekt med historik
        string json = JsonSerializer.Serialize(history, new JsonSerializerOptions { WriteIndented = true }); // Serialiserar objektet till JSON
        System.IO.File.WriteAllText("game_history.json", json); // Skriver JSON-historiken till en fil
        Console.WriteLine("Game history saved to game_history.json."); // Bekräftar att historiken har sparats
    }
}

// Huvudprogrammet som startar spelet
public class Program
{
    public static void Main(string[] args)
    {
        AnsiConsole.Markup("[bold yellow]Welcome to the Hangman game![/]\n"); // Välkomstmeddelande
        Console.Write("Enter a word (Hidden from player): "); // Ber spelaren att skriva in ett ord

        string wordToGuess = Console.ReadLine(); // Läser in ordet som ska gissas
        Console.Clear(); // Rensar skärmen

        IGameHistoryService gameHistoryService = new GameHistoryService(); // Skapar en instans av GameHistoryService
        Hangman game = new Hangman(wordToGuess, 6, gameHistoryService); // Skapar ett nytt Hangman-spel
        game.Play(); // Startar spelet

        AnsiConsole.Markup("[bold green]Thanks for playing Hangman![/]\n"); // Tackmeddelande
    }
}
