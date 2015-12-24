using UnityEngine;
using System.Collections;
using KBEngine;
using UnityEngine.UI;
using System;

public class UI_LoginSystem : MonoBehaviour {
    public InputField if_userName;
    public InputField if_passWord;
    public Button bt_login;
    public Text text_status;

	// Use this for initialization
	void Start () {
        //DontDestroyOnLoad(gameObject.transform);
        KBEngine.Event.registerOut("onConnectStatus", this, "onConnectStatus");
        KBEngine.Event.registerOut("onLoginFailed", this, "onLoginFailed");
        KBEngine.Event.registerOut("onLoginSuccessfully", this, "onLoginSuccessfully");
        KBEngine.Event.registerOut("onCreateAccountResult", this, "onCreateAccountResult");
	}
    void OnDestroy()
    {
        KBEngine.Event.deregisterOut(this);
    }
    public void onConnectStatus(bool beSuccess)
    {
        if (beSuccess)
        {
            print("连接成功，正在登陆");
            text_status.text = "连接成功，正在登陆";
        }
        else
        {
            print("连接错误");
            text_status.text = "连接错误";
        }
    }
    public void onLoginFailed(UInt16 errorCode)
    {
        print("登陆失败" + KBEngineApp.app.serverErr(errorCode));
        text_status.text = "登陆失败" + KBEngineApp.app.serverErr(errorCode);
    }
    public void onLoginSuccessfully(UInt64 uuuid, Int32 id, Account account)
    {
        if (account != null)
        {
            print("登录成功");
            text_status.text = "登录成功";
            //跳转场景
            Application.LoadLevel("selectAvatar");
        }
    }
    public void onLogin()
    {
        print("connect to server...(连接到服务端...)");
        text_status.text = "连接到服务端...";
        KBEngine.Event.fireIn("login", if_userName.text, if_passWord.text, System.Text.Encoding.UTF8.GetBytes("lxqLogin"));
    }
    public void onRegister()
    {
        print("connect to server...(连接到服务端...)");
        text_status.text = "连接到服务端...";
        KBEngine.Event.fireIn("createAccount", if_userName.text, if_passWord.text, System.Text.Encoding.UTF8.GetBytes("lxqRegister"));
    }
    public void onCreateAccountResult(UInt16 retcode, byte[] datas)
    {
        if (retcode != 0)
        {
            print("createAccount is error(注册账号错误)! err=" + KBEngineApp.app.serverErr(retcode));
            text_status.text = "createAccount is error(注册账号错误)! err=" + KBEngineApp.app.serverErr(retcode);
            return;
        }
        print("createAccount is successfully!(注册账号成功!)");
        text_status.text = "createAccount is successfully!(注册账号成功!)";
    }
	// Update is called once per frame
	void Update () {
	
	}
    public void onClose()
    {
        Application.Quit();
    }
}
