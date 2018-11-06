using System;
using System.ComponentModel;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	[TypeConverter(typeof(CalculatedValueConverter))]
	internal class CalculatedValue : ValueBase, IValueConsumer
	{
		private IValueProvider provider;

		private TimerData timerData = new TimerData();

		internal bool timerRefreshCall;

		internal bool noMoreData;

		internal GaugePeriod refreshRate = new GaugePeriod(double.NaN, AspNetCore.Reporting.Gauge.WebForms.PeriodType.Seconds);

		internal GaugeDuration aggregateDuration = new GaugeDuration(0.0, DurationType.Infinite);

		private string baseValueName = string.Empty;

		[SRCategory("CategoryBehavior")]
		[Bindable(true)]
		[DefaultValue(double.NaN)]
		[TypeConverter(typeof(DoubleNanValueConverter))]
		[SRDescription("DescriptionAttributeRefreshRate3")]
		public virtual double RefreshRate
		{
			get
			{
				return this.refreshRate.Duration;
			}
			set
			{
				this.refreshRate.Duration = value;
				this.InitTimerData();
			}
		}

		[Bindable(true)]
		[DefaultValue(AspNetCore.Reporting.Gauge.WebForms.PeriodType.Seconds)]
		[SRDescription("DescriptionAttributeRefreshRateType")]
		[SRCategory("CategoryBehavior")]
		public virtual PeriodType RefreshRateType
		{
			get
			{
				return this.refreshRate.PeriodType;
			}
			set
			{
				this.refreshRate.PeriodType = value;
				this.InitTimerData();
			}
		}

		[SRCategory("CategoryBehavior")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributePeriod")]
		[DefaultValue(0L)]
		public virtual long Period
		{
			get
			{
				return (long)this.aggregateDuration.Count;
			}
			set
			{
				this.aggregateDuration.Count = (double)value;
			}
		}

		[SRCategory("CategoryBehavior")]
		[DefaultValue(DurationType.Infinite)]
		[Bindable(true)]
		[SRDescription("DescriptionAttributePeriodType")]
		public virtual DurationType PeriodType
		{
			get
			{
				return this.aggregateDuration.DurationType;
			}
			set
			{
				this.aggregateDuration.DurationType = value;
			}
		}

		[ParenthesizePropertyName(true)]
		[SRDescription("DescriptionAttributeTypeName")]
		[SRCategory("CategoryBehavior")]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string TypeName
		{
			get
			{
				return base.GetType().Name;
			}
		}

		[SRCategory("CategoryData")]
		[TypeConverter(typeof(CalculatedValueNameConverter))]
		[SRDescription("DescriptionAttributeBaseValueName")]
		[RefreshProperties(RefreshProperties.Repaint)]
		public string BaseValueName
		{
			get
			{
				return this.baseValueName;
			}
			set
			{
				if (!(this.baseValueName != value) && this.provider != null)
				{
					return;
				}
				if (value != string.Empty && value == this.Name)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionCircularReference"));
				}
				string text = this.baseValueName;
				try
				{
					this.baseValueName = value;
					this.AttachToProvider();
				}
				catch
				{
					this.baseValueName = text;
					throw;
				}
			}
		}

		[NotifyParentProperty(true)]
		[Browsable(true)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue(double.NaN)]
		[TypeConverter(typeof(DoubleNanValueConverter))]
		[SRDescription("DescriptionAttributeValue8")]
		[SRCategory("CategoryData")]
		[Bindable(false)]
		public double Value
		{
			get
			{
				return this.GetValue();
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
				if (this.Common != null && !this.Common.GaugeCore.isInitializing)
				{
					this.AttachToProvider();
				}
			}
		}

		internal InputValue InputValueObj
		{
			get
			{
				return (InputValue)this.Collection.parent;
			}
		}

		public CalculatedValue()
		{
			this.InitTimer();
		}

		protected override void OnDispose()
		{
			base.OnDispose();
		}

		private void InitTimer()
		{
		}

		internal void InitTimerData()
		{
			if (this.Common != null)
			{
				TimeSpan timeSpan = this.refreshRate.ToTimeSpan();
				double num = timeSpan.TotalMilliseconds / (double)this.InputValueObj.SpeedMultiplier;
				if (this.timerData.ticks != timeSpan)
				{
					lock (this)
					{
						this.timerData.ticks = timeSpan;
					}
				}
			}
		}

		internal void StartTimer()
		{
		}

		protected void StopTimer()
		{
		}

		private void AttachToProvider()
		{
			if (this.provider != null)
			{
				if (this.provider.GetValueProviderName() == this.baseValueName)
				{
					return;
				}
				this.provider.DetachConsumer(this);
				this.provider = null;
			}
			if (!(this.baseValueName == string.Empty) && this.IsConnectedInCollecton())
			{
				this.provider = this.LocateProviderByName(this.baseValueName);
				if (this.provider != null)
				{
					this.provider.AttachConsumer(this);
					this.SetValueInternal(this.provider.GetValue(), this.provider.GetDate());
					((IValueConsumer)this).Refresh();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptioncalCulatedValueProvider", this.baseValueName));
			}
		}

		private IValueProvider LocateProviderByName(string name)
		{
			object obj = null;
			if (this.Collection != null)
			{
				obj = this.Collection.GetByName(name);
				if (obj == null && this.InputValueObj.Name == name)
				{
					obj = this.Collection.parent;
				}
			}
			if (obj is IValueProvider)
			{
				return (IValueProvider)obj;
			}
			return null;
		}

		private bool IsConnectedInCollecton()
		{
			if (this.Collection != null)
			{
				return this.Collection.parent != null;
			}
			return false;
		}

		internal override void OnAdded()
		{
			base.OnAdded();
			if (this.baseValueName == string.Empty && this.IsConnectedInCollecton())
			{
				this.BaseValueName = this.InputValueObj.Name;
			}
			if (this.Common != null && !this.Common.GaugeCore.isInitializing)
			{
				this.AttachToProvider();
			}
		}

		internal override void EndInit()
		{
			base.EndInit();
			this.InitTimerData();
			this.AttachToProvider();
		}

		internal override object CloneInternals(object copy)
		{
			copy = base.CloneInternals(copy);
			((CalculatedValue)copy).provider = null;
			((CalculatedValue)copy).refreshRate = this.refreshRate.Clone();
			((CalculatedValue)copy).timerData = (TimerData)this.timerData.Clone();
			((CalculatedValue)copy).InitTimer();
			((CalculatedValue)copy).aggregateDuration = (GaugeDuration)this.aggregateDuration.Clone();
			return copy;
		}

		internal override void Recalculate(double value, DateTime timestamp)
		{
			this.noMoreData = false;
			base.Recalculate(value, timestamp);
		}

		internal override void CalculateValue(double value, DateTime timestamp)
		{
			base.CalculateValue(value, timestamp);
			if (this.IsConnectedInCollecton() && !this.noMoreData && !double.IsNaN(base.outputValue))
			{
				this.StopTimer();
				this.StartTimer();
			}
			else
			{
				this.StopTimer();
			}
		}

		void IValueConsumer.ProviderRemoved(IValueProvider provider)
		{
			if (this.provider == provider)
			{
				this.provider = null;
				this.baseValueName = string.Empty;
			}
		}

		void IValueConsumer.ProviderNameChanged(IValueProvider provider)
		{
			this.baseValueName = provider.GetValueProviderName();
			this.OnNameChanged();
		}

		void IValueConsumer.InputValueChanged(object sender, ValueChangedEventArgs e)
		{
			if (sender == this.provider)
			{
				this.SetValueInternal(e.Value, e.Date);
			}
		}

		IValueProvider IValueConsumer.GetProvider()
		{
			return this.provider;
		}

		void IValueConsumer.Reset()
		{
			this.Reset();
		}

		void IValueConsumer.Refresh()
		{
			this.RefreshConsumers();
		}
	}
}
