using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Configuration : BaseEntity
{
    [MaxLength(128)]
    public string ConfigName { get; set; } = default!;

    public int BoardSizeWidth { get; set; } = 3;
    public int BoardSizeHeight { get; set; } = 3;
    public int WinCondition { get; set; } = 3;
    public int NumberOfPieces { get; set; } = 4;
    public int MoveGridAfterNMoves { get; set; } = 100; // 100 disabled
    public int MovePieceAfterNMoves { get; set; } = 100; // 100 disabled
    public int GridSizeWidth { get; set; }
    public int GridSizeHeight { get; set; }
    public bool UsesGrid { get; set; }

    public ICollection<SaveGame>? SaveGames { get; set; } 
}