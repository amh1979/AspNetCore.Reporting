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
	internal class MapLayer : IPersistable
	{
		protected int m_exprHostID = -1;

		[NonSerialized]
		protected MapLayerExprHost m_exprHost;

		[Reference]
		protected Map m_map;

		[NonSerialized]
		private static readonly Declaration m_Declaration = MapLayer.GetDeclaration();

		protected string m_name;

		private ExpressionInfo m_visibilityMode;

		private ExpressionInfo m_minimumZoom;

		private ExpressionInfo m_maximumZoom;

		private ExpressionInfo m_transparency;

		internal string Name
		{
			get
			{
				return this.m_name;
			}
			set
			{
				this.m_name = value;
			}
		}

		internal ExpressionInfo VisibilityMode
		{
			get
			{
				return this.m_visibilityMode;
			}
			set
			{
				this.m_visibilityMode = value;
			}
		}

		internal ExpressionInfo MinimumZoom
		{
			get
			{
				return this.m_minimumZoom;
			}
			set
			{
				this.m_minimumZoom = value;
			}
		}

		internal ExpressionInfo MaximumZoom
		{
			get
			{
				return this.m_maximumZoom;
			}
			set
			{
				this.m_maximumZoom = value;
			}
		}

		internal ExpressionInfo Transparency
		{
			get
			{
				return this.m_transparency;
			}
			set
			{
				this.m_transparency = value;
			}
		}

		internal string OwnerName
		{
			get
			{
				return this.m_map.Name;
			}
		}

		internal MapLayerExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal int ExpressionHostID
		{
			get
			{
				return this.m_exprHostID;
			}
		}

		internal MapLayer()
		{
		}

		internal MapLayer(Map map)
		{
			this.m_map = map;
		}

		internal virtual void Initialize(InitializationContext context)
		{
			if (this.m_visibilityMode != null)
			{
				this.m_visibilityMode.Initialize("VisibilityMode", context);
				context.ExprHostBuilder.MapLayerVisibilityMode(this.m_visibilityMode);
			}
			if (this.m_minimumZoom != null)
			{
				this.m_minimumZoom.Initialize("MinimumZoom", context);
				context.ExprHostBuilder.MapLayerMinimumZoom(this.m_minimumZoom);
			}
			if (this.m_maximumZoom != null)
			{
				this.m_maximumZoom.Initialize("MaximumZoom", context);
				context.ExprHostBuilder.MapLayerMaximumZoom(this.m_maximumZoom);
			}
			if (this.m_transparency != null)
			{
				this.m_transparency.Initialize("Transparency", context);
				context.ExprHostBuilder.MapLayerTransparency(this.m_transparency);
			}
		}

		internal virtual object PublishClone(AutomaticSubtotalContext context)
		{
			MapLayer mapLayer = (MapLayer)base.MemberwiseClone();
			mapLayer.m_map = context.CurrentMapClone;
			if (this.m_visibilityMode != null)
			{
				mapLayer.m_visibilityMode = (ExpressionInfo)this.m_visibilityMode.PublishClone(context);
			}
			if (this.m_minimumZoom != null)
			{
				mapLayer.m_minimumZoom = (ExpressionInfo)this.m_minimumZoom.PublishClone(context);
			}
			if (this.m_maximumZoom != null)
			{
				mapLayer.m_maximumZoom = (ExpressionInfo)this.m_maximumZoom.PublishClone(context);
			}
			if (this.m_transparency != null)
			{
				mapLayer.m_transparency = (ExpressionInfo)this.m_transparency.PublishClone(context);
			}
			return mapLayer;
		}

		internal virtual void SetExprHost(MapLayerExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			this.m_exprHost = exprHost;
			this.m_exprHost.SetReportObjectModel(reportObjectModel);
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.VisibilityMode, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MinimumZoom, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MaximumZoom, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Transparency, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Map, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Map, Token.Reference));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLayer, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(MapLayer.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Map:
					writer.WriteReference(this.m_map);
					break;
				case MemberName.Name:
					writer.Write(this.m_name);
					break;
				case MemberName.VisibilityMode:
					writer.Write(this.m_visibilityMode);
					break;
				case MemberName.MinimumZoom:
					writer.Write(this.m_minimumZoom);
					break;
				case MemberName.MaximumZoom:
					writer.Write(this.m_maximumZoom);
					break;
				case MemberName.Transparency:
					writer.Write(this.m_transparency);
					break;
				case MemberName.ExprHostID:
					writer.Write(this.m_exprHostID);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(MapLayer.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Map:
					this.m_map = reader.ReadReference<Map>(this);
					break;
				case MemberName.Name:
					this.m_name = reader.ReadString();
					break;
				case MemberName.VisibilityMode:
					this.m_visibilityMode = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MinimumZoom:
					this.m_minimumZoom = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MaximumZoom:
					this.m_maximumZoom = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Transparency:
					this.m_transparency = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ExprHostID:
					this.m_exprHostID = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public virtual void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			List<MemberReference> list = default(List<MemberReference>);
			if (memberReferencesCollection.TryGetValue(MapLayer.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item in list)
				{
					MemberName memberName = item.MemberName;
					if (memberName == MemberName.Map)
					{
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						this.m_map = (Map)referenceableItems[item.RefID];
					}
					else
					{
						Global.Tracer.Assert(false);
					}
				}
			}
		}

		public virtual AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLayer;
		}

		internal MapVisibilityMode EvaluateVisibilityMode(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_map, reportScopeInstance);
			return EnumTranslator.TranslateMapVisibilityMode(context.ReportRuntime.EvaluateMapLayerVisibilityModeExpression(this, this.m_map.Name), context.ReportRuntime);
		}

		internal double EvaluateMinimumZoom(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapLayerMinimumZoomExpression(this, this.m_map.Name);
		}

		internal double EvaluateMaximumZoom(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapLayerMaximumZoomExpression(this, this.m_map.Name);
		}

		internal double EvaluateTransparency(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapLayerTransparencyExpression(this, this.m_map.Name);
		}
	}
}
