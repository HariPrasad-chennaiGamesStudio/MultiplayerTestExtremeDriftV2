// using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using UnityEngine.UI;
using UnityEngine.Networking;

using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using System;
using Newtonsoft.Json;
using System.Text;

public class NetworkClient : SocketIOComponent
{
    // ws://127.0.0.1:52300/socket.io/?EIO=4&transport=websocket
    // ws://54.158.111.29:52300/socket.io/?EIO=4&transport=websocket
    //ws://192.168.0.105:52301/socket.io/?EIO=4&transport=websocket
    //ws://testinggameserver.herokuapp.com:80/socket.io/?EIO=4&transport=websocket
    //ws://192.168.1.4:52300/socket.io/?EIO=4&transport=websocket
    //ws://54.158.111.29:52400/socket.io/?EIO=4&transport=websocket

    //HyperRacer...
    //ws://3.110.7.51:52400/socket.io/?EIO=4&transport=websocket

    //https://testinggameserver.herokuapp.com/

    [Header("Network Client")]
    [SerializeField]

    public static NetworkClient instance = null;
    private Transform networkContainer;

    public Dictionary<string, NetworkIdentity> serverObjects;
    //public Dictionary<string, VehicleControl> _vc;
    private Dictionary<string,MultiplayerPlayersPlacement> otherPlayersPlacement;
    private Dictionary<string,float> hardCodeDelayTime;
    //public Dictionary<string,float> otherPlayersPreviousPosX;
    //public Dictionary<string,float> otherPlayersPreviousPosY;
    //public Dictionary<string,float> otherPlayersPreviousPosZ;




    public GameObject playerObject_GO;

    public static string clientID {get; private set;}

    public Text id_Text;

    public string ip = "127.0.0.1";
	public string port = "52300";

    public InputField ip_field;
    public InputField port_field;

    public GameObject connectPanel;
    public GameObject playerControlPanel;

    public  InputField typeMsgToSend_InputFeild;

    public InputField playerName_Input;
    public Text received_Text;

    private Color[] colors_Array = new Color[]{Color.black,Color.blue,Color.grey,Color.green,Color.magenta,Color.white,Color.yellow,Color.cyan};
    private int colorIndex = 0;
    private string currentSceneName = "Garage";//Landing,Garage,Game
    public string gaurdianId = "1234";
    private void Awake() {
        base.Awake();
        if(instance == null)
        {
            instance = this;
        }    
    }
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        Initialize();
        SetUpEvents();

        ip_field.text = ip;
        port_field.text = port;
        // playerControlPanel.SetActive(false);
        connectPanel.SetActive(true);
        GameManager.instance.currentScene = GameManager.Scenes.Garage;

