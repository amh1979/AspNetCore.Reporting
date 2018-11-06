using System;
using System.Collections;
using System.ComponentModel;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class Range : NamedElement
	{
		private Hashtable inRangeTable = new Hashtable();

		private double startValue = 70.0;

		private double endValue = 100.0;

		private double inRangeTimeout;

		private PeriodType inRangeTimeoutType = PeriodType.Seconds;

		public virtual double StartValue
		{
			get
			{
				return this.startValue;
			}
			set
			{
				this.startValue = value;
				this.InvalidateState();
				this.Invalidate();
			}
		}

		public virtual double EndValue
		{
			get
			{
				return this.endValue;
			}
			set
			{
				this.endValue = value;
				this.InvalidateState();
				this.Invalidate();
			}
		}

		[Browsable(false)]
		[DefaultValue(0.0)]
		public virtual double InRangeTimeout
		{
			get
			{
				return this.inRangeTimeout;
			}
			set
			{
				if (value < 0.0)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionNegativeValue"));
				}
				this.inRangeTimeout = value;
			}
		}

		[Browsable(false)]
		[DefaultValue(PeriodType.Seconds)]
		public virtual PeriodType InRangeTimeoutType
		{
			get
			{
				return this.inRangeTimeoutType;
			}
			set
			{
				this.inRangeTimeoutType = value;
			}
		}

		public Range()
		{
		}

		protected Range(double start, double end)
		{
			this.startValue = start;
			this.endValue = end;
		}

		internal RangeDataState GetDataState(DataAttributes data)
		{
			if (this.inRangeTable.ContainsKey(data))
			{
				return (RangeDataState)this.inRangeTable[data];
			}
			RangeDataState rangeDataState = new RangeDataState(this, data);
			this.inRangeTable.Add(data, rangeDataState);
			return rangeDataState;
		}

		private void InvalidateState()
		{
			foreach (DataAttributes key in this.inRangeTable.Keys)
			{
				this.PointerValueChanged(key);
			}
		}

		internal virtual void OnValueRangeTimeOut(object sender, ValueRangeEventArgs e)
		{
			if (this.Common != null)
			{
				this.Common.GaugeContainer.OnValueRangeTimeOut(sender, e);
			}
		}

		internal virtual void OnValueRangeEnter(object sender, ValueRangeEventArgs e)
		{
			if (this.Common != null)
			{
				this.Common.GaugeContainer.OnValueRangeEnter(sender, e);
			}
		}

		internal virtual void OnValueRangeLeave(object sender, ValueRangeEventArgs e)
		{
			if (this.Common != null)
			{
				this.Common.GaugeContainer.OnValueRangeLeave(sender, e);
			}
		}

		internal virtual void PointerValueChanged(DataAttributes data)
		{
			double num = Math.Min(this.startValue, this.endValue);
			double num2 = Math.Max(this.startValue, this.endValue);
			if (this.Common != null)
			{
				bool playbackMode = false;
				if (((IValueConsumer)data).GetProvider() != null)
				{
					playbackMode = ((IValueConsumer)data).GetProvider().GetPlayBackMode();
				}
				NamedElement pointer = (NamedElement)data.Parent;
				RangeDataState dataState = this.GetDataState(data);
				double num3 = data.Value;
				if (data.IsPercentBased)
				{
					num3 = data.GetValueInPercents();
				}
				if (!dataState.IsInRange && num3 >= num && num3 <= num2)
				{
					dataState.IsInRange = true;
					this.OnValueRangeEnter(this, new ValueRangeEventArgs(num3, data.DateValueStamp, this.Name, playbackMode, pointer));
					if (!(this.inRangeTimeout > 0.0))
					{
						dataState.IsTimerExceed = true;
					}
				}
				if (dataState.IsInRange)
				{
					if (!double.IsNaN(num3) && !(num3 < num) && !(num3 > num2))
					{
						return;
					}
					dataState.IsInRange = false;
					dataState.IsTimerExceed = false;
					this.OnValueRangeLeave(this, new ValueRangeEventArgs(num3, data.DateValueStamp, this.Name, playbackMode, pointer));
					double inRangeTimeout2 = this.inRangeTimeout;
				}
			}
		}

		internal override void Notify(MessageType msg, NamedElement element, object param)
		{
			base.Notify(msg, element, param);
			if (msg == MessageType.NamedElementRemove)
			{
				ArrayList arrayList = new ArrayList();
				foreach (DataAttributes key in this.inRangeTable.Keys)
				{
					if (key.Parent == element)
					{
						arrayList.Add(key);
					}
				}
				for (int i = 0; i < arrayList.Count; i++)
				{
					this.inRangeTable.Remove(arrayList[i]);
				}
				arrayList.Clear();
			}
		}

		protected override void OnDispose()
		{
			this.inRangeTable.Clear();
			base.OnDispose();
		}
	}
}
