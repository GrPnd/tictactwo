namespace GameBrain;

public class GameState
{
    public EGamePiece[][] GameBoard { get; set; }
    public EGamePlayers PlayersSetting { get; set; }
    public EGamePlayers PlayerX { get; set; }
    public EGamePlayers PlayerO { get; set; }
    public string? PasswordX { get; set; }
    public string? PasswordO { get; set; }
    public EGamePiece NextMoveBy { get; set; } = EGamePiece.X;
    public EGamePiece LastMoveBy { get; set; } = EGamePiece.O;
    public EGamePiece Winner { get; set; }
    public int RemainingPiecesX { get; set; }
    public int RemainingPiecesO{ get; set; }
    public int GridStartX { get; set; }
    public int GridStartY { get; set; }
    public GameSettings GameSettings { get; set; }

    public GameState(EGamePiece[][] gameBoard, GameSettings gameSettings)
    {
        GameSettings = gameSettings;
        GameBoard = gameBoard;
        RemainingPiecesX = gameSettings.NumberOfPieces;
        RemainingPiecesO = gameSettings.NumberOfPieces;
    }
    
    public override string ToString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }
}