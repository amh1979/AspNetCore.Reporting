using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ChartExprHost : DataRegionExprHost<ChartMemberExprHost, ChartDataPointExprHost>
	{
		[CLSCompliant(false)]
		protected IList<ChartSeriesExprHost> m_seriesCollectionHostsRemotable;

		[CLSCompliant(false)]
		protected IList<ChartDerivedSeriesExprHost> m_derivedSeriesCollectionHostsRemotable;

		[CLSCompliant(false)]
		protected IList<ChartAreaExprHost> m_chartAreasHostsRemotable;

		[CLSCompliant(false)]
		protected IList<ChartTitleExprHost> m_titlesHostsRemotable;

		[CLSCompliant(false)]
		protected IList<ChartLegendExprHost> m_legendsHostsRemotable;

		[CLSCompliant(false)]
		protected IList<DataValueExprHost> m_codeParametersHostsRemotable;

		public ChartTitleExprHost NoDataMessageHost;

		public ChartBorderSkinExprHost BorderSkinHost;

		[CLSCompliant(false)]
		protected IList<ChartCustomPaletteColorExprHost> m_customPaletteColorHostsRemotable;

		internal IList<ChartSeriesExprHost> SeriesCollectionHostsRemotable
		{
			get
			{
				return this.m_seriesCollectionHostsRemotable;
			}
		}

		internal IList<ChartDerivedSeriesExprHost> ChartDerivedSeriesCollectionHostsRemotable
		{
			get
			{
				return this.m_derivedSeriesCollectionHostsRemotable;
			}
		}

		internal IList<ChartAreaExprHost> ChartAreasHostsRemotable
		{
			get
			{
				return this.m_chartAreasHostsRemotable;
			}
		}

		internal IList<ChartTitleExprHost> TitlesHostsRemotable
		{
			get
			{
				return this.m_titlesHostsRemotable;
			}
		}

		internal IList<ChartLegendExprHost> LegendsHostsRemotable
		{
			get
			{
				return this.m_legendsHostsRemotable;
			}
		}

		internal IList<DataValueExprHost> CodeParametersHostsRemotable
		{
			get
			{
				return this.m_codeParametersHostsRemotable;
			}
		}

		public virtual object DynamicHeightExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object DynamicWidthExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object PaletteExpr
		{
			get
			{
				return null;
			}
		}

		internal IList<ChartCustomPaletteColorExprHost> CustomPaletteColorHostsRemotable
		{
			get
			{
				return this.m_customPaletteColorHostsRemotable;
			}
		}

		public virtual object PaletteHatchBehaviorExpr
		{
			get
			{
				return null;
			}
		}
	}
}
