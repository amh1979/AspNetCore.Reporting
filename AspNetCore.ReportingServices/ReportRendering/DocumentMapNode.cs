using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class DocumentMapNode
	{
		private AspNetCore.ReportingServices.ReportProcessing.DocumentMapNode m_underlyingNode;

		private DocumentMapNode[] m_childrenWrappers;

		private object m_nonPersistedRenderingInfo;

		public string Label
		{
			get
			{
				return this.m_underlyingNode.Label;
			}
		}

		public string UniqueName
		{
			get
			{
				return this.m_underlyingNode.Id;
			}
		}

		public int Page
		{
			get
			{
				return this.m_underlyingNode.Page;
			}
		}

		public object NonPersistedRenderingInfo
		{
			get
			{
				return this.m_nonPersistedRenderingInfo;
			}
			set
			{
				this.m_nonPersistedRenderingInfo = value;
			}
		}

		public DocumentMapNode[] Children
		{
			get
			{
				if (this.m_childrenWrappers == null)
				{
					AspNetCore.ReportingServices.ReportProcessing.DocumentMapNode[] children = this.m_underlyingNode.Children;
					if (children != null)
					{
						this.m_childrenWrappers = new DocumentMapNode[children.Length];
						for (int i = 0; i < children.Length; i++)
						{
							this.m_childrenWrappers[i] = new DocumentMapNode(children[i]);
						}
					}
				}
				return this.m_childrenWrappers;
			}
		}

		internal DocumentMapNode(AspNetCore.ReportingServices.ReportProcessing.DocumentMapNode underlyingNode)
		{
			Global.Tracer.Assert(underlyingNode != null, "The document map node being wrapped cannot be null.");
			this.m_underlyingNode = underlyingNode;
		}
	}
}
