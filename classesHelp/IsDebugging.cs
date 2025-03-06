using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace site.classesHelp
{ 

    public class IsDebugging 
    {
        public bool check()
        {
            bool debugging = false;

            IsDebugCheck(ref debugging);

            return debugging;
        }

        [Conditional("DEBUG")]
        private void IsDebugCheck(ref bool isDebug)
        {
            isDebug = true;
        }
    }
}