using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	[StrongNameIdentityPermission(SecurityAction.InheritanceDemand, PublicKey = "0024000004800000940000000602000000240000525341310004000001000100272736ad6e5f9586bac2d531eabc3acc666c2f8ec879fa94f8f7b0327d2ff2ed523448f83c3d5c5dd2dfc7bc99c5286b2c125117bf5cbe242b9d41750732b2bdffe649c6efb8e5526d526fdd130095ecdb7bf210809c6cdad8824faa9ac0310ac3cba2aa0523567b2dfa7fe250b30facbd62d4ec99b94ac47c7d3b28f1f6e4c8")]
	internal abstract class ReportElementCollectionBase<T> : IEnumerable<T>, IEnumerable
	{
		internal class ReportElementEnumerator : IEnumerator<T>, IDisposable, IEnumerator
		{
			private ReportElementCollectionBase<T> m_collection;

			private int m_currentIndex = -1;

			public T Current
			{
				get
				{
					if (this.m_currentIndex >= 0 && this.m_currentIndex < this.m_collection.Count)
					{
						return this.m_collection[this.m_currentIndex];
					}
					return default(T);
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			internal ReportElementEnumerator(ReportElementCollectionBase<T> collection)
			{
				this.m_collection = collection;
			}

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				this.m_currentIndex++;
				return this.m_currentIndex < this.m_collection.Count;
			}

			public void Reset()
			{
				this.m_currentIndex = -1;
			}
		}

		public virtual T this[int i]
		{
			get
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
			set
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
		}

		public abstract int Count
		{
			get;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return new ReportElementEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
