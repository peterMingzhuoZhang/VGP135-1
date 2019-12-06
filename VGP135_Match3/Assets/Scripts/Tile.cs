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
    private static bool mIsSwaping = false;

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
        if (render.sprite == null || mBoard.IsShifting || mIsSwaping)
        {
            return;
        }

        if (isSelected)
        { 
            Deselect();
        }
        else
        {
            if (previousSelected == null)
            { 
                Select();
            }
            else
            {
                if (GetAllAdjacentTiles().Contains(previousSelected.gameObject))
                {
                    StartCoroutine(SwapTile(previousSelected));
                    //SwapSprite(previousSelected.render);
                }
                else
                {
                    previousSelected.GetComponent<Tile>().Deselect();
                    Select();
                }
            }
            
        }
        previousSelected?.ClearAllMatches();
    }

    public void SwapSprite(SpriteRenderer render2)
    {
        if (render.sprite == render2.sprite)
        {
            return;
        }

        Sprite tempSprite = render2.sprite;
        render2.sprite = render.sprite;
        render.sprite = tempSprite;

        mBoard.SetMoveCount(-1);
    }

    public IEnumerator SwapTile(Tile other)
    {
        mIsSwaping = true;
        Vector3 pos0 = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 pos1 = new Vector3(other.transform.position.x, other.transform.position.y, other.transform.position.z);


        while (Vector2.Distance(other.transform.position, pos0) > 0.1f || Vector2.Distance(transform.position, pos1) > 0.1f)
        {
            transform.position += (pos1 - transform.position) * mSwapSpeed * Time.deltaTime;
            other.transform.position += (pos0 - other.transform.position) * mSwapSpeed * Time.deltaTime;
          yield return new WaitForSeconds(Time.deltaTime);
        }
        transform.position = pos0;
        other.transform.position = pos1;

        SwapSprite(previousSelected.render);
        yield return new WaitForSeconds(0.3f);

        other.ClearAllMatches();
        other.Deselect();
        ClearAllMatches();
        //mBoard.SetMoveCount(-1);

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
    {
        List<GameObject> matchingTiles = new List<GameObject>(); 
        RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
        while (hit.collider != null && hit.collider.GetComponent<SpriteRenderer>().sprite == render.sprite)
        {
            matchingTiles.Add(hit.collider.gameObject);
            hit = Physics2D.Raycast(hit.collider.transform.position, castDir);
        }
        return matchingTiles;
    }

    private void ClearMatch(Vector2[] paths)
    {
        List<GameObject> matchingTiles = new List<GameObject>();
        for (int i = 0; i < paths.Length; i++)
        {
            matchingTiles.AddRange(FindMatch(paths[i]));
        }
        if (matchingTiles.Count >= 2)
        {
            for (int i = 0; i < matchingTiles.Count; i++)
            {
                matchingTiles[i].GetComponent<SpriteRenderer>().sprite = null;
            }
            matchFound = true;
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
