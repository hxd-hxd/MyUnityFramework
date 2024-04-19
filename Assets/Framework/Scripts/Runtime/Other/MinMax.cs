// -------------------------
// 创建日期：2024/4/8 11:22:15
// -------------------------

using System;

namespace Framework
{
    [Serializable]
    public struct MinMax<T>
    {
        public T min, max;

        //[SerializeField]
        //private T _min,_max;

        //public T min
        //{
        //    get { return _min; }
        //    set { _min = value; }
        //}

        //public T max
        //{
        //    get { return _max; }
        //    set { _max = value; }
        //}

        public MinMax(T min, T max)
        {
            this.min = min;
            this.max = max;
        }

        public override string ToString()
        {
            return $"min:{min}\r\nmax:{max}";
        }
    }
}