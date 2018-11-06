using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class CompiledParagraphInstanceCollection : ReportElementInstanceCollectionBase<CompiledParagraphInstance>, IList<ICompiledParagraphInstance>, ICollection<ICompiledParagraphInstance>, IEnumerable<ICompiledParagraphInstance>, IEnumerable
	{
		private CompiledRichTextInstance m_compiledRichTextInstance;

		private List<CompiledParagraphInstance> m_compiledParagraphInstances;

		public override CompiledParagraphInstance this[int i]
		{
			get
			{
				return this.m_compiledParagraphInstances[i];
			}
		}

		public override int Count
		{
			get
			{
				return this.m_compiledParagraphInstances.Count;
			}
		}

		ICompiledParagraphInstance IList<ICompiledParagraphInstance>.this[int index]
		{
			get
			{
				return this.m_compiledParagraphInstances[index];
			}
			set
			{
				this.m_compiledParagraphInstances[index] = (CompiledParagraphInstance)value;
			}
		}

		int ICollection<ICompiledParagraphInstance>.Count
		{
			get
			{
				return this.m_compiledParagraphInstances.Count;
			}
		}

		bool ICollection<ICompiledParagraphInstance>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		internal CompiledParagraphInstanceCollection(CompiledRichTextInstance compiledRichTextInstance)
		{
			this.m_compiledRichTextInstance = compiledRichTextInstance;
			this.m_compiledParagraphInstances = new List<CompiledParagraphInstance>();
		}

		int IList<ICompiledParagraphInstance>.IndexOf(ICompiledParagraphInstance item)
		{
			return this.m_compiledParagraphInstances.IndexOf((CompiledParagraphInstance)item);
		}

		void IList<ICompiledParagraphInstance>.Insert(int index, ICompiledParagraphInstance item)
		{
			this.m_compiledParagraphInstances.Insert(index, (CompiledParagraphInstance)item);
		}

		void IList<ICompiledParagraphInstance>.RemoveAt(int index)
		{
			this.m_compiledParagraphInstances.RemoveAt(index);
		}

		void ICollection<ICompiledParagraphInstance>.Add(ICompiledParagraphInstance item)
		{
			this.m_compiledParagraphInstances.Add((CompiledParagraphInstance)item);
		}

		void ICollection<ICompiledParagraphInstance>.Clear()
		{
			this.m_compiledParagraphInstances.Clear();
		}

		bool ICollection<ICompiledParagraphInstance>.Contains(ICompiledParagraphInstance item)
		{
			return this.m_compiledParagraphInstances.Contains((CompiledParagraphInstance)item);
		}

		void ICollection<ICompiledParagraphInstance>.CopyTo(ICompiledParagraphInstance[] array, int arrayIndex)
		{
			CompiledParagraphInstance[] array2 = new CompiledParagraphInstance[array.Length];
			this.m_compiledParagraphInstances.CopyTo(array2, arrayIndex);
			for (int i = 0; i < array2.Length; i++)
			{
				array[i] = array2[i];
			}
		}

		bool ICollection<ICompiledParagraphInstance>.Remove(ICompiledParagraphInstance item)
		{
			return this.m_compiledParagraphInstances.Remove((CompiledParagraphInstance)item);
		}

		IEnumerator<ICompiledParagraphInstance> IEnumerable<ICompiledParagraphInstance>.GetEnumerator()
		{
			foreach (CompiledParagraphInstance compiledParagraphInstance in this.m_compiledParagraphInstances)
			{
				yield return (ICompiledParagraphInstance)compiledParagraphInstance;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)this.m_compiledParagraphInstances).GetEnumerator();
		}
	}
}
