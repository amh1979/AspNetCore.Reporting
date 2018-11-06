using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal sealed class OpenXmlTableRowPropertiesModel : BaseInterleaver, IHaveABorderAndShading
	{
		private float _height;

		private float _rowIndent;

		private bool _exactRowHeight;

		private bool _ignoreRowHeight;

		private float _maxPaddingBottom;

		private float _maxPaddingTop;

		private OpenXmlBorderPropertiesModel _borderTop;

		private OpenXmlBorderPropertiesModel _borderBottom;

		private OpenXmlBorderPropertiesModel _borderLeft;

		private OpenXmlBorderPropertiesModel _borderRight;

		private string _bgColor;

		[NonSerialized]
		private static Declaration _declaration;

		public float Height
		{
			get
			{
				return this._height;
			}
			set
			{
				this._height = value;
			}
		}

		public float RowIndent
		{
			set
			{
				this._rowIndent = value;
			}
		}

		public bool ExactRowHeight
		{
			set
			{
				this._exactRowHeight = value;
			}
		}

		public bool IgnoreRowHeight
		{
			set
			{
				this._ignoreRowHeight = value;
			}
		}

		public string BackgroundColor
		{
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

		public override int Size
		{
			get
			{
				return base.Size + 8 + 2 + 8 + ItemSizes.SizeOf(this._borderTop) + ItemSizes.SizeOf(this._borderBottom) + ItemSizes.SizeOf(this._borderLeft) + ItemSizes.SizeOf(this._borderRight) + ItemSizes.SizeOf(this._bgColor);
			}
		}

		public OpenXmlTableRowPropertiesModel(int index, long location)
			: base(index, location)
		{
		}

		static OpenXmlTableRowPropertiesModel()
		{
			OpenXmlTableRowPropertiesModel._declaration = new Declaration(ObjectType.WordOpenXmlTableRowProperties, ObjectType.WordOpenXmlBaseInterleaver, new List<MemberInfo>
			{
				new MemberInfo(MemberName.RowHeight, Token.Single),
				new MemberInfo(MemberName.LeftIndent, Token.Single),
				new MemberInfo(MemberName.ExactRowHeight, Token.Boolean),
				new MemberInfo(MemberName.IgnoreRowHeight, Token.Boolean),
				new MemberInfo(MemberName.BottomPadding, Token.Single),
				new MemberInfo(MemberName.TopPadding, Token.Single),
				new MemberInfo(MemberName.TopBorder, ObjectType.WordOpenXmlBorderProperties),
				new MemberInfo(MemberName.BottomBorder, ObjectType.WordOpenXmlBorderProperties),
				new MemberInfo(MemberName.LeftBorder, ObjectType.WordOpenXmlBorderProperties),
				new MemberInfo(MemberName.RightBorder, ObjectType.WordOpenXmlBorderProperties),
				new MemberInfo(MemberName.Color, Token.String)
			});
		}

		public OpenXmlTableRowPropertiesModel()
		{
		}

		public void SetCellPaddingTop(double padding)
		{
			this._maxPaddingTop = Math.Max(this._maxPaddingTop, (float)padding);
		}

		public void SetCellPaddingBottom(double padding)
		{
			this._maxPaddingBottom = Math.Max(this._maxPaddingBottom, (float)padding);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(OpenXmlTableRowPropertiesModel.GetDeclaration());
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.RowHeight:
					writer.Write(this._height);
					break;
				case MemberName.LeftIndent:
					writer.Write(this._rowIndent);
					break;
				case MemberName.ExactRowHeight:
					writer.Write(this._exactRowHeight);
					break;
				case MemberName.IgnoreRowHeight:
					writer.Write(this._ignoreRowHeight);
					break;
				case MemberName.TopPadding:
					writer.Write(this._maxPaddingTop);
					break;
				case MemberName.BottomPadding:
					writer.Write(this._maxPaddingBottom);
					break;
				case MemberName.TopBorder:
					writer.Write(this._borderTop);
					break;
				case MemberName.BottomBorder:
					writer.Write(this._borderBottom);
					break;
				case MemberName.LeftBorder:
					writer.Write(this._borderLeft);
					break;
				case MemberName.RightBorder:
					writer.Write(this._borderRight);
					break;
				case MemberName.Color:
					writer.Write(this._bgColor);
					break;
				default:
					WordOpenXmlUtils.FailSerializable();
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(OpenXmlTableRowPropertiesModel.GetDeclaration());
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.RowHeight:
					this._height = reader.ReadSingle();
					break;
				case MemberName.LeftIndent:
					this._rowIndent = reader.ReadSingle();
					break;
				case MemberName.ExactRowHeight:
					this._exactRowHeight = reader.ReadBoolean();
					break;
				case MemberName.IgnoreRowHeight:
					this._ignoreRowHeight = reader.ReadBoolean();
					break;
				case MemberName.TopPadding:
					this._maxPaddingTop = reader.ReadSingle();
					break;
				case MemberName.BottomPadding:
					this._maxPaddingBottom = reader.ReadSingle();
					break;
				case MemberName.TopBorder:
					this._borderTop = (OpenXmlBorderPropertiesModel)reader.ReadRIFObject();
					break;
				case MemberName.BottomBorder:
					this._borderBottom = (OpenXmlBorderPropertiesModel)reader.ReadRIFObject();
					break;
				case MemberName.LeftBorder:
					this._borderLeft = (OpenXmlBorderPropertiesModel)reader.ReadRIFObject();
					break;
				case MemberName.RightBorder:
					this._borderRight = (OpenXmlBorderPropertiesModel)reader.ReadRIFObject();
					break;
				case MemberName.Color:
					this._bgColor = reader.ReadString();
					break;
				default:
					WordOpenXmlUtils.FailSerializable();
					break;
				}
			}
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.WordOpenXmlTableRowProperties;
		}

		internal new static Declaration GetDeclaration()
		{
			return OpenXmlTableRowPropertiesModel._declaration;
		}

		public override void Write(TextWriter output)
		{
			output.Write("<w:trPr>");
			if (this._rowIndent > 0.0)
			{
				output.Write("<w:wBefore w:w=\"");
				output.Write(WordOpenXmlUtils.ToTwips(this._rowIndent, 0f, 31680f));
				output.Write("\" w:type=\"dxa\"/>");
			}
			long num = (long)WordOpenXmlUtils.ToTwips(this._height) - (long)WordOpenXmlUtils.PointsToTwips((double)this._maxPaddingTop, 0.0, 31680.0) - WordOpenXmlUtils.PointsToTwips((double)this._maxPaddingBottom, 0.0, 31680.0);
			if (!this._ignoreRowHeight && num > 0)
			{
				output.Write("<w:trHeight w:val=\"");
				output.Write(WordOpenXmlUtils.TwipsToString(num, 0, 31680));
				output.Write(this._exactRowHeight ? "\" w:hRule=\"exact\"/>" : "\" w:hRule=\"atLeast\"/>");
			}
			output.Write("</w:trPr>");
			bool flag = this._borderTop != null || this._borderBottom != null || this._borderLeft != null || this._borderRight != null;
			if (flag || this._bgColor != null)
			{
				output.Write("<w:tblPrEx>");
			}
			if (flag)
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
				output.Write("<w:shd w:val=\"clear\" w:fill=\"");
				output.Write(this._bgColor);
				output.Write("\"/>");
			}
			if (!flag && this._bgColor == null)
			{
				return;
			}
			output.Write("</w:tblPrEx>");
		}
	}
}
