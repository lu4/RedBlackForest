using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace RedBlackForest
{
    public partial class RedBlackTree<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        /// <summary>
        /// Stores the root node of the tree.
        /// </summary>
        private RedBlackTreeNode<TKey, TValue> RootNode { get; set; }

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
        public void Add(TKey key, TValue value)
        {
            RedBlackTreeNode<TKey, TValue> result;
            RootNode = Add(RootNode, key, value, out result);
            RootNode.IsBlack = true;
        }

        /// <summary>
        /// Adds a value to the tree
        /// </summary>
        /// <remarks>When adding element that exists in collection, this method not throws </remarks>
        /// <param name="key">Key to add.</param>
        /// <param name="value">Value to add.</param>
        /// <returns></returns>
        public Boolean TryAdd(TKey key, TValue value)
        {
            RedBlackTreeNode<TKey, TValue> result;

            RootNode = TryAdd(RootNode, key, value, out result);
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

        /// <summary>
        /// Gets a sequence of all the values in the tree.
        /// </summary>
        /// <returns>Sequence of all values.</returns>
        public IEnumerable<TValue> Values
        {
            get
            {
                return EnumerateFromLeftToRight().Select(node => node.Value);
            }
        }

        public IEnumerable<KeyValuePair<TKey, TValue>> Pairs
        {
            get
            {
                return EnumerateFromLeftToRight().Select(node => node.KeyValuePair);
            }
        }

        private RedBlackTreeNode<TKey, TValue> FindNode(TKey key)
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

        /// <summary>
        /// Gets the value associated with the specified key in a tree.
        /// </summary>
        /// <param name="key">Specified key.</param>
        /// <returns>Value associated with the specified key.</returns>
        public TValue this[TKey key]
        {
            get
            {
                var node = FindNode(key);

                if (null != node)
                {
                    return node.Value;
                }

                throw new KeyNotFoundException();
            }
            set
            {
                RootNode = Set(RootNode, key, value);
                RootNode.IsBlack = true;
            }
        }

        public TValue TryGetValue(TKey key)
        {
            var node = FindNode(key);

            if (null != node)
            {
                return node.Value;
            }

            return default(TValue);
        }

        public TValue TryGetValue(TKey key, TValue defaultValue)
        {
            var node = FindNode(key);

            if (null != node)
            {
                return node.Value;
            }

            return defaultValue;
        }

        public TValue TryGetValue(TKey key, Func<TValue> defaultValue)
        {
            var node = FindNode(key);

            if (null != node)
            {
                return node.Value;
            }

            return defaultValue();
        }

        public Boolean TryGetValue(TKey key, out TValue value)
        {
            var node = FindNode(key);

            if (null != node)
            {
                value = node.Value;

                return true;
            }

            value = default(TValue);
            return false;
        }


        public TKey NextKey(TKey key)
        {
            var node = SiblingNodes(key).B;

            return node == null ? default(TKey) : node.Key;
        }

        public TValue NextValue(TKey key)
        {
            var node = SiblingNodes(key).B;

            return node == null ? default(TValue) : node.Value;
        }

        public KeyValuePair<TKey, TValue>? NextPair(TKey key)
        {
            var node = SiblingNodes(key).B;

            return node == null ? default(KeyValuePair<TKey, TValue>?) : node.KeyValuePair;
        }

        public TKey PreviousKey(TKey key)
        {
            var node = SiblingNodes(key).A;

            return node == null ? default(TKey) : node.Key;
        }

        public TValue PreviousValue(TKey key)
        {
            var node = SiblingNodes(key).A;

            return node == null ? default(TValue) : node.Value;
        }

        public KeyValuePair<TKey, TValue>? PreviousPair(TKey key)
        {
            var node = SiblingNodes(key).A;

            return node == null ? default(KeyValuePair<TKey, TValue>?) : node.KeyValuePair;
        }


        public Pair<TKey> SiblingKeys(TKey key)
        {
            var nodes = SiblingNodes(key);

            return new Pair<TKey>
            (
                nodes.A == null ? default(TKey) : nodes.A.Key,
                nodes.B == null ? default(TKey) : nodes.B.Key
            );
        }

        public Pair<TValue> SiblingValues(TKey key)
        {
            var nodes = SiblingNodes(key);

            return new Pair<TValue>
                (
                nodes.A == null ? default(TValue) : nodes.A.Value,
                nodes.B == null ? default(TValue) : nodes.B.Value
                );
        }

        public Pair<KeyValuePair<TKey, TValue>?> SiblingPairs(TKey key)
        {
            var nodes = SiblingNodes(key);

            return new Pair<KeyValuePair<TKey, TValue>?>
            (
                nodes.A == null ? default(KeyValuePair<TKey, TValue>?) : nodes.A.KeyValuePair,
                nodes.B == null ? default(KeyValuePair<TKey, TValue>?) : nodes.B.KeyValuePair
            );
        }

        private Pair<RedBlackTreeNode<TKey, TValue>> SiblingNodes(TKey key)
        {
            if (IsEmpty)
                return default(Pair<RedBlackTreeNode<TKey, TValue>>);

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
                            return new Pair<RedBlackTreeNode<TKey, TValue>>(null, nodeA);
                        }

                        if (Comparer.Compare(nodeA.Key, nodeB.Key) < 0 ||
                            (Comparer.Compare(nodeA.Key, nodeC.Key) < 0 && 0 < comparison))
                        {
                            return new Pair<RedBlackTreeNode<TKey, TValue>>(nodeA, nodeC);
                        }

                        return new Pair<RedBlackTreeNode<TKey, TValue>>(nodeB, nodeA);
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
                            return new Pair<RedBlackTreeNode<TKey, TValue>>(nodeA, null);
                        }

                        if (Comparer.Compare(nodeA.Key, nodeB.Key) < 0 ||
                            (Comparer.Compare(nodeA.Key, nodeC.Key) < 0 && 0 < comparison))
                        {
                            return new Pair<RedBlackTreeNode<TKey, TValue>>(nodeA, nodeC);
                        }

                        return new Pair<RedBlackTreeNode<TKey, TValue>>(nodeB, nodeA);
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
                            return new Pair<RedBlackTreeNode<TKey, TValue>>(GetMaximumNode(nodeA.Left),
                                GetMinimumNode(nodeA.Right));
                        }

                        if (Comparer.Compare(nodeA.Key, nodeB.Key) < 0 || Comparer.Compare(nodeA.Key, nodeC.Key) < 0)
                        {
                            return new Pair<RedBlackTreeNode<TKey, TValue>>(GetMaximumNode(nodeA.Left), nodeC);
                        }

                        return new Pair<RedBlackTreeNode<TKey, TValue>>(GetMaximumNode(nodeA.Left), null);
                    }

                    if (leftmost && rightmost)
                    {
                        return new Pair<RedBlackTreeNode<TKey, TValue>>(null, null);
                    }

                    if (leftmost)
                    {
                        return new Pair<RedBlackTreeNode<TKey, TValue>>(null, nodeC);
                    }

                    if (rightmost)
                    {
                        return new Pair<RedBlackTreeNode<TKey, TValue>>(nodeB, null);
                    }

                    return new Pair<RedBlackTreeNode<TKey, TValue>>(nodeB, nodeC);
                }

                comparison = Comparer.Compare(key, nodeA.Key);
            }
        }


        public Pair<TKey> NearestKeys(TKey key)
        {
            var nodes = NearestNodes(key);

            return new Pair<TKey>
                (
                nodes.A == null ? default(TKey) : nodes.A.Key,
                nodes.B == null ? default(TKey) : nodes.B.Key
                );
        }

        public Pair<TValue> NearestValues(TKey key)
        {
            var nodes = NearestNodes(key);

            return new Pair<TValue>
                (
                nodes.A == null ? default(TValue) : nodes.A.Value,
                nodes.B == null ? default(TValue) : nodes.B.Value
                );
        }

        public Pair<KeyValuePair<TKey, TValue>?> NearestPairs(TKey key)
        {
            var nodes = NearestNodes(key);

            return new Pair<KeyValuePair<TKey, TValue>?>
            (
                nodes.A == null ? default(KeyValuePair<TKey, TValue>?) : nodes.A.KeyValuePair,
                nodes.B == null ? default(KeyValuePair<TKey, TValue>?) : nodes.B.KeyValuePair
            );
        }

        private Pair<RedBlackTreeNode<TKey, TValue>> NearestNodes(TKey key)
        {
            if (IsEmpty)
                return default(Pair<RedBlackTreeNode<TKey, TValue>>);

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
                            return new Pair<RedBlackTreeNode<TKey, TValue>>(null, nodeA);
                        }

                        if (Comparer.Compare(nodeA.Key, nodeB.Key) < 0 ||
                            (Comparer.Compare(nodeA.Key, nodeC.Key) < 0 && 0 < comparison))
                        {
                            return new Pair<RedBlackTreeNode<TKey, TValue>>(nodeA, nodeC);
                        }

                        return new Pair<RedBlackTreeNode<TKey, TValue>>(nodeB, nodeA);
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
                            return new Pair<RedBlackTreeNode<TKey, TValue>>(nodeA, null);
                        }

                        if (Comparer.Compare(nodeA.Key, nodeB.Key) < 0 ||
                            (Comparer.Compare(nodeA.Key, nodeC.Key) < 0 && 0 < comparison))
                        {
                            return new Pair<RedBlackTreeNode<TKey, TValue>>(nodeA, nodeC);
                        }

                        return new Pair<RedBlackTreeNode<TKey, TValue>>(nodeB, nodeA);
                    }
                }
                else
                {
                    return new Pair<RedBlackTreeNode<TKey, TValue>>(nodeA, nodeA);
                }

                comparison = Comparer.Compare(key, nodeA.Key);
            }
        }


        public TKey GetMinimumKey()
        {
            var node = GetMinimumNode(RootNode);

            return node == null ? default(TKey) : node.Key;
        }

        public TValue GetMinimumValue()
        {
            var node = GetMinimumNode(RootNode);

            return node == null ? default(TValue) : node.Value;
        }

        public KeyValuePair<TKey, TValue> GetMinimumPair()
        {
            var node = GetMinimumNode(RootNode);

            return node == null ? default(KeyValuePair<TKey, TValue>) : node.KeyValuePair;
        }

        public TKey GetMaximumKey()
        {
            var node = GetMaximumNode(RootNode);

            return node == null ? default(TKey) : node.Key;
        }

        public TValue GetMaximumValue()
        {
            var node = GetMaximumNode(RootNode);

            return node == null ? default(TValue) : node.Value;
        }

        public KeyValuePair<TKey, TValue> GetMaximumPair()
        {
            var node = GetMaximumNode(RootNode);

            return node == null ? default(KeyValuePair<TKey, TValue>) : node.KeyValuePair;
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

        public TValue RemoveMinimumValue()
        {
            var node = GetMinimumNode(RootNode);

            if (node != null)
            {
                Remove(node.Key);

                return node.Value;
            }

            return default(TValue);
        }

        public KeyValuePair<TKey, TValue>? RemoveMinimumPair()
        {
            var node = GetMinimumNode(RootNode);

            if (node != null)
            {
                Remove(node.Key);

                return node.KeyValuePair;
            }

            return default(KeyValuePair<TKey, TValue>?);
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

        public TValue RemoveMaximumValue()
        {
            var node = GetMaximumNode(RootNode);

            if (node != null)
            {
                Remove(node.Key);

                return node.Value;
            }

            return default(TValue);
        }

        public KeyValuePair<TKey, TValue>? RemoveMaximumPair()
        {
            var node = GetMaximumNode(RootNode);

            if (node != null)
            {
                Remove(node.Key);

                return node.KeyValuePair;
            }

            return default(KeyValuePair<TKey, TValue>?);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return EnumerateFromLeftToRight().Select(x => x.KeyValuePair).GetEnumerator();
        }

        private RedBlackTreeNode<TKey, TValue> Add(RedBlackTreeNode<TKey, TValue> node, TKey key, TValue value, out RedBlackTreeNode<TKey, TValue> result)
        {
            if (node == null)
            {
                // Insert new node
                Count++;
                return result = new RedBlackTreeNode<TKey, TValue> {Key = key, Value = value};
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
                node.Left = Add(node.Left, key, value, out result);
            }
            else if (0 < comparisonResult)
            {
                node.Right = Add(node.Right, key, value, out result);
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

        private RedBlackTreeNode<TKey, TValue> TryAdd(RedBlackTreeNode<TKey, TValue> node, TKey key, TValue value, out RedBlackTreeNode<TKey, TValue> result)
        {
            if (node == null)
            {
                // Insert new node
                Count++;
                return result = new RedBlackTreeNode<TKey, TValue> {Key = key, Value = value};
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
                node.Left = TryAdd(node.Left, key, value, out result);
            }
            else if (0 < comparisonResult)
            {
                node.Right = TryAdd(node.Right, key, value, out result);
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
        /// Adds the specified key/value pair below the specified root node.
        /// </summary>
        /// <param name="node">Specified node.</param>
        /// <param name="key">Key to add.</param>
        /// <param name="value">Value to add.</param>
        /// <returns>New root node.</returns>
        private RedBlackTreeNode<TKey, TValue> Set(RedBlackTreeNode<TKey, TValue> node, TKey key, TValue value)
        {
            if (null == node)
            {
                // Insert new node
                Count++;
                return new RedBlackTreeNode<TKey, TValue> {Key = key, Value = value};
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
                node.Left = Set(node.Left, key, value);
            }
            else if (0 < comparisonResult)
            {
                node.Right = Set(node.Right, key, value);
            }
            else
            {
                // Replace the value of the existing node
                node.Value = value;
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
        private RedBlackTreeNode<TKey, TValue> Remove(RedBlackTreeNode<TKey, TValue> node, TKey key)
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
                        node.Value = m.Value;
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
        private IEnumerable<RedBlackTreeNode<TKey, TValue>> EnumerateFromLeftToRight()
        {
            var node = RootNode;

            var stack = new Stack<RedBlackTreeNode<TKey, TValue>>();

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
        private IEnumerable<RedBlackTreeNode<TKey, TValue>> EnumerateFromLeftToRight(TKey minimum)
        {
            var node = RootNode;

            var stack = new Stack<RedBlackTreeNode<TKey, TValue>>();

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
        private IEnumerable<RedBlackTreeNode<TKey, TValue>> EnumerateFromLeftToRight(TKey minimum, TKey maximum)
        {
            var node = RootNode;

            var stack = new Stack<RedBlackTreeNode<TKey, TValue>>();

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
        private IEnumerable<RedBlackTreeNode<TKey, TValue>> EnumerateFromRightToLeft()
        {
            var node = RootNode;

            var stack = new Stack<RedBlackTreeNode<TKey, TValue>>();

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
        private IEnumerable<RedBlackTreeNode<TKey, TValue>> EnumerateFromRightToLeft(TKey maximum)
        {
            var node = RootNode;

            var stack = new Stack<RedBlackTreeNode<TKey, TValue>>();

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
        private IEnumerable<RedBlackTreeNode<TKey, TValue>> EnumerateFromRightToLeft(TKey minimum, TKey maximum)
        {
            var node = RootNode;

            var stack = new Stack<RedBlackTreeNode<TKey, TValue>>();

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

        public IEnumerable<KeyValuePair<TKey, TValue>> EnumeratePairsFromLeftToRight()
        {
            return EnumerateFromLeftToRight().Select(x => x.KeyValuePair);
        }
        public IEnumerable<KeyValuePair<TKey, TValue>> EnumeratePairsFromRightToLeft()
        {
            return EnumerateFromRightToLeft().Select(x => x.KeyValuePair);
        }
        public IEnumerable<KeyValuePair<TKey, TValue>> EnumeratePairsFromLeftToRight(TKey minimum)
        {
            return EnumerateFromLeftToRight(minimum).Select(x => x.KeyValuePair);
        }
        public IEnumerable<KeyValuePair<TKey, TValue>> EnumeratePairsFromRightToLeft(TKey maximum)
        {
            return EnumerateFromRightToLeft(maximum).Select(x => x.KeyValuePair);
        }
        public IEnumerable<KeyValuePair<TKey, TValue>> EnumeratePairsFromLeftToRight(TKey minimum, TKey maximum)
        {
            return EnumerateFromLeftToRight(minimum, maximum).Select(x => x.KeyValuePair);
        }
        public IEnumerable<KeyValuePair<TKey, TValue>> EnumeratePairsFromRightToLeft(TKey minimum, TKey maximum)
        {
            return EnumerateFromRightToLeft(minimum, maximum).Select(x => x.KeyValuePair);
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

        public IEnumerable<TValue> EnumerateValuesFromLeftToRight()
        {
            return EnumerateFromLeftToRight().Select(x => x.Value);
        }
        public IEnumerable<TValue> EnumerateValuesFromRightToLeft()
        {
            return EnumerateFromRightToLeft().Select(x => x.Value);
        }
        public IEnumerable<TValue> EnumerateValuesFromLeftToRight(TKey minimum)
        {
            return EnumerateFromLeftToRight(minimum).Select(x => x.Value);
        }
        public IEnumerable<TValue> EnumerateValuesFromRightToLeft(TKey maximum)
        {
            return EnumerateFromRightToLeft(maximum).Select(x => x.Value);
        }
        public IEnumerable<TValue> EnumerateValuesFromLeftToRight(TKey minimum, TKey maximum)
        {
            return EnumerateFromLeftToRight(minimum, maximum).Select(x => x.Value);
        }
        public IEnumerable<TValue> EnumerateValuesFromRightToLeft(TKey minimum, TKey maximum)
        {
            return EnumerateFromRightToLeft(minimum, maximum).Select(x => x.Value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return EnumerateFromLeftToRight().GetEnumerator();
        }

        public TValue Set(TKey key, TValue value)
        {
            RedBlackTreeNode<TKey, TValue> result;

            RootNode = TryGetOrAddValue(RootNode, key, value, out result);
            RootNode.IsBlack = true;

            result.Value = value;

            return result.Value;
        }

        public TValue TryGetOrAddValue(TKey key, TValue value)
        {
            RedBlackTreeNode<TKey, TValue> result;

            RootNode = TryGetOrAddValue(RootNode, key, value, out result);
            RootNode.IsBlack = true;

            return result.Value;
        }
        public TValue TryGetOrAddValue(TKey key, Func<TValue> value)
        {
            RedBlackTreeNode<TKey, TValue> result;

            RootNode = TryGetOrAddValue(RootNode, key, value, out result);
            RootNode.IsBlack = true;

            return result.Value;
        }

        private RedBlackTreeNode<TKey, TValue> TryGetOrAddValue(RedBlackTreeNode<TKey, TValue> node, TKey key, TValue value, out RedBlackTreeNode<TKey, TValue> result)
        {
            if (node == null)
            {
                // Insert new node
                Count++;

                return result = new RedBlackTreeNode<TKey, TValue> { Key = key, Value = value };
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
                node.Left = TryGetOrAddValue(node.Left, key, value, out result);
            }
            else if (0 < comparisonResult)
            {
                node.Right = TryGetOrAddValue(node.Right, key, value, out result);
            }
            else
            {
                return result = node;
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
        private RedBlackTreeNode<TKey, TValue> TryGetOrAddValue(RedBlackTreeNode<TKey, TValue> node, TKey key, Func<TValue> value, out RedBlackTreeNode<TKey, TValue> result)
        {
            if (node == null)
            {
                // Insert new node
                Count++;

                return result = new RedBlackTreeNode<TKey, TValue> {Key = key, Value = value()};
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
                node.Left = TryGetOrAddValue(node.Left, key, value, out result);
            }
            else if (0 < comparisonResult)
            {
                node.Right = TryGetOrAddValue(node.Right, key, value, out result);
            }
            else
            {
                return result = node;
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
    }
}