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
	internal class GaugeScale : GaugePanelStyleContainer, IPersistable, IActionOwner
	{
		private Action m_action;

		protected int m_exprHostID;

		[NonSerialized]
		private List<string> m_fieldsUsedInValueExpression;

		[NonSerialized]
		protected GaugeScaleExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GaugeScale.GetDeclaration();

		protected string m_name;

		private List<ScaleRange> m_scaleRanges;

		private List<CustomLabel> m_customLabels;

		private ExpressionInfo m_interval;

		private ExpressionInfo m_intervalOffset;

		private ExpressionInfo m_logarithmic;

		private ExpressionInfo m_logarithmicBase;

		private GaugeInputValue m_maximumValue;

		private GaugeInputValue m_minimumValue;

		private ExpressionInfo m_multiplier;

		private ExpressionInfo m_reversed;

		private GaugeTickMarks m_gaugeMajorTickMarks;

		private GaugeTickMarks m_gaugeMinorTickMarks;

		private ScalePin m_maximumPin;

		private ScalePin m_minimumPin;

		private ScaleLabels m_scaleLabels;

		private ExpressionInfo m_tickMarksOnTop;

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

		internal List<ScaleRange> ScaleRanges
		{
			get
			{
				return this.m_scaleRanges;
			}
			set
			{
				this.m_scaleRanges = value;
			}
		}

		internal List<CustomLabel> CustomLabels
		{
			get
			{
				return this.m_customLabels;
			}
			set
			{
				this.m_customLabels = value;
			}
		}

		internal ExpressionInfo Interval
		{
			get
			{
				return this.m_interval;
			}
			set
			{
				this.m_interval = value;
			}
		}

		internal ExpressionInfo IntervalOffset
		{
			get
			{
				return this.m_intervalOffset;
			}
			set
			{
				this.m_intervalOffset = value;
			}
		}

		internal ExpressionInfo Logarithmic
		{
			get
			{
				return this.m_logarithmic;
			}
			set
			{
				this.m_logarithmic = value;
			}
		}

		internal ExpressionInfo LogarithmicBase
		{
			get
			{
				return this.m_logarithmicBase;
			}
			set
			{
				this.m_logarithmicBase = value;
			}
		}

		internal GaugeInputValue MaximumValue
		{
			get
			{
				return this.m_maximumValue;
			}
			set
			{
				this.m_maximumValue = value;
			}
		}

		internal GaugeInputValue MinimumValue
		{
			get
			{
				return this.m_minimumValue;
			}
			set
			{
				this.m_minimumValue = value;
			}
		}

		internal ExpressionInfo Multiplier
		{
			get
			{
				return this.m_multiplier;
			}
			set
			{
				this.m_multiplier = value;
			}
		}

		internal ExpressionInfo Reversed
		{
			get
			{
				return this.m_reversed;
			}
			set
			{
				this.m_reversed = value;
			}
		}

		internal GaugeTickMarks GaugeMajorTickMarks
		{
			get
			{
				return this.m_gaugeMajorTickMarks;
			}
			set
			{
				this.m_gaugeMajorTickMarks = value;
			}
		}

		internal GaugeTickMarks GaugeMinorTickMarks
		{
			get
			{
				return this.m_gaugeMinorTickMarks;
			}
			set
			{
				this.m_gaugeMinorTickMarks = value;
			}
		}

		internal ScalePin MaximumPin
		{
			get
			{
				return this.m_maximumPin;
			}
			set
			{
				this.m_maximumPin = value;
			}
		}

		internal ScalePin MinimumPin
		{
			get
			{
				return this.m_minimumPin;
			}
			set
			{
				this.m_minimumPin = value;
			}
		}

		internal ScaleLabels ScaleLabels
		{
			get
			{
				return this.m_scaleLabels;
			}
			set
			{
				this.m_scaleLabels = value;
			}
		}

		internal ExpressionInfo TickMarksOnTop
		{
			get
			{
				return this.m_tickMarksOnTop;
			}
			set
			{
				this.m_tickMarksOnTop = value;
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

		internal GaugeScaleExprHost ExprHost
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

		internal GaugeScale()
		{
		}

		internal GaugeScale(GaugePanel gaugePanel, int id)
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
			if (this.m_scaleRanges != null)
			{
				for (int i = 0; i < this.m_scaleRanges.Count; i++)
				{
					this.m_scaleRanges[i].Initialize(context);
				}
			}
			if (this.m_customLabels != null)
			{
				for (int j = 0; j < this.m_customLabels.Count; j++)
				{
					this.m_customLabels[j].Initialize(context);
				}
			}
			if (this.m_interval != null)
			{
				this.m_interval.Initialize("Interval", context);
				context.ExprHostBuilder.GaugeScaleInterval(this.m_interval);
			}
			if (this.m_intervalOffset != null)
			{
				this.m_intervalOffset.Initialize("IntervalOffset", context);
				context.ExprHostBuilder.GaugeScaleIntervalOffset(this.m_intervalOffset);
			}
			if (this.m_logarithmic != null)
			{
				this.m_logarithmic.Initialize("Logarithmic", context);
				context.ExprHostBuilder.GaugeScaleLogarithmic(this.m_logarithmic);
			}
			if (this.m_logarithmicBase != null)
			{
				this.m_logarithmicBase.Initialize("LogarithmicBase", context);
				context.ExprHostBuilder.GaugeScaleLogarithmicBase(this.m_logarithmicBase);
			}
			if (this.m_multiplier != null)
			{
				this.m_multiplier.Initialize("Multiplier", context);
				context.ExprHostBuilder.GaugeScaleMultiplier(this.m_multiplier);
			}
			if (this.m_reversed != null)
			{
				this.m_reversed.Initialize("Reversed", context);
				context.ExprHostBuilder.GaugeScaleReversed(this.m_reversed);
			}
			if (this.m_gaugeMajorTickMarks != null)
			{
				this.m_gaugeMajorTickMarks.Initialize(context, true);
			}
			if (this.m_gaugeMinorTickMarks != null)
			{
				this.m_gaugeMinorTickMarks.Initialize(context, false);
			}
			if (this.m_maximumPin != null)
			{
				this.m_maximumPin.Initialize(context, true);
			}
			if (this.m_minimumPin != null)
			{
				this.m_minimumPin.Initialize(context, false);
			}
			if (this.m_scaleLabels != null)
			{
				this.m_scaleLabels.Initialize(context);
			}
			if (this.m_tickMarksOnTop != null)
			{
				this.m_tickMarksOnTop.Initialize("TickMarksOnTop", context);
				context.ExprHostBuilder.GaugeScaleTickMarksOnTop(this.m_tickMarksOnTop);
			}
			if (this.m_toolTip != null)
			{
				this.m_toolTip.Initialize("ToolTip", context);
				context.ExprHostBuilder.GaugeScaleToolTip(this.m_toolTip);
			}
			if (this.m_hidden != null)
			{
				this.m_hidden.Initialize("Hidden", context);
				context.ExprHostBuilder.GaugeScaleHidden(this.m_hidden);
			}
			if (this.m_width != null)
			{
				this.m_width.Initialize("Width", context);
				context.ExprHostBuilder.GaugeScaleWidth(this.m_width);
			}
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			GaugeScale gaugeScale = (GaugeScale)base.PublishClone(context);
			if (this.m_action != null)
			{
				gaugeScale.m_action = (Action)this.m_action.PublishClone(context);
			}
			if (this.m_scaleRanges != null)
			{
				gaugeScale.m_scaleRanges = new List<ScaleRange>(this.m_scaleRanges.Count);
				foreach (ScaleRange scaleRange in this.m_scaleRanges)
				{
					gaugeScale.m_scaleRanges.Add((ScaleRange)scaleRange.PublishClone(context));
				}
			}
			if (this.m_customLabels != null)
			{
				gaugeScale.m_customLabels = new List<CustomLabel>(this.m_customLabels.Count);
				foreach (CustomLabel customLabel in this.m_customLabels)
				{
					gaugeScale.m_customLabels.Add((CustomLabel)customLabel.PublishClone(context));
				}
			}
			if (this.m_interval != null)
			{
				gaugeScale.m_interval = (ExpressionInfo)this.m_interval.PublishClone(context);
			}
			if (this.m_intervalOffset != null)
			{
				gaugeScale.m_intervalOffset = (ExpressionInfo)this.m_intervalOffset.PublishClone(context);
			}
			if (this.m_logarithmic != null)
			{
				gaugeScale.m_logarithmic = (ExpressionInfo)this.m_logarithmic.PublishClone(context);
			}
			if (this.m_logarithmicBase != null)
			{
				gaugeScale.m_logarithmicBase = (ExpressionInfo)this.m_logarithmicBase.PublishClone(context);
			}
			if (this.m_maximumValue != null)
			{
				gaugeScale.m_maximumValue = (GaugeInputValue)this.m_maximumValue.PublishClone(context);
			}
			if (this.m_minimumValue != null)
			{
				gaugeScale.m_minimumValue = (GaugeInputValue)this.m_minimumValue.PublishClone(context);
			}
			if (this.m_multiplier != null)
			{
				gaugeScale.m_multiplier = (ExpressionInfo)this.m_multiplier.PublishClone(context);
			}
			if (this.m_reversed != null)
			{
				gaugeScale.m_reversed = (ExpressionInfo)this.m_reversed.PublishClone(context);
			}
			if (this.m_gaugeMajorTickMarks != null)
			{
				gaugeScale.m_gaugeMajorTickMarks = (GaugeTickMarks)this.m_gaugeMajorTickMarks.PublishClone(context);
			}
			if (this.m_gaugeMinorTickMarks != null)
			{
				gaugeScale.m_gaugeMinorTickMarks = (GaugeTickMarks)this.m_gaugeMinorTickMarks.PublishClone(context);
			}
			if (this.m_maximumPin != null)
			{
				gaugeScale.m_maximumPin = (ScalePin)this.m_maximumPin.PublishClone(context);
			}
			if (this.m_minimumPin != null)
			{
				gaugeScale.m_minimumPin = (ScalePin)this.m_minimumPin.PublishClone(context);
			}
			if (this.m_scaleLabels != null)
			{
				gaugeScale.m_scaleLabels = (ScaleLabels)this.m_scaleLabels.PublishClone(context);
			}
			if (this.m_tickMarksOnTop != null)
			{
				gaugeScale.m_tickMarksOnTop = (ExpressionInfo)this.m_tickMarksOnTop.PublishClone(context);
			}
			if (this.m_toolTip != null)
			{
				gaugeScale.m_toolTip = (ExpressionInfo)this.m_toolTip.PublishClone(context);
			}
			if (this.m_hidden != null)
			{
				gaugeScale.m_hidden = (ExpressionInfo)this.m_hidden.PublishClone(context);
			}
			if (this.m_width != null)
			{
				gaugeScale.m_width = (ExpressionInfo)this.m_width.PublishClone(context);
			}
			return gaugeScale;
		}

		internal void SetExprHost(GaugeScaleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			base.SetExprHost(exprHost, reportObjectModel);
			this.m_exprHost = exprHost;
			IList<ScaleRangeExprHost> scaleRangesHostsRemotable = this.m_exprHost.ScaleRangesHostsRemotable;
			if (this.m_scaleRanges != null && scaleRangesHostsRemotable != null)
			{
				for (int i = 0; i < this.m_scaleRanges.Count; i++)
				{
					ScaleRange scaleRange = this.m_scaleRanges[i];
					if (scaleRange != null && scaleRange.ExpressionHostID > -1)
					{
						scaleRange.SetExprHost(scaleRangesHostsRemotable[scaleRange.ExpressionHostID], reportObjectModel);
					}
				}
			}
			IList<CustomLabelExprHost> customLabelsHostsRemotable = this.m_exprHost.CustomLabelsHostsRemotable;
			if (this.m_customLabels != null && customLabelsHostsRemotable != null)
			{
				for (int j = 0; j < this.m_customLabels.Count; j++)
				{
					CustomLabel customLabel = this.m_customLabels[j];
					if (customLabel != null && customLabel.ExpressionHostID > -1)
					{
						customLabel.SetExprHost(customLabelsHostsRemotable[customLabel.ExpressionHostID], reportObjectModel);
					}
				}
			}
			if (this.m_gaugeMajorTickMarks != null && this.m_exprHost.GaugeMajorTickMarksHost != null)
			{
				this.m_gaugeMajorTickMarks.SetExprHost(this.m_exprHost.GaugeMajorTickMarksHost, reportObjectModel);
			}
			if (this.m_gaugeMinorTickMarks != null && this.m_exprHost.GaugeMinorTickMarksHost != null)
			{
				this.m_gaugeMinorTickMarks.SetExprHost(this.m_exprHost.GaugeMinorTickMarksHost, reportObjectModel);
			}
			if (this.m_maximumPin != null && this.m_exprHost.MaximumPinHost != null)
			{
				this.m_maximumPin.SetExprHost(this.m_exprHost.MaximumPinHost, reportObjectModel);
			}
			if (this.m_minimumPin != null && this.m_exprHost.MinimumPinHost != null)
			{
				this.m_minimumPin.SetExprHost(this.m_exprHost.MinimumPinHost, reportObjectModel);
			}
			if (this.m_scaleLabels != null && this.m_exprHost.ScaleLabelsHost != null)
			{
				this.m_scaleLabels.SetExprHost(this.m_exprHost.ScaleLabelsHost, reportObjectModel);
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
			list.Add(new MemberInfo(MemberName.ScaleRanges, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScaleRange));
			list.Add(new MemberInfo(MemberName.CustomLabels, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.CustomLabel));
			list.Add(new MemberInfo(MemberName.Interval, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IntervalOffset, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Logarithmic, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.LogarithmicBase, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MaximumValue, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeInputValue));
			list.Add(new MemberInfo(MemberName.MinimumValue, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeInputValue));
			list.Add(new MemberInfo(MemberName.Multiplier, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Reversed, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.GaugeMajorTickMarks, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeTickMarks));
			list.Add(new MemberInfo(MemberName.GaugeMinorTickMarks, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeTickMarks));
			list.Add(new MemberInfo(MemberName.MaximumPin, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalePin));
			list.Add(new MemberInfo(MemberName.MinimumPin, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalePin));
			list.Add(new MemberInfo(MemberName.ScaleLabels, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScaleLabels));
			list.Add(new MemberInfo(MemberName.TickMarksOnTop, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ToolTip, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Hidden, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Width, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.ID, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeScale, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanelStyleContainer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(GaugeScale.m_Declaration);
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
				case MemberName.ScaleRanges:
					writer.Write(this.m_scaleRanges);
					break;
				case MemberName.CustomLabels:
					writer.Write(this.m_customLabels);
					break;
				case MemberName.Interval:
					writer.Write(this.m_interval);
					break;
				case MemberName.IntervalOffset:
					writer.Write(this.m_intervalOffset);
					break;
				case MemberName.Logarithmic:
					writer.Write(this.m_logarithmic);
					break;
				case MemberName.LogarithmicBase:
					writer.Write(this.m_logarithmicBase);
					break;
				case MemberName.MaximumValue:
					writer.Write(this.m_maximumValue);
					break;
				case MemberName.MinimumValue:
					writer.Write(this.m_minimumValue);
					break;
				case MemberName.Multiplier:
					writer.Write(this.m_multiplier);
					break;
				case MemberName.Reversed:
					writer.Write(this.m_reversed);
					break;
				case MemberName.GaugeMajorTickMarks:
					writer.Write(this.m_gaugeMajorTickMarks);
					break;
				case MemberName.GaugeMinorTickMarks:
					writer.Write(this.m_gaugeMinorTickMarks);
					break;
				case MemberName.MaximumPin:
					writer.Write(this.m_maximumPin);
					break;
				case MemberName.MinimumPin:
					writer.Write(this.m_minimumPin);
					break;
				case MemberName.ScaleLabels:
					writer.Write(this.m_scaleLabels);
					break;
				case MemberName.TickMarksOnTop:
					writer.Write(this.m_tickMarksOnTop);
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
			reader.RegisterDeclaration(GaugeScale.m_Declaration);
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
				case MemberName.ScaleRanges:
					this.m_scaleRanges = reader.ReadGenericListOfRIFObjects<ScaleRange>();
					break;
				case MemberName.CustomLabels:
					this.m_customLabels = reader.ReadGenericListOfRIFObjects<CustomLabel>();
					break;
				case MemberName.Interval:
					this.m_interval = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.IntervalOffset:
					this.m_intervalOffset = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Logarithmic:
					this.m_logarithmic = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.LogarithmicBase:
					this.m_logarithmicBase = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MaximumValue:
					this.m_maximumValue = (GaugeInputValue)reader.ReadRIFObject();
					break;
				case MemberName.MinimumValue:
					this.m_minimumValue = (GaugeInputValue)reader.ReadRIFObject();
					break;
				case MemberName.Multiplier:
					this.m_multiplier = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Reversed:
					this.m_reversed = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.GaugeMajorTickMarks:
					this.m_gaugeMajorTickMarks = (GaugeTickMarks)reader.ReadRIFObject();
					break;
				case MemberName.GaugeMinorTickMarks:
					this.m_gaugeMinorTickMarks = (GaugeTickMarks)reader.ReadRIFObject();
					break;
				case MemberName.MaximumPin:
					this.m_maximumPin = (ScalePin)reader.ReadRIFObject();
					break;
				case MemberName.MinimumPin:
					this.m_minimumPin = (ScalePin)reader.ReadRIFObject();
					break;
				case MemberName.ScaleLabels:
					this.m_scaleLabels = (ScaleLabels)reader.ReadRIFObject();
					break;
				case MemberName.TickMarksOnTop:
					this.m_tickMarksOnTop = (ExpressionInfo)reader.ReadRIFObject();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeScale;
		}

		internal double EvaluateInterval(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeScaleIntervalExpression(this, base.m_gaugePanel.Name);
		}

		internal double EvaluateIntervalOffset(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeScaleIntervalOffsetExpression(this, base.m_gaugePanel.Name);
		}

		internal bool EvaluateLogarithmic(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeScaleLogarithmicExpression(this, base.m_gaugePanel.Name);
		}

		internal double EvaluateLogarithmicBase(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeScaleLogarithmicBaseExpression(this, base.m_gaugePanel.Name);
		}

		internal double EvaluateMultiplier(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeScaleMultiplierExpression(this, base.m_gaugePanel.Name);
		}

		internal bool EvaluateReversed(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeScaleReversedExpression(this, base.m_gaugePanel.Name);
		}

		internal bool EvaluateTickMarksOnTop(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeScaleTickMarksOnTopExpression(this, base.m_gaugePanel.Name);
		}

		internal string EvaluateToolTip(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeScaleToolTipExpression(this, base.m_gaugePanel.Name);
		}

		internal bool EvaluateHidden(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeScaleHiddenExpression(this, base.m_gaugePanel.Name);
		}

		internal double EvaluateWidth(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeScaleWidthExpression(this, base.m_gaugePanel.Name);
		}
	}
}
