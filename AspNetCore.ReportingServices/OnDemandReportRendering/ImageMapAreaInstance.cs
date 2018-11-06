using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ImageMapAreaInstance : IPersistable
	{
		private ImageMapArea.ImageMapAreaShape m_shape;

		private float[] m_coordinates;

		private string m_toolTip;

		private static readonly Declaration m_Declaration = ImageMapAreaInstance.GetDeclaration();

		public ImageMapArea.ImageMapAreaShape Shape
		{
			get
			{
				return this.m_shape;
			}
		}

		public float[] Coordinates
		{
			get
			{
				return this.m_coordinates;
			}
		}

		public string ToolTip
		{
			get
			{
				return this.m_toolTip;
			}
		}

		internal ImageMapAreaInstance(ImageMapArea.ImageMapAreaShape shape, float[] coordinates)
			: this(shape, coordinates, null)
		{
		}

		internal ImageMapAreaInstance(ImageMapArea.ImageMapAreaShape shape, float[] coordinates, string toolTip)
		{
			this.m_shape = shape;
			this.m_coordinates = coordinates;
			this.m_toolTip = toolTip;
		}

		internal ImageMapAreaInstance()
		{
		}

		internal ImageMapAreaInstance(AspNetCore.ReportingServices.ReportRendering.ImageMapArea renderImageMapArea)
		{
			this.m_shape = (ImageMapArea.ImageMapAreaShape)renderImageMapArea.Shape;
			this.m_coordinates = renderImageMapArea.Coordinates;
		}

		void IPersistable.Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(ImageMapAreaInstance.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Shape:
					writer.WriteEnum((int)this.m_shape);
					break;
				case MemberName.Coordinates:
					writer.Write(this.m_coordinates);
					break;
				case MemberName.ToolTip:
					writer.Write(this.m_toolTip);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		void IPersistable.Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(ImageMapAreaInstance.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Shape:
					this.m_shape = (ImageMapArea.ImageMapAreaShape)reader.ReadEnum();
					break;
				case MemberName.Coordinates:
					this.m_coordinates = reader.ReadSingleArray();
					break;
				case MemberName.ToolTip:
					this.m_toolTip = reader.ReadString();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		void IPersistable.ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false);
		}

		AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IPersistable.GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ImageMapAreaInstance;
		}

		private static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Shape, Token.Enum));
			list.Add(new MemberInfo(MemberName.Coordinates, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Single));
			list.Add(new MemberInfo(MemberName.ToolTip, Token.String));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ImageMapAreaInstance, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}
	}
}
