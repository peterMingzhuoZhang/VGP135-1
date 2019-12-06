using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    public GameObject mGameOverMenu;
    public Text mScoreText;
    public Text mMoveCountText;

    private int mScore;
    private int mMoveCount;

    public int Score { get { return mScore; } set
        {
            mScore = value;
            mScoreText.text = "Score: " + mScore.ToString();
        }
    }

    public int MoveCount { get { return mMoveCount; }
        set
        {
            mMoveCount = value;
            mMoveCountText.text = "Move Left: " + mMoveCount.ToString();
           // if (moveCounter <= 0)                             //|
           // {                                                 //|
           //     moveCounter = 0;                              //|--- TODO move this to board
           //     StartCoroutine(WaitForShifting());            //|
           // }                                                 //|

        }
    }

    void Awake()
    {

    }

    // Show the game over panel
    public void GameOver()
    {
        //GameManager.instance.gameOver = true;

        mGameOverMenu.SetActive(true);
        
        // TODO set su the score in the gameovermenu

    }

    //private IEnumerator WaitForShifting()                                         //|
    //{                                                                             //|
    //    yield return new WaitUntil(() => !BoardManager.instance.IsShifting);      //|--- TODO: Move to Board
    //    yield return new WaitForSeconds(.25f);                                    //|
    //    GameOver();                                                               //|
    //}                                                                             //|
}
