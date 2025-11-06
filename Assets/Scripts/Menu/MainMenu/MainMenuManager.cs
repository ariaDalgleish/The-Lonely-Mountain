using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video; // Add this

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject _loadingBarObject;
    [SerializeField] private Image _loadingBar;
    [SerializeField] private GameObject[] _menuObjects;
    [SerializeField] private VideoPlayer introVideoPlayer; // Assign in Inspector

    [Header("Scenes To Load")]
    [SerializeField] private SceneField _persistentGameplay;
    [SerializeField] private SceneField _levelScene;

    private void Awake()
    {
        _loadingBarObject.SetActive(false);
        if (introVideoPlayer != null)
            introVideoPlayer.gameObject.SetActive(false);
    }

    public void StartGame()
    {
        HideMenu();
        _loadingBarObject.SetActive(false);

        if (introVideoPlayer != null)
        {
            introVideoPlayer.gameObject.SetActive(true);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            introVideoPlayer.loopPointReached += OnIntroVideoFinished;
            introVideoPlayer.Play();
        }
        else
        {
            LoadScenes();
        }
    }

    private void OnIntroVideoFinished(VideoPlayer vp)
    {
        vp.loopPointReached -= OnIntroVideoFinished;
        vp.gameObject.SetActive(false);
        LoadScenes();
    }

    private void LoadScenes()
    {
        SceneManager.LoadSceneAsync(_persistentGameplay.SceneName);
        SceneManager.LoadSceneAsync(_levelScene.SceneName, LoadSceneMode.Additive);
    }

    private void HideMenu()
    {
        for (int i = 0; i < _menuObjects.Length; i++)
        {
            _menuObjects[i].SetActive(false);
        }
    }

    //private IEnumerator ProgressLoadingBar()
    //{
    //    float loadProgress = 0f;
    //    for (int i = 0; i < _scenesToLoad.Count; i++)
    //    {
    //        while(!_scenesToLoad[i].isDone)
    //        {
    //            loadProgress += _scenesToLoad[i].progress;
    //            _loadingBar.fillAmount = loadProgress / _scenesToLoad.Count;
    //            yield return null;
    //        }
    //    }
    //}

    public void LoadGame()
    {
        // load previously saved game
    }

    public void QuitGame()
    {
        // close the application
        Application.Quit();
    }

}
