using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReportItemColInstance : InstanceInfoOwner, ISearchByUniqueName, IIndexInto
	{
		private ReportItemInstanceList m_reportItemInstances;

		private RenderingPagesRangesList m_childrenStartAndEndPages;

		[Reference]
		private ReportItemCollection m_reportItemColDef;

		[NonSerialized]
		private NonComputedUniqueNames[] m_childrenNonComputedUniqueNames;

		internal ReportItemInstanceList ReportItemInstances
		{
			get
			{
				return this.m_reportItemInstances;
			}
			set
			{
				this.m_reportItemInstances = value;
			}
		}

		internal ReportItemCollection ReportItemColDef
		{
			get
			{
				return this.m_reportItemColDef;
			}
			set
			{
				this.m_reportItemColDef = value;
			}
		}

		internal ReportItemInstance this[int index]
		{
			get
			{
				return this.m_reportItemInstances[index];
			}
		}

		internal NonComputedUniqueNames[] ChildrenNonComputedUniqueNames
		{
			get
			{
				return this.m_childrenNonComputedUniqueNames;
			}
			set
			{
				this.m_childrenNonComputedUniqueNames = value;
			}
		}

		internal RenderingPagesRangesList ChildrenStartAndEndPages
		{
			get
			{
				return this.m_childrenStartAndEndPages;
			}
			set
			{
				this.m_childrenStartAndEndPages = value;
			}
		}

		internal ReportItemColInstance(ReportProcessing.ProcessingContext pc, ReportItemCollection reportItemsDef)
		{
			this.m_reportItemColDef = reportItemsDef;
			if (reportItemsDef.ComputedReportItems != null)
			{
				this.m_reportItemInstances = new ReportItemInstanceList(reportItemsDef.ComputedReportItems.Count);
			}
			if (pc != null)
			{
				this.m_childrenNonComputedUniqueNames = NonComputedUniqueNames.CreateNonComputedUniqueNames(pc, reportItemsDef);
			}
			base.m_instanceInfo = new ReportItemColInstanceInfo(pc, reportItemsDef, this);
		}

		internal ReportItemColInstance()
		{
		}

		internal void Add(ReportItemInstance riInstance)
		{
			Global.Tracer.Assert(null != this.m_reportItemInstances);
			this.m_reportItemInstances.Add(riInstance);
		}

		internal int GetReportItemUniqueName(int index)
		{
			int num = -1;
			ReportItem reportItem = null;
			Global.Tracer.Assert(index >= 0 && index < this.m_reportItemColDef.Count);
			bool flag = default(bool);
			int num2 = default(int);
			this.m_reportItemColDef.GetReportItem(index, out flag, out num2, out reportItem);
			if (!flag)
			{
				NonComputedUniqueNames nonComputedUniqueNames = this.m_childrenNonComputedUniqueNames[num2];
				return nonComputedUniqueNames.UniqueName;
			}
			return this.m_reportItemInstances[num2].UniqueName;
		}

		internal void GetReportItemStartAndEndPages(int index, ref int startPage, ref int endPage)
		{
			Global.Tracer.Assert(index >= 0 && index < this.m_reportItemColDef.Count);
			if (this.m_childrenStartAndEndPages != null)
			{
				RenderingPagesRanges renderingPagesRanges = this.m_childrenStartAndEndPages[index];
				startPage = renderingPagesRanges.StartPage;
				endPage = renderingPagesRanges.EndPage;
			}
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.ReportItemInstances, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemInstanceList));
			memberInfoList.Add(new MemberInfo(MemberName.ChildrenStartAndEndPages, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.RenderingPagesRangesList));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.InstanceInfoOwner, memberInfoList);
		}

		object ISearchByUniqueName.Find(int targetUniqueName, ref NonComputedUniqueNames nonCompNames, ChunkManager.RenderingChunkManager chunkManager)
		{
			object obj = null;
			int count = this.m_reportItemColDef.Count;
			for (int i = 0; i < count; i++)
			{
				bool flag = default(bool);
				int num = default(int);
				ReportItem reportItem = default(ReportItem);
				this.m_reportItemColDef.GetReportItem(i, out flag, out num, out reportItem);
				if (flag)
				{
					obj = ((ISearchByUniqueName)this.m_reportItemInstances[num]).Find(targetUniqueName, ref nonCompNames, chunkManager);
					if (obj != null)
					{
						return obj;
					}
				}
				else
				{
					NonComputedUniqueNames nonComputedUniqueNames = this.GetInstanceInfo(chunkManager, false).ChildrenNonComputedUniqueNames[num];
					obj = ((ISearchByUniqueName)reportItem).Find(targetUniqueName, ref nonComputedUniqueNames, chunkManager);
					if (obj != null)
					{
						nonCompNames = nonComputedUniqueNames;
						return obj;
					}
				}
			}
			return null;
		}

		object IIndexInto.GetChildAt(int index, out NonComputedUniqueNames nonCompNames)
		{
			bool flag = default(bool);
			int num = default(int);
			ReportItem result = default(ReportItem);
			this.m_reportItemColDef.GetReportItem(index, out flag, out num, out result);
			if (flag)
			{
				nonCompNames = null;
				return this.m_reportItemInstances[num];
			}
			nonCompNames = ((ReportItemColInstanceInfo)base.m_instanceInfo).ChildrenNonComputedUniqueNames[num];
			return result;
		}

		internal ReportItemColInstanceInfo GetInstanceInfo(ChunkManager.RenderingChunkManager chunkManager, bool inPageSection)
		{
			if (base.m_instanceInfo is OffsetInfo)
			{
				Global.Tracer.Assert(null != chunkManager);
				IntermediateFormatReader intermediateFormatReader = null;
				intermediateFormatReader = ((!inPageSection) ? chunkManager.GetReader(((OffsetInfo)base.m_instanceInfo).Offset) : chunkManager.GetPageSectionInstanceReader(((OffsetInfo)base.m_instanceInfo).Offset));
				return intermediateFormatReader.ReadReportItemColInstanceInfo();
			}
			return (ReportItemColInstanceInfo)base.m_instanceInfo;
		}

		internal void SetPaginationForNonComputedChild(ReportProcessing.Pagination pagination, ReportItem reportItem, ReportItem parentDef)
		{
			ReportItemCollection reportItemColDef = this.m_reportItemColDef;
			int num = parentDef.StartPage;
			if (parentDef is Table)
			{
				num = ((Table)parentDef).CurrentPage;
			}
			else if (parentDef is Matrix)
			{
				num = ((Matrix)parentDef).CurrentPage;
			}
			reportItem.TopInStartPage = parentDef.TopInStartPage;
			if (reportItem.SiblingAboveMe != null)
			{
				for (int i = 0; i < reportItem.SiblingAboveMe.Count; i++)
				{
					ReportItem reportItem2 = reportItemColDef[reportItem.SiblingAboveMe[i]];
					int num2 = reportItem2.EndPage;
					double num3 = reportItem2.TopValue + reportItem2.HeightValue;
					if (num3 > reportItem.TopValue)
					{
						num2 = reportItem2.StartPage;
						reportItem.TopInStartPage = reportItem2.TopInStartPage;
					}
					else
					{
						bool flag = reportItem2.ShareMyLastPage;
						if (!(reportItem2 is Table) && !(reportItem2 is Matrix))
						{
							flag = false;
						}
						if (!pagination.IgnorePageBreak && pagination.PageBreakAtEnd(reportItem2) && !flag)
						{
							num2++;
							if (i == reportItem.SiblingAboveMe.Count - 1)
							{
								pagination.SetCurrentPageHeight(reportItem, reportItem.HeightValue);
								reportItem.TopInStartPage = 0.0;
							}
						}
						else
						{
							reportItem.TopInStartPage = pagination.CurrentPageHeight;
						}
					}
					num = Math.Max(num, num2);
				}
			}
			if (pagination.CanMoveToNextPage(pagination.PageBreakAtStart(reportItem)))
			{
				num++;
				reportItem.TopInStartPage = 0.0;
				pagination.SetCurrentPageHeight(reportItem, reportItem.HeightValue);
			}
			reportItem.StartPage = num;
			reportItem.EndPage = num;
		}
	}
}
