using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;

namespace AspNetCore.Reporting.Map.WebForms
{
	[TypeConverter(typeof(ShapeRuleConverter))]
	internal class ShapeRule : RuleBase
	{
		private CustomColorCollection customColors;

		private string shapeField = "(Name)";

		private string category = string.Empty;

		private string showInLegend = "(none)";

		private bool showInColorSwatch;

		private string legendText = "#FROMVALUE{N0} - #TOVALUE{N0}";

		private int colorCount = 5;

		private Color fromColor = Color.Green;

		private Color middleColor = Color.Yellow;

		private Color toColor = Color.Red;

		private string fromValue = "";

		private string toValue = "";

		private bool useCustomColors;

		private ColoringMode coloringMode = ColoringMode.DistinctColors;

		private DataGrouping dataGrouping = DataGrouping.Optimal;

		private MapColorPalette colorPalette;

		private Color borderColor = Color.DarkGray;

		private Color secondaryColor = Color.Empty;

		private GradientType gradientType;

		private MapHatchStyle hatchStyle;

		private string text = "#NAME";

		private string toolTip = "";

		[SRCategory("CategoryAttribute_CustomColors")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRDescription("DescriptionAttributeShapeRule_CustomColors")]
		public CustomColorCollection CustomColors
		{
			get
			{
				return this.customColors;
			}
		}

		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeShapeRule_Name")]
		public override string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}

		[DefaultValue("(Name)")]
		[RefreshProperties(RefreshProperties.All)]
		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeShapeRule_ShapeField")]
		[TypeConverter(typeof(DesignTimeFieldConverter))]
		[ParenthesizePropertyName(true)]
		public string ShapeField
		{
			get
			{
				return this.shapeField;
			}
			set
			{
				this.shapeField = value;
				this.InvalidateRules();
			}
		}

		internal override string Field
		{
			get
			{
				return this.ShapeField;
			}
			set
			{
				this.ShapeField = value;
			}
		}

		[SRDescription("DescriptionAttributeShapeRule_Category")]
		[SRCategory("CategoryAttribute_Data")]
		[DefaultValue("")]
		public override string Category
		{
			get
			{
				return this.category;
			}
			set
			{
				this.category = value;
				this.InvalidateRules();
			}
		}

