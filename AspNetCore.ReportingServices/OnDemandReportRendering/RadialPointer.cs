using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class RadialPointer : GaugePointer
	{
		private ReportEnumProperty<RadialPointerTypes> m_type;

		private PointerCap m_pointerCap;

		private ReportEnumProperty<RadialPointerNeedleStyles> m_needleStyle;

		public ReportEnumProperty<RadialPointerTypes> Type
		{
			get
			{
				if (this.m_type == null && this.RadialPointerDef.Type != null)
				{
					this.m_type = new ReportEnumProperty<RadialPointerTypes>(this.RadialPointerDef.Type.IsExpression, this.RadialPointerDef.Type.OriginalText, EnumTranslator.TranslateRadialPointerTypes(this.RadialPointerDef.Type.StringValue, null));
				}
				return this.m_type;
			}
		}

		public PointerCap PointerCap
		{
			get
			{
				if (this.m_pointerCap == null && this.RadialPointerDef.PointerCap != null)
				{
					this.m_pointerCap = new PointerCap(this.RadialPointerDef.PointerCap, base.m_gaugePanel);
				}
				return this.m_pointerCap;
			}
		}

		public ReportEnumProperty<RadialPointerNeedleStyles> NeedleStyle
		{
			get
			{
				if (this.m_needleStyle == null && this.RadialPointerDef.NeedleStyle != null)
				{
					this.m_needleStyle = new ReportEnumProperty<RadialPointerNeedleStyles>(this.RadialPointerDef.NeedleStyle.IsExpression, this.RadialPointerDef.NeedleStyle.OriginalText, EnumTranslator.TranslateRadialPointerNeedleStyles(this.RadialPointerDef.NeedleStyle.StringValue, null));
				}
				return this.m_needleStyle;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.RadialPointer RadialPointerDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.RadialPointer)base.m_defObject;
			}
		}

		public new RadialPointerInstance Instance
		{
			get
			{
				return (RadialPointerInstance)this.GetInstance();
			}
		}

		internal RadialPointer(AspNetCore.ReportingServices.ReportIntermediateFormat.RadialPointer defObject, GaugePanel gaugePanel)
			: base(defObject, gaugePanel)
		{
			base.m_defObject = defObject;
			base.m_gaugePanel = gaugePanel;
		}

		internal override GaugePointerInstance GetInstance()
		{
			if (base.m_gaugePanel.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (base.m_instance == null)
			{
				base.m_instance = new RadialPointerInstance(this);
			}
			return (GaugePointerInstance)base.m_instance;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (base.m_instance != null)
			{
				base.m_instance.SetNewContext();
			}
			if (this.m_pointerCap != null)
			{
				this.m_pointerCap.SetNewContext();
			}
		}
	}
}
