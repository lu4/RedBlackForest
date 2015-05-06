using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace RedBlackForest
{
    public partial class RedBlackTree<Key, Value> : IEnumerable<RedBlackTreeNode<Key, Value>>
    {
        /// <summary>
        /// Stores the root node of the tree.
        /// </summary>
        private RedBlackTreeNode<Key, Value> rootNode;

        /// <summary>
        /// Stores the key comparison function.
        /// </summary>
        public IComparer<Key> Comparer { get; private set; }

        /// <summary>
        /// Initializes a new instance of the LeftLeaningRedBlackTree class implementing a normal dictionary.
        /// </summary>
        /// <param name="comparer">The key comparison function.</param>
        public RedBlackTree(IComparer<Key> comparer)
        {
            if (null == comparer)
            {
                throw new ArgumentNullException("keyComparison");
            }

            this.Comparer = comparer;
        }

        public RedBlackTree()
            : this(Comparer<Key>.Default)
        {
        }

        public Boolean IsEmpty
        {
            get
            {
                return rootNode == null;
            }
        }

        /// <summary>
        /// Adds a key/value pair to the tree.
        /// </summary>
        /// <param name="key">Key to add.</param>
        /// <param name="value">Value to add.</param>
        public RedBlackTreeNode<Key, Value> Add(Key key, Value value)
        {
            RedBlackTreeNode<Key, Value> result;
            rootNode = Add(rootNode, key, value, out result);
            rootNode.IsBlack = true;
            return result;
        }

        /// <summary>
        /// Adds a value to the tree
        /// </summary>
        /// <remarks>When adding element that exists in collection, this method not throws </remarks>
        /// <param name="key">Key to add.</param>
        /// <param name="value">Value to add.</param>
        /// <returns></returns>
        public RedBlackTreeNode<Key, Value> TryAdd(Key key, Value value)
        {
            RedBlackTreeNode<Key, Value> result;

            rootNode = TryAdd(rootNode, key, value, out result);
            rootNode.IsBlack = true;

            return result;
        }

        /// <summary>
        /// Removes a key/value pair from the tree.
        /// </summary>
        /// <param name="key">Key to remove.</param>
        /// <param name="value">Value to remove.</param>
        /// <returns>True if key/value present and removed.</returns>
        public Boolean Remove(Key key)
        {
            Int32 initialCount = Count;

            if (null != rootNode)
            {
                rootNode = Remove(rootNode, key);

                if (null != rootNode)
                {
                    rootNode.IsBlack = true;
                }
            }

            return initialCount != Count;
        }

        /// <summary>
        /// Removes all nodes in the tree.
        /// </summary>
        public void Clear()
        {
            rootNode = null;
            Count = 0;
        }

        public IEnumerable<RedBlackTreeNode<Key, Value>> Nodes
        {
            get
            {
                return Traverse(rootNode);
            }
        }

        /// <summary>
        /// Gets a sorted list of keys in the tree.
        /// </summary>
        /// <returns>Sorted list of keys.</returns>
        public IEnumerable<Key> Keys
        {
            get
            {
                foreach (var node in Traverse(rootNode))
                {
                    yield return node.Key;
                }
            }
        }


        /// <summary>
        /// Gets a sequence of all the values in the tree.
        /// </summary>
        /// <returns>Sequence of all values.</returns>
        public IEnumerable<Value> Values
        {
            get
            {
                foreach (var node in Traverse(rootNode))
                {
                    yield return node.Value;
                }
            }
        }

        public IEnumerable<KeyValuePair<Key, Value>> Pairs
        {
            get
            {
                foreach (var node in Traverse(rootNode))
                {
                    yield return node.Pair;
                }
            }
        }

        /// <summary>
        /// Gets the (first) node corresponding to the specified key.
        /// </summary>
        /// <param name="key">Key to search for.</param>
        /// <returns>Corresponding node or null if none found.</returns>
        public RedBlackTreeNode<Key, Value> FindNode(Key key)
        {
            // Initialize
            RedBlackTreeNode<Key, Value> node = rootNode;

            while (null != node)
            {
                // Compare keys and go left/right
                Int32 comparisonResult = Comparer.Compare(key, node.Key);

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

        public Key FindKey(Key key)
        {
            var node = FindNode(key);

            return node == null ? default(Key) : node.Key;
        }
        public Value FindValue(Key key)
        {
            var node = FindNode(key);

            return node == null ? default(Value) : node.Value;
        }
        public KeyValuePair<Key, Value> FindPair(Key key)
        {
            var node = FindNode(key);

            return node == null ? default(KeyValuePair<Key, Value>) : node.Pair;
        }

        public Boolean ContainsKey(Key key)
        {
            RedBlackTreeNode<Key, Value> node = FindNode(key);

            return node != null;
        }

        /// <summary>
        /// Gets the value associated with the specified key in a tree.
        /// </summary>
        /// <param name="key">Specified key.</param>
        /// <returns>Value associated with the specified key.</returns>
        public Value this[Key key]
        {
            get
            {
                RedBlackTreeNode<Key, Value> node = FindNode(key);

                if (null != node)
                {
                    return node.Value;
                }
                else
                {
                    throw new KeyNotFoundException();
                }
            }
            set
            {
                rootNode = Set(rootNode, key, value);
                rootNode.IsBlack = true;
            }
        }

        public Key NextKey(Key key)
        {
            var node = NextNode(key);

            return node == null ? default(Key) : node.Key;
        }
        public Value NextValue(Key key)
        {
            var node = NextNode(key);

            return node == null ? default(Value) : node.Value;
        }
        public KeyValuePair<Key, Value> NextPair(Key key)
        {
            var node = NextNode(key);

            return node == null ? default(KeyValuePair<Key, Value>) : node.Pair;
        }
        public RedBlackTreeNode<Key, Value> NextNode(Key key)
        {
            return SiblingNodes(key).Value2;
        }

        public Key PreviousKey(Key key)
        {
            var node = PreviousNode(key);

            return node == null ? default(Key) : node.Key;
        }
        public Value PreviousValue(Key key)
        {
            var node = PreviousNode(key);

            return node == null ? default(Value) : node.Value;
        }
        public KeyValuePair<Key, Value> PreviousPair(Key key)
        {
            var node = PreviousNode(key);

            return node == null ? default(KeyValuePair<Key, Value>) : node.Pair;
        }
        public RedBlackTreeNode<Key, Value> PreviousNode(Key key)
        {
            return SiblingNodes(key).Value1;
        }

        public Tuple<Key, Key> SiblingKeys(Key key)
        {
            var nodes = SiblingNodes(key);

            return new Tuple<Key, Key>
            (
                nodes.Value1 == null ? default(Key) : nodes.Value1.Key,
                nodes.Value2 == null ? default(Key) : nodes.Value2.Key
            );
        }
        public Tuple<Value, Value> SiblingValues(Key key)
        {
            var nodes = SiblingNodes(key);

            return new Tuple<Value, Value>
            (
                nodes.Value1 == null ? default(Value) : nodes.Value1.Value,
                nodes.Value2 == null ? default(Value) : nodes.Value2.Value
            );
        }
        public Tuple<KeyValuePair<Key, Value>, KeyValuePair<Key, Value>> SiblingPairs(Key key)
        {
            var nodes = SiblingNodes(key);

            return new Tuple<KeyValuePair<Key, Value>, KeyValuePair<Key, Value>>
            (
                nodes.Value1 == null ? default(KeyValuePair<Key, Value>) : nodes.Value1.Pair,
                nodes.Value2 == null ? default(KeyValuePair<Key, Value>) : nodes.Value2.Pair
            );
        }
        public Tuple<RedBlackTreeNode<Key, Value>, RedBlackTreeNode<Key, Value>> SiblingNodes(Key key)
        {
            if (IsEmpty)
                return default(Tuple<RedBlackTreeNode<Key, Value>, RedBlackTreeNode<Key, Value>>);

            var leftmost = true;
            var rightmost = true;

            var nodeA = rootNode;
            var nodeB = rootNode;
            var nodeC = rootNode;

            var comparisonA = Comparer.Compare(key, nodeA.Key);
            // var comparisonB = comparisonA; // MonoDevelop says that this line is not used...
			// var comparisonC = comparisonB; // MonoDevelop says that this line is not used...

            while (true)
            {
                if (comparisonA < 0)
                {
                    rightmost = false;

                    if (nodeA.Left != null)
                    {
						// comparisonC = comparisonA; // MonoDevelop says that this line is not used...
                        nodeC = nodeA;
                        nodeA = nodeA.Left;
                    }
                    else
                    {
                        if (leftmost)
                        {
                            return new Tuple<RedBlackTreeNode<Key, Value>, RedBlackTreeNode<Key, Value>>(null, nodeA);
                        }

                        if (rightmost)
                        {
                            return new Tuple<RedBlackTreeNode<Key, Value>, RedBlackTreeNode<Key, Value>>(nodeA, null);
                        }

                        if (Comparer.Compare(nodeA.Key, nodeB.Key) < 0 || (Comparer.Compare(nodeA.Key, nodeC.Key) < 0 && 0 < comparisonA))
                        {
                            return new Tuple<RedBlackTreeNode<Key, Value>, RedBlackTreeNode<Key, Value>>(nodeA, nodeC);
                        }
                        else
                        {
                            return new Tuple<RedBlackTreeNode<Key, Value>, RedBlackTreeNode<Key, Value>>(nodeB, nodeA);
                        }
                    }
                }
                else if (comparisonA > 0)
                {
                    leftmost = false;

                    if (nodeA.Right != null)
                    {
                        // comparisonB = comparisonA; // MonoDevelop says that this line is not used...
                        nodeB = nodeA;
                        nodeA = nodeA.Right;
                    }
                    else
                    {
                        if (leftmost)
                        {
                            return new Tuple<RedBlackTreeNode<Key, Value>, RedBlackTreeNode<Key, Value>>(null, nodeA);
                        }

                        if (rightmost)
                        {
                            return new Tuple<RedBlackTreeNode<Key, Value>, RedBlackTreeNode<Key, Value>>(nodeA, null);
                        }

                        if (Comparer.Compare(nodeA.Key, nodeB.Key) < 0 || (Comparer.Compare(nodeA.Key, nodeC.Key) < 0 && 0 < comparisonA))
                        {
                            return new Tuple<RedBlackTreeNode<Key, Value>, RedBlackTreeNode<Key, Value>>(nodeA, nodeC);
                        }
                        else
                        {
                            return new Tuple<RedBlackTreeNode<Key, Value>, RedBlackTreeNode<Key, Value>>(nodeB, nodeA);
                        }
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
                            return new Tuple<RedBlackTreeNode<Key, Value>, RedBlackTreeNode<Key, Value>>(GetMaximumNode(nodeA.Left), GetMinimumNode(nodeA.Right));
                        }
                        else
                        {
                            if (Comparer.Compare(nodeA.Key, nodeB.Key) < 0 || Comparer.Compare(nodeA.Key, nodeC.Key) < 0)
                            // A < B
                            {
                                return new Tuple<RedBlackTreeNode<Key, Value>, RedBlackTreeNode<Key, Value>>(GetMaximumNode(nodeA.Left), nodeC);
                            }
                            else
                            // B < A
                            {
                                return new Tuple<RedBlackTreeNode<Key, Value>, RedBlackTreeNode<Key, Value>>(GetMaximumNode(nodeA.Left), null);
                            }
                        }
                    }
                    else
                    {
                        if (leftmost && rightmost)
                        {
                            return new Tuple<RedBlackTreeNode<Key, Value>, RedBlackTreeNode<Key, Value>>(null, null);
                        }
                        else if (leftmost)
                        {
                            return new Tuple<RedBlackTreeNode<Key, Value>, RedBlackTreeNode<Key, Value>>(null, nodeC);
                        }
                        else if (rightmost)
                        {
                            return new Tuple<RedBlackTreeNode<Key, Value>, RedBlackTreeNode<Key, Value>>(nodeB, null);
                        }
                        else
                        {
                            return new Tuple<RedBlackTreeNode<Key, Value>, RedBlackTreeNode<Key, Value>>(nodeB, nodeC);
                        }
                    }
                }

                comparisonA = Comparer.Compare(key, nodeA.Key);
            }
        }

        public Tuple<Key, Key> NearestKeys(Key key)
        {
            var nodes = NearestNodes(key);

            return new Tuple<Key, Key>
            (
                nodes.Value1 == null ? default(Key) : nodes.Value1.Key,
                nodes.Value2 == null ? default(Key) : nodes.Value2.Key
            );
        }
        public Tuple<Value, Value> NearestValues(Key key)
        {
            var nodes = NearestNodes(key);

            return new Tuple<Value, Value>
            (
                nodes.Value1 == null ? default(Value) : nodes.Value1.Value,
                nodes.Value2 == null ? default(Value) : nodes.Value2.Value
            );
        }
        public Tuple<KeyValuePair<Key, Value>, KeyValuePair<Key, Value>> NearestPairs(Key key)
        {
            var nodes = NearestNodes(key);

            return new Tuple<KeyValuePair<Key, Value>, KeyValuePair<Key, Value>>
            (
                nodes.Value1 == null ? default(KeyValuePair<Key, Value>) : nodes.Value1.Pair,
                nodes.Value2 == null ? default(KeyValuePair<Key, Value>) : nodes.Value2.Pair
            );
        }
        public Tuple<RedBlackTreeNode<Key, Value>, RedBlackTreeNode<Key, Value>> NearestNodes(Key key)
        {
            if (IsEmpty)
                return default(Tuple<RedBlackTreeNode<Key, Value>, RedBlackTreeNode<Key, Value>>);

            var leftmost = true;
            var rightmost = true;

            var nodeA = rootNode;
            var nodeB = rootNode;
            var nodeC = rootNode;

            var comparisonA = Comparer.Compare(key, nodeA.Key);
            // var comparisonB = comparisonA; // MonoDevelop says that this line is not used...
            // var comparisonC = comparisonB; // MonoDevelop says that this line is not used...

            while (true)
            {
                if (comparisonA < 0)
                {
                    rightmost = false;

                    if (nodeA.Left != null)
                    {
                        // comparisonC = comparisonA;  // MonoDevelop says that this line is not used...
                        nodeC = nodeA;
                        nodeA = nodeA.Left;
                    }
                    else
                    {
                        if (leftmost)
                        {
                            return new Tuple<RedBlackTreeNode<Key, Value>, RedBlackTreeNode<Key, Value>>(null, nodeA);
                        }

                        if (rightmost)
                        {
                            return new Tuple<RedBlackTreeNode<Key, Value>, RedBlackTreeNode<Key, Value>>(nodeA, null);
                        }

                        if (Comparer.Compare(nodeA.Key, nodeB.Key) < 0 || (Comparer.Compare(nodeA.Key, nodeC.Key) < 0 && 0 < comparisonA))
                        {
                            return new Tuple<RedBlackTreeNode<Key, Value>, RedBlackTreeNode<Key, Value>>(nodeA, nodeC);
                        }
                        else
                        {
                            return new Tuple<RedBlackTreeNode<Key, Value>, RedBlackTreeNode<Key, Value>>(nodeB, nodeA);
                        }
                    }
                }
                else if (comparisonA > 0)
                {
                    leftmost = false;

                    if (nodeA.Right != null)
                    {
                        // comparisonB = comparisonA; // MonoDevelop says that this line is not used...
                        nodeB = nodeA;
                        nodeA = nodeA.Right;
                    }
                    else
                    {
                        if (leftmost)
                        {
                            return new Tuple<RedBlackTreeNode<Key, Value>, RedBlackTreeNode<Key, Value>>(null, nodeA);
                        }

                        if (rightmost)
                        {
                            return new Tuple<RedBlackTreeNode<Key, Value>, RedBlackTreeNode<Key, Value>>(nodeA, null);
                        }

                        if (Comparer.Compare(nodeA.Key, nodeB.Key) < 0 || (Comparer.Compare(nodeA.Key, nodeC.Key) < 0 && 0 < comparisonA))
                        {
                            return new Tuple<RedBlackTreeNode<Key, Value>, RedBlackTreeNode<Key, Value>>(nodeA, nodeC);
                        }
                        else
                        {
                            return new Tuple<RedBlackTreeNode<Key, Value>, RedBlackTreeNode<Key, Value>>(nodeB, nodeA);
                        }
                    }
                }
                else
                {
                    return new Tuple<RedBlackTreeNode<Key, Value>, RedBlackTreeNode<Key, Value>>(nodeA, nodeA);
                }

                comparisonA = Comparer.Compare(key, nodeA.Key);
            }
        }

        /// <summary>
        /// Gets the count of key/value pairs in the tree.
        /// </summary>
        public Int32 Count { get; private set; }

        public Key GetMinimumKey()
        {
            var node = GetMinimumNode(rootNode);

            return node == null ? default(Key) : node.Key;
        }
        public Value GetMinimumValue()
        {
            var node = GetMinimumNode(rootNode);

            return node == null ? default(Value) : node.Value;
        }
        public KeyValuePair<Key, Value> GetMinimumPair()
        {
            var node = GetMinimumNode(rootNode);

            return node == null ? default(KeyValuePair<Key, Value>) : node.Pair;
        }
        public RedBlackTreeNode<Key, Value> GetMinimumNode()
        {
            return GetMinimumNode(rootNode);
        }

        public Key GetMaximumKey()
        {
            var node = GetMaximumNode(rootNode);

            return node == null ? default(Key) : node.Key;
        }
        public Value GetMaximumValue()
        {
            var node = GetMaximumNode(rootNode);

            return node == null ? default(Value) : node.Value;
        }
        public KeyValuePair<Key, Value> GetMaximumPair()
        {
            var node = GetMaximumNode(rootNode);

            return node == null ? default(KeyValuePair<Key, Value>) : node.Pair;
        }
        public RedBlackTreeNode<Key, Value> GetMaximumNode()
        {
            return GetMaximumNode(rootNode);
        }

        public void RemoveMinimum()
        {
            var node = GetMaximumNode(rootNode);

            if (node != null) Remove(node.Key);
        }
        public Key RemoveMinimumKey()
        {
            var node = RemoveMinimumNode();

            return node == null ? default(Key) : node.Key;
        }
        public Value RemoveMinimumValue()
        {
            var node = RemoveMinimumNode();

            return node == null ? default(Value) : node.Value;
        }
        public KeyValuePair<Key, Value> RemoveMinimumPair()
        {
            var node = RemoveMinimumNode();

            return node == null ? default(KeyValuePair<Key, Value>) : node.Pair;
        }
        public RedBlackTreeNode<Key, Value> RemoveMinimumNode()
        {
            var node = GetMinimumNode(rootNode);

            if (node != null) Remove(node.Key);

            return node;
        }

        public void RemoveMaximum()
        {
            var node = GetMaximumNode(rootNode);

            if (node != null) Remove(node.Key);
        }
        public Key RemoveMaximumKey()
        {
            var node = RemoveMaximumNode();

            return node == null ? default(Key) : node.Key;
        }
        public Value RemoveMaximumValue()
        {
            var node = RemoveMaximumNode();

            return node == null ? default(Value) : node.Value;
        }
        public KeyValuePair<Key, Value> RemoveMaximumPair()
        {
            var node = RemoveMaximumNode();

            return node == null ? default(KeyValuePair<Key, Value>) : node.Pair;
        }
        public RedBlackTreeNode<Key, Value> RemoveMaximumNode()
        {
            var node = GetMaximumNode(rootNode);

            if (node != null) Remove(node.Key);

            return node;
        }

        public IEnumerable<RedBlackTreeNode<Key, Value>> EnumerateRangeNodes(Key minimum, Key maximum)
        {
            foreach (var node in TraverseRange(rootNode, minimum, maximum))
            {
                yield return node;
            }
        }
        public IEnumerable<RedBlackTreeNode<Key, Value>> EnumerateDescendingNodes(Key key)
        {
            var nearest = NearestNodes(key);

            var prev = nearest.Value1;
            var next = nearest.Value2;

            if (prev != null)
            {
                if (prev == next)
                {
                    yield return prev;

                    next = NextNode(next.Key);
                    prev = PreviousNode(prev.Key);
                }
            }

            var usePrev = false;

            while (prev != null && next != null)
            {
                if (usePrev)
                {
                    yield return prev;

                    prev = PreviousNode(prev.Key);
                }
                else
                {
                    yield return next;

                    next = NextNode(next.Key);
                }
            }

            while (prev != null)
            {
                yield return prev;

                prev = PreviousNode(prev.Key);
            }

            while (next != null)
            {
                yield return next;

                next = NextNode(next.Key);
            }

        }

        public IEnumerator<RedBlackTreeNode<Key, Value>> GetEnumerator()
        {
            foreach (var node in Traverse(rootNode))
            {
                yield return node;
            }
        }

        private RedBlackTreeNode<Key, Value> Add(RedBlackTreeNode<Key, Value> node, Key key, Value value, out RedBlackTreeNode<Key, Value> result)
        {
            if (node == null)
            {
                // Insert new node
                Count++;
                return result = new RedBlackTreeNode<Key, Value> { Key = key, Value = value };
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

        private RedBlackTreeNode<Key, Value> TryAdd(RedBlackTreeNode<Key, Value> node, Key key, Value value, out RedBlackTreeNode<Key, Value> result)
        {
            if (node == null)
            {
                // Insert new node
                Count++;
                return result = new RedBlackTreeNode<Key, Value> { Key = key, Value = value };
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
                node.Left = TryAdd(node.Left, key, value, out result);
            }
            else if (0 < comparisonResult)
            {
                node.Right = TryAdd(node.Right, key, value, out result);
            }
            else
            {
                // Replace the value of the existing node
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
        private RedBlackTreeNode<Key, Value> Set(RedBlackTreeNode<Key, Value> node, Key key, Value value)
        {
            if (null == node)
            {
                // Insert new node
                Count++;
                return new RedBlackTreeNode<Key, Value> { Key = key, Value = value };
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
        /// <param name="value">Value to remove.</param>
        /// <returns>True if key/value present and removed.</returns>
        private RedBlackTreeNode<Key, Value> Remove(RedBlackTreeNode<Key, Value> node, Key key)
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
                        RedBlackTreeNode<Key, Value> m = GetMinimumNode(node.Right);
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
        /// <typeparam name="T">Type of elements.</typeparam>
        /// <param name="node">Starting node.</param>
        /// <param name="condition">Condition method.</param>
        /// <param name="selector">Selector method.</param>
        /// <returns>Sequence of selected nodes.</returns>
        private IEnumerable<RedBlackTreeNode<Key, Value>> Traverse(RedBlackTreeNode<Key, Value> node)
        {
            // Create a stack to avoid recursion
            Stack<RedBlackTreeNode<Key, Value>> stack = new Stack<RedBlackTreeNode<Key, Value>>();
            RedBlackTreeNode<Key, Value> current = node;
            while (current != null)
            {
                if (current.Left != null)
                {
                    // Save current state and go left
                    stack.Push(current);
                    current = current.Left;
                }
                else
                {
                    do
                    {
                        yield return current;
                        // Go right - or up if nothing to the right
                        current = current.Right;
                    }
                    while ((current == null) && (stack.Count > 0) && ((current = stack.Pop()) != null));
                }
            }
        }

        private IEnumerable<RedBlackTreeNode<Key, Value>> TraverseRange(RedBlackTreeNode<Key, Value> node, Key minimum, Key maximum)
        {
            Stack<RedBlackTreeNode<Key, Value>> stack = new Stack<RedBlackTreeNode<Key, Value>>();

            while (node != null)
            {
                Int32 comparison = Comparer.Compare(minimum, node.Key);

                if (comparison <= 0)
                {
                    if (node.Left != null)
                    {
                        stack.Push(node);
                        node = node.Left;
                    }
                    else
                    {
                        Boolean @break = false;

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
                else
                {
                    node = node.Right;

                    if (node == null && stack.Count > 0)
                    {
                        node = stack.Pop();

                        Boolean @break = false;

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

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            foreach (var node in Traverse(rootNode))
            {
                yield return node;
            }
        }

        public RedBlackTreeNode<Key, Value> TryAddInverse(Key key, Value value)
        {
            RedBlackTreeNode<Key, Value> result;

            rootNode = TryAddInverse(rootNode, key, value, out result);
            rootNode.IsBlack = true;

            return result;
        }
        private RedBlackTreeNode<Key, Value> TryAddInverse(RedBlackTreeNode<Key, Value> node, Key key, Value value, out RedBlackTreeNode<Key, Value> result)
        {
            if (node == null)
            {
                // Insert new node
                Count++;

                result = null;

                return new RedBlackTreeNode<Key, Value> {Key = key, Value = value};
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
                node.Left = TryAddInverse(node.Left, key, value, out result);
            }
            else if (0 < comparisonResult)
            {
                node.Right = TryAddInverse(node.Right, key, value, out result);
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
