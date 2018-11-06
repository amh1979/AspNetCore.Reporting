using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLStyleProps : IRPLStyle, IEnumerable<byte>, IEnumerable
	{
		private Dictionary<byte, object> m_styleMap;

		public object this[byte styleName]
		{
			get
			{
				object result = null;
				if (this.m_styleMap.TryGetValue(styleName, out result))
				{
					return result;
				}
				return null;
			}
		}

		public int Count
		{
			get
			{
				return this.m_styleMap.Count;
			}
		}

		internal RPLStyleProps()
		{
			this.m_styleMap = new Dictionary<byte, object>();
		}

		internal void Add(byte name, object value)
		{
			this.m_styleMap.Add(name, value);
		}

		internal void AddAll(RPLStyleProps styleProps)
		{
			if (styleProps != null)
			{
				foreach (KeyValuePair<byte, object> item in styleProps.m_styleMap)
				{
					this.Add(item.Key, item.Value);
				}
			}
		}

		IEnumerator<byte> IEnumerable<byte>.GetEnumerator()
		{
			return (IEnumerator<byte>)(object)this.m_styleMap.Keys.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return (IEnumerator)(object)this.m_styleMap.Keys.GetEnumerator();
		}
	}
}
