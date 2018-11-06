using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal sealed class DocumentMapWriter
	{
		private DocumentMapNode m_node;

		private int m_level;

		private IntermediateFormatWriter m_writer;

		private Stream m_chunkStream;

		private bool m_isClosed;

		private static List<Declaration> m_docMapDeclarations = DocumentMapWriter.GetDocumentMapDeclarations();

		public DocumentMapWriter(Stream aChunkStream, OnDemandProcessingContext odpContext)
		{
			this.m_chunkStream = aChunkStream;
			this.m_writer = new IntermediateFormatWriter(this.m_chunkStream, DocumentMapWriter.m_docMapDeclarations, odpContext.GetActiveCompatibilityVersion(), odpContext.ProhibitSerializableValues);
		}

		public void WriteBeginContainer(string aLabel, string aId)
		{
			Global.Tracer.Assert(!this.m_isClosed, "(!m_isClosed)");
			this.m_level++;
			this.m_writer.Write(DocumentMapBeginContainer.Instance);
			this.WriteNode(aLabel, aId);
		}

		public void WriteNode(string aLabel, string aId)
		{
			Global.Tracer.Assert(!this.m_isClosed, "(!m_isClosed)");
			if (this.m_node == null)
			{
				this.m_node = new DocumentMapNode();
			}
			this.m_node.Label = aLabel;
			this.m_node.Id = aId;
			this.m_writer.Write(this.m_node);
		}

		public void WriteEndContainer()
		{
			Global.Tracer.Assert(!this.m_isClosed, "(!m_isClosed)");
			this.m_level--;
			Global.Tracer.Assert(this.m_level >= 0, "Mismatched EndContainer");
			this.m_writer.Write(DocumentMapEndContainer.Instance);
			if (this.m_level == 0)
			{
				this.Close();
			}
		}

		public void Close()
		{
			Global.Tracer.Assert(this.m_level == 0, "Mismatched Container Structure.  There are still open containers");
			this.m_isClosed = true;
		}

		public bool IsClosed()
		{
			return this.m_isClosed;
		}

		private static List<Declaration> GetDocumentMapDeclarations()
		{
			List<Declaration> list = new List<Declaration>(3);
			list.Add(DocumentMapNode.GetDeclaration());
			list.Add(DocumentMapBeginContainer.GetDeclaration());
			list.Add(DocumentMapEndContainer.GetDeclaration());
			return list;
		}
	}
}
