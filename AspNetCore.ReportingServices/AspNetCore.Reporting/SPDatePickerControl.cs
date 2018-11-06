using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;


namespace AspNetCore.Reporting
{
	[DefaultProperty("SelectedDate")]
	internal class SPDatePickerControl
    {

		private int _lcid = 1033;

		private TimeSpan _timezone = TimeSpan.MinValue;

		private SPCalendarType _calendar = SPCalendarType.Gregorian;

		private string _ww = "0111110";

		private int _fdow;

		private int _hj;

		public string _urlCssClass;

		private short _firstWeekOfYear;

		private int _tdym = -1;

		private string _imageDirName;

		private int _langid = Thread.CurrentThread.CurrentUICulture.LCID;

		public string StartMonth
		{
            get;set;
		}

		[Category("Data")]
		[Bindable(true)]
		[Description(" Use short format of DateTime")]
		[DefaultValue("")]
		public string SelectedDate
		{
            get;set;
		}

		[DefaultValue(1033)]
		[Category("Data")]
		[Bindable(true)]
		public int LocaleId
		{
			get
			{
				return this._lcid;
			}
			set
			{
				this._lcid = value;
			}
		}

		public int LangId
		{
			get
			{
				return this._langid;
			}
			set
			{
				this._langid = value;
			}
		}

		[Description("Difference between UTC and local time")]
		[Category("Data")]
		[Bindable(true)]
		public TimeSpan TimeZone
		{
			get
			{
				return this._timezone;
			}
			set
			{
				this._timezone = value;
			}
		}

		[Bindable(true)]
		[DefaultValue("1")]
		[Category("Data")]
		public SPCalendarType Calendar
		{
			get
			{
				return this._calendar;
			}
			set
			{
				this._calendar = value;
			}
		}

		[DefaultValue("0111110")]
		[Bindable(true)]
		[Category("Data")]
		public string WorkWeek
		{
			get
			{
				return this._ww;
			}
			set
			{
				if (SPUtility.IsValidStringInput("[01]{7}", value))
				{
					this._ww = value;
				}
			}
		}

		[Category("Data")]
		[DefaultValue("0")]
		[Description("Valid values: from 0 to 6.")]
		[Bindable(true)]
		public int FirstDayOfWeek
		{
			get
			{
				return this._fdow;
			}
			set
			{
				if (value >= 0 && value < 7)
				{
					this._fdow = value;
				}
			}
		}

		[Bindable(true)]
		[Description("Valid values: from -3 to 3.")]
		[Category("Data")]
		[DefaultValue("0")]
		public int HijriAdjustment
		{
			get
			{
				return this._hj;
			}
			set
			{
				if (value > -3 && value < 3)
				{
					this._hj = value;
				}
			}
		}

		[Description("Valid values: from 0 to 2.")]
		[Category("Data")]
		[Bindable(true)]
		[DefaultValue("0")]
		public short FirstWeekOfYear
		{
			get
			{
				return this._firstWeekOfYear;
			}
			set
			{
				if (value > -1 && value < 3)
				{
					this._firstWeekOfYear = value;
				}
			}
		}

		[Category("Visibility")]
		[Bindable(true)]
		[DefaultValue(true)]
		public bool ShowNotThisMonthDays
		{
            get;set;
		}

		[DefaultValue(true)]
		[Bindable(true)]
		[Category("Visibility")]
		public bool ShowFooter
		{
            get;set;
		}

		[Category("Visibility")]
		[DefaultValue(false)]
		[Bindable(true)]
		public bool ShowWeekNumber
		{
            get;set;
		}

		[DefaultValue(true)]
		[Bindable(true)]
		[Category("Visibility")]
		public bool ShowNextPrevNavigation
		{
            get;set;
		}

		[DefaultValue(-1)]
		[Bindable(true)]
		[Category("Picker")]
		[Description("Value betweeen -12 and 0")]
		public int StartOffset
		{
            get;set;
		}

		[Category("Picker")]
		[Bindable(true)]
		[DefaultValue(3)]
		[Description("Value betweeen 0 and 12")]
		public int EndOffset
		{
            get;set;
		}

		[Bindable(true)]
		[DefaultValue(-1)]
		[Category("Picker")]
		[Description("")]
		public int TwoDigitYearMax
		{
			get
			{
				return this._tdym;
			}
			set
			{
				if (value >= 0)
				{
					this._tdym = value;
				}
			}
		}

		[Bindable(true)]
		[DefaultValue(0)]
		[Category("Picker")]
		[Description("Full or web relative path to images location. ")]
		public string ImageUrl
		{
			set
			{
				this._imageDirName = value;
			}
		}

		[Description("Name of javascipt function used as onClick event handler. ")]
		[Bindable(true)]
		[Category("Picker")]
		[DefaultValue("")]
		public string OnClickScriptHandler
		{
            get;set;
		}

		public int MinJDay
		{
            get; set;
        }

		public int MaxJDay
		{
            get; set;
        }

		internal string RemoveLoadingScript
		{
			get
			{
				return "HideUnhide('LoadingDiv', 'DatePickerDiv', g_currentID, null);PositionFrame('DatePickerDiv');";
			}
		}

		protected virtual void InitDatePicker()
		{
			 
		}

		protected  void Render(TextWriter output)
		{
			 
			output.Write("<div class='ms-rs-calendar-loading' id='LoadingDiv' style='width:100%;height:100%'>");
			//output.Write(LocalizationHelper.Current.CalendarLoading);
			output.Write("</div>");
			if (this._urlCssClass != null)
			{
				string value = "<LINK REL=\"stylesheet\" TYPE=\"text/css\" HREF=\"" + SPHttpUtility.HtmlUrlAttributeEncode(this._urlCssClass.ToString()) + "\">";
				output.Write(value);
			}
			
			 
		}
	}
}
