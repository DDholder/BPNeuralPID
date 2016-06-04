using System;
using Mci.MatrixLibrary;
namespace Mci.ControlLibrary
{
    /// <summary>
    /// BPNetPID
    /// </summary>
    public sealed class BPNeuralIncrementPID
    {
        /// <summary>
        /// 输入隐藏权重
        /// </summary>
        public Matrix IHLWeight
        {
            private set; get;
        }

        /// <summary>
        /// 上次
        /// </summary>
        private Matrix LastIHLWeight
        {
            set; get;
        }

        /// <summary>
        /// 上上次
        /// </summary>
        private Matrix BeforeLastIHLWeight
        {
            set; get;
        }

        /// <summary>
        /// 隐藏输出权重
        /// </summary>
        public Matrix HOLWeight
        {
            private set; get;
        }

        private Matrix LastHOLWeight
        {
            set; get;
        }

        private Matrix BeforeLastHOLWeight
        {
            set; get;
        }

        /// <summary>
        /// 输入层个数
        /// </summary>
        public int InputItemLen { set; get; }

        /// <summary>
        /// 输入层
        /// </summary>
        private Matrix inputMatrix { set; get; }

        /// <summary>
        /// 输入层数目
        /// </summary>
        public int OutputItemLen { set; get; }

        /// <summary>
        /// 隐藏层数目
        /// </summary>
        private int hideLayerLen { set; get; }

        /// <summary>
        /// 学习效率
        /// </summary>
        public double StudyRatio
        {
            set; get;
        }

        /// <summary>
        /// 惯性
        /// </summary>
        public double Inertia
        {
            set; get;
        }

        /// <summary>
        /// 设定值
        /// </summary>
        public double SetTargetValue { set; get; }

        #region pid 运算保存值 

        private double currentError { set; get; }
        private double lastError { set; get; }
        private double beforeLastError { set; get; }

        public double lastOutputValue { set; get; }
        private double currentOutputValue { set; get; }

        private double currentFeedbackValue { set; get; }
        public double lastFeedbackValue { set; get; }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        private Matrix deltaPID = Matrix.Instance(3, 1);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inum">输入层个数</param>
        /// <param name="mnum">隐藏层个数</param>
        /// <param name="onum">输出层个数</param>
        public BPNeuralIncrementPID(int inum, int mnum, int onum)
        {
            if (inum <= 0 || mnum <= 0 || onum <= 0)
                throw new ArgumentOutOfRangeException();
            hideLayerLen = mnum;
            InputItemLen = inum;
            OutputItemLen = onum;
            HideLevelWeight();
            HideOutLevelWeight();
            inputMatrix = Matrix.Instance(1, InputItemLen);
        }

        /// <summary>
        /// 输入隐藏层权重
        /// </summary>
        public void HideOutLevelWeight()
        {
            HOLWeight = Matrix.Transport(Matrix.Instance(new double[,] {
            { 1.0770,-1.4946,-1.0456,-0.9911,0.4124},
            { -0.6525,0.6362,-0.3351,-0.3075,0.1799 },
            { 2.7938,-1.7449,-2.7563 ,-1.3541,2.0589},
            }));
            LastHOLWeight = (Matrix)HOLWeight.Clone();
            BeforeLastHOLWeight = (Matrix)HOLWeight.Clone();
        }
        public void RandomHideOutLevelWeight(double x)
        {
            HOLWeight = Matrix.Instance(hideLayerLen, OutputItemLen).InitMatrix(x);
            LastHOLWeight = (Matrix)HOLWeight.Clone();
            BeforeLastHOLWeight = (Matrix)HOLWeight.Clone();
        }
        /// <summary>
        /// 隐藏输出层权重
        /// </summary>
        public void HideLevelWeight()
        {
            IHLWeight = Matrix.Transport(Matrix.Instance(new double[,] {
              {-0.2857,0.6536,-1.3780,-0.5275 },
              {-0.2415,0.1331,1.0813,1.2065 },
              {-0.1081,-0.5632,1.7430,0.6511 },
              {0.9828,-0.2190,1.1532,-0.4335},
              {-0.4947,2.2139,-2.4075,-0.1041}
            }));
            LastIHLWeight = (Matrix)IHLWeight.Clone();
            BeforeLastIHLWeight = (Matrix)IHLWeight.Clone();
        }

