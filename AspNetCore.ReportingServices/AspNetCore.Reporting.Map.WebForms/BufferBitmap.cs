using System;
using System.Drawing;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class BufferBitmap : IDisposable
	{
		private Bitmap bitmap;

		private Graphics graphics;

		private Size size;

		private bool disposed;

		public Size Size
		{
			get
			{
				return this.size;
			}
			set
			{
				if (this.size != value)
				{
					this.size = value;
					this.Invalidate();
				}
			}
		}

		public Bitmap Bitmap
		{
			get
			{
				if (this.bitmap == null)
				{
					this.Invalidate();
				}
				return this.bitmap;
			}
		}

		public Graphics Graphics
		{
			get
			{
				if (this.graphics == null)
				{
					this.Invalidate();
				}
				return this.graphics;
			}
		}

		public BufferBitmap()
		{
			this.Size = new Size(5, 5);
		}

		private void DisposeObjects()
		{
			if (this.graphics != null)
			{
				this.graphics.Dispose();
				this.graphics = null;
			}
			if (this.bitmap != null)
			{
				this.bitmap.Dispose();
				this.bitmap = null;
			}
		}

		public void Invalidate()
		{
			this.DisposeObjects();
			this.bitmap = new Bitmap(this.size.Width, this.size.Height);
			this.graphics = Graphics.FromImage(this.bitmap);
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
				this.DisposeObjects();
			}
			this.disposed = true;
		}

		~BufferBitmap()
		{
			this.Dispose(false);
		}
	}
}
