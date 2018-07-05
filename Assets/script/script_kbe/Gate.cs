using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KBEngine
{
    public class Gate : GateBase
	{
        public Gate() : base()
        {
        }

        public override void onNameChanged(string oldValue)
        {
            // Dbg.DEBUG_MSG(className + "::set_name: " + old + " => " + v); 
            Event.fireOut("set_name", new object[] { this, name });
        }
    }
}
