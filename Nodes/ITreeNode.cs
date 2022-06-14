namespace IndoorMapping.Tools.Trees.LdKdTree.Nodes
{
	public interface ITreeNode<TNode> where TNode : ITreeNode<TNode>
	{
		TNode[] Children { get; set; }

		bool IsLeaf { get; }
	}
}