using System;
using System.Collections.Generic;
using IndoorMapping.Tools.Trees.LdKdTree.Nodes;

namespace IndoorMapping.Tools.Trees.LdKdTree
{
	/// <summary>
	/// Tree enumeration method delegate.
	/// </summary>
	/// <typeparam name="TNode">The class type for the nodes of the tree.</typeparam>
	/// <param name="tree">The binary tree to be traversed.</param>
	/// <returns>An enumerator traversing the tree.</returns>
	public delegate IEnumerator<TNode> BinaryTraversalMethod<TNode>(BinaryTree<TNode> tree) where TNode : BinaryNode<TNode>;

	[Serializable]
	public class BinaryTree<TNode> : IEnumerable<TNode> where TNode : BinaryNode<TNode>
	{
		/// <summary>
		/// Gets the root node of this tree.
		/// </summary>
		public TNode Root { get; set; }

		public virtual IEnumerator<TNode> GetEnumerator()
		{
			if (Root == null)
			{
				yield break;
			}

			var stack = new Stack<TNode>(new[] { Root });

			while (stack.Count != 0)
			{
				TNode current = stack.Pop();

				yield return current;

				if (current.Left != null)
				{
					stack.Push(current.Left);
				}

				if (current.Right != null)
				{
					stack.Push(current.Right);
				}
			}
		}

		/// <summary>
		/// Traverse the tree using a <see cref="TreeTraversal">tree traversal
		/// method</see>. Can be iterated with a foreach loop.
		/// </summary>
		/// <param name="method">The tree traversal method. Common methods are
		/// available in the <see cref="TreeTraversal"/>static class.</param>
		/// <returns>An <see cref="IEnumerable{T}"/> object which can be used to
		/// traverse the tree using the chosen traversal method.</returns>
		public IEnumerable<TNode> Traverse(BinaryTraversalMethod<TNode> method)
		{
			return new BinaryTreeTraversal(this, method);
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private class BinaryTreeTraversal : IEnumerable<TNode>
		{
			private readonly BinaryTree<TNode> _tree;
			private readonly BinaryTraversalMethod<TNode> _method;

			public BinaryTreeTraversal(BinaryTree<TNode> tree, BinaryTraversalMethod<TNode> method)
			{
				_tree = tree;
				_method = method;
			}

			public IEnumerator<TNode> GetEnumerator()
			{
				return _method(_tree);
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return _method(_tree);
			}
		}
	}
}