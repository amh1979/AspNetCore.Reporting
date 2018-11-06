using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class MapElementView : MapView, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = MapElementView.GetDeclaration();

		private ExpressionInfo m_layerName;

		private List<MapBindingFieldPair> m_mapBindingFieldPairs;

		internal ExpressionInfo LayerName
		{
			get
			{
				return this.m_layerName;
			}
			set
			{
				this.m_layerName = value;
			}
		}

		internal List<MapBindingFieldPair> MapBindingFieldPairs
		{
			get
			{
				return this.m_mapBindingFieldPairs;
			}
			set
			{
				this.m_mapBindingFieldPairs = value;
			}
		}

		internal new MapElementViewExprHost ExprHost
		{
			get
			{
				return (MapElementViewExprHost)base.m_exprHost;
			}
		}

		internal MapElementView()
		{
		}

		internal MapElementView(Map map)
			: base(map)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapElementViewStart();
			base.Initialize(context);
			if (this.m_layerName != null)
			{
				this.m_layerName.Initialize("LayerName", context);
				context.ExprHostBuilder.MapElementViewLayerName(this.m_layerName);
			}
			if (this.m_mapBindingFieldPairs != null)
			{
				for (int i = 0; i < this.m_mapBindingFieldPairs.Count; i++)
				{
					this.m_mapBindingFieldPairs[i].Initialize(context, i);
				}
			}
			context.ExprHostBuilder.MapElementViewEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapElementView mapElementView = (MapElementView)base.PublishClone(context);
			if (this.m_layerName != null)
			{
				mapElementView.m_layerName = (ExpressionInfo)this.m_layerName.PublishClone(context);
			}
			if (this.m_mapBindingFieldPairs != null)
			{
				mapElementView.m_mapBindingFieldPairs = new List<MapBindingFieldPair>(this.m_mapBindingFieldPairs.Count);
				{
					foreach (MapBindingFieldPair mapBindingFieldPair in this.m_mapBindingFieldPairs)
					{
						mapElementView.m_mapBindingFieldPairs.Add((MapBindingFieldPair)mapBindingFieldPair.PublishClone(context));
					}
					return mapElementView;
				}
			}
			return mapElementView;
		}

		internal override void SetExprHost(MapViewExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			IList<MapBindingFieldPairExprHost> mapBindingFieldPairsHostsRemotable = this.ExprHost.MapBindingFieldPairsHostsRemotable;
			if (this.m_mapBindingFieldPairs != null && mapBindingFieldPairsHostsRemotable != null)
			{
				for (int i = 0; i < this.m_mapBindingFieldPairs.Count; i++)
				{
					MapBindingFieldPair mapBindingFieldPair = this.m_mapBindingFieldPairs[i];
					if (mapBindingFieldPair != null && mapBindingFieldPair.ExpressionHostID > -1)
					{
						mapBindingFieldPair.SetExprHost(mapBindingFieldPairsHostsRemotable[mapBindingFieldPair.ExpressionHostID], reportObjectModel);
					}
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.LayerName, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MapBindingFieldPairs, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapBindingFieldPair));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapElementView, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapView, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(MapElementView.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.LayerName:
					writer.Write(this.m_layerName);
					break;
				case MemberName.MapBindingFieldPairs:
					writer.Write(this.m_mapBindingFieldPairs);
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
			reader.RegisterDeclaration(MapElementView.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.LayerName:
					this.m_layerName = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MapBindingFieldPairs:
					this.m_mapBindingFieldPairs = reader.ReadGenericListOfRIFObjects<MapBindingFieldPair>();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapElementView;
		}

		internal string EvaluateLayerName(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapElementViewLayerNameExpression(this, base.m_map.Name);
		}
	}
}
