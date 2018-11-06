using AspNetCore.Reporting.Map.WebForms;
using System.Drawing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class SpatialElementTemplateMapper
	{
		protected MapVectorLayer m_mapVectorLayer;

		protected MapMapper m_mapMapper;

		protected abstract MapSpatialElementTemplate DefaultTemplate
		{
			get;
		}

		internal SpatialElementTemplateMapper(MapMapper mapMapper, MapVectorLayer mapVectorLayer)
		{
			this.m_mapVectorLayer = mapVectorLayer;
			this.m_mapMapper = mapMapper;
		}

		protected void RenderSpatialElementTemplate(MapSpatialElementTemplate mapSpatialElementTemplate, ISpatialElement coreSpatialElement, bool ignoreBackgroundColor, bool hasScope)
		{
			ReportStringProperty toolTip = mapSpatialElementTemplate.ToolTip;
			string text = null;
			if (toolTip != null)
			{
				if (!toolTip.IsExpression)
				{
					text = toolTip.Value;
				}
				else if (hasScope)
				{
					text = mapSpatialElementTemplate.Instance.ToolTip;
				}
				if (text != null)
				{
					text = (coreSpatialElement.ToolTip = VectorLayerMapper.AddPrefixToFieldNames(this.m_mapVectorLayer.Name, text));
				}
			}
			this.m_mapMapper.RenderActionInfo(mapSpatialElementTemplate.ActionInfo, text, coreSpatialElement, this.m_mapVectorLayer.Name, hasScope);
			ReportBoolProperty hidden = mapSpatialElementTemplate.Hidden;
			if (hidden != null)
			{
				if (!hidden.IsExpression)
				{
					coreSpatialElement.Visible = !hidden.Value;
				}
				else if (hasScope)
				{
					coreSpatialElement.Visible = !mapSpatialElementTemplate.Instance.Hidden;
				}
				else
				{
					coreSpatialElement.Visible = true;
				}
			}
			else
			{
				coreSpatialElement.Visible = true;
			}
			ReportStringProperty label = mapSpatialElementTemplate.Label;
			if (label != null)
			{
				string text3 = "";
				if (!label.IsExpression)
				{
					text3 = label.Value;
				}
				else if (hasScope)
				{
					text3 = mapSpatialElementTemplate.Instance.Label;
				}
				if (text3 != null)
				{
					coreSpatialElement.Text = VectorLayerMapper.AddPrefixToFieldNames(this.m_mapVectorLayer.Name, text3);
				}
			}
			ReportDoubleProperty offsetX = mapSpatialElementTemplate.OffsetX;
			double x = 0.0;
			if (offsetX != null)
			{
				if (!offsetX.IsExpression)
				{
					x = offsetX.Value;
				}
				else if (hasScope)
				{
					x = mapSpatialElementTemplate.Instance.OffsetX;
				}
				coreSpatialElement.Offset.X = x;
			}
			offsetX = mapSpatialElementTemplate.OffsetY;
			x = 0.0;
			if (offsetX != null)
			{
				if (!offsetX.IsExpression)
				{
					x = offsetX.Value;
				}
				else if (hasScope)
				{
					x = mapSpatialElementTemplate.Instance.OffsetY;
				}
				coreSpatialElement.Offset.Y = x;
			}
			Style style = mapSpatialElementTemplate.Style;
			StyleInstance style2 = mapSpatialElementTemplate.Instance.Style;
			this.RenderStyle(style, style2, coreSpatialElement, ignoreBackgroundColor, hasScope);
		}

		protected void RenderStyle(Style style, StyleInstance styleInstance, ISpatialElement coreSpatialElement, bool ignoreBackgroundColor, bool hasScope)
		{
			if (!ignoreBackgroundColor)
			{
				coreSpatialElement.Color = this.GetBackgroundColor(style, styleInstance, hasScope);
			}
			coreSpatialElement.SecondaryColor = this.GetBackGradientEndColor(style, styleInstance, hasScope);
			coreSpatialElement.GradientType = this.GetGradientType(style, styleInstance, hasScope);
			coreSpatialElement.HatchStyle = this.GetHatchStyle(style, styleInstance, hasScope);
			coreSpatialElement.ShadowOffset = this.GetShadowOffset(style, styleInstance, hasScope);
			coreSpatialElement.BorderColor = this.GetBorderColor(style, styleInstance, hasScope);
			coreSpatialElement.BorderWidth = this.GetBorderWidth(style, styleInstance, hasScope);
			coreSpatialElement.TextColor = this.GetTextColor(style, styleInstance, hasScope);
			coreSpatialElement.Font = this.GetFont(style, styleInstance, hasScope);
		}

		internal Font GetFont(bool hasScope)
		{
			Style style = default(Style);
			StyleInstance styleInstance = default(StyleInstance);
			this.GetDefaultStyle(out style, out styleInstance);
			return this.GetFont(style, styleInstance, hasScope);
		}

		internal Font GetFont(Style style, StyleInstance styleInstance, bool hasScope)
		{
			if (style == null)
			{
				return this.m_mapMapper.GetDefaultFontFromCache(0);
			}
			string text = MappingHelper.DefaultFontFamily;
			if (this.m_mapMapper.GetDefaultFont() != null)
			{
				text = this.m_mapMapper.GetDefaultFont().Name;
			}
			if (!MappingHelper.IsPropertyExpression(style.FontFamily) || hasScope)
			{
				text = MappingHelper.GetStyleFontFamily(style, styleInstance, text);
			}
			float fontSize = (MappingHelper.IsPropertyExpression(style.FontSize) && !hasScope) ? MappingHelper.DefaultFontSize : MappingHelper.GetStyleFontSize(style, styleInstance);
			FontStyles fontStyle = (MappingHelper.IsPropertyExpression(style.FontStyle) && !hasScope) ? FontStyles.Normal : MappingHelper.GetStyleFontStyle(style, styleInstance);
			FontWeights fontWeight = (MappingHelper.IsPropertyExpression(style.FontWeight) && !hasScope) ? FontWeights.Normal : MappingHelper.GetStyleFontWeight(style, styleInstance);
			TextDecorations textDecoration = (MappingHelper.IsPropertyExpression(style.TextDecoration) && !hasScope) ? TextDecorations.None : MappingHelper.GetStyleFontTextDecoration(style, styleInstance);
			return this.m_mapMapper.GetFontFromCache(0, text, fontSize, fontStyle, fontWeight, textDecoration);
		}

		internal Color GetTextColor(bool hasScope)
		{
			Style style = default(Style);
			StyleInstance styleInstance = default(StyleInstance);
			this.GetDefaultStyle(out style, out styleInstance);
			return this.GetTextColor(style, styleInstance, hasScope);
		}

		internal Color GetTextColor(Style style, StyleInstance styleInstance, bool hasScope)
		{
			if (style != null && (!MappingHelper.IsPropertyExpression(style.Color) || hasScope))
			{
				return MappingHelper.GetStyleColor(style, styleInstance);
			}
			return MappingHelper.DefaultColor;
		}

		internal int GetShadowOffset(bool hasScope)
		{
			Style style = default(Style);
			StyleInstance styleInstance = default(StyleInstance);
			this.GetDefaultStyle(out style, out styleInstance);
			return this.GetShadowOffset(style, styleInstance, hasScope);
		}

		internal int GetShadowOffset(Style style, StyleInstance styleInstance, bool hasScope)
		{
			if (style != null && (!MappingHelper.IsPropertyExpression(style.ShadowOffset) || hasScope))
			{
				return MapMapper.GetValidShadowOffset(MappingHelper.GetStyleShadowOffset(style, styleInstance, this.m_mapMapper.DpiX));
			}
			return 0;
		}

		internal MapHatchStyle GetHatchStyle(bool hasScope)
		{
			Style style = default(Style);
			StyleInstance styleInstance = default(StyleInstance);
			this.GetDefaultStyle(out style, out styleInstance);
			return this.GetHatchStyle(style, styleInstance, hasScope);
		}

		internal MapHatchStyle GetHatchStyle(Style style, StyleInstance styleInstance, bool hasScope)
		{
			if (style != null && (!MappingHelper.IsPropertyExpression(style.BackgroundHatchType) || hasScope))
			{
				return MapMapper.GetHatchStyle(style, styleInstance);
			}
			return MapHatchStyle.None;
		}

		internal GradientType GetGradientType(bool hasScope)
		{
			Style style = default(Style);
			StyleInstance styleInstance = default(StyleInstance);
			this.GetDefaultStyle(out style, out styleInstance);
			return this.GetGradientType(style, styleInstance, hasScope);
		}

		internal GradientType GetGradientType(Style style, StyleInstance styleInstance, bool hasScope)
		{
			if (style != null && (!MappingHelper.IsPropertyExpression(style.BackgroundGradientType) || hasScope))
			{
				return MapMapper.GetGradientType(style, styleInstance);
			}
			return GradientType.None;
		}

		internal Color GetBackGradientEndColor(bool hasScope)
		{
			Style style = default(Style);
			StyleInstance styleInstance = default(StyleInstance);
			this.GetDefaultStyle(out style, out styleInstance);
			return this.GetBackGradientEndColor(style, styleInstance, hasScope);
		}

		internal Color GetBackGradientEndColor(Style style, StyleInstance styleInstance, bool hasScope)
		{
			if (style != null && (!MappingHelper.IsPropertyExpression(style.BackgroundGradientEndColor) || hasScope))
			{
				return MappingHelper.GetStyleBackGradientEndColor(style, styleInstance);
			}
			return Color.Empty;
		}

		internal Color GetBackgroundColor(bool hasScope)
		{
			Style style = default(Style);
			StyleInstance styleInstance = default(StyleInstance);
			this.GetDefaultStyle(out style, out styleInstance);
			return this.GetBackgroundColor(style, styleInstance, hasScope);
		}

		internal Color GetBackgroundColor(Style style, StyleInstance styleInstance, bool hasScope)
		{
			if (style == null)
			{
				return MappingHelper.DefaultBackgroundColor;
			}
			if (MappingHelper.IsPropertyExpression(style.BackgroundColor) && !hasScope)
			{
				return MappingHelper.DefaultBackgroundColor;
			}
			return MappingHelper.GetStyleBackgroundColor(style, styleInstance);
		}

		internal int GetBorderWidth(bool hasScope)
		{
			Style style = default(Style);
			StyleInstance styleInstance = default(StyleInstance);
			this.GetDefaultStyle(out style, out styleInstance);
			return this.GetBorderWidth(style, styleInstance, hasScope);
		}

		internal int GetBorderWidth(Style style, StyleInstance styleInstance, bool hasScope)
		{
			if (style != null)
			{
				Border border = style.Border;
				if (border != null && (!MappingHelper.IsPropertyExpression(border.Width) || hasScope))
				{
					return MappingHelper.GetStyleBorderWidth(border, this.m_mapMapper.DpiX);
				}
			}
			return MappingHelper.GetDefaultBorderWidth(this.m_mapMapper.DpiX);
		}

		internal Color GetBorderColor(bool hasScope)
		{
			Style style = default(Style);
			StyleInstance styleInstance = default(StyleInstance);
			this.GetDefaultStyle(out style, out styleInstance);
			return this.GetBorderColor(style, styleInstance, hasScope);
		}

		internal Color GetBorderColor(Style style, StyleInstance styleInstance, bool hasScope)
		{
			if (style != null)
			{
				Border border = style.Border;
				if (border != null && (!MappingHelper.IsPropertyExpression(border.Color) || hasScope))
				{
					return MappingHelper.GetStyleBorderColor(border);
				}
			}
			return MappingHelper.DefaultBorderColor;
		}

		internal MapDashStyle GetBorderStyle(bool hasScope)
		{
			Style style = default(Style);
			StyleInstance styleInstance = default(StyleInstance);
			this.GetDefaultStyle(out style, out styleInstance);
			return this.GetBorderStyle(style, styleInstance, hasScope);
		}

		internal MapDashStyle GetBorderStyle(Style style, StyleInstance styleInstance, bool hasScope)
		{
			if (style != null)
			{
				Border border = style.Border;
				if (border != null)
				{
					return MapMapper.GetDashStyle(border, hasScope, false);
				}
			}
			return MapDashStyle.Solid;
		}

		private void GetDefaultStyle(out Style style, out StyleInstance styleInstance)
		{
			MapSpatialElementTemplate defaultTemplate = this.DefaultTemplate;
			if (defaultTemplate == null)
			{
				style = null;
				styleInstance = null;
			}
			else
			{
				style = defaultTemplate.Style;
				styleInstance = defaultTemplate.Instance.Style;
			}
		}
	}
}
