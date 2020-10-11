using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    #region Private Fields

    private static Vector2[] adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.right, Vector2.left };
    private static Tile previousSelected = null;
    private static Color selectedColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    private bool IsSelected = false;
    private bool matchFound = false;
    private SpriteRenderer render;

    #endregion Private Fields

    #region Public Methods

    public void ClearAllMatches()
    {
        if (render.sprite == null)
        {
            return;
        }

        ClearMatches(new Vector2[2] { Vector2.up, Vector2.down });
        ClearMatches(new Vector2[2] { Vector2.right, Vector2.left });
        if (matchFound)
        {
            render.sprite = null;
            matchFound = false;
            GuiManager.instance.MoveCounter--;
            StopCoroutine(BoardManager.instance.FindNullTiles());
            StartCoroutine(BoardManager.instance.FindNullTiles());
        }
    }

    #endregion Public Methods

    #region Private Methods

    private void Awake()
    {
        render = GetComponent<SpriteRenderer>();
    }

    private void ClearMatches(Vector2[] paths)
    {
        List<GameObject> matches = new List<GameObject>();
        // find matches along paths
        for (int i = 0; i < paths.Length; i++)
        {
            matches.AddRange(FindMatch(paths[i]));
        }
        if (matches.Count >= 2)
        {
            for (int j = 0; j < matches.Count; j++)
            {
                matches[j].GetComponent<SpriteRenderer>().sprite = null;
            }
            matchFound = true;
        }
    }

    private void Deselect()
    {
        IsSelected = false;
        render.color = Color.white;
        previousSelected = null;
    }

    private List<GameObject> FindMatch(Vector2 castDir)
    {
        List<GameObject> matches = new List<GameObject>();
        RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
        // find all matches in 1 direction
        while (hit.collider != null && hit.collider.GetComponent<SpriteRenderer>().sprite == render.sprite)
        {
            matches.Add(hit.collider.gameObject);
            hit = Physics2D.Raycast(hit.collider.transform.position, castDir);
        }
        return matches;
    }

    private GameObject GetAdjacent(Vector2 castDir)
    {
        // turn of own collider
        //gameObject.GetComponent<BoxCollider2D>().enabled = false;
        Ray2D ray = new Ray2D(transform.position, castDir);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        if (hit.collider != null)
        {
            //gameObject.GetComponent<BoxCollider2D>().enabled = true;
            //Debug.DrawLine(ray.origin, hit.point, Color.red, 1.0f);
            return hit.collider.gameObject;
        }
        //gameObject.GetComponent<BoxCollider2D>().enabled = true;
        return null;
    }

    private List<GameObject> GetAllAdjacents()
    {
        List<GameObject> adjacents = new List<GameObject>();

        for (int i = 0; i < adjacentDirections.Length; i++)
        {
            adjacents.Add(GetAdjacent(adjacentDirections[i]));
        }
        return adjacents;
    }

    private void OnMouseDown()
    {
        if (render.sprite == null || BoardManager.instance.IsShifting)
        {
            return;
        }
        if (IsSelected)
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
                List<GameObject> adjacents = GetAllAdjacents();
                if (adjacents.Contains(previousSelected.gameObject))
                {
                    //Debug.Log("Swapping");
                    SwapSprite(previousSelected.render);
                    previousSelected.ClearAllMatches();
                    previousSelected.Deselect();
                    ClearAllMatches();
                }
                else
                {
                    previousSelected.Deselect();
                    Select();
                }
            }
        }
    }

    private void Select()
    {
        IsSelected = true;
        render.color = selectedColor;
        previousSelected = gameObject.GetComponent<Tile>();
    }

    private void SwapSprite(SpriteRenderer render2)
    {
        if (render.sprite == render2.sprite) return;

        Sprite tempSprite = render2.sprite;
        render2.sprite = render.sprite;
        render.sprite = tempSprite;
    }

    #endregion Private Methods
}