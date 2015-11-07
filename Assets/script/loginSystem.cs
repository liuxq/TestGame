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
	}
    void onLogin()
    {

    }
	// Update is called once per frame
	void Update () {
	
	}
}
