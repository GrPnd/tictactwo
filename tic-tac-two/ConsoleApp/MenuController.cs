using DAL;
using GameBrain;
using MenuSystem;


namespace ConsoleApp;

public class MenuController
{

    private readonly IConfigRepository _configRepository;
    private readonly IGameRepository _gameRepository;
    
    private static readonly AppDbContext? DbContext;

    private static GameSettings _chosenConfig = null!;
    private static GameState _savedGameState = null!;
    private static TicTacTwoBrain _gameInstance = null!;
    


    public MenuController(IConfigRepository configRepository, IGameRepository gameRepository)
    {
        _configRepository = configRepository;
        _gameRepository = gameRepository;
    }

    
    
    public void MainMenu()
    {
        var mainMenu = new Menu(EMenuLevel.Main, "TIC-TAC-TWO",
        [
            new MenuItem()
            {
                Shortcut = "N",
                Title = "New game",
                MenuItemAction = NewGameMenu
            },
            new MenuItem()
            {
                Shortcut = "L",
                Title = "Load game",
                MenuItemAction = LoadGameMenu
            }
        ]);

        mainMenu.Run();
    }
        
    
    private string LoadGameMenu()
    {
        var chosenConfigShortcut = ChooseSavedGameConfiguration();

        if (!int.TryParse(chosenConfigShortcut, out var configNo))
        {
            return chosenConfigShortcut;
        }

        var savedGameName = _gameRepository.GetSavedGamesNames()[configNo];
        
        var savedGameStateJson = _gameRepository.GetSavedGameStateByName(savedGameName);
        
        _chosenConfig = LoadSavedGame.GetSavedGameSetting(savedGameStateJson);
        _savedGameState = LoadSavedGame.LoadGameState(savedGameStateJson, _chosenConfig);

        _gameInstance = new TicTacTwoBrain(_chosenConfig, _savedGameState);
        
        if (_gameInstance.GameState.PlayersSetting == EGamePlayers.PvP)
        {
            Console.WriteLine("Enter either player X or O password: ");
        }
        else if (_gameInstance.GameState.PlayersSetting == EGamePlayers.PvA)
        {
            if (_gameInstance.GameState.PlayerX == EGamePlayers.Player)
            {
                Console.WriteLine("Enter player X password: ");
            }
            else
            {
                Console.WriteLine("Enter O password: ");
            }
        }
        
        var input = Console.ReadLine();
        if (input != _gameInstance.GameState.PasswordX && input != _gameInstance.GameState.PasswordO)
        {
            do
            {
                Console.WriteLine("Incorrect password: " + input);
                Console.WriteLine("Try again: ");
                input = Console.ReadLine();
            } while (input != _gameInstance.GameState.PasswordX && input != _gameInstance.GameState.PasswordO);     
        }
        
        var game = new GameController(_chosenConfig, _gameRepository, _configRepository, _gameInstance);

        return game.MainLoop();
    }

    private string NewGameMenu()
    {
        var chosenConfigShortcut = ChooseConfiguration();

        if (!int.TryParse(chosenConfigShortcut, out var configNo))
        {
            return chosenConfigShortcut;
        }

        _chosenConfig = _configRepository.GetConfigurationByName(
            _configRepository.GetConfigurationNames()[configNo]);

        if (_chosenConfig.ConfigName == "Custom")
        {
            var customConfig = new GameSettings();
            customConfig = CustomGameOptionsController.ConfigureCustomGame(customConfig);
            _chosenConfig = customConfig;
            
            var configurationToAdd = ConfigToGameSettingsConverter.ConvertGStoC(_chosenConfig);
            DbContext?.Configurations.Add(configurationToAdd);
            DbContext?.SaveChanges();
        }
        
        _gameInstance = new TicTacTwoBrain(_chosenConfig);
        
        ChooseGamePlayers(_gameInstance);
        if (_gameInstance.GameState.PlayersSetting == EGamePlayers.PvA)
        {
            ChooseStartingGamePiece(_gameInstance);    
        }

        var consoleFunctions = new ConsoleFunctions(_chosenConfig, _gameInstance, _gameRepository, _configRepository);
        
        if (_chosenConfig.UsesGrid)
        {
            consoleFunctions.GetGridCoordinates();
        }
        
        var game = new GameController(_chosenConfig, _gameRepository, _configRepository, _gameInstance);
        return game.MainLoop();
    }


