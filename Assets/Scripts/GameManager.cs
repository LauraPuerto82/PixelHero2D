using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private GameObject gameOverPanel;
    private GameObject levelPassedPanel;

    private void Awake()
    {
        instance = this;
        Time.timeScale = 1;
        gameOverPanel = GameObject.Find("GameOverPanel");
        gameOverPanel.SetActive(false);
        levelPassedPanel = GameObject.Find("LevelPassedPanel");
        levelPassedPanel.SetActive(false);
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
        LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadScene(int sceneNumber)
    {       
        SceneManager.LoadScene(sceneNumber);
    }
}