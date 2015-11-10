using UnityEngine;
using System.Collections;
using KBEngine;
using UnityEngine.UI;
using System;

public class loginSystem : MonoBehaviour {
    public InputField if_userName;
    public InputField if_passWord;
    public Button bt_login;

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject.transform);
        KBEngine.Event.registerOut("onConnectStatus", this, "onConnectStatus");
        KBEngine.Event.registerOut("onLoginFailed", this, "onLoginFailed");
	}
    public void onConnectStatus(bool beSuccess)
    {
        if (beSuccess)
        {
            print("连接成功，正在登陆");
        }
        else
        {
            print("连接错误");
        }
    }
    public void onLoginFailed(UInt16 errorCode)
    {
        print("连接失败" + KBEngineApp.app.serverErr(errorCode));
    }
    public void onLogin()
    {
        KBEngine.Event.fireIn("login", if_userName.text, if_passWord.text, System.Text.Encoding.UTF8.GetBytes("2015.11.7"));
    }
	// Update is called once per frame
	void Update () {
	
	}
}
