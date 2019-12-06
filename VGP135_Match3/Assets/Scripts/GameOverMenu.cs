using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public string mainMenuSceneName;
    public GameObject MenuHolder;

    public Text mScore;

    void Start()
    {
        SetGoalMenu(false);
    }

    public void SetGoalMenu(bool value)
    {
        MenuHolder.SetActive(value);
    }

    public void PlayAgainButtonDown()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LevelSelectButtonDown()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
