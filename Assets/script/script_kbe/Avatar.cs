namespace KBEngine
{
    using UnityEngine;
    using System.Collections;

    public class Avatar : Entity
    {
        public override void __init__()
        {
            if (isPlayer())
            {
                Event.registerIn("updatePlayer", this, "updatePlayer");
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
    }
}
