using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    public static bool mIsSwaping = false;
    private static Tile previousSelected = null;

    private static Color mSelectedColor = new Color(.5f, .5f, .5f, 1.0f);

    public float mSwapSpeed = 10.0f;
    private SpriteRenderer mRender;
    private bool mIsSelected = false;

    private Vector2[] mAdjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

    private bool mMatchFound = false;

    private Board mBoard;

    void Start()
    {
        mRender = GetComponent<SpriteRenderer>();
        mBoard = GetComponentInParent<Board>();
    }

    private void Select()
    {
        mIsSelected = true;
        mRender.color = mSelectedColor;
        previousSelected = gameObject.GetComponent<Tile>();
    }

    private void Deselect()
    {
        mIsSelected = false;
        mRender.color = Color.white;
        previousSelected = null;
    }

    void OnMouseDown()
    {
        // 1
        if (mRender.sprite == null || mBoard.IsShifting || mIsSwaping)
        {
            return;
        }

        if (mIsSelected)
        { // 2 Is it already selected?
            Deselect();
        }
        else
        {
            if (previousSelected == null)
            { // 3 Is it the first tile selected?
                Select();
            }
            else
            {
                if (GetAllAdjacentTiles().Contains(previousSelected.gameObject))
                { // 1
                    StartCoroutine(SwapTile(previousSelected));

                    
                }
                else
                { // 3
                    previousSelected.GetComponent<Tile>().Deselect();
                    Select();
                }
            }

        }
    }

    public IEnumerator SwapTile(Tile other)
    { // 1
        mIsSwaping = true;
        Vector3 pos0 = transform.position;
        Vector3 pos1 = other.transform.position;

        while ( Vector2.Distance(other.transform.position, pos0) > 0.1f || Vector2.Distance(transform.position, pos1) > 0.1f)
        {
            transform.position += (pos1 - transform.position) * mSwapSpeed * Time.deltaTime;
            other.transform.position += (pos0 - other.transform.position) * mSwapSpeed * Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        
        transform.position = pos1;
        other.transform.position = pos0;

        bool isMatched0 = other.ClearAllMatches();

        other.Deselect();
        bool isMatched1 = ClearAllMatches();

        //if (!isMatched0 && !isMatched1)
        //{
        //    while (Vector2.Distance(other.transform.position, pos1) > 0.1f || Vector2.Distance(transform.position, pos0) > 0.1f)
        //    {
        //        transform.position += (pos0 - transform.position) * mSwapSpeed * Time.deltaTime;
        //        other.transform.position += (pos1 - other.transform.position) * mSwapSpeed * Time.deltaTime;
        //        yield return new WaitForSeconds(Time.deltaTime);
        //    }
        //    transform.position = pos0;
        //    other.transform.position = pos1;
        //
        //    mInBoardIndex = other.mInBoardIndex + mInBoardIndex;            //|
        //    other.mInBoardIndex = mInBoardIndex - other.mInBoardIndex;      //|--- Swap int without use the temp variable
        //    mInBoardIndex = mInBoardIndex - other.mInBoardIndex;            //|
        //}
        //else
            mBoard.SetMoveCount(-1);

        mIsSwaping = false;
    }

    private GameObject GetAdjacent(Vector2 castDir)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
        if (hit.collider != null)
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    private List<GameObject> GetAllAdjacentTiles()
    {
        List<GameObject> adjacentTiles = new List<GameObject>();
        for (int i = 0; i < mAdjacentDirections.Length; i++)
        {
            adjacentTiles.Add(GetAdjacent(mAdjacentDirections[i]));
        }
        return adjacentTiles;
    }

    private List<GameObject> FindMatch(Vector2 castDir)
    { // 1
        List<GameObject> matchingTiles = new List<GameObject>(); // 2
        RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir); // 3
        while (hit.collider != null && hit.collider.GetComponent<SpriteRenderer>().sprite == mRender.sprite)
        {
            matchingTiles.Add(hit.collider.gameObject);
            hit = Physics2D.Raycast(hit.collider.transform.position, castDir);
        }
        return matchingTiles; // 5
    }

    private void ClearMatch(Vector2[] paths) // 1
    {
        List<GameObject> matchingTiles = new List<GameObject>(); // 2
        for (int i = 0; i < paths.Length; i++) // 3
        {
            matchingTiles.AddRange(FindMatch(paths[i]));
        }
        if (matchingTiles.Count >= 2) // 4
        {
            for (int i = 0; i < matchingTiles.Count; i++) // 5
            {
                matchingTiles[i].GetComponent<Tile>().mRender.sprite = null;
            }
             mMatchFound = true; // 6
        }
        else
        {
            mMatchFound = false; // 6
        }
    }

    public bool ClearAllMatches()
    {
        if (mRender.sprite == null)
            return false;

        ClearMatch(new Vector2[2] { Vector2.left, Vector2.right });
        ClearMatch(new Vector2[2] { Vector2.up, Vector2.down });
        if (mMatchFound)
        {
            mRender.sprite = null;
            mMatchFound = false;
            StopCoroutine(mBoard.FindNullTiles());
            StartCoroutine(mBoard.FindNullTiles());
            
            return true;
        }
        else
        {
            return false;
        }
    }
}
