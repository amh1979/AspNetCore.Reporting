using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class CompiledTextRunInstanceCollection : ReportElementInstanceCollectionBase<CompiledTextRunInstance>, IList<ICompiledTextRunInstance>, ICollection<ICompiledTextRunInstance>, IEnumerable<ICompiledTextRunInstance>, IEnumerable
	{
		private CompiledRichTextInstance m_compiledRichTextInstance;

		private List<CompiledTextRunInstance> m_compiledTextRunInstances;

		public override CompiledTextRunInstance this[int i]
		{
			get
			{
				return this.m_compiledTextRunInstances[i];
			}
		}

		public override int Count
		{
			get
			{
				return this.m_compiledTextRunInstances.Count;
			}
		}

		ICompiledTextRunInstance IList<ICompiledTextRunInstance>.this[int index]
		{
			get
			{
				return this.m_compiledTextRunInstances[index];
			}
			set
			{
				this.m_compiledTextRunInstances[index] = (CompiledTextRunInstance)value;
			}
		}

		int ICollection<ICompiledTextRunInstance>.Count
		{
			get
			{
				return this.m_compiledTextRunInstances.Count;
			}
		}

		bool ICollection<ICompiledTextRunInstance>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		internal CompiledTextRunInstanceCollection(CompiledRichTextInstance compiledRichTextInstance)
		{
			this.m_compiledRichTextInstance = compiledRichTextInstance;
			this.m_compiledTextRunInstances = new List<CompiledTextRunInstance>();
		}

		int IList<ICompiledTextRunInstance>.IndexOf(ICompiledTextRunInstance item)
		{
			return this.m_compiledTextRunInstances.IndexOf((CompiledTextRunInstance)item);
		}

		void IList<ICompiledTextRunInstance>.Insert(int index, ICompiledTextRunInstance item)
		{
			this.m_compiledTextRunInstances.Insert(index, (CompiledTextRunInstance)item);
		}

		void IList<ICompiledTextRunInstance>.RemoveAt(int index)
		{
			this.m_compiledTextRunInstances.RemoveAt(index);
		}

		void ICollection<ICompiledTextRunInstance>.Add(ICompiledTextRunInstance item)
		{
			this.m_compiledTextRunInstances.Add((CompiledTextRunInstance)item);
		}

		void ICollection<ICompiledTextRunInstance>.Clear()
		{
			this.m_compiledTextRunInstances.Clear();
		}

		bool ICollection<ICompiledTextRunInstance>.Contains(ICompiledTextRunInstance item)
		{
			return this.m_compiledTextRunInstances.Contains((CompiledTextRunInstance)item);
		}

		void ICollection<ICompiledTextRunInstance>.CopyTo(ICompiledTextRunInstance[] array, int arrayIndex)
		{
			CompiledTextRunInstance[] array2 = new CompiledTextRunInstance[array.Length];
			this.m_compiledTextRunInstances.CopyTo(array2, arrayIndex);
			for (int i = 0; i < array2.Length; i++)
			{
				array[i] = array2[i];
			}
		}

		bool ICollection<ICompiledTextRunInstance>.Remove(ICompiledTextRunInstance item)
		{
			return this.m_compiledTextRunInstances.Remove((CompiledTextRunInstance)item);
		}

		IEnumerator<ICompiledTextRunInstance> IEnumerable<ICompiledTextRunInstance>.GetEnumerator()
		{
			foreach (CompiledTextRunInstance compiledTextRunInstance in this.m_compiledTextRunInstances)
			{
				yield return (ICompiledTextRunInstance)compiledTextRunInstance;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)this.m_compiledTextRunInstances).GetEnumerator();
		}
	}
}
