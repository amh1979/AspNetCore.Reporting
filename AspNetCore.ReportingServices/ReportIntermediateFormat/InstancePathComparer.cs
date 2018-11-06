using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal class InstancePathComparer : IEqualityComparer<List<InstancePathItem>>
	{
		private static InstancePathComparer m_instance = new InstancePathComparer();

		internal static InstancePathComparer Instance
		{
			get
			{
				return InstancePathComparer.m_instance;
			}
		}

		private InstancePathComparer()
		{
		}

		public bool Equals(List<InstancePathItem> instancePath1, List<InstancePathItem> instancePath2)
		{
			return InstancePathItem.IsSamePath(instancePath1, instancePath2);
		}

		public int GetHashCode(List<InstancePathItem> instancePath)
		{
			int num = 32452867;
			for (int i = 0; i < instancePath.Count; i++)
			{
				InstancePathItem instancePathItem = instancePath[i];
				if (!instancePathItem.IsEmpty)
				{
					int hashCode = instancePathItem.GetHashCode();
					num = (num >> 27 ^ num << 5) + hashCode;
				}
			}
			return num;
		}
	}
}
