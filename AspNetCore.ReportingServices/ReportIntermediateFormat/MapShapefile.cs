using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class MapShapefile : MapSpatialData, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = MapShapefile.GetDeclaration();

		private ExpressionInfo m_source;

		private List<MapFieldName> m_mapFieldNames;

		internal ExpressionInfo Source
		{
			get
			{
				return this.m_source;
			}
			set
			{
				this.m_source = value;
			}
		}

		internal List<MapFieldName> MapFieldNames
		{
			get
			{
				return this.m_mapFieldNames;
			}
			set
			{
				this.m_mapFieldNames = value;
			}
		}

		internal new MapShapefileExprHost ExprHost
		{
			get
			{
				return (MapShapefileExprHost)base.m_exprHost;
			}
		}

		internal MapShapefile()
		{
		}

		internal MapShapefile(MapVectorLayer mapVectorLayer, Map map)
			: base(mapVectorLayer, map)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapShapefileStart();
			base.Initialize(context);
			if (this.m_source != null)
			{
				this.m_source.Initialize("Source", context);
				context.ExprHostBuilder.MapShapefileSource(this.m_source);
			}
			if (this.m_mapFieldNames != null)
			{
				for (int i = 0; i < this.m_mapFieldNames.Count; i++)
				{
					this.m_mapFieldNames[i].Initialize(context, i);
				}
			}
			context.ExprHostBuilder.MapShapefileEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapShapefile mapShapefile = (MapShapefile)base.PublishClone(context);
			if (this.m_source != null)
			{
				mapShapefile.m_source = (ExpressionInfo)this.m_source.PublishClone(context);
			}
			if (this.m_mapFieldNames != null)
			{
				mapShapefile.m_mapFieldNames = new List<MapFieldName>(this.m_mapFieldNames.Count);
				{
					foreach (MapFieldName mapFieldName in this.m_mapFieldNames)
					{
						mapShapefile.m_mapFieldNames.Add((MapFieldName)mapFieldName.PublishClone(context));
					}
					return mapShapefile;
				}
			}
			return mapShapefile;
		}

		internal override void SetExprHost(MapSpatialDataExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHostInternal(exprHost, reportObjectModel);
			IList<MapFieldNameExprHost> mapFieldNamesHostsRemotable = this.ExprHost.MapFieldNamesHostsRemotable;
			if (this.m_mapFieldNames != null && mapFieldNamesHostsRemotable != null)
			{
				for (int i = 0; i < this.m_mapFieldNames.Count; i++)
				{
					MapFieldName mapFieldName = this.m_mapFieldNames[i];
					if (mapFieldName != null && mapFieldName.ExpressionHostID > -1)
					{
						mapFieldName.SetExprHost(mapFieldNamesHostsRemotable[mapFieldName.ExpressionHostID], reportObjectModel);
					}
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Source, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MapFieldNames, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapFieldName));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapShapefile, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSpatialData, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(MapShapefile.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Source:
					writer.Write(this.m_source);
					break;
				case MemberName.MapFieldNames:
					writer.Write(this.m_mapFieldNames);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(MapShapefile.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Source:
					this.m_source = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MapFieldNames:
					this.m_mapFieldNames = reader.ReadGenericListOfRIFObjects<MapFieldName>();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapShapefile;
		}

		internal string EvaluateSource(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapShapefileSourceExpression(this, base.m_map.Name);
		}

		internal string GetFileStreamName(AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext renderingContext, string url)
		{
			string text = null;
			OnDemandMetadata odpMetadata = renderingContext.OdpContext.OdpMetadata;
			ShapefileInfo shapefileInfo = default(ShapefileInfo);
			if (odpMetadata.TryGetShapefile(url, out shapefileInfo))
			{
				if (shapefileInfo.ErrorOccurred)
				{
					return null;
				}
				text = shapefileInfo.StreamName;
			}
			else
			{
				byte[] array = default(byte[]);
				if (!this.GetFileData(renderingContext, url, out array) || array == null)
				{
					shapefileInfo = new ShapefileInfo(null);
					shapefileInfo.ErrorOccurred = true;
				}
				else
				{
					text = this.StoreShapefileInChunk(renderingContext, array);
					shapefileInfo = new ShapefileInfo(text);
				}
				odpMetadata.AddShapefile(url, shapefileInfo);
			}
			return text;
		}

		private bool GetFileData(AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext renderingContext, string url, out byte[] data)
		{
			data = null;
			string text = null;
			try
			{
				bool flag = default(bool);
				if (!renderingContext.OdpContext.TopLevelContext.ReportContext.IsSupportedProtocol(url, true, out flag))
				{
					renderingContext.OdpContext.ErrorContext.Register(ProcessingErrorCode.rsUnsupportedProtocol, Severity.Error, base.m_map.ObjectType, base.m_map.Name, "Source", url, "http://, https://, ftp://, file:, mailto:, or news:");
					return false;
				}
				bool flag2 = default(bool);
				renderingContext.OdpContext.GetResource(url, out data, out text, out flag2);
				return data != null;
			}
			catch (Exception ex)
			{
				renderingContext.OdpContext.ErrorContext.Register(ProcessingErrorCode.rsMapInvalidShapefileReference, Severity.Warning, base.m_map.ObjectType, base.m_map.Name, url, ex.Message);
				return false;
			}
		}

		private string StoreShapefileInChunk(AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext renderingContext, byte[] data)
		{
			string text = Guid.NewGuid().ToString("N");
			IChunkFactory chunkFactory = renderingContext.OdpContext.ChunkFactory;
			using (Stream stream = chunkFactory.CreateChunk(text, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Shapefile, null))
			{
				stream.Write(data, 0, data.Length);
				return text;
			}
		}
	}
}
