using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class TableGroupInstanceInfo : InstanceInfo
	{
		private bool m_startHidden;

		private string m_label;

		private DataValueInstanceList m_customPropertyInstances;

		internal bool StartHidden
		{
			get
			{
				return this.m_startHidden;
			}
			set
			{
				this.m_startHidden = value;
			}
		}

		internal string Label
		{
			get
			{
				return this.m_label;
			}
			set
			{
				this.m_label = value;
			}
		}

		internal DataValueInstanceList CustomPropertyInstances
		{
			get
			{
				return this.m_customPropertyInstances;
			}
			set
			{
				this.m_customPropertyInstances = value;
			}
		}

		internal TableGroupInstanceInfo(ReportProcessing.ProcessingContext pc, TableGroup tableGroupDef, TableGroupInstance owner)
		{
			if (pc.ShowHideType != 0)
			{
				this.m_startHidden = pc.ProcessReceiver(owner.UniqueName, tableGroupDef.Visibility, tableGroupDef.ExprHost, tableGroupDef.DataRegionDef.ObjectType, tableGroupDef.DataRegionDef.Name);
			}
			tableGroupDef.StartHidden = this.m_startHidden;
			if (tableGroupDef.Grouping.GroupLabel != null)
			{
				this.m_label = pc.NavigationInfo.RegisterLabel(pc.ReportRuntime.EvaluateGroupingLabelExpression(tableGroupDef.Grouping, tableGroupDef.DataRegionDef.ObjectType, tableGroupDef.DataRegionDef.Name));
			}
			if (tableGroupDef.Grouping.CustomProperties != null)
			{
				this.m_customPropertyInstances = tableGroupDef.Grouping.CustomProperties.EvaluateExpressions(tableGroupDef.DataRegionDef.ObjectType, tableGroupDef.DataRegionDef.Name, tableGroupDef.Grouping.Name + ".", pc);
			}
			pc.ChunkManager.AddInstance(this, owner, pc.InPageSection);
		}

		internal TableGroupInstanceInfo()
		{
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.StartHidden, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.Label, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.CustomPropertyInstances, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.DataValueInstanceList));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.InstanceInfo, memberInfoList);
		}
	}
}
