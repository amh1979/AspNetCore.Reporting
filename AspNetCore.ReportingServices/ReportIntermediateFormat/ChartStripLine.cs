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
	internal sealed class ChartStripLine : ChartStyleContainer, IPersistable, IActionOwner
	{
		private int m_exprHostID;

		private Action m_action;

		private ExpressionInfo m_title;

		private ExpressionInfo m_titleAngle;

		private ExpressionInfo m_textOrientation;

		private ExpressionInfo m_toolTip;

		private ExpressionInfo m_interval;

		private ExpressionInfo m_intervalType;

		private ExpressionInfo m_intervalOffset;

		private ExpressionInfo m_intervalOffsetType;

		private ExpressionInfo m_stripWidth;

		private ExpressionInfo m_stripWidthType;

		private int m_id;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ChartStripLine.GetDeclaration();

		[NonSerialized]
		private ChartStripLineExprHost m_exprHost;

		[NonSerialized]
		private List<string> m_fieldsUsedInValueExpression;

		internal ChartStripLineExprHost ExprHost
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

		internal int ID
		{
			get
			{
				return this.m_id;
			}
		}

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

		internal ExpressionInfo Title
		{
			get
			{
				return this.m_title;
			}
			set
			{
				this.m_title = value;
			}
		}

		internal ExpressionInfo TitleAngle
		{
			get
			{
				return this.m_titleAngle;
			}
			set
			{
				this.m_titleAngle = value;
			}
		}

		internal ExpressionInfo TextOrientation
		{
			get
			{
				return this.m_textOrientation;
			}
			set
			{
				this.m_textOrientation = value;
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

		internal ExpressionInfo IntervalType
		{
			get
			{
				return this.m_intervalType;
			}
			set
			{
				this.m_intervalType = value;
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

		internal ExpressionInfo IntervalOffsetType
		{
			get
			{
				return this.m_intervalOffsetType;
			}
			set
			{
				this.m_intervalOffsetType = value;
			}
		}

		internal ExpressionInfo StripWidth
		{
			get
			{
				return this.m_stripWidth;
			}
			set
			{
				this.m_stripWidth = value;
			}
		}

		internal ExpressionInfo StripWidthType
		{
			get
			{
				return this.m_stripWidthType;
			}
			set
			{
				this.m_stripWidthType = value;
			}
		}

		internal ChartStripLine()
		{
		}

		internal ChartStripLine(Chart chart, int id)
			: base(chart)
		{
			this.m_id = id;
		}

		internal void SetExprHost(ChartStripLineExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			this.m_exprHost = exprHost;
			if (this.m_action != null && this.m_exprHost.ActionInfoHost != null)
			{
				this.m_action.SetExprHost(exprHost.ActionInfoHost, reportObjectModel);
			}
		}

		internal void Initialize(InitializationContext context, int index)
		{
			context.ExprHostBuilder.ChartStripLineStart(index);
			base.Initialize(context);
			if (this.m_action != null)
			{
				this.m_action.Initialize(context);
			}
			if (this.m_title != null)
			{
				this.m_title.Initialize("Title", context);
				context.ExprHostBuilder.ChartStripLineTitle(this.m_title);
			}
			if (this.m_titleAngle != null)
			{
				this.m_titleAngle.Initialize("TitleAngle", context);
				context.ExprHostBuilder.ChartStripLineTitleAngle(this.m_titleAngle);
			}
			if (this.m_toolTip != null)
			{
				this.m_toolTip.Initialize("ToolTip", context);
				context.ExprHostBuilder.ChartStripLineToolTip(this.m_toolTip);
			}
			if (this.m_interval != null)
			{
				this.m_interval.Initialize("Interval", context);
				context.ExprHostBuilder.ChartStripLineInterval(this.m_interval);
			}
			if (this.m_intervalType != null)
			{
				this.m_intervalType.Initialize("IntervalType", context);
				context.ExprHostBuilder.ChartStripLineIntervalType(this.m_intervalType);
			}
			if (this.m_intervalOffset != null)
			{
				this.m_intervalOffset.Initialize("IntervalOffset", context);
				context.ExprHostBuilder.ChartStripLineIntervalOffset(this.m_intervalOffset);
			}
			if (this.m_intervalOffsetType != null)
			{
				this.m_intervalOffsetType.Initialize("IntervalOffsetType", context);
				context.ExprHostBuilder.ChartStripLineIntervalOffsetType(this.m_intervalOffsetType);
			}
			if (this.m_stripWidth != null)
			{
				this.m_stripWidth.Initialize("StripWidth", context);
				context.ExprHostBuilder.ChartStripLineStripWidth(this.m_stripWidth);
			}
			if (this.m_stripWidthType != null)
			{
				this.m_stripWidthType.Initialize("StripWidthType", context);
				context.ExprHostBuilder.ChartStripLineStripWidthType(this.m_stripWidthType);
			}
			if (this.m_textOrientation != null)
			{
				this.m_textOrientation.Initialize("TextOrientation", context);
				context.ExprHostBuilder.ChartStripLineTextOrientation(this.m_textOrientation);
			}
			this.m_exprHostID = context.ExprHostBuilder.ChartStripLineEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ChartStripLine chartStripLine = (ChartStripLine)base.PublishClone(context);
			if (this.m_action != null)
			{
				chartStripLine.m_action = (Action)this.m_action.PublishClone(context);
			}
			if (this.m_title != null)
			{
				chartStripLine.m_title = (ExpressionInfo)this.m_title.PublishClone(context);
			}
			if (this.m_titleAngle != null)
			{
				chartStripLine.m_titleAngle = (ExpressionInfo)this.m_titleAngle.PublishClone(context);
			}
			if (this.m_toolTip != null)
			{
				chartStripLine.m_toolTip = (ExpressionInfo)this.m_toolTip.PublishClone(context);
			}
			if (this.m_interval != null)
			{
				chartStripLine.m_interval = (ExpressionInfo)this.m_interval.PublishClone(context);
			}
			if (this.m_intervalType != null)
			{
				chartStripLine.m_intervalType = (ExpressionInfo)this.m_intervalType.PublishClone(context);
			}
			if (this.m_intervalOffset != null)
			{
				chartStripLine.m_intervalOffset = (ExpressionInfo)this.m_intervalOffset.PublishClone(context);
			}
			if (this.m_intervalOffsetType != null)
			{
				chartStripLine.m_intervalOffsetType = (ExpressionInfo)this.m_intervalOffsetType.PublishClone(context);
			}
			if (this.m_stripWidth != null)
			{
				chartStripLine.m_stripWidth = (ExpressionInfo)this.m_stripWidth.PublishClone(context);
			}
			if (this.m_stripWidthType != null)
			{
				chartStripLine.m_stripWidthType = (ExpressionInfo)this.m_stripWidthType.PublishClone(context);
			}
			if (this.m_textOrientation != null)
			{
				chartStripLine.m_textOrientation = (ExpressionInfo)this.m_textOrientation.PublishClone(context);
			}
			return chartStripLine;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Action, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Action));
			list.Add(new MemberInfo(MemberName.Title, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.TitleAngle, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ToolTip, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Interval, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IntervalType, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IntervalOffset, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IntervalOffsetType, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.StripWidth, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.StripWidthType, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.TextOrientation, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ID, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartStripLine, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartStyleContainer, list);
		}

		internal string EvaluateTitle(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartStripLineTitleExpression(this, base.m_chart.Name);
		}

		internal int EvaluateTitleAngle(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartStripLineTitleAngleExpression(this, base.m_chart.Name);
		}

		internal TextOrientations EvaluateTextOrientation(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return EnumTranslator.TranslateTextOrientations(context.ReportRuntime.EvaluateChartStripLineTextOrientationExpression(this, base.m_chart.Name), context.ReportRuntime);
		}

		internal string EvaluateToolTip(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartStripLineToolTipExpression(this, base.m_chart.Name);
		}

		internal double EvaluateInterval(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartStripLineIntervalExpression(this, base.m_chart.Name);
		}

		internal ChartIntervalType EvaluateIntervalType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartIntervalType(context.ReportRuntime.EvaluateChartStripLineIntervalTypeExpression(this, base.m_chart.Name), context.ReportRuntime);
		}

		internal double EvaluateIntervalOffset(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartStripLineIntervalOffsetExpression(this, base.m_chart.Name);
		}

		internal ChartIntervalType EvaluateIntervalOffsetType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartIntervalType(context.ReportRuntime.EvaluateChartStripLineIntervalOffsetTypeExpression(this, base.m_chart.Name), context.ReportRuntime);
		}

		internal double EvaluateStripWidth(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartStripLineStripWidthExpression(this, base.m_chart.Name);
		}

		internal ChartIntervalType EvaluateStripWidthType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartIntervalType(context.ReportRuntime.EvaluateChartStripLineStripWidthTypeExpression(this, base.m_chart.Name), context.ReportRuntime);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(ChartStripLine.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ExprHostID:
					writer.Write(this.m_exprHostID);
					break;
				case MemberName.Action:
					writer.Write(this.m_action);
					break;
				case MemberName.Title:
					writer.Write(this.m_title);
					break;
				case MemberName.TitleAngle:
					writer.Write(this.m_titleAngle);
					break;
				case MemberName.ToolTip:
					writer.Write(this.m_toolTip);
					break;
				case MemberName.Interval:
					writer.Write(this.m_interval);
					break;
				case MemberName.IntervalType:
					writer.Write(this.m_intervalType);
					break;
				case MemberName.IntervalOffset:
					writer.Write(this.m_intervalOffset);
					break;
				case MemberName.IntervalOffsetType:
					writer.Write(this.m_intervalOffsetType);
					break;
				case MemberName.StripWidth:
					writer.Write(this.m_stripWidth);
					break;
				case MemberName.StripWidthType:
					writer.Write(this.m_stripWidthType);
					break;
				case MemberName.TextOrientation:
					writer.Write(this.m_textOrientation);
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
			reader.RegisterDeclaration(ChartStripLine.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.ExprHostID:
					this.m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.Action:
					this.m_action = (Action)reader.ReadRIFObject();
					break;
				case MemberName.Title:
					this.m_title = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.TitleAngle:
					this.m_titleAngle = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ToolTip:
					this.m_toolTip = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Interval:
					this.m_interval = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.IntervalType:
					this.m_intervalType = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.IntervalOffset:
					this.m_intervalOffset = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.IntervalOffsetType:
					this.m_intervalOffsetType = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.StripWidth:
					this.m_stripWidth = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.StripWidthType:
					this.m_stripWidthType = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.TextOrientation:
					this.m_textOrientation = (ExpressionInfo)reader.ReadRIFObject();
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
				this.m_id = base.m_chart.GenerateActionOwnerID();
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartStripLine;
		}
	}
}
