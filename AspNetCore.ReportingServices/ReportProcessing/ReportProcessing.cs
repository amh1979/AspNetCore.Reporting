using Microsoft.Cloud.Platform.Utils;
using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.DataExtensions;
using AspNetCore.ReportingServices.DataProcessing;
using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.Diagnostics.Internal;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.Interfaces;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing.Execution;
using AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.Upgrade;
using AspNetCore.ReportingServices.ReportPublishing;
using AspNetCore.ReportingServices.ReportRendering;
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class ReportProcessing
	{
		private sealed class ShowHideProcessing
		{
			private bool m_showHideInfoChanged;

			private SenderInformationHashtable m_senderInfo;

			private ReceiverInformationHashtable m_receiverInfo;

			private Hashtable m_overrideToggleStateInfo;

			private Hashtable m_overrideHiddenInfo;

			internal void Process(string showHideToggle, ReportSnapshot reportSnapshot, EventInformation oldOverrideInformation, ChunkManager.RenderingChunkManager chunkManager, out bool showHideInformationChanged, out EventInformation newOverrideInformation)
			{
				try
				{
					showHideInformationChanged = false;
					newOverrideInformation = null;
					if (showHideToggle != null)
					{
						Global.Tracer.Assert(null != reportSnapshot, "(null != reportSnapshot)");
						this.m_senderInfo = reportSnapshot.GetShowHideSenderInfo(chunkManager);
						this.m_receiverInfo = reportSnapshot.GetShowHideReceiverInfo(chunkManager);
						this.Process(showHideToggle, oldOverrideInformation, ref showHideInformationChanged, ref newOverrideInformation);
					}
				}
				finally
				{
					this.m_showHideInfoChanged = false;
					this.m_senderInfo = null;
					this.m_receiverInfo = null;
					this.m_overrideToggleStateInfo = null;
					this.m_overrideHiddenInfo = null;
				}
			}

			internal bool Process(string showHideToggle, EventInformation oldOverrideInformation, ChunkManager.EventsChunkManager eventsChunkManager, out bool showHideInformationChanged, out EventInformation newOverrideInformation)
			{
				try
				{
					showHideInformationChanged = false;
					newOverrideInformation = null;
					if (showHideToggle == null)
					{
						return false;
					}
					Global.Tracer.Assert(null != eventsChunkManager, "(null != eventsChunkManager)");
					eventsChunkManager.GetShowHideInfo(out this.m_senderInfo, out this.m_receiverInfo);
					return this.Process(showHideToggle, oldOverrideInformation, ref showHideInformationChanged, ref newOverrideInformation);
				}
				finally
				{
					this.m_showHideInfoChanged = false;
					this.m_senderInfo = null;
					this.m_receiverInfo = null;
					this.m_overrideToggleStateInfo = null;
					this.m_overrideHiddenInfo = null;
				}
			}

			private bool Process(string showHideToggle, EventInformation oldOverrideInformation, ref bool showHideInformationChanged, ref EventInformation newOverrideInformation)
			{
				if (this.m_senderInfo != null && this.m_receiverInfo != null)
				{
					int senderUniqueName = default(int);
					if (!int.TryParse(showHideToggle, NumberStyles.None, (IFormatProvider)CultureInfo.InvariantCulture, out senderUniqueName))
					{
						return false;
					}
					EventInformation.SortEventInfo sortInfo = null;
					if (oldOverrideInformation == null || (oldOverrideInformation.ToggleStateInfo == null && oldOverrideInformation.HiddenInfo == null))
					{
						this.m_overrideToggleStateInfo = new Hashtable();
						this.m_overrideHiddenInfo = new Hashtable();
						if (oldOverrideInformation != null)
						{
							sortInfo = oldOverrideInformation.SortInfo;
						}
					}
					else
					{
						this.m_overrideToggleStateInfo = (Hashtable)oldOverrideInformation.ToggleStateInfo.Clone();
						this.m_overrideHiddenInfo = (Hashtable)oldOverrideInformation.HiddenInfo.Clone();
					}
					bool result = this.ProcessSender(senderUniqueName);
					showHideInformationChanged = this.m_showHideInfoChanged;
					if (!this.m_showHideInfoChanged)
					{
						newOverrideInformation = null;
					}
					else if (this.m_overrideToggleStateInfo.Count == 0 && this.m_overrideHiddenInfo.Count == 0)
					{
						newOverrideInformation = null;
					}
					else
					{
						newOverrideInformation = new EventInformation();
						newOverrideInformation.ToggleStateInfo = this.m_overrideToggleStateInfo;
						newOverrideInformation.HiddenInfo = this.m_overrideHiddenInfo;
						newOverrideInformation.SortInfo = sortInfo;
					}
					return result;
				}
				return false;
			}

			private bool ProcessSender(int senderUniqueName)
			{
				SenderInformation senderInformation = this.m_senderInfo[senderUniqueName];
				if (senderInformation == null)
				{
					return false;
				}
				this.UpdateOverrideToggleStateInfo(senderUniqueName);
				for (int i = 0; i < senderInformation.ReceiverUniqueNames.Count; i++)
				{
					this.ProcessReceiver(senderInformation.ReceiverUniqueNames[i]);
				}
				return true;
			}

			private void ProcessReceiver(int receiverUniqueName)
			{
				ReceiverInformation receiverInformation = this.m_receiverInfo[receiverUniqueName];
				Global.Tracer.Assert(null != receiverInformation, "(null != receiver)");
				this.UpdateOverrideHiddenInfo(receiverUniqueName);
			}

			private void UpdateOverrideToggleStateInfo(int uniqueName)
			{
				this.m_showHideInfoChanged = true;
				if (!this.m_overrideToggleStateInfo.ContainsKey(uniqueName))
				{
					this.m_overrideToggleStateInfo.Add(uniqueName, null);
				}
				else
				{
					this.m_overrideToggleStateInfo.Remove(uniqueName);
				}
			}

			private void UpdateOverrideHiddenInfo(int uniqueName)
			{
				this.m_showHideInfoChanged = true;
				if (!this.m_overrideHiddenInfo.ContainsKey(uniqueName))
				{
					this.m_overrideHiddenInfo.Add(uniqueName, null);
				}
				else
				{
					this.m_overrideHiddenInfo.Remove(uniqueName);
				}
			}
		}

		public delegate void OnDemandSubReportCallback(ICatalogItemContext reportContext, string subreportPath, string newChunkName, NeedsUpgrade upgradeCheck, ParameterInfoCollection parentQueryParameters, out ICatalogItemContext subreportContext, out string description, out IChunkFactory getCompiledDefinitionCallback, out ParameterInfoCollection parameters);

		public delegate void OnDemandSubReportDataSourcesCallback(ICatalogItemContext reportContext, string subreportPath, NeedsUpgrade upgradeCheck, out ICatalogItemContext subreportContext, out IChunkFactory getCompiledDefinitionCallback, out DataSourceInfoCollection dataSources, out DataSetInfoCollection dataSetReferences);

		public delegate bool NeedsUpgrade(ReportProcessingFlags processingFlags);

		internal delegate void SubReportCallback(ICatalogItemContext reportContext, string subreportPath, out ICatalogItemContext subreportContext, out string description, out GetReportChunk getCompiledDefinitionCallback, out ParameterInfoCollection parameters);

		internal delegate void SubReportDataSourcesCallback(ICatalogItemContext reportContext, string subreportPath, out ICatalogItemContext subreportContext, out GetReportChunk getCompiledDefinitionCallback, out DataSourceInfoCollection dataSources);

		internal interface IErasable
		{
			bool Erase();
		}

		public delegate AspNetCore.ReportingServices.DataExtensions.DataSourceInfo CheckSharedDataSource(string dataSourcePath, out Guid catalogItemId);

		internal delegate Stream CreateReportChunk(string name, ReportChunkTypes type, string mimeType);

		internal delegate Stream GetReportChunk(string name, ReportChunkTypes type, out string mimeType);

		internal delegate string GetChunkMimeType(string name, ReportChunkTypes type);

		public delegate NameValueCollection StoreServerParameters(ICatalogItemContext item, NameValueCollection reportParameters, bool[] sharedParameters, out bool replaced);

		public delegate IDbConnection CreateDataExtensionInstance(string extensionName, Guid modelID);

		public delegate IDbConnection CreateAndSetupDataExtensionInstance(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSource dataSource, AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet, OnDemandProcessingContext odpContext);

		public delegate void ResolveTemporaryDataSource(AspNetCore.ReportingServices.DataExtensions.DataSourceInfo dataSourceInfo, DataSourceInfoCollection originalDataSources);

		[Serializable]
		internal sealed class DataCacheUnavailableException : Exception
		{
			public DataCacheUnavailableException()
			{
			}

			public DataCacheUnavailableException(DataCacheUnavailableException ex)
				: base(ex.Message, ex)
			{
			}

			private DataCacheUnavailableException(SerializationInfo info, StreamingContext context)
				: base(info, context)
			{
			}
		}

		internal enum ReportChunkTypes
		{
			Main,
			Image,
			Other,
			StaticImage,
			ServerRdlMapping,
			Data,
			Interactivity,
			Pagination,
			Rendering,
			CompiledDefinition,
			GeneratedReportItems,
			LookupInfo,
			Shapefile
		}

		internal enum ExecutionType
		{
			Live,
			ServiceAccount,
			SurrogateAccount
		}

		internal class ProcessingComparer : IDataComparer, IEqualityComparer, IEqualityComparer<object>, IComparer, IComparer<object>, IStaticReferenceable
		{
			private readonly CompareInfo m_compareInfo;

			private readonly CompareOptions m_compareOptions;

			private readonly bool m_nullsAsBlanks;

			private readonly bool m_defaultThrowExceptionOnComparisonFailure;

			private CultureInfo m_cultureInfo;

			private int m_staticRefId = 2147483647;

			int IStaticReferenceable.ID
			{
				get
				{
					return this.m_staticRefId;
				}
			}

			internal ProcessingComparer(CompareInfo compareInfo, CompareOptions compareOptions, bool nullsAsBlanks)
				: this(compareInfo, compareOptions, nullsAsBlanks, true)
			{
			}

			internal ProcessingComparer(CompareInfo compareInfo, CompareOptions compareOptions, bool nullsAsBlanks, bool defaultThrowExceptionOnComparisonFailure)
			{
				this.m_compareInfo = compareInfo;
				this.m_compareOptions = compareOptions;
				this.m_nullsAsBlanks = nullsAsBlanks;
				this.m_defaultThrowExceptionOnComparisonFailure = defaultThrowExceptionOnComparisonFailure;
			}

			void IStaticReferenceable.SetID(int id)
			{
				this.m_staticRefId = id;
			}

			AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IStaticReferenceable.GetObjectType()
			{
				return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ProcessingComparer;
			}

			bool IEqualityComparer.Equals(object x, object y)
			{
				return this.InternalEquals(x, y);
			}

			bool IEqualityComparer<object>.Equals(object x, object y)
			{
				return this.InternalEquals(x, y);
			}

			private bool InternalEquals(object x, object y)
			{
				bool flag = default(bool);
				return 0 == this.Compare(x, y, this.m_defaultThrowExceptionOnComparisonFailure, false, out flag);
			}

			int IComparer.Compare(object x, object y)
			{
				return this.Compare(x, y);
			}

			public int Compare(object x, object y)
			{
				bool flag = default(bool);
				return this.Compare(x, y, this.m_defaultThrowExceptionOnComparisonFailure, false, out flag);
			}

			public int Compare(object x, object y, bool extendedTypeComparisons)
			{
				bool flag = default(bool);
				return this.Compare(x, y, this.m_defaultThrowExceptionOnComparisonFailure, extendedTypeComparisons, out flag);
			}

			public int Compare(object x, object y, bool throwExceptionOnComparisonFailure, bool extendedTypeComparisons, out bool validComparisonResult)
			{
				return ReportProcessing.CompareTo(x, y, this.m_nullsAsBlanks, this.m_compareInfo, this.m_compareOptions, throwExceptionOnComparisonFailure, extendedTypeComparisons, out validComparisonResult);
			}

			public int GetHashCode(object obj)
			{
				bool flag = default(bool);
				DataAggregate.DataTypeCode typeCode = DataAggregate.GetTypeCode(obj, false, out flag);
				if (!flag)
				{
					return obj.GetHashCode();
				}
				switch (typeCode)
				{
				case DataAggregate.DataTypeCode.String:
				{
					string text = (string)obj;
					if (CompareOptions.None < (CompareOptions.IgnoreCase & this.m_compareOptions))
					{
						if (this.m_cultureInfo == null)
						{
							this.m_cultureInfo = new CultureInfo(this.m_compareInfo.Name, false);
						}
						text = text.ToUpper(this.m_cultureInfo);
					}
					return ProcessingComparer.InternalGetHashCode(text);
				}
				case DataAggregate.DataTypeCode.Null:
					return 0;
				case DataAggregate.DataTypeCode.Int32:
					return (int)obj;
				case DataAggregate.DataTypeCode.Int16:
					return (short)obj;
				case DataAggregate.DataTypeCode.Byte:
					return (byte)obj;
				case DataAggregate.DataTypeCode.UInt16:
					return (ushort)obj;
				case DataAggregate.DataTypeCode.SByte:
					return (sbyte)obj;
				case DataAggregate.DataTypeCode.UInt32:
					return (int)(uint)obj;
				case DataAggregate.DataTypeCode.Int64:
				{
					long num6 = (long)obj;
					if (num6 < 2147483647)
					{
						return (int)num6;
					}
					return ProcessingComparer.InternalGetHashCode(num6);
				}
				case DataAggregate.DataTypeCode.UInt64:
				{
					ulong num5 = (ulong)obj;
					if (num5 < 2147483647)
					{
						return (int)num5;
					}
					return (int)num5 ^ (int)(num5 >> 32);
				}
				case DataAggregate.DataTypeCode.Double:
				{
					double num = (double)obj;
					int num2 = (int)num;
					if (num == (double)num2)
					{
						return num2;
					}
					return ProcessingComparer.InternalGetHashCode(num);
				}
				case DataAggregate.DataTypeCode.Single:
				{
					float num7 = (float)obj;
					int num8 = (int)num7;
					if (num7 == (float)num8)
					{
						return num8;
					}
					return ProcessingComparer.InternalGetHashCode((double)num7);
				}
				case DataAggregate.DataTypeCode.Decimal:
				{
					decimal num3 = (decimal)obj;
					decimal num4 = decimal.Truncate(num3);
					if (num3 == num4)
					{
						if (num4 >= -2147483648m && num4 <= 2147483647m)
						{
							return (int)num4;
						}
						if (num4 >= new decimal(-9223372036854775808L) && num4 <= new decimal(9223372036854775807L))
						{
							return ProcessingComparer.InternalGetHashCode((long)num4);
						}
					}
					return ProcessingComparer.InternalGetHashCode((double)num3);
				}
				case DataAggregate.DataTypeCode.DateTime:
					return ProcessingComparer.InternalGetHashCode((DateTime)obj);
				case DataAggregate.DataTypeCode.DateTimeOffset:
					return ProcessingComparer.InternalGetHashCode(((DateTimeOffset)obj).UtcDateTime);
				case DataAggregate.DataTypeCode.TimeSpan:
				{
					long ticks = ((TimeSpan)obj).Ticks;
					return (int)ticks ^ (int)(ticks >> 32);
				}
				case DataAggregate.DataTypeCode.Char:
				{
					char c = (char)obj;
					return (int)(c | (uint)c << 16);
				}
				case DataAggregate.DataTypeCode.Boolean:
					if ((bool)obj)
					{
						return 1;
					}
					return 0;
				default:
					return obj.GetHashCode();
				}
			}

			private static int InternalGetHashCode(string input)
			{
				int num = 5381;
				int num2 = num;
				int num3 = input.Length - 1;
				int length = input.Length;
				int num4;
				for (num4 = 0; num4 < length; num4++)
				{
					num = ((num << 5) + num ^ input[num4]);
					if (num4 == num3)
					{
						break;
					}
					num4++;
					num2 = ((num2 << 5) + num2 ^ input[num4]);
				}
				return num + num2 * 1566083941;
			}

			private static int InternalGetHashCode(double value)
			{
				if (value == 0.0)
				{
					return 0;
				}
				long num = BitConverter.DoubleToInt64Bits(value);
				return (int)num ^ (int)(num >> 32);
			}

			private static int InternalGetHashCode(DateTime value)
			{
				long ticks = value.Ticks;
				return (int)ticks ^ (int)(ticks >> 32);
			}

			private static int InternalGetHashCode(long value)
			{
				return (int)value ^ (int)(value >> 32);
			}
		}

		internal enum ProcessingStages
		{
			Grouping = 1,
			SortAndFilter,
			RunningValues,
			CreatingInstances,
			UserSortFilter
		}

		[Flags]
		internal enum DataActions
		{
			None = 0,
			RecursiveAggregates = 1,
			PostSortAggregates = 2,
			UserSort = 4
		}

		internal sealed class DataSourceInfoHashtable : Hashtable
		{
			internal DataSourceInfo this[string dataSourceName]
			{
				get
				{
					return (DataSourceInfo)base[dataSourceName];
				}
			}

			internal void Add(IProcessingDataSource procDataSource, IDbConnection connection, TransactionInfo transInfo, AspNetCore.ReportingServices.DataExtensions.DataSourceInfo dataExtDataSourceInfo)
			{
				DataSourceInfo value = new DataSourceInfo(procDataSource, connection, transInfo, dataExtDataSourceInfo);
				this.Add(procDataSource.Name, value);
			}
		}

		internal sealed class TransactionInfo
		{
			internal bool RollbackRequired;

			private IDbTransaction m_transaction;

			internal IDbTransaction Transaction
			{
				get
				{
					return this.m_transaction;
				}
			}

			internal TransactionInfo(IDbTransaction transaction)
			{
				Global.Tracer.Assert(transaction != null, "A transaction information object cannot have a null transaction.");
				this.m_transaction = transaction;
			}
		}

		internal struct TableColumnInfo
		{
			internal int StartIndex;

			internal int Span;
		}

		internal sealed class DataSourceInfo
		{
			private IDbConnection m_connection;

			private TransactionInfo m_transactionInfo;

			private AspNetCore.ReportingServices.DataExtensions.DataSourceInfo m_dataExtDataSourceInfo;

			private IProcessingDataSource m_procDataSourceInfo;

			internal string DataSourceName
			{
				get
				{
					return this.m_procDataSourceInfo.Name;
				}
			}

			internal AspNetCore.ReportingServices.DataExtensions.DataSourceInfo DataExtDataSourceInfo
			{
				get
				{
					return this.m_dataExtDataSourceInfo;
				}
			}

			internal IProcessingDataSource ProcDataSourceInfo
			{
				get
				{
					return this.m_procDataSourceInfo;
				}
			}

			internal IDbConnection Connection
			{
				get
				{
					return this.m_connection;
				}
			}

			internal TransactionInfo TransactionInfo
			{
				get
				{
					return this.m_transactionInfo;
				}
			}

			internal DataSourceInfo(IProcessingDataSource procDataSourceInfo, IDbConnection connection, TransactionInfo ti, AspNetCore.ReportingServices.DataExtensions.DataSourceInfo dataExtDataSourceInfo)
			{
				this.m_procDataSourceInfo = procDataSourceInfo;
				this.m_connection = connection;
				this.m_transactionInfo = ti;
				this.m_dataExtDataSourceInfo = dataExtDataSourceInfo;
			}
		}

		internal sealed class ProcessingAbortEventArgs : EventArgs
		{
			private int m_reportUniqueName;

			internal int ReportUniqueName
			{
				get
				{
					return this.m_reportUniqueName;
				}
			}

			internal ProcessingAbortEventArgs(int reportUniqueName)
			{
				this.m_reportUniqueName = reportUniqueName;
			}
		}

		internal sealed class ThreadSet
		{
			private int m_threadsRemaining;

			private ManualResetEvent m_allThreadsDone;

			internal ThreadSet(int threadCount)
			{
				if (Global.Tracer.TraceVerbose)
				{
					Global.Tracer.Trace(TraceLevel.Verbose, "ThreadSet object created. {0} threads remaining.", threadCount);
				}
				this.m_threadsRemaining = threadCount;
				this.m_allThreadsDone = new ManualResetEvent(threadCount <= 0);
			}

			internal void ThreadCompleted()
			{
				int num = Interlocked.Decrement(ref this.m_threadsRemaining);
				if (Global.Tracer.TraceVerbose)
				{
					Global.Tracer.Trace(TraceLevel.Verbose, "Thread completed. {0} thread remaining.", num);
				}
				if (num <= 0)
				{
					this.m_allThreadsDone.Set();
				}
			}

			internal void WaitForCompletion()
			{
				this.m_allThreadsDone.WaitOne();
				if (Global.Tracer.TraceVerbose)
				{
					Global.Tracer.Trace(TraceLevel.Verbose, "All the processing threads have completed.");
				}
			}
		}

		internal class ProcessingContext : IInternalProcessingContext
		{
			[Flags]
			internal enum SecondPassOperations
			{
				Sorting = 1,
				Filtering = 2
			}

			internal sealed class AbortHelper : IAbortHelper, IDisposable
			{
				private ProcessingStatus m_overallStatus;

				private Exception m_exception;

				private Hashtable m_reportStatus;

				private IJobContext m_jobContext;

				internal event EventHandler ProcessingAbortEvent;

				internal AbortHelper(IJobContext jobContext)
				{
					this.m_reportStatus = new Hashtable();
					if (jobContext != null)
					{
						this.m_jobContext = jobContext;
						jobContext.AddAbortHelper(this);
					}
				}

				internal void ThrowAbortException(int reportUniqueName)
				{
					if (this.GetStatus(reportUniqueName) == ProcessingStatus.AbnormalTermination)
					{
						throw new ProcessingAbortedException(this.m_exception);
					}
					throw new ProcessingAbortedException();
				}

				public bool Abort(ProcessingStatus status)
				{
					return this.Abort(-1, status);
				}

				internal bool Abort(int reportUniqueName, ProcessingStatus status)
				{
					if (!Monitor.TryEnter(this))
					{
						if (Global.Tracer.TraceInfo)
						{
							Global.Tracer.Trace(TraceLevel.Info, "Some other thread is aborting processing.");
						}
						return false;
					}
					if (this.GetStatus(reportUniqueName) != 0)
					{
						if (Global.Tracer.TraceInfo)
						{
							Global.Tracer.Trace(TraceLevel.Info, "Some other thread has already aborted processing.");
						}
						Monitor.Exit(this);
						return false;
					}
					bool result = false;
					try
					{
						this.SetStatus(reportUniqueName, status);
						if (this.ProcessingAbortEvent != null)
						{
							try
							{
								this.ProcessingAbortEvent(this, new ProcessingAbortEventArgs(reportUniqueName));
								result = true;
								if (Global.Tracer.TraceVerbose)
								{
									Global.Tracer.Trace(TraceLevel.Verbose, "Abort callback successful.");
									return result;
								}
								return result;
							}
							catch (Exception ex)
							{
								if (Global.Tracer.TraceError)
								{
									Global.Tracer.Trace(TraceLevel.Error, "Exception in abort callback. Details: {0}", ex.ToString());
									return result;
								}
								return result;
							}
						}
						if (Global.Tracer.TraceVerbose)
						{
							Global.Tracer.Trace(TraceLevel.Verbose, "No abort callback.");
							return result;
						}
						return result;
					}
					finally
					{
						Monitor.Exit(this);
					}
				}

				internal void AddSubreportInstance(int subreportInstanceUniqueName)
				{
					Hashtable hashtable = Hashtable.Synchronized(this.m_reportStatus);
					Global.Tracer.Assert(!hashtable.ContainsKey(subreportInstanceUniqueName), "(false == reportStatus.ContainsKey(subreportInstanceUniqueName))");
					hashtable.Add(subreportInstanceUniqueName, ProcessingStatus.Success);
				}

				internal ProcessingStatus GetStatus(int uniqueName)
				{
					if (-1 == uniqueName)
					{
						return this.m_overallStatus;
					}
					Global.Tracer.Assert(this.m_reportStatus.ContainsKey(uniqueName), "(m_reportStatus.ContainsKey(uniqueName))");
					return (ProcessingStatus)this.m_reportStatus[uniqueName];
				}

				private void SetStatus(int uniqueName, ProcessingStatus newStatus)
				{
					if (-1 == uniqueName)
					{
						this.m_overallStatus = newStatus;
					}
					else
					{
						Hashtable hashtable = Hashtable.Synchronized(this.m_reportStatus);
						Global.Tracer.Assert(hashtable.ContainsKey(uniqueName), "(reportStatus.ContainsKey(uniqueName))");
						hashtable[uniqueName] = newStatus;
					}
				}

				internal bool SetError(int reportUniqueName, Exception e)
				{
					if (Global.Tracer.TraceVerbose)
					{
						Global.Tracer.Trace(TraceLevel.Verbose, "An exception has occurred. Trying to abort processing. Details: {0}", e.ToString());
					}
					this.m_exception = e;
					if (!this.Abort(reportUniqueName, ProcessingStatus.AbnormalTermination))
					{
						return false;
					}
					return true;
				}

				public void Dispose()
				{
					if (this.m_jobContext != null)
					{
						this.m_jobContext.RemoveAbortHelper();
					}
				}
			}

			private sealed class CommonInfo
			{
				private bool m_isOnePass;

				private int m_uniqueNameCounter;

				private long m_dataProcessingDurationMs;

				private string m_owcChartName;

				private OWCChartInstanceInfo m_owcChartInstance;

				private string m_requestUserName;

				private CultureInfo m_userLanguage;

				private SubReportCallback m_subReportCallback;

				private ChunkManager.ProcessingChunkManager m_chunkManager;

				private CreateReportChunk m_createReportChunkCallback;

				private IChunkFactory m_createReportChunkFactory;

				private QuickFindHashtable m_quickFind;

				private IGetResource m_getResourceCallback;

				private int m_idCounterForSubreports;

				private ExecutionType m_interactiveExecution;

				private bool m_hasImageStreams;

				private DateTime m_executionTime;

				private UserProfileState m_allowUserProfileState;

				private bool m_hasUserSortFilter;

				private bool m_saveSnapshotData = true;

				private bool m_stopSaveSnapshotDataOnError;

				private bool m_errorSavingSnapshotData;

				private bool m_isHistorySnapshot;

				private bool m_snapshotProcessing;

				private bool m_userSortFilterProcessing;

				private bool m_resetForSubreportDataPrefetch;

				private bool m_processWithCachedData;

				private GetReportChunk m_getReportChunkCallback;

				private CreateReportChunk m_cacheDataCallback;

				private bool m_dataCached;

				private Hashtable m_cachedDataChunkMapping;

				private ReportDrillthroughInfo m_drillthroughInfo;

				private CustomReportItemControls m_criControls;

				private EventInformation m_userSortFilterInfo;

				private SortFilterEventInfoHashtable m_oldSortFilterEventInfo;

				private SortFilterEventInfoHashtable m_newSortFilterEventInfo;

				private RuntimeSortFilterEventInfoList m_reportRuntimeUserSortFilterInfo;

				private ReportRuntimeSetup m_reportRuntimeSetup;

				internal IGetResource GetResourceCallback
				{
					get
					{
						return this.m_getResourceCallback;
					}
				}

				internal CreateReportChunk CreateReportChunkCallback
				{
					get
					{
						return this.m_createReportChunkCallback;
					}
				}

				internal IChunkFactory CreateReportChunkFactory
				{
					get
					{
						return this.m_createReportChunkFactory;
					}
					set
					{
						this.m_createReportChunkFactory = value;
					}
				}

				internal bool IsOnePass
				{
					get
					{
						return this.m_isOnePass;
					}
				}

				internal long DataProcessingDurationMs
				{
					get
					{
						return this.m_dataProcessingDurationMs;
					}
					set
					{
						this.m_dataProcessingDurationMs = value;
					}
				}

				internal string OWCChartName
				{
					get
					{
						return this.m_owcChartName;
					}
				}

				internal OWCChartInstanceInfo OWCChartInstance
				{
					get
					{
						return this.m_owcChartInstance;
					}
					set
					{
						this.m_owcChartInstance = value;
					}
				}

				internal string RequestUserName
				{
					get
					{
						return this.m_requestUserName;
					}
				}

				internal DateTime ExecutionTime
				{
					get
					{
						return this.m_executionTime;
					}
				}

				internal CultureInfo UserLanguage
				{
					get
					{
						return this.m_userLanguage;
					}
				}

				internal SubReportCallback SubReportCallback
				{
					get
					{
						return this.m_subReportCallback;
					}
				}

				internal ChunkManager.ProcessingChunkManager ChunkManager
				{
					get
					{
						return this.m_chunkManager;
					}
				}

				internal QuickFindHashtable QuickFind
				{
					get
					{
						return this.m_quickFind;
					}
				}

				internal ReportDrillthroughInfo DrillthroughInfo
				{
					get
					{
						return this.m_drillthroughInfo;
					}
					set
					{
						this.m_drillthroughInfo = value;
					}
				}

				internal int UniqueNameCounter
				{
					get
					{
						return this.m_uniqueNameCounter;
					}
				}

				internal ExecutionType InteractiveExecution
				{
					get
					{
						return this.m_interactiveExecution;
					}
				}

				internal bool HasImageStreams
				{
					get
					{
						return this.m_hasImageStreams;
					}
					set
					{
						this.m_hasImageStreams = value;
					}
				}

				internal UserProfileState AllowUserProfileState
				{
					get
					{
						return this.m_allowUserProfileState;
					}
				}

				internal bool IsHistorySnapshot
				{
					get
					{
						return this.m_isHistorySnapshot;
					}
				}

				internal bool SnapshotProcessing
				{
					get
					{
						return this.m_snapshotProcessing;
					}
					set
					{
						this.m_snapshotProcessing = value;
					}
				}

				internal bool UserSortFilterProcessing
				{
					get
					{
						return this.m_userSortFilterProcessing;
					}
					set
					{
						this.m_userSortFilterProcessing = value;
					}
				}

				internal bool ResetForSubreportDataPrefetch
				{
					get
					{
						return this.m_resetForSubreportDataPrefetch;
					}
					set
					{
						this.m_resetForSubreportDataPrefetch = value;
					}
				}

				internal bool ProcessWithCachedData
				{
					get
					{
						return this.m_processWithCachedData;
					}
				}

				internal GetReportChunk GetReportChunkCallback
				{
					get
					{
						return this.m_getReportChunkCallback;
					}
				}

				internal CreateReportChunk CacheDataCallback
				{
					get
					{
						return this.m_cacheDataCallback;
					}
				}

				internal bool HasUserSortFilter
				{
					get
					{
						return this.m_hasUserSortFilter;
					}
					set
					{
						this.m_hasUserSortFilter = value;
					}
				}

				internal bool SaveSnapshotData
				{
					get
					{
						return this.m_saveSnapshotData;
					}
					set
					{
						this.m_saveSnapshotData = value;
					}
				}

				internal bool StopSaveSnapshotDataOnError
				{
					get
					{
						return this.m_stopSaveSnapshotDataOnError;
					}
					set
					{
						this.m_stopSaveSnapshotDataOnError = value;
					}
				}

				internal bool ErrorSavingSnapshotData
				{
					get
					{
						return this.m_errorSavingSnapshotData;
					}
					set
					{
						lock (this)
						{
							this.m_errorSavingSnapshotData = value;
						}
					}
				}

				internal bool DataCached
				{
					get
					{
						return this.m_dataCached;
					}
					set
					{
						lock (this)
						{
							this.m_dataCached = value;
						}
					}
				}

				internal Hashtable CachedDataChunkMapping
				{
					get
					{
						return this.m_cachedDataChunkMapping;
					}
					set
					{
						lock (this)
						{
							this.m_cachedDataChunkMapping = value;
						}
					}
				}

				internal CustomReportItemControls CriProcessingControls
				{
					get
					{
						return this.m_criControls;
					}
					set
					{
						lock (this)
						{
							this.m_criControls = value;
						}
					}
				}

				internal EventInformation UserSortFilterInfo
				{
					get
					{
						return this.m_userSortFilterInfo;
					}
					set
					{
						this.m_userSortFilterInfo = value;
					}
				}

				internal SortFilterEventInfoHashtable OldSortFilterEventInfo
				{
					get
					{
						return this.m_oldSortFilterEventInfo;
					}
					set
					{
						this.m_oldSortFilterEventInfo = value;
					}
				}

				internal SortFilterEventInfoHashtable NewSortFilterEventInfo
				{
					get
					{
						return this.m_newSortFilterEventInfo;
					}
					set
					{
						this.m_newSortFilterEventInfo = value;
					}
				}

				internal RuntimeSortFilterEventInfoList ReportRuntimeUserSortFilterInfo
				{
					get
					{
						return this.m_reportRuntimeUserSortFilterInfo;
					}
					set
					{
						this.m_reportRuntimeUserSortFilterInfo = value;
					}
				}

				internal ReportRuntimeSetup ReportRuntimeSetup
				{
					get
					{
						return this.m_reportRuntimeSetup;
					}
				}

				internal CommonInfo(string owcChartName, string requestUserName, CultureInfo userLanguage, SubReportCallback subReportCallback, Report report, CreateReportChunk createReportChunkCallback, IGetResource getResourceCallback, ExecutionType interactiveExecution, DateTime executionTime, UserProfileState allowUserProfileState, bool isHistorySnapshot, bool snapshotProcessing, bool processWithCachedData, GetReportChunk getChunkCallback, CreateReportChunk cacheDataCallback, ReportRuntimeSetup reportRuntimeSetup)
				{
					this.m_uniqueNameCounter = 0;
					this.m_dataProcessingDurationMs = 0L;
					this.m_owcChartName = owcChartName;
					this.m_owcChartInstance = null;
					this.m_requestUserName = requestUserName;
					this.m_userLanguage = userLanguage;
					this.m_subReportCallback = subReportCallback;
					this.m_isOnePass = (report != null && report.MergeOnePass);
					this.m_createReportChunkCallback = createReportChunkCallback;
					this.m_chunkManager = new ChunkManager.ProcessingChunkManager(createReportChunkCallback, this.m_isOnePass);
					this.m_quickFind = new QuickFindHashtable();
					this.m_drillthroughInfo = new ReportDrillthroughInfo();
					this.m_getResourceCallback = getResourceCallback;
					this.m_idCounterForSubreports = ((report != null) ? report.LastID : 0);
					this.m_interactiveExecution = interactiveExecution;
					this.m_hasImageStreams = false;
					this.m_executionTime = executionTime;
					this.m_allowUserProfileState = allowUserProfileState;
					this.m_isHistorySnapshot = isHistorySnapshot;
					this.m_snapshotProcessing = snapshotProcessing;
					this.m_processWithCachedData = processWithCachedData;
					this.m_getReportChunkCallback = getChunkCallback;
					this.m_cacheDataCallback = cacheDataCallback;
					if (cacheDataCallback != null)
					{
						this.m_dataCached = true;
					}
					this.m_cachedDataChunkMapping = new Hashtable();
					this.m_criControls = new CustomReportItemControls();
					this.m_reportRuntimeSetup = reportRuntimeSetup;
				}

				internal CommonInfo(IGetResource getResourceCallback, UserProfileState allowUserProfileState, ReportRuntimeSetup reportRuntimeSetup)
				{
					this.m_uniqueNameCounter = 0;
					this.m_dataProcessingDurationMs = 0L;
					this.m_owcChartName = null;
					this.m_owcChartInstance = null;
					this.m_requestUserName = null;
					this.m_userLanguage = null;
					this.m_subReportCallback = null;
					this.m_chunkManager = null;
					this.m_createReportChunkCallback = null;
					this.m_quickFind = new QuickFindHashtable();
					this.m_drillthroughInfo = new ReportDrillthroughInfo();
					this.m_getResourceCallback = getResourceCallback;
					this.m_idCounterForSubreports = 0;
					this.m_interactiveExecution = ExecutionType.Live;
					this.m_hasImageStreams = false;
					this.m_executionTime = DateTime.MinValue;
					this.m_allowUserProfileState = allowUserProfileState;
					this.m_snapshotProcessing = false;
					this.m_processWithCachedData = false;
					this.m_getReportChunkCallback = null;
					this.m_cacheDataCallback = null;
					this.m_dataCached = false;
					this.m_cachedDataChunkMapping = null;
					this.m_criControls = new CustomReportItemControls();
					this.m_reportRuntimeSetup = reportRuntimeSetup;
				}

				internal CommonInfo(IGetResource getResourceCallback, UserProfileState allowUserProfileState, ReportRuntimeSetup reportRuntimeSetup, CreateReportChunk createChunkCallback, ChunkManager.ProcessingChunkManager processingChunkManager, int uniqueNameCounter, ref ReportDrillthroughInfo drillthroughInfo)
				{
					this.m_uniqueNameCounter = uniqueNameCounter;
					this.m_dataProcessingDurationMs = 0L;
					this.m_owcChartName = null;
					this.m_owcChartInstance = null;
					this.m_requestUserName = null;
					this.m_userLanguage = null;
					this.m_subReportCallback = null;
					this.m_chunkManager = processingChunkManager;
					this.m_createReportChunkCallback = createChunkCallback;
					this.m_quickFind = new QuickFindHashtable();
					this.m_getResourceCallback = getResourceCallback;
					this.m_idCounterForSubreports = 0;
					this.m_interactiveExecution = ExecutionType.Live;
					this.m_hasImageStreams = false;
					this.m_executionTime = DateTime.MinValue;
					this.m_allowUserProfileState = allowUserProfileState;
					this.m_snapshotProcessing = false;
					this.m_processWithCachedData = false;
					this.m_getReportChunkCallback = null;
					this.m_cacheDataCallback = null;
					this.m_dataCached = false;
					this.m_cachedDataChunkMapping = null;
					this.m_criControls = new CustomReportItemControls();
					this.m_reportRuntimeSetup = reportRuntimeSetup;
					if (drillthroughInfo == null)
					{
						drillthroughInfo = new ReportDrillthroughInfo();
					}
					this.m_drillthroughInfo = drillthroughInfo;
				}

				internal int CreateUniqueName()
				{
					if (this.m_isOnePass)
					{
						return Interlocked.Increment(ref this.m_uniqueNameCounter);
					}
					return ++this.m_uniqueNameCounter;
				}

				internal int CreateIDForSubreport()
				{
					if (this.m_isOnePass)
					{
						return Interlocked.Increment(ref this.m_idCounterForSubreports);
					}
					return ++this.m_idCounterForSubreports;
				}

				internal int GetLastIDForReport()
				{
					return this.m_idCounterForSubreports;
				}

				internal EventInformation GetUserSortFilterInformation(ref int sourceUniqueName, ref int page)
				{
					if (this.m_reportRuntimeUserSortFilterInfo != null && this.m_reportRuntimeUserSortFilterInfo.Count != 0)
					{
						EventInformation.SortEventInfo sortEventInfo = new EventInformation.SortEventInfo();
						for (int i = 0; i < this.m_reportRuntimeUserSortFilterInfo.Count; i++)
						{
							RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = this.m_reportRuntimeUserSortFilterInfo[i];
							if (-1 == runtimeSortFilterEventInfo.NewUniqueName)
							{
								runtimeSortFilterEventInfo.NewUniqueName = runtimeSortFilterEventInfo.OldUniqueName;
							}
							Hashtable hashtable = null;
							if (runtimeSortFilterEventInfo.PeerSortFilters != null)
							{
								hashtable = new Hashtable(runtimeSortFilterEventInfo.PeerSortFilters.Count);
								IDictionaryEnumerator enumerator = runtimeSortFilterEventInfo.PeerSortFilters.GetEnumerator();
								while (enumerator.MoveNext())
								{
									if (enumerator.Value != null)
									{
										hashtable.Add(enumerator.Value, null);
									}
								}
							}
							sortEventInfo.Add(runtimeSortFilterEventInfo.NewUniqueName, runtimeSortFilterEventInfo.SortDirection, hashtable);
							if (runtimeSortFilterEventInfo.OldUniqueName == sourceUniqueName)
							{
								sourceUniqueName = runtimeSortFilterEventInfo.NewUniqueName;
								page = runtimeSortFilterEventInfo.Page + 1;
							}
						}
						EventInformation eventInformation = new EventInformation();
						eventInformation.SortInfo = sortEventInfo;
						return eventInformation;
					}
					return null;
				}
			}

			private sealed class ShowHideInfo
			{
				private sealed class SenderInfo
				{
					internal int UniqueName;

					internal bool StartHidden;

					internal int[] Containers;

					internal SenderInfo(int uniqueName, bool startHidden, int[] containers)
					{
						this.UniqueName = uniqueName;
						this.StartHidden = startHidden;
						this.Containers = containers;
					}
				}

				private sealed class ReceiverInfo
				{
					internal int UniqueName;

					internal bool StartHidden;

					internal ReceiverInfo(int uniqueName, bool startHidden)
					{
						this.UniqueName = uniqueName;
						this.StartHidden = startHidden;
					}
				}

				private sealed class ReceiverInfoList : ArrayList
				{
					internal new ReceiverInfo this[int index]
					{
						get
						{
							return (ReceiverInfo)base[index];
						}
					}
				}

				private sealed class IgnoreRange
				{
					private int m_startIgnoreRangeIndex;

					private int m_endIgnoreRangeIndex;

					private bool m_useAllContainers = true;

					private bool m_ignoreFromStart;

					internal bool IgnoreAllFromStart
					{
						set
						{
							this.m_ignoreFromStart = value;
						}
					}

					internal bool UseAllContainers
					{
						set
						{
							this.m_useAllContainers = value;
						}
					}

					internal int EndIgnoreRange
					{
						set
						{
							this.m_endIgnoreRangeIndex = value;
							this.m_useAllContainers = false;
							this.m_ignoreFromStart = false;
						}
					}

					internal IgnoreRange(int startIgnoreRange)
					{
						this.m_startIgnoreRangeIndex = startIgnoreRange;
					}

					internal int[] GetContainerUniqueNames(IntList containerUniqueNames)
					{
						if (containerUniqueNames.Count == 0)
						{
							return null;
						}
						int[] array = null;
						if (!this.m_useAllContainers)
						{
							int count = containerUniqueNames.Count;
							int num = this.m_endIgnoreRangeIndex;
							if (this.m_ignoreFromStart)
							{
								num = count - 1;
							}
							if (num >= this.m_startIgnoreRangeIndex)
							{
								count -= this.m_endIgnoreRangeIndex - this.m_startIgnoreRangeIndex + 1;
								if (count == 0)
								{
									return null;
								}
								array = new int[count];
								containerUniqueNames.CopyTo(0, array, 0, this.m_startIgnoreRangeIndex);
								containerUniqueNames.CopyTo(num + 1, array, this.m_startIgnoreRangeIndex, containerUniqueNames.Count - num - 1);
								return array;
							}
						}
						array = new int[containerUniqueNames.Count];
						containerUniqueNames.CopyTo(array);
						return array;
					}
				}

				private sealed class IgnoreRangeList : ArrayList
				{
					internal new IgnoreRange this[int index]
					{
						get
						{
							return (IgnoreRange)base[index];
						}
					}
				}

				private sealed class CommonInfo
				{
					private SenderInformationHashtable m_senderInformation;

					private ReceiverInformationHashtable m_receiverInformation;

					internal void UpdateSenderAndReceiverInfo(SenderInfo sender, ReceiverInfo receiver)
					{
						SenderInformation senderInformation = null;
						if (this.m_senderInformation == null)
						{
							this.m_senderInformation = new SenderInformationHashtable();
						}
						else
						{
							senderInformation = this.m_senderInformation[sender.UniqueName];
						}
						if (senderInformation == null)
						{
							senderInformation = new SenderInformation(sender.StartHidden, sender.Containers);
							this.m_senderInformation[sender.UniqueName] = senderInformation;
						}
						senderInformation.ReceiverUniqueNames.Add(receiver.UniqueName);
						if (this.m_receiverInformation == null)
						{
							this.m_receiverInformation = new ReceiverInformationHashtable();
						}
						this.m_receiverInformation[receiver.UniqueName] = new ReceiverInformation(receiver.StartHidden, sender.UniqueName);
					}

					internal void GetSenderAndReceiverInfo(out SenderInformationHashtable senderInformation, out ReceiverInformationHashtable receiverInformation)
					{
						senderInformation = this.m_senderInformation;
						receiverInformation = this.m_receiverInformation;
						this.m_senderInformation = null;
						this.m_receiverInformation = null;
					}
				}

				private CommonInfo m_commonInfo;

				private ArrayList m_recursiveSenders;

				private ArrayList m_localSenders;

				private Hashtable m_localReceivers;

				private IntList m_containerUniqueNames;

				private IgnoreRangeList m_ignoreRangeList;

				private IgnoreRange m_currentIgnoreRange;

				internal bool IgnoreAllFromStart
				{
					set
					{
						this.m_currentIgnoreRange.IgnoreAllFromStart = value;
					}
				}

				internal bool UseAllContainers
				{
					set
					{
						this.m_currentIgnoreRange.UseAllContainers = value;
					}
				}

				internal ShowHideInfo()
				{
					this.m_commonInfo = new CommonInfo();
					this.m_recursiveSenders = new ArrayList();
					this.m_localSenders = new ArrayList();
					this.m_localSenders.Add(null);
					this.m_localReceivers = null;
					this.m_containerUniqueNames = new IntList();
				}

				internal ShowHideInfo(ShowHideInfo copy)
				{
					this.m_commonInfo = copy.m_commonInfo;
					this.m_recursiveSenders = new ArrayList();
					this.m_localSenders = new ArrayList();
					this.m_localSenders.Add(null);
					this.m_localReceivers = null;
					this.m_containerUniqueNames = new IntList();
				}

				internal void EnterGrouping()
				{
					this.m_localSenders.Add(null);
				}

				internal void ExitGrouping()
				{
					this.m_localSenders.RemoveAt(this.m_localSenders.Count - 1);
				}

				internal void EnterChildGroupings()
				{
					this.m_recursiveSenders.Add(null);
				}

				internal void ExitChildGroupings()
				{
					this.m_recursiveSenders.RemoveAt(this.m_recursiveSenders.Count - 1);
				}

				internal void RegisterSender(string senderName, int senderUniqueName, bool startHidden, bool recursiveSender)
				{
					int[] containerUniqueNames = this.GetContainerUniqueNames();
					SenderInfo sender = new SenderInfo(senderUniqueName, startHidden, containerUniqueNames);
					ReceiverInfoList receiverInfoList = this.RemoveLocalReceivers(senderName);
					if (receiverInfoList != null)
					{
						for (int i = 0; i < receiverInfoList.Count; i++)
						{
							this.m_commonInfo.UpdateSenderAndReceiverInfo(sender, receiverInfoList[i]);
						}
					}
					this.AddLocalSender(senderName, sender);
					if (recursiveSender)
					{
						this.AddRecursiveSender(senderName, sender);
					}
				}

				internal void RegisterReceiver(string senderName, int receiverUniqueName, bool startHidden, bool recursiveReceiver)
				{
					ReceiverInfo receiver = new ReceiverInfo(receiverUniqueName, startHidden);
					if (!recursiveReceiver)
					{
						SenderInfo senderInfo = this.FindLocalSender(senderName);
						if (senderInfo != null)
						{
							this.m_commonInfo.UpdateSenderAndReceiverInfo(senderInfo, receiver);
						}
						else
						{
							this.AddLocalReceiver(senderName, receiver);
						}
					}
					else
					{
						SenderInfo senderInfo2 = this.FindRecursiveSender(senderName);
						if (senderInfo2 != null)
						{
							this.m_commonInfo.UpdateSenderAndReceiverInfo(senderInfo2, receiver);
						}
					}
				}

				internal void RegisterContainer(int containerUniqueName)
				{
					this.m_containerUniqueNames.Add(containerUniqueName);
				}

				internal void UnRegisterContainer(int containerUniqueName)
				{
					Global.Tracer.Assert(containerUniqueName == this.m_containerUniqueNames[this.m_containerUniqueNames.Count - 1]);
					this.m_containerUniqueNames.RemoveAt(this.m_containerUniqueNames.Count - 1);
				}

				internal void GetSenderAndReceiverInfo(out SenderInformationHashtable senderInfo, out ReceiverInformationHashtable receiverInfo)
				{
					this.m_commonInfo.GetSenderAndReceiverInfo(out senderInfo, out receiverInfo);
					this.m_commonInfo = null;
					this.m_recursiveSenders = null;
					this.m_localSenders = null;
					this.m_localReceivers = null;
					this.m_containerUniqueNames = null;
				}

				private void AddRecursiveSender(string senderName, SenderInfo sender)
				{
					Hashtable hashtable = (Hashtable)this.m_recursiveSenders[this.m_recursiveSenders.Count - 1];
					if (hashtable == null)
					{
						hashtable = new Hashtable();
						this.m_recursiveSenders[this.m_recursiveSenders.Count - 1] = hashtable;
					}
					hashtable[senderName] = sender;
				}

				private SenderInfo FindRecursiveSender(string senderName)
				{
					if (this.m_recursiveSenders.Count - 2 >= 0)
					{
						Hashtable hashtable = (Hashtable)this.m_recursiveSenders[this.m_recursiveSenders.Count - 2];
						if (hashtable != null && hashtable.ContainsKey(senderName))
						{
							return (SenderInfo)hashtable[senderName];
						}
					}
					return null;
				}

				private void AddLocalSender(string senderName, SenderInfo sender)
				{
					Hashtable hashtable = (Hashtable)this.m_localSenders[this.m_localSenders.Count - 1];
					if (hashtable == null)
					{
						hashtable = new Hashtable();
						this.m_localSenders[this.m_localSenders.Count - 1] = hashtable;
					}
					hashtable[senderName] = sender;
				}

				private SenderInfo FindLocalSender(string senderName)
				{
					for (int num = this.m_localSenders.Count - 1; num >= 0; num--)
					{
						Hashtable hashtable = (Hashtable)this.m_localSenders[num];
						if (hashtable != null && hashtable.ContainsKey(senderName))
						{
							return (SenderInfo)hashtable[senderName];
						}
					}
					return null;
				}

				private void AddLocalReceiver(string senderName, ReceiverInfo receiver)
				{
					if (this.m_localReceivers == null)
					{
						this.m_localReceivers = new Hashtable();
					}
					ReceiverInfoList receiverInfoList = (ReceiverInfoList)this.m_localReceivers[senderName];
					if (receiverInfoList == null)
					{
						receiverInfoList = new ReceiverInfoList();
						this.m_localReceivers[senderName] = receiverInfoList;
					}
					receiverInfoList.Add(receiver);
				}

				private ReceiverInfoList RemoveLocalReceivers(string senderName)
				{
					if (this.m_localReceivers != null)
					{
						ReceiverInfoList receiverInfoList = (ReceiverInfoList)this.m_localReceivers[senderName];
						if (receiverInfoList != null)
						{
							this.m_localReceivers.Remove(senderName);
							return receiverInfoList;
						}
					}
					return null;
				}

				private int[] GetContainerUniqueNames()
				{
					if (this.m_containerUniqueNames.Count == 0)
					{
						return null;
					}
					int[] array = null;
					if (this.m_currentIgnoreRange != null)
					{
						array = this.m_currentIgnoreRange.GetContainerUniqueNames(this.m_containerUniqueNames);
					}
					else
					{
						array = new int[this.m_containerUniqueNames.Count];
						this.m_containerUniqueNames.CopyTo(array);
					}
					return array;
				}

				internal void EndIgnoreRange()
				{
					this.m_currentIgnoreRange.EndIgnoreRange = this.m_containerUniqueNames.Count - 1;
				}

				internal void RegisterIgnoreRange()
				{
					if (this.m_ignoreRangeList == null)
					{
						this.m_ignoreRangeList = new IgnoreRangeList();
					}
					this.m_currentIgnoreRange = new IgnoreRange(this.m_containerUniqueNames.Count);
					this.m_ignoreRangeList.Add(this.m_currentIgnoreRange);
				}

				internal void UnRegisterIgnoreRange()
				{
					this.m_ignoreRangeList.RemoveAt(this.m_ignoreRangeList.Count - 1);
					if (this.m_ignoreRangeList.Count > 0)
					{
						this.m_currentIgnoreRange = this.m_ignoreRangeList[this.m_ignoreRangeList.Count - 1];
					}
					else
					{
						this.m_currentIgnoreRange = null;
					}
				}
			}

			private AbortHelper m_abortHelper;

			private CommonInfo m_commonInfo;

			private uint m_subReportLevel;

			private ICatalogItemContext m_reportContext;

			private ObjectModelImpl m_reportObjectModel;

			private bool m_reportItemsReferenced;

			private bool m_reportItemThisDotValueReferenced;

			private ShowHideInfo m_showHideInfo;

			private DataSourceInfoHashtable m_dataSourceInfo;

			private Report.ShowHideTypes m_showHideType;

			private EmbeddedImageHashtable m_embeddedImages;

			private ImageStreamNames m_imageStreamNames;

			private bool m_inPageSection;

			private FiltersList m_specialDataRegionFilters;

			private ErrorContext m_errorContext;

			private bool m_processReportParameters;

			private List<bool> m_pivotRunningValueScopes;

			private ReportRuntime m_reportRuntime;

			private MatrixHeadingInstance m_headingInstance;

			private MatrixHeadingInstance m_headingInstanceOld;

			private bool m_delayAddingInstanceInfo;

			private bool m_specialRecursiveAggregates;

			private SecondPassOperations m_secondPassOperation;

			private CultureInfo m_threadCulture;

			private uint m_languageInstanceId;

			private AggregatesImpl m_globalRVCollection;

			private string m_transparentImageGuid;

			private Pagination m_pagination;

			private NavigationInfo m_navigationInfo;

			private CompareInfo m_compareInfo = Thread.CurrentThread.CurrentCulture.CompareInfo;

			private CompareOptions m_clrCompareOptions;

			private int m_dataSetUniqueName = -1;

			private bool m_createPageSectionImageChunks;

			private PageSectionContext m_pageSectionContext;

			private UserSortFilterContext m_userSortFilterContext;

			private IJobContext m_jobContext;

			private IExtensionFactory m_extFactory;

			private IProcessingDataExtensionConnection m_dataExtensionConnection;

			private IDataProtection m_dataProtection;

			public bool EnableDataBackedParameters
			{
				get
				{
					return true;
				}
			}

			internal CreateReportChunk CreateReportChunkCallback
			{
				get
				{
					return this.m_commonInfo.CreateReportChunkCallback;
				}
			}

			internal IChunkFactory CreateReportChunkFactory
			{
				get
				{
					return this.m_commonInfo.CreateReportChunkFactory;
				}
				set
				{
					this.m_commonInfo.CreateReportChunkFactory = value;
				}
			}

			internal bool IsOnePass
			{
				get
				{
					return this.m_commonInfo.IsOnePass;
				}
			}

			internal long DataProcessingDurationMs
			{
				get
				{
					return this.m_commonInfo.DataProcessingDurationMs;
				}
				set
				{
					this.m_commonInfo.DataProcessingDurationMs = value;
				}
			}

			internal string OWCChartName
			{
				get
				{
					return this.m_commonInfo.OWCChartName;
				}
			}

			internal OWCChartInstanceInfo OWCChartInstance
			{
				get
				{
					return this.m_commonInfo.OWCChartInstance;
				}
				set
				{
					this.m_commonInfo.OWCChartInstance = value;
				}
			}

			internal string RequestUserName
			{
				get
				{
					return this.m_commonInfo.RequestUserName;
				}
			}

			public DateTime ExecutionTime
			{
				get
				{
					return this.m_commonInfo.ExecutionTime;
				}
			}

			internal CultureInfo UserLanguage
			{
				get
				{
					return this.m_commonInfo.UserLanguage;
				}
			}

			internal SubReportCallback SubReportCallback
			{
				get
				{
					return this.m_commonInfo.SubReportCallback;
				}
			}

			internal UserProfileState HasUserProfileState
			{
				get
				{
					if (this.m_reportObjectModel != null && this.m_reportObjectModel.UserImpl != null)
					{
						return this.m_reportObjectModel.UserImpl.HasUserProfileState;
					}
					return UserProfileState.None;
				}
			}

			internal ChunkManager.ProcessingChunkManager ChunkManager
			{
				get
				{
					return this.m_commonInfo.ChunkManager;
				}
			}

			internal QuickFindHashtable QuickFind
			{
				get
				{
					return this.m_commonInfo.QuickFind;
				}
			}

			internal ReportDrillthroughInfo DrillthroughInfo
			{
				get
				{
					return this.m_commonInfo.DrillthroughInfo;
				}
				set
				{
					this.m_commonInfo.DrillthroughInfo = value;
				}
			}

			internal int UniqueNameCounter
			{
				get
				{
					return this.m_commonInfo.UniqueNameCounter;
				}
			}

			internal ExecutionType InteractiveExecution
			{
				get
				{
					return this.m_commonInfo.InteractiveExecution;
				}
			}

			internal bool HasImageStreams
			{
				get
				{
					return this.m_commonInfo.HasImageStreams;
				}
				set
				{
					this.m_commonInfo.HasImageStreams = value;
				}
			}

			internal UserProfileState AllowUserProfileState
			{
				get
				{
					return this.m_commonInfo.AllowUserProfileState;
				}
			}

			internal bool HasUserSortFilter
			{
				get
				{
					return this.m_commonInfo.HasUserSortFilter;
				}
				set
				{
					this.m_commonInfo.HasUserSortFilter = value;
				}
			}

			internal bool SaveSnapshotData
			{
				get
				{
					return this.m_commonInfo.SaveSnapshotData;
				}
				set
				{
					this.m_commonInfo.SaveSnapshotData = value;
				}
			}

			internal bool StopSaveSnapshotDataOnError
			{
				get
				{
					return this.m_commonInfo.StopSaveSnapshotDataOnError;
				}
				set
				{
					this.m_commonInfo.StopSaveSnapshotDataOnError = value;
				}
			}

			internal bool ErrorSavingSnapshotData
			{
				get
				{
					return this.m_commonInfo.ErrorSavingSnapshotData;
				}
				set
				{
					this.m_commonInfo.ErrorSavingSnapshotData = value;
				}
			}

			internal bool IsHistorySnapshot
			{
				get
				{
					return this.m_commonInfo.IsHistorySnapshot;
				}
			}

			public bool SnapshotProcessing
			{
				get
				{
					return this.m_commonInfo.SnapshotProcessing;
				}
				set
				{
					this.m_commonInfo.SnapshotProcessing = value;
				}
			}

			internal bool UserSortFilterProcessing
			{
				get
				{
					return this.m_commonInfo.UserSortFilterProcessing;
				}
				set
				{
					this.m_commonInfo.UserSortFilterProcessing = value;
				}
			}

			internal bool ResetForSubreportDataPrefetch
			{
				get
				{
					return this.m_commonInfo.ResetForSubreportDataPrefetch;
				}
				set
				{
					this.m_commonInfo.ResetForSubreportDataPrefetch = value;
				}
			}

			internal bool ProcessWithCachedData
			{
				get
				{
					return this.m_commonInfo.ProcessWithCachedData;
				}
			}

			internal GetReportChunk GetReportChunkCallback
			{
				get
				{
					return this.m_commonInfo.GetReportChunkCallback;
				}
			}

			internal CreateReportChunk CacheDataCallback
			{
				get
				{
					return this.m_commonInfo.CacheDataCallback;
				}
			}

			internal bool DataCached
			{
				get
				{
					return this.m_commonInfo.DataCached;
				}
				set
				{
					this.m_commonInfo.DataCached = value;
				}
			}

			internal Hashtable CachedDataChunkMapping
			{
				get
				{
					return this.m_commonInfo.CachedDataChunkMapping;
				}
				set
				{
					this.m_commonInfo.CachedDataChunkMapping = value;
				}
			}

			internal CustomReportItemControls CriProcessingControls
			{
				get
				{
					return this.m_commonInfo.CriProcessingControls;
				}
				set
				{
					this.m_commonInfo.CriProcessingControls = value;
				}
			}

			internal EventInformation UserSortFilterInfo
			{
				get
				{
					return this.m_commonInfo.UserSortFilterInfo;
				}
				set
				{
					this.m_commonInfo.UserSortFilterInfo = value;
				}
			}

			internal SortFilterEventInfoHashtable OldSortFilterEventInfo
			{
				get
				{
					return this.m_commonInfo.OldSortFilterEventInfo;
				}
				set
				{
					this.m_commonInfo.OldSortFilterEventInfo = value;
				}
			}

			internal SortFilterEventInfoHashtable NewSortFilterEventInfo
			{
				get
				{
					return this.m_commonInfo.NewSortFilterEventInfo;
				}
				set
				{
					this.m_commonInfo.NewSortFilterEventInfo = value;
				}
			}

			internal RuntimeSortFilterEventInfoList ReportRuntimeUserSortFilterInfo
			{
				get
				{
					return this.m_commonInfo.ReportRuntimeUserSortFilterInfo;
				}
				set
				{
					this.m_commonInfo.ReportRuntimeUserSortFilterInfo = value;
				}
			}

			internal ReportRuntimeSetup ReportRuntimeSetup
			{
				get
				{
					return this.m_commonInfo.ReportRuntimeSetup;
				}
			}

			internal CultureInfo ThreadCulture
			{
				get
				{
					return this.m_threadCulture;
				}
				set
				{
					this.m_threadCulture = value;
				}
			}

			internal uint LanguageInstanceId
			{
				get
				{
					return this.m_languageInstanceId;
				}
				set
				{
					this.m_languageInstanceId = value;
				}
			}

			internal uint SubReportLevel
			{
				get
				{
					return this.m_subReportLevel;
				}
			}

			internal ICatalogItemContext ReportContext
			{
				get
				{
					return this.m_reportContext;
				}
			}

			internal ReportRuntime ReportRuntime
			{
				get
				{
					return this.m_reportRuntime;
				}
				set
				{
					this.m_reportRuntime = value;
				}
			}

			internal ObjectModelImpl ReportObjectModel
			{
				get
				{
					return this.m_reportObjectModel;
				}
				set
				{
					this.m_reportObjectModel = value;
				}
			}

			internal bool ReportItemsReferenced
			{
				get
				{
					return this.m_reportItemsReferenced;
				}
			}

			internal bool ReportItemThisDotValueReferenced
			{
				get
				{
					return this.m_reportItemThisDotValueReferenced;
				}
			}

			internal DataSourceInfoHashtable GlobalDataSourceInfo
			{
				get
				{
					return this.m_dataSourceInfo;
				}
			}

			internal Report.ShowHideTypes ShowHideType
			{
				get
				{
					return this.m_showHideType;
				}
			}

			internal EmbeddedImageHashtable EmbeddedImages
			{
				get
				{
					return this.m_embeddedImages;
				}
			}

			internal ImageStreamNames ImageStreamNames
			{
				get
				{
					return this.m_imageStreamNames;
				}
				set
				{
					this.m_imageStreamNames = value;
				}
			}

			internal bool InPageSection
			{
				get
				{
					return this.m_inPageSection;
				}
			}

			internal AbortHelper AbortInfo
			{
				get
				{
					return this.m_abortHelper;
				}
				set
				{
					this.m_abortHelper = value;
				}
			}

			public ErrorContext ErrorContext
			{
				get
				{
					return this.m_errorContext;
				}
			}

			internal bool ProcessReportParameters
			{
				get
				{
					return this.m_processReportParameters;
				}
			}

			internal MatrixHeadingInstance HeadingInstance
			{
				get
				{
					return this.m_headingInstance;
				}
				set
				{
					this.m_headingInstance = value;
				}
			}

			internal MatrixHeadingInstance HeadingInstanceOld
			{
				get
				{
					return this.m_headingInstanceOld;
				}
				set
				{
					this.m_headingInstanceOld = value;
				}
			}

			internal bool DelayAddingInstanceInfo
			{
				get
				{
					return this.m_delayAddingInstanceInfo;
				}
				set
				{
					this.m_delayAddingInstanceInfo = value;
				}
			}

			internal bool SpecialRecursiveAggregates
			{
				get
				{
					return this.m_specialRecursiveAggregates;
				}
			}

			internal SecondPassOperations SecondPassOperation
			{
				get
				{
					return this.m_secondPassOperation;
				}
				set
				{
					this.m_secondPassOperation = value;
				}
			}

			internal AggregatesImpl GlobalRVCollection
			{
				get
				{
					return this.m_globalRVCollection;
				}
				set
				{
					this.m_globalRVCollection = value;
				}
			}

			internal string TransparentImageGuid
			{
				get
				{
					return this.m_transparentImageGuid;
				}
				set
				{
					this.m_transparentImageGuid = value;
				}
			}

			internal Pagination Pagination
			{
				get
				{
					return this.m_pagination;
				}
				set
				{
					this.m_pagination = value;
				}
			}

			internal NavigationInfo NavigationInfo
			{
				get
				{
					return this.m_navigationInfo;
				}
				set
				{
					this.m_navigationInfo = value;
				}
			}

			internal CompareInfo CompareInfo
			{
				get
				{
					return this.m_compareInfo;
				}
				set
				{
					this.m_compareInfo = value;
				}
			}

			internal CompareOptions ClrCompareOptions
			{
				get
				{
					return this.m_clrCompareOptions;
				}
				set
				{
					this.m_clrCompareOptions = value;
				}
			}

			internal int DataSetUniqueName
			{
				get
				{
					return this.m_dataSetUniqueName;
				}
			}

			internal bool CreatePageSectionImageChunks
			{
				get
				{
					return this.m_createPageSectionImageChunks;
				}
			}

			internal PageSectionContext PageSectionContext
			{
				get
				{
					return this.m_pageSectionContext;
				}
				set
				{
					this.m_pageSectionContext = value;
				}
			}

			internal UserSortFilterContext UserSortFilterContext
			{
				get
				{
					return this.m_userSortFilterContext;
				}
				set
				{
					this.m_userSortFilterContext = value;
				}
			}

			internal RuntimeSortFilterEventInfoList RuntimeSortFilterInfo
			{
				get
				{
					return this.m_userSortFilterContext.RuntimeSortFilterInfo;
				}
				set
				{
					this.m_userSortFilterContext.RuntimeSortFilterInfo = value;
				}
			}

			internal IJobContext JobContext
			{
				get
				{
					return this.m_jobContext;
				}
			}

			internal IExtensionFactory ExtFactory
			{
				get
				{
					return this.m_extFactory;
				}
			}

			internal IProcessingDataExtensionConnection DataExtensionConnection
			{
				get
				{
					return this.m_dataExtensionConnection;
				}
			}

			internal IDataProtection DataProtection
			{
				get
				{
					return this.m_dataProtection;
				}
			}

			internal bool IgnoreAllFromStart
			{
				set
				{
					Global.Tracer.Assert(null != this.m_showHideInfo, "(null != m_showHideInfo)");
					this.m_showHideInfo.IgnoreAllFromStart = value;
				}
			}

			internal bool UseAllContainers
			{
				set
				{
					Global.Tracer.Assert(null != this.m_showHideInfo, "(null != m_showHideInfo)");
					this.m_showHideInfo.UseAllContainers = value;
				}
			}

			protected ProcessingContext(string chartName, string requestUserName, CultureInfo userLanguage, SubReportCallback subReportCallback, ICatalogItemContext reportContext, Report report, ErrorContext errorContext, CreateReportChunk createReportChunkCallback, IGetResource getResourceCallback, ExecutionType interactiveExecution, DateTime executionTime, UserProfileState allowUserProfileState, bool isHistorySnapshot, bool snapshotProcessing, bool processWithCachedData, GetReportChunk getChunkCallback, CreateReportChunk cacheDataCallback, ReportRuntimeSetup reportRuntimeSetup, IJobContext jobContext, IExtensionFactory extFactory, IProcessingDataExtensionConnection dataExtensionConnection, IDataProtection dataProtection)
			{
				this.m_commonInfo = new CommonInfo(chartName, requestUserName, userLanguage, subReportCallback, report, createReportChunkCallback, getResourceCallback, interactiveExecution, executionTime, allowUserProfileState, isHistorySnapshot, snapshotProcessing, processWithCachedData, getChunkCallback, cacheDataCallback, reportRuntimeSetup);
				this.m_subReportLevel = 0u;
				this.m_reportContext = reportContext;
				this.m_reportObjectModel = null;
				this.m_reportItemsReferenced = report.HasReportItemReferences;
				this.m_reportItemThisDotValueReferenced = false;
				this.m_showHideInfo = new ShowHideInfo();
				this.m_dataSourceInfo = new DataSourceInfoHashtable();
				this.m_showHideType = report.ShowHideType;
				this.m_embeddedImages = report.EmbeddedImages;
				this.m_imageStreamNames = report.ImageStreamNames;
				this.m_abortHelper = new AbortHelper(jobContext);
				this.m_inPageSection = false;
				this.m_specialDataRegionFilters = null;
				this.m_errorContext = errorContext;
				this.m_processReportParameters = false;
				this.m_pivotRunningValueScopes = null;
				this.m_reportRuntime = null;
				this.m_delayAddingInstanceInfo = false;
				this.m_specialRecursiveAggregates = report.HasSpecialRecursiveAggregates;
				this.m_pagination = new Pagination(report.InteractiveHeightValue);
				this.m_navigationInfo = new NavigationInfo();
				this.m_pageSectionContext = new PageSectionContext(report.PageHeaderEvaluation || report.PageFooterEvaluation, report.MergeOnePass);
				this.m_userSortFilterContext = new UserSortFilterContext();
				this.m_jobContext = jobContext;
				this.m_extFactory = extFactory;
				this.m_dataExtensionConnection = dataExtensionConnection;
				this.m_dataProtection = dataProtection;
			}

			protected ProcessingContext(ProcessingContext parentContext, string requestUserName, CultureInfo userlanguage, ICatalogItemContext reportContext, ErrorContext errorContext, ExecutionType interactiveExecution, DateTime executionTime, UserProfileState allowUserProfileState, bool snapshotProcessing, ReportRuntimeSetup reportRuntimeSetup, IJobContext jobContext, IExtensionFactory extFactory, IProcessingDataExtensionConnection dataExtensionConnection, IDataProtection dataProtection)
			{
				this.m_commonInfo = new CommonInfo(null, requestUserName, userlanguage, null, null, null, null, interactiveExecution, executionTime, allowUserProfileState, false, snapshotProcessing, false, null, null, reportRuntimeSetup);
				this.m_subReportLevel = 0u;
				this.m_reportContext = reportContext;
				this.m_reportObjectModel = null;
				this.m_reportItemsReferenced = false;
				this.m_reportItemThisDotValueReferenced = false;
				this.m_showHideInfo = new ShowHideInfo();
				this.m_dataSourceInfo = new DataSourceInfoHashtable();
				this.m_showHideType = Report.ShowHideTypes.None;
				this.m_embeddedImages = null;
				this.m_imageStreamNames = null;
				if (parentContext != null)
				{
					this.m_abortHelper = parentContext.AbortInfo;
				}
				else
				{
					this.m_abortHelper = new AbortHelper(jobContext);
				}
				this.m_inPageSection = false;
				this.m_specialDataRegionFilters = null;
				this.m_errorContext = errorContext;
				this.m_processReportParameters = true;
				this.m_pivotRunningValueScopes = null;
				this.m_reportRuntime = null;
				this.m_delayAddingInstanceInfo = false;
				this.m_specialRecursiveAggregates = false;
				this.m_pagination = new Pagination(0.0);
				this.m_navigationInfo = new NavigationInfo();
				this.m_userSortFilterContext = new UserSortFilterContext();
				this.m_jobContext = jobContext;
				this.m_extFactory = extFactory;
				this.m_dataExtensionConnection = dataExtensionConnection;
				this.m_dataProtection = dataProtection;
			}

			protected ProcessingContext(SubReport subReport, ErrorContext errorContext, ProcessingContext copy, int subReportDataSetUniqueName)
			{
				this.m_commonInfo = copy.m_commonInfo;
				this.m_subReportLevel = copy.m_subReportLevel + 1;
				this.m_threadCulture = copy.ThreadCulture;
				this.m_languageInstanceId = copy.m_languageInstanceId;
				this.m_reportContext = subReport.ReportContext;
				this.m_reportObjectModel = null;
				this.m_reportItemsReferenced = subReport.Report.HasReportItemReferences;
				this.m_reportItemThisDotValueReferenced = false;
				this.m_showHideInfo = new ShowHideInfo(copy.m_showHideInfo);
				this.m_dataSourceInfo = copy.m_dataSourceInfo;
				this.m_showHideType = subReport.Report.ShowHideType;
				this.m_embeddedImages = subReport.Report.EmbeddedImages;
				this.m_imageStreamNames = subReport.Report.ImageStreamNames;
				this.m_abortHelper = copy.AbortInfo;
				this.m_inPageSection = false;
				this.m_specialDataRegionFilters = null;
				this.m_errorContext = errorContext;
				this.m_processReportParameters = false;
				this.m_pivotRunningValueScopes = null;
				this.m_reportRuntime = null;
				this.m_delayAddingInstanceInfo = false;
				this.m_specialRecursiveAggregates = subReport.Report.HasSpecialRecursiveAggregates;
				this.m_pagination = copy.m_pagination;
				this.m_dataSetUniqueName = subReportDataSetUniqueName;
				this.m_navigationInfo = copy.m_navigationInfo;
				this.m_pageSectionContext = copy.PageSectionContext;
				this.m_userSortFilterContext = new UserSortFilterContext(copy.UserSortFilterContext, subReport);
				this.m_jobContext = copy.JobContext;
				this.m_extFactory = copy.m_extFactory;
				this.m_dataExtensionConnection = copy.DataExtensionConnection;
				this.m_dataProtection = copy.DataProtection;
			}

			internal ProcessingContext(ICatalogItemContext reportContext, Report.ShowHideTypes showHideType, IGetResource getResourceCallback, EmbeddedImageHashtable embeddedImages, ImageStreamNames imageStreamNames, ErrorContext errorContext, bool reportItemsReferenced, UserProfileState allowUserProfileState, ReportRuntimeSetup reportRuntimeSetup, IDataProtection dataProtection)
			{
				this.m_commonInfo = new CommonInfo(getResourceCallback, allowUserProfileState, reportRuntimeSetup);
				this.m_subReportLevel = 0u;
				this.m_reportContext = reportContext;
				this.m_reportObjectModel = null;
				this.m_reportItemsReferenced = reportItemsReferenced;
				this.m_reportItemThisDotValueReferenced = false;
				this.m_showHideInfo = null;
				this.m_dataSourceInfo = null;
				this.m_showHideType = showHideType;
				this.m_embeddedImages = embeddedImages;
				this.m_imageStreamNames = imageStreamNames;
				this.m_abortHelper = null;
				this.m_inPageSection = true;
				this.m_specialDataRegionFilters = null;
				this.m_errorContext = errorContext;
				this.m_processReportParameters = false;
				this.m_pivotRunningValueScopes = null;
				this.m_reportRuntime = null;
				this.m_delayAddingInstanceInfo = false;
				this.m_specialRecursiveAggregates = false;
				this.m_pagination = new Pagination(0.0);
				this.m_navigationInfo = new NavigationInfo();
				this.m_pageSectionContext = new PageSectionContext(true, false);
				this.m_userSortFilterContext = new UserSortFilterContext();
				this.m_dataProtection = dataProtection;
			}

			internal ProcessingContext(ICatalogItemContext reportContext, Report.ShowHideTypes showHideType, IGetResource getResourceCallback, EmbeddedImageHashtable embeddedImages, ImageStreamNames imageStreamNames, ErrorContext errorContext, bool reportItemsReferenced, UserProfileState allowUserProfileState, ReportRuntimeSetup reportRuntimeSetup, CreateReportChunk createChunkCallback, ChunkManager.ProcessingChunkManager processingChunkManager, int uniqueNameCounter, IDataProtection dataProtection, ref ReportDrillthroughInfo drillthroughInfo)
			{
				this.m_commonInfo = new CommonInfo(getResourceCallback, allowUserProfileState, reportRuntimeSetup, createChunkCallback, processingChunkManager, uniqueNameCounter, ref drillthroughInfo);
				this.m_inPageSection = true;
				this.m_createPageSectionImageChunks = true;
				this.m_subReportLevel = 0u;
				this.m_reportContext = reportContext;
				this.m_reportObjectModel = null;
				this.m_reportItemsReferenced = reportItemsReferenced;
				this.m_reportItemThisDotValueReferenced = false;
				this.m_showHideInfo = null;
				this.m_dataSourceInfo = null;
				this.m_showHideType = showHideType;
				this.m_embeddedImages = embeddedImages;
				this.m_imageStreamNames = imageStreamNames;
				this.m_abortHelper = null;
				this.m_inPageSection = true;
				this.m_specialDataRegionFilters = null;
				this.m_errorContext = errorContext;
				this.m_processReportParameters = false;
				this.m_pivotRunningValueScopes = null;
				this.m_reportRuntime = null;
				this.m_delayAddingInstanceInfo = false;
				this.m_specialRecursiveAggregates = false;
				this.m_pagination = new Pagination(0.0);
				this.m_navigationInfo = new NavigationInfo();
				this.m_userSortFilterContext = new UserSortFilterContext();
				this.m_dataProtection = dataProtection;
			}

			protected ProcessingContext(ProcessingContext copy)
			{
				this.m_commonInfo = copy.m_commonInfo;
				this.m_subReportLevel = copy.m_subReportLevel;
				this.m_reportContext = copy.m_reportContext;
				this.m_threadCulture = copy.m_threadCulture;
				this.m_reportObjectModel = new ObjectModelImpl(copy.ReportObjectModel, this);
				this.m_reportItemsReferenced = copy.ReportItemsReferenced;
				this.m_reportItemThisDotValueReferenced = false;
				this.m_showHideInfo = copy.m_showHideInfo;
				this.m_dataSourceInfo = copy.m_dataSourceInfo;
				this.m_showHideType = copy.m_showHideType;
				this.m_embeddedImages = copy.m_embeddedImages;
				this.m_imageStreamNames = copy.m_imageStreamNames;
				this.m_abortHelper = copy.m_abortHelper;
				this.m_inPageSection = copy.m_inPageSection;
				this.m_specialDataRegionFilters = null;
				this.m_errorContext = copy.m_errorContext;
				this.m_processReportParameters = copy.m_processReportParameters;
				this.m_pivotRunningValueScopes = null;
				this.m_reportRuntime = ((copy.ReportRuntime == null) ? null : new ReportRuntime(this.m_reportObjectModel, this.m_errorContext, copy.ReportRuntime.ReportExprHost, copy.ReportRuntime));
				this.m_delayAddingInstanceInfo = false;
				this.m_specialRecursiveAggregates = copy.m_specialRecursiveAggregates;
				this.m_pagination = copy.m_pagination;
				this.m_dataSetUniqueName = copy.DataSetUniqueName;
				this.m_navigationInfo = copy.m_navigationInfo;
				this.m_pageSectionContext = copy.PageSectionContext;
				this.m_userSortFilterContext = new UserSortFilterContext(copy.UserSortFilterContext);
				this.m_jobContext = copy.m_jobContext;
				this.m_extFactory = copy.m_extFactory;
				this.m_dataExtensionConnection = copy.m_dataExtensionConnection;
				this.m_dataProtection = copy.m_dataProtection;
			}

			internal virtual ProcessingContext ParametersContext(ICatalogItemContext reportContext, ProcessingErrorContext subReportErrorContext)
			{
				return null;
			}

			internal virtual ProcessingContext SubReportContext(SubReport subReport, int subReportDataSetUniqueName, ProcessingErrorContext subReportErrorContext)
			{
				return null;
			}

			internal virtual ProcessingContext CloneContext(ProcessingContext context)
			{
				return null;
			}

			internal void CheckAndThrowIfAborted()
			{
				if (this.m_abortHelper.GetStatus(this.DataSetUniqueName) != 0)
				{
					this.m_abortHelper.ThrowAbortException(this.DataSetUniqueName);
				}
			}

			internal void RuntimeInitializeReportItemObjs(ReportItemCollection reportItems, bool traverseDataRegions, bool setValue)
			{
				for (int i = 0; i < reportItems.Count; i++)
				{
					this.RuntimeInitializeReportItemObjs(reportItems[i], traverseDataRegions, setValue);
				}
			}

			internal void RuntimeInitializeReportItemObjs(ReportItem reportItem, bool traverseDataRegions, bool setValue)
			{
				ReportItemImpl reportItemImpl = null;
				if (reportItem != null)
				{
					if (!(reportItem is DataRegion))
					{
						if (this.m_reportRuntime.ReportExprHost != null)
						{
							reportItem.SetExprHost(this.m_reportRuntime.ReportExprHost, this.m_reportObjectModel);
						}
						if (reportItem is TextBox)
						{
							TextBox textBox = (TextBox)reportItem;
							if (this.m_reportItemsReferenced || textBox.ValueReferenced)
							{
								TextBoxImpl textBoxImpl = new TextBoxImpl(textBox, this.m_reportRuntime, this.m_reportRuntime);
								if (setValue)
								{
									textBoxImpl.SetResult(default(VariantResult));
								}
								if (textBox.ValueReferenced)
								{
									Global.Tracer.Assert(textBox.ExprHost != null, "(textBoxDef.ExprHost != null)");
									this.m_reportItemThisDotValueReferenced = true;
									textBox.TextBoxExprHost.SetTextBox(textBoxImpl);
								}
								if (this.m_reportItemsReferenced)
								{
									reportItemImpl = textBoxImpl;
								}
							}
						}
						else if (reportItem is Rectangle)
						{
							this.RuntimeInitializeReportItemObjs(((Rectangle)reportItem).ReportItems, traverseDataRegions, setValue);
						}
					}
					else
					{
						if (reportItem is CustomReportItem && ((CustomReportItem)reportItem).DataSetName == null && this.m_reportRuntime.ReportExprHost != null)
						{
							reportItem.SetExprHost(this.m_reportRuntime.ReportExprHost, this.m_reportObjectModel);
						}
						if (traverseDataRegions)
						{
							if (this.m_reportRuntime.ReportExprHost != null)
							{
								reportItem.SetExprHost(this.m_reportRuntime.ReportExprHost, this.m_reportObjectModel);
							}
							if (reportItem is List)
							{
								this.RuntimeInitializeReportItemObjs(((List)reportItem).ReportItems, traverseDataRegions, setValue);
							}
							else if (reportItem is Matrix)
							{
								Matrix matrix = (Matrix)reportItem;
								this.RuntimeInitializeReportItemObjs(matrix.CornerReportItems, traverseDataRegions, setValue);
								this.RuntimeInitializeReportItemObjs(matrix.CellReportItems, traverseDataRegions, setValue);
								this.InitializeMatrixHeadingRuntimeObjs(matrix.Rows, (matrix.ExprHost != null) ? matrix.MatrixExprHost.RowGroupingsHost : null, traverseDataRegions, setValue);
								this.InitializeMatrixHeadingRuntimeObjs(matrix.Columns, (matrix.ExprHost != null) ? matrix.MatrixExprHost.ColumnGroupingsHost : null, traverseDataRegions, setValue);
							}
							else if (reportItem is Chart)
							{
								Chart chart = (Chart)reportItem;
								this.InitializeChartHeadingRuntimeObjs(chart.Rows, (chart.ExprHost != null) ? chart.ChartExprHost.RowGroupingsHost : null);
								this.InitializeChartHeadingRuntimeObjs(chart.Columns, (chart.ExprHost != null) ? chart.ChartExprHost.ColumnGroupingsHost : null);
							}
							else if (reportItem is CustomReportItem)
							{
								CustomReportItem customReportItem = (CustomReportItem)reportItem;
								this.InitializeCRIHeadingRuntimeObjs(customReportItem.Rows, (customReportItem.ExprHost == null) ? null : ((CustomReportItemExprHost)customReportItem.ExprHost).DataGroupingHostsRemotable);
								this.InitializeCRIHeadingRuntimeObjs(customReportItem.Columns, (customReportItem.ExprHost == null) ? null : ((CustomReportItemExprHost)customReportItem.ExprHost).DataGroupingHostsRemotable);
							}
							else if (reportItem is Table)
							{
								Table table = (Table)reportItem;
								if (table.HeaderRows != null)
								{
									for (int i = 0; i < table.HeaderRows.Count; i++)
									{
										this.RuntimeInitializeReportItemObjs(table.HeaderRows[i].ReportItems, traverseDataRegions, setValue);
									}
								}
								if (table.FooterRows != null)
								{
									for (int j = 0; j < table.FooterRows.Count; j++)
									{
										this.RuntimeInitializeReportItemObjs(table.FooterRows[j].ReportItems, traverseDataRegions, setValue);
									}
								}
								if (table.TableDetail != null)
								{
									for (int k = 0; k < table.TableDetail.DetailRows.Count; k++)
									{
										this.RuntimeInitializeReportItemObjs(table.TableDetail.DetailRows[k].ReportItems, traverseDataRegions, setValue);
									}
								}
								ProcessingContext.InitializeTableGroupRuntimeObjs(table, table.TableGroups, (table.ExprHost != null) ? table.TableExprHost.TableGroupsHost : null, this, this.m_reportObjectModel, traverseDataRegions, setValue);
							}
						}
					}
					if (reportItemImpl != null)
					{
						this.m_reportObjectModel.ReportItemsImpl.Add(reportItemImpl);
					}
				}
			}

			internal void InitializeMatrixHeadingRuntimeObjs(MatrixHeading heading, MatrixDynamicGroupExprHost headingExprHost, bool traverseDataRegions, bool setValue)
			{
				while (heading != null)
				{
					this.RuntimeInitializeReportItemObjs(heading.ReportItems, traverseDataRegions, setValue);
					if (heading.Subtotal != null)
					{
						if (heading.HasExprHost && headingExprHost.SubtotalHost != null)
						{
							heading.Subtotal.SetExprHost(headingExprHost.SubtotalHost, this.m_reportObjectModel);
						}
						this.RuntimeInitializeReportItemObjs(heading.Subtotal.ReportItems, traverseDataRegions, setValue);
					}
					if (heading.HasExprHost)
					{
						heading.SetExprHost(headingExprHost, this.m_reportObjectModel);
						headingExprHost = (MatrixDynamicGroupExprHost)headingExprHost.SubGroupHost;
					}
					heading = heading.SubHeading;
				}
			}

			internal void InitializeChartHeadingRuntimeObjs(ChartHeading heading, ChartDynamicGroupExprHost headingExprHost)
			{
				while (heading != null)
				{
					if (heading.HasExprHost)
					{
						heading.SetExprHost(headingExprHost, this.m_reportObjectModel);
						headingExprHost = (ChartDynamicGroupExprHost)headingExprHost.SubGroupHost;
					}
					heading = heading.SubHeading;
				}
			}

			internal static void InitializeTableGroupRuntimeObjs(Table table, TableGroup group, TableGroupExprHost groupExprHost, ProcessingContext processingContext, ObjectModelImpl reportObjectModel, bool traverseDataRegions, bool setValue)
			{
				while (group != null)
				{
					if (group.HasExprHost)
					{
						group.SetExprHost(groupExprHost, reportObjectModel);
						groupExprHost = (TableGroupExprHost)groupExprHost.SubGroupHost;
					}
					if (processingContext != null)
					{
						if (group.HeaderRows != null)
						{
							for (int i = 0; i < group.HeaderRows.Count; i++)
							{
								processingContext.RuntimeInitializeReportItemObjs(group.HeaderRows[i].ReportItems, traverseDataRegions, setValue);
							}
						}
						if (group.FooterRows != null)
						{
							for (int j = 0; j < group.FooterRows.Count; j++)
							{
								processingContext.RuntimeInitializeReportItemObjs(group.FooterRows[j].ReportItems, traverseDataRegions, setValue);
							}
						}
					}
					group = group.SubGroup;
				}
				if (table.TableDetail != null && table.TableDetail.HasExprHost)
				{
					table.TableDetail.SetExprHost(groupExprHost, reportObjectModel);
				}
			}

			internal void InitializeCRIHeadingRuntimeObjs(CustomReportItemHeadingList headings, IList<DataGroupingExprHost> headingExprHosts)
			{
				if (headings != null)
				{
					for (int i = 0; i < headings.Count; i++)
					{
						if (headings[i].HasExprHost)
						{
							headings[i].SetExprHost(headingExprHosts, this.m_reportObjectModel);
							if (headings[i].ExprHostID >= 0 && headingExprHosts[headings[i].ExprHostID].DataGroupingHostsRemotable != null)
							{
								Global.Tracer.Assert(null != headings[i].InnerHeadings, "(null != headings[i].InnerHeadings)");
								this.InitializeCRIHeadingRuntimeObjs(headings[i].InnerHeadings, headingExprHosts[headings[i].ExprHostID].DataGroupingHostsRemotable);
							}
						}
					}
				}
			}

			internal void EndIgnoreRange()
			{
				Global.Tracer.Assert(null != this.m_showHideInfo, "(null != m_showHideInfo)");
				this.m_showHideInfo.EndIgnoreRange();
			}

			internal void RegisterIgnoreRange()
			{
				Global.Tracer.Assert(null != this.m_showHideInfo, "(null != m_showHideInfo)");
				this.m_showHideInfo.RegisterIgnoreRange();
			}

			internal void UnRegisterIgnoreRange()
			{
				Global.Tracer.Assert(null != this.m_showHideInfo, "(null != m_showHideInfo)");
				this.m_showHideInfo.UnRegisterIgnoreRange();
			}

			internal void BeginProcessContainer(int uniqueName, Visibility visibility)
			{
				if (visibility != null && visibility.Toggle != null)
				{
					Global.Tracer.Assert(null != this.m_showHideInfo, "(null != m_showHideInfo)");
					this.m_showHideInfo.RegisterContainer(uniqueName);
				}
			}

			internal void EndProcessContainer(int uniqueName, Visibility visibility)
			{
				if (visibility != null && visibility.Toggle != null)
				{
					Global.Tracer.Assert(null != this.m_showHideInfo, "(null != m_showHideInfo)");
					this.m_showHideInfo.UnRegisterContainer(uniqueName);
				}
			}

			internal bool ProcessSender(int uniqueName, bool startHidden, TextBox textBox)
			{
				bool result = false;
				if (textBox.InitialToggleState != null)
				{
					result = this.m_reportRuntime.EvaluateTextBoxInitialToggleStateExpression(textBox);
				}
				if (textBox.IsToggle)
				{
					Global.Tracer.Assert(null != this.m_showHideInfo, "(null != m_showHideInfo)");
					this.m_showHideInfo.RegisterSender(textBox.Name, uniqueName, startHidden, textBox.RecursiveSender);
				}
				return result;
			}

			internal bool ProcessReceiver(int uniqueName, Visibility visibility, IVisibilityHiddenExprHost visibilityExprHostRI, ObjectType objectType, string objectName)
			{
				bool flag = false;
				if (visibility != null)
				{
					flag = this.m_reportRuntime.EvaluateStartHiddenExpression(visibility, visibilityExprHostRI, objectType, objectName);
					if (visibility.Toggle != null)
					{
						Global.Tracer.Assert(null != this.m_showHideInfo, "(null != m_showHideInfo)");
						this.m_showHideInfo.RegisterReceiver(visibility.Toggle, uniqueName, flag, visibility.RecursiveReceiver);
					}
				}
				return flag;
			}

			internal bool ProcessReceiver(int uniqueName, Visibility visibility, IndexedExprHost visibilityExprHostIdx, ObjectType objectType, string objectName)
			{
				bool flag = false;
				if (visibility != null)
				{
					flag = this.m_reportRuntime.EvaluateStartHiddenExpression(visibility, visibilityExprHostIdx, objectType, objectName);
					if (visibility.Toggle != null)
					{
						Global.Tracer.Assert(null != this.m_showHideInfo, "(null != m_showHideInfo)");
						this.m_showHideInfo.RegisterReceiver(visibility.Toggle, uniqueName, flag, visibility.RecursiveReceiver);
					}
				}
				return flag;
			}

			internal void EnterGrouping()
			{
				Global.Tracer.Assert(null != this.m_showHideInfo, "(null != m_showHideInfo)");
				this.m_showHideInfo.EnterGrouping();
			}

			internal void EnterChildGroupings()
			{
				Global.Tracer.Assert(null != this.m_showHideInfo, "(null != m_showHideInfo)");
				this.m_showHideInfo.EnterChildGroupings();
			}

			internal void ExitGrouping()
			{
				Global.Tracer.Assert(null != this.m_showHideInfo, "(null != m_showHideInfo)");
				this.m_showHideInfo.ExitGrouping();
			}

			internal void ExitChildGroupings()
			{
				Global.Tracer.Assert(null != this.m_showHideInfo, "(null != m_showHideInfo)");
				this.m_showHideInfo.ExitChildGroupings();
			}

			internal void GetSenderAndReceiverInfo(out SenderInformationHashtable senderInfo, out ReceiverInformationHashtable receiverInfo)
			{
				Global.Tracer.Assert(null != this.m_showHideInfo, "(null != m_showHideInfo)");
				this.m_showHideInfo.GetSenderAndReceiverInfo(out senderInfo, out receiverInfo);
			}

			internal void AddSpecialDataRegionFilters(Filters filters)
			{
				if (this.m_specialDataRegionFilters == null)
				{
					this.m_specialDataRegionFilters = new FiltersList();
				}
				this.m_specialDataRegionFilters.Add(filters);
			}

			private void ProcessDataRegionsWithSpecialFilters()
			{
				if (this.m_specialDataRegionFilters != null)
				{
					int count = this.m_specialDataRegionFilters.Count;
					for (int i = 0; i < count; i++)
					{
						this.m_specialDataRegionFilters[i].FinishReadingRows();
						count = this.m_specialDataRegionFilters.Count;
					}
					this.m_specialDataRegionFilters = null;
				}
			}

			internal void EnterPivotCell(bool escalateScope)
			{
				if (this.m_pivotRunningValueScopes == null)
				{
					this.m_pivotRunningValueScopes = new List<bool>();
				}
				this.m_pivotRunningValueScopes.Add(escalateScope);
			}

			internal void ExitPivotCell()
			{
				Global.Tracer.Assert(null != this.m_pivotRunningValueScopes, "(null != m_pivotRunningValueScopes)");
				this.m_pivotRunningValueScopes.RemoveAt(this.m_pivotRunningValueScopes.Count - 1);
			}

			internal bool PivotEscalateScope()
			{
				if (this.m_pivotRunningValueScopes != null && 0 < this.m_pivotRunningValueScopes.Count)
				{
					return this.m_pivotRunningValueScopes[this.m_pivotRunningValueScopes.Count - 1];
				}
				return false;
			}

			internal bool PopulateRuntimeSortFilterEventInfo(DataSet myDataSet)
			{
				return this.m_userSortFilterContext.PopulateRuntimeSortFilterEventInfo(this, myDataSet);
			}

			internal bool IsSortFilterTarget(bool[] isSortFilterTarget, IScope outerScope, IHierarchyObj target, ref RuntimeUserSortTargetInfo userSortTargetInfo)
			{
				return this.m_userSortFilterContext.IsSortFilterTarget(isSortFilterTarget, outerScope, target, ref userSortTargetInfo);
			}

			internal EventInformation GetUserSortFilterInformation(ref int oldUniqueName, ref int page)
			{
				return this.m_commonInfo.GetUserSortFilterInformation(ref oldUniqueName, ref page);
			}

			internal void RegisterSortFilterExpressionScope(IScope container, RuntimeDataRegionObj scopeObj, bool[] isSortFilterExpressionScope)
			{
				this.m_userSortFilterContext.RegisterSortFilterExpressionScope(container, scopeObj, isSortFilterExpressionScope);
			}

			internal void ProcessUserSortForTarget(IHierarchyObj target, ref DataRowList dataRows, bool targetForNonDetailSort)
			{
				this.m_userSortFilterContext.ProcessUserSortForTarget(this.m_reportObjectModel, this.m_reportRuntime, target, ref dataRows, targetForNonDetailSort);
			}

			internal ProcessingMessageList RegisterComparisonErrorForSortFilterEvent(string propertyName)
			{
				Global.Tracer.Assert(null != this.m_userSortFilterContext.CurrentSortFilterEventSource, "(null != m_userSortFilterContext.CurrentSortFilterEventSource)");
				this.m_errorContext.Register(ProcessingErrorCode.rsComparisonError, Severity.Error, this.m_userSortFilterContext.CurrentSortFilterEventSource.ObjectType, this.m_userSortFilterContext.CurrentSortFilterEventSource.Name, propertyName);
				return this.m_errorContext.Messages;
			}

			internal void FirstPassPostProcess()
			{
				do
				{
					this.ProcessDataRegionsWithSpecialFilters();
				}
				while (this.m_userSortFilterContext.ProcessUserSort(this));
			}

			internal VariantList[] GetScopeValues(GroupingList containingScopes, IScope containingScope)
			{
				VariantList[] array = null;
				if (containingScopes != null && 0 < containingScopes.Count)
				{
					array = new VariantList[containingScopes.Count];
					int num = 0;
					containingScope.GetScopeValues(null, array, ref num);
				}
				return array;
			}

			internal int CreateUniqueName()
			{
				return this.m_commonInfo.CreateUniqueName();
			}

			internal int CreateIDForSubreport()
			{
				return this.m_commonInfo.CreateIDForSubreport();
			}

			internal int GetLastIDForReport()
			{
				return this.m_commonInfo.GetLastIDForReport();
			}

			internal bool GetResource(string path, out byte[] resource, out string mimeType)
			{
				if (this.m_commonInfo.GetResourceCallback != null)
				{
					bool flag = default(bool);
					bool flag2 = default(bool);
					this.m_commonInfo.GetResourceCallback.GetResource(this.m_reportContext, path, out resource, out mimeType, out flag, out flag2);
					if (flag)
					{
						this.ErrorContext.Register(ProcessingErrorCode.rsWarningFetchingExternalImages, Severity.Warning, ObjectType.Report, null, null);
					}
					return true;
				}
				resource = null;
				mimeType = null;
				return false;
			}
		}

		internal sealed class ReportProcessingContext : ProcessingContext
		{
			private RuntimeDataSourceInfoCollection m_dataSourceInfos;

			internal RuntimeDataSourceInfoCollection DataSourceInfos
			{
				get
				{
					return this.m_dataSourceInfos;
				}
			}

			internal ReportProcessingContext(string chartName, RuntimeDataSourceInfoCollection dataSourceInfos, string requestUserName, CultureInfo userLanguage, SubReportCallback subReportCallback, ICatalogItemContext reportContext, Report report, ErrorContext errorContext, CreateReportChunk createReportChunkCallback, IGetResource getResourceCallback, ExecutionType interactiveExecution, DateTime executionTime, UserProfileState allowUserProfileState, bool isHistorySnapshot, bool snapshotProcessing, bool processWithCachedData, GetReportChunk getChunkCallback, CreateReportChunk cacheDataCallback, IProcessingDataExtensionConnection dataExtensionConnection, ReportRuntimeSetup reportRuntimeSetup, IJobContext jobContext, IExtensionFactory extFactory, IDataProtection dataProtection)
				: base(chartName, requestUserName, userLanguage, subReportCallback, reportContext, report, errorContext, createReportChunkCallback, getResourceCallback, interactiveExecution, executionTime, allowUserProfileState, isHistorySnapshot, snapshotProcessing, processWithCachedData, getChunkCallback, cacheDataCallback, reportRuntimeSetup, jobContext, extFactory, dataExtensionConnection, dataProtection)
			{
				this.m_dataSourceInfos = dataSourceInfos;
			}

			internal ReportProcessingContext(ProcessingContext parentContext, RuntimeDataSourceInfoCollection dataSourceInfos, string requestUserName, CultureInfo userlanguage, ICatalogItemContext reportContext, ErrorContext errorContext, ExecutionType interactiveExecution, DateTime executionTime, UserProfileState allowUserProfileState, bool snapshotProcessing, IProcessingDataExtensionConnection dataExtensionConnection, ReportRuntimeSetup reportRuntimeSetup, IJobContext jobContext, IExtensionFactory extFactory, IDataProtection dataProtection)
				: base(parentContext, requestUserName, userlanguage, reportContext, errorContext, interactiveExecution, executionTime, allowUserProfileState, snapshotProcessing, reportRuntimeSetup, jobContext, extFactory, dataExtensionConnection, dataProtection)
			{
				this.m_dataSourceInfos = dataSourceInfos;
			}

			private ReportProcessingContext(SubReport subReport, ErrorContext errorContext, ReportProcessingContext copy, int subReportUniqueName)
				: base(subReport, errorContext, copy, subReportUniqueName)
			{
				this.m_dataSourceInfos = copy.DataSourceInfos;
			}

			private ReportProcessingContext(ReportProcessingContext copy)
				: base(copy)
			{
				this.m_dataSourceInfos = copy.DataSourceInfos;
			}

			internal override ProcessingContext ParametersContext(ICatalogItemContext reportContext, ProcessingErrorContext errorContext)
			{
				return new ReportProcessingContext(this, this.DataSourceInfos, base.RequestUserName, base.UserLanguage, reportContext, errorContext, base.InteractiveExecution, base.ExecutionTime, base.AllowUserProfileState, base.SnapshotProcessing, base.DataExtensionConnection, base.ReportRuntimeSetup, base.JobContext, base.ExtFactory, base.DataProtection);
			}

			internal override ProcessingContext SubReportContext(SubReport subReport, int subReportDataSetUniqueName, ProcessingErrorContext subReportErrorContext)
			{
				return new ReportProcessingContext(subReport, subReportErrorContext, this, subReportDataSetUniqueName);
			}

			internal override ProcessingContext CloneContext(ProcessingContext context)
			{
				return new ReportProcessingContext((ReportProcessingContext)context);
			}
		}

		internal sealed class Pagination
		{
			private const double MINIMUM_START_ON_PAGE = 12.7;

			private double m_currentPageHeight;

			private double m_pageMaxHeight;

			private int m_ignorePageBreak;

			private int m_ignoreHeight;

			internal double PageHeight
			{
				get
				{
					return this.m_pageMaxHeight;
				}
			}

			internal double CurrentPageHeight
			{
				get
				{
					return this.m_currentPageHeight;
				}
			}

			internal bool IgnorePageBreak
			{
				get
				{
					return 0 != this.m_ignorePageBreak;
				}
			}

			internal bool IgnoreHeight
			{
				get
				{
					return 0 != this.m_ignoreHeight;
				}
			}

			internal Pagination(double pageMaxHeight)
			{
				this.m_pageMaxHeight = pageMaxHeight;
			}

			internal void EnterIgnorePageBreak(Visibility visibility, bool ignoreAlways)
			{
				if (!ignoreAlways && AspNetCore.ReportingServices.ReportRendering.SharedHiddenState.Never == Visibility.GetSharedHidden(visibility))
				{
					return;
				}
				this.m_ignorePageBreak++;
			}

			internal void LeaveIgnorePageBreak(Visibility visibility, bool ignoreAlways)
			{
				if (ignoreAlways || AspNetCore.ReportingServices.ReportRendering.SharedHiddenState.Never != Visibility.GetSharedHidden(visibility))
				{
					this.m_ignorePageBreak--;
				}
				Global.Tracer.Assert(0 <= this.m_ignorePageBreak, "(0 <= m_ignorePageBreak)");
			}

			internal void EnterIgnoreHeight(bool startHidden)
			{
				if (startHidden)
				{
					this.m_ignoreHeight++;
				}
			}

			internal void LeaveIgnoreHeight(bool startHidden)
			{
				if (startHidden)
				{
					this.m_ignoreHeight--;
				}
				Global.Tracer.Assert(0 <= this.m_ignoreHeight, "(0 <= m_ignoreHeight)");
			}

			internal void CopyPaginationInfo(Pagination pagination)
			{
				this.m_ignoreHeight = pagination.m_ignoreHeight;
				this.m_ignorePageBreak = pagination.m_ignorePageBreak;
			}

			internal bool CalculateSoftPageBreak(ReportItem reportItem, double itemHeight, double distanceBeforeOrAfter, bool ignoreSoftPageBreak)
			{
				return this.CalculateSoftPageBreak(reportItem, itemHeight, distanceBeforeOrAfter, ignoreSoftPageBreak, this.PageBreakAtStart(reportItem));
			}

			internal bool CalculateSoftPageBreak(ReportItem reportItem, double itemHeight, double distanceBeforeOrAfter, bool ignoreSoftPageBreak, bool logicalPageBreak)
			{
				if (!this.IgnorePageBreak && logicalPageBreak)
				{
					if (0.0 == this.m_currentPageHeight)
					{
						return false;
					}
					this.SetCurrentPageHeight(reportItem, 0.0);
					return true;
				}
				if (this.IgnoreHeight)
				{
					return false;
				}
				if (reportItem != null)
				{
					this.ComputeReportItemTrueTop(reportItem);
				}
				this.m_currentPageHeight += itemHeight + distanceBeforeOrAfter;
				if (!this.IgnorePageBreak && this.m_currentPageHeight > this.m_pageMaxHeight && !ignoreSoftPageBreak)
				{
					this.SetCurrentPageHeight(reportItem, 0.0);
					return true;
				}
				return false;
			}

			internal void ProcessEndPage(IPageItem riInstance, ReportItem reportItem, bool pageBreakAtEnd, bool childrenOnThisPage)
			{
				riInstance.StartPage = reportItem.StartPage;
				riInstance.EndPage = reportItem.EndPage;
				if (!(reportItem is List))
				{
					this.LeaveIgnoreHeight(reportItem.StartHidden);
				}
				reportItem.BottomInEndPage = this.m_currentPageHeight;
				if (reportItem.Parent != null && reportItem.EndPage > reportItem.Parent.EndPage)
				{
					reportItem.Parent.EndPage = reportItem.EndPage;
					reportItem.Parent.BottomInEndPage = reportItem.BottomInEndPage;
					if (reportItem.Parent is List)
					{
						((List)reportItem.Parent).ContentStartPage = reportItem.EndPage;
					}
				}
				if (!this.IgnorePageBreak && pageBreakAtEnd)
				{
					if (!this.IgnoreHeight)
					{
						this.AddToCurrentPageHeight(reportItem, this.m_pageMaxHeight + 1.0);
					}
					reportItem.ShareMyLastPage = !childrenOnThisPage;
				}
				else if (reportItem.Parent != null)
				{
					reportItem.Parent.ShareMyLastPage = true;
				}
			}

			internal void ProcessEndGroupPage(double distance, bool pageBreakAtEnd, ReportItem parent, bool childrenOnThisPage, bool startHidden)
			{
				this.LeaveIgnoreHeight(startHidden);
				if (!this.IgnoreHeight)
				{
					this.m_currentPageHeight += distance;
				}
				if (!this.IgnorePageBreak && pageBreakAtEnd)
				{
					if (!this.IgnoreHeight)
					{
						this.m_currentPageHeight += this.m_pageMaxHeight + 1.0;
					}
					if (parent != null)
					{
						parent.ShareMyLastPage = !childrenOnThisPage;
					}
				}
				else if (parent != null)
				{
					parent.ShareMyLastPage = true;
				}
				if (parent != null)
				{
					parent.BottomInEndPage = this.m_currentPageHeight;
				}
			}

			internal void SetReportItemStartPage(ReportItem reportItem, bool softPageAtStart)
			{
				ReportItemCollection reportItemCollection = null;
				int num = reportItem.StartPage;
				ReportItem parent = reportItem.Parent;
				if (parent != null)
				{
					if (parent is Rectangle)
					{
						reportItemCollection = ((Rectangle)parent).ReportItems;
					}
					else if (parent is List)
					{
						reportItemCollection = ((List)parent).ReportItems;
						num = ((List)parent).ContentStartPage;
					}
					else if (parent is Table)
					{
						num = ((Table)parent).CurrentPage;
					}
					else if (parent is Matrix)
					{
						num = ((Matrix)parent).CurrentPage;
					}
					else if (parent is Report)
					{
						reportItemCollection = ((Report)parent).ReportItems;
					}
					if (-1 == num)
					{
						num = parent.StartPage;
					}
				}
				bool flag = false;
				bool flag2 = false;
				if (reportItemCollection != null && reportItem.SiblingAboveMe != null)
				{
					for (int i = 0; i < reportItem.SiblingAboveMe.Count; i++)
					{
						ReportItem reportItem2 = reportItemCollection[reportItem.SiblingAboveMe[i]];
						int num2 = reportItem2.EndPage;
						if (!reportItemCollection.IsReportItemComputed(reportItem.SiblingAboveMe[i]))
						{
							flag = true;
						}
						double num3 = reportItem2.TopValue + reportItem2.HeightValue;
						bool flag3 = num3 > reportItem.TopValue + 0.0009;
						if (flag3)
						{
							num2 = reportItem2.StartPage;
						}
						if (num2 > num)
						{
							flag2 = false;
						}
						if (!flag3 && this.PageBreakAtEnd(reportItem2))
						{
							flag2 = (num2 >= num);
						}
						num = Math.Max(num, num2);
					}
				}
				else if (reportItem.Parent != null)
				{
					num = Math.Max(num, reportItem.Parent.StartPage);
				}
				bool flag4 = this.PageBreakAtStart(reportItem);
				if (flag2 || softPageAtStart || this.CanMoveToNextPage(flag4))
				{
					num++;
					this.m_currentPageHeight = 0.0;
				}
				if (flag && !this.IgnoreHeight && 0.0 == this.m_currentPageHeight)
				{
					this.m_currentPageHeight += 1.0;
					if (flag4)
					{
						if (flag2)
						{
							num++;
						}
						else if (!softPageAtStart)
						{
							num++;
						}
					}
				}
				reportItem.StartPage = num;
				reportItem.EndPage = num;
				if ((reportItem is TextBox || reportItem is Image || reportItem is Chart) && !this.IgnoreHeight && 0.0 == this.m_currentPageHeight)
				{
					this.m_currentPageHeight += 1.0;
				}
				reportItem.TopInStartPage = this.m_currentPageHeight;
				reportItem.BottomInEndPage = this.m_currentPageHeight;
			}

			internal bool PageBreakAtEnd(ReportItem reportItem)
			{
				if (reportItem.SoftPageBreak)
				{
					return true;
				}
				if (reportItem is List && ((List)reportItem).PropagatedPageBreakAtEnd)
				{
					return true;
				}
				if (reportItem is Table && ((Table)reportItem).PropagatedPageBreakAtEnd)
				{
					return true;
				}
				if (reportItem is Matrix && ((Matrix)reportItem).PropagatedPageBreakAtEnd)
				{
					return true;
				}
				return this.CheckPageBreak(reportItem, false);
			}

			internal bool PageBreakAtStart(ReportItem reportItem)
			{
				if (reportItem is List && ((List)reportItem).PropagatedPageBreakAtStart)
				{
					return true;
				}
				if (reportItem is Table && ((Table)reportItem).PropagatedPageBreakAtStart)
				{
					return true;
				}
				if (reportItem is Matrix && ((Matrix)reportItem).PropagatedPageBreakAtStart)
				{
					return true;
				}
				return this.CheckPageBreak(reportItem, true);
			}

			private bool CheckPageBreak(ReportItem reportItem, bool start)
			{
				if (this.IgnorePageBreak)
				{
					return false;
				}
				if (!(reportItem is DataRegion) && !(reportItem is Rectangle))
				{
					return false;
				}
				IPageBreakItem pageBreakItem = (IPageBreakItem)reportItem;
				if (pageBreakItem != null)
				{
					if (pageBreakItem.IgnorePageBreaks())
					{
						return false;
					}
					return pageBreakItem.HasPageBreaks(start);
				}
				return false;
			}

			internal bool CanMoveToNextPage(bool pageBreakAtStart)
			{
				if (this.IgnorePageBreak)
				{
					return false;
				}
				if (pageBreakAtStart && 0.0 != this.m_currentPageHeight)
				{
					return true;
				}
				return false;
			}

			internal void ProcessListRenderingPages(ListInstance listInstance, List listDef)
			{
				RenderingPagesRangesList childrenStartAndEndPages = listInstance.ChildrenStartAndEndPages;
				Global.Tracer.Assert(null != childrenStartAndEndPages, "(null != listPagesList)");
				bool childrenOnThisPage = false;
				if (listDef.Grouping == null)
				{
					if (listInstance.NumberOfContentsOnThisPage > 0)
					{
						RenderingPagesRanges renderingPagesRanges = default(RenderingPagesRanges);
						renderingPagesRanges.StartRow = listInstance.ListContents.Count - listInstance.NumberOfContentsOnThisPage;
						renderingPagesRanges.NumberOfDetails = listInstance.NumberOfContentsOnThisPage;
						childrenStartAndEndPages.Add(renderingPagesRanges);
						childrenOnThisPage = true;
					}
					if (childrenStartAndEndPages != null && childrenStartAndEndPages.Count > 0)
					{
						listDef.EndPage = listDef.StartPage + childrenStartAndEndPages.Count - 1;
					}
					else
					{
						listDef.EndPage = listDef.StartPage;
					}
				}
				else if (childrenStartAndEndPages.Count > 0)
				{
					listDef.StartPage = childrenStartAndEndPages[0].StartPage;
					listDef.EndPage = childrenStartAndEndPages[listInstance.ListContents.Count - 1].EndPage;
					childrenOnThisPage = true;
				}
				else
				{
					listDef.EndPage = listDef.StartPage;
				}
				this.ProcessEndPage(listInstance, listDef, this.PageBreakAtEnd(listDef), childrenOnThisPage);
			}

			internal void InitProcessTableRenderingPages(TableInstance tableInstance, Table table)
			{
				double headerHeightValue = table.HeaderHeightValue;
				if (!this.IgnorePageBreak && headerHeightValue < this.m_pageMaxHeight && headerHeightValue + this.m_currentPageHeight > this.m_pageMaxHeight)
				{
					this.SetCurrentPageHeight(table, 0.0);
					table.StartPage++;
					((IPageItem)tableInstance).StartPage = table.StartPage;
					tableInstance.CurrentPage++;
					table.CurrentPage = tableInstance.CurrentPage;
				}
				else if (!this.IgnoreHeight)
				{
					this.AddToCurrentPageHeight(table, headerHeightValue);
				}
			}

			internal void InitProcessingTableGroup(TableInstance tableInstance, Table table, TableGroupInstance tableGroupInstance, TableGroup tableGroup, ref RenderingPagesRanges renderingPagesRanges, bool ignorePageBreakAtStart)
			{
				this.EnterIgnorePageBreak(tableGroup.Visibility, false);
				tableGroup.StartPage = tableInstance.CurrentPage;
				if (tableGroup.InnerHierarchy == null && table.TableDetail == null)
				{
					double headerHeightValue = tableGroup.HeaderHeightValue;
					if (!this.IgnorePageBreak && headerHeightValue + this.m_currentPageHeight > this.m_pageMaxHeight)
					{
						this.SetCurrentPageHeight(table, 0.0);
						tableInstance.CurrentPage++;
						table.CurrentPage = tableInstance.CurrentPage;
					}
					else if (!this.IgnoreHeight)
					{
						this.AddToCurrentPageHeight(table, headerHeightValue);
					}
				}
				bool flag = false;
				if (!ignorePageBreakAtStart)
				{
					flag = this.CalculateSoftPageBreak(null, 0.0, 0.0, false, tableGroup.PropagatedPageBreakAtStart || tableGroup.Grouping.PageBreakAtEnd);
					if (!this.IgnorePageBreak && flag)
					{
						this.SetCurrentPageHeight(table, 0.0);
						tableInstance.CurrentPage++;
						table.CurrentPage = tableInstance.CurrentPage;
					}
				}
				renderingPagesRanges.StartPage = tableInstance.CurrentPage;
			}

			internal void ProcessTableDetails(Table tableDef, TableDetailInstance detailInstance, IList detailInstances, ref double detailHeightValue, TableRowList rowDefs, RenderingPagesRangesList pagesList, ref int numberOfChildrenOnThisPage)
			{
				if (-1.0 == detailHeightValue)
				{
					detailHeightValue = tableDef.DetailHeightValue;
				}
				if (!this.IgnoreHeight)
				{
					this.AddToCurrentPageHeight(tableDef, detailHeightValue);
				}
				if (!this.IgnorePageBreak && this.m_currentPageHeight >= this.m_pageMaxHeight && numberOfChildrenOnThisPage > 0)
				{
					RenderingPagesRanges renderingPagesRanges = default(RenderingPagesRanges);
					renderingPagesRanges.StartRow = detailInstances.Count - numberOfChildrenOnThisPage;
					renderingPagesRanges.NumberOfDetails = numberOfChildrenOnThisPage;
					this.SetCurrentPageHeight(tableDef, 0.0);
					tableDef.CurrentPage++;
					pagesList.Add(renderingPagesRanges);
					numberOfChildrenOnThisPage = 1;
				}
				else
				{
					numberOfChildrenOnThisPage++;
				}
			}

			internal void ProcessTableRenderingPages(TableInstance tableInstance, Table reportItem)
			{
				this.ProcessEndPage(tableInstance, reportItem, this.PageBreakAtEnd(reportItem), tableInstance.NumberOfChildrenOnThisPage > 0);
				reportItem.EndPage = tableInstance.CurrentPage;
				reportItem.CurrentPage = tableInstance.CurrentPage;
			}

			internal void ComputeReportItemTrueTop(ReportItem reportItem)
			{
				ReportItemCollection reportItemCollection = null;
				int num = reportItem.StartPage;
				ReportItem parent = reportItem.Parent;
				double num2 = 0.0;
				if (parent != null)
				{
					if (parent is Rectangle)
					{
						reportItemCollection = ((Rectangle)parent).ReportItems;
						num2 = parent.TopInStartPage;
					}
					else if (parent is List)
					{
						reportItemCollection = ((List)parent).ReportItems;
						num = ((List)parent).ContentStartPage;
						num2 = parent.BottomInEndPage;
					}
					else if (parent is Table)
					{
						num = ((Table)parent).CurrentPage;
						num2 = parent.BottomInEndPage;
					}
					else if (parent is Matrix)
					{
						num = ((Matrix)parent).CurrentPage;
						num2 = parent.BottomInEndPage;
					}
					else if (parent is Report)
					{
						reportItemCollection = ((Report)parent).ReportItems;
						num2 = parent.TopInStartPage;
					}
					if (-1 == num)
					{
						num = parent.StartPage;
					}
				}
				if (reportItemCollection != null && reportItem.SiblingAboveMe != null)
				{
					for (int i = 0; i < reportItem.SiblingAboveMe.Count; i++)
					{
						ReportItem reportItem2 = reportItemCollection[reportItem.SiblingAboveMe[i]];
						int num3 = reportItem2.EndPage;
						double num4 = reportItem2.BottomInEndPage;
						double num5 = reportItem2.TopValue + reportItem2.HeightValue;
						if (num5 > reportItem.TopValue + 0.0009)
						{
							num3 = reportItem2.StartPage;
							num4 = reportItem2.TopInStartPage;
						}
						if (num3 > num)
						{
							num = num3;
							num2 = num4;
						}
						else if (num3 == num)
						{
							num2 = Math.Max(num2, num4);
						}
					}
				}
				this.m_currentPageHeight = num2;
				reportItem.TopInStartPage = num2;
			}

			internal void AddToCurrentPageHeight(ReportItem reportItem, double distance)
			{
				this.m_currentPageHeight += distance;
				if (reportItem != null)
				{
					reportItem.BottomInEndPage = this.m_currentPageHeight;
				}
			}

			internal void SetCurrentPageHeight(ReportItem reportItem, double distance)
			{
				this.m_currentPageHeight = distance;
				if (reportItem != null)
				{
					reportItem.BottomInEndPage = this.m_currentPageHeight;
				}
			}

			internal bool ShouldItemMoveToChildStartPage(ReportItem reportItem)
			{
				List list = reportItem as List;
				if (list == null)
				{
					return false;
				}
				if (this.IgnoreHeight && this.m_pageMaxHeight < 25.4)
				{
					return false;
				}
				if (list.KeepWithChildFirstPage == 0)
				{
					return false;
				}
				if (-1 == list.KeepWithChildFirstPage)
				{
					ReportItemCollection reportItems = list.ReportItems;
					int keepWithChildFirstPage = 0;
					if (reportItems != null && reportItems.Count > 0)
					{
						ReportItem reportItem2 = reportItems[0];
						if (!this.PageBreakAtStart(reportItem2) && reportItem2.TopValue < 12.7)
						{
							keepWithChildFirstPage = 1;
						}
					}
					list.KeepWithChildFirstPage = keepWithChildFirstPage;
				}
				if (1 == list.KeepWithChildFirstPage)
				{
					return true;
				}
				return false;
			}

			internal int GetTextBoxStartPage(TextBox textBox)
			{
				if (-1 == textBox.StartPage)
				{
					Global.Tracer.Assert(textBox.Parent is Table || textBox.Parent is Matrix || textBox.Parent is CustomReportItem);
					if (textBox.Parent is Table)
					{
						return ((Table)textBox.Parent).CurrentPage;
					}
					if (textBox.Parent is Matrix)
					{
						return ((Matrix)textBox.Parent).CurrentPage;
					}
				}
				return textBox.StartPage;
			}
		}

		internal sealed class NavigationInfo
		{
			internal sealed class DocumentMapNodeList : ArrayList
			{
				internal new DocumentMapNode this[int index]
				{
					get
					{
						return (DocumentMapNode)base[index];
					}
					set
					{
						base[index] = value;
					}
				}

				internal DocumentMapNodeList()
				{
				}
			}

			internal sealed class DocumentMapNodeLists : ArrayList
			{
				internal new DocumentMapNodeList this[int index]
				{
					get
					{
						return (DocumentMapNodeList)base[index];
					}
					set
					{
						base[index] = value;
					}
				}

				internal DocumentMapNodeLists()
				{
				}
			}

			private DocumentMapNodeLists m_reportDocumentMapChildren;

			private string m_currentLabel;

			private ArrayList m_matrixColumnDocumentMaps = new ArrayList();

			private int m_inMatrixColumn = -1;

			private BookmarksHashtable m_bookmarksInfo;

			internal DocumentMapNodeLists DocumentMapChildren
			{
				get
				{
					return this.CurrentDocumentMapChildren;
				}
			}

			internal DocumentMapNodeList CurrentDocumentMapSiblings
			{
				get
				{
					DocumentMapNodeLists currentDocumentMapChildren = this.CurrentDocumentMapChildren;
					if (currentDocumentMapChildren != null && 0 < currentDocumentMapChildren.Count)
					{
						return currentDocumentMapChildren[currentDocumentMapChildren.Count - 1];
					}
					return null;
				}
			}

			internal string CurrentLabel
			{
				get
				{
					return this.m_currentLabel;
				}
			}

			internal BookmarksHashtable BookmarksInfo
			{
				get
				{
					return this.m_bookmarksInfo;
				}
				set
				{
					this.m_bookmarksInfo = value;
				}
			}

			private DocumentMapNodeLists CurrentDocumentMapChildren
			{
				get
				{
					if (0 <= this.m_inMatrixColumn)
					{
						return (DocumentMapNodeLists)this.m_matrixColumnDocumentMaps[this.m_inMatrixColumn];
					}
					return this.m_reportDocumentMapChildren;
				}
				set
				{
					if (0 <= this.m_inMatrixColumn)
					{
						this.m_matrixColumnDocumentMaps[this.m_inMatrixColumn] = value;
					}
					else
					{
						this.m_reportDocumentMapChildren = value;
					}
				}
			}

			private DocumentMapNodeLists CurrentMatrixColumnDocumentMapChildren
			{
				get
				{
					if (this.m_matrixColumnDocumentMaps != null && this.m_matrixColumnDocumentMaps.Count != 0)
					{
						if (0 <= this.m_inMatrixColumn)
						{
							return (DocumentMapNodeLists)this.m_matrixColumnDocumentMaps[this.m_inMatrixColumn];
						}
						return (DocumentMapNodeLists)this.m_matrixColumnDocumentMaps[0];
					}
					return null;
				}
				set
				{
					if (this.m_matrixColumnDocumentMaps != null && 0 < this.m_matrixColumnDocumentMaps.Count)
					{
						if (0 <= this.m_inMatrixColumn)
						{
							this.m_matrixColumnDocumentMaps[this.m_inMatrixColumn] = value;
						}
						else
						{
							this.m_matrixColumnDocumentMaps[0] = value;
						}
					}
				}
			}

			internal void GetCurrentDocumentMapPosition(out int siblingIndex, out int nodeIndex)
			{
				siblingIndex = 0;
				nodeIndex = 0;
				DocumentMapNodeLists currentDocumentMapChildren = this.CurrentDocumentMapChildren;
				if (currentDocumentMapChildren != null && 0 < currentDocumentMapChildren.Count)
				{
					siblingIndex = currentDocumentMapChildren.Count - 1;
					DocumentMapNodeList documentMapNodeList = currentDocumentMapChildren[siblingIndex];
					if (documentMapNodeList != null)
					{
						nodeIndex = documentMapNodeList.Count;
					}
				}
			}

			internal void EnterMatrixColumn()
			{
				this.m_inMatrixColumn++;
				if (this.m_matrixColumnDocumentMaps == null)
				{
					this.m_matrixColumnDocumentMaps = new ArrayList();
				}
				if (this.m_matrixColumnDocumentMaps.Count <= this.m_inMatrixColumn)
				{
					Global.Tracer.Assert(this.m_matrixColumnDocumentMaps.Count == this.m_inMatrixColumn, "(m_matrixColumnDocumentMaps.Count == m_inMatrixColumn)");
					this.m_matrixColumnDocumentMaps.Add(null);
				}
			}

			internal void LeaveMatrixColumn()
			{
				this.m_inMatrixColumn--;
				Global.Tracer.Assert(-1 <= this.m_inMatrixColumn, "(-1 <= m_inMatrixColumn)");
			}

			internal void InsertMatrixColumnDocumentMap(int siblingIndex, int nodeIndex)
			{
				DocumentMapNodeLists currentMatrixColumnDocumentMapChildren = this.CurrentMatrixColumnDocumentMapChildren;
				if (currentMatrixColumnDocumentMapChildren != null && 0 < currentMatrixColumnDocumentMapChildren.Count)
				{
					DocumentMapNodeLists currentDocumentMapChildren = this.CurrentDocumentMapChildren;
					DocumentMapNodeList documentMapNodeList = null;
					if (currentDocumentMapChildren != null && 0 <= currentDocumentMapChildren.Count)
					{
						documentMapNodeList = currentDocumentMapChildren[siblingIndex];
					}
					if (documentMapNodeList == null)
					{
						if (currentDocumentMapChildren == null)
						{
							DocumentMapNodeLists documentMapNodeLists2 = this.CurrentDocumentMapChildren = currentMatrixColumnDocumentMapChildren;
							currentDocumentMapChildren = documentMapNodeLists2;
						}
						else
						{
							DocumentMapNodeLists documentMapNodeLists3 = currentDocumentMapChildren;
							documentMapNodeList = (documentMapNodeLists3[siblingIndex] = currentMatrixColumnDocumentMapChildren[0]);
						}
					}
					else
					{
						Global.Tracer.Assert(null != currentDocumentMapChildren, "(null != currentDocMap)");
						Global.Tracer.Assert(0 <= nodeIndex && nodeIndex <= documentMapNodeList.Count, "(0 <= nodeIndex && nodeIndex <= siblings.Count)");
						documentMapNodeList.InsertRange(nodeIndex, currentMatrixColumnDocumentMapChildren[0]);
					}
					this.CurrentMatrixColumnDocumentMapChildren = null;
				}
			}

			internal void AppendNavigationInfo(string label, NavigationInfo navigationInfo, int startPage)
			{
				DocumentMapNodeLists currentDocumentMapChildren = this.CurrentDocumentMapChildren;
				DocumentMapNodeLists currentDocumentMapChildren2 = navigationInfo.CurrentDocumentMapChildren;
				if (currentDocumentMapChildren2 != null && 0 < currentDocumentMapChildren2.Count)
				{
					navigationInfo.UpdateDocumentMapChildrenPage(startPage);
					if (label == null)
					{
						if (currentDocumentMapChildren == null)
						{
							DocumentMapNodeLists documentMapNodeLists2 = this.CurrentDocumentMapChildren = currentDocumentMapChildren2;
							currentDocumentMapChildren = documentMapNodeLists2;
						}
						else
						{
							currentDocumentMapChildren.AddRange(currentDocumentMapChildren2[0]);
						}
					}
					else
					{
						this.EnterDocumentMapChildren();
						currentDocumentMapChildren[currentDocumentMapChildren.Count - 1] = currentDocumentMapChildren2[0];
					}
				}
				if (navigationInfo.m_bookmarksInfo != null && 0 < navigationInfo.m_bookmarksInfo.Count)
				{
					if (this.m_bookmarksInfo == null)
					{
						this.m_bookmarksInfo = new BookmarksHashtable();
					}
					IDictionaryEnumerator enumerator = navigationInfo.m_bookmarksInfo.GetEnumerator();
					while (enumerator.MoveNext())
					{
						string bookmark = (string)enumerator.Key;
						BookmarkInformation bookmarkInformation = (BookmarkInformation)enumerator.Value;
						this.m_bookmarksInfo.Add(bookmark, bookmarkInformation.Page + startPage, bookmarkInformation.Id);
					}
				}
			}

			private void UpdateDocumentMapChildrenPage(int startPage)
			{
				DocumentMapNodeLists currentDocumentMapChildren = this.CurrentDocumentMapChildren;
				if (currentDocumentMapChildren != null)
				{
					for (int i = 0; i < currentDocumentMapChildren.Count; i++)
					{
						DocumentMapNodeList documentMapNodeList = currentDocumentMapChildren[i];
						if (documentMapNodeList != null)
						{
							for (int j = 0; j < documentMapNodeList.Count; j++)
							{
								this.UpdateDocumentMapNodePage(documentMapNodeList[j], startPage);
							}
						}
					}
				}
			}

			private void UpdateDocumentMapNodePage(DocumentMapNode node, int startPage)
			{
				node.Page += startPage;
				if (node.Children != null)
				{
					for (int i = 0; i < node.Children.Length; i++)
					{
						this.UpdateDocumentMapNodePage(node.Children[i], startPage);
					}
				}
			}

			internal void EnterDocumentMapChildren()
			{
				DocumentMapNodeLists documentMapNodeLists = this.CurrentDocumentMapChildren;
				if (documentMapNodeLists == null)
				{
					DocumentMapNodeLists documentMapNodeLists3 = this.CurrentDocumentMapChildren = new DocumentMapNodeLists();
					documentMapNodeLists = documentMapNodeLists3;
					documentMapNodeLists.Add(null);
				}
				documentMapNodeLists.Add(null);
			}

			internal void AddToDocumentMap(int uniqueName, bool isContainer, int startPage, string label)
			{
				if (label != null)
				{
					DocumentMapNodeLists documentMapNodeLists = this.CurrentDocumentMapChildren;
					if (documentMapNodeLists == null)
					{
						DocumentMapNodeLists documentMapNodeLists3 = this.CurrentDocumentMapChildren = new DocumentMapNodeLists();
						documentMapNodeLists = documentMapNodeLists3;
						documentMapNodeLists.Add(null);
					}
					DocumentMapNodeList children = null;
					int num = documentMapNodeLists.Count - 1;
					if (isContainer)
					{
						Global.Tracer.Assert(1 < documentMapNodeLists.Count, "(1 < currentDocMap.Count)");
						children = documentMapNodeLists[documentMapNodeLists.Count - 1];
						num--;
					}
					DocumentMapNodeList documentMapNodeList = documentMapNodeLists[num];
					if (documentMapNodeList == null)
					{
						DocumentMapNodeLists documentMapNodeLists4 = documentMapNodeLists;
						int index = num;
						documentMapNodeList = (documentMapNodeLists4[index] = new DocumentMapNodeList());
					}
					documentMapNodeList.Add(new DocumentMapNode(uniqueName.ToString(CultureInfo.InvariantCulture), label, startPage, children));
					if (isContainer)
					{
						documentMapNodeLists.RemoveAt(documentMapNodeLists.Count - 1);
					}
				}
			}

			internal string RegisterLabel(VariantResult labelResult)
			{
				string text = null;
				if (labelResult.ErrorOccurred)
				{
					text = RPRes.rsExpressionErrorValue;
				}
				else if (labelResult.Value != null)
				{
					if (labelResult.Value is string)
					{
						text = (string)labelResult.Value;
					}
					else
					{
						try
						{
							text = labelResult.Value.ToString();
						}
						catch (Exception ex)
						{
							if (AsynchronousExceptionDetection.IsStoppingException(ex))
							{
								throw;
							}
							Global.Tracer.Trace(TraceLevel.Verbose, ex.Message);
						}
					}
				}
				this.m_currentLabel = text;
				return text;
			}

			internal void ProcessBookmark(ProcessingContext processingContext, ReportItem reportItem, ReportItemInstance riInstance, ReportItemInstanceInfo riInstanceInfo)
			{
				string bookmark = processingContext.ReportRuntime.EvaluateReportItemBookmarkExpression(reportItem);
				this.ProcessBookmark(reportItem, riInstance, riInstanceInfo, bookmark);
			}

			internal void ProcessBookmark(ReportItem reportItem, ReportItemInstance riInstance, ReportItemInstanceInfo riInstanceInfo, string bookmark)
			{
				if (bookmark != null)
				{
					riInstanceInfo.Bookmark = bookmark;
					if (this.m_bookmarksInfo == null)
					{
						this.m_bookmarksInfo = new BookmarksHashtable();
					}
					this.m_bookmarksInfo.Add(bookmark, reportItem.StartPage, riInstance.UniqueName.ToString(CultureInfo.InvariantCulture));
				}
			}

			internal void ProcessBookmark(string bookmark, int startPage, int uniqueName)
			{
				if (bookmark != null)
				{
					if (this.m_bookmarksInfo == null)
					{
						this.m_bookmarksInfo = new BookmarksHashtable();
					}
					this.m_bookmarksInfo.Add(bookmark, startPage, uniqueName.ToString(CultureInfo.InvariantCulture));
				}
			}
		}

		internal sealed class RuntimeGroupingObj
		{
			internal enum GroupingTypes
			{
				None,
				Hash,
				Sort
			}

			private RuntimeHierarchyObj m_owner;

			private GroupingTypes m_type;

			private Hashtable m_hashtable;

			private BTreeNode m_tree;

			private ParentInformation m_parentInfo;

			internal BTreeNode Tree
			{
				get
				{
					return this.m_tree;
				}
				set
				{
					this.m_tree = value;
				}
			}

			internal GroupingTypes GroupingType
			{
				set
				{
					Global.Tracer.Assert(GroupingTypes.None == value, "(GroupingTypes.None == value)");
					this.m_type = value;
					this.m_tree = null;
					this.m_hashtable = null;
				}
			}

			internal RuntimeGroupingObj(RuntimeHierarchyObj owner, GroupingTypes type)
			{
				this.m_type = type;
				this.m_owner = owner;
				if (GroupingTypes.Sort == type)
				{
					this.m_tree = new BTreeNode(owner);
				}
				else
				{
					this.m_hashtable = new Hashtable(new ProcessingComparer(this.m_owner.ProcessingContext.CompareInfo, this.m_owner.ProcessingContext.ClrCompareOptions, false));
				}
			}

			internal void NextRow(object keyValue)
			{
				this.NextRow(keyValue, false, null);
			}

			internal void NextRow(object keyValue, bool hasParent, object parentKey)
			{
				if (GroupingTypes.Sort == this.m_type)
				{
					this.m_tree.NextRow(keyValue);
					Global.Tracer.Assert(!hasParent, "(!hasParent)");
				}
				else
				{
					RuntimeHierarchyObj runtimeHierarchyObj = null;
					Global.Tracer.Assert(GroupingTypes.Hash == this.m_type, "(GroupingTypes.Hash == m_type)");
					try
					{
						runtimeHierarchyObj = (RuntimeHierarchyObj)this.m_hashtable[keyValue];
					}
					catch (ReportProcessingException_SpatialTypeComparisonError reportProcessingException_SpatialTypeComparisonError)
					{
						throw new ReportProcessingException(this.m_owner.RegisterSpatialTypeComparisonError(reportProcessingException_SpatialTypeComparisonError.Type));
					}
					catch (ReportProcessingException_ComparisonError e)
					{
						throw new ReportProcessingException(this.m_owner.RegisterComparisonError("GroupExpression", e));
					}
					if (runtimeHierarchyObj != null)
					{
						runtimeHierarchyObj.NextRow();
					}
					else
					{
						runtimeHierarchyObj = new RuntimeHierarchyObj(this.m_owner);
						this.m_hashtable.Add(keyValue, runtimeHierarchyObj);
						runtimeHierarchyObj.NextRow();
						if (hasParent)
						{
							RuntimeHierarchyObj runtimeHierarchyObj2 = null;
							RuntimeGroupLeafObj runtimeGroupLeafObj = null;
							try
							{
								runtimeHierarchyObj2 = (RuntimeHierarchyObj)this.m_hashtable[parentKey];
							}
							catch (ReportProcessingException_SpatialTypeComparisonError reportProcessingException_SpatialTypeComparisonError2)
							{
								throw new ReportProcessingException(this.m_owner.RegisterSpatialTypeComparisonError(reportProcessingException_SpatialTypeComparisonError2.Type));
							}
							catch (ReportProcessingException_ComparisonError e2)
							{
								throw new ReportProcessingException(this.m_owner.RegisterComparisonError("Parent", e2));
							}
							if (runtimeHierarchyObj2 != null)
							{
								Global.Tracer.Assert(null != runtimeHierarchyObj2.HierarchyObjs, "(null != parentHierarchyObj.HierarchyObjs)");
								runtimeGroupLeafObj = (RuntimeGroupLeafObj)runtimeHierarchyObj2.HierarchyObjs[0];
							}
							Global.Tracer.Assert(null != runtimeHierarchyObj.HierarchyObjs, "(null != hierarchyObj.HierarchyObjs)");
							RuntimeGroupLeafObj runtimeGroupLeafObj2 = (RuntimeGroupLeafObj)runtimeHierarchyObj.HierarchyObjs[0];
							bool addToWaitList = true;
							if (runtimeGroupLeafObj == runtimeGroupLeafObj2)
							{
								runtimeGroupLeafObj = null;
								addToWaitList = false;
							}
							this.ProcessChildren(keyValue, runtimeGroupLeafObj, runtimeGroupLeafObj2);
							this.ProcessParent(parentKey, runtimeGroupLeafObj, runtimeGroupLeafObj2, addToWaitList);
						}
					}
				}
			}

			internal void Traverse(ProcessingStages operation, bool ascending)
			{
				if (GroupingTypes.Sort == this.m_type)
				{
					this.m_tree.Traverse(operation, ascending);
				}
				else if (((RuntimeGroupRootObj)this.m_owner).FirstChild != null)
				{
					((RuntimeGroupRootObj)this.m_owner).FirstChild.TraverseAllLeafNodes(operation);
				}
			}

			private void ProcessParent(object parentKey, RuntimeGroupLeafObj parentObj, RuntimeGroupLeafObj childObj, bool addToWaitList)
			{
				if (parentObj != null)
				{
					parentObj.AddChild(childObj);
				}
				else
				{
					((RuntimeGroupRootObj)this.m_owner).AddChild(childObj);
					if (addToWaitList)
					{
						RuntimeGroupLeafObjList runtimeGroupLeafObjList = null;
						if (this.m_parentInfo == null)
						{
							this.m_parentInfo = new ParentInformation();
						}
						else
						{
							runtimeGroupLeafObjList = this.m_parentInfo[parentKey];
						}
						if (runtimeGroupLeafObjList == null)
						{
							runtimeGroupLeafObjList = new RuntimeGroupLeafObjList();
							this.m_parentInfo.Add(parentKey, runtimeGroupLeafObjList);
						}
						runtimeGroupLeafObjList.Add(childObj);
					}
				}
			}

			private void ProcessChildren(object thisKey, RuntimeGroupLeafObj parentObj, RuntimeGroupLeafObj thisObj)
			{
				RuntimeGroupLeafObjList runtimeGroupLeafObjList = null;
				if (this.m_parentInfo != null)
				{
					runtimeGroupLeafObjList = this.m_parentInfo[thisKey];
				}
				if (runtimeGroupLeafObjList != null)
				{
					for (int i = 0; i < runtimeGroupLeafObjList.Count; i++)
					{
						RuntimeGroupLeafObj runtimeGroupLeafObj = runtimeGroupLeafObjList[i];
						bool flag = false;
						RuntimeGroupLeafObj runtimeGroupLeafObj2 = parentObj;
						while (!flag && runtimeGroupLeafObj2 != null)
						{
							if (runtimeGroupLeafObj2 == runtimeGroupLeafObj)
							{
								flag = true;
							}
							runtimeGroupLeafObj2 = (runtimeGroupLeafObj2.Parent as RuntimeGroupLeafObj);
						}
						if (!flag)
						{
							runtimeGroupLeafObj.RemoveFromParent((RuntimeGroupRootObj)this.m_owner);
							thisObj.AddChild(runtimeGroupLeafObj);
						}
					}
					this.m_parentInfo.Remove(thisKey);
				}
			}
		}

		internal sealed class BTreeNode
		{
			private const int BTreeOrder = 3;

			private BTreeNodeTupleList m_tuples;

			private BTreeNode m_parent;

			private int m_indexInParent;

			private IHierarchyObj m_owner;

			internal BTreeNode Parent
			{
				set
				{
					this.m_parent = value;
				}
			}

			internal int IndexInParent
			{
				set
				{
					this.m_indexInParent = value;
				}
			}

			internal BTreeNode(IHierarchyObj owner)
			{
				this.m_owner = owner;
				this.m_tuples = new BTreeNodeTupleList(this, 3);
				BTreeNodeTuple tuple = new BTreeNodeTuple(new BTreeNodeValue(null, owner), null);
				this.m_tuples.Add(tuple);
			}

			internal void NextRow(object keyValue)
			{
				try
				{
					this.SearchAndInsert(keyValue);
				}
				catch (ReportProcessingException_SpatialTypeComparisonError)
				{
					throw new ReportProcessingException(this.m_owner.RegisterComparisonError("SortExpression"));
				}
				catch (ReportProcessingException_ComparisonError)
				{
					throw new ReportProcessingException(this.m_owner.RegisterComparisonError("SortExpression"));
				}
			}

			internal void Traverse(ProcessingStages operation, bool ascending)
			{
				if (ascending)
				{
					for (int i = 0; i < this.m_tuples.Count; i++)
					{
						this.m_tuples[i].Traverse(operation, ascending);
					}
				}
				else
				{
					for (int num = this.m_tuples.Count - 1; num >= 0; num--)
					{
						this.m_tuples[num].Traverse(operation, ascending);
					}
				}
			}

			private void SetFirstChild(BTreeNode child)
			{
				Global.Tracer.Assert(1 <= this.m_tuples.Count, "(1 <= m_tuples.Count)");
				this.m_tuples[0].Child = child;
				if (this.m_tuples[0].Child != null)
				{
					this.m_tuples[0].Child.Parent = this;
					this.m_tuples[0].Child.IndexInParent = 0;
				}
			}

			private void SearchAndInsert(object keyValue)
			{
				int num = -1;
				int i;
				for (i = 1; i < this.m_tuples.Count; i++)
				{
					BTreeNodeTuple bTreeNodeTuple = this.m_tuples[i];
					num = bTreeNodeTuple.Value.CompareTo(keyValue);
					if (num >= 0)
					{
						break;
					}
				}
				if (num == 0)
				{
					this.m_tuples[i].Value.AddRow();
				}
				else if (this.m_tuples[i - 1].Child == null)
				{
					this.InsertBTreeNode(new BTreeNodeValue(keyValue, this.m_owner), null, i);
				}
				else
				{
					this.m_tuples[i - 1].Child.SearchAndInsert(keyValue);
				}
			}

			private void InsertBTreeNode(BTreeNodeValue nodeValueToInsert, BTreeNode subTreeToInsert, int nodeIndexToInsert)
			{
				if (3 > this.m_tuples.Count)
				{
					this.m_tuples.Insert(nodeIndexToInsert, new BTreeNodeTuple(nodeValueToInsert, subTreeToInsert));
				}
				else
				{
					int num = 2;
					BTreeNode bTreeNode = new BTreeNode(this.m_owner);
					BTreeNodeValue bTreeNodeValue;
					if (num < nodeIndexToInsert)
					{
						bTreeNodeValue = this.m_tuples[num].Value;
						bTreeNode.SetFirstChild(this.m_tuples[num].Child);
						for (int i = num + 1; i < ((this.m_tuples.Count <= nodeIndexToInsert) ? this.m_tuples.Count : nodeIndexToInsert); i++)
						{
							bTreeNode.m_tuples.Add(this.m_tuples[i]);
						}
						bTreeNode.m_tuples.Add(new BTreeNodeTuple(nodeValueToInsert, subTreeToInsert));
						for (int j = nodeIndexToInsert; j < this.m_tuples.Count; j++)
						{
							bTreeNode.m_tuples.Add(this.m_tuples[j]);
						}
						int count = this.m_tuples.Count;
						for (int k = num; k < count; k++)
						{
							this.m_tuples.RemoveAtEnd();
						}
					}
					else if (num > nodeIndexToInsert)
					{
						bTreeNodeValue = this.m_tuples[num - 1].Value;
						bTreeNode.SetFirstChild(this.m_tuples[num - 1].Child);
						for (int l = num; l < this.m_tuples.Count; l++)
						{
							bTreeNode.m_tuples.Add(this.m_tuples[l]);
						}
						int count2 = this.m_tuples.Count;
						for (int m = num - 1; m < count2; m++)
						{
							this.m_tuples.RemoveAtEnd();
						}
						this.m_tuples.Insert(nodeIndexToInsert, new BTreeNodeTuple(nodeValueToInsert, subTreeToInsert));
					}
					else
					{
						bTreeNodeValue = nodeValueToInsert;
						bTreeNode.SetFirstChild(subTreeToInsert);
						for (int n = num; n < this.m_tuples.Count; n++)
						{
							bTreeNode.m_tuples.Add(this.m_tuples[n]);
						}
						int count3 = this.m_tuples.Count;
						for (int num2 = num; num2 < count3; num2++)
						{
							this.m_tuples.RemoveAtEnd();
						}
					}
					if (this.m_parent != null)
					{
						this.m_parent.InsertBTreeNode(bTreeNodeValue, bTreeNode, this.m_indexInParent + 1);
					}
					else
					{
						BTreeNode bTreeNode2 = new BTreeNode(this.m_owner);
						bTreeNode2.SetFirstChild(this);
						bTreeNode2.m_tuples.Add(new BTreeNodeTuple(bTreeNodeValue, bTreeNode));
						this.m_owner.SortTree = bTreeNode2;
					}
				}
			}
		}

		internal sealed class BTreeNodeTuple
		{
			private BTreeNodeValue m_value;

			private BTreeNode m_child;

			internal BTreeNodeValue Value
			{
				get
				{
					return this.m_value;
				}
			}

			internal BTreeNode Child
			{
				get
				{
					return this.m_child;
				}
				set
				{
					this.m_child = value;
				}
			}

			internal BTreeNodeTuple(BTreeNodeValue value, BTreeNode child)
			{
				this.m_value = value;
				this.m_child = child;
			}

			internal void Traverse(ProcessingStages operation, bool ascending)
			{
				if (ascending)
				{
					if (this.m_value != null)
					{
						this.m_value.Traverse(operation);
					}
					if (this.m_child != null)
					{
						this.m_child.Traverse(operation, ascending);
					}
				}
				else
				{
					if (this.m_child != null)
					{
						this.m_child.Traverse(operation, ascending);
					}
					if (this.m_value != null)
					{
						this.m_value.Traverse(operation);
					}
				}
			}
		}

		internal sealed class BTreeNodeValue
		{
			private object m_key;

			private IHierarchyObj m_hierarchyNode;

			internal BTreeNodeValue(object key, IHierarchyObj owner)
			{
				this.m_key = key;
				if (key != null)
				{
					this.m_hierarchyNode = owner.CreateHierarchyObj();
					this.m_hierarchyNode.NextRow();
				}
			}

			internal void AddRow()
			{
				this.m_hierarchyNode.NextRow();
			}

			internal void Traverse(ProcessingStages operation)
			{
				if (this.m_hierarchyNode != null)
				{
					this.m_hierarchyNode.Traverse(operation);
				}
			}

			internal int CompareTo(object keyValue)
			{
				return ReportProcessing.CompareTo(this.m_key, keyValue, this.m_hierarchyNode.ProcessingContext.CompareInfo, this.m_hierarchyNode.ProcessingContext.ClrCompareOptions);
			}
		}

		private sealed class BTreeNodeTupleList
		{
			private ArrayList m_list;

			private int m_capacity;

			private BTreeNode m_owner;

			internal BTreeNodeTuple this[int index]
			{
				get
				{
					return (BTreeNodeTuple)this.m_list[index];
				}
			}

			internal int Count
			{
				get
				{
					return this.m_list.Count;
				}
			}

			internal BTreeNodeTupleList(BTreeNode owner, int capacity)
			{
				this.m_owner = owner;
				this.m_list = new ArrayList(capacity);
				this.m_capacity = capacity;
			}

			internal void Add(BTreeNodeTuple tuple)
			{
				if (this.m_list.Count == this.m_capacity)
				{
					throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
				}
				this.m_list.Add(tuple);
				if (tuple.Child != null)
				{
					tuple.Child.Parent = this.m_owner;
					tuple.Child.IndexInParent = this.m_list.Count - 1;
				}
			}

			internal void Insert(int index, BTreeNodeTuple tuple)
			{
				if (this.m_list.Count == this.m_capacity)
				{
					throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
				}
				this.m_list.Insert(index, tuple);
				if (tuple.Child != null)
				{
					tuple.Child.Parent = this.m_owner;
				}
				for (int i = index; i < this.m_list.Count; i++)
				{
					BTreeNode child = ((BTreeNodeTuple)this.m_list[i]).Child;
					if (child != null)
					{
						child.IndexInParent = i;
					}
				}
			}

			internal void RemoveAtEnd()
			{
				this.m_list.RemoveAt(this.m_list.Count - 1);
			}
		}

		internal sealed class RuntimeHierarchyObjList : ArrayList
		{
			internal new RuntimeHierarchyObj this[int index]
			{
				get
				{
					return (RuntimeHierarchyObj)base[index];
				}
			}
		}

		internal sealed class DataRowList : ArrayList
		{
			internal new FieldImpl[] this[int index]
			{
				get
				{
					return (FieldImpl[])base[index];
				}
			}
		}

		internal sealed class PageTextboxes
		{
			private ArrayList m_pages;

			internal PageTextboxes()
			{
				this.m_pages = new ArrayList();
			}

			internal int GetPageCount()
			{
				return this.m_pages.Count;
			}

			internal void IntegrateRepeatingTextboxValues(PageTextboxes source, int targetStartPage, int targetEndPage)
			{
				if (source != null && targetStartPage <= targetEndPage)
				{
					int num = source.GetPageCount() - 1;
					if (num >= 0)
					{
						Hashtable textboxesOnPage = source.GetTextboxesOnPage(num);
						if (textboxesOnPage != null)
						{
							for (int i = targetStartPage; i <= targetEndPage; i++)
							{
								IDictionaryEnumerator enumerator = textboxesOnPage.GetEnumerator();
								while (enumerator.MoveNext())
								{
									this.AddTextboxValue(i, enumerator.Key as string, enumerator.Value as ArrayList);
								}
							}
						}
					}
				}
			}

			internal void IntegrateNonRepeatingTextboxValues(PageTextboxes source)
			{
				if (source != null)
				{
					int pageCount = source.GetPageCount();
					for (int i = 0; i < pageCount; i++)
					{
						Hashtable textboxesOnPage = source.GetTextboxesOnPage(i);
						if (textboxesOnPage != null)
						{
							IDictionaryEnumerator enumerator = textboxesOnPage.GetEnumerator();
							while (enumerator.MoveNext())
							{
								this.AddTextboxValue(i, enumerator.Key as string, enumerator.Value as ArrayList);
							}
						}
					}
				}
			}

			internal void AddTextboxValue(int page, string name, object value)
			{
				ArrayList arrayList = new ArrayList();
				arrayList.Add(value);
				this.AddTextboxValue(page, name, arrayList);
			}

			internal void AddTextboxValue(int page, string name, ArrayList values)
			{
				Global.Tracer.Assert(0 <= page && name != null && null != values, "(0 <= page && null != name && null != values)");
				if (0 <= page)
				{
					int count = this.m_pages.Count;
					if (count <= page)
					{
						for (int i = count; i <= page; i++)
						{
							this.m_pages.Add(null);
						}
					}
					Hashtable hashtable = this.m_pages[page] as Hashtable;
					if (hashtable == null)
					{
						hashtable = new Hashtable();
						this.m_pages[page] = hashtable;
					}
					ArrayList arrayList = hashtable[name] as ArrayList;
					if (arrayList == null)
					{
						arrayList = new ArrayList();
						hashtable.Add(name, arrayList);
					}
					arrayList.AddRange(values);
				}
			}

			internal Hashtable GetTextboxesOnPage(int page)
			{
				Global.Tracer.Assert(0 <= page, "(0 <= page)");
				if (page >= this.m_pages.Count)
				{
					return null;
				}
				return this.m_pages[page] as Hashtable;
			}

			internal ArrayList GetTextboxValues(int page, string name)
			{
				Global.Tracer.Assert(0 <= page && null != name, "(0 <= page && null != name)");
				Hashtable textboxesOnPage = this.GetTextboxesOnPage(page);
				if (textboxesOnPage == null)
				{
					return null;
				}
				return textboxesOnPage[name] as ArrayList;
			}
		}

		internal sealed class PageSectionContext
		{
			private bool m_needPageSectionEvaluation;

			private bool m_isOnePass;

			private PageTextboxes m_pageTextboxes;

			private List<bool> m_pageItemVisibility;

			private List<bool> m_matrixColumnVisibility;

			private List<bool> m_matrixRowVisibility;

			private List<bool> m_matrixInColumnHeader;

			private ArrayList m_tableColumnVisibility;

			private IntList m_tableColumnPosition;

			private IntList m_tableColumnSpans;

			private List<PageTextboxes> m_repeatingItemList;

			private bool m_inMatrixSubtotal;

			private bool m_inMatrixCell;

			private int m_subreportLevel;

			internal PageTextboxes PageTextboxes
			{
				get
				{
					return this.m_pageTextboxes;
				}
				set
				{
					this.m_pageTextboxes = value;
				}
			}

			internal bool InMatrixSubtotal
			{
				get
				{
					return this.m_inMatrixSubtotal;
				}
				set
				{
					this.m_inMatrixSubtotal = value;
				}
			}

			internal bool InMatrixCell
			{
				get
				{
					return this.m_inMatrixCell;
				}
				set
				{
					this.m_inMatrixCell = value;
				}
			}

			internal bool InSubreport
			{
				get
				{
					return 0 != this.m_subreportLevel;
				}
			}

			internal bool InRepeatingItem
			{
				get
				{
					if (this.m_repeatingItemList == null)
					{
						return false;
					}
					return this.m_repeatingItemList.Count > 1;
				}
			}

			internal IntList TableColumnSpans
			{
				get
				{
					return this.m_tableColumnSpans;
				}
				set
				{
					this.m_tableColumnSpans = value;
				}
			}

			internal bool HasPageSections
			{
				get
				{
					return this.m_needPageSectionEvaluation;
				}
			}

			internal PageSectionContext(bool hasPageSections, bool isOnePass)
			{
				this.ConstructorHelper(hasPageSections, isOnePass, true, false, false, 0);
			}

			internal PageSectionContext(PageSectionContext copy)
			{
				this.ConstructorHelper(copy.m_needPageSectionEvaluation, copy.m_isOnePass, copy.IsParentVisible(), copy.InMatrixSubtotal, copy.InMatrixCell, copy.m_subreportLevel);
			}

			private void ConstructorHelper(bool needPageSectionEvaluation, bool isOnePass, bool parentVisible, bool inMatrixSubtotal, bool inMatrixCell, int subreportLevel)
			{
				this.m_needPageSectionEvaluation = needPageSectionEvaluation;
				this.m_isOnePass = isOnePass;
				if (needPageSectionEvaluation)
				{
					this.m_pageTextboxes = new PageTextboxes();
					this.m_inMatrixSubtotal = inMatrixSubtotal;
					this.m_inMatrixCell = inMatrixCell;
					this.m_subreportLevel = subreportLevel;
					this.m_repeatingItemList = new List<PageTextboxes>();
					this.m_repeatingItemList.Add(this.m_pageTextboxes);
				}
				if (!isOnePass && !needPageSectionEvaluation)
				{
					return;
				}
				if (!parentVisible)
				{
					this.m_pageItemVisibility = new List<bool>();
					this.m_pageItemVisibility.Add(false);
				}
			}

			internal void EnterSubreport()
			{
				if (!this.m_needPageSectionEvaluation && !this.m_isOnePass)
				{
					return;
				}
				this.m_subreportLevel++;
			}

			internal void ExitSubreport()
			{
				if (!this.m_needPageSectionEvaluation && !this.m_isOnePass)
				{
					return;
				}
				this.m_subreportLevel--;
			}

			internal void EnterRepeatingItem()
			{
				if (this.m_needPageSectionEvaluation)
				{
					this.m_repeatingItemList.Insert(0, this.m_pageTextboxes);
					this.m_pageTextboxes = new PageTextboxes();
				}
			}

			internal PageTextboxes ExitRepeatingItem()
			{
				if (!this.m_needPageSectionEvaluation)
				{
					return null;
				}
				PageTextboxes pageTextboxes = this.m_pageTextboxes;
				this.m_pageTextboxes = this.m_repeatingItemList[0];
				this.m_repeatingItemList.RemoveAt(0);
				return pageTextboxes;
			}

			internal void RegisterTableColumnVisibility(bool isOnePass, TableColumnList columns, bool[] columnsStartHidden)
			{
				if (!this.m_isOnePass)
				{
					if (!this.m_needPageSectionEvaluation)
					{
						return;
					}
					if (this.InSubreport)
					{
						return;
					}
				}
				Global.Tracer.Assert(null != columns, "(null != columns)");
				bool[] array;
				if (isOnePass && columnsStartHidden == null)
				{
					array = null;
				}
				else
				{
					Global.Tracer.Assert(columns.Count == columnsStartHidden.Length, "(columns.Count == columnsStartHidden.Length)");
					array = new bool[columnsStartHidden.Length];
					for (int i = 0; i < columnsStartHidden.Length; i++)
					{
						array[i] = Visibility.IsVisible(columns[i].Visibility, columnsStartHidden[i]);
					}
				}
				if (this.m_tableColumnVisibility == null)
				{
					this.m_tableColumnVisibility = new ArrayList();
					this.m_tableColumnPosition = new IntList();
				}
				this.m_tableColumnVisibility.Insert(0, array);
				this.m_tableColumnPosition.Insert(0, -1);
			}

			internal void UnregisterTableColumnVisibility()
			{
				if (!this.m_isOnePass)
				{
					if (!this.m_needPageSectionEvaluation)
					{
						return;
					}
					if (this.InSubreport)
					{
						return;
					}
				}
				Global.Tracer.Assert(this.m_tableColumnVisibility != null && this.m_tableColumnPosition != null && this.m_tableColumnPosition.Count > 0 && this.m_tableColumnPosition.Count == this.m_tableColumnPosition.Count);
				this.m_tableColumnVisibility.RemoveAt(0);
				this.m_tableColumnPosition.RemoveAt(0);
				if (this.m_tableColumnPosition.Count == 0)
				{
					this.m_tableColumnPosition = null;
					this.m_tableColumnVisibility = null;
				}
			}

			internal void SetTableCellIndex(bool isOnePass, int position)
			{
				if (!this.m_isOnePass)
				{
					if (!this.m_needPageSectionEvaluation)
					{
						return;
					}
					if (this.InSubreport)
					{
						return;
					}
				}
				if (isOnePass && this.m_tableColumnPosition == null)
				{
					this.m_tableColumnPosition = new IntList();
					this.m_tableColumnPosition.Add(-1);
					this.m_tableColumnVisibility = new ArrayList();
					this.m_tableColumnVisibility.Add(null);
				}
				Global.Tracer.Assert(this.m_tableColumnPosition != null && position >= 0);
				this.m_tableColumnPosition[0] = position;
			}

			internal TableColumnInfo GetOnePassTableCellProperties()
			{
				if (!this.m_isOnePass && (!this.m_needPageSectionEvaluation || this.InSubreport))
				{
					return default(TableColumnInfo);
				}
				Global.Tracer.Assert(null != this.m_tableColumnPosition, "(null != m_tableColumnPosition)");
				return this.GetTableCellProperties(this.m_tableColumnPosition[0]);
			}

			internal bool IsTableColumnVisible(TableColumnInfo columnInfo)
			{
				if (!this.m_isOnePass && (!this.m_needPageSectionEvaluation || this.InSubreport))
				{
					return false;
				}
				Global.Tracer.Assert(this.m_tableColumnVisibility != null && 0 < this.m_tableColumnVisibility.Count);
				return Visibility.IsTableCellVisible(this.m_tableColumnVisibility[0] as bool[], columnInfo.StartIndex, columnInfo.Span);
			}

			internal void EnterVisibilityScope(Visibility visibility, bool startHidden)
			{
				if (!this.m_isOnePass && !this.m_needPageSectionEvaluation)
				{
					return;
				}
				if (this.m_pageItemVisibility == null)
				{
					this.m_pageItemVisibility = new List<bool>();
				}
				bool flag = true;
				if (0 < this.m_pageItemVisibility.Count)
				{
					flag = this.m_pageItemVisibility[0];
				}
				if (flag)
				{
					this.m_pageItemVisibility.Insert(0, Visibility.IsVisible(visibility, startHidden));
				}
				else
				{
					this.m_pageItemVisibility.Insert(0, false);
				}
			}

			internal void ExitVisibilityScope()
			{
				if (!this.m_isOnePass && !this.m_needPageSectionEvaluation)
				{
					return;
				}
				Global.Tracer.Assert(this.m_pageItemVisibility != null && this.m_pageItemVisibility.Count > 0, "(null != m_pageItemVisibility && m_pageItemVisibility.Count > 0)");
				this.m_pageItemVisibility.RemoveAt(0);
			}

			private bool IsMatrixHeadingVisible()
			{
				if (!this.m_isOnePass && !this.m_needPageSectionEvaluation)
				{
					return false;
				}
				if (this.m_matrixInColumnHeader == null)
				{
					return true;
				}
				Global.Tracer.Assert(0 < this.m_matrixInColumnHeader.Count, "(0 < m_matrixInColumnHeader.Count)");
				if (this.m_matrixInColumnHeader[0])
				{
					return this.m_matrixColumnVisibility[0];
				}
				return this.m_matrixRowVisibility[0];
			}

			private bool IsMatrixCellVisible()
			{
				if (!this.m_isOnePass && !this.m_needPageSectionEvaluation)
				{
					return false;
				}
				if (this.m_matrixInColumnHeader == null)
				{
					return true;
				}
				bool flag = true;
				if (this.m_matrixColumnVisibility != null)
				{
					flag &= this.m_matrixColumnVisibility[0];
				}
				if (this.m_matrixRowVisibility != null)
				{
					flag &= this.m_matrixRowVisibility[0];
				}
				return flag;
			}

			private bool IsParentMatrixHeadingVisible(bool newMatrixHeadingColumn)
			{
				if (!this.m_isOnePass && !this.m_needPageSectionEvaluation)
				{
					return false;
				}
				if (this.m_matrixInColumnHeader == null)
				{
					return true;
				}
				Global.Tracer.Assert(0 < this.m_matrixInColumnHeader.Count, "(0 < m_matrixInColumnHeader.Count)");
				if (this.m_matrixInColumnHeader[0] == newMatrixHeadingColumn)
				{
					return this.IsMatrixHeadingVisible();
				}
				return true;
			}

			internal void EnterMatrixSubtotalScope(bool isColumn)
			{
				if (this.m_needPageSectionEvaluation)
				{
					if (this.m_matrixInColumnHeader == null)
					{
						this.m_matrixInColumnHeader = new List<bool>();
						this.m_matrixColumnVisibility = new List<bool>();
						this.m_matrixRowVisibility = new List<bool>();
					}
					this.m_matrixInColumnHeader.Insert(0, isColumn);
					if (isColumn)
					{
						this.m_matrixColumnVisibility.Insert(0, false);
					}
					else
					{
						this.m_matrixRowVisibility.Insert(0, false);
					}
				}
			}

			internal void EnterMatrixHeadingScope(bool currentHeadingIsVisible, bool isColumn)
			{
				if (this.m_needPageSectionEvaluation && !this.InSubreport)
				{
					bool flag = this.IsParentMatrixHeadingVisible(isColumn);
					if (this.m_matrixInColumnHeader == null)
					{
						this.m_matrixInColumnHeader = new List<bool>();
						this.m_matrixColumnVisibility = new List<bool>();
						this.m_matrixRowVisibility = new List<bool>();
					}
					this.m_matrixInColumnHeader.Insert(0, isColumn);
					if (isColumn)
					{
						this.m_matrixColumnVisibility.Insert(0, flag && currentHeadingIsVisible);
					}
					else
					{
						this.m_matrixRowVisibility.Insert(0, flag && currentHeadingIsVisible);
					}
				}
			}

			internal void ExitMatrixHeadingScope(bool isColumn)
			{
				if (this.m_needPageSectionEvaluation && !this.InSubreport)
				{
					Global.Tracer.Assert(this.m_matrixInColumnHeader != null && 0 < this.m_matrixInColumnHeader.Count, "(null != m_matrixInColumnHeader && 0 < m_matrixInColumnHeader.Count)");
					if (isColumn)
					{
						this.m_matrixColumnVisibility.RemoveAt(0);
					}
					else
					{
						this.m_matrixRowVisibility.RemoveAt(0);
					}
					this.m_matrixInColumnHeader.RemoveAt(0);
					if (this.m_matrixInColumnHeader.Count == 0)
					{
						this.m_matrixInColumnHeader = null;
						this.m_matrixColumnVisibility = null;
						this.m_matrixRowVisibility = null;
					}
				}
			}

			internal bool IsParentVisible()
			{
				if (!this.m_isOnePass && !this.m_needPageSectionEvaluation)
				{
					return false;
				}
				if (this.m_subreportLevel == 0 && !this.m_inMatrixSubtotal)
				{
					if (!this.IsMatrixHeadingVisible())
					{
						return false;
					}
					if (this.m_inMatrixCell && !this.IsMatrixCellVisible())
					{
						return false;
					}
					if (this.m_pageItemVisibility != null && 0 < this.m_pageItemVisibility.Count)
					{
						if (this.m_tableColumnVisibility != null && 0 < this.m_tableColumnVisibility.Count && this.m_tableColumnVisibility[0] != null && this.m_tableColumnSpans != null)
						{
							TableColumnInfo tableCellProperties = this.GetTableCellProperties(this.m_tableColumnPosition[0]);
							if (this.m_pageItemVisibility[0])
							{
								return Visibility.IsTableCellVisible(this.m_tableColumnVisibility[0] as bool[], tableCellProperties.StartIndex, tableCellProperties.Span);
							}
							return false;
						}
						return this.m_pageItemVisibility[0];
					}
					return true;
				}
				return false;
			}

			internal TableColumnInfo GetTableCellProperties(int cellIndex)
			{
				if (cellIndex >= 0 && this.m_tableColumnSpans != null)
				{
					Global.Tracer.Assert(cellIndex < this.m_tableColumnSpans.Count, "(cellIndex < m_tableColumnSpans.Count)");
					TableColumnInfo result = default(TableColumnInfo);
					result.StartIndex = 0;
					for (int i = 0; i < cellIndex; i++)
					{
						result.StartIndex += this.m_tableColumnSpans[i];
					}
					result.Span = this.m_tableColumnSpans[cellIndex];
					return result;
				}
				return default(TableColumnInfo);
			}
		}

		internal sealed class AggregateRow
		{
			private FieldImpl[] m_fields;

			private bool m_isAggregateRow;

			private int m_aggregationFieldCount;

			private bool m_validAggregateRow;

			internal AggregateRow(ProcessingContext processingContext)
			{
				FieldsImpl fieldsImpl = processingContext.ReportObjectModel.FieldsImpl;
				this.m_fields = fieldsImpl.GetAndSaveFields();
				this.m_isAggregateRow = fieldsImpl.IsAggregateRow;
				this.m_aggregationFieldCount = fieldsImpl.AggregationFieldCount;
				this.m_validAggregateRow = fieldsImpl.ValidAggregateRow;
			}

			internal void SetFields(ProcessingContext processingContext)
			{
				processingContext.ReportObjectModel.FieldsImpl.SetFields(this.m_fields, this.m_isAggregateRow, this.m_aggregationFieldCount, this.m_validAggregateRow);
			}
		}

		internal sealed class AggregateRowList : ArrayList
		{
			internal new AggregateRow this[int index]
			{
				get
				{
					return (AggregateRow)base[index];
				}
			}
		}

		internal sealed class AggregateRowInfo
		{
			private bool[] m_aggregationFieldChecked;

			private int m_aggregationFieldCount;

			private bool m_validAggregateRow;

			internal void SaveAggregateInfo(ProcessingContext processingContext)
			{
				FieldsImpl fieldsImpl = processingContext.ReportObjectModel.FieldsImpl;
				this.m_aggregationFieldCount = fieldsImpl.AggregationFieldCount;
				if (this.m_aggregationFieldChecked == null)
				{
					this.m_aggregationFieldChecked = new bool[fieldsImpl.Count];
				}
				for (int i = 0; i < fieldsImpl.Count; i++)
				{
					this.m_aggregationFieldChecked[i] = fieldsImpl[i].AggregationFieldChecked;
				}
				this.m_validAggregateRow = fieldsImpl.ValidAggregateRow;
			}

			internal void RestoreAggregateInfo(ProcessingContext processingContext)
			{
				FieldsImpl fieldsImpl = processingContext.ReportObjectModel.FieldsImpl;
				fieldsImpl.AggregationFieldCount = this.m_aggregationFieldCount;
				Global.Tracer.Assert(null != this.m_aggregationFieldChecked, "(null != m_aggregationFieldChecked)");
				for (int i = 0; i < fieldsImpl.Count; i++)
				{
					fieldsImpl[i].AggregationFieldChecked = this.m_aggregationFieldChecked[i];
				}
				fieldsImpl.ValidAggregateRow = this.m_validAggregateRow;
			}

			internal void CombineAggregateInfo(ProcessingContext processingContext, AggregateRowInfo updated)
			{
				FieldsImpl fieldsImpl = processingContext.ReportObjectModel.FieldsImpl;
				if (updated == null)
				{
					fieldsImpl.ValidAggregateRow = false;
				}
				else
				{
					if (!updated.m_validAggregateRow)
					{
						fieldsImpl.ValidAggregateRow = false;
					}
					for (int i = 0; i < fieldsImpl.Count; i++)
					{
						if (updated.m_aggregationFieldChecked[i] && !fieldsImpl[i].AggregationFieldChecked)
						{
							fieldsImpl[i].AggregationFieldChecked = true;
							fieldsImpl.AggregationFieldCount--;
						}
					}
				}
			}
		}

		private sealed class RuntimeRICollectionList : ArrayList
		{
			internal new RuntimeRICollection this[int index]
			{
				get
				{
					return (RuntimeRICollection)base[index];
				}
			}

			internal RuntimeRICollectionList()
			{
			}

			internal RuntimeRICollectionList(int capacity)
				: base(capacity)
			{
			}

			internal void FirstPassNextDataRow()
			{
				for (int i = 0; i < this.Count; i++)
				{
					this[i].FirstPassNextDataRow();
				}
			}

			internal void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				for (int i = 0; i < this.Count; i++)
				{
					this[i].CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				}
			}

			internal void CalculatePreviousAggregates(AggregatesImpl globalRVCol)
			{
				for (int i = 0; i < this.Count; i++)
				{
					this[i].CalculatePreviousAggregates(globalRVCol);
				}
			}

			internal void ResetReportItemObjs()
			{
				for (int i = 0; i < this.Count; i++)
				{
					this[i].ResetReportItemObjs();
				}
			}
		}

		internal interface IFilterOwner
		{
			void PostFilterNextRow();
		}

		internal interface IScope
		{
			bool TargetForNonDetailSort
			{
				get;
			}

			int[] SortFilterExpressionScopeInfoIndices
			{
				get;
			}

			bool IsTargetForSort(int index, bool detailSort);

			void ReadRow(DataActions dataAction);

			bool InScope(string scope);

			IScope GetOuterScope(bool includeSubReportContainingScope);

			string GetScopeName();

			int RecursiveLevel(string scope);

			bool TargetScopeMatched(int index, bool detailSort);

			void GetScopeValues(IHierarchyObj targetScopeObj, VariantList[] scopeValues, ref int index);

			void GetGroupNameValuePairs(Dictionary<string, object> pairs);
		}

		internal interface IHierarchyObj
		{
			IHierarchyObj HierarchyRoot
			{
				get;
			}

			ProcessingContext ProcessingContext
			{
				get;
			}

			BTreeNode SortTree
			{
				get;
				set;
			}

			int ExpressionIndex
			{
				get;
			}

			IntList SortFilterInfoIndices
			{
				get;
			}

			bool IsDetail
			{
				get;
			}

			IHierarchyObj CreateHierarchyObj();

			ProcessingMessageList RegisterComparisonError(string propertyName);

			void NextRow();

			void Traverse(ProcessingStages operation);

			void ReadRow();

			void ProcessUserSort();

			void MarkSortInfoProcessed(RuntimeSortFilterEventInfoList runtimeSortFilterInfo);

			void AddSortInfoIndex(int sortInfoIndex, RuntimeSortFilterEventInfo sortInfo);
		}

		internal interface ISortDataHolder
		{
			void NextRow();

			void Traverse(ProcessingStages operation);
		}

		private sealed class RuntimeDRCollection
		{
			private RuntimeDataRegionObjList m_dataRegionObjs;

			private ProcessingContext m_processingContext;

			internal RuntimeDRCollection(IScope outerScope, DataRegionList dataRegionDefs, ProcessingContext processingContext, bool onePass)
			{
				this.m_processingContext = processingContext;
				this.m_dataRegionObjs = new RuntimeDataRegionObjList();
				this.CreateDataRegions(outerScope, dataRegionDefs, onePass);
			}

			private void CreateDataRegions(IScope outerScope, DataRegionList dataRegionDefs, bool onePass)
			{
				DataActions dataActions = DataActions.None;
				for (int i = 0; i < dataRegionDefs.Count; i++)
				{
					DataRegion dataRegion = dataRegionDefs[i];
					RuntimeDataRegionObj runtimeDataRegionObj = null;
					if (dataRegion is List)
					{
						List list = (List)dataRegion;
						if (onePass)
						{
							Global.Tracer.Assert(null == list.Grouping, "(null == list.Grouping)");
							runtimeDataRegionObj = new RuntimeOnePassListDetailObj(outerScope, list, this.m_processingContext);
						}
						else
						{
							runtimeDataRegionObj = (RuntimeDataRegionObj)((list.Grouping == null) ? ((object)new RuntimeListDetailObj(outerScope, list, ref dataActions, this.m_processingContext)) : ((object)new RuntimeListGroupRootObj(outerScope, list, ref dataActions, this.m_processingContext)));
						}
					}
					else if (dataRegion is Matrix)
					{
						runtimeDataRegionObj = new RuntimeMatrixObj(outerScope, (Matrix)dataRegion, ref dataActions, this.m_processingContext, onePass);
					}
					else if (dataRegion is Chart)
					{
						runtimeDataRegionObj = new RuntimeChartObj(outerScope, (Chart)dataRegion, ref dataActions, this.m_processingContext, onePass);
					}
					else if (dataRegion is Table)
					{
						runtimeDataRegionObj = new RuntimeTableObj(outerScope, (Table)dataRegion, ref dataActions, this.m_processingContext, onePass);
					}
					else if (dataRegion is CustomReportItem)
					{
						CustomReportItem customReportItem = dataRegion as CustomReportItem;
						Global.Tracer.Assert(null != customReportItem, "(null != crItem)");
						if (customReportItem.DataSetName != null)
						{
							runtimeDataRegionObj = new RuntimeCustomReportItemObj(outerScope, customReportItem, ref dataActions, this.m_processingContext, onePass);
						}
					}
					else if (dataRegion is OWCChart)
					{
						OWCChart oWCChart = (OWCChart)dataRegion;
						oWCChart.SetExprHost(this.m_processingContext.ReportRuntime.ReportExprHost, this.m_processingContext.ReportObjectModel);
						runtimeDataRegionObj = (RuntimeDataRegionObj)((!onePass) ? ((object)new RuntimeOWCChartDetailObj(outerScope, oWCChart, ref dataActions, this.m_processingContext)) : ((object)new RuntimeOnePassOWCChartDetailObj(outerScope, oWCChart, this.m_processingContext)));
					}
					if (runtimeDataRegionObj != null)
					{
						this.m_dataRegionObjs.Add(runtimeDataRegionObj);
						dataRegion.RuntimeDataRegionObj = runtimeDataRegionObj;
					}
				}
			}

			internal void FirstPassNextDataRow()
			{
				AggregateRowInfo aggregateRowInfo = new AggregateRowInfo();
				aggregateRowInfo.SaveAggregateInfo(this.m_processingContext);
				for (int i = 0; i < this.m_dataRegionObjs.Count; i++)
				{
					this.m_dataRegionObjs[i].NextRow();
					aggregateRowInfo.RestoreAggregateInfo(this.m_processingContext);
				}
			}

			internal void SortAndFilter()
			{
				for (int i = 0; i < this.m_dataRegionObjs.Count; i++)
				{
					RuntimeDataRegionObj runtimeDataRegionObj = this.m_dataRegionObjs[i];
					if (!(runtimeDataRegionObj is RuntimeDetailObj))
					{
						runtimeDataRegionObj.SortAndFilter();
					}
				}
			}

			internal void CalculateRunningValues(AggregatesImpl globalRunningValueCollection, RuntimeGroupRootObjList groupCollection)
			{
				for (int i = 0; i < this.m_dataRegionObjs.Count; i++)
				{
					this.m_dataRegionObjs[i].CalculateRunningValues(globalRunningValueCollection, groupCollection, null);
				}
			}
		}

		internal sealed class RuntimeRICollection
		{
			internal enum SubReportInitialization
			{
				AssignIDsOnly,
				RuntimeOnly,
				All
			}

			private IScope m_owner;

			private ReportItemCollection m_reportItemsDef;

			private ProcessingContext m_processingContext;

			private RuntimeDataRegionObjList m_dataRegionObjs;

			private DataAggregateObjResult[] m_runningValueValues;

			private int m_currDataRegion;

			internal ReportItemCollection ReportItemsDef
			{
				get
				{
					return this.m_reportItemsDef;
				}
			}

			internal RuntimeRICollection(IScope owner, ReportItemCollection RIColDef, ref DataActions dataAction, ProcessingContext processingContext, bool createDataRegions)
			{
				this.m_owner = owner;
				this.m_reportItemsDef = RIColDef;
				this.m_processingContext = processingContext;
				if (createDataRegions && RIColDef != null)
				{
					this.CreateDataRegions(owner, RIColDef.ComputedReportItems, ref dataAction);
				}
			}

			internal RuntimeRICollection(IScope owner, ReportItemCollection RIColDef, ProcessingContext processingContext, bool createDataRegions)
			{
				this.m_owner = owner;
				this.m_reportItemsDef = RIColDef;
				this.m_processingContext = processingContext;
				if (createDataRegions)
				{
					this.CreateDataRegions(owner, RIColDef.ComputedReportItems);
				}
			}

			private void CreateDataRegions(IScope owner, ReportItemList computedRIs, ref DataActions dataAction)
			{
				if (computedRIs != null)
				{
					RuntimeDataRegionObj runtimeDataRegionObj = null;
					for (int i = 0; i < computedRIs.Count; i++)
					{
						ReportItem reportItem = computedRIs[i];
						runtimeDataRegionObj = null;
						if (reportItem is Rectangle)
						{
							ReportItemCollection reportItems = ((Rectangle)reportItem).ReportItems;
							if (reportItems != null && reportItems.ComputedReportItems != null)
							{
								this.CreateDataRegions(owner, reportItems.ComputedReportItems, ref dataAction);
							}
						}
						else if (reportItem is DataRegion && !(owner is RuntimeDetailObj))
						{
							if (reportItem is List)
							{
								List list = (List)reportItem;
								runtimeDataRegionObj = (RuntimeDataRegionObj)((list.Grouping == null) ? ((object)new RuntimeListDetailObj(owner, list, ref dataAction, this.m_processingContext)) : ((object)new RuntimeListGroupRootObj(owner, list, ref dataAction, this.m_processingContext)));
							}
							else if (reportItem is Matrix)
							{
								runtimeDataRegionObj = new RuntimeMatrixObj(owner, (Matrix)reportItem, ref dataAction, this.m_processingContext, false);
							}
							else if (reportItem is Table)
							{
								runtimeDataRegionObj = new RuntimeTableObj(owner, (Table)reportItem, ref dataAction, this.m_processingContext, false);
							}
							else if (reportItem is Chart)
							{
								runtimeDataRegionObj = new RuntimeChartObj(owner, (Chart)reportItem, ref dataAction, this.m_processingContext, false);
							}
							else if (reportItem is OWCChart)
							{
								runtimeDataRegionObj = new RuntimeOWCChartDetailObj(owner, (OWCChart)reportItem, ref dataAction, this.m_processingContext);
							}
							else if (reportItem is CustomReportItem && ((CustomReportItem)reportItem).DataSetName != null)
							{
								runtimeDataRegionObj = new RuntimeCustomReportItemObj(owner, (CustomReportItem)reportItem, ref dataAction, this.m_processingContext, false);
							}
						}
						if (runtimeDataRegionObj != null)
						{
							if (this.m_dataRegionObjs == null)
							{
								this.m_dataRegionObjs = new RuntimeDataRegionObjList();
							}
							this.m_dataRegionObjs.Add(runtimeDataRegionObj);
						}
					}
				}
			}

			private void CreateDataRegions(IScope owner, ReportItemList computedRIs)
			{
				if (computedRIs != null)
				{
					DataActions dataActions = DataActions.None;
					RuntimeDataRegionObj runtimeDataRegionObj = null;
					for (int i = 0; i < computedRIs.Count; i++)
					{
						ReportItem reportItem = computedRIs[i];
						runtimeDataRegionObj = null;
						if (reportItem is Rectangle)
						{
							ReportItemCollection reportItems = ((Rectangle)reportItem).ReportItems;
							if (reportItems != null && reportItems.ComputedReportItems != null)
							{
								this.CreateDataRegions(owner, reportItems.ComputedReportItems);
							}
						}
						else if (reportItem is DataRegion)
						{
							if (reportItem is List)
							{
								List list = (List)reportItem;
								Global.Tracer.Assert(null == list.Grouping, "(null == list.Grouping)");
								runtimeDataRegionObj = new RuntimeOnePassListDetailObj(owner, list, this.m_processingContext);
							}
							else if (reportItem is Matrix)
							{
								runtimeDataRegionObj = new RuntimeMatrixObj(owner, (Matrix)reportItem, ref dataActions, this.m_processingContext, true);
							}
							else if (reportItem is Table)
							{
								runtimeDataRegionObj = new RuntimeTableObj(owner, (Table)reportItem, ref dataActions, this.m_processingContext, true);
							}
							else if (reportItem is Chart)
							{
								runtimeDataRegionObj = new RuntimeChartObj(owner, (Chart)reportItem, ref dataActions, this.m_processingContext, true);
							}
							else if (reportItem is OWCChart)
							{
								runtimeDataRegionObj = new RuntimeOnePassOWCChartDetailObj(owner, (OWCChart)reportItem, this.m_processingContext);
							}
							else if (reportItem is CustomReportItem)
							{
								runtimeDataRegionObj = new RuntimeCustomReportItemObj(owner, (CustomReportItem)reportItem, ref dataActions, this.m_processingContext, true);
							}
						}
						if (runtimeDataRegionObj != null)
						{
							if (this.m_dataRegionObjs == null)
							{
								this.m_dataRegionObjs = new RuntimeDataRegionObjList();
							}
							this.m_dataRegionObjs.Add(runtimeDataRegionObj);
						}
					}
				}
			}

			internal void FirstPassNextDataRow()
			{
				if (this.m_dataRegionObjs != null)
				{
					AggregateRowInfo aggregateRowInfo = new AggregateRowInfo();
					aggregateRowInfo.SaveAggregateInfo(this.m_processingContext);
					for (int i = 0; i < this.m_dataRegionObjs.Count; i++)
					{
						this.m_dataRegionObjs[i].NextRow();
						aggregateRowInfo.RestoreAggregateInfo(this.m_processingContext);
					}
				}
			}

			internal void SortAndFilter()
			{
				if (this.m_dataRegionObjs != null)
				{
					for (int i = 0; i < this.m_dataRegionObjs.Count; i++)
					{
						RuntimeDataRegionObj runtimeDataRegionObj = this.m_dataRegionObjs[i];
						if (!(runtimeDataRegionObj is RuntimeDetailObj))
						{
							runtimeDataRegionObj.SortAndFilter();
						}
					}
				}
			}

			internal void CalculatePreviousAggregates(AggregatesImpl globalRunningValueCollection)
			{
				RuntimeRICollection.DoneReadingRows(globalRunningValueCollection, this.m_reportItemsDef.RunningValues, ref this.m_runningValueValues, true);
			}

			internal void CalculateRunningValues(AggregatesImpl globalRunningValueCollection, RuntimeGroupRootObjList groupCollection, RuntimeGroupRootObj lastGroup)
			{
				this.CalculateInnerRunningValues(globalRunningValueCollection, groupCollection, lastGroup);
				RuntimeRICollection.DoneReadingRows(globalRunningValueCollection, this.m_reportItemsDef.RunningValues, ref this.m_runningValueValues, false);
			}

			internal void CalculateInnerRunningValues(AggregatesImpl globalRunningValueCollection, RuntimeGroupRootObjList groupCollection, RuntimeGroupRootObj lastGroup)
			{
				if (this.m_dataRegionObjs != null)
				{
					for (int i = 0; i < this.m_dataRegionObjs.Count; i++)
					{
						this.m_dataRegionObjs[i].CalculateRunningValues(globalRunningValueCollection, groupCollection, lastGroup);
					}
				}
			}

			internal static void DoneReadingRows(AggregatesImpl globalRVCol, RunningValueInfoList runningValues, ref DataAggregateObjResult[] runningValueValues, bool processPreviousAggregates)
			{
				if (runningValues != null && 0 < runningValues.Count)
				{
					if (runningValueValues == null)
					{
						runningValueValues = new DataAggregateObjResult[runningValues.Count];
					}
					for (int i = 0; i < runningValues.Count; i++)
					{
						if (processPreviousAggregates == (DataAggregateInfo.AggregateTypes.Previous == runningValues[i].AggregateType))
						{
							runningValueValues[i] = globalRVCol.GetAggregateObj(runningValues[i].Name).AggregateResult();
						}
					}
				}
				else
				{
					runningValueValues = null;
				}
			}

			private void SetupEnvironment()
			{
				RunningValueInfoList runningValues = this.m_reportItemsDef.RunningValues;
				if (runningValues != null && this.m_runningValueValues != null)
				{
					for (int i = 0; i < runningValues.Count; i++)
					{
						this.m_processingContext.ReportObjectModel.AggregatesImpl.Set(runningValues[i].Name, runningValues[i], runningValues[i].DuplicateNames, this.m_runningValueValues[i]);
					}
				}
				this.SetReportItemObjScope();
			}

			private void SetupEnvironment(ReportItemCollection reportItemsDef)
			{
				this.SetupEnvironment();
				if (reportItemsDef.ComputedReportItems != null)
				{
					int num = 0;
					for (int i = 0; i < reportItemsDef.ComputedReportItems.Count; i++)
					{
						ReportItem reportItem = reportItemsDef.ComputedReportItems[i];
						if (reportItem is DataRegion)
						{
							if (reportItem is Table || reportItem is Matrix)
							{
								RuntimeDataRegionObj runtimeDataRegionObj = (this.m_dataRegionObjs == null) ? ((DataRegion)reportItem).RuntimeDataRegionObj : this.m_dataRegionObjs[num];
								Global.Tracer.Assert(null != runtimeDataRegionObj, "(null != dataRegionObj)");
								runtimeDataRegionObj.SetupEnvironment();
							}
							if (this.m_dataRegionObjs != null)
							{
								num++;
							}
						}
					}
				}
			}

			internal void CreateInstances(ReportItemColInstance collectionInstance, bool ignorePageBreaks, bool ignoreInstances)
			{
				if (ignorePageBreaks)
				{
					this.m_processingContext.ChunkManager.EnterIgnorePageBreakItem();
				}
				if (ignoreInstances)
				{
					this.m_processingContext.ChunkManager.EnterIgnoreInstances();
				}
				this.CreateInstances(collectionInstance);
				if (ignoreInstances)
				{
					this.m_processingContext.ChunkManager.LeaveIgnoreInstances();
				}
				if (ignorePageBreaks)
				{
					this.m_processingContext.ChunkManager.LeaveIgnorePageBreakItem();
				}
			}

			internal void CreateInstances(ReportItemColInstance collectionInstance)
			{
				this.SetupEnvironment(this.m_reportItemsDef);
				this.m_currDataRegion = 0;
				this.CreateInstances(collectionInstance, this.m_reportItemsDef);
			}

			private void CreateInstances(ReportItemColInstance collectionInstance, ReportItemCollection reportItemsDef)
			{
				if (reportItemsDef != null && reportItemsDef.Count >= 1)
				{
					reportItemsDef.ProcessDrillthroughAction(this.m_processingContext, collectionInstance.ChildrenNonComputedUniqueNames);
					this.m_processingContext.ChunkManager.EnterReportItemCollection();
					ReportItemInstance reportItemInstance = null;
					ReportItem parent = reportItemsDef[0].Parent;
					int num = parent.StartPage;
					bool flag = false;
					bool flag2 = parent is Table;
					if (parent is Report || parent is List || parent is Rectangle || parent is SubReport || parent is CustomReportItem)
					{
						flag = true;
						collectionInstance.ChildrenStartAndEndPages = new RenderingPagesRangesList(reportItemsDef.Count);
					}
					List<DataRegion> list = new List<DataRegion>();
					for (int i = 0; i < reportItemsDef.Count; i++)
					{
						if (flag2)
						{
							this.m_processingContext.PageSectionContext.SetTableCellIndex(this.m_processingContext.IsOnePass, i);
						}
						bool flag3 = default(bool);
						int num2 = default(int);
						ReportItem reportItem = default(ReportItem);
						reportItemsDef.GetReportItem(i, out flag3, out num2, out reportItem);
						if (reportItem is DataRegion && ((DataRegion)reportItem).RepeatSiblings != null)
						{
							list.Add(reportItem as DataRegion);
						}
						if (reportItem.RepeatedSibling)
						{
							this.m_processingContext.PageSectionContext.EnterRepeatingItem();
						}
						if (flag3)
						{
							reportItem = reportItemsDef.ComputedReportItems[num2];
							reportItemInstance = this.CreateInstance(reportItem, false, i);
							if (reportItemInstance != null)
							{
								collectionInstance.Add(reportItemInstance);
							}
						}
						else
						{
							collectionInstance.SetPaginationForNonComputedChild(this.m_processingContext.Pagination, reportItem, parent);
							reportItem.ProcessNavigationAction(this.m_processingContext.NavigationInfo, collectionInstance.ChildrenNonComputedUniqueNames[num2], reportItem.StartPage);
							RuntimeRICollection.AddNonComputedPageTextboxes(reportItem, reportItem.StartPage, this.m_processingContext);
						}
						if (reportItem.RepeatedSibling)
						{
							reportItem.RepeatedSiblingTextboxes = this.m_processingContext.PageSectionContext.ExitRepeatingItem();
						}
						num = Math.Max(num, reportItem.EndPage);
						if (flag)
						{
							RenderingPagesRanges renderingPagesRanges = default(RenderingPagesRanges);
							renderingPagesRanges.StartPage = reportItem.StartPage;
							renderingPagesRanges.EndPage = reportItem.EndPage;
							collectionInstance.ChildrenStartAndEndPages.Add(renderingPagesRanges);
						}
						if (reportItem.RepeatedSiblingTextboxes != null && this.m_processingContext.PageSectionContext.PageTextboxes != null)
						{
							this.m_processingContext.PageSectionContext.PageTextboxes.IntegrateRepeatingTextboxValues(reportItem.RepeatedSiblingTextboxes, reportItem.StartPage, reportItem.EndPage);
						}
					}
					if (this.m_processingContext.PageSectionContext.PageTextboxes != null)
					{
						for (int j = 0; j < list.Count; j++)
						{
							DataRegion dataRegion = list[j];
							Global.Tracer.Assert(null != dataRegion.RepeatSiblings, "(null != dataRegion.RepeatSiblings)");
							for (int k = 0; k < dataRegion.RepeatSiblings.Count; k++)
							{
								ReportItem reportItem2 = reportItemsDef[dataRegion.RepeatSiblings[k]];
								Global.Tracer.Assert(null != reportItem2, "(null != sibling)");
								this.m_processingContext.PageSectionContext.PageTextboxes.IntegrateRepeatingTextboxValues(reportItem2.RepeatedSiblingTextboxes, dataRegion.StartPage, dataRegion.EndPage);
							}
						}
					}
					if (num > parent.EndPage)
					{
						parent.EndPage = num;
						this.m_processingContext.Pagination.SetCurrentPageHeight(parent, 1.0);
					}
					this.m_processingContext.ChunkManager.LeaveReportItemCollection();
				}
			}

			internal static void AddNonComputedPageTextboxes(ReportItem reportItem, int startPage, ProcessingContext processingContext)
			{
				if (processingContext.PageSectionContext.IsParentVisible() && Visibility.IsVisible(reportItem))
				{
					if (reportItem is TextBox)
					{
						TextBox textBox = reportItem as TextBox;
						object value = null;
						if (textBox.Value != null)
						{
							value = textBox.Value.Value;
						}
						if (0 <= startPage)
						{
							textBox.StartPage = startPage;
						}
						RuntimeRICollection.AddPageTextbox(processingContext, textBox, null, null, value);
					}
					else if (reportItem is Rectangle)
					{
						Rectangle rectangle = reportItem as Rectangle;
						if (rectangle.ReportItems != null)
						{
							for (int i = 0; i < rectangle.ReportItems.Count; i++)
							{
								RuntimeRICollection.AddNonComputedPageTextboxes(rectangle.ReportItems[i], startPage, processingContext);
							}
						}
					}
				}
			}

			internal ReportItemInstance CreateInstance(ReportItem reportItem, bool setupEnvironment, bool ignorePageBreaks, bool ignoreInstances)
			{
				if (ignorePageBreaks)
				{
					this.m_processingContext.ChunkManager.EnterIgnorePageBreakItem();
				}
				if (ignoreInstances)
				{
					this.m_processingContext.ChunkManager.EnterIgnoreInstances();
				}
				ReportItemInstance result = this.CreateInstance(reportItem, setupEnvironment, -1);
				if (ignoreInstances)
				{
					this.m_processingContext.ChunkManager.LeaveIgnoreInstances();
				}
				if (ignorePageBreaks)
				{
					this.m_processingContext.ChunkManager.LeaveIgnorePageBreakItem();
				}
				return result;
			}

			private ReportItemInstance CreateInstance(ReportItem reportItem, bool setupEnvironment, int index)
			{
				ReportItemInstance reportItemInstance = null;
				string text = null;
				if (setupEnvironment)
				{
					this.SetupEnvironment();
				}
				bool flag = reportItem is SubReport || reportItem is Rectangle || reportItem is DataRegion;
				this.m_processingContext.Pagination.EnterIgnorePageBreak(reportItem.Visibility, false);
				if (!(reportItem is Rectangle) && !(reportItem is DataRegion) && reportItem.Parent != null)
				{
					if (reportItem.Parent is Rectangle || reportItem.Parent is Report || reportItem.Parent is List)
					{
						bool softPageAtStart = this.m_processingContext.Pagination.CalculateSoftPageBreak(reportItem, 0.0, (double)reportItem.DistanceBeforeTop, false, false);
						this.m_processingContext.Pagination.SetReportItemStartPage(reportItem, softPageAtStart);
					}
					else
					{
						int num = reportItem.Parent.StartPage;
						if (reportItem.Parent is Table)
						{
							num = ((Table)reportItem.Parent).CurrentPage;
						}
						else if (reportItem.Parent is Matrix)
						{
							num = ((Matrix)reportItem.Parent).CurrentPage;
						}
						reportItem.StartPage = num;
						reportItem.EndPage = num;
					}
				}
				if (reportItem is TextBox)
				{
					reportItemInstance = RuntimeRICollection.CreateTextBoxInstance((TextBox)reportItem, this.m_processingContext, index, this.m_owner);
				}
				else if (reportItem is Line)
				{
					reportItemInstance = RuntimeRICollection.CreateLineInstance((Line)reportItem, this.m_processingContext, index);
				}
				else if (reportItem is Image)
				{
					reportItemInstance = RuntimeRICollection.CreateImageInstance((Image)reportItem, this.m_processingContext, index);
				}
				else if (reportItem is ActiveXControl)
				{
					reportItemInstance = RuntimeRICollection.CreateActiveXControlInstance((ActiveXControl)reportItem, this.m_processingContext, index);
				}
				else if (reportItem is SubReport)
				{
					reportItemInstance = RuntimeRICollection.CreateSubReportInstance((SubReport)reportItem, this.m_processingContext, index, this.m_owner, out text);
				}
				else if (reportItem is Rectangle)
				{
					Rectangle rectangle = (Rectangle)reportItem;
					this.m_processingContext.ChunkManager.CheckPageBreak(rectangle, true);
					bool softPageAtStart2 = this.m_processingContext.Pagination.CalculateSoftPageBreak(rectangle, 0.0, (double)rectangle.DistanceBeforeTop, false);
					this.m_processingContext.Pagination.SetReportItemStartPage(rectangle, softPageAtStart2);
					RectangleInstance rectangleInstance = new RectangleInstance(this.m_processingContext, rectangle, index);
					if (reportItem.Label != null)
					{
						text = this.m_processingContext.NavigationInfo.CurrentLabel;
						if (text != null)
						{
							this.m_processingContext.NavigationInfo.EnterDocumentMapChildren();
						}
					}
					if (Report.ShowHideTypes.Interactive == this.m_processingContext.ShowHideType)
					{
						((IShowHideContainer)rectangleInstance).BeginProcessContainer(this.m_processingContext);
					}
					this.m_processingContext.PageSectionContext.EnterVisibilityScope(rectangle.Visibility, rectangle.StartHidden);
					this.CreateInstances(rectangleInstance.ReportItemColInstance, rectangle.ReportItems);
					this.m_processingContext.PageSectionContext.ExitVisibilityScope();
					if (Report.ShowHideTypes.Interactive == this.m_processingContext.ShowHideType)
					{
						((IShowHideContainer)rectangleInstance).EndProcessContainer(this.m_processingContext);
					}
					this.m_processingContext.ChunkManager.CheckPageBreak(rectangle, false);
					this.m_processingContext.Pagination.ProcessEndPage(rectangleInstance, reportItem, rectangle.PageBreakAtEnd, false);
					reportItemInstance = rectangleInstance;
				}
				else if (reportItem is DataRegion)
				{
					DataRegion dataRegion = (DataRegion)reportItem;
					bool flag2 = true;
					RuntimeDataRegionObj runtimeDataRegionObj;
					if (this.m_dataRegionObjs != null)
					{
						runtimeDataRegionObj = this.m_dataRegionObjs[this.m_currDataRegion];
					}
					else
					{
						runtimeDataRegionObj = dataRegion.RuntimeDataRegionObj;
						dataRegion.RuntimeDataRegionObj = null;
					}
					bool flag3;
					if (reportItem is CustomReportItem && runtimeDataRegionObj == null)
					{
						Global.Tracer.Assert(null == ((CustomReportItem)reportItem).DataSetName, "(null == ((CustomReportItem)reportItem).DataSetName)");
						flag3 = false;
					}
					else
					{
						Global.Tracer.Assert(null != runtimeDataRegionObj, "(null != dataRegionObj)");
						runtimeDataRegionObj.SetupEnvironment();
						flag3 = true;
						this.m_processingContext.ChunkManager.CheckPageBreak(dataRegion, true);
						this.m_processingContext.ChunkManager.AddRepeatSiblings(dataRegion, index);
					}
					bool softPageAtStart3 = this.m_processingContext.Pagination.CalculateSoftPageBreak(dataRegion, 0.0, (double)dataRegion.DistanceBeforeTop, false);
					this.m_processingContext.Pagination.SetReportItemStartPage(dataRegion, softPageAtStart3);
					if (reportItem is List)
					{
						List list = (List)reportItem;
						list.ContentStartPage = list.StartPage;
						RuntimeOnePassListDetailObj runtimeOnePassListDetailObj = null;
						RenderingPagesRangesList renderingPagesRangesList = null;
						ListInstance listInstance;
						if (runtimeDataRegionObj is RuntimeOnePassListDetailObj)
						{
							runtimeOnePassListDetailObj = (RuntimeOnePassListDetailObj)runtimeDataRegionObj;
							renderingPagesRangesList = runtimeOnePassListDetailObj.ChildrenStartAndEndPages;
							listInstance = new ListInstance(this.m_processingContext, list, runtimeOnePassListDetailObj.ListContentInstances, renderingPagesRangesList);
							if (renderingPagesRangesList != null && (!this.m_processingContext.PageSectionContext.IsParentVisible() || !Visibility.IsOnePassHierarchyVisible(list)))
							{
								runtimeOnePassListDetailObj.MoveAllToFirstPage();
								int totalCount = (runtimeOnePassListDetailObj.ListContentInstances != null) ? runtimeOnePassListDetailObj.ListContentInstances.Count : 0;
								renderingPagesRangesList.MoveAllToFirstPage(totalCount);
								runtimeOnePassListDetailObj.NumberOfContentsOnThisPage = 0;
							}
							if (runtimeOnePassListDetailObj.NumberOfContentsOnThisPage > 0)
							{
								if (renderingPagesRangesList != null && 0 < renderingPagesRangesList.Count)
								{
									this.m_processingContext.Pagination.SetCurrentPageHeight(list, runtimeOnePassListDetailObj.Pagination.CurrentPageHeight);
								}
								else
								{
									this.m_processingContext.Pagination.AddToCurrentPageHeight(list, runtimeOnePassListDetailObj.Pagination.CurrentPageHeight);
								}
							}
							listInstance.NumberOfContentsOnThisPage = runtimeOnePassListDetailObj.NumberOfContentsOnThisPage;
							if (reportItem.Label != null)
							{
								text = this.m_processingContext.NavigationInfo.CurrentLabel;
							}
							this.m_processingContext.NavigationInfo.AppendNavigationInfo(text, runtimeOnePassListDetailObj.NavigationInfo, list.StartPage);
						}
						else
						{
							listInstance = new ListInstance(this.m_processingContext, list);
							bool delayAddingInstanceInfo = this.m_processingContext.DelayAddingInstanceInfo;
							this.m_processingContext.DelayAddingInstanceInfo = false;
							if (list.Label != null)
							{
								text = this.m_processingContext.NavigationInfo.CurrentLabel;
								if (text != null)
								{
									this.m_processingContext.NavigationInfo.EnterDocumentMapChildren();
								}
							}
							runtimeDataRegionObj.CreateInstances(listInstance, listInstance.ListContents, listInstance.ChildrenStartAndEndPages);
							this.m_processingContext.DelayAddingInstanceInfo = delayAddingInstanceInfo;
						}
						this.m_processingContext.Pagination.ProcessListRenderingPages(listInstance, list);
						if (this.m_processingContext.PageSectionContext.IsParentVisible() && runtimeOnePassListDetailObj != null)
						{
							if (renderingPagesRangesList != null && 0 < renderingPagesRangesList.Count)
							{
								for (int i = 0; i < renderingPagesRangesList.Count; i++)
								{
									runtimeOnePassListDetailObj.ProcessOnePassDetailTextboxes(i, list.StartPage + i);
								}
							}
							else if (listInstance.ListContents != null)
							{
								runtimeOnePassListDetailObj.ProcessOnePassDetailTextboxes(0, list.StartPage);
							}
						}
						reportItemInstance = listInstance;
					}
					else if (reportItem is Matrix)
					{
						MatrixHeadingInstance headingInstance = this.m_processingContext.HeadingInstance;
						MatrixHeadingInstance headingInstanceOld = this.m_processingContext.HeadingInstanceOld;
						this.m_processingContext.HeadingInstance = null;
						this.m_processingContext.HeadingInstanceOld = null;
						RuntimeMatrixObj runtimeMatrixObj = (RuntimeMatrixObj)runtimeDataRegionObj;
						MatrixInstance matrixInstance = new MatrixInstance(this.m_processingContext, (Matrix)reportItem);
						if (reportItem.Label != null)
						{
							text = this.m_processingContext.NavigationInfo.CurrentLabel;
							if (text != null)
							{
								this.m_processingContext.NavigationInfo.EnterDocumentMapChildren();
							}
						}
						if (Report.ShowHideTypes.Interactive == this.m_processingContext.ShowHideType)
						{
							((IShowHideContainer)matrixInstance).BeginProcessContainer(this.m_processingContext);
						}
						runtimeMatrixObj.CreateInstances(matrixInstance, matrixInstance.Cells, matrixInstance.ChildrenStartAndEndPages);
						if (Report.ShowHideTypes.Interactive == this.m_processingContext.ShowHideType)
						{
							((IShowHideContainer)matrixInstance).EndProcessContainer(this.m_processingContext);
						}
						if (setupEnvironment)
						{
							runtimeMatrixObj.ResetReportItems();
						}
						this.m_processingContext.HeadingInstance = headingInstance;
						this.m_processingContext.HeadingInstanceOld = headingInstanceOld;
						reportItemInstance = matrixInstance;
					}
					else if (reportItem is CustomReportItem)
					{
						CustomReportItem customReportItem = (CustomReportItem)reportItem;
						CustomReportItemInstance customReportItemInstance = new CustomReportItemInstance(this.m_processingContext, customReportItem);
						if (customReportItem.DataSetName != null)
						{
							bool delayAddingInstanceInfo2 = this.m_processingContext.DelayAddingInstanceInfo;
							this.m_processingContext.DelayAddingInstanceInfo = false;
							RuntimeCustomReportItemObj runtimeCustomReportItemObj = (RuntimeCustomReportItemObj)runtimeDataRegionObj;
							runtimeCustomReportItemObj.CreateInstances(customReportItemInstance, customReportItemInstance.Cells, null);
							this.m_processingContext.DelayAddingInstanceInfo = delayAddingInstanceInfo2;
							this.m_processingContext.Pagination.ProcessEndPage(customReportItemInstance, reportItem, customReportItem.PageBreakAtEnd, false);
						}
						else
						{
							flag2 = false;
						}
						AspNetCore.ReportingServices.ReportRendering.ICustomReportItem controlInstance = this.m_processingContext.CriProcessingControls.GetControlInstance(customReportItem.Type, this.m_processingContext.ExtFactory);
						if (controlInstance == null)
						{
							if (customReportItem.AltReportItem != null)
							{
								customReportItemInstance.AltReportItemColInstance = new ReportItemColInstance(this.m_processingContext, customReportItem.AltReportItem);
								Global.Tracer.Assert(1 == customReportItem.AltReportItem.Count, "(1 == criDef.AltReportItem.Count)");
								this.m_processingContext.RuntimeInitializeReportItemObjs(customReportItem.AltReportItem, false, false);
								this.CreateInstances(customReportItemInstance.AltReportItemColInstance, customReportItem.AltReportItem);
							}
						}
						else
						{
							CustomReportItemInstanceInfo instanceInfo = (CustomReportItemInstanceInfo)customReportItemInstance.GetInstanceInfo(null);
							customReportItem.CustomProcessingInitialize(customReportItemInstance, instanceInfo, this.m_processingContext, index);
							AspNetCore.ReportingServices.ReportRendering.CustomReportItem customItem = new AspNetCore.ReportingServices.ReportRendering.CustomReportItem(customReportItem, customReportItemInstance, instanceInfo);
							AspNetCore.ReportingServices.ReportRendering.ReportItem reportItem2 = null;
							try
							{
								controlInstance.CustomItem = customItem;
								controlInstance.Process();
								reportItem2 = controlInstance.RenderItem;
								if (reportItem2 != null)
								{
									reportItem2 = ((IDeepCloneable)reportItem2).DeepClone();
								}
								customReportItem.DeconstructRenderItem(reportItem2, customReportItemInstance);
								customReportItem.CustomProcessingReset();
							}
							catch (Exception innerException)
							{
								throw new ReportProcessingException(ErrorCode.rsCRIProcessingError, innerException, customReportItem.Name, customReportItem.Type);
							}
						}
						reportItemInstance = customReportItemInstance;
					}
					else if (reportItem is Chart)
					{
						RuntimeChartObj runtimeChartObj = (RuntimeChartObj)runtimeDataRegionObj;
						ChartInstance chartInstance = new ChartInstance(this.m_processingContext, (Chart)reportItem);
						bool delayAddingInstanceInfo3 = this.m_processingContext.DelayAddingInstanceInfo;
						this.m_processingContext.DelayAddingInstanceInfo = false;
						runtimeDataRegionObj.CreateInstances(chartInstance, chartInstance.MultiCharts, null);
						this.m_processingContext.DelayAddingInstanceInfo = delayAddingInstanceInfo3;
						this.m_processingContext.Pagination.ProcessEndPage(chartInstance, reportItem, ((Chart)reportItem).PageBreakAtEnd, false);
						reportItemInstance = chartInstance;
					}
					else if (reportItem is Table)
					{
						RuntimeTableObj runtimeTableObj = (RuntimeTableObj)runtimeDataRegionObj;
						Table table = (Table)reportItem;
						table.CurrentPage = reportItem.StartPage;
						TableInstance tableInstance = (runtimeTableObj.TableDetailInstances != null) ? new TableInstance(this.m_processingContext, table, runtimeTableObj.TableDetailInstances, runtimeTableObj.ChildrenStartAndEndPages) : new TableInstance(this.m_processingContext, table);
						if (table.Label != null)
						{
							text = this.m_processingContext.NavigationInfo.CurrentLabel;
							if (text != null)
							{
								this.m_processingContext.NavigationInfo.EnterDocumentMapChildren();
							}
						}
						if (Report.ShowHideTypes.Interactive == this.m_processingContext.ShowHideType)
						{
							((IShowHideContainer)tableInstance).BeginProcessContainer(this.m_processingContext);
						}
						runtimeTableObj.CreateInstances(tableInstance, tableInstance.TableGroupInstances, tableInstance.ChildrenStartAndEndPages);
						if (Report.ShowHideTypes.Interactive == this.m_processingContext.ShowHideType)
						{
							((IShowHideContainer)tableInstance).EndProcessContainer(this.m_processingContext);
						}
						if (setupEnvironment)
						{
							runtimeTableObj.ResetReportItems();
						}
						this.m_processingContext.Pagination.ProcessTableRenderingPages(tableInstance, table);
						reportItemInstance = tableInstance;
					}
					else if (reportItem is OWCChart)
					{
						OWCChartInstance oWCChartInstance;
						if (runtimeDataRegionObj is RuntimeOnePassOWCChartDetailObj)
						{
							oWCChartInstance = new OWCChartInstance(this.m_processingContext, (OWCChart)reportItem, ((RuntimeOnePassOWCChartDetailObj)runtimeDataRegionObj).OWCChartData);
						}
						else
						{
							oWCChartInstance = new OWCChartInstance(this.m_processingContext, (OWCChart)reportItem);
							bool delayAddingInstanceInfo4 = this.m_processingContext.DelayAddingInstanceInfo;
							this.m_processingContext.DelayAddingInstanceInfo = false;
							runtimeDataRegionObj.CreateInstances(oWCChartInstance, oWCChartInstance.InstanceInfo.ChartData, null);
							this.m_processingContext.DelayAddingInstanceInfo = delayAddingInstanceInfo4;
						}
						this.m_processingContext.ChunkManager.AddInstance(oWCChartInstance.InstanceInfo, oWCChartInstance, this.m_processingContext.InPageSection);
						this.m_processingContext.Pagination.ProcessEndPage(oWCChartInstance, reportItem, ((OWCChart)reportItem).PageBreakAtEnd, false);
						reportItemInstance = oWCChartInstance;
						if (this.m_processingContext.OWCChartName == reportItem.Name && this.m_processingContext.OWCChartInstance == null)
						{
							this.m_processingContext.OWCChartInstance = oWCChartInstance.InstanceInfo;
						}
					}
					if (flag3)
					{
						this.m_processingContext.ChunkManager.CheckPageBreak(dataRegion, false);
					}
					if (flag2 && this.m_dataRegionObjs != null)
					{
						this.m_dataRegionObjs[this.m_currDataRegion++] = null;
					}
				}
				if (!flag && reportItem.Label != null)
				{
					text = this.m_processingContext.NavigationInfo.CurrentLabel;
				}
				if (text != null)
				{
					this.m_processingContext.NavigationInfo.AddToDocumentMap(reportItemInstance.GetDocumentMapUniqueName(), flag, reportItem.StartPage, text);
				}
				if (reportItem.Parent != null)
				{
					if (reportItem.EndPage > reportItem.Parent.EndPage)
					{
						reportItem.Parent.EndPage = reportItem.EndPage;
						reportItem.Parent.BottomInEndPage = reportItem.BottomInEndPage;
						if (reportItem.Parent is List)
						{
							((List)reportItem.Parent).ContentStartPage = reportItem.EndPage;
						}
					}
					else if (reportItem.EndPage == reportItem.Parent.EndPage)
					{
						reportItem.Parent.BottomInEndPage = Math.Max(reportItem.Parent.BottomInEndPage, reportItem.BottomInEndPage);
					}
				}
				this.m_processingContext.Pagination.LeaveIgnorePageBreak(reportItem.Visibility, false);
				return reportItemInstance;
			}

			internal void ResetReportItemObjs()
			{
				if (!this.m_processingContext.ReportItemsReferenced && !this.m_processingContext.ReportItemThisDotValueReferenced)
				{
					return;
				}
				RuntimeRICollection.TraverseReportItemObjs(this.m_reportItemsDef, this.m_processingContext, true, this.m_owner);
			}

			internal static void ResetReportItemObjs(ReportItemCollection reportItems, ProcessingContext processingContext)
			{
				if (!processingContext.ReportItemsReferenced && !processingContext.ReportItemThisDotValueReferenced)
				{
					return;
				}
				RuntimeRICollection.TraverseReportItemObjs(reportItems, processingContext, true, null);
			}

			internal void SetReportItemObjScope()
			{
				if (!this.m_processingContext.ReportItemsReferenced && !this.m_processingContext.ReportItemThisDotValueReferenced)
				{
					return;
				}
				RuntimeRICollection.TraverseReportItemObjs(this.m_reportItemsDef, this.m_processingContext, false, this.m_owner);
			}

			private static void TraverseReportItemObjs(ReportItemCollection reportItems, ProcessingContext processingContext, bool reset, IScope scope)
			{
				if (reportItems != null && reportItems.ComputedReportItems != null)
				{
					for (int i = 0; i < reportItems.ComputedReportItems.Count; i++)
					{
						ReportItem reportItem = reportItems.ComputedReportItems[i];
						if (reportItem is TextBox)
						{
							TextBox textBox = (TextBox)reportItem;
							TextBoxImpl textBoxImpl = null;
							if (processingContext.ReportItemsReferenced)
							{
								textBoxImpl = (TextBoxImpl)((ReportItems)processingContext.ReportObjectModel.ReportItemsImpl)[textBox.Name];
							}
							else if (textBox.ValueReferenced)
							{
								textBoxImpl = (TextBoxImpl)textBox.TextBoxExprHost.ReportObjectModelTextBox;
							}
							if (textBoxImpl != null)
							{
								if (reset)
								{
									textBoxImpl.Reset();
								}
								else
								{
									textBoxImpl.Scope = scope;
								}
							}
						}
						else if (reportItem is Rectangle)
						{
							RuntimeRICollection.TraverseReportItemObjs(((Rectangle)reportItem).ReportItems, processingContext, reset, scope);
						}
						else if (reportItem is Table)
						{
							Table table = (Table)reportItem;
							if (table.HeaderRows != null)
							{
								for (int j = 0; j < table.HeaderRows.Count; j++)
								{
									RuntimeRICollection.TraverseReportItemObjs(table.HeaderRows[j].ReportItems, processingContext, reset, scope);
								}
							}
							if (table.FooterRows != null)
							{
								for (int k = 0; k < table.FooterRows.Count; k++)
								{
									RuntimeRICollection.TraverseReportItemObjs(table.FooterRows[k].ReportItems, processingContext, reset, scope);
								}
							}
						}
						else if (reportItem is Matrix)
						{
							Matrix matrix = (Matrix)reportItem;
							RuntimeRICollection.TraverseReportItemObjs(matrix.CornerReportItems, processingContext, reset, scope);
							RuntimeRICollection.TraverseReportItemObjs(matrix.Rows.ReportItems, processingContext, reset, scope);
							if (matrix.Rows.Subtotal != null)
							{
								RuntimeRICollection.TraverseReportItemObjs(matrix.Rows.Subtotal.ReportItems, processingContext, reset, scope);
							}
							RuntimeRICollection.TraverseReportItemObjs(matrix.Columns.ReportItems, processingContext, reset, scope);
							if (matrix.Columns.Subtotal != null)
							{
								RuntimeRICollection.TraverseReportItemObjs(matrix.Columns.Subtotal.ReportItems, processingContext, reset, scope);
							}
						}
					}
				}
			}

			internal static bool GetExternalImage(ProcessingContext processingContext, string currentPath, ObjectType objectType, string objectName, out byte[] imageData, out string mimeType)
			{
				imageData = null;
				mimeType = null;
				try
				{
					if (!processingContext.ReportContext.IsSupportedProtocol(currentPath, true))
					{
						processingContext.ErrorContext.Register(ProcessingErrorCode.rsUnsupportedProtocol, Severity.Error, objectType, objectName, "Value", currentPath.MarkAsPrivate(), "http://, https://, ftp://, file:, mailto:, or news:");
					}
					else
					{
						processingContext.GetResource(currentPath, out imageData, out mimeType);
						if (imageData != null && !Validator.ValidateMimeType(mimeType))
						{
							processingContext.ErrorContext.Register(ProcessingErrorCode.rsInvalidMIMEType, Severity.Warning, objectType, objectName, "MIMEType", mimeType.MarkAsPrivate());
							mimeType = null;
							imageData = null;
						}
					}
				}
				catch (Exception ex)
				{
					processingContext.ErrorContext.Register(ProcessingErrorCode.rsInvalidImageReference, Severity.Warning, objectType, objectName, "Value", ex.Message);
					return false;
				}
				return true;
			}

			internal static ActionInstance CreateActionInstance(ProcessingContext processingContext, IActionOwner actionOwner, int ownerUniqueName, ObjectType objectType, string objectName)
			{
				Action action = actionOwner.Action;
				if (action == null)
				{
					return null;
				}
				processingContext.ReportRuntime.CurrentActionOwner = actionOwner;
				ActionInstance actionInstance = null;
				object[] array = null;
				ActionItemInstanceList actionItemInstanceList = null;
				string text = objectName + ".ActionInfo";
				Style styleClass = action.StyleClass;
				if (styleClass != null && styleClass.ExpressionList != null && 0 < styleClass.ExpressionList.Count)
				{
					actionInstance = new ActionInstance(processingContext);
					array = new object[styleClass.ExpressionList.Count];
					RuntimeRICollection.EvaluateStyleAttributes(objectType, text, styleClass, actionInstance.UniqueName, array, processingContext);
					actionInstance.StyleAttributeValues = array;
				}
				if (action.ComputedActionItemsCount > 0)
				{
					if (actionInstance == null)
					{
						actionInstance = new ActionInstance(processingContext);
					}
					actionItemInstanceList = new ActionItemInstanceList();
					ActionItemInstance actionItemInstance = null;
					ActionItem actionItem = null;
					text += ".Action";
					for (int i = 0; i < action.ActionItems.Count; i++)
					{
						actionItem = action.ActionItems[i];
						if (actionItem.ComputedIndex >= 0)
						{
							actionItemInstance = RuntimeRICollection.CreateActionItemInstance(processingContext, actionItem, ownerUniqueName, objectType, text, i);
							actionItemInstanceList.Add(actionItemInstance);
						}
						else
						{
							actionItem.ProcessDrillthroughAction(processingContext, ownerUniqueName, i);
						}
					}
					actionInstance.ActionItemsValues = actionItemInstanceList;
				}
				else
				{
					action.ProcessDrillthroughAction(processingContext, ownerUniqueName);
				}
				return actionInstance;
			}

			internal static ActionItemInstance CreateActionItemInstance(ProcessingContext processingContext, ActionItem actionItemDef, int ownerUniqueName, ObjectType objectType, string objectName, int index)
			{
				if (actionItemDef == null)
				{
					return null;
				}
				ActionItemInstance actionItemInstance = new ActionItemInstance(processingContext, actionItemDef);
				actionItemInstance.HyperLinkURL = processingContext.ReportRuntime.EvaluateReportItemHyperlinkURLExpression(actionItemDef, actionItemDef.HyperLinkURL, objectType, objectName);
				string text2 = actionItemInstance.DrillthroughReportName = processingContext.ReportRuntime.EvaluateReportItemDrillthroughReportName(actionItemDef, actionItemDef.DrillthroughReportName, objectType, objectName);
				actionItemInstance.BookmarkLink = processingContext.ReportRuntime.EvaluateReportItemBookmarkLinkExpression(actionItemDef, actionItemDef.BookmarkLink, objectType, objectName);
				ParameterValueList drillthroughParameters = actionItemDef.DrillthroughParameters;
				object[] drillthroughParametersValues = actionItemInstance.DrillthroughParametersValues;
				BoolList drillthroughParametersOmits = actionItemInstance.DrillthroughParametersOmits;
				DrillthroughParameters drillthroughParameters2 = null;
				IntList intList = null;
				if (drillthroughParameters != null && drillthroughParametersValues != null)
				{
					for (int i = 0; i < drillthroughParameters.Count; i++)
					{
						bool flag = false;
						if (drillthroughParameters[i].Omit != null)
						{
							flag = processingContext.ReportRuntime.EvaluateParamValueOmitExpression(drillthroughParameters[i], objectType, objectName);
						}
						drillthroughParametersOmits.Add(flag);
						if (flag)
						{
							drillthroughParametersValues[i] = null;
						}
						else
						{
							drillthroughParametersValues[i] = processingContext.ReportRuntime.EvaluateParamVariantValueExpression(drillthroughParameters[i], objectType, objectName, "DrillthroughParameterValue");
							if (intList == null)
							{
								intList = new IntList();
							}
							if (drillthroughParameters2 == null)
							{
								drillthroughParameters2 = new DrillthroughParameters();
							}
							if (drillthroughParameters[i].Value != null && drillthroughParameters[i].Value.Type == ExpressionInfo.Types.Token)
							{
								intList.Add(drillthroughParameters[i].Value.IntValue);
								drillthroughParameters2.Add(drillthroughParameters[i].Name, null);
							}
							else
							{
								intList.Add(-1);
								drillthroughParameters2.Add(drillthroughParameters[i].Name, drillthroughParametersValues[i]);
							}
						}
					}
				}
				actionItemInstance.Label = processingContext.ReportRuntime.EvaluateActionLabelExpression(actionItemDef, actionItemDef.Label, objectType, objectName);
				if (text2 != null)
				{
					DrillthroughInformation drillthroughInfo = new DrillthroughInformation(text2, drillthroughParameters2, intList);
					string drillthroughId = ownerUniqueName.ToString(CultureInfo.InvariantCulture) + ":" + index.ToString(CultureInfo.InvariantCulture);
					processingContext.DrillthroughInfo.AddDrillthrough(drillthroughId, drillthroughInfo);
				}
				return actionItemInstance;
			}

			internal static TextBoxInstance CreateTextBoxInstance(TextBox textBox, ProcessingContext processingContext, int index, IScope containingScope)
			{
				TextBoxInstance textBoxInstance = new TextBoxInstance(processingContext, textBox, index);
				bool flag = textBox.IsSimpleTextBox();
				SimpleTextBoxInstanceInfo simpleTextBoxInstanceInfo = null;
				TextBoxInstanceInfo textBoxInstanceInfo = null;
				if (flag)
				{
					simpleTextBoxInstanceInfo = (SimpleTextBoxInstanceInfo)textBoxInstance.InstanceInfo;
				}
				else
				{
					textBoxInstanceInfo = (TextBoxInstanceInfo)textBoxInstance.InstanceInfo;
				}
				bool flag2 = false;
				if (textBox.Action != null)
				{
					flag2 = textBox.Action.ResetObjectModelForDrillthroughContext(processingContext.ReportObjectModel, textBox);
				}
				VariantResult textBoxResult;
				if (processingContext.ReportItemsReferenced)
				{
					TextBoxImpl textBoxImpl = (TextBoxImpl)((ReportItems)processingContext.ReportObjectModel.ReportItemsImpl)[textBox.Name];
					textBoxResult = textBoxImpl.GetResult();
				}
				else if (textBox.ValueReferenced)
				{
					Global.Tracer.Assert(textBox.TextBoxExprHost != null, "(textBox.TextBoxExprHost != null)");
					TextBoxImpl textBoxImpl2 = (TextBoxImpl)textBox.TextBoxExprHost.ReportObjectModelTextBox;
					textBoxResult = textBoxImpl2.GetResult();
				}
				else
				{
					textBoxResult = processingContext.ReportRuntime.EvaluateTextBoxValueExpression(textBox);
				}
				if (flag2)
				{
					textBox.Action.GetSelectedItemsForDrillthroughContext(processingContext.ReportObjectModel, textBox);
				}
				if (flag)
				{
					simpleTextBoxInstanceInfo.OriginalValue = textBoxResult.Value;
				}
				else
				{
					textBoxInstanceInfo.OriginalValue = textBoxResult.Value;
				}
				if (!flag)
				{
					textBoxInstanceInfo.Duplicate = RuntimeRICollection.CalculateDuplicates(textBoxResult, textBox, processingContext);
				}
				RuntimeRICollection.AddPageTextbox(processingContext, textBox, textBoxInstance, textBoxInstanceInfo, textBoxResult.Value);
				if (!(textBoxResult.Value is string))
				{
					string formattedTextBoxValue = RuntimeRICollection.GetFormattedTextBoxValue(textBoxInstanceInfo, textBoxResult, textBox, processingContext);
					if (flag)
					{
						simpleTextBoxInstanceInfo.FormattedValue = formattedTextBoxValue;
					}
					else
					{
						textBoxInstanceInfo.FormattedValue = formattedTextBoxValue;
					}
				}
				if (!flag)
				{
					textBoxInstanceInfo.Action = RuntimeRICollection.CreateActionInstance(processingContext, textBox, textBoxInstance.UniqueName, textBox.ObjectType, textBox.Name);
				}
				textBox.SetValueType(textBoxResult.Value);
				if (textBox.UserSort != null)
				{
					SortFilterEventInfo sortFilterEventInfo = new SortFilterEventInfo(textBox);
					sortFilterEventInfo.EventSourceScopeInfo = processingContext.GetScopeValues(textBox.ContainingScopes, containingScope);
					if (processingContext.NewSortFilterEventInfo == null)
					{
						processingContext.NewSortFilterEventInfo = new SortFilterEventInfoHashtable();
					}
					processingContext.NewSortFilterEventInfo.Add(textBoxInstance.UniqueName, sortFilterEventInfo);
					RuntimeSortFilterEventInfoList runtimeSortFilterEventInfoList = processingContext.RuntimeSortFilterInfo;
					if (runtimeSortFilterEventInfoList == null && -1 == processingContext.DataSetUniqueName)
					{
						runtimeSortFilterEventInfoList = processingContext.ReportRuntimeUserSortFilterInfo;
					}
					if (runtimeSortFilterEventInfoList != null)
					{
						for (int i = 0; i < runtimeSortFilterEventInfoList.Count; i++)
						{
							RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = runtimeSortFilterEventInfoList[i];
							runtimeSortFilterEventInfo.MatchEventSource(textBox, textBoxInstance, containingScope, processingContext);
						}
					}
				}
				return textBoxInstance;
			}

			private static void AddPageTextbox(ProcessingContext processingContext, TextBox textbox, TextBoxInstance textboxInstance, TextBoxInstanceInfo textboxInstanceInfo, object value)
			{
				if (processingContext.SubReportLevel == 0 && !processingContext.InPageSection && processingContext.PageSectionContext.HasPageSections && !processingContext.PageSectionContext.InMatrixSubtotal && processingContext.PageSectionContext.IsParentVisible() && Visibility.IsVisible(textbox, textboxInstance, textboxInstanceInfo))
				{
					int page = 0;
					if (!processingContext.PageSectionContext.InRepeatingItem)
					{
						if (processingContext.ReportRuntime.CurrentScope is RuntimeListGroupLeafObj)
						{
							page = ((RuntimeListGroupLeafObj)processingContext.ReportRuntime.CurrentScope).StartPage;
						}
						else if (processingContext.IsOnePass)
						{
							RuntimeOnePassDetailObj runtimeOnePassDetailObj = processingContext.ReportRuntime.CurrentScope as RuntimeOnePassDetailObj;
							if (runtimeOnePassDetailObj != null)
							{
								page = runtimeOnePassDetailObj.GetDetailPage();
								runtimeOnePassDetailObj.OnePassTextboxes.AddTextboxValue(page, textbox.Name, value);
								if (page == 0)
								{
									RuntimeOnePassTableDetailObj runtimeOnePassTableDetailObj = runtimeOnePassDetailObj as RuntimeOnePassTableDetailObj;
									if (runtimeOnePassTableDetailObj != null && !runtimeOnePassTableDetailObj.TextboxColumnPositions.ContainsKey(textbox.Name))
									{
										((RuntimeOnePassTableDetailObj)runtimeOnePassDetailObj).TextboxColumnPositions.Add(textbox.Name, processingContext.PageSectionContext.GetOnePassTableCellProperties());
									}
								}
								return;
							}
							page = processingContext.Pagination.GetTextBoxStartPage(textbox);
						}
						else
						{
							page = processingContext.Pagination.GetTextBoxStartPage(textbox);
						}
					}
					processingContext.PageSectionContext.PageTextboxes.AddTextboxValue(page, textbox.Name, value);
				}
			}

			internal static LineInstance CreateLineInstance(Line line, ProcessingContext processingContext, int index)
			{
				return new LineInstance(processingContext, line, index);
			}

			internal static ReportItemInstance CreateImageInstance(Image image, ProcessingContext processingContext, int index)
			{
				ImageInstance imageInstance = new ImageInstance(processingContext, image, index);
				ImageInstanceInfo instanceInfo = imageInstance.InstanceInfo;
				bool flag = false;
				bool flag2 = false;
				if (image.Action != null)
				{
					flag2 = image.Action.ResetObjectModelForDrillthroughContext(processingContext.ReportObjectModel, image);
				}
				switch (image.Source)
				{
				case Image.SourceType.External:
				{
					string text = processingContext.ReportRuntime.EvaluateImageStringValueExpression(image, out flag);
					if (flag2)
					{
						image.Action.GetSelectedItemsForDrillthroughContext(processingContext.ReportObjectModel, image);
					}
					instanceInfo.ImageValue = text;
					if (text != null && !processingContext.ImageStreamNames.ContainsKey(text))
					{
						string mimeType2 = null;
						byte[] array2 = null;
						if (ExpressionInfo.Types.Constant != image.Value.Type)
						{
							RuntimeRICollection.GetExternalImage(processingContext, text, image.ObjectType, image.Name, out array2, out mimeType2);
						}
						if (array2 == null)
						{
							instanceInfo.BrokenImage = true;
						}
						else if (processingContext.InPageSection && !processingContext.CreatePageSectionImageChunks)
						{
							instanceInfo.Data = new ImageData(array2, mimeType2);
						}
						else if (processingContext.CreateReportChunkCallback != null)
						{
							string text2 = Guid.NewGuid().ToString();
							using (Stream stream2 = processingContext.CreateReportChunkCallback(text2, ReportChunkTypes.Image, mimeType2))
							{
								stream2.Write(array2, 0, array2.Length);
							}
							processingContext.ImageStreamNames[text] = new ImageInfo(text2, mimeType2);
						}
					}
					break;
				}
				case Image.SourceType.Embedded:
				{
					string text3 = processingContext.ReportRuntime.EvaluateImageStringValueExpression(image, out flag);
					if (flag2)
					{
						image.Action.GetSelectedItemsForDrillthroughContext(processingContext.ReportObjectModel, image);
					}
					if (flag)
					{
						instanceInfo.BrokenImage = true;
					}
					else
					{
						instanceInfo.ImageValue = ProcessingValidator.ValidateEmbeddedImageName(text3, processingContext.EmbeddedImages, image.ObjectType, image.Name, "Value", processingContext.ErrorContext);
						instanceInfo.BrokenImage = (null != text3);
					}
					break;
				}
				case Image.SourceType.Database:
				{
					Global.Tracer.Assert(null != instanceInfo, "(null != imageInstanceInfo)");
					byte[] array = processingContext.ReportRuntime.EvaluateImageBinaryValueExpression(image, out flag);
					if (flag2)
					{
						image.Action.GetSelectedItemsForDrillthroughContext(processingContext.ReportObjectModel, image);
					}
					if (flag)
					{
						instanceInfo.BrokenImage = true;
						processingContext.ErrorContext.Register(ProcessingErrorCode.rsInvalidDatabaseImage, Severity.Warning, image.ObjectType, image.Name, "Value");
					}
					Global.Tracer.Assert(null != image.MIMEType, "(null != image.MIMEType)");
					string mimeType = (ExpressionInfo.Types.Constant == image.MIMEType.Type) ? image.MIMEType.Value : ProcessingValidator.ValidateMimeType(processingContext.ReportRuntime.EvaluateImageMIMETypeExpression(image), image.ObjectType, image.Name, "MIMEType", processingContext.ErrorContext);
					if (array != null)
					{
						if (processingContext.InPageSection && !processingContext.CreatePageSectionImageChunks)
						{
							instanceInfo.Data = new ImageData(array, mimeType);
						}
						else if (processingContext.CreateReportChunkCallback != null)
						{
							instanceInfo.ImageValue = Guid.NewGuid().ToString();
							using (Stream stream = processingContext.CreateReportChunkCallback(instanceInfo.ImageValue, ReportChunkTypes.Image, mimeType))
							{
								stream.Write(array, 0, array.Length);
							}
						}
					}
					break;
				}
				}
				if (instanceInfo != null && instanceInfo.ImageValue == null && !instanceInfo.BrokenImage && processingContext.CreateReportChunkCallback != null)
				{
					if (processingContext.TransparentImageGuid == null)
					{
						string mimeType3 = "image/gif";
						Guid guid = Guid.NewGuid();
						string text5 = processingContext.TransparentImageGuid = guid.ToString();
						using (Stream outputStream = processingContext.CreateReportChunkCallback(text5, ReportChunkTypes.Image, mimeType3))
						{
							RuntimeRICollection.FetchTransparentImage(outputStream);
						}
						processingContext.ImageStreamNames[text5] = new ImageInfo(text5, mimeType3);
						if (processingContext.EmbeddedImages != null)
						{
							processingContext.EmbeddedImages.Add(text5, new ImageInfo(text5, mimeType3));
						}
					}
					instanceInfo.ImageValue = processingContext.TransparentImageGuid;
				}
				instanceInfo.Action = RuntimeRICollection.CreateActionInstance(processingContext, image, imageInstance.UniqueName, image.ObjectType, image.Name);
				return imageInstance;
			}

			internal static void FetchTransparentImage(Stream outputStream)
			{
				byte[] transparentImageBytes = AspNetCore.ReportingServices.ReportIntermediateFormat.Image.TransparentImageBytes;
				outputStream.Write(transparentImageBytes, 0, transparentImageBytes.Length);
			}

			internal static ActiveXControlInstance CreateActiveXControlInstance(ActiveXControl activeXControl, ProcessingContext processingContext, int index)
			{
				ActiveXControlInstance activeXControlInstance = new ActiveXControlInstance(processingContext, activeXControl, index);
				ActiveXControlInstanceInfo instanceInfo = activeXControlInstance.InstanceInfo;
				ParameterValueList parameters = activeXControl.Parameters;
				object[] parameterValues = instanceInfo.ParameterValues;
				if (parameters != null && parameterValues != null)
				{
					for (int i = 0; i < parameters.Count; i++)
					{
						parameterValues[i] = processingContext.ReportRuntime.EvaluateParamVariantValueExpression(parameters[i], activeXControl.ObjectType, activeXControl.Name, "ParameterValue");
					}
				}
				return activeXControlInstance;
			}

			internal static void RetrieveSubReport(SubReport subReport, ProcessingContext processingContext, ProcessingErrorContext subReportErrorContext, bool isProcessingPrefetch)
			{
				Global.Tracer.Assert(isProcessingPrefetch || null != subReportErrorContext, "(isProcessingPrefetch || (null != subReportErrorContext))");
				try
				{
					ICatalogItemContext catalogItemContext = default(ICatalogItemContext);
					string description = default(string);
					GetReportChunk getChunkCallback = default(GetReportChunk);
					ParameterInfoCollection parametersFromCatalog = default(ParameterInfoCollection);
					try
					{
						if (!isProcessingPrefetch && processingContext.IsOnePass)
						{
							Monitor.Enter(processingContext.SubReportCallback);
						}
						processingContext.SubReportCallback(processingContext.ReportContext, subReport.ReportPath, out catalogItemContext, out description, out getChunkCallback, out parametersFromCatalog);
					}
					finally
					{
						if (!isProcessingPrefetch && processingContext.IsOnePass)
						{
							Monitor.Exit(processingContext.SubReportCallback);
						}
					}
					Global.Tracer.Assert(null != catalogItemContext, "(null != subreportContext)");
					subReport.ReportContext = catalogItemContext;
					subReport.Report = ReportProcessing.DeserializeReport(getChunkCallback, subReport);
					subReport.Report = RuntimeRICollection.AssignIDsForSubReport(subReport, processingContext, (SubReportInitialization)((!isProcessingPrefetch) ? 2 : 0));
					subReport.RetrievalStatus = (SubReport.Status)((!isProcessingPrefetch) ? 1 : 3);
					subReport.Description = description;
					subReport.ReportName = catalogItemContext.ItemName;
					subReport.StringUri = new CatalogItemUrlBuilder(catalogItemContext).ToString();
					subReport.ParametersFromCatalog = parametersFromCatalog;
				}
				catch (VersionMismatchException)
				{
					throw;
				}
				catch (Exception ex2)
				{
					if (ex2 is DataCacheUnavailableException)
					{
						throw;
					}
					RuntimeRICollection.HandleSubReportProcessingError(processingContext, subReport, subReportErrorContext, ex2);
					subReport.ReportContext = null;
					subReport.Report = null;
					subReport.Description = null;
					subReport.ReportName = null;
					subReport.StringUri = null;
					subReport.ParametersFromCatalog = null;
					subReport.RetrievalStatus = SubReport.Status.RetrieveFailed;
				}
			}

			private static SubReportInstance CreateSubReportInstance(SubReport subReport, ProcessingContext processingContext, int index, IScope containingScope, out string label)
			{
				processingContext.ChunkManager.CheckPageBreak(subReport, true);
				processingContext.PageSectionContext.EnterSubreport();
				SubReportInstance subReportInstance = new SubReportInstance(processingContext, subReport, index);
				if (subReport.Label != null)
				{
					label = processingContext.NavigationInfo.CurrentLabel;
					if (label != null)
					{
						processingContext.NavigationInfo.EnterDocumentMapChildren();
					}
				}
				else
				{
					label = null;
				}
				bool delayAddingInstanceInfo = processingContext.DelayAddingInstanceInfo;
				processingContext.DelayAddingInstanceInfo = false;
				ProcessingErrorContext processingErrorContext = new ProcessingErrorContext();
				bool firstSubreportInstance = false;
				if (processingContext.SubReportLevel <= 20)
				{
					if (SubReport.Status.PreFetched == subReport.RetrievalStatus)
					{
						firstSubreportInstance = true;
						subReport.Report = RuntimeRICollection.AssignIDsForSubReport(subReport, processingContext, SubReportInitialization.RuntimeOnly);
						subReport.RetrievalStatus = SubReport.Status.Retrieved;
					}
					else if (subReport.RetrievalStatus == SubReport.Status.NotRetrieved && processingContext.SubReportCallback != null)
					{
						RuntimeRICollection.RetrieveSubReport(subReport, processingContext, processingErrorContext, false);
						firstSubreportInstance = true;
					}
					else if (processingContext.SnapshotProcessing)
					{
						subReport.ReportContext = processingContext.ReportContext.GetSubreportContext(subReport.ReportPath);
					}
					if (containingScope == null)
					{
						containingScope = processingContext.UserSortFilterContext.CurrentContainingScope;
					}
					VariantList[] scopeValues = processingContext.GetScopeValues(subReport.ContainingScopes, containingScope);
					int num;
					if (processingContext.SnapshotProcessing && subReport.DataSetUniqueNameMap != null && !subReport.SaveDataSetUniqueName)
					{
						num = subReport.GetDataSetUniqueName(scopeValues);
					}
					else
					{
						subReport.AddDataSetUniqueName(scopeValues, subReportInstance.UniqueName);
						num = subReportInstance.UniqueName;
					}
					if (SubReport.Status.Retrieved == subReport.RetrievalStatus)
					{
						CultureInfo cultureInfo = null;
						try
						{
							ParameterInfoCollection parameterInfoCollection = new ParameterInfoCollection();
							if (subReport.Parameters != null && subReport.ParametersFromCatalog != null)
							{
								for (int i = 0; i < subReport.Parameters.Count; i++)
								{
									string name = subReport.Parameters[i].Name;
									ParameterInfo parameterInfo = subReport.ParametersFromCatalog[name];
									if (parameterInfo == null)
									{
										throw new UnknownReportParameterException(name);
									}
									ParameterValueResult parameterValueResult = processingContext.ReportRuntime.EvaluateParameterValueExpression(subReport.Parameters[i], subReport.ObjectType, subReport.Name, "ParameterValue");
									if (parameterValueResult.ErrorOccurred)
									{
										throw new ReportProcessingException(ErrorCode.rsReportParameterProcessingError, name);
									}
									object[] array = null;
									object[] array2 = parameterValueResult.Value as object[];
									array = ((array2 == null) ? new object[1]
									{
										parameterValueResult.Value
									} : array2);
									ParameterInfo parameterInfo2 = new ParameterInfo();
									parameterInfo2.Name = name;
									parameterInfo2.Values = array;
									parameterInfo2.DataType = parameterValueResult.Type;
									parameterInfoCollection.Add(parameterInfo2);
								}
							}
							ParameterInfoCollection parameterInfoCollection2 = ParameterInfoCollection.Combine(subReport.ParametersFromCatalog, parameterInfoCollection, true, false, false, false, Localization.ClientPrimaryCulture);
							ProcessingContext subReportParametersAndContext = RuntimeRICollection.GetSubReportParametersAndContext(processingContext, subReport, num, parameterInfoCollection2, processingErrorContext);
							subReportParametersAndContext.AbortInfo.AddSubreportInstance(num);
							cultureInfo = Thread.CurrentThread.CurrentCulture;
							subReportParametersAndContext.UserSortFilterContext.CurrentContainingScope = containingScope;
							Merge merge = new Merge(subReport.Report, subReportParametersAndContext, firstSubreportInstance);
							if (!subReportParametersAndContext.SnapshotProcessing && !subReportParametersAndContext.ProcessWithCachedData && merge.PrefetchData(parameterInfoCollection2, subReport.MergeTransactions))
							{
								subReportParametersAndContext.SnapshotProcessing = true;
							}
							subReportInstance.ReportInstance = merge.Process(parameterInfoCollection2, subReport.MergeTransactions);
							Thread.CurrentThread.CurrentCulture = cultureInfo;
							cultureInfo = null;
							if (subReport.Report.HasImageStreams)
							{
								processingContext.HasImageStreams = true;
							}
							if (processingErrorContext.Messages != null && 0 < processingErrorContext.Messages.Count)
							{
								processingContext.ErrorContext.Register(ProcessingErrorCode.rsWarningExecutingSubreport, Severity.Warning, subReport.ObjectType, subReport.Name, null, processingErrorContext.Messages);
							}
						}
						catch (Exception ex)
						{
							if (ex.InnerException is DataCacheUnavailableException)
							{
								throw;
							}
							RuntimeRICollection.HandleSubReportProcessingError(processingContext, subReport, processingErrorContext, ex);
							subReportInstance.ReportInstance = null;
						}
						finally
						{
							if (cultureInfo != null)
							{
								Thread.CurrentThread.CurrentCulture = cultureInfo;
							}
						}
					}
				}
				processingContext.PageSectionContext.ExitSubreport();
				processingContext.ChunkManager.CheckPageBreak(subReport, false);
				processingContext.DelayAddingInstanceInfo = delayAddingInstanceInfo;
				processingContext.Pagination.ProcessEndPage(subReportInstance, subReport, false, false);
				return subReportInstance;
			}

			private static ProcessingContext GetSubReportParametersAndContext(ProcessingContext processingContext, SubReport subReport, int subReportDataSetUniqueName, ParameterInfoCollection effectiveParameters, ProcessingErrorContext subReportErrorContext)
			{
				ProcessingContext processingContext2 = processingContext.ParametersContext(subReport.ReportContext, subReportErrorContext);
				if (processingContext.ResetForSubreportDataPrefetch)
				{
					processingContext2.SnapshotProcessing = false;
				}
				ReportProcessing.ProcessReportParameters(subReport.Report, processingContext2, effectiveParameters);
				if (!effectiveParameters.ValuesAreValid())
				{
					throw new ReportProcessingException(ErrorCode.rsParametersNotSpecified);
				}
				ProcessingContext processingContext3 = processingContext.SubReportContext(subReport, subReportDataSetUniqueName, subReportErrorContext);
				if (processingContext.ResetForSubreportDataPrefetch)
				{
					processingContext3.SnapshotProcessing = false;
				}
				return processingContext3;
			}

			private static void HandleSubReportProcessingError(ProcessingContext processingContext, SubReport subReport, ProcessingErrorContext subReportErrorContext, Exception e)
			{
				Global.Tracer.Assert(e != null, "(e != null)");
				if (!(e is ProcessingAbortedException) && Global.Tracer.TraceError)
				{
					Global.Tracer.Trace(TraceLevel.Error, "An error has occurred while processing a sub-report. Details: {0} Stack trace:\r\n{1}", e.Message, e.StackTrace);
				}
				if (subReportErrorContext == null)
				{
					processingContext.ErrorContext.Register(ProcessingErrorCode.rsErrorExecutingSubreport, Severity.Warning, subReport.ObjectType, subReport.Name, null, e.Message);
				}
				else
				{
					Global.Tracer.Assert(null != subReportErrorContext, "(null != subReportErrorContext)");
					if (e is RSException)
					{
						subReportErrorContext.Register((RSException)e, subReport.ObjectType);
					}
					processingContext.ErrorContext.Register(ProcessingErrorCode.rsErrorExecutingSubreport, Severity.Warning, subReport.ObjectType, subReport.Name, null, subReportErrorContext.Messages, e.Message);
				}
			}

			private static Report AssignIDsForSubReport(SubReport subReport, ProcessingContext context, SubReportInitialization initializationAction)
			{
				Report report = subReport.Report;
				if (initializationAction != 0)
				{
					subReport.UpdateSubReportScopes(context.UserSortFilterContext);
				}
				if (report != null)
				{
					ArrayList arrayList = null;
					Hashtable iDMap = null;
					if (SubReportInitialization.RuntimeOnly != initializationAction)
					{
						RuntimeRICollection.AssignIDsForIDOwnerBase(report, context);
						report.BodyID = context.CreateIDForSubreport();
						arrayList = new ArrayList();
						arrayList.Add(report);
						iDMap = new Hashtable();
						RuntimeRICollection.AssignIDsForDataSets(report.DataSources, context, iDMap);
					}
					RuntimeRICollection.AssignIDsForReportItemCollection(report.ReportItems, context, iDMap, subReport, arrayList, initializationAction);
					RuntimeRICollection.AssignIDsForPageSection(report.PageHeader, context, iDMap, subReport, arrayList, initializationAction);
					RuntimeRICollection.AssignIDsForPageSection(report.PageFooter, context, iDMap, subReport, arrayList, initializationAction);
					if (SubReportInitialization.RuntimeOnly != initializationAction)
					{
						for (int i = 0; i < arrayList.Count; i++)
						{
							object obj = arrayList[i];
							if (obj is Report)
							{
								((Report)obj).NonDetailSortFiltersInScope = RuntimeRICollection.UpdateSortFilterTable(((Report)obj).NonDetailSortFiltersInScope, iDMap);
								((Report)obj).DetailSortFiltersInScope = RuntimeRICollection.UpdateSortFilterTable(((Report)obj).DetailSortFiltersInScope, iDMap);
							}
							else if (obj is DataRegion)
							{
								((DataRegion)obj).DetailSortFiltersInScope = RuntimeRICollection.UpdateSortFilterTable(((DataRegion)obj).DetailSortFiltersInScope, iDMap);
							}
							else if (obj is Grouping)
							{
								((Grouping)obj).NonDetailSortFiltersInScope = RuntimeRICollection.UpdateSortFilterTable(((Grouping)obj).NonDetailSortFiltersInScope, iDMap);
								((Grouping)obj).DetailSortFiltersInScope = RuntimeRICollection.UpdateSortFilterTable(((Grouping)obj).DetailSortFiltersInScope, iDMap);
							}
						}
					}
				}
				return report;
			}

			private static InScopeSortFilterHashtable UpdateSortFilterTable(InScopeSortFilterHashtable oldTable, Hashtable IDMap)
			{
				if (oldTable != null)
				{
					InScopeSortFilterHashtable inScopeSortFilterHashtable = new InScopeSortFilterHashtable(oldTable.Count);
					IDictionaryEnumerator enumerator = oldTable.GetEnumerator();
					while (enumerator.MoveNext())
					{
						int num = (int)enumerator.Key;
						IntList intList = (IntList)enumerator.Value;
						IntList intList2 = new IntList(intList.Count);
						for (int i = 0; i < intList.Count; i++)
						{
							intList2.Add((int)IDMap[intList[i]]);
						}
						inScopeSortFilterHashtable.Add(IDMap[num], intList2);
					}
					return inScopeSortFilterHashtable;
				}
				return null;
			}

			private static void AssignIDsForDataSets(DataSourceList dataSources, ProcessingContext context, Hashtable IDMap)
			{
				if (dataSources != null)
				{
					for (int i = 0; i < dataSources.Count; i++)
					{
						DataSource dataSource = dataSources[i];
						if (dataSource.DataSets != null)
						{
							for (int j = 0; j < dataSource.DataSets.Count; j++)
							{
								DataSet dataSet = dataSource.DataSets[j];
								int iD = dataSet.ID;
								RuntimeRICollection.AssignIDsForIDOwnerBase(dataSet, context);
								RuntimeRICollection.AddToIDMap(dataSet, iD, IDMap);
							}
						}
					}
				}
			}

			private static void AssignIDsForPageSection(PageSection pageSection, ProcessingContext context, Hashtable IDMap, SubReport subReport, ArrayList sortFilterOwners, SubReportInitialization initializationAction)
			{
				if (pageSection != null)
				{
					if (SubReportInitialization.RuntimeOnly != initializationAction)
					{
						RuntimeRICollection.AssignIDsForIDOwnerBase(pageSection, context);
					}
					RuntimeRICollection.AssignIDsForReportItemCollection(pageSection.ReportItems, context, IDMap, subReport, sortFilterOwners, initializationAction);
				}
			}

			private static void AssignIDsForReportItemCollection(ReportItemCollection reportItems, ProcessingContext context, Hashtable IDMap, SubReport subReport, ArrayList sortFilterOwners, SubReportInitialization initializationAction)
			{
				if (reportItems != null)
				{
					if (SubReportInitialization.RuntimeOnly != initializationAction)
					{
						RuntimeRICollection.AssignIDsForIDOwnerBase(reportItems, context);
					}
					for (int i = 0; i < reportItems.Count; i++)
					{
						ReportItem reportItem = reportItems[i];
						int iD = reportItem.ID;
						if (SubReportInitialization.RuntimeOnly != initializationAction)
						{
							RuntimeRICollection.AssignIDsForIDOwnerBase(reportItem, context);
						}
						if (reportItem is TextBox)
						{
							if (IDMap != null)
							{
								IDMap.Add(iD, reportItem.ID);
							}
							TextBox textBox = (TextBox)reportItem;
							EndUserSort userSort = textBox.UserSort;
							if (userSort != null)
							{
								if (-1 != context.UserSortFilterContext.DataSetID)
								{
									userSort.DataSetID = context.UserSortFilterContext.DataSetID;
								}
								else if (IDMap != null)
								{
									userSort.DataSetID = (int)IDMap[userSort.DataSetID];
								}
								userSort.DetailScopeSubReports = subReport.DetailScopeSubReports;
							}
							if (initializationAction != 0 && subReport.ContainingScopes != null && 0 < subReport.ContainingScopes.Count)
							{
								if (textBox.ContainingScopes != null && 0 < textBox.ContainingScopes.Count)
								{
									textBox.ContainingScopes.InsertRange(0, subReport.ContainingScopes);
								}
								else
								{
									textBox.IsSubReportTopLevelScope = true;
									textBox.ContainingScopes = subReport.ContainingScopes;
								}
							}
						}
						else if (reportItem is Rectangle)
						{
							RuntimeRICollection.AssignIDsForReportItemCollection(((Rectangle)reportItem).ReportItems, context, IDMap, subReport, sortFilterOwners, initializationAction);
						}
						else if (reportItem is List)
						{
							List list = (List)reportItem;
							int iD2 = list.HierarchyDef.ID;
							if (SubReportInitialization.RuntimeOnly != initializationAction)
							{
								RuntimeRICollection.AssignIDsForIDOwnerBase(list.HierarchyDef, context);
								RuntimeRICollection.AddToIDMap(list.HierarchyDef.Grouping, iD2, IDMap);
								RuntimeRICollection.AddToSortFilterOwners(list.HierarchyDef, sortFilterOwners);
							}
							RuntimeRICollection.AssignIDsForReportItemCollection(list.ReportItems, context, IDMap, subReport, sortFilterOwners, initializationAction);
						}
						else if (reportItem is Matrix)
						{
							Matrix matrix = (Matrix)reportItem;
							RuntimeRICollection.AssignIDsForReportItemCollection(matrix.CornerReportItems, context, IDMap, subReport, sortFilterOwners, initializationAction);
							RuntimeRICollection.AssignIDsForMatrixHeading(matrix.Columns, context, IDMap, subReport, sortFilterOwners, initializationAction);
							RuntimeRICollection.AssignIDsForMatrixHeading(matrix.Rows, context, IDMap, subReport, sortFilterOwners, initializationAction);
							RuntimeRICollection.AssignIDsForReportItemCollection(matrix.CellReportItems, context, IDMap, subReport, sortFilterOwners, initializationAction);
							if (SubReportInitialization.RuntimeOnly != initializationAction)
							{
								RuntimeRICollection.AssignArrayOfIDs(matrix.CellIDs, context);
							}
						}
						else if (reportItem is Table)
						{
							Table table = (Table)reportItem;
							RuntimeRICollection.AssignIDsForTableRows(table.HeaderRows, context, IDMap, subReport, sortFilterOwners, initializationAction);
							RuntimeRICollection.AssignIDsForTableGroup(table.TableGroups, context, IDMap, subReport, sortFilterOwners, initializationAction);
							RuntimeRICollection.AssignIDsForTableDetail(table.TableDetail, context, IDMap, subReport, sortFilterOwners, initializationAction);
							RuntimeRICollection.AssignIDsForTableRows(table.FooterRows, context, IDMap, subReport, sortFilterOwners, initializationAction);
						}
						else if (reportItem is Chart)
						{
							if (SubReportInitialization.RuntimeOnly != initializationAction)
							{
								Chart chart = (Chart)reportItem;
								RuntimeRICollection.AssignIDsForChartHeading(chart.Columns, context, IDMap);
								RuntimeRICollection.AssignIDsForChartHeading(chart.Rows, context, IDMap);
								if (chart.MultiChart != null)
								{
									RuntimeRICollection.AssignIDsForIDOwnerBase(chart.MultiChart, context);
								}
							}
						}
						else if (reportItem is CustomReportItem)
						{
							CustomReportItem customReportItem = (CustomReportItem)reportItem;
							if (SubReportInitialization.RuntimeOnly != initializationAction)
							{
								RuntimeRICollection.AssignIDsForCRIHeading(customReportItem.Columns, context, IDMap);
								RuntimeRICollection.AssignIDsForCRIHeading(customReportItem.Rows, context, IDMap);
							}
							if (customReportItem.AltReportItem != null)
							{
								RuntimeRICollection.AssignIDsForReportItemCollection(customReportItem.AltReportItem, context, IDMap, subReport, sortFilterOwners, initializationAction);
							}
							if (customReportItem.RenderReportItem != null)
							{
								RuntimeRICollection.AssignIDsForReportItemCollection(customReportItem.RenderReportItem, context, IDMap, subReport, sortFilterOwners, initializationAction);
							}
						}
						if (SubReportInitialization.RuntimeOnly != initializationAction && reportItem is DataRegion)
						{
							RuntimeRICollection.AddToIDMap((DataRegion)reportItem, iD, IDMap);
							sortFilterOwners.Add(reportItem);
						}
					}
				}
			}

			private static void AddToIDMap(ISortFilterScope scope, int oldID, Hashtable IDMap)
			{
				if (scope != null)
				{
					IDMap.Add(oldID, scope.ID);
				}
			}

			private static void AddToSortFilterOwners(ReportHierarchyNode scope, ArrayList sortFilterOwners)
			{
				if (scope.Grouping != null)
				{
					sortFilterOwners.Add(scope.Grouping);
				}
			}

			private static void AssignIDsForPivotHeading(PivotHeading heading, ProcessingContext context, Hashtable IDMap)
			{
				if (heading != null)
				{
					int iD = heading.ID;
					RuntimeRICollection.AssignIDsForIDOwnerBase(heading, context);
					RuntimeRICollection.AddToIDMap(heading.Grouping, iD, IDMap);
					RuntimeRICollection.AssignArrayOfIDs(heading.IDs, context);
				}
			}

			private static void AssignIDsForTablixHeading(TablixHeading heading, ProcessingContext context, Hashtable IDMap)
			{
				if (heading != null)
				{
					int iD = heading.ID;
					RuntimeRICollection.AssignIDsForIDOwnerBase(heading, context);
					RuntimeRICollection.AddToIDMap(heading.Grouping, iD, IDMap);
				}
			}

			private static void AssignIDsForCRIHeading(CustomReportItemHeadingList headings, ProcessingContext context, Hashtable IDMap)
			{
				if (headings != null)
				{
					for (int i = 0; i < headings.Count; i++)
					{
						RuntimeRICollection.AssignIDsForTablixHeading(headings[i], context, IDMap);
						RuntimeRICollection.AssignIDsForCRIHeading(headings[i].InnerHeadings, context, IDMap);
					}
				}
			}

			private static void AssignIDsForChartHeading(ChartHeading heading, ProcessingContext context, Hashtable IDMap)
			{
				if (heading != null)
				{
					RuntimeRICollection.AssignIDsForPivotHeading(heading, context, IDMap);
					RuntimeRICollection.AssignIDsForChartHeading(heading.SubHeading, context, IDMap);
				}
			}

			private static void AssignIDsForMatrixHeading(MatrixHeading heading, ProcessingContext context, Hashtable IDMap, SubReport subReport, ArrayList sortFilterOwners, SubReportInitialization initializationAction)
			{
				if (heading != null)
				{
					if (SubReportInitialization.RuntimeOnly != initializationAction)
					{
						RuntimeRICollection.AssignIDsForPivotHeading(heading, context, IDMap);
						RuntimeRICollection.AddToSortFilterOwners(heading, sortFilterOwners);
					}
					RuntimeRICollection.AssignIDsForMatrixHeading(heading.SubHeading, context, IDMap, subReport, sortFilterOwners, initializationAction);
					RuntimeRICollection.AssignIDsForSubtotal(heading.Subtotal, context, IDMap, subReport, sortFilterOwners, initializationAction);
					RuntimeRICollection.AssignIDsForReportItemCollection(heading.ReportItems, context, IDMap, subReport, sortFilterOwners, initializationAction);
				}
			}

			private static void AssignIDsForSubtotal(Subtotal subtotal, ProcessingContext context, Hashtable IDMap, SubReport subReport, ArrayList sortFilterOwners, SubReportInitialization initializationAction)
			{
				if (subtotal != null)
				{
					if (SubReportInitialization.RuntimeOnly != initializationAction)
					{
						RuntimeRICollection.AssignIDsForIDOwnerBase(subtotal, context);
					}
					RuntimeRICollection.AssignIDsForReportItemCollection(subtotal.ReportItems, context, IDMap, subReport, sortFilterOwners, initializationAction);
				}
			}

			private static void AssignIDsForTableRows(TableRowList rows, ProcessingContext context, Hashtable IDMap, SubReport subReport, ArrayList sortFilterOwners, SubReportInitialization initializationAction)
			{
				if (rows != null)
				{
					for (int i = 0; i < rows.Count; i++)
					{
						if (rows[i] != null)
						{
							if (SubReportInitialization.RuntimeOnly != initializationAction)
							{
								RuntimeRICollection.AssignIDsForIDOwnerBase(rows[i], context);
							}
							RuntimeRICollection.AssignIDsForReportItemCollection(rows[i].ReportItems, context, IDMap, subReport, sortFilterOwners, initializationAction);
							if (SubReportInitialization.RuntimeOnly != initializationAction)
							{
								RuntimeRICollection.AssignArrayOfIDs(rows[i].IDs, context);
							}
						}
					}
				}
			}

			private static void AssignIDsForTableGroup(TableGroup group, ProcessingContext context, Hashtable IDMap, SubReport subReport, ArrayList sortFilterOwners, SubReportInitialization initializationAction)
			{
				if (group != null)
				{
					int iD = group.ID;
					if (SubReportInitialization.RuntimeOnly != initializationAction)
					{
						RuntimeRICollection.AssignIDsForIDOwnerBase(group, context);
						RuntimeRICollection.AddToIDMap(group.Grouping, iD, IDMap);
						RuntimeRICollection.AddToSortFilterOwners(group, sortFilterOwners);
					}
					RuntimeRICollection.AssignIDsForTableGroup(group.SubGroup, context, IDMap, subReport, sortFilterOwners, initializationAction);
					RuntimeRICollection.AssignIDsForTableRows(group.HeaderRows, context, IDMap, subReport, sortFilterOwners, initializationAction);
					RuntimeRICollection.AssignIDsForTableRows(group.FooterRows, context, IDMap, subReport, sortFilterOwners, initializationAction);
				}
			}

			private static void AssignIDsForTableDetail(TableDetail detail, ProcessingContext context, Hashtable IDMap, SubReport subReport, ArrayList sortFilterOwners, SubReportInitialization initializationAction)
			{
				if (detail != null)
				{
					if (SubReportInitialization.RuntimeOnly != initializationAction)
					{
						RuntimeRICollection.AssignIDsForIDOwnerBase(detail, context);
					}
					RuntimeRICollection.AssignIDsForTableRows(detail.DetailRows, context, IDMap, subReport, sortFilterOwners, initializationAction);
				}
			}

			private static void AssignIDsForIDOwnerBase(IDOwner idOwner, ProcessingContext context)
			{
				if (idOwner != null)
				{
					idOwner.ID = context.CreateIDForSubreport();
				}
			}

			private static void AssignArrayOfIDs(IntList ids, ProcessingContext context)
			{
				if (ids != null)
				{
					for (int i = 0; i < ids.Count; i++)
					{
						ids[i] = context.CreateIDForSubreport();
					}
				}
			}

			private static bool CalculateDuplicates(VariantResult textBoxResult, TextBox textBox, ProcessingContext processingContext)
			{
				bool flag = false;
				if (textBox.HideDuplicates != null)
				{
					if (textBox.HasOldResult)
					{
						if (textBoxResult.ErrorOccurred && textBox.OldResult.ErrorOccurred)
						{
							flag = true;
						}
						else if (textBoxResult.ErrorOccurred)
						{
							flag = false;
						}
						else if (textBox.OldResult.ErrorOccurred)
						{
							flag = false;
						}
						else if (textBoxResult.Value == null && textBox.OldResult.Value == null)
						{
							flag = true;
						}
						else if (textBoxResult.Value == null)
						{
							flag = false;
						}
						else if (textBoxResult.Value.Equals(textBox.OldResult.Value))
						{
							flag = true;
						}
					}
					if (!flag)
					{
						textBox.OldResult = textBoxResult;
					}
				}
				return flag;
			}

			private static string GetFormattedTextBoxValue(TextBoxInstanceInfo textBoxInstanceInfo, VariantResult textBoxResult, TextBox textBox, ProcessingContext processingContext)
			{
				if (textBoxResult.ErrorOccurred)
				{
					return RPRes.rsExpressionErrorValue;
				}
				if (textBoxResult.Value == null)
				{
					return null;
				}
				if (textBoxInstanceInfo != null && textBoxInstanceInfo.Duplicate && textBox.SharedFormatSettings)
				{
					return textBox.FormattedValue;
				}
				Type type = textBoxResult.Value.GetType();
				TypeCode typeCode = Type.GetTypeCode(type);
				string text = RuntimeRICollection.FormatTextboxValue(textBoxInstanceInfo, textBoxResult.Value, textBox, typeCode, processingContext);
				if (textBox.HideDuplicates != null)
				{
					textBox.FormattedValue = text;
				}
				return text;
			}

			private static int GetTextBoxStyleAttribute(Style style, string styleAttributeName, TextBoxInstanceInfo textBoxInstanceInfo, ref bool sharedFormatSettings, out string styleStringValue)
			{
				styleStringValue = null;
				int result = 0;
				object obj = null;
				AttributeInfo attributeInfo = style.StyleAttributes[styleAttributeName];
				if (attributeInfo != null)
				{
					if (attributeInfo.IsExpression)
					{
						result = 1;
						sharedFormatSettings = false;
						obj = textBoxInstanceInfo.GetStyleAttributeValue(attributeInfo.IntValue);
					}
					else
					{
						result = 2;
						obj = attributeInfo.Value;
					}
				}
				if (obj != null)
				{
					styleStringValue = (string)obj;
				}
				return result;
			}

			private static void GetTextBoxStyleAttribute(Style style, string styleAttributeName, TextBoxInstanceInfo textBoxInstanceInfo, ref bool sharedFormatSettings, out int styleIntValue)
			{
				styleIntValue = 0;
				AttributeInfo attributeInfo = style.StyleAttributes[styleAttributeName];
				if (attributeInfo != null)
				{
					if (attributeInfo.IsExpression)
					{
						sharedFormatSettings = false;
						object styleAttributeValue = textBoxInstanceInfo.GetStyleAttributeValue(attributeInfo.IntValue);
						if (styleAttributeValue != null)
						{
							styleIntValue = (int)styleAttributeValue;
						}
					}
					else
					{
						styleIntValue = attributeInfo.IntValue;
					}
				}
			}

			private static void GetAndValidateCalendar(Style style, TextBox textBox, TextBoxInstanceInfo textBoxInstanceInfo, int languageState, ref bool sharedFormatSettings, CultureInfo formattingCulture, ProcessingContext context, out Calendar formattingCalendar)
			{
				AttributeInfo attributeInfo = style.StyleAttributes["Calendar"];
				string text = null;
				Calendar calendar = null;
				bool flag = false;
				formattingCalendar = null;
				if (attributeInfo != null)
				{
					if (attributeInfo.IsExpression)
					{
						flag = true;
						text = (string)textBoxInstanceInfo.GetStyleAttributeValue(attributeInfo.IntValue);
						sharedFormatSettings = false;
					}
					else
					{
						text = attributeInfo.Value;
						switch (languageState)
						{
						case 1:
							flag = true;
							break;
						default:
							if (textBox.CalendarValidated)
							{
								break;
							}
							textBox.CalendarValidated = true;
							textBox.Calendar = (formattingCalendar = ProcessingValidator.CreateCalendar(text));
							return;
						case 0:
							break;
						}
					}
				}
				if (!flag && textBox.CalendarValidated)
				{
					return;
				}
				if (text != null && ProcessingValidator.ValidateCalendar(formattingCulture, text, textBox.ObjectType, textBox.Name, "Calendar", context.ErrorContext))
				{
					calendar = (formattingCalendar = ProcessingValidator.CreateCalendar(text));
				}
				if (!flag)
				{
					textBox.Calendar = calendar;
					textBox.CalendarValidated = true;
				}
			}

			private static string FormatTextboxValue(TextBoxInstanceInfo textBoxInstanceInfo, object textBoxValue, TextBox textBox, TypeCode typeCode, ProcessingContext processingContext)
			{
				string text = null;
				Style styleClass = textBox.StyleClass;
				CultureInfo cultureInfo = null;
				string text2 = null;
				bool sharedFormatSettings = true;
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				int num = 0;
				bool flag4 = false;
				string text3 = null;
				Calendar calendar = null;
				Calendar calendar2 = null;
				try
				{
					if (styleClass != null)
					{
						RuntimeRICollection.GetTextBoxStyleAttribute(styleClass, "Format", textBoxInstanceInfo, ref sharedFormatSettings, out text);
						num = RuntimeRICollection.GetTextBoxStyleAttribute(styleClass, "Language", textBoxInstanceInfo, ref sharedFormatSettings, out text2);
						if (text2 != null)
						{
							cultureInfo = new CultureInfo(text2, false);
							if (cultureInfo.IsNeutralCulture)
							{
								cultureInfo = CultureInfo.CreateSpecificCulture(text2);
								cultureInfo = new CultureInfo(cultureInfo.Name, false);
							}
						}
						else
						{
							num = 0;
							flag2 = true;
							cultureInfo = Thread.CurrentThread.CurrentCulture;
							if (processingContext.LanguageInstanceId != textBox.LanguageInstanceId)
							{
								textBox.CalendarValidated = false;
								textBox.Calendar = null;
								textBox.LanguageInstanceId = processingContext.LanguageInstanceId;
							}
						}
						if (typeCode == TypeCode.DateTime)
						{
							if (textBox.CalendarValidated)
							{
								if (textBox.Calendar != null)
								{
									calendar2 = textBox.Calendar;
								}
							}
							else
							{
								RuntimeRICollection.GetAndValidateCalendar(styleClass, textBox, textBoxInstanceInfo, num, ref sharedFormatSettings, cultureInfo, processingContext, out calendar2);
							}
						}
					}
					if (cultureInfo != null && calendar2 != null)
					{
						if (flag2)
						{
							if (cultureInfo.DateTimeFormat.IsReadOnly)
							{
								cultureInfo = (CultureInfo)cultureInfo.Clone();
								flag3 = true;
							}
							else
							{
								calendar = cultureInfo.DateTimeFormat.Calendar;
							}
						}
						cultureInfo.DateTimeFormat.Calendar = calendar2;
					}
					bool flag5 = false;
					if (text != null && textBoxValue is IFormattable)
					{
						try
						{
							if (cultureInfo == null)
							{
								cultureInfo = Thread.CurrentThread.CurrentCulture;
								flag2 = true;
							}
							if (ReportProcessing.CompareWithInvariantCulture(text, "x", true) == 0)
							{
								flag4 = true;
							}
							text3 = ((IFormattable)textBoxValue).ToString(text, cultureInfo);
							flag5 = true;
						}
						catch
						{
						}
					}
					CultureInfo cultureInfo2;
					if (!flag5)
					{
						cultureInfo2 = null;
						if (!flag2 && cultureInfo != null)
						{
							goto IL_0181;
						}
						if (flag3)
						{
							goto IL_0181;
						}
						text3 = textBoxValue.ToString();
					}
					goto end_IL_0028;
					IL_0181:
					cultureInfo2 = Thread.CurrentThread.CurrentCulture;
					Thread.CurrentThread.CurrentCulture = cultureInfo;
					try
					{
						text3 = textBoxValue.ToString();
					}
					finally
					{
						if (cultureInfo2 != null)
						{
							Thread.CurrentThread.CurrentCulture = cultureInfo2;
						}
					}
					end_IL_0028:;
				}
				finally
				{
					if (flag2 && calendar != null)
					{
						Global.Tracer.Assert(!Thread.CurrentThread.CurrentCulture.DateTimeFormat.IsReadOnly, "(!System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.IsReadOnly)");
						Thread.CurrentThread.CurrentCulture.DateTimeFormat.Calendar = calendar;
					}
				}
				if (!flag4 && styleClass != null)
				{
					switch (typeCode)
					{
					case TypeCode.SByte:
					case TypeCode.Byte:
					case TypeCode.Int16:
					case TypeCode.UInt16:
					case TypeCode.Int32:
					case TypeCode.UInt32:
					case TypeCode.Int64:
					case TypeCode.UInt64:
					case TypeCode.Single:
					case TypeCode.Double:
					case TypeCode.Decimal:
						flag = true;
						break;
					}
					if (flag)
					{
						int num2 = 1;
						RuntimeRICollection.GetTextBoxStyleAttribute(styleClass, "NumeralVariant", textBoxInstanceInfo, ref sharedFormatSettings, out num2);
						if (num2 > 2)
						{
							CultureInfo cultureInfo3 = cultureInfo;
							if (cultureInfo3 == null)
							{
								cultureInfo3 = Thread.CurrentThread.CurrentCulture;
							}
							string numberDecimalSeparator = cultureInfo3.NumberFormat.NumberDecimalSeparator;
							RuntimeRICollection.GetTextBoxStyleAttribute(styleClass, "NumeralLanguage", textBoxInstanceInfo, ref sharedFormatSettings, out text2);
							if (text2 != null)
							{
								cultureInfo = new CultureInfo(text2, false);
							}
							else if (cultureInfo == null)
							{
								cultureInfo = cultureInfo3;
							}
							bool flag6 = true;
							text3 = FormatDigitReplacement.FormatNumeralVariant(text3, num2, cultureInfo, numberDecimalSeparator, out flag6);
							if (!flag6)
							{
								processingContext.ErrorContext.Register(ProcessingErrorCode.rsInvalidNumeralVariantForLanguage, Severity.Warning, textBox.ObjectType, textBox.Name, "NumeralVariant", num2.ToString(CultureInfo.InvariantCulture), cultureInfo.Name);
							}
						}
					}
				}
				textBox.SharedFormatSettings = sharedFormatSettings;
				return text3;
			}

			internal static void EvalReportItemAttr(ReportItem reportItem, ReportItemInstance riInstance, ReportItemInstanceInfo riInstanceInfo, ProcessingContext processingContext)
			{
				if (processingContext.ShowHideType != 0)
				{
					if (!(reportItem is List))
					{
						((IShowHideReceiver)riInstanceInfo).ProcessReceiver(processingContext, riInstance.UniqueName);
					}
					if (reportItem is TextBox)
					{
						((IShowHideSender)riInstanceInfo).ProcessSender(processingContext, riInstance.UniqueName);
					}
				}
				RuntimeRICollection.EvaluateStyleAttributes(reportItem.ObjectType, reportItem.Name, reportItem.StyleClass, riInstance.UniqueName, riInstanceInfo.StyleAttributeValues, processingContext);
				RuntimeRICollection.ResetSubtotalReferences(processingContext);
				if (reportItem.Label != null)
				{
					string text = processingContext.NavigationInfo.RegisterLabel(processingContext.ReportRuntime.EvaluateReportItemLabelExpression(reportItem));
					if (text != null)
					{
						riInstanceInfo.Label = text;
					}
				}
				if (reportItem.Bookmark != null)
				{
					processingContext.NavigationInfo.ProcessBookmark(processingContext, reportItem, riInstance, riInstanceInfo);
				}
				if (reportItem.ToolTip != null && ExpressionInfo.Types.Constant != reportItem.ToolTip.Type)
				{
					riInstanceInfo.ToolTip = processingContext.ReportRuntime.EvaluateReportItemToolTipExpression(reportItem);
				}
			}

			internal static void ResetSubtotalReferences(ProcessingContext processingContext)
			{
				if (processingContext.HeadingInstance != null)
				{
					MatrixHeading matrixHeadingDef = processingContext.HeadingInstance.MatrixHeadingDef;
					bool flag = Pivot.ProcessingInnerGroupings.Column == ((Matrix)matrixHeadingDef.DataRegionDef).ProcessingInnerGrouping;
					if (flag && matrixHeadingDef.IsColumn)
					{
						processingContext.HeadingInstance = null;
					}
					if (!flag && !matrixHeadingDef.IsColumn)
					{
						processingContext.HeadingInstance = processingContext.HeadingInstanceOld;
						processingContext.HeadingInstanceOld = null;
					}
				}
			}

			internal static void EvaluateStyleAttributes(ObjectType objectType, string objectName, Style style, int itemUniqueName, object[] values, ProcessingContext processingContext)
			{
				if (style != null && style.ExpressionList != null)
				{
					AttributeInfo attributeInfo = style.StyleAttributes["BorderColor"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleBorderColor(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["BorderColorLeft"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleBorderColorLeft(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["BorderColorRight"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleBorderColorRight(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["BorderColorTop"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleBorderColorTop(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["BorderColorBottom"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleBorderColorBottom(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["BorderStyle"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleBorderStyle(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["BorderStyleLeft"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleBorderStyleLeft(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["BorderStyleRight"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleBorderStyleRight(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["BorderStyleTop"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleBorderStyleTop(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["BorderStyleBottom"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleBorderStyleBottom(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["BorderWidth"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleBorderWidth(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["BorderWidthLeft"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleBorderWidthLeft(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["BorderWidthRight"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleBorderWidthRight(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["BorderWidthTop"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleBorderWidthTop(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["BorderWidthBottom"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleBorderWidthBottom(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["BackgroundColor"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleBackgroundColor(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["BackgroundGradientType"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleBackgroundGradientType(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["BackgroundGradientEndColor"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleBackgroundGradientEndColor(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					RuntimeRICollection.EvaluateBackgroundImage(objectType, objectName, itemUniqueName, style, values, processingContext);
					attributeInfo = style.StyleAttributes["BackgroundRepeat"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleBackgroundRepeat(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["FontStyle"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleFontStyle(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["FontFamily"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleFontFamily(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["FontSize"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleFontSize(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["FontWeight"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleFontWeight(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["Format"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleFormat(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["TextDecoration"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleTextDecoration(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["TextAlign"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleTextAlign(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["VerticalAlign"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleVerticalAlign(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["Color"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleColor(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["PaddingLeft"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStylePaddingLeft(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["PaddingRight"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStylePaddingRight(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["PaddingTop"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStylePaddingTop(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["PaddingBottom"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStylePaddingBottom(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["LineHeight"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleLineHeight(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["Direction"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleDirection(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["WritingMode"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleWritingMode(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["UnicodeBiDi"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleUnicodeBiDi(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["Language"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleLanguage(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["Calendar"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleCalendar(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["NumeralLanguage"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleNumeralLanguage(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["NumeralVariant"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleNumeralVariant(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
				}
			}

			private static void EvaluateBackgroundImage(ObjectType objectType, string objectName, int itemUniqueName, Style style, object[] values, ProcessingContext processingContext)
			{
				AttributeInfo attributeInfo = style.StyleAttributes["BackgroundImageSource"];
				if (attributeInfo != null)
				{
					Global.Tracer.Assert(!attributeInfo.IsExpression, "(!sourceAttribute.IsExpression)");
					AspNetCore.ReportingServices.OnDemandReportRendering.Image.SourceType intValue = (AspNetCore.ReportingServices.OnDemandReportRendering.Image.SourceType)attributeInfo.IntValue;
					AttributeInfo attributeInfo2 = style.StyleAttributes["BackgroundImageValue"];
					Global.Tracer.Assert(null != attributeInfo2, "(null != valueAttribute)");
					object obj = null;
					AttributeInfo attributeInfo3 = null;
					string text = null;
					switch (intValue)
					{
					case AspNetCore.ReportingServices.OnDemandReportRendering.Image.SourceType.External:
					{
						string text3 = (!attributeInfo2.IsExpression) ? attributeInfo2.Value : processingContext.ReportRuntime.EvaluateStyleBackgroundUrlImageValue(style, style.ExpressionList[attributeInfo2.IntValue], objectType, objectName);
						obj = text3;
						if (text3 != null && !processingContext.ImageStreamNames.ContainsKey(text3))
						{
							byte[] array2 = null;
							RuntimeRICollection.GetExternalImage(processingContext, text3, objectType, objectName, out array2, out text);
							if (array2 != null)
							{
								if (processingContext.InPageSection && !processingContext.CreatePageSectionImageChunks)
								{
									obj = new ImageData(array2, text);
								}
								else
								{
									string text4 = "BG" + Guid.NewGuid().ToString();
									using (Stream stream2 = processingContext.CreateReportChunkCallback(text4, ReportChunkTypes.Image, text))
									{
										stream2.Write(array2, 0, array2.Length);
									}
									processingContext.ImageStreamNames[text3] = new ImageInfo(text4, text);
								}
								attributeInfo3 = style.StyleAttributes["BackgroundImageMIMEType"];
							}
						}
						break;
					}
					case AspNetCore.ReportingServices.OnDemandReportRendering.Image.SourceType.Embedded:
					{
						string text5 = (!attributeInfo2.IsExpression) ? attributeInfo2.Value : processingContext.ReportRuntime.EvaluateStyleBackgroundEmbeddedImageValue(style, style.ExpressionList[attributeInfo2.IntValue], processingContext.EmbeddedImages, objectType, objectName);
						obj = text5;
						attributeInfo3 = null;
						text = null;
						break;
					}
					case AspNetCore.ReportingServices.OnDemandReportRendering.Image.SourceType.Database:
					{
						byte[] array = (!attributeInfo2.IsExpression) ? null : processingContext.ReportRuntime.EvaluateStyleBackgroundDatabaseImageValue(style, style.ExpressionList[attributeInfo2.IntValue], objectType, objectName);
						attributeInfo3 = style.StyleAttributes["BackgroundImageMIMEType"];
						Global.Tracer.Assert(null != attributeInfo3, "(null != mimeTypeAttribute)");
						text = ((!attributeInfo3.IsExpression) ? attributeInfo3.Value : processingContext.ReportRuntime.EvaluateStyleBackgroundImageMIMEType(style, style.ExpressionList[attributeInfo2.IntValue], objectType, objectName));
						if (array != null)
						{
							if (processingContext.InPageSection && !processingContext.CreatePageSectionImageChunks)
							{
								obj = new ImageData(array, text);
							}
							else if (processingContext.CreateReportChunkCallback != null)
							{
								string text2 = "BG" + Guid.NewGuid().ToString();
								using (Stream stream = processingContext.CreateReportChunkCallback(text2, ReportChunkTypes.Image, text))
								{
									stream.Write(array, 0, array.Length);
								}
								obj = text2;
							}
						}
						break;
					}
					default:
						obj = null;
						attributeInfo3 = null;
						text = null;
						break;
					}
					if (attributeInfo2.IsExpression)
					{
						values[attributeInfo2.IntValue] = obj;
					}
					if (attributeInfo3 != null && attributeInfo3.IsExpression)
					{
						values[attributeInfo3.IntValue] = text;
					}
				}
			}
		}

		internal abstract class RuntimeDataRegionObj : IScope
		{
			protected ProcessingContext m_processingContext;

			protected bool m_processedPreviousAggregates;

			internal ProcessingContext ProcessingContext
			{
				get
				{
					return this.m_processingContext;
				}
			}

			protected abstract IScope OuterScope
			{
				get;
			}

			protected virtual string ScopeName
			{
				get
				{
					return null;
				}
			}

			internal virtual bool TargetForNonDetailSort
			{
				get
				{
					if (this.OuterScope != null)
					{
						return this.OuterScope.TargetForNonDetailSort;
					}
					return false;
				}
			}

			protected virtual int[] SortFilterExpressionScopeInfoIndices
			{
				get
				{
					Global.Tracer.Assert(false);
					return null;
				}
			}

			bool IScope.TargetForNonDetailSort
			{
				get
				{
					return this.TargetForNonDetailSort;
				}
			}

			int[] IScope.SortFilterExpressionScopeInfoIndices
			{
				get
				{
					return this.SortFilterExpressionScopeInfoIndices;
				}
			}

			protected RuntimeDataRegionObj(ProcessingContext processingContext)
			{
				this.m_processingContext = processingContext;
			}

			protected RuntimeDataRegionObj(RuntimeDataRegionObj outerDataRegion)
			{
				this.m_processingContext = outerDataRegion.ProcessingContext;
			}

			internal virtual bool IsTargetForSort(int index, bool detailSort)
			{
				if (this.OuterScope != null)
				{
					return this.OuterScope.IsTargetForSort(index, detailSort);
				}
				return false;
			}

			internal abstract void NextRow();

			internal abstract bool SortAndFilter();

			internal abstract void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup);

			internal abstract void CreateInstances(ReportItemInstance riInstance, IList instanceList, RenderingPagesRangesList pagesList);

			internal abstract void SetupEnvironment();

			bool IScope.IsTargetForSort(int index, bool detailSort)
			{
				return this.IsTargetForSort(index, detailSort);
			}

			void IScope.ReadRow(DataActions dataAction)
			{
				this.ReadRow(dataAction);
			}

			bool IScope.InScope(string scope)
			{
				return this.InScope(scope);
			}

			IScope IScope.GetOuterScope(bool includeSubReportContainingScope)
			{
				return this.OuterScope;
			}

			string IScope.GetScopeName()
			{
				return this.ScopeName;
			}

			int IScope.RecursiveLevel(string scope)
			{
				return this.GetRecursiveLevel(scope);
			}

			bool IScope.TargetScopeMatched(int index, bool detailSort)
			{
				return this.TargetScopeMatched(index, detailSort);
			}

			void IScope.GetScopeValues(IHierarchyObj targetScopeObj, VariantList[] scopeValues, ref int index)
			{
				this.GetScopeValues(targetScopeObj, scopeValues, ref index);
			}

			void IScope.GetGroupNameValuePairs(Dictionary<string, object> pairs)
			{
				this.GetGroupNameValuePairs(pairs);
			}

			internal static void AddAggregate(ref DataAggregateObjList aggregates, DataAggregateObj aggregate)
			{
				if (aggregates == null)
				{
					aggregates = new DataAggregateObjList();
				}
				aggregates.Add(aggregate);
			}

			internal static void CreateAggregates(ProcessingContext processingContext, DataAggregateInfoList aggDefs, ref DataAggregateObjList nonCustomAggregates, ref DataAggregateObjList customAggregates)
			{
				if (aggDefs != null && 0 < aggDefs.Count)
				{
					for (int i = 0; i < aggDefs.Count; i++)
					{
						DataAggregateObj aggregate = new DataAggregateObj(aggDefs[i], processingContext);
						if (DataAggregateInfo.AggregateTypes.Aggregate == aggDefs[i].AggregateType)
						{
							RuntimeDataRegionObj.AddAggregate(ref customAggregates, aggregate);
						}
						else
						{
							RuntimeDataRegionObj.AddAggregate(ref nonCustomAggregates, aggregate);
						}
					}
				}
			}

			internal static void CreateAggregates(ProcessingContext processingContext, DataAggregateInfoList aggDefs, ref DataAggregateObjList aggregates)
			{
				if (aggDefs != null && 0 < aggDefs.Count)
				{
					for (int i = 0; i < aggDefs.Count; i++)
					{
						DataAggregateObj aggregate = new DataAggregateObj(aggDefs[i], processingContext);
						RuntimeDataRegionObj.AddAggregate(ref aggregates, aggregate);
					}
				}
			}

			internal static void CreateAggregates(ProcessingContext processingContext, RunningValueInfoList aggDefs, ref DataAggregateObjList aggregates)
			{
				if (aggDefs != null && 0 < aggDefs.Count)
				{
					for (int i = 0; i < aggDefs.Count; i++)
					{
						DataAggregateObj aggregate = new DataAggregateObj(aggDefs[i], processingContext);
						RuntimeDataRegionObj.AddAggregate(ref aggregates, aggregate);
					}
				}
			}

			internal static void UpdateAggregates(ProcessingContext processingContext, DataAggregateObjList aggregates, bool updateAndSetup)
			{
				if (aggregates != null)
				{
					for (int i = 0; i < aggregates.Count; i++)
					{
						DataAggregateObj dataAggregateObj = aggregates[i];
						dataAggregateObj.Update();
						if (updateAndSetup)
						{
							processingContext.ReportObjectModel.AggregatesImpl.Set(dataAggregateObj.Name, dataAggregateObj.AggregateDef, dataAggregateObj.DuplicateNames, dataAggregateObj.AggregateResult());
						}
					}
				}
			}

			protected void SetupAggregates(DataAggregateObjList aggregates)
			{
				if (aggregates != null)
				{
					for (int i = 0; i < aggregates.Count; i++)
					{
						DataAggregateObj dataAggregateObj = aggregates[i];
						this.m_processingContext.ReportObjectModel.AggregatesImpl.Set(dataAggregateObj.Name, dataAggregateObj.AggregateDef, dataAggregateObj.DuplicateNames, dataAggregateObj.AggregateResult());
					}
				}
			}

			protected void SetupEnvironment(DataAggregateObjList nonCustomAggregates, DataAggregateObjList customAggregates, FieldImpl[] dataRow)
			{
				this.SetupAggregates(nonCustomAggregates);
				this.SetupAggregates(customAggregates);
				this.SetupFields(dataRow);
				this.m_processingContext.ReportRuntime.CurrentScope = this;
			}

			protected void SetupFields(FieldImpl[] dataRow)
			{
				this.m_processingContext.ReportObjectModel.FieldsImpl.SetFields(dataRow);
			}

			internal static void SetupRunningValues(ProcessingContext processingContext, RunningValueInfoList rvDefs, DataAggregateObjResult[] rvValues)
			{
				int num = 0;
				RuntimeDataRegionObj.SetupRunningValues(processingContext, ref num, rvDefs, rvValues);
			}

			protected void SetupRunningValues(RunningValueInfoList rvDefs, DataAggregateObjResult[] rvValues)
			{
				int num = 0;
				RuntimeDataRegionObj.SetupRunningValues(this.m_processingContext, ref num, rvDefs, rvValues);
			}

			protected void SetupRunningValues(ref int startIndex, RunningValueInfoList rvDefs, DataAggregateObjResult[] rvValues)
			{
				RuntimeDataRegionObj.SetupRunningValues(this.m_processingContext, ref startIndex, rvDefs, rvValues);
			}

			private static void SetupRunningValues(ProcessingContext processingContext, ref int startIndex, RunningValueInfoList rvDefs, DataAggregateObjResult[] rvValues)
			{
				if (rvDefs != null && rvValues != null)
				{
					for (int i = 0; i < rvDefs.Count; i++)
					{
						processingContext.ReportObjectModel.AggregatesImpl.Set(rvDefs[i].Name, rvDefs[i], rvDefs[i].DuplicateNames, rvValues[startIndex + i]);
					}
					startIndex += rvDefs.Count;
				}
			}

			internal abstract void ReadRow(DataActions dataAction);

			internal abstract bool InScope(string scope);

			protected Hashtable GetScopeNames(RuntimeDataRegionObj currentScope, string targetScope, ref bool inPivotCell, out bool inScope)
			{
				inScope = false;
				Hashtable hashtable = null;
				if (!inPivotCell)
				{
					hashtable = new Hashtable();
				}
				for (IScope scope = currentScope; scope != null; scope = scope.GetOuterScope(false))
				{
					string scopeName = scope.GetScopeName();
					if (scopeName != null)
					{
						if (!inScope && scopeName.Equals(targetScope))
						{
							inScope = true;
							if (hashtable == null)
							{
								return null;
							}
						}
						if (hashtable != null)
						{
							Grouping value = null;
							if (scope is RuntimeGroupLeafObj)
							{
								value = ((RuntimeGroupLeafObj)scope).GroupingDef;
							}
							hashtable.Add(scopeName, value);
						}
						continue;
					}
					if (!(scope is RuntimePivotCell) && !(scope is RuntimeTablixCell))
					{
						continue;
					}
					inPivotCell = true;
					if (inScope)
					{
						return null;
					}
					inScope = scope.InScope(targetScope);
					break;
				}
				return hashtable;
			}

			protected Hashtable GetScopeNames(RuntimeDataRegionObj currentScope, string targetScope, ref bool inPivotCell, out int level)
			{
				level = -1;
				Hashtable hashtable = null;
				if (!inPivotCell)
				{
					hashtable = new Hashtable();
				}
				for (IScope scope = currentScope; scope != null; scope = scope.GetOuterScope(false))
				{
					string scopeName = scope.GetScopeName();
					if (scopeName != null)
					{
						Grouping grouping = null;
						if (scope is RuntimeGroupLeafObj)
						{
							grouping = ((RuntimeGroupLeafObj)scope).GroupingDef;
							if (-1 == level && scopeName.Equals(targetScope))
							{
								level = grouping.RecursiveLevel;
								if (hashtable == null)
								{
									return null;
								}
							}
						}
						if (hashtable != null)
						{
							hashtable.Add(scopeName, grouping);
						}
						continue;
					}
					if (!(scope is RuntimePivotCell) && !(scope is RuntimeTablixCell))
					{
						continue;
					}
					inPivotCell = true;
					if (-1 != level)
					{
						return null;
					}
					level = scope.RecursiveLevel(targetScope);
					break;
				}
				return hashtable;
			}

			protected Hashtable GetScopeNames(RuntimeDataRegionObj currentScope, ref bool inPivotCell, Dictionary<string, object> nameValuePairs)
			{
				Hashtable hashtable = null;
				if (!inPivotCell)
				{
					hashtable = new Hashtable();
				}
				for (IScope scope = currentScope; scope != null; scope = scope.GetOuterScope(false))
				{
					string scopeName = scope.GetScopeName();
					if (scopeName != null)
					{
						Grouping grouping = null;
						if (scope is RuntimeGroupLeafObj)
						{
							grouping = ((RuntimeGroupLeafObj)scope).GroupingDef;
							RuntimeDataRegionObj.AddGroupNameValuePair(this.m_processingContext, grouping, nameValuePairs);
						}
						if (hashtable != null)
						{
							hashtable.Add(scopeName, grouping);
						}
					}
					else if (scope is RuntimePivotCell || scope is RuntimeTablixCell)
					{
						inPivotCell = true;
						scope.GetGroupNameValuePairs(nameValuePairs);
						hashtable = null;
					}
				}
				return hashtable;
			}

			internal static void AddGroupNameValuePair(ProcessingContext processingContext, Grouping grouping, Dictionary<string, object> nameValuePairs)
			{
				if (grouping != null)
				{
					Global.Tracer.Assert(grouping.GroupExpressions != null && 0 < grouping.GroupExpressions.Count);
					ExpressionInfo expressionInfo = grouping.GroupExpressions[0];
					if (expressionInfo.Type == ExpressionInfo.Types.Field)
					{
						try
						{
							FieldImpl fieldImpl = processingContext.ReportObjectModel.FieldsImpl[expressionInfo.IntValue];
							if (fieldImpl.FieldDef != null)
							{
								object value = fieldImpl.Value;
								if (!nameValuePairs.ContainsKey(fieldImpl.FieldDef.DataField))
								{
									nameValuePairs.Add(fieldImpl.FieldDef.DataField, (value is DBNull) ? null : value);
								}
							}
						}
						catch (Exception ex)
						{
							if (AsynchronousExceptionDetection.IsStoppingException(ex))
							{
								throw;
							}
							Global.Tracer.Trace(TraceLevel.Verbose, ex.Message);
						}
					}
				}
			}

			protected bool DataRegionInScope(DataRegion dataRegionDef, string scope)
			{
				if (dataRegionDef.ScopeNames == null)
				{
					bool inPivotCell = dataRegionDef.InPivotCell;
					bool result = default(bool);
					dataRegionDef.ScopeNames = this.GetScopeNames(this, scope, ref inPivotCell, out result);
					dataRegionDef.InPivotCell = inPivotCell;
					return result;
				}
				return dataRegionDef.ScopeNames.Contains(scope);
			}

			protected virtual int GetRecursiveLevel(string scope)
			{
				return -1;
			}

			protected int DataRegionRecursiveLevel(DataRegion dataRegionDef, string scope)
			{
				if (scope == null)
				{
					return -1;
				}
				if (dataRegionDef.ScopeNames == null)
				{
					bool inPivotCell = dataRegionDef.InPivotCell;
					int result = default(int);
					dataRegionDef.ScopeNames = this.GetScopeNames(this, scope, ref inPivotCell, out result);
					dataRegionDef.InPivotCell = inPivotCell;
					return result;
				}
				Grouping grouping = dataRegionDef.ScopeNames[scope] as Grouping;
				if (grouping != null)
				{
					return grouping.RecursiveLevel;
				}
				return -1;
			}

			protected void DataRegionGetGroupNameValuePairs(DataRegion dataRegionDef, Dictionary<string, object> nameValuePairs)
			{
				if (dataRegionDef.ScopeNames == null)
				{
					bool inPivotCell = dataRegionDef.InPivotCell;
					dataRegionDef.ScopeNames = this.GetScopeNames(this, ref inPivotCell, nameValuePairs);
					dataRegionDef.InPivotCell = inPivotCell;
				}
				else
				{
					IEnumerator enumerator = dataRegionDef.ScopeNames.Values.GetEnumerator();
					while (enumerator.MoveNext())
					{
						RuntimeDataRegionObj.AddGroupNameValuePair(this.m_processingContext, enumerator.Current as Grouping, nameValuePairs);
					}
				}
			}

			protected void ScopeNextNonAggregateRow(DataAggregateObjList aggregates, DataRowList dataRows)
			{
				RuntimeDataRegionObj.UpdateAggregates(this.m_processingContext, aggregates, true);
				this.CommonNextRow(dataRows);
			}

			internal static void CommonFirstRow(FieldsImpl fields, ref bool firstRowIsAggregate, ref FieldImpl[] firstRow)
			{
				if (!firstRowIsAggregate && firstRow != null)
				{
					return;
				}
				firstRow = fields.GetAndSaveFields();
				firstRowIsAggregate = fields.IsAggregateRow;
			}

			protected void CommonNextRow(DataRowList dataRows)
			{
				if (dataRows != null)
				{
					RuntimeDetailObj.SaveData(dataRows, this.m_processingContext);
				}
				this.SendToInner();
			}

			protected virtual void SendToInner()
			{
				Global.Tracer.Assert(false);
			}

			protected void ScopeNextAggregateRow(RuntimeUserSortTargetInfo sortTargetInfo)
			{
				if (sortTargetInfo != null)
				{
					if (sortTargetInfo.AggregateRows == null)
					{
						sortTargetInfo.AggregateRows = new AggregateRowList();
					}
					AggregateRow value = new AggregateRow(this.m_processingContext);
					sortTargetInfo.AggregateRows.Add(value);
					if (!sortTargetInfo.TargetForNonDetailSort)
					{
						return;
					}
				}
				this.SendToInner();
			}

			protected void ScopeFinishSorting(ref FieldImpl[] firstRow, RuntimeUserSortTargetInfo sortTargetInfo)
			{
				Global.Tracer.Assert(null != sortTargetInfo, "(null != sortTargetInfo)");
				firstRow = null;
				sortTargetInfo.SortTree.Traverse(ProcessingStages.UserSortFilter, true);
				sortTargetInfo.SortTree = null;
				if (sortTargetInfo.AggregateRows != null)
				{
					for (int i = 0; i < sortTargetInfo.AggregateRows.Count; i++)
					{
						sortTargetInfo.AggregateRows[i].SetFields(this.m_processingContext);
						this.SendToInner();
					}
					sortTargetInfo.AggregateRows = null;
				}
			}

			internal virtual bool TargetScopeMatched(int index, bool detailSort)
			{
				Global.Tracer.Assert(false);
				return false;
			}

			internal virtual void GetScopeValues(IHierarchyObj targetScopeObj, VariantList[] scopeValues, ref int index)
			{
				Global.Tracer.Assert(false);
			}

			protected void ReleaseDataRows(DataActions finishedDataAction, ref DataActions dataAction, ref DataRowList dataRows)
			{
				dataAction &= ~finishedDataAction;
				if (dataAction == DataActions.None)
				{
					dataRows = null;
				}
			}

			protected void DetailHandleSortFilterEvent(DataRegion dataRegionDef, IScope outerScope, int rowIndex)
			{
				RuntimeSortFilterEventInfoList runtimeSortFilterInfo = this.m_processingContext.RuntimeSortFilterInfo;
				if (runtimeSortFilterInfo != null && dataRegionDef.SortFilterSourceDetailScopeInfo != null && !outerScope.TargetForNonDetailSort)
				{
					for (int i = 0; i < runtimeSortFilterInfo.Count; i++)
					{
						RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = runtimeSortFilterInfo[i];
						if (runtimeSortFilterEventInfo.EventSource.ContainingScopes != null && 0 < runtimeSortFilterEventInfo.EventSource.ContainingScopes.Count && -1 != dataRegionDef.SortFilterSourceDetailScopeInfo[i] && outerScope.TargetScopeMatched(i, false) && this.m_processingContext.ReportObjectModel.FieldsImpl.GetRowIndex() == dataRegionDef.SortFilterSourceDetailScopeInfo[i])
						{
							if (runtimeSortFilterEventInfo.EventSource.ContainingScopes.LastEntry == null)
							{
								ReportItem parent = runtimeSortFilterEventInfo.EventSource.Parent;
								if (runtimeSortFilterEventInfo.EventSource.IsSubReportTopLevelScope)
								{
									while (parent != null && !(parent is SubReport))
									{
										parent = parent.Parent;
									}
									Global.Tracer.Assert(parent is SubReport, "(parent is SubReport)");
									parent = parent.Parent;
								}
								if (parent == dataRegionDef)
								{
									Global.Tracer.Assert(null == runtimeSortFilterEventInfo.EventSourceScope, "(null == sortFilterInfo.EventSourceScope)");
									runtimeSortFilterEventInfo.EventSourceScope = this;
									runtimeSortFilterEventInfo.EventSourceDetailIndex = rowIndex;
								}
							}
							if (runtimeSortFilterEventInfo.DetailScopes == null)
							{
								runtimeSortFilterEventInfo.DetailScopes = new RuntimeDataRegionObjList();
								runtimeSortFilterEventInfo.DetailScopeIndices = new IntList();
							}
							runtimeSortFilterEventInfo.DetailScopes.Add(this);
							runtimeSortFilterEventInfo.DetailScopeIndices.Add(rowIndex);
						}
					}
				}
			}

			protected void DetailGetScopeValues(IScope outerScope, IHierarchyObj targetScopeObj, VariantList[] scopeValues, ref int index)
			{
				Global.Tracer.Assert(null == targetScopeObj, "(null == targetScopeObj)");
				outerScope.GetScopeValues(targetScopeObj, scopeValues, ref index);
				Global.Tracer.Assert(index < scopeValues.Length, "(index < scopeValues.Length)");
				VariantList variantList = new VariantList(1);
				variantList.Add(this.m_processingContext.ReportObjectModel.FieldsImpl.GetRowIndex());
				scopeValues[index++] = variantList;
			}

			protected bool DetailTargetScopeMatched(DataRegion dataRegionDef, IScope outerScope, int index)
			{
				if (this.m_processingContext.RuntimeSortFilterInfo != null)
				{
					RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = this.m_processingContext.RuntimeSortFilterInfo[index];
					if (runtimeSortFilterEventInfo != null && runtimeSortFilterEventInfo.DetailScopes != null)
					{
						for (int i = 0; i < runtimeSortFilterEventInfo.DetailScopes.Count; i++)
						{
							if (this == runtimeSortFilterEventInfo.DetailScopes[i] && dataRegionDef.CurrentDetailRowIndex == runtimeSortFilterEventInfo.DetailScopeIndices[i])
							{
								return true;
							}
						}
					}
				}
				return false;
			}

			protected virtual void GetGroupNameValuePairs(Dictionary<string, object> pairs)
			{
			}
		}

		internal class RuntimeSortHierarchyObj : IHierarchyObj
		{
			private class SortHierarchyStructure
			{
				internal RuntimeSortFilterEventInfo SortInfo;

				internal int SortIndex;

				internal BTreeNode SortTree;

				internal SortHierarchyStructure(IHierarchyObj owner, int sortIndex, RuntimeSortFilterEventInfoList sortInfoList, IntList sortInfoIndices)
				{
					this.SortIndex = sortIndex;
					this.SortInfo = sortInfoList[sortInfoIndices[sortIndex]];
					this.SortTree = new BTreeNode(owner);
				}
			}

			private IHierarchyObj m_hierarchyRoot;

			private SortHierarchyStructure m_sortHierarchyStruct;

			private ISortDataHolder m_dataHolder;

			IHierarchyObj IHierarchyObj.HierarchyRoot
			{
				get
				{
					return this.m_hierarchyRoot;
				}
			}

			ProcessingContext IHierarchyObj.ProcessingContext
			{
				get
				{
					return this.m_hierarchyRoot.ProcessingContext;
				}
			}

			BTreeNode IHierarchyObj.SortTree
			{
				get
				{
					if (this.m_sortHierarchyStruct != null)
					{
						return this.m_sortHierarchyStruct.SortTree;
					}
					return null;
				}
				set
				{
					if (this.m_sortHierarchyStruct != null)
					{
						this.m_sortHierarchyStruct.SortTree = value;
					}
					else
					{
						Global.Tracer.Assert(false);
					}
				}
			}

			int IHierarchyObj.ExpressionIndex
			{
				get
				{
					if (this.m_sortHierarchyStruct != null)
					{
						return this.m_sortHierarchyStruct.SortIndex;
					}
					return -1;
				}
			}

			IntList IHierarchyObj.SortFilterInfoIndices
			{
				get
				{
					return this.m_hierarchyRoot.SortFilterInfoIndices;
				}
			}

			bool IHierarchyObj.IsDetail
			{
				get
				{
					return false;
				}
			}

			internal RuntimeSortHierarchyObj(IHierarchyObj outerHierarchy)
			{
				this.m_hierarchyRoot = outerHierarchy.HierarchyRoot;
				ProcessingContext processingContext = this.m_hierarchyRoot.ProcessingContext;
				IntList sortFilterInfoIndices = this.m_hierarchyRoot.SortFilterInfoIndices;
				int num = outerHierarchy.ExpressionIndex + 1;
				if (sortFilterInfoIndices == null || num >= sortFilterInfoIndices.Count)
				{
					if (this.m_hierarchyRoot is RuntimeListDetailObj)
					{
						this.m_dataHolder = new RuntimeListDetailObj((RuntimeListDetailObj)this.m_hierarchyRoot);
					}
					else if (this.m_hierarchyRoot is RuntimeTableDetailObj)
					{
						this.m_dataHolder = new RuntimeTableDetailObj((RuntimeTableDetailObj)this.m_hierarchyRoot);
					}
					else
					{
						this.m_dataHolder = new RuntimeSortDataHolder(this.m_hierarchyRoot);
					}
				}
				else
				{
					this.m_sortHierarchyStruct = new SortHierarchyStructure(this, num, processingContext.RuntimeSortFilterInfo, sortFilterInfoIndices);
				}
			}

			IHierarchyObj IHierarchyObj.CreateHierarchyObj()
			{
				return new RuntimeSortHierarchyObj(this);
			}

			ProcessingMessageList IHierarchyObj.RegisterComparisonError(string propertyName)
			{
				return this.m_hierarchyRoot.ProcessingContext.RegisterComparisonErrorForSortFilterEvent(propertyName);
			}

			void IHierarchyObj.NextRow()
			{
				if (this.m_dataHolder != null)
				{
					this.m_dataHolder.NextRow();
				}
				else if (this.m_sortHierarchyStruct != null)
				{
					object sortOrder = this.m_sortHierarchyStruct.SortInfo.GetSortOrder(this.m_hierarchyRoot.ProcessingContext.ReportRuntime);
					this.m_sortHierarchyStruct.SortTree.NextRow(sortOrder);
				}
			}

			void IHierarchyObj.Traverse(ProcessingStages operation)
			{
				if (this.m_sortHierarchyStruct != null)
				{
					bool ascending = true;
					if (this.m_sortHierarchyStruct.SortInfo.EventSource.UserSort.SortExpressionScope == null)
					{
						ascending = this.m_sortHierarchyStruct.SortInfo.SortDirection;
					}
					this.m_sortHierarchyStruct.SortTree.Traverse(operation, ascending);
				}
				if (this.m_dataHolder != null)
				{
					this.m_dataHolder.Traverse(operation);
				}
			}

			void IHierarchyObj.ReadRow()
			{
				Global.Tracer.Assert(false);
			}

			void IHierarchyObj.ProcessUserSort()
			{
				Global.Tracer.Assert(false);
			}

			void IHierarchyObj.MarkSortInfoProcessed(RuntimeSortFilterEventInfoList runtimeSortFilterInfo)
			{
				Global.Tracer.Assert(false);
			}

			void IHierarchyObj.AddSortInfoIndex(int sortInfoIndex, RuntimeSortFilterEventInfo sortInfo)
			{
				Global.Tracer.Assert(false);
			}
		}

		internal class RuntimeHierarchyObj : RuntimeDataRegionObj, IHierarchyObj
		{
			protected RuntimeGroupingObj m_grouping;

			protected RuntimeExpressionInfo m_expression;

			protected RuntimeHierarchyObj m_hierarchyRoot;

			protected RuntimeHierarchyObjList m_hierarchyObjs;

			internal RuntimeHierarchyObjList HierarchyObjs
			{
				get
				{
					return this.m_hierarchyObjs;
				}
			}

			protected override IScope OuterScope
			{
				get
				{
					Global.Tracer.Assert(false);
					return null;
				}
			}

			protected virtual IHierarchyObj HierarchyRoot
			{
				get
				{
					return this.m_hierarchyRoot;
				}
			}

			protected virtual BTreeNode SortTree
			{
				get
				{
					return this.m_grouping.Tree;
				}
				set
				{
					this.m_grouping.Tree = value;
				}
			}

			protected virtual int ExpressionIndex
			{
				get
				{
					if (this.m_expression != null)
					{
						return this.m_expression.ExpressionIndex;
					}
					Global.Tracer.Assert(false);
					return -1;
				}
			}

			protected virtual DataRowList SortDataRows
			{
				get
				{
					Global.Tracer.Assert(false);
					return null;
				}
			}

			protected virtual IntList SortFilterInfoIndices
			{
				get
				{
					Global.Tracer.Assert(false);
					return null;
				}
			}

			protected virtual bool IsDetail
			{
				get
				{
					return false;
				}
			}

			IHierarchyObj IHierarchyObj.HierarchyRoot
			{
				get
				{
					return this.HierarchyRoot;
				}
			}

			ProcessingContext IHierarchyObj.ProcessingContext
			{
				get
				{
					return base.m_processingContext;
				}
			}

			BTreeNode IHierarchyObj.SortTree
			{
				get
				{
					return this.SortTree;
				}
				set
				{
					this.SortTree = value;
				}
			}

			int IHierarchyObj.ExpressionIndex
			{
				get
				{
					return this.ExpressionIndex;
				}
			}

			IntList IHierarchyObj.SortFilterInfoIndices
			{
				get
				{
					return this.SortFilterInfoIndices;
				}
			}

			bool IHierarchyObj.IsDetail
			{
				get
				{
					return this.IsDetail;
				}
			}

			protected RuntimeHierarchyObj(ProcessingContext processingContext)
				: base(processingContext)
			{
			}

			internal RuntimeHierarchyObj(RuntimeHierarchyObj outerHierarchy)
				: base(outerHierarchy)
			{
				this.ConstructorHelper(outerHierarchy.m_expression.ExpressionIndex + 1, outerHierarchy.m_hierarchyRoot);
			}

			IHierarchyObj IHierarchyObj.CreateHierarchyObj()
			{
				return this.CreateHierarchyObj();
			}

			ProcessingMessageList IHierarchyObj.RegisterComparisonError(string propertyName)
			{
				return this.RegisterComparisonError(propertyName);
			}

			void IHierarchyObj.NextRow()
			{
				this.NextRow();
			}

			void IHierarchyObj.Traverse(ProcessingStages operation)
			{
				switch (operation)
				{
				case ProcessingStages.SortAndFilter:
					this.SortAndFilter();
					break;
				case ProcessingStages.RunningValues:
					this.CalculateRunningValues();
					break;
				case ProcessingStages.CreatingInstances:
					this.CreateInstances();
					break;
				default:
					Global.Tracer.Assert(false, "Invalid processing stage for RuntimeHierarchyObj");
					break;
				}
			}

			void IHierarchyObj.ReadRow()
			{
				this.ReadRow(DataActions.UserSort);
			}

			void IHierarchyObj.ProcessUserSort()
			{
				this.ProcessUserSort();
			}

			void IHierarchyObj.MarkSortInfoProcessed(RuntimeSortFilterEventInfoList runtimeSortFilterInfo)
			{
				this.MarkSortInfoProcessed(runtimeSortFilterInfo);
			}

			void IHierarchyObj.AddSortInfoIndex(int sortInfoIndex, RuntimeSortFilterEventInfo sortInfo)
			{
				this.AddSortInfoIndex(sortInfoIndex, sortInfo);
			}

			private void ConstructorHelper(int exprIndex, RuntimeHierarchyObj hierarchyRoot)
			{
				this.m_hierarchyRoot = hierarchyRoot;
				RuntimeGroupRootObj runtimeGroupRootObj = null;
				RuntimeDetailObj runtimeDetailObj = null;
				ExpressionInfoList expressionInfoList;
				IndexedExprHost expressionsHost;
				BoolList directions;
				if (this.m_hierarchyRoot is RuntimeGroupRootObj)
				{
					runtimeGroupRootObj = (RuntimeGroupRootObj)this.m_hierarchyRoot;
					if (ProcessingStages.Grouping == runtimeGroupRootObj.ProcessingStage)
					{
						expressionInfoList = runtimeGroupRootObj.GroupExpressions;
						expressionsHost = runtimeGroupRootObj.GroupExpressionHost;
						directions = runtimeGroupRootObj.GroupDirections;
					}
					else
					{
						Global.Tracer.Assert(ProcessingStages.SortAndFilter == runtimeGroupRootObj.ProcessingStage, "(ProcessingStages.SortAndFilter == groupRoot.ProcessingStage)");
						expressionInfoList = runtimeGroupRootObj.SortExpressions;
						expressionsHost = runtimeGroupRootObj.SortExpressionHost;
						directions = runtimeGroupRootObj.SortDirections;
					}
				}
				else
				{
					Global.Tracer.Assert(this.m_hierarchyRoot is RuntimeDetailObj, "(m_hierarchyRoot is RuntimeDetailObj)");
					runtimeDetailObj = (RuntimeDetailObj)this.m_hierarchyRoot;
					expressionInfoList = runtimeDetailObj.SortExpressions;
					expressionsHost = runtimeDetailObj.SortExpressionHost;
					directions = runtimeDetailObj.SortDirections;
				}
				if (exprIndex >= expressionInfoList.Count)
				{
					this.m_hierarchyObjs = new RuntimeHierarchyObjList();
					RuntimeHierarchyObj runtimeHierarchyObj = null;
					if (runtimeGroupRootObj != null)
					{
						if (ProcessingStages.Grouping == runtimeGroupRootObj.ProcessingStage)
						{
							if (this.m_hierarchyRoot is RuntimeListGroupRootObj)
							{
								runtimeHierarchyObj = new RuntimeListGroupLeafObj((RuntimeListGroupRootObj)this.m_hierarchyRoot);
							}
							else if (this.m_hierarchyRoot is RuntimeTableGroupRootObj)
							{
								runtimeHierarchyObj = new RuntimeTableGroupLeafObj((RuntimeTableGroupRootObj)this.m_hierarchyRoot);
							}
							else if (this.m_hierarchyRoot is RuntimeMatrixGroupRootObj)
							{
								runtimeHierarchyObj = new RuntimeMatrixGroupLeafObj((RuntimeMatrixGroupRootObj)this.m_hierarchyRoot);
							}
							else if (this.m_hierarchyRoot is RuntimeChartGroupRootObj)
							{
								runtimeHierarchyObj = new RuntimeChartGroupLeafObj((RuntimeChartGroupRootObj)this.m_hierarchyRoot);
							}
							else if (this.m_hierarchyRoot is RuntimeCustomReportItemGroupRootObj)
							{
								runtimeHierarchyObj = new RuntimeCustomReportItemGroupLeafObj((RuntimeCustomReportItemGroupRootObj)this.m_hierarchyRoot);
							}
							if (!runtimeGroupRootObj.HasParent)
							{
								runtimeGroupRootObj.AddChildWithNoParent((RuntimeGroupLeafObj)runtimeHierarchyObj);
							}
						}
					}
					else if (runtimeDetailObj is RuntimeListDetailObj)
					{
						runtimeHierarchyObj = new RuntimeListDetailObj((RuntimeListDetailObj)runtimeDetailObj);
					}
					else if (runtimeDetailObj is RuntimeTableDetailObj)
					{
						runtimeHierarchyObj = new RuntimeTableDetailObj((RuntimeTableDetailObj)runtimeDetailObj);
					}
					else if (runtimeDetailObj is RuntimeOWCChartDetailObj)
					{
						runtimeHierarchyObj = new RuntimeOWCChartDetailObj((RuntimeOWCChartDetailObj)runtimeDetailObj);
					}
					if (runtimeHierarchyObj != null)
					{
						this.m_hierarchyObjs.Add(runtimeHierarchyObj);
					}
				}
				else
				{
					this.m_expression = new RuntimeExpressionInfo(expressionInfoList, expressionsHost, directions, exprIndex);
					if (runtimeGroupRootObj != null)
					{
						this.m_grouping = new RuntimeGroupingObj(this, runtimeGroupRootObj.GroupingType);
					}
					else
					{
						Global.Tracer.Assert(null != runtimeDetailObj, "(null != detailRoot)");
						this.m_grouping = new RuntimeGroupingObj(this, RuntimeGroupingObj.GroupingTypes.Sort);
					}
				}
			}

			internal ProcessingMessageList RegisterComparisonError(string propertyName)
			{
				return this.RegisterComparisonError(propertyName, null);
			}

			internal ProcessingMessageList RegisterComparisonError(string propertyName, ReportProcessingException_ComparisonError e)
			{
				ObjectType objectType;
				string name;
				if (this.m_hierarchyRoot is RuntimeGroupRootObj)
				{
					RuntimeGroupRootObj runtimeGroupRootObj = (RuntimeGroupRootObj)this.m_hierarchyRoot;
					objectType = runtimeGroupRootObj.HierarchyDef.DataRegionDef.ObjectType;
					name = runtimeGroupRootObj.HierarchyDef.DataRegionDef.Name;
				}
				else
				{
					Global.Tracer.Assert(this.m_hierarchyRoot is RuntimeDetailObj, "(m_hierarchyRoot is RuntimeDetailObj)");
					RuntimeDetailObj runtimeDetailObj = (RuntimeDetailObj)this.m_hierarchyRoot;
					objectType = runtimeDetailObj.DataRegionDef.ObjectType;
					name = runtimeDetailObj.DataRegionDef.Name;
				}
				if (e == null)
				{
					base.m_processingContext.ErrorContext.Register(ProcessingErrorCode.rsComparisonError, Severity.Error, objectType, name, propertyName);
				}
				else
				{
					base.m_processingContext.ErrorContext.Register(ProcessingErrorCode.rsComparisonTypeError, Severity.Error, objectType, name, propertyName, e.TypeX, e.TypeY);
				}
				return base.m_processingContext.ErrorContext.Messages;
			}

			internal ProcessingMessageList RegisterSpatialTypeComparisonError(string type)
			{
				ObjectType objectType;
				string name;
				if (this.m_hierarchyRoot is RuntimeGroupRootObj)
				{
					RuntimeGroupRootObj runtimeGroupRootObj = (RuntimeGroupRootObj)this.m_hierarchyRoot;
					objectType = runtimeGroupRootObj.HierarchyDef.DataRegionDef.ObjectType;
					name = runtimeGroupRootObj.HierarchyDef.DataRegionDef.Name;
				}
				else
				{
					Global.Tracer.Assert(this.m_hierarchyRoot is RuntimeDetailObj, "(m_hierarchyRoot is RuntimeDetailObj)");
					RuntimeDetailObj runtimeDetailObj = (RuntimeDetailObj)this.m_hierarchyRoot;
					objectType = runtimeDetailObj.DataRegionDef.ObjectType;
					name = runtimeDetailObj.DataRegionDef.Name;
				}
				base.m_processingContext.ErrorContext.Register(ProcessingErrorCode.rsCannotCompareSpatialType, Severity.Error, objectType, name, type);
				return base.m_processingContext.ErrorContext.Messages;
			}

			internal override void NextRow()
			{
				bool flag = true;
				RuntimeGroupRootObj runtimeGroupRootObj = null;
				if (this.m_hierarchyRoot is RuntimeGroupRootObj)
				{
					runtimeGroupRootObj = (RuntimeGroupRootObj)this.m_hierarchyRoot;
					if (ProcessingStages.SortAndFilter == runtimeGroupRootObj.ProcessingStage)
					{
						flag = false;
					}
				}
				if (this.m_hierarchyObjs != null)
				{
					if (flag)
					{
						Global.Tracer.Assert(null != this.m_hierarchyObjs[0], "(null != m_hierarchyObjs[0])");
						this.m_hierarchyObjs[0].NextRow();
					}
					else if (runtimeGroupRootObj != null)
					{
						Global.Tracer.Assert(null != runtimeGroupRootObj.LastChild, "(null != groupRoot.LastChild)");
						this.m_hierarchyObjs.Add(runtimeGroupRootObj.LastChild);
					}
				}
				else if (this.m_grouping != null)
				{
					ObjectType objectType;
					string name;
					string propertyName;
					if (runtimeGroupRootObj != null)
					{
						objectType = runtimeGroupRootObj.HierarchyDef.DataRegionDef.ObjectType;
						name = runtimeGroupRootObj.HierarchyDef.DataRegionDef.Name;
						propertyName = "GroupExpression";
					}
					else
					{
						Global.Tracer.Assert(this.m_hierarchyRoot is RuntimeDetailObj, "(m_hierarchyRoot is RuntimeDetailObj)");
						RuntimeDetailObj runtimeDetailObj = (RuntimeDetailObj)this.m_hierarchyRoot;
						objectType = runtimeDetailObj.DataRegionDef.ObjectType;
						name = runtimeDetailObj.DataRegionDef.Name;
						propertyName = "SortExpression";
					}
					object obj = base.m_processingContext.ReportRuntime.EvaluateRuntimeExpression(this.m_expression, objectType, name, propertyName);
					if (runtimeGroupRootObj != null && flag)
					{
						Grouping grouping = runtimeGroupRootObj.HierarchyDef.Grouping;
						if (runtimeGroupRootObj.SaveGroupExprValues)
						{
							grouping.CurrentGroupExpressionValues.Add(obj);
						}
						this.MatchSortFilterScope(runtimeGroupRootObj, grouping, obj, this.m_expression.ExpressionIndex);
					}
					this.m_grouping.NextRow(obj);
				}
			}

			internal override bool SortAndFilter()
			{
				if (this.m_grouping != null)
				{
					this.m_grouping.Traverse(ProcessingStages.SortAndFilter, true);
				}
				if (this.m_hierarchyObjs != null)
				{
					for (int i = 0; i < this.m_hierarchyObjs.Count; i++)
					{
						this.m_hierarchyObjs[i].SortAndFilter();
					}
				}
				return true;
			}

			internal virtual void CalculateRunningValues()
			{
				if (this.m_grouping != null)
				{
					this.m_grouping.Traverse(ProcessingStages.RunningValues, this.m_expression.Direction);
				}
				if (this.m_hierarchyObjs != null)
				{
					bool flag = true;
					for (int i = 0; i < this.m_hierarchyObjs.Count; i++)
					{
						RuntimeHierarchyObj runtimeHierarchyObj = this.m_hierarchyObjs[i];
						if (!flag || runtimeHierarchyObj is RuntimeGroupLeafObj)
						{
							((RuntimeGroupLeafObj)runtimeHierarchyObj).TraverseAllLeafNodes(ProcessingStages.RunningValues);
							flag = false;
						}
						else
						{
							((RuntimeDetailObj)runtimeHierarchyObj).ReadRows(DataActions.PostSortAggregates);
						}
					}
				}
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				Global.Tracer.Assert(false);
			}

			internal override void CreateInstances(ReportItemInstance riInstance, IList instanceList, RenderingPagesRangesList pagesList)
			{
				Global.Tracer.Assert(false);
			}

			internal void CreateInstances()
			{
				if (this.m_grouping != null)
				{
					this.m_grouping.Traverse(ProcessingStages.CreatingInstances, this.m_expression.Direction);
				}
				if (this.m_hierarchyObjs != null)
				{
					bool flag = true;
					for (int i = 0; i < this.m_hierarchyObjs.Count; i++)
					{
						RuntimeHierarchyObj runtimeHierarchyObj = this.m_hierarchyObjs[i];
						if (!flag || runtimeHierarchyObj is RuntimeGroupLeafObj)
						{
							((RuntimeGroupLeafObj)runtimeHierarchyObj).TraverseAllLeafNodes(ProcessingStages.CreatingInstances);
							flag = false;
						}
						else
						{
							((RuntimeDetailObj)runtimeHierarchyObj).CreateInstance();
						}
					}
				}
			}

			internal virtual void CreateInstance()
			{
				Global.Tracer.Assert(false);
			}

			internal override void SetupEnvironment()
			{
			}

			internal override void ReadRow(DataActions dataAction)
			{
				Global.Tracer.Assert(false);
			}

			internal override bool InScope(string scope)
			{
				Global.Tracer.Assert(false);
				return false;
			}

			protected void MatchSortFilterScope(IScope outerScope, Grouping groupDef, object groupExprValue, int groupExprIndex)
			{
				if (base.m_processingContext.RuntimeSortFilterInfo != null && groupDef.SortFilterScopeInfo != null)
				{
					RuntimeSortFilterEventInfoList runtimeSortFilterInfo = base.m_processingContext.RuntimeSortFilterInfo;
					if (groupDef.SortFilterScopeMatched == null)
					{
						groupDef.SortFilterScopeMatched = new bool[runtimeSortFilterInfo.Count];
					}
					for (int i = 0; i < runtimeSortFilterInfo.Count; i++)
					{
						RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = runtimeSortFilterInfo[i];
						VariantList variantList = groupDef.SortFilterScopeInfo[i];
						if (variantList != null && outerScope.TargetScopeMatched(i, false))
						{
							if (ReportProcessing.CompareTo(((ArrayList)variantList)[groupExprIndex], groupExprValue, base.m_processingContext.CompareInfo, base.m_processingContext.ClrCompareOptions) == 0)
							{
								groupDef.SortFilterScopeMatched[i] = true;
							}
							else
							{
								groupDef.SortFilterScopeMatched[i] = false;
							}
						}
						else
						{
							groupDef.SortFilterScopeMatched[i] = false;
						}
					}
				}
			}

			protected virtual IHierarchyObj CreateHierarchyObj()
			{
				return new RuntimeHierarchyObj(this);
			}

			protected virtual void ProcessUserSort()
			{
				Global.Tracer.Assert(false);
			}

			protected virtual void MarkSortInfoProcessed(RuntimeSortFilterEventInfoList runtimeSortFilterInfo)
			{
				Global.Tracer.Assert(false);
			}

			protected virtual void AddSortInfoIndex(int sortInfoIndex, RuntimeSortFilterEventInfo sortInfo)
			{
				Global.Tracer.Assert(false);
			}
		}

		internal abstract class RuntimeGroupObj : RuntimeHierarchyObj
		{
			protected RuntimeGroupLeafObj m_lastChild;

			protected RuntimeGroupLeafObj m_firstChild;

			internal RuntimeGroupLeafObj LastChild
			{
				get
				{
					return this.m_lastChild;
				}
				set
				{
					this.m_lastChild = value;
				}
			}

			internal RuntimeGroupLeafObj FirstChild
			{
				get
				{
					return this.m_firstChild;
				}
				set
				{
					this.m_firstChild = value;
				}
			}

			internal virtual int RecursiveLevel
			{
				get
				{
					return -1;
				}
			}

			protected RuntimeGroupObj(ProcessingContext processingContext)
				: base(processingContext)
			{
			}

			internal void AddChild(RuntimeGroupLeafObj child)
			{
				if (this.m_lastChild != null)
				{
					this.m_lastChild.NextLeaf = child;
				}
				else
				{
					this.m_firstChild = child;
				}
				child.PrevLeaf = this.m_lastChild;
				child.NextLeaf = null;
				child.Parent = this;
				this.m_lastChild = child;
			}

			internal void InsertToSortTree(RuntimeGroupLeafObj groupLeaf)
			{
				RuntimeGroupRootObj runtimeGroupRootObj = (RuntimeGroupRootObj)base.m_hierarchyRoot;
				Grouping grouping = runtimeGroupRootObj.HierarchyDef.Grouping;
				if (!runtimeGroupRootObj.BuiltinSortOverridden && (ProcessingContext.SecondPassOperations.Sorting & base.m_processingContext.SecondPassOperation) != 0 && runtimeGroupRootObj.HierarchyDef.Sorting != null)
				{
					Global.Tracer.Assert(base.m_grouping != null, "(m_grouping != null)");
					runtimeGroupRootObj.LastChild = groupLeaf;
					object keyValue = base.m_processingContext.ReportRuntime.EvaluateRuntimeExpression(base.m_expression, ObjectType.Grouping, grouping.Name, "Sort");
					base.m_grouping.NextRow(keyValue);
				}
				else
				{
					Global.Tracer.Assert(grouping.Filters != null || grouping.HasInnerFilters, "(null != groupingDef.Filters || groupingDef.HasInnerFilters)");
					this.AddChild(groupLeaf);
				}
			}
		}

		internal abstract class RuntimeGroupRootObj : RuntimeGroupObj, IFilterOwner
		{
			protected ReportHierarchyNode m_hierarchyDef;

			protected IScope m_outerScope;

			protected ProcessingStages m_processingStage = ProcessingStages.Grouping;

			protected AggregatesImpl m_scopedRunningValues;

			protected DataAggregateObjList m_runningValuesInGroup;

			protected AggregatesImpl m_globalRunningValueCollection;

			protected RuntimeGroupRootObjList m_groupCollection;

			protected DataActions m_dataAction;

			protected DataActions m_outerDataAction;

			protected ReportItemInstance m_reportItemInstance;

			protected IList m_instanceList;

			protected RuntimeGroupingObj.GroupingTypes m_groupingType;

			protected Filters m_groupFilters;

			protected RuntimeExpressionInfo m_parentExpression;

			protected RenderingPagesRangesList m_pagesList;

			protected bool m_saveGroupExprValues;

			protected int[] m_sortFilterExpressionScopeInfoIndices;

			private bool[] m_builtinSortOverridden;

			internal ReportHierarchyNode HierarchyDef
			{
				get
				{
					return this.m_hierarchyDef;
				}
			}

			internal ExpressionInfoList GroupExpressions
			{
				get
				{
					return this.m_hierarchyDef.Grouping.GroupExpressions;
				}
			}

			internal GroupingExprHost GroupExpressionHost
			{
				get
				{
					return this.m_hierarchyDef.Grouping.ExprHost;
				}
			}

			internal ExpressionInfoList SortExpressions
			{
				get
				{
					return this.m_hierarchyDef.Sorting.SortExpressions;
				}
			}

			internal SortingExprHost SortExpressionHost
			{
				get
				{
					return this.m_hierarchyDef.Sorting.ExprHost;
				}
			}

			internal BoolList GroupDirections
			{
				get
				{
					return this.m_hierarchyDef.Grouping.SortDirections;
				}
			}

			internal BoolList SortDirections
			{
				get
				{
					return this.m_hierarchyDef.Sorting.SortDirections;
				}
			}

			internal RuntimeExpressionInfo Expression
			{
				get
				{
					return base.m_expression;
				}
			}

			internal AggregatesImpl ScopedRunningValues
			{
				get
				{
					return this.m_scopedRunningValues;
				}
			}

			internal AggregatesImpl GlobalRunningValueCollection
			{
				get
				{
					return this.m_globalRunningValueCollection;
				}
			}

			internal RuntimeGroupRootObjList GroupCollection
			{
				get
				{
					return this.m_groupCollection;
				}
			}

			internal DataActions DataAction
			{
				get
				{
					return this.m_dataAction;
				}
			}

			internal ProcessingStages ProcessingStage
			{
				get
				{
					return this.m_processingStage;
				}
				set
				{
					this.m_processingStage = value;
				}
			}

			internal ReportItemInstance ReportItemInstance
			{
				get
				{
					return this.m_reportItemInstance;
				}
			}

			internal IList InstanceList
			{
				get
				{
					return this.m_instanceList;
				}
			}

			internal RenderingPagesRangesList PagesList
			{
				get
				{
					return this.m_pagesList;
				}
			}

			internal RuntimeGroupingObj.GroupingTypes GroupingType
			{
				get
				{
					return this.m_groupingType;
				}
			}

			internal Filters GroupFilters
			{
				get
				{
					return this.m_groupFilters;
				}
			}

			internal bool HasParent
			{
				get
				{
					return null != this.m_parentExpression;
				}
			}

			protected override IScope OuterScope
			{
				get
				{
					return this.m_outerScope;
				}
			}

			internal bool SaveGroupExprValues
			{
				get
				{
					return this.m_saveGroupExprValues;
				}
			}

			protected override int[] SortFilterExpressionScopeInfoIndices
			{
				get
				{
					if (this.m_sortFilterExpressionScopeInfoIndices == null)
					{
						this.m_sortFilterExpressionScopeInfoIndices = new int[base.m_processingContext.RuntimeSortFilterInfo.Count];
						for (int i = 0; i < base.m_processingContext.RuntimeSortFilterInfo.Count; i++)
						{
							this.m_sortFilterExpressionScopeInfoIndices[i] = -1;
						}
					}
					return this.m_sortFilterExpressionScopeInfoIndices;
				}
			}

			internal bool BuiltinSortOverridden
			{
				get
				{
					if (base.m_processingContext.RuntimeSortFilterInfo != null && this.m_builtinSortOverridden != null)
					{
						for (int i = 0; i < base.m_processingContext.RuntimeSortFilterInfo.Count; i++)
						{
							if (base.m_processingContext.UserSortFilterContext.InProcessUserSortPhase(i) && this.m_builtinSortOverridden[i])
							{
								return true;
							}
						}
					}
					return false;
				}
			}

			protected RuntimeGroupRootObj(IScope outerScope, ReportHierarchyNode hierarchyDef, DataActions dataAction, ProcessingContext processingContext)
				: base(processingContext)
			{
				base.m_hierarchyRoot = this;
				this.m_outerScope = outerScope;
				this.m_hierarchyDef = hierarchyDef;
				Grouping grouping = hierarchyDef.Grouping;
				Global.Tracer.Assert(null != grouping, "(null != groupDef)");
				base.m_expression = new RuntimeExpressionInfo(grouping.GroupExpressions, grouping.ExprHost, grouping.SortDirections, 0);
				if (base.m_processingContext.RuntimeSortFilterInfo != null && grouping.IsSortFilterExpressionScope != null)
				{
					int count = base.m_processingContext.RuntimeSortFilterInfo.Count;
					for (int i = 0; i < count; i++)
					{
						RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = base.m_processingContext.RuntimeSortFilterInfo[i];
						if ((runtimeSortFilterEventInfo.EventSource.ContainingScopes == null || runtimeSortFilterEventInfo.EventSource.ContainingScopes.Count == 0 || runtimeSortFilterEventInfo.EventSourceScope != null) && grouping.IsSortFilterExpressionScope[i] && base.m_processingContext.UserSortFilterContext.InProcessUserSortPhase(i) && this.TargetScopeMatched(i, false))
						{
							if (this.m_builtinSortOverridden == null)
							{
								this.m_builtinSortOverridden = new bool[count];
							}
							this.m_builtinSortOverridden[i] = true;
						}
					}
				}
				if (grouping.GroupAndSort && !this.BuiltinSortOverridden)
				{
					this.m_groupingType = RuntimeGroupingObj.GroupingTypes.Sort;
				}
				else
				{
					this.m_groupingType = RuntimeGroupingObj.GroupingTypes.Hash;
				}
				base.m_grouping = new RuntimeGroupingObj(this, this.m_groupingType);
				if (grouping.Filters == null)
				{
					this.m_dataAction = dataAction;
					this.m_outerDataAction = dataAction;
				}
				if (grouping.RecursiveAggregates != null)
				{
					this.m_dataAction |= DataActions.RecursiveAggregates;
				}
				if (grouping.PostSortAggregates != null)
				{
					this.m_dataAction |= DataActions.PostSortAggregates;
				}
				if (grouping.Parent != null)
				{
					this.m_parentExpression = new RuntimeExpressionInfo(grouping.Parent, grouping.ParentExprHost, null, 0);
				}
				this.m_saveGroupExprValues = grouping.SaveGroupExprValues;
				if (this.m_hierarchyDef.Grouping.NeedScopeInfoForSortFilterExpression != null && base.m_processingContext.RuntimeSortFilterInfo != null)
				{
					int num = 0;
					while (true)
					{
						if (num < base.m_processingContext.RuntimeSortFilterInfo.Count)
						{
							if (this.m_hierarchyDef.Grouping.NeedScopeInfoForSortFilterExpression[num] && this.m_outerScope.TargetScopeMatched(num, false))
							{
								break;
							}
							num++;
							continue;
						}
						return;
					}
					this.m_saveGroupExprValues = true;
				}
			}

			internal override void GetScopeValues(IHierarchyObj targetScopeObj, VariantList[] scopeValues, ref int index)
			{
				if (this != targetScopeObj)
				{
					this.m_outerScope.GetScopeValues(targetScopeObj, scopeValues, ref index);
				}
			}

			internal override bool TargetScopeMatched(int index, bool detailSort)
			{
				return this.m_outerScope.TargetScopeMatched(index, detailSort);
			}

			internal override void NextRow()
			{
				if (this.ProcessThisRow())
				{
					Global.Tracer.Assert(null != base.m_grouping, "(null != m_grouping)");
					object obj = this.EvaluateGroupExpression(base.m_expression, "Group");
					Grouping grouping = this.m_hierarchyDef.Grouping;
					if (this.m_saveGroupExprValues)
					{
						grouping.CurrentGroupExpressionValues = new VariantList();
						grouping.CurrentGroupExpressionValues.Add(obj);
					}
					base.MatchSortFilterScope(this.m_outerScope, grouping, obj, 0);
					object parentKey = null;
					bool flag = null != this.m_parentExpression;
					if (flag)
					{
						parentKey = this.EvaluateGroupExpression(this.m_parentExpression, "Parent");
					}
					base.m_grouping.NextRow(obj, flag, parentKey);
				}
			}

			protected object EvaluateGroupExpression(RuntimeExpressionInfo expression, string propertyName)
			{
				Global.Tracer.Assert(null != this.m_hierarchyDef.Grouping, "(null != m_hierarchyDef.Grouping)");
				return base.m_processingContext.ReportRuntime.EvaluateRuntimeExpression(expression, ObjectType.Grouping, this.m_hierarchyDef.Grouping.Name, propertyName);
			}

			protected bool ProcessThisRow()
			{
				FieldsImpl fieldsImpl = base.m_processingContext.ReportObjectModel.FieldsImpl;
				if (fieldsImpl.IsAggregateRow && 0 > fieldsImpl.AggregationFieldCount)
				{
					return false;
				}
				int[] groupExpressionFieldIndices = this.m_hierarchyDef.Grouping.GetGroupExpressionFieldIndices();
				if (groupExpressionFieldIndices == null)
				{
					fieldsImpl.ValidAggregateRow = false;
				}
				else
				{
					foreach (int num in groupExpressionFieldIndices)
					{
						if (-1 > num || (0 <= num && !fieldsImpl[num].IsAggregationField))
						{
							fieldsImpl.ValidAggregateRow = false;
						}
					}
				}
				if (fieldsImpl.IsAggregateRow && !fieldsImpl.ValidAggregateRow)
				{
					return false;
				}
				return true;
			}

			internal void AddChildWithNoParent(RuntimeGroupLeafObj child)
			{
				if (RuntimeGroupingObj.GroupingTypes.Sort == this.m_groupingType)
				{
					child.Parent = this;
				}
				else
				{
					base.AddChild(child);
				}
			}

			internal override bool SortAndFilter()
			{
				RuntimeGroupingObj grouping = base.m_grouping;
				bool direction = base.m_expression.Direction;
				bool result = true;
				bool flag = !this.BuiltinSortOverridden && (ProcessingContext.SecondPassOperations.Sorting & base.m_processingContext.SecondPassOperation) != 0 && null != this.m_hierarchyDef.Sorting;
				bool flag2 = (ProcessingContext.SecondPassOperations.Filtering & base.m_processingContext.SecondPassOperation) != 0 && (this.m_hierarchyDef.Grouping.Filters != null || this.m_hierarchyDef.Grouping.HasInnerFilters);
				if (flag)
				{
					base.m_expression = new RuntimeExpressionInfo(this.m_hierarchyDef.Sorting.SortExpressions, this.m_hierarchyDef.Sorting.ExprHost, this.m_hierarchyDef.Sorting.SortDirections, 0);
					this.m_groupingType = RuntimeGroupingObj.GroupingTypes.Sort;
					base.m_grouping = new RuntimeGroupingObj(this, this.m_groupingType);
				}
				else if (flag2)
				{
					this.m_groupingType = RuntimeGroupingObj.GroupingTypes.None;
					base.m_grouping = new RuntimeGroupingObj(this, this.m_groupingType);
				}
				if (flag2)
				{
					this.m_groupFilters = new Filters(Filters.FilterTypes.GroupFilter, this, this.m_hierarchyDef.Grouping.Filters, ObjectType.Grouping, this.m_hierarchyDef.Grouping.Name, base.m_processingContext);
				}
				this.m_processingStage = ProcessingStages.SortAndFilter;
				base.m_lastChild = null;
				grouping.Traverse(ProcessingStages.SortAndFilter, direction);
				if (flag2)
				{
					this.m_groupFilters.FinishReadingRows();
					if (!flag && base.m_lastChild == null)
					{
						base.m_firstChild = null;
						result = false;
					}
				}
				return result;
			}

			void IFilterOwner.PostFilterNextRow()
			{
				Global.Tracer.Assert(false);
			}

			internal virtual void AddScopedRunningValue(DataAggregateObj runningValueObj, bool escalate)
			{
				if (this.m_scopedRunningValues == null)
				{
					this.m_scopedRunningValues = new AggregatesImpl(base.m_processingContext.ReportRuntime);
				}
				if (this.m_scopedRunningValues.GetAggregateObj(runningValueObj.Name) == null)
				{
					this.m_scopedRunningValues.Add(runningValueObj);
				}
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				this.SetupRunningValues(globalRVCol, groupCol);
			}

			protected void SetupRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol)
			{
				this.m_globalRunningValueCollection = globalRVCol;
				this.m_groupCollection = groupCol;
				if (this.m_hierarchyDef.Grouping.Name != null)
				{
					groupCol[this.m_hierarchyDef.Grouping.Name] = this;
				}
			}

			protected void AddRunningValues(RunningValueInfoList runningValues)
			{
				this.AddRunningValues(runningValues, ref this.m_runningValuesInGroup, this.m_globalRunningValueCollection, this.m_groupCollection);
			}

			protected void AddRunningValues(RunningValueInfoList runningValues, ref DataAggregateObjList runningValuesInGroup, AggregatesImpl globalRunningValueCollection, RuntimeGroupRootObjList groupCollection)
			{
				if (runningValues != null && 0 < runningValues.Count)
				{
					if (runningValuesInGroup == null)
					{
						runningValuesInGroup = new DataAggregateObjList();
					}
					for (int i = 0; i < runningValues.Count; i++)
					{
						RunningValueInfo runningValueInfo = runningValues[i];
						DataAggregateObj dataAggregateObj = globalRunningValueCollection.GetAggregateObj(runningValueInfo.Name);
						if (dataAggregateObj == null)
						{
							dataAggregateObj = new DataAggregateObj(runningValueInfo, base.m_processingContext);
							globalRunningValueCollection.Add(dataAggregateObj);
						}
						if (runningValueInfo.Scope != null)
						{
							RuntimeGroupRootObj runtimeGroupRootObj = groupCollection[runningValueInfo.Scope];
							if (runtimeGroupRootObj != null)
							{
								runtimeGroupRootObj.AddScopedRunningValue(dataAggregateObj, false);
							}
							else if (base.m_processingContext.PivotEscalateScope())
							{
								this.AddScopedRunningValue(dataAggregateObj, true);
							}
						}
						runningValuesInGroup.Add(dataAggregateObj);
					}
				}
			}

			internal override void ReadRow(DataActions dataAction)
			{
				if (DataActions.PostSortAggregates == dataAction && this.m_runningValuesInGroup != null)
				{
					for (int i = 0; i < this.m_runningValuesInGroup.Count; i++)
					{
						this.m_runningValuesInGroup[i].Update();
					}
				}
				if (this.m_outerScope != null && (dataAction & this.m_outerDataAction) != 0)
				{
					this.m_outerScope.ReadRow(dataAction);
				}
			}

			internal override void CreateInstances(ReportItemInstance riInstance, IList instanceList, RenderingPagesRangesList pagesList)
			{
				this.m_reportItemInstance = riInstance;
				this.m_instanceList = instanceList;
				this.m_pagesList = pagesList;
				if (Report.ShowHideTypes.Interactive == base.m_processingContext.ShowHideType && this.m_parentExpression != null)
				{
					base.m_processingContext.EnterChildGroupings();
				}
				base.m_grouping.Traverse(ProcessingStages.CreatingInstances, base.m_expression.Direction);
				if (Report.ShowHideTypes.Interactive == base.m_processingContext.ShowHideType && this.m_parentExpression != null)
				{
					base.m_processingContext.ExitChildGroupings();
				}
			}
		}

		internal abstract class RuntimeGroupLeafObj : RuntimeGroupObj
		{
			protected DataAggregateObjList m_nonCustomAggregates;

			protected DataAggregateObjList m_customAggregates;

			protected FieldImpl[] m_firstRow;

			protected bool m_firstRowIsAggregate;

			protected RuntimeGroupLeafObj m_nextLeaf;

			protected RuntimeGroupLeafObj m_prevLeaf;

			protected DataRowList m_dataRows;

			protected RuntimeGroupObj m_parent;

			protected DataAggregateObjList m_recursiveAggregates;

			protected DataAggregateObjList m_postSortAggregates;

			protected int m_recursiveLevel;

			protected VariantList m_groupExprValues;

			protected bool[] m_targetScopeMatched;

			protected DataActions m_dataAction;

			protected RuntimeUserSortTargetInfo m_userSortTargetInfo;

			protected int[] m_sortFilterExpressionScopeInfoIndices;

			internal RuntimeGroupLeafObj NextLeaf
			{
				set
				{
					this.m_nextLeaf = value;
				}
			}

			internal RuntimeGroupLeafObj PrevLeaf
			{
				set
				{
					this.m_prevLeaf = value;
				}
			}

			internal RuntimeGroupObj Parent
			{
				get
				{
					return this.m_parent;
				}
				set
				{
					this.m_parent = value;
				}
			}

			protected override IScope OuterScope
			{
				get
				{
					return base.m_hierarchyRoot;
				}
			}

			protected override string ScopeName
			{
				get
				{
					return ((RuntimeGroupRootObj)base.m_hierarchyRoot).HierarchyDef.Grouping.Name;
				}
			}

			internal override int RecursiveLevel
			{
				get
				{
					return this.m_recursiveLevel;
				}
			}

			internal Grouping GroupingDef
			{
				get
				{
					return ((RuntimeGroupRootObj)base.m_hierarchyRoot).HierarchyDef.Grouping;
				}
			}

			protected override IHierarchyObj HierarchyRoot
			{
				get
				{
					if (ProcessingStages.UserSortFilter == ((RuntimeGroupRootObj)base.m_hierarchyRoot).ProcessingStage)
					{
						return this;
					}
					return base.m_hierarchyRoot;
				}
			}

			protected override BTreeNode SortTree
			{
				get
				{
					if (ProcessingStages.UserSortFilter == ((RuntimeGroupRootObj)base.m_hierarchyRoot).ProcessingStage)
					{
						if (this.m_userSortTargetInfo != null)
						{
							return this.m_userSortTargetInfo.SortTree;
						}
						return null;
					}
					return base.m_grouping.Tree;
				}
				set
				{
					if (ProcessingStages.UserSortFilter == ((RuntimeGroupRootObj)base.m_hierarchyRoot).ProcessingStage)
					{
						if (this.m_userSortTargetInfo != null)
						{
							this.m_userSortTargetInfo.SortTree = value;
						}
						else
						{
							Global.Tracer.Assert(false);
						}
					}
					else
					{
						base.m_grouping.Tree = value;
					}
				}
			}

			protected override int ExpressionIndex
			{
				get
				{
					if (ProcessingStages.UserSortFilter == ((RuntimeGroupRootObj)base.m_hierarchyRoot).ProcessingStage)
					{
						return 0;
					}
					Global.Tracer.Assert(false);
					return -1;
				}
			}

			protected override IntList SortFilterInfoIndices
			{
				get
				{
					if (this.m_userSortTargetInfo != null)
					{
						return this.m_userSortTargetInfo.SortFilterInfoIndices;
					}
					return null;
				}
			}

			internal override bool TargetForNonDetailSort
			{
				get
				{
					if (this.m_userSortTargetInfo != null && this.m_userSortTargetInfo.TargetForNonDetailSort)
					{
						return true;
					}
					return base.m_hierarchyRoot.TargetForNonDetailSort;
				}
			}

			protected override int[] SortFilterExpressionScopeInfoIndices
			{
				get
				{
					if (this.m_sortFilterExpressionScopeInfoIndices == null)
					{
						this.m_sortFilterExpressionScopeInfoIndices = new int[base.m_processingContext.RuntimeSortFilterInfo.Count];
						for (int i = 0; i < base.m_processingContext.RuntimeSortFilterInfo.Count; i++)
						{
							this.m_sortFilterExpressionScopeInfoIndices[i] = -1;
						}
					}
					return this.m_sortFilterExpressionScopeInfoIndices;
				}
			}

			protected RuntimeGroupLeafObj(RuntimeGroupRootObj groupRoot)
				: base(groupRoot.ProcessingContext)
			{
				ReportHierarchyNode hierarchyDef = groupRoot.HierarchyDef;
				base.m_hierarchyRoot = groupRoot;
				Grouping grouping = hierarchyDef.Grouping;
				RuntimeDataRegionObj.CreateAggregates(base.m_processingContext, grouping.Aggregates, ref this.m_nonCustomAggregates, ref this.m_customAggregates);
				RuntimeDataRegionObj.CreateAggregates(base.m_processingContext, grouping.RecursiveAggregates, ref this.m_recursiveAggregates);
				RuntimeDataRegionObj.CreateAggregates(base.m_processingContext, grouping.PostSortAggregates, ref this.m_postSortAggregates);
				if (groupRoot.SaveGroupExprValues)
				{
					this.m_groupExprValues = grouping.CurrentGroupExpressionValues;
				}
			}

			internal override bool IsTargetForSort(int index, bool detailSort)
			{
				if (this.m_userSortTargetInfo != null && this.m_userSortTargetInfo.IsTargetForSort(index, detailSort))
				{
					return true;
				}
				return base.m_hierarchyRoot.IsTargetForSort(index, detailSort);
			}

			protected virtual void ConstructRuntimeStructure(ref bool handleMyDataAction, out DataActions innerDataAction)
			{
				if (this.m_postSortAggregates != null || (this.m_recursiveAggregates != null && base.m_processingContext.SpecialRecursiveAggregates))
				{
					handleMyDataAction = true;
				}
				if (handleMyDataAction)
				{
					innerDataAction = DataActions.None;
				}
				else
				{
					innerDataAction = ((RuntimeGroupRootObj)base.m_hierarchyRoot).DataAction;
				}
			}

			protected bool HandleSortFilterEvent()
			{
				if (base.m_processingContext.RuntimeSortFilterInfo == null)
				{
					return false;
				}
				Grouping groupingDef = this.GroupingDef;
				int count = base.m_processingContext.RuntimeSortFilterInfo.Count;
				if (groupingDef.SortFilterScopeMatched != null || groupingDef.NeedScopeInfoForSortFilterExpression != null)
				{
					this.m_targetScopeMatched = new bool[count];
					for (int i = 0; i < count; i++)
					{
						if (groupingDef.SortFilterScopeMatched != null && -1 != groupingDef.SortFilterScopeIndex[i])
						{
							if (groupingDef.SortFilterScopeMatched[i])
							{
								this.m_targetScopeMatched[i] = true;
								RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = base.m_processingContext.RuntimeSortFilterInfo[i];
								if (groupingDef.IsSortFilterTarget != null && groupingDef.IsSortFilterTarget[i] && !base.m_hierarchyRoot.TargetForNonDetailSort)
								{
									runtimeSortFilterEventInfo.EventTarget = this;
									if (this.m_userSortTargetInfo == null)
									{
										this.m_userSortTargetInfo = new RuntimeUserSortTargetInfo(this, i, runtimeSortFilterEventInfo);
									}
									else
									{
										this.m_userSortTargetInfo.AddSortInfo(this, i, runtimeSortFilterEventInfo);
									}
								}
								Global.Tracer.Assert(null != runtimeSortFilterEventInfo.EventSource.ContainingScopes, "(null != sortFilterInfo.EventSource.ContainingScopes)");
								if (groupingDef == runtimeSortFilterEventInfo.EventSource.ContainingScopes.LastEntry && !runtimeSortFilterEventInfo.EventSource.IsMatrixCellScope && !base.m_hierarchyRoot.TargetForNonDetailSort)
								{
									Global.Tracer.Assert(null == runtimeSortFilterEventInfo.EventSourceScope, "(null == sortFilterInfo.EventSourceScope)");
									runtimeSortFilterEventInfo.EventSourceScope = this;
								}
							}
							else
							{
								this.m_targetScopeMatched[i] = false;
							}
						}
						else
						{
							this.m_targetScopeMatched[i] = ((RuntimeGroupRootObj)base.m_hierarchyRoot).TargetScopeMatched(i, false);
						}
					}
				}
				base.m_processingContext.RegisterSortFilterExpressionScope(base.m_hierarchyRoot, this, groupingDef.IsSortFilterExpressionScope);
				if (this.m_userSortTargetInfo != null && this.m_userSortTargetInfo.TargetForNonDetailSort)
				{
					return true;
				}
				return false;
			}

			internal override void GetScopeValues(IHierarchyObj targetScopeObj, VariantList[] scopeValues, ref int index)
			{
				if (this != targetScopeObj)
				{
					((RuntimeDataRegionObj)base.m_hierarchyRoot).GetScopeValues(targetScopeObj, scopeValues, ref index);
					Global.Tracer.Assert(null != this.m_groupExprValues, "(null != m_groupExprValues)");
					Global.Tracer.Assert(index < scopeValues.Length, "(index < scopeValues.Length)");
					scopeValues[index++] = this.m_groupExprValues;
				}
			}

			internal override bool TargetScopeMatched(int index, bool detailSort)
			{
				if (detailSort && this.GroupingDef.SortFilterScopeInfo == null)
				{
					return true;
				}
				if (this.m_targetScopeMatched != null)
				{
					return this.m_targetScopeMatched[index];
				}
				return false;
			}

			internal override void NextRow()
			{
				this.UpdateAggregateInfo();
				this.InternalNextRow();
			}

			protected void UpdateAggregateInfo()
			{
				FieldsImpl fieldsImpl = base.m_processingContext.ReportObjectModel.FieldsImpl;
				if (fieldsImpl.ValidAggregateRow)
				{
					int[] groupExpressionFieldIndices = this.GroupingDef.GetGroupExpressionFieldIndices();
					if (groupExpressionFieldIndices != null)
					{
						foreach (int num in groupExpressionFieldIndices)
						{
							if (num >= 0)
							{
								FieldImpl fieldImpl = fieldsImpl[num];
								if (!fieldImpl.AggregationFieldChecked && fieldImpl.IsAggregationField)
								{
									fieldImpl.AggregationFieldChecked = true;
									fieldsImpl.AggregationFieldCount--;
								}
							}
						}
					}
					if (fieldsImpl.AggregationFieldCount == 0 && this.m_customAggregates != null)
					{
						RuntimeDataRegionObj.UpdateAggregates(base.m_processingContext, this.m_customAggregates, false);
					}
				}
			}

			protected void InternalNextRow()
			{
				RuntimeGroupRootObj runtimeGroupRootObj = (RuntimeGroupRootObj)base.m_hierarchyRoot;
				ProcessingStages processingStage = runtimeGroupRootObj.ProcessingStage;
				runtimeGroupRootObj.ProcessingStage = ProcessingStages.UserSortFilter;
				RuntimeDataRegionObj.CommonFirstRow(base.m_processingContext.ReportObjectModel.FieldsImpl, ref this.m_firstRowIsAggregate, ref this.m_firstRow);
				if (base.m_processingContext.ReportObjectModel.FieldsImpl.IsAggregateRow)
				{
					base.ScopeNextAggregateRow(this.m_userSortTargetInfo);
				}
				else
				{
					base.ScopeNextNonAggregateRow(this.m_nonCustomAggregates, this.m_dataRows);
				}
				runtimeGroupRootObj.ProcessingStage = processingStage;
			}

			protected override void SendToInner()
			{
				((RuntimeGroupRootObj)base.m_hierarchyRoot).ProcessingStage = ProcessingStages.Grouping;
			}

			internal void RemoveFromParent(RuntimeGroupObj parent)
			{
				if (this.m_prevLeaf == null)
				{
					parent.FirstChild = this.m_nextLeaf;
				}
				else
				{
					this.m_prevLeaf.m_nextLeaf = this.m_nextLeaf;
				}
				if (this.m_nextLeaf == null)
				{
					parent.LastChild = this.m_prevLeaf;
				}
				else
				{
					this.m_nextLeaf.m_prevLeaf = this.m_prevLeaf;
				}
			}

			private RuntimeGroupLeafObj Traverse(ProcessingStages operation)
			{
				RuntimeGroupLeafObj nextLeaf = this.m_nextLeaf;
				if (((RuntimeGroupRootObj)base.m_hierarchyRoot).HasParent)
				{
					this.m_recursiveLevel = this.m_parent.RecursiveLevel + 1;
				}
				bool flag = this.IsSpecialFilteringPass(operation);
				if (flag)
				{
					base.m_lastChild = null;
					this.ProcessChildren(operation);
				}
				switch (operation)
				{
				case ProcessingStages.SortAndFilter:
					this.SortAndFilter();
					break;
				case ProcessingStages.RunningValues:
					this.CalculateRunningValues();
					break;
				case ProcessingStages.CreatingInstances:
					this.CreateInstance();
					break;
				}
				if (!flag)
				{
					this.ProcessChildren(operation);
				}
				return nextLeaf;
			}

			internal void TraverseAllLeafNodes(ProcessingStages operation)
			{
				for (RuntimeGroupLeafObj runtimeGroupLeafObj = this; runtimeGroupLeafObj != null; runtimeGroupLeafObj = runtimeGroupLeafObj.Traverse(operation))
				{
				}
			}

			private void ProcessChildren(ProcessingStages operation)
			{
				RuntimeGroupRootObj runtimeGroupRootObj = (RuntimeGroupRootObj)base.m_hierarchyRoot;
				if (base.m_firstChild != null || base.m_grouping != null)
				{
					if (ProcessingStages.CreatingInstances == operation && Report.ShowHideTypes.Interactive == base.m_processingContext.ShowHideType)
					{
						base.m_processingContext.EnterChildGroupings();
					}
					if (base.m_firstChild != null)
					{
						base.m_firstChild.TraverseAllLeafNodes(operation);
						if (operation == ProcessingStages.SortAndFilter)
						{
							if ((ProcessingContext.SecondPassOperations.Filtering & base.m_processingContext.SecondPassOperation) != 0 && runtimeGroupRootObj.HierarchyDef.Grouping.Filters != null)
							{
								if (base.m_lastChild == null)
								{
									base.m_firstChild = null;
								}
							}
							else if (base.m_grouping != null)
							{
								base.m_firstChild = null;
							}
						}
					}
					else if (base.m_grouping != null)
					{
						base.m_grouping.Traverse(operation, runtimeGroupRootObj.Expression.Direction);
					}
					if (ProcessingStages.CreatingInstances == operation && Report.ShowHideTypes.Interactive == base.m_processingContext.ShowHideType)
					{
						base.m_processingContext.ExitChildGroupings();
					}
				}
				if (ProcessingStages.CreatingInstances == operation)
				{
					this.AddToDocumentMap();
				}
			}

			private bool IsSpecialFilteringPass(ProcessingStages operation)
			{
				if (ProcessingStages.SortAndFilter == operation && base.m_processingContext.SpecialRecursiveAggregates && (ProcessingContext.SecondPassOperations.Filtering & base.m_processingContext.SecondPassOperation) != 0)
				{
					return true;
				}
				return false;
			}

			internal override bool SortAndFilter()
			{
				bool flag = true;
				bool flag2 = false;
				RuntimeGroupRootObj runtimeGroupRootObj = (RuntimeGroupRootObj)base.m_hierarchyRoot;
				Global.Tracer.Assert(null != runtimeGroupRootObj, "(null != groupRoot)");
				if (!runtimeGroupRootObj.BuiltinSortOverridden && (ProcessingContext.SecondPassOperations.Sorting & base.m_processingContext.SecondPassOperation) != 0 && null != runtimeGroupRootObj.HierarchyDef.Sorting && base.m_firstChild != null)
				{
					base.m_expression = runtimeGroupRootObj.Expression;
					base.m_grouping = new RuntimeGroupingObj(this, RuntimeGroupingObj.GroupingTypes.Sort);
				}
				base.m_lastChild = null;
				if ((runtimeGroupRootObj.BuiltinSortOverridden || runtimeGroupRootObj.HierarchyDef.Sorting == null) && runtimeGroupRootObj.GroupFilters == null && this.m_recursiveAggregates == null)
				{
					goto IL_0119;
				}
				if ((ProcessingContext.SecondPassOperations.Filtering & base.m_processingContext.SecondPassOperation) != 0)
				{
					if (base.m_processingContext.SpecialRecursiveAggregates && this.m_recursiveAggregates != null)
					{
						Global.Tracer.Assert(null != this.m_dataRows, "(null != m_dataRows)");
						this.ReadRows(false);
					}
					if (runtimeGroupRootObj.GroupFilters != null)
					{
						this.SetupEnvironment();
						flag = runtimeGroupRootObj.GroupFilters.PassFilters((object)this, out flag2);
					}
				}
				if (flag)
				{
					this.PostFilterNextRow();
				}
				else if (!flag2)
				{
					this.FailFilter();
				}
				goto IL_0119;
				IL_0119:
				return flag;
			}

			internal void FailFilter()
			{
				RuntimeGroupLeafObj runtimeGroupLeafObj = null;
				bool flag = false;
				if (this.IsSpecialFilteringPass(ProcessingStages.SortAndFilter))
				{
					flag = true;
				}
				for (RuntimeGroupLeafObj runtimeGroupLeafObj2 = base.m_firstChild; runtimeGroupLeafObj2 != null; runtimeGroupLeafObj2 = runtimeGroupLeafObj)
				{
					runtimeGroupLeafObj = runtimeGroupLeafObj2.m_nextLeaf;
					runtimeGroupLeafObj2.m_parent = this.m_parent;
					if (flag)
					{
						this.m_parent.AddChild(runtimeGroupLeafObj2);
					}
				}
			}

			internal void PostFilterNextRow()
			{
				RuntimeGroupRootObj runtimeGroupRootObj = (RuntimeGroupRootObj)base.m_hierarchyRoot;
				if ((ProcessingContext.SecondPassOperations.Filtering & base.m_processingContext.SecondPassOperation) != 0 && this.m_dataRows != null && (this.m_dataAction & DataActions.RecursiveAggregates) != 0)
				{
					if (base.m_processingContext.SpecialRecursiveAggregates)
					{
						this.ReadRows(true);
					}
					else
					{
						this.ReadRows(DataActions.RecursiveAggregates);
					}
					base.ReleaseDataRows(DataActions.RecursiveAggregates, ref this.m_dataAction, ref this.m_dataRows);
				}
				if ((ProcessingContext.SecondPassOperations.Sorting & base.m_processingContext.SecondPassOperation) != 0 && !runtimeGroupRootObj.BuiltinSortOverridden && runtimeGroupRootObj.HierarchyDef.Sorting != null)
				{
					this.SetupEnvironment();
				}
				if ((runtimeGroupRootObj.BuiltinSortOverridden || runtimeGroupRootObj.HierarchyDef.Sorting == null) && runtimeGroupRootObj.GroupFilters == null)
				{
					return;
				}
				this.m_nextLeaf = null;
				this.m_parent.InsertToSortTree(this);
			}

			internal override void CalculateRunningValues()
			{
				this.ResetScopedRunningValues();
			}

			internal override void ReadRow(DataActions dataAction)
			{
				Global.Tracer.Assert(DataActions.UserSort != dataAction, "(DataActions.UserSort != dataAction)");
				if (DataActions.PostSortAggregates == dataAction)
				{
					if (this.m_postSortAggregates != null)
					{
						RuntimeDataRegionObj.UpdateAggregates(base.m_processingContext, this.m_postSortAggregates, false);
					}
					Global.Tracer.Assert(null != base.m_hierarchyRoot, "(null != m_hierarchyRoot)");
					((IScope)base.m_hierarchyRoot).ReadRow(DataActions.PostSortAggregates);
				}
				else
				{
					Global.Tracer.Assert(DataActions.RecursiveAggregates == dataAction, "(DataActions.RecursiveAggregates == dataAction)");
					if (this.m_recursiveAggregates != null)
					{
						RuntimeDataRegionObj.UpdateAggregates(base.m_processingContext, this.m_recursiveAggregates, false);
					}
					((IScope)this.m_parent).ReadRow(DataActions.RecursiveAggregates);
				}
			}

			private void ReadRow(bool sendToParent)
			{
				if (!sendToParent)
				{
					Global.Tracer.Assert(null != this.m_recursiveAggregates, "(null != m_recursiveAggregates)");
					RuntimeDataRegionObj.UpdateAggregates(base.m_processingContext, this.m_recursiveAggregates, false);
				}
				else
				{
					((IScope)this.m_parent).ReadRow(DataActions.RecursiveAggregates);
				}
			}

			internal override void CreateInstances(ReportItemInstance riInstance, IList instanceList, RenderingPagesRangesList pagesList)
			{
				Global.Tracer.Assert(false);
			}

			protected virtual void AddToDocumentMap()
			{
			}

			internal override void SetupEnvironment()
			{
				base.SetupEnvironment(this.m_nonCustomAggregates, this.m_customAggregates, this.m_firstRow);
				base.SetupAggregates(this.m_recursiveAggregates);
				base.SetupAggregates(this.m_postSortAggregates);
				if (((RuntimeGroupRootObj)base.m_hierarchyRoot).HasParent)
				{
					this.GroupingDef.RecursiveLevel = this.m_recursiveLevel;
				}
				if (((RuntimeGroupRootObj)base.m_hierarchyRoot).SaveGroupExprValues)
				{
					this.GroupingDef.CurrentGroupExpressionValues = this.m_groupExprValues;
				}
			}

			protected void ReadRows(DataActions action)
			{
				for (int i = 0; i < this.m_dataRows.Count; i++)
				{
					FieldImpl[] fields = this.m_dataRows[i];
					base.m_processingContext.ReportObjectModel.FieldsImpl.SetFields(fields);
					this.ReadRow(action);
				}
			}

			private void ReadRows(bool sendToParent)
			{
				for (int i = 0; i < this.m_dataRows.Count; i++)
				{
					FieldImpl[] fields = this.m_dataRows[i];
					base.m_processingContext.ReportObjectModel.FieldsImpl.SetFields(fields);
					this.ReadRow(sendToParent);
				}
			}

			protected virtual void ResetScopedRunningValues()
			{
				RuntimeGroupRootObj runtimeGroupRootObj = (RuntimeGroupRootObj)base.m_hierarchyRoot;
				if (runtimeGroupRootObj.ScopedRunningValues != null)
				{
					foreach (DataAggregateObj @object in runtimeGroupRootObj.ScopedRunningValues.Objects)
					{
						@object.Init();
					}
				}
			}

			protected void ResetReportItemsWithHideDuplicates()
			{
				RuntimeGroupRootObj runtimeGroupRootObj = (RuntimeGroupRootObj)base.m_hierarchyRoot;
				ReportItemList reportItemsWithHideDuplicates = runtimeGroupRootObj.HierarchyDef.Grouping.ReportItemsWithHideDuplicates;
				if (reportItemsWithHideDuplicates != null)
				{
					for (int i = 0; i < reportItemsWithHideDuplicates.Count; i++)
					{
						TextBox textBox = reportItemsWithHideDuplicates[i] as TextBox;
						Global.Tracer.Assert(textBox != null && null != textBox.HideDuplicates);
						textBox.HasOldResult = false;
					}
				}
			}

			internal override bool InScope(string scope)
			{
				Grouping grouping = ((RuntimeGroupRootObj)base.m_hierarchyRoot).HierarchyDef.Grouping;
				if (grouping.ScopeNames == null)
				{
					bool inPivotCell = grouping.InPivotCell;
					bool result = default(bool);
					grouping.ScopeNames = base.GetScopeNames((RuntimeDataRegionObj)this, scope, ref inPivotCell, out result);
					grouping.InPivotCell = inPivotCell;
					return result;
				}
				return grouping.ScopeNames.Contains(scope);
			}

			protected override int GetRecursiveLevel(string scope)
			{
				if (scope == null)
				{
					return this.m_recursiveLevel;
				}
				Grouping grouping = ((RuntimeGroupRootObj)base.m_hierarchyRoot).HierarchyDef.Grouping;
				if (grouping.ScopeNames == null)
				{
					bool inPivotCell = grouping.InPivotCell;
					int result = default(int);
					grouping.ScopeNames = base.GetScopeNames((RuntimeDataRegionObj)this, scope, ref inPivotCell, out result);
					grouping.InPivotCell = inPivotCell;
					return result;
				}
				Grouping grouping2 = grouping.ScopeNames[scope] as Grouping;
				if (grouping2 != null)
				{
					return grouping2.RecursiveLevel;
				}
				return -1;
			}

			protected override void ProcessUserSort()
			{
				((RuntimeGroupRootObj)base.m_hierarchyRoot).ProcessingStage = ProcessingStages.UserSortFilter;
				base.m_processingContext.ProcessUserSortForTarget((IHierarchyObj)this, ref this.m_dataRows, this.m_userSortTargetInfo.TargetForNonDetailSort);
				if (this.m_userSortTargetInfo.TargetForNonDetailSort)
				{
					this.m_dataAction &= ~DataActions.UserSort;
					this.m_userSortTargetInfo.ResetTargetForNonDetailSort();
					this.m_userSortTargetInfo.EnterProcessUserSortPhase(base.m_processingContext);
					bool flag = false;
					DataActions dataActions = default(DataActions);
					this.ConstructRuntimeStructure(ref flag, out dataActions);
					if (!flag)
					{
						Global.Tracer.Assert(dataActions == this.m_dataAction, "(innerDataAction == m_dataAction)");
					}
					if (this.m_dataAction != 0)
					{
						this.m_dataRows = new DataRowList();
					}
					base.ScopeFinishSorting(ref this.m_firstRow, this.m_userSortTargetInfo);
					this.m_userSortTargetInfo.LeaveProcessUserSortPhase(base.m_processingContext);
				}
			}

			protected override IHierarchyObj CreateHierarchyObj()
			{
				if (ProcessingStages.UserSortFilter == ((RuntimeGroupRootObj)base.m_hierarchyRoot).ProcessingStage)
				{
					return new RuntimeSortHierarchyObj(this);
				}
				return base.CreateHierarchyObj();
			}

			protected override void MarkSortInfoProcessed(RuntimeSortFilterEventInfoList runtimeSortFilterInfo)
			{
				if (this.m_userSortTargetInfo != null)
				{
					this.m_userSortTargetInfo.MarkSortInfoProcessed(runtimeSortFilterInfo, this);
				}
			}

			protected override void AddSortInfoIndex(int sortInfoIndex, RuntimeSortFilterEventInfo sortInfo)
			{
				if (this.m_userSortTargetInfo != null)
				{
					this.m_userSortTargetInfo.AddSortInfoIndex(sortInfoIndex, sortInfo);
				}
			}

			protected override void GetGroupNameValuePairs(Dictionary<string, object> pairs)
			{
				Grouping grouping = ((RuntimeGroupRootObj)base.m_hierarchyRoot).HierarchyDef.Grouping;
				if (grouping.ScopeNames == null)
				{
					bool inPivotCell = grouping.InPivotCell;
					grouping.ScopeNames = base.GetScopeNames(this, ref inPivotCell, pairs);
					grouping.InPivotCell = inPivotCell;
				}
				else
				{
					IEnumerator enumerator = grouping.ScopeNames.Values.GetEnumerator();
					while (enumerator.MoveNext())
					{
						RuntimeDataRegionObj.AddGroupNameValuePair(base.m_processingContext, enumerator.Current as Grouping, pairs);
					}
				}
			}
		}

		internal abstract class RuntimeDetailObj : RuntimeHierarchyObj
		{
			protected IScope m_outerScope;

			protected DataRegion m_dataRegionDef;

			protected DataRowList m_dataRows;

			protected DataAggregateObjResultsList m_rvValueList;

			protected DataAggregateObjList m_runningValuesInGroup;

			protected AggregatesImpl m_globalRunningValueCollection;

			protected RuntimeGroupRootObjList m_groupCollection;

			protected DataActions m_outerDataAction;

			protected ReportItemInstance m_reportItemInstance;

			protected IList m_instanceList;

			protected RenderingPagesRangesList m_pagesList;

			protected int m_numberOfChildrenOnThisPage;

			internal DataRegion DataRegionDef
			{
				get
				{
					return this.m_dataRegionDef;
				}
			}

			internal virtual ExpressionInfoList SortExpressions
			{
				get
				{
					return null;
				}
			}

			internal virtual SortingExprHost SortExpressionHost
			{
				get
				{
					return null;
				}
			}

			internal virtual BoolList SortDirections
			{
				get
				{
					return null;
				}
			}

			protected override IScope OuterScope
			{
				get
				{
					return this.m_outerScope;
				}
			}

			protected override bool IsDetail
			{
				get
				{
					return true;
				}
			}

			protected RuntimeDetailObj(IScope outerScope, DataRegion dataRegionDef, DataActions dataAction, ProcessingContext processingContext)
				: base(processingContext)
			{
				base.m_hierarchyRoot = this;
				this.m_outerScope = outerScope;
				this.m_dataRegionDef = dataRegionDef;
				this.m_outerDataAction = dataAction;
			}

			internal RuntimeDetailObj(RuntimeDetailObj detailRoot)
				: base(detailRoot.ProcessingContext)
			{
				base.m_hierarchyRoot = detailRoot;
				this.m_outerScope = detailRoot.m_outerScope;
				this.m_dataRegionDef = detailRoot.m_dataRegionDef;
			}

			protected void HandleSortFilterEvent(ref RuntimeUserSortTargetInfo userSortTargetInfo)
			{
				RuntimeSortFilterEventInfoList runtimeSortFilterInfo = base.m_processingContext.RuntimeSortFilterInfo;
				if (runtimeSortFilterInfo != null && !this.m_outerScope.TargetForNonDetailSort)
				{
					for (int i = 0; i < runtimeSortFilterInfo.Count; i++)
					{
						RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = runtimeSortFilterInfo[i];
						if ((runtimeSortFilterEventInfo.EventSource.ContainingScopes == null || runtimeSortFilterEventInfo.EventSource.ContainingScopes.Count == 0 || runtimeSortFilterEventInfo.EventSourceScope != null || runtimeSortFilterEventInfo.EventSource.IsSubReportTopLevelScope) && this.IsTargetForSort(i, true) && runtimeSortFilterEventInfo.EventTarget != this && this.m_outerScope.TargetScopeMatched(i, true))
						{
							if (userSortTargetInfo == null)
							{
								userSortTargetInfo = new RuntimeUserSortTargetInfo(this, i, runtimeSortFilterEventInfo);
							}
							else
							{
								userSortTargetInfo.AddSortInfo(this, i, runtimeSortFilterInfo[i]);
							}
						}
					}
				}
			}

			private void HandleSortFilterEvent(int rowIndex)
			{
				base.DetailHandleSortFilterEvent(this.m_dataRegionDef, this.m_outerScope, rowIndex);
			}

			protected void ProcessDetailSort(RuntimeUserSortTargetInfo userSortTargetInfo)
			{
				if (userSortTargetInfo != null && !userSortTargetInfo.TargetForNonDetailSort)
				{
					RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = base.m_processingContext.RuntimeSortFilterInfo[userSortTargetInfo.SortFilterInfoIndices[0]];
					object sortOrder = runtimeSortFilterEventInfo.GetSortOrder(base.m_processingContext.ReportRuntime);
					userSortTargetInfo.SortTree.NextRow(sortOrder);
				}
			}

			internal override void NextRow()
			{
				if (!base.m_processingContext.ReportObjectModel.FieldsImpl.IsAggregateRow)
				{
					if (base.m_grouping != null)
					{
						object keyValue = base.m_processingContext.ReportRuntime.EvaluateRuntimeExpression(base.m_expression, this.m_dataRegionDef.ObjectType, this.m_dataRegionDef.Name, "Sort");
						base.m_grouping.NextRow(keyValue);
					}
					else
					{
						if (this.m_dataRows == null)
						{
							this.m_dataRows = new DataRowList();
						}
						this.HandleSortFilterEvent(this.m_dataRows.Count);
						RuntimeDetailObj.SaveData(this.m_dataRows, base.m_processingContext);
					}
				}
			}

			internal override bool SortAndFilter()
			{
				if ((ProcessingContext.SecondPassOperations.Filtering & base.m_processingContext.SecondPassOperation) != 0 && this.m_dataRows != null && (this.m_outerDataAction & DataActions.RecursiveAggregates) != 0)
				{
					this.ReadRows(DataActions.RecursiveAggregates);
				}
				return true;
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				if (base.m_grouping != null)
				{
					base.m_grouping.Traverse(ProcessingStages.RunningValues, base.m_expression.Direction);
					base.m_grouping = null;
				}
				else
				{
					this.ReadRows(DataActions.PostSortAggregates);
				}
			}

			internal void ReadRows(DataActions dataAction)
			{
				if (this.m_dataRows != null)
				{
					RuntimeDetailObj runtimeDetailObj = (RuntimeDetailObj)base.m_hierarchyRoot;
					bool flag = false;
					if (this != base.m_hierarchyRoot)
					{
						flag = true;
						if (runtimeDetailObj.m_dataRows == null)
						{
							runtimeDetailObj.m_dataRows = new DataRowList();
						}
						this.UpdateSortFilterInfo(runtimeDetailObj, runtimeDetailObj.m_dataRows.Count);
					}
					for (int i = 0; i < this.m_dataRows.Count; i++)
					{
						FieldImpl[] array = this.m_dataRows[i];
						base.m_processingContext.ReportObjectModel.FieldsImpl.SetFields(array);
						if (flag)
						{
							runtimeDetailObj.m_dataRows.Add(array);
						}
						runtimeDetailObj.ReadRow(dataAction);
					}
				}
			}

			private void UpdateSortFilterInfo(RuntimeDetailObj detailRoot, int rootRowCount)
			{
				RuntimeSortFilterEventInfoList runtimeSortFilterInfo = base.m_processingContext.RuntimeSortFilterInfo;
				if (runtimeSortFilterInfo != null && this.m_dataRegionDef.SortFilterSourceDetailScopeInfo != null)
				{
					for (int i = 0; i < runtimeSortFilterInfo.Count; i++)
					{
						RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = runtimeSortFilterInfo[i];
						if (this == runtimeSortFilterEventInfo.EventSourceScope)
						{
							runtimeSortFilterEventInfo.EventSourceScope = detailRoot;
							runtimeSortFilterEventInfo.EventSourceDetailIndex += rootRowCount;
						}
						if (runtimeSortFilterEventInfo.DetailScopes != null)
						{
							for (int j = 0; j < runtimeSortFilterEventInfo.DetailScopes.Count; j++)
							{
								if (this == runtimeSortFilterEventInfo.DetailScopes[j])
								{
									runtimeSortFilterEventInfo.DetailScopes[j] = detailRoot;
									IntList detailScopeIndices;
									int index;
									(detailScopeIndices = runtimeSortFilterEventInfo.DetailScopeIndices)[index = j] = detailScopeIndices[index] + rootRowCount;
								}
							}
						}
					}
				}
			}

			internal override void ReadRow(DataActions dataAction)
			{
				if (DataActions.PostSortAggregates == dataAction && this.m_runningValuesInGroup != null && 0 < this.m_runningValuesInGroup.Count)
				{
					DataAggregateObjResult[] array = new DataAggregateObjResult[this.m_runningValuesInGroup.Count];
					for (int i = 0; i < this.m_runningValuesInGroup.Count; i++)
					{
						this.m_runningValuesInGroup[i].Update();
						array[i] = this.m_runningValuesInGroup[i].AggregateResult();
					}
					this.m_rvValueList.Add(array);
				}
				if (this.m_outerScope != null && (this.m_outerDataAction & dataAction) != 0)
				{
					this.m_outerScope.ReadRow(dataAction);
				}
			}

			internal void CreateInstances(ReportItemInstance riInstance, IList instanceList, RenderingPagesRangesList pagesList, out int numberOfChildrenOnThisPage)
			{
				this.m_numberOfChildrenOnThisPage = 0;
				this.CreateInstances(riInstance, instanceList, pagesList);
				numberOfChildrenOnThisPage = this.m_numberOfChildrenOnThisPage;
			}

			internal override void CreateInstances(ReportItemInstance riInstance, IList instanceList, RenderingPagesRangesList pagesList)
			{
				this.m_reportItemInstance = riInstance;
				this.m_instanceList = instanceList;
				this.m_pagesList = pagesList;
				if (base.m_grouping != null)
				{
					base.m_grouping.Traverse(ProcessingStages.CreatingInstances, base.m_expression.Direction);
				}
				else
				{
					this.CreateInstance();
				}
			}

			internal override void CreateInstance()
			{
				Global.Tracer.Assert(false);
			}

			internal static void SaveData(DataRowList dataRows, ProcessingContext processingContext)
			{
				Global.Tracer.Assert(null != dataRows, "(null != dataRows)");
				FieldImpl[] andSaveFields = processingContext.ReportObjectModel.FieldsImpl.GetAndSaveFields();
				dataRows.Add(andSaveFields);
			}

			protected void AddRunningValues(RunningValueInfoList runningValues, RuntimeGroupRootObj lastGroup)
			{
				RuntimeDetailObj.AddRunningValues(base.m_processingContext, runningValues, ref this.m_runningValuesInGroup, this.m_globalRunningValueCollection, this.m_groupCollection, lastGroup);
			}

			internal static void AddRunningValues(ProcessingContext processingContext, RunningValueInfoList runningValues, ref DataAggregateObjList runningValuesInGroup, AggregatesImpl globalRunningValueCollection, RuntimeGroupRootObjList groupCollection, RuntimeGroupRootObj lastGroup)
			{
				if (runningValues != null && 0 < runningValues.Count)
				{
					if (runningValuesInGroup == null)
					{
						runningValuesInGroup = new DataAggregateObjList();
					}
					for (int i = 0; i < runningValues.Count; i++)
					{
						RunningValueInfo runningValueInfo = runningValues[i];
						DataAggregateObj dataAggregateObj = globalRunningValueCollection.GetAggregateObj(runningValueInfo.Name);
						if (dataAggregateObj == null)
						{
							dataAggregateObj = new DataAggregateObj(runningValueInfo, processingContext);
							globalRunningValueCollection.Add(dataAggregateObj);
						}
						if (runningValueInfo.Scope != null)
						{
							RuntimeGroupRootObj runtimeGroupRootObj = groupCollection[runningValueInfo.Scope];
							if (runtimeGroupRootObj != null)
							{
								runtimeGroupRootObj.AddScopedRunningValue(dataAggregateObj, false);
							}
							else if (processingContext.PivotEscalateScope() && lastGroup != null)
							{
								lastGroup.AddScopedRunningValue(dataAggregateObj, true);
							}
						}
						runningValuesInGroup.Add(dataAggregateObj);
					}
				}
			}

			protected void SetupEnvironment(int dataRowIndex, RunningValueInfoList runningValueDefs)
			{
				base.SetupFields(this.m_dataRows[dataRowIndex]);
				if (runningValueDefs != null && 0 < runningValueDefs.Count)
				{
					base.SetupRunningValues(runningValueDefs, this.m_rvValueList[dataRowIndex]);
				}
				base.m_processingContext.ReportRuntime.CurrentScope = this;
			}

			protected void SetupEnvironment(int dataRowIndex, ref int startIndex, RunningValueInfoList runningValueDefs, bool rvOnly)
			{
				if (!rvOnly)
				{
					base.SetupFields(this.m_dataRows[dataRowIndex]);
					base.m_processingContext.ReportRuntime.CurrentScope = this;
				}
				if (runningValueDefs != null && 0 < runningValueDefs.Count)
				{
					base.SetupRunningValues(ref startIndex, runningValueDefs, this.m_rvValueList[dataRowIndex]);
				}
			}

			internal override bool InScope(string scope)
			{
				Global.Tracer.Assert(null != this.m_outerScope, "(null != m_outerScope)");
				return this.m_outerScope.InScope(scope);
			}

			protected override int GetRecursiveLevel(string scope)
			{
				Global.Tracer.Assert(null != this.m_outerScope, "(null != m_outerScope)");
				return this.m_outerScope.RecursiveLevel(scope);
			}

			internal override void GetScopeValues(IHierarchyObj targetScopeObj, VariantList[] scopeValues, ref int index)
			{
				base.DetailGetScopeValues(this.m_outerScope, targetScopeObj, scopeValues, ref index);
			}

			internal override bool TargetScopeMatched(int index, bool detailSort)
			{
				return base.DetailTargetScopeMatched(this.m_dataRegionDef, this.m_outerScope, index);
			}

			protected override void GetGroupNameValuePairs(Dictionary<string, object> pairs)
			{
				Global.Tracer.Assert(null != this.m_outerScope, "(null != m_outerScope)");
				this.m_outerScope.GetGroupNameValuePairs(pairs);
			}
		}

		internal abstract class RuntimeOnePassDetailObj : RuntimeDataRegionObj
		{
			protected IScope m_outerScope;

			protected DataRegion m_dataRegionDef;

			protected DataAggregateObjList m_runningValues;

			protected Pagination m_pagination;

			protected int m_numberOfContentsOnThisPage;

			protected RenderingPagesRangesList m_renderingPages;

			protected NavigationInfo m_navigationInfo;

			protected PageTextboxes m_onePassTextboxes;

			protected override IScope OuterScope
			{
				get
				{
					return this.m_outerScope;
				}
			}

			internal Pagination Pagination
			{
				get
				{
					return this.m_pagination;
				}
			}

			internal int NumberOfContentsOnThisPage
			{
				get
				{
					return this.m_numberOfContentsOnThisPage;
				}
				set
				{
					this.m_numberOfContentsOnThisPage = value;
				}
			}

			internal RenderingPagesRangesList ChildrenStartAndEndPages
			{
				get
				{
					return this.m_renderingPages;
				}
			}

			internal NavigationInfo NavigationInfo
			{
				get
				{
					return this.m_navigationInfo;
				}
			}

			internal PageTextboxes OnePassTextboxes
			{
				get
				{
					return this.m_onePassTextboxes;
				}
			}

			internal DataRegion DataRegionDef
			{
				get
				{
					return this.m_dataRegionDef;
				}
			}

			protected RuntimeOnePassDetailObj(IScope outerScope, DataRegion dataRegionDef, ProcessingContext processingContext)
				: base(processingContext)
			{
				this.m_outerScope = outerScope;
				this.m_dataRegionDef = dataRegionDef;
				this.m_pagination = new Pagination(base.m_processingContext.Pagination.PageHeight);
				this.m_renderingPages = new RenderingPagesRangesList();
				this.m_navigationInfo = new NavigationInfo();
				this.m_onePassTextboxes = new PageTextboxes();
			}

			internal abstract int GetDetailPage();

			internal override void NextRow()
			{
				FieldsImpl fieldsImpl = base.m_processingContext.ReportObjectModel.FieldsImpl;
				if (!fieldsImpl.IsAggregateRow)
				{
					if (this.m_runningValues != null)
					{
						for (int i = 0; i < this.m_runningValues.Count; i++)
						{
							this.m_runningValues[i].Update();
						}
					}
					base.m_processingContext.ReportRuntime.CurrentScope = this;
					if (fieldsImpl.AddRowIndex)
					{
						base.DetailHandleSortFilterEvent(this.m_dataRegionDef, this.m_outerScope, fieldsImpl.GetRowIndex());
					}
					this.CreateInstance();
				}
			}

			internal override bool SortAndFilter()
			{
				return true;
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
			}

			protected abstract void CreateInstance();

			internal override void CreateInstances(ReportItemInstance riInstance, IList instanceList, RenderingPagesRangesList pagesList)
			{
			}

			protected void AddRunningValues(RunningValueInfoList runningValues)
			{
				if (runningValues != null && 0 < runningValues.Count)
				{
					if (this.m_runningValues == null)
					{
						this.m_runningValues = new DataAggregateObjList();
					}
					for (int i = 0; i < runningValues.Count; i++)
					{
						DataAggregateObj dataAggregateObj = new DataAggregateObj(runningValues[i], base.m_processingContext);
						this.m_runningValues.Add(dataAggregateObj);
						base.m_processingContext.ReportObjectModel.AggregatesImpl.Add(dataAggregateObj);
					}
				}
			}

			internal override void SetupEnvironment()
			{
			}

			internal override void ReadRow(DataActions dataAction)
			{
			}

			internal override bool InScope(string scope)
			{
				Global.Tracer.Assert(null != this.m_outerScope, "(null != m_outerScope)");
				return this.m_outerScope.InScope(scope);
			}

			internal virtual bool IsVisible(string textboxName)
			{
				return true;
			}

			internal void MoveAllToFirstPage()
			{
				int pageCount = this.m_onePassTextboxes.GetPageCount();
				for (int i = 1; i < pageCount; i++)
				{
					Hashtable textboxesOnPage = this.m_onePassTextboxes.GetTextboxesOnPage(i);
					if (textboxesOnPage != null)
					{
						IDictionaryEnumerator enumerator = textboxesOnPage.GetEnumerator();
						while (enumerator.MoveNext())
						{
							string text = enumerator.Key as string;
							ArrayList arrayList = enumerator.Value as ArrayList;
							Global.Tracer.Assert(text != null && null != arrayList, "(null != textboxName && null != values)");
							this.m_onePassTextboxes.AddTextboxValue(0, text, arrayList);
						}
					}
				}
			}

			internal virtual void ProcessOnePassDetailTextboxes(int sourcePage, int targetPage)
			{
				Hashtable textboxesOnPage = this.m_onePassTextboxes.GetTextboxesOnPage(sourcePage);
				if (textboxesOnPage != null)
				{
					IDictionaryEnumerator enumerator = textboxesOnPage.GetEnumerator();
					while (enumerator.MoveNext())
					{
						string text = enumerator.Key as string;
						ArrayList arrayList = enumerator.Value as ArrayList;
						Global.Tracer.Assert(text != null && null != arrayList, "(null != textboxName && null != values)");
						if (this.IsVisible(text))
						{
							base.m_processingContext.PageSectionContext.PageTextboxes.AddTextboxValue(targetPage, text, arrayList);
						}
					}
				}
			}

			internal override void GetScopeValues(IHierarchyObj targetScopeObj, VariantList[] scopeValues, ref int index)
			{
				base.DetailGetScopeValues(this.m_outerScope, targetScopeObj, scopeValues, ref index);
			}

			internal override bool TargetScopeMatched(int index, bool detailSort)
			{
				return base.DetailTargetScopeMatched(this.m_dataRegionDef, this.m_outerScope, index);
			}
		}

		private sealed class RuntimeListGroupRootObj : RuntimeGroupRootObj, IFilterOwner
		{
			private List m_listDef;

			private Filters m_filters;

			private DataAggregateObjList m_nonCustomAggregates;

			private DataAggregateObjList m_customAggregates;

			private FieldImpl[] m_firstRow;

			private bool m_firstRowIsAggregate;

			private DataAggregateObjList m_postSortAggregates;

			private DataRowList m_dataRows;

			private RuntimeUserSortTargetInfo m_userSortTargetInfo;

			internal ReportItemCollection ReportItemsDef
			{
				get
				{
					return this.m_listDef.ReportItems;
				}
			}

			protected override string ScopeName
			{
				get
				{
					return this.m_listDef.Name;
				}
			}

			protected override BTreeNode SortTree
			{
				get
				{
					if (ProcessingStages.UserSortFilter == base.m_processingStage)
					{
						if (this.m_userSortTargetInfo != null)
						{
							return this.m_userSortTargetInfo.SortTree;
						}
						return null;
					}
					return base.m_grouping.Tree;
				}
				set
				{
					if (ProcessingStages.UserSortFilter == base.m_processingStage)
					{
						if (this.m_userSortTargetInfo != null)
						{
							this.m_userSortTargetInfo.SortTree = value;
						}
						else
						{
							Global.Tracer.Assert(false);
						}
					}
					else
					{
						base.m_grouping.Tree = value;
					}
				}
			}

			protected override int ExpressionIndex
			{
				get
				{
					return 0;
				}
			}

			protected override IntList SortFilterInfoIndices
			{
				get
				{
					if (this.m_userSortTargetInfo != null)
					{
						return this.m_userSortTargetInfo.SortFilterInfoIndices;
					}
					return null;
				}
			}

			internal override bool TargetForNonDetailSort
			{
				get
				{
					if (this.m_userSortTargetInfo != null && this.m_userSortTargetInfo.TargetForNonDetailSort)
					{
						return true;
					}
					return base.m_outerScope.TargetForNonDetailSort;
				}
			}

			internal RuntimeListGroupRootObj(IScope outerScope, List listDef, ref DataActions dataAction, ProcessingContext processingContext)
				: base(outerScope, listDef.HierarchyDef, (listDef.PostSortAggregates == null && listDef.Filters == null) ? dataAction : DataActions.None, processingContext)
			{
				this.m_listDef = listDef;
				ReportItemCollection reportItems = listDef.ReportItems;
				RuntimeDataRegionObj.CreateAggregates(base.m_processingContext, listDef.Aggregates, ref this.m_nonCustomAggregates, ref this.m_customAggregates);
				if (listDef.Filters != null)
				{
					this.m_filters = new Filters(Filters.FilterTypes.DataRegionFilter, this, listDef.Filters, listDef.ObjectType, listDef.Name, base.m_processingContext);
				}
				bool flag = false;
				if (listDef.PostSortAggregates != null)
				{
					RuntimeDataRegionObj.CreateAggregates(base.m_processingContext, listDef.PostSortAggregates, ref this.m_postSortAggregates);
					flag = true;
				}
				if (reportItems != null && reportItems.RunningValues != null && 0 < reportItems.RunningValues.Count)
				{
					base.m_dataAction |= DataActions.PostSortAggregates;
				}
				if (listDef.Filters == null && (base.m_hierarchyDef.Grouping.Filters == null || listDef.PostSortAggregates != null))
				{
					dataAction = DataActions.None;
				}
				if (base.m_processingContext.IsSortFilterTarget(listDef.IsSortFilterTarget, base.m_outerScope, (IHierarchyObj)this, ref this.m_userSortTargetInfo) && this.m_userSortTargetInfo.TargetForNonDetailSort)
				{
					flag = true;
				}
				base.m_processingContext.RegisterSortFilterExpressionScope(base.m_outerScope, this, listDef.IsSortFilterExpressionScope);
				if (flag)
				{
					this.m_dataRows = new DataRowList();
				}
			}

			internal override bool IsTargetForSort(int index, bool detailSort)
			{
				if (this.m_userSortTargetInfo != null && this.m_userSortTargetInfo.IsTargetForSort(index, detailSort))
				{
					return true;
				}
				return base.m_outerScope.IsTargetForSort(index, detailSort);
			}

			internal override void NextRow()
			{
				if (base.m_processingContext.ReportObjectModel.FieldsImpl.AggregationFieldCount == 0)
				{
					RuntimeDataRegionObj.UpdateAggregates(base.m_processingContext, this.m_customAggregates, false);
				}
				if (base.m_processingContext.ReportObjectModel.FieldsImpl.IsAggregateRow)
				{
					base.ScopeNextAggregateRow(this.m_userSortTargetInfo);
				}
				else
				{
					this.NextNonAggregateRow();
				}
			}

			private void NextNonAggregateRow()
			{
				bool flag = true;
				if (this.m_filters != null)
				{
					flag = this.m_filters.PassFilters(base.m_processingContext.ReportObjectModel.FieldsImpl.GetFields());
				}
				if (flag)
				{
					((IFilterOwner)this).PostFilterNextRow();
				}
			}

			void IFilterOwner.PostFilterNextRow()
			{
				ProcessingStages processingStage = base.m_processingStage;
				base.m_processingStage = ProcessingStages.UserSortFilter;
				RuntimeDataRegionObj.CommonFirstRow(base.m_processingContext.ReportObjectModel.FieldsImpl, ref this.m_firstRowIsAggregate, ref this.m_firstRow);
				base.ScopeNextNonAggregateRow(this.m_nonCustomAggregates, this.m_dataRows);
				base.m_processingStage = processingStage;
			}

			protected override void SendToInner()
			{
				base.m_processingStage = ProcessingStages.Grouping;
				base.NextRow();
			}

			internal override bool SortAndFilter()
			{
				this.SetupEnvironment();
				if (this.m_userSortTargetInfo != null)
				{
					this.m_userSortTargetInfo.EnterProcessUserSortPhase(base.m_processingContext);
				}
				if ((ProcessingContext.SecondPassOperations.Filtering & base.m_processingContext.SecondPassOperation) != 0 && this.m_dataRows != null && (base.m_outerDataAction & DataActions.RecursiveAggregates) != 0)
				{
					this.ReadRows(DataActions.RecursiveAggregates);
				}
				bool result = base.SortAndFilter();
				if (this.m_userSortTargetInfo != null)
				{
					this.m_userSortTargetInfo.LeaveProcessUserSortPhase(base.m_processingContext);
				}
				return result;
			}

			private void ReadRows(DataActions dataAction)
			{
				for (int i = 0; i < this.m_dataRows.Count; i++)
				{
					FieldImpl[] fields = this.m_dataRows[i];
					base.m_processingContext.ReportObjectModel.FieldsImpl.SetFields(fields);
					if (DataActions.PostSortAggregates == dataAction)
					{
						RuntimeDataRegionObj.UpdateAggregates(base.m_processingContext, this.m_postSortAggregates, false);
					}
					if (base.m_outerScope != null && (dataAction & base.m_outerDataAction) != 0)
					{
						base.m_outerScope.ReadRow(dataAction);
					}
				}
			}

			internal override void ReadRow(DataActions dataAction)
			{
				if (DataActions.UserSort == dataAction)
				{
					RuntimeDataRegionObj.CommonFirstRow(base.m_processingContext.ReportObjectModel.FieldsImpl, ref this.m_firstRowIsAggregate, ref this.m_firstRow);
					base.CommonNextRow(this.m_dataRows);
				}
				else if (this.m_postSortAggregates == null)
				{
					base.ReadRow(dataAction);
				}
				else if (DataActions.PostSortAggregates == dataAction && base.m_runningValuesInGroup != null)
				{
					for (int i = 0; i < base.m_runningValuesInGroup.Count; i++)
					{
						base.m_runningValuesInGroup[i].Update();
					}
				}
			}

			internal override void SetupEnvironment()
			{
				base.SetupEnvironment(this.m_nonCustomAggregates, this.m_customAggregates, this.m_firstRow);
				base.SetupAggregates(this.m_postSortAggregates);
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				base.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				if (this.m_listDef.ReportItems != null)
				{
					base.AddRunningValues(this.m_listDef.ReportItems.RunningValues);
				}
				if (this.m_dataRows != null)
				{
					this.ReadRows(DataActions.PostSortAggregates);
					this.m_dataRows = null;
				}
				base.m_grouping.Traverse(ProcessingStages.RunningValues, base.m_expression.Direction);
			}

			internal override bool InScope(string scope)
			{
				return base.DataRegionInScope(this.m_listDef, scope);
			}

			protected override int GetRecursiveLevel(string scope)
			{
				return base.DataRegionRecursiveLevel(this.m_listDef, scope);
			}

			internal override bool TargetScopeMatched(int index, bool detailSort)
			{
				return base.m_outerScope.TargetScopeMatched(index, detailSort);
			}

			protected override IHierarchyObj CreateHierarchyObj()
			{
				if (ProcessingStages.UserSortFilter == base.m_processingStage)
				{
					return new RuntimeSortHierarchyObj(this);
				}
				return base.CreateHierarchyObj();
			}

			protected override void ProcessUserSort()
			{
				base.m_processingStage = ProcessingStages.UserSortFilter;
				base.m_processingContext.ProcessUserSortForTarget((IHierarchyObj)this, ref this.m_dataRows, this.m_userSortTargetInfo.TargetForNonDetailSort);
				if (this.m_userSortTargetInfo.TargetForNonDetailSort)
				{
					this.m_userSortTargetInfo.ResetTargetForNonDetailSort();
					this.m_userSortTargetInfo.EnterProcessUserSortPhase(base.m_processingContext);
					base.m_grouping = new RuntimeGroupingObj(this, base.m_groupingType);
					base.m_firstChild = (base.m_lastChild = null);
					if (this.m_listDef.PostSortAggregates != null)
					{
						this.m_dataRows = new DataRowList();
					}
					base.ScopeFinishSorting(ref this.m_firstRow, this.m_userSortTargetInfo);
					this.m_userSortTargetInfo.LeaveProcessUserSortPhase(base.m_processingContext);
				}
			}

			protected override void MarkSortInfoProcessed(RuntimeSortFilterEventInfoList runtimeSortFilterInfo)
			{
				if (this.m_userSortTargetInfo != null)
				{
					this.m_userSortTargetInfo.MarkSortInfoProcessed(runtimeSortFilterInfo, this);
				}
			}

			protected override void AddSortInfoIndex(int sortInfoIndex, RuntimeSortFilterEventInfo sortInfo)
			{
				if (this.m_userSortTargetInfo != null)
				{
					this.m_userSortTargetInfo.AddSortInfoIndex(sortInfoIndex, sortInfo);
				}
			}

			protected override void GetGroupNameValuePairs(Dictionary<string, object> pairs)
			{
				base.DataRegionGetGroupNameValuePairs(this.m_listDef, pairs);
			}
		}

        private sealed class RuntimeListGroupLeafObj : ReportProcessing.RuntimeGroupLeafObj
        {
            internal RuntimeListGroupLeafObj(ReportProcessing.RuntimeListGroupRootObj groupRoot) : base(groupRoot)
            {
                this.m_dataAction = groupRoot.DataAction;
                bool flag = false;
                bool flag2 = base.HandleSortFilterEvent();
                ReportProcessing.DataActions dataAction;
                this.ConstructRuntimeStructure(ref flag, out dataAction);
                if (!flag)
                {
                    this.m_dataAction = dataAction;
                }
                if (flag2)
                {
                    this.m_dataAction |= ReportProcessing.DataActions.UserSort;
                }
                if (this.m_dataAction != ReportProcessing.DataActions.None)
                {
                    this.m_dataRows = new ReportProcessing.DataRowList();
                }
            }

            internal int StartPage
            {
                get
                {
                    return this.m_startPage;
                }
            }

            protected override void ConstructRuntimeStructure(ref bool handleMyDataAction, out ReportProcessing.DataActions innerDataAction)
            {
                base.ConstructRuntimeStructure(ref handleMyDataAction, out innerDataAction);
                this.m_reportItemCol = new ReportProcessing.RuntimeRICollection(this, ((ReportProcessing.RuntimeListGroupRootObj)this.m_hierarchyRoot).ReportItemsDef, ref innerDataAction, this.m_processingContext, true);
            }

            protected override void SendToInner()
            {
                base.SendToInner();
                this.m_reportItemCol.FirstPassNextDataRow();
            }

            internal override bool SortAndFilter()
            {
                this.SetupEnvironment();
                if (this.m_userSortTargetInfo != null)
                {
                    this.m_userSortTargetInfo.EnterProcessUserSortPhase(this.m_processingContext);
                }
                this.m_reportItemCol.SortAndFilter();
                bool result = base.SortAndFilter();
                if (this.m_userSortTargetInfo != null)
                {
                    this.m_userSortTargetInfo.LeaveProcessUserSortPhase(this.m_processingContext);
                }
                return result;
            }

            internal override void ReadRow(ReportProcessing.DataActions dataAction)
            {
                if (ReportProcessing.DataActions.UserSort == dataAction)
                {
                    ReportProcessing.RuntimeDataRegionObj.CommonFirstRow(this.m_processingContext.ReportObjectModel.FieldsImpl, ref this.m_firstRowIsAggregate, ref this.m_firstRow);
                    base.CommonNextRow(this.m_dataRows);
                    return;
                }
                base.ReadRow(dataAction);
                if (ReportProcessing.DataActions.PostSortAggregates == dataAction)
                {
                    this.CalculatePreviousAggregates();
                }
            }

            private void CalculatePreviousAggregates()
            {
                if (!this.m_processedPreviousAggregates && this.m_processingContext.GlobalRVCollection != null)
                {
                    if (this.m_reportItemCol != null)
                    {
                        this.m_reportItemCol.CalculatePreviousAggregates(this.m_processingContext.GlobalRVCollection);
                    }
                    this.m_processedPreviousAggregates = true;
                }
            }

            internal override void CalculateRunningValues()
            {
                ReportProcessing.RuntimeGroupRootObj runtimeGroupRootObj = (ReportProcessing.RuntimeGroupRootObj)this.m_hierarchyRoot;
                AggregatesImpl globalRunningValueCollection = runtimeGroupRootObj.GlobalRunningValueCollection;
                ReportProcessing.RuntimeGroupRootObjList groupCollection = runtimeGroupRootObj.GroupCollection;
                if (this.m_dataRows != null)
                {
                    base.ReadRows(ReportProcessing.DataActions.PostSortAggregates);
                    this.m_dataRows = null;
                }
                if (this.m_reportItemCol != null)
                {
                    this.m_reportItemCol.CalculateRunningValues(globalRunningValueCollection, groupCollection, runtimeGroupRootObj);
                }
                base.CalculateRunningValues();
            }

            internal override void CreateInstance()
            {
                this.SetupEnvironment();
                ReportProcessing.RuntimeListGroupRootObj runtimeListGroupRootObj = (ReportProcessing.RuntimeListGroupRootObj)this.m_hierarchyRoot;
                ReportItemInstance reportItemInstance = runtimeListGroupRootObj.ReportItemInstance;
                IList instanceList = runtimeListGroupRootObj.InstanceList;
                ReportHierarchyNode hierarchyDef = runtimeListGroupRootObj.HierarchyDef;
                List list = (List)hierarchyDef.DataRegionDef;
                Global.Tracer.Assert(null != this.m_reportItemCol, "(null != m_reportItemCol)");
                this.m_processingContext.ChunkManager.CheckPageBreak(hierarchyDef, true);
                this.m_processingContext.Pagination.EnterIgnorePageBreak(list.Visibility, false);
                if (instanceList.Count != 0)
                {
                    bool flag = this.m_processingContext.Pagination.CalculateSoftPageBreak(null, 0.0, 0.0, false, list.Grouping.PageBreakAtStart);
                    if (!this.m_processingContext.Pagination.IgnorePageBreak && (this.m_processingContext.Pagination.CanMoveToNextPage(list.Grouping.PageBreakAtStart) || flag))
                    {
                        list.ContentStartPage++;
                        this.m_processingContext.Pagination.SetCurrentPageHeight(list, 0.0);
                    }
                }
                this.m_listContentInstance = new ListContentInstance(this.m_processingContext, list);
                RenderingPagesRanges renderingPagesRanges = default(RenderingPagesRanges);
                renderingPagesRanges.StartPage = list.ContentStartPage;
                this.m_startPage = renderingPagesRanges.StartPage;
                if (AspNetCore.ReportingServices.ReportProcessing.Report.ShowHideTypes.Interactive == this.m_processingContext.ShowHideType)
                {
                    this.m_processingContext.EnterGrouping();
                    ((IShowHideContainer)this.m_listContentInstance).BeginProcessContainer(this.m_processingContext);
                }
                if (list.Grouping.GroupLabel != null)
                {
                    this.m_label = this.m_processingContext.NavigationInfo.CurrentLabel;
                    if (this.m_label != null)
                    {
                        this.m_processingContext.NavigationInfo.EnterDocumentMapChildren();
                    }
                }
                int startPage = list.StartPage;
                list.StartPage = list.ContentStartPage;
                this.m_processingContext.PageSectionContext.EnterVisibilityScope(list.Visibility, list.StartHidden);
                this.m_reportItemCol.CreateInstances(this.m_listContentInstance.ReportItemColInstance);
                this.m_processingContext.PageSectionContext.ExitVisibilityScope();
                if (AspNetCore.ReportingServices.ReportProcessing.Report.ShowHideTypes.Interactive == this.m_processingContext.ShowHideType)
                {
                    ((IShowHideContainer)this.m_listContentInstance).EndProcessContainer(this.m_processingContext);
                }
                this.m_processingContext.ChunkManager.CheckPageBreak(hierarchyDef, false);
                if (AspNetCore.ReportingServices.ReportProcessing.Report.ShowHideTypes.Interactive == this.m_processingContext.ShowHideType)
                {
                    this.m_processingContext.ExitGrouping();
                }
                this.m_processingContext.Pagination.ProcessEndGroupPage(list.IsListMostInner ? list.HeightValue : 0.0, list.Grouping.PageBreakAtEnd, list, true, list.StartHidden);
                list.ContentStartPage = list.EndPage;
                list.StartPage = startPage;
                if (this.m_processingContext.Pagination.ShouldItemMoveToChildStartPage(list))
                {
                    this.m_startPage = (renderingPagesRanges.StartPage = this.m_listContentInstance.ReportItemColInstance.ChildrenStartAndEndPages[0].StartPage);
                }
                renderingPagesRanges.EndPage = list.EndPage;
                ((ListInstance)reportItemInstance).ChildrenStartAndEndPages.Add(renderingPagesRanges);
                this.m_processingContext.Pagination.LeaveIgnorePageBreak(list.Visibility, false);
                instanceList.Add(this.m_listContentInstance);
                if (this.m_reportItemCol != null)
                {
                    this.m_reportItemCol.ResetReportItemObjs();
                }
                base.ResetReportItemsWithHideDuplicates();
            }

            protected override void AddToDocumentMap()
            {
                if (base.GroupingDef.GroupLabel != null && this.m_label != null)
                {
                    this.m_processingContext.NavigationInfo.AddToDocumentMap(this.m_listContentInstance.UniqueName, true, this.m_startPage, this.m_label);
                }
            }

            private ReportProcessing.RuntimeRICollection m_reportItemCol;

            private ListContentInstance m_listContentInstance;

            private string m_label;

            private int m_startPage = -1;
        }

        private sealed class RuntimeListDetailObj : RuntimeDetailObj, IFilterOwner, ISortDataHolder
		{
			private Filters m_filters;

			private DataAggregateObjList m_nonCustomAggregates;

			private DataAggregateObjList m_customAggregates;

			private DataAggregateObjList m_postSortAggregates;

			private RuntimeUserSortTargetInfo m_userSortTargetInfo;

			internal override ExpressionInfoList SortExpressions
			{
				get
				{
					Sorting sorting = ((List)base.m_dataRegionDef).Sorting;
					if (sorting != null && 0 < sorting.SortExpressions.Count)
					{
						return sorting.SortExpressions;
					}
					return null;
				}
			}

			internal override SortingExprHost SortExpressionHost
			{
				get
				{
					Sorting sorting = ((List)base.m_dataRegionDef).Sorting;
					if (sorting != null)
					{
						return sorting.ExprHost;
					}
					return null;
				}
			}

			internal override BoolList SortDirections
			{
				get
				{
					Sorting sorting = ((List)base.m_dataRegionDef).Sorting;
					if (sorting != null && 0 < sorting.SortDirections.Count)
					{
						return sorting.SortDirections;
					}
					return null;
				}
			}

			protected override string ScopeName
			{
				get
				{
					return base.m_dataRegionDef.Name;
				}
			}

			protected override BTreeNode SortTree
			{
				get
				{
					if (this.m_userSortTargetInfo != null)
					{
						return this.m_userSortTargetInfo.SortTree;
					}
					if (base.m_grouping != null)
					{
						return base.m_grouping.Tree;
					}
					return null;
				}
				set
				{
					if (this.m_userSortTargetInfo != null)
					{
						this.m_userSortTargetInfo.SortTree = value;
					}
					else if (base.m_grouping != null)
					{
						base.m_grouping.Tree = value;
					}
					else
					{
						Global.Tracer.Assert(false);
					}
				}
			}

			protected override int ExpressionIndex
			{
				get
				{
					return 0;
				}
			}

			protected override IntList SortFilterInfoIndices
			{
				get
				{
					if (this.m_userSortTargetInfo != null)
					{
						return this.m_userSortTargetInfo.SortFilterInfoIndices;
					}
					return null;
				}
			}

			internal override bool TargetForNonDetailSort
			{
				get
				{
					if (this.m_userSortTargetInfo != null && this.m_userSortTargetInfo.TargetForNonDetailSort)
					{
						return true;
					}
					return base.m_outerScope.TargetForNonDetailSort;
				}
			}

			internal RuntimeListDetailObj(IScope outerScope, List listDef, ref DataActions dataAction, ProcessingContext processingContext)
				: base(outerScope, listDef, (listDef.Filters == null) ? dataAction : DataActions.None, processingContext)
			{
				RuntimeDataRegionObj.CreateAggregates(base.m_processingContext, listDef.Aggregates, ref this.m_nonCustomAggregates, ref this.m_customAggregates);
				RuntimeDataRegionObj.CreateAggregates(base.m_processingContext, listDef.PostSortAggregates, ref this.m_postSortAggregates);
				if (listDef.Filters != null)
				{
					this.m_filters = new Filters(Filters.FilterTypes.DataRegionFilter, this, listDef.Filters, listDef.ObjectType, listDef.Name, base.m_processingContext);
				}
				else
				{
					dataAction = DataActions.None;
				}
				RunningValueInfoList runningValueInfoList = null;
				if (listDef.ReportItems != null)
				{
					runningValueInfoList = listDef.ReportItems.RunningValues;
				}
				if (runningValueInfoList != null && 0 < runningValueInfoList.Count)
				{
					base.m_rvValueList = new DataAggregateObjResultsList();
				}
				if (base.m_processingContext.IsSortFilterTarget(listDef.IsSortFilterTarget, base.m_outerScope, (IHierarchyObj)this, ref this.m_userSortTargetInfo) && this.m_userSortTargetInfo.TargetForNonDetailSort)
				{
					base.m_dataRows = new DataRowList();
				}
				base.HandleSortFilterEvent(ref this.m_userSortTargetInfo);
				if (this.m_userSortTargetInfo == null && listDef.Sorting != null && 0 < listDef.Sorting.SortExpressions.Count)
				{
					base.m_expression = new RuntimeExpressionInfo(listDef.Sorting.SortExpressions, listDef.Sorting.ExprHost, listDef.Sorting.SortDirections, 0);
					base.m_grouping = new RuntimeGroupingObj(this, RuntimeGroupingObj.GroupingTypes.Sort);
				}
				base.m_processingContext.RegisterSortFilterExpressionScope(base.m_outerScope, this, listDef.IsSortFilterExpressionScope);
			}

			internal RuntimeListDetailObj(RuntimeListDetailObj detailRoot)
				: base(detailRoot)
			{
			}

			internal override bool IsTargetForSort(int index, bool detailSort)
			{
				if (this.m_userSortTargetInfo != null && this.m_userSortTargetInfo.IsTargetForSort(index, detailSort))
				{
					return true;
				}
				return base.m_outerScope.IsTargetForSort(index, detailSort);
			}

			void ISortDataHolder.NextRow()
			{
				this.NextRow();
			}

			void ISortDataHolder.Traverse(ProcessingStages operation)
			{
				switch (operation)
				{
				case ProcessingStages.SortAndFilter:
					this.SortAndFilter();
					break;
				case ProcessingStages.RunningValues:
					base.ReadRows(DataActions.PostSortAggregates);
					break;
				case ProcessingStages.CreatingInstances:
					this.CreateInstance();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}

			internal override void NextRow()
			{
				if (base.m_processingContext.ReportObjectModel.FieldsImpl.IsAggregateRow)
				{
					this.NextAggregateRow();
				}
				else
				{
					this.NextNonAggregateRow();
				}
			}

			private void NextNonAggregateRow()
			{
				bool flag = true;
				if (this.m_filters != null)
				{
					flag = this.m_filters.PassFilters(base.m_processingContext.ReportObjectModel.FieldsImpl.GetFields());
				}
				if (flag)
				{
					((IFilterOwner)this).PostFilterNextRow();
				}
			}

			private void NextAggregateRow()
			{
				if (base.m_processingContext.ReportObjectModel.FieldsImpl.AggregationFieldCount == 0)
				{
					RuntimeDataRegionObj.UpdateAggregates(base.m_processingContext, this.m_customAggregates, false);
				}
			}

			void IFilterOwner.PostFilterNextRow()
			{
				RuntimeDataRegionObj.UpdateAggregates(base.m_processingContext, this.m_nonCustomAggregates, false);
				if (this.m_userSortTargetInfo != null)
				{
					base.ProcessDetailSort(this.m_userSortTargetInfo);
				}
				else
				{
					base.NextRow();
				}
			}

			protected override void SendToInner()
			{
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				if (base.m_rvValueList == null && this.m_postSortAggregates == null && (base.m_outerDataAction & DataActions.PostSortAggregates) == DataActions.None)
				{
					return;
				}
				base.m_globalRunningValueCollection = globalRVCol;
				base.m_groupCollection = groupCol;
				ReportItemCollection reportItemCollection = null;
				RunningValueInfoList runningValueInfoList = null;
				reportItemCollection = ((List)base.m_dataRegionDef).ReportItems;
				if (reportItemCollection != null)
				{
					runningValueInfoList = reportItemCollection.RunningValues;
				}
				if (runningValueInfoList != null && 0 < runningValueInfoList.Count)
				{
					base.AddRunningValues(runningValueInfoList, lastGroup);
				}
				if (this.m_userSortTargetInfo != null)
				{
					bool sortDirection = base.m_processingContext.RuntimeSortFilterInfo[this.m_userSortTargetInfo.SortFilterInfoIndices[0]].SortDirection;
					this.m_userSortTargetInfo.SortTree.Traverse(ProcessingStages.RunningValues, sortDirection);
					this.m_userSortTargetInfo = null;
				}
				else
				{
					base.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				}
			}

			internal override void ReadRow(DataActions dataAction)
			{
				if (DataActions.PostSortAggregates == dataAction && this.m_postSortAggregates != null)
				{
					RuntimeDataRegionObj.UpdateAggregates(base.m_processingContext, this.m_postSortAggregates, false);
				}
				base.ReadRow(dataAction);
			}

			internal override void CreateInstances(ReportItemInstance riInstance, IList instanceList, RenderingPagesRangesList pagesList)
			{
				if (this.m_userSortTargetInfo != null)
				{
					base.m_reportItemInstance = riInstance;
					base.m_instanceList = instanceList;
					base.m_pagesList = pagesList;
					bool sortDirection = base.m_processingContext.RuntimeSortFilterInfo[this.m_userSortTargetInfo.SortFilterInfoIndices[0]].SortDirection;
					this.m_userSortTargetInfo.SortTree.Traverse(ProcessingStages.CreatingInstances, sortDirection);
					this.m_userSortTargetInfo = null;
				}
				else
				{
					base.CreateInstances(riInstance, instanceList, pagesList);
				}
			}

			internal override void CreateInstance()
			{
				RuntimeListDetailObj runtimeListDetailObj = (RuntimeListDetailObj)base.m_hierarchyRoot;
				ListInstance listInstance = (ListInstance)runtimeListDetailObj.m_reportItemInstance;
				IList instanceList = runtimeListDetailObj.m_instanceList;
				ReportItemCollection reportItems = ((List)base.m_dataRegionDef).ReportItems;
				List list = (List)base.m_dataRegionDef;
				Pagination pagination = base.m_processingContext.Pagination;
				if (reportItems != null && base.m_dataRows != null)
				{
					DataActions dataActions = DataActions.None;
					RuntimeRICollection runtimeRICollection = new RuntimeRICollection(this, reportItems, ref dataActions, base.m_processingContext, false);
					double heightValue = list.HeightValue;
					pagination.EnterIgnorePageBreak(list.Visibility, false);
					for (int i = 0; i < base.m_dataRows.Count; i++)
					{
						base.SetupEnvironment(i, reportItems.RunningValues);
						ListContentInstance listContentInstance = new ListContentInstance(base.m_processingContext, (List)base.m_dataRegionDef);
						if (Report.ShowHideTypes.Interactive == base.m_processingContext.ShowHideType)
						{
							base.m_processingContext.EnterGrouping();
							((IShowHideContainer)listContentInstance).BeginProcessContainer(base.m_processingContext);
						}
						if (!pagination.IgnoreHeight)
						{
							pagination.AddToCurrentPageHeight(list, heightValue);
						}
						if (!pagination.IgnorePageBreak && pagination.CurrentPageHeight >= pagination.PageHeight && listInstance.NumberOfContentsOnThisPage > 0)
						{
							RenderingPagesRanges renderingPagesRanges = default(RenderingPagesRanges);
							renderingPagesRanges.StartRow = instanceList.Count - listInstance.NumberOfContentsOnThisPage;
							renderingPagesRanges.NumberOfDetails = listInstance.NumberOfContentsOnThisPage;
							pagination.SetCurrentPageHeight(list, 0.0);
							list.ContentStartPage++;
							list.BottomInEndPage = 0.0;
							listInstance.ChildrenStartAndEndPages.Add(renderingPagesRanges);
							listInstance.NumberOfContentsOnThisPage = 1;
						}
						else
						{
							listInstance.NumberOfContentsOnThisPage++;
						}
						int startPage = list.StartPage;
						list.StartPage = list.ContentStartPage;
						pagination.EnterIgnorePageBreak(null, true);
						pagination.EnterIgnoreHeight(true);
						base.m_dataRegionDef.CurrentDetailRowIndex = i;
						base.m_processingContext.PageSectionContext.EnterVisibilityScope(list.Visibility, list.StartHidden);
						runtimeRICollection.CreateInstances(listContentInstance.ReportItemColInstance);
						base.m_processingContext.PageSectionContext.ExitVisibilityScope();
						pagination.LeaveIgnoreHeight(true);
						pagination.LeaveIgnorePageBreak(null, true);
						pagination.ProcessEndGroupPage(0.0, false, list, listInstance.NumberOfContentsOnThisPage > 0, list.StartHidden);
						if (Report.ShowHideTypes.Interactive == base.m_processingContext.ShowHideType)
						{
							((IShowHideContainer)listContentInstance).EndProcessContainer(base.m_processingContext);
							base.m_processingContext.ExitGrouping();
						}
						list.StartPage = startPage;
						instanceList.Add(listContentInstance);
						runtimeRICollection.ResetReportItemObjs();
					}
					list.EndPage = Math.Max(list.ContentStartPage, list.EndPage);
					pagination.LeaveIgnorePageBreak(list.Visibility, false);
				}
			}

			internal override void SetupEnvironment()
			{
				base.SetupEnvironment(this.m_nonCustomAggregates, this.m_customAggregates, (base.m_dataRows == null) ? null : base.m_dataRows[0]);
				base.SetupAggregates(this.m_postSortAggregates);
			}

			internal override bool InScope(string scope)
			{
				return base.DataRegionInScope(base.m_dataRegionDef, scope);
			}

			protected override int GetRecursiveLevel(string scope)
			{
				return base.DataRegionRecursiveLevel(base.m_dataRegionDef, scope);
			}

			protected override IHierarchyObj CreateHierarchyObj()
			{
				if (this.m_userSortTargetInfo != null)
				{
					return new RuntimeSortHierarchyObj(this);
				}
				return base.CreateHierarchyObj();
			}

			protected override void ProcessUserSort()
			{
				base.m_processingContext.ProcessUserSortForTarget((IHierarchyObj)this, ref base.m_dataRows, this.m_userSortTargetInfo.TargetForNonDetailSort);
			}

			protected override void MarkSortInfoProcessed(RuntimeSortFilterEventInfoList runtimeSortFilterInfo)
			{
				if (this.m_userSortTargetInfo != null)
				{
					this.m_userSortTargetInfo.MarkSortInfoProcessed(runtimeSortFilterInfo, this);
				}
			}

			protected override void AddSortInfoIndex(int sortInfoIndex, RuntimeSortFilterEventInfo sortInfo)
			{
				if (this.m_userSortTargetInfo != null)
				{
					this.m_userSortTargetInfo.AddSortInfoIndex(sortInfoIndex, sortInfo);
				}
			}

			internal override bool TargetScopeMatched(int index, bool detailSort)
			{
				return base.m_outerScope.TargetScopeMatched(index, detailSort);
			}

			internal override void GetScopeValues(IHierarchyObj targetScopeObj, VariantList[] scopeValues, ref int index)
			{
				if (targetScopeObj == null)
				{
					base.GetScopeValues(targetScopeObj, scopeValues, ref index);
				}
				else if (this != targetScopeObj)
				{
					base.m_outerScope.GetScopeValues(targetScopeObj, scopeValues, ref index);
				}
			}

			protected override void GetGroupNameValuePairs(Dictionary<string, object> pairs)
			{
				base.DataRegionGetGroupNameValuePairs(base.m_dataRegionDef, pairs);
			}
		}

		private sealed class RuntimeOnePassListDetailObj : RuntimeOnePassDetailObj, IFilterOwner
		{
			private RuntimeRICollection m_reportItemCol;

			private Filters m_filters;

			private ListContentInstanceList m_listContentInstances;

			internal ListContentInstanceList ListContentInstances
			{
				get
				{
					return this.m_listContentInstances;
				}
			}

			protected override string ScopeName
			{
				get
				{
					return base.m_dataRegionDef.Name;
				}
			}

			internal RuntimeOnePassListDetailObj(IScope outerScope, List listDef, ProcessingContext processingContext)
				: base(outerScope, listDef, processingContext)
			{
				Global.Tracer.Assert(null == listDef.Sorting, "(null == listDef.Sorting)");
				Global.Tracer.Assert(null == listDef.Aggregates, "(null == listDef.Aggregates)");
				if (listDef.Filters != null)
				{
					this.m_filters = new Filters(Filters.FilterTypes.DataRegionFilter, this, listDef.Filters, listDef.ObjectType, listDef.Name, base.m_processingContext);
				}
				if (listDef.ReportItems != null)
				{
					this.m_reportItemCol = new RuntimeRICollection(this, listDef.ReportItems, base.m_processingContext, false);
					base.AddRunningValues(listDef.ReportItems.RunningValues);
				}
				this.m_listContentInstances = new ListContentInstanceList();
				listDef.ContentStartPage = 0;
			}

			internal override int GetDetailPage()
			{
				return ((List)base.m_dataRegionDef).ContentStartPage;
			}

			internal override void NextRow()
			{
				if (!base.m_processingContext.ReportObjectModel.FieldsImpl.IsAggregateRow)
				{
					bool flag = true;
					if (this.m_filters != null)
					{
						flag = this.m_filters.PassFilters(base.m_processingContext.ReportObjectModel.FieldsImpl.GetFields());
					}
					if (flag)
					{
						((IFilterOwner)this).PostFilterNextRow();
					}
				}
			}

			void IFilterOwner.PostFilterNextRow()
			{
				base.NextRow();
			}

			protected override void CreateInstance()
			{
				List list = (List)base.m_dataRegionDef;
				double heightValue = list.HeightValue;
				Pagination pagination = base.m_processingContext.Pagination;
				base.m_pagination.CopyPaginationInfo(pagination);
				base.m_processingContext.Pagination = base.m_pagination;
				NavigationInfo navigationInfo = base.m_processingContext.NavigationInfo;
				base.m_processingContext.NavigationInfo = base.m_navigationInfo;
				base.m_pagination.EnterIgnorePageBreak(list.Visibility, false);
				ListContentInstance listContentInstance = new ListContentInstance(base.m_processingContext, list);
				if (Report.ShowHideTypes.Interactive == base.m_processingContext.ShowHideType)
				{
					base.m_processingContext.EnterGrouping();
					((IShowHideContainer)listContentInstance).BeginProcessContainer(base.m_processingContext);
				}
				if (!base.m_pagination.IgnoreHeight)
				{
					base.m_pagination.AddToCurrentPageHeight(list, heightValue);
				}
				if (!base.m_pagination.IgnorePageBreak && base.m_pagination.CurrentPageHeight >= base.m_pagination.PageHeight && base.m_numberOfContentsOnThisPage > 0)
				{
					RenderingPagesRanges renderingPagesRanges = default(RenderingPagesRanges);
					renderingPagesRanges.StartRow = this.m_listContentInstances.Count - base.m_numberOfContentsOnThisPage;
					renderingPagesRanges.NumberOfDetails = base.m_numberOfContentsOnThisPage;
					base.m_pagination.SetCurrentPageHeight(list, 0.0);
					list.ContentStartPage++;
					list.BottomInEndPage = 0.0;
					base.m_renderingPages.Add(renderingPagesRanges);
					base.m_numberOfContentsOnThisPage = 1;
				}
				else
				{
					base.m_numberOfContentsOnThisPage++;
				}
				int startPage = list.StartPage;
				list.StartPage = list.ContentStartPage;
				if (base.m_processingContext.ReportObjectModel.FieldsImpl.AddRowIndex)
				{
					base.m_dataRegionDef.CurrentDetailRowIndex = base.m_processingContext.ReportObjectModel.FieldsImpl.GetRowIndex();
				}
				base.m_pagination.EnterIgnorePageBreak(null, true);
				base.m_pagination.EnterIgnoreHeight(true);
				base.m_processingContext.PageSectionContext.EnterVisibilityScope(list.Visibility, list.StartHidden);
				this.m_reportItemCol.CreateInstances(listContentInstance.ReportItemColInstance);
				base.m_processingContext.PageSectionContext.ExitVisibilityScope();
				base.m_pagination.LeaveIgnoreHeight(true);
				base.m_pagination.LeaveIgnorePageBreak(null, true);
				if (Report.ShowHideTypes.Interactive == base.m_processingContext.ShowHideType)
				{
					((IShowHideContainer)listContentInstance).EndProcessContainer(base.m_processingContext);
				}
				if (Report.ShowHideTypes.Interactive == base.m_processingContext.ShowHideType)
				{
					base.m_processingContext.ExitGrouping();
				}
				base.m_pagination.ProcessEndGroupPage(0.0, false, list, base.m_numberOfContentsOnThisPage > 0, list.StartHidden);
				list.StartPage = startPage;
				list.EndPage = Math.Max(list.ContentStartPage, list.EndPage);
				base.m_pagination.LeaveIgnorePageBreak(list.Visibility, false);
				base.m_processingContext.Pagination = pagination;
				base.m_processingContext.NavigationInfo = navigationInfo;
				this.m_listContentInstances.Add(listContentInstance);
			}

			internal override bool InScope(string scope)
			{
				return base.DataRegionInScope(base.m_dataRegionDef, scope);
			}
		}

		internal abstract class RuntimeRDLDataRegionObj : RuntimeDataRegionObj, IFilterOwner, IHierarchyObj
		{
			protected IScope m_outerScope;

			protected FieldImpl[] m_firstRow;

			protected bool m_firstRowIsAggregate;

			protected Filters m_filters;

			protected DataAggregateObjList m_nonCustomAggregates;

			protected DataAggregateObjList m_customAggregates;

			protected DataActions m_dataAction;

			protected DataActions m_outerDataAction;

			protected DataAggregateObjList m_runningValues;

			protected DataAggregateObjResult[] m_runningValueValues;

			protected DataAggregateObjList m_postSortAggregates;

			protected DataRowList m_dataRows;

			protected DataActions m_innerDataAction;

			protected RuntimeUserSortTargetInfo m_userSortTargetInfo;

			protected int[] m_sortFilterExpressionScopeInfoIndices;

			protected override IScope OuterScope
			{
				get
				{
					return this.m_outerScope;
				}
			}

			protected abstract DataRegion DataRegionDef
			{
				get;
			}

			internal override bool TargetForNonDetailSort
			{
				get
				{
					if (this.m_userSortTargetInfo != null && this.m_userSortTargetInfo.TargetForNonDetailSort)
					{
						return true;
					}
					return this.m_outerScope.TargetForNonDetailSort;
				}
			}

			protected override int[] SortFilterExpressionScopeInfoIndices
			{
				get
				{
					if (this.m_sortFilterExpressionScopeInfoIndices == null)
					{
						this.m_sortFilterExpressionScopeInfoIndices = new int[base.m_processingContext.RuntimeSortFilterInfo.Count];
						for (int i = 0; i < base.m_processingContext.RuntimeSortFilterInfo.Count; i++)
						{
							this.m_sortFilterExpressionScopeInfoIndices[i] = -1;
						}
					}
					return this.m_sortFilterExpressionScopeInfoIndices;
				}
			}

			IHierarchyObj IHierarchyObj.HierarchyRoot
			{
				get
				{
					return this;
				}
			}

			ProcessingContext IHierarchyObj.ProcessingContext
			{
				get
				{
					return base.m_processingContext;
				}
			}

			BTreeNode IHierarchyObj.SortTree
			{
				get
				{
					if (this.m_userSortTargetInfo != null)
					{
						return this.m_userSortTargetInfo.SortTree;
					}
					return null;
				}
				set
				{
					if (this.m_userSortTargetInfo != null)
					{
						this.m_userSortTargetInfo.SortTree = value;
					}
					else
					{
						Global.Tracer.Assert(false);
					}
				}
			}

			int IHierarchyObj.ExpressionIndex
			{
				get
				{
					return 0;
				}
			}

			IntList IHierarchyObj.SortFilterInfoIndices
			{
				get
				{
					if (this.m_userSortTargetInfo != null)
					{
						return this.m_userSortTargetInfo.SortFilterInfoIndices;
					}
					return null;
				}
			}

			bool IHierarchyObj.IsDetail
			{
				get
				{
					return false;
				}
			}

			internal RuntimeRDLDataRegionObj(IScope outerScope, DataRegion dataRegionDef, ref DataActions dataAction, ProcessingContext processingContext, bool onePassProcess, RunningValueInfoList runningValues)
				: base(processingContext)
			{
				this.m_outerScope = outerScope;
				RuntimeDataRegionObj.CreateAggregates(base.m_processingContext, dataRegionDef.Aggregates, ref this.m_nonCustomAggregates, ref this.m_customAggregates);
				if (dataRegionDef.Filters != null)
				{
					this.m_filters = new Filters(Filters.FilterTypes.DataRegionFilter, this, dataRegionDef.Filters, dataRegionDef.ObjectType, dataRegionDef.Name, base.m_processingContext);
				}
				else
				{
					this.m_outerDataAction = dataAction;
					this.m_dataAction = dataAction;
					dataAction = DataActions.None;
				}
				if (onePassProcess)
				{
					if (runningValues != null && 0 < runningValues.Count)
					{
						RuntimeDataRegionObj.CreateAggregates(base.m_processingContext, runningValues, ref this.m_nonCustomAggregates);
					}
				}
				else if (runningValues != null && 0 < runningValues.Count)
				{
					this.m_dataAction |= DataActions.PostSortAggregates;
				}
			}

			internal override bool IsTargetForSort(int index, bool detailSort)
			{
				if (this.m_userSortTargetInfo != null && this.m_userSortTargetInfo.IsTargetForSort(index, detailSort))
				{
					return true;
				}
				return this.m_outerScope.IsTargetForSort(index, detailSort);
			}

			IHierarchyObj IHierarchyObj.CreateHierarchyObj()
			{
				return new RuntimeSortHierarchyObj(this);
			}

			ProcessingMessageList IHierarchyObj.RegisterComparisonError(string propertyName)
			{
				return base.m_processingContext.RegisterComparisonErrorForSortFilterEvent(propertyName);
			}

			void IHierarchyObj.NextRow()
			{
				Global.Tracer.Assert(false);
			}

			void IHierarchyObj.Traverse(ProcessingStages operation)
			{
				Global.Tracer.Assert(false);
			}

			void IHierarchyObj.ReadRow()
			{
				this.ReadRow(DataActions.UserSort);
			}

			void IHierarchyObj.ProcessUserSort()
			{
				base.m_processingContext.ProcessUserSortForTarget((IHierarchyObj)this, ref this.m_dataRows, this.m_userSortTargetInfo.TargetForNonDetailSort);
				this.m_dataAction &= ~DataActions.UserSort;
				if (this.m_userSortTargetInfo.TargetForNonDetailSort)
				{
					this.m_userSortTargetInfo.ResetTargetForNonDetailSort();
					this.m_userSortTargetInfo.EnterProcessUserSortPhase(base.m_processingContext);
					DataActions innerDataAction = this.m_innerDataAction;
					this.ConstructRuntimeStructure(ref innerDataAction);
					if (this.m_dataAction != 0)
					{
						this.m_dataRows = new DataRowList();
					}
					base.ScopeFinishSorting(ref this.m_firstRow, this.m_userSortTargetInfo);
					this.m_userSortTargetInfo.LeaveProcessUserSortPhase(base.m_processingContext);
				}
			}

			void IHierarchyObj.MarkSortInfoProcessed(RuntimeSortFilterEventInfoList runtimeSortFilterInfo)
			{
				if (this.m_userSortTargetInfo != null)
				{
					this.m_userSortTargetInfo.MarkSortInfoProcessed(runtimeSortFilterInfo, this);
				}
			}

			void IHierarchyObj.AddSortInfoIndex(int sortInfoIndex, RuntimeSortFilterEventInfo sortInfo)
			{
				if (this.m_userSortTargetInfo != null)
				{
					this.m_userSortTargetInfo.AddSortInfoIndex(sortInfoIndex, sortInfo);
				}
			}

			protected abstract void ConstructRuntimeStructure(ref DataActions innerDataAction);

			protected DataActions HandleSortFilterEvent()
			{
				DataActions result = DataActions.None;
				if (base.m_processingContext.IsSortFilterTarget(this.DataRegionDef.IsSortFilterTarget, this.m_outerScope, (IHierarchyObj)this, ref this.m_userSortTargetInfo) && this.m_userSortTargetInfo.TargetForNonDetailSort)
				{
					result = DataActions.UserSort;
				}
				base.m_processingContext.RegisterSortFilterExpressionScope(this.m_outerScope, this, this.DataRegionDef.IsSortFilterExpressionScope);
				return result;
			}

			internal override bool TargetScopeMatched(int index, bool detailSort)
			{
				return this.m_outerScope.TargetScopeMatched(index, detailSort);
			}

			internal override void GetScopeValues(IHierarchyObj targetScopeObj, VariantList[] scopeValues, ref int index)
			{
				if (this != targetScopeObj)
				{
					this.m_outerScope.GetScopeValues(targetScopeObj, scopeValues, ref index);
				}
			}

			internal override void NextRow()
			{
				if (base.m_processingContext.ReportObjectModel.FieldsImpl.AggregationFieldCount == 0)
				{
					RuntimeDataRegionObj.UpdateAggregates(base.m_processingContext, this.m_customAggregates, false);
				}
				if (base.m_processingContext.ReportObjectModel.FieldsImpl.IsAggregateRow)
				{
					base.ScopeNextAggregateRow(this.m_userSortTargetInfo);
				}
				else
				{
					this.NextNonAggregateRow();
				}
			}

			private void NextNonAggregateRow()
			{
				bool flag = true;
				if (this.m_filters != null)
				{
					flag = this.m_filters.PassFilters(base.m_processingContext.ReportObjectModel.FieldsImpl.GetFields());
				}
				if (flag)
				{
					((IFilterOwner)this).PostFilterNextRow();
				}
			}

			void IFilterOwner.PostFilterNextRow()
			{
				RuntimeDataRegionObj.CommonFirstRow(base.m_processingContext.ReportObjectModel.FieldsImpl, ref this.m_firstRowIsAggregate, ref this.m_firstRow);
				base.ScopeNextNonAggregateRow(this.m_nonCustomAggregates, this.m_dataRows);
			}

			internal override bool SortAndFilter()
			{
				if ((ProcessingContext.SecondPassOperations.Filtering & base.m_processingContext.SecondPassOperation) != 0 && this.m_dataRows != null && (this.m_outerDataAction & DataActions.RecursiveAggregates) != 0)
				{
					this.ReadRows(DataActions.RecursiveAggregates);
					base.ReleaseDataRows(DataActions.RecursiveAggregates, ref this.m_dataAction, ref this.m_dataRows);
				}
				return true;
			}

			protected void ReadRows(DataActions action)
			{
				for (int i = 0; i < this.m_dataRows.Count; i++)
				{
					FieldImpl[] fields = this.m_dataRows[i];
					base.m_processingContext.ReportObjectModel.FieldsImpl.SetFields(fields);
					this.ReadRow(action);
				}
			}

			protected void SetupEnvironment(RunningValueInfoList runningValues)
			{
				base.SetupEnvironment(this.m_nonCustomAggregates, this.m_customAggregates, this.m_firstRow);
				base.SetupAggregates(this.m_postSortAggregates);
				base.SetupRunningValues(runningValues, this.m_runningValueValues);
			}

			internal override bool InScope(string scope)
			{
				return base.DataRegionInScope(this.DataRegionDef, scope);
			}

			protected override int GetRecursiveLevel(string scope)
			{
				return base.DataRegionRecursiveLevel(this.DataRegionDef, scope);
			}

			protected override void GetGroupNameValuePairs(Dictionary<string, object> pairs)
			{
				base.DataRegionGetGroupNameValuePairs(this.DataRegionDef, pairs);
			}
		}

		private sealed class RuntimeTableObj : RuntimeRDLDataRegionObj
		{
			private Table m_tableDef;

			private RuntimeGroupRootObj m_tableGroups;

			private RuntimeDataRegionObj m_detailObj;

			private RuntimeRICollectionList m_headerReportItemCols;

			private RuntimeRICollectionList m_footerReportItemCols;

			internal TableDetailInstanceList TableDetailInstances
			{
				get
				{
					if (this.m_detailObj is RuntimeOnePassTableDetailObj)
					{
						return ((RuntimeOnePassTableDetailObj)this.m_detailObj).TableDetailInstances;
					}
					return null;
				}
			}

			internal RenderingPagesRangesList ChildrenStartAndEndPages
			{
				get
				{
					if (this.m_detailObj is RuntimeOnePassTableDetailObj)
					{
						return ((RuntimeOnePassTableDetailObj)this.m_detailObj).ChildrenStartAndEndPages;
					}
					return null;
				}
			}

			protected override string ScopeName
			{
				get
				{
					return this.m_tableDef.Name;
				}
			}

			protected override DataRegion DataRegionDef
			{
				get
				{
					return this.m_tableDef;
				}
			}

			internal RuntimeTableObj(IScope outerScope, Table tableDef, ref DataActions dataAction, ProcessingContext processingContext, bool onePassProcess)
				: base(outerScope, (DataRegion)tableDef, ref dataAction, processingContext, onePassProcess, tableDef.RunningValues)
			{
				this.m_tableDef = tableDef;
				DataActions dataActions = base.HandleSortFilterEvent();
				if (onePassProcess)
				{
					RuntimeDataRegionObj.CreateAggregates(base.m_processingContext, tableDef.PostSortAggregates, ref base.m_nonCustomAggregates);
					if (tableDef.TableDetail != null)
					{
						this.m_detailObj = new RuntimeOnePassTableDetailObj(this, tableDef, processingContext);
					}
					TableRowList headerRows = tableDef.HeaderRows;
					if (headerRows != null)
					{
						this.m_headerReportItemCols = new RuntimeRICollectionList(headerRows.Count);
						for (int i = 0; i < headerRows.Count; i++)
						{
							this.m_headerReportItemCols.Add(new RuntimeRICollection(this, headerRows[i].ReportItems, base.m_processingContext, true));
						}
					}
					headerRows = tableDef.FooterRows;
					if (headerRows != null)
					{
						this.m_footerReportItemCols = new RuntimeRICollectionList(headerRows.Count);
						for (int j = 0; j < headerRows.Count; j++)
						{
							this.m_footerReportItemCols.Add(new RuntimeRICollection(this, headerRows[j].ReportItems, base.m_processingContext, true));
						}
					}
				}
				else
				{
					bool flag = false;
					if (tableDef.PostSortAggregates != null)
					{
						RuntimeDataRegionObj.CreateAggregates(base.m_processingContext, tableDef.PostSortAggregates, ref base.m_postSortAggregates);
						base.m_dataAction |= DataActions.PostSortAggregates;
						if (tableDef.TableDetail == null || tableDef.TableGroups != null)
						{
							flag = true;
						}
					}
					DataActions dataAction2 = base.m_innerDataAction = ((!flag) ? base.m_dataAction : DataActions.None);
					this.ConstructRuntimeStructure(ref dataAction2);
					if (!flag)
					{
						base.m_dataAction = dataAction2;
					}
				}
				base.m_dataAction |= dataActions;
				if (base.m_dataAction != 0)
				{
					base.m_dataRows = new DataRowList();
				}
			}

			protected override void ConstructRuntimeStructure(ref DataActions innerDataAction)
			{
				if (this.m_tableDef.TableGroups != null)
				{
					this.m_tableGroups = new RuntimeTableGroupRootObj((IScope)this, this.m_tableDef.TableGroups, ref innerDataAction, base.m_processingContext);
				}
				else if (this.m_tableDef.TableDetail != null)
				{
					innerDataAction = base.m_dataAction;
					this.m_detailObj = new RuntimeTableDetailObj((IScope)this, this.m_tableDef, ref innerDataAction, base.m_processingContext);
				}
				TableRowList headerRows = this.m_tableDef.HeaderRows;
				if (headerRows != null)
				{
					this.m_headerReportItemCols = new RuntimeRICollectionList(headerRows.Count);
					for (int i = 0; i < headerRows.Count; i++)
					{
						RuntimeRICollection value = new RuntimeRICollection((IScope)this, headerRows[i].ReportItems, ref innerDataAction, base.m_processingContext, true);
						this.m_headerReportItemCols.Add(value);
					}
				}
				headerRows = this.m_tableDef.FooterRows;
				if (headerRows != null)
				{
					this.m_footerReportItemCols = new RuntimeRICollectionList(headerRows.Count);
					for (int j = 0; j < headerRows.Count; j++)
					{
						RuntimeRICollection value = new RuntimeRICollection((IScope)this, headerRows[j].ReportItems, ref innerDataAction, base.m_processingContext, true);
						this.m_footerReportItemCols.Add(value);
					}
				}
			}

			protected override void SendToInner()
			{
				if (this.m_headerReportItemCols != null)
				{
					this.m_headerReportItemCols.FirstPassNextDataRow();
				}
				if (this.m_footerReportItemCols != null)
				{
					this.m_footerReportItemCols.FirstPassNextDataRow();
				}
				if (this.m_tableGroups != null)
				{
					this.m_tableGroups.NextRow();
				}
				if (this.m_detailObj != null)
				{
					this.m_detailObj.NextRow();
				}
			}

			internal override bool SortAndFilter()
			{
				this.SetupEnvironment();
				if (base.m_userSortTargetInfo != null)
				{
					base.m_userSortTargetInfo.EnterProcessUserSortPhase(base.m_processingContext);
				}
				if (this.m_tableGroups != null)
				{
					this.m_tableGroups.SortAndFilter();
				}
				if (this.m_detailObj != null)
				{
					this.m_detailObj.SortAndFilter();
				}
				bool result = base.SortAndFilter();
				if (base.m_userSortTargetInfo != null)
				{
					base.m_userSortTargetInfo.LeaveProcessUserSortPhase(base.m_processingContext);
				}
				return result;
			}

			internal override void ReadRow(DataActions dataAction)
			{
				if (DataActions.UserSort == dataAction)
				{
					RuntimeDataRegionObj.CommonFirstRow(base.m_processingContext.ReportObjectModel.FieldsImpl, ref base.m_firstRowIsAggregate, ref base.m_firstRow);
					base.CommonNextRow(base.m_dataRows);
				}
				else
				{
					if (DataActions.PostSortAggregates == dataAction)
					{
						if (base.m_postSortAggregates != null)
						{
							RuntimeDataRegionObj.UpdateAggregates(base.m_processingContext, base.m_postSortAggregates, false);
						}
						if (base.m_runningValues != null)
						{
							for (int i = 0; i < base.m_runningValues.Count; i++)
							{
								base.m_runningValues[i].Update();
							}
						}
						this.CalculatePreviousAggregates();
					}
					if (base.m_outerScope != null && (dataAction & base.m_outerDataAction) != 0)
					{
						base.m_outerScope.ReadRow(dataAction);
					}
				}
			}

			private void CalculatePreviousAggregates()
			{
				if (!base.m_processedPreviousAggregates && base.m_processingContext.GlobalRVCollection != null)
				{
					Global.Tracer.Assert(null == base.m_runningValueValues, "(null == m_runningValueValues)");
					RunningValueInfoList runningValues = this.m_tableDef.RunningValues;
					if (runningValues != null && 0 < runningValues.Count)
					{
						RuntimeRICollection.DoneReadingRows(base.m_processingContext.GlobalRVCollection, runningValues, ref base.m_runningValueValues, true);
					}
					if (this.m_headerReportItemCols != null)
					{
						this.m_headerReportItemCols.CalculatePreviousAggregates(base.m_processingContext.GlobalRVCollection);
					}
					if (this.m_footerReportItemCols != null)
					{
						this.m_footerReportItemCols.CalculatePreviousAggregates(base.m_processingContext.GlobalRVCollection);
					}
					base.m_processedPreviousAggregates = true;
				}
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				if (this.m_tableDef.RunningValues != null)
				{
					RuntimeDetailObj.AddRunningValues(base.m_processingContext, this.m_tableDef.RunningValues, ref base.m_runningValues, globalRVCol, groupCol, lastGroup);
				}
				if (this.m_headerReportItemCols != null)
				{
					this.m_headerReportItemCols.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				}
				if (this.m_footerReportItemCols != null)
				{
					this.m_footerReportItemCols.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				}
				if (this.m_tableGroups != null)
				{
					this.m_tableGroups.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				}
				if (this.m_detailObj != null)
				{
					this.m_detailObj.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				}
				if (base.m_dataRows != null)
				{
					base.ReadRows(DataActions.PostSortAggregates);
					base.m_dataRows = null;
				}
				RuntimeRICollection.DoneReadingRows(globalRVCol, this.m_tableDef.RunningValues, ref base.m_runningValueValues, false);
			}

			internal static void CreateRowInstances(ProcessingContext processingContext, RuntimeRICollectionList rowRICols, TableRowInstance[] rowInstances, bool repeatOnNewPages, bool enterGrouping)
			{
				if (rowRICols != null)
				{
					if (Report.ShowHideTypes.Interactive == processingContext.ShowHideType && enterGrouping)
					{
						processingContext.EnterGrouping();
					}
					for (int i = 0; i < rowRICols.Count; i++)
					{
						if (Report.ShowHideTypes.Interactive == processingContext.ShowHideType)
						{
							((IShowHideContainer)rowInstances[i]).BeginProcessContainer(processingContext);
						}
						processingContext.Pagination.EnterIgnorePageBreak(null, true);
						processingContext.Pagination.EnterIgnoreHeight(true);
						processingContext.PageSectionContext.EnterVisibilityScope(rowInstances[i].TableRowDef.Visibility, rowInstances[i].TableRowDef.StartHidden);
						IntList tableColumnSpans = processingContext.PageSectionContext.TableColumnSpans;
						processingContext.PageSectionContext.TableColumnSpans = rowInstances[i].TableRowDef.ColSpans;
						rowRICols[i].CreateInstances(rowInstances[i].TableRowReportItemColInstance, true, repeatOnNewPages);
						processingContext.PageSectionContext.TableColumnSpans = tableColumnSpans;
						processingContext.PageSectionContext.ExitVisibilityScope();
						processingContext.Pagination.LeaveIgnoreHeight(true);
						processingContext.Pagination.LeaveIgnorePageBreak(null, true);
						if (Report.ShowHideTypes.Interactive == processingContext.ShowHideType)
						{
							((IShowHideContainer)rowInstances[i]).EndProcessContainer(processingContext);
						}
					}
					if (Report.ShowHideTypes.Interactive == processingContext.ShowHideType && enterGrouping)
					{
						processingContext.ExitGrouping();
					}
				}
			}

			internal override void CreateInstances(ReportItemInstance riInstance, IList instanceList, RenderingPagesRangesList pagesList)
			{
				this.SetupEnvironment();
				TableInstance tableInstance = (TableInstance)riInstance;
				Table table = (Table)tableInstance.ReportItemDef;
				base.m_processingContext.Pagination.InitProcessTableRenderingPages(tableInstance, table);
				base.m_processingContext.PageSectionContext.RegisterTableColumnVisibility(base.m_processingContext.IsOnePass, table.TableColumns, table.ColumnsStartHidden);
				base.m_processingContext.PageSectionContext.EnterRepeatingItem();
				RuntimeTableObj.CreateRowInstances(base.m_processingContext, this.m_headerReportItemCols, tableInstance.HeaderRowInstances, table.HeaderRepeatOnNewPage, false);
				PageTextboxes source = base.m_processingContext.PageSectionContext.ExitRepeatingItem();
				base.m_processingContext.PageSectionContext.EnterRepeatingItem();
				RuntimeTableObj.CreateRowInstances(base.m_processingContext, this.m_footerReportItemCols, tableInstance.FooterRowInstances, table.FooterRepeatOnNewPage, false);
				PageTextboxes source2 = base.m_processingContext.PageSectionContext.ExitRepeatingItem();
				bool delayAddingInstanceInfo = base.m_processingContext.DelayAddingInstanceInfo;
				base.m_processingContext.DelayAddingInstanceInfo = false;
				if (this.m_tableGroups != null)
				{
					this.m_tableGroups.CreateInstances(tableInstance, tableInstance.TableGroupInstances, tableInstance.ChildrenStartAndEndPages);
					this.m_tableGroups = null;
				}
				else if (this.m_detailObj != null)
				{
					int num = 0;
					if (this.m_detailObj is RuntimeDetailObj)
					{
						((RuntimeDetailObj)this.m_detailObj).CreateInstances((ReportItemInstance)tableInstance, (IList)tableInstance.TableDetailInstances, tableInstance.ChildrenStartAndEndPages, out num);
						tableInstance.NumberOfChildrenOnThisPage = num;
					}
					else
					{
						RenderingPagesRangesList childrenStartAndEndPages = tableInstance.ChildrenStartAndEndPages;
						this.m_detailObj.CreateInstances(tableInstance, tableInstance.TableDetailInstances, childrenStartAndEndPages);
						Global.Tracer.Assert(this.m_detailObj is RuntimeOnePassTableDetailObj, "(m_detailObj is RuntimeOnePassTableDetailObj)");
						RuntimeOnePassTableDetailObj runtimeOnePassTableDetailObj = (RuntimeOnePassTableDetailObj)this.m_detailObj;
						if (childrenStartAndEndPages != null && (!base.m_processingContext.PageSectionContext.IsParentVisible() || !Visibility.IsOnePassHierarchyVisible(tableInstance.ReportItemDef)))
						{
							runtimeOnePassTableDetailObj.MoveAllToFirstPage();
							int totalCount = (tableInstance.TableDetailInstances != null) ? tableInstance.TableDetailInstances.Count : 0;
							childrenStartAndEndPages.MoveAllToFirstPage(totalCount);
							runtimeOnePassTableDetailObj.NumberOfContentsOnThisPage = 0;
						}
						TableInstance tableInstance2 = tableInstance;
						num = (tableInstance2.NumberOfChildrenOnThisPage = runtimeOnePassTableDetailObj.NumberOfContentsOnThisPage);
						if (num > 0)
						{
							if (childrenStartAndEndPages != null && 0 < childrenStartAndEndPages.Count)
							{
								base.m_processingContext.Pagination.SetCurrentPageHeight(table, runtimeOnePassTableDetailObj.Pagination.CurrentPageHeight);
							}
							else
							{
								base.m_processingContext.Pagination.AddToCurrentPageHeight(table, runtimeOnePassTableDetailObj.Pagination.CurrentPageHeight);
							}
						}
						base.m_processingContext.NavigationInfo.AppendNavigationInfo(base.m_processingContext.NavigationInfo.CurrentLabel, runtimeOnePassTableDetailObj.NavigationInfo, table.StartPage);
					}
					if (num > 0)
					{
						RenderingPagesRanges renderingPagesRanges = default(RenderingPagesRanges);
						renderingPagesRanges.StartRow = tableInstance.TableDetailInstances.Count - num;
						renderingPagesRanges.NumberOfDetails = num;
						tableInstance.ChildrenStartAndEndPages.Add(renderingPagesRanges);
					}
					if (this.m_detailObj is RuntimeOnePassTableDetailObj)
					{
						RenderingPagesRangesList childrenStartAndEndPages2 = tableInstance.ChildrenStartAndEndPages;
						RuntimeOnePassDetailObj runtimeOnePassDetailObj = this.m_detailObj as RuntimeOnePassDetailObj;
						if (childrenStartAndEndPages2 != null && 0 < childrenStartAndEndPages2.Count)
						{
							TableInstance tableInstance3 = tableInstance;
							Table table2 = table;
							int num4 = tableInstance3.CurrentPage = (table2.CurrentPage = table.StartPage + childrenStartAndEndPages2.Count - 1);
							if (base.m_processingContext.PageSectionContext.IsParentVisible())
							{
								for (int i = 0; i < childrenStartAndEndPages2.Count; i++)
								{
									runtimeOnePassDetailObj.ProcessOnePassDetailTextboxes(i, table.StartPage + i);
								}
							}
						}
						else
						{
							TableInstance tableInstance4 = tableInstance;
							Table table3 = table;
							int num6 = tableInstance4.CurrentPage = (table3.CurrentPage = table.StartPage);
							if (base.m_processingContext.PageSectionContext.IsParentVisible() && tableInstance.TableDetailInstances != null)
							{
								runtimeOnePassDetailObj.ProcessOnePassDetailTextboxes(0, tableInstance.CurrentPage);
							}
						}
					}
					this.m_detailObj = null;
				}
				base.m_processingContext.PageSectionContext.UnregisterTableColumnVisibility();
				Global.Tracer.Assert(table.StartPage <= table.CurrentPage, "(tableDef.StartPage <= tableDef.CurrentPage)");
				if (base.m_processingContext.PageSectionContext.PageTextboxes != null)
				{
					base.m_processingContext.PageSectionContext.PageTextboxes.IntegrateRepeatingTextboxValues(source, table.StartPage, table.HeaderRepeatOnNewPage ? table.CurrentPage : table.StartPage);
					base.m_processingContext.PageSectionContext.PageTextboxes.IntegrateRepeatingTextboxValues(source2, table.FooterRepeatOnNewPage ? table.StartPage : table.CurrentPage, table.CurrentPage);
				}
				base.m_processingContext.DelayAddingInstanceInfo = delayAddingInstanceInfo;
			}

			internal override void SetupEnvironment()
			{
				base.SetupEnvironment(this.m_tableDef.RunningValues);
			}

			internal void ResetReportItems()
			{
				if (this.m_headerReportItemCols != null)
				{
					this.m_headerReportItemCols.ResetReportItemObjs();
				}
				if (this.m_footerReportItemCols != null)
				{
					this.m_footerReportItemCols.ResetReportItemObjs();
				}
				this.m_headerReportItemCols = null;
				this.m_footerReportItemCols = null;
			}
		}

		private sealed class RuntimeTableGroupRootObj : RuntimeGroupRootObj
		{
			internal RuntimeTableGroupRootObj(IScope outerScope, TableGroup tableGroupDef, ref DataActions dataAction, ProcessingContext processingContext)
				: base(outerScope, tableGroupDef, dataAction, processingContext)
			{
				if (tableGroupDef.RunningValues != null && 0 < tableGroupDef.RunningValues.Count)
				{
					base.m_dataAction |= DataActions.PostSortAggregates;
				}
				if ((base.m_dataAction & DataActions.PostSortAggregates) == DataActions.None && tableGroupDef.HeaderRows != null)
				{
					int num = 0;
					while (num < tableGroupDef.HeaderRows.Count)
					{
						if (tableGroupDef.HeaderRows[num].ReportItems.RunningValues == null || 0 >= tableGroupDef.HeaderRows[num].ReportItems.RunningValues.Count)
						{
							num++;
							continue;
						}
						base.m_dataAction |= DataActions.PostSortAggregates;
						break;
					}
				}
				if ((base.m_dataAction & DataActions.PostSortAggregates) == DataActions.None && tableGroupDef.FooterRows != null)
				{
					int num2 = 0;
					while (num2 < tableGroupDef.FooterRows.Count)
					{
						if (tableGroupDef.FooterRows[num2].ReportItems.RunningValues == null || 0 >= tableGroupDef.FooterRows[num2].ReportItems.RunningValues.Count)
						{
							num2++;
							continue;
						}
						base.m_dataAction |= DataActions.PostSortAggregates;
						break;
					}
				}
				if (tableGroupDef.Grouping.Filters == null)
				{
					dataAction = DataActions.None;
				}
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				base.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				TableGroup tableGroup = (TableGroup)base.m_hierarchyDef;
				if (tableGroup.HeaderRows != null)
				{
					for (int i = 0; i < tableGroup.HeaderRows.Count; i++)
					{
						base.AddRunningValues(tableGroup.HeaderRows[i].ReportItems.RunningValues);
					}
				}
				if (tableGroup.FooterRows != null)
				{
					for (int j = 0; j < tableGroup.FooterRows.Count; j++)
					{
						base.AddRunningValues(tableGroup.FooterRows[j].ReportItems.RunningValues);
					}
				}
				base.AddRunningValues(tableGroup.RunningValues);
				base.m_grouping.Traverse(ProcessingStages.RunningValues, base.m_expression.Direction);
			}
		}

        private sealed class RuntimeTableGroupLeafObj : ReportProcessing.RuntimeGroupLeafObj
        {
            internal RuntimeTableGroupLeafObj(ReportProcessing.RuntimeTableGroupRootObj groupRoot) : base(groupRoot)
            {
                this.m_dataAction = groupRoot.DataAction;
                bool flag = false;
                bool flag2 = base.HandleSortFilterEvent();
                ReportProcessing.DataActions dataAction;
                this.ConstructRuntimeStructure(ref flag, out dataAction);
                if (!flag)
                {
                    this.m_dataAction = dataAction;
                }
                if (flag2)
                {
                    this.m_dataAction |= ReportProcessing.DataActions.UserSort;
                }
                if (this.m_dataAction != ReportProcessing.DataActions.None)
                {
                    this.m_dataRows = new ReportProcessing.DataRowList();
                }
            }

            protected override void ConstructRuntimeStructure(ref bool handleMyDataAction, out ReportProcessing.DataActions innerDataAction)
            {
                TableGroup tableGroup = (TableGroup)((ReportProcessing.RuntimeGroupRootObj)this.m_hierarchyRoot).HierarchyDef;
                base.ConstructRuntimeStructure(ref handleMyDataAction, out innerDataAction);
                if (tableGroup.InnerHierarchy != null)
                {
                    this.m_innerHierarchy = new ReportProcessing.RuntimeTableGroupRootObj(this, (TableGroup)tableGroup.InnerHierarchy, ref innerDataAction, this.m_processingContext);
                }
                else if (((Table)tableGroup.DataRegionDef).TableDetail != null)
                {
                    this.m_innerHierarchy = new ReportProcessing.RuntimeTableDetailObj(this, (Table)tableGroup.DataRegionDef, ref innerDataAction, this.m_processingContext);
                }
                TableRowList tableRowList = tableGroup.HeaderRows;
                if (tableRowList != null)
                {
                    this.m_headerReportItemCols = new ReportProcessing.RuntimeRICollectionList(tableRowList.Count);
                    for (int i = 0; i < tableRowList.Count; i++)
                    {
                        ReportProcessing.RuntimeRICollection value = new ReportProcessing.RuntimeRICollection(this, tableRowList[i].ReportItems, ref innerDataAction, this.m_processingContext, true);
                        this.m_headerReportItemCols.Add(value);
                    }
                }
                tableRowList = tableGroup.FooterRows;
                if (tableRowList != null)
                {
                    this.m_footerReportItemCols = new ReportProcessing.RuntimeRICollectionList(tableRowList.Count);
                    for (int j = 0; j < tableRowList.Count; j++)
                    {
                        ReportProcessing.RuntimeRICollection value = new ReportProcessing.RuntimeRICollection(this, tableRowList[j].ReportItems, ref innerDataAction, this.m_processingContext, true);
                        this.m_footerReportItemCols.Add(value);
                    }
                }
            }

            protected override void SendToInner()
            {
                base.SendToInner();
                if (this.m_headerReportItemCols != null)
                {
                    this.m_headerReportItemCols.FirstPassNextDataRow();
                }
                if (this.m_footerReportItemCols != null)
                {
                    this.m_footerReportItemCols.FirstPassNextDataRow();
                }
                if (this.m_innerHierarchy != null)
                {
                    this.m_innerHierarchy.NextRow();
                }
            }

            internal override bool SortAndFilter()
            {
                this.SetupEnvironment();
                if (this.m_userSortTargetInfo != null)
                {
                    this.m_userSortTargetInfo.EnterProcessUserSortPhase(this.m_processingContext);
                }
                if (this.m_headerReportItemCols != null)
                {
                    for (int i = 0; i < this.m_headerReportItemCols.Count; i++)
                    {
                        this.m_headerReportItemCols[i].SortAndFilter();
                    }
                }
                if (this.m_footerReportItemCols != null)
                {
                    for (int j = 0; j < this.m_footerReportItemCols.Count; j++)
                    {
                        this.m_footerReportItemCols[j].SortAndFilter();
                    }
                }
                if (this.m_innerHierarchy != null)
                {
                    this.m_innerHierarchy.SortAndFilter();
                }
                bool result = base.SortAndFilter();
                if (this.m_userSortTargetInfo != null)
                {
                    this.m_userSortTargetInfo.LeaveProcessUserSortPhase(this.m_processingContext);
                }
                return result;
            }

            internal override void ReadRow(ReportProcessing.DataActions dataAction)
            {
                if (ReportProcessing.DataActions.UserSort == dataAction)
                {
                    ReportProcessing.RuntimeDataRegionObj.CommonFirstRow(this.m_processingContext.ReportObjectModel.FieldsImpl, ref this.m_firstRowIsAggregate, ref this.m_firstRow);
                    base.CommonNextRow(this.m_dataRows);
                    return;
                }
                base.ReadRow(dataAction);
                if (ReportProcessing.DataActions.PostSortAggregates == dataAction)
                {
                    this.CalculatePreviousAggregates();
                }
            }

            private void CalculatePreviousAggregates()
            {
                if (!this.m_processedPreviousAggregates && this.m_processingContext.GlobalRVCollection != null)
                {
                    Global.Tracer.Assert(null == this.m_runningValueValues, "(null == m_runningValueValues)");
                    ReportProcessing.RuntimeGroupRootObj runtimeGroupRootObj = (ReportProcessing.RuntimeGroupRootObj)this.m_hierarchyRoot;
                    RunningValueInfoList runningValues = ((TableGroup)runtimeGroupRootObj.HierarchyDef).RunningValues;
                    if (runningValues != null && 0 < runningValues.Count)
                    {
                        ReportProcessing.RuntimeRICollection.DoneReadingRows(this.m_processingContext.GlobalRVCollection, runningValues, ref this.m_runningValueValues, true);
                    }
                    if (this.m_headerReportItemCols != null)
                    {
                        this.m_headerReportItemCols.CalculatePreviousAggregates(this.m_processingContext.GlobalRVCollection);
                    }
                    if (this.m_footerReportItemCols != null)
                    {
                        this.m_footerReportItemCols.CalculatePreviousAggregates(this.m_processingContext.GlobalRVCollection);
                    }
                    this.m_processedPreviousAggregates = true;
                }
            }

            internal override void CalculateRunningValues()
            {
                ReportProcessing.RuntimeGroupRootObj runtimeGroupRootObj = (ReportProcessing.RuntimeGroupRootObj)this.m_hierarchyRoot;
                AggregatesImpl globalRunningValueCollection = runtimeGroupRootObj.GlobalRunningValueCollection;
                ReportProcessing.RuntimeGroupRootObjList groupCollection = runtimeGroupRootObj.GroupCollection;
                if (this.m_innerHierarchy != null)
                {
                    this.m_innerHierarchy.CalculateRunningValues(globalRunningValueCollection, groupCollection, runtimeGroupRootObj);
                }
                if (this.m_dataRows != null)
                {
                    base.ReadRows(ReportProcessing.DataActions.PostSortAggregates);
                    this.m_dataRows = null;
                }
                if (this.m_headerReportItemCols != null)
                {
                    this.m_headerReportItemCols.CalculateRunningValues(globalRunningValueCollection, groupCollection, runtimeGroupRootObj);
                }
                if (this.m_footerReportItemCols != null)
                {
                    this.m_footerReportItemCols.CalculateRunningValues(globalRunningValueCollection, groupCollection, runtimeGroupRootObj);
                }
                RunningValueInfoList runningValues = ((TableGroup)runtimeGroupRootObj.HierarchyDef).RunningValues;
                if (runningValues != null && 0 < runningValues.Count)
                {
                    ReportProcessing.RuntimeRICollection.DoneReadingRows(globalRunningValueCollection, runningValues, ref this.m_runningValueValues, false);
                }
                base.CalculateRunningValues();
            }

            internal override void CreateInstance()
            {
                this.SetupEnvironment();
                ReportProcessing.RuntimeGroupRootObj runtimeGroupRootObj = (ReportProcessing.RuntimeGroupRootObj)this.m_hierarchyRoot;
                TableInstance tableInstance = (TableInstance)runtimeGroupRootObj.ReportItemInstance;
                Table table = (Table)tableInstance.ReportItemDef;
                IList instanceList = runtimeGroupRootObj.InstanceList;
                TableGroup tableGroup = (TableGroup)runtimeGroupRootObj.HierarchyDef;
                this.m_processingContext.ChunkManager.CheckPageBreak(tableGroup, true);
                base.SetupRunningValues(tableGroup.RunningValues, this.m_runningValueValues);
                this.m_tableGroupInstance = new TableGroupInstance(this.m_processingContext, tableGroup);
                RenderingPagesRanges renderingPagesRanges = default(RenderingPagesRanges);
                this.m_processingContext.Pagination.InitProcessingTableGroup(tableInstance, table, this.m_tableGroupInstance, tableGroup, ref renderingPagesRanges, 0 == instanceList.Count);
                this.m_startPage = renderingPagesRanges.StartPage;
                if (Report.ShowHideTypes.Interactive == this.m_processingContext.ShowHideType)
                {
                    this.m_processingContext.EnterGrouping();
                    ((IShowHideContainer)this.m_tableGroupInstance).BeginProcessContainer(this.m_processingContext);
                }
                if (tableGroup.Grouping.GroupLabel != null)
                {
                    this.m_label = this.m_processingContext.NavigationInfo.CurrentLabel;
                    if (this.m_label != null)
                    {
                        this.m_processingContext.NavigationInfo.EnterDocumentMapChildren();
                    }
                }
                this.m_processingContext.PageSectionContext.EnterVisibilityScope(tableGroup.Visibility, tableGroup.StartHidden);
                this.m_processingContext.PageSectionContext.EnterRepeatingItem();
                ReportProcessing.RuntimeTableObj.CreateRowInstances(this.m_processingContext, this.m_headerReportItemCols, this.m_tableGroupInstance.HeaderRowInstances, tableGroup.HeaderRepeatOnNewPage, false);
                ReportProcessing.PageTextboxes source = this.m_processingContext.PageSectionContext.ExitRepeatingItem();
                this.m_processingContext.PageSectionContext.EnterRepeatingItem();
                ReportProcessing.RuntimeTableObj.CreateRowInstances(this.m_processingContext, this.m_footerReportItemCols, this.m_tableGroupInstance.FooterRowInstances, tableGroup.FooterRepeatOnNewPage, false);
                ReportProcessing.PageTextboxes source2 = this.m_processingContext.PageSectionContext.ExitRepeatingItem();
                if (this.m_innerHierarchy != null)
                {
                    if (this.m_tableGroupInstance.SubGroupInstances != null)
                    {
                        this.m_innerHierarchy.CreateInstances(tableInstance, this.m_tableGroupInstance.SubGroupInstances, this.m_tableGroupInstance.ChildrenStartAndEndPages);
                    }
                    else
                    {
                        Global.Tracer.Assert(this.m_innerHierarchy is ReportProcessing.RuntimeDetailObj, "(m_innerHierarchy is RuntimeDetailObj)");
                        int num = 0;
                        ((ReportProcessing.RuntimeDetailObj)this.m_innerHierarchy).CreateInstances(tableInstance, this.m_tableGroupInstance.TableDetailInstances, this.m_tableGroupInstance.ChildrenStartAndEndPages, out num);
                        this.m_tableGroupInstance.NumberOfChildrenOnThisPage = num;
                        if (num > 0)
                        {
                            RenderingPagesRanges renderingPagesRanges2 = default(RenderingPagesRanges);
                            renderingPagesRanges2.StartRow = this.m_tableGroupInstance.TableDetailInstances.Count - num;
                            renderingPagesRanges2.NumberOfDetails = num;
                            this.m_tableGroupInstance.ChildrenStartAndEndPages.Add(renderingPagesRanges2);
                        }
                    }
                }
                this.m_processingContext.PageSectionContext.ExitVisibilityScope();
                if (Report.ShowHideTypes.Interactive == this.m_processingContext.ShowHideType)
                {
                    ((IShowHideContainer)this.m_tableGroupInstance).EndProcessContainer(this.m_processingContext);
                    this.m_processingContext.ExitGrouping();
                }
                this.m_processingContext.ChunkManager.CheckPageBreak(tableGroup, false);
                double footerHeightValue = tableGroup.FooterHeightValue;
                tableGroup.EndPage = tableInstance.CurrentPage;
                this.m_processingContext.Pagination.ProcessEndGroupPage(footerHeightValue, tableGroup.PropagatedPageBreakAtEnd || tableGroup.Grouping.PageBreakAtEnd, table, this.m_tableGroupInstance.NumberOfChildrenOnThisPage > 0, tableGroup.StartHidden);
                renderingPagesRanges.EndPage = tableGroup.EndPage;
                runtimeGroupRootObj.PagesList.Add(renderingPagesRanges);
                if (this.m_processingContext.PageSectionContext.PageTextboxes != null)
                {
                    this.m_processingContext.PageSectionContext.PageTextboxes.IntegrateRepeatingTextboxValues(source, renderingPagesRanges.StartPage, tableGroup.HeaderRepeatOnNewPage ? renderingPagesRanges.EndPage : renderingPagesRanges.StartPage);
                    this.m_processingContext.PageSectionContext.PageTextboxes.IntegrateRepeatingTextboxValues(source2, tableGroup.FooterRepeatOnNewPage ? renderingPagesRanges.StartPage : renderingPagesRanges.EndPage, renderingPagesRanges.EndPage);
                }
                this.m_processingContext.Pagination.LeaveIgnorePageBreak(tableGroup.Visibility, false);
                instanceList.Add(this.m_tableGroupInstance);
                if (this.m_headerReportItemCols != null)
                {
                    this.m_headerReportItemCols.ResetReportItemObjs();
                }
                if (this.m_footerReportItemCols != null)
                {
                    this.m_footerReportItemCols.ResetReportItemObjs();
                }
                base.ResetReportItemsWithHideDuplicates();
            }

            protected override void AddToDocumentMap()
            {
                if (base.GroupingDef.GroupLabel != null && this.m_label != null)
                {
                    this.m_processingContext.NavigationInfo.AddToDocumentMap(this.m_tableGroupInstance.UniqueName, true, this.m_startPage, this.m_label);
                }
            }

            private ReportProcessing.RuntimeRICollectionList m_headerReportItemCols;

            private ReportProcessing.RuntimeRICollectionList m_footerReportItemCols;

            private ReportProcessing.RuntimeHierarchyObj m_innerHierarchy;

            private DataAggregateObjResult[] m_runningValueValues;

            private TableGroupInstance m_tableGroupInstance;

            private string m_label;

            private int m_startPage = -1;
        }

        private sealed class RuntimeTableDetailObj : RuntimeDetailObj, ISortDataHolder
		{
			private TableDetail m_detailDef;

			private RuntimeUserSortTargetInfo m_userSortTargetInfo;

			internal override ExpressionInfoList SortExpressions
			{
				get
				{
					Sorting sorting = ((Table)base.m_dataRegionDef).TableDetail.Sorting;
					if (sorting != null && 0 < sorting.SortExpressions.Count)
					{
						return sorting.SortExpressions;
					}
					return null;
				}
			}

			internal override SortingExprHost SortExpressionHost
			{
				get
				{
					Sorting sorting = ((Table)base.m_dataRegionDef).TableDetail.Sorting;
					if (sorting != null)
					{
						return sorting.ExprHost;
					}
					return null;
				}
			}

			internal override BoolList SortDirections
			{
				get
				{
					Sorting sorting = ((Table)base.m_dataRegionDef).TableDetail.Sorting;
					if (sorting != null && 0 < sorting.SortDirections.Count)
					{
						return sorting.SortDirections;
					}
					return null;
				}
			}

			protected override BTreeNode SortTree
			{
				get
				{
					if (this.m_userSortTargetInfo != null)
					{
						return this.m_userSortTargetInfo.SortTree;
					}
					if (base.m_grouping != null)
					{
						return base.m_grouping.Tree;
					}
					return null;
				}
				set
				{
					if (this.m_userSortTargetInfo != null)
					{
						this.m_userSortTargetInfo.SortTree = value;
					}
					else if (base.m_grouping != null)
					{
						base.m_grouping.Tree = value;
					}
					else
					{
						Global.Tracer.Assert(false);
					}
				}
			}

			protected override int ExpressionIndex
			{
				get
				{
					return 0;
				}
			}

			protected override IntList SortFilterInfoIndices
			{
				get
				{
					if (this.m_userSortTargetInfo != null)
					{
						return this.m_userSortTargetInfo.SortFilterInfoIndices;
					}
					return null;
				}
			}

			internal RuntimeTableDetailObj(IScope outerScope, Table tableDef, ref DataActions dataAction, ProcessingContext processingContext)
				: base(outerScope, tableDef, dataAction, processingContext)
			{
				bool flag = false;
				this.m_detailDef = tableDef.TableDetail;
				if (this.m_detailDef.RunningValues != null && 0 < this.m_detailDef.RunningValues.Count)
				{
					flag = true;
				}
				if (!flag && this.m_detailDef.DetailRows != null)
				{
					int num = 0;
					while (num < this.m_detailDef.DetailRows.Count)
					{
						if (this.m_detailDef.DetailRows[num].ReportItems.RunningValues == null || 0 >= this.m_detailDef.DetailRows[num].ReportItems.RunningValues.Count)
						{
							num++;
							continue;
						}
						flag = true;
						break;
					}
				}
				if (flag)
				{
					base.m_rvValueList = new DataAggregateObjResultsList();
				}
				base.HandleSortFilterEvent(ref this.m_userSortTargetInfo);
				if (this.m_userSortTargetInfo == null && this.m_detailDef.Sorting != null && 0 < this.m_detailDef.Sorting.SortExpressions.Count)
				{
					base.m_expression = new RuntimeExpressionInfo(this.m_detailDef.Sorting.SortExpressions, this.m_detailDef.Sorting.ExprHost, this.m_detailDef.Sorting.SortDirections, 0);
					base.m_grouping = new RuntimeGroupingObj(this, RuntimeGroupingObj.GroupingTypes.Sort);
				}
				dataAction = DataActions.None;
			}

			internal RuntimeTableDetailObj(RuntimeTableDetailObj detailRoot)
				: base(detailRoot)
			{
				this.m_detailDef = detailRoot.m_detailDef;
			}

			void ISortDataHolder.NextRow()
			{
				this.NextRow();
			}

			void ISortDataHolder.Traverse(ProcessingStages operation)
			{
				switch (operation)
				{
				case ProcessingStages.SortAndFilter:
					this.SortAndFilter();
					break;
				case ProcessingStages.RunningValues:
					base.ReadRows(DataActions.PostSortAggregates);
					break;
				case ProcessingStages.CreatingInstances:
					this.CreateInstance();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}

			internal override void NextRow()
			{
				if (this.m_userSortTargetInfo != null)
				{
					base.ProcessDetailSort(this.m_userSortTargetInfo);
				}
				else
				{
					base.NextRow();
				}
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				if (base.m_rvValueList == null && (base.m_outerDataAction & DataActions.PostSortAggregates) == DataActions.None)
				{
					return;
				}
				base.m_globalRunningValueCollection = globalRVCol;
				base.m_groupCollection = groupCol;
				if (this.m_detailDef != null)
				{
					base.AddRunningValues(this.m_detailDef.RunningValues, lastGroup);
					TableRowList detailRows = this.m_detailDef.DetailRows;
					if (detailRows != null)
					{
						for (int i = 0; i < detailRows.Count; i++)
						{
							RunningValueInfoList runningValues = null;
							if (detailRows[i].ReportItems != null)
							{
								runningValues = detailRows[i].ReportItems.RunningValues;
							}
							base.AddRunningValues(runningValues, lastGroup);
						}
					}
				}
				if (this.m_userSortTargetInfo != null)
				{
					bool sortDirection = base.m_processingContext.RuntimeSortFilterInfo[this.m_userSortTargetInfo.SortFilterInfoIndices[0]].SortDirection;
					this.m_userSortTargetInfo.SortTree.Traverse(ProcessingStages.RunningValues, sortDirection);
					this.m_userSortTargetInfo = null;
				}
				else
				{
					base.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				}
			}

			internal override void CreateInstances(ReportItemInstance riInstance, IList instanceList, RenderingPagesRangesList pagesList)
			{
				if (this.m_userSortTargetInfo != null)
				{
					base.m_reportItemInstance = riInstance;
					base.m_instanceList = instanceList;
					base.m_pagesList = pagesList;
					bool sortDirection = base.m_processingContext.RuntimeSortFilterInfo[this.m_userSortTargetInfo.SortFilterInfoIndices[0]].SortDirection;
					this.m_userSortTargetInfo.SortTree.Traverse(ProcessingStages.CreatingInstances, sortDirection);
					this.m_userSortTargetInfo = null;
				}
				else
				{
					base.CreateInstances(riInstance, instanceList, pagesList);
				}
			}

			internal override void CreateInstance()
			{
				if (this.m_detailDef != null)
				{
					TableRowList detailRows = this.m_detailDef.DetailRows;
					RuntimeTableDetailObj runtimeTableDetailObj = (RuntimeTableDetailObj)base.m_hierarchyRoot;
					TableInstance tableInstance = (TableInstance)runtimeTableDetailObj.m_reportItemInstance;
					Table table = (Table)tableInstance.ReportItemDef;
					IList instanceList = runtimeTableDetailObj.m_instanceList;
					if (base.m_dataRows != null)
					{
						DataActions dataActions = DataActions.None;
						RuntimeRICollectionList runtimeRICollectionList = new RuntimeRICollectionList(detailRows.Count);
						for (int i = 0; i < detailRows.Count; i++)
						{
							runtimeRICollectionList.Add(new RuntimeRICollection(this, detailRows[i].ReportItems, ref dataActions, base.m_processingContext, false));
						}
						base.m_processingContext.ChunkManager.EnterIgnorePageBreakItem();
						double num = -1.0;
						base.m_processingContext.Pagination.EnterIgnorePageBreak(detailRows[0].Visibility, false);
						for (int j = 0; j < base.m_dataRows.Count; j++)
						{
							int num2 = 0;
							base.SetupEnvironment(j, ref num2, this.m_detailDef.RunningValues, false);
							TableDetailInstance tableDetailInstance = new TableDetailInstance(base.m_processingContext, this.m_detailDef, (Table)base.m_dataRegionDef);
							base.m_processingContext.Pagination.ProcessTableDetails(table, tableDetailInstance, instanceList, ref num, detailRows, runtimeTableDetailObj.m_pagesList, ref runtimeTableDetailObj.m_numberOfChildrenOnThisPage);
							tableInstance.CurrentPage = table.CurrentPage;
							tableInstance.NumberOfChildrenOnThisPage = runtimeTableDetailObj.m_numberOfChildrenOnThisPage;
							base.m_processingContext.Pagination.EnterIgnorePageBreak(null, true);
							base.m_processingContext.Pagination.EnterIgnoreHeight(true);
							if (Report.ShowHideTypes.Interactive == base.m_processingContext.ShowHideType)
							{
								base.m_processingContext.EnterGrouping();
							}
							base.m_dataRegionDef.CurrentDetailRowIndex = j;
							for (int k = 0; k < detailRows.Count; k++)
							{
								ReportItemCollection reportItems = detailRows[k].ReportItems;
								base.SetupEnvironment(j, ref num2, reportItems.RunningValues, true);
								TableRowInstance tableRowInstance = tableDetailInstance.DetailRowInstances[k];
								if (Report.ShowHideTypes.Interactive == base.m_processingContext.ShowHideType)
								{
									((IShowHideContainer)tableRowInstance).BeginProcessContainer(base.m_processingContext);
								}
								base.m_processingContext.PageSectionContext.EnterVisibilityScope(detailRows[k].Visibility, detailRows[k].StartHidden);
								IntList tableColumnSpans = base.m_processingContext.PageSectionContext.TableColumnSpans;
								base.m_processingContext.PageSectionContext.TableColumnSpans = detailRows[k].ColSpans;
								runtimeRICollectionList[k].CreateInstances(tableRowInstance.TableRowReportItemColInstance);
								base.m_processingContext.PageSectionContext.TableColumnSpans = tableColumnSpans;
								base.m_processingContext.PageSectionContext.ExitVisibilityScope();
								if (Report.ShowHideTypes.Interactive == base.m_processingContext.ShowHideType)
								{
									((IShowHideContainer)tableRowInstance).EndProcessContainer(base.m_processingContext);
								}
							}
							if (Report.ShowHideTypes.Interactive == base.m_processingContext.ShowHideType)
							{
								base.m_processingContext.ExitGrouping();
							}
							base.m_processingContext.Pagination.LeaveIgnorePageBreak(null, true);
							base.m_processingContext.Pagination.LeaveIgnoreHeight(true);
							base.m_processingContext.Pagination.LeaveIgnoreHeight(this.m_detailDef.StartHidden);
							instanceList.Add(tableDetailInstance);
							runtimeRICollectionList.ResetReportItemObjs();
						}
						base.m_processingContext.Pagination.LeaveIgnorePageBreak(detailRows[0].Visibility, false);
						base.m_processingContext.ChunkManager.LeaveIgnorePageBreakItem();
					}
				}
			}

			protected override IHierarchyObj CreateHierarchyObj()
			{
				if (this.m_userSortTargetInfo != null)
				{
					return new RuntimeSortHierarchyObj(this);
				}
				return base.CreateHierarchyObj();
			}

			protected override void ProcessUserSort()
			{
				base.m_processingContext.ProcessUserSortForTarget((IHierarchyObj)this, ref base.m_dataRows, this.m_userSortTargetInfo.TargetForNonDetailSort);
			}

			protected override void MarkSortInfoProcessed(RuntimeSortFilterEventInfoList runtimeSortFilterInfo)
			{
				if (this.m_userSortTargetInfo != null)
				{
					this.m_userSortTargetInfo.MarkSortInfoProcessed(runtimeSortFilterInfo, this);
				}
			}

			protected override void AddSortInfoIndex(int sortInfoIndex, RuntimeSortFilterEventInfo sortInfo)
			{
				if (this.m_userSortTargetInfo != null)
				{
					this.m_userSortTargetInfo.AddSortInfoIndex(sortInfoIndex, sortInfo);
				}
			}
		}

		private sealed class RuntimeOnePassTableDetailObj : RuntimeOnePassDetailObj
		{
			private RuntimeRICollectionList m_reportItemCols;

			private TableDetailInstanceList m_tableDetailInstances;

			private Hashtable m_textboxColumnPositions;

			internal TableDetailInstanceList TableDetailInstances
			{
				get
				{
					return this.m_tableDetailInstances;
				}
			}

			internal Hashtable TextboxColumnPositions
			{
				get
				{
					return this.m_textboxColumnPositions;
				}
			}

			internal RuntimeOnePassTableDetailObj(IScope outerScope, Table tableDef, ProcessingContext processingContext)
				: base(outerScope, tableDef, processingContext)
			{
				TableDetail tableDetail = tableDef.TableDetail;
				if (tableDetail.RunningValues != null && 0 < tableDetail.RunningValues.Count)
				{
					base.AddRunningValues(tableDetail.RunningValues);
				}
				TableRowList detailRows = tableDetail.DetailRows;
				if (detailRows != null)
				{
					this.m_reportItemCols = new RuntimeRICollectionList(detailRows.Count);
					for (int i = 0; i < detailRows.Count; i++)
					{
						if (detailRows[i].ReportItems != null)
						{
							this.m_reportItemCols.Add(new RuntimeRICollection(this, detailRows[i].ReportItems, base.m_processingContext, false));
							base.AddRunningValues(detailRows[i].ReportItems.RunningValues);
						}
					}
				}
				this.m_tableDetailInstances = new TableDetailInstanceList();
				this.m_textboxColumnPositions = new Hashtable();
				tableDef.CurrentPage = 0;
			}

			internal override int GetDetailPage()
			{
				return ((Table)base.m_dataRegionDef).CurrentPage;
			}

			protected override void CreateInstance()
			{
				Table table = (Table)base.m_dataRegionDef;
				TableRowList detailRows = table.TableDetail.DetailRows;
				double num = -1.0;
				Pagination pagination = base.m_processingContext.Pagination;
				base.m_pagination.CopyPaginationInfo(pagination);
				base.m_processingContext.Pagination = base.m_pagination;
				NavigationInfo navigationInfo = base.m_processingContext.NavigationInfo;
				base.m_processingContext.NavigationInfo = base.m_navigationInfo;
				TableDetailInstance tableDetailInstance = new TableDetailInstance(base.m_processingContext, table.TableDetail, table);
				if (table.Visibility != null && table.Visibility.Toggle != null)
				{
					base.m_processingContext.Pagination.EnterIgnoreHeight(true);
				}
				base.m_processingContext.Pagination.ProcessTableDetails(table, tableDetailInstance, (IList)this.m_tableDetailInstances, ref num, detailRows, base.m_renderingPages, ref base.m_numberOfContentsOnThisPage);
				if (table.Visibility != null && table.Visibility.Toggle != null)
				{
					base.m_processingContext.Pagination.LeaveIgnoreHeight(true);
				}
				if (base.m_processingContext.ReportObjectModel.FieldsImpl.AddRowIndex)
				{
					base.m_dataRegionDef.CurrentDetailRowIndex = base.m_processingContext.ReportObjectModel.FieldsImpl.GetRowIndex();
				}
				RuntimeTableObj.CreateRowInstances(base.m_processingContext, this.m_reportItemCols, tableDetailInstance.DetailRowInstances, false, true);
				base.m_processingContext.Pagination = pagination;
				base.m_processingContext.NavigationInfo = navigationInfo;
				this.m_tableDetailInstances.Add(tableDetailInstance);
			}

			internal override bool IsVisible(string textboxName)
			{
				Global.Tracer.Assert(null != this.m_textboxColumnPositions, "(null != m_textboxColumnPositions)");
				object obj = this.m_textboxColumnPositions[textboxName];
				if (obj != null)
				{
					return base.m_processingContext.PageSectionContext.IsTableColumnVisible((TableColumnInfo)obj);
				}
				return false;
			}
		}

		private sealed class RuntimeOWCChartDetailObj : RuntimeDetailObj, IFilterOwner
		{
			private Filters m_filters;

			private DataAggregateObjList m_nonCustomAggregates;

			private DataAggregateObjList m_customAggregates;

			private DataAggregateObjList m_postSortAggregates;

			private DataAggregateObjList m_runningValues;

			private DataAggregateObjResult[] m_runningValueValues;

			protected override string ScopeName
			{
				get
				{
					return base.m_dataRegionDef.Name;
				}
			}

			internal RuntimeOWCChartDetailObj(IScope outerScope, OWCChart chartDef, ref DataActions dataAction, ProcessingContext processingContext)
				: base(outerScope, chartDef, (chartDef.Filters == null) ? dataAction : DataActions.None, processingContext)
			{
				RuntimeDataRegionObj.CreateAggregates(base.m_processingContext, chartDef.Aggregates, ref this.m_nonCustomAggregates, ref this.m_customAggregates);
				RuntimeDataRegionObj.CreateAggregates(base.m_processingContext, chartDef.PostSortAggregates, ref this.m_postSortAggregates);
				if (chartDef.Filters != null)
				{
					this.m_filters = new Filters(Filters.FilterTypes.DataRegionFilter, this, chartDef.Filters, chartDef.ObjectType, chartDef.Name, base.m_processingContext);
				}
				else
				{
					dataAction = DataActions.None;
				}
				RunningValueInfoList detailRunningValues = chartDef.DetailRunningValues;
				if (detailRunningValues != null && 0 < detailRunningValues.Count)
				{
					base.m_rvValueList = new DataAggregateObjResultsList();
				}
			}

			internal RuntimeOWCChartDetailObj(RuntimeOWCChartDetailObj detailRoot)
				: base(detailRoot)
			{
			}

			internal override void NextRow()
			{
				if (base.m_processingContext.ReportObjectModel.FieldsImpl.IsAggregateRow)
				{
					this.NextAggregateRow();
				}
				else
				{
					this.NextNonAggregateRow();
				}
			}

			private void NextNonAggregateRow()
			{
				bool flag = true;
				if (this.m_filters != null)
				{
					flag = this.m_filters.PassFilters(base.m_processingContext.ReportObjectModel.FieldsImpl.GetFields());
				}
				if (flag)
				{
					((IFilterOwner)this).PostFilterNextRow();
				}
			}

			private void NextAggregateRow()
			{
				if (base.m_processingContext.ReportObjectModel.FieldsImpl.AggregationFieldCount == 0)
				{
					RuntimeDataRegionObj.UpdateAggregates(base.m_processingContext, this.m_customAggregates, false);
				}
			}

			void IFilterOwner.PostFilterNextRow()
			{
				RuntimeDataRegionObj.UpdateAggregates(base.m_processingContext, this.m_nonCustomAggregates, false);
				base.NextRow();
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				OWCChart oWCChart = (OWCChart)base.m_dataRegionDef;
				RunningValueInfoList runningValues = oWCChart.RunningValues;
				if (base.m_rvValueList == null && this.m_postSortAggregates == null && (base.m_outerDataAction & DataActions.PostSortAggregates) == DataActions.None)
				{
					if (runningValues == null)
					{
						return;
					}
					if (runningValues.Count == 0)
					{
						return;
					}
				}
				base.m_globalRunningValueCollection = globalRVCol;
				base.m_groupCollection = groupCol;
				if (runningValues != null)
				{
					RuntimeDetailObj.AddRunningValues(base.m_processingContext, runningValues, ref this.m_runningValues, globalRVCol, groupCol, lastGroup);
				}
				runningValues = oWCChart.DetailRunningValues;
				if (runningValues != null && 0 < runningValues.Count)
				{
					base.AddRunningValues(runningValues, lastGroup);
				}
				base.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				RuntimeRICollection.DoneReadingRows(globalRVCol, oWCChart.RunningValues, ref this.m_runningValueValues, false);
			}

			internal override void ReadRow(DataActions dataAction)
			{
				if (DataActions.PostSortAggregates == dataAction)
				{
					if (this.m_postSortAggregates != null)
					{
						RuntimeDataRegionObj.UpdateAggregates(base.m_processingContext, this.m_postSortAggregates, false);
					}
					if (this.m_runningValues != null)
					{
						for (int i = 0; i < this.m_runningValues.Count; i++)
						{
							this.m_runningValues[i].Update();
						}
					}
				}
				base.ReadRow(dataAction);
			}

			internal override void CreateInstance()
			{
				if (base.m_dataRows != null)
				{
					OWCChart oWCChart = (OWCChart)base.m_dataRegionDef;
					RuntimeOWCChartDetailObj runtimeOWCChartDetailObj = (RuntimeOWCChartDetailObj)base.m_hierarchyRoot;
					OWCChartInstance oWCChartInstance = (OWCChartInstance)runtimeOWCChartDetailObj.m_reportItemInstance;
					for (int i = 0; i < base.m_dataRows.Count; i++)
					{
						base.SetupEnvironment(i, oWCChart.RunningValues);
						for (int num = oWCChart.ChartData.Count - 1; num >= 0; num--)
						{
							oWCChartInstance.InstanceInfo.ChartData[num].Add(base.m_processingContext.ReportRuntime.EvaluateOWCChartData(oWCChart, oWCChart.ChartData[num].Value));
						}
					}
				}
			}

			internal override void SetupEnvironment()
			{
				base.SetupEnvironment(this.m_nonCustomAggregates, this.m_customAggregates, (base.m_dataRows == null) ? null : base.m_dataRows[0]);
				base.SetupAggregates(this.m_postSortAggregates);
				base.SetupRunningValues(((OWCChart)base.m_dataRegionDef).RunningValues, this.m_runningValueValues);
			}

			internal override bool InScope(string scope)
			{
				return base.DataRegionInScope(base.m_dataRegionDef, scope);
			}

			protected override void GetGroupNameValuePairs(Dictionary<string, object> pairs)
			{
				base.DataRegionGetGroupNameValuePairs(base.m_dataRegionDef, pairs);
			}
		}

		internal abstract class RuntimePivotObj : RuntimeRDLDataRegionObj
		{
			protected Pivot m_pivotDef;

			protected RuntimePivotHeadingsObj m_pivotRows;

			protected RuntimePivotHeadingsObj m_pivotColumns;

			protected RuntimePivotHeadingsObj m_outerGroupings;

			protected RuntimePivotHeadingsObj m_innerGroupings;

			protected int[] m_outerGroupingCounters;

			protected override string ScopeName
			{
				get
				{
					return this.m_pivotDef.Name;
				}
			}

			protected override DataRegion DataRegionDef
			{
				get
				{
					return this.m_pivotDef;
				}
			}

			internal int[] OuterGroupingCounters
			{
				get
				{
					return this.m_outerGroupingCounters;
				}
			}

			internal RuntimePivotObj(IScope outerScope, Pivot pivotDef, ref DataActions dataAction, ProcessingContext processingContext, bool onePassProcess)
				: base(outerScope, (DataRegion)pivotDef, ref dataAction, processingContext, onePassProcess, pivotDef.RunningValues)
			{
				this.m_pivotDef = pivotDef;
			}

			protected void ConstructorHelper(ref DataActions dataAction, bool onePassProcess, out bool handleMyDataAction, out DataActions innerDataAction, out PivotHeading outermostColumn, out bool outermostColumnSubtotal, out PivotHeading staticColumn, out PivotHeading outermostRow, out bool outermostRowSubtotal, out PivotHeading staticRow)
			{
				this.m_pivotDef.GetHeadingDefState(out outermostColumn, out outermostColumnSubtotal, out staticColumn, out outermostRow, out outermostRowSubtotal, out staticRow);
				innerDataAction = base.m_dataAction;
				handleMyDataAction = false;
				bool flag = false;
				if (onePassProcess)
				{
					flag = true;
					RuntimeDataRegionObj.CreateAggregates(base.m_processingContext, this.m_pivotDef.PostSortAggregates, ref base.m_nonCustomAggregates);
					Global.Tracer.Assert(outermostRow == null && null == outermostColumn, "((null == outermostRow) && (null == outermostColumn))");
					RuntimeDataRegionObj.CreateAggregates(base.m_processingContext, this.m_pivotDef.CellPostSortAggregates, ref base.m_nonCustomAggregates);
					goto IL_0113;
				}
				if (this.m_pivotDef.PostSortAggregates != null)
				{
					RuntimeDataRegionObj.CreateAggregates(base.m_processingContext, this.m_pivotDef.PostSortAggregates, ref base.m_postSortAggregates);
					handleMyDataAction = true;
				}
				if (outermostRowSubtotal && outermostColumnSubtotal)
				{
					goto IL_00c4;
				}
				if (outermostRow == null && outermostColumn == null)
				{
					goto IL_00c4;
				}
				goto IL_00f2;
				IL_00f2:
				if (handleMyDataAction)
				{
					base.m_dataAction |= DataActions.PostSortAggregates;
					innerDataAction = DataActions.None;
				}
				else
				{
					innerDataAction = base.m_dataAction;
				}
				goto IL_0113;
				IL_0113:
				if (flag)
				{
					RuntimeDataRegionObj.CreateAggregates(base.m_processingContext, this.m_pivotDef.CellAggregates, ref base.m_nonCustomAggregates, ref base.m_customAggregates);
					RunningValueInfoList pivotCellRunningValues = this.m_pivotDef.PivotCellRunningValues;
					if (pivotCellRunningValues != null && 0 < pivotCellRunningValues.Count)
					{
						if (base.m_nonCustomAggregates == null)
						{
							base.m_nonCustomAggregates = new DataAggregateObjList();
						}
						for (int i = 0; i < pivotCellRunningValues.Count; i++)
						{
							base.m_nonCustomAggregates.Add(new DataAggregateObj(pivotCellRunningValues[i], base.m_processingContext));
						}
					}
				}
				int num = this.m_pivotDef.CreateOuterGroupingIndexList();
				this.m_outerGroupingCounters = new int[num];
				for (int j = 0; j < this.m_outerGroupingCounters.Length; j++)
				{
					this.m_outerGroupingCounters[j] = -1;
				}
				return;
				IL_00c4:
				flag = true;
				if (this.m_pivotDef.CellPostSortAggregates != null)
				{
					RuntimeDataRegionObj.CreateAggregates(base.m_processingContext, this.m_pivotDef.CellPostSortAggregates, ref base.m_postSortAggregates);
					handleMyDataAction = true;
				}
				goto IL_00f2;
			}

			protected void HandleDataAction(bool handleMyDataAction, DataActions innerDataAction, DataActions userSortDataAction)
			{
				if (!handleMyDataAction)
				{
					base.m_dataAction = innerDataAction;
				}
				base.m_dataAction |= userSortDataAction;
				if (base.m_dataAction != 0)
				{
					base.m_dataRows = new DataRowList();
				}
			}

			protected override void SendToInner()
			{
				this.m_pivotDef.RuntimeDataRegionObj = this;
				this.m_pivotDef.ResetOutergGroupingAggregateRowInfo();
				this.m_pivotDef.SavePivotAggregateRowInfo(base.m_processingContext);
				if (this.m_outerGroupings != null)
				{
					this.m_outerGroupings.NextRow();
				}
				this.m_pivotDef.RestorePivotAggregateRowInfo(base.m_processingContext);
				if (this.m_innerGroupings != null)
				{
					this.m_innerGroupings.NextRow();
				}
			}

			internal override bool SortAndFilter()
			{
				if (this.m_pivotRows != null)
				{
					this.m_pivotRows.SortAndFilter();
				}
				if (this.m_pivotColumns != null)
				{
					this.m_pivotColumns.SortAndFilter();
				}
				return base.SortAndFilter();
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				if (this.m_pivotDef.RunningValues != null && base.m_runningValues == null)
				{
					RuntimeDetailObj.AddRunningValues(base.m_processingContext, this.m_pivotDef.RunningValues, ref base.m_runningValues, globalRVCol, groupCol, lastGroup);
				}
				if (base.m_dataRows != null)
				{
					base.ReadRows(DataActions.PostSortAggregates);
					base.m_dataRows = null;
				}
				if (this.m_outerGroupings != null)
				{
					this.m_outerGroupings.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				}
				if (this.m_outerGroupings != null && this.m_outerGroupings.Headings != null)
				{
					return;
				}
				if (this.m_innerGroupings != null)
				{
					this.m_innerGroupings.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				}
			}

			protected virtual void CalculatePreviousAggregates()
			{
				if (!base.m_processedPreviousAggregates && base.m_processingContext.GlobalRVCollection != null)
				{
					Global.Tracer.Assert(null == base.m_runningValueValues, "(null == m_runningValueValues)");
					AggregatesImpl globalRVCollection = base.m_processingContext.GlobalRVCollection;
					RunningValueInfoList runningValues = this.m_pivotDef.RunningValues;
					RuntimeRICollection.DoneReadingRows(globalRVCollection, runningValues, ref base.m_runningValueValues, true);
					if (this.m_pivotRows != null)
					{
						this.m_pivotRows.CalculatePreviousAggregates(globalRVCollection);
					}
					if (this.m_pivotColumns != null)
					{
						this.m_pivotColumns.CalculatePreviousAggregates(globalRVCollection);
					}
					base.m_processedPreviousAggregates = true;
				}
			}

			internal override void ReadRow(DataActions dataAction)
			{
				if (DataActions.UserSort == dataAction)
				{
					RuntimeDataRegionObj.CommonFirstRow(base.m_processingContext.ReportObjectModel.FieldsImpl, ref base.m_firstRowIsAggregate, ref base.m_firstRow);
					base.CommonNextRow(base.m_dataRows);
				}
				else if (!this.m_pivotDef.ProcessCellRunningValues)
				{
					if (DataActions.PostSortAggregates == dataAction)
					{
						if (base.m_postSortAggregates != null)
						{
							RuntimeDataRegionObj.UpdateAggregates(base.m_processingContext, base.m_postSortAggregates, false);
						}
						if (base.m_runningValues != null)
						{
							for (int i = 0; i < base.m_runningValues.Count; i++)
							{
								base.m_runningValues[i].Update();
							}
						}
						this.CalculatePreviousAggregates();
					}
					if (base.m_outerScope != null && (dataAction & base.m_outerDataAction) != 0)
					{
						base.m_outerScope.ReadRow(dataAction);
					}
				}
			}

			internal override void SetupEnvironment()
			{
				base.SetupEnvironment(this.m_pivotDef.RunningValues);
			}
		}

		internal abstract class RuntimePivotHeadingsObj
		{
			protected IScope m_owner;

			protected RuntimePivotGroupRootObj m_pivotHeadings;

			protected PivotHeading m_staticHeadingDef;

			internal RuntimePivotGroupRootObj Headings
			{
				get
				{
					return this.m_pivotHeadings;
				}
			}

			internal RuntimePivotHeadingsObj(IScope owner, PivotHeading headingDef, ref DataActions dataAction, ProcessingContext processingContext, PivotHeading staticHeadingDef, RuntimePivotHeadingsObj innerGroupings, bool outermostHeadingSubtotal, int headingLevel)
			{
				this.m_owner = owner;
				if (staticHeadingDef != null)
				{
					this.m_staticHeadingDef = staticHeadingDef;
				}
			}

			internal virtual void NextRow()
			{
				if (this.m_pivotHeadings != null)
				{
					this.m_pivotHeadings.NextRow();
				}
			}

			internal virtual bool SortAndFilter()
			{
				if (this.m_pivotHeadings != null)
				{
					return this.m_pivotHeadings.SortAndFilter();
				}
				return true;
			}

			internal virtual void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				if (this.m_pivotHeadings != null)
				{
					this.m_pivotHeadings.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				}
			}

			internal abstract void CalculatePreviousAggregates(AggregatesImpl globalRVCol);
		}

		private sealed class RuntimeChartObj : RuntimePivotObj
		{
			private bool m_subtotalCorner;

			internal RuntimeChartObj(IScope outerScope, Chart chartDef, ref DataActions dataAction, ProcessingContext processingContext, bool onePassProcess)
				: base(outerScope, (Pivot)chartDef, ref dataAction, processingContext, onePassProcess)
			{
				bool handleMyDataAction = default(bool);
				DataActions innerDataAction = default(DataActions);
				PivotHeading pivotHeading = default(PivotHeading);
				bool flag = default(bool);
				PivotHeading staticColumn = default(PivotHeading);
				PivotHeading pivotHeading2 = default(PivotHeading);
				bool flag2 = default(bool);
				PivotHeading staticRow = default(PivotHeading);
				base.ConstructorHelper(ref dataAction, onePassProcess, out handleMyDataAction, out innerDataAction, out pivotHeading, out flag, out staticColumn, out pivotHeading2, out flag2, out staticRow);
				base.m_innerDataAction = innerDataAction;
				DataActions userSortDataAction = base.HandleSortFilterEvent();
				this.ChartConstructRuntimeStructure(ref innerDataAction, onePassProcess, pivotHeading, flag, staticColumn, pivotHeading2, flag2, staticRow);
				if (onePassProcess || (flag2 && flag) || (pivotHeading2 == null && pivotHeading == null))
				{
					this.m_subtotalCorner = true;
				}
				base.HandleDataAction(handleMyDataAction, innerDataAction, userSortDataAction);
			}

			protected override void ConstructRuntimeStructure(ref DataActions innerDataAction)
			{
				PivotHeading outermostColumn = default(PivotHeading);
				bool outermostColumnSubtotal = default(bool);
				PivotHeading staticColumn = default(PivotHeading);
				PivotHeading outermostRow = default(PivotHeading);
				bool outermostRowSubtotal = default(bool);
				PivotHeading staticRow = default(PivotHeading);
				base.m_pivotDef.GetHeadingDefState(out outermostColumn, out outermostColumnSubtotal, out staticColumn, out outermostRow, out outermostRowSubtotal, out staticRow);
				this.ChartConstructRuntimeStructure(ref innerDataAction, false, outermostColumn, outermostColumnSubtotal, staticColumn, outermostRow, outermostRowSubtotal, staticRow);
			}

			private void ChartConstructRuntimeStructure(ref DataActions innerDataAction, bool onePassProcess, PivotHeading outermostColumn, bool outermostColumnSubtotal, PivotHeading staticColumn, PivotHeading outermostRow, bool outermostRowSubtotal, PivotHeading staticRow)
			{
				DataActions dataActions = DataActions.None;
				if (base.m_pivotDef.ProcessingInnerGrouping == Pivot.ProcessingInnerGroupings.Column)
				{
					base.m_innerGroupings = (base.m_pivotColumns = new RuntimeChartHeadingsObj(this, (ChartHeading)outermostColumn, ref dataActions, base.m_processingContext, (ChartHeading)staticColumn, null, outermostRowSubtotal, 0));
					base.m_outerGroupings = (base.m_pivotRows = new RuntimeChartHeadingsObj((IScope)this, (ChartHeading)outermostRow, ref innerDataAction, base.m_processingContext, (ChartHeading)staticRow, (RuntimeChartHeadingsObj)base.m_innerGroupings, outermostColumnSubtotal, 0));
				}
				else
				{
					base.m_innerGroupings = (base.m_pivotRows = new RuntimeChartHeadingsObj(this, (ChartHeading)outermostRow, ref dataActions, base.m_processingContext, (ChartHeading)staticRow, null, outermostColumnSubtotal, 0));
					base.m_outerGroupings = (base.m_pivotColumns = new RuntimeChartHeadingsObj((IScope)this, (ChartHeading)outermostColumn, ref innerDataAction, base.m_processingContext, (ChartHeading)staticColumn, (RuntimeChartHeadingsObj)base.m_innerGroupings, outermostRowSubtotal, 0));
				}
			}

			internal override bool SortAndFilter()
			{
				this.SetupEnvironment();
				if (base.m_userSortTargetInfo != null)
				{
					base.m_userSortTargetInfo.EnterProcessUserSortPhase(base.m_processingContext);
				}
				base.SortAndFilter();
				if (base.m_userSortTargetInfo != null)
				{
					base.m_userSortTargetInfo.LeaveProcessUserSortPhase(base.m_processingContext);
				}
				return true;
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				base.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				RuntimeRICollection.DoneReadingRows(globalRVCol, base.m_pivotDef.RunningValues, ref base.m_runningValueValues, false);
			}

			internal override void CreateInstances(ReportItemInstance riInstance, IList instanceList, RenderingPagesRangesList pagesList)
			{
				if (base.m_firstRow != null)
				{
					Chart chart = (Chart)base.m_pivotDef;
					ChartInstance chartInstance = (ChartInstance)riInstance;
					if (base.m_outerGroupings == base.m_pivotRows)
					{
						chartInstance.InnerHeadingInstanceList = chartInstance.ColumnInstances;
						((RuntimeChartHeadingsObj)base.m_outerGroupings).CreateInstances(this, base.m_processingContext, chartInstance, true, null, chartInstance.RowInstances);
					}
					else
					{
						chartInstance.InnerHeadingInstanceList = chartInstance.RowInstances;
						((RuntimeChartHeadingsObj)base.m_outerGroupings).CreateInstances(this, base.m_processingContext, chartInstance, true, null, chartInstance.ColumnInstances);
					}
				}
			}

			internal void CreateOutermostSubtotalCells(ChartInstance chartInstance, bool outerGroupings)
			{
				if (outerGroupings)
				{
					this.SetupEnvironment();
					((RuntimeChartHeadingsObj)base.m_innerGroupings).CreateInstances(this, base.m_processingContext, chartInstance, false, null, chartInstance.InnerHeadingInstanceList);
				}
				else if (this.m_subtotalCorner)
				{
					this.SetupEnvironment();
					chartInstance.AddCell(base.m_processingContext, -1);
				}
			}
		}

		private sealed class RuntimeChartHeadingsObj : RuntimePivotHeadingsObj
		{
			private DataAggregateObjResult[] m_runningValueValues;

			internal RuntimeChartHeadingsObj(IScope owner, ChartHeading headingDef, ref DataActions dataAction, ProcessingContext processingContext, ChartHeading staticHeadingDef, RuntimeChartHeadingsObj innerGroupings, bool outermostHeadingSubtotal, int headingLevel)
				: base(owner, (PivotHeading)headingDef, ref dataAction, processingContext, (PivotHeading)staticHeadingDef, (RuntimePivotHeadingsObj)innerGroupings, outermostHeadingSubtotal, headingLevel)
			{
				if (headingDef != null)
				{
					base.m_pivotHeadings = new RuntimeChartGroupRootObj(owner, headingDef, ref dataAction, processingContext, innerGroupings, outermostHeadingSubtotal, headingLevel);
				}
			}

			internal override void CalculatePreviousAggregates(AggregatesImpl globalRVCol)
			{
				if (base.m_staticHeadingDef != null)
				{
					RuntimeRICollection.DoneReadingRows(globalRVCol, ((ChartHeading)base.m_staticHeadingDef).RunningValues, ref this.m_runningValueValues, true);
				}
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				base.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				if (base.m_staticHeadingDef != null && base.m_owner is RuntimeChartGroupLeafObj)
				{
					RuntimeRICollection.DoneReadingRows(globalRVCol, ((ChartHeading)base.m_staticHeadingDef).RunningValues, ref this.m_runningValueValues, false);
				}
			}

			private void SetupEnvironment(ProcessingContext processingContext)
			{
				if (base.m_staticHeadingDef != null && this.m_runningValueValues != null)
				{
					RuntimeDataRegionObj.SetupRunningValues(processingContext, ((ChartHeading)base.m_staticHeadingDef).RunningValues, this.m_runningValueValues);
				}
			}

			internal void CreateInstances(RuntimeDataRegionObj outerGroup, ProcessingContext processingContext, ChartInstance chartInstance, bool outerGroupings, RuntimePivotGroupRootObj currOuterHeadingGroupRoot, ChartHeadingInstanceList headingInstances)
			{
				bool flag = outerGroupings || 0 == chartInstance.CurrentCellOuterIndex;
				int num = 1;
				this.SetupEnvironment(processingContext);
				if (base.m_staticHeadingDef != null && ((ChartHeading)base.m_staticHeadingDef).Labels != null)
				{
					num = ((ChartHeading)base.m_staticHeadingDef).Labels.Count;
				}
				ChartHeadingInstanceList chartHeadingInstanceList = headingInstances;
				for (int i = 0; i < num; i++)
				{
					if (base.m_staticHeadingDef != null)
					{
						if (flag)
						{
							chartHeadingInstanceList = this.CreateHeadingInstance(processingContext, chartInstance, (ChartHeading)base.m_staticHeadingDef, headingInstances, outerGroupings, i);
						}
						if (outerGroupings)
						{
							chartInstance.CurrentOuterStaticIndex = i;
						}
						else
						{
							chartInstance.CurrentInnerStaticIndex = i;
						}
					}
					if (base.m_pivotHeadings != null)
					{
						Chart chart = (Chart)base.m_pivotHeadings.HierarchyDef.DataRegionDef;
						chart.CurrentOuterHeadingGroupRoot = currOuterHeadingGroupRoot;
						base.m_pivotHeadings.CreateInstances(chartInstance, chartHeadingInstanceList, null);
						if (flag)
						{
							this.SetHeadingSpan(chartInstance, chartHeadingInstanceList, outerGroupings, processingContext);
						}
					}
					else if (outerGroup is RuntimeChartGroupLeafObj)
					{
						RuntimeChartGroupLeafObj runtimeChartGroupLeafObj = (RuntimeChartGroupLeafObj)outerGroup;
						if (!outerGroupings && runtimeChartGroupLeafObj.IsOuterGrouping())
						{
							runtimeChartGroupLeafObj.CreateSubtotalOrStaticCells(chartInstance, currOuterHeadingGroupRoot, outerGroupings);
						}
						else
						{
							runtimeChartGroupLeafObj.CreateInnerGroupingsOrCells(chartInstance, currOuterHeadingGroupRoot);
						}
					}
					else
					{
						((RuntimeChartObj)outerGroup).CreateOutermostSubtotalCells(chartInstance, outerGroupings);
					}
				}
				if (base.m_staticHeadingDef != null && flag)
				{
					this.SetHeadingSpan(chartInstance, headingInstances, outerGroupings, processingContext);
				}
			}

			private void SetHeadingSpan(ChartInstance chartInstance, ChartHeadingInstanceList headingInstances, bool outerGroupings, ProcessingContext processingContext)
			{
				int currentCellIndex = (!outerGroupings) ? chartInstance.CurrentCellInnerIndex : (chartInstance.CurrentCellOuterIndex + 1);
				headingInstances.SetLastHeadingSpan(currentCellIndex, processingContext);
			}

			private ChartHeadingInstanceList CreateHeadingInstance(ProcessingContext processingContext, ChartInstance chartInstance, ChartHeading headingDef, ChartHeadingInstanceList headingInstances, bool outerGroupings, int labelIndex)
			{
				ChartHeadingInstance chartHeadingInstance = null;
				int headingCellIndex;
				if (outerGroupings)
				{
					chartInstance.NewOuterCells();
					headingCellIndex = chartInstance.CurrentCellOuterIndex;
				}
				else
				{
					headingCellIndex = chartInstance.CurrentCellInnerIndex;
				}
				chartHeadingInstance = new ChartHeadingInstance(processingContext, headingCellIndex, headingDef, labelIndex, null);
				headingInstances.Add(chartHeadingInstance, processingContext);
				return chartHeadingInstance.SubHeadingInstances;
			}
		}

		internal abstract class RuntimePivotGroupRootObj : RuntimeGroupRootObj
		{
			protected RuntimePivotHeadingsObj m_innerGroupings;

			protected PivotHeading m_staticHeadingDef;

			protected bool m_outermostSubtotal;

			protected PivotHeading m_innerHeading;

			protected Subtotal m_innerSubtotal;

			protected PivotHeading m_innerSubtotalStaticHeading;

			protected bool m_processOutermostSTCells;

			protected DataAggregateObjList m_outermostSTCellRVs;

			protected DataAggregateObjList m_cellRVs;

			protected int m_headingLevel;

			internal RuntimePivotHeadingsObj InnerGroupings
			{
				get
				{
					return this.m_innerGroupings;
				}
			}

			internal PivotHeading StaticHeadingDef
			{
				get
				{
					return this.m_staticHeadingDef;
				}
			}

			internal bool OutermostSubtotal
			{
				get
				{
					return this.m_outermostSubtotal;
				}
			}

			internal PivotHeading InnerHeading
			{
				get
				{
					return this.m_innerHeading;
				}
			}

			internal bool ProcessOutermostSTCells
			{
				get
				{
					return this.m_processOutermostSTCells;
				}
			}

			internal AggregatesImpl OutermostSTCellRVCol
			{
				get
				{
					return ((PivotHeading)base.m_hierarchyDef).OutermostSTCellRVCol;
				}
			}

			internal AggregatesImpl[] OutermostSTScopedCellRVCollections
			{
				get
				{
					return ((PivotHeading)base.m_hierarchyDef).OutermostSTCellScopedRVCollections;
				}
			}

			internal AggregatesImpl CellRVCol
			{
				get
				{
					return ((PivotHeading)base.m_hierarchyDef).CellRVCol;
				}
			}

			internal AggregatesImpl[] CellScopedRVCollections
			{
				get
				{
					return ((PivotHeading)base.m_hierarchyDef).CellScopedRVCollections;
				}
			}

			internal int HeadingLevel
			{
				get
				{
					return this.m_headingLevel;
				}
			}

			internal RuntimePivotGroupRootObj(IScope outerScope, PivotHeading pivotHeadingDef, ref DataActions dataAction, ProcessingContext processingContext, RuntimePivotHeadingsObj innerGroupings, bool outermostSubtotal, int headingLevel)
				: base(outerScope, pivotHeadingDef, dataAction, processingContext)
			{
				Pivot pivot = (Pivot)pivotHeadingDef.DataRegionDef;
				this.m_innerHeading = (PivotHeading)pivotHeadingDef.InnerHierarchy;
				pivot.SkipStaticHeading(ref this.m_innerHeading, ref this.m_staticHeadingDef);
				if (this.m_innerHeading != null)
				{
					this.m_innerSubtotal = this.m_innerHeading.Subtotal;
					if (this.m_innerSubtotal != null)
					{
						this.m_innerSubtotalStaticHeading = this.m_innerHeading.GetInnerStaticHeading();
					}
				}
				if (outermostSubtotal && (this.m_innerHeading == null || this.m_innerSubtotal != null))
				{
					this.m_processOutermostSTCells = true;
					if (pivot.CellPostSortAggregates != null)
					{
						base.m_dataAction |= DataActions.PostSortAggregates;
					}
				}
				this.NeedProcessDataActions(pivotHeadingDef);
				this.NeedProcessDataActions(this.m_staticHeadingDef);
				if ((base.m_dataAction & DataActions.PostSortAggregates) == DataActions.None && this.m_innerSubtotal != null && this.m_innerSubtotal.ReportItems.RunningValues != null && 0 < this.m_innerSubtotal.ReportItems.RunningValues.Count)
				{
					base.m_dataAction |= DataActions.PostSortAggregates;
				}
				this.NeedProcessDataActions(this.m_innerSubtotalStaticHeading);
				this.m_outermostSubtotal = outermostSubtotal;
				this.m_innerGroupings = innerGroupings;
				this.m_headingLevel = headingLevel;
				if (pivotHeadingDef.Grouping.Filters == null)
				{
					dataAction = DataActions.None;
				}
			}

			protected abstract void NeedProcessDataActions(PivotHeading heading);

			protected void NeedProcessDataActions(RunningValueInfoList runningValues)
			{
				if ((base.m_dataAction & DataActions.PostSortAggregates) == DataActions.None && runningValues != null && 0 < runningValues.Count)
				{
					base.m_dataAction |= DataActions.PostSortAggregates;
				}
			}

			internal override bool SortAndFilter()
			{
				Pivot pivot = (Pivot)base.m_hierarchyDef.DataRegionDef;
				PivotHeading pivotHeading = (PivotHeading)base.m_hierarchyDef;
				if ((ProcessingContext.SecondPassOperations.Filtering & base.m_processingContext.SecondPassOperation) != 0 && base.m_hierarchyDef.Grouping.Filters == null)
				{
					if (pivotHeading.IsColumn && this.m_headingLevel < pivot.InnermostColumnFilterLevel)
					{
						goto IL_006a;
					}
					if (!pivotHeading.IsColumn && this.m_headingLevel < pivot.InnermostRowFilterLevel)
					{
						goto IL_006a;
					}
				}
				goto IL_0076;
				IL_006a:
				pivotHeading.Grouping.HasInnerFilters = true;
				goto IL_0076;
				IL_0076:
				bool result = base.SortAndFilter();
				pivotHeading.Grouping.HasInnerFilters = false;
				return result;
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				base.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				Pivot pivot = (Pivot)base.m_hierarchyDef.DataRegionDef;
				PivotHeading pivotHeading = (PivotHeading)base.m_hierarchyDef;
				AggregatesImpl outermostSTCellRVCol = pivotHeading.OutermostSTCellRVCol;
				AggregatesImpl[] outermostSTCellScopedRVCollections = pivotHeading.OutermostSTCellScopedRVCollections;
				if (this.SetupCellRunningValues(ref outermostSTCellRVCol, ref outermostSTCellScopedRVCollections))
				{
					pivotHeading.OutermostSTCellRVCol = outermostSTCellRVCol;
					pivotHeading.OutermostSTCellScopedRVCollections = outermostSTCellScopedRVCollections;
				}
				if (this.m_processOutermostSTCells)
				{
					if (this.m_innerGroupings != null)
					{
						pivot.CurrentOuterHeadingGroupRoot = this;
					}
					base.m_processingContext.EnterPivotCell(null != this.m_innerGroupings);
					pivot.ProcessOutermostSTCellRunningValues = true;
					this.AddCellRunningValues(outermostSTCellRVCol, groupCol, ref this.m_outermostSTCellRVs);
					pivot.ProcessOutermostSTCellRunningValues = false;
					base.m_processingContext.ExitPivotCell();
				}
				if (this.m_innerGroupings != null)
				{
					AggregatesImpl cellRVCol = pivotHeading.CellRVCol;
					AggregatesImpl[] cellScopedRVCollections = pivotHeading.CellScopedRVCollections;
					if (this.SetupCellRunningValues(ref cellRVCol, ref cellScopedRVCollections))
					{
						pivotHeading.CellRVCol = cellRVCol;
						pivotHeading.CellScopedRVCollections = cellScopedRVCollections;
					}
				}
				else
				{
					RuntimePivotGroupRootObj currentOuterHeadingGroupRoot = pivot.CurrentOuterHeadingGroupRoot;
					if (this.m_innerHeading == null && currentOuterHeadingGroupRoot != null)
					{
						base.m_processingContext.EnterPivotCell(true);
						pivot.ProcessCellRunningValues = true;
						this.m_cellRVs = null;
						this.AddCellRunningValues(currentOuterHeadingGroupRoot.CellRVCol, groupCol, ref this.m_cellRVs);
						pivot.ProcessCellRunningValues = false;
						base.m_processingContext.ExitPivotCell();
					}
				}
			}

			private bool SetupCellRunningValues(ref AggregatesImpl globalCellRVCol, ref AggregatesImpl[] cellScopedRVLists)
			{
				if (globalCellRVCol != null && cellScopedRVLists != null)
				{
					return false;
				}
				globalCellRVCol = new AggregatesImpl(base.m_processingContext.ReportRuntime);
				cellScopedRVLists = this.CreateScopedCellRVCollections();
				return true;
			}

			protected abstract void AddCellRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, ref DataAggregateObjList runningValues);

			internal override void AddScopedRunningValue(DataAggregateObj runningValueObj, bool escalate)
			{
				Pivot pivot = (Pivot)base.m_hierarchyDef.DataRegionDef;
				if (pivot.ProcessOutermostSTCellRunningValues || pivot.ProcessCellRunningValues)
				{
					RuntimePivotGroupRootObj currentOuterHeadingGroupRoot = pivot.CurrentOuterHeadingGroupRoot;
					int headingLevel = currentOuterHeadingGroupRoot.HeadingLevel;
					PivotHeading pivotHeading = (!escalate) ? ((PivotHeading)base.m_hierarchyDef) : ((PivotHeading)currentOuterHeadingGroupRoot.HierarchyDef);
					if (pivot.ProcessOutermostSTCellRunningValues)
					{
						this.AddCellScopedRunningValue(runningValueObj, pivotHeading.OutermostSTCellScopedRVCollections, headingLevel);
					}
					else if (pivot.ProcessCellRunningValues)
					{
						this.AddCellScopedRunningValue(runningValueObj, pivotHeading.CellScopedRVCollections, headingLevel);
					}
				}
				else
				{
					base.AddScopedRunningValue(runningValueObj, escalate);
				}
			}

			private void AddCellScopedRunningValue(DataAggregateObj runningValueObj, AggregatesImpl[] cellScopedRVLists, int currentOuterHeadingLevel)
			{
				if (cellScopedRVLists != null)
				{
					AggregatesImpl aggregatesImpl = cellScopedRVLists[currentOuterHeadingLevel];
					if (aggregatesImpl == null)
					{
						aggregatesImpl = (cellScopedRVLists[currentOuterHeadingLevel] = new AggregatesImpl(base.m_processingContext.ReportRuntime));
					}
					if (aggregatesImpl.GetAggregateObj(runningValueObj.Name) == null)
					{
						aggregatesImpl.Add(runningValueObj);
					}
				}
			}

			internal override void ReadRow(DataActions dataAction)
			{
				Pivot pivot = (Pivot)base.m_hierarchyDef.DataRegionDef;
				if (pivot.ProcessCellRunningValues)
				{
					Global.Tracer.Assert(DataActions.PostSortAggregates == dataAction, "(DataActions.PostSortAggregates == dataAction)");
					if (this.m_cellRVs != null)
					{
						for (int i = 0; i < this.m_cellRVs.Count; i++)
						{
							this.m_cellRVs[i].Update();
						}
					}
					if (base.m_outerScope != null && pivot.CellPostSortAggregates != null)
					{
						base.m_outerScope.ReadRow(dataAction);
					}
				}
				else
				{
					if (DataActions.PostSortAggregates == dataAction && this.m_outermostSTCellRVs != null)
					{
						for (int j = 0; j < this.m_outermostSTCellRVs.Count; j++)
						{
							this.m_outermostSTCellRVs[j].Update();
						}
					}
					base.ReadRow(dataAction);
				}
			}

			private AggregatesImpl[] CreateScopedCellRVCollections()
			{
				Pivot pivot = (Pivot)base.m_hierarchyDef.DataRegionDef;
				int dynamicHeadingCount = pivot.GetDynamicHeadingCount(true);
				if (0 < dynamicHeadingCount)
				{
					return new AggregatesImpl[dynamicHeadingCount];
				}
				return null;
			}

			internal bool GetCellTargetForNonDetailSort()
			{
				if (base.m_outerScope is RuntimePivotObj)
				{
					return base.m_outerScope.TargetForNonDetailSort;
				}
				return ((RuntimePivotGroupLeafObj)base.m_outerScope).GetCellTargetForNonDetailSort();
			}

			internal bool GetCellTargetForSort(int index, bool detailSort)
			{
				if (base.m_outerScope is RuntimePivotObj)
				{
					return base.m_outerScope.IsTargetForSort(index, detailSort);
				}
				return ((RuntimePivotGroupLeafObj)base.m_outerScope).GetCellTargetForSort(index, detailSort);
			}
		}

		private sealed class RuntimeChartGroupRootObj : RuntimePivotGroupRootObj
		{
			internal RuntimeChartGroupRootObj(IScope outerScope, ChartHeading chartHeadingDef, ref DataActions dataAction, ProcessingContext processingContext, RuntimeChartHeadingsObj innerGroupings, bool outermostSubtotal, int headingLevel)
				: base(outerScope, (PivotHeading)chartHeadingDef, ref dataAction, processingContext, (RuntimePivotHeadingsObj)innerGroupings, outermostSubtotal, headingLevel)
			{
				if (base.m_processOutermostSTCells)
				{
					Chart chart = (Chart)chartHeadingDef.DataRegionDef;
					if (chart.CellRunningValues != null && 0 < chart.CellRunningValues.Count)
					{
						base.m_dataAction |= DataActions.PostSortAggregates;
					}
				}
				if (chartHeadingDef.ChartGroupExpression)
				{
					base.m_saveGroupExprValues = true;
				}
			}

			protected override void NeedProcessDataActions(PivotHeading heading)
			{
				ChartHeading chartHeading = (ChartHeading)heading;
				if (chartHeading != null)
				{
					base.NeedProcessDataActions(chartHeading.RunningValues);
				}
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				base.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				base.AddRunningValues(((ChartHeading)base.m_hierarchyDef).RunningValues);
				if (base.m_staticHeadingDef != null)
				{
					base.AddRunningValues(((ChartHeading)base.m_staticHeadingDef).RunningValues);
				}
				base.m_grouping.Traverse(ProcessingStages.RunningValues, base.m_expression.Direction);
				if (base.m_hierarchyDef.Grouping.Name != null)
				{
					groupCol.Remove(base.m_hierarchyDef.Grouping.Name);
				}
			}

			protected override void AddCellRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, ref DataAggregateObjList runningValues)
			{
				Chart chart = (Chart)base.m_hierarchyDef.DataRegionDef;
				if (chart.CellRunningValues != null && 0 < chart.CellRunningValues.Count)
				{
					base.m_dataAction |= DataActions.PostSortAggregates;
					if (runningValues == null)
					{
						base.AddRunningValues(chart.CellRunningValues, ref runningValues, globalRVCol, groupCol);
					}
				}
			}
		}

		internal abstract class RuntimePivotCell : IScope
		{
			protected RuntimePivotGroupLeafObj m_owner;

			protected int m_cellLevel;

			protected DataAggregateObjList m_cellNonCustomAggObjs;

			protected DataAggregateObjList m_cellCustomAggObjs;

			protected DataAggregateObjResult[] m_cellAggValueList;

			protected DataRowList m_dataRows;

			protected bool m_innermost;

			protected FieldImpl[] m_firstRow;

			protected bool m_firstRowIsAggregate;

			protected RuntimePivotCell m_nextCell;

			protected int[] m_sortFilterExpressionScopeInfoIndices;

			internal RuntimePivotCell NextCell
			{
				get
				{
					return this.m_nextCell;
				}
				set
				{
					this.m_nextCell = value;
				}
			}

			bool IScope.TargetForNonDetailSort
			{
				get
				{
					return this.m_owner.GetCellTargetForNonDetailSort();
				}
			}

			int[] IScope.SortFilterExpressionScopeInfoIndices
			{
				get
				{
					if (this.m_sortFilterExpressionScopeInfoIndices == null)
					{
						this.m_sortFilterExpressionScopeInfoIndices = new int[this.m_owner.ProcessingContext.RuntimeSortFilterInfo.Count];
						for (int i = 0; i < this.m_owner.ProcessingContext.RuntimeSortFilterInfo.Count; i++)
						{
							this.m_sortFilterExpressionScopeInfoIndices[i] = -1;
						}
					}
					return this.m_sortFilterExpressionScopeInfoIndices;
				}
			}

			internal RuntimePivotCell(RuntimePivotGroupLeafObj owner, int cellLevel, DataAggregateInfoList aggDefs, bool innermost)
			{
				this.m_owner = owner;
				this.m_cellLevel = cellLevel;
				RuntimeDataRegionObj.CreateAggregates(owner.ProcessingContext, aggDefs, ref this.m_cellNonCustomAggObjs, ref this.m_cellCustomAggObjs);
				DataAggregateObjList cellPostSortAggregates = this.m_owner.CellPostSortAggregates;
				if (cellPostSortAggregates != null && 0 < cellPostSortAggregates.Count)
				{
					this.m_cellAggValueList = new DataAggregateObjResult[cellPostSortAggregates.Count];
				}
				this.m_innermost = innermost;
			}

			internal virtual void NextRow()
			{
				RuntimeDataRegionObj.CommonFirstRow(this.m_owner.ProcessingContext.ReportObjectModel.FieldsImpl, ref this.m_firstRowIsAggregate, ref this.m_firstRow);
				this.NextAggregateRow();
				if (!this.m_owner.ProcessingContext.ReportObjectModel.FieldsImpl.IsAggregateRow)
				{
					this.NextNonAggregateRow();
				}
			}

			private void NextNonAggregateRow()
			{
				RuntimeDataRegionObj.UpdateAggregates(this.m_owner.ProcessingContext, this.m_cellNonCustomAggObjs, false);
				if (this.m_dataRows != null)
				{
					RuntimeDetailObj.SaveData(this.m_dataRows, this.m_owner.ProcessingContext);
				}
			}

			private void NextAggregateRow()
			{
				FieldsImpl fieldsImpl = this.m_owner.ProcessingContext.ReportObjectModel.FieldsImpl;
				if (fieldsImpl.ValidAggregateRow && fieldsImpl.AggregationFieldCount == 0 && this.m_cellCustomAggObjs != null)
				{
					RuntimeDataRegionObj.UpdateAggregates(this.m_owner.ProcessingContext, this.m_cellCustomAggObjs, false);
				}
			}

			internal virtual void SortAndFilter()
			{
			}

			internal virtual void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				if (this.m_dataRows != null)
				{
					Global.Tracer.Assert(this.m_innermost, "(m_innermost)");
					this.ReadRows();
					this.m_dataRows = null;
				}
				DataAggregateObjList cellPostSortAggregates = this.m_owner.CellPostSortAggregates;
				if (cellPostSortAggregates != null && 0 < cellPostSortAggregates.Count)
				{
					for (int i = 0; i < cellPostSortAggregates.Count; i++)
					{
						this.m_cellAggValueList[i] = cellPostSortAggregates[i].AggregateResult();
						cellPostSortAggregates[i].Init();
					}
				}
			}

			private void ReadRows()
			{
				for (int i = 0; i < this.m_dataRows.Count; i++)
				{
					FieldImpl[] fields = this.m_dataRows[i];
					this.m_owner.ProcessingContext.ReportObjectModel.FieldsImpl.SetFields(fields);
					((IScope)this).ReadRow(DataActions.PostSortAggregates);
				}
			}

			protected void SetupAggregates(DataAggregateObjList aggregates, DataAggregateObjResult[] aggValues)
			{
				if (aggregates != null)
				{
					for (int i = 0; i < aggregates.Count; i++)
					{
						DataAggregateObj dataAggregateObj = aggregates[i];
						this.m_owner.ProcessingContext.ReportObjectModel.AggregatesImpl.Set(dataAggregateObj.Name, dataAggregateObj.AggregateDef, dataAggregateObj.DuplicateNames, (aggValues == null) ? dataAggregateObj.AggregateResult() : aggValues[i]);
					}
				}
			}

			protected void SetupEnvironment()
			{
				this.SetupAggregates(this.m_cellNonCustomAggObjs, null);
				this.SetupAggregates(this.m_cellCustomAggObjs, null);
				this.SetupAggregates(this.m_owner.CellPostSortAggregates, this.m_cellAggValueList);
				this.m_owner.ProcessingContext.ReportObjectModel.FieldsImpl.SetFields(this.m_firstRow);
				this.m_owner.ProcessingContext.ReportRuntime.CurrentScope = this;
			}

			private Hashtable GetOuterScopeNames()
			{
				PivotHeading pivotHeadingDef = this.m_owner.PivotHeadingDef;
				Pivot pivot = (Pivot)pivotHeadingDef.DataRegionDef;
				Hashtable hashtable = null;
				if (pivotHeadingDef.CellScopeNames == null)
				{
					pivotHeadingDef.CellScopeNames = new Hashtable[pivot.GetDynamicHeadingCount(true)];
				}
				else
				{
					hashtable = pivotHeadingDef.CellScopeNames[this.m_cellLevel];
				}
				if (hashtable == null)
				{
					hashtable = pivot.GetOuterScopeNames(this.m_cellLevel);
					pivotHeadingDef.CellScopeNames[this.m_cellLevel] = hashtable;
				}
				return hashtable;
			}

			bool IScope.IsTargetForSort(int index, bool detailSort)
			{
				return this.m_owner.GetCellTargetForSort(index, detailSort);
			}

			string IScope.GetScopeName()
			{
				return null;
			}

			IScope IScope.GetOuterScope(bool includeSubReportContainingScope)
			{
				return this.m_owner;
			}

			void IScope.ReadRow(DataActions dataAction)
			{
				this.m_owner.ReadRow(dataAction);
			}

			bool IScope.InScope(string scope)
			{
				if (this.m_owner.InScope(scope))
				{
					return true;
				}
				Hashtable outerScopeNames = this.GetOuterScopeNames();
				return outerScopeNames.Contains(scope);
			}

			int IScope.RecursiveLevel(string scope)
			{
				if (scope == null)
				{
					return 0;
				}
				int num = ((IScope)this.m_owner).RecursiveLevel(scope);
				if (-1 != num)
				{
					return num;
				}
				Hashtable outerScopeNames = this.GetOuterScopeNames();
				Grouping grouping = outerScopeNames[scope] as Grouping;
				if (grouping != null)
				{
					return grouping.RecursiveLevel;
				}
				return -1;
			}

			bool IScope.TargetScopeMatched(int index, bool detailSort)
			{
				if (!this.m_owner.TargetScopeMatched(index, detailSort))
				{
					return false;
				}
				Hashtable outerScopeNames = this.GetOuterScopeNames();
				IDictionaryEnumerator enumerator = outerScopeNames.GetEnumerator();
				while (enumerator.MoveNext())
				{
					Grouping grouping = (Grouping)enumerator.Value;
					if ((!detailSort || grouping.SortFilterScopeInfo != null) && grouping.SortFilterScopeMatched != null && !grouping.SortFilterScopeMatched[index])
					{
						return false;
					}
				}
				if (detailSort)
				{
					return true;
				}
				Pivot pivotDef = this.m_owner.PivotDef;
				RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = this.m_owner.ProcessingContext.RuntimeSortFilterInfo[index];
				VariantList[] sortSourceScopeInfo = runtimeSortFilterEventInfo.SortSourceScopeInfo;
				if (this.m_owner.GroupingDef.SortFilterScopeIndex != null && -1 != this.m_owner.GroupingDef.SortFilterScopeIndex[index])
				{
					int num = this.m_owner.GroupingDef.SortFilterScopeIndex[index] + 1;
					if (!this.m_innermost)
					{
						int dynamicHeadingCount = pivotDef.GetDynamicHeadingCount(false);
						int num2 = this.m_owner.HeadingLevel + 1;
						while (num2 < dynamicHeadingCount && num < sortSourceScopeInfo.Length)
						{
							if (sortSourceScopeInfo[num] != null)
							{
								return false;
							}
							num2++;
							num++;
						}
					}
				}
				PivotHeading outerHeading = pivotDef.GetOuterHeading(this.m_cellLevel + 1);
				if (outerHeading != null && outerHeading.Grouping.SortFilterScopeIndex != null && -1 != outerHeading.Grouping.SortFilterScopeIndex[index])
				{
					int dynamicHeadingCount2 = pivotDef.GetDynamicHeadingCount(true);
					int num = outerHeading.Grouping.SortFilterScopeIndex[index];
					int num3 = this.m_cellLevel + 1;
					while (num3 < dynamicHeadingCount2 && num < sortSourceScopeInfo.Length)
					{
						if (sortSourceScopeInfo[num] != null)
						{
							return false;
						}
						num3++;
						num++;
					}
				}
				return true;
			}

			void IScope.GetScopeValues(IHierarchyObj targetScopeObj, VariantList[] scopeValues, ref int index)
			{
				Pivot pivotDef = this.m_owner.PivotDef;
				((RuntimeDataRegionObj)this.m_owner).GetScopeValues(targetScopeObj, scopeValues, ref index);
				int dynamicHeadingCount;
				if (!this.m_innermost)
				{
					dynamicHeadingCount = pivotDef.GetDynamicHeadingCount(false);
					for (int i = this.m_owner.HeadingLevel + 1; i < dynamicHeadingCount; i++)
					{
						Global.Tracer.Assert(index < scopeValues.Length, "Subtotal inner headings");
						scopeValues[index++] = null;
					}
				}
				Hashtable outerScopeNames = this.GetOuterScopeNames();
				IDictionaryEnumerator enumerator = outerScopeNames.GetEnumerator();
				while (enumerator.MoveNext())
				{
					Grouping grouping = (Grouping)enumerator.Value;
					Global.Tracer.Assert(index < scopeValues.Length, "Inner headings");
					scopeValues[index++] = grouping.CurrentGroupExpressionValues;
				}
				dynamicHeadingCount = pivotDef.GetDynamicHeadingCount(true);
				for (int j = outerScopeNames.Count; j < dynamicHeadingCount; j++)
				{
					Global.Tracer.Assert(index < scopeValues.Length, "Outer headings");
					scopeValues[index++] = null;
				}
			}

			void IScope.GetGroupNameValuePairs(Dictionary<string, object> pairs)
			{
				((IScope)this.m_owner).GetGroupNameValuePairs(pairs);
				Hashtable outerScopeNames = this.GetOuterScopeNames();
				if (outerScopeNames != null)
				{
					IEnumerator enumerator = outerScopeNames.Values.GetEnumerator();
					while (enumerator.MoveNext())
					{
						RuntimeDataRegionObj.AddGroupNameValuePair(this.m_owner.ProcessingContext, enumerator.Current as Grouping, pairs);
					}
				}
			}
		}

		private sealed class RuntimeChartCell : RuntimePivotCell
		{
			private DataAggregateObjResult[] m_runningValueValues;

			internal RuntimeChartCell(RuntimeChartGroupLeafObj owner, int cellLevel, DataAggregateInfoList aggDefs, ChartDataPointList cellDef, bool innermost)
				: base(owner, cellLevel, aggDefs, innermost)
			{
				Chart chart = (Chart)owner.PivotDef;
				DataActions dataActions = DataActions.None;
				bool flag = chart.CellRunningValues != null && 0 < chart.CellRunningValues.Count;
				if (base.m_innermost && (flag || base.m_owner.CellPostSortAggregates != null))
				{
					dataActions = DataActions.PostSortAggregates;
				}
				if (dataActions != 0)
				{
					base.m_dataRows = new DataRowList();
				}
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				base.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				RuntimeRICollection.DoneReadingRows(globalRVCol, ((Chart)base.m_owner.PivotDef).CellRunningValues, ref this.m_runningValueValues, false);
			}

			internal void CreateInstance(ChartInstance chartInstance)
			{
				int currentCellDPIndex = chartInstance.GetCurrentCellDPIndex();
				base.SetupEnvironment();
				RuntimeDataRegionObj.SetupRunningValues(base.m_owner.ProcessingContext, ((Chart)base.m_owner.PivotDef).CellRunningValues, this.m_runningValueValues);
				chartInstance.AddCell(base.m_owner.ProcessingContext, currentCellDPIndex);
			}
		}

        internal abstract class RuntimePivotGroupLeafObj : ReportProcessing.RuntimeGroupLeafObj
        {
            internal RuntimePivotGroupLeafObj(ReportProcessing.RuntimePivotGroupRootObj groupRoot) : base(groupRoot)
            {
            }

            internal PivotHeading PivotHeadingDef
            {
                get
                {
                    ReportProcessing.RuntimePivotGroupRootObj runtimePivotGroupRootObj = (ReportProcessing.RuntimePivotGroupRootObj)this.m_hierarchyRoot;
                    return (PivotHeading)runtimePivotGroupRootObj.HierarchyDef;
                }
            }

            internal DataAggregateObjList CellPostSortAggregates
            {
                get
                {
                    return this.m_cellPostSortAggregates;
                }
            }

            internal Pivot PivotDef
            {
                get
                {
                    return (Pivot)this.PivotHeadingDef.DataRegionDef;
                }
            }

            internal int HeadingLevel
            {
                get
                {
                    ReportProcessing.RuntimePivotGroupRootObj runtimePivotGroupRootObj = (ReportProcessing.RuntimePivotGroupRootObj)this.m_hierarchyRoot;
                    return runtimePivotGroupRootObj.HeadingLevel;
                }
            }

            protected void ConstructorHelper(ReportProcessing.RuntimePivotGroupRootObj groupRoot, Pivot pivotDef, out bool handleMyDataAction, out ReportProcessing.DataActions innerDataAction)
            {
                PivotHeading innerHeading = groupRoot.InnerHeading;
                this.m_dataAction = groupRoot.DataAction;
                handleMyDataAction = false;
                if (groupRoot.ProcessOutermostSTCells)
                {
                    ReportProcessing.RuntimeDataRegionObj.CreateAggregates(this.m_processingContext, pivotDef.CellAggregates, ref this.m_firstPassCellNonCustomAggs, ref this.m_firstPassCellCustomAggs);
                    if (pivotDef.CellPostSortAggregates != null)
                    {
                        handleMyDataAction = true;
                        ReportProcessing.RuntimeDataRegionObj.CreateAggregates(this.m_processingContext, pivotDef.CellPostSortAggregates, ref this.m_postSortAggregates);
                    }
                }
                this.ConstructRuntimeStructure(ref handleMyDataAction, out innerDataAction);
                if (this.IsOuterGrouping())
                {
                    this.m_groupLeafIndex = ++((ReportProcessing.RuntimePivotObj)pivotDef.RuntimeDataRegionObj).OuterGroupingCounters[groupRoot.HeadingLevel];
                }
                PivotHeading pivotHeading = (PivotHeading)groupRoot.HierarchyDef;
                Global.Tracer.Assert(null != pivotHeading.Grouping, "(null != pivotHeading.Grouping)");
                if (pivotHeading.Grouping.Filters != null)
                {
                    if (pivotHeading.IsColumn)
                    {
                        if (groupRoot.HeadingLevel > pivotDef.InnermostColumnFilterLevel)
                        {
                            pivotDef.InnermostColumnFilterLevel = groupRoot.HeadingLevel;
                            return;
                        }
                    }
                    else if (groupRoot.HeadingLevel > pivotDef.InnermostRowFilterLevel)
                    {
                        pivotDef.InnermostRowFilterLevel = groupRoot.HeadingLevel;
                    }
                }
            }

            protected override void ConstructRuntimeStructure(ref bool handleMyDataAction, out ReportProcessing.DataActions innerDataAction)
            {
                ReportProcessing.RuntimePivotGroupRootObj runtimePivotGroupRootObj = (ReportProcessing.RuntimePivotGroupRootObj)this.m_hierarchyRoot;
                Pivot pivot = (Pivot)runtimePivotGroupRootObj.HierarchyDef.DataRegionDef;
                base.ConstructRuntimeStructure(ref handleMyDataAction, out innerDataAction);
                if (!this.IsOuterGrouping() && (runtimePivotGroupRootObj.InnerHeading == null || runtimePivotGroupRootObj.InnerHeading.Subtotal != null))
                {
                    PivotHeading pivotHeading = null;
                    PivotHeading pivotHeading2 = pivot.GetPivotHeading(true);
                    int dynamicHeadingCount = pivot.GetDynamicHeadingCount(true);
                    int num = 0;
                    pivot.SkipStaticHeading(ref pivotHeading2, ref pivotHeading);
                    while (pivotHeading2 != null)
                    {
                        pivotHeading2 = (PivotHeading)pivotHeading2.InnerHierarchy;
                        bool flag = pivot.SubtotalInInnerHeading(ref pivotHeading2, ref pivotHeading);
                        if (this.m_cellsList == null)
                        {
                            this.m_cellsList = new ReportProcessing.RuntimePivotCells[dynamicHeadingCount];
                            if (this.m_cellPostSortAggregates == null)
                            {
                                ReportProcessing.RuntimeDataRegionObj.CreateAggregates(this.m_processingContext, pivot.CellPostSortAggregates, ref this.m_cellPostSortAggregates);
                            }
                        }
                        ReportProcessing.RuntimePivotCells runtimePivotCells = null;
                        if (pivotHeading2 == null || flag)
                        {
                            runtimePivotCells = new ReportProcessing.RuntimePivotCells();
                        }
                        this.m_cellsList[num++] = runtimePivotCells;
                    }
                }
            }

            internal abstract ReportProcessing.RuntimePivotCell CreateCell(int index, Pivot pivotDef);

            internal override void NextRow()
            {
                Pivot pivotDef = this.PivotDef;
                int headingLevel = this.HeadingLevel;
                bool flag = this.IsOuterGrouping();
                if (flag)
                {
                    pivotDef.OuterGroupingIndexes[headingLevel] = this.m_groupLeafIndex;
                }
                base.UpdateAggregateInfo();
                if (flag)
                {
                    pivotDef.SaveOuterGroupingAggregateRowInfo(headingLevel, this.m_processingContext);
                }
                FieldsImpl fieldsImpl = this.m_processingContext.ReportObjectModel.FieldsImpl;
                if (fieldsImpl.AggregationFieldCount == 0 && fieldsImpl.ValidAggregateRow)
                {
                    ReportProcessing.RuntimeDataRegionObj.UpdateAggregates(this.m_processingContext, this.m_firstPassCellCustomAggs, false);
                }
                if (!this.m_processingContext.ReportObjectModel.FieldsImpl.IsAggregateRow)
                {
                    ReportProcessing.RuntimeDataRegionObj.UpdateAggregates(this.m_processingContext, this.m_firstPassCellNonCustomAggs, false);
                }
                base.InternalNextRow();
            }

            protected override void SendToInner()
            {
                base.SendToInner();
                if (this.m_cellsList != null)
                {
                    Global.Tracer.Assert(!this.IsOuterGrouping(), "(!IsOuterGrouping())");
                    Pivot pivotDef = this.PivotDef;
                    int[] outerGroupingIndexes = pivotDef.OuterGroupingIndexes;
                    for (int i = 0; i < pivotDef.GetDynamicHeadingCount(true); i++)
                    {
                        int num = outerGroupingIndexes[i];
                        ReportProcessing.AggregateRowInfo aggregateRowInfo = new ReportProcessing.AggregateRowInfo();
                        aggregateRowInfo.SaveAggregateInfo(this.m_processingContext);
                        pivotDef.SetCellAggregateRowInfo(i, this.m_processingContext);
                        ReportProcessing.RuntimePivotCells runtimePivotCells = this.m_cellsList[i];
                        if (runtimePivotCells != null)
                        {
                            ReportProcessing.RuntimePivotCell runtimePivotCell = runtimePivotCells[num];
                            if (runtimePivotCell == null)
                            {
                                runtimePivotCell = this.CreateCell(i, pivotDef);
                                runtimePivotCells.Add(num, runtimePivotCell);
                            }
                            runtimePivotCell.NextRow();
                        }
                        aggregateRowInfo.RestoreAggregateInfo(this.m_processingContext);
                    }
                }
                if (this.m_pivotHeadings != null)
                {
                    this.m_pivotHeadings.NextRow();
                }
            }

            internal override bool SortAndFilter()
            {
                ReportProcessing.RuntimePivotGroupRootObj runtimePivotGroupRootObj = (ReportProcessing.RuntimePivotGroupRootObj)this.m_hierarchyRoot;
                bool flag = false;
                if (this.m_innerHierarchy != null && !this.m_pivotHeadings.SortAndFilter())
                {
                    Global.Tracer.Assert((ReportProcessing.ProcessingContext.SecondPassOperations)0 != (ReportProcessing.ProcessingContext.SecondPassOperations.Filtering & this.m_processingContext.SecondPassOperation));
                    Global.Tracer.Assert(null != runtimePivotGroupRootObj.GroupFilters, "(null != groupRoot.GroupFilters)");
                    runtimePivotGroupRootObj.GroupFilters.FailFilters = true;
                    flag = true;
                }
                bool flag2 = base.SortAndFilter();
                if (flag)
                {
                    runtimePivotGroupRootObj.GroupFilters.FailFilters = false;
                }
                if (flag2 && this.m_cellsList != null)
                {
                    for (int i = 0; i < this.m_cellsList.Length; i++)
                    {
                        if (this.m_cellsList[i] != null)
                        {
                            this.m_cellsList[i].SortAndFilter();
                        }
                    }
                }
                return flag2;
            }

            internal override void CalculateRunningValues()
            {
                Pivot pivotDef = this.PivotDef;
                ReportProcessing.RuntimePivotGroupRootObj runtimePivotGroupRootObj = (ReportProcessing.RuntimePivotGroupRootObj)this.m_hierarchyRoot;
                AggregatesImpl globalRunningValueCollection = runtimePivotGroupRootObj.GlobalRunningValueCollection;
                ReportProcessing.RuntimeGroupRootObjList groupCollection = runtimePivotGroupRootObj.GroupCollection;
                bool flag = this.IsOuterGrouping();
                pivotDef.GetDynamicHeadingCount(true);
                if (this.m_processHeading)
                {
                    if (this.m_dataRows != null && (ReportProcessing.DataActions.PostSortAggregates & this.m_dataAction) != ReportProcessing.DataActions.None)
                    {
                        base.ReadRows(ReportProcessing.DataActions.PostSortAggregates);
                        this.m_dataRows = null;
                    }
                    this.m_pivotHeadings.CalculateRunningValues(globalRunningValueCollection, groupCollection, runtimePivotGroupRootObj);
                }
                else if (this.m_innerHierarchy != null)
                {
                    this.m_innerHierarchy.CalculateRunningValues(globalRunningValueCollection, groupCollection, runtimePivotGroupRootObj);
                }
                if (flag)
                {
                    if (this.m_innerHierarchy == null || ((PivotHeading)this.m_innerHierarchy.HierarchyDef).Subtotal != null)
                    {
                        pivotDef.CurrentOuterHeadingGroupRoot = runtimePivotGroupRootObj;
                        pivotDef.OuterGroupingIndexes[runtimePivotGroupRootObj.HeadingLevel] = this.m_groupLeafIndex;
                        runtimePivotGroupRootObj.InnerGroupings.CalculateRunningValues(globalRunningValueCollection, groupCollection, runtimePivotGroupRootObj);
                        return;
                    }
                }
                else if (this.m_cellsList != null)
                {
                    ReportProcessing.RuntimePivotGroupRootObj currentOuterHeadingGroupRoot = pivotDef.CurrentOuterHeadingGroupRoot;
                    ReportProcessing.RuntimePivotCells runtimePivotCells = this.m_cellsList[currentOuterHeadingGroupRoot.HeadingLevel];
                    Global.Tracer.Assert(null != runtimePivotCells, "(null != cells)");
                    pivotDef.ProcessCellRunningValues = true;
                    runtimePivotCells.CalculateRunningValues(pivotDef, currentOuterHeadingGroupRoot.CellRVCol, groupCollection, runtimePivotGroupRootObj, this, currentOuterHeadingGroupRoot.HeadingLevel);
                    pivotDef.ProcessCellRunningValues = false;
                }
            }

            protected override void ResetScopedRunningValues()
            {
                base.ResetScopedRunningValues();
                this.ResetScopedCellRunningValues();
            }

            internal bool IsOuterGrouping()
            {
                ReportProcessing.RuntimePivotGroupRootObj runtimePivotGroupRootObj = (ReportProcessing.RuntimePivotGroupRootObj)this.m_hierarchyRoot;
                return null != runtimePivotGroupRootObj.InnerGroupings;
            }

            internal override void ReadRow(ReportProcessing.DataActions dataAction)
            {
                if (ReportProcessing.DataActions.UserSort == dataAction)
                {
                    ReportProcessing.RuntimeDataRegionObj.CommonFirstRow(this.m_processingContext.ReportObjectModel.FieldsImpl, ref this.m_firstRowIsAggregate, ref this.m_firstRow);
                    base.CommonNextRow(this.m_dataRows);
                    return;
                }
                if (this.PivotDef.ProcessCellRunningValues)
                {
                    if (ReportProcessing.DataActions.PostSortAggregates == dataAction && this.m_cellPostSortAggregates != null)
                    {
                        ReportProcessing.RuntimeDataRegionObj.UpdateAggregates(this.m_processingContext, this.m_cellPostSortAggregates, false);
                    }
                    ((ReportProcessing.IScope)this.m_hierarchyRoot).ReadRow(dataAction);
                    return;
                }
                base.ReadRow(dataAction);
                if (ReportProcessing.DataActions.PostSortAggregates == dataAction)
                {
                    this.CalculatePreviousAggregates();
                }
            }

            protected virtual bool CalculatePreviousAggregates()
            {
                if (!this.m_processedPreviousAggregates && this.m_processingContext.GlobalRVCollection != null)
                {
                    if (this.m_innerHierarchy != null)
                    {
                        this.m_pivotHeadings.CalculatePreviousAggregates(this.m_processingContext.GlobalRVCollection);
                    }
                    this.m_processedPreviousAggregates = true;
                    return true;
                }
                return false;
            }

            protected void ResetScopedCellRunningValues()
            {
                ReportProcessing.RuntimePivotGroupRootObj runtimePivotGroupRootObj = (ReportProcessing.RuntimePivotGroupRootObj)this.m_hierarchyRoot;
                if (runtimePivotGroupRootObj.OutermostSTScopedCellRVCollections != null)
                {
                    for (int i = 0; i < runtimePivotGroupRootObj.OutermostSTScopedCellRVCollections.Length; i++)
                    {
                        AggregatesImpl aggregatesImpl = runtimePivotGroupRootObj.OutermostSTScopedCellRVCollections[i];
                        if (aggregatesImpl != null)
                        {
                            foreach (object obj in aggregatesImpl.Objects)
                            {
                                DataAggregateObj dataAggregateObj = (DataAggregateObj)obj;
                                dataAggregateObj.Init();
                            }
                        }
                    }
                }
                if (runtimePivotGroupRootObj.CellScopedRVCollections != null)
                {
                    for (int j = 0; j < runtimePivotGroupRootObj.CellScopedRVCollections.Length; j++)
                    {
                        AggregatesImpl aggregatesImpl2 = runtimePivotGroupRootObj.CellScopedRVCollections[j];
                        if (aggregatesImpl2 != null)
                        {
                            foreach (object obj2 in aggregatesImpl2.Objects)
                            {
                                DataAggregateObj dataAggregateObj2 = (DataAggregateObj)obj2;
                                dataAggregateObj2.Init();
                            }
                        }
                    }
                }
            }

            internal override void SetupEnvironment()
            {
                base.SetupEnvironment();
                this.SetupAggregateValues(this.m_firstPassCellNonCustomAggs, this.m_firstPassCellCustomAggs);
            }

            private void SetupAggregateValues(DataAggregateObjList nonCustomAggCollection, DataAggregateObjList customAggCollection)
            {
                base.SetupAggregates(nonCustomAggCollection);
                base.SetupAggregates(customAggCollection);
            }

            internal bool GetCellTargetForNonDetailSort()
            {
                return ((ReportProcessing.RuntimePivotGroupRootObj)this.m_hierarchyRoot).GetCellTargetForNonDetailSort();
            }

            internal bool GetCellTargetForSort(int index, bool detailSort)
            {
                return ((ReportProcessing.RuntimePivotGroupRootObj)this.m_hierarchyRoot).GetCellTargetForSort(index, detailSort);
            }

            internal bool NeedHandleCellSortFilterEvent()
            {
                return base.GroupingDef.SortFilterScopeMatched != null || null != base.GroupingDef.NeedScopeInfoForSortFilterExpression;
            }

            internal ReportProcessing.RuntimePivotObj GetOwnerPivot()
            {
                ReportProcessing.IScope outerScope = this.OuterScope;
                while (!(outerScope is ReportProcessing.RuntimePivotObj))
                {
                    outerScope = outerScope.GetOuterScope(false);
                }
                Global.Tracer.Assert(outerScope is ReportProcessing.RuntimePivotObj, "(outerScope is RuntimePivotObj)");
                return (ReportProcessing.RuntimePivotObj)outerScope;
            }

            protected ReportProcessing.RuntimePivotHeadingsObj m_pivotHeadings;

            protected ReportProcessing.RuntimePivotGroupRootObj m_innerHierarchy;

            protected DataAggregateObjList m_firstPassCellNonCustomAggs;

            protected DataAggregateObjList m_firstPassCellCustomAggs;

            protected ReportProcessing.RuntimePivotCells[] m_cellsList;

            protected DataAggregateObjList m_cellPostSortAggregates;

            protected int m_groupLeafIndex = -1;

            protected bool m_processHeading = true;
        }

        private sealed class RuntimeChartGroupLeafObj : RuntimePivotGroupLeafObj
		{
			private DataAggregateObjResult[] m_runningValueValues;

			private DataAggregateObjResult[] m_cellRunningValueValues;

			internal RuntimeChartGroupLeafObj(RuntimeChartGroupRootObj groupRoot)
				: base(groupRoot)
			{
				ChartHeading chartHeading = (ChartHeading)groupRoot.HierarchyDef;
				Chart pivotDef = (Chart)chartHeading.DataRegionDef;
				ChartHeading headingDef = (ChartHeading)groupRoot.InnerHeading;
				bool flag = false;
				bool flag2 = base.HandleSortFilterEvent();
				DataActions dataAction = default(DataActions);
				base.ConstructorHelper((RuntimePivotGroupRootObj)groupRoot, (Pivot)pivotDef, out flag, out dataAction);
				base.m_pivotHeadings = new RuntimeChartHeadingsObj(this, headingDef, ref dataAction, groupRoot.ProcessingContext, (ChartHeading)groupRoot.StaticHeadingDef, (RuntimeChartHeadingsObj)groupRoot.InnerGroupings, groupRoot.OutermostSubtotal, groupRoot.HeadingLevel + 1);
				base.m_innerHierarchy = base.m_pivotHeadings.Headings;
				if (!flag)
				{
					base.m_dataAction = dataAction;
				}
				if (flag2)
				{
					base.m_dataAction |= DataActions.UserSort;
				}
				if (base.m_dataAction != 0)
				{
					base.m_dataRows = new DataRowList();
				}
			}

			internal override RuntimePivotCell CreateCell(int index, Pivot pivotDef)
			{
				return new RuntimeChartCell(this, index, pivotDef.CellAggregates, ((Chart)pivotDef).ChartDataPoints, null == base.m_innerHierarchy);
			}

			internal override bool SortAndFilter()
			{
				this.SetupEnvironment();
				if (base.m_userSortTargetInfo != null)
				{
					base.m_userSortTargetInfo.EnterProcessUserSortPhase(base.m_processingContext);
				}
				bool result = base.SortAndFilter();
				if (base.m_userSortTargetInfo != null)
				{
					base.m_userSortTargetInfo.LeaveProcessUserSortPhase(base.m_processingContext);
				}
				return result;
			}

			internal override void CalculateRunningValues()
			{
				base.CalculateRunningValues();
				if (base.m_processHeading)
				{
					RuntimePivotGroupRootObj runtimePivotGroupRootObj = (RuntimePivotGroupRootObj)base.m_hierarchyRoot;
					AggregatesImpl globalRunningValueCollection = runtimePivotGroupRootObj.GlobalRunningValueCollection;
					RuntimeGroupRootObjList groupCollection = runtimePivotGroupRootObj.GroupCollection;
					RuntimeRICollection.DoneReadingRows(globalRunningValueCollection, ((ChartHeading)runtimePivotGroupRootObj.HierarchyDef).RunningValues, ref this.m_runningValueValues, false);
					if (runtimePivotGroupRootObj.ProcessOutermostSTCells)
					{
						RuntimeRICollection.DoneReadingRows(runtimePivotGroupRootObj.OutermostSTCellRVCol, ((Chart)base.PivotDef).CellRunningValues, ref this.m_cellRunningValueValues, false);
					}
					base.m_processHeading = false;
				}
				this.ResetScopedRunningValues();
			}

			internal override void CreateInstance()
			{
				this.SetupEnvironment();
				RuntimeChartGroupRootObj runtimeChartGroupRootObj = (RuntimeChartGroupRootObj)base.m_hierarchyRoot;
				Chart chart = (Chart)base.PivotDef;
				ChartInstance chartInstance = (ChartInstance)runtimeChartGroupRootObj.ReportItemInstance;
				ChartHeadingInstanceList chartHeadingInstanceList = (ChartHeadingInstanceList)runtimeChartGroupRootObj.InstanceList;
				ChartHeading chartHeading = (ChartHeading)runtimeChartGroupRootObj.HierarchyDef;
				bool flag = base.IsOuterGrouping();
				base.SetupRunningValues(chartHeading.RunningValues, this.m_runningValueValues);
				if (this.m_cellRunningValueValues != null)
				{
					base.SetupRunningValues(chart.CellRunningValues, this.m_cellRunningValueValues);
				}
				RuntimePivotGroupRootObj currOuterHeadingGroupRoot;
				int headingCellIndex;
				if (flag)
				{
					currOuterHeadingGroupRoot = (chart.CurrentOuterHeadingGroupRoot = runtimeChartGroupRootObj);
					chart.OuterGroupingIndexes[runtimeChartGroupRootObj.HeadingLevel] = base.m_groupLeafIndex;
					chartInstance.NewOuterCells();
					headingCellIndex = chartInstance.CurrentCellOuterIndex;
				}
				else
				{
					currOuterHeadingGroupRoot = chart.CurrentOuterHeadingGroupRoot;
					headingCellIndex = chartInstance.CurrentCellInnerIndex;
				}
				if (flag || chartInstance.CurrentCellOuterIndex == 0)
				{
					ChartHeadingInstance chartHeadingInstance = new ChartHeadingInstance(base.m_processingContext, headingCellIndex, chartHeading, 0, base.m_groupExprValues);
					chartHeadingInstanceList.Add(chartHeadingInstance, base.m_processingContext);
					chartHeadingInstanceList = chartHeadingInstance.SubHeadingInstances;
				}
				((RuntimeChartHeadingsObj)base.m_pivotHeadings).CreateInstances(this, base.m_processingContext, chartInstance, flag, currOuterHeadingGroupRoot, chartHeadingInstanceList);
			}

			internal void CreateInnerGroupingsOrCells(ChartInstance chartInstance, RuntimePivotGroupRootObj currOuterHeadingGroupRoot)
			{
				this.SetupEnvironment();
				if (base.IsOuterGrouping())
				{
					RuntimeChartHeadingsObj runtimeChartHeadingsObj = (RuntimeChartHeadingsObj)((RuntimeChartGroupRootObj)base.m_hierarchyRoot).InnerGroupings;
					runtimeChartHeadingsObj.CreateInstances(this, base.m_processingContext, chartInstance, false, currOuterHeadingGroupRoot, chartInstance.InnerHeadingInstanceList);
				}
				else if (currOuterHeadingGroupRoot == null)
				{
					this.CreateOutermostSubtotalCell(chartInstance);
				}
				else
				{
					this.CreateCellInstance(chartInstance, currOuterHeadingGroupRoot);
				}
			}

			private void CreateCellInstance(ChartInstance chartInstance, RuntimePivotGroupRootObj currOuterHeadingGroupRoot)
			{
				Global.Tracer.Assert(base.m_cellsList != null && null != base.m_cellsList[currOuterHeadingGroupRoot.HeadingLevel]);
				RuntimeChartCell runtimeChartCell = (RuntimeChartCell)base.m_cellsList[currOuterHeadingGroupRoot.HeadingLevel].GetCell(base.PivotDef, this, currOuterHeadingGroupRoot.HeadingLevel);
				Global.Tracer.Assert(null != runtimeChartCell, "(null != cell)");
				runtimeChartCell.CreateInstance(chartInstance);
			}

			private void CreateOutermostSubtotalCell(ChartInstance chartInstance)
			{
				this.SetupEnvironment();
				chartInstance.AddCell(base.m_processingContext, -1);
			}

			internal void CreateSubtotalOrStaticCells(ChartInstance chartInstance, RuntimePivotGroupRootObj currOuterHeadingGroupRoot, bool outerGroupingSubtotal)
			{
				RuntimeChartHeadingsObj runtimeChartHeadingsObj = (RuntimeChartHeadingsObj)((RuntimeChartGroupRootObj)base.m_hierarchyRoot).InnerGroupings;
				if (base.IsOuterGrouping() && !outerGroupingSubtotal)
				{
					this.CreateOutermostSubtotalCell(chartInstance);
				}
				else
				{
					this.CreateInnerGroupingsOrCells(chartInstance, currOuterHeadingGroupRoot);
				}
			}
		}

		private sealed class RuntimeOnePassOWCChartDetailObj : RuntimeOnePassDetailObj, IFilterOwner
		{
			private Filters m_filters;

			private VariantList[] m_chartData;

			private DataAggregateObjList m_nonCustomAggregates;

			private DataAggregateObjList m_customAggregates;

			private FieldImpl[] m_firstRow;

			internal VariantList[] OWCChartData
			{
				get
				{
					return this.m_chartData;
				}
			}

			protected override string ScopeName
			{
				get
				{
					return base.m_dataRegionDef.Name;
				}
			}

			internal RuntimeOnePassOWCChartDetailObj(IScope outerScope, OWCChart chartDef, ProcessingContext processingContext)
				: base(outerScope, chartDef, processingContext)
			{
				RuntimeDataRegionObj.CreateAggregates(base.m_processingContext, chartDef.Aggregates, ref this.m_nonCustomAggregates, ref this.m_customAggregates);
				RuntimeDataRegionObj.CreateAggregates(base.m_processingContext, chartDef.PostSortAggregates, ref this.m_nonCustomAggregates);
				if (chartDef.RunningValues != null && 0 < chartDef.RunningValues.Count)
				{
					RuntimeDataRegionObj.CreateAggregates(base.m_processingContext, chartDef.RunningValues, ref this.m_nonCustomAggregates);
				}
				if (chartDef.Filters != null)
				{
					this.m_filters = new Filters(Filters.FilterTypes.DataRegionFilter, this, chartDef.Filters, chartDef.ObjectType, chartDef.Name, base.m_processingContext);
				}
				base.AddRunningValues(chartDef.DetailRunningValues);
				this.m_chartData = new VariantList[chartDef.ChartData.Count];
				for (int i = 0; i < chartDef.ChartData.Count; i++)
				{
					this.m_chartData[i] = new VariantList();
				}
			}

			internal override int GetDetailPage()
			{
				return 0;
			}

			internal override void NextRow()
			{
				if (base.m_processingContext.ReportObjectModel.FieldsImpl.IsAggregateRow)
				{
					this.NextAggregateRow();
				}
				else
				{
					this.NextNonAggregateRow();
				}
			}

			private void NextNonAggregateRow()
			{
				bool flag = true;
				if (this.m_filters != null)
				{
					flag = this.m_filters.PassFilters(base.m_processingContext.ReportObjectModel.FieldsImpl.GetFields());
				}
				if (flag)
				{
					((IFilterOwner)this).PostFilterNextRow();
				}
			}

			private void NextAggregateRow()
			{
				if (base.m_processingContext.ReportObjectModel.FieldsImpl.AggregationFieldCount == 0)
				{
					RuntimeDataRegionObj.UpdateAggregates(base.m_processingContext, this.m_customAggregates, false);
				}
			}

			void IFilterOwner.PostFilterNextRow()
			{
				if (this.m_firstRow == null)
				{
					this.m_firstRow = base.m_processingContext.ReportObjectModel.FieldsImpl.GetAndSaveFields();
				}
				base.NextRow();
			}

			protected override void CreateInstance()
			{
				OWCChart oWCChart = (OWCChart)base.m_dataRegionDef;
				for (int i = 0; i < oWCChart.ChartData.Count; i++)
				{
					this.m_chartData[i].Add(base.m_processingContext.ReportRuntime.EvaluateOWCChartData(oWCChart, oWCChart.ChartData[i].Value));
				}
			}

			internal override void SetupEnvironment()
			{
				base.SetupEnvironment(this.m_nonCustomAggregates, this.m_customAggregates, this.m_firstRow);
			}

			internal override bool InScope(string scope)
			{
				return base.DataRegionInScope(base.m_dataRegionDef, scope);
			}
		}

		private sealed class RuntimeMatrixHeadingsObj : RuntimePivotHeadingsObj
		{
			private RuntimeRICollection m_subtotal;

			private MatrixHeading m_subtotalHeadingDef;

			private RuntimeRICollection m_subtotalStaticHeading;

			private MatrixHeading m_subtotalStaticHeadingDef;

			private RuntimeRICollection m_staticHeading;

			private MatrixHeadingInstance[] m_subtotalHeadingInstances;

			internal RuntimeMatrixHeadingsObj(IScope owner, MatrixHeading headingDef, ref DataActions dataAction, ProcessingContext processingContext, MatrixHeading staticHeadingDef, RuntimeMatrixHeadingsObj innerGroupings, bool outermostHeadingSubtotal, int headingLevel)
				: base(owner, (PivotHeading)headingDef, ref dataAction, processingContext, (PivotHeading)staticHeadingDef, (RuntimePivotHeadingsObj)innerGroupings, outermostHeadingSubtotal, headingLevel)
			{
				if (headingDef != null)
				{
					base.m_pivotHeadings = new RuntimeMatrixGroupRootObj(owner, headingDef, ref dataAction, processingContext, innerGroupings, outermostHeadingSubtotal, headingLevel);
					if (headingDef.Subtotal != null)
					{
						this.m_subtotalHeadingDef = headingDef;
						this.m_subtotal = new RuntimeRICollection(owner, this.m_subtotalHeadingDef.Subtotal.ReportItems, ref dataAction, processingContext, true);
						MatrixHeading matrixHeading = (MatrixHeading)headingDef.GetInnerStaticHeading();
						if (matrixHeading != null)
						{
							this.m_subtotalStaticHeadingDef = matrixHeading;
							this.m_subtotalStaticHeading = new RuntimeRICollection(owner, this.m_subtotalStaticHeadingDef.ReportItems, ref dataAction, processingContext, true);
						}
					}
				}
				if (base.m_staticHeadingDef != null)
				{
					this.m_staticHeading = new RuntimeRICollection(owner, ((MatrixHeading)base.m_staticHeadingDef).ReportItems, ref dataAction, processingContext, true);
				}
			}

			internal override void NextRow()
			{
				base.NextRow();
				if (this.m_subtotal != null)
				{
					this.m_subtotal.FirstPassNextDataRow();
					if (this.m_subtotalStaticHeading != null)
					{
						this.m_subtotalStaticHeading.FirstPassNextDataRow();
					}
				}
				if (this.m_staticHeading != null)
				{
					this.m_staticHeading.FirstPassNextDataRow();
				}
			}

			internal override bool SortAndFilter()
			{
				bool flag = base.SortAndFilter();
				if (flag)
				{
					if (this.m_subtotal != null)
					{
						this.m_subtotal.SortAndFilter();
						if (this.m_subtotalStaticHeading != null)
						{
							this.m_subtotalStaticHeading.SortAndFilter();
						}
					}
					if (this.m_staticHeading != null)
					{
						this.m_staticHeading.SortAndFilter();
					}
				}
				return flag;
			}

			internal override void CalculatePreviousAggregates(AggregatesImpl globalRVCol)
			{
				if (base.m_pivotHeadings != null && this.m_subtotal != null)
				{
					this.m_subtotal.CalculatePreviousAggregates(globalRVCol);
					if (this.m_subtotalStaticHeading != null)
					{
						this.m_subtotalStaticHeading.CalculatePreviousAggregates(globalRVCol);
					}
				}
				if (this.m_staticHeading != null)
				{
					this.m_staticHeading.CalculatePreviousAggregates(globalRVCol);
				}
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				base.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				if (base.m_owner is RuntimeMatrixObj)
				{
					if (this.m_subtotal != null)
					{
						this.m_subtotal.CalculateInnerRunningValues(globalRVCol, groupCol, lastGroup);
						if (this.m_subtotalStaticHeading != null)
						{
							this.m_subtotalStaticHeading.CalculateInnerRunningValues(globalRVCol, groupCol, lastGroup);
						}
					}
					if (this.m_staticHeading != null)
					{
						this.m_staticHeading.CalculateInnerRunningValues(globalRVCol, groupCol, lastGroup);
					}
				}
				else
				{
					if (this.m_subtotal != null)
					{
						this.m_subtotal.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
						if (this.m_subtotalStaticHeading != null)
						{
							this.m_subtotalStaticHeading.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
						}
					}
					if (this.m_staticHeading != null)
					{
						this.m_staticHeading.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
					}
				}
			}

			internal void CreateInstances(RuntimeDataRegionObj outerGroup, ProcessingContext processingContext, MatrixInstance matrixInstance, bool outerGroupings, RuntimePivotGroupRootObj currOuterHeadingGroupRoot, MatrixHeadingInstanceList headingInstances, RenderingPagesRangesList pagesList)
			{
				bool flag = outerGroupings || 0 == matrixInstance.CurrentCellOuterIndex;
				int num = 1;
				if (this.m_staticHeading != null)
				{
					num = this.m_staticHeading.ReportItemsDef.Count;
				}
				MatrixHeadingInstanceList matrixHeadingInstanceList = headingInstances;
				RenderingPagesRangesList renderingPagesRangesList = pagesList;
				MatrixHeadingInstance matrixHeadingInstance = null;
				for (int i = 0; i < num; i++)
				{
					bool flag2 = false;
					RenderingPagesRanges renderingPagesRanges = default(RenderingPagesRanges);
					PageTextboxes source = null;
					if (this.m_staticHeading != null)
					{
						if (flag)
						{
							matrixHeadingInstanceList = this.CreateHeadingInstance(processingContext, matrixInstance, (MatrixHeading)base.m_staticHeadingDef, headingInstances, outerGroupings, this.m_staticHeading, i, false, true, 0, num, out renderingPagesRangesList, out source);
							if (!((MatrixHeading)base.m_staticHeadingDef).IsColumn)
							{
								processingContext.Pagination.EnterIgnorePageBreak(base.m_staticHeadingDef.Visibility, false);
								renderingPagesRanges.StartPage = ((Matrix)matrixInstance.ReportItemDef).CurrentPage;
								flag2 = true;
							}
						}
						if (outerGroupings)
						{
							matrixInstance.CurrentOuterStaticIndex = i;
						}
						else
						{
							matrixInstance.CurrentInnerStaticIndex = i;
						}
					}
					if (base.m_pivotHeadings != null)
					{
						Matrix matrix = (Matrix)base.m_pivotHeadings.HierarchyDef.DataRegionDef;
						matrix.CurrentOuterHeadingGroupRoot = currOuterHeadingGroupRoot;
						if (this.m_subtotal == null || this.m_subtotalHeadingDef.Subtotal.Position == Subtotal.PositionType.After)
						{
							base.m_pivotHeadings.CreateInstances(matrixInstance, matrixHeadingInstanceList, renderingPagesRangesList);
						}
						if (this.m_subtotal != null)
						{
							MatrixHeadingInstanceList matrixHeadingInstanceList2 = null;
							RenderingPagesRangesList renderingPagesRangesList2 = null;
							PageTextboxes source2 = null;
							bool flag3 = false;
							RenderingPagesRanges renderingPagesRanges2 = default(RenderingPagesRanges);
							if (flag)
							{
								matrixHeadingInstanceList2 = this.CreateHeadingInstance(processingContext, matrixInstance, this.m_subtotalHeadingDef, matrixHeadingInstanceList, outerGroupings, this.m_subtotal, 0, true, false, i, num, out renderingPagesRangesList2, out source2);
								if (!this.m_subtotalHeadingDef.IsColumn)
								{
									processingContext.Pagination.EnterIgnorePageBreak(this.m_subtotalHeadingDef.Visibility, true);
									renderingPagesRanges2.StartPage = ((Matrix)matrixInstance.ReportItemDef).CurrentPage;
									flag3 = true;
								}
							}
							Global.Tracer.Assert(null != this.m_subtotalHeadingInstances[i], "(null != m_subtotalHeadingInstances[i])");
							if (((Matrix)matrixInstance.ReportItemDef).ProcessingInnerGrouping == Pivot.ProcessingInnerGroupings.Column)
							{
								if (!this.m_subtotalHeadingInstances[i].MatrixHeadingDef.IsColumn)
								{
									processingContext.HeadingInstance = this.m_subtotalHeadingInstances[i];
								}
								else if (processingContext.HeadingInstance == null)
								{
									processingContext.HeadingInstance = this.m_subtotalHeadingInstances[i];
								}
							}
							else
							{
								processingContext.HeadingInstanceOld = processingContext.HeadingInstance;
								processingContext.HeadingInstance = this.m_subtotalHeadingInstances[i];
							}
							int num2 = 1;
							if (this.m_subtotalStaticHeading != null)
							{
								num2 = this.m_subtotalStaticHeading.ReportItemsDef.Count;
							}
							for (int j = 0; j < num2; j++)
							{
								bool flag4 = false;
								RenderingPagesRanges renderingPagesRanges3 = default(RenderingPagesRanges);
								if (base.m_owner is RuntimeMatrixObj)
								{
									((RuntimeMatrixObj)base.m_owner).SetupEnvironment();
								}
								else
								{
									((RuntimeMatrixGroupLeafObj)outerGroup).SetupEnvironment();
								}
								MatrixHeadingInstance headingInstance = processingContext.HeadingInstance;
								MatrixHeadingInstance headingInstanceOld = processingContext.HeadingInstanceOld;
								if (this.m_subtotalStaticHeading != null)
								{
									if (flag)
									{
										Global.Tracer.Assert(null != matrixHeadingInstanceList2, "(null != subtotalInnerHeadings)");
										PageTextboxes source3 = null;
										RenderingPagesRangesList renderingPagesRangesList3 = default(RenderingPagesRangesList);
										this.CreateHeadingInstance(processingContext, matrixInstance, this.m_subtotalStaticHeadingDef, matrixHeadingInstanceList2, outerGroupings, this.m_subtotalStaticHeading, j, true, true, 0, num, out renderingPagesRangesList3, out source3);
										processingContext.HeadingInstance = headingInstance;
										processingContext.HeadingInstanceOld = headingInstanceOld;
										if (!this.m_subtotalStaticHeadingDef.IsColumn)
										{
											processingContext.Pagination.EnterIgnorePageBreak(this.m_subtotalStaticHeadingDef.Visibility, true);
											renderingPagesRanges3.StartPage = matrixInstance.MatrixDef.CurrentPage;
											matrixInstance.MatrixDef.RowHeaderPageTextboxes.IntegrateRepeatingTextboxValues(source3, renderingPagesRanges3.StartPage, renderingPagesRanges3.StartPage);
											flag4 = true;
										}
									}
									if (outerGroupings)
									{
										matrixInstance.CurrentOuterStaticIndex = j;
									}
									else
									{
										matrixInstance.CurrentInnerStaticIndex = j;
									}
								}
								if (outerGroup is RuntimeMatrixGroupLeafObj)
								{
									((RuntimeMatrixGroupLeafObj)outerGroup).CreateSubtotalOrStaticCells(matrixInstance, currOuterHeadingGroupRoot, outerGroupings);
								}
								else
								{
									((RuntimeMatrixObj)outerGroup).CreateOutermostSubtotalCells(matrixInstance, outerGroupings);
								}
								if (num2 - 1 != j)
								{
									processingContext.HeadingInstance = headingInstance;
									processingContext.HeadingInstanceOld = headingInstanceOld;
								}
								if (flag4)
								{
									processingContext.Pagination.LeaveIgnorePageBreak(this.m_subtotalStaticHeadingDef.Visibility, true);
									renderingPagesRanges3.EndPage = renderingPagesRanges3.StartPage;
									renderingPagesRangesList2.Add(renderingPagesRanges3);
								}
								if (this.m_subtotalStaticHeading != null)
								{
									this.m_subtotalStaticHeading.ResetReportItemObjs();
								}
							}
							if (this.m_subtotalStaticHeading != null && flag)
							{
								this.SetHeadingSpan(matrixInstance, matrixHeadingInstanceList2, outerGroupings, processingContext);
							}
							if (flag3)
							{
								processingContext.Pagination.LeaveIgnorePageBreak(this.m_subtotalHeadingDef.Visibility, true);
								renderingPagesRanges2.EndPage = matrixInstance.MatrixDef.CurrentPage;
								if (renderingPagesRangesList2 == null || renderingPagesRangesList2.Count < 1)
								{
									renderingPagesRanges2.EndPage = renderingPagesRanges2.StartPage;
								}
								matrixInstance.MatrixDef.RowHeaderPageTextboxes.IntegrateRepeatingTextboxValues(source2, renderingPagesRanges2.StartPage, renderingPagesRanges2.EndPage);
								renderingPagesRangesList.Add(renderingPagesRanges2);
							}
						}
						if (outerGroupings)
						{
							processingContext.HeadingInstance = null;
						}
						if (this.m_subtotal != null && Subtotal.PositionType.Before == this.m_subtotalHeadingDef.Subtotal.Position)
						{
							base.m_pivotHeadings.CreateInstances(matrixInstance, matrixHeadingInstanceList, renderingPagesRangesList);
						}
						if (flag)
						{
							this.SetHeadingSpan(matrixInstance, matrixHeadingInstanceList, outerGroupings, processingContext);
						}
					}
					else if (outerGroup is RuntimeMatrixGroupLeafObj)
					{
						RuntimeMatrixGroupLeafObj runtimeMatrixGroupLeafObj = (RuntimeMatrixGroupLeafObj)outerGroup;
						runtimeMatrixGroupLeafObj.SetContentsPage();
						if (!outerGroupings && runtimeMatrixGroupLeafObj.IsOuterGrouping())
						{
							runtimeMatrixGroupLeafObj.CreateSubtotalOrStaticCells(matrixInstance, currOuterHeadingGroupRoot, outerGroupings);
						}
						else
						{
							runtimeMatrixGroupLeafObj.CreateInnerGroupingsOrCells(matrixInstance, currOuterHeadingGroupRoot);
							if (outerGroupings)
							{
								processingContext.HeadingInstance = null;
							}
						}
					}
					else
					{
						((RuntimeMatrixObj)outerGroup).CreateOutermostSubtotalCells(matrixInstance, outerGroupings);
					}
					if (flag2)
					{
						processingContext.Pagination.LeaveIgnorePageBreak(base.m_staticHeadingDef.Visibility, false);
						renderingPagesRanges.EndPage = matrixInstance.MatrixDef.CurrentPage;
						if (matrixHeadingInstanceList == null || matrixHeadingInstanceList.Count < 1)
						{
							renderingPagesRanges.EndPage = renderingPagesRanges.StartPage;
						}
						pagesList.Add(renderingPagesRanges);
						matrixInstance.MatrixDef.RowHeaderPageTextboxes.IntegrateRepeatingTextboxValues(source, renderingPagesRanges.StartPage, renderingPagesRanges.EndPage);
					}
					this.ResetReportItemObjs(processingContext);
				}
				if (Report.ShowHideTypes.Interactive == processingContext.ShowHideType && flag && this.m_staticHeading != null)
				{
					for (int num3 = num - 1; num3 >= 0; num3--)
					{
						matrixHeadingInstance = headingInstances[num3];
						((IShowHideContainer)matrixHeadingInstance).EndProcessContainer(processingContext);
						processingContext.ExitGrouping();
					}
				}
				if (this.m_staticHeading != null && flag)
				{
					this.SetHeadingSpan(matrixInstance, headingInstances, outerGroupings, processingContext);
				}
			}

			private void SetHeadingSpan(MatrixInstance matrixInstance, MatrixHeadingInstanceList headingInstances, bool outerGroupings, ProcessingContext processingContext)
			{
				int currentCellIndex = (!outerGroupings) ? matrixInstance.CurrentCellInnerIndex : (matrixInstance.CurrentCellOuterIndex + 1);
				headingInstances.SetLastHeadingSpan(currentCellIndex, processingContext);
			}

			private MatrixHeadingInstanceList CreateHeadingInstance(ProcessingContext processingContext, MatrixInstance matrixInstance, MatrixHeading headingDef, MatrixHeadingInstanceList headingInstances, bool outerGroupings, RuntimeRICollection headingReportItems, int reportItemCount, bool isSubtotal, bool isStatic, int subtotalHeadingIndex, int staticHeadingCount, out RenderingPagesRangesList innerPagesList, out PageTextboxes rowTextboxes)
			{
				rowTextboxes = null;
				MatrixHeadingInstance matrixHeadingInstance = null;
				bool flag = false;
				int headingCellIndex;
				if (outerGroupings)
				{
					matrixInstance.NewOuterCells();
					headingCellIndex = matrixInstance.CurrentCellOuterIndex;
					if (!headingDef.IsColumn)
					{
						processingContext.ChunkManager.CheckPageBreak(headingDef, true);
					}
				}
				else
				{
					headingCellIndex = matrixInstance.CurrentCellInnerIndex;
					if (processingContext.ReportItemsReferenced)
					{
						processingContext.DelayAddingInstanceInfo = true;
						flag = true;
					}
				}
				NonComputedUniqueNames nonCompNames = default(NonComputedUniqueNames);
				matrixHeadingInstance = new MatrixHeadingInstance(processingContext, headingCellIndex, headingDef, isSubtotal && !isStatic, reportItemCount, (VariantList)null, out nonCompNames);
				headingInstances.Add(matrixHeadingInstance, processingContext);
				if (isSubtotal && !isStatic)
				{
					if (this.m_subtotalHeadingInstances == null)
					{
						this.m_subtotalHeadingInstances = new MatrixHeadingInstance[staticHeadingCount];
					}
					this.m_subtotalHeadingInstances[subtotalHeadingIndex] = matrixHeadingInstance;
				}
				if (!isSubtotal && isStatic && Report.ShowHideTypes.Interactive == processingContext.ShowHideType)
				{
					processingContext.EnterGrouping();
					((IShowHideContainer)matrixHeadingInstance).BeginProcessContainer(processingContext);
				}
				if (headingReportItems != null)
				{
					ReportItemCollection reportItemsDef = headingReportItems.ReportItemsDef;
					int num = 0;
					bool flag2 = false;
					ReportItem reportItem = null;
					reportItemsDef.GetReportItem(reportItemCount, out flag2, out num, out reportItem);
					processingContext.PageSectionContext.EnterRepeatingItem();
					processingContext.PageSectionContext.EnterMatrixHeadingScope(Visibility.IsVisible(headingDef.Visibility, headingDef.StartHidden), headingDef.IsColumn);
					if (reportItem != null)
					{
						if (flag2)
						{
							processingContext.Pagination.EnterIgnorePageBreak(null, true);
							processingContext.Pagination.EnterIgnoreHeight(true);
							matrixHeadingInstance.Content = headingReportItems.CreateInstance(reportItem, true, true, headingDef.IsColumn);
							processingContext.Pagination.LeaveIgnoreHeight(true);
							processingContext.Pagination.LeaveIgnorePageBreak(null, true);
						}
						else
						{
							reportItem.ProcessDrillthroughAction(processingContext, nonCompNames);
							reportItem.ProcessNavigationAction(processingContext.NavigationInfo, nonCompNames, ((Matrix)headingDef.DataRegionDef).CurrentPage);
							RuntimeRICollection.AddNonComputedPageTextboxes(reportItem, ((Matrix)headingDef.DataRegionDef).CurrentPage, processingContext);
						}
					}
					processingContext.PageSectionContext.ExitMatrixHeadingScope(headingDef.IsColumn);
					PageTextboxes pageTextboxes = processingContext.PageSectionContext.ExitRepeatingItem();
					if (isStatic && isSubtotal)
					{
						pageTextboxes = null;
					}
					if (headingDef.IsColumn)
					{
						matrixInstance.MatrixDef.ColumnHeaderPageTextboxes.IntegrateNonRepeatingTextboxValues(pageTextboxes);
					}
					else
					{
						rowTextboxes = pageTextboxes;
					}
				}
				if (flag)
				{
					processingContext.DelayAddingInstanceInfo = false;
				}
				if (outerGroupings && !headingDef.IsColumn)
				{
					processingContext.ChunkManager.CheckPageBreak(headingDef, false);
				}
				innerPagesList = matrixHeadingInstance.ChildrenStartAndEndPages;
				return matrixHeadingInstance.SubHeadingInstances;
			}

			internal void ResetReportItemObjs(ProcessingContext processingContext)
			{
				if (this.m_subtotal != null)
				{
					this.m_subtotal.ResetReportItemObjs();
					for (MatrixHeading subHeading = this.m_subtotalHeadingDef.SubHeading; subHeading != null; subHeading = subHeading.SubHeading)
					{
						if (subHeading.Grouping != null)
						{
							RuntimeRICollection.ResetReportItemObjs(subHeading.ReportItems, processingContext);
						}
					}
				}
				if (this.m_staticHeading != null)
				{
					this.m_staticHeading.ResetReportItemObjs();
				}
				if (this.m_subtotalStaticHeading != null)
				{
					this.m_subtotalStaticHeading.ResetReportItemObjs();
				}
			}
		}

		private sealed class RuntimeMatrixObj : RuntimePivotObj
		{
			private RuntimeRICollection m_matrixCorner;

			private RuntimeRICollection m_subtotalCorner;

			internal RuntimeMatrixObj(IScope outerScope, Matrix matrixDef, ref DataActions dataAction, ProcessingContext processingContext, bool onePassProcess)
				: base(outerScope, (Pivot)matrixDef, ref dataAction, processingContext, onePassProcess)
			{
				bool handleMyDataAction = default(bool);
				DataActions innerDataAction = default(DataActions);
				PivotHeading outermostColumn = default(PivotHeading);
				bool outermostColumnSubtotal = default(bool);
				PivotHeading staticColumn = default(PivotHeading);
				PivotHeading outermostRow = default(PivotHeading);
				bool outermostRowSubtotal = default(bool);
				PivotHeading staticRow = default(PivotHeading);
				base.ConstructorHelper(ref dataAction, onePassProcess, out handleMyDataAction, out innerDataAction, out outermostColumn, out outermostColumnSubtotal, out staticColumn, out outermostRow, out outermostRowSubtotal, out staticRow);
				base.m_innerDataAction = innerDataAction;
				DataActions userSortDataAction = base.HandleSortFilterEvent();
				this.MatrixConstructRuntimeStructure(ref innerDataAction, onePassProcess, outermostColumn, outermostColumnSubtotal, staticColumn, outermostRow, outermostRowSubtotal, staticRow);
				base.HandleDataAction(handleMyDataAction, innerDataAction, userSortDataAction);
			}

			protected override void ConstructRuntimeStructure(ref DataActions innerDataAction)
			{
				PivotHeading outermostColumn = default(PivotHeading);
				bool outermostColumnSubtotal = default(bool);
				PivotHeading staticColumn = default(PivotHeading);
				PivotHeading outermostRow = default(PivotHeading);
				bool outermostRowSubtotal = default(bool);
				PivotHeading staticRow = default(PivotHeading);
				base.m_pivotDef.GetHeadingDefState(out outermostColumn, out outermostColumnSubtotal, out staticColumn, out outermostRow, out outermostRowSubtotal, out staticRow);
				this.MatrixConstructRuntimeStructure(ref innerDataAction, false, outermostColumn, outermostColumnSubtotal, staticColumn, outermostRow, outermostRowSubtotal, staticRow);
			}

			private void MatrixConstructRuntimeStructure(ref DataActions innerDataAction, bool onePassProcess, PivotHeading outermostColumn, bool outermostColumnSubtotal, PivotHeading staticColumn, PivotHeading outermostRow, bool outermostRowSubtotal, PivotHeading staticRow)
			{
				Matrix matrix = (Matrix)base.m_pivotDef;
				DataActions dataActions = DataActions.None;
				if (base.m_pivotDef.ProcessingInnerGrouping == Pivot.ProcessingInnerGroupings.Column)
				{
					base.m_innerGroupings = (base.m_pivotColumns = new RuntimeMatrixHeadingsObj(this, (MatrixHeading)outermostColumn, ref dataActions, base.m_processingContext, (MatrixHeading)staticColumn, null, outermostRowSubtotal, 0));
					base.m_outerGroupings = (base.m_pivotRows = new RuntimeMatrixHeadingsObj((IScope)this, (MatrixHeading)outermostRow, ref innerDataAction, base.m_processingContext, (MatrixHeading)staticRow, (RuntimeMatrixHeadingsObj)base.m_innerGroupings, outermostColumnSubtotal, 0));
				}
				else
				{
					base.m_innerGroupings = (base.m_pivotRows = new RuntimeMatrixHeadingsObj(this, (MatrixHeading)outermostRow, ref dataActions, base.m_processingContext, (MatrixHeading)staticRow, null, outermostColumnSubtotal, 0));
					base.m_outerGroupings = (base.m_pivotColumns = new RuntimeMatrixHeadingsObj((IScope)this, (MatrixHeading)outermostColumn, ref innerDataAction, base.m_processingContext, (MatrixHeading)staticColumn, (RuntimeMatrixHeadingsObj)base.m_innerGroupings, outermostRowSubtotal, 0));
				}
				if (matrix.CornerReportItems != null)
				{
					if (onePassProcess)
					{
						this.m_matrixCorner = new RuntimeRICollection(this, matrix.CornerReportItems, base.m_processingContext, true);
					}
					else
					{
						this.m_matrixCorner = new RuntimeRICollection((IScope)this, matrix.CornerReportItems, ref innerDataAction, base.m_processingContext, true);
					}
				}
				matrix.InOutermostSubtotalCell = true;
				if (onePassProcess)
				{
					this.m_subtotalCorner = new RuntimeRICollection(this, matrix.CellReportItems, base.m_processingContext, true);
				}
				else
				{
					if (outermostRowSubtotal && outermostColumnSubtotal)
					{
						goto IL_0161;
					}
					if (outermostRow == null && outermostColumn == null)
					{
						goto IL_0161;
					}
				}
				goto IL_017b;
				IL_0161:
				this.m_subtotalCorner = new RuntimeRICollection((IScope)this, matrix.CellReportItems, ref innerDataAction, base.m_processingContext, true);
				goto IL_017b;
				IL_017b:
				matrix.InOutermostSubtotalCell = false;
			}

			private bool OutermostSTCellTargetScopeMatched(int index, RuntimeSortFilterEventInfo sortFilterInfo)
			{
				VariantList[] sortSourceScopeInfo = sortFilterInfo.SortSourceScopeInfo;
				PivotHeading pivotHeading = base.m_pivotDef.GetPivotHeading(false);
				PivotHeading pivotHeading2 = null;
				base.m_pivotDef.SkipStaticHeading(ref pivotHeading, ref pivotHeading2);
				if (pivotHeading != null)
				{
					Grouping grouping = pivotHeading.Grouping;
					if (grouping.IsOnPathToSortFilterSource(index))
					{
						int dynamicHeadingCount = base.m_pivotDef.GetDynamicHeadingCount(false);
						int num = grouping.SortFilterScopeIndex[index];
						int num2 = 0;
						while (num2 < dynamicHeadingCount && num < sortSourceScopeInfo.Length)
						{
							if (sortSourceScopeInfo[num] != null)
							{
								return false;
							}
							num2++;
							num++;
						}
					}
				}
				PivotHeading pivotHeading3 = base.m_pivotDef.GetPivotHeading(true);
				base.m_pivotDef.SkipStaticHeading(ref pivotHeading3, ref pivotHeading2);
				if (pivotHeading3 != null)
				{
					Grouping grouping2 = pivotHeading3.Grouping;
					if (grouping2.IsOnPathToSortFilterSource(index))
					{
						int dynamicHeadingCount2 = base.m_pivotDef.GetDynamicHeadingCount(true);
						int num = grouping2.SortFilterScopeIndex[index];
						int num3 = 0;
						while (num3 < dynamicHeadingCount2 && num < sortSourceScopeInfo.Length)
						{
							if (sortSourceScopeInfo[num] != null)
							{
								return false;
							}
							num3++;
							num++;
						}
					}
				}
				return true;
			}

			internal override bool TargetScopeMatched(int index, bool detailSort)
			{
				if (((Matrix)base.m_pivotDef).InOutermostSubtotalCell && !this.OutermostSTCellTargetScopeMatched(index, base.m_processingContext.RuntimeSortFilterInfo[index]))
				{
					return false;
				}
				return base.TargetScopeMatched(index, detailSort);
			}

			private void GetScopeValuesForOutermostSTCell(IHierarchyObj targetScopeObj, VariantList[] scopeValues, ref int index)
			{
				int dynamicHeadingCount = base.m_pivotDef.GetDynamicHeadingCount(false);
				for (int i = 0; i < dynamicHeadingCount; i++)
				{
					Global.Tracer.Assert(index < scopeValues.Length, "Inner headings scope values");
					scopeValues[index++] = null;
				}
				dynamicHeadingCount = base.m_pivotDef.GetDynamicHeadingCount(true);
				for (int j = 0; j < dynamicHeadingCount; j++)
				{
					Global.Tracer.Assert(index < scopeValues.Length, "Outer headings scope values");
					scopeValues[index++] = null;
				}
			}

			internal override void GetScopeValues(IHierarchyObj targetScopeObj, VariantList[] scopeValues, ref int index)
			{
				base.GetScopeValues(targetScopeObj, scopeValues, ref index);
				if (((Matrix)base.m_pivotDef).InOutermostSubtotalCell)
				{
					this.GetScopeValuesForOutermostSTCell(targetScopeObj, scopeValues, ref index);
				}
			}

			protected override void SendToInner()
			{
				base.SendToInner();
				if (this.m_matrixCorner != null)
				{
					this.m_matrixCorner.FirstPassNextDataRow();
				}
				if (this.m_subtotalCorner != null)
				{
					((Matrix)base.m_pivotDef).InOutermostSubtotalCell = true;
					this.m_subtotalCorner.FirstPassNextDataRow();
					((Matrix)base.m_pivotDef).InOutermostSubtotalCell = false;
				}
			}

			internal override bool SortAndFilter()
			{
				this.SetupEnvironment();
				if (base.m_userSortTargetInfo != null)
				{
					base.m_userSortTargetInfo.EnterProcessUserSortPhase(base.m_processingContext);
				}
				base.SortAndFilter();
				if (this.m_matrixCorner != null)
				{
					this.m_matrixCorner.SortAndFilter();
				}
				if (this.m_subtotalCorner != null)
				{
					this.m_subtotalCorner.SortAndFilter();
				}
				if (base.m_userSortTargetInfo != null)
				{
					base.m_userSortTargetInfo.LeaveProcessUserSortPhase(base.m_processingContext);
				}
				return true;
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				base.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				if (this.m_matrixCorner != null)
				{
					this.m_matrixCorner.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				}
				if (this.m_subtotalCorner != null)
				{
					this.m_subtotalCorner.CalculateInnerRunningValues(globalRVCol, groupCol, lastGroup);
				}
				RuntimeRICollection.DoneReadingRows(globalRVCol, base.m_pivotDef.RunningValues, ref base.m_runningValueValues, false);
			}

			protected override void CalculatePreviousAggregates()
			{
				if (!base.m_processedPreviousAggregates && base.m_processingContext.GlobalRVCollection != null)
				{
					base.CalculatePreviousAggregates();
					if (this.m_matrixCorner != null)
					{
						this.m_matrixCorner.CalculatePreviousAggregates(base.m_processingContext.GlobalRVCollection);
					}
				}
			}

			internal override void CreateInstances(ReportItemInstance riInstance, IList instanceList, RenderingPagesRangesList pagesList)
			{
				if (base.m_firstRow != null)
				{
					MatrixInstance matrixInstance = (MatrixInstance)riInstance;
					Matrix matrix = (Matrix)base.m_pivotDef;
					matrix.InitializePageSectionProcessing();
					PageSectionContext pageSectionContext = base.m_processingContext.PageSectionContext;
					base.m_processingContext.PageSectionContext = new PageSectionContext(pageSectionContext);
					base.m_processingContext.PageSectionContext.EnterVisibilityScope(matrix.Visibility, matrix.StartHidden);
					PageTextboxes source = null;
					if (this.m_matrixCorner != null)
					{
						ReportItemCollection cornerReportItems = matrix.CornerReportItems;
						int num = 0;
						bool flag = false;
						ReportItem reportItem = null;
						cornerReportItems.GetReportItem(0, out flag, out num, out reportItem);
						base.m_processingContext.PageSectionContext.EnterRepeatingItem();
						if (reportItem != null)
						{
							if (flag)
							{
								base.m_processingContext.Pagination.EnterIgnorePageBreak(null, true);
								base.m_processingContext.Pagination.EnterIgnoreHeight(true);
								matrixInstance.CornerContent = this.m_matrixCorner.CreateInstance(reportItem, true, true, true);
								base.m_processingContext.Pagination.LeaveIgnoreHeight(true);
								base.m_processingContext.Pagination.LeaveIgnorePageBreak(null, true);
							}
							else
							{
								NonComputedUniqueNames cornerNonComputedUniqueNames = matrix.CornerNonComputedUniqueNames;
								reportItem.ProcessDrillthroughAction(base.m_processingContext, cornerNonComputedUniqueNames);
								reportItem.ProcessNavigationAction(base.m_processingContext.NavigationInfo, cornerNonComputedUniqueNames, matrix.CurrentPage);
								RuntimeRICollection.AddNonComputedPageTextboxes(reportItem, matrix.CurrentPage, base.m_processingContext);
							}
						}
						source = base.m_processingContext.PageSectionContext.ExitRepeatingItem();
					}
					if (Report.ShowHideTypes.Interactive == base.m_processingContext.ShowHideType)
					{
						base.m_processingContext.RegisterIgnoreRange();
					}
					bool delayAddingInstanceInfo = base.m_processingContext.DelayAddingInstanceInfo;
					base.m_processingContext.DelayAddingInstanceInfo = false;
					int siblingIndex = default(int);
					int nodeIndex = default(int);
					base.m_processingContext.NavigationInfo.GetCurrentDocumentMapPosition(out siblingIndex, out nodeIndex);
					if (base.m_outerGroupings == base.m_pivotRows)
					{
						matrixInstance.InnerHeadingInstanceList = matrixInstance.ColumnInstances;
						((RuntimeMatrixHeadingsObj)base.m_outerGroupings).CreateInstances(this, base.m_processingContext, matrixInstance, true, null, matrixInstance.RowInstances, pagesList);
					}
					else
					{
						matrixInstance.InnerHeadingInstanceList = matrixInstance.RowInstances;
						((RuntimeMatrixHeadingsObj)base.m_outerGroupings).CreateInstances(this, base.m_processingContext, matrixInstance, true, null, matrixInstance.ColumnInstances, pagesList);
					}
					base.m_processingContext.DelayAddingInstanceInfo = delayAddingInstanceInfo;
					if (Report.ShowHideTypes.Interactive == base.m_processingContext.ShowHideType)
					{
						base.m_processingContext.UnRegisterIgnoreRange();
					}
					base.m_processingContext.NavigationInfo.InsertMatrixColumnDocumentMap(siblingIndex, nodeIndex);
					if (base.m_processingContext.ReportItemsReferenced)
					{
						this.AddInnerHeadingsToChunk(matrixInstance.InnerHeadingInstanceList, (byte)(matrixInstance.InFirstPage ? 1 : 0) != 0);
					}
					int count = matrixInstance.ChildrenStartAndEndPages.Count;
					if (count > 0)
					{
						matrix.EndPage = matrixInstance.ChildrenStartAndEndPages[count - 1].EndPage;
					}
					else
					{
						matrix.EndPage = ((IPageItem)matrixInstance).StartPage;
					}
					base.m_processingContext.Pagination.ProcessEndPage(matrixInstance, matrix, matrix.PageBreakAtEnd || matrix.PropagatedPageBreakAtEnd, matrixInstance.NumberOfChildrenOnThisPage > 0);
					base.m_processingContext.PageSectionContext.ExitVisibilityScope();
					if (base.m_processingContext.PageSectionContext.PageTextboxes != null)
					{
						Global.Tracer.Assert(0 == base.m_processingContext.PageSectionContext.PageTextboxes.GetPageCount(), "Invalid state");
						pageSectionContext.PageTextboxes.IntegrateNonRepeatingTextboxValues(base.m_processingContext.PageSectionContext.PageTextboxes);
					}
					base.m_processingContext.PageSectionContext = pageSectionContext;
					if (base.m_processingContext.PageSectionContext.PageTextboxes != null)
					{
						base.m_processingContext.PageSectionContext.PageTextboxes.IntegrateRepeatingTextboxValues(source, matrix.StartPage, matrix.EndPage);
						base.m_processingContext.PageSectionContext.PageTextboxes.IntegrateRepeatingTextboxValues(matrixInstance.MatrixDef.ColumnHeaderPageTextboxes, matrix.StartPage, matrix.EndPage);
						base.m_processingContext.PageSectionContext.PageTextboxes.IntegrateNonRepeatingTextboxValues(matrixInstance.MatrixDef.RowHeaderPageTextboxes);
						base.m_processingContext.PageSectionContext.PageTextboxes.IntegrateNonRepeatingTextboxValues(matrixInstance.MatrixDef.CellPageTextboxes);
					}
				}
			}

			internal void ResetReportItems()
			{
				if (this.m_matrixCorner != null)
				{
					this.m_matrixCorner.ResetReportItemObjs();
				}
				if (base.m_pivotRows != null)
				{
					((RuntimeMatrixHeadingsObj)base.m_pivotRows).ResetReportItemObjs(base.m_processingContext);
				}
				if (base.m_pivotColumns != null)
				{
					((RuntimeMatrixHeadingsObj)base.m_pivotColumns).ResetReportItemObjs(base.m_processingContext);
				}
			}

			internal void CreateOutermostSubtotalCells(MatrixInstance matrixInstance, bool outerGroupings)
			{
				if (outerGroupings)
				{
					this.SetupEnvironment();
					if (Report.ShowHideTypes.Interactive == base.m_processingContext.ShowHideType)
					{
						base.m_processingContext.EndIgnoreRange();
					}
					((RuntimeMatrixHeadingsObj)base.m_innerGroupings).CreateInstances(this, base.m_processingContext, matrixInstance, false, null, matrixInstance.InnerHeadingInstanceList, matrixInstance.ChildrenStartAndEndPages);
					if (Report.ShowHideTypes.Interactive == base.m_processingContext.ShowHideType)
					{
						base.m_processingContext.UseAllContainers = true;
					}
				}
				else if (this.m_subtotalCorner != null)
				{
					if (Report.ShowHideTypes.Interactive == base.m_processingContext.ShowHideType)
					{
						base.m_processingContext.IgnoreAllFromStart = true;
					}
					bool inMatrixSubtotal = base.m_processingContext.PageSectionContext.InMatrixSubtotal;
					base.m_processingContext.PageSectionContext.InMatrixSubtotal = true;
					bool flag = default(bool);
					ReportItem cellReportItemDef = matrixInstance.GetCellReportItemDef(-1, out flag);
					NonComputedUniqueNames nonCompNames = default(NonComputedUniqueNames);
					MatrixCellInstance matrixCellInstance = matrixInstance.AddCell(base.m_processingContext, out nonCompNames);
					if (cellReportItemDef != null)
					{
						if (flag)
						{
							this.SetupEnvironment();
							base.m_processingContext.Pagination.EnterIgnorePageBreak(null, true);
							base.m_processingContext.Pagination.EnterIgnoreHeight(true);
							((Matrix)base.m_pivotDef).InOutermostSubtotalCell = true;
							matrixCellInstance.Content = this.m_subtotalCorner.CreateInstance(cellReportItemDef, true, true, false);
							((Matrix)base.m_pivotDef).InOutermostSubtotalCell = false;
							base.m_processingContext.Pagination.LeaveIgnoreHeight(true);
							base.m_processingContext.Pagination.LeaveIgnorePageBreak(null, true);
							this.m_subtotalCorner.ResetReportItemObjs();
						}
						else
						{
							cellReportItemDef.ProcessDrillthroughAction(base.m_processingContext, nonCompNames);
							cellReportItemDef.ProcessNavigationAction(base.m_processingContext.NavigationInfo, nonCompNames, ((Matrix)matrixInstance.ReportItemDef).CurrentPage);
						}
					}
					base.m_processingContext.PageSectionContext.InMatrixSubtotal = inMatrixSubtotal;
					if (Report.ShowHideTypes.Interactive == base.m_processingContext.ShowHideType)
					{
						base.m_processingContext.IgnoreAllFromStart = false;
					}
				}
			}

			private void AddInnerHeadingsToChunk(MatrixHeadingInstanceList headings, bool addToFirstPage)
			{
				if (headings != null)
				{
					for (int i = 0; i < headings.Count; i++)
					{
						MatrixHeadingInstance matrixHeadingInstance = headings[i];
						base.m_processingContext.ChunkManager.AddInstance(matrixHeadingInstance.InstanceInfo, matrixHeadingInstance, addToFirstPage, base.m_processingContext.InPageSection);
						if (matrixHeadingInstance.Content != null)
						{
							this.AddInnerHeadingsToChunk(matrixHeadingInstance.Content, addToFirstPage);
						}
						this.AddInnerHeadingsToChunk(matrixHeadingInstance.SubHeadingInstances, addToFirstPage);
					}
				}
			}

			private void AddInnerHeadingsToChunk(ReportItemColInstance reportItemColInstance, bool addToFirstPage)
			{
				ReportItemColInstanceInfo instanceInfo = reportItemColInstance.GetInstanceInfo(null, base.m_processingContext.InPageSection);
				base.m_processingContext.ChunkManager.AddInstance(instanceInfo, reportItemColInstance, addToFirstPage, base.m_processingContext.InPageSection);
				if (reportItemColInstance.ReportItemInstances != null)
				{
					for (int i = 0; i < reportItemColInstance.ReportItemInstances.Count; i++)
					{
						this.AddInnerHeadingsToChunk(reportItemColInstance[i], addToFirstPage);
					}
				}
			}

			private void AddInnerHeadingsToChunk(ReportItemInstance reportItemInstance, bool addToFirstPage)
			{
				if (reportItemInstance is TextBoxInstance)
				{
					base.m_processingContext.ChunkManager.AddInstance(((TextBoxInstance)reportItemInstance).InstanceInfo, reportItemInstance, addToFirstPage, base.m_processingContext.InPageSection);
				}
				else
				{
					ReportItemInstanceInfo instanceInfo = reportItemInstance.GetInstanceInfo(null);
					base.m_processingContext.ChunkManager.AddInstance(instanceInfo, reportItemInstance, addToFirstPage, base.m_processingContext.InPageSection);
					if (reportItemInstance is RectangleInstance)
					{
						this.AddInnerHeadingsToChunk(((RectangleInstance)reportItemInstance).ReportItemColInstance, addToFirstPage);
					}
					else if (reportItemInstance is MatrixInstance)
					{
						this.AddInnerHeadingsToChunk(((MatrixInstance)reportItemInstance).CornerContent, addToFirstPage);
					}
					else if (reportItemInstance is TableInstance)
					{
						TableInstance tableInstance = (TableInstance)reportItemInstance;
						if (tableInstance.HeaderRowInstances != null)
						{
							for (int i = 0; i < tableInstance.HeaderRowInstances.Length; i++)
							{
								TableRowInstance tableRowInstance = tableInstance.HeaderRowInstances[i];
								base.m_processingContext.ChunkManager.AddInstance(tableRowInstance.GetInstanceInfo(null), tableRowInstance, addToFirstPage, base.m_processingContext.InPageSection);
								this.AddInnerHeadingsToChunk(tableRowInstance.TableRowReportItemColInstance, addToFirstPage);
							}
						}
						if (tableInstance.FooterRowInstances != null)
						{
							for (int j = 0; j < tableInstance.FooterRowInstances.Length; j++)
							{
								TableRowInstance tableRowInstance2 = tableInstance.FooterRowInstances[j];
								base.m_processingContext.ChunkManager.AddInstance(tableRowInstance2.GetInstanceInfo(null), tableRowInstance2, addToFirstPage, base.m_processingContext.InPageSection);
								this.AddInnerHeadingsToChunk(tableRowInstance2.TableRowReportItemColInstance, addToFirstPage);
							}
						}
					}
				}
			}
		}

		private sealed class RuntimeMatrixGroupRootObj : RuntimePivotGroupRootObj
		{
			private ReportItemCollection m_cellRIs;

			internal ReportItemCollection CellRIs
			{
				get
				{
					return this.m_cellRIs;
				}
			}

			internal RuntimeMatrixGroupRootObj(IScope outerScope, MatrixHeading matrixHeadingDef, ref DataActions dataAction, ProcessingContext processingContext, RuntimeMatrixHeadingsObj innerGroupings, bool outermostSubtotal, int headingLevel)
				: base(outerScope, (PivotHeading)matrixHeadingDef, ref dataAction, processingContext, (RuntimePivotHeadingsObj)innerGroupings, outermostSubtotal, headingLevel)
			{
				if (base.m_processOutermostSTCells)
				{
					Matrix matrix = (Matrix)matrixHeadingDef.DataRegionDef;
					this.m_cellRIs = matrix.CellReportItems;
					if (this.m_cellRIs.RunningValues != null && 0 < this.m_cellRIs.RunningValues.Count)
					{
						base.m_dataAction |= DataActions.PostSortAggregates;
					}
				}
				if (matrixHeadingDef.OwcGroupExpression)
				{
					base.m_saveGroupExprValues = true;
				}
			}

			protected override void NeedProcessDataActions(PivotHeading heading)
			{
				MatrixHeading matrixHeading = (MatrixHeading)heading;
				if (matrixHeading != null)
				{
					base.NeedProcessDataActions(matrixHeading.ReportItems.RunningValues);
				}
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				base.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				ReportItemCollection reportItems = ((MatrixHeading)base.m_hierarchyDef).ReportItems;
				if (reportItems != null)
				{
					base.AddRunningValues(reportItems.RunningValues);
				}
				if (base.m_staticHeadingDef != null)
				{
					base.AddRunningValues(((MatrixHeading)base.m_staticHeadingDef).ReportItems.RunningValues);
				}
				if (base.m_innerSubtotal != null)
				{
					base.AddRunningValues(base.m_innerSubtotal.ReportItems.RunningValues);
				}
				if (base.m_innerSubtotalStaticHeading != null)
				{
					base.AddRunningValues(((MatrixHeading)base.m_innerSubtotalStaticHeading).ReportItems.RunningValues);
				}
				base.m_grouping.Traverse(ProcessingStages.RunningValues, base.m_expression.Direction);
				if (base.m_hierarchyDef.Grouping.Name != null)
				{
					groupCol.Remove(base.m_hierarchyDef.Grouping.Name);
				}
			}

			protected override void AddCellRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, ref DataAggregateObjList runningValues)
			{
				Matrix matrix = (Matrix)base.m_hierarchyDef.DataRegionDef;
				ReportItemCollection cellReportItems = matrix.CellReportItems;
				if (cellReportItems.RunningValues != null && 0 < cellReportItems.RunningValues.Count)
				{
					base.m_dataAction |= DataActions.PostSortAggregates;
					if (runningValues == null)
					{
						base.AddRunningValues(cellReportItems.RunningValues, ref runningValues, globalRVCol, groupCol);
					}
				}
			}
		}

		private sealed class RuntimeMatrixCell : RuntimePivotCell
		{
			private ReportItemCollection m_cellDef;

			private RuntimeRICollection m_cellReportItems;

			internal RuntimeMatrixCell(RuntimeMatrixGroupLeafObj owner, int cellLevel, DataAggregateInfoList aggDefs, ReportItemCollection cellDef, bool innermost)
				: base(owner, cellLevel, aggDefs, innermost)
			{
				this.m_cellDef = cellDef;
				DataActions dataActions = DataActions.None;
				bool flag = this.m_cellDef.RunningValues != null && 0 < this.m_cellDef.RunningValues.Count;
				if (base.m_innermost && (flag || base.m_owner.CellPostSortAggregates != null))
				{
					dataActions = DataActions.PostSortAggregates;
				}
				this.HandleSortFilterEvent();
				this.m_cellReportItems = new RuntimeRICollection(this, this.m_cellDef, ref dataActions, base.m_owner.ProcessingContext, true);
				if (dataActions != 0)
				{
					base.m_dataRows = new DataRowList();
				}
			}

			private void HandleSortFilterEvent()
			{
				if (base.m_owner.NeedHandleCellSortFilterEvent())
				{
					int count = base.m_owner.ProcessingContext.RuntimeSortFilterInfo.Count;
					for (int i = 0; i < count; i++)
					{
						RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = base.m_owner.ProcessingContext.RuntimeSortFilterInfo[i];
						if (runtimeSortFilterEventInfo.EventSource.IsMatrixCellScope)
						{
							ReportItem parent = runtimeSortFilterEventInfo.EventSource.Parent;
							while (parent != null && !(parent is Matrix))
							{
								parent = parent.Parent;
							}
							if (parent == base.m_owner.PivotDef && ((IScope)this).TargetScopeMatched(i, false) && !base.m_owner.GetOwnerPivot().TargetForNonDetailSort && runtimeSortFilterEventInfo.EventSourceScope == null)
							{
								runtimeSortFilterEventInfo.EventSourceScope = this;
							}
						}
					}
				}
			}

			internal override void NextRow()
			{
				base.NextRow();
				this.m_cellReportItems.FirstPassNextDataRow();
			}

			internal override void SortAndFilter()
			{
				this.m_cellReportItems.SortAndFilter();
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				base.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				this.m_cellReportItems.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
			}

			internal void CreateInstance(MatrixInstance matrixInstance, ReportItem reportItemDef, MatrixCellInstance cellInstance)
			{
				base.SetupEnvironment();
				base.m_owner.ProcessingContext.Pagination.EnterIgnorePageBreak(null, true);
				base.m_owner.ProcessingContext.Pagination.EnterIgnoreHeight(true);
				cellInstance.Content = this.m_cellReportItems.CreateInstance(reportItemDef, true, true, false);
				base.m_owner.ProcessingContext.Pagination.LeaveIgnoreHeight(true);
				base.m_owner.ProcessingContext.Pagination.LeaveIgnorePageBreak(null, true);
				this.m_cellReportItems.ResetReportItemObjs();
			}
		}

		private sealed class RuntimeMatrixGroupLeafObj : RuntimePivotGroupLeafObj
		{
			private RuntimeRICollection m_headingReportItemCol;

			private RuntimeRICollection m_firstPassCell;

			private MatrixHeadingInstance m_headingInstance;

			private string m_label;

			private int m_startPage = -1;

			private bool m_startHidden;

			internal RuntimeMatrixGroupLeafObj(RuntimeMatrixGroupRootObj groupRoot)
				: base(groupRoot)
			{
				MatrixHeading matrixHeading = (MatrixHeading)groupRoot.HierarchyDef;
				Matrix pivotDef = (Matrix)matrixHeading.DataRegionDef;
				MatrixHeading headingDef = (MatrixHeading)groupRoot.InnerHeading;
				bool flag = false;
				bool flag2 = base.HandleSortFilterEvent();
				DataActions dataAction = default(DataActions);
				base.ConstructorHelper((RuntimePivotGroupRootObj)groupRoot, (Pivot)pivotDef, out flag, out dataAction);
				base.m_pivotHeadings = new RuntimeMatrixHeadingsObj(this, headingDef, ref dataAction, groupRoot.ProcessingContext, (MatrixHeading)groupRoot.StaticHeadingDef, (RuntimeMatrixHeadingsObj)groupRoot.InnerGroupings, groupRoot.OutermostSubtotal, groupRoot.HeadingLevel + 1);
				base.m_innerHierarchy = base.m_pivotHeadings.Headings;
				if (matrixHeading.ReportItems != null)
				{
					this.m_headingReportItemCol = new RuntimeRICollection(this, matrixHeading.ReportItems, ref dataAction, base.m_processingContext, true);
				}
				if (groupRoot.CellRIs != null)
				{
					DataActions dataActions = DataActions.None;
					matrixHeading.InOutermostSubtotalCell = true;
					this.m_firstPassCell = new RuntimeRICollection(this, groupRoot.CellRIs, ref dataActions, base.m_processingContext, true);
					matrixHeading.InOutermostSubtotalCell = false;
				}
				if (!flag)
				{
					base.m_dataAction = dataAction;
				}
				if (flag2)
				{
					base.m_dataAction |= DataActions.UserSort;
				}
				if (this.m_firstPassCell != null)
				{
					this.HandleOutermostSTCellSortFilterEvent();
				}
				if (base.m_dataAction != 0)
				{
					base.m_dataRows = new DataRowList();
				}
			}

			private void HandleOutermostSTCellSortFilterEvent()
			{
				if (base.NeedHandleCellSortFilterEvent())
				{
					int count = base.m_processingContext.RuntimeSortFilterInfo.Count;
					for (int i = 0; i < count; i++)
					{
						if (base.GroupingDef.IsOnPathToSortFilterSource(i))
						{
							RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = base.m_processingContext.RuntimeSortFilterInfo[i];
							if (base.m_targetScopeMatched[i] && runtimeSortFilterEventInfo.EventSource.IsMatrixCellScope)
							{
								ReportItem parent = runtimeSortFilterEventInfo.EventSource.Parent;
								while (parent != null && !(parent is Matrix))
								{
									parent = parent.Parent;
								}
								if (parent == base.PivotDef && this.OutermostSTCellTargetScopeMatched(i, runtimeSortFilterEventInfo) && !base.GetOwnerPivot().TargetForNonDetailSort && runtimeSortFilterEventInfo.EventSourceScope == null)
								{
									runtimeSortFilterEventInfo.EventSourceScope = this;
								}
							}
						}
					}
				}
			}

			private bool OutermostSTCellTargetScopeMatched(int index, RuntimeSortFilterEventInfo sortFilterInfo)
			{
				Pivot pivotDef = base.PivotDef;
				int dynamicHeadingCount = pivotDef.GetDynamicHeadingCount(false);
				int dynamicHeadingCount2 = pivotDef.GetDynamicHeadingCount(true);
				VariantList[] sortSourceScopeInfo = sortFilterInfo.SortSourceScopeInfo;
				if (base.IsOuterGrouping())
				{
					PivotHeading pivotHeading = pivotDef.GetPivotHeading(false);
					PivotHeading pivotHeading2 = null;
					pivotDef.SkipStaticHeading(ref pivotHeading, ref pivotHeading2);
					if (pivotHeading != null)
					{
						Grouping grouping = pivotHeading.Grouping;
						if (grouping.IsOnPathToSortFilterSource(index))
						{
							int num = grouping.SortFilterScopeIndex[index];
							int num2 = 0;
							while (num2 < dynamicHeadingCount && num < sortSourceScopeInfo.Length)
							{
								if (sortSourceScopeInfo[num] != null)
								{
									return false;
								}
								num2++;
								num++;
							}
						}
					}
					if (base.GroupingDef.IsOnPathToSortFilterSource(index))
					{
						int num = base.GroupingDef.SortFilterScopeIndex[index] + 1;
						int num3 = base.HeadingLevel + 1;
						while (num3 < dynamicHeadingCount2 && num < sortSourceScopeInfo.Length)
						{
							if (sortSourceScopeInfo[num] != null)
							{
								return false;
							}
							num3++;
							num++;
						}
					}
				}
				else
				{
					if (base.GroupingDef.IsOnPathToSortFilterSource(index))
					{
						int num = base.GroupingDef.SortFilterScopeIndex[index] + 1;
						int num4 = base.HeadingLevel + 1;
						while (num4 < dynamicHeadingCount && num < sortSourceScopeInfo.Length)
						{
							if (sortSourceScopeInfo[num] != null)
							{
								return false;
							}
							num4++;
							num++;
						}
					}
					PivotHeading pivotHeading3 = pivotDef.GetPivotHeading(true);
					PivotHeading pivotHeading4 = null;
					pivotDef.SkipStaticHeading(ref pivotHeading3, ref pivotHeading4);
					if (pivotHeading3 != null)
					{
						Grouping grouping2 = pivotHeading3.Grouping;
						if (grouping2.IsOnPathToSortFilterSource(index))
						{
							int num = grouping2.SortFilterScopeIndex[index];
							int num5 = 0;
							while (num5 < dynamicHeadingCount2 && num < sortSourceScopeInfo.Length)
							{
								if (sortSourceScopeInfo[num] != null)
								{
									return false;
								}
								num5++;
								num++;
							}
						}
					}
				}
				return true;
			}

			internal override bool TargetScopeMatched(int index, bool detailSort)
			{
				if (detailSort && base.GroupingDef.SortFilterScopeInfo == null)
				{
					return true;
				}
				if (base.m_targetScopeMatched != null && base.m_targetScopeMatched[index])
				{
					if (((MatrixHeading)base.PivotHeadingDef).InOutermostSubtotalCell)
					{
						return this.OutermostSTCellTargetScopeMatched(index, base.m_processingContext.RuntimeSortFilterInfo[index]);
					}
					return true;
				}
				return false;
			}

			internal override RuntimePivotCell CreateCell(int index, Pivot pivotDef)
			{
				return new RuntimeMatrixCell(this, index, pivotDef.CellAggregates, ((Matrix)pivotDef).CellReportItems, null == base.m_innerHierarchy);
			}

			protected override void SendToInner()
			{
				base.SendToInner();
				if (this.m_headingReportItemCol != null)
				{
					this.m_headingReportItemCol.FirstPassNextDataRow();
				}
				if (this.m_firstPassCell != null)
				{
					((MatrixHeading)base.PivotHeadingDef).InOutermostSubtotalCell = true;
					this.m_firstPassCell.FirstPassNextDataRow();
					((MatrixHeading)base.PivotHeadingDef).InOutermostSubtotalCell = false;
				}
			}

			internal override bool SortAndFilter()
			{
				this.SetupEnvironment();
				if (base.m_userSortTargetInfo != null)
				{
					base.m_userSortTargetInfo.EnterProcessUserSortPhase(base.m_processingContext);
				}
				if (this.m_headingReportItemCol != null)
				{
					this.m_headingReportItemCol.SortAndFilter();
				}
				if (this.m_firstPassCell != null)
				{
					this.m_firstPassCell.SortAndFilter();
				}
				bool result = base.SortAndFilter();
				if (base.m_userSortTargetInfo != null)
				{
					base.m_userSortTargetInfo.LeaveProcessUserSortPhase(base.m_processingContext);
				}
				return result;
			}

			internal override void CalculateRunningValues()
			{
				base.CalculateRunningValues();
				if (base.m_processHeading)
				{
					RuntimePivotGroupRootObj runtimePivotGroupRootObj = (RuntimePivotGroupRootObj)base.m_hierarchyRoot;
					AggregatesImpl globalRunningValueCollection = runtimePivotGroupRootObj.GlobalRunningValueCollection;
					RuntimeGroupRootObjList groupCollection = runtimePivotGroupRootObj.GroupCollection;
					if (this.m_headingReportItemCol != null)
					{
						this.m_headingReportItemCol.CalculateRunningValues(globalRunningValueCollection, groupCollection, runtimePivotGroupRootObj);
					}
					if (this.m_firstPassCell != null)
					{
						Matrix matrix = (Matrix)base.PivotDef;
						base.m_processingContext.EnterPivotCell(true);
						this.m_firstPassCell.CalculateRunningValues(runtimePivotGroupRootObj.OutermostSTCellRVCol, groupCollection, runtimePivotGroupRootObj);
						base.m_processingContext.ExitPivotCell();
					}
					base.m_processHeading = false;
				}
				this.ResetScopedRunningValues();
			}

			protected override bool CalculatePreviousAggregates()
			{
				if (base.CalculatePreviousAggregates() && this.m_headingReportItemCol != null)
				{
					this.m_headingReportItemCol.CalculatePreviousAggregates(base.m_processingContext.GlobalRVCollection);
					return true;
				}
				return false;
			}

			internal void SetContentsPage()
			{
				RuntimeMatrixGroupRootObj runtimeMatrixGroupRootObj = (RuntimeMatrixGroupRootObj)base.m_hierarchyRoot;
				MatrixHeading matrixHeading = (MatrixHeading)runtimeMatrixGroupRootObj.HierarchyDef;
				if (!matrixHeading.IsColumn)
				{
					((MatrixInstance)runtimeMatrixGroupRootObj.ReportItemInstance).MatrixDef.CellPage = this.m_startPage;
				}
			}

			internal override void CreateInstance()
			{
				this.SetupEnvironment();
				RuntimeMatrixGroupRootObj runtimeMatrixGroupRootObj = (RuntimeMatrixGroupRootObj)base.m_hierarchyRoot;
				Matrix matrix = (Matrix)base.PivotDef;
				MatrixInstance matrixInstance = (MatrixInstance)runtimeMatrixGroupRootObj.ReportItemInstance;
				MatrixHeadingInstanceList matrixHeadingInstanceList = (MatrixHeadingInstanceList)runtimeMatrixGroupRootObj.InstanceList;
				MatrixHeading matrixHeading = (MatrixHeading)runtimeMatrixGroupRootObj.HierarchyDef;
				bool flag = false;
				bool flag2 = base.IsOuterGrouping();
				RenderingPagesRangesList pagesList = runtimeMatrixGroupRootObj.PagesList;
				PageTextboxes pageTextboxes = null;
				if (base.m_targetScopeMatched != null)
				{
					matrixHeading.Grouping.SortFilterScopeMatched = base.m_targetScopeMatched;
				}
				RuntimePivotGroupRootObj currOuterHeadingGroupRoot;
				int headingCellIndex;
				if (flag2)
				{
					currOuterHeadingGroupRoot = (matrix.CurrentOuterHeadingGroupRoot = runtimeMatrixGroupRootObj);
					matrix.OuterGroupingIndexes[runtimeMatrixGroupRootObj.HeadingLevel] = base.m_groupLeafIndex;
					matrixInstance.NewOuterCells();
					headingCellIndex = matrixInstance.CurrentCellOuterIndex;
					if (!matrixHeading.IsColumn)
					{
						base.m_processingContext.ChunkManager.CheckPageBreak(matrixHeading, true);
					}
				}
				else
				{
					currOuterHeadingGroupRoot = matrix.CurrentOuterHeadingGroupRoot;
					headingCellIndex = matrixInstance.CurrentCellInnerIndex;
				}
				RenderingPagesRanges renderingPagesRanges = default(RenderingPagesRanges);
				if (flag2 || matrixInstance.CurrentCellOuterIndex == 0)
				{
					if (matrixHeading.IsColumn)
					{
						base.m_processingContext.NavigationInfo.EnterMatrixColumn();
					}
					else
					{
						base.m_processingContext.Pagination.EnterIgnorePageBreak(matrixHeading.Visibility, false);
						if (!base.m_processingContext.Pagination.IgnorePageBreak && matrixHeadingInstanceList.Count != 0 && matrixHeading.Grouping.PageBreakAtStart && matrixInstance.NumberOfChildrenOnThisPage > 0)
						{
							base.m_processingContext.Pagination.SetCurrentPageHeight(matrix, 0.0);
							matrixInstance.ExtraPagesFilled++;
							matrix.CurrentPage++;
							matrixInstance.NumberOfChildrenOnThisPage = 0;
						}
						renderingPagesRanges.StartPage = matrix.CurrentPage;
						this.m_startPage = renderingPagesRanges.StartPage;
					}
					flag = true;
					if (!flag2 && base.m_processingContext.ReportItemsReferenced)
					{
						base.m_processingContext.DelayAddingInstanceInfo = true;
					}
					NonComputedUniqueNames nonCompNames = default(NonComputedUniqueNames);
					this.m_headingInstance = new MatrixHeadingInstance(base.m_processingContext, headingCellIndex, matrixHeading, false, 0, base.m_groupExprValues, out nonCompNames);
					this.m_startHidden = matrixHeading.StartHidden;
					matrixHeadingInstanceList.Add(this.m_headingInstance, base.m_processingContext);
					matrixHeadingInstanceList = this.m_headingInstance.SubHeadingInstances;
					pagesList = this.m_headingInstance.ChildrenStartAndEndPages;
					if (Report.ShowHideTypes.Interactive == base.m_processingContext.ShowHideType)
					{
						base.m_processingContext.EnterGrouping();
						((IShowHideContainer)this.m_headingInstance).BeginProcessContainer(base.m_processingContext);
					}
					if (matrixHeading.Grouping.GroupLabel != null)
					{
						this.m_label = base.m_processingContext.NavigationInfo.CurrentLabel;
						if (this.m_label != null)
						{
							base.m_processingContext.NavigationInfo.EnterDocumentMapChildren();
						}
					}
					if (this.m_headingReportItemCol != null)
					{
						bool flag3 = default(bool);
						ReportItem content = matrixHeading.GetContent(out flag3);
						if (content != null)
						{
							base.m_processingContext.PageSectionContext.EnterRepeatingItem();
							base.m_processingContext.PageSectionContext.EnterMatrixHeadingScope(Visibility.IsVisible(matrixHeading.Visibility, this.m_startHidden), matrixHeading.IsColumn);
							if (flag3)
							{
								base.m_processingContext.Pagination.EnterIgnorePageBreak(null, true);
								base.m_processingContext.Pagination.EnterIgnoreHeight(true);
								this.m_headingInstance.Content = this.m_headingReportItemCol.CreateInstance(content, true, true, matrixHeading.IsColumn);
								base.m_processingContext.Pagination.LeaveIgnoreHeight(true);
								base.m_processingContext.Pagination.LeaveIgnorePageBreak(null, true);
							}
							else
							{
								content.ProcessDrillthroughAction(base.m_processingContext, nonCompNames);
								content.ProcessNavigationAction(base.m_processingContext.NavigationInfo, nonCompNames, matrix.CurrentPage);
							}
							base.m_processingContext.PageSectionContext.ExitMatrixHeadingScope(matrixHeading.IsColumn);
							if (matrixHeading.IsColumn)
							{
								matrixInstance.MatrixDef.ColumnHeaderPageTextboxes.IntegrateNonRepeatingTextboxValues(base.m_processingContext.PageSectionContext.ExitRepeatingItem());
							}
							else
							{
								pageTextboxes = base.m_processingContext.PageSectionContext.ExitRepeatingItem();
							}
						}
					}
					if (!flag2 && base.m_processingContext.ReportItemsReferenced)
					{
						base.m_processingContext.DelayAddingInstanceInfo = false;
					}
					if (matrixHeading.IsColumn)
					{
						base.m_processingContext.NavigationInfo.LeaveMatrixColumn();
					}
				}
				else
				{
					if (base.m_processingContext.ReportItemsReferenced)
					{
						this.SetReportItemObjs(this.m_headingInstance.Content);
					}
					this.SetContentsPage();
				}
				base.m_processingContext.PageSectionContext.EnterMatrixHeadingScope(Visibility.IsVisible(matrixHeading.Visibility, this.m_startHidden), matrixHeading.IsColumn);
				((RuntimeMatrixHeadingsObj)base.m_pivotHeadings).CreateInstances(this, base.m_processingContext, matrixInstance, flag2, currOuterHeadingGroupRoot, matrixHeadingInstanceList, pagesList);
				base.m_processingContext.PageSectionContext.ExitMatrixHeadingScope(matrixHeading.IsColumn);
				if (flag && Report.ShowHideTypes.Interactive == base.m_processingContext.ShowHideType)
				{
					((IShowHideContainer)this.m_headingInstance).EndProcessContainer(base.m_processingContext);
					base.m_processingContext.ExitGrouping();
				}
				if ((flag2 || matrixInstance.CurrentCellOuterIndex == 0) && !matrixHeading.IsColumn)
				{
					bool pageBreakAtEnd = matrixHeading.Grouping.PageBreakAtEnd;
					renderingPagesRanges.EndPage = ((Matrix)matrixInstance.ReportItemDef).CurrentPage;
					if (this.m_headingInstance.SubHeadingInstances == null || this.m_headingInstance.SubHeadingInstances.Count < 1)
					{
						renderingPagesRanges.EndPage = renderingPagesRanges.StartPage;
					}
					else
					{
						renderingPagesRanges.EndPage = this.m_headingInstance.ChildrenStartAndEndPages[this.m_headingInstance.ChildrenStartAndEndPages.Count - 1].EndPage;
					}
					if (!base.m_processingContext.Pagination.IgnorePageBreak && matrixInstance.NumberOfChildrenOnThisPage > 0 && base.m_processingContext.Pagination.CanMoveToNextPage(pageBreakAtEnd))
					{
						base.m_processingContext.Pagination.SetCurrentPageHeight(matrix, 0.0);
						matrixInstance.ExtraPagesFilled++;
						matrix.CurrentPage++;
						matrixInstance.NumberOfChildrenOnThisPage = 0;
					}
					runtimeMatrixGroupRootObj.PagesList.Add(renderingPagesRanges);
					this.m_startPage = renderingPagesRanges.StartPage;
					if (pageTextboxes != null)
					{
						matrixInstance.MatrixDef.RowHeaderPageTextboxes.IntegrateRepeatingTextboxValues(pageTextboxes, renderingPagesRanges.StartPage, renderingPagesRanges.EndPage);
					}
					base.m_processingContext.Pagination.LeaveIgnoreHeight(matrixHeading.StartHidden);
					base.m_processingContext.Pagination.LeaveIgnorePageBreak(matrixHeading.Visibility, false);
				}
				if (flag2 && !matrixHeading.IsColumn)
				{
					base.m_processingContext.ChunkManager.CheckPageBreak(matrixHeading, false);
				}
				if (this.m_headingReportItemCol != null)
				{
					this.m_headingReportItemCol.ResetReportItemObjs();
				}
				((RuntimeMatrixHeadingsObj)base.m_pivotHeadings).ResetReportItemObjs(base.m_processingContext);
				base.ResetReportItemsWithHideDuplicates();
			}

			protected override void AddToDocumentMap()
			{
				if (base.GroupingDef.GroupLabel != null && this.m_label != null)
				{
					RuntimeGroupRootObj runtimeGroupRootObj = (RuntimeGroupRootObj)base.m_hierarchyRoot;
					bool isColumn = ((MatrixHeading)runtimeGroupRootObj.HierarchyDef).IsColumn;
					NavigationInfo navigationInfo = base.m_processingContext.NavigationInfo;
					if (isColumn)
					{
						navigationInfo.EnterMatrixColumn();
					}
					navigationInfo.AddToDocumentMap(this.m_headingInstance.UniqueName, true, this.m_startPage, this.m_label);
					if (isColumn)
					{
						navigationInfo.LeaveMatrixColumn();
					}
					this.m_label = null;
				}
			}

			internal void CreateInnerGroupingsOrCells(MatrixInstance matrixInstance, RuntimePivotGroupRootObj currOuterHeadingGroupRoot)
			{
				this.SetupEnvironment();
				if (base.IsOuterGrouping())
				{
					RuntimeMatrixHeadingsObj runtimeMatrixHeadingsObj = (RuntimeMatrixHeadingsObj)((RuntimeMatrixGroupRootObj)base.m_hierarchyRoot).InnerGroupings;
					if (Report.ShowHideTypes.Interactive == base.m_processingContext.ShowHideType)
					{
						base.m_processingContext.EndIgnoreRange();
					}
					runtimeMatrixHeadingsObj.CreateInstances(this, base.m_processingContext, matrixInstance, false, currOuterHeadingGroupRoot, matrixInstance.InnerHeadingInstanceList, matrixInstance.ChildrenStartAndEndPages);
					if (Report.ShowHideTypes.Interactive == base.m_processingContext.ShowHideType)
					{
						base.m_processingContext.UseAllContainers = true;
					}
				}
				else if (currOuterHeadingGroupRoot == null)
				{
					this.CreateOutermostSubtotalCell(matrixInstance);
				}
				else
				{
					this.CreateCellInstance(matrixInstance, currOuterHeadingGroupRoot);
				}
			}

			private void CreateCellInstance(MatrixInstance matrixInstance, RuntimePivotGroupRootObj currOuterHeadingGroupRoot)
			{
				if (Report.ShowHideTypes.Interactive == base.m_processingContext.ShowHideType)
				{
					base.m_processingContext.IgnoreAllFromStart = true;
					base.m_processingContext.EnterGrouping();
				}
				int currentCellRIIndex = matrixInstance.GetCurrentCellRIIndex();
				bool flag = default(bool);
				ReportItem cellReportItemDef = matrixInstance.GetCellReportItemDef(currentCellRIIndex, out flag);
				NonComputedUniqueNames nonCompNames = default(NonComputedUniqueNames);
				MatrixCellInstance cellInstance = matrixInstance.AddCell(base.m_processingContext, out nonCompNames);
				if (cellReportItemDef != null)
				{
					base.m_processingContext.PageSectionContext.InMatrixCell = true;
					base.m_processingContext.PageSectionContext.InMatrixSubtotal = (null != base.m_processingContext.HeadingInstance);
					base.m_processingContext.PageSectionContext.EnterRepeatingItem();
					if (flag)
					{
						Global.Tracer.Assert(base.m_cellsList != null && null != base.m_cellsList[currOuterHeadingGroupRoot.HeadingLevel]);
						RuntimeMatrixCell runtimeMatrixCell = (RuntimeMatrixCell)base.m_cellsList[currOuterHeadingGroupRoot.HeadingLevel].GetCell(base.PivotDef, this, currOuterHeadingGroupRoot.HeadingLevel);
						Global.Tracer.Assert(null != runtimeMatrixCell, "(null != cell)");
						runtimeMatrixCell.CreateInstance(matrixInstance, cellReportItemDef, cellInstance);
					}
					else
					{
						cellReportItemDef.ProcessDrillthroughAction(base.m_processingContext, nonCompNames);
						cellReportItemDef.ProcessNavigationAction(base.m_processingContext.NavigationInfo, nonCompNames, matrixInstance.MatrixDef.CurrentPage);
						RuntimeRICollection.AddNonComputedPageTextboxes(cellReportItemDef, matrixInstance.MatrixDef.CurrentPage, base.m_processingContext);
					}
					matrixInstance.MatrixDef.CellPageTextboxes.IntegrateRepeatingTextboxValues(base.m_processingContext.PageSectionContext.ExitRepeatingItem(), matrixInstance.MatrixDef.CellPage, matrixInstance.MatrixDef.CellPage);
					base.m_processingContext.PageSectionContext.InMatrixSubtotal = false;
					base.m_processingContext.PageSectionContext.InMatrixCell = false;
				}
				if (Report.ShowHideTypes.Interactive == base.m_processingContext.ShowHideType)
				{
					base.m_processingContext.IgnoreAllFromStart = false;
					base.m_processingContext.ExitGrouping();
				}
			}

			private void SetupAggregateValues()
			{
				base.SetupEnvironment(base.m_nonCustomAggregates, base.m_customAggregates, base.m_firstRow);
			}

			private void SetupEnvironmentForOuterGroupings()
			{
				if (base.IsOuterGrouping())
				{
					IScope outerScope = this.OuterScope;
					while (outerScope != null && !(outerScope is RuntimeMatrixObj))
					{
						if (outerScope is RuntimeMatrixGroupLeafObj)
						{
							((RuntimeMatrixGroupLeafObj)outerScope).SetupAggregateValues();
						}
						outerScope = outerScope.GetOuterScope(false);
					}
				}
			}

			private void CreateOutermostSubtotalCell(MatrixInstance matrixInstance)
			{
				if (this.m_firstPassCell != null)
				{
					bool inMatrixSubtotal = base.m_processingContext.PageSectionContext.InMatrixSubtotal;
					base.m_processingContext.PageSectionContext.InMatrixSubtotal = true;
					if (Report.ShowHideTypes.Interactive == base.m_processingContext.ShowHideType)
					{
						base.m_processingContext.IgnoreAllFromStart = true;
						base.m_processingContext.EnterGrouping();
					}
					bool flag = default(bool);
					ReportItem cellReportItemDef = matrixInstance.GetCellReportItemDef(-1, out flag);
					NonComputedUniqueNames nonCompNames = default(NonComputedUniqueNames);
					MatrixCellInstance matrixCellInstance = matrixInstance.AddCell(base.m_processingContext, out nonCompNames);
					if (cellReportItemDef != null)
					{
						if (flag)
						{
							this.SetupEnvironmentForOuterGroupings();
							this.SetupEnvironment();
							base.m_processingContext.Pagination.EnterIgnorePageBreak(null, true);
							base.m_processingContext.Pagination.EnterIgnoreHeight(true);
							MatrixHeading matrixHeading = (MatrixHeading)base.PivotHeadingDef;
							matrixHeading.InOutermostSubtotalCell = true;
							matrixCellInstance.Content = this.m_firstPassCell.CreateInstance(cellReportItemDef, true, true, false);
							matrixHeading.InOutermostSubtotalCell = false;
							base.m_processingContext.Pagination.LeaveIgnoreHeight(true);
							base.m_processingContext.Pagination.LeaveIgnorePageBreak(null, true);
							this.m_firstPassCell.ResetReportItemObjs();
						}
						else
						{
							cellReportItemDef.ProcessDrillthroughAction(base.m_processingContext, nonCompNames);
							cellReportItemDef.ProcessNavigationAction(base.m_processingContext.NavigationInfo, nonCompNames, ((Matrix)matrixInstance.ReportItemDef).CurrentPage);
						}
					}
					if (Report.ShowHideTypes.Interactive == base.m_processingContext.ShowHideType)
					{
						base.m_processingContext.IgnoreAllFromStart = false;
						base.m_processingContext.ExitGrouping();
					}
					base.m_processingContext.PageSectionContext.InMatrixSubtotal = inMatrixSubtotal;
				}
			}

			internal void CreateSubtotalOrStaticCells(MatrixInstance matrixInstance, RuntimePivotGroupRootObj currOuterHeadingGroupRoot, bool outerGroupingSubtotal)
			{
				RuntimeMatrixHeadingsObj runtimeMatrixHeadingsObj = (RuntimeMatrixHeadingsObj)((RuntimeMatrixGroupRootObj)base.m_hierarchyRoot).InnerGroupings;
				if (base.IsOuterGrouping() && !outerGroupingSubtotal)
				{
					this.CreateOutermostSubtotalCell(matrixInstance);
				}
				else
				{
					this.CreateInnerGroupingsOrCells(matrixInstance, currOuterHeadingGroupRoot);
				}
			}

			internal void SetReportItemObjs(ReportItemColInstance reportItemColInstance)
			{
				if (reportItemColInstance.ReportItemInstances != null)
				{
					for (int i = 0; i < reportItemColInstance.ReportItemInstances.Count; i++)
					{
						this.SetReportItemObjs(reportItemColInstance[i]);
					}
				}
			}

			private void SetReportItemObjs(ReportItemInstance reportItemInstance)
			{
				if (reportItemInstance is TextBoxInstance)
				{
					TextBox textBox = (TextBox)reportItemInstance.ReportItemDef;
					TextBoxInstance textBoxInstance = (TextBoxInstance)reportItemInstance;
					object obj = null;
					obj = ((!textBox.IsSimpleTextBox()) ? ((TextBoxInstanceInfo)textBoxInstance.InstanceInfo).OriginalValue : ((SimpleTextBoxInstanceInfo)textBoxInstance.InstanceInfo).OriginalValue);
					TextBoxImpl textBoxImpl = (TextBoxImpl)((ReportItems)base.m_processingContext.ReportObjectModel.ReportItemsImpl)[textBox.Name];
					textBoxImpl.SetResult(new VariantResult(false, obj));
				}
				else if (reportItemInstance is RectangleInstance)
				{
					this.SetReportItemObjs(((RectangleInstance)reportItemInstance).ReportItemColInstance);
				}
				else if (reportItemInstance is MatrixInstance)
				{
					this.SetReportItemObjs(((MatrixInstance)reportItemInstance).CornerContent);
				}
				else if (reportItemInstance is TableInstance)
				{
					TableInstance tableInstance = (TableInstance)reportItemInstance;
					if (tableInstance.HeaderRowInstances != null)
					{
						for (int i = 0; i < tableInstance.HeaderRowInstances.Length; i++)
						{
							this.SetReportItemObjs(tableInstance.HeaderRowInstances[i].TableRowReportItemColInstance);
						}
					}
					if (tableInstance.FooterRowInstances != null)
					{
						for (int j = 0; j < tableInstance.FooterRowInstances.Length; j++)
						{
							this.SetReportItemObjs(tableInstance.FooterRowInstances[j].TableRowReportItemColInstance);
						}
					}
				}
			}

			private void GetScopeValuesForOutermostSTCell(IHierarchyObj targetScopeObj, VariantList[] scopeValues, ref int index)
			{
				Pivot pivotDef = base.PivotDef;
				if (base.IsOuterGrouping())
				{
					RuntimePivotObj ownerPivot = base.GetOwnerPivot();
					((RuntimeDataRegionObj)ownerPivot).GetScopeValues(targetScopeObj, scopeValues, ref index);
					int dynamicHeadingCount = pivotDef.GetDynamicHeadingCount(false);
					for (int i = 0; i < dynamicHeadingCount; i++)
					{
						Global.Tracer.Assert(index < scopeValues.Length, "Inner headings");
						scopeValues[index++] = null;
					}
					base.GetScopeValues((IHierarchyObj)ownerPivot, scopeValues, ref index);
					if (base.m_innerHierarchy != null)
					{
						dynamicHeadingCount = pivotDef.GetDynamicHeadingCount(true);
						for (int j = base.HeadingLevel + 1; j < dynamicHeadingCount; j++)
						{
							Global.Tracer.Assert(index < scopeValues.Length, "Outer headings");
							scopeValues[index++] = null;
						}
					}
				}
				else
				{
					base.GetScopeValues(targetScopeObj, scopeValues, ref index);
					int dynamicHeadingCount2;
					if (base.m_innerHierarchy != null)
					{
						dynamicHeadingCount2 = pivotDef.GetDynamicHeadingCount(false);
						for (int k = base.HeadingLevel + 1; k < dynamicHeadingCount2; k++)
						{
							Global.Tracer.Assert(index < scopeValues.Length, "Subtotal inner headings");
							scopeValues[index++] = null;
						}
					}
					dynamicHeadingCount2 = pivotDef.GetDynamicHeadingCount(true);
					for (int l = 0; l < dynamicHeadingCount2; l++)
					{
						Global.Tracer.Assert(index < scopeValues.Length, "Subtotal outer headings");
						scopeValues[index++] = null;
					}
				}
			}

			internal override void GetScopeValues(IHierarchyObj targetScopeObj, VariantList[] scopeValues, ref int index)
			{
				if (((MatrixHeading)base.PivotHeadingDef).InOutermostSubtotalCell)
				{
					this.GetScopeValuesForOutermostSTCell(targetScopeObj, scopeValues, ref index);
				}
				else
				{
					base.GetScopeValues(targetScopeObj, scopeValues, ref index);
				}
			}
		}

		internal sealed class RuntimeExpressionInfo
		{
			private ExpressionInfo m_expression;

			private bool m_direction = true;

			private IndexedExprHost m_expressionsHost;

			private int m_expressionIndex;

			internal ExpressionInfo Expression
			{
				get
				{
					return this.m_expression;
				}
			}

			internal bool Direction
			{
				get
				{
					return this.m_direction;
				}
			}

			internal IndexedExprHost ExpressionsHost
			{
				get
				{
					return this.m_expressionsHost;
				}
			}

			internal int ExpressionIndex
			{
				get
				{
					return this.m_expressionIndex;
				}
			}

			internal RuntimeExpressionInfo(ExpressionInfoList expressions, IndexedExprHost expressionsHost, BoolList directions, int expressionIndex)
			{
				this.m_expressionsHost = expressionsHost;
				this.m_expressionIndex = expressionIndex;
				this.m_expression = expressions[this.m_expressionIndex];
				if (directions != null)
				{
					this.m_direction = directions[this.m_expressionIndex];
				}
			}
		}

		internal sealed class RuntimeExpressionInfoList : ArrayList
		{
			internal new RuntimeExpressionInfo this[int index]
			{
				get
				{
					return (RuntimeExpressionInfo)base[index];
				}
			}

			internal RuntimeExpressionInfoList()
			{
			}
		}

		internal sealed class RuntimeDataRegionObjList : ArrayList
		{
			internal new RuntimeDataRegionObj this[int index]
			{
				get
				{
					return (RuntimeDataRegionObj)base[index];
				}
				set
				{
					base[index] = value;
				}
			}
		}

		internal sealed class RuntimeGroupRootObjList : Hashtable
		{
			internal RuntimeGroupRootObj this[string index]
			{
				get
				{
					return (RuntimeGroupRootObj)base[index];
				}
				set
				{
					base[index] = value;
				}
			}
		}

		private sealed class RuntimeGroupLeafObjList : ArrayList
		{
			internal new RuntimeGroupLeafObj this[int index]
			{
				get
				{
					return (RuntimeGroupLeafObj)base[index];
				}
			}
		}

		private sealed class ParentInformation : Hashtable
		{
			internal new RuntimeGroupLeafObjList this[object parentKey]
			{
				get
				{
					return (RuntimeGroupLeafObjList)base[parentKey];
				}
			}

			internal ParentInformation()
			{
			}
		}

		internal sealed class RuntimePivotCells : Hashtable
		{
			private RuntimePivotCell m_firstCell;

			private RuntimePivotCell m_lastCell;

			internal RuntimePivotCell this[int index]
			{
				get
				{
					return (RuntimePivotCell)base[index];
				}
				set
				{
					if (base.Count == 0)
					{
						this.m_firstCell = value;
					}
					base[index] = value;
				}
			}

			internal void Add(int key, RuntimePivotCell cell)
			{
				if (this.m_lastCell != null)
				{
					this.m_lastCell.NextCell = cell;
				}
				else
				{
					this.m_firstCell = cell;
				}
				this.m_lastCell = cell;
				base.Add(key, cell);
			}

			internal RuntimePivotCell GetCell(Pivot pivotDef, RuntimePivotGroupLeafObj owner, int cellLevel)
			{
				RuntimePivotGroupRootObj currentOuterHeadingGroupRoot = pivotDef.CurrentOuterHeadingGroupRoot;
				int index = pivotDef.OuterGroupingIndexes[currentOuterHeadingGroupRoot.HeadingLevel];
				RuntimePivotCell runtimePivotCell = this[index];
				if (runtimePivotCell == null)
				{
					runtimePivotCell = (this[index] = owner.CreateCell(cellLevel, pivotDef));
				}
				return runtimePivotCell;
			}

			internal void SortAndFilter()
			{
				for (RuntimePivotCell runtimePivotCell = this.m_firstCell; runtimePivotCell != null; runtimePivotCell = runtimePivotCell.NextCell)
				{
					runtimePivotCell.SortAndFilter();
				}
			}

			internal void CalculateRunningValues(Pivot pivotDef, AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup, RuntimePivotGroupLeafObj owner, int cellLevel)
			{
				RuntimePivotCell cell = this.GetCell(pivotDef, owner, cellLevel);
				Global.Tracer.Assert(null != cell, "(null != cell)");
				cell.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
			}
		}

		private sealed class Merge
		{
			private Report m_report;

			private ReportInstance m_reportInstance;

			private string m_reportLanguage;

			private ProcessingContext m_processingContext;

			private RuntimeDataSourceNodeList m_runtimeDataSourceNodes = new RuntimeDataSourceNodeList();

			private bool m_fetchImageStreams;

			private bool m_initialized;

			internal Merge(Report report, ProcessingContext context)
			{
				this.m_report = report;
				this.m_processingContext = context;
				this.m_fetchImageStreams = true;
			}

			internal Merge(Report report, ProcessingContext context, bool firstSubreportInstance)
			{
				this.m_report = report;
				this.m_processingContext = context;
				this.m_fetchImageStreams = firstSubreportInstance;
			}

			internal bool PrefetchData(ParameterInfoCollection parameters, bool mergeTran)
			{
				EventHandler eventHandler = null;
				try
				{
					bool flag = false;
					this.Init(parameters, true);
					this.EvaluateAndSetReportLanguage();
					if (this.m_report.DataSourceCount != 0)
					{
						int count = this.m_report.DataSources.Count;
						flag = true;
						for (int i = 0; i < count; i++)
						{
							this.m_runtimeDataSourceNodes.Add(new ReportRuntimeDataSourceNode(this.m_report, this.m_report.DataSources[i], this.m_processingContext));
						}
						eventHandler = this.AbortHandler;
						this.m_processingContext.AbortInfo.ProcessingAbortEvent += eventHandler;
						if (Global.Tracer.TraceVerbose)
						{
							Global.Tracer.Trace(TraceLevel.Verbose, "Abort handler registered.");
						}
						ThreadSet threadSet = null;
						RuntimeDataSourceNode runtimeDataSourceNode;
						if (count > 1)
						{
							threadSet = new ThreadSet(count - 1);
							for (int j = 1; j < count; j++)
							{
								runtimeDataSourceNode = this.m_runtimeDataSourceNodes[j];
								runtimeDataSourceNode.InitProcessingParams(mergeTran, true);
								this.m_processingContext.JobContext.TryQueueWorkItem(runtimeDataSourceNode.ProcessConcurrent, threadSet);
							}
						}
						runtimeDataSourceNode = this.m_runtimeDataSourceNodes[0];
						runtimeDataSourceNode.InitProcessingParams(mergeTran, true);
						runtimeDataSourceNode.ProcessConcurrent(null);
						this.m_processingContext.CheckAndThrowIfAborted();
						if (count > 1)
						{
							threadSet.WaitForCompletion();
						}
						this.m_processingContext.CheckAndThrowIfAborted();
						for (int k = 0; k < count; k++)
						{
							runtimeDataSourceNode = this.m_runtimeDataSourceNodes[k];
							if (flag)
							{
								flag = runtimeDataSourceNode.NoRows;
							}
						}
					}
					if (this.m_report.ParametersNotUsedInQuery && this.m_processingContext.ErrorSavingSnapshotData)
					{
						for (int l = 0; l < parameters.Count; l++)
						{
							parameters[l].UsedInQuery = true;
						}
						return false;
					}
				}
				finally
				{
					if (eventHandler != null)
					{
						this.m_processingContext.AbortInfo.ProcessingAbortEvent -= eventHandler;
					}
					if (this.m_report.DataSources != null && 0 < this.m_report.DataSources.Count)
					{
						for (int num = this.m_runtimeDataSourceNodes.Count - 1; num >= 0; num--)
						{
							RuntimeDataSourceNode runtimeDataSourceNode2 = this.m_runtimeDataSourceNodes[num];
							if (runtimeDataSourceNode2.DataProcessingDurationMs > this.m_processingContext.DataProcessingDurationMs)
							{
								this.m_processingContext.DataProcessingDurationMs = runtimeDataSourceNode2.DataProcessingDurationMs;
							}
						}
					}
					this.m_runtimeDataSourceNodes.Clear();
				}
				return true;
			}

			internal ReportInstance Process(ParameterInfoCollection parameters, bool mergeTran)
			{
				EventHandler eventHandler = null;
				try
				{
					bool flag = false;
					if (Global.Tracer.TraceVerbose)
					{
						Global.Tracer.Trace(TraceLevel.Verbose, "One Pass Processing? {0}", this.m_report.MergeOnePass ? "Yes" : "No");
					}
					ImageStreamNames imageStreamNames = this.m_report.ImageStreamNames;
					if (this.m_fetchImageStreams && imageStreamNames != null && 0 < imageStreamNames.Count)
					{
						ImageStreamNames imageStreamNames2 = new ImageStreamNames();
						IDictionaryEnumerator enumerator = imageStreamNames.GetEnumerator();
						while (enumerator.MoveNext())
						{
							string text = (string)enumerator.Key;
							Global.Tracer.Assert(text != null, "The URL to this image should not be null.");
							string mimeType = null;
							byte[] array = null;
							ImageInfo imageInfo = (ImageInfo)enumerator.Value;
							RuntimeRICollection.GetExternalImage(this.m_processingContext, text, ObjectType.Image, imageInfo.StreamName, out array, out mimeType);
							if (array != null && this.m_processingContext.CreateReportChunkCallback != null)
							{
								string text2 = Guid.NewGuid().ToString();
								using (Stream stream = this.m_processingContext.CreateReportChunkCallback(text2, ReportChunkTypes.Image, mimeType))
								{
									stream.Write(array, 0, array.Length);
								}
								imageStreamNames2[text] = new ImageInfo(text2, mimeType);
							}
						}
						this.m_report.ImageStreamNames = imageStreamNames2;
						this.m_processingContext.ImageStreamNames = imageStreamNames2;
					}
					this.Init(parameters, false);
					this.EvaluateAndSetReportLanguage();
					if (this.m_processingContext.ReportObjectModel.DataSourcesImpl == null)
					{
						this.m_processingContext.ReportObjectModel.DataSourcesImpl = new DataSourcesImpl(this.m_report.DataSourceCount);
					}
					if (this.m_report.DataSourceCount != 0)
					{
						int count = this.m_report.DataSources.Count;
						flag = true;
						for (int i = 0; i < count; i++)
						{
							this.m_runtimeDataSourceNodes.Add(new ReportRuntimeDataSourceNode(this.m_report, this.m_report.DataSources[i], this.m_processingContext));
						}
						eventHandler = this.AbortHandler;
						this.m_processingContext.AbortInfo.ProcessingAbortEvent += eventHandler;
						if (Global.Tracer.TraceVerbose)
						{
							Global.Tracer.Trace(TraceLevel.Verbose, "Abort handler registered.");
						}
						ThreadSet threadSet = null;
						RuntimeDataSourceNode runtimeDataSourceNode;
						if (count > 1)
						{
							threadSet = new ThreadSet(count - 1);
							for (int j = 1; j < count; j++)
							{
								runtimeDataSourceNode = this.m_runtimeDataSourceNodes[j];
								runtimeDataSourceNode.InitProcessingParams(mergeTran, false);
								this.m_processingContext.JobContext.TryQueueWorkItem(runtimeDataSourceNode.ProcessConcurrent, threadSet);
							}
						}
						runtimeDataSourceNode = this.m_runtimeDataSourceNodes[0];
						runtimeDataSourceNode.InitProcessingParams(mergeTran, false);
						runtimeDataSourceNode.ProcessConcurrent(null);
						this.m_processingContext.CheckAndThrowIfAborted();
						if (count > 1)
						{
							threadSet.WaitForCompletion();
						}
						if (Global.Tracer.TraceVerbose)
						{
							Global.Tracer.Trace(TraceLevel.Verbose, "The processing of all data sources has been completed.");
						}
						this.m_processingContext.CheckAndThrowIfAborted();
						for (int k = 0; k < count; k++)
						{
							runtimeDataSourceNode = this.m_runtimeDataSourceNodes[k];
							if (flag)
							{
								flag = runtimeDataSourceNode.NoRows;
							}
							if (this.m_processingContext.SaveSnapshotData && this.m_processingContext.ErrorSavingSnapshotData && this.m_processingContext.StopSaveSnapshotDataOnError)
							{
								runtimeDataSourceNode.EraseDataChunk();
							}
						}
					}
					if (this.m_report.ParametersNotUsedInQuery && this.m_processingContext.ErrorSavingSnapshotData)
					{
						for (int l = 0; l < parameters.Count; l++)
						{
							parameters[l].UsedInQuery = true;
						}
					}
					this.CreateInstances(parameters, flag);
					this.m_report.IntermediateFormatVersion.SetCurrent();
					this.m_report.LastID = this.m_processingContext.GetLastIDForReport();
					return this.m_reportInstance;
				}
				finally
				{
					if (eventHandler != null)
					{
						this.m_processingContext.AbortInfo.ProcessingAbortEvent -= eventHandler;
					}
					if (this.m_report.DataSources != null && 0 < this.m_report.DataSources.Count)
					{
						for (int num = this.m_runtimeDataSourceNodes.Count - 1; num >= 0; num--)
						{
							RuntimeDataSourceNode runtimeDataSourceNode2 = this.m_runtimeDataSourceNodes[num];
							if (runtimeDataSourceNode2.DataProcessingDurationMs > this.m_processingContext.DataProcessingDurationMs)
							{
								this.m_processingContext.DataProcessingDurationMs = runtimeDataSourceNode2.DataProcessingDurationMs;
							}
						}
					}
					if (this.m_processingContext.ReportRuntime != null)
					{
						this.m_processingContext.ReportRuntime.Close();
					}
					for (int m = 0; m < this.m_runtimeDataSourceNodes.Count; m++)
					{
						this.m_runtimeDataSourceNodes[m].Cleanup();
					}
				}
			}

			internal void CleanupDataChunk(UserProfileState userProfileState)
			{
				if (!this.m_processingContext.IsHistorySnapshot && this.m_processingContext.SaveSnapshotData && (!this.m_processingContext.ErrorSavingSnapshotData || (this.m_processingContext.ErrorSavingSnapshotData && !this.m_processingContext.StopSaveSnapshotDataOnError)) && !this.m_report.ParametersNotUsedInQuery && !this.m_processingContext.HasUserSortFilter && (userProfileState & UserProfileState.InReport) == UserProfileState.None)
				{
					for (int i = 0; i < this.m_report.DataSources.Count; i++)
					{
						RuntimeDataSourceNode runtimeDataSourceNode = this.m_runtimeDataSourceNodes[i];
						runtimeDataSourceNode.EraseDataChunk();
					}
				}
			}

			private void AbortHandler(object sender, EventArgs e)
			{
				if (((ProcessingAbortEventArgs)e).ReportUniqueName == this.m_processingContext.DataSetUniqueName)
				{
					if (Global.Tracer.TraceInfo)
					{
						Global.Tracer.Trace(TraceLevel.Info, "Merge abort handler called for ID={0}. Aborting data sources ...", this.m_processingContext.DataSetUniqueName);
					}
					int count = this.m_runtimeDataSourceNodes.Count;
					for (int i = 0; i < count; i++)
					{
						this.m_runtimeDataSourceNodes[i].Abort();
					}
				}
			}

			internal void Init(ParameterInfoCollection parameters, bool prefetchDataOnly)
			{
				if (this.m_processingContext.ReportObjectModel == null && this.m_processingContext.ReportRuntime == null)
				{
					this.m_processingContext.ReportObjectModel = new ObjectModelImpl(this.m_processingContext);
					this.m_processingContext.ReportRuntime = new ReportRuntime(this.m_processingContext.ReportObjectModel, this.m_processingContext.ErrorContext);
					this.m_processingContext.ReportObjectModel.ParametersImpl = new ParametersImpl(parameters.Count);
					if (parameters != null && parameters.Count > 0)
					{
						for (int i = 0; i < parameters.Count; i++)
						{
							ParameterInfo parameterInfo = parameters[i];
							this.m_processingContext.ReportObjectModel.ParametersImpl.Add(parameterInfo.Name, new ParameterImpl(parameterInfo.Values, parameterInfo.Labels, parameterInfo.MultiValue));
						}
					}
					this.m_processingContext.ReportObjectModel.GlobalsImpl = new GlobalsImpl(this.m_processingContext.ReportContext.ItemName, this.m_processingContext.ExecutionTime, this.m_processingContext.ReportContext.HostRootUri, this.m_processingContext.ReportContext.ParentPath);
					this.m_processingContext.ReportObjectModel.UserImpl = new UserImpl(this.m_processingContext.RequestUserName, this.m_processingContext.UserLanguage.Name, this.m_processingContext.AllowUserProfileState);
				}
				if (!prefetchDataOnly)
				{
					this.m_processingContext.ReportObjectModel.FieldsImpl = new FieldsImpl();
					int num = 0;
					for (int j = 0; j < this.m_report.DataSourceCount; j++)
					{
						DataSource dataSource = this.m_report.DataSources[j];
						if (dataSource.DataSets != null)
						{
							for (int k = 0; k < dataSource.DataSets.Count; k++)
							{
								if (!dataSource.DataSets[k].UsedOnlyInParameters)
								{
									num++;
								}
							}
						}
					}
					this.m_processingContext.ReportObjectModel.AggregatesImpl = new AggregatesImpl(num > 1, this.m_processingContext.ReportRuntime);
					this.m_processingContext.ReportObjectModel.ReportItemsImpl = new ReportItemsImpl(num > 1);
					this.m_processingContext.ReportObjectModel.DataSetsImpl = new DataSetsImpl(num > 1, num);
				}
				if (!this.m_initialized)
				{
					this.m_initialized = true;
					if (this.m_processingContext.ReportRuntime.ReportExprHost == null)
					{
						this.m_processingContext.ReportRuntime.LoadCompiledCode(this.m_report, false, this.m_processingContext.ReportObjectModel, this.m_processingContext.ReportRuntimeSetup);
					}
					if (this.m_processingContext.ReportRuntime.ReportExprHost != null)
					{
						this.m_report.SetExprHost(this.m_processingContext.ReportRuntime.ReportExprHost, this.m_processingContext.ReportObjectModel);
					}
					if (this.m_report.HasUserSortFilter)
					{
						this.m_processingContext.HasUserSortFilter = true;
					}
				}
			}

			private void CreateInstances(ParameterInfoCollection parameters, bool noRows)
			{
				int num = 0;
				RuntimeReportDataSetNode runtimeReportDataSetNode = null;
				RuntimeReportDataSetNode runtimeReportDataSetNode2 = null;
				if (this.m_runtimeDataSourceNodes != null)
				{
					for (int i = 0; i < this.m_runtimeDataSourceNodes.Count; i++)
					{
						RuntimeDataSourceNode runtimeDataSourceNode = this.m_runtimeDataSourceNodes[i];
						if (runtimeDataSourceNode != null && runtimeDataSourceNode.RuntimeDataSetNodes != null)
						{
							for (int j = 0; j < runtimeDataSourceNode.RuntimeDataSetNodes.Count; j++)
							{
								num++;
								runtimeReportDataSetNode = (RuntimeReportDataSetNode)runtimeDataSourceNode.RuntimeDataSetNodes[j];
								if (runtimeReportDataSetNode.HasSortFilterInfo)
								{
									RuntimeSortFilterEventInfoList runtimeSortFilterInfo = runtimeReportDataSetNode.ProcessingContext.RuntimeSortFilterInfo;
									if (runtimeSortFilterInfo != null)
									{
										if (this.m_processingContext.ReportRuntimeUserSortFilterInfo == null)
										{
											this.m_processingContext.ReportRuntimeUserSortFilterInfo = new RuntimeSortFilterEventInfoList();
										}
										for (int k = 0; k < runtimeSortFilterInfo.Count; k++)
										{
											this.m_processingContext.ReportRuntimeUserSortFilterInfo.Add(runtimeSortFilterInfo[k]);
										}
									}
								}
							}
						}
					}
				}
				if (1 == num && runtimeReportDataSetNode != null)
				{
					this.m_processingContext.ReportObjectModel.FieldsImpl.Clone(runtimeReportDataSetNode.Fields);
					runtimeReportDataSetNode2 = runtimeReportDataSetNode;
				}
				this.m_reportInstance = new ReportInstance(this.m_processingContext, this.m_report, parameters, this.m_reportLanguage, noRows);
				if (this.m_report.HasReportItemReferences || this.m_processingContext.ReportRuntime.ReportExprHost != null)
				{
					this.m_processingContext.RuntimeInitializeReportItemObjs(this.m_report.ReportItems, false, false);
				}
				this.m_processingContext.ReportRuntime.CurrentScope = runtimeReportDataSetNode2;
				RuntimeRICollection runtimeRICollection = new RuntimeRICollection(runtimeReportDataSetNode2, this.m_report.ReportItems, this.m_processingContext, false);
				if (runtimeReportDataSetNode2 != null)
				{
					this.m_processingContext.UserSortFilterContext.UpdateContextFromDataSet(runtimeReportDataSetNode2.ProcessingContext.UserSortFilterContext);
				}
				this.m_processingContext.Pagination.SetReportItemStartPage(this.m_report, false);
				runtimeRICollection.CreateInstances(this.m_reportInstance.ReportItemColInstance);
				this.m_processingContext.Pagination.ProcessEndPage(this.m_reportInstance, this.m_report, false, false);
				this.m_reportInstance.NumberOfPages = this.m_report.EndPage + 1;
			}

			private void EvaluateAndSetReportLanguage()
			{
				CultureInfo cultureInfo = null;
				if (this.m_report.Language != null)
				{
					if (this.m_report.Language.Type != ExpressionInfo.Types.Constant)
					{
						this.m_processingContext.LanguageInstanceId = this.m_processingContext.LanguageInstanceId + 1;
						this.m_reportLanguage = this.m_processingContext.ReportRuntime.EvaluateReportLanguageExpression(this.m_report, out cultureInfo);
					}
					else
					{
						Exception ex = null;
						try
						{
							cultureInfo = new CultureInfo(this.m_report.Language.Value, false);
						}
						catch (Exception ex2)
						{
							cultureInfo = null;
							ex = ex2;
						}
						if (cultureInfo != null && cultureInfo.IsNeutralCulture)
						{
							try
							{
								cultureInfo = CultureInfo.CreateSpecificCulture(this.m_report.Language.Value);
								cultureInfo = new CultureInfo(cultureInfo.Name, false);
							}
							catch (Exception ex3)
							{
								cultureInfo = null;
								ex = ex3;
							}
						}
						if (ex != null)
						{
							this.m_processingContext.ErrorContext.Register(ProcessingErrorCode.rsInvalidLanguage, Severity.Warning, ObjectType.Report, this.m_report.Name, "Language", ex.Message);
						}
					}
				}
				if (cultureInfo == null && this.m_processingContext.SubReportLevel == 0)
				{
					cultureInfo = Localization.DefaultReportServerSpecificCulture;
				}
				if (cultureInfo != null)
				{
					Thread.CurrentThread.CurrentCulture = cultureInfo;
					this.m_processingContext.ThreadCulture = cultureInfo;
				}
			}
		}

		private sealed class RuntimeDataSourceNodeList : ArrayList
		{
			internal new RuntimeDataSourceNode this[int index]
			{
				get
				{
					return (RuntimeDataSourceNode)base[index];
				}
			}
		}

		internal abstract class RuntimeDataSourceNode
		{
			protected bool m_noRows = true;

			protected ProcessingContext m_processingContext;

			protected Report m_report;

			protected DataSource m_dataSource;

			protected RuntimeDataSetNodeList m_runtimeDataSetNodes = new RuntimeDataSetNodeList();

			protected bool m_canAbort;

			protected long m_dataProcessingDurationMs;

			protected int m_dataSetIndex = -1;

			internal long DataProcessingDurationMs
			{
				get
				{
					return this.m_dataProcessingDurationMs;
				}
			}

			internal bool NoRows
			{
				get
				{
					return this.m_noRows;
				}
			}

			internal RuntimeDataSetNodeList RuntimeDataSetNodes
			{
				get
				{
					return this.m_runtimeDataSetNodes;
				}
			}

			internal RuntimeDataSourceNode(Report report, DataSource dataSource, ProcessingContext processingContext)
			{
				this.m_report = report;
				this.m_dataSource = dataSource;
				this.m_processingContext = processingContext;
			}

			internal RuntimeDataSourceNode(Report report, DataSource dataSource, int dataSetIndex, ProcessingContext processingContext)
			{
				this.m_report = report;
				this.m_dataSource = dataSource;
				this.m_processingContext = processingContext;
				this.m_dataSetIndex = dataSetIndex;
			}

			internal void Abort()
			{
				if (Global.Tracer.TraceVerbose)
				{
					Global.Tracer.Trace(TraceLevel.Verbose, "Data source '{0}': Abort handler called. CanAbort = {1}.", this.m_dataSource.Name.MarkAsModelInfo(), this.m_canAbort);
				}
				if (this.m_canAbort)
				{
					int count = this.m_runtimeDataSetNodes.Count;
					for (int i = 0; i < count; i++)
					{
						this.m_runtimeDataSetNodes[i].Abort();
					}
				}
			}

			internal void Cleanup()
			{
				for (int i = 0; i < this.m_runtimeDataSetNodes.Count; i++)
				{
					this.m_runtimeDataSetNodes[i].Cleanup();
				}
			}

			internal virtual void InitProcessingParams(bool mergeTran, bool prefetchDataOnly)
			{
			}

			internal void ProcessConcurrent(object threadSet)
			{
				CultureInfo cultureInfo = null;
				Global.Tracer.Assert(this.m_dataSource.Name != null, "The name of a data source cannot be null.");
				try
				{
					if (Global.Tracer.TraceVerbose)
					{
						Global.Tracer.Trace(TraceLevel.Verbose, "Thread has started processing data source '{0}'", this.m_dataSource.Name.MarkAsModelInfo());
					}
					if (this.m_processingContext.ThreadCulture != null)
					{
						cultureInfo = Thread.CurrentThread.CurrentCulture;
						Thread.CurrentThread.CurrentCulture = this.m_processingContext.ThreadCulture;
					}
					this.Process();
				}
				catch (ProcessingAbortedException)
				{
					if (Global.Tracer.TraceWarning)
					{
						Global.Tracer.Trace(TraceLevel.Warning, "Data source '{0}': Report processing has been aborted.", this.m_dataSource.Name.MarkAsModelInfo());
					}
				}
				catch (Exception ex2)
				{
					if (Global.Tracer.TraceError)
					{
						Global.Tracer.Trace(TraceLevel.Error, "An exception has occurred in data source '{0}'. Details: {1}", this.m_dataSource.Name.MarkAsModelInfo(), ex2.ToString());
					}
					if (this.m_processingContext.AbortInfo != null)
					{
						this.m_processingContext.AbortInfo.SetError(this.m_processingContext.DataSetUniqueName, ex2);
					}
				}
				finally
				{
					if (Global.Tracer.TraceVerbose)
					{
						Global.Tracer.Trace(TraceLevel.Verbose, "Processing of data source '{0}' completed.", this.m_dataSource.Name.MarkAsModelInfo());
					}
					if (cultureInfo != null)
					{
						Thread.CurrentThread.CurrentCulture = cultureInfo;
					}
					ThreadSet threadSet2 = threadSet as ThreadSet;
					if (threadSet2 != null)
					{
						threadSet2.ThreadCompleted();
					}
				}
			}

			protected abstract void Process();

			internal void EraseDataChunk()
			{
				for (int i = 0; i < this.m_runtimeDataSetNodes.Count; i++)
				{
					this.m_runtimeDataSetNodes[i].EraseDataChunk();
				}
			}
		}

		internal sealed class ReportRuntimeDataSourceNode : RuntimeDataSourceNode
		{
			private enum ConnectionSecurity
			{
				UseIntegratedSecurity,
				ImpersonateWindowsUser,
				UseDataSourceCredentials,
				None
			}

			private bool m_mergeTran;

			private bool m_prefetchDataOnly;

			private LegacyReportParameterDataSetCache m_cache;

			internal ReportRuntimeDataSourceNode(Report report, DataSource dataSource, ProcessingContext processingContext)
				: base(report, dataSource, processingContext)
			{
			}

			internal ReportRuntimeDataSourceNode(Report report, DataSource dataSource, int dataSetIndex, ProcessingContext processingContext, LegacyReportParameterDataSetCache aCache)
				: base(report, dataSource, dataSetIndex, processingContext)
			{
				this.m_cache = aCache;
			}

			internal override void InitProcessingParams(bool mergeTran, bool prefetchDataOnly)
			{
				this.m_mergeTran = mergeTran;
				this.m_prefetchDataOnly = prefetchDataOnly;
			}

			protected override void Process()
			{
				if (base.m_dataSource.DataSets != null && 0 < base.m_dataSource.DataSets.Count)
				{
					IDbConnection dbConnection = null;
					TransactionInfo transactionInfo = null;
					bool flag = false;
					int num = 1;
					DataSet dataSet = null;
					bool flag2 = false;
					bool flag3 = false;
					if (base.m_processingContext.ProcessReportParameters)
					{
						base.m_runtimeDataSetNodes.Add(new RuntimeReportParametersDataSetNode(base.m_report, base.m_dataSource.DataSets[base.m_dataSetIndex], base.m_processingContext, this.m_cache));
					}
					else
					{
						num = base.m_dataSource.DataSets.Count;
						for (int i = 0; i < num; i++)
						{
							dataSet = base.m_dataSource.DataSets[i];
							if (!dataSet.UsedOnlyInParameters)
							{
								RuntimeDataSetNode value = (!this.m_prefetchDataOnly) ? ((RuntimeDataSetNode)new RuntimeReportDataSetNode(base.m_report, dataSet, base.m_processingContext)) : ((RuntimeDataSetNode)new RuntimePrefetchDataSetNode(base.m_report, dataSet, base.m_processingContext));
								base.m_runtimeDataSetNodes.Add(value);
							}
						}
					}
					num = base.m_runtimeDataSetNodes.Count;
					if (0 >= num)
					{
						base.m_noRows = false;
					}
					else
					{
						base.m_canAbort = true;
						base.m_processingContext.CheckAndThrowIfAborted();
						try
						{
							bool flag4 = num > 1 && (!base.m_processingContext.UserSortFilterProcessing || base.m_processingContext.SubReportLevel == 0 || !base.m_report.HasUserSortFilter);
							if (!base.m_processingContext.SnapshotProcessing && !base.m_processingContext.ProcessWithCachedData)
							{
								if (base.m_dataSource.Transaction && this.m_mergeTran)
								{
									DataSourceInfo dataSourceInfo = base.m_processingContext.GlobalDataSourceInfo[base.m_dataSource.Name];
									if (dataSourceInfo != null)
									{
										dbConnection = dataSourceInfo.Connection;
										transactionInfo = dataSourceInfo.TransactionInfo;
									}
								}
								if (Global.Tracer.TraceVerbose)
								{
									Global.Tracer.Trace(TraceLevel.Verbose, "Data source '{0}': Transaction = {1}, MergeTran = {2}, NumDataSets = {3}", base.m_dataSource.Name.MarkAsModelInfo(), base.m_dataSource.Transaction, this.m_mergeTran, num);
								}
								if (dbConnection == null)
								{
									ReportProcessingContext reportProcessingContext = (ReportProcessingContext)base.m_processingContext;
									AspNetCore.ReportingServices.DataExtensions.DataSourceInfo dataSourceInfo2 = default(AspNetCore.ReportingServices.DataExtensions.DataSourceInfo);
									string connectionString = base.m_dataSource.ResolveConnectionString(reportProcessingContext, out dataSourceInfo2);
									dbConnection = reportProcessingContext.DataExtensionConnection.OpenDataSourceExtensionConnection(base.m_dataSource, connectionString, dataSourceInfo2, null);
									if (Global.Tracer.TraceVerbose)
									{
										Global.Tracer.Trace(TraceLevel.Verbose, "Data source '{0}': Created a connection.", base.m_dataSource.Name.MarkAsModelInfo());
									}
									flag = true;
								}
								if (base.m_dataSource.Transaction)
								{
									if (transactionInfo == null)
									{
										IDbTransaction transaction = dbConnection.BeginTransaction();
										if (Global.Tracer.TraceVerbose)
										{
											Global.Tracer.Trace(TraceLevel.Verbose, "Data source '{0}': Begun a transaction.", base.m_dataSource.Name.MarkAsModelInfo());
										}
										transactionInfo = new TransactionInfo(transaction);
										flag2 = true;
									}
									IDbTransactionExtension dbTransactionExtension = transactionInfo.Transaction as IDbTransactionExtension;
									flag3 = (dbTransactionExtension != null && dbTransactionExtension.AllowMultiConnection);
									flag4 &= flag3;
									if (Global.Tracer.TraceVerbose)
									{
										Global.Tracer.Trace(TraceLevel.Verbose, "Data source '{0}': TransactionCanSpanConnections = {1}, ConcurrentDataSets = {2}", base.m_dataSource.Name.MarkAsModelInfo(), flag3, flag4);
									}
								}
							}
							if (!this.m_prefetchDataOnly)
							{
								base.m_processingContext.ReportObjectModel.DataSourcesImpl.Add(base.m_dataSource);
							}
							int num2 = default(int);
							if (!base.m_processingContext.SnapshotProcessing && !base.m_processingContext.ProcessWithCachedData && dbConnection is IDbCollationProperties && this.NeedAutoDetectCollation(out num2))
							{
								try
								{
									string cultureName = default(string);
									bool caseSensitive = default(bool);
									bool accentSensitive = default(bool);
									bool kanatypeSensitive = default(bool);
									bool widthSensitive = default(bool);
									if (((IDbCollationProperties)dbConnection).GetCollationProperties(out cultureName, out caseSensitive, out accentSensitive, out kanatypeSensitive, out widthSensitive))
									{
										for (int j = 0; j < base.m_dataSource.DataSets.Count; j++)
										{
											base.m_dataSource.DataSets[j].MergeCollationSettings(base.m_processingContext.ErrorContext, base.m_dataSource.Type, cultureName, caseSensitive, accentSensitive, kanatypeSensitive, widthSensitive);
										}
									}
								}
								catch (Exception ex)
								{
									base.m_processingContext.ErrorContext.Register(ProcessingErrorCode.rsCollationDetectionFailed, Severity.Warning, ObjectType.DataSource, base.m_dataSource.Name.MarkAsModelInfo(), "Collation", ex.ToString());
								}
							}
							if (flag4)
							{
								ThreadSet threadSet = new ThreadSet(num - 1);
								RuntimeDataSetNode runtimeDataSetNode;
								for (int k = 1; k < num; k++)
								{
									runtimeDataSetNode = base.m_runtimeDataSetNodes[k];
									runtimeDataSetNode.InitProcessingParams(base.m_dataSource, null, transactionInfo);
									base.m_processingContext.JobContext.TryQueueWorkItem(runtimeDataSetNode.ProcessConcurrent, threadSet);
								}
								runtimeDataSetNode = base.m_runtimeDataSetNodes[0];
								runtimeDataSetNode.InitProcessingParams(base.m_dataSource, dbConnection, transactionInfo);
								runtimeDataSetNode.ProcessConcurrent(null);
								base.m_processingContext.CheckAndThrowIfAborted();
								threadSet.WaitForCompletion();
								if (base.m_processingContext.JobContext != null)
								{
									for (int l = 0; l < num; l++)
									{
										runtimeDataSetNode = base.m_runtimeDataSetNodes[l];
										if (runtimeDataSetNode.DataProcessingDurationMs > base.m_dataProcessingDurationMs)
										{
											base.m_dataProcessingDurationMs = runtimeDataSetNode.DataProcessingDurationMs;
										}
									}
								}
							}
							else
							{
								for (int m = 0; m < num; m++)
								{
									base.m_processingContext.CheckAndThrowIfAborted();
									RuntimeDataSetNode runtimeDataSetNode = base.m_runtimeDataSetNodes[m];
									runtimeDataSetNode.InitProcessingParams(base.m_dataSource, dbConnection, transactionInfo);
									runtimeDataSetNode.ProcessConcurrent(null);
									base.m_dataProcessingDurationMs += runtimeDataSetNode.DataProcessingDurationMs;
								}
							}
							if (Global.Tracer.TraceVerbose)
							{
								Global.Tracer.Trace(TraceLevel.Verbose, "Data source '{0}': Processing of all data sets completed.", base.m_dataSource.Name.MarkAsModelInfo());
							}
							base.m_processingContext.CheckAndThrowIfAborted();
							long num3 = 0L;
							base.m_noRows = true;
							for (int n = 0; n < num; n++)
							{
								if (!base.m_runtimeDataSetNodes[n].NoRows)
								{
									base.m_noRows = false;
								}
								num3 += base.m_runtimeDataSetNodes[n].NumRowsRead;
							}
							IJobContext jobContext = base.m_processingContext.JobContext;
							if (jobContext != null)
							{
								lock (jobContext.SyncRoot)
								{
									jobContext.RowCount += num3;
								}
							}
							if (flag2)
							{
								if (!base.m_report.SubReportMergeTransactions || base.m_processingContext.ProcessReportParameters)
								{
									if (Global.Tracer.TraceVerbose)
									{
										Global.Tracer.Trace(TraceLevel.Verbose, "Data source '{0}': Committing transaction.", base.m_dataSource.Name.MarkAsModelInfo());
									}
									try
									{
										transactionInfo.Transaction.Commit();
									}
									catch (Exception innerException)
									{
										throw new ReportProcessingException(ErrorCode.rsErrorCommitTransaction, innerException, base.m_dataSource.Name.MarkAsModelInfo());
									}
								}
								else
								{
									IDbConnection connection;
									if (flag3)
									{
										connection = null;
									}
									else
									{
										connection = dbConnection;
										flag = false;
									}
									if (Global.Tracer.TraceVerbose)
									{
										Global.Tracer.Trace(TraceLevel.Verbose, "Data source '{0}': Storing trans+conn into GlobalDataSourceInfo. CloseConnection = {1}.", base.m_dataSource.Name.MarkAsModelInfo(), flag);
									}
									base.m_processingContext.GlobalDataSourceInfo.Add(base.m_dataSource, connection, transactionInfo, null);
								}
								flag2 = false;
								transactionInfo = null;
							}
						}
						catch (Exception ex2)
						{
							if (!(ex2 is ProcessingAbortedException) && Global.Tracer.TraceError)
							{
								Global.Tracer.Trace(TraceLevel.Error, "Data source '{0}': An error has occurred. Details: {1}", base.m_dataSource.Name.MarkAsModelInfo(), ex2.ToString());
							}
							if (transactionInfo != null)
							{
								transactionInfo.RollbackRequired = true;
							}
							throw ex2;
						}
						finally
						{
							if (flag2 && transactionInfo.RollbackRequired)
							{
								if (Global.Tracer.TraceError)
								{
									Global.Tracer.Trace(TraceLevel.Error, "Data source '{0}': Rolling the transaction back.", base.m_dataSource.Name.MarkAsModelInfo());
								}
								try
								{
									transactionInfo.Transaction.Rollback();
								}
								catch (Exception innerException2)
								{
									throw new ReportProcessingException(ErrorCode.rsErrorRollbackTransaction, innerException2, base.m_dataSource.Name.MarkAsModelInfo());
								}
							}
							if (flag)
							{
								try
								{
									ReportProcessingContext reportProcessingContext2 = base.m_processingContext as ReportProcessingContext;
									Global.Tracer.Assert(reportProcessingContext2 != null, "rptContext == null in closeConnection");
									reportProcessingContext2.DataExtensionConnection.CloseConnectionWithoutPool(dbConnection);
								}
								catch (Exception innerException3)
								{
									throw new ReportProcessingException(ErrorCode.rsErrorClosingConnection, innerException3, base.m_dataSource.Name.MarkAsModelInfo());
								}
							}
						}
					}
				}
			}

			private bool NeedAutoDetectCollation(out int index)
			{
				Global.Tracer.Assert(null != base.m_dataSource.DataSets, "(null != m_dataSource.DataSets)");
				bool flag = false;
				int count = base.m_dataSource.DataSets.Count;
				index = 0;
				if (base.m_processingContext.ProcessReportParameters && base.m_dataSource.DataSets[base.m_dataSetIndex].NeedAutoDetectCollation())
				{
					flag = true;
					index = base.m_dataSetIndex;
				}
				else
				{
					while (index < count && !flag)
					{
						DataSet dataSet = base.m_dataSource.DataSets[index];
						if (!dataSet.UsedOnlyInParameters && dataSet.NeedAutoDetectCollation())
						{
							flag = true;
						}
						else
						{
							index++;
						}
					}
				}
				return flag;
			}
		}

		internal sealed class RuntimeDataSetNodeList : ArrayList
		{
			internal new RuntimeDataSetNode this[int index]
			{
				get
				{
					return (RuntimeDataSetNode)base[index];
				}
			}
		}

		internal abstract class RuntimeDataSetNode : IFilterOwner
		{
			protected DataSource m_dataSource;

			protected IDbConnection m_dataSourceConnection;

			protected TransactionInfo m_transInfo;

			protected Report m_report;

			protected DataSet m_dataSet;

			protected IDbCommand m_command;

			protected ProcessingDataReader m_dataReader;

			protected ProcessingContext m_processingContext;

			protected int m_dataRowsRead;

			protected long m_dataProcessingDurationMs;

			private Hashtable[] m_fieldAliasPropertyNames;

			protected Hashtable[] m_referencedAliasPropertyNames;

			protected bool m_foundExtendedProperties;

			protected bool m_hasSortFilterInfo;

			internal DataSet DataSet
			{
				get
				{
					return this.m_dataSet;
				}
			}

			internal bool NoRows
			{
				get
				{
					return this.m_dataRowsRead <= 0;
				}
			}

			internal int NumRowsRead
			{
				get
				{
					return this.m_dataRowsRead;
				}
			}

			internal long DataProcessingDurationMs
			{
				get
				{
					return this.m_dataProcessingDurationMs;
				}
			}

			internal FieldsImpl Fields
			{
				get
				{
					if (this.m_processingContext != null && this.m_processingContext.ReportObjectModel != null)
					{
						return this.m_processingContext.ReportObjectModel.FieldsImpl;
					}
					return null;
				}
			}

			internal ProcessingContext ProcessingContext
			{
				get
				{
					return this.m_processingContext;
				}
			}

			internal RuntimeDataSetNode(Report report, DataSet dataSet, ProcessingContext processingContext)
			{
				this.m_report = report;
				this.m_dataSet = dataSet;
				this.m_processingContext = processingContext.CloneContext(processingContext);
			}

			internal void Abort()
			{
				IDbCommand command = this.m_command;
				if (command != null)
				{
					if (Global.Tracer.TraceVerbose)
					{
						Global.Tracer.Trace(TraceLevel.Verbose, "Data set '{0}': Cancelling command.", this.m_dataSet.Name.MarkAsPrivate());
					}
					command.Cancel();
				}
			}

			internal void Cleanup()
			{
				if (this.m_processingContext.ReportRuntime != null)
				{
					this.m_processingContext.ReportRuntime.Close();
				}
			}

			internal void InitProcessingParams(DataSource dataSource, IDbConnection conn, TransactionInfo transInfo)
			{
				this.m_dataSource = dataSource;
				this.m_dataSourceConnection = conn;
				this.m_transInfo = transInfo;
			}

			internal void ProcessConcurrent(object threadSet)
			{
				CultureInfo cultureInfo = null;
				Global.Tracer.Assert(this.m_dataSet.Name != null, "The name of a data set cannot be null.");
				try
				{
					if (Global.Tracer.TraceVerbose)
					{
						Global.Tracer.Trace(TraceLevel.Verbose, "Thread has started processing data set '{0}'", this.m_dataSet.Name.MarkAsPrivate());
					}
					if (this.m_processingContext.ThreadCulture != null)
					{
						cultureInfo = Thread.CurrentThread.CurrentCulture;
						Thread.CurrentThread.CurrentCulture = this.m_processingContext.ThreadCulture;
					}
					if (DataSetValidator.LOCALE_SYSTEM_DEFAULT == this.m_dataSet.LCID)
					{
						this.m_dataSet.LCID = DataSetValidator.LCIDfromRDLCollation(this.m_dataSet.Collation);
					}
					this.m_processingContext.CompareInfo = new CultureInfo((int)this.m_dataSet.LCID, false).CompareInfo;
					this.m_processingContext.ClrCompareOptions = this.m_dataSet.GetCLRCompareOptions();
					this.Process();
				}
				catch (ProcessingAbortedException)
				{
					if (Global.Tracer.TraceWarning)
					{
						Global.Tracer.Trace(TraceLevel.Warning, "Data set '{0}': Report processing has been aborted.", this.m_dataSet.Name.MarkAsPrivate());
					}
				}
				catch (Exception ex2)
				{
					if (Global.Tracer.TraceError)
					{
						Global.Tracer.Trace(TraceLevel.Error, "An exception has occurred in data source '{0}'. Details: {1}", this.m_dataSet.Name.MarkAsPrivate(), ex2.ToString());
					}
					if (this.m_processingContext.AbortInfo != null)
					{
						this.m_processingContext.AbortInfo.SetError(this.m_processingContext.DataSetUniqueName, ex2);
					}
				}
				finally
				{
					if (Global.Tracer.TraceVerbose)
					{
						Global.Tracer.Trace(TraceLevel.Verbose, "Processing of data set '{0}' completed.", this.m_dataSet.Name.MarkAsPrivate());
					}
					if (cultureInfo != null)
					{
						Thread.CurrentThread.CurrentCulture = cultureInfo;
					}
					ThreadSet threadSet2 = threadSet as ThreadSet;
					if (threadSet2 != null)
					{
						threadSet2.ThreadCompleted();
					}
				}
			}

			protected void InitRuntime(bool processReport)
			{
				Global.Tracer.Assert(this.m_processingContext.ReportObjectModel != null && this.m_processingContext.ReportRuntime != null);
				if (this.m_processingContext.ReportRuntime.ReportExprHost != null)
				{
					this.m_dataSet.SetExprHost(this.m_processingContext.ReportRuntime.ReportExprHost, this.m_processingContext.ReportObjectModel);
				}
				DataFieldList fields = this.m_dataSet.Fields;
				int num = (fields != null) ? fields.Count : 0;
				this.m_processingContext.ReportObjectModel.FieldsImpl = new FieldsImpl(num, processReport && this.m_dataSet.HasDetailUserSortFilter);
				for (int i = 0; i < num; i++)
				{
					Field field = fields[i];
					if (this.m_dataSet.ExprHost != null)
					{
						field.SetExprHost(this.m_dataSet.ExprHost, this.m_processingContext.ReportObjectModel);
					}
					this.m_processingContext.ReportObjectModel.FieldsImpl.Add(field.Name, null);
				}
				if (processReport && this.m_dataSet.HasDetailUserSortFilter)
				{
					this.m_processingContext.ReportObjectModel.FieldsImpl.AddRowIndexField();
				}
				if (this.m_dataSet.Filters != null && this.m_dataSet.ExprHost != null)
				{
					int count = this.m_dataSet.Filters.Count;
					for (int j = 0; j < count; j++)
					{
						this.m_dataSet.Filters[j].SetExprHost(this.m_dataSet.ExprHost.FilterHostsRemotable, this.m_processingContext.ReportObjectModel);
					}
				}
				if (this.m_processingContext.ReportRuntime.ReportExprHost != null)
				{
					this.RuntimeInitializeReportItemObjs();
				}
				this.RegisterAggregates();
			}

			protected abstract void Process();

			protected abstract void FirstPassProcessDetailRow(Filters filters);

			protected abstract bool FirstPassGetNextDetailRow();

			protected abstract void FirstPassInit();

			protected abstract void NextNonAggregateRow();

			internal virtual void RuntimeInitializeReportItemObjs()
			{
			}

			internal virtual void EraseDataChunk()
			{
			}

			protected virtual void RegisterAggregates()
			{
			}

			protected virtual void FirstPassCleanup(bool flushData)
			{
			}

			void IFilterOwner.PostFilterNextRow()
			{
				this.NextNonAggregateRow();
			}

			protected void FirstPassProcess(ref bool closeConnWhenFinish)
			{
				if (this.m_dataSourceConnection == null && !this.m_processingContext.SnapshotProcessing && !this.m_processingContext.ProcessWithCachedData)
				{
					closeConnWhenFinish = true;
				}
				this.FirstPassInit();
				this.m_processingContext.CheckAndThrowIfAborted();
				this.FirstPass();
				this.m_processingContext.CheckAndThrowIfAborted();
				if (closeConnWhenFinish)
				{
					Global.Tracer.Assert(null != this.m_dataSourceConnection, "(null != m_dataSourceConnection)");
					try
					{
						this.m_processingContext.DataExtensionConnection.CloseConnectionWithoutPool(this.m_dataSourceConnection);
					}
					catch (Exception innerException)
					{
						throw new ReportProcessingException(ErrorCode.rsErrorClosingConnection, innerException, this.m_dataSource.Name.MarkAsModelInfo());
					}
					this.m_dataSourceConnection = null;
				}
			}

			private void FirstPass()
			{
				Filters filters = null;
				if (this.m_dataSet.Filters != null)
				{
					filters = new Filters(Filters.FilterTypes.DataSetFilter, this, this.m_dataSet.Filters, this.m_dataSet.ObjectType, this.m_dataSet.Name, this.m_processingContext);
				}
				bool flushData = false;
				try
				{
					this.m_dataRowsRead = 0;
					while (this.FirstPassGetNextDetailRow())
					{
						this.FirstPassProcessDetailRow(filters);
					}
					this.m_dataSet.RecordSetSize = this.m_dataRowsRead;
					flushData = true;
				}
				finally
				{
					if (this.m_dataReader != null)
					{
						((IDisposable)this.m_dataReader).Dispose();
						this.m_dataReader = null;
					}
					this.CloseCommand();
					this.FirstPassCleanup(flushData);
				}
				if (filters != null)
				{
					filters.FinishReadingRows();
				}
			}

			private void CloseCommand()
			{
				if (this.m_command != null)
				{
					IDbCommand command = this.m_command;
					this.m_command = null;
					command.Dispose();
					command = null;
				}
			}

			protected bool GetNextDetailRow()
			{
				bool result = false;
				bool flag = 0 == this.m_dataRowsRead;
				AspNetCore.ReportingServices.Diagnostics.Timer timer = null;
				if (this.m_processingContext.JobContext != null)
				{
					timer = new AspNetCore.ReportingServices.Diagnostics.Timer();
					timer.StartTimer();
				}
				FieldsImpl fieldsImpl = null;
				if (this.m_dataReader != null && this.m_dataReader.GetNextRow())
				{
					fieldsImpl = this.m_processingContext.ReportObjectModel.FieldsImpl;
					if (flag)
					{
						this.m_fieldAliasPropertyNames = new Hashtable[fieldsImpl.Count];
						this.m_referencedAliasPropertyNames = new Hashtable[fieldsImpl.Count];
					}
					fieldsImpl.NewRow();
					if (fieldsImpl.ReaderExtensionsSupported && !this.m_dataSet.InterpretSubtotalsAsDetails)
					{
						fieldsImpl.IsAggregateRow = this.m_dataReader.IsAggregateRow;
						fieldsImpl.AggregationFieldCount = this.m_dataReader.AggregationFieldCount;
						if (!fieldsImpl.IsAggregateRow)
						{
							fieldsImpl.AggregationFieldCountForDetailRow = fieldsImpl.AggregationFieldCount;
						}
					}
					bool flag2 = false;
					for (int i = 0; i < fieldsImpl.Count; i++)
					{
						Field field = this.m_dataSet.Fields[i];
						if (field.IsCalculatedField)
						{
							CalculatedFieldWrapperImpl value = new CalculatedFieldWrapperImpl(field, this.m_processingContext.ReportRuntime);
							if (this.m_dataSet.InterpretSubtotalsAsDetails)
							{
								fieldsImpl[i] = new FieldImpl(value, true, field);
							}
							else
							{
								fieldsImpl[i] = new FieldImpl(value, (byte)((!fieldsImpl.ReaderExtensionsSupported) ? 1 : 0) != 0, field);
							}
							flag2 = true;
						}
						else
						{
							Global.Tracer.Assert(!flag2, "(!inCalculatedFields)");
							try
							{
								if (flag || !fieldsImpl.IsFieldMissing(i))
								{
									if (this.m_dataSet.InterpretSubtotalsAsDetails)
									{
										fieldsImpl[i] = new FieldImpl(this.m_dataReader.GetColumn(i), true, field);
									}
									else
									{
										fieldsImpl[i] = new FieldImpl(this.m_dataReader.GetColumn(i), !fieldsImpl.ReaderExtensionsSupported || this.m_dataReader.IsAggregationField(i), field);
									}
									if (fieldsImpl.ReaderFieldProperties)
									{
										int num = 0;
										if (this.m_fieldAliasPropertyNames[i] != null)
										{
											num = this.m_fieldAliasPropertyNames[i].Count;
										}
										else
										{
											num = this.m_dataReader.GetPropertyCount(i);
											this.m_fieldAliasPropertyNames[i] = new Hashtable(num);
											this.m_referencedAliasPropertyNames[i] = new Hashtable(num);
											this.m_foundExtendedProperties = true;
										}
										for (int j = 0; j < num; j++)
										{
											string text = null;
											if (flag)
											{
												text = this.m_dataReader.GetPropertyName(i, j);
												this.m_fieldAliasPropertyNames[i].Add(j, text);
											}
											else
											{
												Global.Tracer.Assert(this.m_fieldAliasPropertyNames[i].ContainsKey(j), "(m_fieldAliasPropertyNames[i].ContainsKey(j))");
												text = (this.m_fieldAliasPropertyNames[i][j] as string);
											}
											if (this.m_processingContext.CacheDataCallback != null || this.m_dataSet.DynamicFieldReferences || field.DynamicPropertyReferences || (field.ReferencedProperties != null && field.ReferencedProperties.ContainsKey(text)))
											{
												if (flag)
												{
													this.m_referencedAliasPropertyNames[i].Add(j, text);
												}
												object propertyValue = this.m_dataReader.GetPropertyValue(i, j);
												fieldsImpl[i].SetProperty(text, propertyValue);
											}
										}
									}
								}
								else
								{
									fieldsImpl[i] = new FieldImpl(DataFieldStatus.IsMissing, null, field);
								}
							}
							catch (ReportProcessingException_FieldError reportProcessingException_FieldError)
							{
								bool flag3 = false;
								if (this.m_dataRowsRead == 0 && DataFieldStatus.UnSupportedDataType != reportProcessingException_FieldError.Status && DataFieldStatus.Overflow != reportProcessingException_FieldError.Status)
								{
									fieldsImpl.SetFieldIsMissing(i);
									fieldsImpl[i] = new FieldImpl(DataFieldStatus.IsMissing, reportProcessingException_FieldError.Message, field);
									flag3 = true;
									this.m_processingContext.ErrorContext.Register(ProcessingErrorCode.rsMissingFieldInDataSet, Severity.Warning, ObjectType.DataSet, this.m_dataSet.Name, "Field", field.Name.MarkAsModelInfo());
								}
								if (!flag3)
								{
									fieldsImpl[i] = new FieldImpl(reportProcessingException_FieldError.Status, reportProcessingException_FieldError.Message, field);
								}
								if (!fieldsImpl.IsFieldErrorRegistered(i))
								{
									fieldsImpl.SetFieldErrorRegistered(i);
									if (DataFieldStatus.UnSupportedDataType == reportProcessingException_FieldError.Status)
									{
										this.m_processingContext.ErrorContext.Register(ProcessingErrorCode.rsDataSetFieldTypeNotSupported, Severity.Warning, ObjectType.DataSet, this.m_dataSet.Name, "Field", field.Name.MarkAsModelInfo());
									}
									else
									{
										this.m_processingContext.ErrorContext.Register(ProcessingErrorCode.rsErrorReadingDataSetField, Severity.Warning, ObjectType.DataSet, this.m_dataSet.Name, "Field", field.Name.MarkAsModelInfo(), reportProcessingException_FieldError.Message);
									}
								}
							}
						}
					}
					this.m_dataRowsRead++;
					if (fieldsImpl.AddRowIndex)
					{
						fieldsImpl.SetRowIndex(this.m_dataRowsRead);
					}
					result = true;
				}
				if (this.m_processingContext.JobContext != null)
				{
					this.m_dataProcessingDurationMs += timer.ElapsedTimeMs();
				}
				return result;
			}

			protected virtual bool RunDataSetQuery()
			{
				bool result = false;
				bool flag = false;
				if (this.m_dataSet.Query == null)
				{
					return result;
				}
				ParameterValueList parameters = this.m_dataSet.Query.Parameters;
				object[] array = new object[(parameters != null) ? parameters.Count : 0];
				for (int i = 0; i < array.Length; i++)
				{
					ParameterValue parameterValue = parameters[i];
					this.m_processingContext.CheckAndThrowIfAborted();
					array[i] = this.m_processingContext.ReportRuntime.EvaluateQueryParamValue(parameterValue.Value, (this.m_dataSet.ExprHost != null) ? this.m_dataSet.ExprHost.QueryParametersHost : null, ObjectType.QueryParameter, parameterValue.Name);
				}
				this.m_processingContext.CheckAndThrowIfAborted();
				AspNetCore.ReportingServices.Diagnostics.Timer timer = null;
				if (this.m_processingContext.JobContext != null)
				{
					timer = new AspNetCore.ReportingServices.Diagnostics.Timer();
					timer.StartTimer();
				}
				IDataReader dataReader = null;
				IDbCommand dbCommand = null;
				try
				{
					if (this.m_dataSourceConnection == null)
					{
						ReportProcessingContext reportProcessingContext = (ReportProcessingContext)this.m_processingContext;
						AspNetCore.ReportingServices.DataExtensions.DataSourceInfo dataSourceInfo = default(AspNetCore.ReportingServices.DataExtensions.DataSourceInfo);
						string connectionString = this.m_dataSource.ResolveConnectionString(reportProcessingContext, out dataSourceInfo);
						this.m_dataSourceConnection = reportProcessingContext.DataExtensionConnection.OpenDataSourceExtensionConnection(this.m_dataSource, connectionString, dataSourceInfo, null);
					}
					try
					{
						dbCommand = this.m_dataSourceConnection.CreateCommand();
					}
					catch (Exception innerException)
					{
						throw new ReportProcessingException(ErrorCode.rsErrorCreatingCommand, innerException, this.m_dataSource.Name.MarkAsModelInfo());
					}
					for (int j = 0; j < array.Length; j++)
					{
						IDataParameter dataParameter;
						try
						{
							dataParameter = dbCommand.CreateParameter();
						}
						catch (Exception innerException2)
						{
							throw new ReportProcessingException(ErrorCode.rsErrorCreatingQueryParameter, innerException2, this.m_dataSet.Name.MarkAsPrivate());
						}
						dataParameter.ParameterName = parameters[j].Name;
						bool flag2 = dataParameter is IDataMultiValueParameter && array[j] is ICollection;
						object obj = array[j];
						if (obj == null)
						{
							obj = DBNull.Value;
						}
						if (!(dataParameter is IDataMultiValueParameter) && array[j] is ICollection)
						{
							throw new ReportProcessingException(ErrorCode.rsErrorAddingMultiValueQueryParameter, null, this.m_dataSet.Name.MarkAsPrivate(), dataParameter.ParameterName.MarkAsPrivate());
						}
						if (flag2)
						{
							int count = ((ICollection)obj).Count;
							if (1 == count)
							{
								try
								{
									Global.Tracer.Assert(obj is object[], "(paramValue is object[])");
									dataParameter.Value = (obj as object[])[0];
								}
								catch (Exception innerException3)
								{
									throw new ReportProcessingException(ErrorCode.rsErrorAddingQueryParameter, innerException3, this.m_dataSource.Name.MarkAsModelInfo());
								}
							}
							else
							{
								object[] array2 = new object[count];
								((ICollection)obj).CopyTo(array2, 0);
								((IDataMultiValueParameter)dataParameter).Values = array2;
							}
						}
						else
						{
							try
							{
								dataParameter.Value = obj;
							}
							catch (Exception innerException4)
							{
								throw new ReportProcessingException(ErrorCode.rsErrorAddingQueryParameter, innerException4, this.m_dataSource.Name.MarkAsModelInfo());
							}
						}
						try
						{
							dbCommand.Parameters.Add(dataParameter);
						}
						catch (Exception innerException5)
						{
							throw new ReportProcessingException(ErrorCode.rsErrorAddingQueryParameter, innerException5, this.m_dataSource.Name.MarkAsModelInfo());
						}
					}
					this.m_processingContext.CheckAndThrowIfAborted();
					try
					{
						if (this.m_dataSet.Query.CommandText != null)
						{
							StringResult stringResult = this.m_processingContext.ReportRuntime.EvaluateCommandText(this.m_dataSet);
							if (stringResult.ErrorOccurred)
							{
								throw new ReportProcessingException(ErrorCode.rsQueryCommandTextProcessingError, this.m_dataSet.Name.MarkAsPrivate());
							}
							dbCommand.CommandText = stringResult.Value;
							this.m_dataSet.Query.CommandTextValue = stringResult.Value;
						}
					}
					catch (Exception innerException6)
					{
						throw new ReportProcessingException(ErrorCode.rsErrorSettingCommandText, innerException6, this.m_dataSet.Name.MarkAsPrivate());
					}
					try
					{
						dbCommand.CommandType = (CommandType)this.m_dataSet.Query.CommandType;
					}
					catch (Exception innerException7)
					{
						throw new ReportProcessingException(ErrorCode.rsErrorSettingCommandType, innerException7, this.m_dataSet.Name.MarkAsPrivate());
					}
					if (this.m_transInfo != null)
					{
						try
						{
							dbCommand.Transaction = this.m_transInfo.Transaction;
						}
						catch (Exception innerException8)
						{
							throw new ReportProcessingException(ErrorCode.rsErrorSettingTransaction, innerException8, this.m_dataSet.Name.MarkAsPrivate());
						}
					}
					this.m_processingContext.CheckAndThrowIfAborted();
					try
					{
						if (this.m_dataSet.Query.TimeOut == 0 && dbCommand is CommandWrapper && ((CommandWrapper)dbCommand).UnderlyingCommand is SqlCommand)
						{
							dbCommand.CommandTimeout = 2147483646;
						}
						else
						{
							dbCommand.CommandTimeout = this.m_dataSet.Query.TimeOut;
						}
					}
					catch (Exception innerException9)
					{
						throw new ReportProcessingException(ErrorCode.rsErrorSettingQueryTimeout, innerException9, this.m_dataSet.Name.MarkAsPrivate());
					}
					if (dbCommand is IDbCommandRewriter)
					{
						this.m_dataSet.Query.RewrittenCommandText = ((IDbCommandRewriter)dbCommand).RewrittenCommandText;
						this.m_processingContext.DrillthroughInfo.AddRewrittenCommand(this.m_dataSet.ID, this.m_dataSet.Query.RewrittenCommandText);
					}
					this.m_command = dbCommand;
					IJobContext jobContext = this.m_processingContext.JobContext;
					try
					{
						if (jobContext != null)
						{
							jobContext.AddCommand(this.m_command);
						}
						try
						{
							dataReader = this.m_command.ExecuteReader(CommandBehavior.SingleResult);
						}
						catch (Exception innerException10)
						{
							throw new ReportProcessingException(ErrorCode.rsErrorExecutingCommand, innerException10, this.m_dataSet.Name.MarkAsPrivate());
						}
					}
					finally
					{
						if (jobContext != null)
						{
							jobContext.RemoveCommand(this.m_command);
						}
					}
					if (dataReader == null)
					{
						if (Global.Tracer.TraceError)
						{
							Global.Tracer.Trace(TraceLevel.Error, "The source data reader is null. Cannot read results.");
						}
						throw new ReportProcessingException(ErrorCode.rsErrorCreatingDataReader, this.m_dataSet.Name.MarkAsPrivate());
					}
					result = (dataReader is IDataReaderExtension);
					flag = (dataReader is IDataReaderFieldProperties);
					if (dataReader.FieldCount > 0)
					{
						this.m_dataSet.CheckNonCalculatedFieldCount();
						DataFieldList fields = this.m_dataSet.Fields;
						int num = (fields != null) ? this.m_dataSet.NonCalculatedFieldCount : 0;
						string[] array3 = new string[num];
						string[] array4 = new string[num];
						for (int k = 0; k < num; k++)
						{
							Field field = fields[k];
							array3[k] = field.DataField;
							array4[k] = field.Name;
						}
						this.m_dataReader = new ProcessingDataReader(this.m_dataSet.Name, dataReader, array4, array3);
					}
					this.m_processingContext.ReportObjectModel.FieldsImpl.ReaderExtensionsSupported = result;
					this.m_processingContext.ReportObjectModel.FieldsImpl.ReaderFieldProperties = flag;
					return result;
				}
				catch (Exception e)
				{
					if (AsynchronousExceptionDetection.IsStoppingException(e))
					{
						throw;
					}
					if (dataReader != null)
					{
						dataReader.Dispose();
						dataReader = null;
					}
					if (dbCommand != null)
					{
						dbCommand.Dispose();
						dbCommand = null;
					}
					throw;
				}
				finally
				{
					if (this.m_processingContext.JobContext != null)
					{
						this.m_dataProcessingDurationMs += timer.ElapsedTimeMs();
					}
				}
			}
		}

		private class RuntimeReportDataSetNode : RuntimeDataSetNode, IScope, IHierarchyObj
		{
			private const int m_chartDataRowCount = 1000;

			private RuntimeDRCollection m_runtimeDataRegions;

			private DataAggregateObjList m_nonCustomAggregates;

			private DataAggregateObjList m_customAggregates;

			private ChunkManager.DataChunkWriter m_dataChunkWriter;

			private bool m_dataChunkSaved;

			private DataRowList m_dataRows;

			private RuntimeUserSortTargetInfo m_userSortTargetInfo;

			private int[] m_sortFilterExpressionScopeInfoIndices;

			internal bool HasSortFilterInfo
			{
				get
				{
					return base.m_hasSortFilterInfo;
				}
			}

			bool IScope.TargetForNonDetailSort
			{
				get
				{
					if (this.m_userSortTargetInfo != null)
					{
						return this.m_userSortTargetInfo.TargetForNonDetailSort;
					}
					return false;
				}
			}

			int[] IScope.SortFilterExpressionScopeInfoIndices
			{
				get
				{
					if (this.m_sortFilterExpressionScopeInfoIndices == null)
					{
						this.m_sortFilterExpressionScopeInfoIndices = new int[base.m_processingContext.RuntimeSortFilterInfo.Count];
						for (int i = 0; i < base.m_processingContext.RuntimeSortFilterInfo.Count; i++)
						{
							this.m_sortFilterExpressionScopeInfoIndices[i] = -1;
						}
					}
					return this.m_sortFilterExpressionScopeInfoIndices;
				}
			}

			IHierarchyObj IHierarchyObj.HierarchyRoot
			{
				get
				{
					return this;
				}
			}

			ProcessingContext IHierarchyObj.ProcessingContext
			{
				get
				{
					return base.m_processingContext;
				}
			}

			BTreeNode IHierarchyObj.SortTree
			{
				get
				{
					if (this.m_userSortTargetInfo != null)
					{
						return this.m_userSortTargetInfo.SortTree;
					}
					return null;
				}
				set
				{
					if (this.m_userSortTargetInfo != null)
					{
						this.m_userSortTargetInfo.SortTree = value;
					}
					else
					{
						Global.Tracer.Assert(false);
					}
				}
			}

			int IHierarchyObj.ExpressionIndex
			{
				get
				{
					return 0;
				}
			}

			IntList IHierarchyObj.SortFilterInfoIndices
			{
				get
				{
					if (this.m_userSortTargetInfo != null)
					{
						return this.m_userSortTargetInfo.SortFilterInfoIndices;
					}
					return null;
				}
			}

			bool IHierarchyObj.IsDetail
			{
				get
				{
					return false;
				}
			}

			internal RuntimeReportDataSetNode(Report report, DataSet dataSet, ProcessingContext processingContext)
				: base(report, dataSet, processingContext)
			{
				base.m_hasSortFilterInfo = base.m_processingContext.PopulateRuntimeSortFilterEventInfo(base.m_dataSet);
				UserSortFilterContext userSortFilterContext = base.m_processingContext.UserSortFilterContext;
				if (-1 == userSortFilterContext.DataSetID)
				{
					userSortFilterContext.DataSetID = base.m_dataSet.ID;
				}
				if (base.m_processingContext.IsSortFilterTarget(base.m_dataSet.IsSortFilterTarget, userSortFilterContext.CurrentContainingScope, (IHierarchyObj)this, ref this.m_userSortTargetInfo) && this.m_userSortTargetInfo.TargetForNonDetailSort)
				{
					this.m_dataRows = new DataRowList();
				}
			}

			bool IScope.IsTargetForSort(int index, bool detailSort)
			{
				if (this.m_userSortTargetInfo != null)
				{
					return this.m_userSortTargetInfo.IsTargetForSort(index, detailSort);
				}
				return false;
			}

			bool IScope.InScope(string scope)
			{
				if (ReportProcessing.CompareWithInvariantCulture(base.m_dataSet.Name, scope, false) == 0)
				{
					return true;
				}
				return false;
			}

			void IScope.ReadRow(DataActions dataAction)
			{
				Global.Tracer.Assert(false);
			}

			IScope IScope.GetOuterScope(bool includeSubReportContainingScope)
			{
				if (includeSubReportContainingScope)
				{
					return base.m_processingContext.UserSortFilterContext.CurrentContainingScope;
				}
				return null;
			}

			string IScope.GetScopeName()
			{
				return base.m_dataSet.Name;
			}

			int IScope.RecursiveLevel(string scope)
			{
				return 0;
			}

			bool IScope.TargetScopeMatched(int index, bool detailSort)
			{
				if (base.m_processingContext.UserSortFilterContext.CurrentContainingScope != null)
				{
					return base.m_processingContext.UserSortFilterContext.CurrentContainingScope.TargetScopeMatched(index, detailSort);
				}
				if (base.m_processingContext.RuntimeSortFilterInfo != null)
				{
					return true;
				}
				return false;
			}

			void IScope.GetScopeValues(IHierarchyObj targetScopeObj, VariantList[] scopeValues, ref int index)
			{
				IScope currentContainingScope = base.m_processingContext.UserSortFilterContext.CurrentContainingScope;
				if (this != targetScopeObj && currentContainingScope != null)
				{
					Global.Tracer.Assert(null == targetScopeObj, "(null == targetScopeObj)");
					currentContainingScope.GetScopeValues((IHierarchyObj)null, scopeValues, ref index);
				}
			}

			void IScope.GetGroupNameValuePairs(Dictionary<string, object> pairs)
			{
			}

			IHierarchyObj IHierarchyObj.CreateHierarchyObj()
			{
				return new RuntimeSortHierarchyObj(this);
			}

			ProcessingMessageList IHierarchyObj.RegisterComparisonError(string propertyName)
			{
				return base.m_processingContext.RegisterComparisonErrorForSortFilterEvent(propertyName);
			}

			void IHierarchyObj.NextRow()
			{
				Global.Tracer.Assert(false);
			}

			void IHierarchyObj.Traverse(ProcessingStages operation)
			{
				Global.Tracer.Assert(false);
			}

			void IHierarchyObj.ReadRow()
			{
				this.SendToInner();
			}

			void IHierarchyObj.ProcessUserSort()
			{
				Global.Tracer.Assert(null != this.m_userSortTargetInfo, "(null != m_userSortTargetInfo)");
				base.m_processingContext.ProcessUserSortForTarget((IHierarchyObj)this, ref this.m_dataRows, this.m_userSortTargetInfo.TargetForNonDetailSort);
				if (this.m_userSortTargetInfo.TargetForNonDetailSort)
				{
					this.m_userSortTargetInfo.ResetTargetForNonDetailSort();
					this.m_userSortTargetInfo.EnterProcessUserSortPhase(base.m_processingContext);
					this.m_runtimeDataRegions = new RuntimeDRCollection(this, base.m_dataSet.DataRegions, base.m_processingContext, base.m_report.MergeOnePass);
					this.m_userSortTargetInfo.SortTree.Traverse(ProcessingStages.UserSortFilter, true);
					this.m_userSortTargetInfo.SortTree = null;
					if (this.m_userSortTargetInfo.AggregateRows != null)
					{
						for (int i = 0; i < this.m_userSortTargetInfo.AggregateRows.Count; i++)
						{
							this.m_userSortTargetInfo.AggregateRows[i].SetFields(base.m_processingContext);
							this.SendToInner();
						}
						this.m_userSortTargetInfo.AggregateRows = null;
					}
					this.m_userSortTargetInfo.LeaveProcessUserSortPhase(base.m_processingContext);
				}
			}

			void IHierarchyObj.MarkSortInfoProcessed(RuntimeSortFilterEventInfoList runtimeSortFilterInfo)
			{
				if (this.m_userSortTargetInfo != null)
				{
					this.m_userSortTargetInfo.MarkSortInfoProcessed(runtimeSortFilterInfo, this);
				}
			}

			void IHierarchyObj.AddSortInfoIndex(int sortInfoIndex, RuntimeSortFilterEventInfo sortInfo)
			{
				if (this.m_userSortTargetInfo != null)
				{
					this.m_userSortTargetInfo.AddSortInfoIndex(sortInfoIndex, sortInfo);
				}
			}

			internal override void RuntimeInitializeReportItemObjs()
			{
				for (int i = 0; i < base.m_dataSet.DataRegions.Count; i++)
				{
					base.m_processingContext.RuntimeInitializeReportItemObjs(base.m_dataSet.DataRegions[i], true, false);
				}
			}

			internal override void EraseDataChunk()
			{
				if (this.m_dataChunkSaved)
				{
					this.m_dataChunkWriter = new ChunkManager.DataChunkWriter(base.m_dataSet, base.m_processingContext);
					this.m_dataChunkWriter.CloseAndEraseChunk();
				}
			}

			protected override void Process()
			{
				bool flag = false;
				try
				{
					base.FirstPassProcess(ref flag);
					base.m_processingContext.FirstPassPostProcess();
					if (!base.m_report.MergeOnePass)
					{
						if (base.m_processingContext.OWCChartName == null)
						{
							base.m_processingContext.CheckAndThrowIfAborted();
							this.SecondPass();
						}
						base.m_processingContext.CheckAndThrowIfAborted();
						this.ThirdPass();
					}
				}
				catch (Exception e)
				{
					if (AsynchronousExceptionDetection.IsStoppingException(e))
					{
						throw;
					}
					if (base.m_transInfo != null)
					{
						base.m_transInfo.RollbackRequired = true;
					}
					if (flag && base.m_dataSourceConnection != null)
					{
						base.m_processingContext.DataExtensionConnection.CloseConnectionWithoutPool(base.m_dataSourceConnection);
					}
					throw;
				}
				finally
				{
					if (base.m_dataReader != null)
					{
						((IDisposable)base.m_dataReader).Dispose();
						base.m_dataReader = null;
					}
					if (this.m_dataChunkWriter != null)
					{
						this.m_dataChunkWriter.Close();
						this.m_dataChunkWriter = null;
					}
				}
			}

			protected override void RegisterAggregates()
			{
				this.CreateAggregates(base.m_dataSet.Aggregates);
				this.CreateAggregates(base.m_dataSet.PostSortAggregates);
			}

			private void CreateAggregates(DataAggregateInfoList aggDefs)
			{
				if (aggDefs != null && 0 < aggDefs.Count)
				{
					for (int i = 0; i < aggDefs.Count; i++)
					{
						DataAggregateInfo dataAggregateInfo = aggDefs[i];
						DataAggregateObj dataAggregateObj = new DataAggregateObj(dataAggregateInfo, base.m_processingContext);
						base.m_processingContext.ReportObjectModel.AggregatesImpl.Add(dataAggregateObj);
						if (DataAggregateInfo.AggregateTypes.Previous != dataAggregateInfo.AggregateType)
						{
							if (DataAggregateInfo.AggregateTypes.Aggregate == dataAggregateInfo.AggregateType)
							{
								RuntimeDataRegionObj.AddAggregate(ref this.m_customAggregates, dataAggregateObj);
							}
							else
							{
								RuntimeDataRegionObj.AddAggregate(ref this.m_nonCustomAggregates, dataAggregateObj);
							}
						}
					}
				}
			}

			protected override void FirstPassInit()
			{
				bool flag = false;
				base.InitRuntime(true);
				this.m_runtimeDataRegions = new RuntimeDRCollection(this, base.m_dataSet.DataRegions, base.m_processingContext, base.m_report.MergeOnePass);
				if (base.m_processingContext.SnapshotProcessing || base.m_processingContext.ProcessWithCachedData)
				{
					base.m_dataSet.CheckNonCalculatedFieldCount();
					base.m_dataReader = new ProcessingDataReader(base.m_dataSet, base.m_processingContext);
					flag = base.m_dataReader.ReaderExtensionsSupported;
					base.m_processingContext.ReportObjectModel.FieldsImpl.ReaderExtensionsSupported = flag;
					base.m_processingContext.ReportObjectModel.FieldsImpl.ReaderFieldProperties = base.m_dataReader.ReaderFieldProperties;
					if (base.m_dataSet.Query.RewrittenCommandText != null)
					{
						base.m_processingContext.DrillthroughInfo.AddRewrittenCommand(base.m_dataSet.ID, base.m_dataSet.Query.RewrittenCommandText);
					}
					base.m_dataReader.OverrideDataCacheCompareOptions(ref base.m_processingContext);
				}
				else
				{
					flag = this.RunDataSetQuery();
					if ((base.m_processingContext.HasUserProfileState & UserProfileState.InQuery) > UserProfileState.None && base.m_processingContext.SaveSnapshotData && !base.m_processingContext.HasUserSortFilter && (base.m_report.SubReports == null || 0 >= base.m_report.SubReports.Count))
					{
						base.m_processingContext.SaveSnapshotData = false;
					}
				}
				Global.Tracer.Assert(null != base.m_processingContext.ReportObjectModel.DataSetsImpl, "(null != m_processingContext.ReportObjectModel.DataSetsImpl)");
				base.m_processingContext.ReportObjectModel.DataSetsImpl.Add(base.m_dataSet);
				if (!base.m_processingContext.ResetForSubreportDataPrefetch)
				{
					if (!base.m_processingContext.SaveSnapshotData || base.m_processingContext.CreateReportChunkCallback == null)
					{
						if (!base.m_processingContext.DataCached)
						{
							return;
						}
						if (base.m_processingContext.CacheDataCallback == null)
						{
							return;
						}
					}
					this.m_dataChunkWriter = new ChunkManager.DataChunkWriter(base.m_dataSet, base.m_processingContext, flag, base.m_processingContext.StopSaveSnapshotDataOnError);
					if (base.m_processingContext.SaveSnapshotData && base.m_processingContext.CreateReportChunkCallback != null)
					{
						this.m_dataChunkSaved = true;
					}
				}
			}

			protected override void FirstPassProcessDetailRow(Filters filters)
			{
				if (base.m_processingContext.ReportObjectModel.FieldsImpl.IsAggregateRow)
				{
					this.NextAggregateRow();
				}
				else
				{
					bool flag = true;
					if (filters != null)
					{
						flag = filters.PassFilters(base.m_processingContext.ReportObjectModel.FieldsImpl.GetFields());
					}
					if (flag)
					{
						((IFilterOwner)this).PostFilterNextRow();
					}
				}
			}

			protected override void FirstPassCleanup(bool flushData)
			{
				if (this.m_dataChunkWriter != null)
				{
					bool flag = true;
					if (flushData)
					{
						flag = !this.m_dataChunkWriter.FinalFlush();
					}
					else
					{
						this.m_dataChunkWriter.Close();
					}
					if (flag)
					{
						base.m_processingContext.ErrorSavingSnapshotData = true;
						base.m_processingContext.DataCached = false;
						this.m_dataChunkSaved = !base.m_processingContext.StopSaveSnapshotDataOnError;
					}
					this.m_dataChunkWriter = null;
				}
			}

			private void SecondPass()
			{
				if (base.m_report.NeedPostGroupProcessing)
				{
					if (base.m_report.HasSpecialRecursiveAggregates)
					{
						base.m_processingContext.SecondPassOperation = ProcessingContext.SecondPassOperations.Filtering;
					}
					else
					{
						base.m_processingContext.SecondPassOperation = (ProcessingContext.SecondPassOperations.Sorting | ProcessingContext.SecondPassOperations.Filtering);
					}
					if (this.m_userSortTargetInfo != null)
					{
						this.m_userSortTargetInfo.EnterProcessUserSortPhase(base.m_processingContext);
					}
					this.m_runtimeDataRegions.SortAndFilter();
					if (base.m_report.HasSpecialRecursiveAggregates)
					{
						base.m_processingContext.SecondPassOperation = ProcessingContext.SecondPassOperations.Sorting;
						this.m_runtimeDataRegions.SortAndFilter();
					}
					if (this.m_userSortTargetInfo != null)
					{
						this.m_userSortTargetInfo.LeaveProcessUserSortPhase(base.m_processingContext);
					}
				}
			}

			private void ThirdPass()
			{
				if (base.m_report.HasPostSortAggregates)
				{
					AggregatesImpl aggregatesImpl = new AggregatesImpl(base.m_processingContext.ReportRuntime);
					RuntimeGroupRootObjList groupCollection = new RuntimeGroupRootObjList();
					base.m_processingContext.GlobalRVCollection = aggregatesImpl;
					this.m_runtimeDataRegions.CalculateRunningValues(aggregatesImpl, groupCollection);
				}
			}

			protected override bool FirstPassGetNextDetailRow()
			{
				base.m_processingContext.CheckAndThrowIfAborted();
				bool result = false;
				if (base.m_processingContext.OWCChartName != null && 1000 < base.m_dataRowsRead)
				{
					return result;
				}
				result = base.GetNextDetailRow();
				if (this.m_dataChunkWriter != null)
				{
					if (1 == base.m_dataRowsRead && base.m_foundExtendedProperties)
					{
						this.m_dataChunkWriter.FieldAliasPropertyNames = base.m_referencedAliasPropertyNames;
					}
					if (result && this.m_dataChunkWriter != null && !this.m_dataChunkWriter.AddRecordRow(base.m_processingContext.ReportObjectModel.FieldsImpl, base.m_dataSet.NonCalculatedFieldCount))
					{
						base.m_processingContext.ErrorSavingSnapshotData = true;
						base.m_processingContext.DataCached = false;
						if (base.m_processingContext.StopSaveSnapshotDataOnError)
						{
							this.m_dataChunkSaved = false;
							this.m_dataChunkWriter = null;
						}
					}
				}
				return result;
			}

			private void NextAggregateRow()
			{
				if (base.m_processingContext.ReportObjectModel.FieldsImpl.AggregationFieldCount == 0 && this.m_customAggregates != null)
				{
					for (int i = 0; i < this.m_customAggregates.Count; i++)
					{
						this.m_customAggregates[i].Update();
					}
				}
				if (this.m_userSortTargetInfo != null && this.m_userSortTargetInfo.SortTree != null)
				{
					if (this.m_userSortTargetInfo.AggregateRows == null)
					{
						this.m_userSortTargetInfo.AggregateRows = new AggregateRowList();
					}
					AggregateRow value = new AggregateRow(base.m_processingContext);
					this.m_userSortTargetInfo.AggregateRows.Add(value);
					if (!this.m_userSortTargetInfo.TargetForNonDetailSort)
					{
						return;
					}
				}
				this.SendToInner();
			}

			protected override void NextNonAggregateRow()
			{
				if (this.m_nonCustomAggregates != null)
				{
					for (int i = 0; i < this.m_nonCustomAggregates.Count; i++)
					{
						this.m_nonCustomAggregates[i].Update();
					}
				}
				if (this.m_dataRows != null)
				{
					RuntimeDetailObj.SaveData(this.m_dataRows, base.m_processingContext);
				}
				this.SendToInner();
			}

			private void SendToInner()
			{
				this.m_runtimeDataRegions.FirstPassNextDataRow();
			}
		}

		private sealed class RuntimeSortDataHolder : ISortDataHolder
		{
			private IHierarchyObj m_owner;

			private DataRowList m_dataRows;

			internal RuntimeSortDataHolder(IHierarchyObj owner)
			{
				this.m_owner = owner;
				this.m_dataRows = new DataRowList();
			}

			void ISortDataHolder.NextRow()
			{
				FieldImpl[] andSaveFields = this.m_owner.ProcessingContext.ReportObjectModel.FieldsImpl.GetAndSaveFields();
				this.m_dataRows.Add(andSaveFields);
			}

			void ISortDataHolder.Traverse(ProcessingStages operation)
			{
				Global.Tracer.Assert(ProcessingStages.UserSortFilter == operation, "(ProcessingStages.UserSortFilter == operation)");
				if (this.m_dataRows != null)
				{
					for (int i = 0; i < this.m_dataRows.Count; i++)
					{
						FieldImpl[] fields = this.m_dataRows[i];
						this.m_owner.ProcessingContext.ReportObjectModel.FieldsImpl.SetFields(fields);
						this.m_owner.ReadRow();
					}
				}
			}
		}

		private sealed class RuntimePrefetchDataSetNode : RuntimeDataSetNode
		{
			private ChunkManager.DataChunkWriter m_dataChunkWriter;

			internal RuntimePrefetchDataSetNode(Report report, DataSet dataSet, ProcessingContext processingContext)
				: base(report, dataSet, processingContext)
			{
			}

			protected override void Process()
			{
				bool flag = false;
				if (base.m_dataSet.IsShareable() && base.m_processingContext.CachedDataChunkMapping.ContainsKey(base.m_dataSet.ID))
				{
					return;
				}
				try
				{
					base.FirstPassProcess(ref flag);
				}
				catch (Exception e)
				{
					if (AsynchronousExceptionDetection.IsStoppingException(e))
					{
						throw;
					}
					if (base.m_transInfo != null)
					{
						base.m_transInfo.RollbackRequired = true;
					}
					if (flag && base.m_dataSourceConnection != null)
					{
						base.m_processingContext.DataExtensionConnection.CloseConnectionWithoutPool(base.m_dataSourceConnection);
					}
					throw;
				}
				finally
				{
					if (base.m_dataReader != null)
					{
						((IDisposable)base.m_dataReader).Dispose();
						base.m_dataReader = null;
					}
				}
			}

			protected override void FirstPassInit()
			{
				base.InitRuntime(false);
				bool readerExtensionsSupported = this.RunDataSetQuery();
				if (base.m_processingContext.CreateReportChunkCallback == null && base.m_processingContext.CacheDataCallback == null)
				{
					return;
				}
				this.m_dataChunkWriter = new ChunkManager.DataChunkWriter(base.m_dataSet, base.m_processingContext, readerExtensionsSupported, false);
			}

			protected override bool FirstPassGetNextDetailRow()
			{
				base.m_processingContext.CheckAndThrowIfAborted();
				bool nextDetailRow = base.GetNextDetailRow();
				if (this.m_dataChunkWriter != null)
				{
					if (1 == base.m_dataRowsRead && base.m_foundExtendedProperties)
					{
						this.m_dataChunkWriter.FieldAliasPropertyNames = base.m_referencedAliasPropertyNames;
					}
					if (nextDetailRow)
					{
						this.m_dataChunkWriter.AddRecordRow(base.m_processingContext.ReportObjectModel.FieldsImpl, base.m_dataSet.NonCalculatedFieldCount);
					}
				}
				return nextDetailRow;
			}

			protected override void FirstPassProcessDetailRow(Filters filters)
			{
			}

			protected override void NextNonAggregateRow()
			{
			}

			protected override void FirstPassCleanup(bool flushData)
			{
				if (this.m_dataChunkWriter != null)
				{
					if (flushData)
					{
						this.m_dataChunkWriter.FinalFlush();
					}
					else
					{
						this.m_dataChunkWriter.Close();
					}
					this.m_dataChunkWriter = null;
				}
			}
		}

		private sealed class RuntimeReportParametersDataSetNode : RuntimeDataSetNode
		{
			private LegacyReportParameterDataSetCache m_reportParameterDataSetObj;

			internal RuntimeReportParametersDataSetNode(Report report, DataSet dataSet, ProcessingContext processingContext, LegacyReportParameterDataSetCache aCache)
				: base(report, dataSet, processingContext)
			{
				this.m_reportParameterDataSetObj = aCache;
			}

			protected override void FirstPassInit()
			{
				base.InitRuntime(false);
				this.RunDataSetQuery();
			}

			protected override void FirstPassProcessDetailRow(Filters filters)
			{
				bool flag = true;
				if (filters != null)
				{
					flag = filters.PassFilters(base.m_processingContext.ReportObjectModel.FieldsImpl.GetFields());
				}
				if (flag)
				{
					((IFilterOwner)this).PostFilterNextRow();
				}
			}

			protected override void Process()
			{
				bool flag = false;
				try
				{
					base.FirstPassProcess(ref flag);
				}
				catch (Exception e)
				{
					if (AsynchronousExceptionDetection.IsStoppingException(e))
					{
						throw;
					}
					if (base.m_transInfo != null)
					{
						base.m_transInfo.RollbackRequired = true;
					}
					if (flag && base.m_dataSourceConnection != null)
					{
						base.m_processingContext.DataExtensionConnection.CloseConnectionWithoutPool(base.m_dataSourceConnection);
					}
					throw;
				}
				finally
				{
					if (base.m_dataReader != null)
					{
						((IDisposable)base.m_dataReader).Dispose();
						base.m_dataReader = null;
					}
				}
			}

			protected override bool FirstPassGetNextDetailRow()
			{
				base.m_processingContext.CheckAndThrowIfAborted();
				return base.GetNextDetailRow();
			}

			protected override void NextNonAggregateRow()
			{
				this.m_reportParameterDataSetObj.NextRow(base.m_processingContext.ReportObjectModel.FieldsImpl.GetAndSaveFields());
			}
		}

		internal sealed class RuntimeReportParameterDataSetObj
		{
			private ProcessingContext m_processingContext;

			private DataRowList m_dataRows;

			internal int Count
			{
				get
				{
					if (this.m_dataRows == null)
					{
						return 0;
					}
					return this.m_dataRows.Count;
				}
			}

			internal RuntimeReportParameterDataSetObj(ProcessingContext processingContext)
			{
				this.m_processingContext = processingContext;
			}

			internal void NextRow()
			{
				if (this.m_dataRows == null)
				{
					this.m_dataRows = new DataRowList();
				}
				FieldImpl[] andSaveFields = this.m_processingContext.ReportObjectModel.FieldsImpl.GetAndSaveFields();
				this.m_dataRows.Add(andSaveFields);
			}

			internal object GetFieldValue(int row, int col)
			{
				if (this.Count == 0)
				{
					return null;
				}
				Global.Tracer.Assert(null != this.m_dataRows[row][col], "(null != m_dataRows[row][col])");
				if (this.m_dataRows[row][col].IsMissing)
				{
					return null;
				}
				this.m_processingContext.ReportObjectModel.FieldsImpl.SetFields(this.m_dataRows[row]);
				return this.m_dataRows[row][col].Value;
			}
		}

		internal sealed class ProcessingDataReader : IDisposable
		{
			private MappingDataReader m_dataSourceDataReader;

			private ChunkManager.DataChunkReader m_snapshotDataReader;

			internal bool ReaderExtensionsSupported
			{
				get
				{
					if (this.m_dataSourceDataReader != null)
					{
						return this.m_dataSourceDataReader.ReaderExtensionsSupported;
					}
					return this.m_snapshotDataReader.ReaderExtensionsSupported;
				}
			}

			internal bool ReaderFieldProperties
			{
				get
				{
					if (this.m_dataSourceDataReader != null)
					{
						return this.m_dataSourceDataReader.ReaderFieldProperties;
					}
					return this.m_snapshotDataReader.ReaderFieldProperties;
				}
			}

			public bool IsAggregateRow
			{
				get
				{
					if (this.m_dataSourceDataReader != null)
					{
						return this.m_dataSourceDataReader.IsAggregateRow;
					}
					return this.m_snapshotDataReader.IsAggregateRow;
				}
			}

			public int AggregationFieldCount
			{
				get
				{
					if (this.m_dataSourceDataReader != null)
					{
						return this.m_dataSourceDataReader.AggregationFieldCount;
					}
					return this.m_snapshotDataReader.AggregationFieldCount;
				}
			}

			internal ProcessingDataReader(string dataSetName, IDataReader sourceReader, string[] aliases, string[] names)
			{
				this.m_dataSourceDataReader = new MappingDataReader(dataSetName, sourceReader, aliases, names, null);
			}

			internal ProcessingDataReader(DataSet dataSet, ProcessingContext context)
			{
				this.m_snapshotDataReader = new ChunkManager.DataChunkReader(dataSet, context);
			}

			void IDisposable.Dispose()
			{
				if (this.m_dataSourceDataReader != null)
				{
					((IDisposable)this.m_dataSourceDataReader).Dispose();
				}
				else
				{
					((IDisposable)this.m_snapshotDataReader).Dispose();
				}
			}

			internal void OverrideDataCacheCompareOptions(ref ProcessingContext context)
			{
				if (this.m_snapshotDataReader != null && context.ProcessWithCachedData && this.m_snapshotDataReader.ValidCompareOptions)
				{
					context.ClrCompareOptions = this.m_snapshotDataReader.CompareOptions;
				}
			}

			public bool GetNextRow()
			{
				if (this.m_dataSourceDataReader != null)
				{
					return this.m_dataSourceDataReader.GetNextRow();
				}
				return this.m_snapshotDataReader.GetNextRow();
			}

			internal object GetColumn(int aliasIndex)
			{
				object obj = null;
				obj = ((this.m_dataSourceDataReader == null) ? this.m_snapshotDataReader.GetFieldValue(aliasIndex) : this.m_dataSourceDataReader.GetFieldValue(aliasIndex));
				if (obj is DBNull)
				{
					return null;
				}
				return obj;
			}

			internal bool IsAggregationField(int aliasIndex)
			{
				if (this.m_dataSourceDataReader != null)
				{
					return this.m_dataSourceDataReader.IsAggregationField(aliasIndex);
				}
				return this.m_snapshotDataReader.IsAggregationField(aliasIndex);
			}

			internal int GetPropertyCount(int aliasIndex)
			{
				if (this.m_dataSourceDataReader != null)
				{
					return this.m_dataSourceDataReader.GetPropertyCount(aliasIndex);
				}
				if (this.m_snapshotDataReader != null && this.m_snapshotDataReader.FieldPropertyNames != null && this.m_snapshotDataReader.FieldPropertyNames[aliasIndex] != null)
				{
					StringList propertyNames = this.m_snapshotDataReader.FieldPropertyNames.GetPropertyNames(aliasIndex);
					if (propertyNames != null)
					{
						return propertyNames.Count;
					}
				}
				return 0;
			}

			internal string GetPropertyName(int aliasIndex, int propertyIndex)
			{
				if (this.m_dataSourceDataReader != null)
				{
					return this.m_dataSourceDataReader.GetPropertyName(aliasIndex, propertyIndex);
				}
				if (this.m_snapshotDataReader != null && this.m_snapshotDataReader.FieldPropertyNames != null)
				{
					return this.m_snapshotDataReader.FieldPropertyNames.GetPropertyName(aliasIndex, propertyIndex);
				}
				return null;
			}

			internal object GetPropertyValue(int aliasIndex, int propertyIndex)
			{
				object obj = null;
				if (this.m_dataSourceDataReader != null)
				{
					obj = this.m_dataSourceDataReader.GetPropertyValue(aliasIndex, propertyIndex);
				}
				else if (this.m_snapshotDataReader != null)
				{
					obj = this.m_snapshotDataReader.GetPropertyValue(aliasIndex, propertyIndex);
				}
				if (obj is DBNull)
				{
					return null;
				}
				return obj;
			}
		}

		internal sealed class PageMergeInteractive
		{
			private PageTextboxes m_pageTextboxes;

			private ReportSnapshot m_reportSnapshot;

			private Report m_report;

			private ReportInstance m_reportInstance;

			private ParameterInfoCollection m_parameters;

			private ProcessingContext m_processingContext;

			private AggregatesImpl m_aggregates;

			private Hashtable m_aggregatesOverReportItems;

			internal UserProfileState Process(PageTextboxes pageTextboxes, ReportSnapshot reportSnapshot, ICatalogItemContext reportContext, string reportName, ParameterInfoCollection parameters, ChunkManager.ProcessingChunkManager pageSectionManager, CreateReportChunk createChunkCallback, IGetResource getResourceCallback, ErrorContext errorContext, UserProfileState allowUserProfileState, ReportRuntimeSetup reportRuntimeSetup, int uniqueNameCounter, IDataProtection dataProtection, ref ReportDrillthroughInfo drillthroughInfo)
			{
				UserProfileState result = UserProfileState.None;
				try
				{
					this.m_pageTextboxes = pageTextboxes;
					this.m_reportSnapshot = reportSnapshot;
					this.m_report = reportSnapshot.Report;
					this.m_reportInstance = reportSnapshot.ReportInstance;
					this.m_parameters = parameters;
					this.m_processingContext = new ProcessingContext(reportContext, this.m_report.ShowHideType, getResourceCallback, this.m_report.EmbeddedImages, this.m_report.ImageStreamNames, errorContext, !this.m_report.PageMergeOnePass, allowUserProfileState, reportRuntimeSetup, createChunkCallback, pageSectionManager, uniqueNameCounter, dataProtection, ref drillthroughInfo);
					if (this.m_report.Language != null)
					{
						string text = null;
						text = ((this.m_report.Language.Type != ExpressionInfo.Types.Constant) ? this.m_reportInstance.Language : this.m_report.Language.Value);
						if (text != null)
						{
							try
							{
								CultureInfo cultureInfo = new CultureInfo(text, false);
								if (cultureInfo.IsNeutralCulture)
								{
									cultureInfo = CultureInfo.CreateSpecificCulture(text);
									cultureInfo = new CultureInfo(cultureInfo.Name, false);
								}
								Thread.CurrentThread.CurrentCulture = cultureInfo;
							}
							catch (Exception)
							{
								Thread.CurrentThread.CurrentCulture = Localization.DefaultReportServerSpecificCulture;
							}
						}
						else
						{
							Thread.CurrentThread.CurrentCulture = Localization.DefaultReportServerSpecificCulture;
						}
					}
					else
					{
						Thread.CurrentThread.CurrentCulture = Localization.DefaultReportServerSpecificCulture;
					}
					this.GlobalInit(reportName, this.m_reportInstance.NumberOfPages);
					this.m_reportSnapshot.PageSections = new List<PageSectionInstance>(2 * this.m_reportInstance.NumberOfPages);
					for (int i = 0; i < this.m_reportInstance.NumberOfPages; i++)
					{
						this.PageInit(i);
						if (!this.m_report.PageMergeOnePass)
						{
							this.FirstPass(i);
						}
						this.SecondPass(i);
					}
					return result;
				}
				finally
				{
					if (this.m_processingContext != null)
					{
						if (this.m_processingContext.ReportRuntime != null)
						{
							this.m_processingContext.ReportRuntime.Close();
						}
						result = this.m_processingContext.HasUserProfileState;
					}
					this.m_report = null;
					this.m_reportInstance = null;
					this.m_processingContext = null;
				}
			}

			private void GlobalInit(string reportName, int totalPages)
			{
				this.m_processingContext.ReportObjectModel = new ObjectModelImpl(this.m_processingContext);
				Global.Tracer.Assert(this.m_processingContext.ReportRuntime == null, "(m_processingContext.ReportRuntime == null)");
				this.m_processingContext.ReportRuntime = new ReportRuntime(this.m_processingContext.ReportObjectModel, this.m_processingContext.ErrorContext);
				this.m_processingContext.ReportObjectModel.FieldsImpl = new FieldsImpl();
				this.m_processingContext.ReportObjectModel.ParametersImpl = new ParametersImpl(this.m_parameters.Count);
				this.m_processingContext.ReportObjectModel.GlobalsImpl = new GlobalsImpl(reportName, 0, totalPages, this.m_reportSnapshot.ExecutionTime, this.m_reportSnapshot.ReportServerUrl, this.m_reportSnapshot.ReportFolder);
				this.m_processingContext.ReportObjectModel.UserImpl = new UserImpl(this.m_reportSnapshot.RequestUserName, this.m_reportSnapshot.Language, this.m_processingContext.AllowUserProfileState);
				this.m_processingContext.ReportObjectModel.ReportItemsImpl = new ReportItemsImpl();
				this.m_processingContext.ReportObjectModel.AggregatesImpl = new AggregatesImpl(this.m_processingContext.ReportRuntime);
				this.m_processingContext.ReportObjectModel.DataSetsImpl = new DataSetsImpl();
				this.m_processingContext.ReportObjectModel.DataSourcesImpl = new DataSourcesImpl(this.m_report.DataSourceCount);
				for (int i = 0; i < this.m_parameters.Count; i++)
				{
					this.m_processingContext.ReportObjectModel.ParametersImpl.Add(this.m_parameters[i].Name, new ParameterImpl(this.m_parameters[i].Values, this.m_parameters[i].Labels, this.m_parameters[i].MultiValue));
				}
				this.m_processingContext.ReportRuntime.LoadCompiledCode(this.m_report, false, this.m_processingContext.ReportObjectModel, this.m_processingContext.ReportRuntimeSetup);
			}

			private void PageInit(int currentPageNumber)
			{
				this.m_processingContext.ReportObjectModel.GlobalsImpl.SetPageNumber(currentPageNumber + 1);
				this.m_processingContext.ReportObjectModel.ReportItemsImpl = new ReportItemsImpl();
				this.m_processingContext.ReportObjectModel.AggregatesImpl = new AggregatesImpl(this.m_processingContext.ReportRuntime);
				if (this.m_processingContext.ReportRuntime.ReportExprHost != null)
				{
					this.m_processingContext.RuntimeInitializeReportItemObjs(this.m_report.ReportItems, true, true);
					if (this.m_report.PageHeader != null)
					{
						if (this.m_processingContext.ReportRuntime.ReportExprHost != null)
						{
							this.m_report.PageHeader.SetExprHost(this.m_processingContext.ReportRuntime.ReportExprHost, this.m_processingContext.ReportObjectModel);
						}
						this.m_processingContext.RuntimeInitializeReportItemObjs(this.m_report.PageHeader.ReportItems, false, false);
					}
					if (this.m_report.PageFooter != null)
					{
						if (this.m_processingContext.ReportRuntime.ReportExprHost != null)
						{
							this.m_report.PageFooter.SetExprHost(this.m_processingContext.ReportRuntime.ReportExprHost, this.m_processingContext.ReportObjectModel);
						}
						this.m_processingContext.RuntimeInitializeReportItemObjs(this.m_report.PageFooter.ReportItems, false, false);
					}
				}
				this.m_aggregates = new AggregatesImpl(this.m_processingContext.ReportRuntime);
				this.m_aggregatesOverReportItems = new Hashtable();
				this.m_processingContext.ReportObjectModel.ReportItemsImpl.SpecialMode = true;
				if (this.m_report.PageAggregates != null)
				{
					for (int i = 0; i < this.m_report.PageAggregates.Count; i++)
					{
						DataAggregateInfo dataAggregateInfo = this.m_report.PageAggregates[i];
						dataAggregateInfo.ExprHostInitialized = false;
						DataAggregateObj dataAggregateObj = new DataAggregateObj(dataAggregateInfo, this.m_processingContext);
						object[] array = default(object[]);
						DataFieldStatus dataFieldStatus = default(DataFieldStatus);
						dataAggregateObj.EvaluateParameters(out array, out dataFieldStatus);
						string specialModeIndex = this.m_processingContext.ReportObjectModel.ReportItemsImpl.GetSpecialModeIndex();
						if (specialModeIndex == null)
						{
							this.m_aggregates.Add(dataAggregateObj);
						}
						else
						{
							AggregatesImpl aggregatesImpl = (AggregatesImpl)this.m_aggregatesOverReportItems[specialModeIndex];
							if (aggregatesImpl == null)
							{
								aggregatesImpl = new AggregatesImpl(this.m_processingContext.ReportRuntime);
								this.m_aggregatesOverReportItems.Add(specialModeIndex, aggregatesImpl);
							}
							aggregatesImpl.Add(dataAggregateObj);
						}
						dataAggregateObj.Init();
					}
				}
				this.m_processingContext.ReportObjectModel.ReportItemsImpl.SpecialMode = false;
			}

			private void FirstPass(int currentPageNumber)
			{
				Hashtable hashtable = null;
				if (this.m_pageTextboxes != null)
				{
					hashtable = this.m_pageTextboxes.GetTextboxesOnPage(currentPageNumber);
				}
				try
				{
					foreach (DataAggregateObj @object in this.m_aggregates.Objects)
					{
						this.m_processingContext.ReportObjectModel.AggregatesImpl.Add(@object);
					}
					if (hashtable != null)
					{
						IEnumerator enumerator2 = hashtable.Keys.GetEnumerator();
						while (enumerator2.MoveNext())
						{
							string text = enumerator2.Current as string;
							Global.Tracer.Assert(null != text, "(null != name)");
							ArrayList arrayList = hashtable[text] as ArrayList;
							Global.Tracer.Assert(arrayList != null && 0 < arrayList.Count, "(null != values && 0 < values.Count)");
							AggregatesImpl aggregatesImpl = (AggregatesImpl)this.m_aggregatesOverReportItems[text];
							TextBoxImpl textBoxImpl = (TextBoxImpl)((ReportItems)this.m_processingContext.ReportObjectModel.ReportItemsImpl)[text];
							if (aggregatesImpl != null)
							{
								Global.Tracer.Assert(null != textBoxImpl, "(null != textBoxObj)");
								for (int i = 0; i < arrayList.Count; i++)
								{
									textBoxImpl.SetResult(new VariantResult(false, arrayList[i]));
									foreach (DataAggregateObj object2 in aggregatesImpl.Objects)
									{
										object2.Update();
									}
								}
							}
							else
							{
								textBoxImpl.SetResult(new VariantResult(false, arrayList[arrayList.Count - 1]));
							}
						}
					}
					foreach (AggregatesImpl value in this.m_aggregatesOverReportItems.Values)
					{
						foreach (DataAggregateObj object3 in value.Objects)
						{
							this.m_processingContext.ReportObjectModel.AggregatesImpl.Add(object3);
						}
					}
				}
				finally
				{
					this.m_aggregates = null;
					this.m_aggregatesOverReportItems = null;
				}
			}

			private void SecondPass(int currentPageNumber)
			{
				PageSectionInstance pageSectionInstance = null;
				PageSectionInstance pageSectionInstance2 = null;
				if (this.m_report.PageHeaderEvaluation)
				{
					pageSectionInstance = new PageSectionInstance(this.m_processingContext, currentPageNumber, this.m_report.PageHeader);
					this.CreateInstances(pageSectionInstance.ReportItemColInstance, this.m_report.PageHeader.ReportItems);
				}
				if (this.m_report.PageFooterEvaluation)
				{
					pageSectionInstance2 = new PageSectionInstance(this.m_processingContext, currentPageNumber, this.m_report.PageFooter);
					this.CreateInstances(pageSectionInstance2.ReportItemColInstance, this.m_report.PageFooter.ReportItems);
				}
				this.m_reportSnapshot.PageSections.Add(pageSectionInstance);
				this.m_reportSnapshot.PageSections.Add(pageSectionInstance2);
			}

			private void CreateInstances(ReportItemColInstance collectionInstance, ReportItemCollection reportItemsDef)
			{
				reportItemsDef.ProcessDrillthroughAction(this.m_processingContext, collectionInstance.ChildrenNonComputedUniqueNames);
				if (reportItemsDef.ComputedReportItems != null)
				{
					ReportItemInstance reportItemInstance = null;
					for (int i = 0; i < reportItemsDef.ComputedReportItems.Count; i++)
					{
						ReportItem reportItem = reportItemsDef.ComputedReportItems[i];
						if (reportItem is TextBox)
						{
							reportItemInstance = RuntimeRICollection.CreateTextBoxInstance((TextBox)reportItem, this.m_processingContext, i, null);
						}
						else if (reportItem is Line)
						{
							reportItemInstance = RuntimeRICollection.CreateLineInstance((Line)reportItem, this.m_processingContext, i);
						}
						else if (reportItem is Image)
						{
							reportItemInstance = RuntimeRICollection.CreateImageInstance((Image)reportItem, this.m_processingContext, i);
						}
						else if (reportItem is Rectangle)
						{
							Rectangle rectangle = (Rectangle)reportItem;
							RectangleInstance rectangleInstance = new RectangleInstance(this.m_processingContext, rectangle, i);
							this.CreateInstances(rectangleInstance.ReportItemColInstance, rectangle.ReportItems);
							reportItemInstance = rectangleInstance;
						}
						if (reportItemInstance != null)
						{
							collectionInstance.Add(reportItemInstance);
						}
					}
				}
			}
		}

		internal sealed class PageMerge
		{
			private int m_pageNumber;

			private AspNetCore.ReportingServices.ReportRendering.Report m_renderingReport;

			private ReportSnapshot m_reportSnapshot;

			private Report m_report;

			private ReportInstance m_reportInstance;

			private PageReportItems m_pageReportItems;

			private ProcessingContext m_processingContext;

			private AspNetCore.ReportingServices.ReportRendering.PageSection m_pageHeader;

			private AspNetCore.ReportingServices.ReportRendering.PageSection m_pageFooter;

			private AggregatesImpl m_aggregates;

			private Hashtable m_aggregatesOverReportItems;

			internal void Process(int pageNumber, int totalPages, AspNetCore.ReportingServices.ReportRendering.Report report, PageReportItems pageReportItems, ErrorContext errorContext, out AspNetCore.ReportingServices.ReportRendering.PageSection pageHeader, out AspNetCore.ReportingServices.ReportRendering.PageSection pageFooter)
			{
				if (!report.NeedsHeaderFooterEvaluation)
				{
					pageHeader = null;
					pageFooter = null;
				}
				else
				{
					try
					{
						this.m_pageNumber = pageNumber;
						this.m_renderingReport = report;
						this.m_reportSnapshot = report.RenderingContext.ReportSnapshot;
						this.m_report = report.ReportDef;
						this.m_reportInstance = report.ReportInstance;
						this.m_pageReportItems = pageReportItems;
						this.m_processingContext = new ProcessingContext(report.RenderingContext.TopLevelReportContext, this.m_report.ShowHideType, report.RenderingContext.GetResourceCallback, this.m_report.EmbeddedImages, this.m_report.ImageStreamNames, errorContext, !this.m_report.PageMergeOnePass, report.RenderingContext.AllowUserProfileState, report.RenderingContext.ReportRuntimeSetup, report.RenderingContext.DataProtection);
						if (this.m_report.Language != null)
						{
							string text = null;
							text = ((this.m_report.Language.Type != ExpressionInfo.Types.Constant) ? this.m_reportInstance.Language : this.m_report.Language.Value);
							if (text != null)
							{
								try
								{
									CultureInfo cultureInfo = new CultureInfo(text, false);
									if (cultureInfo.IsNeutralCulture)
									{
										cultureInfo = CultureInfo.CreateSpecificCulture(text);
										cultureInfo = new CultureInfo(cultureInfo.Name, false);
									}
									Thread.CurrentThread.CurrentCulture = cultureInfo;
								}
								catch (Exception)
								{
									Thread.CurrentThread.CurrentCulture = Localization.DefaultReportServerSpecificCulture;
								}
							}
							else
							{
								Thread.CurrentThread.CurrentCulture = Localization.DefaultReportServerSpecificCulture;
							}
						}
						else
						{
							Thread.CurrentThread.CurrentCulture = Localization.DefaultReportServerSpecificCulture;
						}
						this.FirstPassInit(totalPages);
						this.FirstPass();
						this.SecondPass();
						pageHeader = this.m_pageHeader;
						pageFooter = this.m_pageFooter;
					}
					finally
					{
						if (this.m_processingContext != null)
						{
							if (this.m_processingContext.ReportRuntime != null)
							{
								this.m_processingContext.ReportRuntime.Close();
							}
							report.RenderingContext.UsedUserProfileState = this.m_processingContext.HasUserProfileState;
						}
						this.m_renderingReport = null;
						this.m_report = null;
						this.m_reportInstance = null;
						this.m_pageReportItems = null;
						this.m_processingContext = null;
						this.m_pageHeader = null;
						this.m_pageFooter = null;
					}
				}
			}

			private void FirstPassInit(int totalPages)
			{
				ReportInstanceInfo reportInstanceInfo = (ReportInstanceInfo)this.m_reportInstance.GetInstanceInfo(this.m_renderingReport.RenderingContext.ChunkManager);
				this.m_processingContext.ReportObjectModel = new ObjectModelImpl(this.m_processingContext);
				Global.Tracer.Assert(this.m_processingContext.ReportRuntime == null, "(m_processingContext.ReportRuntime == null)");
				this.m_processingContext.ReportRuntime = new ReportRuntime(this.m_processingContext.ReportObjectModel, this.m_processingContext.ErrorContext);
				this.m_processingContext.ReportObjectModel.FieldsImpl = new FieldsImpl();
				this.m_processingContext.ReportObjectModel.ParametersImpl = new ParametersImpl(reportInstanceInfo.Parameters.Count);
				this.m_processingContext.ReportObjectModel.GlobalsImpl = new GlobalsImpl(reportInstanceInfo.ReportName, this.m_pageNumber, totalPages, this.m_reportSnapshot.ExecutionTime, this.m_reportSnapshot.ReportServerUrl, this.m_reportSnapshot.ReportFolder);
				this.m_processingContext.ReportObjectModel.UserImpl = new UserImpl(this.m_reportSnapshot.RequestUserName, this.m_reportSnapshot.Language, this.m_processingContext.AllowUserProfileState);
				this.m_processingContext.ReportObjectModel.ReportItemsImpl = new ReportItemsImpl();
				this.m_processingContext.ReportObjectModel.AggregatesImpl = new AggregatesImpl(this.m_processingContext.ReportRuntime);
				this.m_processingContext.ReportObjectModel.DataSetsImpl = new DataSetsImpl();
				this.m_processingContext.ReportObjectModel.DataSourcesImpl = new DataSourcesImpl(this.m_report.DataSourceCount);
				for (int i = 0; i < reportInstanceInfo.Parameters.Count; i++)
				{
					this.m_processingContext.ReportObjectModel.ParametersImpl.Add(reportInstanceInfo.Parameters[i].Name, new ParameterImpl(reportInstanceInfo.Parameters[i].Values, reportInstanceInfo.Parameters[i].Labels, reportInstanceInfo.Parameters[i].MultiValue));
				}
				this.m_processingContext.ReportRuntime.LoadCompiledCode(this.m_report, false, this.m_processingContext.ReportObjectModel, this.m_processingContext.ReportRuntimeSetup);
				if (this.m_processingContext.ReportRuntime.ReportExprHost != null)
				{
					this.m_processingContext.RuntimeInitializeReportItemObjs(this.m_report.ReportItems, true, true);
					if (this.m_report.PageHeader != null)
					{
						if (this.m_processingContext.ReportRuntime.ReportExprHost != null)
						{
							this.m_report.PageHeader.SetExprHost(this.m_processingContext.ReportRuntime.ReportExprHost, this.m_processingContext.ReportObjectModel);
						}
						this.m_processingContext.RuntimeInitializeReportItemObjs(this.m_report.PageHeader.ReportItems, false, false);
					}
					if (this.m_report.PageFooter != null)
					{
						if (this.m_processingContext.ReportRuntime.ReportExprHost != null)
						{
							this.m_report.PageFooter.SetExprHost(this.m_processingContext.ReportRuntime.ReportExprHost, this.m_processingContext.ReportObjectModel);
						}
						this.m_processingContext.RuntimeInitializeReportItemObjs(this.m_report.PageFooter.ReportItems, false, false);
					}
				}
				this.m_aggregates = new AggregatesImpl(this.m_processingContext.ReportRuntime);
				this.m_aggregatesOverReportItems = new Hashtable();
				this.m_processingContext.ReportObjectModel.ReportItemsImpl.SpecialMode = true;
				if (this.m_report.PageAggregates != null)
				{
					for (int j = 0; j < this.m_report.PageAggregates.Count; j++)
					{
						DataAggregateInfo dataAggregateInfo = this.m_report.PageAggregates[j];
						dataAggregateInfo.ExprHostInitialized = false;
						DataAggregateObj dataAggregateObj = new DataAggregateObj(dataAggregateInfo, this.m_processingContext);
						object[] array = default(object[]);
						DataFieldStatus dataFieldStatus = default(DataFieldStatus);
						dataAggregateObj.EvaluateParameters(out array, out dataFieldStatus);
						string specialModeIndex = this.m_processingContext.ReportObjectModel.ReportItemsImpl.GetSpecialModeIndex();
						if (specialModeIndex == null)
						{
							this.m_aggregates.Add(dataAggregateObj);
						}
						else
						{
							AggregatesImpl aggregatesImpl = (AggregatesImpl)this.m_aggregatesOverReportItems[specialModeIndex];
							if (aggregatesImpl == null)
							{
								aggregatesImpl = new AggregatesImpl(this.m_processingContext.ReportRuntime);
								this.m_aggregatesOverReportItems.Add(specialModeIndex, aggregatesImpl);
							}
							aggregatesImpl.Add(dataAggregateObj);
						}
						dataAggregateObj.Init();
					}
				}
				this.m_processingContext.ReportObjectModel.ReportItemsImpl.SpecialMode = false;
			}

			private void FirstPass()
			{
				try
				{
					if (!this.m_report.PageMergeOnePass)
					{
						foreach (DataAggregateObj @object in this.m_aggregates.Objects)
						{
							this.m_processingContext.ReportObjectModel.AggregatesImpl.Add(@object);
						}
						for (int i = 0; i < this.m_pageReportItems.Count; i++)
						{
							this.FirstPassReportItem(this.m_pageReportItems[i]);
						}
						foreach (AggregatesImpl value in this.m_aggregatesOverReportItems.Values)
						{
							foreach (DataAggregateObj object2 in value.Objects)
							{
								this.m_processingContext.ReportObjectModel.AggregatesImpl.Add(object2);
							}
						}
					}
				}
				finally
				{
					this.m_aggregates = null;
					this.m_aggregatesOverReportItems = null;
				}
			}

			private void FirstPassReportItems(AspNetCore.ReportingServices.ReportRendering.ReportItemCollection reportItems)
			{
				if (reportItems != null)
				{
					for (int i = 0; i < reportItems.Count; i++)
					{
						this.FirstPassReportItem(reportItems[i]);
					}
				}
			}

			private void FirstPassReportItem(AspNetCore.ReportingServices.ReportRendering.ReportItem reportItem)
			{
				if (reportItem != null && Visibility.IsVisible(reportItem.SharedHidden, reportItem.Hidden, reportItem.HasToggle) && this.m_processingContext.PageSectionContext.IsParentVisible())
				{
					if (reportItem is AspNetCore.ReportingServices.ReportRendering.TextBox)
					{
						TextBoxImpl textBoxImpl = (TextBoxImpl)((ReportItems)this.m_processingContext.ReportObjectModel.ReportItemsImpl)[reportItem.Name];
						Global.Tracer.Assert(null != textBoxImpl, "(null != textBoxObj)");
						textBoxImpl.SetResult(new VariantResult(false, ((AspNetCore.ReportingServices.ReportRendering.TextBox)reportItem).OriginalValue));
					}
					else if (reportItem is AspNetCore.ReportingServices.ReportRendering.Rectangle)
					{
						this.FirstPassReportItems(((AspNetCore.ReportingServices.ReportRendering.Rectangle)reportItem).ReportItemCollection);
					}
					else if (reportItem is AspNetCore.ReportingServices.ReportRendering.List)
					{
						AspNetCore.ReportingServices.ReportRendering.List list = (AspNetCore.ReportingServices.ReportRendering.List)reportItem;
						if (list.Contents != null)
						{
							for (int i = 0; i < list.Contents.Count; i++)
							{
								ListContent listContent = list.Contents[i];
								if (listContent != null && Visibility.IsVisible(listContent.SharedHidden, listContent.Hidden, listContent.HasToggle))
								{
									this.FirstPassReportItems(listContent.ReportItemCollection);
								}
							}
						}
					}
					else if (reportItem is AspNetCore.ReportingServices.ReportRendering.Table)
					{
						AspNetCore.ReportingServices.ReportRendering.Table table = (AspNetCore.ReportingServices.ReportRendering.Table)reportItem;
						bool[] array = new bool[table.Columns.Count];
						for (int j = 0; j < array.Length; j++)
						{
							array[j] = Visibility.IsVisible(table.Columns[j].SharedHidden, table.Columns[j].Hidden, table.Columns[j].HasToggle);
						}
						this.FirstPassTableGroups(array, table.TableHeader, table.DetailRows, table.TableGroups, table.TableFooter);
					}
					else if (reportItem is AspNetCore.ReportingServices.ReportRendering.Matrix)
					{
						AspNetCore.ReportingServices.ReportRendering.Matrix matrix = (AspNetCore.ReportingServices.ReportRendering.Matrix)reportItem;
						this.FirstPassReportItem(matrix.Corner);
						bool[] array2 = new bool[matrix.CellRows];
						bool[] array3 = new bool[matrix.CellColumns];
						this.FirstPassMatrixHeadings(matrix.ColumnMemberCollection, true, ref array3);
						this.FirstPassMatrixHeadings(matrix.RowMemberCollection, false, ref array2);
						if (matrix.CellCollection != null)
						{
							for (int k = 0; k < matrix.CellRows; k++)
							{
								if (array2[k])
								{
									for (int l = 0; l < matrix.CellColumns; l++)
									{
										if (array3[l])
										{
											this.FirstPassReportItem(matrix.CellCollection[k, l].ReportItem);
										}
									}
								}
							}
						}
					}
					AggregatesImpl aggregatesImpl = null;
					if (reportItem.Name != null)
					{
						aggregatesImpl = (AggregatesImpl)this.m_aggregatesOverReportItems[reportItem.Name];
					}
					if (aggregatesImpl != null)
					{
						foreach (DataAggregateObj @object in aggregatesImpl.Objects)
						{
							@object.Update();
						}
					}
				}
			}

			private void FirstPassTableRow(bool[] tableColumnsVisible, AspNetCore.ReportingServices.ReportRendering.TableRow row)
			{
				if (row != null && row.TableCellCollection != null && Visibility.IsVisible(row.SharedHidden, row.Hidden, row.HasToggle))
				{
					int count = row.TableCellCollection.Count;
					Global.Tracer.Assert(count <= tableColumnsVisible.Length, "(cellCount <= tableColumnsVisible.Length)");
					int num = 0;
					for (int i = 0; i < count; i++)
					{
						int colSpan = row.TableCellCollection[i].ColSpan;
						if (Visibility.IsTableCellVisible(tableColumnsVisible, num, colSpan))
						{
							this.FirstPassReportItem(row.TableCellCollection[i].ReportItem);
						}
						num += colSpan;
					}
				}
			}

			private void FirstPassTableGroups(bool[] tableColumnsVisible, TableHeaderFooterRows header, TableRowsCollection detailRows, TableGroupCollection subGroups, TableHeaderFooterRows footer)
			{
				if (header != null)
				{
					for (int i = 0; i < header.Count; i++)
					{
						this.FirstPassTableRow(tableColumnsVisible, ((TableRowCollection)header)[i]);
					}
				}
				Global.Tracer.Assert(detailRows == null || null == subGroups);
				if (detailRows != null)
				{
					for (int j = 0; j < detailRows.Count; j++)
					{
						if (detailRows[j] != null)
						{
							for (int k = 0; k < detailRows[j].Count; k++)
							{
								this.FirstPassTableRow(tableColumnsVisible, ((TableRowCollection)detailRows[j])[k]);
							}
						}
					}
				}
				if (subGroups != null)
				{
					for (int l = 0; l < subGroups.Count; l++)
					{
						if (subGroups[l] != null)
						{
							this.FirstPassTableGroups(tableColumnsVisible, subGroups[l].GroupHeader, subGroups[l].DetailRows, subGroups[l].SubGroups, subGroups[l].GroupFooter);
						}
					}
				}
				if (footer != null)
				{
					for (int m = 0; m < footer.Count; m++)
					{
						this.FirstPassTableRow(tableColumnsVisible, ((TableRowCollection)footer)[m]);
					}
				}
			}

			private void FirstPassMatrixHeadings(MatrixMemberCollection headings, bool isColumn, ref bool[] cellsCanGetReferenced)
			{
				if (headings != null)
				{
					for (int i = 0; i < headings.Count; i++)
					{
						MatrixMember matrixMember = headings[i];
						if (matrixMember != null)
						{
							this.m_processingContext.PageSectionContext.EnterMatrixHeadingScope(Visibility.IsVisible(matrixMember.SharedHidden, matrixMember.Hidden, matrixMember.HasToggle), isColumn);
							this.FirstPassReportItem(matrixMember.ReportItem);
							if (matrixMember.IsTotal)
							{
								this.m_processingContext.PageSectionContext.EnterMatrixSubtotalScope(isColumn);
							}
							int num = isColumn ? matrixMember.ColumnSpan : matrixMember.RowSpan;
							Global.Tracer.Assert(cellsCanGetReferenced != null && matrixMember.MemberCellIndex >= 0 && num > 0 && matrixMember.MemberCellIndex + num <= cellsCanGetReferenced.Length);
							for (int j = 0; j < num; j++)
							{
								cellsCanGetReferenced[matrixMember.MemberCellIndex + j] = this.m_processingContext.PageSectionContext.IsParentVisible();
							}
							this.FirstPassMatrixHeadings(matrixMember.Children, isColumn, ref cellsCanGetReferenced);
							if (matrixMember.IsTotal)
							{
								this.m_processingContext.PageSectionContext.ExitMatrixHeadingScope(isColumn);
							}
							this.m_processingContext.PageSectionContext.ExitMatrixHeadingScope(isColumn);
						}
					}
				}
			}

			private void SecondPass()
			{
				if (!this.m_report.PageHeaderEvaluation)
				{
					this.m_pageHeader = null;
				}
				else
				{
					PageSectionInstance pageSectionInstance = new PageSectionInstance(this.m_processingContext, this.m_pageNumber, this.m_report.PageHeader);
					PageMerge.CreateInstances(this.m_processingContext, pageSectionInstance.ReportItemColInstance, this.m_report.PageHeader.ReportItems);
					string text = this.m_pageNumber.ToString(CultureInfo.InvariantCulture) + "ph";
					AspNetCore.ReportingServices.ReportRendering.RenderingContext renderingContext = new AspNetCore.ReportingServices.ReportRendering.RenderingContext(this.m_renderingReport.RenderingContext, text);
					this.m_pageHeader = new AspNetCore.ReportingServices.ReportRendering.PageSection(text, this.m_report.PageHeader, pageSectionInstance, this.m_renderingReport, renderingContext, false);
				}
				if (!this.m_report.PageFooterEvaluation)
				{
					this.m_pageFooter = null;
				}
				else
				{
					PageSectionInstance pageSectionInstance2 = new PageSectionInstance(this.m_processingContext, this.m_pageNumber, this.m_report.PageFooter);
					PageMerge.CreateInstances(this.m_processingContext, pageSectionInstance2.ReportItemColInstance, this.m_report.PageFooter.ReportItems);
					string text2 = this.m_pageNumber.ToString(CultureInfo.InvariantCulture) + "pf";
					AspNetCore.ReportingServices.ReportRendering.RenderingContext renderingContext2 = new AspNetCore.ReportingServices.ReportRendering.RenderingContext(this.m_renderingReport.RenderingContext, text2);
					this.m_pageFooter = new AspNetCore.ReportingServices.ReportRendering.PageSection(text2, this.m_report.PageFooter, pageSectionInstance2, this.m_renderingReport, renderingContext2, false);
				}
			}

			internal static void CreateInstances(ProcessingContext processingContext, ReportItemColInstance collectionInstance, ReportItemCollection reportItemsDef)
			{
				if (reportItemsDef.ComputedReportItems != null)
				{
					ReportItemInstance reportItemInstance = null;
					for (int i = 0; i < reportItemsDef.ComputedReportItems.Count; i++)
					{
						ReportItem reportItem = reportItemsDef.ComputedReportItems[i];
						if (reportItem is TextBox)
						{
							reportItemInstance = RuntimeRICollection.CreateTextBoxInstance((TextBox)reportItem, processingContext, i, null);
						}
						else if (reportItem is Line)
						{
							reportItemInstance = RuntimeRICollection.CreateLineInstance((Line)reportItem, processingContext, i);
						}
						else if (reportItem is Image)
						{
							reportItemInstance = RuntimeRICollection.CreateImageInstance((Image)reportItem, processingContext, i);
						}
						else if (reportItem is ActiveXControl)
						{
							reportItemInstance = RuntimeRICollection.CreateActiveXControlInstance((ActiveXControl)reportItem, processingContext, i);
						}
						else if (reportItem is Rectangle)
						{
							Rectangle rectangle = (Rectangle)reportItem;
							RectangleInstance rectangleInstance = new RectangleInstance(processingContext, rectangle, i);
							PageMerge.CreateInstances(processingContext, rectangleInstance.ReportItemColInstance, rectangle.ReportItems);
							reportItemInstance = rectangleInstance;
						}
						if (reportItemInstance != null)
						{
							collectionInstance.Add(reportItemInstance);
						}
					}
				}
			}
		}

		public delegate bool CheckSharedDataSet(string dataSetPath, out Guid catalogItemId);

		public delegate void ResolveTemporaryDataSet(DataSetInfo dataSetInfo, DataSetInfoCollection originalDataSets);

		internal sealed class CustomReportItemControls
		{
			private class CustomControlInfo
			{
				private bool m_valid;

				private AspNetCore.ReportingServices.ReportRendering.ICustomReportItem m_instance;

				internal bool IsValid
				{
					get
					{
						return this.m_valid;
					}
					set
					{
						this.m_valid = value;
					}
				}

				internal AspNetCore.ReportingServices.ReportRendering.ICustomReportItem Instance
				{
					get
					{
						return this.m_instance;
					}
					set
					{
						this.m_instance = value;
					}
				}
			}

			private Hashtable m_controls;

			internal CustomReportItemControls()
			{
				this.m_controls = new Hashtable();
			}

			internal AspNetCore.ReportingServices.ReportRendering.ICustomReportItem GetControlInstance(string name, IExtensionFactory extFactory)
			{
				lock (this)
				{
					CustomControlInfo customControlInfo = this.m_controls[name] as CustomControlInfo;
					if (customControlInfo == null)
					{
						AspNetCore.ReportingServices.ReportRendering.ICustomReportItem customReportItem = null;
						Global.Tracer.Assert(extFactory != null, "extFactory != null");
						customReportItem = (extFactory.GetNewCustomReportItemProcessingInstanceClass(name) as AspNetCore.ReportingServices.ReportRendering.ICustomReportItem);
						customControlInfo = new CustomControlInfo();
						customControlInfo.Instance = customReportItem;
						customControlInfo.IsValid = (null != customReportItem);
						this.m_controls.Add(name, customControlInfo);
					}
					Global.Tracer.Assert(null != customControlInfo);
					if (customControlInfo.IsValid)
					{
						return customControlInfo.Instance;
					}
					return null;
				}
			}
		}

		private abstract class RuntimeTablixObj : RuntimeRDLDataRegionObj
		{
			protected Tablix m_tablixDef;

			protected RuntimeTablixHeadingsObj m_tablixRows;

			protected RuntimeTablixHeadingsObj m_tablixColumns;

			protected RuntimeTablixHeadingsObj m_outerGroupings;

			protected RuntimeTablixHeadingsObj m_innerGroupings;

			protected int[] m_outerGroupingCounters;

			protected override IScope OuterScope
			{
				get
				{
					return base.m_outerScope;
				}
			}

			protected override string ScopeName
			{
				get
				{
					return this.m_tablixDef.Name;
				}
			}

			protected override DataRegion DataRegionDef
			{
				get
				{
					return this.m_tablixDef;
				}
			}

			internal int[] OuterGroupingCounters
			{
				get
				{
					return this.m_outerGroupingCounters;
				}
			}

			internal RuntimeTablixObj(IScope outerScope, Tablix tablixDef, ref DataActions dataAction, ProcessingContext processingContext, bool onePassProcess)
				: base(outerScope, (DataRegion)tablixDef, ref dataAction, processingContext, onePassProcess, tablixDef.RunningValues)
			{
				base.m_outerScope = outerScope;
				this.m_tablixDef = tablixDef;
			}

			protected void ConstructorHelper(ref DataActions dataAction, bool onePassProcess, out bool handleMyDataAction, out DataActions innerDataAction, out TablixHeadingList outermostColumns, out TablixHeadingList outermostRows, out TablixHeadingList staticColumns, out TablixHeadingList staticRows)
			{
				if (this.m_tablixDef.Filters != null)
				{
					base.m_filters = new Filters(Filters.FilterTypes.DataRegionFilter, this, this.m_tablixDef.Filters, this.m_tablixDef.ObjectType, this.m_tablixDef.Name, base.m_processingContext);
				}
				else
				{
					base.m_outerDataAction = dataAction;
					base.m_dataAction = dataAction;
					dataAction = DataActions.None;
				}
				this.m_tablixDef.GetHeadingDefState(out outermostColumns, out outermostRows, out staticColumns, out staticRows);
				innerDataAction = base.m_dataAction;
				handleMyDataAction = false;
				bool flag = false;
				RuntimeDataRegionObj.CreateAggregates(base.m_processingContext, this.m_tablixDef.Aggregates, ref base.m_nonCustomAggregates, ref base.m_customAggregates);
				if (onePassProcess)
				{
					flag = true;
					if (this.m_tablixDef.RunningValues != null && 0 < this.m_tablixDef.RunningValues.Count)
					{
						RuntimeDataRegionObj.CreateAggregates(base.m_processingContext, this.m_tablixDef.RunningValues, ref base.m_nonCustomAggregates);
					}
					RuntimeDataRegionObj.CreateAggregates(base.m_processingContext, this.m_tablixDef.PostSortAggregates, ref base.m_nonCustomAggregates);
					Global.Tracer.Assert(outermostRows == null && null == outermostColumns);
					RuntimeDataRegionObj.CreateAggregates(base.m_processingContext, this.m_tablixDef.CellPostSortAggregates, ref base.m_nonCustomAggregates);
				}
				else
				{
					if (this.m_tablixDef.RunningValues != null && 0 < this.m_tablixDef.RunningValues.Count)
					{
						base.m_dataAction |= DataActions.PostSortAggregates;
					}
					if (this.m_tablixDef.PostSortAggregates != null)
					{
						RuntimeDataRegionObj.CreateAggregates(base.m_processingContext, this.m_tablixDef.PostSortAggregates, ref base.m_postSortAggregates);
						handleMyDataAction = true;
					}
					if (outermostRows == null && outermostColumns == null)
					{
						flag = true;
						if (this.m_tablixDef.CellPostSortAggregates != null)
						{
							RuntimeDataRegionObj.CreateAggregates(base.m_processingContext, this.m_tablixDef.CellPostSortAggregates, ref base.m_postSortAggregates);
							handleMyDataAction = true;
						}
					}
					if (handleMyDataAction)
					{
						base.m_dataAction |= DataActions.PostSortAggregates;
						innerDataAction = DataActions.None;
					}
					else
					{
						innerDataAction = base.m_dataAction;
					}
				}
				if (flag)
				{
					RuntimeDataRegionObj.CreateAggregates(base.m_processingContext, this.m_tablixDef.CellAggregates, ref base.m_nonCustomAggregates, ref base.m_customAggregates);
					RunningValueInfoList tablixCellRunningValues = this.m_tablixDef.TablixCellRunningValues;
					if (tablixCellRunningValues != null && 0 < tablixCellRunningValues.Count)
					{
						if (base.m_nonCustomAggregates == null)
						{
							base.m_nonCustomAggregates = new DataAggregateObjList();
						}
						for (int i = 0; i < tablixCellRunningValues.Count; i++)
						{
							base.m_nonCustomAggregates.Add(new DataAggregateObj(tablixCellRunningValues[i], base.m_processingContext));
						}
					}
				}
				int num = this.m_tablixDef.CreateOuterGroupingIndexList();
				this.m_outerGroupingCounters = new int[num];
				for (int j = 0; j < this.m_outerGroupingCounters.Length; j++)
				{
					this.m_outerGroupingCounters[j] = -1;
				}
			}

			protected void HandleDataAction(bool handleMyDataAction, DataActions innerDataAction)
			{
				if (!handleMyDataAction)
				{
					base.m_dataAction = innerDataAction;
				}
				if (base.m_dataAction != 0)
				{
					base.m_dataRows = new DataRowList();
				}
			}

			protected override void SendToInner()
			{
				this.m_tablixDef.RuntimeDataRegionObj = this;
				this.m_tablixDef.ResetOutergGroupingAggregateRowInfo();
				this.m_tablixDef.SaveTablixAggregateRowInfo(base.m_processingContext);
				if (this.m_outerGroupings != null)
				{
					this.m_outerGroupings.NextRow();
				}
				this.m_tablixDef.RestoreTablixAggregateRowInfo(base.m_processingContext);
				if (this.m_innerGroupings != null)
				{
					this.m_innerGroupings.NextRow();
				}
			}

			internal override bool SortAndFilter()
			{
				this.SetupEnvironment();
				if (this.m_tablixRows != null)
				{
					this.m_tablixRows.SortAndFilter();
				}
				if (this.m_tablixColumns != null)
				{
					this.m_tablixColumns.SortAndFilter();
				}
				return base.SortAndFilter();
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				if (this.m_tablixDef.RunningValues != null && base.m_runningValues == null)
				{
					RuntimeDetailObj.AddRunningValues(base.m_processingContext, this.m_tablixDef.RunningValues, ref base.m_runningValues, globalRVCol, groupCol, lastGroup);
				}
				if (base.m_dataRows != null)
				{
					base.ReadRows(DataActions.PostSortAggregates);
					base.m_dataRows = null;
				}
				if (this.m_outerGroupings != null)
				{
					this.m_outerGroupings.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				}
				if (this.m_outerGroupings != null && this.m_outerGroupings.Headings != null)
				{
					return;
				}
				if (this.m_innerGroupings != null)
				{
					this.m_innerGroupings.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				}
			}

			protected virtual void CalculatePreviousAggregates()
			{
				if (!base.m_processedPreviousAggregates && base.m_processingContext.GlobalRVCollection != null)
				{
					Global.Tracer.Assert(null == base.m_runningValueValues);
					AggregatesImpl globalRVCollection = base.m_processingContext.GlobalRVCollection;
					RunningValueInfoList runningValues = this.m_tablixDef.RunningValues;
					RuntimeRICollection.DoneReadingRows(globalRVCollection, runningValues, ref base.m_runningValueValues, true);
					if (this.m_tablixRows != null)
					{
						this.m_tablixRows.CalculatePreviousAggregates(globalRVCollection);
					}
					if (this.m_tablixColumns != null)
					{
						this.m_tablixColumns.CalculatePreviousAggregates(globalRVCollection);
					}
					base.m_processedPreviousAggregates = true;
				}
			}

			internal override void ReadRow(DataActions dataAction)
			{
				if (!this.m_tablixDef.ProcessCellRunningValues)
				{
					if (DataActions.UserSort == dataAction)
					{
						RuntimeDataRegionObj.CommonFirstRow(base.m_processingContext.ReportObjectModel.FieldsImpl, ref base.m_firstRowIsAggregate, ref base.m_firstRow);
						base.CommonNextRow(base.m_dataRows);
					}
					else
					{
						if (DataActions.PostSortAggregates == dataAction)
						{
							if (base.m_postSortAggregates != null)
							{
								RuntimeDataRegionObj.UpdateAggregates(base.m_processingContext, base.m_postSortAggregates, false);
							}
							if (base.m_runningValues != null)
							{
								for (int i = 0; i < base.m_runningValues.Count; i++)
								{
									base.m_runningValues[i].Update();
								}
							}
							this.CalculatePreviousAggregates();
						}
						if (base.m_outerScope != null && (dataAction & base.m_outerDataAction) != 0)
						{
							base.m_outerScope.ReadRow(dataAction);
						}
					}
				}
			}

			internal override void SetupEnvironment()
			{
				base.SetupEnvironment(this.m_tablixDef.RunningValues);
			}

			internal override bool InScope(string scope)
			{
				return base.DataRegionInScope(this.m_tablixDef, scope);
			}

			protected override int GetRecursiveLevel(string scope)
			{
				return base.DataRegionRecursiveLevel(this.m_tablixDef, scope);
			}
		}

		internal abstract class RuntimeTablixHeadingsObj
		{
			protected IScope m_owner;

			protected RuntimeTablixGroupRootObj m_tablixHeadings;

			protected TablixHeadingList m_staticHeadingDef;

			internal RuntimeTablixGroupRootObj Headings
			{
				get
				{
					return this.m_tablixHeadings;
				}
			}

			internal RuntimeTablixHeadingsObj(IScope owner, TablixHeadingList headingDef, ref DataActions dataAction, ProcessingContext processingContext, TablixHeadingList staticHeadingDef, RuntimeTablixHeadingsObj innerGroupings, int headingLevel)
			{
				this.m_owner = owner;
				if (staticHeadingDef != null)
				{
					this.m_staticHeadingDef = staticHeadingDef;
				}
			}

			internal virtual void NextRow()
			{
				if (this.m_tablixHeadings != null)
				{
					this.m_tablixHeadings.NextRow();
				}
			}

			internal virtual bool SortAndFilter()
			{
				if (this.m_tablixHeadings != null)
				{
					return this.m_tablixHeadings.SortAndFilter();
				}
				return true;
			}

			internal virtual void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				if (this.m_tablixHeadings != null)
				{
					this.m_tablixHeadings.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				}
			}

			internal abstract void CalculatePreviousAggregates(AggregatesImpl globalRVCol);
		}

		internal abstract class RuntimeTablixGroupRootObj : RuntimeGroupRootObj
		{
			protected RuntimeTablixHeadingsObj m_innerGroupings;

			protected TablixHeadingList m_staticHeadingDef;

			protected bool m_outermostSubtotal;

			protected TablixHeadingList m_innerHeading;

			protected bool m_processOutermostSTCells;

			protected DataAggregateObjList m_outermostSTCellRVs;

			protected DataAggregateObjList m_cellRVs;

			protected int m_headingIndex = -1;

			protected int m_headingLevel;

			protected object m_currentGroupExprValue;

			internal RuntimeTablixHeadingsObj InnerGroupings
			{
				get
				{
					return this.m_innerGroupings;
				}
			}

			internal TablixHeadingList StaticHeadingDef
			{
				get
				{
					return this.m_staticHeadingDef;
				}
			}

			internal bool OutermostSubtotal
			{
				get
				{
					return this.m_outermostSubtotal;
				}
			}

			internal TablixHeadingList InnerHeading
			{
				get
				{
					return this.m_innerHeading;
				}
			}

			internal bool ProcessOutermostSTCells
			{
				get
				{
					return this.m_processOutermostSTCells;
				}
			}

			internal AggregatesImpl OutermostSTCellRVCol
			{
				get
				{
					return ((TablixHeading)base.m_hierarchyDef).OutermostSTCellRVCol;
				}
			}

			internal AggregatesImpl[] OutermostSTScopedCellRVCollections
			{
				get
				{
					return ((TablixHeading)base.m_hierarchyDef).OutermostSTCellScopedRVCollections;
				}
			}

			internal AggregatesImpl CellRVCol
			{
				get
				{
					return ((TablixHeading)base.m_hierarchyDef).CellRVCol;
				}
			}

			internal AggregatesImpl[] CellScopedRVCollections
			{
				get
				{
					return ((TablixHeading)base.m_hierarchyDef).CellScopedRVCollections;
				}
			}

			internal int HeadingLevel
			{
				get
				{
					return this.m_headingLevel;
				}
			}

			internal object CurrentGroupExpressionValue
			{
				get
				{
					return this.m_currentGroupExprValue;
				}
			}

			internal RuntimeTablixGroupRootObj(IScope outerScope, TablixHeadingList tablixHeadingDef, int headingIndex, ref DataActions dataAction, ProcessingContext processingContext, RuntimeTablixHeadingsObj innerGroupings, bool outermostSubtotal, int headingLevel)
				: base(outerScope, tablixHeadingDef[headingIndex], dataAction, processingContext)
			{
				Tablix tablix = (Tablix)tablixHeadingDef[headingIndex].DataRegionDef;
				Global.Tracer.Assert(tablixHeadingDef != null && headingIndex < tablixHeadingDef.Count && 0 <= headingIndex);
				this.m_headingIndex = headingIndex;
				this.m_innerHeading = ((CustomReportItemHeadingList)tablixHeadingDef)[headingIndex].InnerHeadings;
				tablix.SkipStaticHeading(ref this.m_innerHeading, ref this.m_staticHeadingDef);
				if (outermostSubtotal && this.m_innerHeading == null)
				{
					this.m_processOutermostSTCells = true;
					if (tablix.CellPostSortAggregates != null)
					{
						base.m_dataAction |= DataActions.PostSortAggregates;
					}
				}
				this.NeedProcessDataActions(tablixHeadingDef);
				this.NeedProcessDataActions(this.m_staticHeadingDef);
				this.m_outermostSubtotal = outermostSubtotal;
				this.m_innerGroupings = innerGroupings;
				this.m_headingLevel = headingLevel;
				if (tablixHeadingDef[headingIndex].Grouping.Filters == null)
				{
					dataAction = DataActions.None;
				}
			}

			protected abstract void NeedProcessDataActions(TablixHeadingList heading);

			protected void NeedProcessDataActions(RunningValueInfoList runningValues)
			{
				if ((base.m_dataAction & DataActions.PostSortAggregates) == DataActions.None && runningValues != null && 0 < runningValues.Count)
				{
					base.m_dataAction |= DataActions.PostSortAggregates;
				}
			}

			internal override void NextRow()
			{
				if (base.ProcessThisRow())
				{
					this.m_currentGroupExprValue = base.EvaluateGroupExpression(base.m_expression, "Group");
					Grouping grouping = base.m_hierarchyDef.Grouping;
					if (base.m_saveGroupExprValues)
					{
						grouping.CurrentGroupExpressionValues = new VariantList();
						grouping.CurrentGroupExpressionValues.Add(this.m_currentGroupExprValue);
					}
					object parentKey = null;
					bool flag = null != base.m_parentExpression;
					if (flag)
					{
						parentKey = base.EvaluateGroupExpression(base.m_parentExpression, "Parent");
					}
					Global.Tracer.Assert(null != base.m_grouping);
					base.m_grouping.NextRow(this.m_currentGroupExprValue, flag, parentKey);
				}
			}

			internal override bool SortAndFilter()
			{
				Tablix tablix = (Tablix)base.m_hierarchyDef.DataRegionDef;
				TablixHeading tablixHeading = (TablixHeading)base.m_hierarchyDef;
				Global.Tracer.Assert(null != base.m_hierarchyDef.Grouping);
				if ((ProcessingContext.SecondPassOperations.Filtering & base.m_processingContext.SecondPassOperation) != 0 && base.m_hierarchyDef.Grouping.Filters == null)
				{
					if (tablixHeading.IsColumn && this.m_headingLevel < tablix.InnermostColumnFilterLevel)
					{
						goto IL_0085;
					}
					if (!tablixHeading.IsColumn && this.m_headingLevel < tablix.InnermostRowFilterLevel)
					{
						goto IL_0085;
					}
				}
				goto IL_0091;
				IL_0085:
				tablixHeading.Grouping.HasInnerFilters = true;
				goto IL_0091;
				IL_0091:
				bool result = base.SortAndFilter();
				tablixHeading.Grouping.HasInnerFilters = false;
				return result;
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				base.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				Tablix tablix = (Tablix)base.m_hierarchyDef.DataRegionDef;
				TablixHeading tablixHeading = (TablixHeading)base.m_hierarchyDef;
				AggregatesImpl outermostSTCellRVCol = tablixHeading.OutermostSTCellRVCol;
				AggregatesImpl[] outermostSTCellScopedRVCollections = tablixHeading.OutermostSTCellScopedRVCollections;
				if (this.SetupCellRunningValues(ref outermostSTCellRVCol, ref outermostSTCellScopedRVCollections))
				{
					tablixHeading.OutermostSTCellRVCol = outermostSTCellRVCol;
					tablixHeading.OutermostSTCellScopedRVCollections = outermostSTCellScopedRVCollections;
				}
				if (this.m_processOutermostSTCells)
				{
					if (this.m_innerGroupings != null)
					{
						tablix.CurrentOuterHeadingGroupRoot = this;
					}
					base.m_processingContext.EnterPivotCell(null != this.m_innerGroupings);
					tablix.ProcessOutermostSTCellRunningValues = true;
					this.AddCellRunningValues(outermostSTCellRVCol, groupCol, ref this.m_outermostSTCellRVs);
					tablix.ProcessOutermostSTCellRunningValues = false;
					base.m_processingContext.ExitPivotCell();
				}
				if (this.m_innerGroupings != null)
				{
					AggregatesImpl cellRVCol = tablixHeading.CellRVCol;
					AggregatesImpl[] cellScopedRVCollections = tablixHeading.CellScopedRVCollections;
					if (this.SetupCellRunningValues(ref cellRVCol, ref cellScopedRVCollections))
					{
						tablixHeading.CellRVCol = cellRVCol;
						tablixHeading.CellScopedRVCollections = cellScopedRVCollections;
					}
				}
				else
				{
					RuntimeTablixGroupRootObj currentOuterHeadingGroupRoot = tablix.CurrentOuterHeadingGroupRoot;
					if (this.m_innerHeading == null && currentOuterHeadingGroupRoot != null)
					{
						base.m_processingContext.EnterPivotCell(true);
						tablix.ProcessCellRunningValues = true;
						this.m_cellRVs = null;
						this.AddCellRunningValues(currentOuterHeadingGroupRoot.CellRVCol, groupCol, ref this.m_cellRVs);
						tablix.ProcessCellRunningValues = false;
						base.m_processingContext.ExitPivotCell();
					}
				}
			}

			private bool SetupCellRunningValues(ref AggregatesImpl globalCellRVCol, ref AggregatesImpl[] cellScopedRVLists)
			{
				if (globalCellRVCol != null && cellScopedRVLists != null)
				{
					return false;
				}
				globalCellRVCol = new AggregatesImpl(base.m_processingContext.ReportRuntime);
				cellScopedRVLists = this.CreateScopedCellRVCollections();
				return true;
			}

			protected abstract void AddCellRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, ref DataAggregateObjList runningValues);

			internal override void AddScopedRunningValue(DataAggregateObj runningValueObj, bool escalate)
			{
				Tablix tablix = (Tablix)base.m_hierarchyDef.DataRegionDef;
				if (tablix.ProcessOutermostSTCellRunningValues || tablix.ProcessCellRunningValues)
				{
					RuntimeTablixGroupRootObj currentOuterHeadingGroupRoot = tablix.CurrentOuterHeadingGroupRoot;
					int headingLevel = currentOuterHeadingGroupRoot.HeadingLevel;
					TablixHeading tablixHeading = (!escalate) ? ((TablixHeading)base.m_hierarchyDef) : ((TablixHeading)currentOuterHeadingGroupRoot.HierarchyDef);
					if (tablix.ProcessOutermostSTCellRunningValues)
					{
						this.AddCellScopedRunningValue(runningValueObj, tablixHeading.OutermostSTCellScopedRVCollections, headingLevel);
					}
					else if (tablix.ProcessCellRunningValues)
					{
						this.AddCellScopedRunningValue(runningValueObj, tablixHeading.CellScopedRVCollections, headingLevel);
					}
				}
				else
				{
					base.AddScopedRunningValue(runningValueObj, escalate);
				}
			}

			private void AddCellScopedRunningValue(DataAggregateObj runningValueObj, AggregatesImpl[] cellScopedRVLists, int currentOuterHeadingLevel)
			{
				if (cellScopedRVLists != null)
				{
					AggregatesImpl aggregatesImpl = cellScopedRVLists[currentOuterHeadingLevel];
					if (aggregatesImpl == null)
					{
						aggregatesImpl = (cellScopedRVLists[currentOuterHeadingLevel] = new AggregatesImpl(base.m_processingContext.ReportRuntime));
					}
					if (aggregatesImpl.GetAggregateObj(runningValueObj.Name) == null)
					{
						aggregatesImpl.Add(runningValueObj);
					}
				}
			}

			internal override void ReadRow(DataActions dataAction)
			{
				Tablix tablix = (Tablix)base.m_hierarchyDef.DataRegionDef;
				if (tablix.ProcessCellRunningValues)
				{
					Global.Tracer.Assert(DataActions.PostSortAggregates == dataAction);
					if (this.m_cellRVs != null)
					{
						for (int i = 0; i < this.m_cellRVs.Count; i++)
						{
							this.m_cellRVs[i].Update();
						}
					}
					if (base.m_outerScope != null && tablix.CellPostSortAggregates != null)
					{
						base.m_outerScope.ReadRow(dataAction);
					}
				}
				else
				{
					if (DataActions.PostSortAggregates == dataAction && this.m_outermostSTCellRVs != null)
					{
						for (int j = 0; j < this.m_outermostSTCellRVs.Count; j++)
						{
							this.m_outermostSTCellRVs[j].Update();
						}
					}
					base.ReadRow(dataAction);
				}
			}

			private AggregatesImpl[] CreateScopedCellRVCollections()
			{
				Tablix tablix = (Tablix)base.m_hierarchyDef.DataRegionDef;
				int dynamicHeadingCount = tablix.GetDynamicHeadingCount(true);
				if (0 < dynamicHeadingCount)
				{
					return new AggregatesImpl[dynamicHeadingCount];
				}
				return null;
			}

			internal bool GetCellTargetForNonDetailSort()
			{
				if (base.m_outerScope is RuntimeTablixObj)
				{
					return base.m_outerScope.TargetForNonDetailSort;
				}
				Global.Tracer.Assert(base.m_outerScope is RuntimeTablixGroupLeafObj);
				return ((RuntimeTablixGroupLeafObj)base.m_outerScope).GetCellTargetForNonDetailSort();
			}

			internal bool GetCellTargetForSort(int index, bool detailSort)
			{
				if (base.m_outerScope is RuntimeTablixObj)
				{
					return base.m_outerScope.IsTargetForSort(index, detailSort);
				}
				Global.Tracer.Assert(base.m_outerScope is RuntimeTablixGroupLeafObj);
				return ((RuntimeTablixGroupLeafObj)base.m_outerScope).GetCellTargetForSort(index, detailSort);
			}
		}

		private abstract class RuntimeTablixGroupLeafObj : RuntimeGroupLeafObj
		{
			protected RuntimeTablixHeadingsObj m_tablixHeadings;

			protected RuntimeTablixGroupRootObj m_innerHeadingList;

			protected DataAggregateObjList m_firstPassCellNonCustomAggs;

			protected DataAggregateObjList m_firstPassCellCustomAggs;

			protected RuntimeTablixCells[] m_cellsList;

			protected DataAggregateObjList m_cellPostSortAggregates;

			protected int m_groupLeafIndex = -1;

			protected bool m_processHeading = true;

			internal TablixHeading TablixHeadingDef
			{
				get
				{
					RuntimeTablixGroupRootObj runtimeTablixGroupRootObj = (RuntimeTablixGroupRootObj)base.m_hierarchyRoot;
					return (TablixHeading)runtimeTablixGroupRootObj.HierarchyDef;
				}
			}

			internal DataAggregateObjList CellPostSortAggregates
			{
				get
				{
					return this.m_cellPostSortAggregates;
				}
			}

			internal Tablix TablixDef
			{
				get
				{
					return (Tablix)this.TablixHeadingDef.DataRegionDef;
				}
			}

			internal int HeadingLevel
			{
				get
				{
					RuntimeTablixGroupRootObj runtimeTablixGroupRootObj = (RuntimeTablixGroupRootObj)base.m_hierarchyRoot;
					return runtimeTablixGroupRootObj.HeadingLevel;
				}
			}

			internal RuntimeTablixGroupLeafObj(RuntimeTablixGroupRootObj groupRoot)
				: base(groupRoot)
			{
			}

			protected void ConstructorHelper(RuntimeTablixGroupRootObj groupRoot, Tablix tablixDef, out bool handleMyDataAction, out DataActions innerDataAction)
			{
				TablixHeadingList innerHeading = groupRoot.InnerHeading;
				base.m_dataAction = groupRoot.DataAction;
				handleMyDataAction = false;
				if (base.m_postSortAggregates != null || (base.m_recursiveAggregates != null && base.m_processingContext.SpecialRecursiveAggregates))
				{
					handleMyDataAction = true;
				}
				if (groupRoot.ProcessOutermostSTCells)
				{
					RuntimeDataRegionObj.CreateAggregates(base.m_processingContext, tablixDef.CellAggregates, ref this.m_firstPassCellNonCustomAggs, ref this.m_firstPassCellCustomAggs);
					if (tablixDef.CellPostSortAggregates != null)
					{
						handleMyDataAction = true;
						RuntimeDataRegionObj.CreateAggregates(base.m_processingContext, tablixDef.CellPostSortAggregates, ref base.m_postSortAggregates);
					}
				}
				if (handleMyDataAction)
				{
					innerDataAction = DataActions.None;
				}
				else
				{
					innerDataAction = base.m_dataAction;
				}
				if (!this.IsOuterGrouping())
				{
					if (groupRoot.InnerHeading == null)
					{
						TablixHeadingList tablixHeadingList = null;
						TablixHeadingList tablixHeadingList2 = tablixDef.GetOuterHeading();
						int dynamicHeadingCount = tablixDef.GetDynamicHeadingCount(true);
						int num = 0;
						tablixDef.SkipStaticHeading(ref tablixHeadingList2, ref tablixHeadingList);
						while (tablixHeadingList2 != null)
						{
							tablixHeadingList2 = tablixHeadingList2.InnerHeadings();
							tablixDef.SkipStaticHeading(ref tablixHeadingList2, ref tablixHeadingList);
							if (this.m_cellsList == null)
							{
								this.m_cellsList = new RuntimeTablixCells[dynamicHeadingCount];
								RuntimeDataRegionObj.CreateAggregates(base.m_processingContext, tablixDef.CellPostSortAggregates, ref this.m_cellPostSortAggregates);
							}
							RuntimeTablixCells runtimeTablixCells = null;
							if (tablixHeadingList2 == null)
							{
								runtimeTablixCells = new RuntimeTablixCells();
							}
							this.m_cellsList[num++] = runtimeTablixCells;
						}
					}
				}
				else
				{
					this.m_groupLeafIndex = ++((RuntimeTablixObj)tablixDef.RuntimeDataRegionObj).OuterGroupingCounters[groupRoot.HeadingLevel];
				}
				TablixHeading tablixHeading = (TablixHeading)groupRoot.HierarchyDef;
				Global.Tracer.Assert(null != tablixHeading.Grouping);
				if (tablixHeading.Grouping.Filters != null)
				{
					if (tablixHeading.IsColumn)
					{
						if (groupRoot.HeadingLevel > tablixDef.InnermostColumnFilterLevel)
						{
							tablixDef.InnermostColumnFilterLevel = groupRoot.HeadingLevel;
						}
					}
					else if (groupRoot.HeadingLevel > tablixDef.InnermostRowFilterLevel)
					{
						tablixDef.InnermostRowFilterLevel = groupRoot.HeadingLevel;
					}
				}
			}

			internal abstract RuntimeTablixCell CreateCell(int index, Tablix tablixDef);

			internal override void NextRow()
			{
				Tablix tablixDef = this.TablixDef;
				int headingLevel = this.HeadingLevel;
				bool flag = this.IsOuterGrouping();
				if (flag)
				{
					tablixDef.OuterGroupingIndexes[headingLevel] = this.m_groupLeafIndex;
				}
				base.NextRow();
				if (flag)
				{
					tablixDef.SaveOuterGroupingAggregateRowInfo(headingLevel, base.m_processingContext);
				}
				FieldsImpl fieldsImpl = base.m_processingContext.ReportObjectModel.FieldsImpl;
				if (fieldsImpl.AggregationFieldCount == 0 && fieldsImpl.ValidAggregateRow)
				{
					RuntimeDataRegionObj.UpdateAggregates(base.m_processingContext, this.m_firstPassCellCustomAggs, false);
				}
				if (!base.m_processingContext.ReportObjectModel.FieldsImpl.IsAggregateRow)
				{
					RuntimeDataRegionObj.UpdateAggregates(base.m_processingContext, this.m_firstPassCellNonCustomAggs, false);
				}
			}

			protected override void SendToInner()
			{
				base.SendToInner();
				if (this.m_tablixHeadings != null)
				{
					this.m_tablixHeadings.NextRow();
				}
				if (this.m_cellsList != null)
				{
					Global.Tracer.Assert(!this.IsOuterGrouping());
					Tablix tablixDef = this.TablixDef;
					int[] outerGroupingIndexes = tablixDef.OuterGroupingIndexes;
					for (int i = 0; i < tablixDef.GetDynamicHeadingCount(true); i++)
					{
						int num = outerGroupingIndexes[i];
						AggregateRowInfo aggregateRowInfo = new AggregateRowInfo();
						aggregateRowInfo.SaveAggregateInfo(base.m_processingContext);
						tablixDef.SetCellAggregateRowInfo(i, base.m_processingContext);
						RuntimeTablixCells runtimeTablixCells = this.m_cellsList[i];
						if (runtimeTablixCells != null)
						{
							RuntimeTablixCell runtimeTablixCell = runtimeTablixCells[num];
							if (runtimeTablixCell == null)
							{
								runtimeTablixCell = this.CreateCell(i, tablixDef);
								runtimeTablixCells.Add(num, runtimeTablixCell);
							}
							runtimeTablixCell.NextRow();
						}
						aggregateRowInfo.RestoreAggregateInfo(base.m_processingContext);
					}
				}
			}

			internal override bool SortAndFilter()
			{
				this.SetupEnvironment();
				RuntimeTablixGroupRootObj runtimeTablixGroupRootObj = (RuntimeTablixGroupRootObj)base.m_hierarchyRoot;
				bool flag = false;
				if (this.m_innerHeadingList != null && !this.m_tablixHeadings.SortAndFilter())
				{
					Global.Tracer.Assert((ProcessingContext.SecondPassOperations)0 != (ProcessingContext.SecondPassOperations.Filtering & base.m_processingContext.SecondPassOperation));
					Global.Tracer.Assert(null != runtimeTablixGroupRootObj.GroupFilters);
					runtimeTablixGroupRootObj.GroupFilters.FailFilters = true;
					flag = true;
				}
				bool flag2 = base.SortAndFilter();
				if (flag)
				{
					runtimeTablixGroupRootObj.GroupFilters.FailFilters = false;
				}
				if (flag2 && this.m_cellsList != null)
				{
					for (int i = 0; i < this.m_cellsList.Length; i++)
					{
						if (this.m_cellsList[i] != null)
						{
							this.m_cellsList[i].SortAndFilter();
						}
					}
				}
				return flag2;
			}

			internal override void CalculateRunningValues()
			{
				Tablix tablixDef = this.TablixDef;
				RuntimeTablixGroupRootObj runtimeTablixGroupRootObj = (RuntimeTablixGroupRootObj)base.m_hierarchyRoot;
				AggregatesImpl globalRunningValueCollection = runtimeTablixGroupRootObj.GlobalRunningValueCollection;
				RuntimeGroupRootObjList groupCollection = runtimeTablixGroupRootObj.GroupCollection;
				bool flag = this.IsOuterGrouping();
				tablixDef.GetDynamicHeadingCount(true);
				if (this.m_processHeading)
				{
					if (base.m_dataRows != null && (DataActions.PostSortAggregates & base.m_dataAction) != 0)
					{
						base.ReadRows(DataActions.PostSortAggregates);
						base.m_dataRows = null;
					}
					this.m_tablixHeadings.CalculateRunningValues(globalRunningValueCollection, groupCollection, runtimeTablixGroupRootObj);
				}
				else if (this.m_innerHeadingList != null)
				{
					this.m_innerHeadingList.CalculateRunningValues(globalRunningValueCollection, groupCollection, runtimeTablixGroupRootObj);
				}
				if (flag)
				{
					if (this.m_innerHeadingList == null)
					{
						tablixDef.CurrentOuterHeadingGroupRoot = runtimeTablixGroupRootObj;
						tablixDef.OuterGroupingIndexes[runtimeTablixGroupRootObj.HeadingLevel] = this.m_groupLeafIndex;
						runtimeTablixGroupRootObj.InnerGroupings.CalculateRunningValues(globalRunningValueCollection, groupCollection, runtimeTablixGroupRootObj);
					}
				}
				else if (this.m_cellsList != null)
				{
					RuntimeTablixGroupRootObj currentOuterHeadingGroupRoot = tablixDef.CurrentOuterHeadingGroupRoot;
					RuntimeTablixCells runtimeTablixCells = this.m_cellsList[currentOuterHeadingGroupRoot.HeadingLevel];
					Global.Tracer.Assert(null != runtimeTablixCells);
					tablixDef.ProcessCellRunningValues = true;
					runtimeTablixCells.CalculateRunningValues(tablixDef, currentOuterHeadingGroupRoot.CellRVCol, groupCollection, runtimeTablixGroupRootObj, this, currentOuterHeadingGroupRoot.HeadingLevel);
					tablixDef.ProcessCellRunningValues = false;
				}
			}

			protected override void ResetScopedRunningValues()
			{
				base.ResetScopedRunningValues();
				this.ResetScopedCellRunningValues();
			}

			internal bool IsOuterGrouping()
			{
				RuntimeTablixGroupRootObj runtimeTablixGroupRootObj = (RuntimeTablixGroupRootObj)base.m_hierarchyRoot;
				return null != runtimeTablixGroupRootObj.InnerGroupings;
			}

			internal override void ReadRow(DataActions dataAction)
			{
				if (DataActions.UserSort == dataAction)
				{
					RuntimeDataRegionObj.CommonFirstRow(base.m_processingContext.ReportObjectModel.FieldsImpl, ref base.m_firstRowIsAggregate, ref base.m_firstRow);
					base.CommonNextRow(base.m_dataRows);
				}
				else if (this.TablixDef.ProcessCellRunningValues)
				{
					if (DataActions.PostSortAggregates == dataAction && this.m_cellPostSortAggregates != null)
					{
						RuntimeDataRegionObj.UpdateAggregates(base.m_processingContext, this.m_cellPostSortAggregates, false);
					}
					((IScope)base.m_hierarchyRoot).ReadRow(dataAction);
				}
				else
				{
					base.ReadRow(dataAction);
					if (DataActions.PostSortAggregates == dataAction)
					{
						this.CalculatePreviousAggregates();
					}
				}
			}

			protected virtual bool CalculatePreviousAggregates()
			{
				if (!base.m_processedPreviousAggregates && base.m_processingContext.GlobalRVCollection != null)
				{
					if (this.m_innerHeadingList != null)
					{
						this.m_tablixHeadings.CalculatePreviousAggregates(base.m_processingContext.GlobalRVCollection);
					}
					base.m_processedPreviousAggregates = true;
					return true;
				}
				return false;
			}

			protected void ResetScopedCellRunningValues()
			{
				RuntimeTablixGroupRootObj runtimeTablixGroupRootObj = (RuntimeTablixGroupRootObj)base.m_hierarchyRoot;
				if (runtimeTablixGroupRootObj.OutermostSTScopedCellRVCollections != null)
				{
					for (int i = 0; i < runtimeTablixGroupRootObj.OutermostSTScopedCellRVCollections.Length; i++)
					{
						AggregatesImpl aggregatesImpl = runtimeTablixGroupRootObj.OutermostSTScopedCellRVCollections[i];
						if (aggregatesImpl != null)
						{
							foreach (DataAggregateObj @object in aggregatesImpl.Objects)
							{
								@object.Init();
							}
						}
					}
				}
				if (runtimeTablixGroupRootObj.CellScopedRVCollections != null)
				{
					for (int j = 0; j < runtimeTablixGroupRootObj.CellScopedRVCollections.Length; j++)
					{
						AggregatesImpl aggregatesImpl2 = runtimeTablixGroupRootObj.CellScopedRVCollections[j];
						if (aggregatesImpl2 != null)
						{
							foreach (DataAggregateObj object2 in aggregatesImpl2.Objects)
							{
								object2.Init();
							}
						}
					}
				}
			}

			internal override void SetupEnvironment()
			{
				base.SetupEnvironment();
				this.SetupAggregateValues(this.m_firstPassCellNonCustomAggs, this.m_firstPassCellCustomAggs);
			}

			private void SetupAggregateValues(DataAggregateObjList nonCustomAggCollection, DataAggregateObjList customAggCollection)
			{
				base.SetupAggregates(nonCustomAggCollection);
				base.SetupAggregates(customAggCollection);
			}

			internal bool GetCellTargetForNonDetailSort()
			{
				return ((RuntimeTablixGroupRootObj)base.m_hierarchyRoot).GetCellTargetForNonDetailSort();
			}

			internal bool GetCellTargetForSort(int index, bool detailSort)
			{
				return ((RuntimeTablixGroupRootObj)base.m_hierarchyRoot).GetCellTargetForSort(index, detailSort);
			}
		}

		private sealed class RuntimeTablixCells : Hashtable
		{
			private RuntimeTablixCell m_firstCell;

			private RuntimeTablixCell m_lastCell;

			internal RuntimeTablixCell this[int index]
			{
				get
				{
					return (RuntimeTablixCell)base[index];
				}
				set
				{
					if (base.Count == 0)
					{
						this.m_firstCell = value;
					}
					base[index] = value;
				}
			}

			internal void Add(int key, RuntimeTablixCell cell)
			{
				if (this.m_lastCell != null)
				{
					this.m_lastCell.NextCell = cell;
				}
				else
				{
					this.m_firstCell = cell;
				}
				this.m_lastCell = cell;
				base.Add(key, cell);
			}

			internal RuntimeTablixCell GetCell(Tablix tablixDef, RuntimeTablixGroupLeafObj owner, int cellLevel)
			{
				RuntimeTablixGroupRootObj currentOuterHeadingGroupRoot = tablixDef.CurrentOuterHeadingGroupRoot;
				int index = tablixDef.OuterGroupingIndexes[currentOuterHeadingGroupRoot.HeadingLevel];
				RuntimeTablixCell runtimeTablixCell = this[index];
				if (runtimeTablixCell == null)
				{
					runtimeTablixCell = (this[index] = owner.CreateCell(cellLevel, tablixDef));
				}
				return runtimeTablixCell;
			}

			internal void SortAndFilter()
			{
				for (RuntimeTablixCell runtimeTablixCell = this.m_firstCell; runtimeTablixCell != null; runtimeTablixCell = runtimeTablixCell.NextCell)
				{
					runtimeTablixCell.SortAndFilter();
				}
			}

			internal void CalculateRunningValues(Tablix tablixDef, AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup, RuntimeTablixGroupLeafObj owner, int cellLevel)
			{
				RuntimeTablixCell cell = this.GetCell(tablixDef, owner, cellLevel);
				Global.Tracer.Assert(null != cell);
				cell.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
			}
		}

		private abstract class RuntimeTablixCell : IScope
		{
			protected RuntimeTablixGroupLeafObj m_owner;

			protected int m_cellLevel;

			protected DataAggregateObjList m_cellNonCustomAggObjs;

			protected DataAggregateObjList m_cellCustomAggObjs;

			protected DataAggregateObjResult[] m_cellAggValueList;

			protected DataRowList m_dataRows;

			protected bool m_innermost;

			protected FieldImpl[] m_firstRow;

			protected bool m_firstRowIsAggregate;

			protected RuntimeTablixCell m_nextCell;

			internal RuntimeTablixCell NextCell
			{
				get
				{
					return this.m_nextCell;
				}
				set
				{
					this.m_nextCell = value;
				}
			}

			bool IScope.TargetForNonDetailSort
			{
				get
				{
					return this.m_owner.GetCellTargetForNonDetailSort();
				}
			}

			int[] IScope.SortFilterExpressionScopeInfoIndices
			{
				get
				{
					Global.Tracer.Assert(false, string.Empty);
					return null;
				}
			}

			internal RuntimeTablixCell(RuntimeTablixGroupLeafObj owner, int cellLevel, DataAggregateInfoList aggDefs, bool innermost)
			{
				this.m_owner = owner;
				this.m_cellLevel = cellLevel;
				RuntimeDataRegionObj.CreateAggregates(owner.ProcessingContext, aggDefs, ref this.m_cellNonCustomAggObjs, ref this.m_cellCustomAggObjs);
				DataAggregateObjList cellPostSortAggregates = this.m_owner.CellPostSortAggregates;
				if (cellPostSortAggregates != null && 0 < cellPostSortAggregates.Count)
				{
					this.m_cellAggValueList = new DataAggregateObjResult[cellPostSortAggregates.Count];
				}
				this.m_innermost = innermost;
			}

			internal virtual void NextRow()
			{
				RuntimeDataRegionObj.CommonFirstRow(this.m_owner.ProcessingContext.ReportObjectModel.FieldsImpl, ref this.m_firstRowIsAggregate, ref this.m_firstRow);
				this.NextAggregateRow();
				if (!this.m_owner.ProcessingContext.ReportObjectModel.FieldsImpl.IsAggregateRow)
				{
					this.NextNonAggregateRow();
				}
			}

			private void NextNonAggregateRow()
			{
				RuntimeDataRegionObj.UpdateAggregates(this.m_owner.ProcessingContext, this.m_cellNonCustomAggObjs, false);
				if (this.m_dataRows != null)
				{
					RuntimeDetailObj.SaveData(this.m_dataRows, this.m_owner.ProcessingContext);
				}
			}

			private void NextAggregateRow()
			{
				FieldsImpl fieldsImpl = this.m_owner.ProcessingContext.ReportObjectModel.FieldsImpl;
				if (fieldsImpl.ValidAggregateRow && fieldsImpl.AggregationFieldCount == 0 && this.m_cellCustomAggObjs != null)
				{
					RuntimeDataRegionObj.UpdateAggregates(this.m_owner.ProcessingContext, this.m_cellCustomAggObjs, false);
				}
			}

			internal virtual void SortAndFilter()
			{
			}

			internal virtual void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				if (this.m_dataRows != null)
				{
					Global.Tracer.Assert(this.m_innermost);
					this.ReadRows();
					this.m_dataRows = null;
				}
				DataAggregateObjList cellPostSortAggregates = this.m_owner.CellPostSortAggregates;
				if (cellPostSortAggregates != null && 0 < cellPostSortAggregates.Count)
				{
					for (int i = 0; i < cellPostSortAggregates.Count; i++)
					{
						this.m_cellAggValueList[i] = cellPostSortAggregates[i].AggregateResult();
						cellPostSortAggregates[i].Init();
					}
				}
			}

			private void ReadRows()
			{
				for (int i = 0; i < this.m_dataRows.Count; i++)
				{
					FieldImpl[] fields = this.m_dataRows[i];
					this.m_owner.ProcessingContext.ReportObjectModel.FieldsImpl.SetFields(fields);
					((IScope)this).ReadRow(DataActions.PostSortAggregates);
				}
			}

			protected void SetupAggregates(DataAggregateObjList aggregates, DataAggregateObjResult[] aggValues)
			{
				if (aggregates != null)
				{
					for (int i = 0; i < aggregates.Count; i++)
					{
						DataAggregateObj dataAggregateObj = aggregates[i];
						this.m_owner.ProcessingContext.ReportObjectModel.AggregatesImpl.Set(dataAggregateObj.Name, dataAggregateObj.AggregateDef, dataAggregateObj.DuplicateNames, (aggValues == null) ? dataAggregateObj.AggregateResult() : aggValues[i]);
					}
				}
			}

			protected void SetupEnvironment()
			{
				this.SetupAggregates(this.m_cellNonCustomAggObjs, null);
				this.SetupAggregates(this.m_cellCustomAggObjs, null);
				this.SetupAggregates(this.m_owner.CellPostSortAggregates, this.m_cellAggValueList);
				this.m_owner.ProcessingContext.ReportObjectModel.FieldsImpl.SetFields(this.m_firstRow);
				this.m_owner.ProcessingContext.ReportRuntime.CurrentScope = this;
			}

			bool IScope.IsTargetForSort(int index, bool detailSort)
			{
				return this.m_owner.GetCellTargetForSort(index, detailSort);
			}

			string IScope.GetScopeName()
			{
				return null;
			}

			IScope IScope.GetOuterScope(bool includeSubReportContainingScope)
			{
				return this.m_owner;
			}

			void IScope.ReadRow(DataActions dataAction)
			{
				this.m_owner.ReadRow(dataAction);
			}

			bool IScope.InScope(string scope)
			{
				if (this.m_owner.InScope(scope))
				{
					return true;
				}
				Hashtable outerScopeNames = this.GetOuterScopeNames();
				return outerScopeNames.Contains(scope);
			}

			int IScope.RecursiveLevel(string scope)
			{
				if (scope == null)
				{
					return 0;
				}
				int num = ((IScope)this.m_owner).RecursiveLevel(scope);
				if (-1 != num)
				{
					return num;
				}
				Hashtable outerScopeNames = this.GetOuterScopeNames();
				Grouping grouping = outerScopeNames[scope] as Grouping;
				if (grouping != null)
				{
					return grouping.RecursiveLevel;
				}
				return -1;
			}

			private Hashtable GetOuterScopeNames()
			{
				Global.Tracer.Assert(null != this.m_owner.TablixHeadingDef);
				TablixHeading tablixHeadingDef = this.m_owner.TablixHeadingDef;
				Tablix tablix = (Tablix)tablixHeadingDef.DataRegionDef;
				Hashtable hashtable = null;
				if (tablixHeadingDef.CellScopeNames == null)
				{
					tablixHeadingDef.CellScopeNames = new Hashtable[tablix.GetDynamicHeadingCount(true)];
				}
				else
				{
					hashtable = tablixHeadingDef.CellScopeNames[this.m_cellLevel];
				}
				if (hashtable == null)
				{
					hashtable = tablix.GetOuterScopeNames(this.m_cellLevel);
					tablixHeadingDef.CellScopeNames[this.m_cellLevel] = hashtable;
				}
				return hashtable;
			}

			bool IScope.TargetScopeMatched(int index, bool detailSort)
			{
				if (this.m_owner.TargetScopeMatched(index, detailSort))
				{
					Hashtable outerScopeNames = this.GetOuterScopeNames();
					IDictionaryEnumerator enumerator = outerScopeNames.GetEnumerator();
					while (enumerator.MoveNext())
					{
						Grouping grouping = (Grouping)enumerator.Value;
						if (detailSort && grouping.SortFilterScopeInfo == null)
						{
							continue;
						}
						if (grouping.SortFilterScopeMatched == null || !grouping.SortFilterScopeMatched[index])
						{
							return false;
						}
					}
					return true;
				}
				return false;
			}

			void IScope.GetScopeValues(IHierarchyObj targetScopeObj, VariantList[] scopeValues, ref int index)
			{
				((RuntimeDataRegionObj)this.m_owner).GetScopeValues(targetScopeObj, scopeValues, ref index);
				Global.Tracer.Assert(this.m_innermost);
				Hashtable outerScopeNames = this.GetOuterScopeNames();
				IDictionaryEnumerator enumerator = outerScopeNames.GetEnumerator();
				while (enumerator.MoveNext())
				{
					Grouping grouping = (Grouping)enumerator.Value;
					Global.Tracer.Assert(index < scopeValues.Length);
					scopeValues[index++] = grouping.CurrentGroupExpressionValues;
				}
				Global.Tracer.Assert(this.m_owner.TablixDef.GetDynamicHeadingCount(true) == outerScopeNames.Count);
			}

			void IScope.GetGroupNameValuePairs(Dictionary<string, object> pairs)
			{
				((IScope)this.m_owner).GetGroupNameValuePairs(pairs);
				Hashtable outerScopeNames = this.GetOuterScopeNames();
				if (outerScopeNames != null)
				{
					IEnumerator enumerator = outerScopeNames.Values.GetEnumerator();
					while (enumerator.MoveNext())
					{
						RuntimeDataRegionObj.AddGroupNameValuePair(this.m_owner.ProcessingContext, enumerator.Current as Grouping, pairs);
					}
				}
			}
		}

		private sealed class RuntimeCustomReportItemCell : RuntimeTablixCell
		{
			private DataAggregateObjResult[] m_runningValueValues;

			internal RuntimeCustomReportItemCell(RuntimeCustomReportItemGroupLeafObj owner, int cellLevel, DataAggregateInfoList aggDefs, DataCellsList dataRowCells, bool innermost)
				: base(owner, cellLevel, aggDefs, innermost)
			{
				CustomReportItem customReportItem = (CustomReportItem)owner.TablixDef;
				DataActions dataActions = DataActions.None;
				bool flag = customReportItem.CellRunningValues != null && 0 < customReportItem.CellRunningValues.Count;
				if (base.m_innermost && (flag || base.m_owner.CellPostSortAggregates != null))
				{
					dataActions = DataActions.PostSortAggregates;
				}
				if (dataActions != 0)
				{
					base.m_dataRows = new DataRowList();
				}
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				base.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				RuntimeRICollection.DoneReadingRows(globalRVCol, base.m_owner.TablixDef.TablixCellRunningValues, ref this.m_runningValueValues, false);
			}

			internal void CreateInstance(CustomReportItemInstance criInstance)
			{
				base.SetupEnvironment();
				RuntimeDataRegionObj.SetupRunningValues(base.m_owner.ProcessingContext, base.m_owner.TablixDef.TablixCellRunningValues, this.m_runningValueValues);
				criInstance.AddCell(base.m_owner.ProcessingContext);
			}
		}

		private sealed class RuntimeCustomReportItemObj : RuntimeTablixObj
		{
			private bool m_subtotalCorner;

			internal RuntimeCustomReportItemObj(IScope outerScope, CustomReportItem crItem, ref DataActions dataAction, ProcessingContext processingContext, bool onePassProcess)
				: base(outerScope, (Tablix)crItem, ref dataAction, processingContext, onePassProcess)
			{
				bool handleMyDataAction = default(bool);
				DataActions innerDataAction = default(DataActions);
				TablixHeadingList tablixHeadingList = default(TablixHeadingList);
				TablixHeadingList tablixHeadingList2 = default(TablixHeadingList);
				TablixHeadingList staticColumns = default(TablixHeadingList);
				TablixHeadingList staticRows = default(TablixHeadingList);
				base.ConstructorHelper(ref dataAction, onePassProcess, out handleMyDataAction, out innerDataAction, out tablixHeadingList, out tablixHeadingList2, out staticColumns, out staticRows);
				base.m_innerDataAction = innerDataAction;
				this.CRIConstructRuntimeStructure(ref innerDataAction, onePassProcess, tablixHeadingList, tablixHeadingList2, staticColumns, staticRows);
				if (onePassProcess || (tablixHeadingList2 == null && tablixHeadingList == null))
				{
					this.m_subtotalCorner = true;
				}
				base.HandleDataAction(handleMyDataAction, innerDataAction);
			}

			protected override void ConstructRuntimeStructure(ref DataActions innerDataAction)
			{
				TablixHeadingList outermostColumns = default(TablixHeadingList);
				TablixHeadingList outermostRows = default(TablixHeadingList);
				TablixHeadingList staticColumns = default(TablixHeadingList);
				TablixHeadingList staticRows = default(TablixHeadingList);
				base.m_tablixDef.GetHeadingDefState(out outermostColumns, out outermostRows, out staticColumns, out staticRows);
				this.CRIConstructRuntimeStructure(ref innerDataAction, false, outermostColumns, outermostRows, staticColumns, staticRows);
			}

			private void CRIConstructRuntimeStructure(ref DataActions innerDataAction, bool onePassProcess, TablixHeadingList outermostColumns, TablixHeadingList outermostRows, TablixHeadingList staticColumns, TablixHeadingList staticRows)
			{
				DataActions dataActions = DataActions.None;
				if (base.m_tablixDef.ProcessingInnerGrouping == Pivot.ProcessingInnerGroupings.Column)
				{
					base.m_innerGroupings = (base.m_tablixColumns = new RuntimeCustomReportItemHeadingsObj(this, (CustomReportItemHeadingList)outermostColumns, ref dataActions, base.m_processingContext, (CustomReportItemHeadingList)staticColumns, null, null == outermostRows, 0));
					base.m_outerGroupings = (base.m_tablixRows = new RuntimeCustomReportItemHeadingsObj((IScope)this, (CustomReportItemHeadingList)outermostRows, ref innerDataAction, base.m_processingContext, (CustomReportItemHeadingList)staticRows, (RuntimeCustomReportItemHeadingsObj)base.m_innerGroupings, null == outermostColumns, 0));
				}
				else
				{
					base.m_innerGroupings = (base.m_tablixRows = new RuntimeCustomReportItemHeadingsObj(this, (CustomReportItemHeadingList)outermostRows, ref dataActions, base.m_processingContext, (CustomReportItemHeadingList)staticRows, null, null == outermostColumns, 0));
					base.m_outerGroupings = (base.m_tablixColumns = new RuntimeCustomReportItemHeadingsObj((IScope)this, (CustomReportItemHeadingList)outermostColumns, ref innerDataAction, base.m_processingContext, (CustomReportItemHeadingList)staticColumns, (RuntimeCustomReportItemHeadingsObj)base.m_innerGroupings, null == outermostRows, 0));
				}
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				base.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				RuntimeRICollection.DoneReadingRows(globalRVCol, base.m_tablixDef.RunningValues, ref base.m_runningValueValues, false);
			}

			internal override void CreateInstances(ReportItemInstance riInstance, IList instanceList, RenderingPagesRangesList pagesList)
			{
				if (base.m_firstRow != null)
				{
					CustomReportItem customReportItem = (CustomReportItem)base.m_tablixDef;
					CustomReportItemInstance customReportItemInstance = (CustomReportItemInstance)riInstance;
					if (base.m_outerGroupings == base.m_tablixRows)
					{
						customReportItemInstance.InnerHeadingInstanceList = customReportItemInstance.ColumnInstances;
						((RuntimeCustomReportItemHeadingsObj)base.m_outerGroupings).CreateInstances(this, base.m_processingContext, customReportItemInstance, true, null, customReportItemInstance.RowInstances);
					}
					else
					{
						customReportItemInstance.InnerHeadingInstanceList = customReportItemInstance.RowInstances;
						((RuntimeCustomReportItemHeadingsObj)base.m_outerGroupings).CreateInstances(this, base.m_processingContext, customReportItemInstance, true, null, customReportItemInstance.ColumnInstances);
					}
				}
			}

			internal void CreateOutermostSubtotalCells(CustomReportItemInstance criInstance, bool outerGroupings)
			{
				if (outerGroupings)
				{
					this.SetupEnvironment();
					((RuntimeCustomReportItemHeadingsObj)base.m_innerGroupings).CreateInstances(this, base.m_processingContext, criInstance, false, null, criInstance.InnerHeadingInstanceList);
				}
				else if (this.m_subtotalCorner)
				{
					this.SetupEnvironment();
					criInstance.AddCell(base.m_processingContext);
				}
			}
		}

		private sealed class RuntimeCustomReportItemHeadingsObj : RuntimeTablixHeadingsObj
		{
			private DataAggregateObjResult[] m_runningValueValues;

			internal RuntimeCustomReportItemHeadingsObj(IScope owner, CustomReportItemHeadingList headingDef, ref DataActions dataAction, ProcessingContext processingContext, CustomReportItemHeadingList staticHeadingDef, RuntimeCustomReportItemHeadingsObj innerGroupings, bool outermostSubtotal, int headingLevel)
				: base(owner, (TablixHeadingList)headingDef, ref dataAction, processingContext, (TablixHeadingList)staticHeadingDef, (RuntimeTablixHeadingsObj)innerGroupings, headingLevel)
			{
				if (headingDef != null)
				{
					base.m_tablixHeadings = new RuntimeCustomReportItemGroupRootObj(owner, headingDef, 0, ref dataAction, processingContext, innerGroupings, outermostSubtotal, headingLevel);
				}
			}

			internal override void CalculatePreviousAggregates(AggregatesImpl globalRVCol)
			{
				if (base.m_staticHeadingDef != null)
				{
					for (int i = 0; i < base.m_staticHeadingDef.Count; i++)
					{
						RuntimeRICollection.DoneReadingRows(globalRVCol, ((CustomReportItemHeading)base.m_staticHeadingDef[i]).RunningValues, ref this.m_runningValueValues, true);
					}
				}
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				base.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				if (base.m_staticHeadingDef != null && base.m_owner is RuntimeCustomReportItemGroupLeafObj)
				{
					for (int i = 0; i < base.m_staticHeadingDef.Count; i++)
					{
						RuntimeRICollection.DoneReadingRows(globalRVCol, ((CustomReportItemHeading)base.m_staticHeadingDef[i]).RunningValues, ref this.m_runningValueValues, false);
					}
				}
			}

			private void SetupEnvironment(ProcessingContext processingContext)
			{
				if (base.m_staticHeadingDef != null && this.m_runningValueValues != null)
				{
					for (int i = 0; i < base.m_staticHeadingDef.Count; i++)
					{
						RuntimeDataRegionObj.SetupRunningValues(processingContext, ((CustomReportItemHeading)base.m_staticHeadingDef[i]).RunningValues, this.m_runningValueValues);
					}
				}
			}

			internal void CreateInstances(RuntimeDataRegionObj outerGroup, ProcessingContext processingContext, CustomReportItemInstance criInstance, bool outerGroupings, RuntimeTablixGroupRootObj currOuterHeadingGroupRoot, CustomReportItemHeadingInstanceList headingInstances)
			{
				bool flag = outerGroupings || 0 == criInstance.CurrentCellOuterIndex;
				CustomReportItemHeadingList customReportItemHeadingList = base.m_staticHeadingDef as CustomReportItemHeadingList;
				this.SetupEnvironment(processingContext);
				int num = (customReportItemHeadingList == null) ? 1 : customReportItemHeadingList.Count;
				CustomReportItemHeadingInstanceList customReportItemHeadingInstanceList = headingInstances;
				for (int i = 0; i < num; i++)
				{
					if (customReportItemHeadingList != null)
					{
						if (flag)
						{
							customReportItemHeadingInstanceList = this.CreateHeadingInstance(processingContext, criInstance, customReportItemHeadingList, headingInstances, outerGroupings, i);
						}
						if (outerGroupings)
						{
							criInstance.CurrentOuterStaticIndex = i;
						}
						else
						{
							criInstance.CurrentInnerStaticIndex = i;
						}
					}
					if (base.m_tablixHeadings != null)
					{
						CustomReportItem customReportItem = (CustomReportItem)base.m_tablixHeadings.HierarchyDef.DataRegionDef;
						customReportItem.CurrentOuterHeadingGroupRoot = currOuterHeadingGroupRoot;
						base.m_tablixHeadings.CreateInstances(criInstance, customReportItemHeadingInstanceList, null);
						if (flag)
						{
							this.SetHeadingSpan(criInstance, customReportItemHeadingInstanceList, outerGroupings, processingContext);
						}
					}
					else if (outerGroup is RuntimeCustomReportItemGroupLeafObj)
					{
						RuntimeCustomReportItemGroupLeafObj runtimeCustomReportItemGroupLeafObj = (RuntimeCustomReportItemGroupLeafObj)outerGroup;
						if (!outerGroupings && runtimeCustomReportItemGroupLeafObj.IsOuterGrouping())
						{
							runtimeCustomReportItemGroupLeafObj.CreateSubtotalOrStaticCells(criInstance, currOuterHeadingGroupRoot, outerGroupings);
						}
						else
						{
							runtimeCustomReportItemGroupLeafObj.CreateInnerGroupingsOrCells(criInstance, currOuterHeadingGroupRoot);
						}
					}
					else
					{
						((RuntimeCustomReportItemObj)outerGroup).CreateOutermostSubtotalCells(criInstance, outerGroupings);
					}
				}
				if (customReportItemHeadingList != null && flag)
				{
					this.SetHeadingSpan(criInstance, headingInstances, outerGroupings, processingContext);
				}
			}

			private void SetHeadingSpan(CustomReportItemInstance criInstance, CustomReportItemHeadingInstanceList headingInstances, bool outerGroupings, ProcessingContext processingContext)
			{
				int currentCellIndex = (!outerGroupings) ? criInstance.CurrentCellInnerIndex : (criInstance.CurrentCellOuterIndex + 1);
				headingInstances.SetLastHeadingSpan(currentCellIndex, processingContext);
			}

			private CustomReportItemHeadingInstanceList CreateHeadingInstance(ProcessingContext processingContext, CustomReportItemInstance criInstance, CustomReportItemHeadingList headingDef, CustomReportItemHeadingInstanceList headingInstances, bool outerGroupings, int headingIndex)
			{
				CustomReportItemHeadingInstance customReportItemHeadingInstance = null;
				int headingCellIndex;
				if (outerGroupings)
				{
					criInstance.NewOuterCells();
					headingCellIndex = criInstance.CurrentCellOuterIndex;
				}
				else
				{
					headingCellIndex = criInstance.CurrentCellInnerIndex;
				}
				customReportItemHeadingInstance = new CustomReportItemHeadingInstance(processingContext, headingCellIndex, headingDef[headingIndex], null, 0);
				headingInstances.Add(customReportItemHeadingInstance, processingContext);
				return customReportItemHeadingInstance.SubHeadingInstances;
			}
		}

		private sealed class RuntimeCustomReportItemGroupRootObj : RuntimeTablixGroupRootObj
		{
			internal RuntimeCustomReportItemGroupRootObj(IScope outerScope, CustomReportItemHeadingList headingDef, int headingIndex, ref DataActions dataAction, ProcessingContext processingContext, RuntimeCustomReportItemHeadingsObj innerGroupings, bool outermostSubtotal, int headingLevel)
				: base(outerScope, (TablixHeadingList)headingDef, headingIndex, ref dataAction, processingContext, (RuntimeTablixHeadingsObj)innerGroupings, outermostSubtotal, headingLevel)
			{
				Global.Tracer.Assert(0 == headingIndex);
				if (base.m_processOutermostSTCells)
				{
					CustomReportItem customReportItem = (CustomReportItem)headingDef[headingIndex].DataRegionDef;
					if (customReportItem.CellRunningValues != null && 0 < customReportItem.CellRunningValues.Count)
					{
						base.m_dataAction |= DataActions.PostSortAggregates;
					}
				}
				base.m_saveGroupExprValues = true;
			}

			protected override void NeedProcessDataActions(TablixHeadingList heading)
			{
				CustomReportItemHeadingList customReportItemHeadingList = (CustomReportItemHeadingList)heading;
				if (customReportItemHeadingList != null)
				{
					base.NeedProcessDataActions(customReportItemHeadingList[base.m_headingIndex].RunningValues);
				}
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				base.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				base.AddRunningValues(((CustomReportItemHeading)base.m_hierarchyDef).RunningValues);
				if (base.m_staticHeadingDef != null)
				{
					for (int i = 0; i < base.m_staticHeadingDef.Count; i++)
					{
						base.AddRunningValues(((CustomReportItemHeading)base.m_staticHeadingDef[i]).RunningValues);
					}
				}
				base.m_grouping.Traverse(ProcessingStages.RunningValues, base.m_expression.Direction);
				if (base.m_hierarchyDef.Grouping.Name != null)
				{
					groupCol.Remove(base.m_hierarchyDef.Grouping.Name);
				}
			}

			protected override void AddCellRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, ref DataAggregateObjList runningValues)
			{
				CustomReportItem customReportItem = (CustomReportItem)base.m_hierarchyDef.DataRegionDef;
				if (customReportItem.CellRunningValues != null && 0 < customReportItem.CellRunningValues.Count)
				{
					base.m_dataAction |= DataActions.PostSortAggregates;
					if (runningValues == null)
					{
						base.AddRunningValues(customReportItem.CellRunningValues, ref runningValues, globalRVCol, groupCol);
					}
				}
			}
		}

		private sealed class RuntimeCustomReportItemGroupLeafObj : RuntimeTablixGroupLeafObj
		{
			private DataAggregateObjResult[] m_runningValueValues;

			private DataAggregateObjResult[] m_cellRunningValueValues;

			internal RuntimeCustomReportItemGroupLeafObj(RuntimeCustomReportItemGroupRootObj groupRoot)
				: base(groupRoot)
			{
				CustomReportItemHeading customReportItemHeading = (CustomReportItemHeading)groupRoot.HierarchyDef;
				CustomReportItem tablixDef = (CustomReportItem)customReportItemHeading.DataRegionDef;
				CustomReportItemHeadingList headingDef = (CustomReportItemHeadingList)groupRoot.InnerHeading;
				bool flag = false;
				bool flag2 = base.HandleSortFilterEvent();
				DataActions dataAction = default(DataActions);
				base.ConstructorHelper((RuntimeTablixGroupRootObj)groupRoot, (Tablix)tablixDef, out flag, out dataAction);
				base.m_tablixHeadings = new RuntimeCustomReportItemHeadingsObj(this, headingDef, ref dataAction, groupRoot.ProcessingContext, (CustomReportItemHeadingList)groupRoot.StaticHeadingDef, (RuntimeCustomReportItemHeadingsObj)groupRoot.InnerGroupings, groupRoot.OutermostSubtotal, groupRoot.HeadingLevel + 1);
				base.m_innerHeadingList = base.m_tablixHeadings.Headings;
				if (!flag)
				{
					base.m_dataAction = dataAction;
				}
				if (flag2)
				{
					base.m_dataAction |= DataActions.UserSort;
				}
				if (base.m_dataAction != 0)
				{
					base.m_dataRows = new DataRowList();
				}
			}

			internal override RuntimeTablixCell CreateCell(int index, Tablix tablixDef)
			{
				return new RuntimeCustomReportItemCell(this, index, tablixDef.CellAggregates, ((CustomReportItem)tablixDef).DataRowCells, null == base.m_innerHeadingList);
			}

			internal override void CalculateRunningValues()
			{
				base.CalculateRunningValues();
				if (base.m_processHeading)
				{
					RuntimeTablixGroupRootObj runtimeTablixGroupRootObj = (RuntimeTablixGroupRootObj)base.m_hierarchyRoot;
					AggregatesImpl globalRunningValueCollection = runtimeTablixGroupRootObj.GlobalRunningValueCollection;
					RuntimeGroupRootObjList groupCollection = runtimeTablixGroupRootObj.GroupCollection;
					RuntimeRICollection.DoneReadingRows(globalRunningValueCollection, ((CustomReportItemHeading)runtimeTablixGroupRootObj.HierarchyDef).RunningValues, ref this.m_runningValueValues, false);
					if (runtimeTablixGroupRootObj.ProcessOutermostSTCells)
					{
						RuntimeRICollection.DoneReadingRows(runtimeTablixGroupRootObj.OutermostSTCellRVCol, ((CustomReportItem)base.TablixDef).CellRunningValues, ref this.m_cellRunningValueValues, false);
					}
					base.m_processHeading = false;
				}
				this.ResetScopedRunningValues();
			}

			internal override void CreateInstance()
			{
				this.SetupEnvironment();
				RuntimeCustomReportItemGroupRootObj runtimeCustomReportItemGroupRootObj = (RuntimeCustomReportItemGroupRootObj)base.m_hierarchyRoot;
				CustomReportItem customReportItem = (CustomReportItem)base.TablixDef;
				CustomReportItemInstance customReportItemInstance = (CustomReportItemInstance)runtimeCustomReportItemGroupRootObj.ReportItemInstance;
				CustomReportItemHeadingInstanceList customReportItemHeadingInstanceList = (CustomReportItemHeadingInstanceList)runtimeCustomReportItemGroupRootObj.InstanceList;
				CustomReportItemHeading customReportItemHeading = (CustomReportItemHeading)runtimeCustomReportItemGroupRootObj.HierarchyDef;
				bool flag = base.IsOuterGrouping();
				base.SetupRunningValues(customReportItemHeading.RunningValues, this.m_runningValueValues);
				if (this.m_cellRunningValueValues != null)
				{
					base.SetupRunningValues(customReportItem.CellRunningValues, this.m_cellRunningValueValues);
				}
				RuntimeTablixGroupRootObj currOuterHeadingGroupRoot;
				int headingCellIndex;
				if (flag)
				{
					currOuterHeadingGroupRoot = (customReportItem.CurrentOuterHeadingGroupRoot = runtimeCustomReportItemGroupRootObj);
					customReportItem.OuterGroupingIndexes[runtimeCustomReportItemGroupRootObj.HeadingLevel] = base.m_groupLeafIndex;
					customReportItemInstance.NewOuterCells();
					headingCellIndex = customReportItemInstance.CurrentCellOuterIndex;
				}
				else
				{
					currOuterHeadingGroupRoot = customReportItem.CurrentOuterHeadingGroupRoot;
					headingCellIndex = customReportItemInstance.CurrentCellInnerIndex;
				}
				if (flag || customReportItemInstance.CurrentCellOuterIndex == 0)
				{
					CustomReportItemHeadingInstance customReportItemHeadingInstance = new CustomReportItemHeadingInstance(base.m_processingContext, headingCellIndex, customReportItemHeading, base.m_groupExprValues, base.m_recursiveLevel);
					customReportItemHeadingInstanceList.Add(customReportItemHeadingInstance, base.m_processingContext);
					customReportItemHeadingInstanceList = customReportItemHeadingInstance.SubHeadingInstances;
				}
				((RuntimeCustomReportItemHeadingsObj)base.m_tablixHeadings).CreateInstances(this, base.m_processingContext, customReportItemInstance, flag, currOuterHeadingGroupRoot, customReportItemHeadingInstanceList);
			}

			internal void CreateInnerGroupingsOrCells(CustomReportItemInstance criInstance, RuntimeTablixGroupRootObj currOuterHeadingGroupRoot)
			{
				this.SetupEnvironment();
				if (base.IsOuterGrouping())
				{
					RuntimeCustomReportItemHeadingsObj runtimeCustomReportItemHeadingsObj = (RuntimeCustomReportItemHeadingsObj)((RuntimeCustomReportItemGroupRootObj)base.m_hierarchyRoot).InnerGroupings;
					runtimeCustomReportItemHeadingsObj.CreateInstances(this, base.m_processingContext, criInstance, false, currOuterHeadingGroupRoot, criInstance.InnerHeadingInstanceList);
				}
				else if (currOuterHeadingGroupRoot == null)
				{
					this.CreateOutermostSubtotalCell(criInstance);
				}
				else
				{
					this.CreateCellInstance(criInstance, currOuterHeadingGroupRoot);
				}
			}

			private void CreateCellInstance(CustomReportItemInstance criInstance, RuntimeTablixGroupRootObj currOuterHeadingGroupRoot)
			{
				Global.Tracer.Assert(base.m_cellsList != null && null != base.m_cellsList[currOuterHeadingGroupRoot.HeadingLevel]);
				RuntimeCustomReportItemCell runtimeCustomReportItemCell = (RuntimeCustomReportItemCell)base.m_cellsList[currOuterHeadingGroupRoot.HeadingLevel].GetCell(base.TablixDef, this, currOuterHeadingGroupRoot.HeadingLevel);
				Global.Tracer.Assert(null != runtimeCustomReportItemCell);
				runtimeCustomReportItemCell.CreateInstance(criInstance);
			}

			private void CreateOutermostSubtotalCell(CustomReportItemInstance criInstance)
			{
				this.SetupEnvironment();
				criInstance.AddCell(base.m_processingContext);
			}

			internal void CreateSubtotalOrStaticCells(CustomReportItemInstance criInstance, RuntimeTablixGroupRootObj currOuterHeadingGroupRoot, bool outerGroupingSubtotal)
			{
				RuntimeCustomReportItemHeadingsObj runtimeCustomReportItemHeadingsObj = (RuntimeCustomReportItemHeadingsObj)((RuntimeCustomReportItemGroupRootObj)base.m_hierarchyRoot).InnerGroupings;
				if (base.IsOuterGrouping() && !outerGroupingSubtotal)
				{
					this.CreateOutermostSubtotalCell(criInstance);
				}
				else
				{
					this.CreateInnerGroupingsOrCells(criInstance, currOuterHeadingGroupRoot);
				}
			}
		}

		internal const int MaximumChartThreads = 5;

		private IConfiguration m_configuration;

		internal IConfiguration Configuration
		{
			get
			{
				return this.m_configuration;
			}
			set
			{
				this.m_configuration = value;
			}
		}

		public bool ProcessToggleEvent(string showHideToggle, IChunkFactory getReportChunkFactory, EventInformation oldShowHideInfo, out EventInformation newShowHideInfo, out bool showHideInfoChanged)
		{
			newShowHideInfo = null;
			showHideInfoChanged = false;
			if (getReportChunkFactory == null)
			{
				return false;
			}
			if (showHideToggle == null)
			{
				return false;
			}
			if (ReportProcessing.ContainsFlag(getReportChunkFactory.ReportProcessingFlags, ReportProcessingFlags.OnDemandEngine))
			{
				return ReportProcessing.ProcessOdpToggleEvent(showHideToggle, getReportChunkFactory, oldShowHideInfo, out newShowHideInfo, out showHideInfoChanged);
			}
			ChunkFactoryAdapter @object = new ChunkFactoryAdapter(getReportChunkFactory);
			return this.ProcessYukonToggleEvent(showHideToggle, (GetReportChunk)@object.GetReportChunk, oldShowHideInfo, out newShowHideInfo, out showHideInfoChanged);
		}

		internal static bool ProcessOdpToggleEvent(string showHideToggle, IChunkFactory getReportChunkFactory, EventInformation oldShowHideInfo, out EventInformation newShowHideInfo, out bool showHideInfoChanged)
		{
			newShowHideInfo = null;
			showHideInfoChanged = false;
			if (showHideToggle != null && oldShowHideInfo != null && oldShowHideInfo.ValidToggleSender(showHideToggle))
			{
				newShowHideInfo = new EventInformation(oldShowHideInfo);
				showHideInfoChanged = true;
				if (newShowHideInfo.ToggleStateInfo == null)
				{
					newShowHideInfo.ToggleStateInfo = new Hashtable();
				}
				Hashtable toggleStateInfo = newShowHideInfo.ToggleStateInfo;
				if (toggleStateInfo.ContainsKey(showHideToggle))
				{
					toggleStateInfo.Remove(showHideToggle);
				}
				else
				{
					toggleStateInfo.Add(showHideToggle, null);
				}
				return true;
			}
			return false;
		}

		private bool ProcessYukonToggleEvent(string showHideToggle, GetReportChunk getReportChunk, EventInformation oldShowHideInfo, out EventInformation newShowHideInfo, out bool showHideInfoChanged)
		{
			newShowHideInfo = null;
			showHideInfoChanged = false;
			ChunkManager.EventsChunkManager eventsChunkManager = null;
			try
			{
				eventsChunkManager = new ChunkManager.EventsChunkManager(getReportChunk);
				ShowHideProcessing showHideProcessing = new ShowHideProcessing();
				return showHideProcessing.Process(showHideToggle, oldShowHideInfo, eventsChunkManager, out showHideInfoChanged, out newShowHideInfo);
			}
			finally
			{
				if (eventsChunkManager != null)
				{
					eventsChunkManager.Close();
				}
			}
		}

		public int ProcessFindStringEvent(int startPage, int endPage, string findValue, EventInformation eventInfo, AspNetCore.ReportingServices.ReportProcessing.ProcessingContext processingContext, out OnDemandProcessingResult result)
		{
			result = null;
			if (findValue != null && processingContext != null && startPage > 0 && endPage > 0)
			{
				if (ReportProcessing.ContainsFlag(processingContext.ReportProcessingFlags, ReportProcessingFlags.OnDemandEngine))
				{
					return this.ProcessOdpFindStringEvent(startPage, endPage, findValue, eventInfo, processingContext, out result);
				}
				return this.ProcessYukonFindStringEvent(startPage, endPage, findValue, processingContext, eventInfo);
			}
			return 0;
		}

		private int ProcessOdpFindStringEvent(int startPage, int endPage, string findValue, EventInformation eventInfo, AspNetCore.ReportingServices.ReportProcessing.ProcessingContext processingContext, out OnDemandProcessingResult result)
		{
			ExecutionLogContext executionLogContext = new ExecutionLogContext(processingContext.JobContext);
			ProcessingErrorContext errorContext = null;
			OnDemandProcessingContext odpContext = null;
			AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext odpRenderingContext = null;
			AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSnapshot odpReportSnapshot = null;
			AspNetCore.ReportingServices.OnDemandReportRendering.Report report = null;
			bool exceptionGenerated = false;
			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			try
			{
				report = this.GenerateEventROM(processingContext, (OnDemandMetadata)null, eventInfo, executionLogContext, out errorContext, out odpContext, out odpRenderingContext, out odpReportSnapshot);
				IInteractivityPaginationModule interactivityPaginationModule = InteractivityPaginationModuleFactory.CreateInteractivityPaginationModule();
				return interactivityPaginationModule.ProcessFindStringEvent(report, -1, startPage, endPage, findValue);
			}
			catch (Exception exception)
			{
				exceptionGenerated = true;
				Exception ex = default(Exception);
				if (this.NeedWrapperException(exception, errorContext, out ex))
				{
					throw ex;
				}
				throw;
			}
			finally
			{
				result = this.CleanupEventROM(processingContext, executionLogContext, errorContext, odpContext, odpRenderingContext, odpReportSnapshot, exceptionGenerated);
				if (currentCulture != null)
				{
					Thread.CurrentThread.CurrentCulture = currentCulture;
				}
			}
		}

		private int ProcessYukonFindStringEvent(int startPage, int endPage, string findValue, AspNetCore.ReportingServices.ReportProcessing.ProcessingContext pc, EventInformation eventInfo)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.Report report = null;
			try
			{
				ReportProcessing.GenerateEventShimROM(pc.ChunkFactory, eventInfo, pc, out report);
				IInteractivityPaginationModule interactivityPaginationModule = InteractivityPaginationModuleFactory.CreateInteractivityPaginationModule();
				return interactivityPaginationModule.ProcessFindStringEvent(report, -1, startPage, endPage, findValue);
			}
			finally
			{
				if (report != null)
				{
					report.RenderingContext.CloseRenderingChunkManager();
				}
			}
		}

		public int ProcessBookmarkNavigationEvent(string bookmarkId, EventInformation eventInfo, AspNetCore.ReportingServices.ReportProcessing.ProcessingContext processingContext, out string uniqueName, out OnDemandProcessingResult result)
		{
			uniqueName = null;
			result = null;
			if (processingContext != null && bookmarkId != null)
			{
				if (ReportProcessing.ContainsFlag(processingContext.ReportProcessingFlags, ReportProcessingFlags.OnDemandEngine))
				{
					return this.ProcessOdpBookmarkNavigationEvent(bookmarkId, eventInfo, processingContext, out uniqueName, out result);
				}
				result = null;
				return this.ProcessYukonBookmarkNavigationEvent(bookmarkId, processingContext, out uniqueName);
			}
			return 0;
		}

		private int ProcessOdpBookmarkNavigationEvent(string bookmarkId, EventInformation eventInfo, AspNetCore.ReportingServices.ReportProcessing.ProcessingContext processingContext, out string uniqueName, out OnDemandProcessingResult result)
		{
			uniqueName = null;
			result = null;
			ExecutionLogContext executionLogContext = new ExecutionLogContext(processingContext.JobContext);
			ProcessingErrorContext errorContext = null;
			OnDemandProcessingContext odpContext = null;
			AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext odpRenderingContext = null;
			AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSnapshot odpReportSnapshot = null;
			AspNetCore.ReportingServices.OnDemandReportRendering.Report report = null;
			bool exceptionGenerated = false;
			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			try
			{
				report = this.GenerateEventROM(processingContext, (OnDemandMetadata)null, eventInfo, executionLogContext, out errorContext, out odpContext, out odpRenderingContext, out odpReportSnapshot);
				IInteractivityPaginationModule interactivityPaginationModule = InteractivityPaginationModuleFactory.CreateInteractivityPaginationModule();
				return interactivityPaginationModule.ProcessBookmarkNavigationEvent(report, -1, bookmarkId, out uniqueName);
			}
			catch (Exception exception)
			{
				exceptionGenerated = true;
				Exception ex = default(Exception);
				if (this.NeedWrapperException(exception, errorContext, out ex))
				{
					throw ex;
				}
				throw;
			}
			finally
			{
				result = this.CleanupEventROM(processingContext, executionLogContext, errorContext, odpContext, odpRenderingContext, odpReportSnapshot, exceptionGenerated);
				if (currentCulture != null)
				{
					Thread.CurrentThread.CurrentCulture = currentCulture;
				}
			}
		}

		private int ProcessYukonBookmarkNavigationEvent(string bookmarkId, AspNetCore.ReportingServices.ReportProcessing.ProcessingContext pc, out string uniqueName)
		{
			uniqueName = null;
			AspNetCore.ReportingServices.OnDemandReportRendering.Report report = null;
			try
			{
				ReportProcessing.GenerateEventShimROM(pc.ChunkFactory, (EventInformation)null, pc, out report);
				IInteractivityPaginationModule interactivityPaginationModule = InteractivityPaginationModuleFactory.CreateInteractivityPaginationModule();
				return interactivityPaginationModule.ProcessBookmarkNavigationEvent(report, -1, bookmarkId, out uniqueName);
			}
			finally
			{
				if (report != null)
				{
					report.RenderingContext.CloseRenderingChunkManager();
				}
			}
		}

		public int ProcessDocumentMapNavigationEvent(string documentMapId, EventInformation eventInfo, AspNetCore.ReportingServices.ReportProcessing.ProcessingContext processingContext, out OnDemandProcessingResult result)
		{
			result = null;
			if (processingContext != null && documentMapId != null)
			{
				if (ReportProcessing.ContainsFlag(processingContext.ReportProcessingFlags, ReportProcessingFlags.OnDemandEngine))
				{
					return this.ProcessOdpDocumentMapNavigationEvent(documentMapId, eventInfo, processingContext, out result);
				}
				result = null;
				return this.ProcessYukonDocumentMapNavigationEvent(documentMapId, processingContext);
			}
			return 0;
		}

		private int ProcessOdpDocumentMapNavigationEvent(string documentMapId, EventInformation eventInfo, AspNetCore.ReportingServices.ReportProcessing.ProcessingContext processingContext, out OnDemandProcessingResult result)
		{
			result = null;
			OnDemandMetadata onDemandMetadata = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager.DeserializeOnDemandMetadata(processingContext.ChunkFactory, null);
			if (!onDemandMetadata.ReportSnapshot.HasDocumentMap)
			{
				return 0;
			}
			ExecutionLogContext executionLogContext = new ExecutionLogContext(processingContext.JobContext);
			ProcessingErrorContext errorContext = null;
			OnDemandProcessingContext odpContext = null;
			AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext odpRenderingContext = null;
			AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSnapshot odpReportSnapshot = null;
			AspNetCore.ReportingServices.OnDemandReportRendering.Report report = null;
			bool exceptionGenerated = false;
			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			try
			{
				report = this.GenerateEventROM(processingContext, onDemandMetadata, eventInfo, executionLogContext, out errorContext, out odpContext, out odpRenderingContext, out odpReportSnapshot);
				IInteractivityPaginationModule interactivityPaginationModule = InteractivityPaginationModuleFactory.CreateInteractivityPaginationModule();
				return interactivityPaginationModule.ProcessDocumentMapNavigationEvent(report, documentMapId);
			}
			catch (Exception exception)
			{
				exceptionGenerated = true;
				Exception ex = default(Exception);
				if (this.NeedWrapperException(exception, errorContext, out ex))
				{
					throw ex;
				}
				throw;
			}
			finally
			{
				result = this.CleanupEventROM(processingContext, executionLogContext, errorContext, odpContext, odpRenderingContext, odpReportSnapshot, exceptionGenerated);
				if (currentCulture != null)
				{
					Thread.CurrentThread.CurrentCulture = currentCulture;
				}
			}
		}

		private int ProcessYukonDocumentMapNavigationEvent(string documentMapId, AspNetCore.ReportingServices.ReportProcessing.ProcessingContext pc)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.Report report = null;
			try
			{
				ReportProcessing.GenerateEventShimROM(pc.ChunkFactory, (EventInformation)null, pc, out report);
				IInteractivityPaginationModule interactivityPaginationModule = InteractivityPaginationModuleFactory.CreateInteractivityPaginationModule();
				return interactivityPaginationModule.ProcessDocumentMapNavigationEvent(report, documentMapId);
			}
			finally
			{
				if (report != null)
				{
					report.RenderingContext.CloseRenderingChunkManager();
				}
			}
		}

		public IDocumentMap GetDocumentMap(EventInformation eventInfo, AspNetCore.ReportingServices.ReportProcessing.ProcessingContext processingContext, out OnDemandProcessingResult result)
		{
			result = null;
			if (ReportProcessing.ContainsFlag(processingContext.ReportProcessingFlags, ReportProcessingFlags.OnDemandEngine))
			{
				return this.GetOdpDocumentMap(eventInfo, processingContext, out result);
			}
			ChunkFactoryAdapter @object = new ChunkFactoryAdapter(processingContext.ChunkFactory);
			return new ShimDocumentMap(this.GetYukonDocumentMap(@object.GetReportChunk));
		}

		private IDocumentMap GetOdpDocumentMap(EventInformation eventInfo, AspNetCore.ReportingServices.ReportProcessing.ProcessingContext processingContext, out OnDemandProcessingResult result)
		{
			result = null;
			OnDemandMetadata onDemandMetadata = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager.DeserializeOnDemandMetadata(processingContext.ChunkFactory, null);
			if (!onDemandMetadata.ReportSnapshot.HasDocumentMap)
			{
				return null;
			}
			Stream stream = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager.OpenExistingDocumentMapStream(onDemandMetadata, processingContext.ReportContext, processingContext.ChunkFactory);
			if (stream == null)
			{
				ExecutionLogContext executionLogContext = new ExecutionLogContext(processingContext.JobContext);
				ProcessingErrorContext errorContext = null;
				OnDemandProcessingContext odpContext = null;
				AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext odpRenderingContext = null;
				AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSnapshot odpReportSnapshot = null;
				AspNetCore.ReportingServices.OnDemandReportRendering.Report report = null;
				bool exceptionGenerated = false;
				CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
				try
				{
					report = this.GenerateEventROM(processingContext, onDemandMetadata, eventInfo, executionLogContext, out errorContext, out odpContext, out odpRenderingContext, out odpReportSnapshot);
					NullRenderer nullRenderer = new NullRenderer();
					nullRenderer.Process(report, odpContext, true, false);
					stream = nullRenderer.DocumentMapStream;
					if (stream == null)
					{
						onDemandMetadata.ReportSnapshot.HasDocumentMap = false;
					}
					else
					{
						stream.Seek(0L, SeekOrigin.Begin);
					}
				}
				catch (Exception exception)
				{
					exceptionGenerated = true;
					Exception ex = default(Exception);
					if (this.NeedWrapperException(exception, errorContext, out ex))
					{
						throw ex;
					}
					throw;
				}
				finally
				{
					result = this.CleanupEventROM(processingContext, executionLogContext, errorContext, odpContext, odpRenderingContext, odpReportSnapshot, exceptionGenerated);
					if (currentCulture != null)
					{
						Thread.CurrentThread.CurrentCulture = currentCulture;
					}
				}
			}
			if (stream != null)
			{
				return new InternalDocumentMap(new DocumentMapReader(stream));
			}
			return null;
		}

		private DocumentMapNode GetYukonDocumentMap(GetReportChunk getReportChunk)
		{
			if (getReportChunk == null)
			{
				return null;
			}
			DocumentMapNode documentMapNode = null;
			ChunkManager.EventsChunkManager eventsChunkManager = null;
			try
			{
				eventsChunkManager = new ChunkManager.EventsChunkManager(getReportChunk);
				return eventsChunkManager.GetDocumentMapNode();
			}
			finally
			{
				if (eventsChunkManager != null)
				{
					eventsChunkManager.Close();
				}
			}
		}

		public string ProcessDrillthroughEvent(string drillthroughId, EventInformation eventInfo, AspNetCore.ReportingServices.ReportProcessing.ProcessingContext processingContext, out NameValueCollection parameters, out OnDemandProcessingResult result)
		{
			parameters = null;
			result = null;
			if (processingContext != null && drillthroughId != null)
			{
				string text = null;
				DrillthroughInfo drillthroughInfo = null;
				if (eventInfo != null)
				{
					drillthroughInfo = eventInfo.GetDrillthroughInfo(drillthroughId);
				}
				if (drillthroughInfo != null)
				{
					text = drillthroughInfo.ReportName;
					DrillthroughParameters reportParameters = drillthroughInfo.ReportParameters;
					parameters = ReportProcessing.ConvertDrillthroughParametersToNameValueCollection(reportParameters);
					return text;
				}
				ExecutionLogContext executionLogContext = new ExecutionLogContext(processingContext.JobContext);
				ProcessingErrorContext errorContext = null;
				OnDemandProcessingContext odpContext = null;
				AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext odpRenderingContext = null;
				AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSnapshot odpReportSnapshot = null;
				AspNetCore.ReportingServices.OnDemandReportRendering.Report report = null;
				bool exceptionGenerated = false;
				CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
				try
				{
					report = this.GenerateEventROM(processingContext, (OnDemandMetadata)null, eventInfo, executionLogContext, out errorContext, out odpContext, out odpRenderingContext, out odpReportSnapshot);
					IInteractivityPaginationModule interactivityPaginationModule = InteractivityPaginationModuleFactory.CreateInteractivityPaginationModule();
					return interactivityPaginationModule.ProcessDrillthroughEvent(report, -1, drillthroughId, out parameters);
				}
				catch (Exception exception)
				{
					exceptionGenerated = true;
					Exception ex = default(Exception);
					if (this.NeedWrapperException(exception, errorContext, out ex))
					{
						throw ex;
					}
					throw;
				}
				finally
				{
					result = this.CleanupEventROM(processingContext, executionLogContext, errorContext, odpContext, odpRenderingContext, odpReportSnapshot, exceptionGenerated);
					if (currentCulture != null)
					{
						Thread.CurrentThread.CurrentCulture = currentCulture;
					}
				}
			}
			return null;
		}

		private static NameValueCollection ConvertDrillthroughParametersToNameValueCollection(DrillthroughParameters reportParameters)
		{
			NameValueCollection result = null;
			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			try
			{
				if (reportParameters != null)
				{
					Thread.CurrentThread.CurrentCulture = Localization.ClientPrimaryCulture;
					result = new NameValueCollection();
					object obj = null;
					string text = null;
					for (int i = 0; i < reportParameters.Count; i++)
					{
						text = reportParameters.GetKey(i);
						obj = reportParameters.GetValue(i);
						object[] array = obj as object[];
						if (array != null)
						{
							for (int j = 0; j < array.Length; j++)
							{
								result.Add(text, ReportProcessing.ConvertToStringUsingThreadCulture(array[j]));
							}
						}
						else
						{
							result.Add(text, ReportProcessing.ConvertToStringUsingThreadCulture(obj));
						}
					}
					return result;
				}
				return result;
			}
			finally
			{
				Thread.CurrentThread.CurrentCulture = currentCulture;
			}
		}

		private static string ConvertToStringUsingThreadCulture(object value)
		{
			if (value == null)
			{
				return null;
			}
			return value.ToString();
		}

		public OnDemandProcessingResult ProcessUserSortEvent(string reportItem, SortOptions sortOption, bool clearOldSorts, AspNetCore.ReportingServices.ReportProcessing.ProcessingContext pc, RenderingContext rc, IChunkFactory originalSnapshot, out string newReportItem, out int page)
		{
			if (ReportProcessing.ContainsFlag(pc.ReportProcessingFlags, ReportProcessingFlags.OnDemandEngine))
			{
				return this.ProcessOdpUserSortEvent(reportItem, sortOption, clearOldSorts, pc, rc, originalSnapshot, out newReportItem, out page);
			}
			return this.ProcessYukonUserSortEvent(reportItem, sortOption, clearOldSorts, pc, rc, originalSnapshot, out newReportItem, out page);
		}

		private OnDemandProcessingResult ProcessOdpUserSortEvent(string reportItem, SortOptions sortOption, bool clearOldSorts, AspNetCore.ReportingServices.ReportProcessing.ProcessingContext pc, RenderingContext rc, IChunkFactory originalSnapshotChunks, out string newReportItem, out int page)
		{
			page = 1;
			newReportItem = null;
			if (originalSnapshotChunks != null && reportItem != null)
			{
				EventInformation eventInformation = null;
				EventInformation eventInfo = rc.EventInfo;
				if (eventInfo != null)
				{
					eventInformation = new EventInformation(eventInfo);
				}
				bool flag = this.ProcessOdpUserSortInformation(reportItem, sortOption, clearOldSorts, ref eventInformation);
				OnDemandProcessingResult result = null;
				ExecutionLogContext executionLogContext = new ExecutionLogContext(pc.JobContext);
				OnDemandProcessingContext onDemandProcessingContext = null;
				AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext renderingContext = null;
				AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSnapshot reportSnapshot = null;
				AspNetCore.ReportingServices.OnDemandReportRendering.Report report = null;
				bool exceptionGenerated = false;
				string itemName = pc.ReportContext.ItemName;
				ProcessingErrorContext errorContext = new ProcessingErrorContext();
				int previousTotalPages = rc.PreviousTotalPages;
				PaginationMode clientPaginationMode = rc.ClientPaginationMode;
				executionLogContext.StartProcessingTimer();
				CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
				try
				{
					OnDemandMetadata odpMetadataFromSnapshot = null;
					newReportItem = reportItem;
					AspNetCore.ReportingServices.ReportIntermediateFormat.Report report2 = default(AspNetCore.ReportingServices.ReportIntermediateFormat.Report);
					GlobalIDOwnerCollection globalIDOwnerCollection = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager.DeserializeOdpReportSnapshot(pc, originalSnapshotChunks, errorContext, true, !flag, this.m_configuration, ref odpMetadataFromSnapshot, out report2);
					SortFilterEventInfoMap oldUserSortInformation = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager.DeserializeSortFilterEventInfo(originalSnapshotChunks, globalIDOwnerCollection);
					if (flag)
					{
						ProcessReportOdpUserSort processReportOdpUserSort = new ProcessReportOdpUserSort(this.Configuration, pc, report2, errorContext, rc.StoreServerParametersCallback, globalIDOwnerCollection, executionLogContext, odpMetadataFromSnapshot, oldUserSortInformation, eventInformation, newReportItem);
						reportSnapshot = ((ProcessReportOdp)processReportOdpUserSort).Execute(out onDemandProcessingContext);
						eventInformation = onDemandProcessingContext.GetUserSortFilterInformation(out newReportItem);
						renderingContext = new AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext(rc.Format, reportSnapshot, eventInformation, onDemandProcessingContext);
					}
					else
					{
						ProcessReportOdpSnapshot processReportOdpSnapshot = new ProcessReportOdpSnapshot(this.Configuration, pc, report2, errorContext, rc.StoreServerParametersCallback, globalIDOwnerCollection, executionLogContext, odpMetadataFromSnapshot);
						reportSnapshot = ((ProcessReportOdp)processReportOdpSnapshot).Execute(out onDemandProcessingContext);
						eventInformation = null;
						renderingContext = new AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext(rc.Format, reportSnapshot, rc.EventInfo, onDemandProcessingContext);
					}
					if (eventInformation != null)
					{
						eventInformation.Changed = true;
					}
					report = new AspNetCore.ReportingServices.OnDemandReportRendering.Report(reportSnapshot.Report, reportSnapshot.ReportInstance, renderingContext, itemName, rc.ReportDescription);
					IInteractivityPaginationModule interactivityPaginationModule = InteractivityPaginationModuleFactory.CreateInteractivityPaginationModule();
					page = interactivityPaginationModule.ProcessUserSortEvent(report, newReportItem, ref previousTotalPages, ref clientPaginationMode);
					if (page <= 0)
					{
						if (flag && eventInformation != null && eventInformation.Changed)
						{
							if (Global.Tracer.TraceVerbose)
							{
								Global.Tracer.Trace(TraceLevel.Verbose, "SortId not found in reprocessed report. Original=" + reportItem + " Reprocessed=" + newReportItem);
								return result;
							}
							return result;
						}
						if (Global.Tracer.TraceVerbose)
						{
							Global.Tracer.Trace(TraceLevel.Verbose, "SortId '" + reportItem + "' not found.");
							return result;
						}
						return result;
					}
					return result;
				}
				catch (Exception exception)
				{
					exceptionGenerated = true;
					Exception ex = default(Exception);
					if (this.NeedWrapperException(exception, errorContext, out ex))
					{
						throw ex;
					}
					throw;
				}
				finally
				{
					result = this.CleanupEventROM(pc, executionLogContext, errorContext, onDemandProcessingContext, renderingContext, reportSnapshot, previousTotalPages, clientPaginationMode, exceptionGenerated);
					if (currentCulture != null)
					{
						Thread.CurrentThread.CurrentCulture = currentCulture;
					}
				}
			}
			return null;
		}

		private OnDemandProcessingResult ProcessYukonUserSortEvent(string reportItem, SortOptions sortOption, bool clearOldSorts, AspNetCore.ReportingServices.ReportProcessing.ProcessingContext pc, RenderingContext rc, IChunkFactory sourceSnapshotChunks, out string newReportItem, out int page)
		{
			page = 1;
			newReportItem = null;
			EventInformation eventInformation = null;
			if (sourceSnapshotChunks != null && reportItem != null)
			{
				ChunkFactoryAdapter @object = new ChunkFactoryAdapter(sourceSnapshotChunks);
				ChunkFactoryAdapter object2 = new ChunkFactoryAdapter(pc.ChunkFactory);
				EventInformation eventInfo = rc.EventInfo;
				if (eventInfo != null)
				{
					eventInformation = new EventInformation(eventInfo);
				}
				int num = default(int);
				if (!this.ProcessUserSortInformation(reportItem, sortOption, clearOldSorts, ref eventInformation, out num))
				{
					return null;
				}
				ChunkManager.EventsChunkManager eventsChunkManager = null;
				ExecutionLogContext executionLogContext = new ExecutionLogContext(pc.JobContext);
				executionLogContext.StartProcessingTimer();
				string itemName = pc.ReportContext.ItemName;
				ProcessingErrorContext processingErrorContext = new ProcessingErrorContext();
				AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext renderingContext = null;
				try
				{
					DateTime executionTime = default(DateTime);
					Hashtable definitionObjects = default(Hashtable);
					IntermediateFormatVersion intermediateFormatVersion = default(IntermediateFormatVersion);
					Report report = ReportProcessing.DeserializeReportFromSnapshot((GetReportChunk)@object.GetReportChunk, out executionTime, out definitionObjects, out intermediateFormatVersion);
					ProcessingContext processingContext = pc.CreateInternalProcessingContext(null, report, processingErrorContext, executionTime, pc.AllowUserProfileState, false, true, false, @object.GetReportChunk, null);
					processingContext.CreateReportChunkFactory = pc.ChunkFactory;
					processingContext.UserSortFilterProcessing = true;
					eventsChunkManager = new ChunkManager.EventsChunkManager(@object.GetReportChunk, definitionObjects, intermediateFormatVersion);
					processingContext.OldSortFilterEventInfo = eventsChunkManager.GetSortFilterEventInfo();
					processingContext.UserSortFilterInfo = eventInformation;
					if (pc.Parameters != null)
					{
						pc.Parameters.StoreLabels();
					}
					UserProfileState userProfileState = default(UserProfileState);
					ReportSnapshot reportSnapshot = this.ProcessReport(report, pc, processingContext, out userProfileState);
					eventInformation = processingContext.GetUserSortFilterInformation(ref num, ref page);
					newReportItem = num.ToString(CultureInfo.InvariantCulture);
					ChunkManager.RenderingChunkManager chunkManager = new ChunkManager.RenderingChunkManager(object2.GetReportChunk, null, definitionObjects, null, report.IntermediateFormatVersion);
					AspNetCore.ReportingServices.ReportRendering.RenderingContext renderingContext2 = new AspNetCore.ReportingServices.ReportRendering.RenderingContext(reportSnapshot, null, executionTime, report.EmbeddedImages, report.ImageStreamNames, null, pc.ReportContext, null, null, object2.GetReportChunk, chunkManager, pc.GetResourceCallback, null, rc.StoreServerParametersCallback, false, pc.AllowUserProfileState, pc.ReportRuntimeSetup, pc.JobContext, pc.DataProtection);
					renderingContext = new AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext(null, reportSnapshot, pc.ChunkFactory, eventInfo);
					AspNetCore.ReportingServices.OnDemandReportRendering.Report report2 = new AspNetCore.ReportingServices.OnDemandReportRendering.Report(reportSnapshot.Report, reportSnapshot.ReportInstance, renderingContext2, renderingContext, itemName, null);
					int previousTotalPages = rc.PreviousTotalPages;
					PaginationMode clientPaginationMode = rc.ClientPaginationMode;
					IInteractivityPaginationModule interactivityPaginationModule = InteractivityPaginationModuleFactory.CreateInteractivityPaginationModule();
					page = interactivityPaginationModule.ProcessUserSortEvent(report2, newReportItem, ref previousTotalPages, ref clientPaginationMode);
					return new YukonProcessingResult(reportSnapshot, processingContext.ChunkManager, pc.ChunkFactory, pc.Parameters, report.AutoRefresh, previousTotalPages, processingErrorContext.Messages, false, renderingContext2.RenderingInfoManager, true, eventInformation, clientPaginationMode, pc.ChunkFactory.ReportProcessingFlags, userProfileState | renderingContext2.UsedUserProfileState, executionLogContext);
				}
				catch (RSException)
				{
					throw;
				}
				catch (Exception innerException)
				{
					throw new ReportProcessingException(innerException, processingErrorContext.Messages);
				}
				finally
				{
					if (eventsChunkManager != null)
					{
						eventsChunkManager.Close();
					}
					if (renderingContext != null)
					{
						renderingContext.CloseRenderingChunkManager();
					}
					ReportProcessing.UpdateHostingEnvironment(processingErrorContext, pc.ReportContext, executionLogContext, ProcessingEngine.YukonEngine, pc.JobContext);
				}
			}
			return null;
		}

		private AspNetCore.ReportingServices.OnDemandReportRendering.Report GenerateEventROM(AspNetCore.ReportingServices.ReportProcessing.ProcessingContext pc, OnDemandMetadata odpMetadata, EventInformation eventInfo, ExecutionLogContext executionLogContext, out ProcessingErrorContext errorContext, out OnDemandProcessingContext odpContext, out AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext odpRenderingContext, out AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSnapshot odpReportSnapshot)
		{
			Global.Tracer.Assert(executionLogContext != null, "ExecutionLogContext may not be null");
			odpRenderingContext = null;
			odpContext = null;
			errorContext = new ProcessingErrorContext();
			executionLogContext.StartProcessingTimer();
			AspNetCore.ReportingServices.ReportIntermediateFormat.Report report = default(AspNetCore.ReportingServices.ReportIntermediateFormat.Report);
			GlobalIDOwnerCollection globalIDOwnerCollection = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager.DeserializeOdpReportSnapshot(pc, (IChunkFactory)null, errorContext, true, true, this.m_configuration, ref odpMetadata, out report);
			odpReportSnapshot = odpMetadata.ReportSnapshot;
			ProcessReportOdpSnapshot processReportOdpSnapshot = new ProcessReportOdpSnapshot(this.Configuration, pc, report, errorContext, null, globalIDOwnerCollection, executionLogContext, odpMetadata);
			odpReportSnapshot = ((ProcessReportOdp)processReportOdpSnapshot).Execute(out odpContext);
			odpRenderingContext = new AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext(null, odpReportSnapshot, eventInfo, odpContext);
			return new AspNetCore.ReportingServices.OnDemandReportRendering.Report(odpReportSnapshot.Report, odpReportSnapshot.ReportInstance, odpRenderingContext, pc.ReportContext.ItemName, null);
		}

		private bool NeedWrapperException(Exception exception, ProcessingErrorContext errorContext, out Exception wrappedException)
		{
			if (exception is RSException)
			{
				wrappedException = null;
				return false;
			}
			wrappedException = new ReportProcessingException(exception, errorContext.Messages);
			return true;
		}

		private OnDemandProcessingResult CleanupEventROM(AspNetCore.ReportingServices.ReportProcessing.ProcessingContext pc, ExecutionLogContext executionLogContext, ProcessingErrorContext errorContext, OnDemandProcessingContext odpContext, AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext odpRenderingContext, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSnapshot odpReportSnapshot, bool exceptionGenerated)
		{
			return this.CleanupEventROM(pc, executionLogContext, errorContext, odpContext, odpRenderingContext, odpReportSnapshot, 0, PaginationMode.Estimate, exceptionGenerated);
		}

		private OnDemandProcessingResult CleanupEventROM(AspNetCore.ReportingServices.ReportProcessing.ProcessingContext pc, ExecutionLogContext executionLogContext, ProcessingErrorContext errorContext, OnDemandProcessingContext odpContext, AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext odpRenderingContext, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSnapshot odpReportSnapshot, int pageCount, PaginationMode updatedPaginationMode, bool exceptionGenerated)
		{
			try
			{
				bool eventInfoChanged = false;
				EventInformation newEventInfo = null;
				if (odpRenderingContext != null)
				{
					ReportProcessing.UpdateEventInfo(odpReportSnapshot, odpContext, odpRenderingContext, ref eventInfoChanged);
					newEventInfo = odpRenderingContext.EventInfo;
					odpRenderingContext.CloseRenderingChunkManager();
				}
				if (errorContext != null && odpReportSnapshot != null)
				{
					errorContext.Combine(odpReportSnapshot.Warnings);
				}
				ReportProcessing.CleanupOnDemandProcessing(odpContext, false);
				OnDemandProcessingResult result = null;
				if (exceptionGenerated)
				{
					ReportProcessing.RequestErrorGroupTreeCleanup(odpContext);
				}
				else
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager.PreparePartitionedTreesForAsyncSerialization(odpContext);
					result = new FullOnDemandProcessingResult(odpReportSnapshot, odpContext.OdpMetadata.OdpChunkManager, odpContext.OdpMetadata.SnapshotHasChanged, pc.ChunkFactory, pc.Parameters, odpReportSnapshot.Report.EvaluateAutoRefresh(null, odpContext), pageCount, errorContext.Messages, eventInfoChanged, newEventInfo, updatedPaginationMode, pc.ReportProcessingFlags, odpContext.HasUserProfileState, executionLogContext);
				}
				return result;
			}
			finally
			{
				if (odpContext != null)
				{
					odpContext.FreeAllResources();
				}
				ReportProcessing.UpdateHostingEnvironment(errorContext, pc.ReportContext, executionLogContext, ProcessingEngine.OnDemandEngine, pc.JobContext);
			}
		}

		private static void GenerateEventShimROM(IChunkFactory chunkFactory, EventInformation eventInfo, AspNetCore.ReportingServices.ReportProcessing.ProcessingContext pc, out AspNetCore.ReportingServices.OnDemandReportRendering.Report reportToRender)
		{
			bool flag = false;
			reportToRender = null;
			Stream stream = null;
			ReportSnapshot reportSnapshot = null;
			ChunkManager.RenderingChunkManager renderingChunkManager = null;
			Hashtable instanceObjects = null;
			Hashtable definitionObjects = null;
			AspNetCore.ReportingServices.ReportProcessing.Persistence.IntermediateFormatReader.State declarationsRead = null;
			try
			{
				string text = default(string);
				stream = chunkFactory.GetChunk("Main", ReportChunkTypes.Main, ChunkMode.Open, out text);
				if (stream != null)
				{
					flag = true;
				}
				if (flag)
				{
					AspNetCore.ReportingServices.ReportProcessing.Persistence.IntermediateFormatReader intermediateFormatReader = new AspNetCore.ReportingServices.ReportProcessing.Persistence.IntermediateFormatReader(stream);
					reportSnapshot = intermediateFormatReader.ReadReportSnapshot();
					instanceObjects = intermediateFormatReader.InstanceObjects;
					declarationsRead = intermediateFormatReader.ReaderState;
					definitionObjects = intermediateFormatReader.DefinitionObjects;
				}
			}
			finally
			{
				if (stream != null)
				{
					stream.Close();
				}
			}
			Global.Tracer.Assert(null != reportSnapshot, "(null != reportSnapshot)");
			Global.Tracer.Assert(null != reportSnapshot.Report, "(null != reportSnapshot.Report)");
			Global.Tracer.Assert(null != reportSnapshot.ReportInstance, "(null != reportSnapshot.ReportInstance)");
			ChunkFactoryAdapter @object = new ChunkFactoryAdapter(chunkFactory);
			renderingChunkManager = new ChunkManager.RenderingChunkManager(@object.GetReportChunk, instanceObjects, definitionObjects, declarationsRead, reportSnapshot.Report.IntermediateFormatVersion);
			AspNetCore.ReportingServices.ReportRendering.RenderingContext oldRenderingContext = new AspNetCore.ReportingServices.ReportRendering.RenderingContext(reportSnapshot, null, reportSnapshot.ExecutionTime, null, null, eventInfo, null, null, null, @object.GetReportChunk, renderingChunkManager, null, null, null, false, UserProfileState.None, pc.ReportRuntimeSetup, pc.JobContext, pc.DataProtection);
			AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext renderingContext = new AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext(null, reportSnapshot, chunkFactory, eventInfo);
			reportToRender = new AspNetCore.ReportingServices.OnDemandReportRendering.Report(reportSnapshot.Report, reportSnapshot.ReportInstance, oldRenderingContext, renderingContext, null, null);
		}

		internal void ProcessShowHideToggle(string showHideToggle, ReportSnapshot reportSnapshot, EventInformation oldOverrideInformation, ChunkManager.RenderingChunkManager chunkManager, out bool showHideInformationChanged, out EventInformation newOverrideInformation)
		{
			ShowHideProcessing showHideProcessing = new ShowHideProcessing();
			showHideProcessing.Process(showHideToggle, reportSnapshot, oldOverrideInformation, chunkManager, out showHideInformationChanged, out newOverrideInformation);
		}

		private bool ProcessOdpUserSortInformation(string reportItemUniqueName, SortOptions sortOption, bool clearOldSorts, ref EventInformation userSortInformation)
		{
			bool result = false;
			bool flag = false;
			if (sortOption == SortOptions.None)
			{
				if (userSortInformation != null && userSortInformation.OdpSortInfo != null && this.ProcessOdpUserSortInformation(reportItemUniqueName, sortOption, clearOldSorts, ref userSortInformation, out flag))
				{
					if (userSortInformation.OdpSortInfo.Count == 0)
					{
						if (userSortInformation.ToggleStateInfo == null && userSortInformation.HiddenInfo == null)
						{
							userSortInformation = null;
						}
						else
						{
							userSortInformation.OdpSortInfo = null;
						}
					}
					result = true;
				}
			}
			else
			{
				if (userSortInformation == null)
				{
					userSortInformation = new EventInformation();
				}
				if (userSortInformation.OdpSortInfo == null)
				{
					userSortInformation.OdpSortInfo = new EventInformation.OdpSortEventInfo();
				}
				result = this.ProcessOdpUserSortInformation(reportItemUniqueName, sortOption, clearOldSorts, ref userSortInformation, out flag);
				if (!flag)
				{
					userSortInformation.OdpSortInfo.Add(reportItemUniqueName, (byte)((sortOption == SortOptions.Ascending) ? 1 : 0) != 0, null);
					result = true;
				}
			}
			return result;
		}

		private bool ProcessOdpUserSortInformation(string reportItemUniqueName, SortOptions sortOption, bool clearOldSorts, ref EventInformation userSortInformation, out bool eventExists)
		{
			eventExists = false;
			bool flag = false;
			if (clearOldSorts)
			{
				flag = userSortInformation.OdpSortInfo.ClearPeerSorts(reportItemUniqueName);
			}
			SortOptions sortState = userSortInformation.OdpSortInfo.GetSortState(reportItemUniqueName);
			if (sortState != 0)
			{
				if (sortState == sortOption)
				{
					eventExists = true;
				}
				else
				{
					flag |= userSortInformation.OdpSortInfo.Remove(reportItemUniqueName);
				}
			}
			return flag;
		}

		private bool ProcessUserSortInformation(string reportItem, SortOptions sortOption, bool clearOldSorts, ref EventInformation userSortInformation, out int reportItemUniqueName)
		{
			reportItemUniqueName = -1;
			if (!int.TryParse(reportItem, NumberStyles.None, (IFormatProvider)CultureInfo.InvariantCulture, out reportItemUniqueName))
			{
				throw new ReportProcessingException(ErrorCode.rsInvalidSortItemID);
			}
			if (0 > reportItemUniqueName)
			{
				throw new ReportProcessingException(ErrorCode.rsInvalidSortItemID);
			}
			bool result = false;
			bool flag = false;
			if (sortOption == SortOptions.None)
			{
				if (userSortInformation != null && userSortInformation.SortInfo != null && this.ProcessUserSortInformation(reportItemUniqueName, sortOption, clearOldSorts, ref userSortInformation, out flag))
				{
					if (userSortInformation.SortInfo.Count == 0)
					{
						if (userSortInformation.ToggleStateInfo == null && userSortInformation.HiddenInfo == null)
						{
							userSortInformation = null;
						}
						else
						{
							userSortInformation.SortInfo = null;
						}
					}
					result = true;
				}
			}
			else
			{
				if (userSortInformation == null)
				{
					userSortInformation = new EventInformation();
				}
				if (userSortInformation.SortInfo == null)
				{
					userSortInformation.SortInfo = new EventInformation.SortEventInfo();
				}
				result = this.ProcessUserSortInformation(reportItemUniqueName, sortOption, clearOldSorts, ref userSortInformation, out flag);
				if (!flag)
				{
					userSortInformation.SortInfo.Add(reportItemUniqueName, (byte)((sortOption == SortOptions.Ascending) ? 1 : 0) != 0, null);
					result = true;
				}
			}
			return result;
		}

		private bool ProcessUserSortInformation(int reportItemUniqueName, SortOptions sortOption, bool clearOldSorts, ref EventInformation userSortInformation, out bool eventExists)
		{
			eventExists = false;
			bool flag = false;
			if (clearOldSorts)
			{
				flag = userSortInformation.SortInfo.ClearPeerSorts(reportItemUniqueName);
			}
			SortOptions sortState = userSortInformation.SortInfo.GetSortState(reportItemUniqueName);
			if (sortState != 0)
			{
				if (sortState == sortOption)
				{
					eventExists = true;
				}
				else
				{
					flag |= userSortInformation.SortInfo.Remove(reportItemUniqueName);
				}
			}
			return flag;
		}

		internal ReportProcessing()
		{
		}

		public static bool NeedsUpgradeToLatest(ReportProcessingFlags processingFlags)
		{
			return processingFlags == ReportProcessingFlags.NotSet;
		}

		public static ReportChunkTypes GetImageChunkTypeToCopy(ReportProcessingFlags processingFlags)
		{
			if (ReportProcessing.ContainsFlag(processingFlags, ReportProcessingFlags.OnDemandEngine))
			{
				return ReportChunkTypes.StaticImage;
			}
			return ReportChunkTypes.Image;
		}

		internal static void CheckReportCredentialsAndConnectionUserDependency(AspNetCore.ReportingServices.ReportProcessing.ProcessingContext pc)
		{
			if (pc != null)
			{
				string itemName = (pc.ReportContext != null) ? pc.ReportContext.ItemName : null;
				ReportProcessing.CheckReportCredentialsAndConnectionUserDependency(pc.DataSources, pc.AllowUserProfileState, itemName);
			}
		}

		internal static void CheckReportCredentialsAndConnectionUserDependency(RuntimeDataSourceInfoCollection dataSources, UserProfileState allowUserProfileState, string itemName)
		{
			if (dataSources != null)
			{
				if (dataSources.NeedPrompt)
				{
					throw new ReportProcessingException(ErrorCode.rsCredentialsNotSpecified);
				}
				if ((allowUserProfileState & UserProfileState.InQuery) != 0)
				{
					return;
				}
				if (!dataSources.HasConnectionStringUseridReference())
				{
					return;
				}
				throw new ReportProcessingException(ErrorCode.rsHasUserProfileDependencies, null, itemName);
			}
		}

		public static bool UpgradeSnapshot(IChunkFactory getChunkFactory, bool isSnapshot, IChunkFactory createChunkFactory, ICatalogItemContext reportContext, out int pageCount, out bool hasDocumentMap)
		{
			pageCount = 0;
			hasDocumentMap = false;
			if (createChunkFactory != null && getChunkFactory != null)
			{
				ChunkFactoryAdapter @object = new ChunkFactoryAdapter(createChunkFactory);
				ChunkFactoryAdapter object2 = new ChunkFactoryAdapter(getChunkFactory);
				CreateReportChunk createChunkCallback = @object.CreateReportChunk;
				GetReportChunk getReportChunk = object2.GetReportChunk;
				if (!isSnapshot)
				{
					Report report = ReportProcessing.DeserializeReport(getReportChunk);
					ReportProcessing.SerializeReport(report, createChunkCallback);
					return true;
				}
				Stream stream = null;
				ChunkManager.RenderingChunkManager renderingChunkManager = null;
				ChunkManager.UpgradeManager upgradeManager = null;
				try
				{
					string text = default(string);
					stream = getReportChunk("Main", ReportChunkTypes.Main, out text);
					AspNetCore.ReportingServices.ReportProcessing.Persistence.IntermediateFormatReader intermediateFormatReader = new AspNetCore.ReportingServices.ReportProcessing.Persistence.IntermediateFormatReader(stream);
					ReportSnapshot reportSnapshot = intermediateFormatReader.ReadReportSnapshot();
					Hashtable instanceObjects = intermediateFormatReader.InstanceObjects;
					AspNetCore.ReportingServices.ReportProcessing.Persistence.IntermediateFormatReader.State readerState = intermediateFormatReader.ReaderState;
					Hashtable definitionObjects = intermediateFormatReader.DefinitionObjects;
					Global.Tracer.Assert(null != reportSnapshot, "(null != reportSnapshot)");
					Global.Tracer.Assert(null != reportSnapshot.Report, "(null != reportSnapshot.Report)");
					Global.Tracer.Assert(null != reportSnapshot.ReportInstance, "(null != reportSnapshot.ReportInstance)");
					renderingChunkManager = new ChunkManager.RenderingChunkManager(getReportChunk, instanceObjects, definitionObjects, readerState, reportSnapshot.Report.IntermediateFormatVersion);
					Upgrader.UpgradeToCurrent(reportSnapshot, renderingChunkManager, createChunkCallback);
					Upgrader.UpgradeDatasetIDs(reportSnapshot.Report);
					reportSnapshot.DocumentMap = reportSnapshot.GetDocumentMap(renderingChunkManager);
					hasDocumentMap = (null != reportSnapshot.DocumentMap);
					pageCount = reportSnapshot.ReportInstance.NumberOfPages;
					reportSnapshot.QuickFind = reportSnapshot.GetQuickFind(renderingChunkManager);
					reportSnapshot.ShowHideReceiverInfo = reportSnapshot.GetShowHideReceiverInfo(renderingChunkManager);
					reportSnapshot.ShowHideSenderInfo = reportSnapshot.GetShowHideSenderInfo(renderingChunkManager);
					upgradeManager = new ChunkManager.UpgradeManager(createChunkCallback);
					Upgrader.CreateBookmarkDrillthroughChunks(reportSnapshot, renderingChunkManager, upgradeManager);
					renderingChunkManager.Close();
					renderingChunkManager = null;
					if (stream != null)
					{
						stream.Close();
						stream = null;
					}
					upgradeManager.FinalFlush();
					upgradeManager.SaveFirstPage();
					upgradeManager.SaveReportSnapshot(reportSnapshot);
				}
				finally
				{
					if (stream != null)
					{
						stream.Close();
					}
					if (renderingChunkManager != null)
					{
						renderingChunkManager.Close();
					}
					if (upgradeManager != null)
					{
						upgradeManager.Close();
					}
				}
				return true;
			}
			return false;
		}

		public OnDemandProcessingResult CreateSnapshot(DateTime executionTimeStamp, AspNetCore.ReportingServices.ReportProcessing.ProcessingContext pc, IChunkFactory yukonCompiledDefinition)
		{
			ExecutionLogContext executionLogContext = new ExecutionLogContext(pc.JobContext);
			executionLogContext.StartProcessingTimer();
			ProcessingEngine processingEngine = ProcessingEngine.OnDemandEngine;
			ProcessingErrorContext processingErrorContext = new ProcessingErrorContext();
			AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext renderingContext = null;
			OnDemandProcessingContext onDemandProcessingContext = null;
			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			try
			{
				if (!pc.Parameters.ValuesAreValid())
				{
					throw new ReportProcessingException(ErrorCode.rsParameterError);
				}
				string itemName = pc.ReportContext.ItemName;
				ReportProcessing.CheckReportCredentialsAndConnectionUserDependency(pc.DataSources, pc.AllowUserProfileState, itemName);
				if (pc.ReportProcessingFlags != 0 && !ReportProcessing.ContainsFlag(pc.ReportProcessingFlags, ReportProcessingFlags.YukonEngine))
				{
					processingEngine = ProcessingEngine.OnDemandEngine;
					GlobalIDOwnerCollection globalIDOwnerCollection = new GlobalIDOwnerCollection();
					AspNetCore.ReportingServices.ReportIntermediateFormat.Report report = ReportProcessing.DeserializeKatmaiReport(pc.ChunkFactory, false, globalIDOwnerCollection);
					AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSnapshot reportSnapshot = null;
					AspNetCore.ReportingServices.OnDemandReportRendering.Report report2 = null;
					ProcessReportOdpInitial processReportOdpInitial = new ProcessReportOdpInitial(this.Configuration, pc, report, processingErrorContext, null, globalIDOwnerCollection, executionLogContext, executionTimeStamp);
					reportSnapshot = ((ProcessReportOdp)processReportOdpInitial).Execute(out onDemandProcessingContext);
					renderingContext = new AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext(null, reportSnapshot, null, onDemandProcessingContext);
					report2 = new AspNetCore.ReportingServices.OnDemandReportRendering.Report(reportSnapshot.Report, reportSnapshot.ReportInstance, renderingContext, itemName, null);
					NullRenderer nullRenderer = new NullRenderer();
					nullRenderer.Process(report2, onDemandProcessingContext, false, true);
					ReportProcessing.CleanupOnDemandProcessing(onDemandProcessingContext, false);
					bool eventInfoChanged = false;
					ReportProcessing.UpdateEventInfo(reportSnapshot, onDemandProcessingContext, renderingContext, ref eventInfoChanged);
					AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager.PreparePartitionedTreesForSyncSerialization(onDemandProcessingContext);
					OnDemandProcessingResult onDemandProcessingResult = new FullOnDemandProcessingResult(reportSnapshot, onDemandProcessingContext.OdpMetadata.OdpChunkManager, onDemandProcessingContext.OdpMetadata.SnapshotHasChanged, pc.ChunkFactory, pc.Parameters, reportSnapshot.Report.EvaluateAutoRefresh(null, onDemandProcessingContext), 0, processingErrorContext.Messages, eventInfoChanged, renderingContext.EventInfo, PaginationMode.Estimate, pc.ChunkFactory.ReportProcessingFlags, onDemandProcessingContext.HasUserProfileState, executionLogContext);
					onDemandProcessingResult.Save();
					return onDemandProcessingResult;
				}
				processingEngine = ProcessingEngine.YukonEngine;
				ChunkFactoryAdapter @object = new ChunkFactoryAdapter(pc.ChunkFactory);
				ChunkFactoryAdapter object2 = new ChunkFactoryAdapter(yukonCompiledDefinition);
				Report report3 = ReportProcessing.DeserializeReport(object2.GetReportChunk);
				ProcessingContext processingContext = default(ProcessingContext);
				UserProfileState usedUserProfileState = default(UserProfileState);
				ReportSnapshot reportSnapshot2 = this.ProcessReport(report3, pc, false, false, (GetReportChunk)@object.GetReportChunk, (ErrorContext)processingErrorContext, executionTimeStamp, (CreateReportChunk)null, out processingContext, out usedUserProfileState);
				Global.Tracer.Assert(processingContext != null && null != processingContext.ChunkManager, "(null != context && null != context.ChunkManager)");
				Global.Tracer.Assert(null != reportSnapshot2, "(null != reportSnapshot)");
				executionLogContext.AddLegacyDataProcessingTime(processingContext.DataProcessingDurationMs);
				processingContext.ChunkManager.SaveFirstPage();
				processingContext.ChunkManager.SaveReportSnapshot(reportSnapshot2);
				return new YukonProcessingResult(reportSnapshot2, processingContext.ChunkManager, pc.Parameters, report3.AutoRefresh, reportSnapshot2.ReportInstance.NumberOfPages, processingErrorContext.Messages, pc.ChunkFactory.ReportProcessingFlags, usedUserProfileState, executionLogContext);
			}
			catch (RSException)
			{
				ReportProcessing.RequestErrorGroupTreeCleanup(onDemandProcessingContext);
				throw;
			}
			catch (Exception innerException)
			{
				ReportProcessing.RequestErrorGroupTreeCleanup(onDemandProcessingContext);
				throw new ReportProcessingException(innerException, processingErrorContext.Messages);
			}
			finally
			{
				if (onDemandProcessingContext != null)
				{
					onDemandProcessingContext.FreeAllResources();
				}
				if (renderingContext != null)
				{
					renderingContext.CloseRenderingChunkManager();
				}
				ReportProcessing.UpdateHostingEnvironment(processingErrorContext, pc.ReportContext, executionLogContext, processingEngine, pc.JobContext);
				if (currentCulture != null)
				{
					Thread.CurrentThread.CurrentCulture = currentCulture;
				}
			}
		}

		public static void CreateRenderer(string format, IExtensionFactory extFactory, out IRenderingExtension newRenderer)
		{
			newRenderer = null;
			try
			{
				newRenderer = ReportRendererFactory.GetNewRenderer(format, extFactory);
				if (newRenderer == null)
				{
					throw new ReportProcessingException(ErrorCode.rsRenderingExtensionNotFound);
				}
			}
			catch (RSException)
			{
				throw;
			}
			catch (Exception innerException)
			{
				throw new ReportProcessingException(innerException, null);
			}
		}

		public OnDemandProcessingResult RenderReportAndCacheData(DateTime executionTimeStamp, AspNetCore.ReportingServices.ReportProcessing.ProcessingContext pc, RenderingContext rc, IChunkFactory metaDataChunkFactory, IChunkFactory yukonCompiledDefinition)
		{
			IRenderingExtension newRenderer = default(IRenderingExtension);
			ReportProcessing.CreateRenderer(rc.Format, pc.ExtFactory, out newRenderer);
			return this.RenderReportAndCacheData(newRenderer, executionTimeStamp, pc, rc, metaDataChunkFactory, yukonCompiledDefinition);
		}

		public OnDemandProcessingResult RenderReportAndCacheData(IRenderingExtension newRenderer, DateTime executionTimeStamp, AspNetCore.ReportingServices.ReportProcessing.ProcessingContext pc, RenderingContext rc, IChunkFactory metaDataChunkFactory, IChunkFactory yukonCompiledDefinition)
		{
			RenderReport renderReport = (!ReportProcessing.IsYukonProcessingEngine(pc.ReportProcessingFlags)) ? ((RenderReport)new RenderReportOdpLiveAndCacheData(pc, rc, executionTimeStamp, this.Configuration, metaDataChunkFactory)) : ((RenderReport)new RenderReportYukonInitial(pc, rc, executionTimeStamp, this, yukonCompiledDefinition));
			return renderReport.Execute(newRenderer);
		}

		public OnDemandProcessingResult RenderDefinitionOnly(DateTime executionTimeStamp, AspNetCore.ReportingServices.ReportProcessing.ProcessingContext pc, RenderingContext rc, IChunkFactory yukonCompiledDefinition)
		{
			IRenderingExtension newRenderer = default(IRenderingExtension);
			ReportProcessing.CreateRenderer(rc.Format, pc.ExtFactory, out newRenderer);
			return this.RenderDefinitionOnly(newRenderer, executionTimeStamp, pc, rc, yukonCompiledDefinition);
		}

		public OnDemandProcessingResult RenderDefinitionOnly(IRenderingExtension newRenderer, DateTime executionTimeStamp, AspNetCore.ReportingServices.ReportProcessing.ProcessingContext pc, RenderingContext rc, IChunkFactory yukonCompiledDefinition)
		{
			RenderReport renderReport = (!ReportProcessing.IsYukonProcessingEngine(pc.ReportProcessingFlags)) ? ((RenderReport)new RenderReportDefinitionOnly(pc, rc, executionTimeStamp, this.Configuration)) : ((RenderReport)new RenderReportYukonDefinitionOnly(pc, rc, executionTimeStamp, this, yukonCompiledDefinition));
			return renderReport.Execute(newRenderer);
		}

		public OnDemandProcessingResult RenderReport(DateTime executionTimeStamp, AspNetCore.ReportingServices.ReportProcessing.ProcessingContext pc, RenderingContext rc, IChunkFactory yukonCompiledDefinition)
		{
			IRenderingExtension newRenderer = default(IRenderingExtension);
			ReportProcessing.CreateRenderer(rc.Format, pc.ExtFactory, out newRenderer);
			return this.RenderReport(newRenderer, executionTimeStamp, pc, rc, yukonCompiledDefinition);
		}

		public OnDemandProcessingResult RenderReport(IRenderingExtension newRenderer, DateTime executionTimeStamp, AspNetCore.ReportingServices.ReportProcessing.ProcessingContext pc, RenderingContext rc, IChunkFactory yukonCompiledDefinition)
		{
			RenderReport renderReport = (!ReportProcessing.IsYukonProcessingEngine(pc.ReportProcessingFlags)) 
                ? ((RenderReport)new RenderReportOdpInitial(pc, rc, executionTimeStamp, this.Configuration)) : 
                ((RenderReport)new RenderReportYukonInitial(pc, rc, executionTimeStamp, this, yukonCompiledDefinition));
            return renderReport.Execute(newRenderer);
		}

		public OnDemandProcessingResult RenderSnapshot(RenderingContext rc, AspNetCore.ReportingServices.ReportProcessing.ProcessingContext pc)
		{
			IRenderingExtension newRenderer = default(IRenderingExtension);
			ReportProcessing.CreateRenderer(rc.Format, pc.ExtFactory, out newRenderer);
			return this.RenderSnapshot(newRenderer, rc, pc);
		}

		public OnDemandProcessingResult RenderSnapshot(IRenderingExtension newRenderer, RenderingContext rc, AspNetCore.ReportingServices.ReportProcessing.ProcessingContext pc)
		{
			RenderReport renderReport = (!ReportProcessing.IsYukonProcessingEngine(pc.ReportProcessingFlags)) ? ((RenderReport)new RenderReportOdpSnapshot(pc, rc, this.Configuration)) : ((RenderReport)new RenderReportYukonSnapshot(pc, rc, this));
			return renderReport.Execute(newRenderer);
		}

		public OnDemandProcessingResult RenderReportWithCachedData(DateTime executionTimeStamp, AspNetCore.ReportingServices.ReportProcessing.ProcessingContext pc, RenderingContext rc, IChunkFactory getMetaDataFactory)
		{
			IRenderingExtension newRenderer = default(IRenderingExtension);
			ReportProcessing.CreateRenderer(rc.Format, pc.ExtFactory, out newRenderer);
			return this.RenderReportWithCachedData(newRenderer, executionTimeStamp, pc, rc, getMetaDataFactory);
		}

		public OnDemandProcessingResult RenderReportWithCachedData(IRenderingExtension newRenderer, DateTime executionTimeStamp, AspNetCore.ReportingServices.ReportProcessing.ProcessingContext pc, RenderingContext rc, IChunkFactory getMetaDataFactory)
		{
			if (ReportProcessing.ContainsFlag(pc.ReportProcessingFlags, ReportProcessingFlags.YukonEngine))
			{
				Global.Tracer.Assert(false, "initial processing based on cached data in Yukon");
				throw new InvalidOperationException();
			}
			RenderReport renderReport = new RenderReportOdpWithCachedData(pc, rc, executionTimeStamp, this.Configuration, getMetaDataFactory);
			return renderReport.Execute(newRenderer);
		}

		public OnDemandProcessingResult ProcessAndRenderSnapshot(AspNetCore.ReportingServices.ReportProcessing.ProcessingContext pc, RenderingContext rc, IChunkFactory originalSnapshotChunks)
		{
			IRenderingExtension newRenderer = default(IRenderingExtension);
			ReportProcessing.CreateRenderer(rc.Format, pc.ExtFactory, out newRenderer);
			return this.ProcessAndRenderSnapshot(newRenderer, pc, rc, originalSnapshotChunks);
		}

		public OnDemandProcessingResult ProcessAndRenderSnapshot(IRenderingExtension newRenderer, AspNetCore.ReportingServices.ReportProcessing.ProcessingContext pc, RenderingContext rc, IChunkFactory originalSnapshotChunks)
		{
			RenderReport renderReport = (!ReportProcessing.IsYukonProcessingEngine(pc.ReportProcessingFlags)) ? ((RenderReport)new RenderReportOdpReprocessSnapshot(pc, rc, this.Configuration, originalSnapshotChunks)) : ((RenderReport)new RenderReportYukonReprocessSnapshot(pc, rc, this, originalSnapshotChunks));
			return renderReport.Execute(newRenderer);
		}

		public OnDemandProcessingResult RenderSnapshotStream(string streamName, RenderingContext rc, AspNetCore.ReportingServices.ReportProcessing.ProcessingContext pc)
		{
			IRenderingExtension newRenderer = default(IRenderingExtension);
			ReportProcessing.CreateRenderer(rc.Format, pc.ExtFactory, out newRenderer);
			return this.RenderSnapshotStream(newRenderer, streamName, rc, pc);
		}

		public OnDemandProcessingResult RenderSnapshotStream(IRenderingExtension newRenderer, string streamName, RenderingContext rc, AspNetCore.ReportingServices.ReportProcessing.ProcessingContext pc)
		{
			RenderReport renderReport = (!ReportProcessing.IsYukonProcessingEngine(pc.ReportProcessingFlags)) ? ((RenderReport)new RenderReportOdpSnapshotStream(pc, rc, this.Configuration, streamName)) : ((RenderReport)new RenderReportYukonSnapshotStream(pc, rc, this, streamName));
			return renderReport.Execute(newRenderer);
		}

		public void CallRenderer(ICatalogItemContext cc, IExtensionFactory extFactory, CreateAndRegisterStream createAndRegisterStreamCallback)
		{
			IRenderingExtension newRenderer = default(IRenderingExtension);
			ReportProcessing.CreateRenderer(cc.RSRequestParameters.FormatParamValue, extFactory, out newRenderer);
			this.CallRenderer(newRenderer, cc, createAndRegisterStreamCallback);
		}

		public void CallRenderer(IRenderingExtension newRenderer, ICatalogItemContext cc, CreateAndRegisterStream createAndRegisterStreamCallback)
		{
			try
			{
				newRenderer.GetRenderingResource(createAndRegisterStreamCallback, cc.RSRequestParameters.RenderingParameters);
			}
			catch (RSException)
			{
				throw;
			}
			catch (ReportRenderingException rex)
			{
				ReportProcessing.HandleRenderingException(rex);
			}
			catch (Exception ex2)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(ex2))
				{
					throw;
				}
				throw new UnhandledReportRenderingException(ex2);
			}
		}

		public void GetAllDataSources(ICatalogItemContext reportContext, IChunkFactory getCompiledDefinitionFactory, OnDemandSubReportDataSourcesCallback subReportCallback, DataSourceInfoCollection dataSources, DataSetInfoCollection dataSetReferences, bool checkIfUsable, ServerDataSourceSettings serverDatasourceSettings, out RuntimeDataSourceInfoCollection allDataSources, out RuntimeDataSetInfoCollection allDataSetReferences)
		{
			try
			{
				allDataSources = new RuntimeDataSourceInfoCollection();
				allDataSetReferences = new RuntimeDataSetInfoCollection();
				Report report = null;
				Hashtable subReportNames = new Hashtable();
				if (getCompiledDefinitionFactory.ReportProcessingFlags == ReportProcessingFlags.NotSet || ReportProcessing.ContainsFlag(getCompiledDefinitionFactory.ReportProcessingFlags, ReportProcessingFlags.YukonEngine))
				{
					ChunkFactoryAdapter @object = new ChunkFactoryAdapter(getCompiledDefinitionFactory);
					SubreportCallbackAdapter object2 = new SubreportCallbackAdapter(subReportCallback);
					report = ReportProcessing.DeserializeReport(@object.GetReportChunk);
					ReportProcessing.CheckCredentials(report, dataSources, reportContext, object2.SubReportDataSourcesCallback, allDataSources, 0, checkIfUsable, serverDatasourceSettings, subReportNames);
				}
				else
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.Report report2 = ReportProcessing.DeserializeKatmaiReport(getCompiledDefinitionFactory);
					ReportProcessing.CheckCredentialsOdp(report2, dataSources, dataSetReferences, reportContext, subReportCallback, allDataSources, allDataSetReferences, 0, checkIfUsable, serverDatasourceSettings, subReportNames);
				}
			}
			catch (RSException)
			{
				throw;
			}
			catch (Exception innerException)
			{
				throw new ReportProcessingException(innerException, null);
			}
		}

		public ParameterInfoCollection GetSnapshotParameters(IChunkFactory getReportChunkFactory)
		{
			ReportProcessingFlags reportProcessingFlags = getReportChunkFactory.ReportProcessingFlags;
			if (reportProcessingFlags != 0 && !ReportProcessing.ContainsFlag(reportProcessingFlags, ReportProcessingFlags.YukonEngine))
			{
				return this.GetOdpSnapshotParameters(getReportChunkFactory);
			}
			ChunkFactoryAdapter @object = new ChunkFactoryAdapter(getReportChunkFactory);
			return this.GetYukonSnapshotParameters(@object.GetReportChunk);
		}

		private ParameterInfoCollection GetOdpSnapshotParameters(IChunkFactory chunkFactory)
		{
			OnDemandMetadata onDemandMetadata = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager.DeserializeOnDemandMetadata(chunkFactory, null);
			return onDemandMetadata.ReportSnapshot.Parameters;
		}

		private ParameterInfoCollection GetYukonSnapshotParameters(GetReportChunk getReportChunkCallback)
		{
			try
			{
				Stream stream = null;
				try
				{
					string text = default(string);
					stream = getReportChunkCallback("Main", ReportChunkTypes.Main, out text);
					AspNetCore.ReportingServices.ReportProcessing.Persistence.IntermediateFormatReader intermediateFormatReader = new AspNetCore.ReportingServices.ReportProcessing.Persistence.IntermediateFormatReader(stream);
					return intermediateFormatReader.ReadSnapshotParameters();
				}
				finally
				{
					if (stream != null)
					{
						stream.Close();
					}
				}
			}
			catch (RSException)
			{
				throw;
			}
			catch (Exception innerException)
			{
				throw new ReportProcessingException(innerException, null);
			}
		}

		public ProcessingMessageList ProcessReportParameters(DateTime executionTimeStamp, AspNetCore.ReportingServices.ReportProcessing.ProcessingContext pc, bool isSnapshot, out bool needsUpgrade)
		{
			needsUpgrade = false;
			bool flag = true;
			ErrorContext errorContext = new ProcessingErrorContext();
			ProcessingContext processingContext = null;
			Report report = null;
			OnDemandProcessingContext onDemandProcessingContext = null;
			AspNetCore.ReportingServices.ReportIntermediateFormat.Report report2 = null;
			CultureInfo cultureInfo = null;
			if (pc.Parameters.IsAnyParameterDynamic)
			{
				if (ReportProcessing.ContainsFlag(pc.ReportProcessingFlags, ReportProcessingFlags.YukonEngine) || pc.ReportProcessingFlags == ReportProcessingFlags.NotSet)
				{
					flag = false;
					ChunkFactoryAdapter @object = new ChunkFactoryAdapter(pc.ChunkFactory);
					DateTime dateTime = default(DateTime);
					report = ((!isSnapshot) ? ReportProcessing.DeserializeReport(@object.GetReportChunk) : ReportProcessing.DeserializeReportFromSnapshot((GetReportChunk)@object.GetReportChunk, out dateTime));
				}
				else
				{
					flag = true;
					report2 = ReportProcessing.DeserializeKatmaiReport(pc.ChunkFactory);
				}
			}
			if (flag)
			{
				onDemandProcessingContext = new OnDemandProcessingContext(pc, report2, errorContext, executionTimeStamp, isSnapshot, this.m_configuration);
			}
			else
			{
				processingContext = pc.ParametersInternalProcessingContext(errorContext, executionTimeStamp, isSnapshot);
			}
			try
			{
				cultureInfo = Thread.CurrentThread.CurrentCulture;
				Thread.CurrentThread.CurrentCulture = Localization.ClientPrimaryCulture;
				ProcessingMessageList processingMessageList = null;
				if (flag)
				{
					return ReportProcessing.ProcessReportParameters(report2, onDemandProcessingContext, pc.Parameters);
				}
				return ReportProcessing.ProcessReportParameters(report, processingContext, pc.Parameters);
			}
			catch (RSException)
			{
				throw;
			}
			catch (Exception innerException)
			{
				throw new ReportProcessingException(innerException, errorContext.Messages);
			}
			finally
			{
				if (flag)
				{
					onDemandProcessingContext.UnregisterAbortInfo();
					pc.Parameters.UserProfileState |= onDemandProcessingContext.HasUserProfileState;
				}
				else
				{
					processingContext.AbortInfo.Dispose();
					processingContext.AbortInfo = null;
					pc.Parameters.UserProfileState |= processingContext.HasUserProfileState;
				}
				if (cultureInfo != null)
				{
					Thread.CurrentThread.CurrentCulture = cultureInfo;
				}
			}
		}

		internal static bool ContainsFlag(ReportProcessingFlags processingFlags, ReportProcessingFlags flag)
		{
			return (processingFlags & flag) == flag;
		}

		internal static bool IsYukonProcessingEngine(ReportProcessingFlags processingFlags)
		{
			if (processingFlags != 0)
			{
				return ReportProcessing.ContainsFlag(processingFlags, ReportProcessingFlags.YukonEngine);
			}
			return true;
		}

		internal static void RequestErrorGroupTreeCleanup(OnDemandProcessingContext odpContext)
		{
			if (odpContext != null && odpContext.OdpMetadata != null)
			{
				odpContext.OdpMetadata.DisposePersistedTreeScalability();
			}
		}

		internal static void CleanupOnDemandProcessing(OnDemandProcessingContext topLevelOdpContext, bool resetGroupTreeStorage)
		{
			if (topLevelOdpContext != null)
			{
				topLevelOdpContext.FreeAllResources();
				OnDemandMetadata odpMetadata = topLevelOdpContext.OdpMetadata;
				if (odpMetadata != null)
				{
					if (odpMetadata.OdpChunkManager != null)
					{
						topLevelOdpContext.OdpMetadata.OdpChunkManager.SetOdpContext(topLevelOdpContext);
					}
					if (odpMetadata.GroupTreeScalabilityCache != null)
					{
						ReportProcessing.UpdateExecutionLogContextForTreeCache(topLevelOdpContext, odpMetadata, odpMetadata.GroupTreeScalabilityCache);
					}
					if (odpMetadata.LookupScalabilityCache != null)
					{
						ReportProcessing.UpdateExecutionLogContextForTreeCache(topLevelOdpContext, odpMetadata, odpMetadata.LookupScalabilityCache);
					}
				}
				if (resetGroupTreeStorage)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager.PreparePartitionedTreesForAsyncSerialization(topLevelOdpContext);
				}
			}
		}

		private static void UpdateExecutionLogContextForTreeCache(OnDemandProcessingContext topLevelOdpContext, OnDemandMetadata odpMetadata, PartitionedTreeScalabilityCache cache)
		{
			ExecutionLogContext executionLogContext = topLevelOdpContext.ExecutionLogContext;
			long num = cache.SerializationDurationMs;
			if (odpMetadata.IsInitialProcessingRequest)
			{
				num += cache.DeserializationDurationMs;
			}
			executionLogContext.UpdateForTreeScaleCache(num, cache.PeakMemoryUsageKBytes);
		}

		internal static void UpdateHostingEnvironment(ErrorContext errorContext, ICatalogItemContext itemContext, ExecutionLogContext executionLogContext, ProcessingEngine processingEngine, IJobContext jobContext)
		{
			ReportProcessing.UpdateHostingEnvironment(errorContext, itemContext, executionLogContext, processingEngine, jobContext, null);
		}

		internal static void UpdateHostingEnvironment(ErrorContext errorContext, ICatalogItemContext itemContext, ExecutionLogContext executionLogContext, ProcessingEngine processingEngine, IJobContext jobContext, string sharedDataSetMessage)
		{
			if (jobContext != null)
			{
				Global.Tracer.Assert(null != executionLogContext, "ExecutionLogContext must not be null");
				executionLogContext.StopAllRunningTimers();
				long reportProcessingDurationMsNormalized = executionLogContext.ReportProcessingDurationMsNormalized;
				long dataProcessingDurationMsNormalized = executionLogContext.DataProcessingDurationMsNormalized;
				long reportRenderingDurationMsNormalized = executionLogContext.ReportRenderingDurationMsNormalized;
				long processingScalabilityDurationMsNormalized = executionLogContext.ProcessingScalabilityDurationMsNormalized;
				lock (jobContext.SyncRoot)
				{
					if (0 != dataProcessingDurationMsNormalized)
					{
						jobContext.TimeDataRetrieval += TimeSpan.FromMilliseconds((double)dataProcessingDurationMsNormalized);
					}
					if (0 != reportProcessingDurationMsNormalized)
					{
						jobContext.TimeProcessing += TimeSpan.FromMilliseconds((double)reportProcessingDurationMsNormalized);
					}
					if (0 != reportRenderingDurationMsNormalized)
					{
						jobContext.TimeRendering += TimeSpan.FromMilliseconds((double)reportRenderingDurationMsNormalized);
					}
					if (jobContext.AdditionalInfo.ScalabilityTime == null)
					{
						jobContext.AdditionalInfo.ScalabilityTime = new ScaleTimeCategory();
					}
					jobContext.AdditionalInfo.ScalabilityTime.Processing = executionLogContext.ProcessingScalabilityDurationMsNormalized;
					if (jobContext.AdditionalInfo.EstimatedMemoryUsageKB == null)
					{
						jobContext.AdditionalInfo.EstimatedMemoryUsageKB = new EstimatedMemoryUsageKBCategory();
					}
					jobContext.AdditionalInfo.EstimatedMemoryUsageKB.Processing = executionLogContext.PeakProcesssingMemoryUsage;
					if (sharedDataSetMessage != null)
					{
						jobContext.AdditionalInfo.SharedDataSet = sharedDataSetMessage;
					}
					else
					{
						AdditionalInfo additionalInfo = jobContext.AdditionalInfo;
						int num = (int)processingEngine;
						additionalInfo.ProcessingEngine = num.ToString(CultureInfo.InvariantCulture);
					}
					if (executionLogContext.ExternalImageCount > 0)
					{
						ExternalImageCategory externalImageCategory = new ExternalImageCategory();
						externalImageCategory.Count = executionLogContext.ExternalImageCount.ToString(CultureInfo.InvariantCulture);
						externalImageCategory.ByteCount = executionLogContext.ExternalImageBytes.ToString(CultureInfo.InvariantCulture);
						externalImageCategory.ResourceFetchTime = executionLogContext.ExternalImageDurationMs.ToString(CultureInfo.InvariantCulture);
						jobContext.AdditionalInfo.ExternalImages = externalImageCategory;
					}
					jobContext.AdditionalInfo.Connections = executionLogContext.GetConnectionMetrics();
				}
			}
			ReportProcessing.TraceProcessingMessages(errorContext, itemContext);
		}

		internal static void TraceProcessingMessages(ErrorContext errorContext, ICatalogItemContext itemContext)
		{
			if (errorContext != null && errorContext.Messages != null)
			{
				ProcessingMessageList messages = errorContext.Messages;
				int count = messages.Count;
				if (Global.Tracer.TraceVerbose && count > 0)
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.AppendFormat("The following messages were generated while processing item: '{0}':", itemContext.ItemPathAsString.MarkAsPrivate());
					for (int i = 0; i < count; i++)
					{
						stringBuilder.AppendLine();
						stringBuilder.Append("\t");
						stringBuilder.Append(messages[i].FormatMessage());
					}
					Global.Tracer.Trace(TraceLevel.Verbose, stringBuilder.ToString());
				}
			}
		}

		internal static void UpdateEventInfo(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSnapshot odpReportSnapshot, OnDemandProcessingContext odpContext, AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext odpRenderingContext, ref bool eventInfoChanged)
		{
			if (odpReportSnapshot != null)
			{
				if (odpContext.NewSortFilterEventInfo != null && odpContext.NewSortFilterEventInfo.Count > 0)
				{
					odpReportSnapshot.SortFilterEventInfo = odpContext.NewSortFilterEventInfo;
				}
				else
				{
					odpReportSnapshot.SortFilterEventInfo = null;
				}
			}
			eventInfoChanged |= odpRenderingContext.EventInfoChanged;
		}

		internal static void UpdateEventInfo(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSnapshot odpReportSnapshot, OnDemandProcessingContext odpContext, AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext odpRenderingContext, RenderingContext rc, ref bool eventInfoChanged)
		{
			ReportProcessing.UpdateEventInfo(odpReportSnapshot, odpContext, odpRenderingContext, ref eventInfoChanged);
			if (eventInfoChanged)
			{
				rc.EventInfo = odpRenderingContext.EventInfo;
			}
		}

		internal static void HandleRenderingException(ReportRenderingException rex)
		{
			if (rex.InnerException != null && (rex.InnerException is RSException || rex.InnerException is DataCacheUnavailableException))
			{
				if (rex.InnerException is RSException)
				{
					throw new RSException((RSException)rex.InnerException);
				}
				throw new DataCacheUnavailableException((DataCacheUnavailableException)rex.InnerException);
			}
			if (rex.Unexpected)
			{
				throw new UnhandledReportRenderingException(rex);
			}
			throw new HandledReportRenderingException(rex);
		}

		private Report CompileYukonReport(ICatalogItemContext reportContext, byte[] reportDefinition, CreateReportChunk createChunkCallback, CheckSharedDataSource checkDataSourceCallback, ResolveTemporaryDataSource resolveTemporaryDataSourceCallback, DataSourceInfoCollection originalDataSources, PublishingErrorContext errorContext, AppDomain compilationTempAppDomain, bool generateExpressionHostWithRefusedPermissions, IDataProtection dataProtection, out string reportDescription, out string reportLanguage, out ParameterInfoCollection parameters, out DataSourceInfoCollection dataSources, out UserLocationFlags userReferenceLocation, out ArrayList dataSetsName, out bool hasExternalImages, out bool hasHyperlinks)
		{
			ReportPublishing reportPublishing = new ReportPublishing();
			Report report = reportPublishing.CreateIntermediateFormat(reportContext, reportDefinition, createChunkCallback, checkDataSourceCallback, resolveTemporaryDataSourceCallback, originalDataSources, errorContext, compilationTempAppDomain, generateExpressionHostWithRefusedPermissions, dataProtection, out reportDescription, out reportLanguage, out parameters, out dataSources, out userReferenceLocation, out dataSetsName, out hasExternalImages, out hasHyperlinks);
			if (createChunkCallback != null)
			{
				ReportProcessing.SerializeReport(report, createChunkCallback);
			}
			return report;
		}

		internal ReportSnapshot ProcessReport(Report report, AspNetCore.ReportingServices.ReportProcessing.ProcessingContext pc, bool snapshotProcessing, bool processWithCachedData, GetReportChunk getChunkCallback, ErrorContext errorContext, DateTime executionTime, CreateReportChunk cacheDataCallback, out ProcessingContext context, out UserProfileState userProfileState)
		{
			context = pc.CreateInternalProcessingContext(null, report, errorContext, executionTime, pc.AllowUserProfileState, pc.IsHistorySnapshot, snapshotProcessing, processWithCachedData, getChunkCallback, cacheDataCallback);
			context.CreateReportChunkFactory = pc.ChunkFactory;
			return this.ProcessReport(report, pc, context, out userProfileState);
		}

		private bool HasUserSortFilter(Report report, uint subreportLevel, ProcessingContext context)
		{
			if (report == null)
			{
				return false;
			}
			if (report.HasUserSortFilter)
			{
				return true;
			}
			if (context.SubReportCallback == null)
			{
				return true;
			}
			if (subreportLevel <= 20 && report.SubReports != null)
			{
				int count = report.SubReports.Count;
				for (int i = 0; i < count; i++)
				{
					SubReport subReport = report.SubReports[i];
					if (subReport.RetrievalStatus == SubReport.Status.NotRetrieved && context.SubReportCallback != null)
					{
						RuntimeRICollection.RetrieveSubReport(subReport, context, null, true);
					}
					if (subReport.Report != null && this.HasUserSortFilter(subReport.Report, subreportLevel + 1, context))
					{
						return true;
					}
				}
			}
			return false;
		}

		internal static void FetchSubReports(AspNetCore.ReportingServices.ReportIntermediateFormat.Report report, IChunkFactory getReportChunks, ErrorContext errorContext, OnDemandMetadata odpMetadata, ICatalogItemContext parentReportContext, OnDemandSubReportCallback subReportCallback, int subReportLevel, bool snapshotProcessing, bool processWithCachedData, GlobalIDOwnerCollection globalIDOwnerCollection, ParameterInfoCollection parentQueryParameters)
		{
			if ((long)subReportLevel > 20L)
			{
				foreach (AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport subReport in report.SubReports)
				{
					subReport.ExceededMaxLevel = true;
				}
			}
			else
			{
				foreach (AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport subReport2 in report.SubReports)
				{
					try
					{
						string reportName = subReport2.ReportName;
						SubReportInfo subReportInfo = default(SubReportInfo);
						if (processWithCachedData)
						{
							if (!odpMetadata.TryGetSubReportInfo(subReportLevel == 0, subReport2.SubReportDefinitionPath, reportName, out subReportInfo))
							{
								throw new DataCacheUnavailableException();
							}
						}
						else if (!snapshotProcessing)
						{
							subReport2.OriginalCatalogPath = parentReportContext.MapUserProvidedPath(subReport2.ReportName);
							subReportInfo = odpMetadata.AddSubReportInfo(subReportLevel == 0, subReport2.SubReportDefinitionPath, reportName, subReport2.OriginalCatalogPath);
						}
						else
						{
							subReportInfo = odpMetadata.GetSubReportInfo(subReportLevel == 0, subReport2.SubReportDefinitionPath, reportName);
							if (subReportInfo != null && subReportInfo.CommonSubReportInfo != null)
							{
								subReport2.OriginalCatalogPath = subReportInfo.CommonSubReportInfo.OriginalCatalogPath;
							}
						}
						ReportProcessing.DeserializeKatmaiSubReport(subReport2, getReportChunks, parentReportContext, subReportCallback, subReportInfo, snapshotProcessing, errorContext, globalIDOwnerCollection, processWithCachedData, parentQueryParameters);
					}
					catch (DataCacheUnavailableException)
					{
						throw;
					}
					catch (Exception e)
					{
						subReport2.RetrievalStatus = AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport.Status.DefinitionRetrieveFailed;
						ReportProcessing.HandleSubReportProcessingError(errorContext, subReport2, "0", null, e);
					}
					if (subReport2.Report != null)
					{
						if (subReport2.Report.HasSubReports)
						{
							ReportProcessing.FetchSubReports(subReport2.Report, getReportChunks, errorContext, odpMetadata, subReport2.ReportContext, subReportCallback, subReportLevel + 1, snapshotProcessing, processWithCachedData, globalIDOwnerCollection, parentQueryParameters);
						}
						report.ReportOrDescendentHasUserSortFilter |= subReport2.Report.ReportOrDescendentHasUserSortFilter;
					}
				}
			}
		}

		private static void DeserializeKatmaiSubReport(AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport subReport, IChunkFactory getReportChunks, ICatalogItemContext reportContext, OnDemandSubReportCallback subReportCallback, SubReportInfo subReportInfo, bool snapshotProcessing, ErrorContext errorContext, GlobalIDOwnerCollection globalIDOwnerCollection, bool processWithCachedData, ParameterInfoCollection parentQueryParameters)
		{
			CommonSubReportInfo commonSubReportInfo = subReportInfo.CommonSubReportInfo;
			try
			{
				IChunkFactory chunkFactory = default(IChunkFactory);
				string chunkName;
				if (commonSubReportInfo.DefinitionChunkFactory == null)
				{
					if (snapshotProcessing)
					{
						chunkFactory = getReportChunks;
						chunkName = commonSubReportInfo.DefinitionUniqueName;
						subReport.ReportContext = reportContext.GetSubreportContext(commonSubReportInfo.ReportPath);
						if (commonSubReportInfo.RetrievalFailed)
						{
							subReport.RetrievalStatus = AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport.Status.DefinitionRetrieveFailed;
							goto end_IL_0009;
						}
						goto IL_012a;
					}
					ICatalogItemContext reportContext2 = default(ICatalogItemContext);
					string description = default(string);
					ParameterInfoCollection parametersFromCatalog = default(ParameterInfoCollection);
					subReportCallback(reportContext, subReport.ReportName, commonSubReportInfo.DefinitionUniqueName, ReportProcessing.NeedsUpgradeImpl, parentQueryParameters, out reportContext2, out description, out chunkFactory, out parametersFromCatalog);
					if (chunkFactory != null)
					{
						if (!ReportProcessing.ContainsFlag(chunkFactory.ReportProcessingFlags, ReportProcessingFlags.OnDemandEngine))
						{
							Global.Tracer.Trace(TraceLevel.Warning, "The subreport '{0}' could not be processed within parent report '{1}' due to a mismatch in execution engines. Either the subreport failed to automatically republish, or the subreport contains a Reporting Services 2005-style CustomReportItem. To correct this error, please attempt to republish the subreport manually. If it contains a CustomReportItem, please upgrade the report to the latest version.", subReport.ReportName.MarkAsPrivate(), reportContext.ItemPathAsString.MarkAsPrivate());
							errorContext.Register(ProcessingErrorCode.rsEngineMismatchSubReport, Severity.Warning, subReport.ObjectType, subReport.Name, null, subReport.Name, reportContext.ItemPathAsString.MarkAsPrivate());
							subReport.RetrievalStatus = AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport.Status.DefinitionRetrieveFailed;
						}
						chunkName = "CompiledDefinition";
						commonSubReportInfo.ParametersFromCatalog = parametersFromCatalog;
						commonSubReportInfo.Description = description;
						subReport.ReportContext = reportContext2;
						goto IL_012a;
					}
					goto end_IL_0009;
				}
				chunkName = ((!snapshotProcessing) ? "CompiledDefinition" : commonSubReportInfo.DefinitionUniqueName);
				chunkFactory = commonSubReportInfo.DefinitionChunkFactory;
				subReport.ReportContext = reportContext.GetSubreportContext(commonSubReportInfo.ReportPath);
				if (commonSubReportInfo.RetrievalFailed)
				{
					subReport.RetrievalStatus = AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport.Status.DefinitionRetrieveFailed;
					goto end_IL_0009;
				}
				goto IL_0177;
				IL_012a:
				commonSubReportInfo.DefinitionChunkFactory = chunkFactory;
				goto IL_0177;
				IL_0177:
				subReport.ParametersFromCatalog = commonSubReportInfo.ParametersFromCatalog;
				subReport.Description = commonSubReportInfo.Description;
				subReport.Report = ReportProcessing.DeserializeKatmaiReport(chunkFactory, chunkName, snapshotProcessing, globalIDOwnerCollection, subReport, subReport);
				subReport.UpdateSubReportEventSourceGlobalDataSetIds(subReportInfo);
				subReport.RetrievalStatus = AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport.Status.DefinitionRetrieved;
				end_IL_0009:;
			}
			catch (IncompatibleFormatVersionException)
			{
				Global.Tracer.Assert(false, "IncompatibleFormatVersion");
			}
			catch (Exception)
			{
				commonSubReportInfo.RetrievalFailed = true;
				subReport.RetrievalStatus = AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport.Status.DefinitionRetrieveFailed;
				throw;
			}
		}

		private static bool NeedsUpgradeImpl(ReportProcessingFlags flags)
		{
			if (flags == ReportProcessingFlags.NotSet)
			{
				return true;
			}
			return false;
		}

		internal static void HandleSubReportProcessingError(ErrorContext errorContext, AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport subReport, string instanceID, ErrorContext subReportErrorContext, Exception e)
		{
			if (!(e is DataCacheUnavailableException) && !(e.InnerException is DataCacheUnavailableException))
			{
				if (e is ItemNotFoundException)
				{
					if (Global.Tracer.TraceError)
					{
						Global.Tracer.Trace(TraceLevel.Error, "An error has occurred while processing a sub-report.  The report definition could not be retrieved. Details: {0}", e.Message);
					}
				}
				else if (!(e is ProcessingAbortedException) && Global.Tracer.TraceError)
				{
					Global.Tracer.Trace(TraceLevel.Error, "An error has occurred while processing a sub-report. Details: {0} Stack trace:\r\n{1}", e.Message, e.StackTrace);
				}
				if (subReportErrorContext == null)
				{
					errorContext.Register(ProcessingErrorCode.rsErrorExecutingSubreport, Severity.Warning, subReport.ObjectType, subReport.Name, instanceID, e.Message);
				}
				else
				{
					if (e is RSException)
					{
						subReportErrorContext.Register((RSException)e, subReport.ObjectType);
					}
					errorContext.Register(ProcessingErrorCode.rsErrorExecutingSubreport, Severity.Warning, subReport.ObjectType, subReport.Name, instanceID, subReportErrorContext.Messages, e.Message);
				}
				return;
			}
			throw e;
		}

		private ReportSnapshot ProcessReport(Report report, AspNetCore.ReportingServices.ReportProcessing.ProcessingContext pc, ProcessingContext context, out UserProfileState userProfileState)
		{
			ReportSnapshot reportSnapshot = null;
			CultureInfo cultureInfo = null;
			bool flag = this.HasUserSortFilter(report, 0u, context);
			if ((pc.InitialUserProfileState & UserProfileState.InQuery) > UserProfileState.None)
			{
				context.SaveSnapshotData = flag;
				context.StopSaveSnapshotDataOnError = !flag;
			}
			else if ((pc.InitialUserProfileState & UserProfileState.InReport) == UserProfileState.None)
			{
				context.SaveSnapshotData = (report.ParametersNotUsedInQuery || flag);
				context.StopSaveSnapshotDataOnError = !flag;
			}
			if (pc.IsHistorySnapshot)
			{
				context.SaveSnapshotData = true;
				context.StopSaveSnapshotDataOnError = false;
			}
			userProfileState = UserProfileState.None;
			try
			{
				reportSnapshot = new ReportSnapshot(report, pc.ReportContext.ItemName, pc.Parameters, pc.RequestUserName, context.ExecutionTime, pc.ReportContext.HostRootUri, pc.ReportContext.ParentPath, pc.UserLanguage.Name);
				cultureInfo = Thread.CurrentThread.CurrentCulture;
				Merge merge = new Merge(report, context);
				if (!context.SnapshotProcessing && !context.ProcessWithCachedData && merge.PrefetchData(pc.Parameters, false))
				{
					context.SnapshotProcessing = true;
					context.ResetForSubreportDataPrefetch = true;
				}
				reportSnapshot.ReportInstance = merge.Process(pc.Parameters, false);
				userProfileState = context.HasUserProfileState;
				ReportDrillthroughInfo drillthroughInfo = context.DrillthroughInfo;
				PageMergeInteractive pageMergeInteractive = new PageMergeInteractive();
				userProfileState |= pageMergeInteractive.Process(context.PageSectionContext.PageTextboxes, reportSnapshot, pc.ReportContext, pc.ReportContext.ItemName, pc.Parameters, context.ChunkManager, pc.CreateReportChunkCallback, pc.GetResourceCallback, context.ErrorContext, context.AllowUserProfileState, context.ReportRuntimeSetup, context.UniqueNameCounter, pc.DataProtection, ref drillthroughInfo);
				merge.CleanupDataChunk(userProfileState);
				SenderInformationHashtable senderInformationHashtable = default(SenderInformationHashtable);
				ReceiverInformationHashtable showHideReceiverInfo = default(ReceiverInformationHashtable);
				context.GetSenderAndReceiverInfo(out senderInformationHashtable, out showHideReceiverInfo);
				reportSnapshot.ShowHideSenderInfo = senderInformationHashtable;
				if (senderInformationHashtable != null || context.HasUserSortFilter)
				{
					reportSnapshot.HasShowHide = true;
				}
				reportSnapshot.ShowHideReceiverInfo = showHideReceiverInfo;
				if (context.QuickFind != null && context.QuickFind.Count > 0)
				{
					reportSnapshot.QuickFind = context.QuickFind;
				}
				else
				{
					reportSnapshot.QuickFind = null;
				}
				if (drillthroughInfo != null && drillthroughInfo.Count > 0)
				{
					reportSnapshot.DrillthroughInfo = drillthroughInfo;
				}
				else
				{
					reportSnapshot.DrillthroughInfo = null;
				}
				reportSnapshot.CreateNavigationActions(context.NavigationInfo);
				if (context.HasImageStreams || report.HasImageStreams)
				{
					reportSnapshot.HasImageStreams = true;
				}
				if (context.NewSortFilterEventInfo != null && context.NewSortFilterEventInfo.Count > 0)
				{
					reportSnapshot.SortFilterEventInfo = context.NewSortFilterEventInfo;
				}
				else
				{
					reportSnapshot.SortFilterEventInfo = null;
				}
				context.ChunkManager.PageSectionFlush(reportSnapshot);
				context.ChunkManager.FinalFlush();
				report.MainChunkSize = context.ChunkManager.TotalCount * 50;
				reportSnapshot.Warnings = context.ErrorContext.Messages;
				return reportSnapshot;
			}
			finally
			{
				if (cultureInfo != null)
				{
					Thread.CurrentThread.CurrentCulture = cultureInfo;
				}
				foreach (DataSourceInfo value in context.GlobalDataSourceInfo.Values)
				{
					if (value.TransactionInfo != null)
					{
						if (value.TransactionInfo.RollbackRequired)
						{
							if (Global.Tracer.TraceInfo)
							{
								Global.Tracer.Trace(TraceLevel.Info, "Data source '{0}': Rolling back transaction.", value.DataSourceName.MarkAsModelInfo());
							}
							try
							{
								value.TransactionInfo.Transaction.Rollback();
							}
							catch (Exception innerException)
							{
								throw new ReportProcessingException(ErrorCode.rsErrorRollbackTransaction, innerException, value.DataSourceName);
							}
						}
						else
						{
							if (Global.Tracer.TraceVerbose)
							{
								Global.Tracer.Trace(TraceLevel.Verbose, "Data source '{0}': Committing transaction.", value.DataSourceName.MarkAsModelInfo());
							}
							try
							{
								value.TransactionInfo.Transaction.Commit();
							}
							catch (Exception innerException2)
							{
								throw new ReportProcessingException(ErrorCode.rsErrorCommitTransaction, innerException2, value.DataSourceName.MarkAsModelInfo());
							}
						}
					}
					if (value.Connection != null)
					{
						try
						{
							pc.CreateAndSetupDataExtensionFunction.CloseConnectionWithoutPool(value.Connection);
						}
						catch (Exception innerException3)
						{
							throw new ReportProcessingException(ErrorCode.rsErrorClosingConnection, innerException3, value.DataSourceName);
						}
					}
				}
				if (context != null && context.ChunkManager != null)
				{
					context.ChunkManager.Close();
				}
				context.AbortInfo.Dispose();
				context.AbortInfo = null;
			}
		}

		internal static ProcessingMessageList ProcessReportParameters(Report report, ProcessingContext mergeContext, ParameterInfoCollection parameters)
		{
			LegacyProcessReportParameters legacyProcessReportParameters = new LegacyProcessReportParameters(report, (ReportProcessingContext)mergeContext);
			return legacyProcessReportParameters.Process(parameters);
		}

		internal static ProcessingMessageList ProcessReportParameters(AspNetCore.ReportingServices.ReportIntermediateFormat.Report report, OnDemandProcessingContext context, ParameterInfoCollection parameters)
		{
			OnDemandProcessReportParameters onDemandProcessReportParameters = new OnDemandProcessReportParameters(report, context);
			return onDemandProcessReportParameters.Process(parameters);
		}

		private static AspNetCore.ReportingServices.ReportIntermediateFormat.Report DeserializeKatmaiReport(IChunkFactory chunkFactory)
		{
			return ReportProcessing.DeserializeKatmaiReport(chunkFactory, "CompiledDefinition", false, null, null, null);
		}

		internal static AspNetCore.ReportingServices.ReportIntermediateFormat.Report DeserializeKatmaiReport(IChunkFactory chunkFactory, bool keepReferences, GlobalIDOwnerCollection globalIDOwnerCollection)
		{
			return ReportProcessing.DeserializeKatmaiReport(chunkFactory, "CompiledDefinition", keepReferences, globalIDOwnerCollection, null, null);
		}

		private static AspNetCore.ReportingServices.ReportIntermediateFormat.Report DeserializeKatmaiReport(IChunkFactory chunkFactory, string chunkName, bool keepReferences, GlobalIDOwnerCollection globalIDOwnerCollection, AspNetCore.ReportingServices.ReportIntermediateFormat.IDOwner parentIDOwner, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem parentReportItem)
		{
			Stream stream = null;
			try
			{
				string text = default(string);
				stream = chunkFactory.GetChunk(chunkName, ReportChunkTypes.CompiledDefinition, ChunkMode.Open, out text);
				return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.DeserializeReport(keepReferences, globalIDOwnerCollection, parentIDOwner, parentReportItem, stream);
			}
			finally
			{
				if (stream != null)
				{
					stream.Close();
				}
			}
		}

		private static Report DeserializeReport(GetReportChunk getChunkCallback)
		{
			return ReportProcessing.DeserializeReport(getChunkCallback, null);
		}

		internal static Report DeserializeReport(GetReportChunk getChunkCallback, out Hashtable definitionObjects)
		{
			return ReportProcessing.DeserializeReport(getChunkCallback, (ReportItem)null, out definitionObjects);
		}

		private static Report DeserializeReport(GetReportChunk getChunkCallback, ReportItem parent)
		{
			Hashtable hashtable = null;
			return ReportProcessing.DeserializeReport(getChunkCallback, parent, out hashtable);
		}

		private static Report DeserializeReport(GetReportChunk getChunkCallback, ReportItem parent, out Hashtable definitionObjects)
		{
			Stream stream = null;
			try
			{
				string text = default(string);
				stream = getChunkCallback("CompiledDefinition", ReportChunkTypes.Main, out text);
				AspNetCore.ReportingServices.ReportProcessing.Persistence.IntermediateFormatReader intermediateFormatReader = new AspNetCore.ReportingServices.ReportProcessing.Persistence.IntermediateFormatReader(stream);
				Report report = intermediateFormatReader.ReadReport(parent);
				definitionObjects = intermediateFormatReader.DefinitionObjects;
				if (report.IntermediateFormatVersion.IsOldVersion)
				{
					Upgrader.UpgradeToCurrent(report);
					Upgrader.UpgradeDatasetIDs(report);
				}
				return report;
			}
			finally
			{
				if (stream != null)
				{
					stream.Close();
				}
			}
		}

		private static Report DeserializeReportFromSnapshot(GetReportChunk getChunkCallback, out DateTime executionTime)
		{
			Hashtable hashtable = default(Hashtable);
			IntermediateFormatVersion intermediateFormatVersion = default(IntermediateFormatVersion);
			return ReportProcessing.DeserializeReportFromSnapshot(getChunkCallback, out executionTime, out hashtable, out intermediateFormatVersion);
		}

		internal static Report DeserializeReportFromSnapshot(GetReportChunk getChunkCallback, out DateTime executionTime, out Hashtable definitionObjects)
		{
			IntermediateFormatVersion intermediateFormatVersion = default(IntermediateFormatVersion);
			return ReportProcessing.DeserializeReportFromSnapshot(getChunkCallback, out executionTime, out definitionObjects, out intermediateFormatVersion);
		}

		private static Report DeserializeReportFromSnapshot(GetReportChunk getChunkCallback, out DateTime executionTime, out Hashtable definitionObjects, out IntermediateFormatVersion intermediateFormatVersion)
		{
			Stream stream = null;
			try
			{
				string text = default(string);
				stream = getChunkCallback("Main", ReportChunkTypes.Main, out text);
				AspNetCore.ReportingServices.ReportProcessing.Persistence.IntermediateFormatReader intermediateFormatReader = new AspNetCore.ReportingServices.ReportProcessing.Persistence.IntermediateFormatReader(stream);
				Report report = intermediateFormatReader.ReadReportFromSnapshot(out executionTime);
				report.MainChunkSize = stream.Length;
				definitionObjects = intermediateFormatReader.DefinitionObjects;
				intermediateFormatVersion = intermediateFormatReader.IntermediateFormatVersion;
				if (report.IntermediateFormatVersion.IsOldVersion)
				{
					Upgrader.UpgradeToCurrent(report);
					Upgrader.UpgradeDatasetIDs(report);
				}
				return report;
			}
			finally
			{
				if (stream != null)
				{
					stream.Close();
				}
			}
		}

		private static void SerializeReport(Report report, CreateReportChunk createChunkCallback)
		{
			Stream stream = null;
			try
			{
				stream = createChunkCallback("CompiledDefinition", ReportChunkTypes.Main, null);
				AspNetCore.ReportingServices.ReportProcessing.Persistence.IntermediateFormatWriter intermediateFormatWriter = new AspNetCore.ReportingServices.ReportProcessing.Persistence.IntermediateFormatWriter(stream, true);
				intermediateFormatWriter.WriteReport(report);
			}
			finally
			{
				if (stream != null)
				{
					stream.Close();
				}
			}
		}

		private static void SerializeReport(AspNetCore.ReportingServices.ReportIntermediateFormat.Report report, IChunkFactory chunkFactory, IConfiguration configuration)
		{
			Stream stream = null;
			try
			{
				stream = chunkFactory.CreateChunk("CompiledDefinition", ReportChunkTypes.CompiledDefinition, null);
				AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.SerializeReport(report, stream, configuration);
			}
			finally
			{
				if (stream != null)
				{
					stream.Close();
				}
			}
		}

		internal static ReportSnapshot DeserializeReportSnapshot(GetReportChunk getChunkCallback, CreateReportChunk createChunkCallback, IGetResource getResourceCallback, RenderingContext renderingContext, IDataProtection dataProtection, out Hashtable instanceObjects, out Hashtable definitionObjects, out AspNetCore.ReportingServices.ReportProcessing.Persistence.IntermediateFormatReader.State declarationsRead, out bool isOldSnapshot)
		{
			Stream stream = null;
			ChunkManager.RenderingChunkManager renderingChunkManager = null;
			isOldSnapshot = false;
			try
			{
				string text = default(string);
				stream = getChunkCallback("Main", ReportChunkTypes.Main, out text);
				AspNetCore.ReportingServices.ReportProcessing.Persistence.IntermediateFormatReader intermediateFormatReader = new AspNetCore.ReportingServices.ReportProcessing.Persistence.IntermediateFormatReader(stream);
				ReportSnapshot reportSnapshot = intermediateFormatReader.ReadReportSnapshot();
				reportSnapshot.Report.MainChunkSize = stream.Length;
				instanceObjects = intermediateFormatReader.InstanceObjects;
				declarationsRead = intermediateFormatReader.ReaderState;
				definitionObjects = intermediateFormatReader.DefinitionObjects;
				renderingChunkManager = new ChunkManager.RenderingChunkManager(getChunkCallback, instanceObjects, definitionObjects, declarationsRead, reportSnapshot.Report.IntermediateFormatVersion);
				if (reportSnapshot.Report.IntermediateFormatVersion.IsOldVersion)
				{
					Upgrader.UpgradeToCurrent(reportSnapshot, renderingChunkManager, createChunkCallback);
					Upgrader.UpgradeDatasetIDs(reportSnapshot.Report);
					isOldSnapshot = true;
				}
				Upgrader.UpgradeToPageSectionsChunk(reportSnapshot, renderingContext.ReportContext, renderingChunkManager, createChunkCallback, getResourceCallback, renderingContext, dataProtection);
				return reportSnapshot;
			}
			finally
			{
				if (stream != null)
				{
					stream.Close();
				}
				if (renderingChunkManager != null)
				{
					renderingChunkManager.Close();
				}
			}
		}

		internal static int CompareTo(object x, object y, CompareInfo compareInfo, CompareOptions compareOptions)
		{
			bool flag = default(bool);
			return ReportProcessing.CompareTo(x, y, false, compareInfo, compareOptions, true, false, out flag);
		}

		internal static int CompareTo(object x, object y, bool nullsAsBlanks, CompareInfo compareInfo, CompareOptions compareOptions, bool throwExceptionOnComparisonFailure, bool extendedTypeComparisons, out bool validComparisonResult)
		{
			validComparisonResult = true;
			if (x == null && y == null)
			{
				return 0;
			}
			if (x is string && y is string)
			{
				return compareInfo.Compare((string)x, (string)y, compareOptions);
			}
			if (x is int && y is int)
			{
				return ((int)x).CompareTo(y);
			}
			bool flag = default(bool);
			DataAggregate.DataTypeCode typeCode = DataAggregate.GetTypeCode(x, false, out flag);
			bool flag2 = default(bool);
			DataAggregate.DataTypeCode typeCode2 = DataAggregate.GetTypeCode(y, false, out flag2);
			if (flag && flag2)
			{
				if (typeCode == typeCode2)
				{
					IComparable comparable = x as IComparable;
					if (comparable != null)
					{
						return comparable.CompareTo(y);
					}
				}
				DataAggregate.DataTypeCode dataTypeCode = DataTypeUtility.CommonNumericDenominator(typeCode, typeCode2);
				if (dataTypeCode != 0)
				{
					Type numericTypeFromDataTypeCode = DataTypeUtility.GetNumericTypeFromDataTypeCode(dataTypeCode);
					if (DataAggregate.DataTypeCode.Int32 == dataTypeCode)
					{
						int num = (int)Convert.ChangeType(x, numericTypeFromDataTypeCode, CultureInfo.CurrentCulture);
						int value = (int)Convert.ChangeType(y, numericTypeFromDataTypeCode, CultureInfo.CurrentCulture);
						return num.CompareTo(value);
					}
					if (DataAggregate.DataTypeCode.Double == dataTypeCode)
					{
						double num2 = (double)Convert.ChangeType(x, numericTypeFromDataTypeCode, CultureInfo.CurrentCulture);
						double value2 = (double)Convert.ChangeType(y, numericTypeFromDataTypeCode, CultureInfo.CurrentCulture);
						return num2.CompareTo(value2);
					}
					if (DataAggregate.DataTypeCode.Decimal == dataTypeCode)
					{
						decimal num3 = (decimal)Convert.ChangeType(x, numericTypeFromDataTypeCode, CultureInfo.CurrentCulture);
						decimal value3 = (decimal)Convert.ChangeType(y, numericTypeFromDataTypeCode, CultureInfo.CurrentCulture);
						return num3.CompareTo(value3);
					}
					if (DataAggregate.DataTypeCode.UInt32 == dataTypeCode)
					{
						uint num4 = (uint)Convert.ChangeType(x, numericTypeFromDataTypeCode, CultureInfo.CurrentCulture);
						uint value4 = (uint)Convert.ChangeType(y, numericTypeFromDataTypeCode, CultureInfo.CurrentCulture);
						return num4.CompareTo(value4);
					}
					if (DataAggregate.DataTypeCode.Int64 == dataTypeCode)
					{
						long num5 = (long)Convert.ChangeType(x, numericTypeFromDataTypeCode, CultureInfo.CurrentCulture);
						long value5 = (long)Convert.ChangeType(y, numericTypeFromDataTypeCode, CultureInfo.CurrentCulture);
						return num5.CompareTo(value5);
					}
					if (DataAggregate.DataTypeCode.UInt64 == dataTypeCode)
					{
						ulong num6 = (ulong)Convert.ChangeType(x, numericTypeFromDataTypeCode, CultureInfo.CurrentCulture);
						ulong value6 = (ulong)Convert.ChangeType(y, numericTypeFromDataTypeCode, CultureInfo.CurrentCulture);
						return num6.CompareTo(value6);
					}
				}
				if (typeCode == DataAggregate.DataTypeCode.Null && typeCode2 == DataAggregate.DataTypeCode.Null)
				{
					return 0;
				}
				if (typeCode == DataAggregate.DataTypeCode.Null)
				{
					if (nullsAsBlanks && DataTypeUtility.IsNumericLessThanZero(y, typeCode2))
					{
						return 1;
					}
					return -1;
				}
				if (typeCode2 == DataAggregate.DataTypeCode.Null)
				{
					if (nullsAsBlanks && DataTypeUtility.IsNumericLessThanZero(x, typeCode))
					{
						return -1;
					}
					return 1;
				}
				if (extendedTypeComparisons)
				{
					if (typeCode == DataAggregate.DataTypeCode.Int64 && typeCode2 == DataAggregate.DataTypeCode.Double)
					{
						return ReportProcessing.CompareTo((long)x, (double)y);
					}
					if (typeCode == DataAggregate.DataTypeCode.Double && typeCode2 == DataAggregate.DataTypeCode.Int64)
					{
						return ReportProcessing.CompareTo((long)y, (double)x) * -1;
					}
					if (typeCode == DataAggregate.DataTypeCode.Decimal && typeCode2 == DataAggregate.DataTypeCode.Double)
					{
						return ReportProcessing.CompareTo((decimal)x, (double)y);
					}
					if (typeCode == DataAggregate.DataTypeCode.Double && typeCode2 == DataAggregate.DataTypeCode.Decimal)
					{
						return ReportProcessing.CompareTo((decimal)y, (double)x) * -1;
					}
				}
				return ReportProcessing.CompareWithIComparable(x, y, throwExceptionOnComparisonFailure, out validComparisonResult);
			}
			/*Type type = null;
            
			if (x is SqlGeography || x is SqlGeometry)
			{
				type = x.GetType();
			}
			else if (y is SqlGeography || y is SqlGeometry)
			{
				type = y.GetType();
			}
            
			if (type != null)
			{
				if (throwExceptionOnComparisonFailure)
				{
					throw new ReportProcessingException_SpatialTypeComparisonError(type.ToString());
				}
				validComparisonResult = false;
				return -1;
			}
            */
			return ReportProcessing.CompareWithIComparable(x, y, throwExceptionOnComparisonFailure, out validComparisonResult);
		}

		private static int CompareTo(long longVal, double doubleVal)
		{
			if (doubleVal > 9.2233720368547758E+18)
			{
				return -1;
			}
			if (doubleVal < -9.2233720368547758E+18)
			{
				return 1;
			}
			if (double.IsNaN(doubleVal))
			{
				if (longVal < 0)
				{
					return -1;
				}
				return 1;
			}
			long num = (long)doubleVal;
			int num2 = longVal.CompareTo(num);
			if (num2 == 0)
			{
				num2 = ((double)num).CompareTo(doubleVal);
			}
			return num2;
		}

		private static int CompareTo(decimal decimalVal, double doubleVal)
		{
			if (doubleVal > 7.9228162514264338E+28)
			{
				return -1;
			}
			if (doubleVal < -7.9228162514264338E+28)
			{
				return 1;
			}
			if (double.IsNaN(doubleVal))
			{
				if (decimalVal < 0m)
				{
					return -1;
				}
				return 1;
			}
			decimal value = (decimal)doubleVal;
			int num = decimalVal.CompareTo(value);
			if (num == 0)
			{
				num = ((double)value).CompareTo(doubleVal);
			}
			return num;
		}

		private static int CompareWithIComparable(object x, object y, bool throwExceptionOnComparisonFailure, out bool validComparisonResult)
		{
			validComparisonResult = true;
			try
			{
				if (x is IComparable)
				{
					return ((IComparable)x).CompareTo(y);
				}
				if (y is IComparable)
				{
					return -((IComparable)y).CompareTo(x);
				}
			}
			catch (Exception e)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(e))
				{
					throw;
				}
				validComparisonResult = false;
				if (!throwExceptionOnComparisonFailure)
				{
					goto end_IL_0034;
				}
				throw new ReportProcessingException_ComparisonError(x.GetType().ToString(), y.GetType().ToString());
				end_IL_0034:;
			}
			return -1;
		}

		internal static int CompareWithInvariantCulture(string x, string y, bool ignoreCase)
		{
			return string.Compare(x, y, (StringComparison)(ignoreCase ? 5 : 4));
		}

		private static void GetInteractivePageHeaderFooter(int pageNumber, AspNetCore.ReportingServices.ReportRendering.Report report, out AspNetCore.ReportingServices.ReportRendering.PageSection pageHeader, out AspNetCore.ReportingServices.ReportRendering.PageSection pageFooter)
		{
			if (report == null)
			{
				throw new ArgumentNullException("report");
			}
			pageHeader = null;
			pageFooter = null;
			pageNumber--;
			if (pageNumber >= 0 && pageNumber < report.NumberOfPages)
			{
				try
				{
					Global.Tracer.Assert(report.RenderingContext != null && null != report.RenderingContext.ReportSnapshot, "Invalid rendering context");
					ReportSnapshot reportSnapshot = report.RenderingContext.ReportSnapshot;
					int startPage = default(int);
					AspNetCore.ReportingServices.ReportProcessing.Persistence.IntermediateFormatReader pageSectionReader = report.RenderingContext.ChunkManager.GetPageSectionReader(pageNumber, out startPage);
					if (pageSectionReader != null)
					{
						List<PageSectionInstance> list = pageSectionReader.ReadPageSections(pageNumber, startPage, report.ReportDef.PageHeader, report.ReportDef.PageFooter);
						report.RenderingContext.ChunkManager.SetPageSectionReaderState(pageSectionReader.ReaderState, pageNumber);
						if (list != null)
						{
							Global.Tracer.Assert(2 == list.Count, "Invalid persisted page section structure");
							pageHeader = ReportProcessing.GetRenderingPageSection(list[0], report, pageNumber, true);
							pageFooter = ReportProcessing.GetRenderingPageSection(list[1], report, pageNumber, false);
						}
					}
				}
				catch (RSException)
				{
					throw;
				}
				catch (Exception innerException)
				{
					throw new ReportProcessingException(innerException, null);
				}
			}
		}

		private static AspNetCore.ReportingServices.ReportRendering.PageSection GetRenderingPageSection(PageSectionInstance instance, AspNetCore.ReportingServices.ReportRendering.Report report, int pageNumber, bool isHeader)
		{
			AspNetCore.ReportingServices.ReportRendering.PageSection result = null;
			if (instance != null)
			{
				string text = pageNumber.ToString(CultureInfo.InvariantCulture) + (isHeader ? "ph" : "pf");
				AspNetCore.ReportingServices.ReportRendering.RenderingContext renderingContext = new AspNetCore.ReportingServices.ReportRendering.RenderingContext(report.RenderingContext, text);
				result = new AspNetCore.ReportingServices.ReportRendering.PageSection(text, isHeader ? report.ReportDef.PageHeader : report.ReportDef.PageFooter, instance, report, renderingContext, false);
			}
			return result;
		}

		internal static void EvaluateHeaderFooterExpressions(int pageNumber, int totalPages, AspNetCore.ReportingServices.ReportRendering.Report report, PageReportItems pageReportItems, out AspNetCore.ReportingServices.ReportRendering.PageSection pageHeader, out AspNetCore.ReportingServices.ReportRendering.PageSection pageFooter)
		{
			if (report == null)
			{
				throw new ArgumentNullException("report");
			}
			if (pageReportItems == null)
			{
				ReportProcessing.GetInteractivePageHeaderFooter(pageNumber, report, out pageHeader, out pageFooter);
			}
			else
			{
				CultureInfo cultureInfo = null;
				ProcessingErrorContext processingErrorContext = new ProcessingErrorContext();
				try
				{
					cultureInfo = Thread.CurrentThread.CurrentCulture;
					PageMerge pageMerge = new PageMerge();
					pageMerge.Process(pageNumber, totalPages, report, pageReportItems, (ErrorContext)processingErrorContext, out pageHeader, out pageFooter);
				}
				catch (RSException)
				{
					throw;
				}
				catch (Exception innerException)
				{
					throw new ReportProcessingException(innerException, processingErrorContext.Messages);
				}
				finally
				{
					if (cultureInfo != null)
					{
						Thread.CurrentThread.CurrentCulture = cultureInfo;
					}
				}
			}
		}

		internal static void CheckAndConvertDataSources(ICatalogItemContext itemContext, DataSourceInfoCollection dataSources, DataSetInfoCollection dataSetReferences, bool checkIfUsable, ServerDataSourceSettings serverDatasourceSettings, RuntimeDataSourceInfoCollection allDataSources, RuntimeDataSetInfoCollection allDataSetReferences)
		{
			if (dataSetReferences != null)
			{
				foreach (DataSetInfo dataSetReference in dataSetReferences)
				{
					if (checkIfUsable && !dataSetReference.IsValidReference())
					{
						throw new InvalidDataSetReferenceException(dataSetReference.DataSetName.MarkAsPrivate());
					}
					allDataSetReferences.Add(dataSetReference, itemContext);
				}
			}
			if (dataSources != null)
			{
				foreach (AspNetCore.ReportingServices.DataExtensions.DataSourceInfo dataSource in dataSources)
				{
					if (checkIfUsable)
					{
						dataSource.ThrowIfNotUsable(serverDatasourceSettings);
					}
					if (allDataSources != null)
					{
						allDataSources.Add(dataSource, itemContext);
					}
				}
			}
		}

		private static void TraceError(Exception e)
		{
			if (Global.Tracer.TraceError)
			{
				Global.Tracer.Trace(TraceLevel.Error, "An error has occurred while retrieving datasources for a sub-report. The report definition could not be retrieved. Details: {0}", e.Message);
			}
		}

		private static void CheckCredentialsOdp(AspNetCore.ReportingServices.ReportIntermediateFormat.Report report, DataSourceInfoCollection dataSources, DataSetInfoCollection dataSetReferences, ICatalogItemContext reportContext, OnDemandSubReportDataSourcesCallback subReportCallback, RuntimeDataSourceInfoCollection allDataSources, RuntimeDataSetInfoCollection allDataSetReferences, int subReportLevel, bool checkIfUsable, ServerDataSourceSettings serverDatasourceSettings, Hashtable subReportNames)
		{
			if ((long)subReportLevel <= 20L)
			{
				ReportProcessing.CheckAndConvertDataSources(reportContext, dataSources, dataSetReferences, checkIfUsable, serverDatasourceSettings, allDataSources, allDataSetReferences);
				if (report.SubReports != null)
				{
					DataSourceInfoCollection dataSources2 = null;
					DataSetInfoCollection dataSetReferences2 = null;
					for (int i = 0; i < report.SubReports.Count; i++)
					{
						string reportName = report.SubReports[i].ReportName;
						if (!subReportNames.ContainsKey(reportName))
						{
							subReportNames.Add(reportName, null);
							try
							{
								ICatalogItemContext reportContext2 = default(ICatalogItemContext);
								IChunkFactory chunkFactory = default(IChunkFactory);
								subReportCallback(reportContext, reportName, ReportProcessing.NeedsUpgradeImpl, out reportContext2, out chunkFactory, out dataSources2, out dataSetReferences2);
								if (chunkFactory != null && ReportProcessing.ContainsFlag(chunkFactory.ReportProcessingFlags, ReportProcessingFlags.OnDemandEngine))
								{
									report.SubReports[i].Report = ReportProcessing.DeserializeKatmaiReport(chunkFactory);
									ReportProcessing.CheckCredentialsOdp(report.SubReports[i].Report, dataSources2, dataSetReferences2, reportContext2, subReportCallback, allDataSources, allDataSetReferences, subReportLevel + 1, checkIfUsable, serverDatasourceSettings, subReportNames);
								}
							}
							catch (ReportProcessingException e)
							{
								ReportProcessing.TraceError(e);
							}
							catch (ReportCatalogException ex)
							{
								ReportProcessing.TraceError(ex);
								if (ex.Code != ErrorCode.rsReportServerDatabaseUnavailable)
								{
									goto end_IL_00dc;
								}
								throw;
								end_IL_00dc:;
							}
						}
					}
				}
			}
		}

		private static void CheckCredentials(Report report, DataSourceInfoCollection dataSources, ICatalogItemContext reportContext, SubReportDataSourcesCallback subReportCallback, RuntimeDataSourceInfoCollection allDataSources, int subReportLevel, bool checkIfUsable, ServerDataSourceSettings serverDatasourceSettings, Hashtable subReportNames)
		{
			if ((long)subReportLevel <= 20L)
			{
				ReportProcessing.CheckAndConvertDataSources(reportContext, dataSources, null, checkIfUsable, serverDatasourceSettings, allDataSources, null);
				if (report.SubReports != null)
				{
					DataSourceInfoCollection dataSources2 = null;
					for (int i = 0; i < report.SubReports.Count; i++)
					{
						string reportPath = report.SubReports[i].ReportPath;
						if (!subReportNames.ContainsKey(reportPath))
						{
							try
							{
								ICatalogItemContext reportContext2 = default(ICatalogItemContext);
								GetReportChunk getReportChunk = default(GetReportChunk);
								subReportCallback(reportContext, reportPath, out reportContext2, out getReportChunk, out dataSources2);
								if (getReportChunk != null)
								{
									subReportNames.Add(reportPath, null);
									Report report2 = ReportProcessing.DeserializeReport(getReportChunk);
									ReportProcessing.CheckCredentials(report2, dataSources2, reportContext2, subReportCallback, allDataSources, subReportLevel + 1, checkIfUsable, serverDatasourceSettings, subReportNames);
								}
							}
							catch (VersionMismatchException)
							{
								throw;
							}
							catch (ReportProcessingException e)
							{
								ReportProcessing.TraceError(e);
							}
							catch (ReportCatalogException ex2)
							{
								ReportProcessing.TraceError(ex2);
								if (ex2.Code != ErrorCode.rsReportServerDatabaseUnavailable)
								{
									goto end_IL_0096;
								}
								throw;
								end_IL_0096:;
							}
						}
					}
				}
			}
		}

		public PublishingResult CreateIntermediateFormat(PublishingContext reportPublishingContext)
		{
			if (reportPublishingContext.DataProtection == null)
			{
				throw new ArgumentNullException("reportPublishingContext.DataProtection");
			}
			Global.Tracer.Assert(reportPublishingContext != null && reportPublishingContext.PublishingContextKind != PublishingContextKind.SharedDataSet, "Publishing a report must provide a correct report publishing context");
			PublishingErrorContext publishingErrorContext = new PublishingErrorContext();
			try
			{
				DataSetInfoCollection dataSetInfoCollection = null;
				ArrayList dataSetsName = null;
				byte[] dataSetsHash = null;
				double num = 0.0;
				double num2 = 0.0;
				double num3 = 0.0;
				double num4 = 0.0;
				double num5 = 0.0;
				double num6 = 0.0;
				string reportDescription = default(string);
				string reportLanguage = default(string);
				ParameterInfoCollection parameters = default(ParameterInfoCollection);
				DataSourceInfoCollection dataSources = default(DataSourceInfoCollection);
				UserLocationFlags userReferenceLocation = default(UserLocationFlags);
				bool hasExternalImages = default(bool);
				bool hasHyperlinks = default(bool);
				try
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.Report report = this.CompileOdpReport(reportPublishingContext, publishingErrorContext, out reportDescription, out reportLanguage, out parameters, out dataSources, out dataSetInfoCollection, out userReferenceLocation, out dataSetsName, out hasExternalImages, out hasHyperlinks, out dataSetsHash);
					Global.Tracer.Assert(report.ReportSections != null && report.ReportSections.Count > 0, "Report should have at least one section.");
					AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSection reportSection = report.ReportSections[0];
					num = reportSection.Page.PageHeightValue;
					num2 = reportSection.Page.PageWidthValue;
					num3 = reportSection.Page.TopMarginValue;
					num4 = reportSection.Page.BottomMarginValue;
					num5 = reportSection.Page.LeftMarginValue;
					num6 = reportSection.Page.RightMarginValue;
					reportPublishingContext.ProcessingFlags = ReportProcessingFlags.OnDemandEngine;
				}
				catch (CRI2005UpgradeException)
				{
					ICatalogItemContext catalogContext = reportPublishingContext.CatalogContext;
					byte[] definition = reportPublishingContext.Definition;
					IChunkFactory createChunkFactory = reportPublishingContext.CreateChunkFactory;
					Report report2 = this.CompileYukonReport(catalogContext, definition, (CreateReportChunk)createChunkFactory.CreateChunk, reportPublishingContext.CheckDataSourceCallback, reportPublishingContext.ResolveTemporaryDataSourceCallback, reportPublishingContext.OriginalDataSources, publishingErrorContext, reportPublishingContext.CompilationTempAppDomain, reportPublishingContext.GenerateExpressionHostWithRefusedPermissions, reportPublishingContext.DataProtection, out reportDescription, out reportLanguage, out parameters, out dataSources, out userReferenceLocation, out dataSetsName, out hasExternalImages, out hasHyperlinks);
					num = report2.InteractiveHeightValue;
					num2 = report2.InteractiveWidthValue;
					num3 = report2.TopMarginValue;
					num4 = report2.BottomMarginValue;
					num5 = report2.LeftMarginValue;
					num6 = report2.RightMarginValue;
					reportPublishingContext.ProcessingFlags = ReportProcessingFlags.YukonEngine;
				}
				return new PublishingResult(reportDescription, reportLanguage, parameters, dataSources, dataSetInfoCollection ?? new DataSetInfoCollection(), publishingErrorContext.Messages, userReferenceLocation, num, num2, num3, num4, num5, num6, dataSetsName, hasExternalImages, hasHyperlinks, reportPublishingContext.ProcessingFlags, dataSetsHash);
			}
			catch (RSException)
			{
				throw;
			}
			catch (Exception innerException)
			{
				ProcessingMessageList processingMessages = (publishingErrorContext != null) ? publishingErrorContext.Messages : new ProcessingMessageList();
				throw new ReportProcessingException(innerException, processingMessages);
			}
		}

		private ReportIntermediateFormat.Report CompileOdpReport(PublishingContext reportPublishingContext, PublishingErrorContext errorContext, out string reportDescription, out string reportLanguage, out ParameterInfoCollection parameters, out DataSourceInfoCollection dataSources, out DataSetInfoCollection sharedDataSetReferences, out UserLocationFlags userReferenceLocation, out ArrayList dataSetsName, out bool hasExternalImages, out bool hasHyperlinks, out byte[] dataSetsHash)
		{
			ReportUpgradeStrategy reportUpgradeStrategy = new RdlObjectModelUpgradeStrategy(!reportPublishingContext.IsInternalRepublish, !reportPublishingContext.IsRdlSandboxingEnabled);
			AspNetCore.ReportingServices.ReportPublishing.ReportPublishing reportPublishing = new AspNetCore.ReportingServices.ReportPublishing.ReportPublishing(reportPublishingContext, errorContext, reportUpgradeStrategy);
			ReportIntermediateFormat.Report report = reportPublishing.CreateIntermediateFormat(reportPublishingContext.Definition, out reportDescription, out reportLanguage, out parameters, out dataSources, out sharedDataSetReferences, out userReferenceLocation, out dataSetsName, out hasExternalImages, out hasHyperlinks, out dataSetsHash);
			if (reportPublishingContext.CreateChunkFactory != null)
			{
				ReportProcessing.SerializeReport(report, reportPublishingContext.CreateChunkFactory, this.m_configuration);
			}
			return report;
		}

		public ProcessingMessageList ProcessDataSetParameters(DataSetContext dc, DataSetDefinition dataSetDefinition)
		{
			if (dc == null)
			{
				throw new ArgumentNullException("dc");
			}
			if (dataSetDefinition == null)
			{
				throw new ArgumentNullException("dataSetDefinition");
			}
			OnDemandProcessingContext onDemandProcessingContext = this.CreateODPContextForSharedDataSet(dc, dataSetDefinition);
			CultureInfo originalCulture = null;
			try
			{
				if (dc.Culture != null)
				{
					originalCulture = Thread.CurrentThread.CurrentCulture;
					Thread.CurrentThread.CurrentCulture = Localization.ClientPrimaryCulture;
				}
				ReportProcessing.ProcessDataSetParameters(onDemandProcessingContext, dc, dataSetDefinition);
			}
			finally
			{
				ReportProcessing.FinallyBlockForSharedDataSetProcessing(onDemandProcessingContext, dc, originalCulture);
			}
			return onDemandProcessingContext.ErrorContext.Messages;
		}

		public DataSetResult ProcessSharedDataSet(DataSetContext dc, DataSetDefinition dataSetDefinition)
		{
			if (dc == null)
			{
				throw new ArgumentNullException("dc");
			}
			if (dataSetDefinition == null)
			{
				throw new ArgumentNullException("dataSetDefinition");
			}
			Global.Tracer.Assert(null != dataSetDefinition.DataSetCore, "Shared dataset definition is missing dataset information");
			OnDemandProcessingContext onDemandProcessingContext = this.CreateODPContextForSharedDataSet(dc, dataSetDefinition);
			onDemandProcessingContext.SetSharedDataSetUniqueName(onDemandProcessingContext.ExternalDataSetContext.TargetChunkNameInSnapshot ?? dataSetDefinition.DataSetCore.Name);
			onDemandProcessingContext.ExecutionLogContext.StartProcessingTimer();
			if (dc.Parameters != null || dataSetDefinition.DataSetCore.Query != null)
			{
				int num = (dc.Parameters != null) ? dc.Parameters.Count : 0;
				int num2 = (dataSetDefinition.DataSetCore.Query != null && dataSetDefinition.DataSetCore.Query.Parameters != null) ? dataSetDefinition.DataSetCore.Query.Parameters.Count : 0;
				if (num != num2)
				{
					throw new DataSetExecutionException(ErrorCode.rsParameterError);
				}
			}
			bool flag = default(bool);
			if (dc.Parameters != null && !dc.Parameters.ValuesAreValid(out flag, true))
			{
				throw new DataSetExecutionException(ErrorCode.rsParametersNotSpecified);
			}
			bool successfulCompletion = false;
			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			try
			{
				if (dc.Culture != null && Thread.CurrentThread.CurrentCulture != dc.Culture)
				{
					Thread.CurrentThread.CurrentCulture = dc.Culture;
				}
				RetrievalManager retrievalManager = new RetrievalManager(dataSetDefinition, onDemandProcessingContext);
				successfulCompletion = retrievalManager.FetchSharedDataSet(dc.Parameters);
			}
			catch (Exception ex)
			{
				Exception ex2 = ex;
				if (ex2 is ProcessingAbortedException)
				{
					ex2 = ex2.InnerException;
				}
				string plainString = (dc.ConsumerRequest == null || dc.ConsumerRequest.ReportDataSetName == null) ? dc.ItemContext.ItemPathAsString : dc.ConsumerRequest.ReportDataSetName;
				throw new DataSetExecutionException(plainString.MarkAsPrivate(), ex2);
			}
			finally
			{
				ReportProcessing.FinallyBlockForSharedDataSetProcessing(onDemandProcessingContext, dc, currentCulture);
			}
			if (dc.DataSources != null && dc.DataSources.HasConnectionStringUseridReference())
			{
				onDemandProcessingContext.MergeHasUserProfileState(UserProfileState.InQuery);
			}
			return new DataSetResult(dc.Parameters, onDemandProcessingContext.ErrorContext.Messages, onDemandProcessingContext.HasUserProfileState, successfulCompletion);
		}

		private static void ProcessDataSetParameters(OnDemandProcessingContext odpContext, DataSetContext dc, DataSetDefinition dataSetDefinition)
		{
			SharedDataSetProcessReportParameters sharedDataSetProcessReportParameters = new SharedDataSetProcessReportParameters(dataSetDefinition.DataSetCore, odpContext);
			sharedDataSetProcessReportParameters.Process(dc.Parameters);
		}

		private static void FinallyBlockForSharedDataSetProcessing(OnDemandProcessingContext odpContext, DataSetContext dc, CultureInfo originalCulture)
		{
			if (originalCulture != null && Thread.CurrentThread.CurrentCulture != originalCulture)
			{
				Thread.CurrentThread.CurrentCulture = originalCulture;
			}
			odpContext.UnregisterAbortInfo();
			dc.Parameters.UserProfileState |= odpContext.HasUserProfileState;
			if (odpContext.ExecutionLogContext != null && odpContext.ExecutionLogContext.IsProcessingTimerRunning)
			{
				ReportProcessing.UpdateHostingEnvironment(null, dc.ItemContext, odpContext.ExecutionLogContext, ProcessingEngine.OnDemandEngine, dc.JobContext, (dc.ConsumerRequest == null) ? "Standalone" : "Inline");
			}
		}

		private OnDemandProcessingContext CreateODPContextForSharedDataSet(DataSetContext dc, DataSetDefinition dataSetDefinition)
		{
			ProcessingErrorContext errorContext = new ProcessingErrorContext();
			return new OnDemandProcessingContext(dc, dataSetDefinition, errorContext, this.m_configuration);
		}

		public DataSetPublishingResult CreateSharedDataSet(PublishingContext sharedDataSetPublishingContext)
		{
			Global.Tracer.Assert(sharedDataSetPublishingContext != null && sharedDataSetPublishingContext.PublishingContextKind == PublishingContextKind.SharedDataSet, "CreateSharedDataSet must be called with a valid publishing context");
			PublishingErrorContext errorContext = new PublishingErrorContext();
			AspNetCore.ReportingServices.ReportPublishing.ReportPublishing reportPublishing = new AspNetCore.ReportingServices.ReportPublishing.ReportPublishing(sharedDataSetPublishingContext, errorContext);
			return reportPublishing.CreateSharedDataSet(sharedDataSetPublishingContext.Definition);
		}
	}
}
