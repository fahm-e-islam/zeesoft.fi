using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Utilities
{
    public static class GCCollector
    {
        public static void Clean()
        {
            try
            {
                Utilities.TextLogger2.Log(LogType.Debug,"GC Collecting...");
                GC.Collect();
                Thread.Sleep(5000);
            }
            catch (Exception x)
            {
                Utilities.TextLogger2.Log(LogType.Exception,"Error GC Collecting:" + x.StackTrace);
            }        
        }
    }
}
