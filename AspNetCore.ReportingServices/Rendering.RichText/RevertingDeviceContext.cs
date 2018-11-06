using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace AspNetCore.ReportingServices.Rendering.RichText
{
	internal class RevertingDeviceContext : IDisposable
	{
		private Win32DCSafeHandle m_hdc;

		private Matrix m_matrix;

		private Win32.XFORM m_oldXForm;

		private Win32.XFORM m_xForm;

		private int m_oldMode;

		private Graphics m_graphics;

		private GraphicsUnit m_pageUnits;

		private float m_pageScale;

		internal Win32DCSafeHandle Hdc
		{
			get
			{
				return this.m_hdc;
			}
		}

		internal Win32.XFORM XForm
		{
			get
			{
				return this.m_xForm;
			}
		}

		internal RevertingDeviceContext(Graphics g, float dpi)
		{
			this.m_graphics = g;
			this.m_matrix = this.m_graphics.Transform;
			this.m_pageUnits = this.m_graphics.PageUnit;
			this.m_pageScale = this.m_graphics.PageScale;
			this.SetupGraphics(dpi);
		}

		public void Dispose()
		{
			if (this.m_matrix != null)
			{
				if (!Win32.SetWorldTransform(this.m_hdc, ref this.m_oldXForm))
				{
					Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
				}
				if (Win32.SetGraphicsMode(this.m_hdc, this.m_oldMode) == 0)
				{
					Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
				}
				this.m_matrix.Dispose();
				this.m_matrix = null;
			}
			if (!this.m_hdc.IsInvalid)
			{
				this.m_graphics.ReleaseHdc();
			}
			this.m_graphics.PageScale = this.m_pageScale;
			this.m_graphics.PageUnit = this.m_pageUnits;
			GC.SuppressFinalize(this);
		}

		private void SetupGraphics(float dpi)
		{
			this.m_graphics.PageUnit = GraphicsUnit.Pixel;
			this.m_graphics.PageScale = 1f;
			this.m_hdc = new Win32DCSafeHandle(this.m_graphics.GetHdc(), false);
			this.m_oldXForm = default(Win32.XFORM);
			if (this.m_matrix != null)
			{
				this.m_xForm = new Win32.XFORM(this.m_matrix, this.m_pageUnits, dpi);
				this.m_oldMode = Win32.SetGraphicsMode(this.m_hdc, 2);
				if (!Win32.GetWorldTransform(this.m_hdc, ref this.m_oldXForm))
				{
					Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
				}
				if (!Win32.SetWorldTransform(this.m_hdc, ref this.m_xForm))
				{
					Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
				}
			}
			else
			{
				this.m_xForm = Win32.XFORM.Identity;
			}
		}
	}
}
