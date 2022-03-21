using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    //Garage...
    public GameObject potPanel;
    public void SelectFreePot()
    {
        GameManager.instance.potSelected = GameManager.Pots.Free;    
    }
    public void SelectHundredPot()
    {
        GameManager.instance.potSelected = GameManager.Pots.Hundred;    
    }
    public void SelectTwoHundredPot()
    {
        GameManager.instance.potSelected = GameManager.Pots.TwoHundred;    
    }
    public void SelectThreeHundredPot()
    {
        GameManager.instance.potSelected = GameManager.Pots.ThreeHundred;    
    }
    public void SelectFourHundredPot()
    {
        GameManager.instance.potSelected = GameManager.Pots.FourHundred;    
    }public void SelectFiveHundredPot()
    {
        GameManager.instance.potSelected = GameManager.Pots.FiveHundred;    
    }
    
    public void LeftBtnPressed()
    {
        GameManager.instance.leftBtnPressed = true;
        Debug.Log("Left btn pressed");
    }
    public void LeftBtnReleased()
    {
        GameManager.instance.leftBtnPressed = false;
        Debug.Log("Left btn released");
    }
    public void RightBtnPressed()
    {
        GameManager.instance.rightButtonPressed = true;
        Debug.Log("Right btn pressed");
    }
    public void RightBtnReleased()
    {
        GameManager.instance.rightButtonPressed = false;
        Debug.Log("Right btn released");
    }
    public void ForwardBtnPressed()
    {
        GameManager.instance.forwardButtonPressed = true;
        Debug.Log("Forward btn pressed");
    }
    public void ForwardBtnReleased()
    {
        GameManager.instance.forwardButtonPressed = false;
        Debug.Log("Forward btn released");
    }
    public void BackwardBtnPressed()
    {
        GameManager.instance.backwardButtonPressed = true;
        Debug.Log("Backward btn pressed");
    }
    public void BackwardBtnReleased()
    {
        GameManager.instance.backwardButtonPressed = false;
        Debug.Log("Backward btn released");
    }
}
