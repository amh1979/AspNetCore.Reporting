using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.IO;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapShapefileInstance : MapSpatialDataInstance
	{
		private MapShapefile m_defObject;

		private string m_source;

		private Stream m_stream;

		private Stream m_dbfStream;

		public string Source
		{
			get
			{
				if (this.m_source == null)
				{
					this.m_source = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapShapefile)this.m_defObject.MapSpatialDataDef).EvaluateSource(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_source;
			}
		}

		public Stream Stream
		{
			get
			{
				if (this.m_stream == null)
				{
					this.m_stream = this.GetFileStream(this.Source);
				}
				return this.m_stream;
			}
		}

		public Stream DBFStream
		{
			get
			{
				if (this.m_dbfStream == null)
				{
					this.m_dbfStream = this.GetFileStream(this.GetDBFUrl());
				}
				return this.m_dbfStream;
			}
		}

		internal MapShapefileInstance(MapShapefile defObject)
			: base(defObject)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_source = null;
			this.m_stream = null;
			this.m_dbfStream = null;
		}

		private Stream GetFileStream(string url)
		{
			if (string.IsNullOrEmpty(url))
			{
				return null;
			}
			string fileStreamName = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapShapefile)this.m_defObject.MapSpatialDataDef).GetFileStreamName(this.m_defObject.MapDef.RenderingContext, url);
			if (fileStreamName != null)
			{
				IChunkFactory chunkFactory = this.m_defObject.MapDef.RenderingContext.OdpContext.ChunkFactory;
				string text = default(string);
				return chunkFactory.GetChunk(fileStreamName, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Shapefile, ChunkMode.Open, out text);
			}
			return null;
		}

		private string GetDBFUrl()
		{
			if (this.Source.EndsWith(".shp", StringComparison.OrdinalIgnoreCase))
			{
				return this.Source.Substring(0, this.Source.Length - 3) + "dbf";
			}
			return null;
		}
	}
}
