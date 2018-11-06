using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System.ComponentModel;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeAnnotationPathPoint_AnnotationPathPoint")]
	internal class AnnotationPathPoint
	{
		private float x;

		private float y;

		private byte pointType = 1;

		[SRCategory("CategoryAttributePosition")]
		[DefaultValue(0f)]
		[Browsable(true)]
		[SRDescription("DescriptionAttributeAnnotationPathPoint_X")]
		public float X
		{
			get
			{
				return this.x;
			}
			set
			{
				this.x = value;
			}
		}

		[DefaultValue(0f)]
		[SRCategory("CategoryAttributePosition")]
		[Browsable(true)]
		[SRDescription("DescriptionAttributeAnnotationPathPoint_Y")]
		public float Y
		{
			get
			{
				return this.y;
			}
			set
			{
				this.y = value;
			}
		}

		[SRDescription("DescriptionAttributeAnnotationPathPoint_Name")]
		[SRCategory("CategoryAttributePosition")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(typeof(byte), "1")]
		public byte PointType
		{
			get
			{
				return this.pointType;
			}
			set
			{
				this.pointType = value;
			}
		}

		[DefaultValue("PathPoint")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRDescription("DescriptionAttributeAnnotationPathPoint_Name")]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRCategory("CategoryAttributeMisc")]
		public string Name
		{
			get
			{
				return "PathPoint";
			}
		}

		public AnnotationPathPoint()
		{
		}

		public AnnotationPathPoint(float x, float y)
		{
			this.x = x;
			this.y = y;
		}

		public AnnotationPathPoint(float x, float y, byte type)
		{
			this.x = x;
			this.y = y;
			this.pointType = type;
		}
	}
}
