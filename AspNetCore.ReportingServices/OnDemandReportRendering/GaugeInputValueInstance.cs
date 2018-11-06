using AspNetCore.ReportingServices.RdlExpressions;
using System;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class GaugeInputValueInstance : BaseInstance
	{
		private GaugeInputValue m_defObject;

		private VariantResult? m_valueResult;

		private GaugeInputValueFormulas? m_formula;

		private double? m_minPercent;

		private double? m_maxPercent;

		private double? m_multiplier;

		private double? m_addConstant;

		public object Value
		{
			get
			{
				this.EnsureValueIsEvaluated();
				return this.m_valueResult.Value.Value;
			}
		}

		internal TypeCode ValueTypeCode
		{
			get
			{
				this.EnsureValueIsEvaluated();
				return this.m_valueResult.Value.TypeCode;
			}
		}

		internal bool ErrorOccured
		{
			get
			{
				this.EnsureValueIsEvaluated();
				return this.m_valueResult.Value.ErrorOccurred;
			}
		}

		public GaugeInputValueFormulas Formula
		{
			get
			{
				if (!this.m_formula.HasValue)
				{
					this.m_formula = this.m_defObject.GaugeInputValueDef.EvaluateFormula(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_formula.Value;
			}
		}

		public double MinPercent
		{
			get
			{
				if (!this.m_minPercent.HasValue)
				{
					this.m_minPercent = this.m_defObject.GaugeInputValueDef.EvaluateMinPercent(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_minPercent.Value;
			}
		}

		public double MaxPercent
		{
			get
			{
				if (!this.m_maxPercent.HasValue)
				{
					this.m_maxPercent = this.m_defObject.GaugeInputValueDef.EvaluateMaxPercent(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_maxPercent.Value;
			}
		}

		public double Multiplier
		{
			get
			{
				if (!this.m_multiplier.HasValue)
				{
					this.m_multiplier = this.m_defObject.GaugeInputValueDef.EvaluateMultiplier(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_multiplier.Value;
			}
		}

		public double AddConstant
		{
			get
			{
				if (!this.m_addConstant.HasValue)
				{
					this.m_addConstant = this.m_defObject.GaugeInputValueDef.EvaluateAddConstant(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_addConstant.Value;
			}
		}

		internal GaugeInputValueInstance(GaugeInputValue defObject)
			: base((GaugeCell)defObject.GaugePanelDef.RowCollection.GetIfExists(0).GetIfExists(0))
		{
			this.m_defObject = defObject;
		}

		private void EnsureValueIsEvaluated()
		{
			if (!this.m_valueResult.HasValue)
			{
				this.m_valueResult = this.m_defObject.GaugeInputValueDef.EvaluateValue(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
			}
		}

		protected override void ResetInstanceCache()
		{
			this.m_valueResult = null;
			this.m_formula = null;
			this.m_minPercent = null;
			this.m_maxPercent = null;
			this.m_multiplier = null;
			this.m_addConstant = null;
		}
	}
}
