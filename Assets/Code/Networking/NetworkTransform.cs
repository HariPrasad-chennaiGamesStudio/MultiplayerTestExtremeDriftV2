using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


[RequireComponent(typeof(NetworkIdentity))]
public class NetworkTransform : MonoBehaviour
{
    [SerializeField]
    private Vector3 oldPostion;
    private NetworkIdentity networkIdentity;
    Details details = new Details();
    public float stillCounter = 0;

    // Start is called before the first frame update
    void Start()
    {
        networkIdentity = GetComponent<NetworkIdentity>();
        oldPostion = transform.position;
        
        details.pos = new Positions();
        details.rot = new Positions();
        // playerDetails.pos.x = 0.0f;
        // playerDetails.pos.y = 0.0f;
        // playerDetails.pos.z = 0.0f;
        TextMeshPro playerId = GetComponentInChildren<TextMeshPro>();
        playerId.text = networkIdentity.GetID();

        lastSyncTime = Time.time;

        // playerId.text = networkIdentity.GetName();
        if(!networkIdentity.IsControlling())
        {
            enabled = false;
        }else
        {
            GameObject.Find("Main Camera").GetComponent<VehicleCamera>().target = this.transform;
        }
    }

    private int tempSendCount = 0;
    private float lastSyncTime = 0.0f;
    private float delayTime = 0.0f;

    // Update is called once per frame
    void Update()
    {
        if(networkIdentity.IsControlling())
        {
            /*
            if(oldPostion != transform.position)
            {
                oldPostion = transform.position;
                stillCounter = 0;
                SendData();

            }else
            {
                stillCounter += Time.deltaTime;

                if(stillCounter >= 0)
                {
                    SendData();
                    tempSendCount += 1;
                    // Debug.Log("Player data is pushing! :: "+Time.time +", Temp count :: "+tempSendCount);
                }
            }
            */
            if(lastSyncTime + 0.05f + delayTime < Time.time)
            {
                lastSyncTime = Time.time;
             //   delayTime = Random.Range (0.2f, 1.0f);
                SendData();
            }




            //Send the typed msg...
            if(GameManager.instance.canSendTypedText)
            {
                GameManager.instance.canSendTypedText = false;
                Player messagingPlayer = new Player();
                messagingPlayer.username = GameManager.instance.playerName;
                Debug.Log("Sending Message :: "+GameManager.instance.sendingMsg);
                messagingPlayer.text = GameManager.instance.sendingMsg;
                Debug.Log("Sending text :: "+JsonUtility.ToJson(messagingPlayer));
                networkIdentity.GetSocket().Emit("updateText",new JSONObject(JsonUtility.ToJson(messagingPlayer)));
            }
        }
    }
    private void SendData()
    {

        
        // Debug.Log("details :: "+details.rot);
        details.pos.x = Mathf.Round(transform.position.x * 1000.0f)/ 1000.0f;
        details.pos.y = Mathf.Round(transform.position.y * 1000.0f)/ 1000.0f;
        details.pos.z = Mathf.Round(transform.position.z * 1000.0f)/ 1000.0f;

        details.rot.x = Mathf.Round(transform.eulerAngles.x * 1000.0f)/ 1000.0f;
        details.rot.y = Mathf.Round(transform.eulerAngles.y * 1000.0f)/ 1000.0f;
        details.rot.z = Mathf.Round(transform.eulerAngles.z * 1000.0f)/ 1000.0f;

        // Debug.Log("Player Data :: "+JsonUtility.ToJson(playerPosition));
        networkIdentity.GetSocket().Emit("updatePosition",new JSONObject(JsonUtility.ToJson(details)));
    }
}
