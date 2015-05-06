using System;
using System.Collections.Generic;

namespace RedBlackForest
{
    public partial class RedBlackTree<TValue> : IEnumerable<RedBlackTreeNode<TValue>>
    {
        /// <summary>
        /// Stores the root node of the tree.
        /// </summary>
        private RedBlackTreeNode<TValue> rootNode;

        /// <summary>
        /// Stores the value comparison function.
        /// </summary>
        public IComparer<TValue> Comparer { get; private set; }

        /// <summary>
        /// Initializes a new instance of the LeftLeaningRedBlackTree class implementing a normal dictionary.
        /// </summary>
        /// <param name="comparer">The value comparison function.</param>
        public RedBlackTree(IComparer<TValue> comparer)
        {
            if (null == comparer)
            {
                throw new ArgumentNullException("comparer");
            }

            this.Comparer = comparer;
        }

        public RedBlackTree()
            : this(Comparer<TValue>.Default)
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
        /// Adds a value to the tree
        /// </summary>
        /// <remarks>This method throws exception in case when element exists in collection</remarks>
        /// <param name="value">Value to add.</param>
        public RedBlackTreeNode<TValue> Add(TValue value)
        {
            RedBlackTreeNode<TValue> result;
            rootNode = Add(rootNode, value, out result);
            rootNode.IsBlack = true;
            return result;
        }

        /// <summary>
        /// Adds a value to the tree
        /// </summary>
        /// <remarks>When adding element that exists in collection, this method not throws </remarks>
        /// <param name="value">Value to add.</param>
        /// <returns></returns>
        public RedBlackTreeNode<TValue> TryAdd(TValue value)
        {
            RedBlackTreeNode<TValue> result;

            rootNode = TryAdd(rootNode, value, out result);
            rootNode.IsBlack = true;

            return result;
        }

