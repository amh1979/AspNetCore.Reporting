using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLPageContent
	{
		private RPLSizes[] m_reportSectionSizes;

		private RPLPageLayout m_pageLayout;

		private long[] m_sectionOffsets;

		private int m_sectionCount;

		private Queue<RPLReportSection> m_sections;

		private long m_endOffset = -1L;

		private RPLContext m_context;

		private float m_maxSectionWidth = -1f;

		public RPLSizes[] ReportSectionSizes
		{
			get
			{
				return this.m_reportSectionSizes;
			}
			set
			{
				this.m_reportSectionSizes = value;
			}
		}

		public RPLPageLayout PageLayout
		{
			get
			{
				return this.m_pageLayout;
			}
			set
			{
				this.m_pageLayout = value;
			}
		}

		public float MaxSectionWidth
		{
			get
			{
				if (this.m_maxSectionWidth < 0.0)
				{
					for (int i = 0; i < this.m_reportSectionSizes.Length; i++)
					{
						this.m_maxSectionWidth = Math.Max(this.m_maxSectionWidth, this.m_reportSectionSizes[i].Width);
					}
				}
				return this.m_maxSectionWidth;
			}
		}

		internal long[] SectionOffsets
		{
			set
			{
				this.m_sectionOffsets = value;
			}
		}

		internal int SectionCount
		{
			set
			{
				this.m_sectionCount = value;
			}
		}

		internal RPLPageContent(long endOffset, RPLContext context, Version rplVersion)
		{
			this.m_endOffset = endOffset;
			this.m_context = context;
			switch (this.m_context.VersionPicker)
			{
			case RPLVersionEnum.RPL2008:
			case RPLVersionEnum.RPL2008WithImageConsolidation:
				RPLReader.ReadPageContent2008(this, endOffset, context);
				break;
			case RPLVersionEnum.RPLAccess:
			case RPLVersionEnum.RPLMap:
			case RPLVersionEnum.RPL2009:
				RPLReader.ReadPageContent(this, endOffset, context);
				break;
			default:
				throw new ArgumentException(RPLRes.UnsupportedRPLVersion(rplVersion.ToString(3), "10.6"));
			}
		}

		internal RPLPageContent(int sectionCount, RPLPageLayout pageLayout)
		{
			this.m_reportSectionSizes = new RPLSizes[sectionCount];
			this.m_pageLayout = pageLayout;
		}

		internal RPLPageContent(int sectionCount)
		{
			this.m_reportSectionSizes = new RPLSizes[sectionCount];
		}

		public bool HasNextReportSection()
		{
			if (this.m_sections == null && this.m_sectionCount == 0)
			{
				return false;
			}
			return true;
		}

		public RPLReportSection GetNextReportSection()
		{
			if (this.m_context != null && this.m_context.VersionPicker == RPLVersionEnum.RPL2008)
			{
				if (this.m_sections != null)
				{
					this.m_sectionCount--;
					RPLReportSection result = this.m_sections.Dequeue();
					this.m_sections = null;
					return result;
				}
				return null;
			}
			if (this.m_sections != null)
			{
				this.m_sectionCount--;
				RPLReportSection result2 = this.m_sections.Dequeue();
				if (this.m_sections.Count == 0)
				{
					this.m_sections = null;
				}
				return result2;
			}
			if (this.m_sectionCount == 0)
			{
				return null;
			}
			this.m_sectionCount--;
			return RPLReader.ReadReportSection(this.m_sectionOffsets[this.m_sectionOffsets.Length - this.m_sectionCount - 1], this.m_context);
		}

		internal void AddReportSection(RPLReportSection section)
		{
			if (this.m_sections == null)
			{
				this.m_sections = new Queue<RPLReportSection>();
			}
			this.m_sections.Enqueue(section);
			this.m_sectionCount++;
		}
	}
}
