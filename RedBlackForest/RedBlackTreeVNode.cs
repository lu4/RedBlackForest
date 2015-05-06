using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace RedBlackForest
{
    public class RBTreeNode<V>
    {
        public V Value { get; internal set; }

        internal Boolean IsBlack { get; set; }

        internal RBTreeNode<V> Left { get; set; }
        internal RBTreeNode<V> Right { get; set; }

        public override string ToString()
        {
            return String.Format("[{1}]", Value);
        }
    }
}
