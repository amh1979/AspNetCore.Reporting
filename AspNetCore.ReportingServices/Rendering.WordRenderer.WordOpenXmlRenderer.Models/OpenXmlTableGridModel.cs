using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal sealed class OpenXmlTableGridModel : BaseInterleaver
	{
		private List<int> _columns = new List<int>();

		private static readonly Declaration _declaration;

		public override int Size
		{
			get
			{
				return base.Size + ItemSizes.SizeOf(this._columns);
			}
		}

		public OpenXmlTableGridModel(int index, long location)
			: base(index, location)
		{
		}

		public OpenXmlTableGridModel()
		{
		}

		static OpenXmlTableGridModel()
		{
			OpenXmlTableGridModel._declaration = new Declaration(ObjectType.WordOpenXmlTableGrid, ObjectType.WordOpenXmlBaseInterleaver, new List<MemberInfo>
			{
				new MemberInfo(MemberName.Columns, ObjectType.PrimitiveList, Token.Int32)
			});
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(OpenXmlTableGridModel.GetDeclaration());
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.Columns)
				{
					writer.WriteListOfPrimitives(this._columns);
				}
				else
				{
					WordOpenXmlUtils.FailSerializable();
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(OpenXmlTableGridModel.GetDeclaration());
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.Columns)
				{
					this._columns = reader.ReadListOfPrimitives<int>();
				}
				else
				{
					WordOpenXmlUtils.FailSerializable();
				}
			}
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.WordOpenXmlTableGrid;
		}

		internal new static Declaration GetDeclaration()
		{
			return OpenXmlTableGridModel._declaration;
		}

		public void AddRow(int[] columns)
		{
			List<int> list = new List<int>();
			int i = 0;
			int j = 0;
			while (i < this._columns.Count && j < columns.Length)
			{
				if (columns[j] == 0)
				{
					j++;
				}
				else if (this._columns[i] < columns[j])
				{
					list.Add(this._columns[i]);
					columns[j] -= this._columns[i];
					i++;
				}
				else if (this._columns[i] > columns[j])
				{
					list.Add(columns[j]);
					List<int> columns2;
					int index;
					(columns2 = this._columns)[index = i] = columns2[index] - columns[j];
					j++;
				}
				else
				{
					list.Add(this._columns[i]);
					i++;
					j++;
				}
			}
			for (; i < this._columns.Count; i++)
			{
				list.Add(this._columns[i]);
			}
			for (; j < columns.Length; j++)
			{
				if (columns[j] > 0)
				{
					list.Add(columns[j]);
				}
			}
			this._columns = list;
		}

		public override void Write(TextWriter output)
		{
			output.Write("<w:tblGrid>");
			foreach (int column in this._columns)
			{
				output.Write("<w:gridCol w:w=\"");
				output.Write(WordOpenXmlUtils.TwipsToString(column, 0, 31680));
				output.Write("\"/>");
			}
			output.Write("</w:tblGrid>");
		}
	}
}
