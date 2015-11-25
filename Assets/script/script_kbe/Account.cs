namespace KBEngine
{
    using UnityEngine;
    using System.Collections;
    using System;

    public class Account : Entity
    {
        public override void __init__()
        {
            Event.fireOut("onLoginSuccessfully", new object[] { KBEngineApp.app.entity_uuid, id, this });
        }
        public void reqHello()
        {
            baseCall("reqHello");
        }
        public void reqCreateAvatar(string name, byte roleType)
        {
            baseCall("reqCreateAvatar", new object[]{name, roleType});
        }

        public void onHello(string data)
        {
            Event.fireOut("onHello", new object[] { data });
        }
        public void onCreateAvatarResult(byte success, object data)
        {
            Event.fireOut("onCreateAvatarResult", new object[] { success, data });
        }
    }
}