        p = new MultiplayerPlayersPlacement();
        p.p = new Positions();
        p.r = new Positions();
    }

    private string BytesToString(byte[] bytes)
    {
        return Encoding.UTF8.GetString(bytes);
    }
    public void UpdateIPField()
    {
        ip = ip_field.text;
    }
    public void UpdatePortField()
    {
        port = port_field.text;
    }


    public void ConnectToTheServer()
    {
        if(!IsConnected)
        {
            Debug.Log("Port :: "+port);
            // Debug.Log("IP :: "+ip);
            // string url = "ws://"+ip+":"+port+"/socket.io/?EIO=4&transport=websocket";
            // base.url = url;
            base.Connect();
            // playerControlPanel.SetActive(true);
            connectPanel.SetActive(false);
            otherPlayersSpawning = false;
        }
    }

    private void Initialize()
    {
        serverObjects = new Dictionary<string, NetworkIdentity>();
        //_vc = new Dictionary<string, VehicleControl>();
        otherPlayersPlacement = new Dictionary<string, MultiplayerPlayersPlacement>();
        hardCodeDelayTime = new Dictionary<string, float>();
        //otherPlayersPreviousPosX = new Dictionary<string, float>();
        //otherPlayersPreviousPosY = new Dictionary<string, float>();
        //otherPlayersPreviousPosZ = new Dictionary<string, float>();
    }

    private Vector3 tempVector3;
    private Quaternion _tempQuaternion;
    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        foreach (var player in otherPlayersPlacement)
        {
            if (player.Key != clientID)
            {
                if (hardCodeDelayTime[player.Key] > 3f)
                {
                    float moveTime = 0.5f;
                    tempVector3 = serverObjects[player.Key].transform.position;
                    float speed = Math.Abs(tempVector3.x - otherPlayersPlacement[player.Key].p.x) / moveTime;
                    tempVector3.x = Mathf.Lerp(tempVector3.x, otherPlayersPlacement[player.Key].p.x, Time.deltaTime * speed);
                    speed = Math.Abs(tempVector3.y - otherPlayersPlacement[player.Key].p.y) / moveTime;
                    tempVector3.y = Mathf.Lerp(tempVector3.y, otherPlayersPlacement[player.Key].p.y, Time.deltaTime * speed);
                    speed = Math.Abs(tempVector3.z - otherPlayersPlacement[player.Key].p.z) / moveTime;
                    tempVector3.z = Mathf.Lerp(tempVector3.z, otherPlayersPlacement[player.Key].p.z, Time.deltaTime * speed);
                    serverObjects[player.Key].transform.position = tempVector3;

                    //tempVector3 = serverObjects[player.Key].transform.eulerAngles;

                    //tempVector3.x = Mathf.Lerp(tempVector3.x, otherPlayersPlacement[player.Key].r.x, 0.2f);
                    //tempVector3.y = Mathf.Lerp(tempVector3.y, otherPlayersPlacement[player.Key].r.y, 0.2f);
                    //tempVector3.z = Mathf.Lerp(tempVector3.z, otherPlayersPlacement[player.Key].r.z, 0.2f);
                    //serverObjects[player.Key].transform.eulerAngles = tempVector3;

                    tempVector3.x = otherPlayersPlacement[player.Key].r.x;
                    tempVector3.y = otherPlayersPlacement[player.Key].r.y;
                    tempVector3.z = otherPlayersPlacement[player.Key].r.z;
                    ////serverObjects[player.Key].transform.eulerAngles = tempVector3;
                    //serverObjects[player.Key].transform.eulerAngles = Vector3.Slerp(serverObjects[player.Key].transform.eulerAngles, tempVector3, Time.deltaTime * speed);

                    _tempQuaternion = Quaternion.Euler(tempVector3);
                    serverObjects[player.Key].transform.rotation = Quaternion.Lerp(serverObjects[player.Key].transform.rotation, _tempQuaternion, Time.deltaTime * speed);

                    //tempVector3.x = otherPlayersPlacement[player.Key].p.x;
                    //tempVector3.y = otherPlayersPlacement[player.Key].p.y;
                    //tempVector3.z = otherPlayersPlacement[player.Key].p.z;
                    //Debug.Log(tempVector3);
                    //serverObjects[player.Key].transform.position = tempVector3;

                    //serverObjects[player.Key].transform.eulerAngles = otherPlayersPlacement[player.Key].r;
                }
                else
                {
                    hardCodeDelayTime[player.Key] += Time.deltaTime;
                    otherPlayersPlacement[player.Key].p.x = serverObjects[player.Key].transform.position.x;
                    otherPlayersPlacement[player.Key].p.y = serverObjects[player.Key].transform.position.y;
                    otherPlayersPlacement[player.Key].p.z = serverObjects[player.Key].transform.position.z;
                }
            }
        }

        // if(Input.GetKeyDown(KeyCode.G))
        // {
        //     StartCoroutine(TestWebREquest());
        // }

        //For testing purpose only...
        //ChangeTheSceneName();
        //DisconnectFromTheServer();
        //TestingConnectToTheServer();
        //CheckConnectionStatus();
    }

    private bool garagScene = false;
    //For testing only....
    private void ChangeTheSceneName()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            if(GameManager.instance.currentScene == GameManager.Scenes.Garage)
            {
                Debug.Log("Current scene is Game!");
                GameManager.instance.currentScene = GameManager.Scenes.GamePlay;
            }else if(GameManager.instance.currentScene == GameManager.Scenes.GamePlay)
            {
                Debug.Log("Current scene is Game!");
                GameManager.instance.currentScene = GameManager.Scenes.Garage;
            }
        }
    }

    private void DisconnectFromTheServer()
    {
        if(Input.GetKeyDown(KeyCode.D) && IsConnected)
        {
            Debug.Log("Player disconnect!");
            // Debug.Break();
            base.Close();
            otherPlayersSpawning = false;
        }
    }

    private void TestingConnectToTheServer()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Player testing connect!");
            ConnectToTheServer();
        }
    }
    private void CheckConnectionStatus()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            // gaurdianId = gaurdianId+Random.Range(0,20);
            Debug.Log("Connection status :: "+IsConnected);
        }
    }

    private IEnumerator TestWebREquest()
    {
        WWWForm form = new WWWForm();
        form.AddField("myField", "myData");

        using (UnityWebRequest www = UnityWebRequest.Post("http://192.68.14.138/test.php", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form test complete! "+Encoding.UTF8.GetString(www.downloadHandler.data));
            }
        }
    }
    private bool openedConnection = false;
    private float spawnedXPos = 0.0f;
    private float spawnedYPos = 0.0f;
    private float spawnedZPos = 0.0f;
    public bool otherPlayersSpawning = false;
    private void SetUpEvents()
    {
        On("open", (E) => {
            Debug.Log("Connetion made to the server! :: "+openedConnection);
            if(openedConnection == false)
            {
                openedConnection = true;
                Get_Id getId = new Get_Id();
                getId.c_scene = GameManager.instance.currentScene.ToString();
                getId.g_id = playerName_Input.text;
                // getId.g_id = gaurdianId+(Random.Range(0,20));
                this.Emit("get_id",new JSONObject(JsonUtility.ToJson(getId)));
                // Debug.LogError("getId.c_scene :: "+getId.c_scene);
            }
            else
            {
                openedConnection = false;
            }
        });

        On("register", (E) => {
            clientID = E.data["i"].ToString();
            clientID = RemoveQuotes(clientID);
            id_Text.text = "Your id : "+clientID;
            // id_Text.text = "Your id : "+clientID +", members in room : "+RemoveQuotes(E.data["numofplayers"].ToString());

            Debug.LogFormat("Our Client's ID ({0})", clientID);
            JoinGame joinGameData = new JoinGame();
            joinGameData.name = playerName_Input.text;
            this.Emit("join",new JSONObject(JsonUtility.ToJson(joinGameData)));

            RabbitMQController.instance.UpdateQueueName(clientID);
        });
        
        On("connect_error", (err) => {
            Debug.LogError("connect_error due to "+err.ToString());
        });

        On("reconnect",(E) => {
            Debug.Log("Reconnect :: "+E.data.ToString());
        });

        On("garageReconnect",(E) => {
            Debug.Log("garageReconnect :: "+E.data.ToString());
        });
        
        On("spawn", (E) => {
            //Handling all spawning all players
            //Passed Data
            Debug.Log("E.Data :: "+E.data);
            string id = E.data["i"].ToString();
            string userName = E.data["name"].ToString();
            //spawnedXPos = E.data["x"].f;
            //spawnedYPos = E.data["y"].f;
            //spawnedZPos = E.data["z"].f;
            userName = RemoveQuotes(userName);
            id = RemoveQuotes(id);

            string index = RemoveQuotes(E.data["index"].ToString());

            Debug.Log("players orderindex == " + index);
            if (clientID == id)
            {
                //otherPlayersSpawning = false;
                SpawnThePlayers(id, userName, int.Parse(index));
            }else
            {
                //otherPlayersSpawning = true;
                StartCoroutine(SpawnPlayersDelayed(id, userName, int.Parse(index)));
            }


            // if(ni.IsControlling())
            // {
            /*PlayerColor color = new PlayerColor();

            colorIndex = ni.GetRandomNumber(0,colors_Array.Length);
            go.GetComponent<MeshRenderer>().material.color = colors_Array[colorIndex];
            Color matColor = colors_Array[colorIndex];
            color.r = matColor.r;
            color.g = matColor.g;
            color.b = matColor.b;
            color.a = matColor.a;

            ni.SetPlayerColor(color);

            Player player = new Player();
            player.id = id;
            player.username = GameManager.instance.playerName;
            player.color = ni.GetPlayerColor();
            // player.color.g = ni.GetPlayerColor().g;
            // player.color.b = ni.GetPlayerColor().b;
            // player.color.a = ni.GetPlayerColor().a;
            ni.SetPlayerName(userName);
            // TextMeshPro playerId = go.GetComponentInChildren<TextMeshPro>();
            // playerId.text = userName;
            Debug.Log("adduser :: "+JsonUtility.ToJson(player));
            ni.GetSocket().Emit("addUser",new JSONObject(JsonUtility.ToJson(player)));*/
            // Debug.Break();
            // }
            
            if (clientID == id)
            {
                RabbitMQController.instance.UpdateExchangeKeyAndCreateQueue(RemoveQuotes(E.data["roomid"].ToString()));
                //playerControlPanel.SetActive(true);
            }
        });

        On("applycolor", (E) => {
            string id = E.data["id"].ToString();
            string userName = E.data["username"].ToString();
            id = RemoveQuotes(id);
            Debug.Log("Color changed :: "+E.data["color"]);
            serverObjects[id].GetComponent<MeshRenderer>().material.color = new Color(E.data["color"]["r"].f,E.data["color"]["g"].f,E.data["color"]["b"].f,E.data["color"]["a"].f);
            // id_Text.text = "Your id : "+clientID +", members in room : "+RemoveQuotes(E.data["numberofplayers"].ToString());
        });

        On("disconnected", (E) => {

            Debug.Log("disconnected -- " + E.data.ToString());
            string id = E.data["id"].ToString();
            id = RemoveQuotes(id);

            GameObject go = serverObjects[id].gameObject;
            Destroy(go); //Remove from game
            serverObjects.Remove(id); //Remove from memory
            //_vc.Remove(id); //Remove from memory
            otherPlayersPlacement.Remove(id);
            hardCodeDelayTime.Remove(id);
            //otherPlayersPreviousPosX.Remove(id);
            //otherPlayersPreviousPosY.Remove(id);
            //otherPlayersPreviousPosZ.Remove(id);
        });

        /*On("updatePosition", (F) => {
            //Debug.Log("F.Data :: "+F.data);
            string id = F.data["id"].ToString();
            id = RemoveQuotes(id);
            // float x = F.data["position"]["x"].f;
            // float y = F.data["position"]["y"].f;
            // float z = F.data["position"]["z"].f;



            ////-----------------------------------------------
            //float x = F.data["pos"]["x"].f;
            //float y = F.data["pos"]["y"].f;
            //float z = F.data["pos"]["z"].f;

            //float rX = F.data["rot"]["x"].f;
            //float rY = F.data["rot"]["y"].f;
            //float rZ = F.data["rot"]["z"].f;
            // Debug.Log("x :: "+x);

            // Debug.Log("x ::: "+x+" , z :: "+z);

            //NetworkIdentity ni = serverObjects[id];
            //// x = Mathf.Lerp(otherPlayersPreviousPosX[id],x,0.4f);
            //// y = Mathf.Lerp(otherPlayersPreviousPosY[id],y,0.4f);
            //// z = Mathf.Lerp(otherPlayersPreviousPosZ[id],z,0.4f);
            //ni.transform.position = new Vector3(x,y,z);
            //ni.transform.eulerAngles = new Vector3(rX,rY,rZ);
            //otherPlayersPreviousPosX[id] = x;
            //otherPlayersPreviousPosY[id] = y;
            //otherPlayersPreviousPosZ[id] = z;
            ////---------------------------------------------
            ///

            //Debug.Log("id == " + id);
            //_vc[id].GetMultiplayerValues(F.data["data"].ToString());


            // tempSendCount += 1;
            // Debug.Log("Player data is pushing! :: "+Time.time +", Temp count :: "+tempSendCount);

            //UpdatePosition(id, F.data["data"].ToString());
        });
        */

        On("updateText",(T) =>
        {
            string id = T.data["id"].ToString();
            id = RemoveQuotes(id);
            string playerName = RemoveQuotes(T.data["username"].ToString());
            string receivedMsg = RemoveQuotes(T.data["text"].ToString());
            received_Text.text = received_Text.text +"\nMsg from : "+playerName+", Msg : "+receivedMsg;
            Debug.Log("Msg received from :: "+playerName+" Msg :: "+receivedMsg);
        });

    }

    public void UpdatePosition(string id, MultiplayerPlayersPlacement _data)
    {
        if(id != clientID && otherPlayersPlacement.ContainsKey(id))
        {
            Debug.Log("Consumed Message = " + _data.p.x + " " + _data.p.y + _data.p.z);
            //otherPlayersPlacement[id] = JsonConvert.DeserializeObject<MultiplayerPlayersPlacement>(_json);
            otherPlayersPlacement[id] = _data;
            _data = null;
        }
    }

    public void UpdatePosition(string[] _data)
    {
        
        if (_data[0] != clientID && otherPlayersPlacement.ContainsKey(_data[0]))
        {
            Debug.Log("Consumed Message = " + _data[0] + " " + _data[1] + " " + _data[2]);
            //otherPlayersPlacement[id] = JsonConvert.DeserializeObject<MultiplayerPlayersPlacement>(_json);


            //p.id = _data[0];

            //var _data2 = _data[1].Split(',');

            //p.p.x = float.Parse(_data2[0]);
            //p.p.y = float.Parse(_data2[1]);
            //p.p.z = float.Parse(_data2[2]);

            //Debug.Log("Consumed Message POS = " + _data2[0] + " " + _data2[1] + " " + _data2[2]);

            //_data2 = _data[2].Split(',');

            //p.r.x = float.Parse(_data2[0]);
            //p.r.y = float.Parse(_data2[1]);
            //p.r.z = float.Parse(_data2[2]);

            //Debug.Log("Consumed Message ROT = " + _data2[0] + " " + _data2[1] + " " + _data2[2]);

            var pData = otherPlayersPlacement[_data[0]];

            var _data2 = _data[1].Split(',');
            pData.p.x = float.Parse(_data2[0]);
            pData.p.y = float.Parse(_data2[1]);
            pData.p.z = float.Parse(_data2[2]);

            _data2 = null;

            _data2 = _data[2].Split(',');
            pData.r.x = float.Parse(_data2[0]);
            pData.r.y = float.Parse(_data2[1]);
            pData.r.z = float.Parse(_data2[2]);

            Debug.Log(otherPlayersPlacement[_data[0]].p.x + " -- " + otherPlayersPlacement[_data[0]].p.y + " -- " + otherPlayersPlacement[_data[0]].p.z);

            _data = null;
            _data2 = null;
        }
    }

    private MultiplayerPlayersPlacement p;
    private StringBuilder _sb = new StringBuilder();
    public void _sendJson()
    {
        p.id = clientID;
        tempVector3 = serverObjects[clientID].transform.position;

        p.p.x = (int)(tempVector3.x * 1000f) / 1000f;
        p.p.y = (int)(tempVector3.y * 1000f) / 1000f;
        p.p.z = (int)(tempVector3.z * 1000f) / 1000f;

        tempVector3 = serverObjects[clientID].transform.eulerAngles;
        p.r.x = (int)(tempVector3.x * 1000f) / 1000f;
        p.r.y = (int)(tempVector3.y * 1000f) / 1000f;
        p.r.z = (int)(tempVector3.z * 1000f) / 1000f;

        //RabbitMQController.instance.SendMessageInQueue(p);

        _sb.Clear();
        _sb.Append(p.id);
        _sb.Append('|');
        _sb.Append(p.p.x);
        _sb.Append(',');
        _sb.Append(p.p.y);
        _sb.Append(',');
        _sb.Append(p.p.z);
        _sb.Append('|');
        _sb.Append(p.r.x);
        _sb.Append(',');
        _sb.Append(p.r.y);
        _sb.Append(',');
        _sb.Append(p.r.z);
        RabbitMQController.instance.SendMessageInQueue(_sb.ToString());
    }

    private int tempSendCount = 0;
    private string sendMessageText;
    public void UpdateTypedMessage()
    {
        sendMessageText = typeMsgToSend_InputFeild.text;
        // Debug.Log("Typing message :: "+GameManager.instance.sendingMsg);
    }

    public void SendTypedMsg()
    {
        GameManager.instance.canSendTypedText = true;
        GameManager.instance.sendingMsg = sendMessageText;
        // Debug.Log("Sending msg :: "+GameManager.instance.sendingMsg);
        received_Text.text = received_Text.text +"\nMe : "+GameManager.instance.playerName+", Msg : "+GameManager.instance.sendingMsg;
        typeMsgToSend_InputFeild.text = "";

        if(garagScene == false)
        {
            Debug.Log("Current scene is Garage!");
            currentSceneName = "Garage";
            garagScene = true;
        }else if(garagScene == true)
        {
            Debug.Log("Current scene is Game!");
            currentSceneName = "Game";
            garagScene = false;
        }
    }

    public void InputPlayerName()
    {
        GameManager.instance.playerName = playerName_Input.text;
        Debug.Log("Player name :: "+GameManager.instance.playerName);
    }

    //[ContextMenu("TEST")]
    //private void Test()
    //{
    //    for (int i = 0; i < 100; i++)
    //    {
    //        tempVector3.x = -140f + ((i % 5) * 5);
    //        tempVector3.y = 0f;
    //        tempVector3.z = 0f + ((i / 5) * 5);

    //        Debug.Log(i + " -- " + tempVector3);
    //    }
    //}

    private void SpawnThePlayers(string id, string userName, int index)
    {
        if (clientID == id)
        {
            otherPlayersSpawning = false;
        }
        else
        {
            otherPlayersSpawning = true;
        }
        Debug.Log("SpawnThePlayers -- " + id);

        tempVector3.x = -140f + ((index % 5) * 5);
        tempVector3.y = 0f;
        tempVector3.z = 0f + ((index / 5) * 5);

        // GameObject go = new GameObject("Server ID : "+ id);
        //GameObject go = Instantiate(playerObject_GO, new Vector3(spawnedXPos,spawnedYPos,spawnedZPos),Quaternion.identity);
        GameObject go = Instantiate(playerObject_GO, new Vector3(tempVector3.x, tempVector3.y, tempVector3.z),Quaternion.identity);
        // spawnedXPos += 10.0f; 
        Debug.Log("Server spawned objects x position :: "+spawnedXPos+" , "+Time.time);
        go.name = string.Format("Player ({0})",id);//id);//"Server ID : "+ id;
        // go.GetComponent<MeshRenderer>().material.color = Color.
        go.transform.SetParent(networkContainer);
        NetworkIdentity ni = go.GetComponent<NetworkIdentity>();
        ni.SetControllerID(id);
        ni.SetPlayerName(userName);
        ni.SetTheSocketReference(this);
        
        
        serverObjects.Add(id, ni);
        //_vc.Add(id, ni.GetComponent<VehicleControl>());

        var placement = new MultiplayerPlayersPlacement();
        placement.id = id;
        placement.p = new Positions();
        placement.r = new Positions();

        tempVector3 = ni.transform.position;

        placement.p.x = tempVector3.x;
        placement.p.y = tempVector3.y;
        placement.p.z = tempVector3.z;


        tempVector3 = ni.transform.eulerAngles;

        placement.r.x = tempVector3.x;
        placement.r.y = tempVector3.y;
        placement.r.z = tempVector3.z;
        hardCodeDelayTime.Add(id, 0f);
        otherPlayersPlacement.Add(id, placement);

        //otherPlayersPreviousPosX.Add(id,ni.transform.position.x);
        //otherPlayersPreviousPosY.Add(id,ni.transform.position.y);
        //otherPlayersPreviousPosZ.Add(id,ni.transform.position.z);
    }

    private IEnumerator SpawnPlayersDelayed(string id, string userName, int index)
    {
        yield return new WaitForSeconds(3.0f);
        SpawnThePlayers(id, userName, index);
    }

    
    private string RemoveQuotes(string idString)
    {
       idString = idString.Replace('"', ' ').Trim();
       return idString;
    }
}

[System.Serializable]
public class MultiplayerPlayersPlacement
{
    public string id;
    public Positions p;
    public Positions r;
}