using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public InGameUI mInGameUI;
    public List<Sprite> tilesSprite = new List<Sprite>();
    public GameObject tile;
    public int xSize, ySize;

    private GameObject[,] tiles;

    public bool IsShifting { get; set; }

    public int mPreviousSelectedTileIndex = -1;

    public int mMoveCount = 10;
    public int mScore = 0;


    private Vector2 tileOffset;
    void Start()
    {
        tileOffset = tile.GetComponent<SpriteRenderer>().bounds.size;
        CreateBoard(tileOffset.x, tileOffset.y);

        SetScore(0);
        SetMoveCount(0);
    }

    public void SetScore(int value)
    {
        mScore += value;
        mInGameUI.Score = mScore;
    }

    public void SetMoveCount(int value)
    {
        mMoveCount += value;
        mInGameUI.MoveCount = mMoveCount;

    }

    private void CreateBoard(float xOffset, float yOffset)
    {
        tiles = new GameObject[xSize, ySize];

        float startX = transform.position.x;
        float startY = transform.position.y;

        Sprite[] previousLeft = new Sprite[ySize];
        Sprite previousBelow = null;

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                GameObject newTile = Instantiate(tile, new Vector3(startX + (xOffset * x), startY + (yOffset * y), 0), tile.transform.rotation);
                tiles[x, y] = newTile;
                newTile.transform.parent = transform; // 1

                List<Sprite> possibleCharacters = new List<Sprite>(); // 1
                possibleCharacters.AddRange(tilesSprite); // 2

                possibleCharacters.Remove(previousLeft[y]); // 3
                possibleCharacters.Remove(previousBelow);

                Sprite newSprite = possibleCharacters[Random.Range(0, possibleCharacters.Count)];

                newTile.GetComponent<SpriteRenderer>().sprite = newSprite; // 3
                previousLeft[y] = newSprite;
                previousBelow = newSprite;
            }
        }
    }

    public IEnumerator FindNullTiles()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                if (tiles[x, y].GetComponent<SpriteRenderer>().sprite == null)
                {
                    yield return StartCoroutine(ShiftTilesDown(x, y));
                    break;
                }
            }
        }

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                while (IsShifting) ;
                tiles[x, y].GetComponent<Tile>().ClearAllMatches();
            }
        }

    }

    private IEnumerator ShiftTilesDown(int x, int yStart, float shiftDelay = .03f)
    {
        IsShifting = true;
        List<GameObject> moveDownTiles = new List<GameObject>();
        int nullCount = 0;

        for (int y = yStart; y < ySize; y++)
        {  // 1
            SpriteRenderer render = tiles[x, y].GetComponent<SpriteRenderer>();
            if (render.sprite == null)
            { // 2
                nullCount++;
            }
            moveDownTiles.Add(tiles[x, y]);
        }

        for (int i = 0; i < nullCount; i++)
        { // 3
            SetScore(+50);
            var newSprite = GetNewSprite(x, ySize - 1);
            moveDownTiles[i].GetComponent<SpriteRenderer>().sprite = newSprite;
           // moveDownTiles[i].GetComponent<Tile>().mInBoardIndex = x * ySize + ySize - (nullCount - i);      // Get the InBoundCount
            moveDownTiles[i].transform.position = tiles[x, ySize - 1].transform.position + new Vector3(0.0f, tileOffset.y * (i + 1), 0.0f);

        }
        //for (int k = 0; k < moveDownTiles.Count; k++)
        //{ // 5
        //    moveDownTiles[k].transform.position = moveDownTiles[k].transform.position - new Vector3(0.0f, tileOffset.y * nullCount, 0.0f);
        //    //moveDownTiles[k].GetComponent<Tile>().mInBoardIndex = 
        //}
        yield return new WaitForSeconds(shiftDelay);// 4
        Debug.Log("shift finished")
;
        IsShifting = false;
    }

    private Sprite GetNewSprite(int x, int y)
    {
        List<Sprite> possibleCharacters = new List<Sprite>();
        possibleCharacters.AddRange(tilesSprite);

        if (x > 0)
        {
            possibleCharacters.Remove(tiles[x - 1, y].GetComponent<SpriteRenderer>().sprite);
        }
        if (x < xSize - 1)
        {
            possibleCharacters.Remove(tiles[x + 1, y].GetComponent<SpriteRenderer>().sprite);
        }
        if (y > 0)
        {
            possibleCharacters.Remove(tiles[x, y - 1].GetComponent<SpriteRenderer>().sprite);
        }

        return possibleCharacters[Random.Range(0, possibleCharacters.Count)];
    }
}
