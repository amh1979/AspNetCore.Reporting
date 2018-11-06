using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class MatrixHeadingInstance : InstanceInfoOwner, IShowHideContainer
	{
		private int m_uniqueName;

		private ReportItemInstance m_content;

		private MatrixHeadingInstanceList m_subHeadingInstances;

		private bool m_isSubtotal;

		private RenderingPagesRangesList m_renderingPages;

		[NonSerialized]
		[Reference]
		private MatrixHeading m_matrixHeadingDef;

		[NonSerialized]
		private int m_headingDefIndex;

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

		internal MatrixHeading MatrixHeadingDef
		{
			get
			{
				return this.m_matrixHeadingDef;
			}
			set
			{
				this.m_matrixHeadingDef = value;
			}
		}

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

		internal MatrixHeadingInstanceList SubHeadingInstances
		{
			get
			{
				return this.m_subHeadingInstances;
			}
			set
			{
				this.m_subHeadingInstances = value;
			}
		}

		internal bool IsSubtotal
		{
			get
			{
				return this.m_isSubtotal;
			}
			set
			{
				this.m_isSubtotal = value;
			}
		}

		internal MatrixHeadingInstanceInfo InstanceInfo
		{
			get
			{
				if (base.m_instanceInfo is OffsetInfo)
				{
					Global.Tracer.Assert(false, string.Empty);
					return null;
				}
				return (MatrixHeadingInstanceInfo)base.m_instanceInfo;
			}
		}

		internal int HeadingIndex
		{
			get
			{
				return this.m_headingDefIndex;
			}
			set
			{
				this.m_headingDefIndex = value;
			}
		}

		internal RenderingPagesRangesList ChildrenStartAndEndPages
		{
			get
			{
				return this.m_renderingPages;
			}
			set
			{
				this.m_renderingPages = value;
			}
		}

		internal MatrixHeadingInstance(ReportProcessing.ProcessingContext pc, int headingCellIndex, MatrixHeading matrixHeadingDef, bool isSubtotal, int reportItemDefIndex, VariantList groupExpressionValues, out NonComputedUniqueNames nonComputedUniqueNames)
		{
			this.m_uniqueName = pc.CreateUniqueName();
			if (isSubtotal && matrixHeadingDef.Subtotal.StyleClass != null)
			{
				base.m_instanceInfo = new MatrixSubtotalHeadingInstanceInfo(pc, headingCellIndex, matrixHeadingDef, this, isSubtotal, reportItemDefIndex, groupExpressionValues, out nonComputedUniqueNames);
				if (matrixHeadingDef.GetInnerStaticHeading() != null)
				{
					this.m_subHeadingInstances = new MatrixHeadingInstanceList();
				}
			}
			else
			{
				base.m_instanceInfo = new MatrixHeadingInstanceInfo(pc, headingCellIndex, matrixHeadingDef, this, isSubtotal, reportItemDefIndex, groupExpressionValues, out nonComputedUniqueNames);
				if (matrixHeadingDef.SubHeading != null)
				{
					this.m_subHeadingInstances = new MatrixHeadingInstanceList();
				}
			}
			this.m_renderingPages = new RenderingPagesRangesList();
			this.m_matrixHeadingDef = matrixHeadingDef;
			this.m_isSubtotal = isSubtotal;
			this.m_headingDefIndex = reportItemDefIndex;
			if (!matrixHeadingDef.IsColumn)
			{
				pc.Pagination.EnterIgnoreHeight(matrixHeadingDef.StartHidden);
			}
			if (matrixHeadingDef.FirstHeadingInstances == null)
			{
				int count = matrixHeadingDef.ReportItems.Count;
				matrixHeadingDef.FirstHeadingInstances = new BoolList(count);
				for (int i = 0; i < count; i++)
				{
					matrixHeadingDef.FirstHeadingInstances.Add(true);
				}
			}
		}

		internal MatrixHeadingInstance()
		{
		}

		void IShowHideContainer.BeginProcessContainer(ReportProcessing.ProcessingContext context)
		{
			context.BeginProcessContainer(this.m_uniqueName, this.m_matrixHeadingDef.Visibility);
		}

		void IShowHideContainer.EndProcessContainer(ReportProcessing.ProcessingContext context)
		{
			context.EndProcessContainer(this.m_uniqueName, this.m_matrixHeadingDef.Visibility);
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.UniqueName, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.Content, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemInstance));
			memberInfoList.Add(new MemberInfo(MemberName.SubHeadingInstances, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.MatrixHeadingInstanceList));
			memberInfoList.Add(new MemberInfo(MemberName.IsSubtotal, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.ChildrenStartAndEndPages, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.RenderingPagesRangesList));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.InstanceInfoOwner, memberInfoList);
		}

		internal object Find(int index, int targetUniqueName, ref NonComputedUniqueNames nonCompNames, ChunkManager.RenderingChunkManager chunkManager)
		{
			object obj = null;
			ReportItemCollection reportItemCollection = (!this.IsSubtotal) ? this.MatrixHeadingDef.ReportItems : this.MatrixHeadingDef.Subtotal.ReportItems;
			if (reportItemCollection.Count > 0)
			{
				if (reportItemCollection.Count == 1)
				{
					index = 0;
				}
				if (reportItemCollection.IsReportItemComputed(index))
				{
					Global.Tracer.Assert(this.m_content != null, "The instance of a computed report item cannot be null.");
					obj = ((ISearchByUniqueName)this.m_content).Find(targetUniqueName, ref nonCompNames, chunkManager);
					if (obj != null)
					{
						return obj;
					}
				}
				else
				{
					NonComputedUniqueNames contentUniqueNames = this.GetInstanceInfo(chunkManager).ContentUniqueNames;
					obj = ((ISearchByUniqueName)reportItemCollection[index]).Find(targetUniqueName, ref contentUniqueNames, chunkManager);
					if (obj != null)
					{
						nonCompNames = contentUniqueNames;
						return obj;
					}
				}
			}
			if (this.m_subHeadingInstances != null)
			{
				return ((ISearchByUniqueName)this.m_subHeadingInstances).Find(targetUniqueName, ref nonCompNames, chunkManager);
			}
			return null;
		}

		internal MatrixHeadingInstanceInfo GetInstanceInfo(ChunkManager.RenderingChunkManager chunkManager)
		{
			if (base.m_instanceInfo is OffsetInfo)
			{
				IntermediateFormatReader reader = chunkManager.GetReader(((OffsetInfo)base.m_instanceInfo).Offset);
				return reader.ReadMatrixHeadingInstanceInfoBase();
			}
			return (MatrixHeadingInstanceInfo)base.m_instanceInfo;
		}
	}
}
