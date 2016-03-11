using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using KBEngine;
using System;
using System.Collections.Generic;

public class UI_CreateAvatar : MonoBehaviour {
    public InputField if_createAvatarName;
    public Toggle[] tg_profs = new Toggle[2];
	// Use this for initialization
	void Start () {
        KBEngine.Event.registerOut("onCreateAvatarResult", this, "onCreateAvatarResult");
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
        Account account = KBEngineApp.app==null? null:(Account)KBEngineApp.app.player();
        if (account != null)
        {
            byte roleType = 1;

            for (int i = 0; i < tg_profs.Length; i++)
            {
                if (tg_profs[i].isOn)
                {
                    roleType = (byte)(i + 1);
                }
            }
            
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
    public void onCancel()
    {
        Application.LoadLevel("selectAvatar");
    }
}
