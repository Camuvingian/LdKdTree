using System;
using System.Collections.Generic;
using IndoorMapping.Tools.Trees.LdKdTree.Helpers;
using IndoorMapping.Tools.Trees.LdKdTree.Metrics;
using IndoorMapping.Tools.Trees.LdKdTree.Nodes;

namespace IndoorMapping.Tools.Trees.LdKdTree
{
	/// <summary>
	/// Base class for K-dimensional trees.
	/// </summary>
	/// <typeparam name="TNode">The class type for the nodes of the tree.</typeparam>
	public class KdTreeBase<TNode> : BinaryTree<TNode>, IEnumerable<TNode> where TNode : KdTreeNodeBase<TNode>, IComparable<TNode>, new()
	{
		public KdTreeBase(int dimensions)
		{
			Dimensions = dimensions;
		}

		public KdTreeBase(int dimension, TNode root) : this(dimension)
		{
			Root = root;

			foreach (var node in this)
			{
				Count++;

				if (node.IsLeaf)
				{
					Leaves++;
				}
			}
		}

		/// <summary>
		/// Creates a new <see cref="KdTree&lt;T&gt;"/>.
		/// </summary>
		/// <param name="dimension">The number of dimensions in the tree.</param>
		/// <param name="root">The Root node, if already existent.</param>
		/// <param name="count">The number of elements in the Root node.</param>
		/// <param name="leaves">The number of leaves linked through the Root node.</param>
		public KdTreeBase(int dimension, TNode root, int count, int leaves) : this(dimension)
		{
			Root = root;
			Count = count;
			Leaves = leaves;
		}

		/// <summary>
		/// Retrieves the nearest points to a given point within a given radius.
		/// </summary>
		/// <param name="position">The queried point.</param>
		/// <param name="radius">The search radius.</param>
		/// <param name="maximum">The maximum number of neighbors to retrieve.</param>
		/// <returns>A list of neighbor points, ordered by distance.</returns>
		public ICollection<NodeDistance<TNode>> Nearest(double[] position, double radius, int maximum)
		{
			if (maximum == 0)
			{
				var list = new List<NodeDistance<TNode>>();

				if (Root != null)
				{
					Nearest(Root, position, radius, list);
				}

				return list;
			}
			else
			{
				var list = new KdTreeNodeCollection<TNode>(maximum);

				if (Root != null)
				{
					Nearest(Root, position, radius, list);
				}

				return list;
			}
		}

		/// <summary>
		/// Retrieves the nearest points to a given point within a given radius.
		/// </summary>
		/// <param name="position">The queried point.</param>
		/// <param name="radius">The search radius.</param>
		/// <returns>A list of neighbor points, ordered by distance.</returns>
		public List<NodeDistance<TNode>> Nearest(double[] position, double radius)
		{
			var list = new List<NodeDistance<TNode>>();

			if (Root != null)
			{
				Nearest(Root, position, radius, list);
			}

			return list;
		}

		/// <summary>
		/// Retrieves a fixed number of nearest points to a given point.
		/// </summary>
		/// <param name="position">The queried point.</param>
		/// <param name="neighbors">The number of neighbors to retrieve.</param>
		/// <returns>A list of neighbor points, ordered by distance.</returns>
		public KdTreeNodeCollection<TNode> Nearest(double[] position, int neighbors)
		{
			var list = new KdTreeNodeCollection<TNode>(size: neighbors);

			if (Root != null)
			{
				Nearest(Root, position, list);
			}

			return list;
		}

		/// <summary>
		/// Retrieves the nearest point to a given point.
		/// </summary>
		/// <param name="position">The queried point.</param>
		/// <returns>A list of neighbor points, ordered by distance.</returns>
		public TNode Nearest(double[] position)
		{
			return Nearest(position, out _);
		}

		/// <summary>
		/// Retrieves the nearest point to a given point.
		/// </summary>
		/// <param name="position">The queried point.</param>
		/// <param name="distance">The distance from the <paramref name="position"/>
		/// to its nearest neighbor found in the tree.</param>
		/// <returns>A list of neighbor points, ordered by distance.</returns>
		public TNode Nearest(double[] position, out double distance)
		{
			TNode result = Root;
			distance = Metric.Distance(Root.Position, position);

			Nearest(Root, position, ref result, ref distance);
			return result;
		}

