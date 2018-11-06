using System;
using System.Threading;

namespace AspNetCore.ReportingServices.RdlObjectModel
{
	internal class DefinitionStore<T, E>
	{
		private static PropertyStore m_propertyDefinitions = new PropertyStore(null);

		private static ReaderWriterLock m_lock = new ReaderWriterLock();

		public static IPropertyDefinition GetProperty(int index)
		{
			DefinitionStore<T, E>.m_lock.AcquireReaderLock(-1);
			IPropertyDefinition propertyDefinition;
			try
			{
				propertyDefinition = (IPropertyDefinition)DefinitionStore<T, E>.m_propertyDefinitions.GetObject(index);
				if (propertyDefinition != null)
				{
					return propertyDefinition;
				}
			}
			finally
			{
				DefinitionStore<T, E>.m_lock.ReleaseReaderLock();
			}
			propertyDefinition = PropertyDefinition.Create(typeof(T), Enum.GetName(typeof(E), index));
			DefinitionStore<T, E>.m_lock.AcquireWriterLock(-1);
			try
			{
				DefinitionStore<T, E>.m_propertyDefinitions.SetObject(index, propertyDefinition);
				return propertyDefinition;
			}
			finally
			{
				DefinitionStore<T, E>.m_lock.ReleaseWriterLock();
			}
		}

		protected DefinitionStore()
		{
		}
	}
}
