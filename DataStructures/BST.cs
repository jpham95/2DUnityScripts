#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;

namespace DataStructures
{
    public class BSTNode<T> where T : IComparable<T>
    {
        // Properties
        public T Value { get; set; }
        public BSTNode<T>? Left { get; set; }
        public BSTNode<T>? Right { get; set; }

        // Constructor
        public BSTNode(T value)
        {
            Value = value;
            Left = null;
            Right = null;
        }

        public override string ToString()
        {
            return $"Node containing: {Value}";
        }
    }
    
    public class BinarySearchTree<T> : IEnumerable<T> where T : IComparable<T>
    {
        // Root node
        private BSTNode<T>? root;
        // Constructor
        public BinarySearchTree()
        {
            root = null;
        }
        // Properties
        public bool IsEmpty
        {
            get { return root == null; }
        }

        public int Count
        {
            get { return GetCount(root); }
        }
        // Count helper method
        private int GetCount(BSTNode<T>? node)
        {
            if (node == null)
            {
                return 0;
            }
            else
            {
                return 1 + GetCount(node.Left) + GetCount(node.Right);
            }
        }
        // Contains() method
        public bool Contains(T value)
        {
            return containsHelper(root, value);
        }
        // Contains() helper method
        private bool containsHelper(BSTNode<T>? node, T value)
        {
            if (node == null)
            {
                return false;
            }
            else if (node.Value.CompareTo(value) == 0)
            {
                return true;
            }
            else if (node.Value.CompareTo(value) > 0)
            {
                return containsHelper(node.Left, value);
            }
            else // node.Value.CompareTo(value) < 0
            {
                return containsHelper(node.Right, value);
            }
        }
        // Insert() method
        public void Insert(T value)
        {
            root = insertHelper(root, value);
        }
        // Insert() helper method
        private BSTNode<T> insertHelper(BSTNode<T>? node, T value)
        {
            if (node == null)
            {
                node = new BSTNode<T>(value);
            }
            
            else if (node.Value.CompareTo(value) > 0)
            {
                node.Left = insertHelper(node.Left, value);
            }
            else if (node.Value.CompareTo(value) < 0)
            {
                node.Right = insertHelper(node.Right, value);
            }
            else // node.Value.CompareTo(value) == 0
            {
                throw new Exception("Value already exists in tree");
            }
            return node;
        }

        // Remove method
        public void Remove(T value)
        {
            root = removeHelper(root, value);
        }
        // Remove helper method
        private BSTNode<T>? removeHelper(BSTNode<T>? node, T value)
        {
            if (node == null)
            {
                throw new Exception("Value does not exist in tree");
            }
            else if (node.Value.CompareTo(value) > 0)
            {
                node.Left = removeHelper(node.Left, value);
            }
            else if (node.Value.CompareTo(value) < 0)
            {
                node.Right = removeHelper(node.Right, value);
            }
            else // node
            {
                if (node.Left == null && node.Right == null)
                {
                    node = null;
                }
                else if (node.Left == null)
                {
                    node = node.Right;
                }
                else if (node.Right == null)
                {
                    node = node.Left;
                }
                else
                {
                    BSTNode<T> minNode = FindMin(node.Right);
                    node.Value = minNode.Value;
                    node.Right = removeHelper(node.Right, minNode.Value);
                }
            }
            return node;
        }
        // findMin method returns min node
        private BSTNode<T> FindMin(BSTNode<T> node)
        {
            if (node.Left == null)
            {
                return node;
            }
            else
            {
                return FindMin(node.Left);
            }
        }
        // preorder traversal
        public void Preorder(Action<T> action)
        {
            preorderHelper(root, action);
        }
        private void preorderHelper(BSTNode<T>? node, Action<T> action)
        {
            if (node != null)
            {
                action(node.Value);
                preorderHelper(node.Left, action);
                preorderHelper(node.Right, action);
            }
        }
        // postorder traversal
        public void Postorder(Action<T> action)
        {
            postorderHelper(root, action);
        }
        private void postorderHelper(BSTNode<T>? node, Action<T> action)
        {
            if (node != null)
            {
                postorderHelper(node.Left, action);
                postorderHelper(node.Right, action);
                action(node.Value);
            }
        }
        // inorder traversal
        public void Inorder(Action<T> action)
        {
            inorderHelper(root, action);
        }
        private void inorderHelper(BSTNode<T>? node, Action<T> action)
        {
            if (node != null)
            {
                inorderHelper(node.Left, action);
                action(node.Value);
                inorderHelper(node.Right, action);
            }
        }

        // Iterator
        public IEnumerator<T> GetEnumerator()
        {
            return InOrderTraversal(root).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private IEnumerable<T> InOrderTraversal(BSTNode<T>? node)
        {
            if (node != null)
            {
                foreach (var value in InOrderTraversal(node.Left))
                {
                    yield return value;
                }
                yield return node.Value;
                
                foreach (var value in InOrderTraversal(node.Right))
                {
                    yield return value;
                }
            }
        }
    }
}