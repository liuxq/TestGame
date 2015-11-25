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
    public void onCreateAvatarResult(byte success, object info)
    {
        if (success == 0)
        {
            Dictionary<string, object> avatorInfo = (Dictionary<string, object>)info;
            print("创建成功");
            print("角色数据库id:" + (UInt64)avatorInfo["dbid"]);
            print("角色名称:" + (string)avatorInfo["name"]);
            print("角色类型:" + (byte)avatorInfo["roleType"]);
            print("角色等级:" + (UInt16)avatorInfo["level"]);
        }
        else
            print("创建失败");
    }
}
