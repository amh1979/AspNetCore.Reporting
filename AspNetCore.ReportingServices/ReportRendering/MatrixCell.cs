using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Diagnostics;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class MatrixCell
	{
		private Matrix m_owner;

		private int m_rowIndex;

		private int m_columnIndex;

		private MatrixCellInstance m_matrixCellInstance;

		private ReportItem m_cellReportItem;

		private MatrixCellInstanceInfo m_matrixCellInstanceInfo;

		public ReportItem ReportItem
		{
			get
			{
				ReportItem reportItem = this.m_cellReportItem;
				if (this.m_cellReportItem == null)
				{
					AspNetCore.ReportingServices.ReportProcessing.Matrix matrix = (AspNetCore.ReportingServices.ReportProcessing.Matrix)this.m_owner.ReportItemDef;
					ReportItemInstance reportItemInstance = null;
					NonComputedUniqueNames nonComputedUniqueNames = null;
					AspNetCore.ReportingServices.ReportProcessing.ReportItem reportItem2 = null;
					if (this.m_owner.NoRows)
					{
						reportItem2 = matrix.GetCellReportItem(this.m_rowIndex, this.m_columnIndex);
					}
					else
					{
						reportItem2 = matrix.GetCellReportItem(this.InstanceInfo.RowIndex, this.InstanceInfo.ColumnIndex);
						reportItemInstance = this.m_matrixCellInstance.Content;
						nonComputedUniqueNames = this.InstanceInfo.ContentUniqueNames;
					}
					if (reportItem2 != null)
					{
						try
						{
							MatrixSubtotalCellInstance matrixSubtotalCellInstance = this.m_matrixCellInstance as MatrixSubtotalCellInstance;
							if (matrixSubtotalCellInstance != null)
							{
								Global.Tracer.Assert(null != matrixSubtotalCellInstance.SubtotalHeadingInstance.MatrixHeadingDef.Subtotal.StyleClass);
								this.m_owner.RenderingContext.HeadingInstance = matrixSubtotalCellInstance.SubtotalHeadingInstance;
							}
						}
						catch (Exception ex)
						{
							Global.Tracer.Trace(TraceLevel.Error, "Could not restore matrix subtotal heading instance from intermediate format: {0}", ex.StackTrace);
							this.m_owner.RenderingContext.HeadingInstance = null;
						}
						reportItem = ReportItem.CreateItem(0, reportItem2, reportItemInstance, this.m_owner.RenderingContext, nonComputedUniqueNames);
						this.m_owner.RenderingContext.HeadingInstance = null;
					}
					if (this.m_owner.RenderingContext.CacheState)
					{
						this.m_cellReportItem = reportItem;
					}
				}
				return reportItem;
			}
		}

		public string CellLabel
		{
			get
			{
				AspNetCore.ReportingServices.ReportProcessing.Matrix matrix = (AspNetCore.ReportingServices.ReportProcessing.Matrix)this.m_owner.ReportItemDef;
				if (matrix.OwcCellNames != null)
				{
					int index = this.IndexCellDefinition(matrix);
					return matrix.OwcCellNames[index];
				}
				return null;
			}
		}

		public string ID
		{
			get
			{
				AspNetCore.ReportingServices.ReportProcessing.Matrix matrix = (AspNetCore.ReportingServices.ReportProcessing.Matrix)this.m_owner.ReportItemDef;
				int num = this.IndexCellDefinition(matrix);
				if (matrix.CellIDsForRendering == null)
				{
					matrix.CellIDsForRendering = new string[matrix.CellIDs.Count];
				}
				if (matrix.CellIDsForRendering[num] == null)
				{
					matrix.CellIDsForRendering[num] = matrix.CellIDs[num].ToString(CultureInfo.InvariantCulture);
				}
				return matrix.CellIDsForRendering[num];
			}
		}

		public object SharedRenderingInfo
		{
			get
			{
				AspNetCore.ReportingServices.ReportProcessing.Matrix matrix = (AspNetCore.ReportingServices.ReportProcessing.Matrix)this.m_owner.ReportItemDef;
				int index = this.IndexCellDefinition(matrix);
				int num = matrix.CellIDs[index];
				return this.m_owner.RenderingContext.RenderingInfoManager.SharedRenderingInfo[num];
			}
			set
			{
				AspNetCore.ReportingServices.ReportProcessing.Matrix matrix = (AspNetCore.ReportingServices.ReportProcessing.Matrix)this.m_owner.ReportItemDef;
				int index = this.IndexCellDefinition(matrix);
				int num = matrix.CellIDs[index];
				this.m_owner.RenderingContext.RenderingInfoManager.SharedRenderingInfo[num] = value;
			}
		}

		public DataElementOutputTypes DataElementOutput
		{
			get
			{
				return ((AspNetCore.ReportingServices.ReportProcessing.Matrix)this.m_owner.ReportItemDef).CellDataElementOutput;
			}
		}

		public string DataElementName
		{
			get
			{
				return ((AspNetCore.ReportingServices.ReportProcessing.Matrix)this.m_owner.ReportItemDef).CellDataElementName;
			}
		}

		internal int ColumnIndex
		{
			get
			{
				if (this.m_matrixCellInstance == null)
				{
					return 0;
				}
				return this.InstanceInfo.ColumnIndex;
			}
		}

		internal int RowIndex
		{
			get
			{
				if (this.m_matrixCellInstance == null)
				{
					return 0;
				}
				return this.InstanceInfo.RowIndex;
			}
		}

		private MatrixCellInstanceInfo InstanceInfo
		{
			get
			{
				if (this.m_matrixCellInstance == null)
				{
					return null;
				}
				if (this.m_matrixCellInstanceInfo == null)
				{
					this.m_matrixCellInstanceInfo = this.m_matrixCellInstance.GetInstanceInfo(this.m_owner.RenderingContext.ChunkManager);
				}
				return this.m_matrixCellInstanceInfo;
			}
		}

		internal MatrixCell(Matrix owner, int rowIndex, int columnIndex)
		{
			this.m_owner = owner;
			this.m_rowIndex = rowIndex;
			this.m_columnIndex = columnIndex;
			if (!owner.NoRows)
			{
				MatrixCellInstancesList cells = ((MatrixInstance)owner.ReportItemInstance).Cells;
				this.m_matrixCellInstance = cells[rowIndex][columnIndex];
			}
		}

		private int IndexCellDefinition(AspNetCore.ReportingServices.ReportProcessing.Matrix matrixDef)
		{
			int num = 0;
			if (this.m_owner.NoRows)
			{
				return this.m_rowIndex * matrixDef.MatrixColumns.Count + this.m_columnIndex;
			}
			return this.InstanceInfo.RowIndex * matrixDef.MatrixColumns.Count + this.InstanceInfo.ColumnIndex;
		}
	}
}
