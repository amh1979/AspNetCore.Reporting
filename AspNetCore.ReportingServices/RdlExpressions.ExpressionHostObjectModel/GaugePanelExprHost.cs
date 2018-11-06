using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class GaugePanelExprHost : DataRegionExprHost<GaugeMemberExprHost, GaugeCellExprHost>
	{
		[CLSCompliant(false)]
		protected IList<LinearGaugeExprHost> m_linearGaugesHostsRemotable;

		[CLSCompliant(false)]
		protected IList<RadialGaugeExprHost> m_radialGaugesHostsRemotable;

		[CLSCompliant(false)]
		protected IList<NumericIndicatorExprHost> m_numericIndicatorsHostsRemotable;

		[CLSCompliant(false)]
		protected IList<StateIndicatorExprHost> m_stateIndicatorsHostsRemotable;

		[CLSCompliant(false)]
		protected IList<GaugeImageExprHost> m_gaugeImagesHostsRemotable;

		[CLSCompliant(false)]
		protected IList<GaugeLabelExprHost> m_gaugeLabelsHostsRemotable;

		public BackFrameExprHost BackFrameHost;

		public TopImageExprHost TopImageHost;

		internal IList<LinearGaugeExprHost> LinearGaugesHostsRemotable
		{
			get
			{
				return this.m_linearGaugesHostsRemotable;
			}
		}

		internal IList<RadialGaugeExprHost> RadialGaugesHostsRemotable
		{
			get
			{
				return this.m_radialGaugesHostsRemotable;
			}
		}

		internal IList<NumericIndicatorExprHost> NumericIndicatorsHostsRemotable
		{
			get
			{
				return this.m_numericIndicatorsHostsRemotable;
			}
		}

		internal IList<StateIndicatorExprHost> StateIndicatorsHostsRemotable
		{
			get
			{
				return this.m_stateIndicatorsHostsRemotable;
			}
		}

		internal IList<GaugeImageExprHost> GaugeImagesHostsRemotable
		{
			get
			{
				return this.m_gaugeImagesHostsRemotable;
			}
		}

		internal IList<GaugeLabelExprHost> GaugeLabelsHostsRemotable
		{
			get
			{
				return this.m_gaugeLabelsHostsRemotable;
			}
		}

		public virtual object AntiAliasingExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object AutoLayoutExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object ShadowIntensityExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object TextAntiAliasingQualityExpr
		{
			get
			{
				return null;
			}
		}
	}
}
