using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Common
{
	internal sealed class ObjectReferenceComparer<T> : EqualityComparer<T>
	{
		private static readonly ObjectReferenceComparer<T> m_instance = new ObjectReferenceComparer<T>();

		public static ObjectReferenceComparer<T> Instance
		{
			get
			{
				return ObjectReferenceComparer<T>.m_instance;
			}
		}

		private ObjectReferenceComparer()
		{
		}

		public override bool Equals(T x, T y)
		{
			return object.ReferenceEquals(x, y);
		}

		public override int GetHashCode(T x)
		{
			return x.GetHashCode();
		}
	}
}
