using System.Text.Json;
using Domain;
using Microsoft.CSharp.RuntimeBinder;

namespace DAL;

public class GameRepositoryJson : IGameRepository
{

    public string SaveGame(string jsonStateString, string configName)
    {
        var fileName = $"{configName}-{DateTime.Now:yyyy-MM-dd-HH_mm_ss}" + FileHelper.GameExtension;
        var filePath = Path.Combine(FileHelper.BasePath, fileName);

        if (string.IsNullOrEmpty(jsonStateString))
        {
            throw new ArgumentException("Game state cannot be empty or null.", nameof(jsonStateString));
        }

        File.WriteAllText(filePath, jsonStateString);
        return fileName;
    }

    public void UpdateGame(string gameName, string newJsonState)
    {
        var files = Directory.GetFiles(FileHelper.BasePath, $"*{FileHelper.GameExtension}");
        
        var filePath = files.FirstOrDefault(file => Path.GetFileName(file).StartsWith(gameName));
        
        if (filePath == null)
        {
            throw new RuntimeBinderException();
        }

        try
        {
            File.WriteAllText(filePath, newJsonState);
        }
        catch (Exception ex)
        {
            throw new RuntimeBinderException($"An error occurred while updating the game: {ex.Message}");
        }
    }
    
    public string? GetSavedGameStateByName(string gameName)
    {
        var files = Directory.GetFiles(FileHelper.BasePath, $"*{FileHelper.GameExtension}");
        
        var filePath = files.FirstOrDefault(file => Path.GetFileName(file).StartsWith(gameName));

        // If filePath is null, return null; otherwise, read the file
        return filePath != null ? File.ReadAllText(filePath) : null;
    }

    public List<string> GetSavedGamesNames()
    {
        var files = Directory.GetFiles(FileHelper.BasePath, $"*{FileHelper.GameExtension}");
        
        return files.Select(filePath => Path.GetFileName(filePath)).ToList();
    }
}