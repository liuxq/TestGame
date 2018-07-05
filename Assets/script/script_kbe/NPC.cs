namespace KBEngine
{
    using UnityEngine;
    using System.Collections;
    using System;
    public class NPC : NPCBase
    {
        public NPC() : base()
        {
        }

        public override void onNameChanged(string oldValue)
        {
            // Dbg.DEBUG_MSG(className + "::set_name: " + old + " => " + v); 
            Event.fireOut("set_name", new object[] { this, name });
        }

    }
}
