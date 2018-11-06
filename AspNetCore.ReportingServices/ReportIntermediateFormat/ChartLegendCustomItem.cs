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
	internal sealed class ChartLegendCustomItem : ChartStyleContainer, IPersistable, IActionOwner
	{
		private string m_name;

		private int m_exprHostID;

		private Action m_action;

		private ChartMarker m_marker;

		private ExpressionInfo m_separator;

		private ExpressionInfo m_separatorColor;

		private ExpressionInfo m_toolTip;

		private List<ChartLegendCustomItemCell> m_chartLegendCustomItemCells;

		private int m_id;

		[NonSerialized]
		private List<string> m_fieldsUsedInValueExpression;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ChartLegendCustomItem.GetDeclaration();

		[NonSerialized]
		private ChartLegendCustomItemExprHost m_exprHost;

		internal string LegendCustomItemName
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

		internal ChartLegendCustomItemExprHost ExprHost
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

		internal ChartMarker Marker
		{
			get
			{
				return this.m_marker;
			}
			set
			{
				this.m_marker = value;
			}
		}

		internal ExpressionInfo Separator
		{
			get
			{
				return this.m_separator;
			}
			set
			{
				this.m_separator = value;
			}
		}

		internal ExpressionInfo SeparatorColor
		{
			get
			{
				return this.m_separatorColor;
			}
			set
			{
				this.m_separatorColor = value;
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

		internal List<ChartLegendCustomItemCell> LegendCustomItemCells
		{
			get
			{
				return this.m_chartLegendCustomItemCells;
			}
			set
			{
				this.m_chartLegendCustomItemCells = value;
			}
		}

		internal ChartLegendCustomItem()
		{
		}

		internal ChartLegendCustomItem(Chart chart, int id)
			: base(chart)
		{
			this.m_id = id;
		}

		internal void SetExprHost(ChartLegendCustomItemExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			this.m_exprHost = exprHost;
			if (this.m_marker != null && this.m_exprHost.ChartMarkerHost != null)
			{
				this.m_marker.SetExprHost(this.m_exprHost.ChartMarkerHost, reportObjectModel);
			}
			if (this.m_action != null && this.m_exprHost.ActionInfoHost != null)
			{
				this.m_action.SetExprHost(this.m_exprHost.ActionInfoHost, reportObjectModel);
			}
			IList<ChartLegendCustomItemCellExprHost> chartLegendCustomItemCellsHostsRemotable = this.m_exprHost.ChartLegendCustomItemCellsHostsRemotable;
			if (this.m_chartLegendCustomItemCells != null && chartLegendCustomItemCellsHostsRemotable != null)
			{
				for (int i = 0; i < this.m_chartLegendCustomItemCells.Count; i++)
				{
					ChartLegendCustomItemCell chartLegendCustomItemCell = this.m_chartLegendCustomItemCells[i];
					if (chartLegendCustomItemCell != null && chartLegendCustomItemCell.ExpressionHostID > -1)
					{
						chartLegendCustomItemCell.SetExprHost(chartLegendCustomItemCellsHostsRemotable[chartLegendCustomItemCell.ExpressionHostID], reportObjectModel);
					}
				}
			}
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ChartLegendCustomItemStart(this.m_name);
			base.Initialize(context);
			if (this.m_action != null)
			{
				this.m_action.Initialize(context);
			}
			if (this.m_marker != null)
			{
				this.m_marker.Initialize(context);
			}
			if (this.m_separator != null)
			{
				this.m_separator.Initialize("Separator", context);
				context.ExprHostBuilder.ChartLegendCustomItemSeparator(this.m_separator);
			}
			if (this.m_separatorColor != null)
			{
				this.m_separatorColor.Initialize("SeparatorColor", context);
				context.ExprHostBuilder.ChartLegendCustomItemSeparatorColor(this.m_separatorColor);
			}
			if (this.m_toolTip != null)
			{
				this.m_toolTip.Initialize("ToolTip", context);
				context.ExprHostBuilder.ChartLegendCustomItemToolTip(this.m_toolTip);
			}
			if (this.m_chartLegendCustomItemCells != null)
			{
				for (int i = 0; i < this.m_chartLegendCustomItemCells.Count; i++)
				{
					this.m_chartLegendCustomItemCells[i].Initialize(context, i);
				}
			}
			this.m_exprHostID = context.ExprHostBuilder.ChartLegendCustomItemEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ChartLegendCustomItem chartLegendCustomItem = (ChartLegendCustomItem)base.PublishClone(context);
			if (this.m_action != null)
			{
				chartLegendCustomItem.m_action = (Action)this.m_action.PublishClone(context);
			}
			if (this.m_marker != null)
			{
				chartLegendCustomItem.m_marker = (ChartMarker)this.m_marker.PublishClone(context);
			}
			if (this.m_separator != null)
			{
				chartLegendCustomItem.m_separator = (ExpressionInfo)this.m_separator.PublishClone(context);
			}
			if (this.m_separatorColor != null)
			{
				chartLegendCustomItem.m_separatorColor = (ExpressionInfo)this.m_separatorColor.PublishClone(context);
			}
			if (this.m_toolTip != null)
			{
				chartLegendCustomItem.m_toolTip = (ExpressionInfo)this.m_toolTip.PublishClone(context);
			}
			if (this.m_chartLegendCustomItemCells != null)
			{
				chartLegendCustomItem.m_chartLegendCustomItemCells = new List<ChartLegendCustomItemCell>(this.m_chartLegendCustomItemCells.Count);
				{
					foreach (ChartLegendCustomItemCell chartLegendCustomItemCell in this.m_chartLegendCustomItemCells)
					{
						chartLegendCustomItem.m_chartLegendCustomItemCells.Add((ChartLegendCustomItemCell)chartLegendCustomItemCell.PublishClone(context));
					}
					return chartLegendCustomItem;
				}
			}
			return chartLegendCustomItem;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.Action, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Action));
			list.Add(new MemberInfo(MemberName.Marker, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartMarker));
			list.Add(new MemberInfo(MemberName.Separator, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.SeparatorColor, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ToolTip, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ChartLegendCustomItemCells, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartLegendCustomItemCell));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.ID, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartLegendCustomItem, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartStyleContainer, list);
		}

		internal ChartSeparators EvaluateSeparator(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartSeparator(context.ReportRuntime.EvaluateChartLegendCustomItemSeparatorExpression(this, base.m_chart.Name), context.ReportRuntime);
		}

		internal string EvaluateSeparatorColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendCustomItemSeparatorColorExpression(this, base.m_chart.Name);
		}

		internal string EvaluateToolTip(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendCustomItemToolTipExpression(this, base.m_chart.Name);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(ChartLegendCustomItem.m_Declaration);
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
				case MemberName.Marker:
					writer.Write(this.m_marker);
					break;
				case MemberName.Separator:
					writer.Write(this.m_separator);
					break;
				case MemberName.SeparatorColor:
					writer.Write(this.m_separatorColor);
					break;
				case MemberName.ToolTip:
					writer.Write(this.m_toolTip);
					break;
				case MemberName.ChartLegendCustomItemCells:
					writer.Write(this.m_chartLegendCustomItemCells);
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
			reader.RegisterDeclaration(ChartLegendCustomItem.m_Declaration);
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
				case MemberName.Marker:
					this.m_marker = (ChartMarker)reader.ReadRIFObject();
					break;
				case MemberName.Separator:
					this.m_separator = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.SeparatorColor:
					this.m_separatorColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ToolTip:
					this.m_toolTip = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ChartLegendCustomItemCells:
					this.m_chartLegendCustomItemCells = reader.ReadGenericListOfRIFObjects<ChartLegendCustomItemCell>();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartLegendCustomItem;
		}
	}
}
