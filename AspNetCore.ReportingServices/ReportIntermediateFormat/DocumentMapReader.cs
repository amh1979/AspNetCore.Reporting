using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System.IO;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal sealed class DocumentMapReader
	{
		private AspNetCore.ReportingServices.OnDemandReportRendering.DocumentMapNode m_currentNode;

		private IntermediateFormatReader m_rifReader;

		private int m_level;

		private long m_startIndex;

		private Stream m_chunkStream;

		public AspNetCore.ReportingServices.OnDemandReportRendering.DocumentMapNode Current
		{
			get
			{
				if (this.m_currentNode == null)
				{
					throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
				}
				return this.m_currentNode;
			}
		}

		public DocumentMapReader(Stream chunkStream)
		{
			this.m_chunkStream = chunkStream;
			ProcessingRIFObjectCreator rifObjectCreator = new ProcessingRIFObjectCreator(null, null);
			this.m_rifReader = new IntermediateFormatReader(this.m_chunkStream, rifObjectCreator);
			this.m_startIndex = this.m_rifReader.ObjectStartPosition;
			this.m_level = 1;
		}

		public bool MoveNext()
		{
			if (this.m_rifReader.EOS)
			{
				return false;
			}
			this.m_currentNode = null;
			IPersistable persistable = this.m_rifReader.ReadRIFObject();
			switch (persistable.GetObjectType())
			{
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DocumentMapBeginContainer:
			{
				bool result = this.MoveNext();
				this.m_level++;
				return result;
			}
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DocumentMapEndContainer:
				this.m_level--;
				return this.MoveNext();
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DocumentMapNode:
			{
				DocumentMapNode documentMapNode = (DocumentMapNode)persistable;
				this.m_currentNode = new AspNetCore.ReportingServices.OnDemandReportRendering.DocumentMapNode(documentMapNode.Label, documentMapNode.Id, this.m_level);
				return true;
			}
			default:
				Global.Tracer.Assert(false);
				return false;
			}
		}

		public void Reset()
		{
			this.m_chunkStream.Seek(this.m_startIndex, SeekOrigin.Begin);
			this.m_level = 1;
		}

		public void Close()
		{
			this.m_chunkStream.Close();
		}
	}
}
