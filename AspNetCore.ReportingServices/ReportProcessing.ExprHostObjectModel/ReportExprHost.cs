using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class ReportExprHost : ReportItemExprHost
	{
		protected CustomCodeProxyBase m_codeProxyBase;

		protected AggregateParamExprHost[] AggregateParamHosts;

		[CLSCompliant(false)]
		protected IList<AggregateParamExprHost> m_aggregateParamHostsRemotable;

		protected ReportParamExprHost[] ReportParameterHosts;

		[CLSCompliant(false)]
		protected IList<ReportParamExprHost> m_reportParameterHostsRemotable;

		protected DataSourceExprHost[] DataSourceHosts;

		[CLSCompliant(false)]
		protected IList<DataSourceExprHost> m_dataSourceHostsRemotable;

		protected DataSetExprHost[] DataSetHosts;

		[CLSCompliant(false)]
		protected IList<DataSetExprHost> m_dataSetHostsRemotable;

		protected StyleExprHost[] PageSectionHosts;

		[CLSCompliant(false)]
		protected IList<StyleExprHost> m_pageSectionHostsRemotable;

		protected ReportItemExprHost[] LineHosts;

		[CLSCompliant(false)]
		protected IList<ReportItemExprHost> m_lineHostsRemotable;

		protected ReportItemExprHost[] RectangleHosts;

		[CLSCompliant(false)]
		protected IList<ReportItemExprHost> m_rectangleHostsRemotable;

		protected TextBoxExprHost[] TextBoxHosts;

		[CLSCompliant(false)]
		protected IList<TextBoxExprHost> m_textBoxHostsRemotable;

		protected ImageExprHost[] ImageHosts;

		[CLSCompliant(false)]
		protected IList<ImageExprHost> m_imageHostsRemotable;

		protected SubreportExprHost[] SubreportHosts;

		[CLSCompliant(false)]
		protected IList<SubreportExprHost> m_subreportHostsRemotable;

		protected ActiveXControlExprHost[] ActiveXControlHosts;

		[CLSCompliant(false)]
		protected IList<ActiveXControlExprHost> m_activeXControlHostsRemotable;

		protected ListExprHost[] ListHosts;

		[CLSCompliant(false)]
		protected IList<ListExprHost> m_listHostsRemotable;

		protected MatrixExprHost[] MatrixHosts;

		[CLSCompliant(false)]
		protected IList<MatrixExprHost> m_matrixHostsRemotable;

		protected ChartExprHost[] ChartHosts;

		[CLSCompliant(false)]
		protected IList<ChartExprHost> m_chartHostsRemotable;

		protected TableExprHost[] TableHosts;

		[CLSCompliant(false)]
		protected IList<TableExprHost> m_tableHostsRemotable;

		protected OWCChartExprHost[] OWCChartHosts;

		[CLSCompliant(false)]
		protected IList<OWCChartExprHost> m_OWCChartHostsRemotable;

		[CLSCompliant(false)]
		protected IList<CustomReportItemExprHost> m_customReportItemHostsRemotable;

		public virtual object ReportLanguageExpr
		{
			get
			{
				return null;
			}
		}

		internal IList<AggregateParamExprHost> AggregateParamHostsRemotable
		{
			get
			{
				if (this.m_aggregateParamHostsRemotable == null && this.AggregateParamHosts != null)
				{
					this.m_aggregateParamHostsRemotable = new RemoteArrayWrapper<AggregateParamExprHost>(this.AggregateParamHosts);
				}
				return this.m_aggregateParamHostsRemotable;
			}
		}

		internal IList<ReportParamExprHost> ReportParameterHostsRemotable
		{
			get
			{
				if (this.m_reportParameterHostsRemotable == null && this.ReportParameterHosts != null)
				{
					this.m_reportParameterHostsRemotable = new RemoteArrayWrapper<ReportParamExprHost>(this.ReportParameterHosts);
				}
				return this.m_reportParameterHostsRemotable;
			}
		}

		internal IList<DataSourceExprHost> DataSourceHostsRemotable
		{
			get
			{
				if (this.m_dataSourceHostsRemotable == null && this.DataSourceHosts != null)
				{
					this.m_dataSourceHostsRemotable = new RemoteArrayWrapper<DataSourceExprHost>(this.DataSourceHosts);
				}
				return this.m_dataSourceHostsRemotable;
			}
		}

		internal IList<DataSetExprHost> DataSetHostsRemotable
		{
			get
			{
				if (this.m_dataSetHostsRemotable == null && this.DataSetHosts != null)
				{
					this.m_dataSetHostsRemotable = new RemoteArrayWrapper<DataSetExprHost>(this.DataSetHosts);
				}
				return this.m_dataSetHostsRemotable;
			}
		}

		internal IList<StyleExprHost> PageSectionHostsRemotable
		{
			get
			{
				if (this.m_pageSectionHostsRemotable == null && this.PageSectionHosts != null)
				{
					this.m_pageSectionHostsRemotable = new RemoteArrayWrapper<StyleExprHost>(this.PageSectionHosts);
				}
				return this.m_pageSectionHostsRemotable;
			}
		}

		internal IList<ReportItemExprHost> LineHostsRemotable
		{
			get
			{
				if (this.m_lineHostsRemotable == null && this.LineHosts != null)
				{
					this.m_lineHostsRemotable = new RemoteArrayWrapper<ReportItemExprHost>(this.LineHosts);
				}
				return this.m_lineHostsRemotable;
			}
		}

		internal IList<ReportItemExprHost> RectangleHostsRemotable
		{
			get
			{
				if (this.m_rectangleHostsRemotable == null && this.RectangleHosts != null)
				{
					this.m_rectangleHostsRemotable = new RemoteArrayWrapper<ReportItemExprHost>(this.RectangleHosts);
				}
				return this.m_rectangleHostsRemotable;
			}
		}

		internal IList<TextBoxExprHost> TextBoxHostsRemotable
		{
			get
			{
				if (this.m_textBoxHostsRemotable == null && this.TextBoxHosts != null)
				{
					this.m_textBoxHostsRemotable = new RemoteArrayWrapper<TextBoxExprHost>(this.TextBoxHosts);
				}
				return this.m_textBoxHostsRemotable;
			}
		}

		internal IList<ImageExprHost> ImageHostsRemotable
		{
			get
			{
				if (this.m_imageHostsRemotable == null && this.ImageHosts != null)
				{
					this.m_imageHostsRemotable = new RemoteArrayWrapper<ImageExprHost>(this.ImageHosts);
				}
				return this.m_imageHostsRemotable;
			}
		}

		internal IList<SubreportExprHost> SubreportHostsRemotable
		{
			get
			{
				if (this.m_subreportHostsRemotable == null && this.SubreportHosts != null)
				{
					this.m_subreportHostsRemotable = new RemoteArrayWrapper<SubreportExprHost>(this.SubreportHosts);
				}
				return this.m_subreportHostsRemotable;
			}
		}

		internal IList<ActiveXControlExprHost> ActiveXControlHostsRemotable
		{
			get
			{
				if (this.m_activeXControlHostsRemotable == null && this.ActiveXControlHosts != null)
				{
					this.m_activeXControlHostsRemotable = new RemoteArrayWrapper<ActiveXControlExprHost>(this.ActiveXControlHosts);
				}
				return this.m_activeXControlHostsRemotable;
			}
		}

		internal IList<ListExprHost> ListHostsRemotable
		{
			get
			{
				if (this.m_listHostsRemotable == null && this.ListHosts != null)
				{
					this.m_listHostsRemotable = new RemoteArrayWrapper<ListExprHost>(this.ListHosts);
				}
				return this.m_listHostsRemotable;
			}
		}

		internal IList<MatrixExprHost> MatrixHostsRemotable
		{
			get
			{
				if (this.m_matrixHostsRemotable == null && this.MatrixHosts != null)
				{
					this.m_matrixHostsRemotable = new RemoteArrayWrapper<MatrixExprHost>(this.MatrixHosts);
				}
				return this.m_matrixHostsRemotable;
			}
		}

		internal IList<ChartExprHost> ChartHostsRemotable
		{
			get
			{
				if (this.m_chartHostsRemotable == null && this.ChartHosts != null)
				{
					this.m_chartHostsRemotable = new RemoteArrayWrapper<ChartExprHost>(this.ChartHosts);
				}
				return this.m_chartHostsRemotable;
			}
		}

		internal IList<TableExprHost> TableHostsRemotable
		{
			get
			{
				if (this.m_tableHostsRemotable == null && this.TableHosts != null)
				{
					this.m_tableHostsRemotable = new RemoteArrayWrapper<TableExprHost>(this.TableHosts);
				}
				return this.m_tableHostsRemotable;
			}
		}

		internal IList<OWCChartExprHost> OWCChartHostsRemotable
		{
			get
			{
				if (this.m_OWCChartHostsRemotable == null && this.OWCChartHosts != null)
				{
					this.m_OWCChartHostsRemotable = new RemoteArrayWrapper<OWCChartExprHost>(this.OWCChartHosts);
				}
				return this.m_OWCChartHostsRemotable;
			}
		}

		internal IList<CustomReportItemExprHost> CustomReportItemHostsRemotable
		{
			get
			{
				return this.m_customReportItemHostsRemotable;
			}
		}

		protected ReportExprHost()
		{
		}

		protected ReportExprHost(object reportObjectModel)
		{
			base.SetReportObjectModel((ObjectModel)reportObjectModel);
		}

		internal void CustomCodeOnInit()
		{
			if (this.m_codeProxyBase != null)
			{
				this.m_codeProxyBase.CallOnInit();
			}
		}
	}
}
