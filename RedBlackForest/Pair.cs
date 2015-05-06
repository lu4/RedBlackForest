using System;

namespace RedBlackForest
{
    /// <summary>
    /// In contrast to Tuple Pair is a value type containing a pair of values
    /// </summary>
    /// <typeparam name="T">Pair type parameter</typeparam>
    public struct Pair<T>
    {
        private readonly T a;
        private readonly T b;

        public Pair(T a, T b)
        {
            this.a = a;
            this.b = b;
        }

        public T A
        {
            get
            {
                return a;
            }
        }
        public T B
        {
            get
            {
                return b;
            }
        }

        public override String ToString()
        {
            return String.Format("[({0}), ({1})]", A, B);
        }
    }

    /// <summary>
    /// In contrast to Tuple Pair is a value type containing a pair of values
    /// </summary>
    /// <typeparam name="T1">First pair type parameter</typeparam>
    /// <typeparam name="T2">First pair type parameter</typeparam>
    public struct Pair<T1, T2>
    {
        private readonly T1 a;
        private readonly T2 b;

        public Pair(T1 a, T2 b)
        {
            this.a = a;
            this.b = b;
        }

        public T1 A
        {
            get
            {
                return a;
            }
        }
        public T2 B
        {
            get
            {
                return b;
            }
        }

        public override String ToString()
        {
            return String.Format("[({0}), ({1})]", A, B);
        }
    }
}
