using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class RunningValueInfo : DataAggregateInfo
	{
		private string m_scope;

		internal string Scope
		{
			get
			{
				return this.m_scope;
			}
			set
			{
				this.m_scope = value;
			}
		}

		internal new RunningValueInfo DeepClone(InitializationContext context)
		{
			RunningValueInfo runningValueInfo = new RunningValueInfo();
			base.DeepCloneInternal(runningValueInfo, context);
			runningValueInfo.m_scope = context.EscalateScope(this.m_scope);
			return runningValueInfo;
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Scope, Token.String));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.DataAggregateInfo, memberInfoList);
		}
	}
}
