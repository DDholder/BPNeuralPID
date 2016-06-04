using System;
using System.IO;
namespace Mci.Logging
{
    public abstract class BaseLogging : ILogging
    {
        TextWriter textWriter;
        object _locked = new object();
        private bool isDispose = false;

        public BaseLogging(TextWriter writer)
        {
            textWriter = writer;
        }

        public virtual void Log(string content)
        {
            if (textWriter == null)
                throw new NullReferenceException();
            lock (_locked)
            {
                textWriter.WriteLine(content);
                textWriter.Flush();
            }
        }

        public void Dispose()
        {
            if (!isDispose)
            {
                Dispose(textWriter);
                textWriter = null;
                isDispose = true;
            }
        }
        public abstract void Dispose(object sender);
    }
}
