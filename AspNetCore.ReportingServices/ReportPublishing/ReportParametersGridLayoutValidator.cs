using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportPublishing
{
	internal sealed class ReportParametersGridLayoutValidator
	{
		private const int MaxNumberOfRows = 10000;

		private const int MaxNumberOfColumns = 8;

		private const int MaxNumberOfConsecutiveEmptyRows = 20;

		private const string NoObjectName = "";

		private const string NoPropertyName = "";

		private readonly string[] NoArguments = new string[0];

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterDef> m_parameters;

		private ParametersGridLayout m_paramsLayout;

		private PublishingErrorContext m_errorContext;

		private readonly HashSet<string> m_parameterNames = new HashSet<string>();

		private readonly HashSet<string> m_gridParameterNames = new HashSet<string>();

		private readonly HashSet<long> m_cellAddresses = new HashSet<long>();

		private readonly SortedList<int, bool> m_parameterRowIndexes = new SortedList<int, bool>();

		private int NumberOfRows
		{
			get
			{
				return this.m_paramsLayout.NumberOfRows;
			}
		}

		private int NumberOfColumns
		{
			get
			{
				return this.m_paramsLayout.NumberOfColumns;
			}
		}

		private ReportParametersGridLayoutValidator(List<AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterDef> parameters, ParametersGridLayout paramsLayout, PublishingErrorContext errorContext)
		{
			RSTrace.ProcessingTracer.Assert(paramsLayout != null, "paramsLayout should not be null");
			this.m_paramsLayout = paramsLayout;
			this.m_parameters = parameters;
			this.m_errorContext = errorContext;
			this.InitParameterNames();
		}

		public static bool Validate(List<AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterDef> parameters, ParametersGridLayout paramsLayout, PublishingErrorContext errorContext)
		{
			ReportParametersGridLayoutValidator reportParametersGridLayoutValidator = new ReportParametersGridLayoutValidator(parameters, paramsLayout, errorContext);
			return reportParametersGridLayoutValidator.Validate();
		}

		private bool Validate()
		{
			bool flag = false;
			if (this.m_parameterNames.Count == 0)
			{
				return this.ValidateNumberOfRows() && this.ValidateNumberOfColumns() && this.ValidateGridCells();
			}
			return this.ValidateNumberOfRows() && this.ValidateNumberOfColumns() && this.ValidateParametersCount() && this.ValidateGridCells() && this.ValidateConsecutiveEmptyRowCount();
		}

		private void InitParameterNames()
		{
			foreach (AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterDef parameter in this.m_parameters)
			{
				this.m_parameterNames.Add(parameter.Name);
			}
		}

		private static bool IsParamVisible(AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterDef param)
		{
			return param.Prompt.Length > 0;
		}

		private bool ValidateNumberOfRows()
		{
			if (this.ValidatePrerequisite(this.NumberOfRows > 0, ProcessingErrorCode.rsInvalidParameterLayoutZeroOrLessRowOrCol))
			{
				return this.ValidatePrerequisite(this.NumberOfRows <= 10000, ProcessingErrorCode.rsInvalidParameterLayoutNumberOfRowsOrColumnsExceedingLimit, "", "NumberOfRows", 10000.ToString());
			}
			return false;
		}

		private bool ValidateNumberOfColumns()
		{
			if (this.ValidatePrerequisite(this.NumberOfColumns > 0, ProcessingErrorCode.rsInvalidParameterLayoutZeroOrLessRowOrCol))
			{
				return this.ValidatePrerequisite(this.NumberOfColumns <= 8, ProcessingErrorCode.rsInvalidParameterLayoutNumberOfRowsOrColumnsExceedingLimit, "", "NumberOfColumns", 8.ToString());
			}
			return false;
		}

		private bool ValidateParametersCount()
		{
			bool flag = this.ValidatePrerequisite(this.m_parameters.Count <= this.NumberOfRows * this.NumberOfColumns, ProcessingErrorCode.rsInvalidParameterLayoutParametersMissingFromPanel);
			if (this.m_paramsLayout.CellDefinitions != null)
			{
				return flag & this.ValidatePrerequisite(this.m_paramsLayout.CellDefinitions.Count == this.m_parameters.Count, ProcessingErrorCode.rsInvalidParameterLayoutCellDefNotEqualsParameterCount);
			}
			return flag & this.ValidatePrerequisite(this.m_parameters.Count == 0, ProcessingErrorCode.rsInvalidParameterLayoutCellDefNotEqualsParameterCount);
		}

		private bool ValidateGridCells()
		{
			if (this.m_parameterNames.Count == 0 && this.m_paramsLayout.CellDefinitions == null)
			{
				return true;
			}
			foreach (object cellDefinition in this.m_paramsLayout.CellDefinitions)
			{
				ParameterGridLayoutCellDefinition cell = (ParameterGridLayoutCellDefinition)cellDefinition;
				if (!this.ValidateGridCell(cell))
				{
					return false;
				}
			}
			return true;
		}

		private bool ValidateGridCell(ParameterGridLayoutCellDefinition cell)
		{
			bool flag = true;
			string parameterName = cell.ParameterName;
			long item = cell.RowIndex * this.NumberOfColumns + cell.ColumnIndex;
			flag &= this.ValidatePrerequisite(!string.IsNullOrEmpty(parameterName), ProcessingErrorCode.rsInvalidParameterLayoutParameterNameMissing);
			flag &= this.ValidatePrerequisite(!this.m_gridParameterNames.Contains(parameterName), ProcessingErrorCode.rsInvalidParameterLayoutParameterAppearsTwice, cell.ParameterName);
			flag &= this.ValidatePrerequisite(this.m_parameterNames.Contains(parameterName), ProcessingErrorCode.rsInvalidParameterLayoutParameterNotVisible, parameterName);
			flag &= this.ValidatePrerequisite(cell.RowIndex >= 0 && cell.RowIndex < this.NumberOfRows, ProcessingErrorCode.rsInvalidParameterLayoutRowColOutOfBounds);
			flag &= this.ValidatePrerequisite(cell.ColumnIndex >= 0 && cell.ColumnIndex < this.NumberOfColumns, ProcessingErrorCode.rsInvalidParameterLayoutRowColOutOfBounds);
			flag &= this.ValidatePrerequisite(!this.m_cellAddresses.Contains(item), ProcessingErrorCode.rsInvalidParameterLayoutCellCollition);
			if (flag)
			{
				this.m_parameterRowIndexes[cell.RowIndex] = true;
				this.m_gridParameterNames.Add(parameterName);
				this.m_cellAddresses.Add(item);
			}
			return flag;
		}

		private bool ValidateConsecutiveEmptyRowCount()
		{
			return this.ValidatePrerequisite(!this.DoesNumberOfConsecutiveEmptyRowsExceedLimit(), ProcessingErrorCode.rsInvalidParameterLayoutNumberOfConsecutiveEmptyRowsExceedingLimit, "", "", 20.ToString());
		}

		private bool ValidatePrerequisite(bool condition, ProcessingErrorCode errorCode)
		{
			return this.ValidatePrerequisite(condition, errorCode, "", "", this.NoArguments);
		}

		private bool ValidatePrerequisite(bool condition, ProcessingErrorCode errorCode, string objectName)
		{
			return this.ValidatePrerequisite(condition, errorCode, objectName, "", this.NoArguments);
		}

		private bool ValidatePrerequisite(bool condition, ProcessingErrorCode errorCode, string objectName, string propertyName, params string[] arguments)
		{
			if (!condition)
			{
				this.m_errorContext.Register(errorCode, Severity.Error, ObjectType.ParameterLayout, objectName, propertyName, arguments);
			}
			return condition;
		}

		private bool DoesNumberOfConsecutiveEmptyRowsExceedLimit()
		{
			int num = 0;
			int num2 = 0;
			foreach (int key in this.m_parameterRowIndexes.Keys)
			{
				num2 = key - num - 1;
				if (num2 > 20)
				{
					return true;
				}
				num = key;
			}
			num2 = this.NumberOfRows - 1 - num - 1;
			return num2 > 20;
		}
	}
}
