using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class MatrixSubtotalCellInstance : MatrixCellInstance
	{
		[Reference]
		private MatrixHeadingInstance m_subtotalHeadingInstance;

		internal MatrixHeadingInstance SubtotalHeadingInstance
		{
			get
			{
				return this.m_subtotalHeadingInstance;
			}
			set
			{
				this.m_subtotalHeadingInstance = value;
			}
		}

		internal MatrixSubtotalCellInstance(int rowIndex, int colIndex, Matrix matrixDef, int cellDefIndex, ReportProcessing.ProcessingContext pc, out NonComputedUniqueNames nonComputedUniqueNames)
			: base(rowIndex, colIndex, matrixDef, cellDefIndex, pc, out nonComputedUniqueNames)
		{
			Global.Tracer.Assert(null != pc.HeadingInstance);
			Global.Tracer.Assert(null != pc.HeadingInstance.MatrixHeadingDef.Subtotal.StyleClass);
			this.m_subtotalHeadingInstance = pc.HeadingInstance;
		}

		internal MatrixSubtotalCellInstance()
		{
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.SubtotalHeadingInstance, Token.Reference, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.MatrixHeadingInstance));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.InstanceInfo, memberInfoList);
		}
	}
}
