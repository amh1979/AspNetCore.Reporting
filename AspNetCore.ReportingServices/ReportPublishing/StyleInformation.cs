using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCore.ReportingServices.ReportPublishing
{
	internal sealed class StyleInformation
	{
		internal sealed class StyleInformationAttribute
		{
			public string Name;

			public AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo Value;

			public ValueType ValueType;
		}

		private List<StyleInformationAttribute> m_attributes = new List<StyleInformationAttribute>();

		private static Hashtable StyleNameIndexes;

		private static bool[,] AllowStyleAttributeByType;

		internal List<StyleInformationAttribute> Attributes
		{
			get
			{
				return this.m_attributes;
			}
		}

		static StyleInformation()
		{
			StyleInformation.AllowStyleAttributeByType = new bool[52, 18]
			{
				{
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					false,
					false,
					true
				},
				{
					true,
					false,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					false,
					false,
					true
				},
				{
					true,
					false,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					false,
					false,
					true
				},
				{
					true,
					false,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					false,
					false,
					true
				},
				{
					true,
					false,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					false,
					false,
					true
				},
				{
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					false,
					false,
					true
				},
				{
					true,
					false,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					false,
					false,
					true
				},
				{
					true,
					false,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					false,
					false,
					true
				},
				{
					true,
					false,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					false,
					false,
					true
				},
				{
					true,
					false,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					false,
					false,
					true
				},
				{
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					false,
					false,
					true
				},
				{
					true,
					false,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					false,
					false,
					true
				},
				{
					true,
					false,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					false,
					false,
					true
				},
				{
					true,
					false,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					false,
					false,
					true
				},
				{
					true,
					false,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					false,
					false,
					true
				},
				{
					true,
					false,
					true,
					false,
					false,
					false,
					true,
					true,
					true,
					false,
					true,
					true,
					true,
					true,
					true,
					false,
					false,
					true
				},
				{
					true,
					false,
					true,
					false,
					false,
					false,
					true,
					true,
					true,
					false,
					true,
					true,
					true,
					false,
					true,
					false,
					false,
					false
				},
				{
					true,
					false,
					true,
					false,
					false,
					false,
					true,
					true,
					true,
					false,
					true,
					true,
					true,
					false,
					true,
					false,
					false,
					false
				},
				{
					true,
					false,
					true,
					false,
					false,
					false,
					true,
					true,
					true,
					false,
					true,
					true,
					true,
					false,
					true,
					false,
					false,
					false
				},
				{
					true,
					false,
					true,
					false,
					false,
					false,
					true,
					true,
					true,
					false,
					true,
					true,
					true,
					false,
					true,
					false,
					false,
					false
				},
				{
					true,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					true,
					false,
					false,
					true,
					true
				},
				{
					true,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					true,
					false,
					false,
					true,
					true
				},
				{
					true,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					true,
					false,
					false,
					true,
					true
				},
				{
					true,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					true,
					false,
					false,
					true,
					true
				},
				{
					true,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					true,
					false,
					false,
					true,
					true
				},
				{
					true,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					true,
					false,
					false,
					true,
					true
				},
				{
					true,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					false
				},
				{
					true,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					false,
					false
				},
				{
					true,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					true,
					false,
					false,
					true,
					true
				},
				{
					true,
					false,
					false,
					false,
					true,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					false,
					false
				},
				{
					true,
					false,
					false,
					false,
					true,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					false,
					false
				},
				{
					true,
					false,
					false,
					false,
					true,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					false,
					false
				},
				{
					true,
					false,
					false,
					false,
					true,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					false,
					false
				},
				{
					true,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					false
				},
				{
					true,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					false,
					true,
					false,
					false,
					false
				},
				{
					true,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					true,
					false,
					false,
					true,
					true
				},
				{
					true,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false
				},
				{
					true,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					true,
					false,
					false,
					true,
					true
				},
				{
					true,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					true,
					false,
					false,
					true,
					true
				},
				{
					true,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					true,
					false,
					false,
					true,
					true
				},
				{
					true,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					false,
					true,
					false,
					false,
					false
				},
				{
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					true,
					false,
					false,
					false,
					true
				},
				{
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					true,
					false,
					false,
					false,
					true
				},
				{
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					false,
					false,
					false,
					false,
					false
				},
				{
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					true,
					false,
					false,
					false,
					true
				},
				{
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					false,
					false,
					false,
					false,
					false
				},
				{
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					true,
					false,
					false,
					false,
					true
				},
				{
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					false,
					false,
					false,
					false,
					false
				},
				{
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					false,
					false,
					false,
					false,
					false
				},
				{
					true,
					false,
					true,
					false,
					false,
					false,
					true,
					true,
					true,
					false,
					true,
					true,
					true,
					false,
					true,
					false,
					false,
					false
				},
				{
					true,
					false,
					true,
					false,
					false,
					false,
					true,
					true,
					true,
					false,
					true,
					true,
					true,
					false,
					true,
					false,
					false,
					false
				},
				{
					true,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					true,
					false,
					false,
					true,
					true
				}
			};
			StyleInformation.StyleNameIndexes = new Hashtable();
			StyleInformation.StyleNameIndexes.Add("BorderColor", 0);
			StyleInformation.StyleNameIndexes.Add("BorderColorLeft", 1);
			StyleInformation.StyleNameIndexes.Add("BorderColorRight", 2);
			StyleInformation.StyleNameIndexes.Add("BorderColorTop", 3);
			StyleInformation.StyleNameIndexes.Add("BorderColorBottom", 4);
			StyleInformation.StyleNameIndexes.Add("BorderStyle", 5);
			StyleInformation.StyleNameIndexes.Add("BorderStyleLeft", 6);
			StyleInformation.StyleNameIndexes.Add("BorderStyleRight", 7);
			StyleInformation.StyleNameIndexes.Add("BorderStyleTop", 8);
			StyleInformation.StyleNameIndexes.Add("BorderStyleBottom", 9);
			StyleInformation.StyleNameIndexes.Add("BorderWidth", 10);
			StyleInformation.StyleNameIndexes.Add("BorderWidthLeft", 11);
			StyleInformation.StyleNameIndexes.Add("BorderWidthRight", 12);
			StyleInformation.StyleNameIndexes.Add("BorderWidthTop", 13);
			StyleInformation.StyleNameIndexes.Add("BorderWidthBottom", 14);
			StyleInformation.StyleNameIndexes.Add("BackgroundColor", 15);
			StyleInformation.StyleNameIndexes.Add("BackgroundImageSource", 16);
			StyleInformation.StyleNameIndexes.Add("BackgroundImageValue", 17);
			StyleInformation.StyleNameIndexes.Add("BackgroundImageMIMEType", 18);
			StyleInformation.StyleNameIndexes.Add("BackgroundRepeat", 19);
			StyleInformation.StyleNameIndexes.Add("FontStyle", 20);
			StyleInformation.StyleNameIndexes.Add("FontFamily", 21);
			StyleInformation.StyleNameIndexes.Add("FontSize", 22);
			StyleInformation.StyleNameIndexes.Add("FontWeight", 23);
			StyleInformation.StyleNameIndexes.Add("Format", 24);
			StyleInformation.StyleNameIndexes.Add("TextDecoration", 25);
			StyleInformation.StyleNameIndexes.Add("TextAlign", 26);
			StyleInformation.StyleNameIndexes.Add("VerticalAlign", 27);
			StyleInformation.StyleNameIndexes.Add("Color", 28);
			StyleInformation.StyleNameIndexes.Add("PaddingLeft", 29);
			StyleInformation.StyleNameIndexes.Add("PaddingRight", 30);
			StyleInformation.StyleNameIndexes.Add("PaddingTop", 31);
			StyleInformation.StyleNameIndexes.Add("PaddingBottom", 32);
			StyleInformation.StyleNameIndexes.Add("LineHeight", 33);
			StyleInformation.StyleNameIndexes.Add("Direction", 34);
			StyleInformation.StyleNameIndexes.Add("Language", 35);
			StyleInformation.StyleNameIndexes.Add("UnicodeBiDi", 36);
			StyleInformation.StyleNameIndexes.Add("Calendar", 37);
			StyleInformation.StyleNameIndexes.Add("NumeralLanguage", 38);
			StyleInformation.StyleNameIndexes.Add("NumeralVariant", 39);
			StyleInformation.StyleNameIndexes.Add("WritingMode", 40);
			StyleInformation.StyleNameIndexes.Add("BackgroundGradientType", 41);
			StyleInformation.StyleNameIndexes.Add("BackgroundGradientEndColor", 42);
			StyleInformation.StyleNameIndexes.Add("TextEffect", 43);
			StyleInformation.StyleNameIndexes.Add("BackgroundHatchType", 44);
			StyleInformation.StyleNameIndexes.Add("ShadowColor", 45);
			StyleInformation.StyleNameIndexes.Add("ShadowOffset", 46);
			StyleInformation.StyleNameIndexes.Add("TransparentColor", 47);
			StyleInformation.StyleNameIndexes.Add("Position", 48);
			StyleInformation.StyleNameIndexes.Add("EmbeddingMode", 49);
			StyleInformation.StyleNameIndexes.Add("Transparency", 50);
			StyleInformation.StyleNameIndexes.Add("CurrencyLanguage", 51);
		}

		internal void AddAttribute(string name, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.AddAttribute(name, expression, ValueType.Constant);
		}

		internal void AddAttribute(string name, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ValueType valueType)
		{
			Global.Tracer.Assert(null != name);
			Global.Tracer.Assert(null != expression);
			this.m_attributes.Add(new StyleInformationAttribute
			{
				Name = name,
				Value = expression,
				ValueType = valueType
			});
		}

		internal void RemoveAttribute(string name)
		{
			Global.Tracer.Assert(null != name);
			this.m_attributes.RemoveAll((StyleInformationAttribute a) => a.Name == name);
		}

		internal StyleInformationAttribute GetAttributeByName(string name)
		{
			Global.Tracer.Assert(null != name);
			return this.m_attributes.SingleOrDefault((StyleInformationAttribute a) => a.Name == name);
		}

		internal void Filter(StyleOwnerType ownerType, bool hasNoRows)
		{
			int ownerType2 = this.MapStyleOwnerTypeToIndex(ownerType, hasNoRows);
			for (int num = this.m_attributes.Count - 1; num >= 0; num--)
			{
				if (!this.Allow(this.MapStyleNameToIndex(this.m_attributes[num].Name), ownerType2))
				{
					this.m_attributes.RemoveAt(num);
				}
			}
		}

		internal void FilterChartLegendTitleStyle()
		{
			int ownerType = this.MapStyleOwnerTypeToIndex(StyleOwnerType.Chart, false);
			for (int num = this.m_attributes.Count - 1; num >= 0; num--)
			{
				string name = this.m_attributes[num].Name;
				if (!this.Allow(this.MapStyleNameToIndex(name), ownerType) && name != "TextAlign")
				{
					this.m_attributes.RemoveAt(num);
				}
			}
		}

		internal void FilterChartStripLineStyle()
		{
			int ownerType = this.MapStyleOwnerTypeToIndex(StyleOwnerType.Chart, false);
			for (int num = this.m_attributes.Count - 1; num >= 0; num--)
			{
				string name = this.m_attributes[num].Name;
				if (!this.Allow(this.MapStyleNameToIndex(name), ownerType) && name != "VerticalAlign" && name != "TextAlign")
				{
					this.m_attributes.RemoveAt(num);
				}
			}
		}

		internal void FilterChartSeriesStyle()
		{
			this.MapStyleOwnerTypeToIndex(StyleOwnerType.Chart, false);
			for (int num = this.m_attributes.Count - 1; num >= 0; num--)
			{
				string name = this.m_attributes[num].Name;
				if (name != "ShadowColor" && name != "ShadowOffset")
				{
					this.m_attributes.RemoveAt(num);
				}
			}
		}

		internal void FilterGaugeLabelStyle()
		{
			int ownerType = this.MapStyleOwnerTypeToIndex(StyleOwnerType.GaugePanel, false);
			for (int num = this.m_attributes.Count - 1; num >= 0; num--)
			{
				string name = this.m_attributes[num].Name;
				if (!this.Allow(this.MapStyleNameToIndex(name), ownerType) && name != "VerticalAlign" && name != "TextAlign")
				{
					this.m_attributes.RemoveAt(num);
				}
			}
		}

		internal void FilterMapTitleStyle()
		{
			int ownerType = this.MapStyleOwnerTypeToIndex(StyleOwnerType.Map, false);
			for (int num = this.m_attributes.Count - 1; num >= 0; num--)
			{
				string name = this.m_attributes[num].Name;
				if (!this.Allow(this.MapStyleNameToIndex(name), ownerType) && name != "VerticalAlign" && name != "TextAlign")
				{
					this.m_attributes.RemoveAt(num);
				}
			}
		}

		internal void FilterMapLegendTitleStyle()
		{
			int ownerType = this.MapStyleOwnerTypeToIndex(StyleOwnerType.Map, false);
			for (int num = this.m_attributes.Count - 1; num >= 0; num--)
			{
				string name = this.m_attributes[num].Name;
				if (!this.Allow(this.MapStyleNameToIndex(name), ownerType) && name != "TextAlign")
				{
					this.m_attributes.RemoveAt(num);
				}
			}
		}

		private int MapStyleOwnerTypeToIndex(StyleOwnerType ownerType, bool hasNoRows)
		{
			if (hasNoRows)
			{
				return 0;
			}
			switch (ownerType)
			{
			case StyleOwnerType.PageSection:
				return 2;
			case StyleOwnerType.SubReport:
			case StyleOwnerType.Subtotal:
				return 0;
			default:
				return (int)ownerType;
			}
		}

		private int MapStyleNameToIndex(string name)
		{
			return (int)StyleInformation.StyleNameIndexes[name];
		}

		private bool Allow(int styleName, int ownerType)
		{
			return StyleInformation.AllowStyleAttributeByType[styleName, ownerType];
		}
	}
}