    private static void ChooseStartingGamePiece(TicTacTwoBrain gameInstance)
    {
        // X always starts
        
        var pieces = new List<MenuItem>();
        
        pieces.Add(new MenuItem()
        {
            Title = "X",
            Shortcut = "1",
            MenuItemAction = () =>
            {
                if (gameInstance.GameState.PlayersSetting == EGamePlayers.PvA)
                {
                    gameInstance.GameState.PlayerX = EGamePlayers.Player;
                    gameInstance.GameState.PlayerO = EGamePlayers.Ai;
                }
                return "X";
            }
        });

        pieces.Add(new MenuItem()
        {
            Title = "O",
            Shortcut = "2",
            MenuItemAction = () =>
            {
                if (gameInstance.GameState.PlayersSetting == EGamePlayers.PvA)
                {
                    gameInstance.GameState.PlayerX = EGamePlayers.Ai;
                    gameInstance.GameState.PlayerO = EGamePlayers.Player;
                }
                return "O";
            }
        });

        var configMenu = new Menu(EMenuLevel.Secondary,
            "TIC-TAC-TWO - choose starting game piece (X starts)",
            pieces,
            isCustomMenu: true
        );

        configMenu.Run();
    }

    private static void ChooseGamePlayers(TicTacTwoBrain gameInstance)
    {
        var playerOptions = new List<MenuItem>();
        
        playerOptions.Add(new MenuItem()
        {
            Title = "Player vs Player",
            Shortcut = "1",
            MenuItemAction = () =>
            {
                gameInstance.GameState.PlayersSetting = EGamePlayers.PvP;
                gameInstance.GameState.PlayerX = EGamePlayers.Player;
                gameInstance.GameState.PlayerO = EGamePlayers.Player;
                return "PvP";
            }
        });

        playerOptions.Add(new MenuItem()
        {
            Title = "Player vs AI",
            Shortcut = "2",
            MenuItemAction = () =>
            {
                gameInstance.GameState.PlayersSetting = EGamePlayers.PvA;
                return "PvA";
            }
        });
        
        playerOptions.Add(new MenuItem()
        {
            Title = "AI vs AI",
            Shortcut = "3",
            MenuItemAction = () =>
            {
                gameInstance.GameState.PlayersSetting = EGamePlayers.AvA;
                gameInstance.GameState.PlayerX = EGamePlayers.Ai;
                gameInstance.GameState.PlayerO = EGamePlayers.Ai;
                return "AvA";
            }
        });
        

        var configMenu = new Menu(EMenuLevel.Secondary,
            "TIC-TAC-TWO - choose game players",
            playerOptions,
            isCustomMenu: true
        );

        configMenu.Run();
    }

