using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class Utility
{
    static public Int32 getPostInt(string name)
    {
        return Int32.Parse(name.Substring(name.IndexOf('_')+1));
    }
}

