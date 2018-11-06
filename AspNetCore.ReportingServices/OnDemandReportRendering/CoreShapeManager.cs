using AspNetCore.Reporting.Map.WebForms;
using System.Drawing;
using System.Globalization;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal class CoreShapeManager : CoreSpatialElementManager
	{
		internal override AspNetCore.Reporting.Map.WebForms.FieldCollection FieldDefinitions
		{
			get
			{
				return base.m_coreMap.ShapeFields;
			}
		}

		protected override NamedCollection SpatialElements
		{
			get
			{
				return base.m_coreMap.Shapes;
			}
		}

		internal CoreShapeManager(MapControl mapControl, MapVectorLayer mapVectorLayer)
			: base(mapControl, mapVectorLayer)
		{
		}

		internal override void AddSpatialElement(ISpatialElement spatialElement)
		{
			((NamedElement)spatialElement).Name = base.m_coreMap.Shapes.Count.ToString(CultureInfo.InvariantCulture);
			base.m_coreMap.Shapes.Add((Shape)spatialElement);
		}

		internal override void RemoveSpatialElement(ISpatialElement spatialElement)
		{
			base.m_coreMap.Shapes.Remove((Shape)spatialElement);
		}

		internal override ISpatialElement CreateSpatialElement()
		{
			Shape shape = new Shape();
			shape.BorderColor = Color.Black;
			shape.Text = "";
			return shape;
		}
	}
}
