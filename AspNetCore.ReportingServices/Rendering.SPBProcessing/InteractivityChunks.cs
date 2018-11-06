using AspNetCore.ReportingServices.OnDemandReportRendering;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
{
	internal class InteractivityChunks
	{
		internal enum Token : byte
		{
			BookmarkInformation,
			LabelInformation,
			PageInformation,
			PageInformationOffset,
			EndObject
		}

		internal const string LabelsChunk = "Labels";

		internal const string BookmarksChunk = "Bookmarks";

		protected BinaryWriter m_writer;

		protected int m_page;

		private Stream m_stream;

		internal int Page
		{
			get
			{
				return this.m_page;
			}
			set
			{
				this.m_page = value;
				if (this.m_writer == null)
				{
					this.m_writer = new BinaryWriter(this.m_stream);
				}
			}
		}

		internal static int FindBoomark(AspNetCore.ReportingServices.OnDemandReportRendering.Report report, string bookmarkId, ref string uniqueName, ref int lastPageCollected, ref bool reportDone)
		{
			int result = 0;
			string text = null;
			Stream chunk = report.GetChunk(AspNetCore.ReportingServices.OnDemandReportRendering.Report.ChunkTypes.Interactivity, "Bookmarks");
			if (chunk != null && chunk.Length > 0)
			{
				chunk.Position = 0L;
				BinaryReader binaryReader = new BinaryReader(chunk);
				byte b = binaryReader.ReadByte();
				while (true)
				{
					switch (b)
					{
					case 0:
						break;
					case 2:
						lastPageCollected = binaryReader.ReadInt32();
						reportDone = binaryReader.ReadBoolean();
						goto IL_0090;
					default:
						goto IL_0090;
					}
					text = binaryReader.ReadString();
					if (SPBProcessing.CompareWithOrdinalComparison(bookmarkId, text, false) == 0)
					{
						uniqueName = binaryReader.ReadString();
						return binaryReader.ReadInt32();
					}
					binaryReader.ReadString();
					binaryReader.ReadInt32();
					binaryReader.ReadByte();
					b = binaryReader.ReadByte();
				}
			}
			goto IL_0090;
			IL_0090:
			return result;
		}

		internal static int FindDocumentMapLabel(AspNetCore.ReportingServices.OnDemandReportRendering.Report report, string documentMapId, ref int lastPageCollected, ref bool reportDone)
		{
			int result = 0;
			string text = null;
			Stream chunk = report.GetChunk(AspNetCore.ReportingServices.OnDemandReportRendering.Report.ChunkTypes.Interactivity, "Labels");
			if (chunk != null && chunk.Length > 0)
			{
				chunk.Position = 0L;
				BinaryReader binaryReader = new BinaryReader(chunk);
				byte b = binaryReader.ReadByte();
				while (true)
				{
					switch (b)
					{
					case 1:
						break;
					case 2:
						lastPageCollected = binaryReader.ReadInt32();
						reportDone = binaryReader.ReadBoolean();
						goto IL_0081;
					default:
						goto IL_0081;
					}
					text = binaryReader.ReadString();
					if (SPBProcessing.CompareWithOrdinalComparison(documentMapId, text, true) == 0)
					{
						return binaryReader.ReadInt32();
					}
					binaryReader.ReadInt32();
					binaryReader.ReadByte();
					b = binaryReader.ReadByte();
				}
			}
			goto IL_0081;
			IL_0081:
			return result;
		}

		private static Stream GetInteractivityChunck(AspNetCore.ReportingServices.OnDemandReportRendering.Report report, string chunkName, int page, out int lastPage)
		{
			lastPage = 0;
			Stream stream = null;
			bool flag = false;
			stream = ((page != 1) ? report.GetChunk(AspNetCore.ReportingServices.OnDemandReportRendering.Report.ChunkTypes.Interactivity, chunkName) : report.GetOrCreateChunk(AspNetCore.ReportingServices.OnDemandReportRendering.Report.ChunkTypes.Interactivity, chunkName, out flag));
			if (stream == null)
			{
				return null;
			}
			if (!flag)
			{
				long num = stream.Length - 9;
				if (num > 0)
				{
					stream.Seek(num, SeekOrigin.Begin);
					BinaryReader binaryReader = new BinaryReader(stream);
					binaryReader.ReadByte();
					num = 9 + binaryReader.ReadInt64();
					stream.Seek(-num, SeekOrigin.End);
					binaryReader.ReadByte();
					lastPage = binaryReader.ReadInt32();
					if (binaryReader.ReadBoolean())
					{
						return null;
					}
					stream.Seek(-num, SeekOrigin.End);
				}
				else
				{
					stream.Seek(0L, SeekOrigin.Begin);
				}
			}
			return stream;
		}

		internal static Bookmarks GetBookmarksStream(AspNetCore.ReportingServices.OnDemandReportRendering.Report report, int page)
		{
			int page2 = 0;
			Stream interactivityChunck = InteractivityChunks.GetInteractivityChunck(report, "Bookmarks", page, out page2);
			if (interactivityChunck != null)
			{
				return new Bookmarks(interactivityChunck, page2);
			}
			return null;
		}

		internal static DocumentMapLabels GetLabelsStream(AspNetCore.ReportingServices.OnDemandReportRendering.Report report, int page)
		{
			int page2 = 0;
			Stream interactivityChunck = InteractivityChunks.GetInteractivityChunck(report, "Labels", page, out page2);
			if (interactivityChunck != null)
			{
				return new DocumentMapLabels(interactivityChunck, page2);
			}
			return null;
		}

		internal InteractivityChunks(Stream stream, int page)
		{
			this.m_stream = stream;
			this.m_page = page;
		}

		internal void Flush(bool reportDone)
		{
			if (this.m_writer != null)
			{
				long position = this.m_stream.Position;
				this.m_writer.Write((byte)2);
				this.m_writer.Write(this.m_page);
				this.m_writer.Write(reportDone);
				this.m_writer.Write((byte)4);
				position = this.m_stream.Position - position;
				this.m_writer.Write((byte)3);
				this.m_writer.Write(position);
				this.m_writer = null;
			}
		}
	}
}
