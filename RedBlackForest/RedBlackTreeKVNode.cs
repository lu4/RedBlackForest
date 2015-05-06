using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace RedBlackForest
{
    public class RedBlackTreeNode<TKey, TValue>
    {
        public TKey Key { get; internal set; }
        public TValue Value { get; internal set; }

        internal Boolean IsBlack { get; set; }

        internal RedBlackTreeNode<TKey, TValue> Left { get; set; }
        internal RedBlackTreeNode<TKey, TValue> Right { get; set; }

        public KeyValuePair<TKey, TValue> Pair
        {
            get
            {
                return new KeyValuePair<TKey, TValue>(Key, Value);
            }
        }

        public override string ToString()
        {
            return String.Format("[{0}, {1}]", Key, Value);
        }
    }
}