        public void RandomHideLevelWeight(double x)
        {
            IHLWeight = Matrix.Instance(InputItemLen, hideLayerLen).InitMatrix(x);
            LastIHLWeight = (Matrix)IHLWeight.Clone();
            BeforeLastIHLWeight = (Matrix)IHLWeight.Clone();
        }



        /// <summary>
        /// 隐藏 F(.)
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private double HideLevelFun(double x)
        {
            return (Math.Exp(x) - Math.Exp(-x)) / (Math.Exp(x) + Math.Exp(-x));
        }

        /// <summary>
        /// 输出 F(.)
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private double OutputLevelFun(double x)
        {
            return Math.Exp(x) / (Math.Exp(x) + Math.Exp(-x)); ;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="feedBackValue"></param>
        /// <returns></returns>
        public double CoreExecute(double feedBackValue)
        {
            currentError = SetTargetValue - feedBackValue;
            inputMatrix.SetValue(0, 0, SetTargetValue);
            inputMatrix.SetValue(0, 1, feedBackValue);
            inputMatrix.SetValue(0, 2, currentError);
            inputMatrix.SetValue(0, 3, 1);

            var Oj1 = inputMatrix * IHLWeight;
            var NetI2 = ((Matrix)Oj1.Clone());
            var Oj2 = (Matrix)NetI2.Clone();
            Oj2.ExecuteFun(HideLevelFun);
            var Oj3 = Oj2 * HOLWeight;
            Oj3.ExecuteFun(OutputLevelFun);
            deltaPID.SetValue(0, 0, currentError - lastError); //P
            deltaPID.SetValue(1, 0, currentError);   //I
            deltaPID.SetValue(2, 0, currentError - 2 * lastError + beforeLastError);//D
            var pidOut = Oj3 * deltaPID;
            currentOutputValue = Limit(lastOutputValue + pidOut.GetValue(0, 0));

            var dw0 = LastHOLWeight - BeforeLastHOLWeight;
            dw0.ExecuteFun(a => a * Inertia);

            var direct = SignFun((currentFeedbackValue - lastFeedbackValue) / (currentOutputValue - lastOutputValue + 0.0000001));
            var du = (Matrix)Oj3.Clone();
            du.ExecuteFun(a => 2d / (Math.Pow(Math.Exp(a) + Math.Exp(-a), 2d)));
            du.ExecuteFun(a => a * currentError * direct);
            du.HadamardMul(Matrix.Transport(deltaPID));
            var du_bak = (Matrix)du.Clone();
            du.ExecuteFun(a => a * StudyRatio);
            var dw = Matrix.Transport(Oj2) * du;
            HOLWeight = dw + LastHOLWeight + dw0;

            NetI2.ExecuteFun(a => 4d / (Math.Pow(Math.Exp(a) + Math.Exp(-a), 2d)));
            var hl = Matrix.Transport((HOLWeight * Matrix.Transport(du_bak))).HadamardMul((NetI2)).ExecuteFun(a => a * StudyRatio);
            var di = (Matrix.Transport(inputMatrix) * hl);
            IHLWeight = LastIHLWeight + di + (LastIHLWeight - BeforeLastIHLWeight).ExecuteFun(a => a * Inertia);

            lastOutputValue = currentOutputValue;
            lastFeedbackValue = currentFeedbackValue;
            beforeLastError = lastError;
            lastError = currentError;
            BeforeLastHOLWeight = LastHOLWeight;
            LastHOLWeight = HOLWeight;
            BeforeLastIHLWeight = LastIHLWeight;
            LastIHLWeight = IHLWeight;
            return currentOutputValue;
        }

        /// <summary>
        /// 方向函数
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private double SignFun(double x)
        {
            return x > 0 ? 1d : (x == 0 ? 0 : -1);
        }

        /// <summary>
        /// 限幅
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private double Limit(double val)
        {
            return val > 1d ? 1d : (val < 0 ? 0 : val);
        }


    }
}
