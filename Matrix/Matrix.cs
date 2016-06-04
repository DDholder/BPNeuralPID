using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mci.MatrixLibrary
{
    /// <summary>
    /// 矩阵运算
    /// </summary>
    public class Matrix : ICloneable
    {
        /// <summary>
        /// 数据
        /// </summary>
        private double[,] _data
        {
            set; get;
        }

        /// <summary>
        /// 行数
        /// </summary>
        public int Row
        {
            get;
        }

        /// <summary>
        /// 列数
        /// </summary>
        public int Column
        {
            get;
        }

        /// <summary>
        /// 维度
        /// </summary>
        public Tuple<int, int> Dim
        {
            get
            {
                return this.GetRank();
            }
        }

        public Matrix(int row, int col)
        {
            if (row <= 0 || col <= 0)
                throw new ArgumentOutOfRangeException("Matrix");
            Row = row;
            Column = col;
            _data = Alloc(Row, Column);
        }

        public Matrix(Tuple<int, int> rankTuple) :
            this(rankTuple.Item1, rankTuple.Item2)
        {
        }

        /// <summary>
        /// 生成数组
        /// </summary>
        /// <param name="r">行</param>
        /// <param name="c">列</param>
        /// <returns></returns>
        private static double[,] Alloc(int r, int c)
        {
            return new double[r, c];
        }

        /// <summary>
        /// 生成数组
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private static double[,] Alloc(Tuple<int, int> t)
        {
            return Alloc(t.Item1, t.Item2);
        }

        /// <summary>
        /// 随机数矩阵
        /// </summary>
        /// <returns></returns>
        public Matrix InitMatrix()
        {
            InitMatrix(1d);
            return this;
        }

        /// <summary>
        /// 因子*随机数(0-1) 初始化矩阵
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public Matrix InitMatrix(double param)
        {
            var rd = new Random();
            ExecuteFun(a => rd.NextDouble() * param);
            return this;
        }

        /// <summary>
        /// 用特定值初始化矩阵
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Matrix InitMatrixUsingTheValue(double v)
        {
            ExecuteFun(a => v);
            return this;
        }

        /// <summary>
        /// 设值
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public Matrix SetValue(int row, int col, double v)
        {
            if (row >= Row || row < 0 || col >= Column || col < 0)
                throw new ArgumentException("SetValue");
            _data[row, col] = v;
            return this;
        }

        /// <summary>
        /// 取值
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public double GetValue(int row, int col)
        {
            if (row >= Row || row < 0 || col >= Column || col < 0)
                throw new ArgumentException("GetValue");
            return _data[row, col];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public Matrix InitMatrix(double[,] array)
        {
            var tup = GetRank(array);
            var s = GetRank();
            if (!s.Equals(tup))
                throw new ArgumentOutOfRangeException("InitMatrix");
            for (var i = 0; i < Row; i++)
            {
                for (var j = 0; j < Column; j++)
                {
                    _data[i, j] = array[i, j];
                }
            }
            return this;
        }

        /// <summary>
        /// 从数组
        /// </summary>
        /// <param name="m"></param>
        public Matrix InitMatrix(double[] m)
        {
            var tup = GetRank(m);
            var s = GetRank();
            if (!s.Equals(tup))
                throw new ArgumentOutOfRangeException("InitMatrix");
            for (var j = 0; j < Column; j++)
            {
                _data[0, j] = m[j];
            }
            return this;
        }

        /// <summary>
        /// 哈德马乘积
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public Matrix HadamardMul(Matrix m)
        {
            if (m == null)
                throw new ArgumentNullException("MulPos");
            var r = m.GetRank();
            var s = GetRank();
            if (!r.Equals(s))
                throw new ArgumentOutOfRangeException("MulPos");
            for (var i = 0; i < Row; i++)
                for (var j = 0; j < Column; j++)
                {
                    _data[i, j] *= m._data[i, j];
                }
            return this;
        }

        /// <summary>
        /// 哈德马乘积
        /// </summary>
        /// <param name="s"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static Matrix HadamardMul(Matrix s, Matrix d)
        {
            var st = s.GetRank();
            var dt = d.GetRank();
            if (!st.Equals(dt))
                throw new ArgumentOutOfRangeException();
            var buf = Alloc(st);
            for (var i = 0; i < st.Item1; i++)
            {
                for (var j = 0; j < st.Item2; j++)
                {
                    buf[i, j] = s._data[i, j] * d._data[i, j];
                }
            }
            return Instance(buf);
        }


        /// <summary>
        /// 访问器
        /// </summary>
        /// <param name="fun"></param>
        /// <returns></returns>
        public Matrix ExecuteFun(Func<double, double> fun)
        {
            if (fun == null)
                throw new ArgumentNullException();
            for (var i = 0; i < Row; i++)
            {
                for (var j = 0; j < Column; j++)
                {
                    _data[i, j] = fun(_data[i, j]);
                }
            }
            return this;
        }

        /// <summary>
        /// 实例
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Matrix Instance(double[,] m)
        {
            var tup = GetRank(m);
            var o = Instance(tup).InitMatrix(m);
            return o;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Matrix Instance(double[] m)
        {
            var tup = GetRank(m);
            var t = Alloc(tup);
            for (int i = 0, mx = tup.Item2; i < mx; i++)
            {
                t[0, i] = m[i];
            }
            return Instance(t);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Matrix Instance(Tuple<int, int> m)
        {
            if (m == null)
                throw new ArgumentNullException();
            return Instance(m.Item1, m.Item2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Matrix Instance(int r, int c)
        {
            if (r <= 0 || c <= 0)
                throw new ArgumentException();
            var instance = new Matrix(r, c);
            return instance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Tuple<int, int> GetRank(double[,] m)
        {
            if (m == null || m.Length == 0)
                throw new ArgumentNullException("GetMatrix");
            var rank = m.Rank;
            var cols = m.GetUpperBound(rank - 1) + 1;
            var rows = m.Length / cols;
            return new Tuple<int, int>(rows, cols);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Tuple<int, int> GetRank(double[] m)
        {
            if (m == null || m.Length == 0)
                throw new ArgumentNullException("GetMatrix");
            var cols = m.Length;
            var rows = 1;
            return new Tuple<int, int>(rows, cols);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Tuple<int, int> GetRank()
        {
            return new Tuple<int, int>(Row, Column);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static Matrix operator -(Matrix s, Matrix d)
        {
            var st = s.GetRank();
            var dt = d.GetRank();
            if (!st.Equals(dt))
                throw new ArgumentOutOfRangeException("-");
            var buf = Alloc(st);
            for (var i = 0; i < st.Item1; i++)
            {
                for (var j = 0; j < st.Item2; j++)
                {
                    buf[i, j] = s._data[i, j] - d._data[i, j];
                }
            }
            return Instance(buf);
        }


        public static Matrix operator +(Matrix s, Matrix d)
        {
            var st = s.GetRank();
            var dt = d.GetRank();
            if (!st.Equals(dt))
                throw new ArgumentOutOfRangeException("+");
            var buf = Alloc(st);
            for (var i = 0; i < st.Item1; i++)
            {
                for (var j = 0; j < st.Item2; j++)
                {
                    buf[i, j] = s._data[i, j] + d._data[i, j];
                }
            }
            return Instance(buf);
        }

        public static Matrix operator *(Matrix s, Matrix d)
        {
            if (s.Column != d.Row)
                throw new ArgumentOutOfRangeException("*");
            var _r = s.Row;
            var _c = d.Column;
            var _t = s.Column;
            var newMatrix = Alloc(_r, _c);
            for (var r = 0; r < _r; r++)
            {
                for (var c = 0; c < _c; c++)
                {
                    var j = _t;
                    var sum = 0.0d;
                    while (j-- > 0)
                    {
                        sum += s._data[r, j] * d._data[j, c];
                    }
                    newMatrix[r, c] = sum;
                }
            }
            return Matrix.Instance(newMatrix);
        }


        public static Matrix operator *(Matrix s, double d)
        {
            if (s == null)
                throw new ArgumentNullException();
            var newMatrix = (Matrix)s.Clone();
            return newMatrix.ExecuteFun(a => a * d);
        }


        public static Matrix operator *(double d, Matrix s)
        {
            return s * d;
        }


        public static Matrix Transport(Matrix m)
        {
            if (m == null)
                throw new ArgumentNullException();
            var i = m.GetRank();
            var _r = i.Item1;
            var _c = i.Item2;
            var newOjb = Alloc(_c, _r);
            for (var r = 0; r < _r; r++)
                for (var c = 0; c < _c; c++)
                {
                    newOjb[c, r] = m._data[r, c];
                }
            return Instance(newOjb);
        }


        public override string ToString()
        {
            var sb = new StringBuilder();
            for (var r = 0; r < Row; r++)
            {
                for (var c = 0; c < Column; c++)
                {
                    sb.AppendFormat("{0:F8}\t", _data[r, c]);
                }
                sb.Append(Environment.NewLine);
            }
            return sb.ToString();
        }


        public object Clone()
        {
            return Instance(_data);
        }


    }
}
