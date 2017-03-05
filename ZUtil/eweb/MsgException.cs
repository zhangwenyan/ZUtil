using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eweb
{
    public class MsgException:Exception
    {
        public MsgException(String msg) : base(msg)
        {

        }
    }
}