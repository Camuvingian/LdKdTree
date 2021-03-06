using System;
using System.Collections.Generic;
using IndoorMapping.Tools.Trees.LdKdTree.Nodes;

namespace IndoorMapping.Tools.Trees.LdKdTree.Helpers
{
	/// <summary>
	/// Static class with tree traversal methods.
	/// </summary>
	public static class TreeTraversal
	{
		public static IEnumerator<TNode> BreadthFirst<TNode>(BinaryTree<TNode> tree) where TNode : BinaryNode<TNode>, new()
		{
			if (tree.Root == null)
			{
				yield break;
			}

			var queue = new Queue<TNode>(new[] { tree.Root });

			while (queue.Count != 0)
			{
				TNode current = queue.Dequeue();

				if (current != null)
				{
					yield return current;
				}

				if (current.Left != null)
				{
					queue.Enqueue(current.Left);
				}

				if (current.Right != null)
				{
					queue.Enqueue(current.Right);
				}
			}
		}

		public static IEnumerator<TNode> PreOrder<TNode>(BinaryTree<TNode> tree) where TNode : BinaryNode<TNode>, new()
		{
			if (tree.Root == null)
			{
				yield break;
			}

			var stack = new Stack<TNode>();
			TNode current = tree.Root;

			while (stack.Count != 0 || current != null)
			{
				if (current != null)
				{
					stack.Push(current);
					yield return current;
					current = current.Left;
				}
				else
				{
					current = stack.Pop();
					current = current.Right;
				}
			}
		}

		public static IEnumerator<TNode> InOrder<TNode>(BinaryTree<TNode> tree) where TNode : BinaryNode<TNode>, new()
		{
			if (tree.Root == null)
			{
				yield break;
			}

			var stack = new Stack<TNode>();
			TNode current = tree.Root;

			while (stack.Count != 0 || current != null)
			{
				if (current != null)
				{
					stack.Push(current);
					current = current.Left;
				}
				else
				{
					current = stack.Pop();
					yield return current;
					current = current.Right;
				}
			}
		}

		public static IEnumerator<TNode> PostOrder<TNode>(BinaryTree<TNode> tree) where TNode : BinaryNode<TNode>, new()
		{
			if (tree.Root == null)
			{
				yield break;
			}

			var stack = new Stack<TNode>(new[] { tree.Root });
			TNode previous = tree.Root;

			while (stack.Count != 0)
			{
				TNode current = stack.Peek();

				if (previous == current || previous.Left == current || previous.Right == current)
				{
					if (current.Left != null)
					{
						stack.Push(current.Left);
					}
					else if (current.Right != null)
					{
						stack.Push(current.Right);
					}
					else
					{
						yield return stack.Pop();
					}
				}
				else if (current.Left == previous)
				{
					if (current.Right != null)
					{
						stack.Push(current.Right);
					}
					else
					{
						yield return stack.Pop();
					}
				}
				else if (current.Right == previous)
				{
					yield return stack.Pop();
				}
				else
				{
					throw new InvalidOperationException();
				}

				previous = current;
			}
		}

		public static IEnumerator<TNode> DepthFirst<TNode>(BinaryTree<TNode> tree) where TNode : BinaryNode<TNode>
		{
			if (tree.Root == null)
			{
				yield break;
			}

			var stack = new Stack<TNode>();
			stack.Push(tree.Root);

			while (stack.Count != 0)
			{
				TNode node = stack.Pop();

				if (node != null)
				{
					yield return node;
				}

				if (node.Left != null)
				{
					stack.Push(node.Left);
				}

				if (node.Right != null)
				{
					stack.Push(node.Right);
				}
			}
		}
	}
}