using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal interface IMemberHierarchy
	{
		IList<DataRegionMemberInstance> GetChildMemberInstances(bool isRowMember, int memberIndexInCollection);

		IList<DataCellInstance> GetCellInstances(int columnMemberSequenceId);

		IDisposable AddCellInstance(int columnMemberSequenceId, int cellIndexInCollection, DataCellInstance cellInstance, IScalabilityCache cache);

		IDisposable AddMemberInstance(DataRegionMemberInstance instance, int indexInCollection, IScalabilityCache cache, out int instanceIndex);
	}
}
