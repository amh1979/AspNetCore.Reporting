using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal class CommonSubReportInfo : IPersistable
	{
		private string m_description;

		private string m_reportPath;

		private string m_originalCatalogPath;

		private ParameterInfoCollection m_parametersFromCatalog;

		private bool m_retrievalFailed;

		private string m_definitionUniqueName;

		[NonSerialized]
		private IChunkFactory m_definitionChunkFactory;

		[NonSerialized]
		private static readonly Declaration m_Declaration = CommonSubReportInfo.GetDeclaration();

		internal string ReportPath
		{
			get
			{
				return this.m_reportPath;
			}
			set
			{
				this.m_reportPath = value;
			}
		}

		internal string OriginalCatalogPath
		{
			get
			{
				return this.m_originalCatalogPath;
			}
			set
			{
				this.m_originalCatalogPath = value;
			}
		}

		internal string Description
		{
			get
			{
				return this.m_description;
			}
			set
			{
				this.m_description = value;
			}
		}

		internal ParameterInfoCollection ParametersFromCatalog
		{
			get
			{
				return this.m_parametersFromCatalog;
			}
			set
			{
				this.m_parametersFromCatalog = value;
			}
		}

		internal bool RetrievalFailed
		{
			get
			{
				return this.m_retrievalFailed;
			}
			set
			{
				this.m_retrievalFailed = value;
			}
		}

		internal string DefinitionUniqueName
		{
			get
			{
				return this.m_definitionUniqueName;
			}
			set
			{
				this.m_definitionUniqueName = value;
			}
		}

		internal IChunkFactory DefinitionChunkFactory
		{
			get
			{
				return this.m_definitionChunkFactory;
			}
			set
			{
				this.m_definitionChunkFactory = value;
			}
		}

		internal CommonSubReportInfo()
		{
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ReportPath, Token.String));
			list.Add(new MemberInfo(MemberName.ParametersFromCatalog, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterInfo));
			list.Add(new MemberInfo(MemberName.RetrievalFailed, Token.Boolean));
			list.Add(new MemberInfo(MemberName.Description, Token.String));
			list.Add(new MemberInfo(MemberName.DefinitionUniqueName, Token.String));
			list.Add(new MemberInfo(MemberName.OriginalCatalogPath, Token.String));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.CommonSubReportInfo, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(CommonSubReportInfo.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.DefinitionUniqueName:
					writer.Write(this.m_definitionUniqueName);
					break;
				case MemberName.ReportPath:
					writer.Write(this.m_reportPath);
					break;
				case MemberName.ParametersFromCatalog:
					writer.Write((ArrayList)this.m_parametersFromCatalog);
					break;
				case MemberName.RetrievalFailed:
					writer.Write(this.m_retrievalFailed);
					break;
				case MemberName.Description:
					writer.Write(this.m_description);
					break;
				case MemberName.OriginalCatalogPath:
					writer.Write(this.m_originalCatalogPath);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(CommonSubReportInfo.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.DefinitionUniqueName:
					this.m_definitionUniqueName = reader.ReadString();
					break;
				case MemberName.ReportPath:
					this.m_reportPath = reader.ReadString();
					break;
				case MemberName.ParametersFromCatalog:
					this.m_parametersFromCatalog = reader.ReadListOfRIFObjects<ParameterInfoCollection>();
					break;
				case MemberName.RetrievalFailed:
					this.m_retrievalFailed = reader.ReadBoolean();
					break;
				case MemberName.Description:
					this.m_description = reader.ReadString();
					break;
				case MemberName.OriginalCatalogPath:
					this.m_originalCatalogPath = reader.ReadString();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
			if (this.m_originalCatalogPath == null)
			{
				this.m_originalCatalogPath = this.m_reportPath;
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false);
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.CommonSubReportInfo;
		}
	}
}
