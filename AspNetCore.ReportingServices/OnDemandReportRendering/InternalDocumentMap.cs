using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalDocumentMap : DocumentMap, IDocumentMap, IEnumerator<OnDemandDocumentMapNode>, IDisposable, IEnumerator
	{
		private DocumentMapReader m_reader;

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
				return this.m_reader.Current;
			}
		}

		object IEnumerator.Current
		{
			get
			{
				return this.Current;
			}
		}

		internal InternalDocumentMap(DocumentMapReader aReader)
		{
			this.m_reader = aReader;
		}

		public override void Close()
		{
			if (this.m_reader != null)
			{
				this.m_reader.Close();
			}
			this.m_reader = null;
			base.m_isClosed = true;
		}

		public override void Dispose()
		{
			this.Close();
		}

		public override bool MoveNext()
		{
			return this.m_reader.MoveNext();
		}

		public override void Reset()
		{
			this.m_reader.Reset();
		}
	}
}
