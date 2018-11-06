using System;
using System.ComponentModel;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	[Serializable]
	internal class NamedElement : IDisposable, ICloneable
	{
		private string name = string.Empty;

		[NonSerialized]
		internal CommonElements common;

		[NonSerialized]
		internal NamedCollection collection;

		internal bool initialized = true;

		private object tag;

		internal bool disposed;

		[Browsable(false)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeCommon")]
		[DefaultValue("")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal virtual CommonElements Common
		{
			get
			{
				return this.common;
			}
			set
			{
				if (this.common != value)
				{
					if (value == null)
					{
						this.OnRemove();
						this.common = value;
					}
					else
					{
						this.common = value;
						this.OnAdded();
					}
				}
			}
		}

		[DefaultValue("")]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[Browsable(false)]
		[SRDescription("DescriptionAttributeCollection")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual NamedCollection Collection
		{
			get
			{
				return this.collection;
			}
			set
			{
				this.collection = value;
			}
		}

		[SerializationVisibility(SerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		[SRDescription("DescriptionAttributeParentElement")]
		[DefaultValue("")]
		public virtual NamedElement ParentElement
		{
			get
			{
				if (this.collection != null)
				{
					return this.collection.ParentElement;
				}
				return null;
			}
		}

		internal virtual string DefaultName
		{
			get
			{
				return string.Empty;
			}
		}

		[SRDescription("DescriptionAttributeName10")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public virtual string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				if (this.collection != null)
				{
					this.collection.IsValidNameCheck(value, this);
				}
				if (this.Common != null)
				{
					this.Common.GaugeCore.Notify(MessageType.NamedElementRename, this, value);
				}
				this.name = value;
				this.OnNameChanged();
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual object Tag
		{
			get
			{
				return this.tag;
			}
			set
			{
				this.tag = value;
			}
		}

		public NamedElement()
		{
		}

		protected NamedElement(string name)
		{
			this.name = name;
		}

		internal virtual void OnRemove()
		{
			if (this.Common != null)
			{
				this.Common.GaugeCore.Notify(MessageType.NamedElementRemove, this, null);
			}
		}

		internal virtual void OnAdded()
		{
			if (this.Common != null)
			{
				this.Common.GaugeCore.Notify(MessageType.NamedElementAdded, this, null);
			}
		}

		internal virtual void OnNameChanged()
		{
		}

		internal virtual void BeginInit()
		{
			this.initialized = false;
		}

		internal virtual void EndInit()
		{
			this.initialized = true;
		}

		internal virtual void Invalidate()
		{
			if (this.Common != null)
			{
				this.Common.GaugeCore.Invalidate();
			}
		}

		internal virtual void Refresh()
		{
			if (this.Common != null)
			{
				this.Common.GaugeCore.Refresh();
			}
		}

		internal virtual void ReconnectData(bool exact)
		{
		}

		internal virtual void Notify(MessageType msg, NamedElement element, object param)
		{
		}

		internal string GetNameAsParent()
		{
			return this.GetNameAsParent(this.Name);
		}

		internal string GetNameAsParent(string newName)
		{
			if (this.Collection != null)
			{
				return this.Collection.GetCollectionName() + "." + newName;
			}
			return newName;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed && disposing)
			{
				this.OnDispose();
			}
			this.disposed = true;
		}

		protected virtual void OnDispose()
		{
		}

		public virtual object Clone()
		{
			return this.CloneInternals(this.InitiateCopy());
		}

		internal virtual object InitiateCopy()
		{
			return base.MemberwiseClone();
		}

		internal virtual object CloneInternals(object copy)
		{
			return copy;
		}
	}
}
