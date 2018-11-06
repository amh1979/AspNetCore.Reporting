using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class StateIndicatorExprHost : GaugePanelItemExprHost
	{
		public GaugeInputValueExprHost GaugeInputValueHost;

		public IndicatorImageExprHost IndicatorImageHost;

		[CLSCompliant(false)]
		protected IList<IndicatorStateExprHost> m_indicatorStatesHostsRemotable;

		public GaugeInputValueExprHost MaximumValueHost;

		public GaugeInputValueExprHost MinimumValueHost;

		public virtual object IndicatorStyleExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object ScaleFactorExpr
		{
			get
			{
				return null;
			}
		}

		[CLSCompliant(false)]
		public IList<IndicatorStateExprHost> IndicatorStatesHostsRemotable
		{
			get
			{
				return this.m_indicatorStatesHostsRemotable;
			}
		}

		public virtual object ResizeModeExpr
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

		public virtual object TransformationTypeExpr
		{
			get
			{
				return null;
			}
		}
	}
}
