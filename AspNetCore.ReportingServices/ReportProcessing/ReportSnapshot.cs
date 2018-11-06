using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReportSnapshot
	{
		private DateTime m_executionTime;

		private Report m_report;

		private ParameterInfoCollection m_parameters;

		private ReportInstance m_reportInstance;

		private bool m_hasDocumentMap;

		private bool m_hasShowHide;

		private bool m_hasBookmarks;

		private bool m_hasImageStreams;

		private string m_requestUserName;

		private string m_reportServerUrl;

		private string m_reportFolder;

		private string m_language;

		private ProcessingMessageList m_processingMessages;

		private Int64List m_pageSectionOffsets;

		[NonSerialized]
		private InfoBase m_documentMap;

		[NonSerialized]
		private InfoBase m_showHideSenderInfo;

		[NonSerialized]
		private InfoBase m_showHideReceiverInfo;

		[NonSerialized]
		private InfoBase m_quickFind;

		[NonSerialized]
		private BookmarksHashtable m_bookmarksInfo;

		[NonSerialized]
		private ReportDrillthroughInfo m_drillthroughInfo;

		[NonSerialized]
		private InfoBase m_sortFilterEventInfo;

		[NonSerialized]
		private List<PageSectionInstance> m_pageSections;

		[NonSerialized]
		private string m_reportName;

		internal Report Report
		{
			get
			{
				return this.m_report;
			}
			set
			{
				this.m_report = value;
			}
		}

		internal ParameterInfoCollection Parameters
		{
			get
			{
				return this.m_parameters;
			}
			set
			{
				this.m_parameters = value;
			}
		}

		internal ReportInstance ReportInstance
		{
			get
			{
				return this.m_reportInstance;
			}
			set
			{
				this.m_reportInstance = value;
			}
		}

		internal bool HasDocumentMap
		{
			get
			{
				return this.m_hasDocumentMap;
			}
			set
			{
				this.m_hasDocumentMap = value;
			}
		}

		internal bool HasBookmarks
		{
			get
			{
				return this.m_hasBookmarks;
			}
			set
			{
				this.m_hasBookmarks = value;
			}
		}

		internal bool HasShowHide
		{
			get
			{
				return this.m_hasShowHide;
			}
			set
			{
				this.m_hasShowHide = value;
			}
		}

		internal bool HasImageStreams
		{
			get
			{
				return this.m_hasImageStreams;
			}
			set
			{
				this.m_hasImageStreams = value;
			}
		}

		internal string RequestUserName
		{
			get
			{
				return this.m_requestUserName;
			}
			set
			{
				this.m_requestUserName = value;
			}
		}

		internal DateTime ExecutionTime
		{
			get
			{
				return this.m_executionTime;
			}
			set
			{
				this.m_executionTime = value;
			}
		}

		internal string ReportServerUrl
		{
			get
			{
				return this.m_reportServerUrl;
			}
			set
			{
				this.m_reportServerUrl = value;
			}
		}

		internal string ReportFolder
		{
			get
			{
				return this.m_reportFolder;
			}
			set
			{
				this.m_reportFolder = value;
			}
		}

		internal string Language
		{
			get
			{
				return this.m_language;
			}
			set
			{
				this.m_language = value;
			}
		}

		internal ProcessingMessageList Warnings
		{
			get
			{
				return this.m_processingMessages;
			}
			set
			{
				this.m_processingMessages = value;
			}
		}

		internal Int64List PageSectionOffsets
		{
			get
			{
				return this.m_pageSectionOffsets;
			}
			set
			{
				this.m_pageSectionOffsets = value;
			}
		}

		internal List<PageSectionInstance> PageSections
		{
			get
			{
				return this.m_pageSections;
			}
			set
			{
				this.m_pageSections = value;
			}
		}

		internal OffsetInfo DocumentMapOffset
		{
			set
			{
				this.m_documentMap = value;
			}
		}

		internal OffsetInfo ShowHideSenderInfoOffset
		{
			set
			{
				this.m_showHideSenderInfo = value;
			}
		}

		internal OffsetInfo ShowHideReceiverInfoOffset
		{
			set
			{
				this.m_showHideReceiverInfo = value;
			}
		}

		internal OffsetInfo QuickFindOffset
		{
			set
			{
				this.m_quickFind = value;
			}
		}

		internal DocumentMapNode DocumentMap
		{
			get
			{
				if (this.m_documentMap == null)
				{
					return null;
				}
				if (this.m_documentMap is DocumentMapNode)
				{
					return (DocumentMapNode)this.m_documentMap;
				}
				Global.Tracer.Assert(false, string.Empty);
				return null;
			}
			set
			{
				this.m_documentMap = value;
			}
		}

		internal BookmarksHashtable BookmarksInfo
		{
			get
			{
				return this.m_bookmarksInfo;
			}
			set
			{
				this.m_bookmarksInfo = value;
			}
		}

		internal ReportDrillthroughInfo DrillthroughInfo
		{
			get
			{
				return this.m_drillthroughInfo;
			}
			set
			{
				this.m_drillthroughInfo = value;
			}
		}

		internal SenderInformationHashtable ShowHideSenderInfo
		{
			get
			{
				if (this.m_showHideSenderInfo == null)
				{
					return null;
				}
				if (this.m_showHideSenderInfo is SenderInformationHashtable)
				{
					return (SenderInformationHashtable)this.m_showHideSenderInfo;
				}
				Global.Tracer.Assert(false, string.Empty);
				return null;
			}
			set
			{
				this.m_showHideSenderInfo = value;
			}
		}

		internal ReceiverInformationHashtable ShowHideReceiverInfo
		{
			get
			{
				if (this.m_showHideReceiverInfo == null)
				{
					return null;
				}
				if (this.m_showHideReceiverInfo is ReceiverInformationHashtable)
				{
					return (ReceiverInformationHashtable)this.m_showHideReceiverInfo;
				}
				Global.Tracer.Assert(false, string.Empty);
				return null;
			}
			set
			{
				this.m_showHideReceiverInfo = value;
			}
		}

		internal QuickFindHashtable QuickFind
		{
			get
			{
				if (this.m_quickFind == null)
				{
					return null;
				}
				if (this.m_quickFind is QuickFindHashtable)
				{
					return (QuickFindHashtable)this.m_quickFind;
				}
				Global.Tracer.Assert(false, string.Empty);
				return null;
			}
			set
			{
				this.m_quickFind = value;
			}
		}

		internal SortFilterEventInfoHashtable SortFilterEventInfo
		{
			get
			{
				if (this.m_sortFilterEventInfo == null)
				{
					return null;
				}
				if (this.m_sortFilterEventInfo is SortFilterEventInfoHashtable)
				{
					return (SortFilterEventInfoHashtable)this.m_sortFilterEventInfo;
				}
				Global.Tracer.Assert(false, string.Empty);
				return null;
			}
			set
			{
				this.m_sortFilterEventInfo = value;
			}
		}

		internal OffsetInfo SortFilterEventInfoOffset
		{
			get
			{
				if (this.m_sortFilterEventInfo == null)
				{
					return null;
				}
				Global.Tracer.Assert(this.m_sortFilterEventInfo is OffsetInfo);
				return (OffsetInfo)this.m_sortFilterEventInfo;
			}
			set
			{
				this.m_sortFilterEventInfo = value;
			}
		}

		internal ReportSnapshot(Report report, string reportName, ParameterInfoCollection parameters, string requestUserName, DateTime executionTime, string reportServerUrl, string reportFolder, string language)
		{
			this.m_report = report;
			this.m_reportName = reportName;
			this.m_parameters = parameters;
			this.m_requestUserName = requestUserName;
			this.m_executionTime = executionTime;
			this.m_reportServerUrl = reportServerUrl;
			this.m_reportFolder = reportFolder;
			this.m_language = language;
		}

		internal ReportSnapshot()
		{
			this.m_executionTime = DateTime.Now;
		}

		internal void CreateNavigationActions(ReportProcessing.NavigationInfo navigationInfo)
		{
			if (this.m_reportInstance != null)
			{
				if (navigationInfo.DocumentMapChildren != null && 0 < navigationInfo.DocumentMapChildren.Count && navigationInfo.DocumentMapChildren[0] != null)
				{
					this.m_documentMap = new DocumentMapNode(this.m_reportInstance.UniqueName.ToString(CultureInfo.InvariantCulture), this.m_reportName, 0, navigationInfo.DocumentMapChildren[0]);
					this.m_hasDocumentMap = true;
				}
				if (navigationInfo.BookmarksInfo != null)
				{
					this.m_bookmarksInfo = navigationInfo.BookmarksInfo;
					this.m_hasBookmarks = true;
				}
			}
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.ExecutionTime, Token.DateTime));
			memberInfoList.Add(new MemberInfo(MemberName.Report, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Report));
			memberInfoList.Add(new MemberInfo(MemberName.Parameters, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ParameterInfoCollection));
			memberInfoList.Add(new MemberInfo(MemberName.ReportInstance, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportInstance));
			memberInfoList.Add(new MemberInfo(MemberName.HasDocumentMap, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.HasShowHide, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.HasBookmarks, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.HasImageStreams, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.RequestUserName, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.ReportServerUrl, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.ReportFolder, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.Language, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.ProcessingMessages, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ProcessingMessageList));
			memberInfoList.Add(new MemberInfo(MemberName.PageSectionOffsets, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Int64List));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}

		internal DocumentMapNode GetDocumentMap(ChunkManager.RenderingChunkManager chunkManager)
		{
			IntermediateFormatReader intermediateFormatReader = null;
			if (this.m_documentMap != null)
			{
				if (this.m_documentMap is OffsetInfo)
				{
					intermediateFormatReader = chunkManager.GetReaderForSpecialChunk(((OffsetInfo)this.m_documentMap).Offset);
					goto IL_004c;
				}
				return (DocumentMapNode)this.m_documentMap;
			}
			if (this.m_hasDocumentMap)
			{
				intermediateFormatReader = chunkManager.GetSpecialChunkReader(ChunkManager.SpecialChunkName.DocumentMap);
			}
			goto IL_004c;
			IL_004c:
			if (intermediateFormatReader != null)
			{
				return intermediateFormatReader.ReadDocumentMapNode();
			}
			return null;
		}

		private void GetShowHideInfo(ChunkManager.RenderingChunkManager chunkManager)
		{
			if (this.m_showHideSenderInfo == null && this.m_showHideReceiverInfo == null)
			{
				IntermediateFormatReader specialChunkReader = chunkManager.GetSpecialChunkReader(ChunkManager.SpecialChunkName.ShowHideInfo);
				if (specialChunkReader != null)
				{
					this.m_showHideSenderInfo = specialChunkReader.ReadSenderInformationHashtable();
					this.m_showHideReceiverInfo = specialChunkReader.ReadReceiverInformationHashtable();
				}
			}
		}

		internal SenderInformationHashtable GetShowHideSenderInfo(ChunkManager.RenderingChunkManager chunkManager)
		{
			if (this.m_showHideSenderInfo == null)
			{
				this.GetShowHideInfo(chunkManager);
			}
			else if (this.m_showHideSenderInfo is OffsetInfo)
			{
				IntermediateFormatReader readerForSpecialChunk = chunkManager.GetReaderForSpecialChunk(((OffsetInfo)this.m_showHideSenderInfo).Offset);
				this.m_showHideSenderInfo = readerForSpecialChunk.ReadSenderInformationHashtable();
			}
			return (SenderInformationHashtable)this.m_showHideSenderInfo;
		}

		internal ReceiverInformationHashtable GetShowHideReceiverInfo(ChunkManager.RenderingChunkManager chunkManager)
		{
			if (this.m_showHideReceiverInfo == null)
			{
				this.GetShowHideInfo(chunkManager);
			}
			else if (this.m_showHideReceiverInfo is OffsetInfo)
			{
				IntermediateFormatReader readerForSpecialChunk = chunkManager.GetReaderForSpecialChunk(((OffsetInfo)this.m_showHideReceiverInfo).Offset);
				this.m_showHideReceiverInfo = readerForSpecialChunk.ReadReceiverInformationHashtable();
			}
			return (ReceiverInformationHashtable)this.m_showHideReceiverInfo;
		}

		internal QuickFindHashtable GetQuickFind(ChunkManager.RenderingChunkManager chunkManager)
		{
			IntermediateFormatReader intermediateFormatReader = null;
			if (this.m_quickFind != null)
			{
				if (this.m_quickFind is OffsetInfo)
				{
					intermediateFormatReader = chunkManager.GetReaderForSpecialChunk(((OffsetInfo)this.m_quickFind).Offset);
					goto IL_0044;
				}
				return (QuickFindHashtable)this.m_quickFind;
			}
			intermediateFormatReader = chunkManager.GetSpecialChunkReader(ChunkManager.SpecialChunkName.QuickFind);
			goto IL_0044;
			IL_0044:
			if (intermediateFormatReader != null)
			{
				return intermediateFormatReader.ReadQuickFindHashtable();
			}
			return null;
		}

		internal BookmarksHashtable GetBookmarksInfo(ChunkManager.RenderingChunkManager chunkManager)
		{
			if (this.m_bookmarksInfo != null)
			{
				return this.m_bookmarksInfo;
			}
			IntermediateFormatReader specialChunkReader = chunkManager.GetSpecialChunkReader(ChunkManager.SpecialChunkName.Bookmark);
			if (specialChunkReader != null)
			{
				return specialChunkReader.ReadBookmarksHashtable();
			}
			return null;
		}

		internal SortFilterEventInfoHashtable GetSortFilterEventInfo(ChunkManager.RenderingChunkManager chunkManager)
		{
			IntermediateFormatReader intermediateFormatReader = null;
			if (this.m_sortFilterEventInfo != null)
			{
				return (SortFilterEventInfoHashtable)this.m_sortFilterEventInfo;
			}
			intermediateFormatReader = chunkManager.GetSpecialChunkReader(ChunkManager.SpecialChunkName.SortFilterEventInfo);
			if (intermediateFormatReader != null)
			{
				return intermediateFormatReader.ReadSortFilterEventInfoHashtable();
			}
			return null;
		}

		internal List<PageSectionInstance> GetPageSections(int pageNumber, ChunkManager.RenderingChunkManager chunkManager, PageSection headerDef, PageSection footerDef)
		{
			List<PageSectionInstance> result = null;
			int startPage = default(int);
			IntermediateFormatReader pageSectionReader = chunkManager.GetPageSectionReader(pageNumber, out startPage);
			if (pageSectionReader != null)
			{
				result = pageSectionReader.ReadPageSections(pageNumber, startPage, headerDef, footerDef);
				chunkManager.SetPageSectionReaderState(pageSectionReader.ReaderState, pageNumber);
			}
			return result;
		}
	}
}
