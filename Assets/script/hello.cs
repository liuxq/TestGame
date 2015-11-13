using UnityEngine;
using System.Collections;
using KBEngine;

public class hello : MonoBehaviour {

	// Use this for initialization
	void Start () {
        KBEngine.Event.registerOut("onHello", this, "onHello");
	}
	
	public void reqHello()
    {
        Account account = (Account)KBEngineApp.app.player();
        if(account != null)
        {
            account.reqHello();
        }
    }
    public void onHello(string data)
    {
        print(data);
    }
}
