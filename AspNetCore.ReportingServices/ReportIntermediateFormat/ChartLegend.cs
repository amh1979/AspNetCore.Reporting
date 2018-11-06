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
	internal sealed class ChartLegend : ChartStyleContainer, IPersistable
	{
		private string m_name;

		private ExpressionInfo m_hidden;

		private ExpressionInfo m_position;

		private ExpressionInfo m_layout;

		private List<ChartLegendCustomItem> m_chartLegendCustomItems;

		private string m_dockToChartArea;

		private ExpressionInfo m_dockOutsideChartArea;

		private ChartLegendTitle m_chartLegendTitle;

		private ExpressionInfo m_autoFitTextDisabled;

		private ExpressionInfo m_minFontSize;

		private ExpressionInfo m_headerSeparator;

		private ExpressionInfo m_headerSeparatorColor;

		private ExpressionInfo m_columnSeparator;

		private ExpressionInfo m_columnSeparatorColor;

		private ExpressionInfo m_columnSpacing;

		private ExpressionInfo m_interlacedRows;

		private ExpressionInfo m_interlacedRowsColor;

		private ExpressionInfo m_equallySpacedItems;

		private ExpressionInfo m_reversed;

		private ExpressionInfo m_maxAutoSize;

		private ExpressionInfo m_textWrapThreshold;

		private List<ChartLegendColumn> m_chartLegendColumns;

		private int m_exprHostID;

		private ChartElementPosition m_chartElementPosition;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ChartLegend.GetDeclaration();

		[NonSerialized]
		private ChartLegendExprHost m_exprHost;

		internal string LegendName
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

		internal ExpressionInfo Position
		{
			get
			{
				return this.m_position;
			}
			set
			{
				this.m_position = value;
			}
		}

		internal ExpressionInfo Layout
		{
			get
			{
				return this.m_layout;
			}
			set
			{
				this.m_layout = value;
			}
		}

		internal string DockToChartArea
		{
			get
			{
				return this.m_dockToChartArea;
			}
			set
			{
				this.m_dockToChartArea = value;
			}
		}

		internal ExpressionInfo DockOutsideChartArea
		{
			get
			{
				return this.m_dockOutsideChartArea;
			}
			set
			{
				this.m_dockOutsideChartArea = value;
			}
		}

		internal ChartLegendTitle LegendTitle
		{
			get
			{
				return this.m_chartLegendTitle;
			}
			set
			{
				this.m_chartLegendTitle = value;
			}
		}

		internal ExpressionInfo AutoFitTextDisabled
		{
			get
			{
				return this.m_autoFitTextDisabled;
			}
			set
			{
				this.m_autoFitTextDisabled = value;
			}
		}

		internal ExpressionInfo MinFontSize
		{
			get
			{
				return this.m_minFontSize;
			}
			set
			{
				this.m_minFontSize = value;
			}
		}

		internal ExpressionInfo HeaderSeparator
		{
			get
			{
				return this.m_headerSeparator;
			}
			set
			{
				this.m_headerSeparator = value;
			}
		}

		internal ExpressionInfo HeaderSeparatorColor
		{
			get
			{
				return this.m_headerSeparatorColor;
			}
			set
			{
				this.m_headerSeparatorColor = value;
			}
		}

		internal ExpressionInfo ColumnSeparator
		{
			get
			{
				return this.m_columnSeparator;
			}
			set
			{
				this.m_columnSeparator = value;
			}
		}

		internal ExpressionInfo ColumnSeparatorColor
		{
			get
			{
				return this.m_columnSeparatorColor;
			}
			set
			{
				this.m_columnSeparatorColor = value;
			}
		}

		internal ExpressionInfo ColumnSpacing
		{
			get
			{
				return this.m_columnSpacing;
			}
			set
			{
				this.m_columnSpacing = value;
			}
		}

		internal ExpressionInfo InterlacedRows
		{
			get
			{
				return this.m_interlacedRows;
			}
			set
			{
				this.m_interlacedRows = value;
			}
		}

		internal ExpressionInfo InterlacedRowsColor
		{
			get
			{
				return this.m_interlacedRowsColor;
			}
			set
			{
				this.m_interlacedRowsColor = value;
			}
		}

		internal ExpressionInfo EquallySpacedItems
		{
			get
			{
				return this.m_equallySpacedItems;
			}
			set
			{
				this.m_equallySpacedItems = value;
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

		internal ExpressionInfo MaxAutoSize
		{
			get
			{
				return this.m_maxAutoSize;
			}
			set
			{
				this.m_maxAutoSize = value;
			}
		}

		internal ExpressionInfo TextWrapThreshold
		{
			get
			{
				return this.m_textWrapThreshold;
			}
			set
			{
				this.m_textWrapThreshold = value;
			}
		}

		internal List<ChartLegendCustomItem> LegendCustomItems
		{
			get
			{
				return this.m_chartLegendCustomItems;
			}
			set
			{
				this.m_chartLegendCustomItems = value;
			}
		}

		internal List<ChartLegendColumn> LegendColumns
		{
			get
			{
				return this.m_chartLegendColumns;
			}
			set
			{
				this.m_chartLegendColumns = value;
			}
		}

		internal ChartElementPosition ChartElementPosition
		{
			get
			{
				return this.m_chartElementPosition;
			}
			set
			{
				this.m_chartElementPosition = value;
			}
		}

		internal ChartLegendExprHost ExprHost
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

		internal ChartLegend()
		{
		}

		internal ChartLegend(Chart chart)
			: base(chart)
		{
		}

		internal void SetExprHost(ChartLegendExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			this.m_exprHost = exprHost;
			if (this.m_chartLegendTitle != null && exprHost.TitleExprHost != null)
			{
				this.m_chartLegendTitle.SetExprHost(exprHost.TitleExprHost, reportObjectModel);
			}
			IList<ChartLegendCustomItemExprHost> chartLegendCustomItemsHostsRemotable = this.m_exprHost.ChartLegendCustomItemsHostsRemotable;
			if (this.m_chartLegendCustomItems != null && chartLegendCustomItemsHostsRemotable != null)
			{
				for (int i = 0; i < this.m_chartLegendCustomItems.Count; i++)
				{
					ChartLegendCustomItem chartLegendCustomItem = this.m_chartLegendCustomItems[i];
					if (chartLegendCustomItem != null && chartLegendCustomItem.ExpressionHostID > -1)
					{
						chartLegendCustomItem.SetExprHost(chartLegendCustomItemsHostsRemotable[chartLegendCustomItem.ExpressionHostID], reportObjectModel);
					}
				}
			}
			IList<ChartLegendColumnExprHost> chartLegendColumnsHostsRemotable = this.m_exprHost.ChartLegendColumnsHostsRemotable;
			if (this.m_chartLegendColumns != null && chartLegendColumnsHostsRemotable != null)
			{
				for (int j = 0; j < this.m_chartLegendColumns.Count; j++)
				{
					ChartLegendColumn chartLegendColumn = this.m_chartLegendColumns[j];
					if (chartLegendColumn != null && chartLegendColumn.ExpressionHostID > -1)
					{
						chartLegendColumn.SetExprHost(chartLegendColumnsHostsRemotable[chartLegendColumn.ExpressionHostID], reportObjectModel);
					}
				}
			}
			if (this.m_chartElementPosition != null && this.m_exprHost.ChartElementPositionHost != null)
			{
				this.m_chartElementPosition.SetExprHost(this.m_exprHost.ChartElementPositionHost, reportObjectModel);
			}
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ChartLegendStart(this.m_name);
			base.Initialize(context);
			string dockToChartArea = this.m_dockToChartArea;
			if (this.m_position != null)
			{
				this.m_position.Initialize("Position", context);
				context.ExprHostBuilder.ChartLegendPosition(this.m_position);
			}
			if (this.m_layout != null)
			{
				this.m_layout.Initialize("Layout", context);
				context.ExprHostBuilder.ChartLegendLayout(this.m_layout);
			}
			if (this.m_hidden != null)
			{
				this.m_hidden.Initialize("Hidden", context);
				context.ExprHostBuilder.ChartLegendHidden(this.m_hidden);
			}
			if (this.m_dockOutsideChartArea != null)
			{
				this.m_dockOutsideChartArea.Initialize("DockOutsideChartArea", context);
				context.ExprHostBuilder.ChartLegendDockOutsideChartArea(this.m_dockOutsideChartArea);
			}
			if (this.m_chartLegendTitle != null)
			{
				this.m_chartLegendTitle.Initialize(context);
			}
			if (this.m_autoFitTextDisabled != null)
			{
				this.m_autoFitTextDisabled.Initialize("AutoFitTextDisabled", context);
				context.ExprHostBuilder.ChartLegendAutoFitTextDisabled(this.m_autoFitTextDisabled);
			}
			if (this.m_minFontSize != null)
			{
				this.m_minFontSize.Initialize("MinFontSize", context);
				context.ExprHostBuilder.ChartLegendMinFontSize(this.m_minFontSize);
			}
			if (this.m_headerSeparator != null)
			{
				this.m_headerSeparator.Initialize("HeaderSeparator", context);
				context.ExprHostBuilder.ChartLegendHeaderSeparator(this.m_headerSeparator);
			}
			if (this.m_headerSeparatorColor != null)
			{
				this.m_headerSeparatorColor.Initialize("HeaderSeparatorColor", context);
				context.ExprHostBuilder.ChartLegendHeaderSeparatorColor(this.m_headerSeparatorColor);
			}
			if (this.m_columnSeparator != null)
			{
				this.m_columnSeparator.Initialize("ColumnSeparator", context);
				context.ExprHostBuilder.ChartLegendColumnSeparator(this.m_columnSeparator);
			}
			if (this.m_columnSeparatorColor != null)
			{
				this.m_columnSeparatorColor.Initialize("ColumnSeparatorColor", context);
				context.ExprHostBuilder.ChartLegendColumnSeparatorColor(this.m_columnSeparatorColor);
			}
			if (this.m_columnSpacing != null)
			{
				this.m_columnSpacing.Initialize("ColumnSpacing", context);
				context.ExprHostBuilder.ChartLegendColumnSpacing(this.m_columnSpacing);
			}
			if (this.m_interlacedRows != null)
			{
				this.m_interlacedRows.Initialize("InterlacedRows", context);
				context.ExprHostBuilder.ChartLegendInterlacedRows(this.m_interlacedRows);
			}
			if (this.m_interlacedRowsColor != null)
			{
				this.m_interlacedRowsColor.Initialize("InterlacedRowsColor", context);
				context.ExprHostBuilder.ChartLegendInterlacedRowsColor(this.m_interlacedRowsColor);
			}
			if (this.m_equallySpacedItems != null)
			{
				this.m_equallySpacedItems.Initialize("EquallySpacedItems", context);
				context.ExprHostBuilder.ChartLegendEquallySpacedItems(this.m_equallySpacedItems);
			}
			if (this.m_reversed != null)
			{
				this.m_reversed.Initialize("Reversed", context);
				context.ExprHostBuilder.ChartLegendReversed(this.m_reversed);
			}
			if (this.m_maxAutoSize != null)
			{
				this.m_maxAutoSize.Initialize("MaxAutoSize", context);
				context.ExprHostBuilder.ChartLegendMaxAutoSize(this.m_maxAutoSize);
			}
			if (this.m_textWrapThreshold != null)
			{
				this.m_textWrapThreshold.Initialize("TextWrapThreshold", context);
				context.ExprHostBuilder.ChartLegendTextWrapThreshold(this.m_textWrapThreshold);
			}
			if (this.m_chartLegendCustomItems != null)
			{
				for (int i = 0; i < this.m_chartLegendCustomItems.Count; i++)
				{
					this.m_chartLegendCustomItems[i].Initialize(context);
				}
			}
			if (this.m_chartLegendColumns != null)
			{
				for (int j = 0; j < this.m_chartLegendColumns.Count; j++)
				{
					this.m_chartLegendColumns[j].Initialize(context);
				}
			}
			if (this.m_chartElementPosition != null)
			{
				this.m_chartElementPosition.Initialize(context);
			}
			this.m_exprHostID = context.ExprHostBuilder.ChartLegendEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ChartLegend chartLegend = (ChartLegend)base.PublishClone(context);
			if (this.m_position != null)
			{
				chartLegend.m_position = (ExpressionInfo)this.m_position.PublishClone(context);
			}
			if (this.m_layout != null)
			{
				chartLegend.m_layout = (ExpressionInfo)this.m_layout.PublishClone(context);
			}
			if (this.m_hidden != null)
			{
				chartLegend.m_hidden = (ExpressionInfo)this.m_hidden.PublishClone(context);
			}
			if (this.m_dockOutsideChartArea != null)
			{
				chartLegend.m_dockOutsideChartArea = (ExpressionInfo)this.m_dockOutsideChartArea.PublishClone(context);
			}
			if (this.m_chartLegendTitle != null)
			{
				chartLegend.m_chartLegendTitle = (ChartLegendTitle)this.m_chartLegendTitle.PublishClone(context);
			}
			if (this.m_autoFitTextDisabled != null)
			{
				chartLegend.m_autoFitTextDisabled = (ExpressionInfo)this.m_autoFitTextDisabled.PublishClone(context);
			}
			if (this.m_minFontSize != null)
			{
				chartLegend.m_minFontSize = (ExpressionInfo)this.m_minFontSize.PublishClone(context);
			}
			if (this.m_headerSeparator != null)
			{
				chartLegend.m_headerSeparator = (ExpressionInfo)this.m_headerSeparator.PublishClone(context);
			}
			if (this.m_headerSeparatorColor != null)
			{
				chartLegend.m_headerSeparatorColor = (ExpressionInfo)this.m_headerSeparatorColor.PublishClone(context);
			}
			if (this.m_columnSeparator != null)
			{
				chartLegend.m_columnSeparator = (ExpressionInfo)this.m_columnSeparator.PublishClone(context);
			}
			if (this.m_columnSeparatorColor != null)
			{
				chartLegend.m_columnSeparatorColor = (ExpressionInfo)this.m_columnSeparatorColor.PublishClone(context);
			}
			if (this.m_columnSpacing != null)
			{
				chartLegend.m_columnSpacing = (ExpressionInfo)this.m_columnSpacing.PublishClone(context);
			}
			if (this.m_interlacedRows != null)
			{
				chartLegend.m_interlacedRows = (ExpressionInfo)this.m_interlacedRows.PublishClone(context);
			}
			if (this.m_interlacedRowsColor != null)
			{
				chartLegend.m_interlacedRowsColor = (ExpressionInfo)this.m_interlacedRowsColor.PublishClone(context);
			}
			if (this.m_equallySpacedItems != null)
			{
				chartLegend.m_equallySpacedItems = (ExpressionInfo)this.m_equallySpacedItems.PublishClone(context);
			}
			if (this.m_reversed != null)
			{
				chartLegend.m_reversed = (ExpressionInfo)this.m_reversed.PublishClone(context);
			}
			if (this.m_maxAutoSize != null)
			{
				chartLegend.m_maxAutoSize = (ExpressionInfo)this.m_maxAutoSize.PublishClone(context);
			}
			if (this.m_textWrapThreshold != null)
			{
				chartLegend.m_textWrapThreshold = (ExpressionInfo)this.m_textWrapThreshold.PublishClone(context);
			}
			if (this.m_chartLegendCustomItems != null)
			{
				chartLegend.m_chartLegendCustomItems = new List<ChartLegendCustomItem>(this.m_chartLegendCustomItems.Count);
				foreach (ChartLegendCustomItem chartLegendCustomItem in this.m_chartLegendCustomItems)
				{
					chartLegend.m_chartLegendCustomItems.Add((ChartLegendCustomItem)chartLegendCustomItem.PublishClone(context));
				}
			}
			if (this.m_chartLegendColumns != null)
			{
				chartLegend.m_chartLegendColumns = new List<ChartLegendColumn>(this.m_chartLegendColumns.Count);
				foreach (ChartLegendColumn chartLegendColumn in this.m_chartLegendColumns)
				{
					chartLegend.m_chartLegendColumns.Add((ChartLegendColumn)chartLegendColumn.PublishClone(context));
				}
			}
			if (this.m_chartElementPosition != null)
			{
				chartLegend.m_chartElementPosition = (ChartElementPosition)this.m_chartElementPosition.PublishClone(context);
			}
			return chartLegend;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.Hidden, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Position, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Layout, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DockToChartArea, Token.String));
			list.Add(new MemberInfo(MemberName.DockOutsideChartArea, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ChartLegendTitle, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartLegendTitle));
			list.Add(new MemberInfo(MemberName.AutoFitTextDisabled, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MinFontSize, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.HeaderSeparator, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.HeaderSeparatorColor, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ColumnSeparator, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ColumnSeparatorColor, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ColumnSpacing, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.InterlacedRows, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.InterlacedRowsColor, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.EquallySpacedItems, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Reversed, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MaxAutoSize, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.TextWrapThreshold, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ChartLegendCustomItems, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartLegendCustomItem));
			list.Add(new MemberInfo(MemberName.ChartLegendColumns, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartLegendColumn));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.ChartElementPosition, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartElementPosition));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartLegend, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartStyleContainer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(ChartLegend.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Name:
					writer.Write(this.m_name);
					break;
				case MemberName.Hidden:
					writer.Write(this.m_hidden);
					break;
				case MemberName.Position:
					writer.Write(this.m_position);
					break;
				case MemberName.Layout:
					writer.Write(this.m_layout);
					break;
				case MemberName.DockToChartArea:
					writer.Write(this.m_dockToChartArea);
					break;
				case MemberName.DockOutsideChartArea:
					writer.Write(this.m_dockOutsideChartArea);
					break;
				case MemberName.ChartLegendTitle:
					writer.Write(this.m_chartLegendTitle);
					break;
				case MemberName.AutoFitTextDisabled:
					writer.Write(this.m_autoFitTextDisabled);
					break;
				case MemberName.MinFontSize:
					writer.Write(this.m_minFontSize);
					break;
				case MemberName.HeaderSeparator:
					writer.Write(this.m_headerSeparator);
					break;
				case MemberName.HeaderSeparatorColor:
					writer.Write(this.m_headerSeparatorColor);
					break;
				case MemberName.ColumnSeparator:
					writer.Write(this.m_columnSeparator);
					break;
				case MemberName.ColumnSeparatorColor:
					writer.Write(this.m_columnSeparatorColor);
					break;
				case MemberName.ColumnSpacing:
					writer.Write(this.m_columnSpacing);
					break;
				case MemberName.InterlacedRows:
					writer.Write(this.m_interlacedRows);
					break;
				case MemberName.InterlacedRowsColor:
					writer.Write(this.m_interlacedRowsColor);
					break;
				case MemberName.EquallySpacedItems:
					writer.Write(this.m_equallySpacedItems);
					break;
				case MemberName.Reversed:
					writer.Write(this.m_reversed);
					break;
				case MemberName.MaxAutoSize:
					writer.Write(this.m_maxAutoSize);
					break;
				case MemberName.TextWrapThreshold:
					writer.Write(this.m_textWrapThreshold);
					break;
				case MemberName.ChartLegendColumns:
					writer.Write(this.m_chartLegendColumns);
					break;
				case MemberName.ExprHostID:
					writer.Write(this.m_exprHostID);
					break;
				case MemberName.ChartLegendCustomItems:
					writer.Write(this.m_chartLegendCustomItems);
					break;
				case MemberName.ChartElementPosition:
					writer.Write(this.m_chartElementPosition);
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
			reader.RegisterDeclaration(ChartLegend.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Name:
					this.m_name = reader.ReadString();
					break;
				case MemberName.Hidden:
					this.m_hidden = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Position:
					this.m_position = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Layout:
					this.m_layout = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DockToChartArea:
					this.m_dockToChartArea = reader.ReadString();
					break;
				case MemberName.DockOutsideChartArea:
					this.m_dockOutsideChartArea = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ChartLegendTitle:
					this.m_chartLegendTitle = (ChartLegendTitle)reader.ReadRIFObject();
					break;
				case MemberName.AutoFitTextDisabled:
					this.m_autoFitTextDisabled = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MinFontSize:
					this.m_minFontSize = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.HeaderSeparator:
					this.m_headerSeparator = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.HeaderSeparatorColor:
					this.m_headerSeparatorColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ColumnSeparator:
					this.m_columnSeparator = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ColumnSeparatorColor:
					this.m_columnSeparatorColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ColumnSpacing:
					this.m_columnSpacing = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.InterlacedRows:
					this.m_interlacedRows = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.InterlacedRowsColor:
					this.m_interlacedRowsColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.EquallySpacedItems:
					this.m_equallySpacedItems = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Reversed:
					this.m_reversed = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MaxAutoSize:
					this.m_maxAutoSize = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.TextWrapThreshold:
					this.m_textWrapThreshold = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ExprHostID:
					this.m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.ChartLegendCustomItems:
					this.m_chartLegendCustomItems = reader.ReadGenericListOfRIFObjects<ChartLegendCustomItem>();
					break;
				case MemberName.ChartLegendColumns:
					this.m_chartLegendColumns = reader.ReadGenericListOfRIFObjects<ChartLegendColumn>();
					break;
				case MemberName.ChartElementPosition:
					this.m_chartElementPosition = (ChartElementPosition)reader.ReadRIFObject();
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
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartLegend;
		}

		internal bool EvaluateHidden(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendHiddenExpression(this, base.m_chart.Name, "Hidden");
		}

		internal ChartLegendPositions EvaluatePosition(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartLegendPositions(context.ReportRuntime.EvaluateChartLegendPositionExpression(this, base.m_chart.Name, "Position"), context.ReportRuntime);
		}

		internal ChartLegendLayouts EvaluateLayout(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartLegendLayout(context.ReportRuntime.EvaluateChartLegendLayoutExpression(this, base.m_chart.Name, "Layout"), context.ReportRuntime);
		}

		internal bool EvaluateDockOutsideChartArea(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendDockOutsideChartAreaExpression(this, base.m_chart.Name, "DockOutsideChartArea");
		}

		internal bool EvaluateAutoFitTextDisabled(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendAutoFitTextDisabledExpression(this, base.m_chart.Name, "AutoFitTextDisabled");
		}

		internal string EvaluateMinFontSize(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendMinFontSizeExpression(this, base.m_chart.Name, "MinFontSize");
		}

		internal ChartSeparators EvaluateHeaderSeparator(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartSeparator(context.ReportRuntime.EvaluateChartLegendHeaderSeparatorExpression(this, base.m_chart.Name, "HeaderSeparator"), context.ReportRuntime);
		}

		internal string EvaluateHeaderSeparatorColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendHeaderSeparatorColorExpression(this, base.m_chart.Name, "HeaderSeparatorColor");
		}

		internal ChartSeparators EvaluateColumnSeparator(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartSeparator(context.ReportRuntime.EvaluateChartLegendColumnSeparatorExpression(this, base.m_chart.Name, "ColumnSeparator"), context.ReportRuntime);
		}

		internal string EvaluateColumnSeparatorColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendColumnSeparatorColorExpression(this, base.m_chart.Name, "ColumnSeparatorColor");
		}

		internal int EvaluateColumnSpacing(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendColumnSpacingExpression(this, base.m_chart.Name, "ColumnSpacing");
		}

		internal bool EvaluateInterlacedRows(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendInterlacedRowsExpression(this, base.m_chart.Name, "InterlacedRows");
		}

		internal string EvaluateInterlacedRowsColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendInterlacedRowsColorExpression(this, base.m_chart.Name, "InterlacedRowsColor");
		}

		internal bool EvaluateEquallySpacedItems(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendEquallySpacedItemsExpression(this, base.m_chart.Name, "EquallySpacedItems");
		}

		internal ChartAutoBool EvaluateReversed(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartAutoBool(context.ReportRuntime.EvaluateChartLegendReversedExpression(this, base.m_chart.Name), context.ReportRuntime);
		}

		internal int EvaluateMaxAutoSize(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendMaxAutoSizeExpression(this, base.m_chart.Name, "MaxAutoSize");
		}

		internal int EvaluateTextWrapThreshold(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendTextWrapThresholdExpression(this, base.m_chart.Name, "TextWrapThreshold");
		}
	}
}
