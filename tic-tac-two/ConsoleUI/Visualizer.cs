using GameBrain;

namespace ConsoleUI;

public static class Visualizer
{
    public static void DrawBoard(TicTacTwoBrain gameInstance)
    {
        int gridEndX = gameInstance.GameState.GridStartX + gameInstance.GameState.GameSettings.GridSizeWidth;
        int gridEndY = gameInstance.GameState.GridStartY + gameInstance.GameState.GameSettings.GridSizeHeight;


        // Print X coordinates on top
        Console.Write("   "); // Space for Y coordinates on the left
        for (var x = 0; x < gameInstance.DimXBoard; x++)
        {
            Console.Write(" " + x + "  "); // Print X coordinate with some spacing
        }
        Console.WriteLine(); // Move to the next line after printing the X coordinates


        for (var y = 0; y < gameInstance.DimYBoard; y++)
        {
            // Print Y coordinate on the left
            Console.Write(" " + y + " "); // Print Y coordinate with a space

            for (var x = 0; x < gameInstance.DimXBoard; x++)
            {
                // Set the background color for the grid area
                if (gameInstance.GameState.GameSettings.UsesGrid)
                {
                    if (x >= gameInstance.GameState.GridStartX && x < gridEndX && y >= gameInstance.GameState.GridStartY && y < gridEndY)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGray; // Set background color to gray for the grid
                    }
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Black; // Reset to black for other areas
                }


                EGamePiece pieceToDraw = gameInstance.GameBoard[x][y];


                if (pieceToDraw == EGamePiece.X)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                if (pieceToDraw == EGamePiece.O)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                Console.Write(" " + DrawGamePiece(pieceToDraw) + " ");
                Console.ResetColor(); // Reset color after drawing the piece

                if (x == gameInstance.DimXBoard - 1) continue; // Don't write the right border
                if (gameInstance.GameState.GameSettings.UsesGrid)
                {
                    if (x >= gameInstance.GameState.GridStartX - 1 && x < gridEndX && y >= gameInstance.GameState.GridStartY && y < gridEndY)
                    {
                        Console.ForegroundColor = ConsoleColor.Red; // Set background color to red for grid borders
                    }
                }
                Console.Write("|");
                Console.ResetColor();
            }

            Console.WriteLine();
            if (y == gameInstance.DimYBoard - 1) continue; // Don't write the bottom border
            Console.Write("   "); // Align the borders under the X coordinates

            for (var x = 0; x < gameInstance.DimXBoard; x++)
            {
                if (gameInstance.GameState.GameSettings.UsesGrid)
                {
                    if (x >= gameInstance.GameState.GridStartX && x < gridEndX && y >= gameInstance.GameState.GridStartY - 1 && y < gridEndY)
                    {
                        Console.ForegroundColor = ConsoleColor.Red; // Set color for horizontal grid border
                    }
                }
                Console.Write("---");

                if (x != gameInstance.DimXBoard - 1)
                {
                    if (gameInstance.GameState.GameSettings.UsesGrid)
                    {
                        if (x >= gameInstance.GameState.GridStartX - 1 && x < gridEndX - 1 && y >= gameInstance.GameState.GridStartY - 1 && y < gridEndY)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                        }
                    }
                    Console.Write("+");
                }
                Console.ResetColor();
            }

            Console.WriteLine();
        }
        Console.ResetColor(); // Reset all console colors at the end
    }


    private static string DrawGamePiece(EGamePiece piece) =>
        piece switch
        {
            EGamePiece.O => "O",
            EGamePiece.X => "X",
            _ => " "
        };
}