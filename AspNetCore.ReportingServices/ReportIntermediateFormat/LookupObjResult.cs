using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal class LookupObjResult : IStorable, IPersistable, IErrorContext
	{
		private ProcessingErrorCode m_errorCode;

		private bool m_hasErrorCode;

		private Severity m_errorSeverity;

		private string[] m_errorMessageArgs;

		private DataFieldStatus m_dataFieldStatus;

		private ReferenceID m_lookupTablePartitionId = TreePartitionManager.EmptyTreePartitionID;

		[NonSerialized]
		private LookupTable m_lookupTable;

		[NonSerialized]
		private static readonly Declaration m_Declaration = LookupObjResult.GetDeclaration();

		internal bool ErrorOccured
		{
			get
			{
				if (!this.m_hasErrorCode)
				{
					return this.m_dataFieldStatus != DataFieldStatus.None;
				}
				return true;
			}
		}

		internal DataFieldStatus DataFieldStatus
		{
			get
			{
				return this.m_dataFieldStatus;
			}
			set
			{
				this.m_dataFieldStatus = value;
			}
		}

		internal bool HasErrorCode
		{
			get
			{
				return this.m_hasErrorCode;
			}
		}

		internal ProcessingErrorCode ErrorCode
		{
			get
			{
				return this.m_errorCode;
			}
		}

		internal Severity ErrorSeverity
		{
			get
			{
				return this.m_errorSeverity;
			}
		}

		internal string[] ErrorMessageArgs
		{
			get
			{
				return this.m_errorMessageArgs;
			}
		}

		internal bool HasBeenTransferred
		{
			get
			{
				return this.m_lookupTablePartitionId != TreePartitionManager.EmptyTreePartitionID;
			}
		}

		public int Size
		{
			get
			{
				return 1 + ItemSizes.SizeOf(this.m_lookupTable);
			}
		}

		internal LookupObjResult()
		{
		}

		internal LookupObjResult(LookupTable lookupTable)
		{
			this.m_lookupTable = lookupTable;
		}

		internal LookupTable GetLookupTable(OnDemandProcessingContext odpContext)
		{
			if (this.m_lookupTable == null)
			{
				Global.Tracer.Assert(this.HasBeenTransferred, "Invalid LookupObjResult: PartitionID for LookupTable is empty.");
				OnDemandMetadata odpMetadata = odpContext.OdpMetadata;
				odpMetadata.EnsureLookupScalabilitySetup(odpContext.ChunkFactory, odpContext.GetActiveCompatibilityVersion(), odpContext.ProhibitSerializableValues);
				long treePartitionOffset = odpMetadata.LookupPartitionManager.GetTreePartitionOffset(this.m_lookupTablePartitionId);
				LookupScalabilityCache lookupScalabilityCache = odpMetadata.LookupScalabilityCache;
				this.m_lookupTable = (LookupTable)lookupScalabilityCache.Storage.Retrieve(treePartitionOffset);
				this.m_lookupTable.SetEqualityComparer(odpContext.ProcessingComparer);
			}
			return this.m_lookupTable;
		}

		internal void TransferToLookupCache(OnDemandProcessingContext odpContext)
		{
			Global.Tracer.Assert(this.m_lookupTable != null, "Can't transfer a missing LookupTable");
			Global.Tracer.Assert(!this.HasBeenTransferred, "Can't transfer a LookupTable twice");
			OnDemandMetadata odpMetadata = odpContext.OdpMetadata;
			odpMetadata.EnsureLookupScalabilitySetup(odpContext.ChunkFactory, odpContext.GetActiveCompatibilityVersion(), odpContext.ProhibitSerializableValues);
			LookupScalabilityCache lookupScalabilityCache = odpMetadata.LookupScalabilityCache;
			IReference<LookupTable> reference = lookupScalabilityCache.AllocateEmptyTreePartition<LookupTable>(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupTableReference);
			this.m_lookupTable.TransferTo(lookupScalabilityCache);
			lookupScalabilityCache.SetTreePartitionContentsAndPin(reference, this.m_lookupTable);
			this.m_lookupTablePartitionId = reference.Id;
			reference.UnPinValue();
		}

		void IErrorContext.Register(ProcessingErrorCode code, Severity severity, params string[] arguments)
		{
			if (!this.m_hasErrorCode)
			{
				this.m_hasErrorCode = true;
				this.m_errorCode = code;
				this.m_errorSeverity = severity;
				this.m_errorMessageArgs = arguments;
			}
		}

		void IErrorContext.Register(ProcessingErrorCode code, Severity severity, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName, params string[] arguments)
		{
			((IErrorContext)this).Register(code, severity, arguments);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(LookupObjResult.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.LookupTablePartitionID:
					writer.Write(this.m_lookupTablePartitionId.Value);
					break;
				case MemberName.HasCode:
					writer.Write(this.m_hasErrorCode);
					break;
				case MemberName.Code:
					writer.WriteEnum((int)this.m_errorCode);
					break;
				case MemberName.Severity:
					writer.WriteEnum((int)this.m_errorSeverity);
					break;
				case MemberName.FieldStatus:
					writer.WriteEnum((int)this.m_dataFieldStatus);
					break;
				case MemberName.Arguments:
					writer.Write(this.m_errorMessageArgs);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(LookupObjResult.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.LookupTablePartitionID:
					this.m_lookupTablePartitionId = new ReferenceID(reader.ReadInt64());
					break;
				case MemberName.HasCode:
					this.m_hasErrorCode = reader.ReadBoolean();
					break;
				case MemberName.Code:
					this.m_errorCode = (ProcessingErrorCode)reader.ReadEnum();
					break;
				case MemberName.Severity:
					this.m_errorSeverity = (Severity)reader.ReadEnum();
					break;
				case MemberName.FieldStatus:
					this.m_dataFieldStatus = (DataFieldStatus)reader.ReadEnum();
					break;
				case MemberName.Arguments:
					this.m_errorMessageArgs = reader.ReadStringArray();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupObjResult;
		}

		public static Declaration GetDeclaration()
		{
			if (LookupObjResult.m_Declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.LookupTablePartitionID, Token.Int64));
				list.Add(new MemberInfo(MemberName.HasCode, Token.Boolean));
				list.Add(new MemberInfo(MemberName.Code, Token.Enum));
				list.Add(new MemberInfo(MemberName.Severity, Token.Enum));
				list.Add(new MemberInfo(MemberName.FieldStatus, Token.Enum));
				list.Add(new MemberInfo(MemberName.Arguments, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveArray, Token.String));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupInfo, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return LookupObjResult.m_Declaration;
		}
	}
}
