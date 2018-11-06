using System;
using System.Collections;
using System.Globalization;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class DataAttributes : GaugeObject, IValueConsumer
	{
		private double dValue = double.NaN;

		private bool isPercentBased;

		private double minimum;

		private double maximum = 100.0;

		private string valueSource = "";

		private double oldValue = double.NaN;

		private DateTime dateValueStamp;

		private IValueProvider provider;

		internal double Value
		{
			get
			{
				return this.dValue;
			}
			set
			{
				if (this.dValue != value)
				{
					this.SetValue(value, false);
				}
			}
		}

		public bool IsPercentBased
		{
			get
			{
				return this.isPercentBased;
			}
			set
			{
				this.isPercentBased = value;
				this.Invalidate();
			}
		}

		public double Minimum
		{
			get
			{
				return this.minimum;
			}
			set
			{
				if (this.Common != null)
				{
					if (!(value > this.Maximum) && (value != this.Maximum || value == 0.0))
					{
						if (!double.IsNaN(value) && !double.IsInfinity(value))
						{
							goto IL_0062;
						}
						throw new ArgumentException(Utils.SRGetStr("ExceptionInvalidValue"));
					}
					throw new ArgumentException(Utils.SRGetStr("ExceptionMinMax"));
				}
				goto IL_0062;
				IL_0062:
				this.minimum = value;
				this.Invalidate();
			}
		}

		public double Maximum
		{
			get
			{
				return this.maximum;
			}
			set
			{
				if (this.Common != null)
				{
					if (!(value < this.Minimum) && (value != this.Minimum || value == 0.0))
					{
						if (!double.IsNaN(value) && !double.IsInfinity(value))
						{
							goto IL_0062;
						}
						throw new ArgumentException(Utils.SRGetStr("ExceptionInvalidValue"));
					}
					throw new ArgumentException(Utils.SRGetStr("ExceptionMaxMin"));
				}
				goto IL_0062;
				IL_0062:
				this.maximum = value;
				this.Invalidate();
			}
		}

		internal string ValueSource
		{
			get
			{
				return this.valueSource;
			}
			set
			{
				if (value == "(none)")
				{
					value = string.Empty;
				}
				if (this.valueSource != value)
				{
					this.valueSource = value;
					this.AttachToProvider(false);
					this.Invalidate();
				}
			}
		}

		internal double OldValue
		{
			get
			{
				return this.oldValue;
			}
			set
			{
				this.oldValue = value;
			}
		}

		internal DateTime DateValueStamp
		{
			get
			{
				return this.dateValueStamp;
			}
		}

		public DataAttributes()
			: this(null)
		{
		}

		internal DataAttributes(object parent)
			: base(parent)
		{
		}

		internal double GetValueInPercents()
		{
			double result = double.NaN;
			if (this.IsPercentBased)
			{
				if (this.Maximum - this.Minimum != 0.0)
				{
					result = (this.Value - this.Minimum) / (this.Maximum - this.Minimum) * 100.0;
				}
				else if (this.Minimum == this.Maximum && this.Value == this.Minimum)
				{
					result = 100.0;
				}
			}
			return result;
		}

		internal void SetValue(double value, bool initialize)
		{
			this.OldValue = this.dValue;
			this.dValue = value;
			if (this.Parent != null)
			{
				if (initialize)
				{
					this.StopDampening();
				}
				((IPointerProvider)this.Parent).DataValueChanged(initialize);
			}
		}

		private Hashtable CollectValues()
		{
			Hashtable hashtable = new Hashtable();
			if (this.Common != null)
			{
				{
					foreach (InputValue value in this.Common.GaugeCore.Values)
					{
						hashtable.Add(value.Name, value);
						foreach (CalculatedValue calculatedValue in value.CalculatedValues)
						{
							if (!hashtable.ContainsKey(calculatedValue.Name))
							{
								hashtable.Add(calculatedValue.Name, calculatedValue);
							}
							else
							{
								object obj = hashtable[calculatedValue.Name];
								if (!(obj is InputValue))
								{
									hashtable.Remove(calculatedValue.Name);
								}
							}
							hashtable.Add(string.Format(CultureInfo.InvariantCulture, "{0}.{1}", calculatedValue.InputValueObj.Name, calculatedValue.Name), calculatedValue);
						}
					}
					return hashtable;
				}
			}
			return hashtable;
		}

		private void AttachToProvider(bool exact)
		{
			Hashtable hashtable = this.CollectValues();
			if (this.provider != null)
			{
				if (hashtable.ContainsKey(this.valueSource) && hashtable[this.valueSource] == this.provider)
				{
					return;
				}
				this.provider.DetachConsumer(this);
				this.provider = null;
			}
			if (this.Common != null && !this.Common.GaugeCore.isInitializing)
			{
				if (this.valueSource == string.Empty)
				{
					this.SetValue(this.Value, true);
				}
				else
				{
					if (hashtable.ContainsKey(this.ValueSource))
					{
						this.provider = (IValueProvider)hashtable[this.ValueSource];
						this.provider.AttachConsumer(this);
					}
					else if (exact)
					{
						throw new ArgumentException(Utils.SRGetStr("ExceptionLocateCProviderFailed", this.valueSource));
					}
					if (this.provider != null)
					{
						this.SetValue(this.provider.GetValue(), true);
					}
				}
			}
		}

		internal override void EndInit()
		{
			base.EndInit();
			this.AttachToProvider(true);
		}

		internal override void ReconnectData(bool exact)
		{
			this.AttachToProvider(exact);
		}

		internal override void Notify(MessageType msg, NamedElement element, object param)
		{
			base.Notify(msg, element, param);
			if (msg == MessageType.DataInvalidated)
			{
				((IValueConsumer)this).Refresh();
			}
		}

		internal bool StartDampening(double targetValue, double minimum, double maximum, double dampeningSweepTime, double refreshRate)
		{
			if (this.Common == null)
			{
				return false;
			}
			if (dampeningSweepTime <= 0.0)
			{
				return false;
			}
			IPointerProvider pointerProvider = (IPointerProvider)this.Parent;
			double num = (maximum - minimum) / (dampeningSweepTime * refreshRate);
			if (num > 0.0 && Math.Abs(pointerProvider.Position - targetValue) > num)
			{
				return true;
			}
			return false;
		}

		internal void StopDampening()
		{
		}

		void IValueConsumer.ProviderRemoved(IValueProvider provider)
		{
			if (this.provider == provider)
			{
				this.valueSource = string.Empty;
				this.provider = null;
			}
		}

		void IValueConsumer.ProviderNameChanged(IValueProvider provider)
		{
			if (provider is CalculatedValue)
			{
				this.valueSource = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", ((CalculatedValue)provider).InputValueObj.Name, provider.GetValueProviderName());
			}
			else
			{
				this.valueSource = provider.GetValueProviderName();
			}
		}

		void IValueConsumer.InputValueChanged(object sender, ValueChangedEventArgs e)
		{
			IValueProvider valueProvider = (IValueProvider)sender;
			if (valueProvider.GetProvderState() != ValueState.Interactive && valueProvider.GetProvderState() != ValueState.Playback)
			{
				return;
			}
			this.dateValueStamp = e.Date;
			this.Value = e.Value;
		}

		IValueProvider IValueConsumer.GetProvider()
		{
			return this.provider;
		}

		void IValueConsumer.Reset()
		{
			this.SetValue(double.NaN, true);
		}

		void IValueConsumer.Refresh()
		{
			if (this.provider != null)
			{
				this.dateValueStamp = this.provider.GetDate();
				this.SetValue(this.provider.GetValue(), true);
			}
			else
			{
				this.SetValue(this.Value, true);
			}
		}

		protected override void OnDispose()
		{
			base.OnDispose();
		}
	}
}
