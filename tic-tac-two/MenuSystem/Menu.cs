namespace MenuSystem;

public class Menu
{
    private string MenuHeader { get; set; }
    private static readonly string MenuDivider = "=================";
    private List<MenuItem> MenuItems { get; set; }

    private readonly MenuItem _menuItemExit = new MenuItem()
    {
        Shortcut = "E",
        Title = "Exit",
        MenuItemAction = () =>
        {
            Environment.Exit(0);
            return "Exiting application...";
        }
    };

    private readonly MenuItem _menuItemReturn = new MenuItem()
    {
        Shortcut = "R",
        Title = "Return"
    };
    private readonly MenuItem _menuItemReturnMain = new MenuItem()
    {
        Shortcut = "M",
        Title = "Return to Main menu"
    };

    private EMenuLevel MenuLevel { get; set; }

    private bool IsCustomMenu { get; set; }

    public Menu(EMenuLevel menuLevel, string menuHeader, List<MenuItem> menuItems, bool isCustomMenu = false)
    {
        if (string.IsNullOrWhiteSpace(menuHeader))
        {
            throw new ApplicationException("Menu header cannot be empty.");
        }

        MenuHeader = menuHeader;

        if (menuItems == null || menuItems.Count == 0)
        {
            throw new ApplicationException("Menu items cannot be empty.");
        }


        MenuItems = menuItems;
        IsCustomMenu = isCustomMenu;
        MenuLevel = menuLevel;


        if (MenuLevel != EMenuLevel.Main)
        {
            MenuItems.Add(_menuItemReturn);
        }

        if (MenuLevel == EMenuLevel.Deep)
        {
            MenuItems.Add(_menuItemReturnMain);
        }

        MenuItems.Add(_menuItemExit);
        
        ValidateShortcutConflicts();
    }


    private void ValidateShortcutConflicts()
    {
        var shortcutGroups = MenuItems.GroupBy(mi => mi.Shortcut.ToUpper()).Where(g => g.Count() > 1).ToList();

        if (!shortcutGroups.Any()) return;

        var conflicts = shortcutGroups.Select(g => g.Key).ToList();
        throw new InvalidOperationException(
            $"Shortcut conflicts detected: {string.Join(", ", conflicts)}. Each menu item must have a unique shortcut.");

    }

    public string Run()
    {
        do
        {
            var menuItem = DisplayMenuGetUserChoice();
            var menuReturnValue = "";

            if (menuItem.MenuItemAction != null)
            {
                menuReturnValue = menuItem.MenuItemAction();
                if (IsCustomMenu) return menuReturnValue;
            }

            if (menuItem.Shortcut == _menuItemReturn.Shortcut)
            {
                return menuItem.Shortcut;
            }

            if (menuItem.Shortcut == _menuItemExit.Shortcut || menuReturnValue == _menuItemExit.Shortcut)
            {
                return _menuItemExit.Shortcut;
            }

            if ((menuItem.Shortcut == _menuItemReturnMain.Shortcut || menuReturnValue == _menuItemReturnMain.Shortcut) && MenuLevel != EMenuLevel.Main)
            {
                return _menuItemReturnMain.Shortcut;
            }

        } while (true);
    }

    private MenuItem DisplayMenuGetUserChoice()
    {
        do
        {
            DrawMenu();

            var userInput = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(userInput))
            {
                Console.WriteLine("Input cannot be empty.");
                Console.WriteLine();
            }
            else
            {
                userInput = userInput.ToUpper();

                foreach (var menuItem in MenuItems)
                {
                    if (menuItem.Shortcut.ToUpper() != userInput) continue;
                    return menuItem;
                }

                Console.WriteLine("Choose existing option shortcut.");
                Console.WriteLine();
            }
        } while (true);
    }

    private void DrawMenu()
    {
        Console.WriteLine(MenuHeader);
        Console.WriteLine(MenuDivider);

        foreach (var t in MenuItems)
        {
            Console.WriteLine(t);
        }

        Console.WriteLine();

        Console.Write(">");
    }
}