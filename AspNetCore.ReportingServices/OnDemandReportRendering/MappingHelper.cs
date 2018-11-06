using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;


namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal static class MappingHelper
	{
		internal struct MapAreaInfo
		{
			internal string ToolTip;

			internal ImageMapArea.ImageMapAreaShape MapAreaShape;

			internal object Tag;

			internal float[] Coordinates;

			public MapAreaInfo(string toolTip, object tag, ImageMapArea.ImageMapAreaShape mapAreaShape, float[] coordinates)
			{
				this.ToolTip = toolTip;
				this.MapAreaShape = mapAreaShape;
				this.Tag = tag;
				this.Coordinates = coordinates;
			}
		}

		internal struct ActionParameterInfo
		{
			public string Name;

			public object Value;

			public bool Omit;
		}

		internal class ActionTag
		{
			public Action Action;

			public List<ActionParameterInfo> Parameters = new List<ActionParameterInfo>();
		}

		private static Color m_defaultShadowColor = Color.FromArgb(0, 0, 0, 127);

		internal static Color DefaultBackgroundColor
		{
			get
			{
				return Color.Empty;
			}
		}

		internal static Color DefaultBorderColor
		{
			get
			{
				return Color.Black;
			}
		}

		internal static Color DefaultColor
		{
			get
			{
				return Color.Black;
			}
		}

		internal static string DefaultFontFamily
		{
			get
			{
				return "Arial";
			}
		}

		internal static float DefaultFontSize
		{
			get
			{
				return 10f;
			}
		}

		internal static double ConvertToDouble(object value, bool checkForMaxMinValue, bool checkForStringDate)
		{
			bool flag = false;
			return MappingHelper.ConvertToDouble(value, checkForMaxMinValue, checkForStringDate, ref flag);
		}

		internal static double ConvertToDouble(object value, bool checkForMaxMinValue, bool checkForStringDate, ref bool isDateTime)
		{
			if (value == null)
			{
				return double.NaN;
			}
			switch (Type.GetTypeCode(value.GetType()))
			{
			case TypeCode.Byte:
				return (double)(int)(byte)value;
			case TypeCode.Char:
				return (double)(int)(char)value;
			case TypeCode.Decimal:
				return decimal.ToDouble((decimal)value);
			case TypeCode.Double:
				return (double)value;
			case TypeCode.Int16:
				return (double)(short)value;
			case TypeCode.Int32:
				return (double)(int)value;
			case TypeCode.Int64:
				return (double)(long)value;
			case TypeCode.SByte:
				return (double)(sbyte)value;
			case TypeCode.Single:
				return (double)(float)value;
			case TypeCode.UInt16:
				return (double)(int)(ushort)value;
			case TypeCode.UInt32:
				return (double)(uint)value;
			case TypeCode.UInt64:
				return (double)(ulong)value;
			case TypeCode.DateTime:
				isDateTime = true;
				return ((DateTime)value).ToOADate();
			case TypeCode.String:
			{
				double result = double.NaN;
				string text = value.ToString().Trim();
				if (checkForStringDate)
				{
					DateTime minValue = DateTime.MinValue;
					if (DateTime.TryParse(text, out minValue))
					{
						isDateTime = true;
						return minValue.ToOADate();
					}
				}
				if (double.TryParse(text, out result))
				{
					return result;
				}
				if (!checkForMaxMinValue)
				{
					break;
				}
				if (text == "MaxValue")
				{
					return 1.7976931348623157E+308;
				}
				if (!(text == "MinValue"))
				{
					break;
				}
				return -1.7976931348623157E+308;
			}
			}
			return double.NaN;
		}

		internal static float[] ConvertCoordinatesToRelative(float[] pixelCoordinates, float width, float height)
		{
			float[] array = new float[pixelCoordinates.Length];
			for (int i = 0; i < array.Length; i += 2)
			{
				array[i] = (float)(pixelCoordinates[i] / width * 100.0);
				if (i + 1 < array.Length)
				{
					array[i + 1] = (float)(pixelCoordinates[i + 1] / height * 100.0);
				}
			}
			return array;
		}

		internal static float[] ConvertCoordinatesToRelative(int[] pixelCoordinates, float width, float height)
		{
			float[] array = new float[pixelCoordinates.Length];
			for (int i = 0; i < array.Length; i += 2)
			{
				array[i] = (float)((float)pixelCoordinates[i] / width * 100.0);
				if (i + 1 < array.Length)
				{
					array[i + 1] = (float)((float)pixelCoordinates[i + 1] / height * 100.0);
				}
			}
			return array;
		}

		private static Action GetActionFromActionInfo(ActionInfo actionInfo)
		{
			if (actionInfo == null)
			{
				return null;
			}
			if (actionInfo.Actions == null)
			{
				return null;
			}
			if (actionInfo.Actions.Count == 0)
			{
				return null;
			}
			return ((ReportElementCollectionBase<Action>)actionInfo.Actions)[0];
		}

		private static string EvaluateHref(Action action, out bool isExpression)
		{
			isExpression = false;
			if (action.Hyperlink != null)
			{
				if (!action.Hyperlink.IsExpression)
				{
					if (action.Hyperlink.Value != null)
					{
						return action.Hyperlink.Value.ToString();
					}
				}
				else
				{
					isExpression = true;
					if (action.Instance != null && action.Instance.Hyperlink != null)
					{
						return action.Instance.Hyperlink.ToString();
					}
				}
			}
			else if (action.Drillthrough != null && action.Drillthrough.ReportName != null)
			{
				if (!action.Drillthrough.ReportName.IsExpression)
				{
					if (action.Drillthrough.ReportName.Value != null)
					{
						return action.Drillthrough.ReportName.Value;
					}
				}
				else
				{
					isExpression = true;
					if (action.Drillthrough.Instance != null && action.Drillthrough.Instance.ReportName != null)
					{
						return action.Drillthrough.Instance.ReportName;
					}
				}
			}
			if (action.BookmarkLink != null)
			{
				if (!action.BookmarkLink.IsExpression)
				{
					if (action.BookmarkLink.Value != null)
					{
						return action.BookmarkLink.Value;
					}
				}
				else
				{
					isExpression = true;
					if (action.Instance != null && action.Instance.BookmarkLink != null)
					{
						return action.Instance.BookmarkLink;
					}
				}
			}
			return null;
		}

		private static void EvaluateActionParameters(ActionDrillthrough actionDrillthroughSource, ActionDrillthrough actionDrillthroughDestination)
		{
			if (actionDrillthroughSource.Parameters != null)
			{
				foreach (Parameter parameter2 in actionDrillthroughSource.Parameters)
				{
					Parameter parameter = actionDrillthroughDestination.CreateParameter(parameter2.Name);
					if (!parameter2.Value.IsExpression)
					{
						parameter.Instance.Value = parameter2.Value.Value;
					}
					else
					{
						parameter.Instance.Value = parameter2.Instance.Value;
					}
					if (!parameter2.Omit.IsExpression)
					{
						parameter.Instance.Omit = parameter2.Omit.Value;
					}
					else
					{
						parameter.Instance.Omit = parameter2.Instance.Omit;
					}
				}
			}
		}

		internal static ActionInfoWithDynamicImageMap CreateActionInfoDynamic(ReportItem reportItem, ActionInfo actionInfo, string toolTip, out string href)
		{
			return MappingHelper.CreateActionInfoDynamic(reportItem, actionInfo, toolTip, out href, true);
		}

		internal static ActionInfoWithDynamicImageMap CreateActionInfoDynamic(ReportItem reportItem, ActionInfo actionInfo, string toolTip, out string href, bool applyExpression)
		{
			Action actionFromActionInfo = MappingHelper.GetActionFromActionInfo(actionInfo);
			if (actionFromActionInfo == null)
			{
				href = null;
			}
			else
			{
				bool flag = default(bool);
				href = MappingHelper.EvaluateHref(actionFromActionInfo, out flag);
				if (flag && !applyExpression)
				{
					href = null;
				}
			}
			bool flag2 = actionFromActionInfo == null || href == null;
			bool flag3 = string.IsNullOrEmpty(toolTip);
			if (flag2 && flag3)
			{
				return null;
			}
			ActionInfoWithDynamicImageMap actionInfoWithDynamicImageMap;
			if (!flag2)
			{
				actionInfoWithDynamicImageMap = new ActionInfoWithDynamicImageMap(reportItem.RenderingContext, reportItem, actionInfo.ReportScope, actionInfo.InstancePath, actionInfo.ROMActionOwner, true);
				if (actionFromActionInfo.BookmarkLink != null)
				{
					Action action = actionInfoWithDynamicImageMap.CreateBookmarkLinkAction();
					action.Instance.BookmarkLink = href;
				}
				else if (actionFromActionInfo.Hyperlink != null)
				{
					Action action2 = actionInfoWithDynamicImageMap.CreateHyperlinkAction();
					action2.Instance.HyperlinkText = href;
				}
				else if (actionFromActionInfo.Drillthrough != null)
				{
					Action action3 = actionInfoWithDynamicImageMap.CreateDrillthroughAction();
					action3.Drillthrough.Instance.ReportName = href;
					MappingHelper.EvaluateActionParameters(actionFromActionInfo.Drillthrough, action3.Drillthrough);
					string drillthroughID = action3.Drillthrough.Instance.DrillthroughID;
				}
			}
			else
			{
				actionInfoWithDynamicImageMap = new ActionInfoWithDynamicImageMap(reportItem.RenderingContext, reportItem, reportItem.ReportScope, reportItem.ReportItemDef, null, true);
			}
			return actionInfoWithDynamicImageMap;
		}

		internal static ActionInfoWithDynamicImageMapCollection GetImageMaps(IEnumerable<MapAreaInfo> mapAreaInfoList, ActionInfoWithDynamicImageMapCollection actions, ReportItem reportItem)
		{
			List<ActionInfoWithDynamicImageMap> list = new List<ActionInfoWithDynamicImageMap>();
			bool[] array = new bool[actions.Count];
			foreach (MapAreaInfo mapAreaInfo in mapAreaInfoList)
			{
				MapAreaInfo current = mapAreaInfo;
				int num = MappingHelper.AddMapArea(current, actions, reportItem);
				if (num > -1 && !array[num])
				{
					list.Add(((ReportElementCollectionBase<ActionInfoWithDynamicImageMap>)actions)[num]);
					array[num] = true;
				}
				else if (!string.IsNullOrEmpty(current.ToolTip))
				{
					string text = default(string);
					ActionInfoWithDynamicImageMap actionInfoWithDynamicImageMap = MappingHelper.CreateActionInfoDynamic(reportItem, (ActionInfo)null, current.ToolTip, out text);
					if (actionInfoWithDynamicImageMap != null)
					{
						actionInfoWithDynamicImageMap.CreateImageMapAreaInstance(current.MapAreaShape, current.Coordinates, current.ToolTip);
						list.Add(actionInfoWithDynamicImageMap);
					}
				}
			}
			actions.InternalList.Clear();
			actions.InternalList.AddRange(list);
			if (actions.Count == 0)
			{
				return null;
			}
			return actions;
		}

		private static int AddMapArea(MapAreaInfo mapAreaInfo, ActionInfoWithDynamicImageMapCollection actions, ReportItem reportItem)
		{
			if (mapAreaInfo.Tag == null)
			{
				return -1;
			}
			int num = (int)mapAreaInfo.Tag;
			ActionInfoWithDynamicImageMap actionInfoWithDynamicImageMap = actions.InternalList[num];
			if (actionInfoWithDynamicImageMap.Actions.Count <= 0 && string.IsNullOrEmpty(mapAreaInfo.ToolTip))
			{
				return -1;
			}
			actionInfoWithDynamicImageMap.CreateImageMapAreaInstance(mapAreaInfo.MapAreaShape, mapAreaInfo.Coordinates, mapAreaInfo.ToolTip);
			return num;
		}

		internal static Color GetStyleColor(Style style, StyleInstance styleInstance)
		{
			ReportColorProperty color = style.Color;
			Color result = Color.Black;
			if (!MappingHelper.GetColorFromReportColorProperty(color, ref result))
			{
				ReportColor color2 = styleInstance.Color;
				if (color2 != null)
				{
					result = color2.ToColor();
				}
			}
			return result;
		}

		internal static Color GetStyleBackgroundColor(Style style, StyleInstance styleInstance)
		{
			ReportColorProperty backgroundColor = style.BackgroundColor;
			Color result = Color.Empty;
			if (!MappingHelper.GetColorFromReportColorProperty(backgroundColor, ref result))
			{
				ReportColor backgroundColor2 = styleInstance.BackgroundColor;
				if (backgroundColor2 != null)
				{
					result = backgroundColor2.ToColor();
				}
			}
			return result;
		}

		internal static Color GetStyleBackGradientEndColor(Style style, StyleInstance styleInstance)
		{
			ReportColorProperty backgroundGradientEndColor = style.BackgroundGradientEndColor;
			Color result = Color.Empty;
			if (!MappingHelper.GetColorFromReportColorProperty(backgroundGradientEndColor, ref result))
			{
				ReportColor backgroundGradientEndColor2 = styleInstance.BackgroundGradientEndColor;
				if (backgroundGradientEndColor2 != null)
				{
					result = backgroundGradientEndColor2.ToColor();
				}
			}
			return result;
		}

		internal static Color GetStyleShadowColor(Style style, StyleInstance styleInstance)
		{
			ReportColorProperty shadowColor = style.ShadowColor;
			Color result = MappingHelper.m_defaultShadowColor;
			if (!MappingHelper.GetColorFromReportColorProperty(shadowColor, ref result))
			{
				ReportColor shadowColor2 = styleInstance.ShadowColor;
				if (shadowColor2 != null)
				{
					result = shadowColor2.ToColor();
				}
			}
			return result;
		}

		internal static BackgroundGradients GetStyleBackGradientType(Style style, StyleInstance styleInstance)
		{
			ReportEnumProperty<BackgroundGradients> backgroundGradientType = style.BackgroundGradientType;
			if (!backgroundGradientType.IsExpression)
			{
				return backgroundGradientType.Value;
			}
			return styleInstance.BackgroundGradientType;
		}

		internal static BackgroundHatchTypes GetStyleBackgroundHatchType(Style style, StyleInstance styleInstance)
		{
			ReportEnumProperty<BackgroundHatchTypes> backgroundHatchType = style.BackgroundHatchType;
			if (!backgroundHatchType.IsExpression)
			{
				return backgroundHatchType.Value;
			}
			return styleInstance.BackgroundHatchType;
		}

		internal static int GetStyleShadowOffset(Style style, StyleInstance styleInstance, float dpi)
		{
			ReportSizeProperty shadowOffset = style.ShadowOffset;
			if (!shadowOffset.IsExpression)
			{
				return MappingHelper.ToIntPixels(shadowOffset.Value, dpi);
			}
			ReportSize shadowOffset2 = styleInstance.ShadowOffset;
			if (shadowOffset2 != null)
			{
				return MappingHelper.ToIntPixels(shadowOffset2, dpi);
			}
			return 0;
		}

		internal static Font GetDefaultFont()
		{
			return new Font(MappingHelper.DefaultFontFamily, MappingHelper.DefaultFontSize, MappingHelper.GetStyleFontStyle(FontStyles.Normal, FontWeights.Normal, TextDecorations.None));
		}

		internal static TextDecorations GetStyleFontTextDecoration(Style style, StyleInstance styleInstance)
		{
			ReportEnumProperty<TextDecorations> textDecoration = style.TextDecoration;
			if (MappingHelper.IsStylePropertyDefined(textDecoration))
			{
				if (!textDecoration.IsExpression)
				{
					return textDecoration.Value;
				}
				return styleInstance.TextDecoration;
			}
			return TextDecorations.None;
		}

		internal static FontWeights GetStyleFontWeight(Style style, StyleInstance styleInstance)
		{
			ReportEnumProperty<FontWeights> fontWeight = style.FontWeight;
			if (MappingHelper.IsStylePropertyDefined(fontWeight))
			{
				if (!fontWeight.IsExpression)
				{
					return fontWeight.Value;
				}
				return styleInstance.FontWeight;
			}
			return FontWeights.Normal;
		}

		internal static FontStyles GetStyleFontStyle(Style style, StyleInstance styleInstance)
		{
			ReportEnumProperty<FontStyles> fontStyle = style.FontStyle;
			if (MappingHelper.IsStylePropertyDefined(fontStyle))
			{
				if (!fontStyle.IsExpression)
				{
					return fontStyle.Value;
				}
				return styleInstance.FontStyle;
			}
			return FontStyles.Normal;
		}

		internal static float GetStyleFontSize(Style style, StyleInstance styleInstance)
		{
			ReportSizeProperty fontSize = style.FontSize;
			if (MappingHelper.IsStylePropertyDefined(fontSize))
			{
				if (!fontSize.IsExpression)
				{
					return (float)fontSize.Value.ToPoints();
				}
				if (styleInstance.FontSize != null)
				{
					ReportSize fontSize2 = styleInstance.FontSize;
					if (fontSize2 != null)
					{
						return (float)fontSize2.ToPoints();
					}
				}
			}
			return MappingHelper.DefaultFontSize;
		}

		internal static string GetStyleFontFamily(Style style, StyleInstance styleInstance, string fallbackFont)
		{
			ReportStringProperty fontFamily = style.FontFamily;
			if (MappingHelper.IsStylePropertyDefined(fontFamily))
			{
				if (!fontFamily.IsExpression)
				{
					if (fontFamily != null)
					{
						return fontFamily.Value;
					}
				}
				else if (styleInstance.FontFamily != null)
				{
					return styleInstance.FontFamily;
				}
			}
			return fallbackFont;
		}

		internal static FontStyle GetStyleFontStyle(FontStyles style, FontWeights weight, TextDecorations textDecoration)
		{
			FontStyle fontStyle = FontStyle.Regular;
			if (style == FontStyles.Italic)
			{
				fontStyle = FontStyle.Italic;
			}
			switch (weight)
			{
			case FontWeights.SemiBold:
			case FontWeights.Bold:
			case FontWeights.ExtraBold:
			case FontWeights.Heavy:
				fontStyle |= FontStyle.Bold;
				break;
			}
			switch (textDecoration)
			{
			case TextDecorations.LineThrough:
				fontStyle |= FontStyle.Strikeout;
				break;
			case TextDecorations.Underline:
				fontStyle |= FontStyle.Underline;
				break;
			}
			return fontStyle;
		}

		internal static Color GetStyleBorderColor(Border border)
		{
			ReportColorProperty color = border.Color;
			Color result = Color.Black;
			if (!MappingHelper.GetColorFromReportColorProperty(color, ref result))
			{
				ReportColor color2 = border.Instance.Color;
				if (color2 != null)
				{
					result = color2.ToColor();
				}
			}
			return result;
		}

		internal static int GetStyleBorderWidth(Border border, float dpi)
		{
			ReportSizeProperty width = border.Width;
			int result = MappingHelper.GetDefaultBorderWidth(dpi);
			if (!width.IsExpression)
			{
				if (width.Value != null)
				{
					result = MappingHelper.ToIntPixels(width.Value, dpi);
				}
			}
			else
			{
				ReportSize width2 = border.Instance.Width;
				if (width2 != null)
				{
					result = MappingHelper.ToIntPixels(width2, dpi);
				}
			}
			return result;
		}

		internal static BorderStyles GetStyleBorderStyle(Border border)
		{
			ReportEnumProperty<BorderStyles> style = border.Style;
			if (!style.IsExpression)
			{
				return style.Value;
			}
			return border.Instance.Style;
		}

		internal static TextAlignments GetStyleTextAlign(Style style, StyleInstance styleInstance)
		{
			ReportEnumProperty<TextAlignments> textAlign = style.TextAlign;
			if (!textAlign.IsExpression)
			{
				return textAlign.Value;
			}
			return styleInstance.TextAlign;
		}

		internal static VerticalAlignments GetStyleVerticalAlignment(Style style, StyleInstance styleInstance)
		{
			ReportEnumProperty<VerticalAlignments> verticalAlign = style.VerticalAlign;
			if (!verticalAlign.IsExpression)
			{
				return verticalAlign.Value;
			}
			return styleInstance.VerticalAlign;
		}

		internal static TextEffects GetStyleTextEffect(Style style, StyleInstance styleInstance)
		{
			ReportEnumProperty<TextEffects> textEffect = style.TextEffect;
			if (!textEffect.IsExpression)
			{
				return textEffect.Value;
			}
			return styleInstance.TextEffect;
		}

		internal static string GetStyleFormat(Style style, StyleInstance styleInstance)
		{
			ReportStringProperty format = style.Format;
			string text = null;
			if (!format.IsExpression)
			{
				if (format.Value != null)
				{
					text = format.Value;
				}
			}
			else if (styleInstance.Format != null)
			{
				text = styleInstance.Format;
			}
			if (text == null)
			{
				return "";
			}
			return text;
		}

		internal static ContentAlignment GetStyleContentAlignment(Style style, StyleInstance styleInstance)
		{
			TextAlignments styleTextAlign = MappingHelper.GetStyleTextAlign(style, styleInstance);
			VerticalAlignments styleVerticalAlignment = MappingHelper.GetStyleVerticalAlignment(style, styleInstance);
			ContentAlignment result = ContentAlignment.TopLeft;
			switch (styleTextAlign)
			{
			case TextAlignments.Center:
				switch (styleVerticalAlignment)
				{
				case VerticalAlignments.Bottom:
					result = ContentAlignment.BottomCenter;
					break;
				case VerticalAlignments.Middle:
					result = ContentAlignment.MiddleCenter;
					break;
				default:
					result = ContentAlignment.TopCenter;
					break;
				}
				break;
			case TextAlignments.Right:
				switch (styleVerticalAlignment)
				{
				case VerticalAlignments.Bottom:
					result = ContentAlignment.BottomRight;
					break;
				case VerticalAlignments.Middle:
					result = ContentAlignment.MiddleRight;
					break;
				default:
					result = ContentAlignment.TopRight;
					break;
				}
				break;
			default:
				switch (styleVerticalAlignment)
				{
				case VerticalAlignments.Bottom:
					result = ContentAlignment.BottomLeft;
					break;
				case VerticalAlignments.Middle:
					result = ContentAlignment.MiddleLeft;
					break;
				}
				break;
			}
			return result;
		}

		internal static bool IsStylePropertyDefined(ReportProperty property)
		{
			if (property != null)
			{
				return property.ExpressionString != null;
			}
			return false;
		}

		internal static bool IsPropertyExpression(ReportProperty property)
		{
			if (property == null)
			{
				return false;
			}
			return property.IsExpression;
		}

		internal static bool GetColorFromReportColorProperty(ReportColorProperty reportColorProperty, ref Color color)
		{
			if (!reportColorProperty.IsExpression && reportColorProperty.Value != null)
			{
				color = reportColorProperty.Value.ToColor();
				return true;
			}
			return false;
		}

		internal static RightToLeft GetStyleDirection(Style style, StyleInstance styleInstance)
		{
			Directions directions = style.Direction.IsExpression ? styleInstance.Direction : style.Direction.Value;
			if (directions == Directions.RTL)
			{
				return RightToLeft.Yes;
			}
			return RightToLeft.No;
		}

		internal static double ToPixels(ReportSize size, float dpi)
		{
			return size.ToInches() * (double)dpi;
		}

		internal static int ToIntPixels(ReportSize size, float dpi)
		{
			return Convert.ToInt32(MappingHelper.ToPixels(size, dpi));
		}

		internal static double ToPixels(double value, Unit unit, float dpi)
		{
			switch (unit)
			{
			case Unit.Centimeter:
				value /= 2.54;
				break;
			case Unit.Millimeter:
				value /= 25.4;
				break;
			case Unit.Pica:
				value /= 6.0;
				break;
			case Unit.Point:
				value /= 72.0;
				break;
			}
			return value * (double)dpi;
		}

		internal static int ToIntPixels(double value, Unit unit, float dpi)
		{
			return Convert.ToInt32(MappingHelper.ToPixels(value, unit, dpi));
		}

		internal static int GetDefaultBorderWidth(float dpi)
		{
			return (int)Math.Round(0.013888888888888888 * (double)dpi);
		}
	}
}
