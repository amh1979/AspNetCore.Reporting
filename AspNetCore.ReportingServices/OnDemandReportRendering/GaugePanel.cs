using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class GaugePanel : DataRegion
	{
		private enum CompilationState
		{
			NotCompiled,
			Compiling,
			Compiled
		}

		private GaugeMember m_gaugeMember;

		private GaugeMember m_gaugeRowMember;

		private GaugeRowCollection m_gaugeRowCollection;

		private LinearGaugeCollection m_linearGauges;

		private RadialGaugeCollection m_radialGauges;

		private NumericIndicatorCollection m_numericIndicators;

		private StateIndicatorCollection m_stateIndicators;

		private GaugeImageCollection m_gaugeImages;

		private GaugeLabelCollection m_gaugeLabels;

		private ReportEnumProperty<GaugeAntiAliasings> m_antiAliasing;

		private ReportBoolProperty m_autoLayout;

		private BackFrame m_backFrame;

		private ReportDoubleProperty m_shadowIntensity;

		private ReportEnumProperty<TextAntiAliasingQualities> m_textAntiAliasingQuality;

		private TopImage m_topImage;

		private CompilationState m_compilationState;

		public GaugeMember GaugeMember
		{
			get
			{
				if (this.m_gaugeMember == null)
				{
					this.m_gaugeMember = new GaugeMember(this.ReportScope, this, this, null, this.GaugePanelDef.GaugeMember);
				}
				return this.m_gaugeMember;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel GaugePanelDef
		{
			get
			{
				return base.m_reportItemDef as AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel;
			}
		}

		internal override bool HasDataCells
		{
			get
			{
				return true;
			}
		}

		internal override IDataRegionRowCollection RowCollection
		{
			get
			{
				if (this.m_gaugeRowCollection == null && this.GaugePanelDef.Rows != null)
				{
					this.m_gaugeRowCollection = new GaugeRowCollection(this, (GaugeRowList)this.GaugePanelDef.Rows);
				}
				return this.m_gaugeRowCollection;
			}
		}

		public LinearGaugeCollection LinearGauges
		{
			get
			{
				if (this.m_linearGauges == null && this.GaugePanelDef.LinearGauges != null)
				{
					this.m_linearGauges = new LinearGaugeCollection(this);
				}
				return this.m_linearGauges;
			}
		}

		public RadialGaugeCollection RadialGauges
		{
			get
			{
				if (this.m_radialGauges == null && this.GaugePanelDef.RadialGauges != null)
				{
					this.m_radialGauges = new RadialGaugeCollection(this);
				}
				return this.m_radialGauges;
			}
		}

		public NumericIndicatorCollection NumericIndicators
		{
			get
			{
				if (this.m_numericIndicators == null && this.GaugePanelDef.NumericIndicators != null)
				{
					this.m_numericIndicators = new NumericIndicatorCollection(this);
				}
				return this.m_numericIndicators;
			}
		}

		public StateIndicatorCollection StateIndicators
		{
			get
			{
				if (this.m_stateIndicators == null && this.GaugePanelDef.StateIndicators != null)
				{
					this.m_stateIndicators = new StateIndicatorCollection(this);
				}
				return this.m_stateIndicators;
			}
		}

		public GaugeImageCollection GaugeImages
		{
			get
			{
				if (this.m_gaugeImages == null && this.GaugePanelDef.GaugeImages != null)
				{
					this.m_gaugeImages = new GaugeImageCollection(this);
				}
				return this.m_gaugeImages;
			}
		}

		public GaugeLabelCollection GaugeLabels
		{
			get
			{
				if (this.m_gaugeLabels == null && this.GaugePanelDef.GaugeLabels != null)
				{
					this.m_gaugeLabels = new GaugeLabelCollection(this);
				}
				return this.m_gaugeLabels;
			}
		}

		public ReportEnumProperty<GaugeAntiAliasings> AntiAliasing
		{
			get
			{
				if (this.m_antiAliasing == null && this.GaugePanelDef.AntiAliasing != null)
				{
					this.m_antiAliasing = new ReportEnumProperty<GaugeAntiAliasings>(this.GaugePanelDef.AntiAliasing.IsExpression, this.GaugePanelDef.AntiAliasing.OriginalText, EnumTranslator.TranslateGaugeAntiAliasings(this.GaugePanelDef.AntiAliasing.StringValue, null));
				}
				return this.m_antiAliasing;
			}
		}

		public ReportBoolProperty AutoLayout
		{
			get
			{
				if (this.m_autoLayout == null && this.GaugePanelDef.AutoLayout != null)
				{
					this.m_autoLayout = new ReportBoolProperty(this.GaugePanelDef.AutoLayout);
				}
				return this.m_autoLayout;
			}
		}

		public BackFrame BackFrame
		{
			get
			{
				if (this.m_backFrame == null && this.GaugePanelDef.BackFrame != null)
				{
					this.m_backFrame = new BackFrame(this.GaugePanelDef.BackFrame, this);
				}
				return this.m_backFrame;
			}
		}

		public ReportDoubleProperty ShadowIntensity
		{
			get
			{
				if (this.m_shadowIntensity == null && this.GaugePanelDef.ShadowIntensity != null)
				{
					this.m_shadowIntensity = new ReportDoubleProperty(this.GaugePanelDef.ShadowIntensity);
				}
				return this.m_shadowIntensity;
			}
		}

		public ReportEnumProperty<TextAntiAliasingQualities> TextAntiAliasingQuality
		{
			get
			{
				if (this.m_textAntiAliasingQuality == null && this.GaugePanelDef.TextAntiAliasingQuality != null)
				{
					this.m_textAntiAliasingQuality = new ReportEnumProperty<TextAntiAliasingQualities>(this.GaugePanelDef.TextAntiAliasingQuality.IsExpression, this.GaugePanelDef.TextAntiAliasingQuality.OriginalText, EnumTranslator.TranslateTextAntiAliasingQualities(this.GaugePanelDef.TextAntiAliasingQuality.StringValue, null));
				}
				return this.m_textAntiAliasingQuality;
			}
		}

		public TopImage TopImage
		{
			get
			{
				if (this.m_topImage == null && this.GaugePanelDef.TopImage != null)
				{
					this.m_topImage = new TopImage(this.GaugePanelDef.TopImage, this);
				}
				return this.m_topImage;
			}
		}

		public new GaugePanelInstance Instance
		{
			get
			{
				return (GaugePanelInstance)this.GetOrCreateInstance();
			}
		}

		private bool RequiresCompilation
		{
			get
			{
				if (this.GaugeMember.IsStatic)
				{
					return this.StateIndicators != null;
				}
				return true;
			}
		}

		internal GaugePanel(IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel reportItemDef, RenderingContext renderingContext)
			: base(parentDefinitionPath, indexIntoParentCollectionDef, reportItemDef, renderingContext)
		{
		}

		internal override ReportItemInstance GetOrCreateInstance()
		{
			if (base.m_instance == null)
			{
				base.m_instance = new GaugePanelInstance(this);
			}
			return base.m_instance;
		}

		internal override void SetNewContextChildren()
		{
			if (this.m_gaugeMember != null)
			{
				this.m_gaugeMember.ResetContext();
			}
			if (this.m_gaugeRowMember != null)
			{
				this.m_gaugeRowMember.ResetContext();
			}
			if (this.m_gaugeRowCollection != null)
			{
				this.m_gaugeRowCollection.SetNewContext();
			}
			if (base.m_instance != null)
			{
				base.m_instance.SetNewContext();
			}
			if (this.m_linearGauges != null)
			{
				this.m_linearGauges.SetNewContext();
			}
			if (this.m_radialGauges != null)
			{
				this.m_radialGauges.SetNewContext();
			}
			if (this.m_numericIndicators != null)
			{
				this.m_numericIndicators.SetNewContext();
			}
			if (this.m_stateIndicators != null)
			{
				this.m_stateIndicators.SetNewContext();
			}
			if (this.m_gaugeImages != null)
			{
				this.m_gaugeImages.SetNewContext();
			}
			if (this.m_gaugeLabels != null)
			{
				this.m_gaugeLabels.SetNewContext();
			}
			if (this.m_backFrame != null)
			{
				this.m_backFrame.SetNewContext();
			}
			if (this.m_topImage != null)
			{
				this.m_topImage.SetNewContext();
			}
			this.m_compilationState = CompilationState.NotCompiled;
		}

		internal List<GaugeInputValue> GetGaugeInputValues()
		{
			List<GaugeInputValue> list = new List<GaugeInputValue>();
			if (this.RadialGauges != null)
			{
				foreach (RadialGauge radialGauge in this.RadialGauges)
				{
					if (radialGauge.GaugeScales != null)
					{
						foreach (RadialScale gaugeScale in radialGauge.GaugeScales)
						{
							if (gaugeScale.MaximumValue != null)
							{
								list.Add(gaugeScale.MaximumValue);
							}
							if (gaugeScale.MinimumValue != null)
							{
								list.Add(gaugeScale.MinimumValue);
							}
							if (gaugeScale.GaugePointers != null)
							{
								foreach (RadialPointer gaugePointer in gaugeScale.GaugePointers)
								{
									if (gaugePointer.GaugeInputValue != null)
									{
										list.Add(gaugePointer.GaugeInputValue);
									}
								}
							}
							if (gaugeScale.ScaleRanges != null)
							{
								foreach (ScaleRange scaleRange in gaugeScale.ScaleRanges)
								{
									if (scaleRange.StartValue != null)
									{
										list.Add(scaleRange.StartValue);
									}
									if (scaleRange.EndValue != null)
									{
										list.Add(scaleRange.EndValue);
									}
								}
							}
						}
					}
				}
			}
			if (this.LinearGauges != null)
			{
				foreach (LinearGauge linearGauge in this.LinearGauges)
				{
					if (linearGauge.GaugeScales != null)
					{
						foreach (LinearScale gaugeScale2 in linearGauge.GaugeScales)
						{
							if (gaugeScale2.MaximumValue != null)
							{
								list.Add(gaugeScale2.MaximumValue);
							}
							if (gaugeScale2.MinimumValue != null)
							{
								list.Add(gaugeScale2.MinimumValue);
							}
							if (gaugeScale2.GaugePointers != null)
							{
								foreach (LinearPointer gaugePointer2 in gaugeScale2.GaugePointers)
								{
									if (gaugePointer2.GaugeInputValue != null)
									{
										list.Add(gaugePointer2.GaugeInputValue);
									}
								}
							}
							if (gaugeScale2.ScaleRanges != null)
							{
								foreach (ScaleRange scaleRange2 in gaugeScale2.ScaleRanges)
								{
									if (scaleRange2.StartValue != null)
									{
										list.Add(scaleRange2.StartValue);
									}
									if (scaleRange2.EndValue != null)
									{
										list.Add(scaleRange2.EndValue);
									}
								}
							}
						}
					}
				}
			}
			NumericIndicatorCollection numericIndicator = this.NumericIndicators;
			if (this.StateIndicators != null)
			{
				{
					foreach (StateIndicator stateIndicator in this.StateIndicators)
					{
						if (stateIndicator.GaugeInputValue != null)
						{
							list.Add(stateIndicator.GaugeInputValue);
						}
						if (stateIndicator.MinimumValue != null)
						{
							list.Add(stateIndicator.MinimumValue);
						}
						if (stateIndicator.MaximumValue != null)
						{
							list.Add(stateIndicator.MaximumValue);
						}
						if (stateIndicator.IndicatorStates != null)
						{
							foreach (IndicatorState indicatorState in stateIndicator.IndicatorStates)
							{
								if (indicatorState.StartValue != null)
								{
									list.Add(indicatorState.StartValue);
								}
								if (indicatorState.EndValue != null)
								{
									list.Add(indicatorState.EndValue);
								}
							}
						}
					}
					return list;
				}
			}
			return list;
		}

		internal void ProcessCompiledInstances()
		{
			if (this.RequiresCompilation && this.m_compilationState == CompilationState.NotCompiled)
			{
				try
				{
					this.m_compilationState = CompilationState.Compiling;
					IGaugeMapper gaugeMapper = GaugeMapperFactory.CreateGaugeMapperInstance(this, base.RenderingContext.OdpContext.ReportDefinition.DefaultFontFamily);
					gaugeMapper.RenderDataGaugePanel();
					this.m_compilationState = CompilationState.Compiled;
				}
				catch (Exception innerException)
				{
					this.m_compilationState = CompilationState.NotCompiled;
					throw new RenderingObjectModelException(innerException);
				}
			}
		}
	}
}
