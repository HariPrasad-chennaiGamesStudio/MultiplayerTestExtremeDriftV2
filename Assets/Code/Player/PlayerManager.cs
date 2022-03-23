using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    [Header("Data")]
//    [SerializeField]
    private float speed = 40.0f;

    [Header("Class Reference")]
    [SerializeField]
    private NetworkIdentity networkIdentity;

    private Material playerMat;
    private string thisPlayerNetworkId;//Cache the Player's client id

    private CameraController cameraController;
    // private Color[] colors_Array = new Color[]{Color.black,Color.blue,Color.grey,Color.green,Color.magenta,Color.white,Color.yellow,Color.cyan};
    // private int colorIndex = 0;


    private void Start() {
        if(networkIdentity.IsControlling())
        {
            // cameraController = GameObject.Find("Main Camera").GetComponent<CameraController>();
            // cameraController.objectToFollow = this.transform;
            // playerMat = GetComponent<MeshRenderer>().material;
            // colorIndex = Random.Range(0,colors_Array.Length);
            // playerMat.color = colors_Array[colorIndex];

        }
        thisPlayerNetworkId = networkIdentity.GetID();
        Debug.Log("thisPlayerNetworkId :: "+thisPlayerNetworkId);
    }

    // Update is called once per frame
    void Update()
    {
        if(networkIdentity.IsControlling())
        {
            CheckMovementForUser(); //For User input
        }else
        {
            UpdateMovementForNetworkOpponent(); //For Opponents network data
        }
    }

    float x,y,z = 0.0f;

    private void UpdateMovementForNetworkOpponent()
    {
        // x = Mathf.Lerp(otherPlayersPreviousPosX[id],x,0.4f);
        // y = Mathf.Lerp(otherPlayersPreviousPosY[id],y,0.4f);
        // z = Mathf.Lerp(otherPlayersPreviousPosZ[id],z,0.4f);
        // ni.transform.position = new Vector3(x,y,z);

        // transform.position = new Vector3 (NetworkClient.instance.otherPlayersPreviousPosX[thisPlayerNetworkId], 
        // NetworkClient.instance.otherPlayersPreviousPosY[thisPlayerNetworkId],
        // NetworkClient.instance.otherPlayersPreviousPosZ[thisPlayerNetworkId]);

        //x = Mathf.Lerp(transform.position.x, NetworkClient.instance.otherPlayersPreviousPosX[thisPlayerNetworkId], 0.1f);
        //y = Mathf.Lerp(transform.position.y, NetworkClient.instance.otherPlayersPreviousPosY[thisPlayerNetworkId], 0.1f);
        //z = Mathf.Lerp(transform.position.z, NetworkClient.instance.otherPlayersPreviousPosZ[thisPlayerNetworkId], 0.1f);
        //transform.position = new Vector3(x,y,z);
    }


    
    float horizontal = 0.0f;
    float vertical = 0.0f;
    float previousH = 0.0f;
    float previousV = 0.0f;

    private void CheckMovementForUser()
    {
        // #if UNITY_ANDROID
        //     horizontal = Input.acceleration.x;
        //     vertical = Input.acceleration.z;
        //     horizontal = Mathf.Lerp(previousH,horizontal,0.4f*Time.deltaTime);
        //     vertical = Mathf.Lerp(previousV,vertical,0.4f*Time.deltaTime);
        //     previousV = vertical;
        //     previousH = horizontal;
        //     // Debug.Log("Looking for this");
        // #endif

        

        

        if(GameManager.instance.leftBtnPressed == false && GameManager.instance.rightButtonPressed == false)
        {
            horizontal = 0.0f;
        }
        if(GameManager.instance.forwardButtonPressed == false && GameManager.instance.backwardButtonPressed == false)
        {
            vertical = 0.0f;
        }

        if(GameManager.instance.leftBtnPressed)
        {
            horizontal = -1.0f;
        }
        if(GameManager.instance.rightButtonPressed)
        {
            horizontal = 1.0f;
        }
        if(GameManager.instance.forwardButtonPressed)
        {
            vertical = 1.0f;
        }
        if(GameManager.instance.backwardButtonPressed)
        {
            vertical = -1.0f;
        }

        #if UNITY_EDITOR || UNITY_STANDALONE
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

        #endif
        transform.position += new Vector3(horizontal,0.0f,vertical) * speed * Time.deltaTime;
    }
}
