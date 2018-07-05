using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KBEngine
{
    public class DroppedItem : DroppedItemBase
    {
        public override void __init__()
        {
        }

        public void pickUpRequest()
        {
            cellCall("pickUpRequest");
        }
    }
}
