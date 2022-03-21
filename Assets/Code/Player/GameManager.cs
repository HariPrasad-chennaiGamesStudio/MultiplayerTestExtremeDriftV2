using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    private void Awake() {
        if(instance == null)
        {
            instance = this;
        }
    }

    public string playerName = "";
    public string receivedMsg = "";
    public string sendingMsg = "";
    public bool canSendTypedText = false;

    public bool leftBtnPressed = false;
    public bool rightButtonPressed = false;
    public bool forwardButtonPressed = false;
    public bool backwardButtonPressed = false;

    public enum Pots
    {
        Free,
        Hundred,
        TwoHundred,
        ThreeHundred,
        FourHundred,
        FiveHundred
    }
    public enum Scenes
    {
        Landing,
        Garage,
        GamePlay
    }

    public Pots potSelected;
    public Scenes currentScene;

}
