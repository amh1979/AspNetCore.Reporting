using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.HPBProcessing
{
	internal class ItemSizes : IStorable, IPersistable
	{
		private double m_deltaX;

		private double m_deltaY;

		protected double m_left;

		protected double m_top;

		protected double m_width;

		protected double m_height;

		private static Declaration m_declaration = ItemSizes.GetDeclaration();

		internal double DeltaX
		{
			get
			{
				return this.m_deltaX;
			}
			set
			{
				this.m_deltaX = value;
			}
		}

		internal double DeltaY
		{
			get
			{
				return this.m_deltaY;
			}
			set
			{
				this.m_deltaY = value;
			}
		}

		internal virtual double Left
		{
			get
			{
				return this.m_left;
			}
			set
			{
				this.m_left = value;
			}
		}

		internal virtual double Top
		{
			get
			{
				return this.m_top;
			}
			set
			{
				this.m_top = value;
			}
		}

		internal double Bottom
		{
			get
			{
				return this.m_top + this.m_height;
			}
		}

		internal double Right
		{
			get
			{
				return this.m_left + this.m_width;
			}
		}

		internal virtual double Width
		{
			get
			{
				return this.m_width;
			}
			set
			{
				this.m_width = value;
			}
		}

		internal virtual double Height
		{
			get
			{
				return this.m_height;
			}
			set
			{
				this.m_height = value;
			}
		}

		public int Size
		{
			get
			{
				return 48;
			}
		}

		internal ItemSizes()
		{
		}

		internal ItemSizes(ReportItem reportItem)
		{
			this.m_top = reportItem.Top.ToMillimeters();
			this.m_left = reportItem.Left.ToMillimeters();
			this.m_width = reportItem.Width.ToMillimeters();
			this.m_height = reportItem.Height.ToMillimeters();
		}

		internal ItemSizes(double left, double top, double width, double height)
		{
			this.m_top = top;
			this.m_left = left;
			this.m_width = width;
			this.m_height = height;
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(ItemSizes.m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.DeltaX:
					writer.Write(this.m_deltaX);
					break;
				case MemberName.DeltaY:
					writer.Write(this.m_deltaY);
					break;
				case MemberName.Left:
					writer.Write(this.m_left);
					break;
				case MemberName.Top:
					writer.Write(this.m_top);
					break;
				case MemberName.Width:
					writer.Write(this.m_width);
					break;
				case MemberName.Height:
					writer.Write(this.m_height);
					break;
				default:
					RSTrace.RenderingTracer.Assert(false, string.Empty);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(ItemSizes.m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.DeltaX:
					this.m_deltaX = reader.ReadDouble();
					break;
				case MemberName.DeltaY:
					this.m_deltaY = reader.ReadDouble();
					break;
				case MemberName.Left:
					this.m_left = reader.ReadDouble();
					break;
				case MemberName.Top:
					this.m_top = reader.ReadDouble();
					break;
				case MemberName.Width:
					this.m_width = reader.ReadDouble();
					break;
				case MemberName.Height:
					this.m_height = reader.ReadDouble();
					break;
				default:
					RSTrace.RenderingTracer.Assert(false, string.Empty);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public ObjectType GetObjectType()
		{
			return ObjectType.ItemSizes;
		}

		internal static Declaration GetDeclaration()
		{
			if (ItemSizes.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.DeltaX, Token.Double));
				list.Add(new MemberInfo(MemberName.DeltaY, Token.Double));
				list.Add(new MemberInfo(MemberName.Left, Token.Double));
				list.Add(new MemberInfo(MemberName.Top, Token.Double));
				list.Add(new MemberInfo(MemberName.Width, Token.Double));
				list.Add(new MemberInfo(MemberName.Height, Token.Double));
				return new Declaration(ObjectType.ItemSizes, ObjectType.None, list);
			}
			return ItemSizes.m_declaration;
		}

		internal virtual void AdjustHeightTo(double amount)
		{
			this.m_deltaY += amount - this.m_height;
			this.m_height = amount;
		}

		internal virtual void AdjustWidthTo(double amount)
		{
			this.m_deltaX += amount - this.m_width;
			this.m_width = amount;
		}

		internal virtual void MoveVertical(double delta)
		{
			this.m_top += delta;
			this.m_deltaY += delta;
		}

		internal virtual void MoveHorizontal(double delta)
		{
			this.m_left += delta;
			this.m_deltaX += delta;
		}
	}
}
