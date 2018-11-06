using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	[PersistedWithinRequestOnly]
	[SkipStaticValidation]
	internal abstract class StreamingNoRowsScopeInstanceBase : IOnDemandScopeInstance, IStorable, IPersistable
	{
		private readonly OnDemandProcessingContext m_odpContext;

		private readonly IRIFReportDataScope m_dataScope;

		public bool IsNoRows
		{
			get
			{
				return true;
			}
		}

		public bool IsMostRecentlyCreatedScopeInstance
		{
			get
			{
				return false;
			}
		}

		public bool HasUnProcessedServerAggregate
		{
			get
			{
				return false;
			}
		}

		public int Size
		{
			get
			{
				Global.Tracer.Assert(false, "Size may not be used on a no rows scope instance.");
				throw new InvalidOperationException();
			}
		}

		public StreamingNoRowsScopeInstanceBase(OnDemandProcessingContext odpContext, IRIFReportDataScope dataScope)
		{
			this.m_odpContext = odpContext;
			this.m_dataScope = dataScope;
		}

		public void SetupEnvironment()
		{
			this.m_odpContext.EnsureRuntimeEnvironmentForDataSet(this.m_dataScope.DataScopeInfo.DataSet, true);
			this.m_odpContext.ReportObjectModel.ResetFieldValues();
			this.m_dataScope.ResetAggregates(this.m_odpContext.ReportObjectModel.AggregatesImpl);
		}

		public IOnDemandMemberOwnerInstanceReference GetDataRegionInstance(AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion rifDataRegion)
		{
			return null;
		}

		public IReference<IDataCorrelation> GetIdcReceiver(IRIFReportDataScope scope)
		{
			return null;
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			Global.Tracer.Assert(false, "Serialize may not be used on a no rows scope instance.");
			throw new InvalidOperationException();
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			Global.Tracer.Assert(false, "Deserialize may not be used on a no rows scope instance.");
			throw new InvalidOperationException();
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false, "ResolveReferences may not be used on a no rows scope instance.");
			throw new InvalidOperationException();
		}

		public abstract AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType();
	}
}
