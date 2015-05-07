using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace RedBlackForest
{
    public partial class RedBlackTree<TKey> : IEnumerable<TKey>
    {
        /// <summary>
        /// Stores the root node of the tree.
        /// </summary>
        private RedBlackTreeNode<TKey> RootNode { get; set; }

        /// <summary>
        /// Stores the key comparison function.
        /// </summary>
        public IComparer<TKey> Comparer { get; private set; }

        /// <summary>
        /// Initializes a new instance of the LeftLeaningRedBlackTree class implementing a normal dictionary.
        /// </summary>
        /// <param name="comparer">The key comparison function.</param>
        public RedBlackTree(IComparer<TKey> comparer)
        {
            if (null == comparer)
            {
                throw new ArgumentNullException("comparer");
            }

            Comparer = comparer;
        }

        public RedBlackTree()
            : this(Comparer<TKey>.Default)
        {
        }

        public Boolean IsEmpty
        {
            get
            {
                return RootNode == null;
            }
        }

        /// <summary>
        /// Gets the count of key/value pairs in the tree.
        /// </summary>
        public Int32 Count { get; private set; }

        /// <summary>
        /// Adds a key/value pair to the tree.
        /// </summary>
        /// <param name="key">Key to add.</param>
        /// <param name="value">Value to add.</param>
        public void Add(TKey key)
        {
            RedBlackTreeNode<TKey> result;
            RootNode = Add(RootNode, key, out result);
            RootNode.IsBlack = true;
        }

        /// <summary>
        /// Adds a value to the tree
        /// </summary>
        /// <remarks>When adding element that exists in collection, this method not throws </remarks>
        /// <param name="key">Key to add.</param>
        /// <param name="value">Value to add.</param>
        /// <returns></returns>
        public Boolean TryAdd(TKey key)
        {
            RedBlackTreeNode<TKey> result;

            RootNode = TryAdd(RootNode, key, out result);
            RootNode.IsBlack = true;

            return result != null;
        }

        /// <summary>
        /// Removes a key/value pair from the tree.
        /// </summary>
        /// <param name="key">Key to remove.</param>
        /// <returns>True if key/value present and removed.</returns>
        public Boolean Remove(TKey key)
        {
            var initialCount = Count;

            if (null != RootNode)
            {
                RootNode = Remove(RootNode, key);

                if (null != RootNode)
                {
                    RootNode.IsBlack = true;
                }
            }

            return initialCount != Count;
        }

        /// <summary>
        /// Removes all nodes in the tree.
        /// </summary>
        public void Clear()
        {
            RootNode = null;
            Count = 0;
        }

        /// <summary>
        /// Gets a sorted list of keys in the tree.
        /// </summary>
        /// <returns>Sorted list of keys.</returns>
        public IEnumerable<TKey> Keys
        {
            get
            {
                return EnumerateFromLeftToRight().Select(node => node.Key);
            }
        }

        private RedBlackTreeNode<TKey> FindNode(TKey key)
        {
            // Initialize
            var node = RootNode;

            while (null != node)
            {
                // Compare keys and go left/right
                var comparisonResult = Comparer.Compare(key, node.Key);

                if (comparisonResult < 0)
                {
                    node = node.Left;
                }
                else if (0 < comparisonResult)
                {
                    node = node.Right;
                }
                else
                {
                    // Match; return node
                    return node;
                }
            }

            // No match found
            return null;
        }

        public Boolean ContainsKey(TKey key)
        {
            var node = FindNode(key);

            return node != null;
        }


        public TKey NextKey(TKey key)
        {
            var node = SiblingNodes(key).B;

            return node == null ? default(TKey) : node.Key;
        }

        public TKey PreviousKey(TKey key)
        {
            var node = SiblingNodes(key).A;

            return node == null ? default(TKey) : node.Key;
        }

        
        public Pair<TKey> SiblingKeys(TKey key)
        {
            var nodes = SiblingNodes(key);

            if (nodes.A != null && nodes.B != null)
            {
                return Pair<TKey>.FromAB(nodes.A.Key, nodes.B.Key);
            }
            
            if (nodes.A != null)
            {
                return Pair<TKey>.FromA(nodes.A.Key);
            }
            
            if (nodes.B != null)
            {
                return Pair<TKey>.FromB(nodes.B.Key);
            }

            return Pair<TKey>.Empty;
        }

        private RawPair<RedBlackTreeNode<TKey>> SiblingNodes(TKey key)
        {
            if (IsEmpty)
                return default(RawPair<RedBlackTreeNode<TKey>>);

            var leftmost = true;
            var rightmost = true;

            var nodeA = RootNode;
            var nodeB = RootNode;
            var nodeC = RootNode;

            var comparison = Comparer.Compare(key, nodeA.Key);

            while (true)
            {
                if (comparison < 0)
                {
                    rightmost = false;

                    if (nodeA.Left != null)
                    {
                        nodeC = nodeA;
                        nodeA = nodeA.Left;
                    }
                    else
                    {
                        if (leftmost)
                        {
                            return new RawPair<RedBlackTreeNode<TKey>>(null, nodeA);
                        }

                        if (Comparer.Compare(nodeA.Key, nodeB.Key) < 0 ||
                            (Comparer.Compare(nodeA.Key, nodeC.Key) < 0 && 0 < comparison))
                        {
                            return new RawPair<RedBlackTreeNode<TKey>>(nodeA, nodeC);
                        }

                        return new RawPair<RedBlackTreeNode<TKey>>(nodeB, nodeA);
                    }
                }
                else if (comparison > 0)
                {
                    leftmost = false;

                    if (nodeA.Right != null)
                    {
                        nodeB = nodeA;
                        nodeA = nodeA.Right;
                    }
                    else
                    {
                        if (rightmost)
                        {
                            return new RawPair<RedBlackTreeNode<TKey>>(nodeA, null);
                        }

                        if (Comparer.Compare(nodeA.Key, nodeB.Key) < 0 ||
                            (Comparer.Compare(nodeA.Key, nodeC.Key) < 0 && 0 < comparison))
                        {
                            return new RawPair<RedBlackTreeNode<TKey>>(nodeA, nodeC);
                        }

                        return new RawPair<RedBlackTreeNode<TKey>>(nodeB, nodeA);
                    }
                }
                else
                {
                    if (nodeA.Left != null)
                    {
                        leftmost = false;
                    }

                    if (nodeA.Right != null)
                    {
                        rightmost = false;
                    }

                    if (nodeA.Left != null)
                    {
                        if (nodeA.Right != null)
                        {
                            return new RawPair<RedBlackTreeNode<TKey>>(GetMaximumNode(nodeA.Left),
                                GetMinimumNode(nodeA.Right));
                        }

                        if (Comparer.Compare(nodeA.Key, nodeB.Key) < 0 || Comparer.Compare(nodeA.Key, nodeC.Key) < 0)
                        {
                            return new RawPair<RedBlackTreeNode<TKey>>(GetMaximumNode(nodeA.Left), nodeC);
                        }

                        return new RawPair<RedBlackTreeNode<TKey>>(GetMaximumNode(nodeA.Left), null);
                    }

                    if (leftmost && rightmost)
                    {
                        return new RawPair<RedBlackTreeNode<TKey>>(null, null);
                    }

                    if (leftmost)
                    {
                        return new RawPair<RedBlackTreeNode<TKey>>(null, nodeC);
                    }

                    if (rightmost)
                    {
                        return new RawPair<RedBlackTreeNode<TKey>>(nodeB, null);
                    }

                    return new RawPair<RedBlackTreeNode<TKey>>(nodeB, nodeC);
                }

                comparison = Comparer.Compare(key, nodeA.Key);
            }
        }

        
        public Pair<TKey> NearestKeys(TKey key)
        {
            var nodes = NearestNodes(key);

            if (nodes.A != null && nodes.B != null)
            {
                return Pair<TKey>.FromAB(nodes.A.Key, nodes.B.Key);
            }

            if (nodes.A != null)
            {
                return Pair<TKey>.FromA(nodes.A.Key);
            }

            if (nodes.B != null)
            {
                return Pair<TKey>.FromB(nodes.B.Key);
            }

            return Pair<TKey>.Empty;
        }

        private RawPair<RedBlackTreeNode<TKey>> NearestNodes(TKey key)
        {
            if (IsEmpty)
                return default(RawPair<RedBlackTreeNode<TKey>>);

            var leftmost = true;
            var rightmost = true;

            var nodeA = RootNode;
            var nodeB = RootNode;
            var nodeC = RootNode;

            var comparison = Comparer.Compare(key, nodeA.Key);

            while (true)
            {
                if (comparison < 0)
                {
                    rightmost = false;

                    if (nodeA.Left != null)
                    {
                        nodeC = nodeA;
                        nodeA = nodeA.Left;
                    }
                    else
                    {
                        if (leftmost)
                        {
                            return new RawPair<RedBlackTreeNode<TKey>>(null, nodeA);
                        }

                        if (Comparer.Compare(nodeA.Key, nodeB.Key) < 0 ||
                            (Comparer.Compare(nodeA.Key, nodeC.Key) < 0 && 0 < comparison))
                        {
                            return new RawPair<RedBlackTreeNode<TKey>>(nodeA, nodeC);
                        }

                        return new RawPair<RedBlackTreeNode<TKey>>(nodeB, nodeA);
                    }
                }
                else if (comparison > 0)
                {
                    leftmost = false;

                    if (nodeA.Right != null)
                    {
                        nodeB = nodeA;
                        nodeA = nodeA.Right;
                    }
                    else
                    {
                        if (rightmost)
                        {
                            return new RawPair<RedBlackTreeNode<TKey>>(nodeA, null);
                        }

                        if (Comparer.Compare(nodeA.Key, nodeB.Key) < 0 ||
                            (Comparer.Compare(nodeA.Key, nodeC.Key) < 0 && 0 < comparison))
                        {
                            return new RawPair<RedBlackTreeNode<TKey>>(nodeA, nodeC);
                        }

                        return new RawPair<RedBlackTreeNode<TKey>>(nodeB, nodeA);
                    }
                }
                else
                {
                    return new RawPair<RedBlackTreeNode<TKey>>(nodeA, nodeA);
                }

                comparison = Comparer.Compare(key, nodeA.Key);
            }
        }


        public TKey GetMinimumKey()
        {
            var node = GetMinimumNode(RootNode);

            return node == null ? default(TKey) : node.Key;
        }

        public TKey GetMaximumKey()
        {
            var node = GetMaximumNode(RootNode);

            return node == null ? default(TKey) : node.Key;
        }

        public void RemoveMinimum()
        {
            var node = GetMaximumNode(RootNode);

            if (node != null) Remove(node.Key);
        }

        public TKey RemoveMinimumKey()
        {
            var node = GetMinimumNode(RootNode);

            if (node != null)
            {
                Remove(node.Key);

                return node.Key;
            }

            return default(TKey);
        }

        public void RemoveMaximum()
        {
            var node = GetMaximumNode(RootNode);

            if (node != null) Remove(node.Key);
        }

        public TKey RemoveMaximumKey()
        {
            var node = GetMaximumNode(RootNode);

            if (node != null)
            {
                Remove(node.Key);

                return node.Key;
            }

            return default(TKey);
        }

        public IEnumerator<TKey> GetEnumerator()
        {
            return EnumerateFromLeftToRight().Select(x => x.Key).GetEnumerator();
        }

        private RedBlackTreeNode<TKey> Add(RedBlackTreeNode<TKey> node, TKey key, out RedBlackTreeNode<TKey> result)
        {
            if (node == null)
            {
                // Insert new node
                Count++;
                return result = new RedBlackTreeNode<TKey> {Key = key};
            }

            if (IsRed(node.Left) && IsRed(node.Right))
            {
                // Split node with two red children
                FlipColor(node);
            }

            // Find right place for new node
            Int32 comparisonResult = Comparer.Compare(key, node.Key);

            if (comparisonResult < 0)
            {
                node.Left = Add(node.Left, key, out result);
            }
            else if (0 < comparisonResult)
            {
                node.Right = Add(node.Right, key, out result);
            }
            else
            {
                // Replace the value of the existing node
                throw new ArgumentException("Tree already contains argument1 root");
            }

            if (IsRed(node.Right))
            {
                // Rotate to prevent red node on right
                node = RotateLeft(node);
            }

            if (IsRed(node.Left) && IsRed(node.Left.Left))
            {
                // Rotate to prevent consecutive red nodes
                node = RotateRight(node);
            }

            return node;
        }

        private RedBlackTreeNode<TKey> TryAdd(RedBlackTreeNode<TKey> node, TKey key, out RedBlackTreeNode<TKey> result)
        {
            if (node == null)
            {
                // Insert new node
                Count++;
                return result = new RedBlackTreeNode<TKey> {Key = key};
            }

            if (IsRed(node.Left) && IsRed(node.Right))
            {
                // Split node with two red children
                FlipColor(node);
            }

            // Find right place for new node
            var comparisonResult = Comparer.Compare(key, node.Key);

            if (comparisonResult < 0)
            {
                node.Left = TryAdd(node.Left, key, out result);
            }
            else if (0 < comparisonResult)
            {
                node.Right = TryAdd(node.Right, key, out result);
            }
            else
            {
                // Value already exists, return 'null' to user
                result = null;

                return node;
            }

            if (IsRed(node.Right))
            {
                // Rotate to prevent red node on right
                node = RotateLeft(node);
            }

            if (IsRed(node.Left) && IsRed(node.Left.Left))
            {
                // Rotate to prevent consecutive red nodes
                node = RotateRight(node);
            }

            return node;
        }

        /// <summary>
        /// Removes the specified key/value pair from below the specified node.
        /// </summary>
        /// <param name="node">Specified node.</param>
        /// <param name="key">Key to remove.</param>
        /// <returns>True if key/value present and removed.</returns>
        private RedBlackTreeNode<TKey> Remove(RedBlackTreeNode<TKey> node, TKey key)
        {
            Int32 comparisonResult = Comparer.Compare(key, node.Key);

            if (comparisonResult < 0)
            {
                // * Continue search if left is present
                if (null != node.Left)
                {
                    if (!IsRed(node.Left) && !IsRed(node.Left.Left))
                    {
                        // Move a red node over
                        node = MoveRedLeft(node);
                    }

                    // Remove from left
                    node.Left = Remove(node.Left, key);
                }
            }
            else
            {
                if (IsRed(node.Left))
                {
                    // Flip a 3 node or unbalance a 4 node
                    node = RotateRight(node);
                }
                if ((0 == Comparer.Compare(key, node.Key)) && (null == node.Right))
                {
                    // Remove leaf node
                    Count--;

                    return null;
                }
                // * Continue search if right is present
                if (null != node.Right)
                {
                    if (!IsRed(node.Right) && !IsRed(node.Right.Left))
                    {
                        // Move a red node over
                        node = MoveRedRight(node);
                    }
                    if (0 == Comparer.Compare(key, node.Key))
                    {
                        // Remove leaf node
                        Count--;

                        // Find the smallest node on the right, swap, and remove it
                        var m = GetMinimumNode(node.Right);

                        node.Key = m.Key;
                        node.Right = DeleteMinimum(node.Right);
                    }
                    else
                    {
                        // Remove from right
                        node.Right = Remove(node.Right, key);
                    }
                }
            }

            // MaInt32ain invariants
            return FixUp(node);
        }

        /// <summary>
        /// Traverses a subset of the sequence of nodes in order and selects the specified nodes.
        /// </summary>
        /// <returns>Sequence of selected nodes.</returns>
        private IEnumerable<RedBlackTreeNode<TKey>> EnumerateFromLeftToRight()
        {
            var node = RootNode;

            var stack = new Stack<RedBlackTreeNode<TKey>>();

            while (node != null)
            {
                if (node.Left != null)
                {
                    stack.Push(node);
                    node = node.Left;
                }
                else
                {
                    while (true)
                    {
                        yield return node;

                        node = node.Right;

                        if (node == null && stack.Count > 0)
                        {
                            node = stack.Pop();
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }
        private IEnumerable<RedBlackTreeNode<TKey>> EnumerateFromLeftToRight(TKey minimum)
        {
            var node = RootNode;

            var stack = new Stack<RedBlackTreeNode<TKey>>();

            while (node != null)
            {
                var comparison = Comparer.Compare(minimum, node.Key);

                if (comparison <= 0)
                {
                    if (node.Left != null)
                    {
                        stack.Push(node);
                        node = node.Left;
                    }
                    else
                    {
                        while (true)
                        {
                            yield return node;

                            node = node.Right;

                            if (node == null && stack.Count > 0)
                            {
                                node = stack.Pop();
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                else
                {
                    node = node.Right;

                    if (node == null && stack.Count > 0)
                    {
                        node = stack.Pop();

                        while (true)
                        {
                            yield return node;

                            node = node.Right;

                            if (node == null && stack.Count > 0)
                            {
                                node = stack.Pop();
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }
        private IEnumerable<RedBlackTreeNode<TKey>> EnumerateFromLeftToRight(TKey minimum, TKey maximum)
        {
            var node = RootNode;

            var stack = new Stack<RedBlackTreeNode<TKey>>();

            while (node != null)
            {
                var comparison = Comparer.Compare(minimum, node.Key);

                if (comparison <= 0)
                {
                    if (node.Left != null)
                    {
                        stack.Push(node);
                        node = node.Left;
                    }
                    else
                    {
                        var @break = false;

                        while (true)
                        {
                            comparison = Comparer.Compare(node.Key, maximum);

                            if (comparison < 0)
                                // node.Key < maximum
                            {
                                yield return node;

                                node = node.Right;

                                if (node == null && stack.Count > 0)
                                {
                                    node = stack.Pop();
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else if (comparison == 0)
                                // node.Key == maximum
                            {
                                yield return node;

                                @break = true;
                                break;
                            }
                            else
                                // node.Key > maximum
                            {
                                @break = true;
                                break;
                            }
                        }

                        if (@break) break;
                    }
                }
                else
                {
                    node = node.Right;

                    if (node == null && stack.Count > 0)
                    {
                        node = stack.Pop();

                        var @break = false;

                        while (true)
                        {
                            comparison = Comparer.Compare(node.Key, maximum);

                            if (comparison < 0)
                            {
                                yield return node;

                                node = node.Right;

                                if (node == null && stack.Count > 0)
                                {
                                    node = stack.Pop();
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else if (comparison == 0)
                            {
                                yield return node;

                                @break = true;
                                break;
                            }
                            else
                            {
                                @break = true;
                                break;
                            }
                        }

                        if (@break) break;
                    }
                }
            }
        }
        private IEnumerable<RedBlackTreeNode<TKey>> EnumerateFromRightToLeft()
        {
            var node = RootNode;

            var stack = new Stack<RedBlackTreeNode<TKey>>();

            while (node != null)
            {
                if (node.Right != null)
                {
                    stack.Push(node);
                    node = node.Right;
                }
                else
                {
                    while (true)
                    {
                        yield return node;

                        node = node.Left;

                        if (node == null && stack.Count > 0)
                        {
                            node = stack.Pop();
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }
        private IEnumerable<RedBlackTreeNode<TKey>> EnumerateFromRightToLeft(TKey maximum)
        {
            var node = RootNode;

            var stack = new Stack<RedBlackTreeNode<TKey>>();

            while (node != null)
            {
                var comparison = Comparer.Compare(maximum, node.Key);

                if (comparison >= 0)
                {
                    if (node.Right != null)
                    {
                        stack.Push(node);
                        node = node.Right;
                    }
                    else
                    {
                        while (true)
                        {
                            yield return node;

                            node = node.Left;

                            if (node == null && stack.Count > 0)
                            {
                                node = stack.Pop();
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                else
                {
                    node = node.Left;

                    if (node == null && stack.Count > 0)
                    {
                        node = stack.Pop();

                        while (true)
                        {
                            yield return node;

                            node = node.Left;

                            if (node == null && stack.Count > 0)
                            {
                                node = stack.Pop();
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }
        private IEnumerable<RedBlackTreeNode<TKey>> EnumerateFromRightToLeft(TKey minimum, TKey maximum)
        {
            var node = RootNode;

            var stack = new Stack<RedBlackTreeNode<TKey>>();

            while (node != null)
            {
                var comparison = Comparer.Compare(maximum, node.Key);

                if (comparison >= 0)
                {
                    if (node.Right != null)
                    {
                        stack.Push(node);
                        node = node.Right;
                    }
                    else
                    {
                        var @break = false;

                        while (true)
                        {
                            comparison = Comparer.Compare(node.Key, minimum);

                            if (comparison > 0)
                            // node.Key > minimum
                            {
                                yield return node;

                                node = node.Left;

                                if (node == null && stack.Count > 0)
                                {
                                    node = stack.Pop();
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else if (comparison == 0)
                            // node.Key == minimum
                            {
                                yield return node;

                                @break = true;
                                break;
                            }
                            else
                            // node.Key < minimum
                            {
                                @break = true;
                                break;
                            }
                        }

                        if (@break) break;
                    }
                }
                else
                {
                    node = node.Left;

                    if (node == null && stack.Count > 0)
                    {
                        node = stack.Pop();

                        var @break = false;

                        while (true)
                        {
                            comparison = Comparer.Compare(node.Key, minimum);

                            if (comparison > 0)
                            {
                                yield return node;

                                node = node.Left;

                                if (node == null && stack.Count > 0)
                                {
                                    node = stack.Pop();
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else if (comparison == 0)
                            {
                                yield return node;

                                @break = true;
                                break;
                            }
                            else
                            {
                                @break = true;
                                break;
                            }
                        }

                        if (@break) break;
                    }
                }
            }
        }

        public IEnumerable<TKey> EnumerateKeysFromLeftToRight()
        {
            return EnumerateFromLeftToRight().Select(x => x.Key);
        }
        public IEnumerable<TKey> EnumerateKeysFromRightToLeft()
        {
            return EnumerateFromRightToLeft().Select(x => x.Key);
        }
        public IEnumerable<TKey> EnumerateKeysFromLeftToRight(TKey minimum)
        {
            return EnumerateFromLeftToRight(minimum).Select(x => x.Key);
        }
        public IEnumerable<TKey> EnumerateKeysFromRightToLeft(TKey maximum)
        {
            return EnumerateFromRightToLeft(maximum).Select(x => x.Key);
        }
        public IEnumerable<TKey> EnumerateKeysFromLeftToRight(TKey minimum, TKey maximum)
        {
            return EnumerateFromLeftToRight(minimum, maximum).Select(x => x.Key);
        }
        public IEnumerable<TKey> EnumerateKeysFromRightToLeft(TKey minimum, TKey maximum)
        {
            return EnumerateFromRightToLeft(minimum, maximum).Select(x => x.Key);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return EnumerateFromLeftToRight().GetEnumerator();
        }
    }
}