using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class PageBreakProperties : IStorable, IPersistable
	{
		private bool m_pageBreakAtStart;

		private bool m_pageBreakAtEnd;

		private bool m_resetPageNumber;

		private object m_source;

		private static Declaration m_declaration = PageBreakProperties.GetDeclaration();

		public bool PageBreakAtStart
		{
			get
			{
				return this.m_pageBreakAtStart;
			}
			set
			{
				this.m_pageBreakAtStart = value;
			}
		}

		public bool PageBreakAtEnd
		{
			get
			{
				return this.m_pageBreakAtEnd;
			}
			set
			{
				this.m_pageBreakAtEnd = value;
			}
		}

		public bool ResetPageNumber
		{
			get
			{
				return this.m_resetPageNumber;
			}
			set
			{
				this.m_resetPageNumber = value;
			}
		}

		public object Source
		{
			get
			{
				return this.m_source;
			}
		}

		public int Size
		{
			get
			{
				return AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.ReferenceSize + 3;
			}
		}

		internal PageBreakProperties()
		{
		}

		private PageBreakProperties(PageBreak pageBreak, object source)
		{
			PageBreakLocation breakLocation = pageBreak.BreakLocation;
			this.m_pageBreakAtStart = (breakLocation == PageBreakLocation.Start || breakLocation == PageBreakLocation.StartAndEnd);
			this.m_pageBreakAtEnd = (breakLocation == PageBreakLocation.End || breakLocation == PageBreakLocation.StartAndEnd);
			this.m_resetPageNumber = pageBreak.Instance.ResetPageNumber;
			this.m_source = source;
		}

		internal static PageBreakProperties Create(PageBreak pageBreak, object source, PageContext pageContext)
		{
			if (pageBreak.BreakLocation != 0)
			{
				if (!pageBreak.Instance.Disabled)
				{
					return new PageBreakProperties(pageBreak, source);
				}
				pageContext.Common.TracePageBreakIgnoredDisabled(source);
			}
			return null;
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(PageBreakProperties.m_declaration);
			IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.PageBreakAtStart:
					writer.Write(this.m_pageBreakAtStart);
					break;
				case MemberName.PageBreakAtEnd:
					writer.Write(this.m_pageBreakAtEnd);
					break;
				case MemberName.ResetPageNumber:
					writer.Write(this.m_resetPageNumber);
					break;
				case MemberName.Source:
				{
					int value = scalabilityCache.StoreStaticReference(this.m_source);
					writer.Write(value);
					break;
				}
				default:
					RSTrace.RenderingTracer.Assert(false, string.Empty);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(PageBreakProperties.m_declaration);
			IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.PageBreakAtStart:
					this.m_pageBreakAtStart = reader.ReadBoolean();
					break;
				case MemberName.PageBreakAtEnd:
					this.m_pageBreakAtEnd = reader.ReadBoolean();
					break;
				case MemberName.ResetPageNumber:
					this.m_resetPageNumber = reader.ReadBoolean();
					break;
				case MemberName.Source:
				{
					int id = reader.ReadInt32();
					this.m_source = scalabilityCache.FetchStaticReference(id);
					break;
				}
				default:
					RSTrace.RenderingTracer.Assert(false, string.Empty);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public ObjectType GetObjectType()
		{
			return ObjectType.PageBreakProperties;
		}

		internal static Declaration GetDeclaration()
		{
			if (PageBreakProperties.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.PageBreakAtStart, Token.Boolean));
				list.Add(new MemberInfo(MemberName.PageBreakAtEnd, Token.Boolean));
				list.Add(new MemberInfo(MemberName.ResetPageNumber, Token.Boolean));
				list.Add(new MemberInfo(MemberName.Source, Token.Int32));
				return new Declaration(ObjectType.PageBreakProperties, ObjectType.None, list);
			}
			return PageBreakProperties.m_declaration;
		}
	}
}
