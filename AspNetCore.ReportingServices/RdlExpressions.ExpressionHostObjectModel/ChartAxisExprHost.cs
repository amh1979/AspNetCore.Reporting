using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ChartAxisExprHost : StyleExprHost
	{
		public ChartAxisTitleExprHost TitleHost;

		public ChartGridLinesExprHost MajorGridLinesHost;

		public ChartGridLinesExprHost MinorGridLinesHost;

		[CLSCompliant(false)]
		protected IList<DataValueExprHost> m_customPropertyHostsRemotable;

		[CLSCompliant(false)]
		protected IList<ChartStripLineExprHost> m_stripLinesHostsRemotable;

		public ChartTickMarksExprHost MajorTickMarksHost;

		public ChartTickMarksExprHost MinorTickMarksHost;

		public ChartAxisScaleBreakExprHost AxisScaleBreakHost;

		internal IList<DataValueExprHost> CustomPropertyHostsRemotable
		{
			get
			{
				return this.m_customPropertyHostsRemotable;
			}
		}

		internal IList<ChartStripLineExprHost> ChartStripLinesHostsRemotable
		{
			get
			{
				return this.m_stripLinesHostsRemotable;
			}
		}

		public virtual object AxisMinExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object AxisMaxExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object AxisCrossAtExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object VisibleExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object MarginExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object IntervalExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object IntervalTypeExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object IntervalOffsetExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object IntervalOffsetTypeExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object MarksAlwaysAtPlotEdgeExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object ReverseExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object LocationExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object InterlacedExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object InterlacedColorExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object LogScaleExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object LogBaseExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object HideLabelsExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object AngleExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object ArrowsExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object PreventFontShrinkExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object PreventFontGrowExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object PreventLabelOffsetExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object PreventWordWrapExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object AllowLabelRotationExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object IncludeZeroExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object LabelsAutoFitDisabledExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object MinFontSizeExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object MaxFontSizeExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object OffsetLabelsExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object HideEndLabelsExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object VariableAutoIntervalExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object LabelIntervalExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object LabelIntervalTypeExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object LabelIntervalOffsetExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object LabelIntervalOffsetTypeExpr
		{
			get
			{
				return null;
			}
		}
	}
}
