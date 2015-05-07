using System;
using System.Collections.Generic;

namespace RedBlackForest
{
    internal class RedBlackTreeNode<TKey, TValue>
    {
        public TKey Key { get; internal set; }
        public TValue Value { get; internal set; }

        public Boolean IsBlack { get; set; }

        public RedBlackTreeNode<TKey, TValue> Left { get; internal set; }
        public RedBlackTreeNode<TKey, TValue> Right { get; internal set; }

        public KeyValuePair<TKey, TValue> KeyValuePair
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
