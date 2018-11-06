using System.Xml.Serialization;

namespace AspNetCore.ReportingServices.RdlObjectModel
{
	internal abstract class ReportObjectBase : IContainedObject
	{
		private IPropertyStore m_propertyStore;

		[XmlIgnore]
		internal IPropertyStore PropertyStore
		{
			get
			{
				return this.m_propertyStore;
			}
		}

		[XmlIgnore]
		public IContainedObject Parent
		{
			get
			{
				return this.m_propertyStore.Parent;
			}
			set
			{
				this.m_propertyStore.Parent = value;
			}
		}

		protected ReportObjectBase()
		{
			this.m_propertyStore = this.WrapPropertyStore(new PropertyStore((ReportObject)this));
			this.Initialize();
		}

		internal ReportObjectBase(IPropertyStore propertyStore)
		{
			this.m_propertyStore = this.WrapPropertyStore(propertyStore);
		}

		public virtual void Initialize()
		{
		}

		internal virtual IPropertyStore WrapPropertyStore(IPropertyStore propertyStore)
		{
			return propertyStore;
		}
	}
}