		/// <summary>
		/// Retrieves a fixed percentage of nearest points to a given point.
		/// </summary>
		/// <param name="position">The queried point.</param>
		/// <param name="neighbors">The number of neighbors to retrieve.</param>
		/// <param name="percentage">The maximum percentage of leaf nodes that
		/// can be visited before the search finishes with an approximate answer.</param>
		/// <returns>A list of neighbor points, ordered by distance.</returns>
		public KdTreeNodeCollection<TNode> ApproximateNearest(double[] position, int neighbors, double percentage)
		{
			int maxLeaves = (int)(Leaves * percentage);
			var list = new KdTreeNodeCollection<TNode>(size: neighbors);

			if (Root != null)
			{
				int visited = 0;
				Approximate(Root, position, list, maxLeaves, ref visited);
			}

			return list;
		}

		/// <summary>
		/// Retrieves a percentage of nearest points to a given point.
		/// </summary>
		/// <param name="position">The queried point.</param>
		/// <param name="percentage">The maximum percentage of leaf nodes that
		/// can be visited before the search finishes with an approximate answer.</param>
		/// <param name="distance">The distance between the query point and its nearest neighbor.</param>
		/// <returns>A list of neighbor points, ordered by distance.</returns>
		public TNode ApproximateNearest(double[] position, double percentage, out double distance)
		{
			TNode result = Root;
			distance = Metric.Distance(Root.Position, position);

			int maxLeaves = (int)(Leaves * percentage);

			int visited = 0;
			ApproximateNearest(Root, position, ref result, ref distance, maxLeaves, ref visited);

			return result;
		}

		/// <summary>
		/// Retrieves a percentage of nearest points to a given point.
		/// </summary>
		/// <param name="position">The queried point.</param>
		/// <param name="percentage">The maximum percentage of leaf nodes that
		/// can be visited before the search finishes with an approximate answer.</param>
		/// <returns>A list of neighbor points, ordered by distance.</returns>
		public TNode ApproximateNearest(double[] position, double percentage)
		{
			var list = ApproximateNearest(position, neighbors: 1, percentage: percentage);
			return list.Nearest;
		}

		/// <summary>
		/// Retrieves a fixed number of nearest points to a given point.
		/// </summary>
		/// <param name="position">The queried point.</param>
		/// <param name="neighbors">The number of neighbors to retrieve.</param>
		/// <param name="maxLeaves">The maximum number of leaf nodes that can
		/// be visited before the search finishes with an approximate answer.</param>
		/// <returns>A list of neighbor points, ordered by distance.</returns>
		public KdTreeNodeCollection<TNode> ApproximateNearest(double[] position, int neighbors, int maxLeaves)
		{
			var list = new KdTreeNodeCollection<TNode>(size: neighbors);
			if (Root != null)
			{
				int visited = 0;
				Approximate(Root, position, list, maxLeaves, ref visited);
			}
			return list;
		}

		/// <summary>
		/// Retrieves a fixed number of nearest points to a given point.
		/// </summary>
		/// <param name="position">The queried point.</param>
		/// <param name="maxLeaves">The maximum number of leaf nodes that can
		/// be visited before the search finishes with an approximate answer.</param>
		/// <returns>A list of neighbor points, ordered by distance.</returns>
		public TNode ApproximateNearest(double[] position, int maxLeaves)
		{
			var list = ApproximateNearest(position, neighbors: 1, maxLeaves: maxLeaves);
			return list.Nearest;
		}

		/// <summary>
		/// Retrieves a list of all points inside a given region.
		/// </summary>
		/// <param name="region">The region.</param>
		/// <returns>A list of all nodes contained in the region.</returns>
		public IList<TNode> GetNodesInsideRegion(HyperRectangle region)
		{
			return GetNodesInsideRegion(this.Root, region, region);
		}

