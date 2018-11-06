using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class NumericIndicatorExprHost : GaugePanelItemExprHost
	{
		public GaugeInputValueExprHost GaugeInputValueHost;

		[CLSCompliant(false)]
		protected IList<NumericIndicatorRangeExprHost> m_numericIndicatorRangesHostsRemotable;

		public GaugeInputValueExprHost MinimumValueHost;

		public GaugeInputValueExprHost MaximumValueHost;

		[CLSCompliant(false)]
		public IList<NumericIndicatorRangeExprHost> NumericIndicatorRangesHostsRemotable
		{
			get
			{
				return this.m_numericIndicatorRangesHostsRemotable;
			}
		}

		public virtual object DecimalDigitColorExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object DigitColorExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object UseFontPercentExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object DecimalDigitsExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object DigitsExpr
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

		public virtual object NonNumericStringExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object OutOfRangeStringExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object ResizeModeExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object ShowDecimalPointExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object ShowLeadingZerosExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object IndicatorStyleExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object ShowSignExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object SnappingEnabledExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object SnappingIntervalExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object LedDimColorExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object SeparatorWidthExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object SeparatorColorExpr
		{
			get
			{
				return null;
			}
		}
	}
}
