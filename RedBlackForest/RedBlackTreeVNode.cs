using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace RedBlackForest
{
    public class RedBlackTreeNode<V>
    {
        public V Value { get; internal set; }

        internal Boolean IsBlack { get; set; }

        internal RedBlackTreeNode<V> Left { get; set; }
        internal RedBlackTreeNode<V> Right { get; set; }

        public override string ToString()
        {
            return String.Format("[{1}]", Value);
        }
    }
}