		private IList<TNode> GetNodesInsideRegion(TNode node, HyperRectangle region, HyperRectangle subRegion)
		{
			var result = new List<TNode>();

			if (node != null && region.IntersectsWith(subRegion))
			{
				if (region.Contains(node.Position))
				{
					result.Add(node);
				}

				result.AddRange(GetNodesInsideRegion(node.Left, region, LeftRectangle(subRegion, node)));
				result.AddRange(GetNodesInsideRegion(node.Right, region, RightRectangle(subRegion, node)));
			}
			return result;
		}

		// TODO: Optimize the two methods below. It shouldn't be necessary to make copies/clones of these arrays
		private static HyperRectangle LeftRectangle(HyperRectangle hyperrect, TNode node)
		{
			//var rect = hyperrect.ToRectangle();
			//return (node.Axis != 0 ?
			//    Rectangle.FromLTRB(rect.Left, rect.Top, rect.Right, (int)node.Position[1]) :
			//    Rectangle.FromLTRB(rect.Left, rect.Top, (int)node.Position[0], rect.Bottom)).ToHyperRectangle();
			var copy = new HyperRectangle((double[])hyperrect.Minimum.Clone(), (double[])hyperrect.Maximum.Clone());
			copy.Maximum[node.Axis] = node.Position[node.Axis];
			return copy;
		}

		// helper: get the right rectangle of node inside parent's rect
		private static HyperRectangle RightRectangle(HyperRectangle hyperrect, TNode node)
		{
			//var rect = hyperrect.ToRectangle();
			//return (node.Axis != 0 ?
			//    Rectangle.FromLTRB(rect.Left, (int)node.Position[1], rect.Right, rect.Bottom) :
			//    Rectangle.FromLTRB((int)node.Position[0], rect.Top, rect.Right, rect.Bottom)).ToHyperRectangle();
			var copy = new HyperRectangle((double[])hyperrect.Minimum.Clone(), (double[])hyperrect.Maximum.Clone());
			copy.Minimum[node.Axis] = node.Position[node.Axis];
			return copy;
		}

		#region Internal Methods.
		/// <summary>
		/// Creates the Root node for a new <see cref="KdTree{T}"/> given
		/// a set of data points and their respective stored values.
		/// </summary>
		/// <param name="points">The data points to be inserted in the tree.</param>
		/// <param name="leaves">Return the number of leaves in the Root subtree.</param>
		/// <param name="inPlace">Whether the given <paramref name="points"/> vector
		/// can be ordered in place. Passing true will change the original order of
		/// the vector. If set to false, all operations will be performed on an extra
		/// copy of the vector.</param>
		/// <returns>The Root node for a new <see cref="KdTree{T}"/>
		/// contained the given <paramref name="points"/>.</returns>
		protected static TNode CreateRoot(double[][] points, bool inPlace, out int leaves)
		{
			// Initial argument checks for creating the tree.
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}

			if (!inPlace)
			{
				points = (double[][])points.Clone();
			}

			leaves = 0;
			int dimensions = points[0].Length;

			// Create a comparer to compare individual array elements at specified positions when sorting.
			ElementComparer comparer = new();

			// Call the recursive algorithm to operate on the whole array (from 0 to points.Length).
			TNode root = Create(points, 0, dimensions, 0, points.Length, comparer, ref leaves);

			// Create and return the newly formed tree.
			return root;
		}

		private static TNode Create(double[][] points, int depth, int k, int start, int length, ElementComparer comparer, ref int leaves)
		{
			if (length <= 0)
			{
				return null;
			}

			// We will be doing sorting in place.
			int axis = comparer.Index = depth % k;
			Array.Sort(points, start, length, comparer);

			// Middle of the input section.
			int half = start + length / 2;

			// Start and end of the left branch.
			int leftStart = start;
			int leftLength = half - start;

			// Start and end of the right branch.
			int rightStart = half + 1;
			int rightLength = length - length / 2 - 1;

			// The median will be located halfway in the sorted array.
			var median = points[half];

			depth++;

			// Continue with the recursion, passing the appropriate left and right array sections
			var left = Create(points, depth, k, leftStart, leftLength, comparer, ref leaves);
			var right = Create(points, depth, k, rightStart, rightLength, comparer, ref leaves);

			if (left == null && right == null)
			{
				leaves++;
			}

			// Backtrack and create
			return new TNode()
			{
				Axis = axis,
				Position = median,
				Left = left,
				Right = right,
			};
		}
		#endregion // Internal Methods.

