using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class MatrixRow
	{
		private string m_height;

		private double m_heightValue;

		[NonSerialized]
		private int m_numberOfMatrixCells;

		internal string Height
		{
			get
			{
				return this.m_height;
			}
			set
			{
				this.m_height = value;
			}
		}

		internal double HeightValue
		{
			get
			{
				return this.m_heightValue;
			}
			set
			{
				this.m_heightValue = value;
			}
		}

		internal int NumberOfMatrixCells
		{
			get
			{
				return this.m_numberOfMatrixCells;
			}
			set
			{
				this.m_numberOfMatrixCells = value;
			}
		}

		internal void Initialize(InitializationContext context)
		{
			this.m_heightValue = context.ValidateSize(ref this.m_height, "Height");
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Height, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.HeightValue, Token.Double));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
