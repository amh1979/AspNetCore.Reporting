using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ParameterInstance : BaseInstance, IPersistable
	{
		private bool m_omit;

		private object m_value;

		[NonSerialized]
		private bool m_isOldSnapshot;

		[NonSerialized]
		private bool m_valueReady;

		[NonSerialized]
		private bool m_omitReady;

		[NonSerialized]
		private bool m_omitAssigned;

		[NonSerialized]
		private Parameter m_parameterDef;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ParameterInstance.GetDeclaration();

		public object Value
		{
			get
			{
				if (!this.m_isOldSnapshot && !this.m_valueReady)
				{
					this.m_valueReady = true;
					if (!this.m_parameterDef.Value.IsExpression)
					{
						this.m_value = this.m_parameterDef.Value.Value;
					}
					else if (this.m_parameterDef.ActionDef.Owner.ReportElementOwner == null || this.m_parameterDef.ActionDef.Owner.ReportElementOwner.CriOwner == null)
					{
						ActionInfo owner = this.m_parameterDef.ActionDef.Owner;
						this.m_value = this.m_parameterDef.ActionDef.ActionItemDef.EvaluateDrillthroughParamValue(this.ReportScopeInstance, owner.RenderingContext.OdpContext, owner.InstancePath, owner.ROMActionOwner.FieldsUsedInValueExpression, this.m_parameterDef.ParameterDef, owner.ObjectType, owner.ObjectName);
					}
				}
				return this.m_value;
			}
			set
			{
				ReportElement reportElementOwner = this.m_parameterDef.ActionDef.Owner.ReportElementOwner;
				Global.Tracer.Assert(this.m_parameterDef.Value != null, "(m_parameterDef.Value != null)");
				if (!this.m_parameterDef.ActionDef.Owner.IsChartConstruction && (reportElementOwner.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (reportElementOwner.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && !this.m_parameterDef.Value.IsExpression)))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				if (value != null)
				{
					if (reportElementOwner.CriGenerationPhase == ReportElement.CriGenerationPhases.Definition)
					{
						if (!(value is string))
						{
							throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWritebackStringExpected);
						}
					}
					else
					{
						bool flag;
						if (value is object[])
						{
							object[] array = (object[])value;
							flag = true;
							object[] array2 = array;
							foreach (object o in array2)
							{
								if (!ReportRuntime.IsVariant(o))
								{
									flag = false;
									break;
								}
							}
						}
						else
						{
							flag = ReportRuntime.IsVariant(value);
						}
						if (!flag)
						{
							throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
						}
					}
				}
				this.m_valueReady = true;
				this.m_value = value;
			}
		}

		public bool Omit
		{
			get
			{
				if (!this.m_isOldSnapshot && !this.m_omitReady)
				{
					this.m_omitReady = true;
					if (!this.m_parameterDef.Omit.IsExpression)
					{
						this.m_omit = this.m_parameterDef.Omit.Value;
					}
					else if (this.m_parameterDef.ActionDef.Owner.ReportElementOwner == null || this.m_parameterDef.ActionDef.Owner.ReportElementOwner.CriOwner == null)
					{
						ActionInfo owner = this.m_parameterDef.ActionDef.Owner;
						this.m_omit = this.m_parameterDef.ActionDef.ActionItemDef.EvaluateDrillthroughParamOmit(this.ReportScopeInstance, owner.RenderingContext.OdpContext, owner.InstancePath, this.m_parameterDef.ParameterDef, owner.ObjectType, owner.ObjectName);
					}
				}
				return this.m_omit;
			}
			set
			{
				ReportElement reportElementOwner = this.m_parameterDef.ActionDef.Owner.ReportElementOwner;
				Global.Tracer.Assert(this.m_parameterDef.Omit != null, "(m_parameterDef.Omit != null)");
				if (!this.m_parameterDef.ActionDef.Owner.IsChartConstruction && (reportElementOwner.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (reportElementOwner.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && !this.m_parameterDef.Omit.IsExpression)))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				this.m_omitReady = true;
				this.m_omitAssigned = true;
				this.m_omit = value;
			}
		}

		internal bool IsOmitAssined
		{
			get
			{
				return this.m_omitAssigned;
			}
		}

		internal ParameterInstance(ActionItemInstance actionInstance, int index)
			: base(null)
		{
			this.m_isOldSnapshot = true;
			this.SetMembers(actionInstance, index);
		}

		internal ParameterInstance(Parameter parameterDef)
			: base(parameterDef.ActionDef.Owner.ReportScope)
		{
			this.m_isOldSnapshot = false;
			this.m_parameterDef = parameterDef;
		}

		private void SetMembers(ActionItemInstance actionInstance, int index)
		{
			this.m_value = null;
			this.m_omit = false;
			if (actionInstance != null)
			{
				if (actionInstance.DrillthroughParametersValues != null)
				{
					this.m_value = actionInstance.DrillthroughParametersValues[index];
				}
				if (actionInstance.DrillthroughParametersOmits != null)
				{
					this.m_omit = actionInstance.DrillthroughParametersOmits[index];
				}
			}
		}

		internal void Update(ActionItemInstance actionInstance, int index)
		{
			this.SetMembers(actionInstance, index);
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
		}

		protected override void ResetInstanceCache()
		{
			this.m_omitAssigned = false;
			this.m_omitReady = false;
			this.m_valueReady = false;
		}

		void IPersistable.Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(ParameterInstance.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Value:
				{
					object obj2 = null;
					if (this.m_parameterDef.Value.IsExpression)
					{
						obj2 = this.Value;
					}
					writer.Write(obj2);
					break;
				}
				case MemberName.Omit:
				{
					object obj = null;
					if (this.m_parameterDef.Omit.IsExpression && this.IsOmitAssined)
					{
						obj = this.Omit;
					}
					writer.Write(obj);
					break;
				}
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		void IPersistable.Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(ParameterInstance.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Value:
				{
					object obj2 = reader.ReadVariant();
					if (this.m_parameterDef.Value.IsExpression)
					{
						this.m_valueReady = true;
						this.m_value = obj2;
					}
					else
					{
						Global.Tracer.Assert(obj2 == null, "(value == null)");
					}
					break;
				}
				case MemberName.Omit:
				{
					object obj = reader.ReadVariant();
					if (this.m_parameterDef.Omit.IsExpression && obj != null)
					{
						this.m_omitReady = true;
						this.m_omit = (bool)obj;
					}
					else
					{
						Global.Tracer.Assert(obj == null, "(omit == null)");
					}
					break;
				}
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterInstance;
		}

		private static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Value, Token.Object));
			list.Add(new MemberInfo(MemberName.Omit, Token.Object));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterInstance, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}
	}
}
