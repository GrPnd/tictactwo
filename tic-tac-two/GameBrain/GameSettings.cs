
using System.ComponentModel.DataAnnotations;

namespace GameBrain;

public class GameSettings
{
    [MaxLength(128)]
    public string ConfigName { get; set; } = default!;
    public int BoardSizeWidth { get; set; } = 3;
    public int BoardSizeHeight { get; set; } = 3;
    public int WinCondition { get; set; } = 3;
    public int NumberOfPieces { get; set; } = 4;
    public int MoveGridAfterNMoves { get; set; } = 100; // 100 disabled
    public int MovePieceAfterNMoves { get; set; } = 100; // 100 disabled
    public bool UsesGrid { get; set; }
    public int GridSizeWidth { get; set; }
    public int GridSizeHeight { get; set; }


    public override string ToString() =>
        $"Board {BoardSizeWidth}x{BoardSizeHeight}, " +
        $"to win: {WinCondition}, " +
        $"number of pieces: {NumberOfPieces}, " +
        $"can move grid after {MoveGridAfterNMoves} moves, " +
        $"can move piece after {MovePieceAfterNMoves} moves, " +
        $"uses grid: {UsesGrid}, " +
        $"grid {GridSizeWidth}x{GridSizeHeight}";
}