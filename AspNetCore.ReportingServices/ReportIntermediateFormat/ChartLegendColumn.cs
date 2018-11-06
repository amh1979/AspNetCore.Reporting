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
	internal sealed class ChartLegendColumn : ChartStyleContainer, IPersistable, IActionOwner
	{
		private string m_name;

		private int m_exprHostID;

		private Action m_action;

		private ExpressionInfo m_columnType;

		private ExpressionInfo m_value;

		private ExpressionInfo m_toolTip;

		private ExpressionInfo m_minimumWidth;

		private ExpressionInfo m_maximumWidth;

		private ExpressionInfo m_seriesSymbolWidth;

		private ExpressionInfo m_seriesSymbolHeight;

		private ChartLegendColumnHeader m_header;

		private int m_id;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ChartLegendColumn.GetDeclaration();

		[NonSerialized]
		private ChartLegendColumnExprHost m_exprHost;

		[NonSerialized]
		private List<string> m_fieldsUsedInValueExpression;

		internal string LegendColumnName
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

		internal ChartLegendColumnExprHost ExprHost
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

		internal ExpressionInfo ColumnType
		{
			get
			{
				return this.m_columnType;
			}
			set
			{
				this.m_columnType = value;
			}
		}

		internal ExpressionInfo Value
		{
			get
			{
				return this.m_value;
			}
			set
			{
				this.m_value = value;
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

		internal ExpressionInfo MinimumWidth
		{
			get
			{
				return this.m_minimumWidth;
			}
			set
			{
				this.m_minimumWidth = value;
			}
		}

		internal ExpressionInfo MaximumWidth
		{
			get
			{
				return this.m_maximumWidth;
			}
			set
			{
				this.m_maximumWidth = value;
			}
		}

		internal ExpressionInfo SeriesSymbolWidth
		{
			get
			{
				return this.m_seriesSymbolWidth;
			}
			set
			{
				this.m_seriesSymbolWidth = value;
			}
		}

		internal ExpressionInfo SeriesSymbolHeight
		{
			get
			{
				return this.m_seriesSymbolHeight;
			}
			set
			{
				this.m_seriesSymbolHeight = value;
			}
		}

		internal ChartLegendColumnHeader Header
		{
			get
			{
				return this.m_header;
			}
			set
			{
				this.m_header = value;
			}
		}

		internal ChartLegendColumn()
		{
		}

		internal ChartLegendColumn(Chart chart, int id)
			: base(chart)
		{
			this.m_id = id;
		}

		internal void SetExprHost(ChartLegendColumnExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			this.m_exprHost = exprHost;
			if (this.m_header != null && this.m_exprHost.HeaderHost != null)
			{
				this.m_header.SetExprHost(this.m_exprHost.HeaderHost, reportObjectModel);
			}
			if (this.m_action != null && this.m_exprHost.ActionInfoHost != null)
			{
				this.m_action.SetExprHost(this.m_exprHost.ActionInfoHost, reportObjectModel);
			}
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ChartLegendColumnStart(this.m_name);
			base.Initialize(context);
			if (this.m_action != null)
			{
				this.m_action.Initialize(context);
			}
			if (this.m_columnType != null)
			{
				this.m_columnType.Initialize("ColumnType", context);
				context.ExprHostBuilder.ChartLegendColumnColumnType(this.m_columnType);
			}
			if (this.m_value != null)
			{
				this.m_value.Initialize("Value", context);
				context.ExprHostBuilder.ChartLegendColumnValue(this.m_value);
			}
			if (this.m_toolTip != null)
			{
				this.m_toolTip.Initialize("ToolTip", context);
				context.ExprHostBuilder.ChartLegendColumnToolTip(this.m_toolTip);
			}
			if (this.m_minimumWidth != null)
			{
				this.m_minimumWidth.Initialize("MinimumWidth", context);
				context.ExprHostBuilder.ChartLegendColumnMinimumWidth(this.m_minimumWidth);
			}
			if (this.m_maximumWidth != null)
			{
				this.m_maximumWidth.Initialize("MaximumWidth", context);
				context.ExprHostBuilder.ChartLegendColumnMaximumWidth(this.m_maximumWidth);
			}
			if (this.m_seriesSymbolWidth != null)
			{
				this.m_seriesSymbolWidth.Initialize("SeriesSymbolWidth", context);
				context.ExprHostBuilder.ChartLegendColumnSeriesSymbolWidth(this.m_seriesSymbolWidth);
			}
			if (this.m_seriesSymbolHeight != null)
			{
				this.m_seriesSymbolHeight.Initialize("SeriesSymbolHeight", context);
				context.ExprHostBuilder.ChartLegendColumnSeriesSymbolHeight(this.m_seriesSymbolHeight);
			}
			if (this.m_header != null)
			{
				this.m_header.Initialize(context);
			}
			this.m_exprHostID = context.ExprHostBuilder.ChartLegendColumnEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ChartLegendColumn chartLegendColumn = (ChartLegendColumn)base.PublishClone(context);
			if (this.m_action != null)
			{
				chartLegendColumn.m_action = (Action)this.m_action.PublishClone(context);
			}
			if (this.m_columnType != null)
			{
				chartLegendColumn.m_columnType = (ExpressionInfo)this.m_columnType.PublishClone(context);
			}
			if (this.m_value != null)
			{
				chartLegendColumn.m_value = (ExpressionInfo)this.m_value.PublishClone(context);
			}
			if (this.m_toolTip != null)
			{
				chartLegendColumn.m_toolTip = (ExpressionInfo)this.m_toolTip.PublishClone(context);
			}
			if (this.m_minimumWidth != null)
			{
				chartLegendColumn.m_minimumWidth = (ExpressionInfo)this.m_minimumWidth.PublishClone(context);
			}
			if (this.m_maximumWidth != null)
			{
				chartLegendColumn.m_maximumWidth = (ExpressionInfo)this.m_maximumWidth.PublishClone(context);
			}
			if (this.m_seriesSymbolWidth != null)
			{
				chartLegendColumn.m_seriesSymbolWidth = (ExpressionInfo)this.m_seriesSymbolWidth.PublishClone(context);
			}
			if (this.m_seriesSymbolHeight != null)
			{
				chartLegendColumn.m_seriesSymbolHeight = (ExpressionInfo)this.m_seriesSymbolHeight.PublishClone(context);
			}
			if (this.m_header != null)
			{
				chartLegendColumn.m_header = (ChartLegendColumnHeader)this.m_header.PublishClone(context);
			}
			return chartLegendColumn;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.Action, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Action));
			list.Add(new MemberInfo(MemberName.ColumnType, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Value, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ToolTip, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MinimumWidth, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MaximumWidth, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.SeriesSymbolWidth, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.SeriesSymbolHeight, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Header, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartLegendColumnHeader));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.ID, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartLegendColumn, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartStyleContainer, list);
		}

		internal ChartColumnType EvaluateColumnType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartColumnType(context.ReportRuntime.EvaluateChartLegendColumnColumnTypeExpression(this, base.m_chart.Name), context.ReportRuntime);
		}

		internal string EvaluateValue(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendColumnValueExpression(this, base.m_chart.Name);
		}

		internal string EvaluateToolTip(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendColumnToolTipExpression(this, base.m_chart.Name);
		}

		internal string EvaluateMinimumWidth(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendColumnMinimumWidthExpression(this, base.m_chart.Name);
		}

		internal string EvaluateMaximumWidth(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendColumnMaximumWidthExpression(this, base.m_chart.Name);
		}

		internal int EvaluateSeriesSymbolWidth(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendColumnSeriesSymbolWidthExpression(this, base.m_chart.Name);
		}

		internal int EvaluateSeriesSymbolHeight(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendColumnSeriesSymbolHeightExpression(this, base.m_chart.Name);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(ChartLegendColumn.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Name:
					writer.Write(this.m_name);
					break;
				case MemberName.Action:
					writer.Write(this.m_action);
					break;
				case MemberName.ColumnType:
					writer.Write(this.m_columnType);
					break;
				case MemberName.Value:
					writer.Write(this.m_value);
					break;
				case MemberName.ToolTip:
					writer.Write(this.m_toolTip);
					break;
				case MemberName.MinimumWidth:
					writer.Write(this.m_minimumWidth);
					break;
				case MemberName.MaximumWidth:
					writer.Write(this.m_maximumWidth);
					break;
				case MemberName.SeriesSymbolWidth:
					writer.Write(this.m_seriesSymbolWidth);
					break;
				case MemberName.SeriesSymbolHeight:
					writer.Write(this.m_seriesSymbolHeight);
					break;
				case MemberName.Header:
					writer.Write(this.m_header);
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
			reader.RegisterDeclaration(ChartLegendColumn.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Name:
					this.m_name = reader.ReadString();
					break;
				case MemberName.Action:
					this.m_action = (Action)reader.ReadRIFObject();
					break;
				case MemberName.ColumnType:
					this.m_columnType = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Value:
					this.m_value = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ToolTip:
					this.m_toolTip = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MinimumWidth:
					this.m_minimumWidth = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MaximumWidth:
					this.m_maximumWidth = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.SeriesSymbolWidth:
					this.m_seriesSymbolWidth = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.SeriesSymbolHeight:
					this.m_seriesSymbolHeight = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Header:
					this.m_header = (ChartLegendColumnHeader)reader.ReadRIFObject();
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
				this.m_id = base.m_chart.GenerateActionOwnerID();
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartLegendColumn;
		}
	}
}
