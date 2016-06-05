using System;
using System.Collections;
using Mci.Logging;
using Mci.MatrixLibrary;
using Mci.ControlLibrary;
using Mci.OPCClient;
namespace MC
{
    class Program
    {
        static void Main(string[] args)
        {
            var svr = Client.Instance();
            var arr = svr.GetOPCServerList();
            if (arr != null)
            {
                foreach (var s in arr)
                {
                    s.ToConsole();
                }
            }
            svr.Connect("Kepware.KEPServerEX.V5").ToString().ToConsole("Connect status");
            var list = svr.GetTags();
            if (list != null)
            {
                foreach (var o in list)
                {
                    o.ToConsole();
                }
            }
            var m1 = Matrix.Instance(4, 4).InitMatrix(0.01);
            var m2 = Matrix.Instance(4, 1).InitMatrix(1);
            m1.ToString().ToConsole("m1 " + m1.Dim.ToString());
            m2.ToString().ToConsole("m2 " + m2.Dim.ToString());
            (m1 * m2).ToString().ToConsole("m1*m2");
            var pid = new BPNeuralIncrementPID(4, 8, 3);
            pid.SetTargetValue = 0.5d;
            pid.StudyRatio = 0.25d;
            pid.Inertia = 0.002d;
            var i = 1;
            var runCount = 1;
            while (true)
            {
                for (var c = 0; c < runCount; c++)
                {
                    var fb = Feedback(i++, pid.lastFeedbackValue, pid.lastOutputValue);
                    pid.CoreExecute(fb);
                }
                runCount = 0;
                var cmd = "Reader << ".ConsoleReader().ToLower();
                #region 命令行
                switch (cmd)
                {
                    case "help":
                    case "?":
                        @"
--------------------------------------------------------------
帮助:
    help | ? 
状态:
    sh
设置:
    set[target|study|inertia|count] value
--------------------------------------------------------------
".ToConsole();
                        break;
                    case "exit":
                        goto _end;
                    case "sh":
                        pid.IHLWeight.ToString().ToConsole("输入－隐藏权重");
                        pid.HOLWeight.ToString().ToConsole("隐藏－输入权重");
                        (pid.SetTargetValue - pid.lastOutputValue).ToString().ToConsole("差值");
                        break;
                    default:
                        var subCmdSets = cmd.Split(' ');
                        if (subCmdSets != null && subCmdSets.Length > 1)
                        {
                            var subCmd = subCmdSets[0].ToString().ToLower();
                            var header = subCmd.Length > 3 ? subCmd.Substring(0, 3) : string.Empty;
                            var val = 0d;
                            if (string.Compare(header, "set") == 0)
                            {
                                if (double.TryParse(subCmdSets[1], out val))
                                {
                                    var behindCmd = subCmd.Substring(3);
                                    switch (behindCmd)
                                    {
                                        case "target":
                                            pid.SetTargetValue = val;
                                            break;
                                        case "study":
                                            pid.StudyRatio = val;
                                            break;
                                        case "inertia":
                                            pid.Inertia = val;
                                            break;
                                        case "count":
                                            var ro = Convert.ToInt32(val);
                                            if (ro >= 0)
                                                runCount = ro;
                                            else
                                            {
                                                "Error : Count low zero!".ToConsole();
                                            }
                                            break;
                                        default:
                                            "No supported command!".ToConsole();
                                            break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(cmd))
                            {
                                "No supported command!".ToConsole();
                            }
                        }
                        break;
                }
                #endregion
            }

        _end:
            "Bye".ToConsole();

        }

        static double TranFun(int i)
        {
            return 1.2d * (1d - 0.8d * Math.Exp(-0.1d * i));
        }
        static double Feedback(int index, double lastFeedBack, double lastOutValue)

        {
            return TranFun(index) * lastFeedBack / (1d + Math.Pow(lastFeedBack, 2d)) + lastOutValue;
        }

    }
}
