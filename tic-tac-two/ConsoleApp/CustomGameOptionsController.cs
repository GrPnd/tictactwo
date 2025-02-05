using GameBrain;

namespace ConsoleApp;

public static class CustomGameOptionsController
{
    public static GameSettings ConfigureCustomGame(GameSettings chosenConfig)
    {
        Console.WriteLine("This is a custom game configuration.");

        chosenConfig.ConfigName = "Custom";
        chosenConfig.BoardSizeWidth = GetValidatedIntegerInput("Enter your game board size width", 2, 20);
        chosenConfig.BoardSizeHeight = GetValidatedIntegerInput("Enter your game board size height", 2, 20);
        chosenConfig.NumberOfPieces = GetValidatedIntegerInput("How many pieces would you like to play", 2, 20);
        chosenConfig.MovePieceAfterNMoves = GetValidatedIntegerInput(
            "How many pieces are needed to be placed in order to move pieces?", 1, chosenConfig.NumberOfPieces);
        chosenConfig.UsesGrid = GetValidatedBooleanInput("Would you like to use a grid? (Y/N): ");
        if (chosenConfig.UsesGrid)
        {
            chosenConfig.MoveGridAfterNMoves = GetValidatedIntegerInput(
                "How many pieces are needed to be placed in order to move the grid?", 0, chosenConfig.NumberOfPieces);
            chosenConfig.GridSizeWidth =
                GetValidatedIntegerInput("Enter your grid size width", 1, chosenConfig.BoardSizeWidth);
            chosenConfig.GridSizeHeight =
                GetValidatedIntegerInput("Enter your grid size height", 1, chosenConfig.BoardSizeHeight);
        }

        chosenConfig.WinCondition = GetValidWinCondition("How many straight numbers are needed to win?", chosenConfig);

        return chosenConfig;
    }
    
    
    private static int GetValidatedIntegerInput(string prompt, int minValue, int maxValue)
    {
        while (true)
        {
            Console.WriteLine($"{prompt} ({minValue}-{maxValue}):");
            var input = Console.ReadLine();

            if (int.TryParse(input, out var result) && result >= minValue && result <= maxValue)
            {
                return result;
            }

            Console.WriteLine($"Please enter a valid number between {minValue} and {maxValue}.");
        }
    }

    private static bool GetValidatedBooleanInput(string prompt)
    {
        do
        {
            Console.WriteLine(prompt);
            var input = Console.ReadLine();

            if (input?.ToUpper() == "Y")
            {
                return true;
            }

            if (input?.ToUpper() == "N")
            {
                return false;
            }

            Console.WriteLine("Please enter 'Y' for Yes or 'N' for No.");
            
        } while (true);
    }

    private static int GetValidWinCondition(string prompt, GameSettings chosenConfig)
    {
        var maxBoardValue = chosenConfig.UsesGrid
            ? Math.Min(chosenConfig.GridSizeWidth, chosenConfig.GridSizeHeight)
            : Math.Min(chosenConfig.BoardSizeWidth, chosenConfig.BoardSizeHeight);

        var maxWinCondition = Math.Min(maxBoardValue, chosenConfig.NumberOfPieces);
        const int minValue = 1;

        do
        {
            Console.WriteLine($"{prompt} ({minValue}-{maxWinCondition}):");
            var input = Console.ReadLine();

            if (int.TryParse(input, out var result) && result >= minValue && result <= maxWinCondition)
            {
                return result;
            }

            Console.WriteLine($"Please enter a valid number between {minValue} and {maxWinCondition}.");
        } while (true);
    }

}