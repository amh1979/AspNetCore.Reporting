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
	internal sealed class MapTileLayer : MapLayer, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = MapTileLayer.GetDeclaration();

		private ExpressionInfo m_serviceUrl;

		private ExpressionInfo m_tileStyle;

		private ExpressionInfo m_useSecureConnection;

		private List<MapTile> m_mapTiles;

		internal ExpressionInfo ServiceUrl
		{
			get
			{
				return this.m_serviceUrl;
			}
			set
			{
				this.m_serviceUrl = value;
			}
		}

		internal ExpressionInfo TileStyle
		{
			get
			{
				return this.m_tileStyle;
			}
			set
			{
				this.m_tileStyle = value;
			}
		}

		internal ExpressionInfo UseSecureConnection
		{
			get
			{
				return this.m_useSecureConnection;
			}
			set
			{
				this.m_useSecureConnection = value;
			}
		}

		internal List<MapTile> MapTiles
		{
			get
			{
				return this.m_mapTiles;
			}
			set
			{
				this.m_mapTiles = value;
			}
		}

		internal new MapTileLayerExprHost ExprHost
		{
			get
			{
				return (MapTileLayerExprHost)base.m_exprHost;
			}
		}

		internal MapTileLayer()
		{
		}

		internal MapTileLayer(Map map)
			: base(map)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapTileLayerStart(base.Name);
			base.Initialize(context);
			if (this.m_serviceUrl != null)
			{
				this.m_serviceUrl.Initialize("ServiceUrl", context);
				context.ExprHostBuilder.MapTileLayerServiceUrl(this.m_serviceUrl);
			}
			if (this.m_tileStyle != null)
			{
				this.m_tileStyle.Initialize("TileStyle", context);
				context.ExprHostBuilder.MapTileLayerTileStyle(this.m_tileStyle);
			}
			if (this.m_useSecureConnection != null)
			{
				this.m_useSecureConnection.Initialize("UseSecureConnection", context);
				context.ExprHostBuilder.MapTileLayerUseSecureConnection(this.m_useSecureConnection);
			}
			if (this.m_mapTiles != null)
			{
				for (int i = 0; i < this.m_mapTiles.Count; i++)
				{
					this.m_mapTiles[i].Initialize(context, i);
				}
			}
			base.m_exprHostID = context.ExprHostBuilder.MapTileLayerEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapTileLayer mapTileLayer = (MapTileLayer)base.PublishClone(context);
			if (this.m_serviceUrl != null)
			{
				mapTileLayer.m_serviceUrl = (ExpressionInfo)this.m_serviceUrl.PublishClone(context);
			}
			if (this.m_tileStyle != null)
			{
				mapTileLayer.m_tileStyle = (ExpressionInfo)this.m_tileStyle.PublishClone(context);
			}
			if (this.m_mapTiles != null)
			{
				mapTileLayer.m_mapTiles = new List<MapTile>(this.m_mapTiles.Count);
				foreach (MapTile mapTile in this.m_mapTiles)
				{
					mapTileLayer.m_mapTiles.Add((MapTile)mapTile.PublishClone(context));
				}
			}
			if (this.m_useSecureConnection != null)
			{
				mapTileLayer.m_useSecureConnection = (ExpressionInfo)this.m_useSecureConnection.PublishClone(context);
			}
			return mapTileLayer;
		}

		internal void SetExprHost(MapTileLayerExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			IList<MapTileExprHost> mapTilesHostsRemotable = this.ExprHost.MapTilesHostsRemotable;
			if (this.m_mapTiles != null && mapTilesHostsRemotable != null)
			{
				for (int i = 0; i < this.m_mapTiles.Count; i++)
				{
					MapTile mapTile = this.m_mapTiles[i];
					if (mapTile != null && mapTile.ExpressionHostID > -1)
					{
						mapTile.SetExprHost(mapTilesHostsRemotable[mapTile.ExpressionHostID], reportObjectModel);
					}
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ServiceUrl, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.TileStyle, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MapTiles, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapTile));
			list.Add(new MemberInfo(MemberName.UseSecureConnection, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapTileLayer, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLayer, list);
		}

		internal Stream GetTileData(string url, out string mimeType, AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext renderingContext)
		{
			OnDemandMetadata odpMetadata = renderingContext.OdpContext.OdpMetadata;
			ImageInfo imageInfo = default(ImageInfo);
			if (odpMetadata.TryGetExternalImage(url, out imageInfo))
			{
				IChunkFactory chunkFactory = renderingContext.OdpContext.ChunkFactory;
				return chunkFactory.GetChunk(imageInfo.StreamName, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Image, ChunkMode.Open, out mimeType);
			}
			mimeType = null;
			return null;
		}

		internal void SetTileData(string url, byte[] data, string mimeType, AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext renderingContext)
		{
			string text = Guid.NewGuid().ToString("N");
			ImageInfo imageInfo = new ImageInfo(text, "");
			renderingContext.OdpContext.OdpMetadata.AddExternalImage(url, imageInfo);
			IChunkFactory chunkFactory = renderingContext.OdpContext.ChunkFactory;
			using (Stream stream = chunkFactory.CreateChunk(text, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Image, mimeType))
			{
				stream.Write(data, 0, data.Length);
			}
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(MapTileLayer.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ServiceUrl:
					writer.Write(this.m_serviceUrl);
					break;
				case MemberName.TileStyle:
					writer.Write(this.m_tileStyle);
					break;
				case MemberName.UseSecureConnection:
					writer.Write(this.m_useSecureConnection);
					break;
				case MemberName.MapTiles:
					writer.Write(this.m_mapTiles);
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
			reader.RegisterDeclaration(MapTileLayer.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.ServiceUrl:
					this.m_serviceUrl = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.TileStyle:
					this.m_tileStyle = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.UseSecureConnection:
					this.m_useSecureConnection = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MapTiles:
					this.m_mapTiles = reader.ReadGenericListOfRIFObjects<MapTile>();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapTileLayer;
		}

		internal string EvaluateServiceUrl(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapTileLayerServiceUrlExpression(this, base.m_map.Name);
		}

		internal MapTileStyle EvaluateTileStyle(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return EnumTranslator.TranslateMapTileStyle(context.ReportRuntime.EvaluateMapTileLayerTileStyleExpression(this, base.m_map.Name), context.ReportRuntime);
		}

		internal bool EvaluateUseSecureConnection(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapTileLayerUseSecureConnectionExpression(this, base.m_map.Name);
		}
	}
}
