using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mci.Logging
{
    public class ConsoleLogging : BaseLogging
    {
        public ConsoleLogging() : base(Console.Out)
        {
        }

        public override void Dispose(object sender)
        {
        }

        public override void Log(string s)
        {
            var dt = string.Format("{0} : {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), s);
            base.Log(s);
        }
    }
}