        /// <summary>
        /// Removes a value from the tree
        /// </summary>
        /// <param name="value">Value to remove.</param>
        /// <returns>True if value/value present and removed.</returns>
        public Boolean Remove(TValue value)
        {
            Int32 initialCount = Count;

            if (null != rootNode)
            {
                rootNode = Remove(rootNode, value);

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

        public IEnumerable<RedBlackTreeNode<TValue>> Nodes
        {
            get
            {
                return Traverse(rootNode);
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
                foreach (var node in Traverse(rootNode))
                {
                    yield return node.Value;
                }
            }
        }

        /// <summary>
        /// Gets the (first) node corresponding to the specified value.
        /// </summary>
        /// <param name="value">Value to search for.</param>
        /// <returns>Corresponding node or null if none found.</returns>
        public RedBlackTreeNode<TValue> FindNode(TValue value)
        {
            // Initialize
            RedBlackTreeNode<TValue> node = rootNode;

            while (null != node)
            {
                // Compare keys and go left/right
                Int32 comparisonResult = Comparer.Compare(value, node.Value);

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

        public Boolean Contains(TValue value)
        {
            RedBlackTreeNode<TValue> node = FindNode(value);

            return node != null;
        }

        public TValue Next(TValue value)
        {
            var node = NextNode(value);

            return node == null ? default(TValue) : node.Value;
        }
        public RedBlackTreeNode<TValue> NextNode(TValue value)
        {
            return SiblingNodes(value).B;
        }

        public TValue Previous(TValue value)
        {
            var node = PreviousNode(value);

            return node == null ? default(TValue) : node.Value;
        }
        public RedBlackTreeNode<TValue> PreviousNode(TValue value)
        {
            return SiblingNodes(value).A;
        }

        public Pair<TValue> Sibling(TValue value)
        {
            var nodes = SiblingNodes(value);

            return new Pair<TValue>
            (
                nodes.A == null ? default(TValue) : nodes.A.Value,
                nodes.B == null ? default(TValue) : nodes.B.Value
            );
        }
        public Pair<RedBlackTreeNode<TValue>> SiblingNodes(TValue value)
        {
            if (IsEmpty)
                return default(Pair<RedBlackTreeNode<TValue>>);

            var leftmost = true;
            var rightmost = true;

            var nodeA = rootNode;
            var nodeB = rootNode;
            var nodeC = rootNode;

            var comparisonA = Comparer.Compare(value, nodeA.Value);
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
                            return new Pair<RedBlackTreeNode<TValue>>(null, nodeA);
                        }

                        if (rightmost)
                        {
                            return new Pair<RedBlackTreeNode<TValue>>(nodeA, null);
                        }

                        if (Comparer.Compare(nodeA.Value, nodeB.Value) < 0 || (Comparer.Compare(nodeA.Value, nodeC.Value) < 0 && 0 < comparisonA))
                        {
                            return new Pair<RedBlackTreeNode<TValue>>(nodeA, nodeC);
                        }
                        else
                        {
                            return new Pair<RedBlackTreeNode<TValue>>(nodeB, nodeA);
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
                            return new Pair<RedBlackTreeNode<TValue>>(null, nodeA);
                        }

                        if (rightmost)
                        {
                            return new Pair<RedBlackTreeNode<TValue>>(nodeA, null);
                        }

                        if (Comparer.Compare(nodeA.Value, nodeB.Value) < 0 || (Comparer.Compare(nodeA.Value, nodeC.Value) < 0 && 0 < comparisonA))
                        {
                            return new Pair<RedBlackTreeNode<TValue>>(nodeA, nodeC);
                        }
                        else
                        {
                            return new Pair<RedBlackTreeNode<TValue>>(nodeB, nodeA);
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
                            return new Pair<RedBlackTreeNode<TValue>>(GetMaximumNode(nodeA.Left), GetMinimumNode(nodeA.Right));
                        }
                        else
                        {
                            if (Comparer.Compare(nodeA.Value, nodeB.Value) < 0 || Comparer.Compare(nodeA.Value, nodeC.Value) < 0)
                            // A < B
                            {
                                return new Pair<RedBlackTreeNode<TValue>>(GetMaximumNode(nodeA.Left), nodeC);
                            }
                            else
                            // B < A
                            {
                                return new Pair<RedBlackTreeNode<TValue>>(GetMaximumNode(nodeA.Left), null);
                            }
                        }
                    }
                    else
                    {
                        if (leftmost && rightmost)
                        {
                            return new Pair<RedBlackTreeNode<TValue>>(null, null);
                        }
                        else if (leftmost)
                        {
                            return new Pair<RedBlackTreeNode<TValue>>(null, nodeC);
                        }
                        else if (rightmost)
                        {
                            return new Pair<RedBlackTreeNode<TValue>>(nodeB, null);
                        }
                        else
                        {
                            return new Pair<RedBlackTreeNode<TValue>>(nodeB, nodeC);
                        }
                    }
                }

                comparisonA = Comparer.Compare(value, nodeA.Value);
            }
        }

        public Pair<TValue> Nearest(TValue value)
        {
            var nodes = NearestNodes(value);

            return new Pair<TValue>
            (
                nodes.A == null ? default(TValue) : nodes.A.Value,
                nodes.B == null ? default(TValue) : nodes.B.Value
            );
        }
        public Pair<RedBlackTreeNode<TValue>> NearestNodes(TValue value)
        {
            if (IsEmpty)
                return default(Pair<RedBlackTreeNode<TValue>>);

            var leftmost = true;
            var rightmost = true;

            var nodeA = rootNode;
            var nodeB = rootNode;
            var nodeC = rootNode;

            var comparisonA = Comparer.Compare(value, nodeA.Value);
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
                            return new Pair<RedBlackTreeNode<TValue>>(null, nodeA);
                        }

                        if (rightmost)
                        {
                            return new Pair<RedBlackTreeNode<TValue>>(nodeA, null);
                        }

