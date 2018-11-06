using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal abstract class ReportItemInstanceInfo : InstanceInfo, IShowHideReceiver
	{
		protected object[] m_styleAttributeValues;

		protected bool m_startHidden;

		protected string m_label;

		protected string m_bookmark;

		protected string m_toolTip;

		protected DataValueInstanceList m_customPropertyInstances;

		[NonSerialized]
		protected ReportItem m_reportItemDef;

		internal object[] StyleAttributeValues
		{
			get
			{
				return this.m_styleAttributeValues;
			}
			set
			{
				this.m_styleAttributeValues = value;
			}
		}

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

		internal ReportItem ReportItemDef
		{
			get
			{
				return this.m_reportItemDef;
			}
			set
			{
				this.m_reportItemDef = value;
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

		internal string Bookmark
		{
			get
			{
				return this.m_bookmark;
			}
			set
			{
				this.m_bookmark = value;
			}
		}

		internal string ToolTip
		{
			get
			{
				return this.m_toolTip;
			}
			set
			{
				this.m_toolTip = value;
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

		protected ReportItemInstanceInfo(ReportProcessing.ProcessingContext pc, ReportItem reportItemDef, ReportItemInstance owner, int index)
		{
			this.ConstructorHelper(pc, reportItemDef, owner);
			if (pc.ChunkManager != null && !pc.DelayAddingInstanceInfo)
			{
				pc.ChunkManager.AddInstance(this, reportItemDef, owner, index, pc.InPageSection);
			}
			reportItemDef.StartHidden = this.m_startHidden;
		}

		protected ReportItemInstanceInfo(ReportProcessing.ProcessingContext pc, ReportItem reportItemDef, ReportItemInstance owner, int index, bool customCreated)
		{
			if (!customCreated)
			{
				this.ConstructorHelper(pc, reportItemDef, owner);
			}
			else
			{
				this.m_reportItemDef = reportItemDef;
			}
			if (pc.ChunkManager != null && !pc.DelayAddingInstanceInfo)
			{
				pc.ChunkManager.AddInstance(this, reportItemDef, owner, index, pc.InPageSection);
			}
			reportItemDef.StartHidden = this.m_startHidden;
		}

		protected ReportItemInstanceInfo(ReportProcessing.ProcessingContext pc, ReportItem reportItemDef, ReportItemInstance owner, bool addToChunk)
		{
			this.ConstructorHelper(pc, reportItemDef, owner);
			if (addToChunk && pc.ChunkManager != null && !pc.DelayAddingInstanceInfo)
			{
				pc.ChunkManager.AddInstance(this, owner, pc.InPageSection);
			}
			reportItemDef.StartHidden = this.m_startHidden;
		}

		protected ReportItemInstanceInfo(ReportItem reportItemDef)
		{
			this.m_reportItemDef = reportItemDef;
		}

		private void ConstructorHelper(ReportProcessing.ProcessingContext pc, ReportItem reportItemDef, ReportItemInstance owner)
		{
			this.m_reportItemDef = reportItemDef;
			Style styleClass = reportItemDef.StyleClass;
			if (styleClass != null && styleClass.ExpressionList != null && 0 < styleClass.ExpressionList.Count)
			{
				this.m_styleAttributeValues = new object[styleClass.ExpressionList.Count];
			}
			ReportProcessing.RuntimeRICollection.EvalReportItemAttr(reportItemDef, owner, this, pc);
			if (reportItemDef.CustomProperties != null)
			{
				this.m_customPropertyInstances = reportItemDef.CustomProperties.EvaluateExpressions(reportItemDef.ObjectType, reportItemDef.Name, null, pc);
			}
		}

		internal object GetStyleAttributeValue(int index)
		{
			Global.Tracer.Assert(this.m_styleAttributeValues != null && 0 <= index && index < this.m_styleAttributeValues.Length);
			return this.m_styleAttributeValues[index];
		}

		void IShowHideReceiver.ProcessReceiver(ReportProcessing.ProcessingContext context, int uniqueName)
		{
			this.m_startHidden = context.ProcessReceiver(uniqueName, this.m_reportItemDef.Visibility, this.m_reportItemDef.ExprHost, this.m_reportItemDef.ObjectType, this.m_reportItemDef.Name);
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.StyleAttributeValues, Token.Array, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new MemberInfo(MemberName.StartHidden, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.Label, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.Bookmark, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.ToolTip, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.CustomPropertyInstances, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.DataValueInstanceList));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.InstanceInfo, memberInfoList);
		}
	}
}
