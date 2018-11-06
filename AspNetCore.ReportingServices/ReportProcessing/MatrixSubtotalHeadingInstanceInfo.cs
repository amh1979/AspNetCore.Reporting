using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class MatrixSubtotalHeadingInstanceInfo : MatrixHeadingInstanceInfo
	{
		private object[] m_styleAttributeValues;

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

		internal MatrixSubtotalHeadingInstanceInfo(ReportProcessing.ProcessingContext pc, int headingCellIndex, MatrixHeading matrixHeadingDef, MatrixHeadingInstance owner, bool isSubtotal, int reportItemDefIndex, VariantList groupExpressionValues, out NonComputedUniqueNames nonComputedUniqueNames)
			: base(pc, headingCellIndex, matrixHeadingDef, owner, isSubtotal, reportItemDefIndex, groupExpressionValues, out nonComputedUniqueNames)
		{
			Global.Tracer.Assert(isSubtotal);
			Global.Tracer.Assert(null != matrixHeadingDef.Subtotal);
			Global.Tracer.Assert(null != matrixHeadingDef.Subtotal.StyleClass);
			if (matrixHeadingDef.Subtotal.StyleClass.ExpressionList != null)
			{
				this.m_styleAttributeValues = new object[matrixHeadingDef.Subtotal.StyleClass.ExpressionList.Count];
				ReportProcessing.RuntimeRICollection.EvaluateStyleAttributes(ObjectType.Subtotal, matrixHeadingDef.Grouping.Name, matrixHeadingDef.Subtotal.StyleClass, owner.UniqueName, this.m_styleAttributeValues, pc);
			}
		}

		internal MatrixSubtotalHeadingInstanceInfo()
		{
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.StyleAttributeValues, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.InstanceInfo, memberInfoList);
		}
	}
}
