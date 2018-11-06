using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class MatrixMemberCollection
	{
		private Matrix m_owner;

		private MatrixHeading m_headingDef;

		private MatrixHeadingInstanceList m_headingInstances;

		private MatrixMember[] m_members;

		private MatrixMember m_firstMember;

		private MatrixMember m_parent;

		private int m_subTotalPosition = -1;

		private bool m_isParentSubTotal;

		private List<int> m_memberMapping;

		public MatrixMember this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					MatrixMember matrixMember = null;
					if (index == 0)
					{
						matrixMember = this.m_firstMember;
					}
					else if (this.m_members != null)
					{
						matrixMember = this.m_members[index - 1];
					}
					if (matrixMember == null)
					{
						bool isSubtotal = false;
						MatrixHeadingInstance matrixHeadingInstance = null;
						if (this.m_memberMapping != null && index < this.m_memberMapping.Count)
						{
							matrixHeadingInstance = this.m_headingInstances[this.m_memberMapping[index]];
							isSubtotal = matrixHeadingInstance.IsSubtotal;
						}
						else if (this.m_subTotalPosition >= 0 && index == this.m_subTotalPosition)
						{
							isSubtotal = true;
						}
						matrixMember = new MatrixMember(this.m_owner, this.m_parent, this.m_headingDef, matrixHeadingInstance, isSubtotal, this.m_isParentSubTotal, index);
						if (this.m_owner.RenderingContext.CacheState)
						{
							if (index == 0)
							{
								this.m_firstMember = matrixMember;
							}
							else
							{
								if (this.m_members == null)
								{
									this.m_members = new MatrixMember[this.Count - 1];
								}
								this.m_members[index - 1] = matrixMember;
							}
						}
					}
					return matrixMember;
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public int Count
		{
			get
			{
				if (this.m_owner.NoRows)
				{
					if (this.m_headingDef.Subtotal == null)
					{
						if (this.m_headingDef.Grouping == null)
						{
							return this.m_headingDef.ReportItems.Count;
						}
						return 1;
					}
					return 2;
				}
				return this.m_memberMapping.Count;
			}
		}

		internal MatrixHeading MatrixHeadingDef
		{
			get
			{
				return this.m_headingDef;
			}
		}

		internal MatrixMemberCollection(Matrix owner, MatrixMember parent, MatrixHeading headingDef, MatrixHeadingInstanceList headingInstances, List<int> memberMapping, bool isParentSubTotal)
		{
			this.m_owner = owner;
			this.m_parent = parent;
			this.m_headingInstances = headingInstances;
			this.m_headingDef = headingDef;
			this.m_memberMapping = memberMapping;
			this.m_isParentSubTotal = isParentSubTotal;
			if (owner.NoRows)
			{
				Subtotal subtotal = this.m_headingDef.Subtotal;
				if (subtotal != null)
				{
					if (subtotal.Position == Subtotal.PositionType.After)
					{
						this.m_subTotalPosition = 1;
					}
					else
					{
						this.m_subTotalPosition = 0;
					}
				}
			}
		}
	}
}
