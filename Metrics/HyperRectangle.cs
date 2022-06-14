using System;

namespace IndoorMapping.Tools.Trees.LdKdTree.Metrics
{
	/// <summary>
	/// Hyperrectangle structure.
	/// </summary>
	/// <remarks>
	/// <para>
	/// In geometry, an n-orthotope (also called a hyperrectangle or a box) is the generalization of a rectangle for higher 
	/// dimensions, formally defined as the Cartesian product of intervals.</para>
	/// <para>
	///  References:
	///  <list type="bullet">
	///    <item><description>
	///      Wikipedia contributors, "Hyperrectangle," Wikipedia, The Free Encyclopedia, 
	///      https://en.wikipedia.org/w/index.php?title=Hyperrectangle </description></item>
	///   </list></para>     
	/// </remarks>
	public struct HyperRectangle : ICloneable, IEquatable<HyperRectangle>, IFormattable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="HyperRectangle"/> struct.
		/// </summary>
		/// <param name="x">The x-coordinate of the upper-left corner of the rectangle..</param>
		/// <param name="y">The y-coordinate of the upper-left corner of the rectangle.</param>
		/// <param name="width">The width of the rectangle.</param>
		/// <param name="height">The height of the rectangle.</param>
		public HyperRectangle(double x, double y, double width, double height)
		{
			Minimum = new double[] { x, y };
			Maximum = new double[] { x + width, y + height };
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HyperRectangle"/> struct.
		/// </summary>
		/// <param name="min">The minimum point in the hyperrectangle (the lower bound).</param>
		/// <param name="max">The maximum point in the hyperrectangle (the upper bound).</param>
		/// <param name="copy">Whether the passed vectors should be copied into this instance
		/// or used as-is. Default is true (elements from the given vectors will be copied
		/// into new array instances).</param>
		public HyperRectangle(double[] min, double[] max, bool copy = true)
		{
			if (min.Length != max.Length)
			{
				throw new ArgumentException("max and min must have the same dimension");
			}

			if (copy)
			{
				Minimum = (double[])min.Clone();
				Maximum = (double[])max.Clone();
			}
			else
			{
				Minimum = min;
				Maximum = max;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HyperRectangle"/> struct from minimum and maximum values.
		/// </summary>
		/// <param name="min">The minimum point in the hyperrectangle (the lower bound).</param>
		/// <param name="max">The maximum point in the hyperrectangle (the upper bound).</param>
		/// <param name="copy">Whether the passed vectors should be copied into this instance
		/// or used as-is. Default is true (elements from the given vectors will be copied
		/// into new array instances).</param>
		public static HyperRectangle FromMinAndMax(double[] min, double[] max, bool copy = true)
		{
			return new HyperRectangle(min, max, copy: copy);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HyperRectangle"/> struct from a minimum value and a size.
		/// </summary>
		/// <param name="min">The minimum point in the hyperrectangle (the lower bound).</param>
		/// <param name="size">The size of each dimension (i.e. width, height, and so on).</param>
		/// <param name="copy">Whether the passed vectors should be copied into this instance
		/// or used as-is. Default is true (elements from the given vectors will be copied
		/// into new array instances).</param>
		public static HyperRectangle FromMinAndLength(double[] min, double[] size, bool copy = true)
		{
			if (copy)
			{
				min = (double[])min.Clone();
				size = (double[])size.Clone();
			}

			for (int i = 0; i < size.Length; i++)
			{
				size[i] = min[i] + size[i];
			}

			return new HyperRectangle(min, size, copy: false);
		}

		/// <summary>
		/// Gets the length of each dimension. The length of the first dimension
		/// can be referred as the width, the second as the height, and so on.
		/// </summary>
		public double[] GetLength()
		{
			double[] length = new double[Minimum.Length];
			for (int i = 0; i < length.Length; i++)
			{
				length[i] = Maximum[i] - Minimum[i];
			}
			return length;
		}

		/// <summary>
		/// Determines if this rectangle intersects with rect.
		/// </summary>
		public bool IntersectsWith(HyperRectangle rect)
		{
			for (int i = 0; i < Minimum.Length; i++)
			{
				double amini = Minimum[i];
				double amaxi = Maximum[i];

				double bmini = rect.Minimum[i];
				double bmaxi = rect.Maximum[i];

				if (amini >= bmaxi || amaxi < bmini)
				{
					return false;
				}
			}

			return true;
		}

		public bool Contains(params double[] point)
		{
			for (int i = 0; i < point.Length; i++)
			{
				double mini = Minimum[i];
				double maxi = Maximum[i];

				double pointi = point[i];

				if (pointi < mini || pointi >= maxi)
				{
					return false;
				}
			}

			return true;
		}

		public bool Equals(HyperRectangle other)
		{
			if (Minimum.Length != other.Minimum.Length)
			{
				return false;
			}

			for (int i = 0; i < Minimum.Length; i++)
			{
				if (Minimum[i] != other.Minimum[i])
				{
					return false;
				}

				if (Maximum[i] != other.Maximum[i])
				{
					return false;
				}
			}

			return true;
		}

		public object Clone()
		{
			return new HyperRectangle((double[])Minimum.Clone(), (double[])Maximum.Clone());
		}

		public override string ToString()
		{
			return ToString("G", null);
		}

		public string ToString(string format, IFormatProvider formatProvider = null)
		{
			if (formatProvider == null)
			{
				formatProvider = System.Globalization.CultureInfo.CurrentCulture;
			}

			if (NumberOfDimensions == 2)
			{
				return String.Format(formatProvider, format,
					"X = {0} Y = {1} Width = {2} Height = {3}",
					Minimum[0], Minimum[1], Maximum[0] - Minimum[0], Maximum[1] - Minimum[1]);
			}

			return String.Format(formatProvider, format,
				"Min = {0} Max = {1} (Length = {2})",
				Minimum, Maximum, GetLength());
		}

		public int NumberOfDimensions
		{
			get { return Minimum.Length; }
		}

		/// <summary>
		/// Gets the minimum point defining the lower bound of the hyperrectangle.
		/// </summary>
		public double[] Minimum { get; }

		/// <summary>
		/// Gets the maximum point defining the upper bound of the hyperrectangle.
		/// </summary>
		public double[] Maximum { get; }
	}
}