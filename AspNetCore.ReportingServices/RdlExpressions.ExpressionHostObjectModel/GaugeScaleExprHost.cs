using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class GaugeScaleExprHost : StyleExprHost
	{
		[CLSCompliant(false)]
		protected IList<ScaleRangeExprHost> m_scaleRangesHostsRemotable;

		[CLSCompliant(false)]
		protected IList<CustomLabelExprHost> m_customLabelsHostsRemotable;

		public GaugeInputValueExprHost MaximumValueHost;

		public GaugeInputValueExprHost MinimumValueHost;

		public GaugeTickMarksExprHost GaugeMajorTickMarksHost;

		public GaugeTickMarksExprHost GaugeMinorTickMarksHost;

		public ScalePinExprHost MaximumPinHost;

		public ScalePinExprHost MinimumPinHost;

		public ScaleLabelsExprHost ScaleLabelsHost;

		public ActionInfoExprHost ActionInfoHost;

		internal IList<ScaleRangeExprHost> ScaleRangesHostsRemotable
		{
			get
			{
				return this.m_scaleRangesHostsRemotable;
			}
		}

		internal IList<CustomLabelExprHost> CustomLabelsHostsRemotable
		{
			get
			{
				return this.m_customLabelsHostsRemotable;
			}
		}

		public virtual object IntervalExpr
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

		public virtual object LogarithmicExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object LogarithmicBaseExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object MultiplierExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object ReversedExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object TickMarksOnTopExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object ToolTipExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object HiddenExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object WidthExpr
		{
			get
			{
				return null;
			}
		}
	}
}
