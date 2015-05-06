using System;
using System.Collections.Generic;
using System.Text;

namespace RedBlackForest
{
    partial class RedBlackTree<TValue>
    {
        /// <summary>
        /// Returns true if the specified node is red.
        /// </summary>
        /// <param name="node">Specified node.</param>
        /// <returns>True if specified node is red.</returns>
        private static Boolean IsRed(RedBlackTreeNode<TValue> node)
        {
            if (null == node)
            {
                // "Virtual" leaf nodes are always black
                return false;
            }

            return !node.IsBlack;
        }

        /// <summary>
        /// Flip the colors of the specified node and its direct children.
        /// </summary>
        /// <param name="node">Specified node.</param>
        private static void FlipColor(RedBlackTreeNode<TValue> node)
        {
            node.IsBlack = !node.IsBlack;
            node.Left.IsBlack = !node.Left.IsBlack;
            node.Right.IsBlack = !node.Right.IsBlack;
        }

        /// <summary>
        /// Rotate the specified node "left".
        /// </summary>
        /// <param name="node">Specified node.</param>
        /// <returns>New root node.</returns>
        private static RedBlackTreeNode<TValue> RotateLeft(RedBlackTreeNode<TValue> node)
        {
            RedBlackTreeNode<TValue> x = node.Right;
            node.Right = x.Left;
            x.Left = node;
            x.IsBlack = node.IsBlack;
            node.IsBlack = false;
            return x;
        }

        /// <summary>
        /// Rotate the specified node "right".
        /// </summary>
        /// <param name="node">Specified node.</param>
        /// <returns>New root node.</returns>
        private static RedBlackTreeNode<TValue> RotateRight(RedBlackTreeNode<TValue> node)
        {
            RedBlackTreeNode<TValue> x = node.Left;
            node.Left = x.Right;
            x.Right = node;
            x.IsBlack = node.IsBlack;
            node.IsBlack = false;
            return x;
        }

        /// <summary>
        /// Moves a red node from the right child to the left child.
        /// </summary>
        /// <param name="node">Parent node.</param>
        /// <returns>New root node.</returns>
        private static RedBlackTreeNode<TValue> MoveRedLeft(RedBlackTreeNode<TValue> node)
        {
            FlipColor(node);
            if (IsRed(node.Right.Left))
            {
                node.Right = RotateRight(node.Right);
                node = RotateLeft(node);
                FlipColor(node);

                // * Avoid creating right-leaning nodes
                if (IsRed(node.Right.Right))
                {
                    node.Right = RotateLeft(node.Right);
                }
            }
            return node;
        }

        /// <summary>
        /// Moves a red node from the left child to the right child.
        /// </summary>
        /// <param name="node">Parent node.</param>
        /// <returns>New root node.</returns>
        private static RedBlackTreeNode<TValue> MoveRedRight(RedBlackTreeNode<TValue> node)
        {
            FlipColor(node);
            if (IsRed(node.Left.Left))
            {
                node = RotateRight(node);
                FlipColor(node);
            }
            return node;
        }

        /// <summary>
        /// Deletes the minimum node under the specified node.
        /// </summary>
        /// <param name="node">Specified node.</param>
        /// <returns>New root node.</returns>
        private static RedBlackTreeNode<TValue> DeleteMinimum(RedBlackTreeNode<TValue> node)
        {
            if (null == node.Left)
            {
                // Nothing to do
                return null;
            }

            if (!IsRed(node.Left) && !IsRed(node.Left.Left))
            {
                // Move red node left
                node = MoveRedLeft(node);
            }

            // Recursively delete
            node.Left = DeleteMinimum(node.Left);

            // MaInt32ain invariants
            return FixUp(node);
        }

        /// <summary>
        /// MaInt32ains invariants by adjusting the specified nodes children.
        /// </summary>
        /// <param name="node">Specified node.</param>
        /// <returns>New root node.</returns>
        private static RedBlackTreeNode<TValue> FixUp(RedBlackTreeNode<TValue> node)
        {
            if (IsRed(node.Right))
            {
                // Avoid right-leaning node
                node = RotateLeft(node);
            }

            if (IsRed(node.Left) && IsRed(node.Left.Left))
            {
                // Balance 4-node
                node = RotateRight(node);
            }

            if (IsRed(node.Left) && IsRed(node.Right))
            {
                // Push red up
                FlipColor(node);
            }

            // * Avoid leaving behind right-leaning nodes
            if ((null != node.Left) && IsRed(node.Left.Right) && !IsRed(node.Left.Left))
            {
                node.Left = RotateLeft(node.Left);
                if (IsRed(node.Left))
                {
                    // Balance 4-node
                    node = RotateRight(node);
                }
            }

            return node;
        }

        private static RedBlackTreeNode<TValue> GetMinimumNode(RedBlackTreeNode<TValue> node)
        {
            if (node != null)
            {
                // Initialize
                RedBlackTreeNode<TValue> current = node;

                while (true)
                {
                    if (current.Left == null)
                    {
                        return current;
                    }
                    else
                    {
                        current = current.Left;
                    }
                }
            }
            else
            {
                return null;
            }
        }
        private static RedBlackTreeNode<TValue> GetMaximumNode(RedBlackTreeNode<TValue> node)
        {
            if (node != null)
            {
                // Initialize
                RedBlackTreeNode<TValue> current = node;

                while (true)
                {
                    if (current.Right == null)
                    {
                        return current;
                    }
                    else
                    {
                        current = current.Right;
                    }
                }
            }
            else
            {
                return null;
            }
        }
    }
}
