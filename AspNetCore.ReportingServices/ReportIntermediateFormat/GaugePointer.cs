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
	internal class GaugePointer : GaugePanelStyleContainer, IPersistable, IActionOwner
	{
		private Action m_action;

		protected int m_exprHostID;

		[NonSerialized]
		private List<string> m_fieldsUsedInValueExpression;

		[NonSerialized]
		protected GaugePointerExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GaugePointer.GetDeclaration();

		protected string m_name;

		private GaugeInputValue m_gaugeInputValue;

		private ExpressionInfo m_barStart;

		private ExpressionInfo m_distanceFromScale;

		private PointerImage m_pointerImage;

		private ExpressionInfo m_markerLength;

		private ExpressionInfo m_markerStyle;

		private ExpressionInfo m_placement;

		private ExpressionInfo m_snappingEnabled;

		private ExpressionInfo m_snappingInterval;

		private ExpressionInfo m_toolTip;

		private ExpressionInfo m_hidden;

		private ExpressionInfo m_width;

		private int m_id;

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

		internal int ID
		{
			get
			{
				return this.m_id;
			}
		}

		internal GaugeInputValue GaugeInputValue
		{
			get
			{
				return this.m_gaugeInputValue;
			}
			set
			{
				this.m_gaugeInputValue = value;
			}
		}

		internal ExpressionInfo BarStart
		{
			get
			{
				return this.m_barStart;
			}
			set
			{
				this.m_barStart = value;
			}
		}

		internal ExpressionInfo DistanceFromScale
		{
			get
			{
				return this.m_distanceFromScale;
			}
			set
			{
				this.m_distanceFromScale = value;
			}
		}

		internal PointerImage PointerImage
		{
			get
			{
				return this.m_pointerImage;
			}
			set
			{
				this.m_pointerImage = value;
			}
		}

		internal ExpressionInfo MarkerLength
		{
			get
			{
				return this.m_markerLength;
			}
			set
			{
				this.m_markerLength = value;
			}
		}

		internal ExpressionInfo MarkerStyle
		{
			get
			{
				return this.m_markerStyle;
			}
			set
			{
				this.m_markerStyle = value;
			}
		}

		internal ExpressionInfo Placement
		{
			get
			{
				return this.m_placement;
			}
			set
			{
				this.m_placement = value;
			}
		}

		internal ExpressionInfo SnappingEnabled
		{
			get
			{
				return this.m_snappingEnabled;
			}
			set
			{
				this.m_snappingEnabled = value;
			}
		}

		internal ExpressionInfo SnappingInterval
		{
			get
			{
				return this.m_snappingInterval;
			}
			set
			{
				this.m_snappingInterval = value;
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

		internal ExpressionInfo Width
		{
			get
			{
				return this.m_width;
			}
			set
			{
				this.m_width = value;
			}
		}

		internal string OwnerName
		{
			get
			{
				return base.m_gaugePanel.Name;
			}
		}

		internal GaugePointerExprHost ExprHost
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

		internal GaugePointer()
		{
		}

		internal GaugePointer(GaugePanel gaugePanel, int id)
			: base(gaugePanel)
		{
			this.m_id = id;
		}

		internal override void Initialize(InitializationContext context)
		{
			base.Initialize(context);
			if (this.m_action != null)
			{
				this.m_action.Initialize(context);
			}
			if (this.m_barStart != null)
			{
				this.m_barStart.Initialize("BarStart", context);
				context.ExprHostBuilder.GaugePointerBarStart(this.m_barStart);
			}
			if (this.m_distanceFromScale != null)
			{
				this.m_distanceFromScale.Initialize("DistanceFromScale", context);
				context.ExprHostBuilder.GaugePointerDistanceFromScale(this.m_distanceFromScale);
			}
			if (this.m_pointerImage != null)
			{
				this.m_pointerImage.Initialize(context);
			}
			if (this.m_markerLength != null)
			{
				this.m_markerLength.Initialize("MarkerLength", context);
				context.ExprHostBuilder.GaugePointerMarkerLength(this.m_markerLength);
			}
			if (this.m_markerStyle != null)
			{
				this.m_markerStyle.Initialize("MarkerStyle", context);
				context.ExprHostBuilder.GaugePointerMarkerStyle(this.m_markerStyle);
			}
			if (this.m_placement != null)
			{
				this.m_placement.Initialize("Placement", context);
				context.ExprHostBuilder.GaugePointerPlacement(this.m_placement);
			}
			if (this.m_snappingEnabled != null)
			{
				this.m_snappingEnabled.Initialize("SnappingEnabled", context);
				context.ExprHostBuilder.GaugePointerSnappingEnabled(this.m_snappingEnabled);
			}
			if (this.m_snappingInterval != null)
			{
				this.m_snappingInterval.Initialize("SnappingInterval", context);
				context.ExprHostBuilder.GaugePointerSnappingInterval(this.m_snappingInterval);
			}
			if (this.m_toolTip != null)
			{
				this.m_toolTip.Initialize("ToolTip", context);
				context.ExprHostBuilder.GaugePointerToolTip(this.m_toolTip);
			}
			if (this.m_hidden != null)
			{
				this.m_hidden.Initialize("Hidden", context);
				context.ExprHostBuilder.GaugePointerHidden(this.m_hidden);
			}
			if (this.m_width != null)
			{
				this.m_width.Initialize("Width", context);
				context.ExprHostBuilder.GaugePointerWidth(this.m_width);
			}
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			GaugePointer gaugePointer = (GaugePointer)base.PublishClone(context);
			if (this.m_action != null)
			{
				gaugePointer.m_action = (Action)this.m_action.PublishClone(context);
			}
			if (this.m_gaugeInputValue != null)
			{
				gaugePointer.m_gaugeInputValue = (GaugeInputValue)this.m_gaugeInputValue.PublishClone(context);
			}
			if (this.m_barStart != null)
			{
				gaugePointer.m_barStart = (ExpressionInfo)this.m_barStart.PublishClone(context);
			}
			if (this.m_distanceFromScale != null)
			{
				gaugePointer.m_distanceFromScale = (ExpressionInfo)this.m_distanceFromScale.PublishClone(context);
			}
			if (this.m_pointerImage != null)
			{
				gaugePointer.m_pointerImage = (PointerImage)this.m_pointerImage.PublishClone(context);
			}
			if (this.m_markerLength != null)
			{
				gaugePointer.m_markerLength = (ExpressionInfo)this.m_markerLength.PublishClone(context);
			}
			if (this.m_markerStyle != null)
			{
				gaugePointer.m_markerStyle = (ExpressionInfo)this.m_markerStyle.PublishClone(context);
			}
			if (this.m_placement != null)
			{
				gaugePointer.m_placement = (ExpressionInfo)this.m_placement.PublishClone(context);
			}
			if (this.m_snappingEnabled != null)
			{
				gaugePointer.m_snappingEnabled = (ExpressionInfo)this.m_snappingEnabled.PublishClone(context);
			}
			if (this.m_snappingInterval != null)
			{
				gaugePointer.m_snappingInterval = (ExpressionInfo)this.m_snappingInterval.PublishClone(context);
			}
			if (this.m_toolTip != null)
			{
				gaugePointer.m_toolTip = (ExpressionInfo)this.m_toolTip.PublishClone(context);
			}
			if (this.m_hidden != null)
			{
				gaugePointer.m_hidden = (ExpressionInfo)this.m_hidden.PublishClone(context);
			}
			if (this.m_width != null)
			{
				gaugePointer.m_width = (ExpressionInfo)this.m_width.PublishClone(context);
			}
			return gaugePointer;
		}

		internal void SetExprHost(GaugePointerExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			base.SetExprHost(exprHost, reportObjectModel);
			this.m_exprHost = exprHost;
			if (this.m_pointerImage != null && this.m_exprHost.PointerImageHost != null)
			{
				this.m_pointerImage.SetExprHost(this.m_exprHost.PointerImageHost, reportObjectModel);
			}
			if (this.m_action != null && exprHost.ActionInfoHost != null)
			{
				this.m_action.SetExprHost(exprHost.ActionInfoHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.Action, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Action));
			list.Add(new MemberInfo(MemberName.GaugeInputValue, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeInputValue));
			list.Add(new MemberInfo(MemberName.BarStart, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DistanceFromScale, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.PointerImage, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PointerImage));
			list.Add(new MemberInfo(MemberName.MarkerLength, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MarkerStyle, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Placement, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.SnappingEnabled, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.SnappingInterval, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ToolTip, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Hidden, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Width, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.ID, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePointer, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanelStyleContainer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(GaugePointer.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Action:
					writer.Write(this.m_action);
					break;
				case MemberName.Name:
					writer.Write(this.m_name);
					break;
				case MemberName.GaugeInputValue:
					writer.Write(this.m_gaugeInputValue);
					break;
				case MemberName.BarStart:
					writer.Write(this.m_barStart);
					break;
				case MemberName.DistanceFromScale:
					writer.Write(this.m_distanceFromScale);
					break;
				case MemberName.PointerImage:
					writer.Write(this.m_pointerImage);
					break;
				case MemberName.MarkerLength:
					writer.Write(this.m_markerLength);
					break;
				case MemberName.MarkerStyle:
					writer.Write(this.m_markerStyle);
					break;
				case MemberName.Placement:
					writer.Write(this.m_placement);
					break;
				case MemberName.SnappingEnabled:
					writer.Write(this.m_snappingEnabled);
					break;
				case MemberName.SnappingInterval:
					writer.Write(this.m_snappingInterval);
					break;
				case MemberName.ToolTip:
					writer.Write(this.m_toolTip);
					break;
				case MemberName.Hidden:
					writer.Write(this.m_hidden);
					break;
				case MemberName.Width:
					writer.Write(this.m_width);
					break;
				case MemberName.ExprHostID:
					writer.Write(this.m_exprHostID);
					break;
				case MemberName.ID:
					writer.Write(this.m_id);
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
			reader.RegisterDeclaration(GaugePointer.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Action:
					this.m_action = (Action)reader.ReadRIFObject();
					break;
				case MemberName.Name:
					this.m_name = reader.ReadString();
					break;
				case MemberName.GaugeInputValue:
					this.m_gaugeInputValue = (GaugeInputValue)reader.ReadRIFObject();
					break;
				case MemberName.BarStart:
					this.m_barStart = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DistanceFromScale:
					this.m_distanceFromScale = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.PointerImage:
					this.m_pointerImage = (PointerImage)reader.ReadRIFObject();
					break;
				case MemberName.MarkerLength:
					this.m_markerLength = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MarkerStyle:
					this.m_markerStyle = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Placement:
					this.m_placement = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.SnappingEnabled:
					this.m_snappingEnabled = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.SnappingInterval:
					this.m_snappingInterval = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ToolTip:
					this.m_toolTip = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Hidden:
					this.m_hidden = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Width:
					this.m_width = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ExprHostID:
					this.m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.ID:
					this.m_id = reader.ReadInt32();
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
			if (this.m_id == 0)
			{
				this.m_id = base.m_gaugePanel.GenerateActionOwnerID();
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePointer;
		}

		internal GaugeBarStarts EvaluateBarStart(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateGaugeBarStarts(context.ReportRuntime.EvaluateGaugePointerBarStartExpression(this, base.m_gaugePanel.Name), context.ReportRuntime);
		}

		internal double EvaluateDistanceFromScale(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugePointerDistanceFromScaleExpression(this, base.m_gaugePanel.Name);
		}

		internal double EvaluateMarkerLength(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugePointerMarkerLengthExpression(this, base.m_gaugePanel.Name);
		}

		internal GaugeMarkerStyles EvaluateMarkerStyle(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateGaugeMarkerStyles(context.ReportRuntime.EvaluateGaugePointerMarkerStyleExpression(this, base.m_gaugePanel.Name), context.ReportRuntime);
		}

		internal GaugePointerPlacements EvaluatePlacement(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateGaugePointerPlacements(context.ReportRuntime.EvaluateGaugePointerPlacementExpression(this, base.m_gaugePanel.Name), context.ReportRuntime);
		}

		internal bool EvaluateSnappingEnabled(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugePointerSnappingEnabledExpression(this, base.m_gaugePanel.Name);
		}

		internal double EvaluateSnappingInterval(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugePointerSnappingIntervalExpression(this, base.m_gaugePanel.Name);
		}

		internal string EvaluateToolTip(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugePointerToolTipExpression(this, base.m_gaugePanel.Name);
		}

		internal bool EvaluateHidden(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugePointerHiddenExpression(this, base.m_gaugePanel.Name);
		}

		internal double EvaluateWidth(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugePointerWidthExpression(this, base.m_gaugePanel.Name);
		}
	}
}
