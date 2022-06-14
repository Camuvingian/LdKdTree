using System;
using System.Collections.Generic;

namespace IndoorMapping.Tools.Trees.LdKdTree.Helpers
{
	public class ElementComparer : ElementComparer<double> { }

	/// <summary>
	/// This class compares arrays by checking the value of a particular element at a given array index.
	/// </summary>
	public class ElementComparer<T> : IComparer<T[]>, IEqualityComparer<T[]> where T : IComparable, IEquatable<T>
	{
		public int Index { get; set; }

		public int Compare(T[] x, T[] y)
		{
			return x[Index].CompareTo(y[Index]);
		}

		public bool Equals(T[] x, T[] y)
		{
			return x[Index].Equals(y[Index]);
		}

		public int GetHashCode(T[] obj)
		{
			return obj[Index].GetHashCode();
		}
	}
}