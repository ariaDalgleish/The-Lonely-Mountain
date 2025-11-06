using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    private static PauseMenuManager instance;

    public GameObject pauseMenuCanvas;
    public GameObject menuHelp;
    public GameObject menuMeters;

    public bool IsPaused { get; private set; }
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.Instance.MenuOpen())
        {
            if (!IsPaused && MenuManager.Instance.CanOpenMenu(MenuType.Pause))
            {
                Pause();
            }
        }
        else if (InputManager.Instance.MenuClose())
        {
            if (IsPaused)
            {
                Unpause();
            }
        }
    }

    public void Pause()
    {
        IsPaused = true;
        Time.timeScale = 0f;
        OpenMenu();
        MenuManager.Instance.OpenMenu(MenuType.Pause);

        menuHelpActive = false;
        // Hide the UI prompt if active
        if (menuHelp != null && menuHelp.activeSelf)
            menuHelp.SetActive(false);
        if (menuMeters != null && menuMeters.activeSelf)
            menuMeters.SetActive(false);

        // Switch to UI controls
        InputManager.Instance.playerControls.Player.Disable();
        InputManager.Instance.playerControls.UI.Enable();
    }

    public void Unpause()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        CloseMenu();
        MenuManager.Instance.CloseMenu(MenuType.Pause);

        // Do NOT reactivate menuHelp here
        // menuHelpActive = true;

        // Show the UI prompt if it was hidden
        if (menuMeters != null && !menuMeters.activeSelf)
            menuMeters.SetActive(true);

        // Switch back to Player controls
        InputManager.Instance.playerControls.UI.Disable();
        InputManager.Instance.playerControls.Player.Enable();
    }

    public void OpenMenu()
    {
        if (pauseMenuCanvas != null)
            pauseMenuCanvas.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseMenu()
    {
        if (pauseMenuCanvas != null)
            pauseMenuCanvas.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Go to main menu scene
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        // close the application
        Application.Quit();
        // if in the editor, stop playing
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

    }

    public bool menuHelpActive
    {
        get => menuHelp != null && menuHelp.activeSelf;
        set
        {
            if (menuHelp != null)
                menuHelp.SetActive(value);
        }
    }

    // Method for Timeline signal to call
    public void SetMenuHelpActive(bool active)
    {
        menuHelpActive = active;
    }
}
