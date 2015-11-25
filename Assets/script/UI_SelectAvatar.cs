using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using KBEngine;
using System;
using System.Collections.Generic;

public class UI_SelectAvatar : MonoBehaviour {
    public Button bt_createAvatar;
    public Button bt_removeAvatar;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void onCreateAvatar()
    {
        Application.LoadLevel("createAvatar");
    }

    public void onRemoveAvatar()
    {
        
    }
}
