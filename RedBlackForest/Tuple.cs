using System;
using System.Collections.Generic;
using System.Text;

namespace RedBlackForest
{
    public struct Tuple<T>
    {
        private T _A;
        private T _B;

        public Tuple(T a, T b)
        {
            this._A = a;
            this._B = b;
        }

        public T A
        {
            get
            {
                return _A;
            }
        }
        public T B
        {
            get
            {
                return _B;
            }
        }

        public override String ToString()
        {
            return String.Format("[({0}), ({1})]", A, B);
        }
    }
}
