using UnityEngine;
using System.Collections;
using KBEngine;
using UnityEngine.UI;

public class loginSystem : MonoBehaviour {
    public InputField if_userName;
    public InputField if_passWord;
    public Button bt_login;

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject.transform);
        KBEngine.Event.registerOut("onConnectStatus", this, "onConnectStatus");
	}
    public void onConnectStatus(bool beSuccess)
    {
        
    }
    public void onLogin()
    {
        KBEngine.Event.fireIn("login", if_userName.text, if_passWord.text, System.Text.Encoding.UTF8.GetBytes("2015.11.7"));
    }
	// Update is called once per frame
	void Update () {
	
	}
}
