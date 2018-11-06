using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class GaugeCell : Cell, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GaugeCell.GetDeclaration();

		[NonSerialized]
		private List<GaugeInputValue> m_gaugeInputValues;

		[NonSerialized]
		private GaugeCellExprHost m_exprHost;

		protected override bool IsDataRegionBodyCell
		{
			get
			{
				return true;
			}
		}

		public AspNetCore.ReportingServices.ReportProcessing.ObjectType ObjectType
		{
			get
			{
				return AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugeCell;
			}
		}

		public override AspNetCore.ReportingServices.ReportProcessing.ObjectType DataScopeObjectType
		{
			get
			{
				return this.ObjectType;
			}
		}

		protected override AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode ExprHostDataRegionMode
		{
			get
			{
				return AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.GaugePanel;
			}
		}

		internal GaugeCell()
		{
		}

		internal GaugeCell(int id, GaugePanel gaugePanel)
			: base(id, gaugePanel)
		{
		}

		internal override void InternalInitialize(int parentRowID, int parentColumnID, int rowindex, int colIndex, InitializationContext context)
		{
			AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder exprHostBuilder = context.ExprHostBuilder;
			List<GaugeInputValue> gaugeInputValues = this.GetGaugeInputValues();
			if (gaugeInputValues != null)
			{
				for (int i = 0; i < gaugeInputValues.Count; i++)
				{
					gaugeInputValues[i].Initialize(context, i);
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> memberInfoList = new List<MemberInfo>();
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeCell, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Cell, memberInfoList);
		}

		internal void SetExprHost(GaugeCellExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			this.m_exprHost = exprHost;
			this.m_exprHost.SetReportObjectModel(reportObjectModel);
			IList<GaugeInputValueExprHost> gaugeInputValueHostsRemotable = this.m_exprHost.GaugeInputValueHostsRemotable;
			List<GaugeInputValue> gaugeInputValues = this.GetGaugeInputValues();
			if (gaugeInputValues != null && gaugeInputValueHostsRemotable != null)
			{
				for (int i = 0; i < gaugeInputValues.Count; i++)
				{
					GaugeInputValue gaugeInputValue = gaugeInputValues[i];
					if (gaugeInputValue != null && gaugeInputValue.ExpressionHostID > -1)
					{
						gaugeInputValue.SetExprHost(gaugeInputValueHostsRemotable[gaugeInputValue.ExpressionHostID], reportObjectModel);
					}
				}
			}
			base.BaseSetExprHost(exprHost, reportObjectModel);
		}

		private List<GaugeInputValue> GetGaugeInputValues()
		{
			if (this.m_gaugeInputValues == null)
			{
				this.m_gaugeInputValues = ((GaugePanel)base.DataRegionDef).GetGaugeInputValues();
			}
			return this.m_gaugeInputValues;
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeCell;
		}
	}
}
