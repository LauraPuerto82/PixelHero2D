using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private GameObject gameOverPanel;
    private GameObject levelPassedPanel;

    private bool _gameStarted;    

    public bool IsGameStarted { get => _gameStarted; set => _gameStarted = value; }    

    private void Awake()
    {
        instance = this;
        _gameStarted = false;

        gameOverPanel = GameObject.Find("GameOverPanel");
        levelPassedPanel = GameObject.Find("LevelPassedPanel");

        gameOverPanel.SetActive(false);
        levelPassedPanel.SetActive(false);
    }

    private void Start()
    {
        if(_gameStarted)
        {
            StartGame();
        }
    }

    public void LevelPassed()
    {
        levelPassedPanel?.SetActive(true);
    }
    
    public void GameOver()
    {        
        StartCoroutine(WaitToReload());        
    }          

    IEnumerator WaitToReload()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(2);        
        SaveDataGame.instance.ResetGame();
    }

    public void LoadScene(int sceneNumber)
    {       
        SceneManager.LoadScene(sceneNumber);
    }

    private void OnApplicationQuit()
    {        
        PlayerPrefs.SetString(SaveDataGame.instance.ItemsKey, SaveDataGame.instance.ItemsValue);

        SaveDataGame.instance.SaveData();
    }

    public void StartGame()
    {        
        Time.timeScale = 1;                               
    }
}