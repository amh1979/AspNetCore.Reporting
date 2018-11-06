using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class GaugeInputValue
	{
		private GaugePanel m_gaugePanel;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeInputValue m_defObject;

		private GaugeInputValueInstance m_instance;

		private CompiledGaugeInputValueInstance m_compiledInstance;

		private ReportVariantProperty m_value;

		private ReportEnumProperty<GaugeInputValueFormulas> m_formula;

		private ReportDoubleProperty m_minPercent;

		private ReportDoubleProperty m_maxPercent;

		private ReportDoubleProperty m_multiplier;

		private ReportDoubleProperty m_addConstant;

		public ReportVariantProperty Value
		{
			get
			{
				if (this.m_value == null && this.m_defObject.Value != null)
				{
					this.m_value = new ReportVariantProperty(this.m_defObject.Value);
				}
				return this.m_value;
			}
		}

		public ReportEnumProperty<GaugeInputValueFormulas> Formula
		{
			get
			{
				if (this.m_formula == null && this.m_defObject.Formula != null)
				{
					this.m_formula = new ReportEnumProperty<GaugeInputValueFormulas>(this.m_defObject.Formula.IsExpression, this.m_defObject.Formula.OriginalText, EnumTranslator.TranslateGaugeInputValueFormulas(this.m_defObject.Formula.StringValue, null));
				}
				return this.m_formula;
			}
		}

		public ReportDoubleProperty MinPercent
		{
			get
			{
				if (this.m_minPercent == null && this.m_defObject.MinPercent != null)
				{
					this.m_minPercent = new ReportDoubleProperty(this.m_defObject.MinPercent);
				}
				return this.m_minPercent;
			}
		}

		public ReportDoubleProperty MaxPercent
		{
			get
			{
				if (this.m_maxPercent == null && this.m_defObject.MaxPercent != null)
				{
					this.m_maxPercent = new ReportDoubleProperty(this.m_defObject.MaxPercent);
				}
				return this.m_maxPercent;
			}
		}

		public ReportDoubleProperty Multiplier
		{
			get
			{
				if (this.m_multiplier == null && this.m_defObject.Multiplier != null)
				{
					this.m_multiplier = new ReportDoubleProperty(this.m_defObject.Multiplier);
				}
				return this.m_multiplier;
			}
		}

		public ReportDoubleProperty AddConstant
		{
			get
			{
				if (this.m_addConstant == null && this.m_defObject.AddConstant != null)
				{
					this.m_addConstant = new ReportDoubleProperty(this.m_defObject.AddConstant);
				}
				return this.m_addConstant;
			}
		}

		public string DataElementName
		{
			get
			{
				return this.m_defObject.DataElementName;
			}
		}

		public DataElementOutputTypes DataElementOutput
		{
			get
			{
				return this.m_defObject.DataElementOutput;
			}
		}

		internal GaugePanel GaugePanelDef
		{
			get
			{
				return this.m_gaugePanel;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeInputValue GaugeInputValueDef
		{
			get
			{
				return this.m_defObject;
			}
		}

		public GaugeInputValueInstance Instance
		{
			get
			{
				if (this.m_gaugePanel.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					this.m_instance = new GaugeInputValueInstance(this);
				}
				return this.m_instance;
			}
		}

		public CompiledGaugeInputValueInstance CompiledInstance
		{
			get
			{
				this.GaugePanelDef.ProcessCompiledInstances();
				return this.m_compiledInstance;
			}
			internal set
			{
				this.m_compiledInstance = value;
			}
		}

		internal GaugeInputValue(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeInputValue defObject, GaugePanel gaugePanel)
		{
			this.m_defObject = defObject;
			this.m_gaugePanel = gaugePanel;
		}

		internal void SetNewContext()
		{
			if (this.m_instance != null)
			{
				this.m_instance.SetNewContext();
			}
		}
	}
}
