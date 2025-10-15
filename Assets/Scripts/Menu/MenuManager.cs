using UnityEngine;

public enum MenuType
{
    None,
    Pause,
    Inventory
}

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    public MenuType CurrentMenu { get; private set; } = MenuType.None;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public bool CanOpenMenu(MenuType menu)
    {
        return CurrentMenu == MenuType.None || CurrentMenu == menu;
    }

    public void OpenMenu(MenuType menu)
    {
        CurrentMenu = menu;
    }

    public void CloseMenu(MenuType menu)
    {
        if (CurrentMenu == menu)
            CurrentMenu = MenuType.None;
    }

    public bool IsMenuOpen(MenuType menu)
    {
        return CurrentMenu == menu;
    }
}