using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace AspNetCore.Reporting.Map.WebForms
{
	[TypeConverter(typeof(SymbolRuleConverter))]
	internal class SymbolRule : RuleBase
	{
		private PredefinedSymbolCollection predefinedSymbols;

		private bool showInColorSwatch;

		private string symbolField = "(Name)";

		private string category = string.Empty;

		private string showInLegend = "(none)";

		private string legendText = "#FROMVALUE{N0} - #TOVALUE{N0}";

		private string fromValue = "";

		private string toValue = "";

		private DataGrouping dataGrouping = DataGrouping.Optimal;

		private AffectedSymbolAttributes affectedAttributes;

		[SRDescription("DescriptionAttributeSymbolRule_PredefinedSymbols")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRCategory("CategoryAttribute_Data")]
		public PredefinedSymbolCollection PredefinedSymbols
		{
			get
			{
				return this.predefinedSymbols;
			}
		}

		[DefaultValue(false)]
		[SRCategory("CategoryAttribute_Misc")]
		[SRDescription("DescriptionAttributeSymbolRule_ShowInColorSwatch")]
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

		[SRDescription("DescriptionAttributeSymbolRule_Name")]
		[SRCategory("CategoryAttribute_Data")]
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

		[SRDescription("DescriptionAttributeSymbolRule_SymbolField")]
		[SRCategory("CategoryAttribute_Data")]
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue("(Name)")]
		[ParenthesizePropertyName(true)]
		[TypeConverter(typeof(DesignTimeFieldConverter))]
		public string SymbolField
		{
			get
			{
				return this.symbolField;
			}
			set
			{
				this.symbolField = value;
				this.InvalidateRules();
			}
		}

		internal override string Field
		{
			get
			{
				return this.SymbolField;
			}
			set
			{
				this.SymbolField = value;
			}
		}

		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeSymbolRule_Category")]
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

		[SRDescription("DescriptionAttributeSymbolRule_ShowInLegend")]
		[TypeConverter(typeof(DesignTimeLegendConverter))]
		[ParenthesizePropertyName(true)]
		[DefaultValue("(none)")]
		[RefreshProperties(RefreshProperties.All)]
		[SRCategory("CategoryAttribute_Legend")]
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

		[SRCategory("CategoryAttribute_Legend")]
		[SRDescription("DescriptionAttributeSymbolRule_LegendText")]
		[DefaultValue("#FROMVALUE{N0} - #TOVALUE{N0}")]
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

		[SRCategory("CategoryAttribute_Values")]
		[SRDescription("DescriptionAttributeSymbolRule_FromValue")]
		[TypeConverter(typeof(StringConverter))]
		[DefaultValue("")]
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

		[TypeConverter(typeof(StringConverter))]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeSymbolRule_ToValue")]
		[SRCategory("CategoryAttribute_Values")]
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

		[DefaultValue(DataGrouping.Optimal)]
		[SRDescription("DescriptionAttributeSymbolRule_DataGrouping")]
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

		internal override CommonElements Common
		{
			get
			{
				return base.Common;
			}
			set
			{
				base.Common = value;
				if (this.predefinedSymbols != null)
				{
					this.predefinedSymbols.Common = value;
				}
			}
		}

		[DefaultValue(AffectedSymbolAttributes.All)]
		internal AffectedSymbolAttributes AffectedAttributes
		{
			get
			{
				return this.affectedAttributes;
			}
			set
			{
				this.affectedAttributes = value;
				this.InvalidateRules();
			}
		}

		public SymbolRule()
			: this(null)
		{
		}

		internal SymbolRule(CommonElements common)
			: base(common)
		{
			this.predefinedSymbols = new PredefinedSymbolCollection(this, common);
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

		internal void UpdateAutoRanges()
		{
			foreach (PredefinedSymbol predefinedSymbol5 in this.PredefinedSymbols)
			{
				predefinedSymbol5.AffectedSymbols.Clear();
			}
			MapCore mapCore = base.GetMapCore();
			if (mapCore != null && mapCore.Symbols.Count != 0 && this.PredefinedSymbols.Count != 0)
			{
				if (this.SymbolField == "(Name)")
				{
					int num = 0;
					foreach (Symbol symbol3 in mapCore.Symbols)
					{
						if (num == this.PredefinedSymbols.Count)
						{
							break;
						}
						PredefinedSymbol predefinedSymbol2 = this.PredefinedSymbols[num++];
						predefinedSymbol2.FromValueInt = symbol3.Name;
						predefinedSymbol2.ToValueInt = symbol3.Name;
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
							int count = this.PredefinedSymbols.Count;
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
								this.GetRangeFromSymbols(field, count, ref obj, ref obj2);
							}
							object[] array = null;
							object[] array2 = null;
							if (this.DataGrouping == DataGrouping.EqualInterval)
							{
								base.GetEqualIntervals(field, obj, obj2, count, ref array, ref array2);
							}
							else if (this.DataGrouping == DataGrouping.EqualDistribution)
							{
								ArrayList sortedValues = this.GetSortedValues(field, obj, obj2);
								base.GetEqualDistributionIntervals(field, sortedValues, obj, obj2, count, ref array, ref array2);
							}
							else if (this.DataGrouping == DataGrouping.Optimal)
							{
								ArrayList sortedValues2 = this.GetSortedValues(field, obj, obj2);
								base.GetOptimalIntervals(field, sortedValues2, obj, obj2, count, ref array, ref array2);
							}
							int num3 = 0;
							foreach (PredefinedSymbol predefinedSymbol6 in this.PredefinedSymbols)
							{
								if (num3 < array.Length)
								{
									predefinedSymbol6.FromValueInt = AspNetCore.Reporting.Map.WebForms.Field.ToStringInvariant(array[num3]);
									predefinedSymbol6.ToValueInt = AspNetCore.Reporting.Map.WebForms.Field.ToStringInvariant(array2[num3]);
									predefinedSymbol6.VisibleInt = true;
								}
								else
								{
									predefinedSymbol6.FromValueInt = AspNetCore.Reporting.Map.WebForms.Field.ToStringInvariant(array2[array2.Length - 1]);
									predefinedSymbol6.ToValueInt = AspNetCore.Reporting.Map.WebForms.Field.ToStringInvariant(array2[array2.Length - 1]);
									predefinedSymbol6.VisibleInt = false;
								}
								num3++;
							}
						}
						else if (field.Type == typeof(string))
						{
							Hashtable hashtable = new Hashtable();
							foreach (Symbol symbol4 in base.GetMapCore().Symbols)
							{
								if (symbol4.Category == this.Category)
								{
									string text = (string)symbol4[field.Name];
									if (text != null)
									{
										hashtable[text] = 0;
									}
								}
							}
							ArrayList arrayList = new ArrayList();
							arrayList.AddRange(hashtable.Keys);
							arrayList.Sort();
							int num4 = 0;
							foreach (object item in arrayList)
							{
								if (num4 == this.PredefinedSymbols.Count)
								{
									break;
								}
								PredefinedSymbol predefinedSymbol4 = this.PredefinedSymbols[num4++];
								predefinedSymbol4.FromValueInt = (string)item;
								predefinedSymbol4.ToValueInt = (string)item;
							}
						}
						else
						{
							this.PredefinedSymbols[0].FromValueInt = "False";
							this.PredefinedSymbols[0].ToValueInt = "False";
							if (this.PredefinedSymbols.Count > 1)
							{
								this.PredefinedSymbols[1].FromValueInt = "True";
								this.PredefinedSymbols[1].ToValueInt = "True";
							}
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
					foreach (PredefinedSymbol predefinedSymbol3 in this.PredefinedSymbols)
					{
						if (predefinedSymbol3.Visible)
						{
							SwatchColor swatchColor = mapCore.ColorSwatchPanel.Colors.Add("");
							swatchColor.automaticallyAdded = true;
							swatchColor.Color = predefinedSymbol3.Color;
							swatchColor.SecondaryColor = predefinedSymbol3.SecondaryColor;
							swatchColor.GradientType = predefinedSymbol3.GradientType;
							swatchColor.HatchStyle = predefinedSymbol3.HatchStyle;
							if (field != null && field.IsNumeric())
							{
								swatchColor.FromValue = field.ConvertToDouble(field.Parse(predefinedSymbol3.FromValueInt));
								swatchColor.ToValue = field.ConvertToDouble(field.Parse(predefinedSymbol3.ToValueInt));
							}
							else
							{
								swatchColor.TextValue = predefinedSymbol3.FromValueInt;
							}
						}
					}
				}
				if (this.ShowInLegend != string.Empty && this.ShowInLegend != "(none)")
				{
					Legend legend = (Legend)mapCore.Legends.GetByName(this.ShowInLegend);
					if (legend != null)
					{
						foreach (PredefinedSymbol predefinedSymbol4 in this.PredefinedSymbols)
						{
							if (predefinedSymbol4.Visible)
							{
								LegendItem legendItem = legend.Items.Add("");
								legendItem.automaticallyAdded = true;
								legendItem.ShadowOffset = predefinedSymbol4.ShadowOffset;
								legendItem.Text = this.GetLegendText(field, predefinedSymbol4.FromValueInt, predefinedSymbol4.ToValueInt);
								if (!string.IsNullOrEmpty(predefinedSymbol4.Image))
								{
									LegendCell legendCell = new LegendCell(LegendCellType.Image, predefinedSymbol4.Image);
									legendCell.ImageTranspColor = predefinedSymbol4.ImageTransColor;
									legendCell.Margins.Top = 15;
									legendCell.Margins.Bottom = 15;
									LegendCell cell = new LegendCell(LegendCellType.Text, "#LEGENDTEXT", ContentAlignment.MiddleLeft);
									legendItem.Cells.Add(legendCell);
									legendItem.Cells.Add(cell);
								}
								else
								{
									legendItem.ItemStyle = LegendItemStyle.Symbol;
									legendItem.MarkerStyle = predefinedSymbol4.MarkerStyle;
									legendItem.MarkerColor = predefinedSymbol4.Color;
									legendItem.MarkerWidth = (float)((predefinedSymbol4.Width < 0.0010000000474974513) ? 13.0 : predefinedSymbol4.Width);
									legendItem.MarkerHeight = (float)((predefinedSymbol4.Height < 0.0010000000474974513) ? 13.0 : predefinedSymbol4.Height);
									legendItem.MarkerGradientType = predefinedSymbol4.GradientType;
									legendItem.MarkerHatchStyle = predefinedSymbol4.HatchStyle;
									legendItem.MarkerSecondaryColor = predefinedSymbol4.SecondaryColor;
									legendItem.MarkerBorderColor = predefinedSymbol4.BorderColor;
									legendItem.MarkerBorderWidth = predefinedSymbol4.BorderWidth;
									legendItem.MarkerBorderStyle = predefinedSymbol4.BorderStyle;
								}
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

		internal void Apply(Symbol symbol)
		{
			if (!(this.Category != symbol.Category))
			{
				if (this.SymbolField == "(Name)")
				{
					foreach (PredefinedSymbol predefinedSymbol3 in this.PredefinedSymbols)
					{
						if (predefinedSymbol3.FromValueInt == symbol.Name)
						{
							symbol.ApplyPredefinedSymbolAttributes(predefinedSymbol3, this.AffectedAttributes);
							predefinedSymbol3.AffectedSymbols.Add(symbol);
							break;
						}
					}
				}
				else
				{
					Field field = this.GetField();
					if (field != null)
					{
						object testValue = symbol[field.Name];
						foreach (PredefinedSymbol predefinedSymbol4 in this.PredefinedSymbols)
						{
							object obj = field.Parse(predefinedSymbol4.FromValueInt);
							object obj2 = field.Parse(predefinedSymbol4.ToValueInt);
							if (base.IsValueInRange(field, testValue, obj, obj2))
							{
								symbol.ApplyPredefinedSymbolAttributes(predefinedSymbol4, this.AffectedAttributes);
								predefinedSymbol4.AffectedSymbols.Add(symbol);
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
			return (Field)mapCore.SymbolFields.GetByName(this.SymbolField);
		}

		internal void GetRangeFromSymbols(Field field, int intervalCount, ref object fromValue, ref object toValue)
		{
			MapCore mapCore = base.GetMapCore();
			if (mapCore != null)
			{
				if (field.Type == typeof(int))
				{
					int num = 2147483647;
					int num2 = -2147483648;
					foreach (Symbol symbol6 in mapCore.Symbols)
					{
						if (!(this.Category != symbol6.Category))
						{
							object obj = symbol6[field.Name];
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
					foreach (Symbol symbol7 in mapCore.Symbols)
					{
						if (!(this.Category != symbol7.Category))
						{
							object obj2 = symbol7[field.Name];
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
					foreach (Symbol symbol8 in mapCore.Symbols)
					{
						if (!(this.Category != symbol8.Category))
						{
							object obj3 = symbol8[field.Name];
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
					foreach (Symbol symbol9 in mapCore.Symbols)
					{
						if (!(this.Category != symbol9.Category))
						{
							object obj4 = symbol9[field.Name];
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
					foreach (Symbol symbol10 in mapCore.Symbols)
					{
						if (!(this.Category != symbol10.Category))
						{
							object obj5 = symbol10[field.Name];
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
			foreach (Symbol symbol in mapCore.Symbols)
			{
				if (symbol.Category == this.Category)
				{
					object obj = symbol[field.Name];
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
