
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AspNetCore.Reporting
{
	internal sealed class DocumentMapNode
	{
		private struct NodeStackEntry
		{
			public DocumentMapNode Node;

			public int Level;
		}

		private string m_label;

		private string m_id;

		private IList<DocumentMapNode> m_children;

		public string Label
		{
			get
			{
				return this.m_label;
			}
		}

		public string Id
		{
			get
			{
				return this.m_id;
			}
		}

		public IList<DocumentMapNode> Children
		{
			get
			{
				return this.m_children;
			}
		}

		internal DocumentMapNode(string label, string id, DocumentMapNode[] children)
		{
			this.m_label = label;
			this.m_id = id;
			if (children != null)
			{
				this.m_children = new ReadOnlyCollection<DocumentMapNode>(children);
			}
			else
			{
				this.m_children = new ReadOnlyCollection<DocumentMapNode>(new DocumentMapNode[0]);
			}
		}

		internal static DocumentMapNode CreateTree(IDocumentMap docMap, string rootName)
		{
			DocumentMapNode documentMapNode = DocumentMapNode.CreateNode(docMap);
			if (documentMapNode != null)
			{
				documentMapNode.m_label = rootName;
			}
			return documentMapNode;
		}


		internal static DocumentMapNode CreateNode(IDocumentMap docMap)
		{
			if (docMap == null)
			{
				return null;
			}
			Stack<NodeStackEntry> stack = new Stack<NodeStackEntry>();
			List<DocumentMapNode> workspace = new List<DocumentMapNode>();
			OnDemandDocumentMapNode onDemandDocumentMapNode = null;
			while (docMap.MoveNext())
			{
				onDemandDocumentMapNode = docMap.Current;
				NodeStackEntry item = default(NodeStackEntry);
				item.Node = DocumentMapNode.FromOnDemandNode(onDemandDocumentMapNode);
				item.Level = onDemandDocumentMapNode.Level;
				while (stack.Count > 0 && onDemandDocumentMapNode.Level < stack.Peek().Level)
				{
					DocumentMapNode.CollapseTopLevel(stack, workspace);
				}
				stack.Push(item);
			}
			while (stack.Count > 1)
			{
				DocumentMapNode.CollapseTopLevel(stack, workspace);
			}
			return stack.Pop().Node;
		}

		private static void CollapseTopLevel(Stack<NodeStackEntry> nodeStack, List<DocumentMapNode> workspace)
		{
			if (nodeStack != null && nodeStack.Count > 1)
			{
				int level = nodeStack.Peek().Level;
				workspace.Clear();
				while (nodeStack.Peek().Level == level)
				{
					workspace.Add(nodeStack.Pop().Node);
				}
				DocumentMapNode node = nodeStack.Peek().Node;
				node.SetNodeChildren(new DocumentMapNode[workspace.Count]);
				for (int num = workspace.Count - 1; num >= 0; num--)
				{
					node.Children[workspace.Count - num - 1] = workspace[num];
				}
			}
		}

		private static DocumentMapNode FromOnDemandNode(OnDemandDocumentMapNode node)
		{
			return new DocumentMapNode(node.Label, node.Id, null);
		}

	 
		private void SetNodeChildren(IList<DocumentMapNode> children)
		{
			this.m_children = children;
		}
	}
}
