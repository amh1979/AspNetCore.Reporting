using System;
using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	[CLSCompliant(false)]
	internal sealed class RemoteMemberArrayWrapper<TMemberType> : MarshalByRefObject, IList<IMemberNode>, ICollection<IMemberNode>, IEnumerable<IMemberNode>, IEnumerable where TMemberType : IMemberNode
	{
		private readonly TMemberType[] m_array;

		public IMemberNode this[int index]
		{
			get
			{
				return (IMemberNode)(object)this.m_array[index];
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public int Count
		{
			get
			{
				return this.m_array.Length;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public RemoteMemberArrayWrapper(params TMemberType[] array)
		{
			this.m_array = array;
		}

		public int IndexOf(IMemberNode item)
		{
			throw new NotSupportedException();
		}

		public void Insert(int index, IMemberNode item)
		{
			throw new NotSupportedException();
		}

		public void RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		public void Add(IMemberNode item)
		{
			throw new NotSupportedException();
		}

		public void Clear()
		{
			throw new NotSupportedException();
		}

		public bool Contains(IMemberNode item)
		{
			throw new NotSupportedException();
		}

		public void CopyTo(IMemberNode[] array, int arrayIndex)
		{
			throw new NotSupportedException();
		}

		public bool Remove(IMemberNode item)
		{
			throw new NotSupportedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			for (int i = 0; i < this.m_array.Length; i++)
			{
				yield return (object)this.m_array[i];
			}
		}

		public IEnumerator<IMemberNode> GetEnumerator()
		{
			for (int i = 0; i < this.m_array.Length; i++)
			{
				yield return (IMemberNode)(object)this.m_array[i];
			}
		}
	}
}
