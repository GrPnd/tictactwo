namespace DAL;

public static class FileHelper
{
    public static readonly string BasePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
                                             + Path.DirectorySeparatorChar
                                             + "RiderProjects" + Path.DirectorySeparatorChar
                                             + "tictactwo" + Path.DirectorySeparatorChar;

    public const string ConfigExtension = ".config.json";
    public const string GameExtension = ".game.json";
}