using System;
using IndoorMapping.Tools.Trees.LdKdTree.Helpers;
using IndoorMapping.Tools.Trees.LdKdTree.Metrics;
using IndoorMapping.Tools.Trees.LdKdTree.Nodes;

namespace IndoorMapping.Tools.Trees.LdKdTree
{
	public class KdTree<T> : KdTreeBase<KdTreeNode<T>>
	{
		/// <summary>
		/// Creates a new <see cref="KdTree&lt;T&gt;"/>.
		/// </summary>
		/// <param name="dimensions">The number of dimensions in the tree.</param>
		public KdTree(int dimensions) : base(dimensions) { }

		/// <summary>
		/// Creates a new <see cref="KdTree&lt;T&gt;"/>.
		/// </summary>
		/// <param name="dimension">The number of dimensions in the tree.</param>
		/// <param name="root">The Root node, if already existent.</param>
		public KdTree(int dimension, KdTreeNode<T> root) : base(dimension, root) { }

		/// <summary>
		/// Creates a new <see cref="KdTree&lt;T&gt;"/>.
		/// </summary>
		/// <param name="dimension">The number of dimensions in the tree.</param>
		/// <param name="root">The Root node, if already existent.</param>
		/// <param name="count">The number of elements in the Root node.</param>
		/// <param name="leaves">The number of leaves linked through the Root node.</param>
		public KdTree(int dimension, KdTreeNode<T> root, int count, int leaves) : base(dimension, root, count, leaves) { }

		/// <summary>
		/// Inserts a value in the tree at the desired position.
		/// </summary>
		/// <param name="position">A double-vector with the same number of elements as dimensions in the tree.</param>
		/// <param name="value">The value to be inserted.</param>
		public void Add(double[] position, T value)
		{
			AddNode(position).Value = value;
		}

		/// <summary>
		/// Creates the Root node for a new <see cref="KdTree{T}"/> given
		/// a set of data points and their respective stored values.
		/// </summary>
		/// <param name="points">The data points to be inserted in the tree.</param>
		/// <param name="values">The values associated with each point.</param>
		/// <param name="leaves">Return the number of leaves in the Root subtree.</param>
		/// <param name="inPlace">Whether the given <paramref name="points"/> vector
		/// can be ordered in place. Passing true will change the original order of
		/// the vector. If set to false, all operations will be performed on an extra
		/// copy of the vector.</param>
		/// <returns>The Root node for a new <see cref="KdTree{T}"/>
		/// contained the given <paramref name="points"/>.</returns>
		internal static KdTreeNode<T> CreateRoot(double[][] points, T[] values, bool inPlace, out int leaves)
		{
			Console.Log("CreateRoot", points);

			createRootCalls++;
			Console.Log("createRootCalls", createRootCalls, null, LogDetail.HIGH);

			// Initial argument checks for creating the tree.
			if (points == null)
			{
				throw new ArgumentNullException(nameof(points));
			}

			Console.Log("CreateRoot points.Length", points.Length);
			Console.Log("CreateRoot values.Length", values.Length);

			if (values != null && points.Length != values.Length)
			{
				throw new ArgumentException("values and points must have the same dimension");
			}

			Console.Log("CreateRoot inPlace", inPlace);

			if (!inPlace)
			{
				points = (double[][])points.Clone();

				Console.Log("CreateRoot points", points.Length);

				if (values != null)
				{
					values = (T[])values.Clone();
				}
			}

			leaves = 0;
			int dimensions = points[0].Length;

			// Create a comparer to compare individual array elements at specified positions when sorting.
			var comparer = new ElementComparer();

			Console.Log("CreateRoot comparer", comparer);

			Console.Log("CreateRoot points", points);
			Console.Log("CreateRoot values", values.Length);
			Console.Log("CreateRoot dimensions", dimensions);
			Console.Log("CreateRoot points.Length", points.Length);
			Console.Log("CreateRoot comparer", comparer);
			Console.Log("CreateRoot leaves", leaves);

			// Call the recursive algorithm to operate on the whole array (from 0 to points.Length).
			KdTreeNode<T> root = Create(points, values, 0, dimensions, 0, points.Length, comparer, ref leaves);

			Console.Log("CreateRoot root", root);

			//Console.Log("CreateRoot createCompleteCalls", createCompleteCalls, null, LogDetail.HIGHLIGHT);

			// Create and return the newly formed tree.
			return root;
		}


		private static int createRootCalls = 0;
		private static int createCalls = 0;
		private static int createCompleteCalls = 0;


