using System;
using System.Collections;
using System.ComponentModel;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	[Serializable]
	internal class ValueBase : NamedElement, IValueProvider, ICloneable
	{
		[NonSerialized]
		internal GaugeDuration historyDept = new GaugeDuration(0.0, DurationType.Count);

		[NonSerialized]
		internal GaugeDuration queryDept = new GaugeDuration(1.0, DurationType.Count);

		private double valueLimit = double.NaN;

		internal double inputValue = double.NaN;

		internal DateTime inputDate = DateTime.Now;

		internal double outputValue = double.NaN;

		internal DateTime outputDate = DateTime.Now;

		internal HistoryCollection history;

		internal ArrayList consumers = new ArrayList();

		internal ValueState provderState = ValueState.Interactive;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public HistoryCollection History
		{
			get
			{
				return this.history;
			}
		}

		[SRCategory("CategoryMisc")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeName")]
		[Browsable(true)]
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

		[SRDescription("DescriptionAttributeDate")]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SRCategory("CategoryData")]
		public DateTime Date
		{
			get
			{
				return this.outputDate;
			}
		}

		[SRDescription("DescriptionAttributeValueLimit")]
		[TypeConverter(typeof(DoubleNanValueConverter))]
		[SRCategory("CategoryBehavior")]
		[Bindable(true)]
		[DefaultValue(double.NaN)]
		internal virtual double ValueLimit
		{
			get
			{
				return this.valueLimit;
			}
			set
			{
				this.valueLimit = value;
			}
		}

		internal virtual GaugeDuration HistoryDeptInternal
		{
			get
			{
				return this.historyDept;
			}
		}

		[SRCategory("CategoryMisc")]
		[Browsable(false)]
		[SRDescription("DescriptionAttributeState")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[Bindable(false)]
		internal virtual ValueState State
		{
			get
			{
				if (this.IntState == ValueState.Suspended)
				{
					return ValueState.Playback;
				}
				return this.IntState;
			}
		}

		internal virtual ValueState IntState
		{
			get
			{
				return this.provderState;
			}
			set
			{
				this.provderState = value;
				foreach (object consumer in this.consumers)
				{
					if (consumer is ValueBase)
					{
						((ValueBase)consumer).IntState = value;
					}
				}
			}
		}

		internal bool IsPlayBackMode
		{
			get
			{
				return this.provderState == ValueState.Playback;
			}
		}

		internal bool IsEventsEnable
		{
			get
			{
				if (this.provderState != ValueState.Interactive)
				{
					return this.provderState == ValueState.Playback;
				}
				return true;
			}
		}

		internal event ValueChangedEventHandler ValueChanged;

		internal event ValueChangedEventHandler ValueLimitOverflow;

		internal ValueBase()
		{
			this.history = new HistoryCollection(this);
		}

		public override string ToString()
		{
			return this.Name;
		}

		public virtual void Reset()
		{
			this.History.Clear();
			this.queryDept = new GaugeDuration(this.historyDept.Count, this.historyDept.DurationType);
			foreach (IValueConsumer consumer in this.consumers)
			{
				consumer.Reset();
			}
		}

		internal virtual void SetValueInternal(double value)
		{
			this.SetValueInternal(value, DateTime.Now);
		}

		internal virtual void SetValueInternal(double value, DateTime timestamp)
		{
			this.inputValue = value;
			this.inputDate = timestamp;
			this.Recalculate(this.inputValue, this.inputDate);
		}

		public virtual double GetValue()
		{
			return this.outputValue;
		}

		internal void ClearUpHistory()
		{
			if (this.History.Count > 0)
			{
				this.queryDept.Extend(this.historyDept, this.inputDate, this.History[0].Timestamp);
			}
			else
			{
				this.queryDept.Extend(this.historyDept, this.inputDate, this.inputDate);
			}
			this.History.Truncate(this.queryDept);
		}

		internal void AddToHistory()
		{
			if (this.queryDept.IsEmpty && this.History.Count != 0)
			{
				return;
			}
			this.AddToHistoryInt(this.outputValue, this.outputDate);
		}

		internal void AddToHistoryInt(double value, DateTime timestamp)
		{
			this.History.Add(timestamp, value);
		}

		internal virtual void OnValueLimitOverflow(ValueChangedEventArgs e)
		{
			if (this.Common != null)
			{
				this.Common.GaugeContainer.OnValueLimitOverflow(this, e);
			}
			if (this.ValueLimitOverflow != null)
			{
				if (this.Common != null && this.Common.GaugeCore.InvokeRequired)
				{
					Delegate[] invocationList = this.ValueLimitOverflow.GetInvocationList();
					for (int i = 0; i < invocationList.Length; i++)
					{
						ValueChangedEventHandler valueChangedEventHandler = (ValueChangedEventHandler)invocationList[i];
						valueChangedEventHandler.BeginInvoke(this, e, null, null);
					}
				}
				else
				{
					this.ValueLimitOverflow(this, e);
				}
			}
		}

		internal virtual void CheckLimit()
		{
			try
			{
				if (!double.IsNaN(this.valueLimit) && this.outputValue > this.valueLimit)
				{
					this.OnValueLimitOverflow(new ValueChangedEventArgs(this.outputValue, this.outputDate, this.Name, this.IsPlayBackMode));
				}
			}
			catch (ApplicationException)
			{
				throw;
			}
		}

		internal virtual void OnValueChanged(ValueChangedEventArgs e)
		{
			if (this.Common != null)
			{
				this.Common.GaugeContainer.OnValueChanged(this, e);
			}
			if (this.ValueChanged != null)
			{
				if (this.Common != null && this.Common.GaugeCore.InvokeRequired)
				{
					Delegate[] invocationList = this.ValueChanged.GetInvocationList();
					for (int i = 0; i < invocationList.Length; i++)
					{
						ValueChangedEventHandler valueChangedEventHandler = (ValueChangedEventHandler)invocationList[i];
						valueChangedEventHandler.BeginInvoke(this, e, null, null);
					}
				}
				else
				{
					this.ValueChanged(this, e);
				}
			}
		}

		internal override void OnNameChanged()
		{
			base.OnNameChanged();
			foreach (IValueConsumer consumer in this.consumers)
			{
				consumer.ProviderNameChanged(this);
			}
		}

		internal override void OnRemove()
		{
			base.OnRemove();
			foreach (IValueConsumer consumer in this.consumers)
			{
				consumer.ProviderRemoved(this);
			}
		}

		internal override void OnAdded()
		{
			base.OnAdded();
			if (this.Common != null)
			{
				this.Common.GaugeCore.ReconnectData(false);
			}
		}

		internal virtual void Recalculate(double value, DateTime timestamp)
		{
			lock (this)
			{
				double num = this.outputValue;
				this.CalculateValue(value, timestamp);
				if (this.IsEventsEnable)
				{
					this.CheckLimit();
				}
				this.AddToHistory();
				if (this.outputValue != num)
				{
					this.OnValueChanged(new ValueChangedEventArgs(this.outputValue, this.outputDate, this.Name, this.IsPlayBackMode));
				}
				this.ClearUpHistory();
			}
		}

		internal virtual void CalculateValue(double value, DateTime timestamp)
		{
			this.outputValue = value;
			this.outputDate = timestamp;
		}

		internal virtual void RefreshConsumers()
		{
			foreach (IValueConsumer consumer in this.consumers)
			{
				consumer.Refresh();
			}
		}

		internal override void Invalidate()
		{
			foreach (IValueConsumer consumer in this.consumers)
			{
				if (consumer is ValueBase)
				{
					((ValueBase)consumer).Invalidate();
				}
				else
				{
					consumer.InputValueChanged(this, new ValueChangedEventArgs(this.GetValue(), this.Date, this.Name, this.IsPlayBackMode));
				}
			}
		}

		protected override void OnDispose()
		{
			this.History.Clear();
			base.OnDispose();
		}

		void IValueProvider.AttachConsumer(IValueConsumer consumer)
		{
			if (!this.consumers.Contains(consumer))
			{
				this.consumers.Add(consumer);
				this.ValueChanged += consumer.InputValueChanged;
			}
		}

		void IValueProvider.DetachConsumer(IValueConsumer consumer)
		{
			if (this.consumers.Contains(consumer))
			{
				this.consumers.Remove(consumer);
				this.ValueChanged -= consumer.InputValueChanged;
			}
		}

		double IValueProvider.GetValue()
		{
			return this.outputValue;
		}

		DateTime IValueProvider.GetDate()
		{
			return this.outputDate;
		}

		string IValueProvider.GetValueProviderName()
		{
			return this.Name;
		}

		HistoryCollection IValueProvider.GetData(GaugeDuration period, DateTime currentDate)
		{
			if (this.History.Count > 0)
			{
				this.queryDept.Extend(period, currentDate, this.History[0].Timestamp);
			}
			return this.History;
		}

		bool IValueProvider.GetPlayBackMode()
		{
			return this.IsPlayBackMode;
		}

		ValueState IValueProvider.GetProvderState()
		{
			return this.provderState;
		}

		internal override object CloneInternals(object copy)
		{
			ValueBase valueBase = (ValueBase)base.CloneInternals(copy);
			valueBase.historyDept = (GaugeDuration)this.historyDept.Clone();
			valueBase.queryDept = (GaugeDuration)this.queryDept.Clone();
			valueBase.history = (HistoryCollection)this.history.Clone();
			valueBase.ValueChanged = null;
			valueBase.ValueLimitOverflow = null;
			valueBase.consumers = new ArrayList();
			foreach (IValueConsumer consumer in this.consumers)
			{
				if (!(consumer is ValueBase))
				{
					((IValueProvider)valueBase).AttachConsumer(consumer);
				}
			}
			return valueBase;
		}
	}
}
