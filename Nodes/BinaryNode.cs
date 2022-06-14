using System;

namespace IndoorMapping.Tools.Trees.LdKdTree.Nodes
{
	/// <summary>
	/// Tree node for binary trees.
	/// </summary>
	public class BinaryNode<TNode> : IEquatable<TNode>, ITreeNode<TNode> where TNode : BinaryNode<TNode>
	{
		/// <summary>
		/// Gets whether this node is a leaf (has no children).
		/// </summary>
		public bool IsLeaf
		{
			get { return Left == default(TNode) && Right == default(TNode); }
		}

		public bool Equals(TNode other)
		{
			return this == other;
		}

		/// <summary>
		/// Gets or sets the collection of child nodes under this node.
		/// </summary>
		public TNode[] Children
		{
			get { return new[] { Left, Right }; }
			set
			{
				if (value.Length != 2)
				{
					throw new ArgumentException("The array must have length 2.", "value");
				}

				Left = value[0];
				Right = value[1];
			}
		}

		/// <summary>
		///  Gets or sets the left subtree of this node.
		/// </summary>
		public TNode Left { get; set; }

		/// <summary>
		/// Gets or sets the right subtree of this node.
		/// </summary>
		public TNode Right { get; set; }
	}
}