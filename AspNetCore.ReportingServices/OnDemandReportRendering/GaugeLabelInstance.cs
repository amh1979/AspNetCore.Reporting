using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class GaugeLabelInstance : GaugePanelItemInstance
	{
		private GaugeLabel m_defObject;

		private VariantResult? m_textResult;

		private string m_formattedText;

		private double? m_angle;

		private GaugeResizeModes? m_resizeMode;

		private ReportSize m_textShadowOffset;

		private bool? m_useFontPercent;

		public string Text
		{
			get
			{
				if (this.m_formattedText == null)
				{
					this.EnsureTextEvaluated();
					this.m_formattedText = this.RifGaugeLabel.FormatText(this.m_textResult.Value, this.OdpContext);
				}
				return this.m_formattedText;
			}
		}

		internal object OriginalValue
		{
			get
			{
				this.EnsureTextEvaluated();
				return this.m_textResult.Value.Value;
			}
		}

		internal TypeCode TypeCode
		{
			get
			{
				this.EnsureTextEvaluated();
				return this.m_textResult.Value.TypeCode;
			}
		}

		public double Angle
		{
			get
			{
				if (!this.m_angle.HasValue)
				{
					this.m_angle = this.RifGaugeLabel.EvaluateAngle(this.ReportScopeInstance, this.OdpContext);
				}
				return this.m_angle.Value;
			}
		}

		public GaugeResizeModes ResizeMode
		{
			get
			{
				if (!this.m_resizeMode.HasValue)
				{
					this.m_resizeMode = this.RifGaugeLabel.EvaluateResizeMode(this.ReportScopeInstance, this.OdpContext);
				}
				return this.m_resizeMode.Value;
			}
		}

		public ReportSize TextShadowOffset
		{
			get
			{
				if (this.m_textShadowOffset == null)
				{
					this.m_textShadowOffset = new ReportSize(this.RifGaugeLabel.EvaluateTextShadowOffset(this.ReportScopeInstance, this.OdpContext));
				}
				return this.m_textShadowOffset;
			}
		}

		public bool UseFontPercent
		{
			get
			{
				if (!this.m_useFontPercent.HasValue)
				{
					this.m_useFontPercent = this.RifGaugeLabel.EvaluateUseFontPercent(this.ReportScopeInstance, this.OdpContext);
				}
				return this.m_useFontPercent.Value;
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeLabel RifGaugeLabel
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeLabel)this.m_defObject.GaugePanelItemDef;
			}
		}

		private OnDemandProcessingContext OdpContext
		{
			get
			{
				return this.m_defObject.GaugePanelDef.RenderingContext.OdpContext;
			}
		}

		internal GaugeLabelInstance(GaugeLabel defObject)
			: base(defObject)
		{
			this.m_defObject = defObject;
		}

		private void EnsureTextEvaluated()
		{
			if (!this.m_textResult.HasValue)
			{
				this.m_textResult = this.RifGaugeLabel.EvaluateText(this.ReportScopeInstance, this.OdpContext);
			}
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_textResult = null;
			this.m_formattedText = null;
			this.m_angle = null;
			this.m_resizeMode = null;
			this.m_textShadowOffset = null;
			this.m_useFontPercent = null;
		}
	}
}
