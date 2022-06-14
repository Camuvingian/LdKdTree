using System;
using System.Collections.Generic;

namespace IndoorMapping.Tools.Trees.LdKdTree.Nodes
{
	public class KdTreeNodeCollection<TNode> : ICollection<NodeDistance<TNode>>
		where TNode : KdTreeNodeBase<TNode>, IComparable<TNode>, IEquatable<TNode>
	{
		private readonly double[] _distances;
		private readonly TNode[] _positions;

		/// <summary>
		/// Creates a new <see cref="KdTreeNodeCollection&lt;T&gt;"/> with a maximum size.
		/// </summary>
		/// <param name="size">The maximum number of elements allowed in this collection.</param>
		public KdTreeNodeCollection(int size)
		{
			if (size <= 0)
			{
				throw new ArgumentOutOfRangeException("size");
			}

			Capacity = size;

			_distances = new double[size];
			_positions = new TNode[size];
		}

		/// <summary>
		/// Attempts to add a value to the collection. If the list is full
		/// and the value is more distant than the farthest node in the
		/// collection, the value will not be added.
		/// </summary>
		/// <param name="value">The node to be added.</param>
		/// <param name="distance">The node distance.</param>
		/// <returns>Returns true if the node has been added; false otherwise.</returns>
		public bool Add(TNode value, double distance)
		{
			// The list does have a limit. We have to check if the list
			// is already full or not, to see if we can discard or keep
			// the point.

			if (Count < Capacity)
			{
				// The list still has room for new elements. 
				// Just add the value at the right position.
				Add(distance, value);

				return true; // a value has been added.
			}

			// The list is at its maximum capacity. Check if the value
			// to be added is closer than the current farthest point.

			if (distance < Maximum)
			{
				// Yes, it is closer. Remove the previous farthest point
				// and insert this new one at an appropriate position to
				// keep the list ordered.
				RemoveFarthest();
				Add(distance, value);

				return true; // a value has been added.
			}

			// The value is even farther.
			return false; // discard it.
		}

		/// <summary>
		/// Attempts to add a value to the collection. If the list is full
		/// and the value is more distant than the farthest node in the
		/// collection, the value will not be added.
		/// </summary>
		/// 
		/// <param name="value">The node to be added.</param>
		/// <param name="distance">The node distance.</param>
		/// 
		/// <returns>Returns true if the node has been added; false otherwise.</returns>
		/// 
		public bool AddFarthest(TNode value, double distance)
		{
			// The list does have a limit. We have to check if the list
			// is already full or not, to see if we can discard or keep
			// the point

			if (Count < Capacity)
			{
				// The list still has room for new elements. 
				// Just add the value at the right position.
				Add(distance, value);

				return true; // a value has been added
			}

			// The list is at its maximum capacity. Check if the value
			// to be added is farther than the current nearest point.

			if (distance > Minimum)
			{
				// Yes, it is farther. Remove the previous nearest point
				// and insert this new one at an appropriate position to
				// keep the list ordered.
				RemoveNearest();

				Add(distance, value);

				return true; // a value has been added.
			}

			// The value is even closer.
			return false; // discard it.
		}

		/// <summary>
		/// Adds the specified item to the collection.
		/// </summary>
		/// <param name="distance">The distance from the node to the query point.</param>
		/// <param name="item">The item to be added.</param>
		private void Add(double distance, TNode item)
		{
			_positions[Count] = item;
			_distances[Count] = distance;
			Count++;

			// Ensure it is in the right place.
			SiftUpLast();
		}

		public void Clear()
		{
			for (int i = 0; i < _positions.Length; i++)
			{
				_positions[i] = null;
			}

			Count = 0;
		}

		public NodeDistance<TNode> this[int index]
		{
			get { return new NodeDistance<TNode>(_positions[index], _distances[index]); }
		}

		public int Count { get; private set; }

		public bool IsReadOnly
		{
			get { return false; }
		}

		public IEnumerator<NodeDistance<TNode>> GetEnumerator()
		{
			for (int i = 0; i < Count; i++)
			{
				yield return new NodeDistance<TNode>(_positions[i], _distances[i]);
			}
			yield break;
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public bool Contains(NodeDistance<TNode> item)
		{
			int i = Array.IndexOf(_positions, item.Node);

			if (i == -1)
			{
				return false;
			}

			return _distances[i] == item.Distance;
		}

		/// <summary>
		/// Copies the entire collection to a compatible one-dimensional <see cref="System.Array"/>, starting
		/// at the specified <paramref name="arrayIndex">index</paramref> of the <paramref name="array">target
		/// array</paramref>.
		/// </summary>
		/// <param name="array">The one-dimensional <see cref="System.Array"/> that is the destination of the
		///  elements copied from tree. The <see cref="System.Array"/> must have zero-based indexing.</param>
		/// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
		public void CopyTo(NodeDistance<TNode>[] array, int arrayIndex)
		{
			int index = arrayIndex;

			foreach (var pair in this)
			{
				array[index++] = pair;
			}
		}

		/// <summary>
		/// Adds the specified item to this collection.
		/// </summary>
		/// <param name="item">The item.</param>
		public void Add(NodeDistance<TNode> item)
		{
			Add(item.Node, item.Distance);
		}

		public bool Remove(NodeDistance<TNode> item)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Removes the farthest tree node from this collection.
		/// </summary>
		public void RemoveFarthest()
		{
			// If we have no items in the queue.
			if (Count == 0)
			{
				throw new InvalidOperationException("The collection is empty.");
			}

			// If we have one item, remove the min.
			if (Count == 1)
			{
				RemoveNearest();
				return;
			}

			// Remove the max.
			Count--;

			_positions[1] = _positions[Count];
			_distances[1] = _distances[Count];

			_positions[Count] = null;
			SiftDownMax(1);
		}

		/// <summary>
		/// Removes the nearest tree node from this collection.
		/// </summary>
		public void RemoveNearest()
		{
			// Check for errors.
			if (Count == 0)
			{
				throw new InvalidOperationException("The collection is empty.");
			}

			// Remove the min
			Count--;

			_positions[0] = _positions[Count];
			_distances[0] = _distances[Count];

			_positions[Count] = null;
			SiftDownMin(0);
		}


		private void SiftUpLast()
		{
			// Work out where the element was inserted.
			int u = Count - 1;

			// If it is the only element, nothing to do.
			if (u == 0)
			{
				return;
			}

			// If it is the second element, sort with it's pair.
			if (u == 1)
			{
				// Swap if less than paired item.
				if (_distances[u] < _distances[u - 1])
				{
					Swap(u, u - 1);
				}
			}
			// If it is on the max side.
			else if (u % 2 == 1)
			{
				// Already paired. Ensure pair is ordered right.
				int p = (u / 2 - 1) | 1; // The larger value of the parent pair.

				// If less than it's pair.
				if (_distances[u] < _distances[u - 1])
				{
					// Swap with it's pair.
					u = Swap(u, u - 1);

					// If smaller than smaller parent pair.
					if (_distances[u] < _distances[p - 1])
					{
						// Swap into min-heap side.
						u = Swap(u, p - 1);
						SiftUpMin(u);
					}
				}
				else
				{
					// If larger that larger parent pair.
					if (_distances[u] > _distances[p])
					{
						// Swap into max-heap side.
						u = Swap(u, p);
						SiftUpMax(u);
					}
				}
			}
			else
			{
				// Inserted in the lower-value slot without a partner.
				int p = (u / 2 - 1) | 1; // The larger value of the parent pair.

				// If larger that larger parent pair.
				if (_distances[u] > _distances[p])
				{
					// Swap into max-heap side.
					u = Swap(u, p);
					SiftUpMax(u);
				}

				// else if smaller than smaller parent pair.
				else if (_distances[u] < _distances[p - 1])
				{
					// Swap into min-heap side.
					u = Swap(u, p - 1);
					SiftUpMin(u);
				}
			}
		}

		private void SiftUpMin(int c)
		{
			// Min-side parent: (x/2-1)&~1.
			for (int p = (c / 2 - 1) & ~1; p >= 0 && _distances[c] < _distances[p]; c = p, p = (c / 2 - 1) & ~1)
			{
				Swap(c, p);
			}
		}

		private void SiftUpMax(int c)
		{
			// Max-side parent: (x/2-1)|1.
			for (int p = (c / 2 - 1) | 1; p >= 0 && _distances[c] > _distances[p]; c = p, p = (c / 2 - 1) | 1)
			{
				Swap(c, p);
			}
		}

		private void SiftDownMin(int p)
		{
			// For each child of the parent.
			for (int c = p * 2 + 2; c < Count; p = c, c = p * 2 + 2)
			{
				// If the next child is less than the current child, select the next one.
				if (c + 2 < Count && _distances[c + 2] < _distances[c])
				{
					c += 2;
				}

				// If it is less than our parent swap.
				if (_distances[c] < _distances[p])
				{
					Swap(p, c);

					// Swap the pair if necessary.
					if (c + 1 < Count && _distances[c + 1] < _distances[c])
					{
						Swap(c, c + 1);
					}
				}
				else
				{
					break;
				}
			}
		}

		private void SiftDownMax(int p)
		{
			// For each child on the max side of the tree.
			for (int c = p * 2 + 1; c <= Count; p = c, c = p * 2 + 1)
			{
				// If the child is the last one (and only has half a pair).
				if (c == Count)
				{
					// Check if we need to swap with th parent.
					if (_distances[c - 1] > _distances[p])
					{
						Swap(p, c - 1);
					}
					break;
				}
				// If there is only room for a right child lower pair.
				else if (c + 2 == Count)
				{
					// Swap the children.
					if (_distances[c + 1] > _distances[c])
					{
						// Swap with the parent.
						if (_distances[c + 1] > _distances[p])
						{
							Swap(p, c + 1);
						}
						break;
					}
				}
				else if (c + 2 < Count)
				{
					// If there is room for a right child upper pair.
					if (_distances[c + 2] > _distances[c])
					{
						c += 2;
					}
				}

				if (_distances[c] > _distances[p])
				{
					Swap(p, c);

					// Swap with pair if necessary.
					if (_distances[c - 1] > _distances[c])
					{
						Swap(c, c - 1);
					}
				}
				else
				{
					break;
				}
			}
		}

		private int Swap(int x, int y)
		{
			// Store temp.
			var node = _positions[y];
			var dist = _distances[y];

			// Swap
			_positions[y] = _positions[x];
			_distances[y] = _distances[x];
			_positions[x] = node;
			_distances[x] = dist;

			return y;
		}

		/// <summary>
		/// Gets or sets the maximum number of elements on this 
		/// collection, if specified. A value of zero indicates
		/// this instance has no upper limit of elements.
		/// </summary>
		public int Capacity { get; private set; }

		/// <summary>
		/// Gets the minimum distance between a node
		/// in this collection and the query point.
		/// </summary>
		public double Minimum
		{
			get
			{
				if (Count == 0)
				{
					throw new InvalidOperationException();
				}
				return _distances[0];
			}
		}

		/// <summary>
		/// Gets the maximum distance between a node
		/// in this collection and the query point.
		/// </summary>
		public double Maximum
		{
			get
			{
				if (Count == 0)
				{
					throw new InvalidOperationException();
				}

				if (Count == 1)
				{
					return _distances[0];
				}

				return _distances[1];
			}
		}

		/// <summary>
		/// Gets the farthest node in the collection (with greatest distance).
		/// </summary>
		public TNode Farthest
		{
			get
			{
				if (Count == 0)
				{
					return null;
				}

				if (Count == 1)
				{
					return _positions[0];
				}

				return _positions[1];
			}
		}

		/// <summary>
		/// Gets the nearest node in the collection (with smallest distance).
		/// </summary>
		public TNode Nearest
		{
			get
			{
				if (Count == 0)
				{
					return null;
				}

				return _positions[0];
			}
		}
	}
}