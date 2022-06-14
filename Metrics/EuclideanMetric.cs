using System;
using System.Runtime.CompilerServices;

namespace IndoorMapping.Tools.Trees.LdKdTree.Metrics
{
	public class EuclideanMetric : IMetric<double[]>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double Distance(double[] x, double[] y)
		{
			double dist = 0.0;
			for (int i = 0; i < x.Length; i++)
			{
				dist += (x[i] - y[i]) * (x[i] - y[i]);
			}
			return Math.Sqrt(dist);
		}
	}
}