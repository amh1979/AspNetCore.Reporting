using System;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class Gap
	{
		public float Inside;

		public float Center;

		public float Outside;

		private float baseInside;

		private float baseOutside;

		public float InsideGap
		{
			get
			{
				return this.Center - this.Inside;
			}
		}

		public float OutsideGap
		{
			get
			{
				return this.Center + this.Outside;
			}
		}

		public Gap(float center)
		{
			this.Center = center;
		}

		public void SetBase()
		{
			this.baseInside = this.Inside;
			this.baseOutside = this.Outside;
		}

		public void SetOffset(Placement placement, float length)
		{
			switch (placement)
			{
			case Placement.Inside:
				this.Inside += length;
				break;
			case Placement.Cross:
				this.Inside += (float)(length / 2.0);
				this.Outside += (float)(length / 2.0);
				break;
			case Placement.Outside:
				this.Outside += length;
				break;
			default:
				throw new InvalidOperationException(SR.invalid_placement_type);
			}
		}

		public void SetOffsetBase(Placement placement, float length)
		{
			switch (placement)
			{
			case Placement.Inside:
				this.Inside = Math.Max(this.Inside, this.baseInside + length);
				break;
			case Placement.Cross:
				this.Inside = Math.Max(this.Inside, (float)(length / 2.0));
				this.Outside = Math.Max(this.Outside, (float)(length / 2.0));
				break;
			case Placement.Outside:
				this.Outside = Math.Max(this.Outside, this.baseOutside + length);
				break;
			default:
				throw new InvalidOperationException(SR.invalid_placement_type);
			}
		}
	}
}
