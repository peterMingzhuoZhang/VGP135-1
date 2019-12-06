using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalMenu : MonoBehaviour
{
    public string levelSelectSceneName;
    public GameObject goalMenuHolder;

    void Start()
    {
        SetGoalMenu(false);
    }

    public void SetGoalMenu(bool value)
    {
        goalMenuHolder.SetActive(value);
    }

    public void PlayAgainButtonDown()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LevelSelectButtonDown()
    {
        SceneManager.LoadScene(levelSelectSceneName);
    }
}
