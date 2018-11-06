using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Globalization;
using System.Reflection;
using System.Timers;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	[Bindable(true)]
	[TypeConverter(typeof(InputValueConverter))]
	internal class InputValue : ValueBase
	{
		private CalculatedValueCollection calculatedValues;

		private string dataMember = string.Empty;

		private string dateFieldMember = string.Empty;

		private string valueFieldMember = string.Empty;

		private InputValue playBackValue;

		private Timer playBackTimer;

		private int playBackMarker;

		private float speedMultiplier = 1f;

		private bool pointersInitialized;

		private bool paused;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CalculatedValueCollection CalculatedValues
		{
			get
			{
				return this.calculatedValues;
			}
		}

		[TypeConverter(typeof(DoubleNanValueConverter))]
		[Browsable(true)]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryData")]
		[SRDescription("DescriptionAttributeInputValue_Value")]
		[DefaultValue(double.NaN)]
		[Bindable(true)]
		public virtual double Value
		{
			get
			{
				return base.GetValue();
			}
			set
			{
				base.SetValueInternal(value);
			}
		}

		[SRCategory("CategoryBehavior")]
		[DefaultValue(0L)]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeInputValue_HistoryDepth")]
		public long HistoryDepth
		{
			get
			{
				return (long)this.HistoryDeptInternal.Count;
			}
			set
			{
				this.HistoryDeptInternal.Count = (double)value;
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeInputValue_HistoryDepthType")]
		[Bindable(true)]
		[DefaultValue(DurationType.Count)]
		public DurationType HistoryDepthType
		{
			get
			{
				return this.HistoryDeptInternal.DurationType;
			}
			set
			{
				this.HistoryDeptInternal.DurationType = value;
			}
		}

		[DefaultValue("")]
		[SRDescription("DescriptionAttributeInputValue_DataMember")]
		[TypeConverter(typeof(DataMemberConverter))]
		[SRCategory("CategoryData")]
		[Bindable(true)]
		public string DataMember
		{
			get
			{
				return this.dataMember;
			}
			set
			{
				if (this.dataMember != value)
				{
					this.dataMember = value;
					if (this.Common != null)
					{
						this.Common.GaugeCore.boundToDataSource = false;
					}
				}
			}
		}

		[DefaultValue(null)]
		[SRCategory("CategoryData")]
		[TypeConverter(typeof(DataSourceConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeDataSource")]
		internal object DataSource
		{
			get
			{
				if (this.Common != null)
				{
					return this.Common.GaugeCore.DataSource;
				}
				return null;
			}
		}

		[SRCategory("CategoryData")]
		[DefaultValue("")]
		[Bindable(true)]
		[TypeConverter(typeof(DataFieldMemberConverter))]
		[SRDescription("DescriptionAttributeInputValue_DateFieldMember")]
		public string DateFieldMember
		{
			get
			{
				return this.dateFieldMember;
			}
			set
			{
				if (this.dateFieldMember != value)
				{
					this.dateFieldMember = value;
					if (this.Common != null)
					{
						this.Common.GaugeCore.boundToDataSource = false;
					}
				}
			}
		}

		[Bindable(true)]
		[SRDescription("DescriptionAttributeInputValue_ValueFieldMember")]
		[SRCategory("CategoryData")]
		[TypeConverter(typeof(DataFieldMemberConverter))]
		[DefaultValue("")]
		public string ValueFieldMember
		{
			get
			{
				return this.valueFieldMember;
			}
			set
			{
				if (this.valueFieldMember != value)
				{
					this.valueFieldMember = value;
					if (this.Common != null)
					{
						this.Common.GaugeCore.boundToDataSource = false;
					}
				}
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
				this.calculatedValues.Common = value;
			}
		}

		internal float SpeedMultiplier
		{
			get
			{
				return this.speedMultiplier;
			}
		}

		public InputValue()
		{
			this.calculatedValues = new CalculatedValueCollection(this, null);
		}

		public void IncValue()
		{
			this.IncValue(1.0);
		}

		public void IncValue(double increment)
		{
			base.SetValueInternal(this.Value + increment);
		}

		public void SetValue(double value)
		{
			base.SetValueInternal(value, DateTime.Now);
		}

		public void SetValue(double value, DateTime timestamp)
		{
			base.SetValueInternal(value, timestamp);
		}

		private void InitTimer()
		{
			this.playBackTimer = new Timer();
			this.playBackTimer.Elapsed += this.OnPlayBackRefresh;
			this.playBackTimer.AutoReset = false;
			this.playBackTimer.Enabled = false;
		}

		private void OnPlayBackRefreshInternal()
		{
			lock (this)
			{
				if (this.IntState != ValueState.Interactive)
				{
					if (this.playBackMarker >= this.playBackValue.History.Count)
					{
						goto IL_016b;
					}
					DateTime timestamp = this.playBackValue.History[this.playBackMarker].Timestamp;
					double value = this.playBackValue.History[this.playBackMarker].Value;
					this.playBackValue.outputValue = value;
					this.playBackValue.outputDate = timestamp;
					this.playBackValue.inputValue = value;
					this.playBackValue.inputDate = timestamp;
					if (!this.pointersInitialized)
					{
						this.playBackValue.RefreshConsumers();
						this.pointersInitialized = true;
					}
					this.playBackValue.OnValueChanged(new ValueChangedEventArgs(value, timestamp, this.playBackValue.Name, true));
					if (!this.paused)
					{
						while (++this.playBackMarker < this.playBackValue.History.Count)
						{
							DateTime timestamp2 = this.playBackValue.History[this.playBackMarker].Timestamp;
							TimeSpan timeSpan = timestamp2 - timestamp;
							if (timeSpan.TotalMilliseconds != 0.0)
							{
								this.playBackTimer.Interval = (double)(int)(timeSpan.TotalMilliseconds / (double)this.playBackValue.speedMultiplier);
								this.playBackTimer.Start();
								return;
							}
						}
						goto IL_016b;
					}
				}
				goto end_IL_0009;
				IL_016b:
				this.PlaybackComplete();
				end_IL_0009:;
			}
		}

		private void OnPlayBackRefresh(object sender, ElapsedEventArgs e)
		{
			this.playBackTimer.Stop();
			this.OnPlayBackRefreshInternal();
		}

		private void PlaybackReverse(float speedMultiplier, int numberOfRecords)
		{
			lock (this)
			{
				if (base.provderState == ValueState.Suspended && this.playBackValue != null)
				{
					this.Stop();
				}
				this.playBackMarker = numberOfRecords;
				this.IntState = ValueState.Suspended;
				this.paused = false;
				this.playBackValue = (InputValue)this.Clone();
				this.pointersInitialized = false;
				this.playBackValue.IntState = ValueState.Playback;
				this.playBackValue.speedMultiplier = speedMultiplier;
				foreach (IValueConsumer consumer in this.playBackValue.consumers)
				{
					consumer.Reset();
				}
				if (this.playBackTimer == null)
				{
					this.InitTimer();
				}
				if (this.Common != null && this.Common.GaugeContainer != null)
				{
					this.Common.GaugeContainer.OnPlaybackStateChanged(this, new PlaybackStateChangedEventArgs(PlaybackState.Started, this.playBackValue.inputValue, this.playBackValue.inputDate, this.Name));
				}
				this.OnPlayBackRefreshInternal();
			}
		}

		public void Playback(float speedMultiplier)
		{
			if (base.History.Count == 0)
			{
				throw new InvalidOperationException(Utils.SRGetStr("ExceptionPlaybackDataEmpty"));
			}
			if (!this.HistoryDeptInternal.IsTimeBased)
			{
				this.PlaybackReverse(speedMultiplier, 0);
			}
			else
			{
				this.PlaybackReverse(speedMultiplier, base.History.Locate(base.History.Top.Timestamp - this.HistoryDeptInternal.ToTimeSpan()));
			}
		}

		public void Playback(float speedMultiplier, DateTime startTime)
		{
			if (base.History.Count == 0)
			{
				throw new InvalidOperationException(Utils.SRGetStr("ExceptionPlaybackDataEmpty"));
			}
			this.PlaybackReverse(speedMultiplier, base.History.Locate(startTime));
		}

		public void Playback(float speedMultiplier, int numberOfRecords)
		{
			if (base.History.Count == 0)
			{
				throw new InvalidOperationException(Utils.SRGetStr("ExceptionPlaybackDataEmpty"));
			}
			this.PlaybackReverse(speedMultiplier, Math.Max(0, base.History.Count - numberOfRecords));
		}

		public void Stop()
		{
			lock (this)
			{
				if (base.provderState == ValueState.Suspended)
				{
					this.IntState = ValueState.Interactive;
					if (this.playBackValue != null)
					{
						if (this.Common != null && this.Common.GaugeContainer != null)
						{
							this.Common.GaugeContainer.OnPlaybackStateChanged(this, new PlaybackStateChangedEventArgs(PlaybackState.Stopped, this.playBackValue.inputValue, this.playBackValue.inputDate, this.Name));
						}
						this.playBackValue.Dispose();
						this.playBackValue = null;
						this.RefreshConsumers();
					}
				}
			}
		}

		public void Pause()
		{
			if (this.IntState == ValueState.Suspended)
			{
				this.paused = true;
				if (this.playBackValue != null && this.Common != null && this.Common.GaugeContainer != null)
				{
					this.Common.GaugeContainer.OnPlaybackStateChanged(this, new PlaybackStateChangedEventArgs(PlaybackState.Paused, this.playBackValue.inputValue, this.playBackValue.inputDate, this.Name));
				}
			}
		}

		public void Resume()
		{
			if (this.IntState == ValueState.Suspended)
			{
				this.paused = false;
				if (this.playBackValue != null && this.Common != null && this.Common.GaugeContainer != null)
				{
					this.Common.GaugeContainer.OnPlaybackStateChanged(this, new PlaybackStateChangedEventArgs(PlaybackState.Resumed, this.playBackValue.inputValue, this.playBackValue.inputDate, this.Name));
				}
				this.OnPlayBackRefreshInternal();
			}
		}

		public double GetPlaybackValue()
		{
			if (this.playBackValue == null)
			{
				return double.NaN;
			}
			return this.playBackValue.Value;
		}

		public DateTime GetPlaybackTimestamp()
		{
			if (this.playBackValue == null)
			{
				return DateTime.MaxValue;
			}
			return this.playBackValue.Date;
		}

		public bool IsPlaybackMode()
		{
			if (this.playBackValue == null)
			{
				return false;
			}
			return true;
		}

		public DataTable GetData()
		{
			return base.History.ToDataTable();
		}

		public DataTable GetData(int toPoint)
		{
			return base.History.ToDataTable(toPoint);
		}

		public DataTable GetData(DateTime toPoint)
		{
			return base.History.ToDataTable(toPoint);
		}

		public void DataBind()
		{
			if (this.DataSource != null)
			{
				this.DataBind(this.DataSource, this.ValueFieldMember, this.DateFieldMember, this.DataMember);
			}
		}

		public void DataBind(object dataSource, string valueFieldName, string dateFieldName)
		{
			this.DataBind(dataSource, valueFieldName, dateFieldName, string.Empty);
		}

		public void DataBind(object dataSource, string valueFieldName, string dateFieldName, string dataMember)
		{
			if (this.IntState != ValueState.Interactive)
			{
				throw new ApplicationException(Utils.SRGetStr("ExceptionDatabindState"));
			}
			bool flag = default(bool);
			IDbConnection dbConnection = default(IDbConnection);
			object dataSourceAsIEnumerable = this.GetDataSourceAsIEnumerable(dataSource, dataMember, out flag, out dbConnection);
			try
			{
				if (dataSource != null && !(valueFieldName == string.Empty))
				{
					object obj = null;
					object obj2 = null;
					double num = 0.0;
					DateTime now = DateTime.Now;
					HistoryCollection historyCollection = new HistoryCollection(this);
					try
					{
						this.IntState = ValueState.DataLoading;
						this.Reset();
						foreach (object item in (IEnumerable)dataSourceAsIEnumerable)
						{
							obj = this.ConvertEnumerationItem(item, valueFieldName);
							if (dateFieldName != string.Empty)
							{
								obj2 = this.ConvertEnumerationItem(item, dateFieldName);
								if (obj != null && obj2 != null && obj2 != Convert.DBNull)
								{
									num = ((obj != Convert.DBNull) ? Convert.ToDouble(obj, CultureInfo.InvariantCulture) : double.NaN);
									DateTime timestamp = Convert.ToDateTime(obj2, CultureInfo.InvariantCulture);
									historyCollection.Add(timestamp, num);
								}
							}
							else if (obj != null)
							{
								num = ((obj != Convert.DBNull) ? Convert.ToDouble(obj, CultureInfo.InvariantCulture) : double.NaN);
								historyCollection.Add(now, num);
							}
						}
						foreach (HistoryEntry item2 in historyCollection)
						{
							this.SetValueInternal(item2.Value, item2.Timestamp);
						}
					}
					finally
					{
						this.IntState = ValueState.Interactive;
						historyCollection.Clear();
					}
					this.Invalidate();
					goto end_IL_002d;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionBadDatasource_fields"));
				end_IL_002d:;
			}
			finally
			{
				if (flag)
				{
					((IDataReader)dataSourceAsIEnumerable).Close();
				}
				if (dbConnection != null)
				{
					dbConnection.Close();
				}
			}
			if (this.Common != null)
			{
				this.Common.GaugeCore.boundToDataSource = true;
			}
		}

		internal void AutoDataBind()
		{
			if (this.DataSource != null && this.valueFieldMember != string.Empty)
			{
				this.DataBind();
			}
		}

		internal void PerformDataBinding(IEnumerable data)
		{
			this.DataBind(data, this.ValueFieldMember, this.DateFieldMember, this.DataMember);
		}

		private IEnumerable GetDataSourceAsIEnumerable(object data, string dataMember, out bool closeDataReader, out IDbConnection connection)
		{
			object obj = data;
			closeDataReader = false;
			connection = null;
			if (obj != null)
			{
				try
				{
					 if (obj is SqlDataAdapter)
					{
						obj = ((SqlDataAdapter)obj).SelectCommand;
					}
					//else if (obj is OleDbDataAdapter)
					//{
					//	obj = ((OleDbDataAdapter)obj).SelectCommand;
					//}
					if (obj is DataSet && ((DataSet)obj).Tables.Count > 0)
					{
						obj = ((!(dataMember == string.Empty) && ((DataSet)obj).Tables.Count != 1) ? ((DataSet)obj).Tables[dataMember] : ((DataSet)obj).Tables[0]);
					}
					if (obj is DataTable)
					{
						obj = new DataView((DataTable)obj);
					}
					else if (obj is IDbCommand)
					{
						if (((IDbCommand)obj).Connection.State != ConnectionState.Open)
						{
							connection = ((IDbCommand)obj).Connection;
							connection.Open();
						}
						obj = ((IDbCommand)obj).ExecuteReader();
						closeDataReader = true;
					}
					else
					{
						obj = (obj as IEnumerable);
					}
				}
				catch (Exception innerException)
				{
					throw new ApplicationException(Utils.SRGetStr("ExceptionBadDatasource"), innerException);
				}
				return obj as IEnumerable;
			}
			return null;
		}

		private object ConvertEnumerationItem(object item, string fieldName)
		{
			object result = item;
			bool flag = true;
			if (item is DataRow)
			{
				if (fieldName != null && fieldName.Length > 0)
				{
					if (((DataRow)item).Table.Columns.Contains(fieldName))
					{
						result = ((DataRow)item)[fieldName];
						flag = false;
					}
					else
					{
						try
						{
							int num = int.Parse(fieldName, CultureInfo.InvariantCulture);
							if (((DataRow)item).Table.Columns.Count < num && num >= 0)
							{
								result = ((DataRow)item)[num];
								flag = false;
							}
						}
						catch
						{
						}
					}
				}
			}
			else if (item is DataRowView)
			{
				if (fieldName != null && fieldName.Length > 0)
				{
					if (((DataRowView)item).DataView.Table.Columns.Contains(fieldName))
					{
						result = ((DataRowView)item)[fieldName];
						flag = false;
					}
					else
					{
						try
						{
							int num2 = int.Parse(fieldName, CultureInfo.InvariantCulture);
							if (((DataRowView)item).DataView.Table.Columns.Count < num2 && num2 >= 0)
							{
								result = ((DataRowView)item)[num2];
								flag = false;
							}
						}
						catch
						{
						}
					}
				}
			}
			else if (item is DbDataRecord)
			{
				if (fieldName != null && fieldName.Length > 0)
				{
					if (!char.IsNumber(fieldName, 0))
					{
						try
						{
							result = ((DbDataRecord)item)[fieldName];
							flag = false;
						}
						catch (Exception)
						{
						}
					}
					if (flag)
					{
						try
						{
							int i = int.Parse(fieldName, CultureInfo.InvariantCulture);
							result = ((DbDataRecord)item)[i];
							flag = false;
						}
						catch
						{
						}
					}
				}
			}
			else if (item != null)
			{
				try
				{
					PropertyInfo property = item.GetType().GetProperty(fieldName, BindingFlags.Instance | BindingFlags.Public);
					if (property != null)
					{
						result = property.GetValue(item, new object[0]);
						flag = false;
					}
					else
					{
						FieldInfo field = item.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public);
						if (field != null)
						{
							result = field.GetValue(item);
							flag = false;
						}
					}
				}
				catch
				{
				}
			}
			if (flag)
			{
				throw new ArgumentException(Utils.SRGetStr("ExceptionColumnNotFound", fieldName));
			}
			return result;
		}

		private void PlaybackComplete()
		{
			lock (this)
			{
				this.IntState = ValueState.Interactive;
				if (this.playBackValue != null)
				{
					if (this.Common != null && this.Common.GaugeContainer != null)
					{
						this.Common.GaugeContainer.OnPlaybackStateChanged(this, new PlaybackStateChangedEventArgs(PlaybackState.Complete, this.playBackValue.inputValue, this.playBackValue.inputDate, this.Name));
					}
					this.playBackValue.Dispose();
					this.playBackValue = null;
					this.RefreshConsumers();
				}
			}
		}

		internal override void BeginInit()
		{
			base.BeginInit();
			this.calculatedValues.BeginInit();
		}

		internal override void EndInit()
		{
			base.EndInit();
			this.calculatedValues.EndInit();
		}

		protected override void OnDispose()
		{
			this.calculatedValues.Dispose();
			base.OnDispose();
		}

		public override void Reset()
		{
			this.Stop();
			base.Reset();
		}

		internal override object CloneInternals(object copy)
		{
			InputValue inputValue = (InputValue)base.CloneInternals(copy);
			inputValue.calculatedValues = new CalculatedValueCollection(inputValue, null);
			foreach (CalculatedValue calculatedValue in this.CalculatedValues)
			{
				inputValue.calculatedValues.Add((CalculatedValue)calculatedValue.Clone());
			}
			inputValue.calculatedValues.EndInit();
			return inputValue;
		}
	}
}
