using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace RedBlackForest
{
    public class RedBlackTreeNode<TValue>
    {
        public TValue Value { get; internal set; }

        internal Boolean IsBlack { get; set; }

        internal RedBlackTreeNode<TValue> Left { get; set; }
        internal RedBlackTreeNode<TValue> Right { get; set; }

        public override string ToString()
        {
            return String.Format("[{0}]", Value);
        }
    }
}
