using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal class MatrixCellInstance : InstanceInfoOwner
	{
		private ReportItemInstance m_content;

		internal ReportItemInstance Content
		{
			get
			{
				return this.m_content;
			}
			set
			{
				this.m_content = value;
			}
		}

		internal MatrixCellInstance(int rowIndex, int colIndex, Matrix matrixDef, int cellDefIndex, ReportProcessing.ProcessingContext pc, out NonComputedUniqueNames nonComputedUniqueNames)
		{
			base.m_instanceInfo = new MatrixCellInstanceInfo(rowIndex, colIndex, matrixDef, cellDefIndex, pc, this, out nonComputedUniqueNames);
		}

		internal MatrixCellInstance()
		{
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.ReportItemDef, Token.Reference, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItem));
			memberInfoList.Add(new MemberInfo(MemberName.Content, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemInstance));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.InstanceInfoOwner, memberInfoList);
		}

		internal MatrixCellInstanceInfo GetInstanceInfo(ChunkManager.RenderingChunkManager chunkManager)
		{
			if (base.m_instanceInfo is OffsetInfo)
			{
				IntermediateFormatReader reader = chunkManager.GetReader(((OffsetInfo)base.m_instanceInfo).Offset);
				return reader.ReadMatrixCellInstanceInfo();
			}
			return (MatrixCellInstanceInfo)base.m_instanceInfo;
		}
	}
}
