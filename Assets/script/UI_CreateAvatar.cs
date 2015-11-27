using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using KBEngine;
using System;
using System.Collections.Generic;

public class UI_CreateAvatar : MonoBehaviour {
    public InputField if_createAvatarName;
    public Dropdown dd_createAvatarType;
    public Button bt_createAvatar;
	// Use this for initialization
	void Start () {
        KBEngine.Event.registerOut("onCreateAvatarResult", this, "onCreateAvatarResult");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void onCreateAvatar()
    {
        Account account = (Account)KBEngineApp.app.player();
        if (account != null)
        {
            byte roleType = 0;
            if(dd_createAvatarType.value == 0)
                roleType = 0;
            else
                roleType = 1;
            account.reqCreateAvatar(if_createAvatarName.text, roleType);
        }
    }

    public void onCreateAvatarResult(Byte retcode, object info, Dictionary<UInt64, Dictionary<string, object>> avatarList)
    {
        if (retcode != 0)
        {
            print("创建失败！" + retcode);
            return;
        }
        Application.LoadLevel("selectAvatar");
        //onReqAvatarList(avatarList);

    }
}
