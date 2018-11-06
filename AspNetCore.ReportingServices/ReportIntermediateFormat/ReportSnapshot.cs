using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class ReportSnapshot : IPersistable
	{
		private DateTime m_executionTime;

		private Report m_report;

		private bool m_hasDocumentMap;

		private bool? m_definitionHasDocumentMap = false;

		private string m_documentMapRenderFormat;

		private bool m_hasShowHide;

		private bool m_hasBookmarks;

		private string m_requestUserName;

		private string m_reportServerUrl;

		private string m_reportFolder;

		private string m_language;

		private ProcessingMessageList m_processingMessages;

		private Dictionary<string, string> m_cachedDatabaseImages;

		private Dictionary<string, string> m_cachedGeneratedReportItems;

		private ParameterInfoCollection m_parameters;

		private bool m_hasUserSortFilter;

		private Dictionary<string, List<string>> m_aggregateFieldReferences;

		[NonSerialized]
		private bool m_cachedDataChanged;

		[NonSerialized]
		private ReportInstance m_reportInstance;

		[NonSerialized]
		private SortFilterEventInfoMap m_sortFilterEventInfo;

		[NonSerialized]
		private string m_reportName;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ReportSnapshot.GetDeclaration();

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

		internal bool HasDocumentMap
		{
			get
			{
				return this.m_hasDocumentMap;
			}
			set
			{
				this.m_hasDocumentMap = value;
				this.m_cachedDataChanged = true;
			}
		}

		private bool DocumentMapHasRenderFormatDependency
		{
			get
			{
				return this.m_documentMapRenderFormat != null;
			}
		}

		internal bool DefinitionTreeHasDocumentMap
		{
			get
			{
				if (!this.m_definitionHasDocumentMap.HasValue)
				{
					this.m_definitionHasDocumentMap = this.m_report.HasLabels;
				}
				return this.m_definitionHasDocumentMap.Value;
			}
			set
			{
				this.m_definitionHasDocumentMap = value;
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

		internal bool HasUserSortFilter
		{
			get
			{
				return this.m_hasUserSortFilter;
			}
			set
			{
				this.m_hasUserSortFilter = value;
			}
		}

		internal string RequestUserName
		{
			get
			{
				return this.m_requestUserName;
			}
		}

		internal DateTime ExecutionTime
		{
			get
			{
				return this.m_executionTime;
			}
		}

		internal string ReportServerUrl
		{
			get
			{
				return this.m_reportServerUrl;
			}
		}

		internal string ReportFolder
		{
			get
			{
				return this.m_reportFolder;
			}
		}

		internal string Language
		{
			get
			{
				return this.m_language;
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

		internal bool CachedDataChanged
		{
			get
			{
				return this.m_cachedDataChanged;
			}
		}

		internal ParameterInfoCollection Parameters
		{
			get
			{
				return this.m_parameters;
			}
		}

		internal Dictionary<string, List<string>> AggregateFieldReferences
		{
			get
			{
				if (this.m_aggregateFieldReferences == null)
				{
					this.m_aggregateFieldReferences = new Dictionary<string, List<string>>();
				}
				return this.m_aggregateFieldReferences;
			}
		}

		internal SortFilterEventInfoMap SortFilterEventInfo
		{
			get
			{
				return this.m_sortFilterEventInfo;
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
			this.m_hasDocumentMap = report.HasLabels;
			this.m_definitionHasDocumentMap = report.HasLabels;
			this.m_hasBookmarks = report.HasBookmarks;
			this.m_cachedDataChanged = true;
		}

		internal ReportSnapshot()
		{
			this.m_executionTime = DateTime.Now;
			this.m_cachedDataChanged = false;
		}

		internal bool CanUseExistingDocumentMapChunk(ICatalogItemContext reportContext)
		{
			if (!this.HasDocumentMap)
			{
				return false;
			}
			if (!this.DocumentMapHasRenderFormatDependency)
			{
				return true;
			}
			string format = ReportSnapshot.NormalizeRenderFormatForDocumentMap(reportContext);
			return RenderFormatImpl.IsRenderFormat(format, this.m_documentMapRenderFormat);
		}

		private static string NormalizeRenderFormatForDocumentMap(ICatalogItemContext reportContext)
		{
			bool flag = default(bool);
			string result = RenderFormatImpl.NormalizeRenderFormat(reportContext, out flag);
			if (flag)
			{
				result = "RPL";
			}
			return result;
		}

		internal void SetRenderFormatDependencyInDocumentMap(OnDemandProcessingContext odpContext)
		{
			this.m_cachedDataChanged = true;
			this.m_documentMapRenderFormat = ReportSnapshot.NormalizeRenderFormatForDocumentMap(odpContext.TopLevelContext.ReportContext);
		}

		internal void AddImageChunkName(string definitionKey, string name)
		{
			this.m_cachedDataChanged = true;
			if (this.m_cachedDatabaseImages == null)
			{
				this.m_cachedDatabaseImages = new Dictionary<string, string>(EqualityComparers.StringComparerInstance);
			}
			this.m_cachedDatabaseImages.Add(definitionKey, name);
		}

		internal bool TryGetImageChunkName(string definitionKey, out string name)
		{
			if (this.m_cachedDatabaseImages != null)
			{
				return this.m_cachedDatabaseImages.TryGetValue(definitionKey, out name);
			}
			name = null;
			return false;
		}

		internal void AddGeneratedReportItemChunkName(string definitionKey, string name)
		{
			this.m_cachedDataChanged = true;
			if (this.m_cachedGeneratedReportItems == null)
			{
				this.m_cachedGeneratedReportItems = new Dictionary<string, string>(EqualityComparers.StringComparerInstance);
			}
			this.m_cachedGeneratedReportItems.Add(definitionKey, name);
		}

		internal bool TryGetGeneratedReportItemChunkName(string definitionKey, out string name)
		{
			if (this.m_cachedGeneratedReportItems != null)
			{
				return this.m_cachedGeneratedReportItems.TryGetValue(definitionKey, out name);
			}
			name = null;
			return false;
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(ReportSnapshot.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ExecutionTime:
					writer.Write(this.m_executionTime);
					break;
				case MemberName.Report:
					Global.Tracer.Assert(null != this.m_report);
					writer.WriteReference(this.m_report);
					break;
				case MemberName.HasDocumentMap:
					writer.Write(this.m_hasDocumentMap);
					break;
				case MemberName.HasShowHide:
					writer.Write(this.m_hasShowHide);
					break;
				case MemberName.HasBookmarks:
					writer.Write(this.m_hasBookmarks);
					break;
				case MemberName.RequestUserName:
					writer.Write(this.m_requestUserName);
					break;
				case MemberName.ReportServerUrl:
					writer.Write(this.m_reportServerUrl);
					break;
				case MemberName.ReportFolder:
					writer.Write(this.m_reportFolder);
					break;
				case MemberName.Language:
					writer.Write(this.m_language);
					break;
				case MemberName.ProcessingMessages:
					writer.Write(this.m_processingMessages);
					break;
				case MemberName.Parameters:
					writer.Write((ArrayList)null);
					break;
				case MemberName.ImageChunkNames:
					writer.WriteStringStringHashtable(this.m_cachedDatabaseImages);
					break;
				case MemberName.GeneratedReportItemChunkNames:
					writer.WriteStringStringHashtable(this.m_cachedGeneratedReportItems);
					break;
				case MemberName.HasUserSortFilter:
					writer.Write(this.m_hasUserSortFilter);
					break;
				case MemberName.AggregateFieldReferences:
					writer.WriteStringListOfStringDictionary(this.m_aggregateFieldReferences);
					break;
				case MemberName.SnapshotParameters:
					writer.Write((IPersistable)this.m_parameters);
					break;
				case MemberName.DefinitionHasDocumentMap:
					writer.Write(this.m_definitionHasDocumentMap);
					break;
				case MemberName.DocumentMapRenderFormat:
					writer.Write(this.m_documentMapRenderFormat);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			ParameterInfoCollection parameterInfoCollection = null;
			ParameterInfoCollection parameterInfoCollection2 = null;
			reader.RegisterDeclaration(ReportSnapshot.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.ExecutionTime:
					this.m_executionTime = reader.ReadDateTime();
					break;
				case MemberName.Report:
					this.m_report = reader.ReadReference<Report>(this);
					break;
				case MemberName.HasDocumentMap:
					this.m_hasDocumentMap = reader.ReadBoolean();
					break;
				case MemberName.HasShowHide:
					this.m_hasShowHide = reader.ReadBoolean();
					break;
				case MemberName.HasBookmarks:
					this.m_hasBookmarks = reader.ReadBoolean();
					break;
				case MemberName.RequestUserName:
					this.m_requestUserName = reader.ReadString();
					break;
				case MemberName.ReportServerUrl:
					this.m_reportServerUrl = reader.ReadString();
					break;
				case MemberName.ReportFolder:
					this.m_reportFolder = reader.ReadString();
					break;
				case MemberName.Language:
					this.m_language = reader.ReadString();
					break;
				case MemberName.ProcessingMessages:
					this.m_processingMessages = reader.ReadListOfRIFObjects<ProcessingMessageList>();
					break;
				case MemberName.Parameters:
					parameterInfoCollection = new ParameterInfoCollection();
					reader.ReadListOfRIFObjects(parameterInfoCollection);
					break;
				case MemberName.ImageChunkNames:
					this.m_cachedDatabaseImages = reader.ReadStringStringHashtable<Dictionary<string, string>>();
					break;
				case MemberName.GeneratedReportItemChunkNames:
					this.m_cachedGeneratedReportItems = reader.ReadStringStringHashtable<Dictionary<string, string>>();
					break;
				case MemberName.HasUserSortFilter:
					this.m_hasUserSortFilter = reader.ReadBoolean();
					break;
				case MemberName.AggregateFieldReferences:
					this.m_aggregateFieldReferences = reader.ReadStringListOfStringDictionary();
					break;
				case MemberName.SnapshotParameters:
					parameterInfoCollection2 = (ParameterInfoCollection)reader.ReadRIFObject();
					break;
				case MemberName.DefinitionHasDocumentMap:
				{
					object obj = reader.ReadVariant();
					if (obj != null)
					{
						this.m_definitionHasDocumentMap = (bool)obj;
					}
					break;
				}
				case MemberName.DocumentMapRenderFormat:
					this.m_documentMapRenderFormat = reader.ReadString();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
			this.m_parameters = parameterInfoCollection;
			if (parameterInfoCollection != null && parameterInfoCollection.Count != 0)
			{
				return;
			}
			if (parameterInfoCollection2 != null)
			{
				this.m_parameters = parameterInfoCollection2;
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			List<MemberReference> list = default(List<MemberReference>);
			if (memberReferencesCollection.TryGetValue(ReportSnapshot.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item in list)
				{
					MemberName memberName = item.MemberName;
					if (memberName == MemberName.Report)
					{
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						this.m_report = (Report)referenceableItems[item.RefID];
					}
					else
					{
						Global.Tracer.Assert(false);
					}
				}
			}
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportSnapshot;
		}

		[SkipMemberStaticValidation(MemberName.Parameters)]
		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ExecutionTime, Token.DateTime));
			list.Add(new MemberInfo(MemberName.Report, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Report, Token.Reference));
			list.Add(new MemberInfo(MemberName.HasDocumentMap, Token.Boolean));
			list.Add(new MemberInfo(MemberName.HasShowHide, Token.Boolean));
			list.Add(new MemberInfo(MemberName.HasBookmarks, Token.Boolean));
			list.Add(new MemberInfo(MemberName.RequestUserName, Token.String));
			list.Add(new MemberInfo(MemberName.ReportServerUrl, Token.String));
			list.Add(new MemberInfo(MemberName.ReportFolder, Token.String));
			list.Add(new MemberInfo(MemberName.Language, Token.String));
			list.Add(new MemberInfo(MemberName.Parameters, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterInfo));
			list.Add(new MemberInfo(MemberName.ImageChunkNames, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StringStringHashtable));
			list.Add(new MemberInfo(MemberName.HasUserSortFilter, Token.Boolean));
			list.Add(new MemberInfo(MemberName.GeneratedReportItemChunkNames, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StringStringHashtable));
			list.Add(new MemberInfo(MemberName.AggregateFieldReferences, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StringListOfStringDictionary, Token.String, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList));
			list.Add(new MemberInfo(MemberName.SnapshotParameters, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterInfoCollection));
			list.Add(new MemberInfo(MemberName.ProcessingMessages, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ProcessingMessage));
			list.Add(new MemberInfo(MemberName.DefinitionHasDocumentMap, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Nullable, Token.Boolean));
			list.Add(new MemberInfo(MemberName.DocumentMapRenderFormat, Token.String));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportSnapshot, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}
	}
}
