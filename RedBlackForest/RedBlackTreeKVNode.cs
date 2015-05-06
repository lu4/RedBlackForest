using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace RedBlackForest
{
    public class RedBlackTreeNode<K, V>
    {
        public K Key { get; internal set; }
        public V Value { get; internal set; }

        internal Boolean IsBlack { get; set; }

        internal RedBlackTreeNode<K, V> Left { get; set; }
        internal RedBlackTreeNode<K, V> Right { get; set; }

        public KeyValuePair<K, V> Pair
        {
            get
            {
                return new KeyValuePair<K, V>(Key, Value);
            }
        }

        public override string ToString()
        {
            return String.Format("[{0}, {1}]", Key, Value);
        }
    }
}
