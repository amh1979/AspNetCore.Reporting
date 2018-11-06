using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class Utility
	{
		internal static bool IsBold(RPLFormat.FontWeights fontWeight)
		{
			switch (fontWeight)
			{
			case RPLFormat.FontWeights.SemiBold:
			case RPLFormat.FontWeights.Bold:
			case RPLFormat.FontWeights.ExtraBold:
			case RPLFormat.FontWeights.Heavy:
				return true;
			default:
				return false;
			}
		}

		internal static bool IsNullOrEmpty<T>(List<T> list)
		{
			if (list != null && list.Count != 0)
			{
				return false;
			}
			return true;
		}

		internal static void AddInstanceStyles(StyleInstance styleInst, ref Dictionary<byte, object> styles)
		{
			List<StyleAttributeNames> styleAttributes = styleInst.StyleAttributes;
			if (styleAttributes.Count != 0)
			{
				if (styles == null)
				{
					styles = new Dictionary<byte, object>();
				}
				StyleWriterDictionary writer = new StyleWriterDictionary(styles);
				foreach (StyleAttributeNames item in styleAttributes)
				{
					object obj = ((StyleBaseInstance)styleInst)[item];
					if (obj != null)
					{
						Utility.SetStyle(Utility.ConvertROMTORPL(item), obj, writer);
					}
				}
			}
		}

		internal static void WriteSharedStyles(StyleWriter writeTo, Style styleDef)
		{
			List<StyleAttributeNames> sharedStyleAttributes = styleDef.SharedStyleAttributes;
			foreach (StyleAttributeNames item in sharedStyleAttributes)
			{
				ReportProperty reportProperty = ((StyleBase)styleDef)[item];
				if (reportProperty != null)
				{
					Utility.WriteStyleProperty(writeTo, item, reportProperty);
				}
			}
		}

		internal static void WriteStyleProperty(StyleWriter writeTo, Style style, StyleAttributeNames name)
		{
			Utility.WriteStyleProperty(writeTo, name, ((StyleBase)style)[name]);
		}

		internal static void WriteStyleProperty(StyleWriter writeTo, StyleAttributeNames styleAtt, ReportProperty prop)
		{
			switch (styleAtt)
			{
			case StyleAttributeNames.VerticalAlign:
			case StyleAttributeNames.PaddingLeft:
			case StyleAttributeNames.PaddingRight:
			case StyleAttributeNames.PaddingTop:
			case StyleAttributeNames.PaddingBottom:
			case StyleAttributeNames.Direction:
			case StyleAttributeNames.WritingMode:
			case StyleAttributeNames.UnicodeBiDi:
				break;
			case StyleAttributeNames.Color:
				Utility.AddStyle(27, prop as ReportColorProperty, writeTo);
				break;
			case StyleAttributeNames.FontFamily:
				Utility.AddStyle(20, prop as ReportStringProperty, writeTo);
				break;
			case StyleAttributeNames.FontSize:
				Utility.AddStyle(21, prop as ReportSizeProperty, writeTo);
				break;
			case StyleAttributeNames.FontStyle:
				Utility.AddStyle(19, prop, writeTo);
				break;
			case StyleAttributeNames.FontWeight:
				Utility.AddStyle(22, prop, writeTo);
				break;
			case StyleAttributeNames.TextDecoration:
				Utility.AddStyle(24, prop, writeTo);
				break;
			case StyleAttributeNames.Format:
				Utility.AddStyle(23, prop as ReportStringProperty, writeTo);
				break;
			case StyleAttributeNames.Language:
				Utility.AddStyle(32, prop as ReportStringProperty, writeTo);
				break;
			case StyleAttributeNames.NumeralLanguage:
				Utility.AddStyle(36, prop as ReportStringProperty, writeTo);
				break;
			case StyleAttributeNames.NumeralVariant:
				Utility.AddStyle(37, prop as ReportIntProperty, writeTo);
				break;
			case StyleAttributeNames.Calendar:
				Utility.AddStyle(38, prop, writeTo);
				break;
			case StyleAttributeNames.TextAlign:
				Utility.AddStyle(25, prop, writeTo);
				break;
			case StyleAttributeNames.LineHeight:
				Utility.AddStyle(28, prop as ReportStringProperty, writeTo);
				break;
			}
		}

		internal static byte ConvertROMTORPL(StyleAttributeNames ROMType)
		{
			switch (ROMType)
			{
			case StyleAttributeNames.Color:
				return 27;
			case StyleAttributeNames.FontFamily:
				return 20;
			case StyleAttributeNames.FontSize:
				return 21;
			case StyleAttributeNames.FontStyle:
				return 19;
			case StyleAttributeNames.FontWeight:
				return 22;
			case StyleAttributeNames.TextDecoration:
				return 24;
			case StyleAttributeNames.Format:
				return 23;
			case StyleAttributeNames.Language:
				return 32;
			case StyleAttributeNames.NumeralLanguage:
				return 36;
			case StyleAttributeNames.NumeralVariant:
				return 37;
			case StyleAttributeNames.Calendar:
				return 38;
			case StyleAttributeNames.TextAlign:
				return 25;
			case StyleAttributeNames.LineHeight:
				return 28;
			default:
				return 0;
			}
		}

		internal static void SetStyle(byte rplId, object styleProp, StyleWriter writer)
		{
			switch (rplId)
			{
			case 33:
				break;
			case 37:
			{
				int num = (int)styleProp;
				if (num > 0)
				{
					writer.Write(rplId, num);
				}
				break;
			}
			default:
			{
				bool flag = true;
				byte? stylePropByte = PageItem.GetStylePropByte(rplId, styleProp, ref flag);
				int? nullable = stylePropByte;
				if (nullable.HasValue)
				{
					writer.Write(rplId, stylePropByte.Value);
				}
				else if (flag)
				{
					writer.Write(rplId, styleProp.ToString());
				}
				break;
			}
			}
		}

		internal static double GetSizePropertyValue(ReportSizeProperty sizeProp, ReportSize instanceValue)
		{
			if (instanceValue != null)
			{
				return instanceValue.ToMillimeters();
			}
			if (sizeProp != null && sizeProp.Value != null)
			{
				return sizeProp.Value.ToMillimeters();
			}
			return 0.0;
		}

		internal static int GetIntPropertyValue(ReportIntProperty intProp, int? instanceValue)
		{
			if (instanceValue.HasValue)
			{
				return instanceValue.Value;
			}
			if (intProp != null)
			{
				return intProp.Value;
			}
			return 0;
		}

		internal static void WriteReportSize(BinaryWriter spbifWriter, byte rplid, ReportSize value)
		{
			if (value != null)
			{
				spbifWriter.Write(rplid);
				spbifWriter.Write(value.ToString());
			}
		}

		internal static void AddStyle(byte rplId, ReportIntProperty prop, StyleWriter writer)
		{
			if (prop != null)
			{
				writer.Write(rplId, prop.Value);
			}
		}

		internal static void AddStyle(byte rplId, ReportStringProperty prop, StyleWriter writer)
		{
			if (prop != null && prop.Value != null)
			{
				writer.Write(rplId, prop.Value);
			}
		}

		internal static void AddStyle(byte rplId, ReportSizeProperty prop, StyleWriter writer)
		{
			if (prop != null && prop.Value != null)
			{
				writer.Write(rplId, prop.Value.ToString());
			}
		}

		internal static void AddStyle(byte rplId, ReportProperty prop, StyleWriter writer)
		{
			byte? stylePropByte = PageItem.GetStylePropByte(rplId, prop);
			int? nullable = stylePropByte;
			if (nullable.HasValue)
			{
				writer.Write(rplId, stylePropByte.Value);
			}
		}

		internal static void AddStyle(byte rplId, ReportColorProperty prop, StyleWriter writer)
		{
			if (prop != null && prop.Value != null)
			{
				writer.Write(rplId, prop.Value.ToString());
			}
		}

		internal static string GetStringProp(byte rplId, StyleAttributeNames styleAttributeName, Style styleDef, Dictionary<byte, object> styles)
		{
			object obj = null;
			if (styles != null && styles.TryGetValue(rplId, out obj))
			{
				string text = obj as string;
				if (text != null)
				{
					return text;
				}
			}
			ReportStringProperty reportStringProperty = ((StyleBase)styleDef)[styleAttributeName] as ReportStringProperty;
			if (reportStringProperty != null && !reportStringProperty.IsExpression)
			{
				return reportStringProperty.Value;
			}
			return null;
		}

		internal static double GetSizeProp(byte rplId, StyleAttributeNames styleAttributeName, float defaultValue, Style styleDef, Dictionary<byte, object> styles)
		{
			object obj = null;
			if (styles != null && styles.TryGetValue(rplId, out obj))
			{
				string text = obj as string;
				if (text != null)
				{
					RPLReportSize rPLReportSize = new RPLReportSize(text);
					return rPLReportSize.ToPoints();
				}
			}
			ReportSizeProperty reportSizeProperty = ((StyleBase)styleDef)[styleAttributeName] as ReportSizeProperty;
			if (reportSizeProperty != null && !reportSizeProperty.IsExpression && reportSizeProperty.Value != null)
			{
				return reportSizeProperty.Value.ToPoints();
			}
			return (double)defaultValue;
		}

		internal static Color GetColorProp(byte rplId, StyleAttributeNames styleAttributeName, Color defaultColor, Style styleDef, Dictionary<byte, object> styles)
		{
			object obj = null;
			if (styles != null && styles.TryGetValue(rplId, out obj))
			{
				string text = obj as string;
				if (text != null)
				{
					ReportColor reportColor = new ReportColor(text);
					return reportColor.ToColor();
				}
			}
			ReportColorProperty reportColorProperty = ((StyleBase)styleDef)[styleAttributeName] as ReportColorProperty;
			if (reportColorProperty != null && !reportColorProperty.IsExpression)
			{
				ReportColor value = reportColorProperty.Value;
				if (value != null)
				{
					return value.ToColor();
				}
			}
			return defaultColor;
		}

		internal static byte? GetEnumProp(byte rplId, StyleAttributeNames styleAttributeName, Style styleDef, Dictionary<byte, object> styles)
		{
			object obj = null;
			if (styles != null && styles.TryGetValue(rplId, out obj))
			{
				byte? nullable = (byte?)(object)(obj as byte?);
				int? nullable2 = nullable;
				if (nullable2.HasValue)
				{
					return nullable.Value;
				}
			}
			ReportProperty reportProperty = ((StyleBase)styleDef)[styleAttributeName];
			if (reportProperty != null && !reportProperty.IsExpression)
			{
				return PageItem.GetStylePropByte(rplId, reportProperty);
			}
			return null;
		}

		internal static byte? GetNonCompiledEnumProp(byte styleAttributeRplId, StyleAttributeNames styleAttributeName, Style style, StyleInstance styleIntance)
		{
			if (style == null)
			{
				return null;
			}
			ReportProperty reportProperty = ((StyleBase)style)[styleAttributeName];
			if (reportProperty == null)
			{
				return null;
			}
			if (reportProperty.IsExpression)
			{
				if (styleIntance != null)
				{
					object obj = ((StyleBaseInstance)styleIntance)[styleAttributeName];
					if (obj != null)
					{
						bool flag = false;
						return PageItem.GetStylePropByte(styleAttributeRplId, obj, ref flag);
					}
				}
				return null;
			}
			return PageItem.GetStylePropByte(styleAttributeRplId, reportProperty);
		}

		public static ReportSize ReadReportSize(IntermediateFormatReader reader)
		{
			string text = reader.ReadString();
			if (text != null)
			{
				return new ReportSize(text);
			}
			return null;
		}

		public static void WriteReportSize(IntermediateFormatWriter writer, ReportSize reportSize)
		{
			if (reportSize == null)
			{
				writer.WriteNull();
			}
			else
			{
				writer.Write(reportSize.ToString());
			}
		}

		public static int ReportSizeItemSize(ReportSize reportSize)
		{
			if (reportSize == null)
			{
				return AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf((string)null);
			}
			return AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(reportSize.ToString());
		}
	}
}