                        if (Comparer.Compare(nodeA.Value, nodeB.Value) < 0 || (Comparer.Compare(nodeA.Value, nodeC.Value) < 0 && 0 < comparisonA))
                        {
                            return new Pair<RedBlackTreeNode<TValue>>(nodeA, nodeC);
                        }
                        else
                        {
                            return new Pair<RedBlackTreeNode<TValue>>(nodeB, nodeA);
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
                            return new Pair<RedBlackTreeNode<TValue>>(null, nodeA);
                        }

                        if (rightmost)
                        {
                            return new Pair<RedBlackTreeNode<TValue>>(nodeA, null);
                        }

                        if (Comparer.Compare(nodeA.Value, nodeB.Value) < 0 || (Comparer.Compare(nodeA.Value, nodeC.Value) < 0 && 0 < comparisonA))
                        {
                            return new Pair<RedBlackTreeNode<TValue>>(nodeA, nodeC);
                        }
                        else
                        {
                            return new Pair<RedBlackTreeNode<TValue>>(nodeB, nodeA);
                        }
                    }
                }
                else
                {
                    return new Pair<RedBlackTreeNode<TValue>>(nodeA, nodeA);
                }

                comparisonA = Comparer.Compare(value, nodeA.Value);
            }
        }

        /// <summary>
        /// Gets the count of values in the tree.
        /// </summary>
        public Int32 Count { get; private set; }

        public TValue GetMinimum()
        {
            var node = GetMinimumNode(rootNode);

            return node == null ? default(TValue) : node.Value;
        }
        public RedBlackTreeNode<TValue> GetMinimumNode()
        {
            return GetMinimumNode(rootNode);
        }

        public TValue GetMaximum()
        {
            var node = GetMaximumNode(rootNode);

            return node == null ? default(TValue) : node.Value;
        }
        public RedBlackTreeNode<TValue> GetMaximumNode()
        {
            return GetMaximumNode(rootNode);
        }

        public TValue RemoveMinimum()
        {
            var node = RemoveMinimumNode();

            return node == null ? default(TValue) : node.Value;
        }
        public RedBlackTreeNode<TValue> RemoveMinimumNode()
        {
            var node = GetMinimumNode(rootNode);

            if (node != null) Remove(node.Value);

            return node;
        }

        public TValue RemoveMaximum()
        {
            var node = RemoveMaximumNode();

            return node == null ? default(TValue) : node.Value;
        }
        public RedBlackTreeNode<TValue> RemoveMaximumNode()
        {
            var node = GetMaximumNode(rootNode);

            if (node != null) Remove(node.Value);

            return node;
        }

        public IEnumerable<RedBlackTreeNode<TValue>> EnumerateNodesDescendingFrom(TValue value)
        {
            var nearest = NearestNodes(value);

            var prev = nearest.A;
            var next = nearest.B;

            if (prev != null)
            {
                if (prev == next)
                {
                    yield return prev;

                    next = NextNode(next.Value);
                    prev = PreviousNode(prev.Value);
                }
            }

            var usePrev = false;

            while (prev != null && next != null)
            {
                if (usePrev)
                {
                    yield return prev;

                    prev = PreviousNode(prev.Value);
                }
                else
                {
                    yield return next;

                    next = NextNode(next.Value);
                }
            }

            while (prev != null)
            {
                yield return prev;

                prev = PreviousNode(prev.Value);
            }

            while (next != null)
            {
                yield return next;

                next = NextNode(next.Value);
            }

        }

        /// <summary>
        /// Adds the specified value below the specified root node.
        /// </summary>
        /// <param name="node">Specified node.</param>
        /// <param name="value">Value to add.</param>
        /// <param name="value">Value to add.</param>
        /// <returns>New root node.</returns>
        public IEnumerable<RedBlackTreeNode<TValue>> EnumerateNodesInRange(TValue minimum, TValue maximum)
        {
            foreach (var node in TraverseRange(rootNode, minimum, maximum))
            {
                yield return node;
            }
        }

        public IEnumerator<RedBlackTreeNode<TValue>> GetEnumerator()
        {
            foreach (var node in Traverse(rootNode))
            {
                yield return node;
            }
        }

        private RedBlackTreeNode<TValue> Add(RedBlackTreeNode<TValue> node, TValue value, out RedBlackTreeNode<TValue> result)
        {
            if (node == null)
            {
                // Insert new node
                Count++;
                return result = new RedBlackTreeNode<TValue> { Value = value };
            }

            if (IsRed(node.Left) && IsRed(node.Right))
            {
                // Split node with two red children
                FlipColor(node);
            }

            // Find right place for new node
            Int32 comparisonResult = Comparer.Compare(value, node.Value);

            if (comparisonResult < 0)
            {
                node.Left = Add(node.Left, value, out result);
            }
            else if (0 < comparisonResult)
            {
                node.Right = Add(node.Right, value, out result);
            }
            else
            {
                throw new ArgumentException("Set already contains argument value");
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

        private RedBlackTreeNode<TValue> TryAdd(RedBlackTreeNode<TValue> node, TValue value, out RedBlackTreeNode<TValue> result)
        {
            if (node == null)
            {
                // Insert new node
                Count++;
                return result = new RedBlackTreeNode<TValue> { Value = value };
            }

            if (IsRed(node.Left) && IsRed(node.Right))
            {
                // Split node with two red children
                FlipColor(node);
            }

            // Find right place for new node
            Int32 comparisonResult = Comparer.Compare(value, node.Value);

            if (comparisonResult < 0)
            {
                node.Left = TryAdd(node.Left, value, out result);
            }
            else if (0 < comparisonResult)
            {
                node.Right = TryAdd(node.Right, value, out result);
            }
            else
            {
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
        /// Removes the specified value from below the specified node.
        /// </summary>
        /// <param name="node">Specified node.</param>
        /// <param name="value">Value to remove.</param>
        /// <param name="value">Value to remove.</param>
        /// <returns>True if value/value present and removed.</returns>
        private RedBlackTreeNode<TValue> Remove(RedBlackTreeNode<TValue> node, TValue value)
        {
            Int32 comparisonResult = Comparer.Compare(value, node.Value);

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
                    node.Left = Remove(node.Left, value);
                }
            }
            else
            {
                if (IsRed(node.Left))
                {
                    // Flip a 3 node or unbalance a 4 node
                    node = RotateRight(node);
                }
                if ((0 == Comparer.Compare(value, node.Value)) && (null == node.Right))
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
                    if (0 == Comparer.Compare(value, node.Value))
                    {
                        // Remove leaf node
                        Count--;

                        // Find the smallest node on the right, swap, and remove it
                        RedBlackTreeNode<TValue> m = GetMinimumNode(node.Right);
                        node.Value = m.Value;
                        node.Value = m.Value;
                        node.Right = DeleteMinimum(node.Right);
                    }
                    else
                    {
                        // Remove from right
                        node.Right = Remove(node.Right, value);
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
        private IEnumerable<RedBlackTreeNode<TValue>> Traverse(RedBlackTreeNode<TValue> node)
        {
            // Create a stack to avoid recursion
            Stack<RedBlackTreeNode<TValue>> stack = new Stack<RedBlackTreeNode<TValue>>();
            RedBlackTreeNode<TValue> current = node;
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

        private IEnumerable<RedBlackTreeNode<TValue>> TraverseRange(RedBlackTreeNode<TValue> node, TValue minimum, TValue maximum)
        {
            Stack<RedBlackTreeNode<TValue>> stack = new Stack<RedBlackTreeNode<TValue>>();

            while (node != null)
            {
                Int32 comparison = Comparer.Compare(minimum, node.Value);

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
                            comparison = Comparer.Compare(node.Value, maximum);

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
                            comparison = Comparer.Compare(node.Value, maximum);

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
    }
}
