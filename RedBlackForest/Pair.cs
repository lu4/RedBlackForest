using System;

namespace RedBlackForest
{
    public struct Pair<T1>
    {
        private readonly T1 a;
        private readonly T1 b;

        public Pair(T1 a, T1 b)
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
        public T1 B
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
