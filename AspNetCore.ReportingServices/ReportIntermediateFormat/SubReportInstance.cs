using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal class SubReportInstance : ScopeInstance, IReportInstanceContainer
	{
		private ParametersImpl m_parameters;

		private IReference<ReportInstance> m_reportInstance;

		private string m_instanceUniqueName;

		private CultureInfo m_threadCulture;

		private SubReport.Status m_status;

		private bool m_processedWithError;

		private SubReport m_subReportDef;

		private bool? m_isInstanceShared = null;

		private int? m_dataChunkNameModifier = null;

		[NonSerialized]
		private bool m_initialized;

		[NonSerialized]
		private static readonly Declaration m_Declaration = SubReportInstance.GetDeclaration();

		internal override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType ObjectType
		{
			get
			{
				return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SubReportInstance;
			}
		}

		internal SubReport SubReportDef
		{
			get
			{
				return this.m_subReportDef;
			}
		}

		internal bool Initialized
		{
			get
			{
				return this.m_initialized;
			}
			set
			{
				this.m_initialized = value;
			}
		}

		internal ParametersImpl Parameters
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

		internal bool NoRows
		{
			get
			{
				if (this.m_reportInstance != null)
				{
					return this.m_reportInstance.Value().NoRows;
				}
				return false;
			}
		}

		public IReference<ReportInstance> ReportInstance
		{
			get
			{
				return this.m_reportInstance;
			}
		}

		internal string InstanceUniqueName
		{
			get
			{
				return this.m_instanceUniqueName;
			}
			set
			{
				this.m_instanceUniqueName = value;
			}
		}

		internal CultureInfo ThreadCulture
		{
			get
			{
				return this.m_threadCulture;
			}
			set
			{
				this.m_threadCulture = value;
			}
		}

		internal SubReport.Status RetrievalStatus
		{
			get
			{
				return this.m_status;
			}
			set
			{
				this.m_status = value;
			}
		}

		internal bool ProcessedWithError
		{
			get
			{
				return this.m_processedWithError;
			}
			set
			{
				this.m_processedWithError = value;
			}
		}

		public override int Size
		{
			get
			{
				return base.Size + ItemSizes.SizeOf(this.m_parameters) + ItemSizes.SizeOf(this.m_reportInstance) + ItemSizes.SizeOf(this.m_instanceUniqueName) + ItemSizes.ReferenceSize + 4 + 1 + ItemSizes.ReferenceSize + 1 + ItemSizes.SizeOf(this.m_dataChunkNameModifier) + ItemSizes.NullableInt32Size + ItemSizes.NullableBoolSize;
			}
		}

		internal SubReportInstance()
		{
		}

		private SubReportInstance(SubReport subreport, OnDemandMetadata odpMetadata)
		{
			this.m_subReportDef = subreport;
			this.m_reportInstance = odpMetadata.GroupTreeScalabilityCache.AllocateEmptyTreePartition<ReportInstance>(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportInstanceReference);
		}

		public IReference<ReportInstance> SetReportInstance(ReportInstance reportInstance, OnDemandMetadata odpMetadata)
		{
			odpMetadata.GroupTreeScalabilityCache.SetTreePartitionContentsAndPin(this.m_reportInstance, reportInstance);
			return this.m_reportInstance;
		}

		internal override void AddChildScope(IReference<ScopeInstance> child, int indexInCollection)
		{
			Global.Tracer.Assert(false);
		}

		internal string GetChunkNameModifier(SubReportInfo subReportInfo, bool useCachedValue, bool addEntry, out bool isShared)
		{
			if (!useCachedValue || !this.m_dataChunkNameModifier.HasValue)
			{
				if (!useCachedValue)
				{
					this.m_isInstanceShared = null;
				}
				this.m_dataChunkNameModifier = subReportInfo.GetChunkNameModifierForParamValues(this.m_parameters, addEntry, ref this.m_isInstanceShared, out this.m_parameters);
			}
			isShared = this.m_isInstanceShared.Value;
			return this.m_dataChunkNameModifier.Value.ToString(CultureInfo.InvariantCulture);
		}

		internal override void InstanceComplete()
		{
			if (this.m_reportInstance != null)
			{
				ReportInstance reportInstance = this.m_reportInstance.Value();
				if (reportInstance != null)
				{
					reportInstance.InstanceComplete();
				}
			}
			IReference<SubReportInstance> reference = (IReference<SubReportInstance>)base.m_cleanupRef;
			base.InstanceComplete();
			reference.PinValue();
		}

		internal static IReference<SubReportInstance> CreateInstance(ScopeInstance parentInstance, SubReport subReport, OnDemandMetadata odpMetadata)
		{
			SubReportInstance subReportInstance = new SubReportInstance(subReport, odpMetadata);
			IReference<SubReportInstance> reference = odpMetadata.GroupTreeScalabilityCache.AllocateAndPin(subReportInstance, 0);
			subReportInstance.m_cleanupRef = (IDisposable)reference;
			parentInstance.AddChildScope((IReference<ScopeInstance>)reference, subReport.IndexInCollection);
			return reference;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.SubReport, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SubReport, Token.GlobalReference));
			list.Add(new MemberInfo(MemberName.ReportInstance, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportInstanceReference));
			list.Add(new MemberInfo(MemberName.DataSetUniqueName, Token.String));
			list.Add(new MemberInfo(MemberName.ThreadCulture, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.CultureInfo));
			list.Add(new MemberInfo(MemberName.Parameters, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Parameters));
			list.Add(new MemberInfo(MemberName.Status, Token.Enum));
			list.Add(new MemberInfo(MemberName.ProcessedWithError, Token.Boolean));
			list.Add(new MemberInfo(MemberName.IsInstanceShared, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Nullable, Token.Boolean));
			list.Add(new MemberInfo(MemberName.DataChunkNameModifier, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Nullable, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SubReportInstance, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScopeInstance, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(SubReportInstance.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.SubReport:
					writer.WriteGlobalReference(this.m_subReportDef);
					break;
				case MemberName.Parameters:
					if (this.m_parameters != null)
					{
						writer.Write(new ParametersImplWrapper(this.m_parameters));
					}
					else
					{
						writer.WriteNull();
					}
					break;
				case MemberName.ReportInstance:
					writer.Write(this.m_reportInstance);
					break;
				case MemberName.DataSetUniqueName:
					writer.Write(this.m_instanceUniqueName);
					break;
				case MemberName.ThreadCulture:
					writer.Write(this.m_threadCulture);
					break;
				case MemberName.Status:
					writer.WriteEnum((int)this.m_status);
					break;
				case MemberName.ProcessedWithError:
					writer.Write(this.m_processedWithError);
					break;
				case MemberName.IsInstanceShared:
					writer.Write(this.m_isInstanceShared);
					break;
				case MemberName.DataChunkNameModifier:
					writer.Write(this.m_dataChunkNameModifier);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(SubReportInstance.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.SubReport:
					this.m_subReportDef = reader.ReadGlobalReference<SubReport>();
					break;
				case MemberName.Parameters:
				{
					ParametersImplWrapper parametersImplWrapper = (ParametersImplWrapper)reader.ReadRIFObject();
					if (parametersImplWrapper != null)
					{
						this.m_parameters = parametersImplWrapper.WrappedParametersImpl;
					}
					break;
				}
				case MemberName.ReportInstance:
					this.m_reportInstance = (IReference<ReportInstance>)reader.ReadRIFObject();
					break;
				case MemberName.DataSetUniqueName:
					this.m_instanceUniqueName = reader.ReadString();
					break;
				case MemberName.ThreadCulture:
					this.m_threadCulture = reader.ReadCultureInfo();
					break;
				case MemberName.Status:
					this.m_status = (SubReport.Status)reader.ReadEnum();
					break;
				case MemberName.ProcessedWithError:
					this.m_processedWithError = reader.ReadBoolean();
					break;
				case MemberName.IsInstanceShared:
				{
					object obj2 = reader.ReadVariant();
					if (obj2 != null)
					{
						this.m_isInstanceShared = (bool)obj2;
					}
					break;
				}
				case MemberName.DataChunkNameModifier:
				{
					object obj = reader.ReadVariant();
					if (obj != null)
					{
						this.m_dataChunkNameModifier = (int)obj;
					}
					break;
				}
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false);
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SubReportInstance;
		}
	}
}
