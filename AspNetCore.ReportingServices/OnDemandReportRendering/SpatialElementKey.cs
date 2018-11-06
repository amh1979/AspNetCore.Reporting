using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal class SpatialElementKey
	{
		private List<object> m_keyValues;

		internal List<object> KeyValues
		{
			get
			{
				return this.m_keyValues;
			}
		}

		internal SpatialElementKey(List<object> values)
		{
			this.m_keyValues = values;
		}

		public override bool Equals(object obj)
		{
			SpatialElementKey spatialElementKey = (SpatialElementKey)obj;
			if (this.m_keyValues != null && spatialElementKey.m_keyValues != null)
			{
				if (this.m_keyValues.Count != spatialElementKey.m_keyValues.Count)
				{
					return false;
				}
				for (int i = 0; i < this.m_keyValues.Count; i++)
				{
					object obj2 = this.m_keyValues[i];
					if (obj2 == null)
					{
						return false;
					}
					if (!obj2.Equals(spatialElementKey.m_keyValues[i]))
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int num = 0;
			if (this.m_keyValues != null)
			{
				for (int i = 0; i < this.m_keyValues.Count; i++)
				{
					if (this.m_keyValues[i] != null)
					{
						num ^= this.m_keyValues[i].GetHashCode();
					}
				}
			}
			return num;
		}
	}
}
