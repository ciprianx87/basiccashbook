using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Reflection;

namespace CashBook.Common
{
    public sealed class Logger
    {
        private static readonly object syncObj = new object();
        private static Logger instance;
        public static Logger Instance
        {
            get
            {
                lock (syncObj)
                {
                    if (instance == null)
                    {
                        instance = new Logger();
                    }
                    return instance;
                }
            }
        }
        private Logger()
        {
            Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }
        //private  ILog log;

        public ILog Log { get; set; }

        public void LogException(Exception ex)
        {
            if (ex != null)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
                Log.Error(ex.Source);
                if (ex.InnerException != null)
                {
                    Log.Error(ex.InnerException.Message);
                    Log.Error(ex.InnerException.StackTrace);
                    Log.Error(ex.InnerException.Source);
                }

            }
        }
    }
}
