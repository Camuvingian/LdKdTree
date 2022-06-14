using System;

namespace IndoorMapping.Tools.Trees.LdKdTree.Nodes
{
	/// <summary>
	/// Node-distance pair.
	/// </summary>
	/// <typeparam name="TNode">The class type for the nodes of the tree.</typeparam>
	public struct NodeDistance<TNode> : IComparable<NodeDistance<TNode>>, IEquatable<NodeDistance<TNode>> where TNode : IEquatable<TNode>
	{
		public NodeDistance(TNode node, double distance)
		{
			Node = node;
			Distance = distance;
		}

		public override bool Equals(object obj)
		{
			if (obj is NodeDistance<TNode>)
			{
				var b = (NodeDistance<TNode>)obj;
				return Node.Equals(b.Node) && Distance == b.Distance;
			}

			return false;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hash = (int)2_166_136_261;

				hash = (hash * 16_777_619) ^ Node.GetHashCode();
				hash = (hash * 16_777_619) ^ Distance.GetHashCode();

				return hash;
			}
		}

		public static bool operator ==(NodeDistance<TNode> a, NodeDistance<TNode> b)
		{
			return a.Node.Equals(b.Node) && a.Distance == b.Distance;
		}

		public static bool operator !=(NodeDistance<TNode> a, NodeDistance<TNode> b)
		{
			return !a.Node.Equals(b.Node) || a.Distance != b.Distance;
		}

		public static bool operator <(NodeDistance<TNode> a, NodeDistance<TNode> b)
		{
			return a.Distance < b.Distance;
		}

		public static bool operator >(NodeDistance<TNode> a, NodeDistance<TNode> b)
		{
			return a.Distance > b.Distance;
		}

		public bool Equals(NodeDistance<TNode> other)
		{
			return Distance == other.Distance && Node.Equals(other.Node);
		}

		public int CompareTo(NodeDistance<TNode> other)
		{
			return Distance.CompareTo(other.Distance);
		}

		public int CompareTo(object obj)
		{
			return CompareTo((NodeDistance<TNode>)obj);
		}

		public override string ToString()
		{
			return String.Format("<{0}, {1}>", Node, Distance);
		}

		/// <summary>
		/// Gets the node in this pair.
		/// </summary>
		public TNode Node { get; }

		/// <summary>
		/// Gets the distance of the node from the query point.
		/// </summary>
		public double Distance { get; }
	}
}
