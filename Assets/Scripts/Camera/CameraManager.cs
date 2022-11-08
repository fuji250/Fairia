
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject wallObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerManager.gameState == (int)PlayerManager.State.Playing)
        {
            return;
        }
        wallObject.SetActive(false);
    }
}
