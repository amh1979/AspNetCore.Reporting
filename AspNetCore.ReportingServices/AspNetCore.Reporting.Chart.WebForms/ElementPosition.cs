using AspNetCore.Reporting.Chart.WebForms.Design;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[DefaultProperty("Data")]
	[TypeConverter(typeof(ElementPositionConverter))]
	[SRDescription("DescriptionAttributeElementPosition_ElementPosition")]
	internal class ElementPosition
	{
		private float x;

		private float y;

		private float width;

		private float height;

		internal bool auto = true;

		internal CommonElements common;

		internal Chart chart;

		internal bool resetAreaAutoPosition;

		[SRCategory("CategoryAttributeMisc")]
		[RefreshProperties(RefreshProperties.All)]
		[Bindable(true)]
		[DefaultValue(0f)]
		[SRDescription("DescriptionAttributeElementPosition_X")]
		[NotifyParentProperty(true)]
		public float X
		{
			get
			{
				return this.x;
			}
			set
			{
				if (!((double)value < 0.0) && !((double)value > 100.0))
				{
					this.x = value;
					this.Auto = false;
					if (this.x + this.Width > 100.0)
					{
						this.Width = (float)(100.0 - this.x);
					}
					this.Invalidate();
					return;
				}
				throw new ArgumentOutOfRangeException("value", SR.ExceptionElementPositionArgumentOutOfRange);
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[DefaultValue(0f)]
		[SRDescription("DescriptionAttributeElementPosition_Y")]
		[NotifyParentProperty(true)]
		public float Y
		{
			get
			{
				return this.y;
			}
			set
			{
				if (!((double)value < 0.0) && !((double)value > 100.0))
				{
					this.y = value;
					this.Auto = false;
					if (this.y + this.Height > 100.0)
					{
						this.Height = (float)(100.0 - this.y);
					}
					this.Invalidate();
					return;
				}
				throw new ArgumentOutOfRangeException("value", SR.ExceptionElementPositionArgumentOutOfRange);
			}
		}

		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[DefaultValue(0f)]
		[SRDescription("DescriptionAttributeElementPosition_Width")]
		public float Width
		{
			get
			{
				return this.width;
			}
			set
			{
				if (!((double)value < 0.0) && !((double)value > 100.0))
				{
					this.width = value;
					this.Auto = false;
					if (this.x + this.Width > 100.0)
					{
						this.x = (float)(100.0 - this.Width);
					}
					this.Invalidate();
					return;
				}
				throw new ArgumentOutOfRangeException("value", SR.ExceptionElementPositionArgumentOutOfRange);
			}
		}

		[DefaultValue(0f)]
		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[RefreshProperties(RefreshProperties.All)]
		[SRDescription("DescriptionAttributeElementPosition_Height")]
		[NotifyParentProperty(true)]
		public float Height
		{
			get
			{
				return this.height;
			}
			set
			{
				if (!((double)value < 0.0) && !((double)value > 100.0))
				{
					this.height = value;
					this.Auto = false;
					if (this.y + this.Height > 100.0)
					{
						this.y = (float)(100.0 - this.Height);
					}
					this.Invalidate();
					return;
				}
				throw new ArgumentOutOfRangeException("value", SR.ExceptionElementPositionArgumentOutOfRange);
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeElementPosition_Auto")]
		[NotifyParentProperty(true)]
		public bool Auto
		{
			get
			{
				return this.auto;
			}
			set
			{
				if (value != this.auto)
				{
					this.ResetAllAreasAutoPosition(value);
					if (value)
					{
						this.x = 0f;
						this.y = 0f;
						this.width = 0f;
						this.height = 0f;
					}
					this.auto = value;
					this.Invalidate();
				}
			}
		}

		public ElementPosition()
		{
		}

		public ElementPosition(float x, float y, float width, float height)
		{
			this.auto = false;
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
		}

		private void ResetAllAreasAutoPosition(bool autoValue)
		{
			if (this.resetAreaAutoPosition)
			{
				if (this.chart == null && this.common != null)
				{
					this.chart = (Chart)this.common.container.GetService(typeof(Chart));
				}
				if (this.chart != null && this.chart.IsDesignMode() && !this.chart.serializing && this.chart.ChartAreas.Count > 1)
				{
					bool flag = this.chart.ChartAreas[0].Position.Auto;
					bool flag2 = true;
					foreach (ChartArea chartArea3 in this.chart.ChartAreas)
					{
						if (chartArea3.Position.Auto != flag)
						{
							flag2 = false;
							break;
						}
					}
					if (flag2)
					{
						string messageChangingChartAreaPositionProperty = SR.MessageChangingChartAreaPositionProperty;
						messageChangingChartAreaPositionProperty = ((!autoValue) ? (messageChangingChartAreaPositionProperty + SR.MessageChangingChartAreaPositionConfirmCustom) : (messageChangingChartAreaPositionProperty + SR.MessageChangingChartAreaPositionConfirmAutomatic));
						DialogResult dialogResult = MessageBox.Show(messageChangingChartAreaPositionProperty, SR.MessageChartTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
						if (dialogResult == DialogResult.Yes)
						{
							foreach (ChartArea chartArea4 in this.chart.ChartAreas)
							{
								if (autoValue)
								{
									this.SetPositionNoAuto(0f, 0f, 0f, 0f);
								}
								chartArea4.Position.auto = autoValue;
							}
						}
					}
				}
			}
		}

		public RectangleF ToRectangleF()
		{
			return new RectangleF(this.x, this.y, this.width, this.height);
		}

		public void FromRectangleF(RectangleF rect)
		{
			this.x = rect.X;
			this.y = rect.Y;
			this.width = rect.Width;
			this.height = rect.Height;
			this.auto = false;
		}

		public SizeF GetSize()
		{
			return new SizeF(this.width, this.height);
		}

		public float Bottom()
		{
			return this.y + this.height;
		}

		public float Right()
		{
			return this.x + this.width;
		}

		public override bool Equals(object obj)
		{
			if (obj is ElementPosition)
			{
				ElementPosition elementPosition = (ElementPosition)obj;
				if (this.auto && this.auto == elementPosition.auto)
				{
					return true;
				}
				if (this.x == elementPosition.x && this.y == elementPosition.y && this.width == elementPosition.width && this.height == elementPosition.height)
				{
					return true;
				}
			}
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			string result = "Auto";
			if (!this.auto)
			{
				result = this.x.ToString(CultureInfo.CurrentCulture) + ", " + this.y.ToString(CultureInfo.CurrentCulture) + ", " + this.width.ToString(CultureInfo.CurrentCulture) + ", " + this.height.ToString(CultureInfo.CurrentCulture);
			}
			return result;
		}

		internal void SetPositionNoAuto(float x, float y, float width, float height)
		{
			bool flag = this.auto;
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
			this.auto = flag;
		}

		private void Invalidate()
		{
		}
	}
}