    public static void DisplayMovesOptions(TicTacTwoBrain gameInstance, ConsoleFunctions consoleFunctions)
    {
        var moveOptions = new List<MenuItem>();

        if (gameInstance.PlayerCanPlaceNewPiece(gameInstance.GameState.NextMoveBy))
        {
            moveOptions.Add(new MenuItem()
            {
                Title = "Place new piece",
                Shortcut = "N",
                MenuItemAction = () =>
                {
                    consoleFunctions.ValidateAndMakeANewMove();
                    return "N";
                }
            });    
        }
        

        if (_gameInstance.ShouldMovePiece(gameInstance.GameState.NextMoveBy))
        {
            moveOptions.Add(new MenuItem()
            {
                Title = "Move existing piece",
                Shortcut = "M",
                MenuItemAction = () =>
                {
                    consoleFunctions.HandleExistingPieceMovement(gameInstance.GameState.NextMoveBy);
                    return "M";
                }
            });    
        }
        
        if (_gameInstance.ShouldMoveGrid(gameInstance.GameState.NextMoveBy))
        {
            moveOptions.Add(new MenuItem()
            {
                Title = "Move the grid",
                Shortcut = "G",
                MenuItemAction = () =>
                {
                    do
                    {
                        var newDirection = ChooseNewGridDirection();
                        var isNewLocationInBounds = gameInstance.MoveGrid(newDirection);
                        if (!isNewLocationInBounds)
                        {
                            Console.WriteLine();
                            Console.WriteLine("Cannot move grid out of bounds!");
                            continue;
                        }   
                        
                        gameInstance.ChangeTurn();
                        return "G";     
                    } while (true);
                }
            });   
        }
        
        moveOptions.Add(new MenuItem()
        {
            Title = "Save",
            Shortcut = "S",
            MenuItemAction = () =>
            {
                consoleFunctions.AskPasswordAndSave();
                return "S";
            }
        });
        
        
        var configMenu = new Menu(EMenuLevel.Main,
            "TIC-TAC-TWO - make a move",
            moveOptions,
            isCustomMenu: true
        );

        configMenu.Run();
    }
    
    
    private static string ChooseNewGridDirection()
    {
        var directions = new List<MenuItem>();
        
        directions.Add(new MenuItem()
        {
            Title = "Up",
            Shortcut = "U",
            MenuItemAction = () => "Up"
        });
        
        directions.Add(new MenuItem()
        {
            Title = "Down",
            Shortcut = "D",
            MenuItemAction = () => "Down"
        });
                
        directions.Add(new MenuItem()
        {
            Title = "Left",
            Shortcut = "L",
            MenuItemAction = () => "Left"
        });
        
        directions.Add(new MenuItem()
        {
            Title = "Right",
            Shortcut = "R",
            MenuItemAction = () => "Right"
        });

        directions.Add(new MenuItem()
        {
            Title = "UpLeft",
            Shortcut = "UL",
            MenuItemAction = () => "UpLeft"
        });
        
        directions.Add(new MenuItem()
        {
            Title = "UpRight",
            Shortcut = "UR",
            MenuItemAction = () => "UpRight"
        });
        
        
        directions.Add(new MenuItem()
        {
            Title = "DownLeft",
            Shortcut = "DL",
            MenuItemAction = () => "DownLeft"
        });
        
        directions.Add(new MenuItem()
        {
            Title = "DownRight",
            Shortcut = "DR",
            MenuItemAction = () => "DownRight"
        });

        
        var configMenu = new Menu(EMenuLevel.Main,
            "TIC-TAC-TWO - choose new grid location",
            directions,
            isCustomMenu: true
        );

        return configMenu.Run();
    }
    
    
    private string ChooseConfiguration()
    {
        var configMenuItems = new List<MenuItem>();

        for (var i = 0; i < _configRepository.GetConfigurationNames().Count; i++)
        {
            var returnValue = i.ToString();
            configMenuItems.Add(new MenuItem()
            {
                Title = _configRepository.GetConfigurationNames()[i],
                Shortcut = (i + 1).ToString(),
                MenuItemAction = () => returnValue
            });
        }

        var configMenu = new Menu(EMenuLevel.Secondary,
            "TIC-TAC-TWO - choose new game config",
            configMenuItems,
            isCustomMenu: true
        );

        return configMenu.Run();
    }

    private string ChooseSavedGameConfiguration()
    {
        var configMenuItems = new List<MenuItem>();

        for (var i = 0; i < _gameRepository.GetSavedGamesNames().Count; i++)
        {
            var returnValue = i.ToString();
            configMenuItems.Add(new MenuItem()
            {
                Title = _gameRepository.GetSavedGamesNames()[i],
                Shortcut = (i + 1).ToString(),
                MenuItemAction = () => returnValue
            });
        }

        try
        {
            var configMenu = new Menu(EMenuLevel.Secondary,
                "TIC-TAC-TWO - choose saved game",
                configMenuItems,
                isCustomMenu: true);

            return configMenu.Run();

        }
        catch (Exception)
        {
            Console.WriteLine("No games have been saved!");
        }

        return "null";
    }
    
}