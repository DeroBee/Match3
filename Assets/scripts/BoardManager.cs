using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    #region Public Fields

    public static BoardManager instance;
    public bool allowDupeSpawning;
    public List<Sprite> characters = new List<Sprite>();
    public float gShiftDelay;
    public Text statusShiftingText;
    public GameObject tile;
    public int xSize, ySize;
    public float xSpacing, ySpacing;

    #endregion Public Fields

    #region Private Fields

    private bool isShifting;

    private GameObject[,] tiles;

    #endregion Private Fields

    #region Public Properties

    public bool IsShifting
    {
        get
        {
            statusShiftingText.text = (isShifting) ? "Currently Shifting" : "Not Shifting";
            return isShifting;
        }
        set
        {
            isShifting = value;
            statusShiftingText.text = (isShifting) ? "Currently Shifting" : "Not Shifting";
        }
    }

    #endregion Public Properties

    #region Public Methods

    public IEnumerator FindNullTiles()
    {
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                if (tiles[i, j].GetComponent<SpriteRenderer>().sprite == null)
                {
                    yield return StartCoroutine(ShiftTilesDown(i, j, gShiftDelay));
                    break;
                }
            }
        }
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                tiles[i, j].GetComponent<Tile>().ClearAllMatches();
            }
        }
    }

    #endregion Public Methods

    #region Private Methods

    private void CreateBoard(float xOff, float yOff)
    {
        tiles = new GameObject[this.xSize, this.ySize];

        float startX = transform.position.x;
        float startY = transform.position.y;

        Sprite[] previousLeft = new Sprite[ySize];
        Sprite previousBelow = null;

        for (int i = 0; i < xSize; i++)
        {
            previousBelow = null;
            for (int j = 0; j < ySize; j++)
            {
                //create new tile
                GameObject newTile = Instantiate(tile, new Vector3(startX + (xOff + xSpacing) * i, startY + (yOff + ySpacing) * j, 0), tile.transform.rotation);
                newTile.name = String.Format("x.{0} y.{1}", i, j);
                tiles[i, j] = newTile;
                //anchor to boardmanagerObject
                newTile.transform.parent = transform;
                // choose sprite
                List<Sprite> possibleCharacters = new List<Sprite>();
                possibleCharacters.AddRange(characters);
                possibleCharacters.Remove(previousBelow);
                possibleCharacters.Remove(previousLeft[j]);
                Sprite newSprite = possibleCharacters[UnityEngine.Random.Range(0, possibleCharacters.Count)];
                // set sprite
                newTile.GetComponent<SpriteRenderer>().sprite = newSprite;

                previousBelow = newSprite;
                previousLeft[j] = newSprite;
            }
        }
    }

    private Sprite GetNewSprite(int x, int y)
    {
        List<Sprite> possibleCharacters = new List<Sprite>();
        possibleCharacters.AddRange(characters);
        if (!allowDupeSpawning)
        {
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
        }

        return possibleCharacters[UnityEngine.Random.Range(0, possibleCharacters.Count)];
    }

    private IEnumerator ShiftTilesDown(int x, int yStart, float shiftDelay)
    {
        IsShifting = true;
        List<SpriteRenderer> renders = new List<SpriteRenderer>();
        int nullCount = 0;

        for (int y = yStart; y < ySize; y++)
        {
            SpriteRenderer render = tiles[x, y].GetComponent<SpriteRenderer>();
            if (render.sprite == null)
            {
                nullCount++;
            }
            renders.Add(render);
        }

        for (int i = 0; i < nullCount; i++)
        {
            GuiManager.instance.Score += 50;
            yield return new WaitForSeconds(shiftDelay);
            for (int j = 0; j < renders.Count - 1; j++)
            {
                renders[j].sprite = renders[j + 1].sprite;
                renders[j + 1].sprite = GetNewSprite(x, ySize - 1);
            }
        }
        if (nullCount == 1 && yStart == ySize - 1)
        {
            renders[0].sprite = GetNewSprite(x, ySize - 1);
        }
        IsShifting = false;
    }

    // Start is called before the first frame update
    private void Start()
    {
        instance = GetComponent<BoardManager>();

        Vector2 offset = tile.GetComponent<SpriteRenderer>().bounds.size;
        CreateBoard(offset.x, offset.y);
    }

    #endregion Private Methods
}