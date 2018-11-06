using AspNetCore.ReportingServices.DataProcessing;
using System.Collections;
using System.Data;

namespace AspNetCore.ReportingServices.DataExtensions
{
	internal class ParameterCollectionWrapper : BaseDataWrapper, AspNetCore.ReportingServices.DataProcessing.IDataParameterCollection, IEnumerable
	{
		internal sealed class Enumerator : IEnumerator
		{
			private IEnumerator m_underlyingEnumerator;

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public ParameterWrapper Current
			{
				get
				{
					return (ParameterWrapper)this.m_underlyingEnumerator.Current;
				}
			}

			internal Enumerator(IEnumerator underlyingEnumerator)
			{
				this.m_underlyingEnumerator = underlyingEnumerator;
			}

			public bool MoveNext()
			{
				return this.m_underlyingEnumerator.MoveNext();
			}

			public void Reset()
			{
				this.m_underlyingEnumerator.Reset();
			}
		}

		private ArrayList m_parameters = new ArrayList();

		protected System.Data.IDataParameterCollection UnderlyingCollection
		{
			get
			{
				return (System.Data.IDataParameterCollection)base.UnderlyingObject;
			}
		}

		protected ArrayList Parameters
		{
			get
			{
				return this.m_parameters;
			}
		}

		protected internal ParameterCollectionWrapper(System.Data.IDataParameterCollection paramCollection)
			: base(paramCollection)
		{
		}

		public virtual int Add(AspNetCore.ReportingServices.DataProcessing.IDataParameter parameter)
		{
			ParameterWrapper parameterWrapper = (ParameterWrapper)parameter;
			int result = this.UnderlyingCollection.Add(parameterWrapper.UnderlyingParameter);
			this.Parameters.Add(parameterWrapper);
			return result;
		}

		public virtual Enumerator GetEnumerator()
		{
			return new Enumerator(this.Parameters.GetEnumerator());
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
