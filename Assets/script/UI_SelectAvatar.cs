using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using KBEngine;
using System;
using System.Collections.Generic;

public class UI_SelectAvatar : MonoBehaviour {
    public Button bt_createAvatar;
    public Button bt_removeAvatar;
    public Toggle[] tg_avatars = new Toggle[3];

    private Dictionary<UInt64, Dictionary<string, object>> ui_avatarList = null;
    private Dictionary<string, UInt64> dic_name_to_dbid = new Dictionary<string,ulong>();
	// Use this for initialization
	void Start () {
        World.instance.init();
        //DontDestroyOnLoad(gameObject.transform);
        KBEngine.Event.registerOut("onReqAvatarList", this, "onReqAvatarList");
        KBEngine.Event.registerOut("onRemoveAvatar", this, "onRemoveAvatar");

        Account account = (Account)KBEngineApp.app.player();
        if (account != null)
        {
            onReqAvatarList(account.avatars);
        }
        
	}
    void OnDestroy()
    {
        KBEngine.Event.deregisterOut(this);
    }
	// Update is called once per frame
	void Update () {
       
	}

    public void onCreateAvatar()
    {
        Application.LoadLevel("createAvatar");
    }

    public void onReqRemoveAvatar()
    {
        Account account = (Account)KBEngineApp.app.player();
        if (account != null)
        {
            string name = "";
            foreach (Toggle bt_Avatar in tg_avatars)
            {
                if (bt_Avatar.isOn)
                {
                    name = bt_Avatar.GetComponentInChildren<Text>().text;
                }
            }
            //if (name.Length > 0)
            //{
                account.reqRemoveAvatar(name);
                return;
            //}
        }
    }
    public void onRemoveAvatar(UInt64 dbid, Dictionary<UInt64, Dictionary<string, object>> avatarList)
    {
        if (dbid == 0)
        {
            print("Delete the avatar error!(删除角色错误!)");
            return;
        }

        onReqAvatarList(avatarList);
    }
    public void onReqAvatarList(Dictionary<UInt64, Dictionary<string, object>> avatarList)
    {
        ui_avatarList = avatarList;
        foreach (Toggle bt_Avatar in tg_avatars)
        {
            bt_Avatar.GetComponentInChildren<Text>().text = "空";
        }

        if (ui_avatarList != null && ui_avatarList.Count > 0)
        {
            int idx = 0;
            foreach (UInt64 dbid in ui_avatarList.Keys)
            {
                Dictionary<string, object> info = ui_avatarList[dbid];
                //	Byte roleType = (Byte)info["roleType"];
                string name = (string)info["name"];
                //	UInt16 level = (UInt16)info["level"];
                //UInt64 idbid = (UInt64)info["dbid"];
                tg_avatars[idx].GetComponentInChildren<Text>().text = name;

                dic_name_to_dbid[name] = dbid;
                idx++;
            }
        }
    }

    public void onEnterGame()
    {
        string name = "";
        foreach (Toggle bt_Avatar in tg_avatars)
        {
            if (bt_Avatar.isOn)
            {
                name = bt_Avatar.GetComponentInChildren<Text>().text;
            }
        }

        UInt64 selAvatarDBID = 0;
        if (name.Length > 0)
        {
            selAvatarDBID = dic_name_to_dbid[name];
        }
        else
        {
            print("未选择角色");
            return;
        }

        Account account = (Account)KBEngineApp.app.player();
        if (account != null)
            account.selectAvatarGame(selAvatarDBID);

        Application.LoadLevel("world");
    }
   
}
