using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimTableRowCollection : TablixRowCollection
	{
		private List<TablixRow> m_rows;

		public override TablixRow this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					return this.m_rows[index];
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public override int Count
		{
			get
			{
				return this.m_rows.Count;
			}
		}

		internal ShimTableRowCollection(Tablix owner)
			: base(owner)
		{
			this.m_rows = new List<TablixRow>();
			this.AppendTableRows(owner.RenderTable.TableHeader);
			if (owner.RenderTable.TableGroups != null)
			{
				this.AppendTableGroups(owner.RenderTable.TableGroups[0]);
			}
			else if (owner.RenderTable.DetailRows != null)
			{
				this.AppendTableRows(owner.RenderTable.DetailRows[0]);
			}
			this.AppendTableRows(owner.RenderTable.TableFooter);
		}

		private void AppendTableGroups(AspNetCore.ReportingServices.ReportRendering.TableGroup renderGroup)
		{
			if (renderGroup != null)
			{
				this.AppendTableRows(renderGroup.GroupHeader);
				if (renderGroup.SubGroups != null)
				{
					this.AppendTableGroups(renderGroup.SubGroups[0]);
				}
				else if (renderGroup.DetailRows != null)
				{
					this.AppendTableRows(renderGroup.DetailRows[0]);
				}
				this.AppendTableRows(renderGroup.GroupFooter);
			}
		}

		private void AppendTableRows(TableRowCollection renderRows)
		{
			if (renderRows != null)
			{
				int count = renderRows.DetailRowDefinitions.Count;
				for (int i = 0; i < count; i++)
				{
					this.m_rows.Add(new ShimTableRow(base.m_owner, this.m_rows.Count, renderRows[i]));
				}
			}
		}
	}
}
