using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public Tile[,] grid;
    public int width = 10;
    public int height = 10;
    public int numMines = 12;
    static public int SetNumMines = 0;
    public int numFlags = 0;
    public int numCoveredTiles = 0;
    private int xOld, yOld = -1;
    private bool gameOver = false;
    private bool middleFunc = false;
    private bool firstReveal = true;
    public Text mineCounter;
    public Text mineSlider;
    //public ChangeMines mineSlider;
    public Slider slider;


    void Start()
    {
        if (SetNumMines > 0) {
            numMines = SetNumMines;
        }
        mineCounter.text = "" + numMines;
        mineSlider.text = "" + numMines;
        //mineSlider.UpdateText(numMines);
        slider.value = numMines;
        numCoveredTiles = width * height;
        grid = new Tile[width, height];
        gameOver = false;
        numFlags = 0;

        /*for (int m = 0; m < numMines; m++) { // place mines in the field
            PlaceMines();
        }*/

        for (int i = 0; i < width; i++){
            for (int j = 0; j < height; j++){
                PlaceTiles(i, j);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameOver) {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int x = Mathf.RoundToInt(mousePosition.x);
            int y = Mathf.RoundToInt(mousePosition.y);
            if (x >= 0 && x < width && y >= 0 && y < height) {//check if mouse was in the game field
                if (Input.GetButtonDown("Fire1")) {//reveal a tile
                    if (firstReveal) {
                        PlaceMines(x,y);
                        firstReveal = false;
                    }
                    Tile tile = grid[x, y];
                    if (tile.isCovered && !tile.isFlaged) {//cant reveal a flaged tile
                        tile.RevealTile();
                        numCoveredTiles--;
                        if (tile.type == Tile.TileType.Blank) {
                            RevealNeighbors(x, y);
                        } else if (tile.type == Tile.TileType.Mine) {//game over
                            tile.HitMine();
                            GameLost();
                        }
                    }
                } else if (Input.GetButtonDown("Fire2")) {//flag a tile
                    Tile tile = grid[x, y];
                    if (tile.isCovered) {//cant flag revealed tiles
                        if (tile.isFlaged) {//if tile is already flaged
                            numFlags--;
                            numCoveredTiles++;
                            tile.UnflagTile();
                        } else {
                            numFlags++;
                            numCoveredTiles--;
                            tile.FlagTile();
                        }
                    }
                } else if (Input.GetButtonDown("Fire3")) {//turn on highlight
                    middleFunc = true;
                    HighlightNeighbors(x, y);
                    xOld = x;
                    yOld = y;
                } else if (Input.GetButtonUp("Fire3")) {//turn off highlight
                    middleFunc = false;
                    UnHighlightNeighbors(x, y);
                }
            }
            if (middleFunc) {//highlight surronding uncovered tiles, and reveal tile if correct number of flags present
                if (xOld != x || yOld != y) {
                    if(xOld >= 0 && yOld >= 0)
                        UnHighlightNeighbors(xOld, yOld);
                    if (x >= 0 && x < width && y >= 0 && y < height) { //check if mouse was in the game field
                        HighlightNeighbors(x, y);
                        xOld = x;
                        yOld = y;
                    } else {
                        if (Input.GetButtonUp("Fire3")) {//turn off highlight
                            middleFunc = false;
                            xOld = -1;
                            yOld = -1;
                        }
                    }
                }
            }
            if (numFlags + numCoveredTiles == numMines || numFlags == numMines) {//checks if win condition met
                CheckWin();
            }
        }
    }

    void RevealNeighbors(int x, int y)
    {
        for (int xOff = -1; xOff <= 1; xOff++) {
            for (int yOff = -1; yOff <= 1; yOff++) {
                if (x + xOff > -1 && x + xOff < width && y + yOff > -1 && y + yOff < height) {//for coner tiles
                    if (!grid[x + xOff, y + yOff].isFlaged) {//dont reveal if the player miss flagged something
                        if (grid[x + xOff, y + yOff].type == Tile.TileType.Blank && grid[x + xOff, y + yOff].isCovered) {
                            numCoveredTiles--;
                            grid[x + xOff, y + yOff].RevealTile();
                            RevealNeighbors(x + xOff, y + yOff);
                        } else if(grid[x + xOff, y + yOff].isCovered) {
                            numCoveredTiles--;
                            if (grid[x + xOff, y + yOff].type == Tile.TileType.Mine) {
                                grid[x + xOff, y + yOff].RevealTile();
                                grid[x + xOff, y + yOff].HitMine();
                                GameLost();
                            } else {
                                grid[x + xOff, y + yOff].RevealTile();
                            }
                        }
                    }
                }
            }
        }
    }

    void HighlightNeighbors(int x, int y)
    {
        int totalFlags = 0;
        for (int xOff = -1; xOff <= 1; xOff++) {
            for (int yOff = -1; yOff <= 1; yOff++) {
                if (x + xOff > -1 && x + xOff < width && y + yOff > -1 && y + yOff < height) {//for coner tiles
                    if (!grid[x + xOff, y + yOff].isFlaged) {//dont highlight if flagged
                        if (grid[x + xOff, y + yOff].isCovered) {
                            grid[x + xOff, y + yOff].HighlightTile();
                        }
                    } else {
                        totalFlags++;
                    }
                }
            }
        }
        if(grid[x, y].type == Tile.TileType.Num && grid[x,y].mineNeighbors == totalFlags) {//reveals neighbor tiles if the correct number of flags present
            RevealNeighbors(x,y);
            middleFunc = false;
        }
    }
    void UnHighlightNeighbors(int x, int y)
    {
        for (int xOff = -1; xOff <= 1; xOff++) {
            for (int yOff = -1; yOff <= 1; yOff++) {
                if (x + xOff > -1 && x + xOff < width && y + yOff > -1 && y + yOff < height) {//for coner tiles
                    if (!grid[x + xOff, y + yOff].isFlaged) {//dont highlight if flagged
                        if (grid[x + xOff, y + yOff].isCovered) {
                            grid[x + xOff, y + yOff].UnHighlightTile();
                        }
                    }
                }
            }
        }
    }

    void GameLost()
    {
        //game over
        Debug.Log("Defeat");
        gameOver = true;//stops revealing more tiles after gameover
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (grid[i, j].type == Tile.TileType.Mine && grid[i, j].isCovered && !grid[i, j].isFlaged) {//check if all flags are right
                    grid[i, j].RevealTile();
                }
                if (grid[i, j].type != Tile.TileType.Mine && grid[i, j].isFlaged) {//check if all flags are right
                    grid[i, j].MissFlag();
                }
            }
        }
    }

    void CheckWin()
    {
        int correctFlags = 0;
        int coveredTiles = 0;
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (grid[i, j].type == Tile.TileType.Mine && grid[i,j].isFlaged) {//check if all flags are right
                    correctFlags++;
                }else if (grid[i, j].isCovered && !grid[i, j].isFlaged) {//counts remaining covered flags
                    coveredTiles++;
                }
            }
        }
        if (correctFlags == numMines || correctFlags + coveredTiles == numMines) {
            Debug.Log("Victory");
            gameOver = true;//stop unflaging and revealing after a win
            for (int i = 0; i < width; i++) {//flag tile that are not flagged yet
                for (int j = 0; j < height; j++) {
                    if (grid[i, j].isCovered && !grid[i, j].isFlaged && grid[i, j].type == Tile.TileType.Mine) {//counts remaining covered flags
                        grid[i, j].FlagTile();
                    }else if (grid[i, j].isCovered && !grid[i, j].isFlaged && grid[i, j].type != Tile.TileType.Mine) {
                        grid[i, j].RevealTile();
                    }
                }
            }
        }
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SetMines()
    {
        SetNumMines = (int)Mathf.Round(slider.value);
        ResetGame();
    }

    /* Sets tiles to mines
     * mousex and mousey are the tile clicked that cant be a mine
     */
    void PlaceMines(int mouseX, int mouseY)
    {
        int x, y;
        for (int m = 0; m < numMines; m++) { // place mines in the field
            x = Random.Range(0, width);
            y = Random.Range(0, height);

            while(grid[x, y].type == Tile.TileType.Mine || (x == mouseX && y == mouseY)) {//makes sure no mine placements overlap
                x = Random.Range(0, width);
                y = Random.Range(0, height);
            }
            grid[x, y].SetTile("Mine", Tile.TileType.Mine);
        }

        for (x = 0; x < width; x++) {
            for (y = 0; y < height; y++) {
                if (grid[x, y].type != Tile.TileType.Mine){
                    int total = 0;
                    for (int xOff = -1; xOff <= 1; xOff++) {
                        for (int yOff = -1; yOff <= 1; yOff++) {
                            if (x + xOff > -1 && x + xOff < width && y + yOff > -1 && y + yOff < height) {//for coner tiles
                                if (grid[x + xOff, y + yOff].type == Tile.TileType.Mine) {
                                    total++;
                                }
                            }
                        }
                    }
                    if (total == 0) {
                        grid[x, y].SetTile("Empty", Tile.TileType.Blank);
                    } else {
                        grid[x, y].SetTile("" + total, Tile.TileType.Num);
                        grid[x, y].SetMineNeighbors(total);
                    }
                }
            }
        }
    }

    void PlaceTiles(int x, int y)
    {
        Tile tile = Instantiate(Resources.Load("Prefabs/Empty", typeof(Tile)), new Vector3(x, y, 0), Quaternion.identity) as Tile;
        grid[x, y] = tile;
    }

    public void SetText()
    {
        mineSlider.text = "" + slider.value;
    }
}
