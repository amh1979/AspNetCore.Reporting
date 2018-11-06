using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal sealed class OpenXmlTablePropertiesModel : IHaveABorderAndShading
	{
		private string _bgColor;

		private OpenXmlBorderPropertiesModel _borderTop;

		private OpenXmlBorderPropertiesModel _borderBottom;

		private OpenXmlBorderPropertiesModel _borderLeft;

		private OpenXmlBorderPropertiesModel _borderRight;

		private bool _autofit;

		public string BackgroundColor
		{
			get
			{
				return this._bgColor;
			}
			set
			{
				this._bgColor = value;
			}
		}

		public OpenXmlBorderPropertiesModel BorderTop
		{
			get
			{
				if (this._borderTop == null)
				{
					this._borderTop = new OpenXmlBorderPropertiesModel();
				}
				return this._borderTop;
			}
		}

		public OpenXmlBorderPropertiesModel BorderBottom
		{
			get
			{
				if (this._borderBottom == null)
				{
					this._borderBottom = new OpenXmlBorderPropertiesModel();
				}
				return this._borderBottom;
			}
		}

		public OpenXmlBorderPropertiesModel BorderLeft
		{
			get
			{
				if (this._borderLeft == null)
				{
					this._borderLeft = new OpenXmlBorderPropertiesModel();
				}
				return this._borderLeft;
			}
		}

		public OpenXmlBorderPropertiesModel BorderRight
		{
			get
			{
				if (this._borderRight == null)
				{
					this._borderRight = new OpenXmlBorderPropertiesModel();
				}
				return this._borderRight;
			}
		}

		static OpenXmlTablePropertiesModel()
		{
		}

		public OpenXmlTablePropertiesModel(AutoFit autofit)
		{
			this._autofit = (autofit == AutoFit.True || autofit == AutoFit.Default);
		}

		public void Write(TextWriter output)
		{
			output.Write("<w:tblPr>");
			if (this._borderTop != null || this._borderBottom != null || this._borderLeft != null || this._borderRight != null)
			{
				output.Write("<w:tblBorders>");
				if (this._borderTop != null)
				{
					this._borderTop.Write(output, "top");
				}
				if (this._borderLeft != null)
				{
					this._borderLeft.Write(output, "left");
				}
				if (this._borderBottom != null)
				{
					this._borderBottom.Write(output, "bottom");
				}
				if (this._borderRight != null)
				{
					this._borderRight.Write(output, "right");
				}
				output.Write("</w:tblBorders>");
			}
			if (this._bgColor != null)
			{
				output.Write("<w:shd w:val=\"clear\" w:fill=\"" + this._bgColor + "\"/>");
			}
			if (!this._autofit)
			{
				output.Write("<w:tblLayout w:type=\"fixed\"/>");
			}
			output.Write("<w:tblCellMar><w:top w:w=\"0\" w:type=\"dxa\"/><w:left w:w=\"0\" w:type=\"dxa\"/><w:bottom w:w=\"0\" w:type=\"dxa\"/><w:right w:w=\"0\" w:type=\"dxa\"/></w:tblCellMar></w:tblPr>");
		}
	}
}
