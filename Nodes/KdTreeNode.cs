namespace IndoorMapping.Tools.Trees.LdKdTree.Nodes
{
	public class KdTreeNode : KdTreeNodeBase<KdTreeNode> { }

	public class KdTreeNode<T> : KdTreeNodeBase<KdTreeNode<T>>
	{
		public T Value { get; set; }
	}
}