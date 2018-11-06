using Microsoft.Cloud.Platform.Utils;
using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.DataProcessing;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Diagnostics;
using System.Globalization;

namespace AspNetCore.ReportingServices.DataExtensions
{
	internal sealed class MappingDataReader : IDisposable
	{
		private string m_dataSetName;

		private IDataReader m_dataReader;

		private IDataReaderExtension m_dataReaderExtension;

		private IDataReaderFieldProperties m_dataFieldProperties;

		private int[] m_aliasIndexToFieldIndex;

		private string[] m_fieldNames;

		private readonly DataSourceErrorInspector m_errorInspector;

		public string[] FieldNames
		{
			get
			{
				return this.m_fieldNames;
			}
		}

		public bool ReaderExtensionsSupported
		{
			get
			{
				return this.m_dataReaderExtension != null;
			}
		}

		public bool IsAggregateRow
		{
			get
			{
				return this.m_dataReaderExtension.IsAggregateRow;
			}
		}

		public int AggregationFieldCount
		{
			get
			{
				return this.m_dataReaderExtension.AggregationFieldCount;
			}
		}

		public bool ReaderFieldProperties
		{
			get
			{
				return this.m_dataFieldProperties != null;
			}
		}

		public MappingDataReader(string dataSetName, IDataReader sourceReader, string[] aliases, string[] fieldNames, DataSourceErrorInspector errorInspector)
		{
			if (sourceReader == null)
			{
				if (Global.Tracer.TraceError)
				{
					Global.Tracer.Trace(TraceLevel.Error, "The source data reader is null. Cannot read results.");
				}
				throw new ReportProcessingException(ErrorCode.rsErrorCreatingDataReader, dataSetName.MarkAsPrivate());
			}
			Global.Tracer.Assert(null != aliases, "(null != aliases)");
			this.m_dataSetName = dataSetName;
			this.m_dataReader = sourceReader;
			this.m_fieldNames = fieldNames;
			this.m_errorInspector = errorInspector;
			this.m_dataReaderExtension = (sourceReader as IDataReaderExtension);
			this.m_dataFieldProperties = (sourceReader as IDataReaderFieldProperties);
			if (fieldNames == null)
			{
				if (Global.Tracer.TraceInfo)
				{
					Global.Tracer.Trace(TraceLevel.Info, "The array of field names is null. Aliases will map positionally to the data reader fields.");
				}
				for (int i = 0; i < aliases.Length; i++)
				{
					this.m_aliasIndexToFieldIndex[i] = i;
				}
			}
			else
			{
				Global.Tracer.Assert(aliases.Length == fieldNames.Length, " (aliases.Length == fieldNames.Length)");
				this.m_aliasIndexToFieldIndex = new int[aliases.Length];
				if (fieldNames == null)
				{
					if (Global.Tracer.TraceWarning)
					{
						Global.Tracer.Trace(TraceLevel.Warning, "The data reader does not have any fields.");
					}
					for (int j = 0; j < aliases.Length; j++)
					{
						this.m_aliasIndexToFieldIndex[j] = -1;
					}
				}
				else
				{
					for (int k = 0; k < aliases.Length; k++)
					{
						string text = fieldNames[k];
						if (text != null)
						{
							int num;
							try
							{
								num = this.m_dataReader.GetOrdinal(text);
							}
							catch (RSException)
							{
								throw;
							}
							catch (Exception ex2)
							{
								if (AsynchronousExceptionDetection.IsStoppingException(ex2))
								{
									throw;
								}
								Global.Tracer.Trace(TraceLevel.Warning, "An exception occurred while trying to map a data set field.  Field: '{0}' DataField: '{1}' Details: {2}", aliases[k].MarkAsModelInfo(), fieldNames[k].MarkAsModelInfo(), ex2.Message);
								num = -1;
							}
							this.m_aliasIndexToFieldIndex[k] = num;
						}
						else
						{
							this.m_aliasIndexToFieldIndex[k] = -1;
							this.m_fieldNames[k] = aliases[k];
						}
					}
					if (Global.Tracer.TraceVerbose)
					{
						Global.Tracer.Trace(TraceLevel.Verbose, "Mapping data reader successfully initialized.");
					}
				}
			}
		}

