using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence
{
	internal class GlobalIDOwnerCollection
	{
		private int m_currentID = -1;

		private Dictionary<int, IGloballyReferenceable> m_globallyReferenceableItems;

		internal int LastAssignedID
		{
			get
			{
				return this.m_currentID;
			}
		}

		internal GlobalIDOwnerCollection()
		{
			this.m_globallyReferenceableItems = new Dictionary<int, IGloballyReferenceable>(EqualityComparers.Int32ComparerInstance);
		}

		internal int GetGlobalID()
		{
			return ++this.m_currentID;
		}

		internal void Add(IGloballyReferenceable globallyReferenceableItem)
		{
			this.m_globallyReferenceableItems.Add(this.m_currentID, globallyReferenceableItem);
		}

		internal bool TryGetValue(int refID, out IGloballyReferenceable referenceableItem)
		{
			return this.m_globallyReferenceableItems.TryGetValue(refID, out referenceableItem);
		}
	}
}
