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
    public Dictionary<string, VehicleControl> _vc;
    private Dictionary<string,Placement> otherPlayersPlacement;
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
        _vc = new Dictionary<string, VehicleControl>();
        otherPlayersPlacement = new Dictionary<string, Placement>();
        //otherPlayersPreviousPosX = new Dictionary<string, float>();
        //otherPlayersPreviousPosY = new Dictionary<string, float>();
        //otherPlayersPreviousPosZ = new Dictionary<string, float>();
    }

    private Vector3 tempVector3;
    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        foreach(var player in serverObjects)
        {
            if(player.Key != clientID)
            {
                //tempVector3 = transform.position;

                //float speed = Math.Abs(tempVector3.x - otherPlayersPlacement[player.Key].p.x) / 0.1f;
                //tempVector3.x = Mathf.Lerp(tempVector3.x, otherPlayersPlacement[player.Key].p.x, Time.deltaTime * speed);
                //speed = Math.Abs(tempVector3.y - otherPlayersPlacement[player.Key].p.y) / 0.1f;
                //tempVector3.y = Mathf.Lerp(tempVector3.y, otherPlayersPlacement[player.Key].p.y, Time.deltaTime * speed);
                //speed = Math.Abs(tempVector3.z - otherPlayersPlacement[player.Key].p.z) / 0.1f;
                //tempVector3.z = Mathf.Lerp(tempVector3.z, otherPlayersPlacement[player.Key].p.z, Time.deltaTime * speed);
                //transform.position = tempVector3;

                ////tempVector3 = transform.eulerAngles;
                ////tempVector3.x = Mathf.Lerp(tempVector3.x, otherPlayersPlacement[player.Key].r.x, Time.deltaTime);
                ////tempVector3.y = Mathf.Lerp(tempVector3.y, otherPlayersPlacement[player.Key].r.y, Time.deltaTime);
                ////tempVector3.z = Mathf.Lerp(tempVector3.z, otherPlayersPlacement[player.Key].r.z, Time.deltaTime);
                ////transform.eulerAngles = tempVector3;

                //transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, otherPlayersPlacement[player.Key].r, Time.deltaTime * speed);

                transform.position = otherPlayersPlacement[player.Key].p;
                transform.eulerAngles = otherPlayersPlacement[player.Key].r;
            }
        }

        // if(Input.GetKeyDown(KeyCode.G))
        // {
        //     StartCoroutine(TestWebREquest());
        // }

        //For testing purpose only...
        ChangeTheSceneName();
        DisconnectFromTheServer();
        TestingConnectToTheServer();
        CheckConnectionStatus();
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
            spawnedXPos = E.data["x"].f;
            spawnedYPos = E.data["y"].f;
            spawnedZPos = E.data["z"].f;
            userName = RemoveQuotes(userName);
            id = RemoveQuotes(id);
            if(clientID == id)
            {
                otherPlayersSpawning = false;
                SpawnThePlayers(id, userName);
            }else
            {
                otherPlayersSpawning = true;
                StartCoroutine(SpawnPlayersDelayed(id, userName));
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
            _vc.Remove(id); //Remove from memory
            otherPlayersPlacement.Remove(id);
            //otherPlayersPreviousPosX.Remove(id);
            //otherPlayersPreviousPosY.Remove(id);
            //otherPlayersPreviousPosZ.Remove(id);
        });

        On("updatePosition", (F) => {
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

            UpdatePosition(id, F.data["data"].ToString());
        });

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

    private struct Placement
    {
        public Vector3 p;
        public Vector3 r;
    }

    public void UpdatePosition(string id, string _json)
    {
        if(id != clientID && otherPlayersPlacement.ContainsKey(id))
        {
            Debug.Log("Received Json == " + _json);
            otherPlayersPlacement[id] = JsonUtility.FromJson<Placement>(_json);
        }
    }

    public string _sendJson()
    {
        var p = new Placement();
        p.p = serverObjects[clientID].transform.position;
        p.r = serverObjects[clientID].transform.eulerAngles;

        return JsonUtility.ToJson(p);
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

    private void SpawnThePlayers(string id, string userName)
    {
        Debug.Log("SpawnThePlayers -- " + id);
        // GameObject go = new GameObject("Server ID : "+ id);
        GameObject go = Instantiate(playerObject_GO, new Vector3(spawnedXPos,spawnedYPos,spawnedZPos),Quaternion.identity);
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
        _vc.Add(id, ni.GetComponent<VehicleControl>());
        var placement = new Placement();
        placement.p = ni.transform.position;
        placement.r = ni.transform.eulerAngles;
        otherPlayersPlacement.Add(id, placement);

        //otherPlayersPreviousPosX.Add(id,ni.transform.position.x);
        //otherPlayersPreviousPosY.Add(id,ni.transform.position.y);
        //otherPlayersPreviousPosZ.Add(id,ni.transform.position.z);
    }

    private IEnumerator SpawnPlayersDelayed(string id, string userName)
    {
        yield return new WaitForSeconds(3.0f);
        SpawnThePlayers(id, userName);
    }

    
    private string RemoveQuotes(string idString)
    {
       idString = idString.Replace('"', ' ').Trim();
       return idString;
    }
}
