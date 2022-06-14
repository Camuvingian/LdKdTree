namespace IndoorMapping.Tools.Trees.LdKdTree.Metrics
{
	public interface IDistance<in T> : IDistance<T, T> { }

	public interface IDistance<in T, in U>
	{
		double Distance(T a, U b);
	}
}