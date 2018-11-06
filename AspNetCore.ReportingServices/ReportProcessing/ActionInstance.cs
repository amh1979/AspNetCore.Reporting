using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ActionInstance
	{
		private ActionItemInstanceList m_actionItemsValues;

		private object[] m_styleAttributeValues;

		private int m_uniqueName;

		internal ActionItemInstanceList ActionItemsValues
		{
			get
			{
				return this.m_actionItemsValues;
			}
			set
			{
				this.m_actionItemsValues = value;
			}
		}

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

		internal int UniqueName
		{
			get
			{
				return this.m_uniqueName;
			}
			set
			{
				this.m_uniqueName = value;
			}
		}

		internal ActionInstance(ReportProcessing.ProcessingContext pc)
		{
			this.m_uniqueName = pc.CreateUniqueName();
		}

		internal ActionInstance(ActionItemInstance actionItemInstance)
		{
			this.m_actionItemsValues = new ActionItemInstanceList();
			this.m_actionItemsValues.Add(actionItemInstance);
		}

		internal ActionInstance()
		{
		}

		internal object GetStyleAttributeValue(int index)
		{
			Global.Tracer.Assert(this.m_styleAttributeValues != null && 0 <= index && index < this.m_styleAttributeValues.Length);
			return this.m_styleAttributeValues[index];
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.ActionItemList, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ActionItemInstanceList));
			memberInfoList.Add(new MemberInfo(MemberName.StyleAttributeValues, Token.Array, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new MemberInfo(MemberName.UniqueName, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
