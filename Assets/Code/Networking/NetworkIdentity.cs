using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class NetworkIdentity : MonoBehaviour
{
  [Header("Helpful values")]
  [SerializeField]
  private string id;
  [SerializeField]
  private bool isControlling;

  private SocketIOComponent socket;
  private string userName = "";
  private PlayerColor playerColor = new PlayerColor();

    // Start is called before the first frame update
    void Awake()
    {
        isControlling = false;
    }

    // Update is called once per frame
    public void SetControllerID(string ID)
    {
        id = ID;
        isControlling = NetworkClient.clientID == ID ? true : false; //Check the incoming id vs the one have saved from the server
    }

    public void SetPlayerName(string name)
    {
        userName = name;
        Debug.Log("Name of the player :: "+userName);
    }

    public void SetTheSocketReference(SocketIOComponent _socket)
    {
        socket = _socket;
    }

    public void SetPlayerColor(PlayerColor color)
    {
        playerColor = color;
    }
    public PlayerColor GetPlayerColor()
    {
        return playerColor;
    }


    public string GetName()
    {
        return userName;
    }

    public string GetID()
    {
        return id;
    }

    public bool IsControlling()
    {
        return isControlling;
    }

    public SocketIOComponent GetSocket()
    {
        return socket;
    }

    public int GetRandomNumber(int min,int max)
    {
        return Random.Range(min,max);
    }
}
