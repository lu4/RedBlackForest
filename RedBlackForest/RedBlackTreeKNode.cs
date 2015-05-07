using System;

namespace RedBlackForest
{
    internal class RedBlackTreeNode<TKey>
    {
        public TKey Key { get; internal set; }

        public Boolean IsBlack { get; set; }

        public RedBlackTreeNode<TKey> Left { get; internal set; }
        public RedBlackTreeNode<TKey> Right { get; internal set; }

        public override string ToString()
        {
            return String.Format("[{0}]", Key);
        }
    }
}
