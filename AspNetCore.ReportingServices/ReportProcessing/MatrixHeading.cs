using AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using AspNetCore.ReportingServices.ReportRendering;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class MatrixHeading : PivotHeading, IPageBreakItem
	{
		private string m_size;

		private double m_sizeValue;

		private ReportItemCollection m_reportItems;

		private bool m_owcGroupExpression;

		[NonSerialized]
		private bool m_inFirstPage = true;

		[NonSerialized]
		private BoolList m_firstHeadingInstances;

		[NonSerialized]
		private MatrixDynamicGroupExprHost m_exprHost;

		[NonSerialized]
		private bool m_startHidden;

		[NonSerialized]
		private bool m_inOutermostSubtotalCell;

		[NonSerialized]
		private string m_renderingModelID;

		[NonSerialized]
		private string[] m_renderingModelIDs;

		[NonSerialized]
		private ReportSize m_sizeForRendering;

		internal new MatrixHeading SubHeading
		{
			get
			{
				return (MatrixHeading)base.m_innerHierarchy;
			}
			set
			{
				base.m_innerHierarchy = value;
			}
		}

		internal string Size
		{
			get
			{
				return this.m_size;
			}
			set
			{
				this.m_size = value;
			}
		}

		internal double SizeValue
		{
			get
			{
				return this.m_sizeValue;
			}
			set
			{
				this.m_sizeValue = value;
			}
		}

		internal bool StartHidden
		{
			get
			{
				return this.m_startHidden;
			}
			set
			{
				this.m_startHidden = value;
			}
		}

		internal ReportItem ReportItem
		{
			get
			{
				if (this.m_reportItems != null && 0 < this.m_reportItems.Count)
				{
					return this.m_reportItems[0];
				}
				return null;
			}
		}

		internal ReportItemCollection ReportItems
		{
			get
			{
				return this.m_reportItems;
			}
			set
			{
				this.m_reportItems = value;
			}
		}

		internal bool OwcGroupExpression
		{
			get
			{
				return this.m_owcGroupExpression;
			}
			set
			{
				this.m_owcGroupExpression = value;
			}
		}

		internal bool InFirstPage
		{
			get
			{
				return this.m_inFirstPage;
			}
			set
			{
				this.m_inFirstPage = value;
			}
		}

		internal BoolList FirstHeadingInstances
		{
			get
			{
				return this.m_firstHeadingInstances;
			}
			set
			{
				this.m_firstHeadingInstances = value;
			}
		}

		internal string RenderingModelID
		{
			get
			{
				return this.m_renderingModelID;
			}
			set
			{
				this.m_renderingModelID = value;
			}
		}

		internal string[] RenderingModelIDs
		{
			get
			{
				return this.m_renderingModelIDs;
			}
			set
			{
				this.m_renderingModelIDs = value;
			}
		}

		internal ReportSize SizeForRendering
		{
			get
			{
				return this.m_sizeForRendering;
			}
			set
			{
				this.m_sizeForRendering = value;
			}
		}

		internal MatrixDynamicGroupExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal bool InOutermostSubtotalCell
		{
			get
			{
				return this.m_inOutermostSubtotalCell;
			}
			set
			{
				this.m_inOutermostSubtotalCell = value;
			}
		}

		internal MatrixHeading()
		{
		}

		internal MatrixHeading(int id, int idForReportItems, Matrix matrixDef)
			: base(id, matrixDef)
		{
			this.m_reportItems = new ReportItemCollection(idForReportItems, false);
		}

		internal int DynamicInitialize(bool column, int level, InitializationContext context, ref double cornerSize)
		{
			base.m_level = level;
			base.m_isColumn = column;
			this.m_sizeValue = context.ValidateSize(ref this.m_size, column ? "Height" : "Width");
			cornerSize = Math.Round(cornerSize + this.m_sizeValue, Validator.DecimalPrecision);
			if (base.m_grouping == null)
			{
				if (this.SubHeading != null)
				{
					context.RegisterReportItems(this.m_reportItems);
					this.SubHeading.DynamicInitialize(column, ++level, context, ref cornerSize);
					context.UnRegisterReportItems(this.m_reportItems);
				}
				return 1;
			}
			context.ExprHostBuilder.MatrixDynamicGroupStart(base.m_grouping.Name);
			if (base.m_subtotal != null)
			{
				base.m_subtotal.RegisterReportItems(context);
				base.m_subtotal.Initialize(context);
			}
			context.Location |= LocationFlags.InGrouping;
			context.RegisterGroupingScope(base.m_grouping.Name, base.m_grouping.SimpleGroupExpressions, base.m_grouping.Aggregates, base.m_grouping.PostSortAggregates, base.m_grouping.RecursiveAggregates, base.m_grouping, true);
			ObjectType objectType = context.ObjectType;
			string objectName = context.ObjectName;
			context.ObjectType = ObjectType.Grouping;
			context.ObjectName = base.m_grouping.Name;
			base.Initialize(context);
			context.RegisterReportItems(this.m_reportItems);
			if (base.m_visibility != null)
			{
				base.m_visibility.Initialize(context, true, false);
			}
			if (this.SubHeading != null)
			{
				base.m_subtotalSpan = this.SubHeading.DynamicInitialize(column, ++level, context, ref cornerSize);
			}
			else
			{
				base.m_subtotalSpan = 1;
			}
			this.m_reportItems.Initialize(context, true);
			if (base.m_visibility != null)
			{
				base.m_visibility.UnRegisterReceiver(context);
			}
			context.UnRegisterReportItems(this.m_reportItems);
			context.ObjectType = objectType;
			context.ObjectName = objectName;
			context.UnRegisterGroupingScope(base.m_grouping.Name, true);
			if (base.m_subtotal != null)
			{
				base.m_subtotal.UnregisterReportItems(context);
			}
			base.m_hasExprHost = context.ExprHostBuilder.MatrixDynamicGroupEnd(column);
			return base.m_subtotalSpan + 1;
		}

		internal void DynamicRegisterReceiver(InitializationContext context)
		{
			if (base.m_grouping == null)
			{
				if (this.SubHeading != null)
				{
					context.RegisterReportItems(this.m_reportItems);
					this.SubHeading.DynamicRegisterReceiver(context);
					context.UnRegisterReportItems(this.m_reportItems);
				}
			}
			else
			{
				if (base.m_subtotal != null)
				{
					base.m_subtotal.RegisterReceiver(context);
				}
				context.RegisterReportItems(this.m_reportItems);
				if (base.m_visibility != null)
				{
					base.m_visibility.RegisterReceiver(context, true);
				}
				if (this.SubHeading != null)
				{
					this.SubHeading.DynamicRegisterReceiver(context);
				}
				this.m_reportItems.RegisterReceiver(context);
				if (base.m_visibility != null)
				{
					base.m_visibility.UnRegisterReceiver(context);
				}
				context.UnRegisterReportItems(this.m_reportItems);
			}
		}

		internal int StaticInitialize(InitializationContext context)
		{
			if (base.m_grouping != null)
			{
				int num = 1;
				if (this.SubHeading != null)
				{
					context.Location |= LocationFlags.InGrouping;
					context.RegisterGroupingScope(base.m_grouping.Name, base.m_grouping.SimpleGroupExpressions, base.m_aggregates, base.m_postSortAggregates, base.m_recursiveAggregates, base.m_grouping, true);
					context.RegisterReportItems(this.m_reportItems);
					num = this.SubHeading.StaticInitialize(context);
					context.UnRegisterReportItems(this.m_reportItems);
					context.UnRegisterGroupingScope(base.m_grouping.Name, true);
				}
				return num + 1;
			}
			context.RegisterReportItems(this.m_reportItems);
			if (this.SubHeading != null)
			{
				base.m_subtotalSpan = this.SubHeading.StaticInitialize(context);
			}
			else
			{
				base.m_subtotalSpan = 1;
			}
			this.m_reportItems.Initialize(context, true);
			context.UnRegisterReportItems(this.m_reportItems);
			return 0;
		}

		internal void StaticRegisterReceiver(InitializationContext context)
		{
			if (base.m_grouping != null)
			{
				if (this.SubHeading != null)
				{
					context.RegisterReportItems(this.m_reportItems);
					this.SubHeading.StaticRegisterReceiver(context);
					context.UnRegisterReportItems(this.m_reportItems);
				}
			}
			else
			{
				context.RegisterReportItems(this.m_reportItems);
				if (this.SubHeading != null)
				{
					this.SubHeading.StaticRegisterReceiver(context);
				}
				this.m_reportItems.RegisterReceiver(context);
				context.UnRegisterReportItems(this.m_reportItems);
			}
		}

		bool IPageBreakItem.IgnorePageBreaks()
		{
			return base.IgnorePageBreaks(base.m_visibility);
		}

		internal void SetExprHost(MatrixDynamicGroupExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null && base.HasExprHost);
			this.m_exprHost = exprHost;
			this.m_exprHost.SetReportObjectModel(reportObjectModel);
			base.ReportHierarchyNodeSetExprHost(this.m_exprHost, reportObjectModel);
		}

		internal ReportItem GetContent(out bool computed)
		{
			ReportItem result = null;
			computed = false;
			if (this.m_reportItems != null && 0 < this.m_reportItems.Count)
			{
				int num = default(int);
				this.m_reportItems.GetReportItem(0, out computed, out num, out result);
			}
			return result;
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Size, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.SizeValue, Token.Double));
			memberInfoList.Add(new MemberInfo(MemberName.ReportItems, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemCollection));
			memberInfoList.Add(new MemberInfo(MemberName.OwcGroupExpression, Token.Boolean));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.PivotHeading, memberInfoList);
		}
	}
}