		private static KdTreeNode<T> Create(double[][] points, T[] values, int depth, int k,
			int start, int length, ElementComparer comparer, ref int leaves)
		{
			createCalls++;
			Console.Log("Create", createCalls, null, LogDetail.VERY_HIGH);
			Console.Log("Create length", length);

			if (length <= 0)
			{
				return null;
			}

			// We will be doing sorting in place.
			int axis = comparer.Index = depth % k;
			Array.Sort(points, values, start, length, comparer);

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
			var value = values != null ? values[half] : default(T);

			depth++;

			Console.Log("Create depth", depth);
			Console.Log("Create leaves 1 ", leaves);

			// Continue with the recursion, passing the appropriate left and right array sections.
			var left = Create(points, values, depth, k, leftStart, leftLength, comparer, ref leaves);
			var right = Create(points, values, depth, k, rightStart, rightLength, comparer, ref leaves);

			if (left == null && right == null)
			{
				leaves++;
			}

			Console.Log("Create leaves 2 ", leaves);

			//Console.Log("Create axis", axis);
			//Console.Log("Create median", median);
			//Console.Log("Create value", value);
			//Console.Log("Create left", left);
			//Console.Log("Create right", right, null, LogDetail.HIGHLIGHT);

			createCompleteCalls++;
			Console.Log("Create complete", createCompleteCalls, null, LogDetail.HIGHLIGHT);

			// Backtrack and create.
			return new KdTreeNode<T>()
			{
				Axis = axis,
				Position = median,
				Value = value,
				Left = left,
				Right = right,
			};
		}
	}

	/// <summary>
	/// Convenience class for k-dimensional tree static methods. To 
	/// create a new KdTree, specify the generic parameter as in <see cref="KdTree{T}"/>.
	/// </summary>
	public class KdTree : KdTreeBase<KdTreeNode>
	{
		public KdTree(int dimensions) : base(dimensions) { }

		public KdTree(int dimension, KdTreeNode root) : base(dimension, root) { }

		public KdTree(int dimension, KdTreeNode root, int count, int leaves) : base(dimension, root, count, leaves) { }

		/// <summary>
		/// Adds a new point to this tree.
		/// </summary>
		/// <param name="position">A double-vector with the same number of elements as dimensions in the tree.</param>
		public void Add(double[] position)
		{
			base.AddNode(position);
		}

		/// <summary>
		/// Creates a new k-dimensional tree from the given points.
		/// </summary>
		/// <typeparam name="T">The type of the value to be stored.</typeparam>
		/// <param name="points">The points to be added to the tree.</param>
		/// <param name="inPlace">Whether the given <paramref name="points"/> vector
		/// can be ordered in place. Passing true will change the original order of
		/// the vector. If set to false, all operations will be performed on an extra
		/// copy of the vector.</param>
		/// <returns>A <see cref="KdTree{T}"/> populated with the given data points.</returns>
		public static KdTree<T> FromData<T>(double[][] points, bool inPlace = false)
		{
			if (points == null)
			{
				throw new ArgumentNullException(nameof(points));
			}

			if (points.Length == 0)
			{
				throw new ArgumentException("Insufficient points for creating a tree.");
			}

			var root = KdTree<T>.CreateRoot(points, inPlace, out int leaves);
			return new KdTree<T>(points[0].Length, root, points.Length, leaves);
		}

		/// <summary>
		/// Creates a new k-dimensional tree from the given points.
		/// </summary>
		/// <param name="points">The points to be added to the tree.</param>
		/// <param name="inPlace">Whether the given <paramref name="points"/> vector
		/// can be ordered in place. Passing true will change the original order of
		/// the vector. If set to false, all operations will be performed on an extra
		/// copy of the vector.</param>
		/// <returns>A <see cref="KdTree{T}"/> populated with the given data points.</returns>
		public static KdTree FromData(double[][] points, bool inPlace = false)
		{
			if (points == null)
			{
				throw new ArgumentNullException(nameof(points));
			}

			if (points.Length == 0)
			{
				throw new ArgumentException("Insufficient points for creating a tree.");
			}

			var root = CreateRoot(points, inPlace, out int leaves);
			return new KdTree(points[0].Length, root, points.Length, leaves);
		}

