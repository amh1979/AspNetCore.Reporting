using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System.Drawing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapDistanceScale : MapDockableSubItem
	{
		private ReportColorProperty m_scaleColor;

		private ReportColorProperty m_scaleBorderColor;

		public ReportColorProperty ScaleColor
		{
			get
			{
				if (this.m_scaleColor == null && this.MapDistanceScaleDef.ScaleColor != null)
				{
					ExpressionInfo scaleColor = this.MapDistanceScaleDef.ScaleColor;
					if (scaleColor != null)
					{
						this.m_scaleColor = new ReportColorProperty(scaleColor.IsExpression, this.MapDistanceScaleDef.ScaleColor.OriginalText, scaleColor.IsExpression ? null : new ReportColor(scaleColor.StringValue.Trim(), true), scaleColor.IsExpression ? new ReportColor("", Color.Empty, true) : null);
					}
				}
				return this.m_scaleColor;
			}
		}

		public ReportColorProperty ScaleBorderColor
		{
			get
			{
				if (this.m_scaleBorderColor == null && this.MapDistanceScaleDef.ScaleBorderColor != null)
				{
					ExpressionInfo scaleBorderColor = this.MapDistanceScaleDef.ScaleBorderColor;
					if (scaleBorderColor != null)
					{
						this.m_scaleBorderColor = new ReportColorProperty(scaleBorderColor.IsExpression, this.MapDistanceScaleDef.ScaleBorderColor.OriginalText, scaleBorderColor.IsExpression ? null : new ReportColor(scaleBorderColor.StringValue.Trim(), true), scaleBorderColor.IsExpression ? new ReportColor("", Color.Empty, true) : null);
					}
				}
				return this.m_scaleBorderColor;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapDistanceScale MapDistanceScaleDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.MapDistanceScale)base.m_defObject;
			}
		}

		public new MapDistanceScaleInstance Instance
		{
			get
			{
				return (MapDistanceScaleInstance)this.GetInstance();
			}
		}

		internal MapDistanceScale(AspNetCore.ReportingServices.ReportIntermediateFormat.MapDistanceScale defObject, Map map)
			: base(defObject, map)
		{
		}

		internal override MapSubItemInstance GetInstance()
		{
			if (base.m_map.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (base.m_instance == null)
			{
				base.m_instance = new MapDistanceScaleInstance(this);
			}
			return (MapSubItemInstance)base.m_instance;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (base.m_instance != null)
			{
				base.m_instance.SetNewContext();
			}
		}
	}
}