		#region Recursive Methods.
		/// <summary>
		/// Radius search.
		/// </summary>
		private void Nearest(TNode current, double[] position, double radius, ICollection<NodeDistance<TNode>> list)
		{
			// Check if the distance of the point from this node is within the desired radius, and if it
			// is, add to the list of nearest nodes.
			double d = Metric.Distance(position, current.Position);

			if (d <= radius)
			{
				list.Add(new NodeDistance<TNode>(current, d));
			}

			// Prepare for recursion. The following null checks will be used to avoid function calls if possible.

			double value = position[current.Axis];
			double median = current.Position[current.Axis];
			double u = value - median;

			if (u <= 0)
			{
				if (current.Left != null)
				{
					Nearest(current.Left, position, radius, list);
				}

				if (current.Right != null && Math.Abs(u) <= radius)
				{
					Nearest(current.Right, position, radius, list);
				}
			}
			else
			{
				if (current.Right != null)
				{
					Nearest(current.Right, position, radius, list);
				}

				if (current.Left != null && Math.Abs(u) <= radius)
				{
					Nearest(current.Left, position, radius, list);
				}
			}
		}

		/// <summary>
		/// k-nearest neighbors search.
		/// </summary>
		private void Nearest(TNode current, double[] position, KdTreeNodeCollection<TNode> list)
		{
			// Compute distance from this node to the point
			double d = Metric.Distance(position, current.Position);
			list.Add(current, d);

			// Check for leafs on the opposite sides of the subtrees to nearest possible neighbors.
			// Prepare for recursion. The following null checks will be used to avoid function calls if possible.

			double value = position[current.Axis];
			double median = current.Position[current.Axis];
			double u = value - median;

			if (u <= 0)
			{
				if (current.Left != null)
				{
					Nearest(current.Left, position, list);
				}

				if (current.Right != null && Math.Abs(u) <= list.Maximum)
				{
					Nearest(current.Right, position, list);
				}
			}
			else
			{
				if (current.Right != null)
				{
					Nearest(current.Right, position, list);
				}

				if (current.Left != null && Math.Abs(u) <= list.Maximum)
				{
					Nearest(current.Left, position, list);
				}
			}
		}

		private void Nearest(TNode current, double[] position, ref TNode match, ref double minDistance)
		{
			// Compute distance from this node to the point
			double d = Metric.Distance(position, current.Position);

			if (d < minDistance)
			{
				minDistance = d;
				match = current;
			}

			// Check for leafs on the opposite sides of the subtrees to nearest possible neighbors.
			// Prepare for recursion. The following null checks will be used to avoid function calls if possible.

			double value = position[current.Axis];
			double median = current.Position[current.Axis];
			double u = value - median;

			if (u <= 0)
			{
				if (current.Left != null)
				{
					Nearest(current.Left, position, ref match, ref minDistance);
				}

				if (current.Right != null && u <= minDistance)
				{
					Nearest(current.Right, position, ref match, ref minDistance);
				}
			}
			else
			{
				if (current.Right != null)
				{
					Nearest(current.Right, position, ref match, ref minDistance);
				}

				if (current.Left != null && u <= minDistance)
				{
					Nearest(current.Left, position, ref match, ref minDistance);
				}
			}
		}

		private bool Approximate(TNode current, double[] position, KdTreeNodeCollection<TNode> list, int maxLeaves, ref int visited)
		{
			// Compute distance from this node to the point
			double d = Metric.Distance(position, current.Position);

			list.Add(current, d);

			if (++visited > maxLeaves)
			{
				return true;
			}

			// Check for leafs on the opposite sides of the subtrees to nearest possible neighbors.
			// Prepare for recursion. The following null checks will be used to avoid function calls if possible.

			double value = position[current.Axis];
			double median = current.Position[current.Axis];
			double u = value - median;

			if (u <= 0)
			{
				if (current.Left != null)
				{
					if (Approximate(current.Left, position, list, maxLeaves, ref visited))
						return true;
				}

				if (current.Right != null && Math.Abs(u) <= list.Maximum)
				{
					if (Approximate(current.Right, position, list, maxLeaves, ref visited))
						return true;
				}
			}
			else
			{
				if (current.Right != null)
				{
					Approximate(current.Right, position, list, maxLeaves, ref visited);
				}

				if (current.Left != null && Math.Abs(u) <= list.Maximum)
				{
					if (Approximate(current.Left, position, list, maxLeaves, ref visited))
						return true;
				}
			}
			return false;
		}

