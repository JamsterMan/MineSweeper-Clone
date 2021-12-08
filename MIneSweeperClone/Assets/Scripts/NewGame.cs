using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewGame : MonoBehaviour
{
    public Dropdown boardSize;
    public InputField numMines;
    public Game game;

    public void OnLoadClick()
    {
        int sizeIndex = boardSize.value;
        int size = 10 + (sizeIndex * 5);//10x10,15x15,20x20,etc



    }

}
