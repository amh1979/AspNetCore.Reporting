using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal abstract class PathRuleBase : RuleBase
	{
		private string pathField = "(Name)";

		private string showInLegend = "(none)";

		private string legendText = "#FROMVALUE{N0} - #TOVALUE{N0}";

		private string category = string.Empty;

		private DataGrouping dataGrouping = DataGrouping.Optimal;

		private string text = "#NAME";

		private string toolTip = "";

		private ColoringMode coloringMode = ColoringMode.DistinctColors;

		private int colorCount = 5;

		private MapDashStyle lineStyleInLegend = MapDashStyle.Solid;

		private int borderWidthInLegend = 1;

		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributePathRule_Name")]
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

		[TypeConverter(typeof(DesignTimeFieldConverter))]
		[RefreshProperties(RefreshProperties.All)]
		[SRDescription("DescriptionAttributePathRule_PathField")]
		[SRCategory("CategoryAttribute_Data")]
		[DefaultValue("(Name)")]
		[ParenthesizePropertyName(true)]
		public string PathField
		{
			get
			{
				return this.pathField;
			}
			set
			{
				this.pathField = value;
				this.InvalidateRules();
			}
		}

		internal override string Field
		{
			get
			{
				return this.PathField;
			}
			set
			{
				this.PathField = value;
			}
		}

		[SRDescription("DescriptionAttributePathRule_ShowInLegend")]
		[RefreshProperties(RefreshProperties.All)]
		[ParenthesizePropertyName(true)]
		[DefaultValue("(none)")]
		[SRCategory("CategoryAttribute_Legend")]
		[TypeConverter(typeof(DesignTimeLegendConverter))]
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

		[SRDescription("DescriptionAttributePathRule_LegendText")]
		[DefaultValue("#FROMVALUE{N0} - #TOVALUE{N0}")]
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

		[SRDescription("DescriptionAttributePathRule_Category")]
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

		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributePathRule_DataGrouping")]
		[DefaultValue(DataGrouping.Optimal)]
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

		[SRDescription("DescriptionAttributePathRule_Text")]
		[TypeConverter(typeof(KeywordConverter))]
		[SRCategory("CategoryAttribute_Misc")]
		[DefaultValue("#NAME")]
		[Localizable(true)]
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

		[EditorBrowsable(EditorBrowsableState.Never)]
		[TypeConverter(typeof(KeywordConverter))]
		[Browsable(false)]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributePathRule_ToolTip")]
		[Localizable(true)]
		[SRCategory("CategoryAttribute_Misc")]
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

		[SerializationVisibility(SerializationVisibility.Hidden)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		internal virtual ColoringMode ColoringMode
		{
			get
			{
				return this.coloringMode;
			}
			set
			{
				this.coloringMode = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[Browsable(false)]
		public virtual int ColorCount
		{
			get
			{
				return this.colorCount;
			}
			set
			{
				this.colorCount = value;
			}
		}

		[SRDescription("DescriptionAttributePathRule_LineStyleInLegend")]
		[NotifyParentProperty(true)]
		[DefaultValue(MapDashStyle.Solid)]
		[SRCategory("CategoryAttribute_Legend")]
		public MapDashStyle LineStyleInLegend
		{
			get
			{
				return this.lineStyleInLegend;
			}
			set
			{
				this.lineStyleInLegend = value;
				this.InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Legend")]
		[SRDescription("DescriptionAttributePathRule_BorderWidthInLegend")]
		[NotifyParentProperty(true)]
		[DefaultValue(1)]
		public int BorderWidthInLegend
		{
			get
			{
				return this.borderWidthInLegend;
			}
			set
			{
				if (value >= 0 && value <= 100)
				{
					this.borderWidthInLegend = value;
					this.InvalidateRules();
					return;
				}
				throw new ArgumentException(SR.must_in_range(0.0, 100.0));
			}
		}

		internal PathRuleBase(CommonElements common)
			: base(common)
		{
		}

		public override string ToString()
		{
			return this.Name;
		}

		internal abstract void Apply(Path path);

		internal abstract void RegenerateRanges();

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

		internal void GetRangeFromPaths(Field field, int intervalCount, ref object fromValue, ref object toValue)
		{
			MapCore mapCore = base.GetMapCore();
			if (mapCore != null)
			{
				if (field.Type == typeof(int))
				{
					int num = 2147483647;
					int num2 = -2147483648;
					foreach (Path path6 in mapCore.Paths)
					{
						if (!(this.Category != path6.Category))
						{
							object obj = path6[field.Name];
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
					foreach (Path path7 in mapCore.Paths)
					{
						if (!(this.Category != path7.Category))
						{
							object obj2 = path7[field.Name];
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
					foreach (Path path8 in mapCore.Paths)
					{
						if (!(this.Category != path8.Category))
						{
							object obj3 = path8[field.Name];
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
					foreach (Path path9 in mapCore.Paths)
					{
						if (!(this.Category != path9.Category))
						{
							object obj4 = path9[field.Name];
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
					foreach (Path path10 in mapCore.Paths)
					{
						if (!(this.Category != path10.Category))
						{
							object obj5 = path10[field.Name];
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
			foreach (Path path in mapCore.Paths)
			{
				if (path.Category == this.Category)
				{
					object obj = path[field.Name];
					if (obj != null && base.IsValueInRange(field, obj, fromValue, toValue))
					{
						arrayList.Add(obj);
					}
				}
			}
			arrayList.Sort();
			return arrayList;
		}

		internal override Field GetField()
		{
			MapCore mapCore = base.GetMapCore();
			if (mapCore == null)
			{
				return null;
			}
			return (Field)mapCore.PathFields.GetByName(this.PathField);
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
	}
}
