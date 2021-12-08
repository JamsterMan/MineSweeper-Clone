using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewGame : MonoBehaviour
{
    public TMPro.TMP_Dropdown boardSize;
    public TMPro.TMP_InputField numMines;
    public Game game;

    private void Start()
    {
        boardSize = GetComponentInChildren<TMPro.TMP_Dropdown>();
        numMines = GetComponentInChildren<TMPro.TMP_InputField>();
    }

    //gets values from UI to change board size and number of mines
    public void OnLoadClick()
    {

        int sizeIndex = boardSize.value;
        int size = 10 + (sizeIndex * 5);//10x10,15x15,20x20,etc
        int mines = int.Parse( numMines.text);

        if(mines > size * size) {
            mines = size * size;
        }

        game.NewBoard(size, size, mines);
        
    }

}