		private bool ApproximateNearest(TNode current, double[] position, ref TNode match, ref double minDistance, int maxLeaves, ref int visited)
		{
			// Compute distance from this node to the point.
			double d = Metric.Distance(position, current.Position);

			// Base: node is leaf.
			if (d < minDistance)
			{
				minDistance = d;
				match = current;
			}

			if (++visited > maxLeaves)
			{
				return true;
			}

			// Check for leafs on the opposite sides of the subtrees to nearest possible neighbors.
			// Prepare for recursion. The following null checks will be used to avoid function calls if possible.

			double value = position[current.Axis];
			double median = current.Position[current.Axis];
			double u = value - median;

			if (u <= 0)
			{
				if (current.Left != null)
				{
					if (ApproximateNearest(current.Left, position, ref match, ref minDistance, maxLeaves, ref visited))
						return true;
				}

				if (current.Right != null && Math.Abs(u) <= minDistance)
				{
					if (ApproximateNearest(current.Right, position, ref match, ref minDistance, maxLeaves, ref visited))
						return true;
				}
			}
			else
			{
				if (current.Right != null)
				{
					ApproximateNearest(current.Right, position, ref match, ref minDistance, maxLeaves, ref visited);
				}

				if (current.Left != null && Math.Abs(u) <= minDistance)
				{
					if (ApproximateNearest(current.Left, position, ref match, ref minDistance, maxLeaves, ref visited))
						return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Inserts a value into the tree at the desired position.
		/// </summary>
		/// <param name="position">A double-vector with the same number of elements as dimensions in the tree.</param>
		protected TNode AddNode(double[] position)
		{
			Count++;
			var root = Root;
			TNode node = Insert(ref root, position, 0);
			Root = root;
			return node;
		}

		private TNode Insert(ref TNode node, double[] position, int depth)
		{
			if (node == null)
			{
				// Base case: node is null
				return node = new TNode()
				{
					Axis = depth % Dimensions,
					Position = position,
				};
			}
			else
			{
				TNode newNode;

				// Recursive case: keep looking for a position to insert
				if (position[node.Axis] < node.Position[node.Axis])
				{
					TNode child = node.Left;
					newNode = Insert(ref child, position, depth + 1);
					node.Left = child;
				}
				else
				{
					TNode child = node.Right;
					newNode = Insert(ref child, position, depth + 1);
					node.Right = child;
				}

				return newNode;
			}
		}
		#endregion // Recursive Methods.

		/// <summary>
		/// Removes all nodes from this tree.
		/// </summary>
		public void Clear()
		{
			Root = null;
		}

		/// <summary>
		/// Copies the entire tree to a compatible one-dimensional <see cref="System.Array"/>, starting
		/// at the specified <paramref name="arrayIndex">index</paramref> of the <paramref name="array">
		/// target array</paramref>.
		/// </summary>
		/// <param name="array">The one-dimensional <see cref="System.Array"/> that is the destination of the
		/// elements copied from tree. The <see cref="System.Array"/> must have zero-based indexing.</param>
		/// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
		public void CopyTo(TNode[] array, int arrayIndex)
		{
			foreach (var node in this)
			{
				array[arrayIndex++] = node;
			}
		}

		public int Dimensions { get; }

		/// <summary>
		/// Gets or set the distance function used to measure distances amongst points on this tree
		/// </summary>
		public IMetric<double[]> Metric { get; set; } = new EuclideanMetric();

		/// <summary>
		/// Gets the number of elements contained in this
		/// tree. This is also the number of tree nodes.
		/// </summary>
		public int Count { get; private set; }

		/// <summary>
		/// Gets the number of leaves contained in this
		/// tree. This can be used to calibrate approximate
		/// nearest searchers.
		/// </summary>
		public int Leaves { get; }
	}
}