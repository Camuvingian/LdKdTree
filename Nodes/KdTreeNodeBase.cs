using System;
using System.Text;

namespace IndoorMapping.Tools.Trees.LdKdTree.Nodes
{
	public class KdTreeNodeBase<TNode> : BinaryNode<TNode>, IComparable<TNode>, IEquatable<TNode>
		where TNode : KdTreeNodeBase<TNode>
	{
		public int CompareTo(TNode other)
		{
			return Position[Axis].CompareTo(other.Position[other.Axis]);
		}

		public new bool Equals(TNode other)
		{
			return this == other;
		}

		/// <summary>
		/// Gets or sets the position of the node in spatial coordinates.
		/// </summary>
		public double[] Position { get; set; }

		/// <summary>
		/// Gets or sets the dimension index of the split. This value is a
		/// index of the <see cref="Position"/> vector and as such should
		/// be higher than zero and less than the number of elements in <see cref="Position"/>.
		/// </summary>
		public int Axis { get; set; }

		public override string ToString()
		{
			if (Position == null)
			{
				return "(null)";
			}

			StringBuilder sb = new();
			sb.Append("(");
			for (int i = 0; i < Position.Length; i++)
			{
				sb.Append(Position[i]);
				if (i < Position.Length - 1)
				{
					sb.Append(",");
				}
			}
			sb.Append(")");

			return sb.ToString();
		}
	}
}