		public bool GetNextRow()
		{
			try
			{
				return this.m_dataReader.Read();
			}
			catch (Exception ex)
			{
				ErrorCode errorCode = default(ErrorCode);
				if (this.m_errorInspector != null && this.m_errorInspector.TryInterpretProviderErrorCode(ex, out errorCode))
				{
					string text = string.Format(CultureInfo.CurrentCulture, RPRes.Keys.GetString(ErrorCode.rsErrorReadingNextDataRow.ToString()), this.m_dataSetName);
					throw new ReportProcessingQueryException(errorCode, ex, text);
				}
				throw new ReportProcessingException(ErrorCode.rsErrorReadingNextDataRow, ex, this.m_dataSetName.MarkAsPrivate());
			}
		}

		private void GenerateFieldErrorException(Exception e)
		{
			if (e == null)
			{
				throw new ReportProcessingException_FieldError(DataFieldStatus.IsError, null);
			}
			if (e is ReportProcessingException)
			{
				throw new ReportProcessingException_FieldError(DataFieldStatus.IsMissing, e.Message);
			}
			if (e is OverflowException)
			{
				throw new ReportProcessingException_FieldError(DataFieldStatus.Overflow, e.ToString());
			}
			if (e is NotSupportedException)
			{
				throw new ReportProcessingException_FieldError(DataFieldStatus.UnSupportedDataType, null);
			}
			throw new ReportProcessingException_FieldError(DataFieldStatus.IsError, e.ToString());
		}

		public object GetFieldValue(int aliasIndex)
		{
			try
			{
				return this.m_dataReader.GetValue(this.GetFieldIndex(aliasIndex));
			}
			catch (Exception e)
			{
				this.GenerateFieldErrorException(e);
			}
			return null;
		}

		public bool IsAggregationField(int aliasIndex)
		{
			try
			{
				Global.Tracer.Assert(null != this.m_dataReaderExtension, "(null != m_dataReaderExtension)");
				return this.m_dataReaderExtension.IsAggregationField(this.GetFieldIndex(aliasIndex));
			}
			catch (Exception innerException)
			{
				throw new ReportProcessingException(ErrorCode.rsErrorReadingDataAggregationField, innerException, this.m_dataSetName.MarkAsPrivate());
			}
		}

		public int GetPropertyCount(int aliasIndex)
		{
			try
			{
				return this.m_dataFieldProperties.GetPropertyCount(this.GetFieldIndex(aliasIndex));
			}
			catch (Exception e)
			{
				this.GenerateFieldErrorException(e);
			}
			return 0;
		}

		public string GetPropertyName(int aliasIndex, int propertyIndex)
		{
			try
			{
				return this.m_dataFieldProperties.GetPropertyName(this.GetFieldIndex(aliasIndex), propertyIndex);
			}
			catch (Exception e)
			{
				this.GenerateFieldErrorException(e);
			}
			return null;
		}

		public object GetPropertyValue(int aliasIndex, int propertyIndex)
		{
			try
			{
				return this.m_dataFieldProperties.GetPropertyValue(this.GetFieldIndex(aliasIndex), propertyIndex);
			}
			catch (Exception e)
			{
				this.GenerateFieldErrorException(e);
			}
			return null;
		}

		void IDisposable.Dispose()
		{
			if (Global.Tracer.TraceVerbose)
			{
				Global.Tracer.Trace(TraceLevel.Verbose, "Closing the data reader in Dispose().");
			}
			if (this.m_dataReader != null)
			{
				this.m_dataReader.Dispose();
			}
			this.m_dataReader = null;
			this.m_aliasIndexToFieldIndex = null;
		}

		private int GetFieldIndex(int aliasIndex)
		{
			int num = -1;
			try
			{
				num = this.m_aliasIndexToFieldIndex[aliasIndex];
			}
			catch (IndexOutOfRangeException)
			{
			}
			if (num < 0)
			{
				throw new ReportProcessingException(ErrorCode.rsNoFieldDataAtIndex, aliasIndex + 1);
			}
			return num;
		}
	}
}
