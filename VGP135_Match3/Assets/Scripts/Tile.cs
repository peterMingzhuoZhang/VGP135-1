using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private Board mBoard;
    private static Color selectedColor = new Color(.5f, .5f, .5f, 1.0f);
    private static Tile previousSelected = null;

    private SpriteRenderer render;
    private bool isSelected = false;

    private Vector2[] adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

    private bool matchFound = false;

    private float mSwapSpeed = 10.0f;
    private bool mIsSwaping = false;

    void Start()
    {
        render = GetComponent<SpriteRenderer>();
        mBoard = GetComponentInParent<Board>();
    }

    private void Select()
    {
        isSelected = true;
        render.color = selectedColor;
        previousSelected = gameObject.GetComponent<Tile>();
    }

    private void Deselect()
    {
        isSelected = false;
        render.color = Color.white;
        previousSelected = null;
    }

    void OnMouseDown()
    {
        // 1
        if (render.sprite == null || mBoard.IsShifting || mIsSwaping)
        {
            return;
        }

        if (isSelected)
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

                    //SwapSprite(previousSelected.render); // 2
                    //previousSelected.ClearAllMatches();
                    //
                    //previousSelected.Deselect();
                    //ClearAllMatches();

                }
                else
                { // 3
                    previousSelected.GetComponent<Tile>().Deselect();
                    Select();
                }
            }
            
        }
    }

    public void SwapSprite(SpriteRenderer render2)
    { // 1
        if (render.sprite == render2.sprite)
        { // 2
            return;
        }

        Sprite tempSprite = render2.sprite; // 3
        render2.sprite = render.sprite; // 4
        render.sprite = tempSprite; // 5

        mBoard.SetMoveCount(-1);
    }

    public IEnumerator SwapTile(Tile other)
    { // 1
        mIsSwaping = true;
        Vector3 pos0 = transform.position;
        Vector3 pos1 = other.transform.position;

        while (Vector2.Distance(other.transform.position, pos0) > 0.1f || Vector2.Distance(transform.position, pos1) > 0.1f)
        {
            transform.position += (pos1 - transform.position) * mSwapSpeed * Time.deltaTime;
            other.transform.position += (pos0 - other.transform.position) * mSwapSpeed * Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime *0.005f);
        }

        transform.position = pos1;
        other.transform.position = pos0;

        other.ClearAllMatches();

        other.Deselect();
        ClearAllMatches();
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
        for (int i = 0; i < adjacentDirections.Length; i++)
        {
            adjacentTiles.Add(GetAdjacent(adjacentDirections[i]));
        }
        return adjacentTiles;
    }

    private List<GameObject> FindMatch(Vector2 castDir)
    { // 1
        List<GameObject> matchingTiles = new List<GameObject>(); // 2
        RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir); // 3
        while (hit.collider != null && hit.collider.GetComponent<SpriteRenderer>().sprite == render.sprite)
        { // 4
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
                matchingTiles[i].GetComponent<SpriteRenderer>().sprite = null;
            }
            matchFound = true; // 6
        }
    }

    public void ClearAllMatches()
    {
        if (render.sprite == null)
            return;

        ClearMatch(new Vector2[2] { Vector2.left, Vector2.right });
        ClearMatch(new Vector2[2] { Vector2.up, Vector2.down });
        if (matchFound)
        {
            render.sprite = null;
            matchFound = false;
            StopCoroutine(mBoard.FindNullTiles());
            StartCoroutine(mBoard.FindNullTiles());
        }
    }

}
