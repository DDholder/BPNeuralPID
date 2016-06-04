using System;
using System.Threading;
using System.Configuration;
using System.IO;
namespace Mci.Logging
{
    public sealed class LoggingFactory
    {
        public static ILogging FileLog(FileStream fs)
        {
            return new FileLogging(fs);
        }

        public static ILogging ConsoleLog()
        {
            return new ConsoleLogging();
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public static class LoggingExtend
    {
        static ILogging fileLogging;
        static ILogging consoleLogging;
        private static string loggingFileName = string.Empty;
        public static void ToFile(this string s)
        {
            LazyInitializer.EnsureInitialized<ILogging>(ref fileLogging, CreateFileLogInstance);
            fileLogging.Log(s);
        }

        public static void ToConsole(this string s)
        {
            LazyInitializer.EnsureInitialized<ILogging>(ref consoleLogging, () => LoggingFactory.ConsoleLog());
            consoleLogging.Log(s);
        }

        public static void ToConsole(this string s, string tag)
        {
            if (tag != null)
            {
                var temp = tag + Environment.NewLine + s;
                temp.ToConsole();
            }
            else s.ToConsole();
        }

        public static string ConsoleReader(this string s)
        {
            var tip = s + " <<< ";
            Console.Write(s);
            return Console.ReadLine();
        }

        private static ILogging CreateFileLogInstance()
        {

            var fileRealPath = ConfigurationManager.AppSettings["LogPath"];
            if (string.IsNullOrWhiteSpace(fileRealPath))
                fileRealPath = Directory.GetCurrentDirectory();
            if (!fileRealPath.EndsWith(@"\"))
                fileRealPath += @"\";
            fileRealPath += loggingFileName;
            var fs = new FileStream(fileRealPath, FileMode.OpenOrCreate | FileMode.Append, FileAccess.Write);
            return LoggingFactory.FileLog(fs);
        }


        public static string FileName
        {
            set
            {
                loggingFileName = value;
            }
            get
            {
                return loggingFileName;
            }
        }
    }
}
