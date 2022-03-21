using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform objectToFollow = null;
    
    private Vector3 currentPos;
    private Vector3 previousPos;

    private float zOffSet = 3.0f;
    private float yOffSet = 1.5f;


    private void Start() {
        // currentPos = transform.position;
    }

    private void LateUpdate() {
        // if(objectToFollow != null)
        // {
        //     currentPos = objectToFollow.transform.position;
        //     currentPos.z = currentPos.z + zOffSet;
        //     currentPos.y = currentPos.y + yOffSet;
        //     currentPos.x = Mathf.Lerp(previousPos.x,currentPos.x,0.8f*Time.deltaTime);
        //     currentPos.y = Mathf.Lerp(previousPos.y,currentPos.y,0.8f*Time.deltaTime);
        //     currentPos.z = Mathf.Lerp(previousPos.z,currentPos.z,0.8f*Time.deltaTime);
        //     this.transform.position = currentPos;
        //     this.transform.LookAt(objectToFollow);
        //     previousPos = currentPos;
        // }
    }
}
