using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterScene : MonoBehaviour
{
    public string lastExitName;

    void Start(){
        if (PlayerPrefs.GetString("LastExitName") == lastExitName ){
          PlayerManager.Instance.transform.position = transform.position;  
        }
    }
}
