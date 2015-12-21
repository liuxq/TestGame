namespace KBEngine
{
    using UnityEngine;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class Avatar : KBEngine.GameObject
    {
        public override void __init__()
        {
            if (isPlayer())
            {
                Event.registerIn("relive", this, "relive");
                Event.registerIn("updatePlayer", this, "updatePlayer");
                Event.registerIn("sendChatMessage", this, "sendChatMessage");
            }	
        }

        public override void onDestroy()
        {
            if (isPlayer())
            {
                KBEngine.Event.deregisterIn(this);
            }
        }

        public virtual void updatePlayer(float x, float y, float z, float yaw)
        {
            position.x = x;
            position.y = y;
            position.z = z;

            direction.z = yaw;
        }
        public override void onEnterWorld()
        {
            base.onEnterWorld();

            if (isPlayer())
            {
                Event.fireOut("onAvatarEnterWorld", new object[] { KBEngineApp.app.entity_uuid, id, this });
            }
        }
        public void sendChatMessage(string msg)
        {
            object name = getDefinedPropterty("name");
            baseCall("sendChatMessage", (string)name + ": " + msg);
        }
        public void ReceiveChatMessage(string msg)
        {
            Event.fireOut("ReceiveChatMessage", msg);
        }
        public void relive(Byte type)
        {
            cellCall("relive", type);
        }
    }
}
