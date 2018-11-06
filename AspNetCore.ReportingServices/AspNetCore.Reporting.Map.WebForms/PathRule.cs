using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace AspNetCore.Reporting.Map.WebForms
{
	[TypeConverter(typeof(PathRuleConverter))]
	internal class PathRule : PathRuleBase
	{
		private CustomColorCollection customColors;

		private bool showInColorSwatch;

		private Color fromColor = Color.Green;

		private Color middleColor = Color.Yellow;

		private Color toColor = Color.Red;

		private string fromValue = "";

		private string toValue = "";

		private bool useCustomColors;

		private MapColorPalette colorPalette;

		private Color borderColor = Color.DarkGray;

		private Color secondaryColor = Color.Empty;

		private GradientType gradientType;

		private MapHatchStyle hatchStyle;

		private int widthInLegend = 5;

		[SRCategory("CategoryAttribute_CustomColors")]
		[SRDescription("DescriptionAttributePathRule_CustomColors")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CustomColorCollection CustomColors
		{
			get
			{
				return this.customColors;
			}
		}

		[DefaultValue(false)]
		[SRDescription("DescriptionAttributePathRule_ShowInColorSwatch")]
		[SRCategory("CategoryAttribute_Misc")]
		public bool ShowInColorSwatch
		{
			get
			{
				return this.showInColorSwatch;
			}
			set
			{
				this.showInColorSwatch = value;
				base.InvalidateRules();
			}
		}

		[DefaultValue(5)]
		[SRDescription("DescriptionAttributePathRule_ColorCount")]
		[SRCategory("CategoryAttribute_Colors")]
		public override int ColorCount
		{
			get
			{
				return base.ColorCount;
			}
			set
			{
				if (value < 1)
				{
					throw new ArgumentException(SR.ExceptionPropertyValueCannotbeLessThanOne);
				}
				base.ColorCount = value;
				base.InvalidateRules();
			}
		}

		[DefaultValue(typeof(Color), "Green")]
		[SRCategory("CategoryAttribute_Colors")]
		[SRDescription("DescriptionAttributePathRule_FromColor")]
		public Color FromColor
		{
			get
			{
				return this.fromColor;
			}
			set
			{
				this.fromColor = value;
				base.InvalidateRules();
			}
		}

		[SRDescription("DescriptionAttributePathRule_MiddleColor")]
		[DefaultValue(typeof(Color), "Yellow")]
		[SRCategory("CategoryAttribute_Colors")]
		public Color MiddleColor
		{
			get
			{
				return this.middleColor;
			}
			set
			{
				this.middleColor = value;
				base.InvalidateRules();
			}
		}

		[DefaultValue(typeof(Color), "Red")]
		[SRCategory("CategoryAttribute_Colors")]
		[SRDescription("DescriptionAttributePathRule_ToColor")]
		public Color ToColor
		{
			get
			{
				return this.toColor;
			}
			set
			{
				this.toColor = value;
				base.InvalidateRules();
			}
		}

		[DefaultValue("")]
		[TypeConverter(typeof(StringConverter))]
		[SRCategory("CategoryAttribute_Values")]
		[SRDescription("DescriptionAttributePathRule_FromValue")]
		public override string FromValue
		{
			get
			{
				return this.fromValue;
			}
			set
			{
				this.fromValue = value;
				base.InvalidateRules();
			}
		}

		[DefaultValue("")]
		[TypeConverter(typeof(StringConverter))]
		[SRCategory("CategoryAttribute_Values")]
		[SRDescription("DescriptionAttributePathRule_ToValue")]
		public override string ToValue
		{
			get
			{
				return this.toValue;
			}
			set
			{
				this.toValue = value;
				base.InvalidateRules();
			}
		}

		[SRDescription("DescriptionAttributePathRule_UseCustomColors")]
		[DefaultValue(false)]
		[RefreshProperties(RefreshProperties.All)]
		[SRCategory("CategoryAttribute_CustomColors")]
		public bool UseCustomColors
		{
			get
			{
				return this.useCustomColors;
			}
			set
			{
				this.useCustomColors = value;
				base.InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Colors")]
		[SRDescription("DescriptionAttributePathRule_ColoringMode")]
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(ColoringMode.DistinctColors)]
		internal override ColoringMode ColoringMode
		{
			get
			{
				return base.ColoringMode;
			}
			set
			{
				base.ColoringMode = value;
				base.InvalidateRules();
			}
		}

		[SRDescription("DescriptionAttributePathRule_ColorPalette")]
		[DefaultValue(MapColorPalette.Random)]
		[SRCategory("CategoryAttribute_Colors")]
		public MapColorPalette ColorPalette
		{
			get
			{
				return this.colorPalette;
			}
			set
			{
				this.colorPalette = value;
				base.InvalidateRules();
			}
		}

		[DefaultValue(typeof(Color), "DarkGray")]
		[SRDescription("DescriptionAttributePathRule_BorderColor")]
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
				base.InvalidateRules();
			}
		}

		[SRDescription("DescriptionAttributePathRule_SecondaryColor")]
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
				base.InvalidateRules();
			}
		}

		//[Editor(typeof(GradientEditor), typeof(UITypeEditor))]
		[DefaultValue(GradientType.None)]
		[SRDescription("DescriptionAttributePathRule_GradientType")]
		[SRCategory("CategoryAttribute_Misc")]
		public GradientType GradientType
		{
			get
			{
				return this.gradientType;
			}
			set
			{
				this.gradientType = value;
				base.InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Misc")]
		[DefaultValue(MapHatchStyle.None)]
		//[Editor(typeof(HatchStyleEditor), typeof(UITypeEditor))]
		[SRDescription("DescriptionAttributePathRule_HatchStyle")]
		public MapHatchStyle HatchStyle
		{
			get
			{
				return this.hatchStyle;
			}
			set
			{
				this.hatchStyle = value;
				base.InvalidateRules();
			}
		}

		[SRDescription("DescriptionAttributePathRule_WidthInLegend")]
		[SRCategory("CategoryAttribute_Legend")]
		[NotifyParentProperty(true)]
		[DefaultValue(5)]
		public int WidthInLegend
		{
			get
			{
				return this.widthInLegend;
			}
			set
			{
				if (value >= 0 && value <= 100)
				{
					this.widthInLegend = value;
					return;
				}
				throw new ArgumentException(SR.must_in_range(0.0, 100.0));
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

		public PathRule()
			: this(null)
		{
		}

		internal PathRule(CommonElements common)
			: base(common)
		{
			this.customColors = new CustomColorCollection(this, common);
		}

		internal override void RegenerateRanges()
		{
			if (this.UseCustomColors)
			{
				foreach (CustomColor customColor10 in this.CustomColors)
				{
					customColor10.AffectedElements.Clear();
				}
			}
			MapCore mapCore = base.GetMapCore();
			if (mapCore != null && mapCore.Paths.Count != 0 && (!this.UseCustomColors || this.CustomColors.Count != 0))
			{
				if (!this.UseCustomColors)
				{
					this.CustomColors.Clear();
				}
				if (base.PathField == "(Name)")
				{
					if (this.UseCustomColors)
					{
						int num = 0;
						foreach (Path path4 in mapCore.Paths)
						{
							if (num == this.CustomColors.Count)
							{
								break;
							}
							CustomColor customColor2 = this.CustomColors[num++];
							customColor2.FromValueInt = path4.Name;
							customColor2.ToValueInt = path4.Name;
						}
					}
					else
					{
						Color[] colors = base.GetColors(this.ColoringMode, this.ColorPalette, this.FromColor, this.MiddleColor, this.ToColor, mapCore.Paths.Count);
						int num3 = 0;
						foreach (Path path5 in mapCore.Paths)
						{
							if (path5.Category == this.Category)
							{
								CustomColor customColor3 = this.CustomColors.Add(string.Empty);
								customColor3.Color = colors[num3++];
								customColor3.SecondaryColor = this.SecondaryColor;
								customColor3.BorderColor = this.BorderColor;
								customColor3.GradientType = this.GradientType;
								customColor3.HatchStyle = this.HatchStyle;
								customColor3.FromValueInt = path5.Name;
								customColor3.ToValueInt = path5.Name;
								customColor3.Text = base.Text;
								customColor3.ToolTip = base.ToolTip;
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
								base.GetRangeFromPaths(field, intervalCount, ref obj, ref obj2);
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
									customColor5.Text = base.Text;
									customColor5.ToolTip = base.ToolTip;
								}
							}
						}
						else if (field.Type == typeof(string))
						{
							Hashtable hashtable = new Hashtable();
							foreach (Path path6 in base.GetMapCore().Paths)
							{
								if (path6.Category == this.Category)
								{
									string text = (string)path6[field.Name];
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
									customColor7.Text = base.Text;
									customColor7.ToolTip = base.ToolTip;
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
							customColor8.Text = base.Text;
							customColor8.ToolTip = base.ToolTip;
							CustomColor customColor9 = this.CustomColors.Add(string.Empty);
							customColor9.Color = this.ToColor;
							customColor9.SecondaryColor = this.SecondaryColor;
							customColor9.BorderColor = this.BorderColor;
							customColor9.GradientType = this.GradientType;
							customColor9.HatchStyle = this.HatchStyle;
							customColor9.FromValueInt = "True";
							customColor9.ToValueInt = "True";
							customColor9.Text = base.Text;
							customColor9.ToolTip = base.ToolTip;
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
								legendItem.ItemStyle = LegendItemStyle.Path;
								legendItem.BorderColor = customColor4.BorderColor;
								legendItem.Color = customColor4.Color;
								legendItem.SecondaryColor = customColor4.SecondaryColor;
								legendItem.GradientType = customColor4.GradientType;
								legendItem.HatchStyle = customColor4.HatchStyle;
								legendItem.Text = base.GetLegendText(field, customColor4.FromValueInt, customColor4.ToValueInt);
								legendItem.PathWidth = this.WidthInLegend;
								legendItem.PathLineStyle = base.LineStyleInLegend;
								legendItem.BorderWidth = base.BorderWidthInLegend;
							}
						}
					}
				}
			}
		}

		internal override void Apply(Path path)
		{
			if (!(this.Category != path.Category))
			{
				if (base.PathField == "(Name)")
				{
					foreach (CustomColor customColor3 in this.CustomColors)
					{
						if (customColor3.FromValueInt == path.Name)
						{
							path.ApplyCustomColorAttributes(customColor3);
							customColor3.AffectedElements.Add(path);
							break;
						}
					}
				}
				else
				{
					Field field = this.GetField();
					if (field != null)
					{
						object testValue = path[field.Name];
						foreach (CustomColor customColor4 in this.CustomColors)
						{
							object obj = field.Parse(customColor4.FromValueInt);
							object obj2 = field.Parse(customColor4.ToValueInt);
							if (base.IsValueInRange(field, testValue, obj, obj2))
							{
								path.ApplyCustomColorAttributes(customColor4);
								customColor4.AffectedElements.Add(path);
								break;
							}
						}
					}
				}
			}
		}
	}
}
