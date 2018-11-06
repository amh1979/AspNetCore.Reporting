using System;
using System.ComponentModel;
using System.Drawing;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal abstract class NamedElement : IDisposable, ICloneable
	{
		private string name = string.Empty;

		internal CommonElements common;

		internal NamedCollection collection;

		internal bool initialized = true;

		private object tag;

		internal bool disposed;

		[SerializationVisibility(SerializationVisibility.Hidden)]
		[Browsable(false)]
		[Description("Indicates that map area is custom.")]
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

		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeNamedElement_Collection")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue("")]
		[Browsable(false)]
		internal virtual NamedCollection Collection
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

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeNamedElement_ParentElement")]
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

		[SRDescription("DescriptionAttributeNamedElement_Name")]
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
					this.Common.MapCore.Notify(MessageType.NamedElementRename, this, value);
				}
				this.name = value;
				this.OnNameChanged();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[Browsable(false)]
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
			: this(null)
		{
		}

		internal NamedElement(CommonElements common)
		{
			this.Common = common;
		}

		internal virtual void OnRemove()
		{
			if (this.Common != null)
			{
				this.Common.MapCore.Notify(MessageType.NamedElementRemove, this, null);
			}
		}

		internal virtual void OnAdded()
		{
			if (this.Common != null)
			{
				this.Common.MapCore.Notify(MessageType.NamedElementAdded, this, null);
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
				this.Common.MapCore.Invalidate();
			}
		}

		internal virtual void Invalidate(RectangleF rect)
		{
			if (this.Common != null)
			{
				this.Common.MapCore.Invalidate(rect);
			}
		}

		internal virtual void InvalidateViewport(bool invalidateGridSections)
		{
			if (this.Common != null)
			{
				this.Common.MapCore.InvalidateViewport(invalidateGridSections);
			}
		}

		internal virtual void InvalidateViewport()
		{
			if (this.Common != null)
			{
				this.Common.MapCore.InvalidateViewport(true);
			}
		}

		internal virtual void InvalidateDistanceScalePanel()
		{
			if (this.Common != null)
			{
				this.Common.MapCore.InvalidateDistanceScalePanel();
			}
		}

		internal virtual void InvalidateAndLayout()
		{
			if (this.Common != null)
			{
				this.Common.MapCore.InvalidateAndLayout();
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