		/// <summary>
		/// Creates a new k-dimensional tree from the given points.
		/// </summary>
		/// <typeparam name="T">The type of the value to be stored.</typeparam>
		/// <param name="points">The points to be added to the tree.</param>
		/// <param name="values">The corresponding values at each data point.</param>
		/// <param name="inPlace">Whether the given <paramref name="points"/> vector
		/// can be ordered in place. Passing true will change the original order of
		/// the vector. If set to false, all operations will be performed on an extra
		/// copy of the vector.</param>
		/// <returns>A <see cref="KdTree{T}"/> populated with the given data points.</returns>
		public static KdTree<T> FromData<T>(double[][] points, T[] values, bool inPlace = false)
		{
			Console.Log("FromData", points);

			if (points == null)
			{
				throw new ArgumentNullException(nameof(points));
			}

			Console.Log("FromData", points.Length);

			if (points.Length == 0)
			{
				throw new ArgumentException("Insufficient points for creating a tree.");
			}

			Console.Log("FromData values", values);
			Console.Log("FromData values.Length", values.Length);
			Console.Log("FromData inPlace", inPlace);

			var root = KdTree<T>.CreateRoot(points, values, inPlace, out int leaves);

			Console.Log("FromData leaves 2", leaves);

			Console.Log("FromData root", root);
			Console.Log("FromData points[0].Length", points[0].Length);
			Console.Log("FromData leaves", leaves);

			return new KdTree<T>(points[0].Length, root, points.Length, leaves);
		}

		/// <summary>
		/// Creates a new k-dimensional tree from the given points.
		/// </summary>
		/// <param name="points">The points to be added to the tree.</param>
		/// <param name="distance">The distance function to use.</param>
		/// <param name="inPlace">Whether the given <paramref name="points"/> vector
		/// can be ordered in place. Passing true will change the original order of
		/// the vector. If set to false, all operations will be performed on an extra
		/// copy of the vector.</param>
		/// <returns>A <see cref="KdTree{T}"/> populated with the given data points.</returns>
		public static KdTree FromData(double[][] points, IMetric<double[]> distance, bool inPlace = false)
		{
			if (points == null)
			{
				throw new ArgumentNullException(nameof(points));
			}

			if (distance == null)
			{
				throw new ArgumentNullException(nameof(distance));
			}

			if (points.Length == 0)
			{
				throw new ArgumentException("Insufficient points for creating a tree.");
			}

			var root = CreateRoot(points, inPlace, out int leaves);
			return new KdTree(points[0].Length, root, points.Length, leaves)
			{
				Metric = distance,
			};
		}

		/// <summary>
		/// Creates a new k-dimensional tree from the given points.
		/// </summary>
		/// <typeparam name="T">The type of the value to be stored.</typeparam>
		/// <param name="points">The points to be added to the tree.</param>
		/// <param name="values">The corresponding values at each data point.</param>
		/// <param name="distance">The distance function to use.</param>
		/// <param name="inPlace">Whether the given <paramref name="points"/> vector can be ordered in place. 
		/// Passing true will change the original order of the vector. If set to false, all operations will be performed on an extra
		/// copy of the vector.</param>
		/// <returns>A <see cref="KdTree{T}"/> populated with the given data points.</returns>
		public static KdTree<T> FromData<T>(double[][] points, T[] values, IMetric<double[]> distance, bool inPlace = false)
		{
			if (values == null)
			{
				throw new ArgumentNullException(nameof(values));
			}

			if (distance == null)
			{
				throw new ArgumentNullException(nameof(distance));
			}

			var root = KdTree<T>.CreateRoot(points, values, inPlace, out int leaves);
			return new KdTree<T>(points[0].Length, root, points.Length, leaves)
			{
				Metric = distance,
			};
		}

		/// <summary>
		/// Creates a new k-dimensional tree from the given points.
		/// </summary>
		/// <typeparam name="T">The type of the value to be stored.</typeparam>
		/// <param name="points">The points to be added to the tree.</param>
		/// <param name="distance">The distance function to use.</param>
		/// <param name="inPlace">Whether the given <paramref name="points"/> vector
		/// can be ordered in place. Passing true will change the original order of
		/// the vector. If set to false, all operations will be performed on an extra
		/// copy of the vector.</param>
		/// <returns>A <see cref="KdTree{T}"/> populated with the given data points.</returns>
		public static KdTree<T> FromData<T>(double[][] points, IMetric<double[]> distance, bool inPlace = false)
		{
			if (distance == null)
			{
				throw new ArgumentNullException(nameof(distance));
			}

			var root = KdTree<T>.CreateRoot(points, inPlace, out int leaves);
			return new KdTree<T>(points[0].Length, root, points.Length, leaves)
			{
				Metric = distance
			};
		}
	}
}