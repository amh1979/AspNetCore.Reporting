using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ChartDataPointExprHost : CellExprHost
	{
		public StyleExprHost StyleHost;

		public ChartDataPointInLegendExprHost DataPointInLegendHost;

		public ChartDataLabelExprHost DataLabelHost;

		public ChartMarkerExprHost ChartMarkerHost;

		public ActionInfoExprHost ActionInfoHost;

		[CLSCompliant(false)]
		protected IList<DataValueExprHost> m_customPropertyHostsRemotable;

		public virtual object DataLabelValueExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object DataLabelVisibleExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object DataPointValuesXExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object DataPointValuesYExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object DataPointValuesSizeExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object DataPointValuesHighExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object DataPointValuesLowExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object DataPointValuesStartExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object DataPointValuesEndExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object DataPointValuesMeanExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object DataPointValuesMedianExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object DataPointValuesHighlightXExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object DataPointValuesHighlightYExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object DataPointValuesHighlightSizeExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object DataPointValuesFormatXExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object DataPointValuesFormatYExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object DataPointValuesFormatSizeExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object DataPointValuesCurrencyLanguageXExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object DataPointValuesCurrencyLanguageYExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object DataPointValuesCurrencyLanguageSizeExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object AxisLabelExpr
		{
			get
			{
				return null;
			}
		}

		internal IList<DataValueExprHost> CustomPropertyHostsRemotable
		{
			get
			{
				return this.m_customPropertyHostsRemotable;
			}
		}

		public virtual object ToolTipExpr
		{
			get
			{
				return null;
			}
		}
	}
}
