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

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal class MapSpatialElementTemplate : MapStyleContainer, IPersistable, IActionOwner
	{
		[NonSerialized]
		protected MapSpatialElementTemplateExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = MapSpatialElementTemplate.GetDeclaration();

		private Action m_action;

		private int m_id;

		[NonSerialized]
		private List<string> m_fieldsUsedInValueExpression;

		[Reference]
		protected MapVectorLayer m_mapVectorLayer;

		private ExpressionInfo m_hidden;

		private ExpressionInfo m_offsetX;

		private ExpressionInfo m_offsetY;

		private ExpressionInfo m_label;

		private ExpressionInfo m_toolTip;

		private ExpressionInfo m_dataElementLabel;

		private string m_dataElementName;

		private DataElementOutputTypes m_dataElementOutput;

		internal Action Action
		{
			get
			{
				return this.m_action;
			}
			set
			{
				this.m_action = value;
			}
		}

		Action IActionOwner.Action
		{
			get
			{
				return this.m_action;
			}
		}

		List<string> IActionOwner.FieldsUsedInValueExpression
		{
			get
			{
				return this.m_fieldsUsedInValueExpression;
			}
			set
			{
				this.m_fieldsUsedInValueExpression = value;
			}
		}

		internal int ID
		{
			get
			{
				return this.m_id;
			}
		}

		internal ExpressionInfo Hidden
		{
			get
			{
				return this.m_hidden;
			}
			set
			{
				this.m_hidden = value;
			}
		}

		internal ExpressionInfo OffsetX
		{
			get
			{
				return this.m_offsetX;
			}
			set
			{
				this.m_offsetX = value;
			}
		}

		internal ExpressionInfo OffsetY
		{
			get
			{
				return this.m_offsetY;
			}
			set
			{
				this.m_offsetY = value;
			}
		}

		internal ExpressionInfo Label
		{
			get
			{
				return this.m_label;
			}
			set
			{
				this.m_label = value;
			}
		}

		internal ExpressionInfo ToolTip
		{
			get
			{
				return this.m_toolTip;
			}
			set
			{
				this.m_toolTip = value;
			}
		}

		internal ExpressionInfo DataElementLabel
		{
			get
			{
				return this.m_dataElementLabel;
			}
			set
			{
				this.m_dataElementLabel = value;
			}
		}

		internal string DataElementName
		{
			get
			{
				return this.m_dataElementName;
			}
			set
			{
				this.m_dataElementName = value;
			}
		}

		internal DataElementOutputTypes DataElementOutput
		{
			get
			{
				return this.m_dataElementOutput;
			}
			set
			{
				this.m_dataElementOutput = value;
			}
		}

		internal string OwnerName
		{
			get
			{
				return base.m_map.Name;
			}
		}

		internal MapSpatialElementTemplateExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		protected IInstancePath InstancePath
		{
			get
			{
				return this.m_mapVectorLayer.InstancePath;
			}
		}

		internal MapSpatialElementTemplate()
		{
		}

		internal MapSpatialElementTemplate(MapVectorLayer mapVectorLayer, Map map, int id)
			: base(map)
		{
			this.m_id = id;
			this.m_mapVectorLayer = mapVectorLayer;
		}

		internal override void Initialize(InitializationContext context)
		{
			base.Initialize(context);
			if (this.m_action != null)
			{
				this.m_action.Initialize(context);
			}
			if (this.m_hidden != null)
			{
				this.m_hidden.Initialize("Hidden", context);
				context.ExprHostBuilder.MapSpatialElementTemplateHidden(this.m_hidden);
			}
			if (this.m_offsetX != null)
			{
				this.m_offsetX.Initialize("OffsetX", context);
				context.ExprHostBuilder.MapSpatialElementTemplateOffsetX(this.m_offsetX);
			}
			if (this.m_offsetY != null)
			{
				this.m_offsetY.Initialize("OffsetY", context);
				context.ExprHostBuilder.MapSpatialElementTemplateOffsetY(this.m_offsetY);
			}
			if (this.m_label != null)
			{
				this.m_label.Initialize("Label", context);
				context.ExprHostBuilder.MapSpatialElementTemplateLabel(this.m_label);
			}
			if (this.m_toolTip != null)
			{
				this.m_toolTip.Initialize("ToolTip", context);
				context.ExprHostBuilder.MapSpatialElementTemplateToolTip(this.m_toolTip);
			}
			if (this.m_dataElementLabel != null)
			{
				this.m_dataElementLabel.Initialize("DataElementLabel", context);
				context.ExprHostBuilder.MapSpatialElementTemplateDataElementLabel(this.m_dataElementLabel);
			}
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapSpatialElementTemplate mapSpatialElementTemplate = (MapSpatialElementTemplate)base.PublishClone(context);
			mapSpatialElementTemplate.m_mapVectorLayer = context.CurrentMapVectorLayerClone;
			if (this.m_action != null)
			{
				mapSpatialElementTemplate.m_action = (Action)this.m_action.PublishClone(context);
			}
			if (this.m_hidden != null)
			{
				mapSpatialElementTemplate.m_hidden = (ExpressionInfo)this.m_hidden.PublishClone(context);
			}
			if (this.m_offsetX != null)
			{
				mapSpatialElementTemplate.m_offsetX = (ExpressionInfo)this.m_offsetX.PublishClone(context);
			}
			if (this.m_offsetY != null)
			{
				mapSpatialElementTemplate.m_offsetY = (ExpressionInfo)this.m_offsetY.PublishClone(context);
			}
			if (this.m_label != null)
			{
				mapSpatialElementTemplate.m_label = (ExpressionInfo)this.m_label.PublishClone(context);
			}
			if (this.m_toolTip != null)
			{
				mapSpatialElementTemplate.m_toolTip = (ExpressionInfo)this.m_toolTip.PublishClone(context);
			}
			if (this.m_dataElementLabel != null)
			{
				mapSpatialElementTemplate.m_dataElementLabel = (ExpressionInfo)this.m_dataElementLabel.PublishClone(context);
			}
			return mapSpatialElementTemplate;
		}

		internal void SetExprHost(MapSpatialElementTemplateExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			this.m_exprHost = exprHost;
			base.SetExprHost(exprHost, reportObjectModel);
			if (this.m_action != null && this.ExprHost.ActionInfoHost != null)
			{
				this.m_action.SetExprHost(this.ExprHost.ActionInfoHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Action, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Action));
			list.Add(new MemberInfo(MemberName.ID, Token.Int32));
			list.Add(new MemberInfo(MemberName.Hidden, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.OffsetX, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.OffsetY, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Label, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ToolTip, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DataElementLabel, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DataElementName, Token.String));
			list.Add(new MemberInfo(MemberName.DataElementOutput, Token.Enum));
			list.Add(new MemberInfo(MemberName.MapVectorLayer, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapVectorLayer, Token.Reference));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSpatialElementTemplate, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapStyleContainer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(MapSpatialElementTemplate.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Action:
					writer.Write(this.m_action);
					break;
				case MemberName.ID:
					writer.Write(this.m_id);
					break;
				case MemberName.Hidden:
					writer.Write(this.m_hidden);
					break;
				case MemberName.OffsetX:
					writer.Write(this.m_offsetX);
					break;
				case MemberName.OffsetY:
					writer.Write(this.m_offsetY);
					break;
				case MemberName.Label:
					writer.Write(this.m_label);
					break;
				case MemberName.ToolTip:
					writer.Write(this.m_toolTip);
					break;
				case MemberName.MapVectorLayer:
					writer.WriteReference(this.m_mapVectorLayer);
					break;
				case MemberName.DataElementLabel:
					writer.Write(this.m_dataElementLabel);
					break;
				case MemberName.DataElementName:
					writer.Write(this.m_dataElementName);
					break;
				case MemberName.DataElementOutput:
					writer.WriteEnum((int)this.m_dataElementOutput);
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
			reader.RegisterDeclaration(MapSpatialElementTemplate.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Action:
					this.m_action = (Action)reader.ReadRIFObject();
					break;
				case MemberName.ID:
					this.m_id = reader.ReadInt32();
					break;
				case MemberName.Hidden:
					this.m_hidden = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.OffsetX:
					this.m_offsetX = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.OffsetY:
					this.m_offsetY = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Label:
					this.m_label = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ToolTip:
					this.m_toolTip = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MapVectorLayer:
					this.m_mapVectorLayer = reader.ReadReference<MapVectorLayer>(this);
					break;
				case MemberName.DataElementLabel:
					this.m_dataElementLabel = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DataElementName:
					this.m_dataElementName = reader.ReadString();
					break;
				case MemberName.DataElementOutput:
					this.m_dataElementOutput = (DataElementOutputTypes)reader.ReadEnum();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
			List<MemberReference> list = default(List<MemberReference>);
			if (memberReferencesCollection.TryGetValue(MapSpatialElementTemplate.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item in list)
				{
					MemberName memberName = item.MemberName;
					if (memberName == MemberName.MapVectorLayer)
					{
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						this.m_mapVectorLayer = (MapVectorLayer)referenceableItems[item.RefID];
					}
					else
					{
						Global.Tracer.Assert(false);
					}
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSpatialElementTemplate;
		}

		internal bool EvaluateHidden(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapSpatialElementTemplateHiddenExpression(this, base.m_map.Name);
		}

		internal double EvaluateOffsetX(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapSpatialElementTemplateOffsetXExpression(this, base.m_map.Name);
		}

		internal double EvaluateOffsetY(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapSpatialElementTemplateOffsetYExpression(this, base.m_map.Name);
		}

		internal string EvaluateLabel(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.InstancePath, reportScopeInstance);
			AspNetCore.ReportingServices.RdlExpressions.VariantResult variantResult = context.ReportRuntime.EvaluateMapSpatialElementTemplateLabelExpression(this, base.m_map.Name);
			return base.m_map.GetFormattedStringFromValue(ref variantResult, context);
		}

		internal string EvaluateToolTip(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.InstancePath, reportScopeInstance);
			AspNetCore.ReportingServices.RdlExpressions.VariantResult variantResult = context.ReportRuntime.EvaluateMapSpatialElementTemplateToolTipExpression(this, base.m_map.Name);
			return base.m_map.GetFormattedStringFromValue(ref variantResult, context);
		}

		internal string EvaluateDataElementLabel(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.InstancePath, reportScopeInstance);
			AspNetCore.ReportingServices.RdlExpressions.VariantResult variantResult = context.ReportRuntime.EvaluateMapSpatialElementTemplateDataElementLabelExpression(this, base.m_map.Name);
			return base.m_map.GetFormattedStringFromValue(ref variantResult, context);
		}
	}
}
