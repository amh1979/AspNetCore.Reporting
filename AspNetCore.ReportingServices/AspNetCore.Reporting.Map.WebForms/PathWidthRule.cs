using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace AspNetCore.Reporting.Map.WebForms
{
	[TypeConverter(typeof(PathWidthRuleConverter))]
	internal class PathWidthRule : PathRuleBase
	{
		private CustomWidthCollection customWidths;

		private int widthCount = 5;

		private float fromWidth = 1f;

		private float toWidth = 30f;

		private string fromValue = "";

		private string toValue = "";

		private bool useCustomWidths;

		private Color borderColorInLegend = Color.DarkGray;

		private Color colorInLegend = Color.LightSalmon;

		private Color secondaryColorInLegend = Color.Empty;

		private GradientType gradientTypeInLegend;

		private MapHatchStyle hatchStyleInLegend;

		[SRCategory("CategoryAttribute_CustomWidths")]
		[SRDescription("DescriptionAttributePathWidthRule_CustomWidths")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CustomWidthCollection CustomWidths
		{
			get
			{
				return this.customWidths;
			}
		}

		[DefaultValue(5)]
		[SRDescription("DescriptionAttributePathWidthRule_WidthCount")]
		[SRCategory("CategoryAttribute_Colors")]
		public int WidthCount
		{
			get
			{
				return this.widthCount;
			}
			set
			{
				if (value < 1)
				{
					throw new ArgumentException(SR.ExceptionPropertyValueCannotbeLessThanOne);
				}
				this.widthCount = value;
				base.InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Widths")]
		[SRDescription("DescriptionAttributePathWidthRule_FromWidth")]
		[DefaultValue(1f)]
		public float FromWidth
		{
			get
			{
				return this.fromWidth;
			}
			set
			{
				this.fromWidth = value;
				base.InvalidateRules();
			}
		}

		[DefaultValue(30f)]
		[SRDescription("DescriptionAttributePathWidthRule_ToWidth")]
		[SRCategory("CategoryAttribute_Widths")]
		public float ToWidth
		{
			get
			{
				return this.toWidth;
			}
			set
			{
				this.toWidth = value;
				base.InvalidateRules();
			}
		}

		[TypeConverter(typeof(StringConverter))]
		[SRDescription("DescriptionAttributePathWidthRule_FromValue")]
		[DefaultValue("")]
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
				base.InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Values")]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributePathWidthRule_ToValue")]
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
				base.InvalidateRules();
			}
		}

		[SRDescription("DescriptionAttributePathWidthRule_UseCustomWidths")]
		[SRCategory("CategoryAttribute_CustomWidths")]
		[DefaultValue(false)]
		[RefreshProperties(RefreshProperties.All)]
		public bool UseCustomWidths
		{
			get
			{
				return this.useCustomWidths;
			}
			set
			{
				this.useCustomWidths = value;
				base.InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Legend")]
		[DefaultValue(typeof(Color), "DarkGray")]
		[SRDescription("DescriptionAttributePathWidthRule_BorderColorInLegend")]
		[NotifyParentProperty(true)]
		public Color BorderColorInLegend
		{
			get
			{
				return this.borderColorInLegend;
			}
			set
			{
				this.borderColorInLegend = value;
				base.InvalidateRules();
			}
		}

		[DefaultValue(typeof(Color), "LightSalmon")]
		[SRCategory("CategoryAttribute_Legend")]
		[SRDescription("DescriptionAttributePathWidthRule_ColorInLegend")]
		public Color ColorInLegend
		{
			get
			{
				return this.colorInLegend;
			}
			set
			{
				this.colorInLegend = value;
				base.InvalidateRules();
			}
		}

		[SRDescription("DescriptionAttributePathWidthRule_SecondaryColorInLegend")]
		[DefaultValue(typeof(Color), "")]
		[SRCategory("CategoryAttribute_Legend")]
		public Color SecondaryColorInLegend
		{
			get
			{
				return this.secondaryColorInLegend;
			}
			set
			{
				this.secondaryColorInLegend = value;
				base.InvalidateRules();
			}
		}

		//[Editor(typeof(GradientEditor), typeof(UITypeEditor))]
		[SRDescription("DescriptionAttributePathWidthRule_GradientTypeInLegend")]
		[DefaultValue(GradientType.None)]
		[SRCategory("CategoryAttribute_Legend")]
		public GradientType GradientTypeInLegend
		{
			get
			{
				return this.gradientTypeInLegend;
			}
			set
			{
				this.gradientTypeInLegend = value;
				base.InvalidateRules();
			}
		}

		[SRDescription("DescriptionAttributePathWidthRule_HatchStyleInLegend")]
		//[Editor(typeof(HatchStyleEditor), typeof(UITypeEditor))]
		[SRCategory("CategoryAttribute_Legend")]
		[DefaultValue(MapHatchStyle.None)]
		public MapHatchStyle HatchStyleInLegend
		{
			get
			{
				return this.hatchStyleInLegend;
			}
			set
			{
				this.hatchStyleInLegend = value;
				base.InvalidateRules();
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
				if (this.customWidths != null)
				{
					this.customWidths.Common = value;
				}
			}
		}

		public PathWidthRule()
			: this(null)
		{
		}

		internal PathWidthRule(CommonElements common)
			: base(common)
		{
			this.customWidths = new CustomWidthCollection(this, common);
		}

		internal override void RegenerateRanges()
		{
			if (this.UseCustomWidths)
			{
				foreach (CustomWidth customWidth10 in this.CustomWidths)
				{
					customWidth10.AffectedElements.Clear();
				}
			}
			MapCore mapCore = base.GetMapCore();
			if (mapCore != null && mapCore.Paths.Count != 0 && (!this.UseCustomWidths || this.CustomWidths.Count != 0))
			{
				if (!this.UseCustomWidths)
				{
					this.CustomWidths.Clear();
				}
				if (base.PathField == "(Name)")
				{
					if (this.UseCustomWidths)
					{
						int num = 0;
						foreach (Path path4 in mapCore.Paths)
						{
							if (num == this.CustomWidths.Count)
							{
								break;
							}
							CustomWidth customWidth2 = this.CustomWidths[num++];
							customWidth2.FromValueInt = path4.Name;
							customWidth2.ToValueInt = path4.Name;
						}
					}
					else
					{
						float[] widths = base.GetWidths(this.FromWidth, this.ToWidth, mapCore.Paths.Count);
						int num3 = 0;
						foreach (Path path5 in mapCore.Paths)
						{
							if (path5.Category == this.Category)
							{
								CustomWidth customWidth3 = this.CustomWidths.Add(string.Empty);
								customWidth3.Width = widths[num3++];
								customWidth3.FromValueInt = path5.Name;
								customWidth3.ToValueInt = path5.Name;
								customWidth3.Text = base.Text;
								customWidth3.ToolTip = base.ToolTip;
							}
						}
					}
					this.UpdateLegend();
				}
				else
				{
					Field field = this.GetField();
					if (field != null)
					{
						if (field.IsNumeric())
						{
							int intervalCount = (!this.UseCustomWidths) ? this.WidthCount : this.CustomWidths.Count;
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
							if (this.UseCustomWidths)
							{
								int num5 = 0;
								foreach (CustomWidth customWidth11 in this.CustomWidths)
								{
									if (num5 < array.Length)
									{
										customWidth11.FromValueInt = AspNetCore.Reporting.Map.WebForms.Field.ToStringInvariant(array[num5]);
										customWidth11.ToValueInt = AspNetCore.Reporting.Map.WebForms.Field.ToStringInvariant(array2[num5]);
										customWidth11.VisibleInt = true;
									}
									else
									{
										customWidth11.FromValueInt = AspNetCore.Reporting.Map.WebForms.Field.ToStringInvariant(array2[array2.Length - 1]);
										customWidth11.ToValueInt = AspNetCore.Reporting.Map.WebForms.Field.ToStringInvariant(array2[array2.Length - 1]);
										customWidth11.VisibleInt = false;
									}
									num5++;
								}
							}
							else
							{
								float[] widths2 = base.GetWidths(this.FromWidth, this.ToWidth, array.Length);
								for (int i = 0; i < array.Length; i++)
								{
									CustomWidth customWidth5 = this.CustomWidths.Add(string.Empty);
									customWidth5.Width = widths2[i];
									customWidth5.FromValueInt = AspNetCore.Reporting.Map.WebForms.Field.ToStringInvariant(array[i]);
									customWidth5.ToValueInt = AspNetCore.Reporting.Map.WebForms.Field.ToStringInvariant(array2[i]);
									customWidth5.Text = base.Text;
									customWidth5.ToolTip = base.ToolTip;
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
							if (this.UseCustomWidths)
							{
								ArrayList arrayList = new ArrayList();
								arrayList.AddRange(hashtable.Keys);
								arrayList.Sort();
								int num6 = 0;
								foreach (object item in arrayList)
								{
									if (num6 == this.CustomWidths.Count)
									{
										break;
									}
									CustomWidth customWidth6 = this.CustomWidths[num6++];
									customWidth6.FromValueInt = (string)item;
									customWidth6.ToValueInt = (string)item;
								}
							}
							else
							{
								float[] widths3 = base.GetWidths(this.FromWidth, this.ToWidth, hashtable.Keys.Count);
								int num8 = 0;
								foreach (object key in hashtable.Keys)
								{
									CustomWidth customWidth7 = this.CustomWidths.Add(string.Empty);
									customWidth7.Width = widths3[num8++];
									customWidth7.FromValueInt = (string)key;
									customWidth7.ToValueInt = (string)key;
									customWidth7.Text = base.Text;
									customWidth7.ToolTip = base.ToolTip;
								}
							}
						}
						else if (this.UseCustomWidths)
						{
							this.CustomWidths[0].FromValueInt = "False";
							this.CustomWidths[0].ToValueInt = "False";
							if (this.CustomWidths.Count > 1)
							{
								this.CustomWidths[1].FromValueInt = "True";
								this.CustomWidths[1].ToValueInt = "True";
							}
						}
						else
						{
							CustomWidth customWidth8 = this.CustomWidths.Add(string.Empty);
							customWidth8.Width = this.FromWidth;
							customWidth8.FromValueInt = "False";
							customWidth8.ToValueInt = "False";
							customWidth8.Text = base.Text;
							customWidth8.ToolTip = base.ToolTip;
							CustomWidth customWidth9 = this.CustomWidths.Add(string.Empty);
							customWidth9.Width = this.ToWidth;
							customWidth9.FromValueInt = "True";
							customWidth9.ToValueInt = "True";
							customWidth9.Text = base.Text;
							customWidth9.ToolTip = base.ToolTip;
						}
						this.UpdateLegend();
					}
				}
			}
		}

		internal void UpdateLegend()
		{
			MapCore mapCore = base.GetMapCore();
			if (mapCore != null)
			{
				Field field = this.GetField();
				if (this.ShowInLegend != string.Empty && this.ShowInLegend != "(none)")
				{
					Legend legend = (Legend)mapCore.Legends.GetByName(this.ShowInLegend);
					if (legend != null)
					{
						foreach (CustomWidth customWidth in this.CustomWidths)
						{
							if (customWidth.VisibleInt)
							{
								LegendItem legendItem = legend.Items.Add("");
								legendItem.automaticallyAdded = true;
								legendItem.PathWidth = (int)Math.Round((double)customWidth.Width);
								legendItem.ItemStyle = LegendItemStyle.Path;
								legendItem.Text = base.GetLegendText(field, customWidth.FromValueInt, customWidth.ToValueInt);
								legendItem.PathLineStyle = base.LineStyleInLegend;
								legendItem.BorderColor = this.BorderColorInLegend;
								legendItem.BorderWidth = base.BorderWidthInLegend;
								legendItem.Color = this.ColorInLegend;
								legendItem.SecondaryColor = this.SecondaryColorInLegend;
								legendItem.GradientType = this.GradientTypeInLegend;
								legendItem.HatchStyle = this.HatchStyleInLegend;
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
					foreach (CustomWidth customWidth3 in this.CustomWidths)
					{
						if (customWidth3.FromValueInt != null && customWidth3.ToValueInt != null && customWidth3.FromValueInt == path.Name)
						{
							path.ApplyCustomWidthAttributes(customWidth3);
							customWidth3.AffectedElements.Add(path);
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
						foreach (CustomWidth customWidth4 in this.CustomWidths)
						{
							object obj = field.Parse(customWidth4.FromValueInt);
							object obj2 = field.Parse(customWidth4.ToValueInt);
							if (base.IsValueInRange(field, testValue, obj, obj2))
							{
								path.ApplyCustomWidthAttributes(customWidth4);
								customWidth4.AffectedElements.Add(path);
								break;
							}
						}
					}
				}
			}
		}
	}
}
