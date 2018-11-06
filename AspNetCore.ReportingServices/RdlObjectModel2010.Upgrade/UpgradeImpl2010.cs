using AspNetCore.ReportingServices.RdlObjectModel;
using AspNetCore.ReportingServices.RdlObjectModel.RdlUpgrade;
using AspNetCore.ReportingServices.RdlObjectModel.Serialization;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlObjectModel2010.Upgrade
{
	internal class UpgradeImpl2010 : UpgraderBase
	{
		private class CellInfo
		{
			public int RowIndex;

			public int ColumnIndex;

			public ReportParameter Parameter;

			public bool IsSecondVisibleColumn;
		}

		private List<IUpgradeable2010> m_upgradeable;

		internal UpgradeImpl2010()
		{
		}

		internal override Type GetReportType()
		{
			return typeof(Report2010);
		}

		protected override void InitUpgrade()
		{
			this.m_upgradeable = new List<IUpgradeable2010>();
			base.InitUpgrade();
		}

		protected override void Upgrade(Report report)
		{
			foreach (IUpgradeable2010 item in this.m_upgradeable)
			{
				item.Upgrade(this);
			}
			base.Upgrade(report);
		}

		protected override RdlSerializerSettings CreateReaderSettings()
		{
			return UpgradeSerializerSettings2010.CreateReaderSettings();
		}

		protected override RdlSerializerSettings CreateWriterSettings()
		{
			return UpgradeSerializerSettings2010.CreateWriterSettings();
		}

		protected override void SetupReaderSettings(RdlSerializerSettings settings)
		{
			SerializerHost2010 serializerHost = (SerializerHost2010)settings.Host;
			serializerHost.Upgradeable2010 = this.m_upgradeable;
			base.SetupReaderSettings(settings);
		}

		internal void UpgradeReport(Report2010 report)
		{
			ReportParametersLayout reportParametersLayout = new ReportParametersLayout();
			GridLayoutDefinition gridLayoutDefinition = new GridLayoutDefinition();
			int numberOfRows = 2;
			int numberOfColumns = 4;
			IList<ReportParameter> reportParameters = report.ReportParameters;
			if (reportParameters != null && report.ReportParameters.Count > 0)
			{
				IList<ReportParameter> reportParameters2 = report.ReportParameters;
				gridLayoutDefinition.CellDefinitions = this.CreatedArrangedCellDefinitions(reportParameters2, out numberOfRows, out numberOfColumns);
				gridLayoutDefinition.NumberOfColumns = numberOfColumns;
				gridLayoutDefinition.NumberOfRows = numberOfRows;
				reportParametersLayout.GridLayoutDefinition = gridLayoutDefinition;
				report.ReportParametersLayout = reportParametersLayout;
			}
		}

		private CellDefinition CreateCellDefinition(ReportParameter parameters, int rowIndex, int columnIndex)
		{
			CellDefinition cellDefinition = new CellDefinition();
			cellDefinition.ParameterName = parameters.Name;
			cellDefinition.ColumnIndex = columnIndex;
			cellDefinition.RowIndex = rowIndex;
			return cellDefinition;
		}

		private IList<CellDefinition> CreatedArrangedCellDefinitions(IList<ReportParameter> parameters, out int numberOfRows, out int numberOfColumns)
		{
			int num = 0;
			int num2 = 0;
			numberOfRows = 1;
			numberOfColumns = 1;
			List<CellDefinition> list = new List<CellDefinition>();
			List<CellInfo> list2 = new List<CellInfo>();
			int num3 = 0;
			int num4 = 1;
			for (int i = 0; i < parameters.Count; i++)
			{
				ReportParameter parameter = parameters[i];
				bool flag = i < parameters.Count - 1 && !UpgradeImpl2010.IsParameterHidden(parameters[i + 1]);
				numberOfColumns = Math.Max(num2 + 1, numberOfColumns);
				numberOfRows = num + 1;
				bool flag2 = false;
				bool flag3 = false;
				if (!UpgradeImpl2010.IsParameterHidden(parameters[i]))
				{
					num3++;
					flag2 = (num3 % 2 == 0);
					if (flag2)
					{
						num4 = Math.Max(num2, num4);
					}
					flag3 = flag2;
				}
				else if (flag)
				{
					flag3 = (num3 % 2 == 0);
				}
				list2.Add(new CellInfo
				{
					RowIndex = num,
					ColumnIndex = num2,
					Parameter = parameter,
					IsSecondVisibleColumn = flag2
				});
				if (num2 == 7 || flag3)
				{
					num++;
					num2 = 0;
				}
				else
				{
					num2++;
				}
			}
			foreach (CellInfo item2 in list2)
			{
				CellDefinition item = this.CreateCellDefinition(item2.Parameter, item2.RowIndex, item2.IsSecondVisibleColumn ? num4 : item2.ColumnIndex);
				list.Add(item);
			}
			return list;
		}

		private static bool IsParameterHidden(ReportParameter parameterNode)
		{
			if (!parameterNode.Hidden && !(parameterNode.Prompt == (string)null))
			{
				return string.IsNullOrEmpty(parameterNode.Prompt.Value);
			}
			return true;
		}
	}
}
