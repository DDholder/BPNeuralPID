using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace Mci.Logging
{
    public class FileLogging : BaseLogging
    {
        public FileLogging(FileStream fs) : this(new StreamWriter(fs))
        {
        }

        public FileLogging(TextWriter tw) : base(tw)
        {
        }

        public override void Dispose(object sender)
        {
            ((TextWriter)sender).Dispose();
        }
        public override void Log(string content)
        {
            var dt = string.Format("[ {0} ] [ {1} ]", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), content);
            base.Log(dt);
        }
    }
}
