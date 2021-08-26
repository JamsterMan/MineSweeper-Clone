using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public enum TileType { Blank, Mine, Num }

    public TileType type = TileType.Blank;
    public int mineNeighbors = 0;
    public bool isCovered = true;
    public bool isFlaged = false;
    public Sprite coverSprite;
    public Sprite flagSprite;
    public Sprite hitMineSprite;
    public Sprite missFlagSprite;
    public Sprite highlightSprite;
    private Sprite defaultSprite;

    private void Start()
    {
        defaultSprite = GetComponent<SpriteRenderer>().sprite;
        GetComponent<SpriteRenderer>().sprite = coverSprite;
    }

    /* sets the icon and type of the tile
     * icon is the string name of the icon for the tile in the graphics folder
     * tile is the TileType val
     */
    public void SetTile(string icon, TileType tile)
    {
        defaultSprite = Resources.Load<Sprite>("Graphics/" + icon);
        type = tile;
    }

    public void SetMineNeighbors(int mines)
    {
        mineNeighbors = mines;
    }

    public void RevealTile()
    {
        isCovered = false;
        isFlaged = false;
        GetComponent<SpriteRenderer>().sprite = defaultSprite;
    }

    public void FlagTile()
    {
        isFlaged = true;
        GetComponent<SpriteRenderer>().sprite = flagSprite;
    }
    public void UnflagTile()
    {
        isFlaged = false;
        GetComponent<SpriteRenderer>().sprite = coverSprite;
    }

    public void HitMine()
    {
        Debug.Log("mine hit :" + type);
        GetComponent<SpriteRenderer>().sprite = hitMineSprite;
    }

    public void MissFlag()
    {
        GetComponent<SpriteRenderer>().sprite = missFlagSprite;
    }

    public void HighlightTile()
    {
        GetComponent<SpriteRenderer>().sprite = highlightSprite;
    }

    public void UnHighlightTile()
    {
        GetComponent<SpriteRenderer>().sprite = coverSprite;
    }

}
