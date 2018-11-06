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
	internal sealed class ChartLegendCustomItemCell : ChartStyleContainer, IPersistable, IActionOwner
	{
		private string m_name;

		private int m_exprHostID;

		private Action m_action;

		private ExpressionInfo m_cellType;

		private ExpressionInfo m_text;

		private ExpressionInfo m_cellSpan;

		private ExpressionInfo m_toolTip;

		private ExpressionInfo m_imageWidth;

		private ExpressionInfo m_imageHeight;

		private ExpressionInfo m_symbolHeight;

		private ExpressionInfo m_symbolWidth;

		private ExpressionInfo m_alignment;

		private ExpressionInfo m_topMargin;

		private ExpressionInfo m_bottomMargin;

		private ExpressionInfo m_leftMargin;

		private ExpressionInfo m_rightMargin;

		private int m_id;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ChartLegendCustomItemCell.GetDeclaration();

		[NonSerialized]
		private ChartLegendCustomItemCellExprHost m_exprHost;

		[NonSerialized]
		private List<string> m_fieldsUsedInValueExpression;

		internal string LegendCustomItemCellName
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

		internal ChartLegendCustomItemCellExprHost ExprHost
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

		internal ExpressionInfo CellType
		{
			get
			{
				return this.m_cellType;
			}
			set
			{
				this.m_cellType = value;
			}
		}

		internal ExpressionInfo Text
		{
			get
			{
				return this.m_text;
			}
			set
			{
				this.m_text = value;
			}
		}

		internal ExpressionInfo CellSpan
		{
			get
			{
				return this.m_cellSpan;
			}
			set
			{
				this.m_cellSpan = value;
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

		internal ExpressionInfo ImageWidth
		{
			get
			{
				return this.m_imageWidth;
			}
			set
			{
				this.m_imageWidth = value;
			}
		}

		internal ExpressionInfo ImageHeight
		{
			get
			{
				return this.m_imageHeight;
			}
			set
			{
				this.m_imageHeight = value;
			}
		}

		internal ExpressionInfo SymbolHeight
		{
			get
			{
				return this.m_symbolHeight;
			}
			set
			{
				this.m_symbolHeight = value;
			}
		}

		internal ExpressionInfo SymbolWidth
		{
			get
			{
				return this.m_symbolWidth;
			}
			set
			{
				this.m_symbolWidth = value;
			}
		}

		internal ExpressionInfo Alignment
		{
			get
			{
				return this.m_alignment;
			}
			set
			{
				this.m_alignment = value;
			}
		}

		internal ExpressionInfo TopMargin
		{
			get
			{
				return this.m_topMargin;
			}
			set
			{
				this.m_topMargin = value;
			}
		}

		internal ExpressionInfo BottomMargin
		{
			get
			{
				return this.m_bottomMargin;
			}
			set
			{
				this.m_bottomMargin = value;
			}
		}

		internal ExpressionInfo LeftMargin
		{
			get
			{
				return this.m_leftMargin;
			}
			set
			{
				this.m_leftMargin = value;
			}
		}

		internal ExpressionInfo RightMargin
		{
			get
			{
				return this.m_rightMargin;
			}
			set
			{
				this.m_rightMargin = value;
			}
		}

		internal ChartLegendCustomItemCell()
		{
		}

		internal ChartLegendCustomItemCell(Chart chart, int id)
			: base(chart)
		{
			this.m_id = id;
		}

		internal void SetExprHost(ChartLegendCustomItemCellExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			this.m_exprHost = exprHost;
			if (this.m_action != null && this.m_exprHost.ActionInfoHost != null)
			{
				this.m_action.SetExprHost(this.m_exprHost.ActionInfoHost, reportObjectModel);
			}
		}

		internal void Initialize(InitializationContext context, int index)
		{
			context.ExprHostBuilder.ChartLegendCustomItemCellStart(this.m_name);
			base.Initialize(context);
			if (this.m_action != null)
			{
				this.m_action.Initialize(context);
			}
			if (this.m_cellType != null)
			{
				this.m_cellType.Initialize("CellType", context);
				context.ExprHostBuilder.ChartLegendCustomItemCellCellType(this.m_cellType);
			}
			if (this.m_text != null)
			{
				this.m_text.Initialize("Text", context);
				context.ExprHostBuilder.ChartLegendCustomItemCellText(this.m_text);
			}
			if (this.m_cellSpan != null)
			{
				this.m_cellSpan.Initialize("CellSpan", context);
				context.ExprHostBuilder.ChartLegendCustomItemCellCellSpan(this.m_cellSpan);
			}
			if (this.m_toolTip != null)
			{
				this.m_toolTip.Initialize("ToolTip", context);
				context.ExprHostBuilder.ChartLegendCustomItemCellToolTip(this.m_toolTip);
			}
			if (this.m_imageWidth != null)
			{
				this.m_imageWidth.Initialize("ImageWidth", context);
				context.ExprHostBuilder.ChartLegendCustomItemCellImageWidth(this.m_imageWidth);
			}
			if (this.m_imageHeight != null)
			{
				this.m_imageHeight.Initialize("ImageHeight", context);
				context.ExprHostBuilder.ChartLegendCustomItemCellImageHeight(this.m_imageHeight);
			}
			if (this.m_symbolHeight != null)
			{
				this.m_symbolHeight.Initialize("SymbolHeight", context);
				context.ExprHostBuilder.ChartLegendCustomItemCellSymbolHeight(this.m_symbolHeight);
			}
			if (this.m_symbolWidth != null)
			{
				this.m_symbolWidth.Initialize("SymbolWidth", context);
				context.ExprHostBuilder.ChartLegendCustomItemCellSymbolWidth(this.m_symbolWidth);
			}
			if (this.m_alignment != null)
			{
				this.m_alignment.Initialize("Alignment", context);
				context.ExprHostBuilder.ChartLegendCustomItemCellAlignment(this.m_alignment);
			}
			if (this.m_topMargin != null)
			{
				this.m_topMargin.Initialize("TopMargin", context);
				context.ExprHostBuilder.ChartLegendCustomItemCellTopMargin(this.m_topMargin);
			}
			if (this.m_bottomMargin != null)
			{
				this.m_bottomMargin.Initialize("BottomMargin", context);
				context.ExprHostBuilder.ChartLegendCustomItemCellBottomMargin(this.m_bottomMargin);
			}
			if (this.m_leftMargin != null)
			{
				this.m_leftMargin.Initialize("LeftMargin", context);
				context.ExprHostBuilder.ChartLegendCustomItemCellLeftMargin(this.m_leftMargin);
			}
			if (this.m_rightMargin != null)
			{
				this.m_rightMargin.Initialize("RightMargin", context);
				context.ExprHostBuilder.ChartLegendCustomItemCellRightMargin(this.m_rightMargin);
			}
			this.m_exprHostID = context.ExprHostBuilder.ChartLegendCustomItemCellEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ChartLegendCustomItemCell chartLegendCustomItemCell = (ChartLegendCustomItemCell)base.PublishClone(context);
			if (this.m_action != null)
			{
				chartLegendCustomItemCell.m_action = (Action)this.m_action.PublishClone(context);
			}
			if (this.m_cellType != null)
			{
				chartLegendCustomItemCell.m_cellType = (ExpressionInfo)this.m_cellType.PublishClone(context);
			}
			if (this.m_text != null)
			{
				chartLegendCustomItemCell.m_text = (ExpressionInfo)this.m_text.PublishClone(context);
			}
			if (this.m_cellSpan != null)
			{
				chartLegendCustomItemCell.m_cellSpan = (ExpressionInfo)this.m_cellSpan.PublishClone(context);
			}
			if (this.m_toolTip != null)
			{
				chartLegendCustomItemCell.m_toolTip = (ExpressionInfo)this.m_toolTip.PublishClone(context);
			}
			if (this.m_imageWidth != null)
			{
				chartLegendCustomItemCell.m_imageWidth = (ExpressionInfo)this.m_imageWidth.PublishClone(context);
			}
			if (this.m_imageHeight != null)
			{
				chartLegendCustomItemCell.m_imageHeight = (ExpressionInfo)this.m_imageHeight.PublishClone(context);
			}
			if (this.m_symbolHeight != null)
			{
				chartLegendCustomItemCell.m_symbolHeight = (ExpressionInfo)this.m_symbolHeight.PublishClone(context);
			}
			if (this.m_symbolWidth != null)
			{
				chartLegendCustomItemCell.m_symbolWidth = (ExpressionInfo)this.m_symbolWidth.PublishClone(context);
			}
			if (this.m_alignment != null)
			{
				chartLegendCustomItemCell.m_alignment = (ExpressionInfo)this.m_alignment.PublishClone(context);
			}
			if (this.m_topMargin != null)
			{
				chartLegendCustomItemCell.m_topMargin = (ExpressionInfo)this.m_topMargin.PublishClone(context);
			}
			if (this.m_bottomMargin != null)
			{
				chartLegendCustomItemCell.m_bottomMargin = (ExpressionInfo)this.m_bottomMargin.PublishClone(context);
			}
			if (this.m_leftMargin != null)
			{
				chartLegendCustomItemCell.m_leftMargin = (ExpressionInfo)this.m_leftMargin.PublishClone(context);
			}
			if (this.m_rightMargin != null)
			{
				chartLegendCustomItemCell.m_rightMargin = (ExpressionInfo)this.m_rightMargin.PublishClone(context);
			}
			return chartLegendCustomItemCell;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.Action, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Action));
			list.Add(new MemberInfo(MemberName.CellType, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Text, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.CellSpan, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ToolTip, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ImageWidth, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ImageHeight, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.SymbolHeight, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.SymbolWidth, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Alignment, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.TopMargin, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.BottomMargin, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.LeftMargin, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.RightMargin, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.ID, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartLegendCustomItemCell, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartStyleContainer, list);
		}

		internal ChartCellType EvaluateCellType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartCellType(context.ReportRuntime.EvaluateChartLegendCustomItemCellCellTypeExpression(this, base.m_chart.Name), context.ReportRuntime);
		}

		internal string EvaluateText(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendCustomItemCellTextExpression(this, base.m_chart.Name);
		}

		internal int EvaluateCellSpan(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendCustomItemCellCellSpanExpression(this, base.m_chart.Name);
		}

		internal string EvaluateToolTip(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendCustomItemCellToolTipExpression(this, base.m_chart.Name);
		}

		internal int EvaluateImageWidth(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendCustomItemCellImageWidthExpression(this, base.m_chart.Name);
		}

		internal int EvaluateImageHeight(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendCustomItemCellImageHeightExpression(this, base.m_chart.Name);
		}

		internal int EvaluateSymbolHeight(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendCustomItemCellSymbolHeightExpression(this, base.m_chart.Name);
		}

		internal int EvaluateSymbolWidth(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendCustomItemCellSymbolWidthExpression(this, base.m_chart.Name);
		}

		internal ChartCellAlignment EvaluateAlignment(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartCellAlignment(context.ReportRuntime.EvaluateChartLegendCustomItemCellAlignmentExpression(this, base.m_chart.Name), context.ReportRuntime);
		}

		internal int EvaluateTopMargin(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendCustomItemCellTopMarginExpression(this, base.m_chart.Name);
		}

		internal int EvaluateBottomMargin(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendCustomItemCellBottomMarginExpression(this, base.m_chart.Name);
		}

		internal int EvaluateLeftMargin(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendCustomItemCellLeftMarginExpression(this, base.m_chart.Name);
		}

		internal int EvaluateRightMargin(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendCustomItemCellRightMarginExpression(this, base.m_chart.Name);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(ChartLegendCustomItemCell.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Name:
					writer.Write(this.m_name);
					break;
				case MemberName.ExprHostID:
					writer.Write(this.m_exprHostID);
					break;
				case MemberName.Action:
					writer.Write(this.m_action);
					break;
				case MemberName.CellType:
					writer.Write(this.m_cellType);
					break;
				case MemberName.Text:
					writer.Write(this.m_text);
					break;
				case MemberName.CellSpan:
					writer.Write(this.m_cellSpan);
					break;
				case MemberName.ToolTip:
					writer.Write(this.m_toolTip);
					break;
				case MemberName.ImageWidth:
					writer.Write(this.m_imageWidth);
					break;
				case MemberName.ImageHeight:
					writer.Write(this.m_imageHeight);
					break;
				case MemberName.SymbolHeight:
					writer.Write(this.m_symbolHeight);
					break;
				case MemberName.SymbolWidth:
					writer.Write(this.m_symbolWidth);
					break;
				case MemberName.Alignment:
					writer.Write(this.m_alignment);
					break;
				case MemberName.TopMargin:
					writer.Write(this.m_topMargin);
					break;
				case MemberName.BottomMargin:
					writer.Write(this.m_bottomMargin);
					break;
				case MemberName.LeftMargin:
					writer.Write(this.m_leftMargin);
					break;
				case MemberName.RightMargin:
					writer.Write(this.m_rightMargin);
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
			reader.RegisterDeclaration(ChartLegendCustomItemCell.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Name:
					this.m_name = reader.ReadString();
					break;
				case MemberName.ExprHostID:
					this.m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.Action:
					this.m_action = (Action)reader.ReadRIFObject();
					break;
				case MemberName.CellType:
					this.m_cellType = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Text:
					this.m_text = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.CellSpan:
					this.m_cellSpan = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ToolTip:
					this.m_toolTip = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ImageWidth:
					this.m_imageWidth = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ImageHeight:
					this.m_imageHeight = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.SymbolHeight:
					this.m_symbolHeight = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.SymbolWidth:
					this.m_symbolWidth = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Alignment:
					this.m_alignment = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.TopMargin:
					this.m_topMargin = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.BottomMargin:
					this.m_bottomMargin = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.LeftMargin:
					this.m_leftMargin = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.RightMargin:
					this.m_rightMargin = (ExpressionInfo)reader.ReadRIFObject();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartLegendCustomItemCell;
		}
	}
}
