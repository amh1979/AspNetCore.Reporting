using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class MapBindingFieldPair : IPersistable
	{
		private int m_exprHostID = -1;

		private int m_exprHostMapMemberID = -1;

		[NonSerialized]
		private MapBindingFieldPairExprHost m_exprHost;

		[NonSerialized]
		private MapBindingFieldPairExprHost m_exprHostMapMember;

		[Reference]
		private Map m_map;

		[Reference]
		private MapVectorLayer m_mapVectorLayer;

		[NonSerialized]
		private static readonly Declaration m_Declaration = MapBindingFieldPair.GetDeclaration();

		private ExpressionInfo m_fieldName;

		private ExpressionInfo m_bindingExpression;

		internal ExpressionInfo FieldName
		{
			get
			{
				return this.m_fieldName;
			}
			set
			{
				this.m_fieldName = value;
			}
		}

		internal ExpressionInfo BindingExpression
		{
			get
			{
				return this.m_bindingExpression;
			}
			set
			{
				this.m_bindingExpression = value;
			}
		}

		internal bool InElementView
		{
			get
			{
				return this.m_mapVectorLayer == null;
			}
		}

		internal string OwnerName
		{
			get
			{
				return this.m_map.Name;
			}
		}

		internal MapBindingFieldPairExprHost ExprHost
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

		internal MapBindingFieldPairExprHost ExprHostMapMember
		{
			get
			{
				return this.m_exprHostMapMember;
			}
		}

		internal int ExpressionHostMapMemberID
		{
			get
			{
				return this.m_exprHostMapMemberID;
			}
		}

		internal IInstancePath InstancePath
		{
			get
			{
				if (this.m_mapVectorLayer != null)
				{
					return this.m_mapVectorLayer.InstancePath;
				}
				return this.m_map;
			}
		}

		internal MapBindingFieldPair()
		{
		}

		internal MapBindingFieldPair(Map map, MapVectorLayer mapVectorLayer)
		{
			this.m_map = map;
			this.m_mapVectorLayer = mapVectorLayer;
		}

		internal void Initialize(InitializationContext context, int index)
		{
			context.ExprHostBuilder.MapBindingFieldPairStart(index.ToString(CultureInfo.InvariantCulture.NumberFormat));
			if (this.m_fieldName != null)
			{
				this.m_fieldName.Initialize("FieldName", context);
				context.ExprHostBuilder.MapBindingFieldPairFieldName(this.m_fieldName);
			}
			if (this.InElementView && this.m_bindingExpression != null)
			{
				this.m_bindingExpression.Initialize("BindingExpression", context);
				context.ExprHostBuilder.MapBindingFieldPairBindingExpression(this.m_bindingExpression);
			}
			this.m_exprHostID = context.ExprHostBuilder.MapBindingFieldPairEnd();
		}

		internal void InitializeMapMember(InitializationContext context, int index)
		{
			context.ExprHostBuilder.MapBindingFieldPairStart(index.ToString(CultureInfo.InvariantCulture.NumberFormat));
			if (!this.InElementView && this.m_bindingExpression != null)
			{
				this.m_bindingExpression.Initialize("BindingExpression", context);
				context.ExprHostBuilder.MapBindingFieldPairBindingExpression(this.m_bindingExpression);
			}
			this.m_exprHostMapMemberID = context.ExprHostBuilder.MapBindingFieldPairEnd();
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			MapBindingFieldPair mapBindingFieldPair = (MapBindingFieldPair)base.MemberwiseClone();
			mapBindingFieldPair.m_map = context.CurrentMapClone;
			if (this.m_mapVectorLayer != null)
			{
				mapBindingFieldPair.m_mapVectorLayer = context.CurrentMapVectorLayerClone;
			}
			if (this.m_fieldName != null)
			{
				mapBindingFieldPair.m_fieldName = (ExpressionInfo)this.m_fieldName.PublishClone(context);
			}
			if (this.m_bindingExpression != null)
			{
				mapBindingFieldPair.m_bindingExpression = (ExpressionInfo)this.m_bindingExpression.PublishClone(context);
			}
			return mapBindingFieldPair;
		}

		internal void SetExprHost(MapBindingFieldPairExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			this.m_exprHost = exprHost;
			this.m_exprHost.SetReportObjectModel(reportObjectModel);
		}

		internal void SetExprHostMapMember(MapBindingFieldPairExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			this.m_exprHostMapMember = exprHost;
			this.m_exprHostMapMember.SetReportObjectModel(reportObjectModel);
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.FieldName, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.BindingExpression, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Map, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Map, Token.Reference));
			list.Add(new MemberInfo(MemberName.MapVectorLayer, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapVectorLayer, Token.Reference));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.ExprHostMapMemberID, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapBindingFieldPair, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(MapBindingFieldPair.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Map:
					writer.WriteReference(this.m_map);
					break;
				case MemberName.MapVectorLayer:
					writer.WriteReference(this.m_mapVectorLayer);
					break;
				case MemberName.FieldName:
					writer.Write(this.m_fieldName);
					break;
				case MemberName.BindingExpression:
					writer.Write(this.m_bindingExpression);
					break;
				case MemberName.ExprHostID:
					writer.Write(this.m_exprHostID);
					break;
				case MemberName.ExprHostMapMemberID:
					writer.Write(this.m_exprHostMapMemberID);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(MapBindingFieldPair.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Map:
					this.m_map = reader.ReadReference<Map>(this);
					break;
				case MemberName.MapVectorLayer:
					this.m_mapVectorLayer = reader.ReadReference<MapVectorLayer>(this);
					break;
				case MemberName.FieldName:
					this.m_fieldName = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.BindingExpression:
					this.m_bindingExpression = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ExprHostID:
					this.m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.ExprHostMapMemberID:
					this.m_exprHostMapMemberID = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			List<MemberReference> list = default(List<MemberReference>);
			if (memberReferencesCollection.TryGetValue(MapBindingFieldPair.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item in list)
				{
					switch (item.MemberName)
					{
					case MemberName.Map:
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						this.m_map = (Map)referenceableItems[item.RefID];
						break;
					case MemberName.MapVectorLayer:
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						this.m_mapVectorLayer = (MapVectorLayer)referenceableItems[item.RefID];
						break;
					default:
						Global.Tracer.Assert(false);
						break;
					}
				}
			}
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapBindingFieldPair;
		}

		internal string EvaluateFieldName(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapBindingFieldPairFieldNameExpression(this, this.m_map.Name);
		}

		internal AspNetCore.ReportingServices.RdlExpressions.VariantResult EvaluateBindingExpression(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapBindingFieldPairBindingExpressionExpression(this, this.m_map.Name);
		}
	}
}
