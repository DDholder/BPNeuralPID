using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OPCAutomation;
using System.Threading;
using Mci.Logging;
using System.Runtime.InteropServices;
namespace Mci.OPCClient
{
    public sealed class Client : IDisposable
    {
        OPCServer _server;
        bool isDisapose = false;
        bool isConnected = false;

        public static Client Instance()
        {
            return new Client();
        }

        private Client()
        {
            _server = CreateSelf();
        }

        public bool Connect(string proId, string nodeName = null)
        {
            if (_server != null)
            {
                if (!isConnected)
                {
                    try
                    {
                        _server.Connect(proId, nodeName);
                        isConnected = true;
                    }
                    catch (COMException e)
                    {
                        e.StackTrace.ToFile();
                        return false;
                    }
                    catch (Exception ex)
                    {
                        ex.StackTrace.ToFile();
                        return false;
                    }
                }
            }
            return isConnected;
        }
        public IList<string> GetTags()
        {
            if (!isConnected)
                return null;
            try
            {
                var tags = new List<string>();
                var cb = _server.CreateBrowser();
                cb.ShowBranches();
                cb.ShowLeafs(true);
                foreach (var i in cb)
                {
                    tags.Add(i.ToString());
                }
                return tags;
            }
            catch (Exception e)
            {
                e.StackTrace.ToFile();
            }
            return null;
        }

        public void MonitorTag(Delegate d)
        {

        }

        /// <summary>
        /// 获取变量列表
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public IList<string> GetOPCServerList(string node)
        {
            if (_server == null)
                return null;
            Array arr = null;
            try
            {
                arr = _server.GetOPCServers(node) as Array;
            }
            catch (Exception e)
            {
                e.StackTrace.ToFile();
                return null;
            }
            if (arr == null)
                return null;
            var ret = new List<string>();
            foreach (var o in arr)
            {
                ret.Add(o.ToString());
            }
            return ret;
        }

        /// <summary>
        /// 获取变量列表
        /// </summary>
        /// <returns></returns>
        public IList<string> GetOPCServerList()
        {
            return GetOPCServerList(null);
        }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <returns></returns>
        private OPCServer CreateSelf()
        {
            OPCServer _svr = null;
            try
            {
                _svr = new OPCServer();
            }
            catch (COMException e)
            {
#if DEBUG
                e.StackTrace.ToConsole();
#endif
                e.StackTrace.ToFile();
                return null;
            }
            catch (Exception ex)
            {
#if DEBUG
                ex.StackTrace.ToConsole();
#endif
                ex.StackTrace.ToFile();
            }
            return _svr;
        }

        public void Dispose()
        {
            if (!isDisapose)
            {
                isDisapose = true;
                if (_server != null && isConnected)
                {
                    _server.Disconnect();
                }
            }
        }

        ~Client()
        {
            Dispose();
        }
    }
}