		[TypeConverter(typeof(DesignTimeLegendConverter))]
		[SRDescription("DescriptionAttributeShapeRule_ShowInLegend")]
		[SRCategory("CategoryAttribute_Legend")]
		[RefreshProperties(RefreshProperties.All)]
		[ParenthesizePropertyName(true)]
		[DefaultValue("(none)")]
		public override string ShowInLegend
		{
			get
			{
				return this.showInLegend;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this.showInLegend = "(none)";
				}
				else
				{
					this.showInLegend = value;
				}
				this.InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Misc")]
		[SRDescription("DescriptionAttributeShapeRule_ShowInColorSwatch")]
		[DefaultValue(false)]
		public bool ShowInColorSwatch
		{
			get
			{
				return this.showInColorSwatch;
			}
			set
			{
				this.showInColorSwatch = value;
				this.InvalidateRules();
			}
		}

		[DefaultValue("#FROMVALUE{N0} - #TOVALUE{N0}")]
		[SRDescription("DescriptionAttributeShapeRule_LegendText")]
		[SRCategory("CategoryAttribute_Legend")]
		public override string LegendText
		{
			get
			{
				return this.legendText;
			}
			set
			{
				this.legendText = value;
				this.InvalidateRules();
			}
		}

		[DefaultValue(5)]
		[SRDescription("DescriptionAttributeShapeRule_ColorCount")]
		[SRCategory("CategoryAttribute_Colors")]
		public int ColorCount
		{
			get
			{
				return this.colorCount;
			}
			set
			{
				if (value < 1)
				{
					throw new ArgumentException(SR.ExceptionValueCannotBeLessThanOne);
				}
				this.colorCount = value;
				this.InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Colors")]
		[DefaultValue(typeof(Color), "Green")]
		[SRDescription("DescriptionAttributeShapeRule_FromColor")]
		public Color FromColor
		{
			get
			{
				return this.fromColor;
			}
			set
			{
				this.fromColor = value;
				this.InvalidateRules();
			}
		}

		[SRDescription("DescriptionAttributeShapeRule_MiddleColor")]
		[SRCategory("CategoryAttribute_Colors")]
		[DefaultValue(typeof(Color), "Yellow")]
		public Color MiddleColor
		{
			get
			{
				return this.middleColor;
			}
			set
			{
				this.middleColor = value;
				this.InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Colors")]
		[SRDescription("DescriptionAttributeShapeRule_ToColor")]
		[DefaultValue(typeof(Color), "Red")]
		public Color ToColor
		{
			get
			{
				return this.toColor;
			}
			set
			{
				this.toColor = value;
				this.InvalidateRules();
			}
		}

		[TypeConverter(typeof(StringConverter))]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeShapeRule_FromValue")]
		[SRCategory("CategoryAttribute_Values")]
		public override string FromValue
		{
			get
			{
				return this.fromValue;
			}
			set
			{
				this.fromValue = value;
				this.InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Values")]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeShapeRule_ToValue")]
		[TypeConverter(typeof(StringConverter))]
		public override string ToValue
		{
			get
			{
				return this.toValue;
			}
			set
			{
				this.toValue = value;
				this.InvalidateRules();
			}
		}

		[DefaultValue(false)]
		[RefreshProperties(RefreshProperties.All)]
		[SRCategory("CategoryAttribute_CustomColors")]
		[SRDescription("DescriptionAttributeShapeRule_UseCustomColors")]
		public bool UseCustomColors
		{
			get
			{
				return this.useCustomColors;
			}
			set
			{
				this.useCustomColors = value;
				this.InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Colors")]
		[DefaultValue(ColoringMode.DistinctColors)]
		[SRDescription("DescriptionAttributeShapeRule_ColoringMode")]
		[RefreshProperties(RefreshProperties.All)]
		public ColoringMode ColoringMode
		{
			get
			{
				return this.coloringMode;
			}
			set
			{
				this.coloringMode = value;
				this.InvalidateRules();
			}
		}

		[DefaultValue(DataGrouping.Optimal)]
		[SRDescription("DescriptionAttributeShapeRule_DataGrouping")]
		[SRCategory("CategoryAttribute_Data")]
		internal override DataGrouping DataGrouping
		{
			get
			{
				return this.dataGrouping;
			}
			set
			{
				this.dataGrouping = value;
				this.InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Colors")]
		[SRDescription("DescriptionAttributeShapeRule_ColorPalette")]
		[DefaultValue(MapColorPalette.Random)]
		public MapColorPalette ColorPalette
		{
			get
			{
				return this.colorPalette;
			}
			set
			{
				this.colorPalette = value;
				this.InvalidateRules();
			}
		}

		[DefaultValue(typeof(Color), "DarkGray")]
		[SRDescription("DescriptionAttributeShapeRule_BorderColor")]
		[SRCategory("CategoryAttribute_Misc")]
		public Color BorderColor
		{
			get
			{
				return this.borderColor;
			}
			set
			{
				this.borderColor = value;
				this.InvalidateRules();
			}
		}

		[SRDescription("DescriptionAttributeShapeRule_SecondaryColor")]
		[DefaultValue(typeof(Color), "")]
		[SRCategory("CategoryAttribute_Misc")]
		public Color SecondaryColor
		{
			get
			{
				return this.secondaryColor;
			}
			set
			{
				this.secondaryColor = value;
				this.InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Misc")]
		[SRDescription("DescriptionAttributeShapeRule_GradientType")]
		//[Editor(typeof(GradientEditor), typeof(UITypeEditor))]
		[DefaultValue(GradientType.None)]
		public GradientType GradientType
		{
			get
			{
				return this.gradientType;
			}
			set
			{
				this.gradientType = value;
				this.InvalidateRules();
			}
		}

		[DefaultValue(MapHatchStyle.None)]
		[SRDescription("DescriptionAttributeShapeRule_HatchStyle")]
		//[Editor(typeof(HatchStyleEditor), typeof(UITypeEditor))]
		[SRCategory("CategoryAttribute_Misc")]
		public MapHatchStyle HatchStyle
		{
			get
			{
				return this.hatchStyle;
			}
			set
			{
				this.hatchStyle = value;
				this.InvalidateRules();
			}
		}

		[TypeConverter(typeof(KeywordConverter))]
		[SRDescription("DescriptionAttributeShapeRule_Text")]
		[SRCategory("CategoryAttribute_Misc")]
		[Localizable(true)]
		[DefaultValue("#NAME")]
		public string Text
		{
			get
			{
				return this.text;
			}
			set
			{
				this.text = value;
				this.InvalidateRules();
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryAttribute_Misc")]
		[SRDescription("DescriptionAttributeShapeRule_ToolTip")]
		[Localizable(true)]
		[TypeConverter(typeof(KeywordConverter))]
		[DefaultValue("")]
		public string ToolTip
		{
			get
			{
				return this.toolTip;
			}
			set
			{
				this.toolTip = value;
				this.InvalidateRules();
			}
		}

		internal override CommonElements Common
		{
			get
			{
				return base.Common;
			}
			set
			{
				base.Common = value;
				if (this.customColors != null)
				{
					this.customColors.Common = value;
				}
			}
		}

		public ShapeRule()
			: this(null)
		{
		}

		internal ShapeRule(CommonElements common)
			: base(common)
		{
			this.customColors = new CustomColorCollection(this, common);
		}

		public override string ToString()
		{
			return this.Name;
		}

		internal override void OnAdded()
		{
			base.OnAdded();
			this.InvalidateRules();
		}

		internal void InvalidateRules()
		{
			MapCore mapCore = base.GetMapCore();
			if (mapCore != null)
			{
				mapCore.InvalidateRules();
				mapCore.Invalidate();
			}
		}

		internal override void OnRemove()
		{
			base.OnRemove();
			this.InvalidateRules();
		}

		internal void RegenerateColorRanges()
		{
			if (this.UseCustomColors)
			{
				foreach (CustomColor customColor10 in this.CustomColors)
				{
					customColor10.AffectedElements.Clear();
				}
			}
			MapCore mapCore = base.GetMapCore();
			if (mapCore != null && mapCore.Shapes.Count != 0 && (!this.UseCustomColors || this.CustomColors.Count != 0))
			{
				if (!this.UseCustomColors)
				{
					this.CustomColors.Clear();
				}
				if (this.ShapeField == "(Name)")
				{
					if (this.UseCustomColors)
					{
						int num = 0;
						foreach (Shape shape4 in mapCore.Shapes)
						{
							if (num == this.CustomColors.Count)
							{
								break;
							}
							CustomColor customColor2 = this.CustomColors[num++];
							customColor2.FromValueInt = shape4.Name;
							customColor2.ToValueInt = shape4.Name;
						}
					}
					else
					{
						Color[] colors = base.GetColors(this.ColoringMode, this.ColorPalette, this.FromColor, this.MiddleColor, this.ToColor, mapCore.Shapes.Count);
						int num3 = 0;
						foreach (Shape shape5 in mapCore.Shapes)
						{
							if (shape5.Category == this.Category)
							{
								CustomColor customColor3 = this.CustomColors.Add(string.Empty);
								customColor3.Color = colors[num3++];
								customColor3.SecondaryColor = this.SecondaryColor;
								customColor3.BorderColor = this.BorderColor;
								customColor3.GradientType = this.GradientType;
								customColor3.HatchStyle = this.HatchStyle;
								customColor3.FromValueInt = shape5.Name;
								customColor3.ToValueInt = shape5.Name;
								customColor3.Text = this.Text;
								customColor3.ToolTip = this.ToolTip;
							}
						}
					}
					this.UpdateColorSwatchAndLegend();
				}
				else
				{
					Field field = this.GetField();
					if (field != null)
					{
						if (field.IsNumeric())
						{
							int intervalCount = (!this.UseCustomColors) ? this.ColorCount : this.CustomColors.Count;
							object obj = null;
							object obj2 = null;
							if (this.FromValue != string.Empty)
							{
								obj = field.Parse(this.FromValue);
							}
							if (this.ToValue != string.Empty)
							{
								obj2 = field.Parse(this.ToValue);
							}
							if (obj == null || obj2 == null)
							{
								this.GetRangeFromShapes(field, intervalCount, ref obj, ref obj2);
							}
							object[] array = null;
							object[] array2 = null;
							if (this.DataGrouping == DataGrouping.EqualInterval)
							{
								base.GetEqualIntervals(field, obj, obj2, intervalCount, ref array, ref array2);
							}
							else if (this.DataGrouping == DataGrouping.EqualDistribution)
							{
								ArrayList sortedValues = this.GetSortedValues(field, obj, obj2);
								base.GetEqualDistributionIntervals(field, sortedValues, obj, obj2, intervalCount, ref array, ref array2);
							}
							else if (this.DataGrouping == DataGrouping.Optimal)
							{
								ArrayList sortedValues2 = this.GetSortedValues(field, obj, obj2);
								base.GetOptimalIntervals(field, sortedValues2, obj, obj2, intervalCount, ref array, ref array2);
							}
							if (this.UseCustomColors)
							{
								int num5 = 0;
								foreach (CustomColor customColor11 in this.CustomColors)
								{
									if (num5 < array.Length)
									{
										customColor11.FromValueInt = AspNetCore.Reporting.Map.WebForms.Field.ToStringInvariant(array[num5]);
										customColor11.ToValueInt = AspNetCore.Reporting.Map.WebForms.Field.ToStringInvariant(array2[num5]);
										customColor11.VisibleInt = true;
									}
									else
									{
										customColor11.FromValueInt = AspNetCore.Reporting.Map.WebForms.Field.ToStringInvariant(array2[array2.Length - 1]);
										customColor11.ToValueInt = AspNetCore.Reporting.Map.WebForms.Field.ToStringInvariant(array2[array2.Length - 1]);
										customColor11.VisibleInt = false;
									}
									num5++;
								}
							}
							else
							{
								Color[] colors2 = base.GetColors(this.ColoringMode, this.ColorPalette, this.FromColor, this.MiddleColor, this.ToColor, array.Length);
								for (int i = 0; i < array.Length; i++)
								{
									CustomColor customColor5 = this.CustomColors.Add(string.Empty);
									customColor5.Color = colors2[i];
									customColor5.SecondaryColor = this.SecondaryColor;
									customColor5.BorderColor = this.BorderColor;
									customColor5.GradientType = this.GradientType;
									customColor5.HatchStyle = this.HatchStyle;
									customColor5.FromValueInt = AspNetCore.Reporting.Map.WebForms.Field.ToStringInvariant(array[i]);
									customColor5.ToValueInt = AspNetCore.Reporting.Map.WebForms.Field.ToStringInvariant(array2[i]);
									customColor5.Text = this.Text;
									customColor5.ToolTip = this.ToolTip;
								}
							}
						}
						else if (field.Type == typeof(string))
						{
							Hashtable hashtable = new Hashtable();
							foreach (Shape shape6 in base.GetMapCore().Shapes)
							{
								if (shape6.Category == this.Category)
								{
									string text = (string)shape6[field.Name];
									if (text != null)
									{
										hashtable[text] = 0;
									}
								}
							}
							if (this.UseCustomColors)
							{
								ArrayList arrayList = new ArrayList();
								arrayList.AddRange(hashtable.Keys);
								arrayList.Sort();
								int num6 = 0;
								foreach (object item in arrayList)
								{
									if (num6 == this.CustomColors.Count)
									{
										break;
									}
									CustomColor customColor6 = this.CustomColors[num6++];
									customColor6.FromValueInt = (string)item;
									customColor6.ToValueInt = (string)item;
								}
							}
							else
							{
								Color[] colors3 = base.GetColors(this.ColoringMode, this.ColorPalette, this.FromColor, this.MiddleColor, this.ToColor, hashtable.Keys.Count);
								int num8 = 0;
								foreach (object key in hashtable.Keys)
								{
									CustomColor customColor7 = this.CustomColors.Add(string.Empty);
									customColor7.Color = colors3[num8++];
									customColor7.SecondaryColor = this.SecondaryColor;
									customColor7.BorderColor = this.BorderColor;
									customColor7.GradientType = this.GradientType;
									customColor7.HatchStyle = this.HatchStyle;
									customColor7.FromValueInt = (string)key;
									customColor7.ToValueInt = (string)key;
									customColor7.Text = this.Text;
									customColor7.ToolTip = this.ToolTip;
								}
							}
						}
						else if (this.UseCustomColors)
						{
							this.CustomColors[0].FromValueInt = "False";
							this.CustomColors[0].ToValueInt = "False";
							if (this.CustomColors.Count > 1)
							{
								this.CustomColors[1].FromValueInt = "True";
								this.CustomColors[1].ToValueInt = "True";
							}
						}
						else
						{
							CustomColor customColor8 = this.CustomColors.Add(string.Empty);
							customColor8.Color = this.FromColor;
							customColor8.SecondaryColor = this.SecondaryColor;
							customColor8.BorderColor = this.BorderColor;
							customColor8.GradientType = this.GradientType;
							customColor8.HatchStyle = this.HatchStyle;
							customColor8.FromValueInt = "False";
							customColor8.ToValueInt = "False";
							customColor8.Text = this.Text;
							customColor8.ToolTip = this.ToolTip;
							CustomColor customColor9 = this.CustomColors.Add(string.Empty);
							customColor9.Color = this.ToColor;
							customColor9.SecondaryColor = this.SecondaryColor;
							customColor9.BorderColor = this.BorderColor;
							customColor9.GradientType = this.GradientType;
							customColor9.HatchStyle = this.HatchStyle;
							customColor9.FromValueInt = "True";
							customColor9.ToValueInt = "True";
							customColor9.Text = this.Text;
							customColor9.ToolTip = this.ToolTip;
						}
						this.UpdateColorSwatchAndLegend();
					}
				}
			}
		}

		internal void UpdateColorSwatchAndLegend()
		{
			MapCore mapCore = base.GetMapCore();
			if (mapCore != null)
			{
				Field field = this.GetField();
				if (this.ShowInColorSwatch)
				{
					foreach (CustomColor customColor3 in this.CustomColors)
					{
						if (customColor3.VisibleInt)
						{
							SwatchColor swatchColor = mapCore.ColorSwatchPanel.Colors.Add("");
							swatchColor.automaticallyAdded = true;
							swatchColor.Color = customColor3.Color;
							swatchColor.SecondaryColor = customColor3.SecondaryColor;
							swatchColor.GradientType = customColor3.GradientType;
							swatchColor.HatchStyle = customColor3.HatchStyle;
							if (field != null && field.IsNumeric())
							{
								swatchColor.FromValue = field.ConvertToDouble(field.Parse(customColor3.FromValueInt));
								swatchColor.ToValue = field.ConvertToDouble(field.Parse(customColor3.ToValueInt));
							}
							else
							{
								swatchColor.TextValue = customColor3.FromValueInt;
							}
						}
					}
				}
				if (this.ShowInLegend != string.Empty && this.ShowInLegend != "(none)")
				{
					Legend legend = (Legend)mapCore.Legends.GetByName(this.ShowInLegend);
					if (legend != null)
					{
						foreach (CustomColor customColor4 in this.CustomColors)
						{
							if (customColor4.VisibleInt)
							{
								LegendItem legendItem = legend.Items.Add("");
								legendItem.automaticallyAdded = true;
								legendItem.ItemStyle = LegendItemStyle.Shape;
								legendItem.Color = customColor4.Color;
								legendItem.SecondaryColor = customColor4.SecondaryColor;
								legendItem.GradientType = customColor4.GradientType;
								legendItem.HatchStyle = customColor4.HatchStyle;
								legendItem.Text = this.GetLegendText(field, customColor4.FromValueInt, customColor4.ToValueInt);
							}
						}
					}
				}
			}
		}

		internal string GetLegendText(Field field, string fromValue, string toValue)
		{
			object obj = null;
			object obj2 = null;
			if (field != null)
			{
				obj = field.Parse(fromValue);
				obj2 = field.Parse(toValue);
			}
			else
			{
				obj = fromValue;
				obj2 = toValue;
			}
			string empty = string.Empty;
			empty = ((!(this.LegendText == "#FROMVALUE{N0} - #TOVALUE{N0}") || (field != null && field.IsNumeric())) ? this.LegendText : "#VALUE");
			MapCore mapCore = base.GetMapCore();
			empty = mapCore.ResolveKeyword(empty, "#FROMVALUE", obj);
			empty = mapCore.ResolveKeyword(empty, "#TOVALUE", obj2);
			return mapCore.ResolveKeyword(empty, "#VALUE", obj);
		}

		internal void Apply(Shape shape)
		{
			if (!(this.Category != shape.Category))
			{
				if (this.ShapeField == "(Name)")
				{
					foreach (CustomColor customColor3 in this.CustomColors)
					{
						if (customColor3.FromValueInt == shape.Name)
						{
							shape.ApplyCustomColorAttributes(customColor3);
							customColor3.AffectedElements.Add(shape);
							break;
						}
					}
				}
				else
				{
					Field field = this.GetField();
					if (field != null)
					{
						object testValue = shape[field.Name];
						foreach (CustomColor customColor4 in this.CustomColors)
						{
							object obj = field.Parse(customColor4.FromValueInt);
							object obj2 = field.Parse(customColor4.ToValueInt);
							if (base.IsValueInRange(field, testValue, obj, obj2))
							{
								shape.ApplyCustomColorAttributes(customColor4);
								customColor4.AffectedElements.Add(shape);
								break;
							}
						}
					}
				}
			}
		}

		internal override Field GetField()
		{
			MapCore mapCore = base.GetMapCore();
			if (mapCore == null)
			{
				return null;
			}
			return (Field)mapCore.ShapeFields.GetByName(this.ShapeField);
		}

		internal void GetRangeFromShapes(Field field, int intervalCount, ref object fromValue, ref object toValue)
		{
			MapCore mapCore = base.GetMapCore();
			if (mapCore != null)
			{
				if (field.Type == typeof(int))
				{
					int num = 2147483647;
					int num2 = -2147483648;
					foreach (Shape shape6 in mapCore.Shapes)
					{
						if (!(this.Category != shape6.Category))
						{
							object obj = shape6[field.Name];
							if (obj != null)
							{
								int val = Convert.ToInt32(obj, CultureInfo.InvariantCulture);
								num = Math.Min(num, val);
								num2 = Math.Max(num2, val);
							}
						}
					}
					if (num == 2147483647)
					{
						num = 0;
					}
					if (num2 == -2147483648)
					{
						num2 = 10;
					}
					if (fromValue == null)
					{
						fromValue = num;
					}
					if (toValue == null)
					{
						toValue = num2;
					}
				}
				else if (field.Type == typeof(double))
				{
					double num3 = 1.7976931348623157E+308;
					double num4 = -1.7976931348623157E+308;
					foreach (Shape shape7 in mapCore.Shapes)
					{
						if (!(this.Category != shape7.Category))
						{
							object obj2 = shape7[field.Name];
							if (obj2 != null)
							{
								double val2 = Convert.ToDouble(obj2, CultureInfo.InvariantCulture);
								num3 = Math.Min(num3, val2);
								num4 = Math.Max(num4, val2);
							}
						}
					}
					if (num3 == 1.7976931348623157E+308)
					{
						num3 = 0.0;
					}
					if (num4 == -1.7976931348623157E+308)
					{
						num4 = 10.0;
					}
					if (num3 != num4 && this.DataGrouping == DataGrouping.EqualInterval)
					{
						int num5 = (int)Math.Log10(Math.Max(Math.Abs(num3), Math.Abs(num4)));
						double num6 = Math.Pow(10.0, (double)(num5 - 1));
						num3 /= num6;
						num3 = Math.Floor(num3);
						int num7 = (int)num3 * 10;
						num3 *= num6;
						num4 /= num6;
						num4 = Math.Ceiling(num4);
						int num8 = (int)num4 * 10 - num7;
						int num9 = num8 % intervalCount;
						num4 += (double)(intervalCount - num9) / 10.0;
						num4 *= num6;
					}
					if (fromValue == null)
					{
						fromValue = num3;
					}
					if (toValue == null)
					{
						toValue = num4;
					}
				}
				else if (field.Type == typeof(decimal))
				{
					decimal num10 = 79228162514264337593543950335m;
					decimal num11 = -79228162514264337593543950335m;
					foreach (Shape shape8 in mapCore.Shapes)
					{
						if (!(this.Category != shape8.Category))
						{
							object obj3 = shape8[field.Name];
							if (obj3 != null)
							{
								decimal val3 = Convert.ToDecimal(obj3, CultureInfo.InvariantCulture);
								num10 = Math.Min(num10, val3);
								num11 = Math.Max(num11, val3);
							}
						}
					}
					if (num10 == 79228162514264337593543950335m)
					{
						num10 = 0m;
					}
					if (num11 == -79228162514264337593543950335m)
					{
						num11 = 10m;
					}
					if (num10 != num11 && this.DataGrouping == DataGrouping.EqualInterval)
					{
						int num12 = (int)Math.Log10((double)Math.Max(Math.Abs(num10), Math.Abs(num11)));
						decimal d = (decimal)Math.Pow(10.0, (double)(num12 - 1));
						num10 /= d;
						num10 = decimal.Floor(num10);
						int num13 = (int)num10 * 10;
						num10 *= d;
						num11 /= d;
						num11 = Math.Ceiling(num11);
						int num14 = (int)num11 * 10 - num13;
						int num15 = num14 % intervalCount;
						num11 += (decimal)(intervalCount - num15) / 10m;
						num11 *= d;
					}
					if (fromValue == null)
					{
						fromValue = num10;
					}
					if (toValue == null)
					{
						toValue = num11;
					}
				}
				else if (field.Type == typeof(DateTime))
				{
					DateTime dateTime = DateTime.MaxValue;
					DateTime dateTime2 = DateTime.MinValue;
					foreach (Shape shape9 in mapCore.Shapes)
					{
						if (!(this.Category != shape9.Category))
						{
							object obj4 = shape9[field.Name];
							if (obj4 != null)
							{
								DateTime dateTime3 = Convert.ToDateTime(obj4, CultureInfo.InvariantCulture);
								if (dateTime > dateTime3)
								{
									dateTime = dateTime3;
								}
								if (dateTime2 < dateTime3)
								{
									dateTime2 = dateTime3;
								}
							}
						}
					}
					if (dateTime == DateTime.MaxValue)
					{
						dateTime = DateTime.MinValue;
					}
					if (dateTime2 == DateTime.MinValue)
					{
						dateTime2 = DateTime.MaxValue;
					}
					if (fromValue == null)
					{
						fromValue = dateTime;
					}
					if (toValue == null)
					{
						toValue = dateTime2;
					}
				}
				else if (field.Type == typeof(TimeSpan))
				{
					TimeSpan timeSpan = TimeSpan.MaxValue;
					TimeSpan timeSpan2 = TimeSpan.MinValue;
					foreach (Shape shape10 in mapCore.Shapes)
					{
						if (!(this.Category != shape10.Category))
						{
							object obj5 = shape10[field.Name];
							if (obj5 != null)
							{
								TimeSpan timeSpan3 = (TimeSpan)obj5;
								if (timeSpan > timeSpan3)
								{
									timeSpan = timeSpan3;
								}
								if (timeSpan2 < timeSpan3)
								{
									timeSpan2 = timeSpan3;
								}
							}
						}
					}
					if (timeSpan == TimeSpan.MaxValue)
					{
						timeSpan = TimeSpan.MinValue;
					}
					if (timeSpan2 == TimeSpan.MinValue)
					{
						timeSpan2 = TimeSpan.MaxValue;
					}
					if (fromValue == null)
					{
						fromValue = timeSpan;
					}
					if (toValue == null)
					{
						toValue = timeSpan2;
					}
				}
			}
		}

		internal override ArrayList GetSortedValues(Field field, object fromValue, object toValue)
		{
			MapCore mapCore = base.GetMapCore();
			if (mapCore == null)
			{
				return null;
			}
			ArrayList arrayList = new ArrayList();
			foreach (Shape shape in mapCore.Shapes)
			{
				if (shape.Category == this.Category)
				{
					object obj = shape[field.Name];
					if (obj != null && base.IsValueInRange(field, obj, fromValue, toValue))
					{
						arrayList.Add(obj);
					}
				}
			}
			arrayList.Sort();
			return arrayList;
		}
	}
}
