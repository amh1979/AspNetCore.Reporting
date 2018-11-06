using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimDocumentMap : DocumentMap, IDocumentMap, IEnumerator<OnDemandDocumentMapNode>, IDisposable, IEnumerator
	{
		private AspNetCore.ReportingServices.ReportProcessing.DocumentMapNode m_oldDocMap;

		private DocumentMapNode m_current;

		private Stack<IEnumerator<AspNetCore.ReportingServices.ReportProcessing.DocumentMapNode>> m_nodeInfoStack;

		OnDemandDocumentMapNode IEnumerator<OnDemandDocumentMapNode>.Current
		{
			get
			{
				return this.Current;
			}
		}

		public override DocumentMapNode Current
		{
			get
			{
				if (this.m_current == null)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				return this.m_current;
			}
		}

		internal ShimDocumentMap(AspNetCore.ReportingServices.ReportProcessing.DocumentMapNode aOldDocMap)
		{
			this.m_oldDocMap = aOldDocMap;
			this.Reset();
		}

		public override void Close()
		{
			this.m_oldDocMap = null;
			base.m_isClosed = true;
		}

		public override void Dispose()
		{
			this.Close();
		}

		public override bool MoveNext()
		{
			this.m_current = null;
			if (this.m_nodeInfoStack == null)
			{
				this.m_nodeInfoStack = new Stack<IEnumerator<AspNetCore.ReportingServices.ReportProcessing.DocumentMapNode>>();
				this.m_current = new DocumentMapNode(this.m_oldDocMap.Label, this.m_oldDocMap.Id, this.m_nodeInfoStack.Count + 1);
				this.m_nodeInfoStack.Push(((IEnumerable<AspNetCore.ReportingServices.ReportProcessing.DocumentMapNode>)this.m_oldDocMap.Children).GetEnumerator());
				return true;
			}
			if (this.m_nodeInfoStack.Count == 0)
			{
				return false;
			}
			while (this.m_nodeInfoStack.Count > 0 && !this.m_nodeInfoStack.Peek().MoveNext())
			{
				this.m_nodeInfoStack.Pop();
			}
			if (this.m_nodeInfoStack.Count == 0)
			{
				return false;
			}
			IEnumerator<AspNetCore.ReportingServices.ReportProcessing.DocumentMapNode> enumerator = this.m_nodeInfoStack.Peek();
			AspNetCore.ReportingServices.ReportProcessing.DocumentMapNode current = enumerator.Current;
			this.m_current = new DocumentMapNode(current.Label, current.Id, this.m_nodeInfoStack.Count + 1);
			if (current.Children != null && current.Children.Length != 0)
			{
				this.m_nodeInfoStack.Push(((IEnumerable<AspNetCore.ReportingServices.ReportProcessing.DocumentMapNode>)current.Children).GetEnumerator());
			}
			return true;
		}

		public override void Reset()
		{
			this.m_current = null;
			this.m_nodeInfoStack = null;
		}
	}
}
