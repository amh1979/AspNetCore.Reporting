using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class Visibility : IPersistable
	{
		private ExpressionInfo m_hidden;

		private string m_toggle;

		private bool m_recursiveReceiver;

		private TablixMember m_recursiveMember;

		private TextBox m_toggleSender;

		[NonSerialized]
		private bool m_isClone;

		[NonSerialized]
		private static readonly Declaration m_Declaration = Visibility.GetDeclaration();

		internal ExpressionInfo Hidden
		{
			get
			{
				return this.m_hidden;
			}
			set
			{
				this.m_hidden = value;
			}
		}

		internal string Toggle
		{
			get
			{
				return this.m_toggle;
			}
			set
			{
				this.m_toggle = value;
			}
		}

		internal TextBox ToggleSender
		{
			get
			{
				return this.m_toggleSender;
			}
			set
			{
				this.m_toggleSender = value;
			}
		}

		internal bool RecursiveReceiver
		{
			get
			{
				return this.m_recursiveReceiver;
			}
			set
			{
				this.m_recursiveReceiver = value;
			}
		}

		internal TablixMember RecursiveMember
		{
			get
			{
				return this.m_recursiveMember;
			}
			set
			{
				this.m_recursiveMember = value;
			}
		}

		internal bool IsToggleReceiver
		{
			get
			{
				if (this.m_toggle != null)
				{
					return this.m_toggle.Length > 0;
				}
				return false;
			}
		}

		internal bool IsConditional
		{
			get
			{
				return this.m_hidden != null;
			}
		}

		internal bool IsClone
		{
			get
			{
				return this.m_isClone;
			}
		}

		internal void Initialize(InitializationContext context)
		{
			this.Initialize(context, true);
		}

		internal void Initialize(InitializationContext context, bool registerVisibilityToggle)
		{
			if (this.m_hidden != null)
			{
				this.m_hidden.Initialize("Hidden", context);
				context.ExprHostBuilder.GenericVisibilityHidden(this.m_hidden);
			}
			if (registerVisibilityToggle)
			{
				this.RegisterVisibilityToggle(context);
			}
		}

		internal VisibilityToggleInfo RegisterVisibilityToggle(InitializationContext context)
		{
			return context.RegisterVisibilityToggle(this);
		}

		internal static SharedHiddenState GetSharedHidden(Visibility visibility)
		{
			if (visibility == null)
			{
				return SharedHiddenState.Never;
			}
			if (visibility.Toggle == null)
			{
				if (visibility.Hidden == null)
				{
					return SharedHiddenState.Never;
				}
				if (ExpressionInfo.Types.Constant == visibility.Hidden.Type)
				{
					if (visibility.Hidden.BoolValue)
					{
						return SharedHiddenState.Always;
					}
					return SharedHiddenState.Never;
				}
			}
			return SharedHiddenState.Sometimes;
		}

		internal static bool HasToggle(Visibility visibility)
		{
			if (visibility == null)
			{
				return false;
			}
			return visibility.IsToggleReceiver;
		}

		internal object PublishClone(AutomaticSubtotalContext context, bool isSubtotalMember)
		{
			Visibility visibility = null;
			if (isSubtotalMember)
			{
				visibility = new Visibility();
				visibility.m_hidden = ExpressionInfo.CreateConstExpression(true);
			}
			else
			{
				visibility = (Visibility)base.MemberwiseClone();
				if (this.m_hidden != null)
				{
					visibility.m_hidden = (ExpressionInfo)this.m_hidden.PublishClone(context);
				}
				if (this.m_toggle != null)
				{
					context.AddVisibilityWithToggleToUpdate(visibility);
					visibility.m_toggle = (string)this.m_toggle.Clone();
				}
			}
			visibility.m_isClone = true;
			return visibility;
		}

		internal void UpdateToggleItemReference(AutomaticSubtotalContext context)
		{
			if (this.m_toggle != null)
			{
				this.m_toggle = context.GetNewReportItemName(this.m_toggle);
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Hidden, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Toggle, Token.String));
			list.Add(new MemberInfo(MemberName.RecursiveReceiver, Token.Boolean));
			list.Add(new MemberInfo(MemberName.ToggleSender, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TextBox, Token.Reference));
			list.Add(new MemberInfo(MemberName.RecursiveMember, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixMember, Token.Reference));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Visibility, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(Visibility.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Hidden:
					writer.Write(this.m_hidden);
					break;
				case MemberName.Toggle:
					writer.Write(this.m_toggle);
					break;
				case MemberName.RecursiveReceiver:
					writer.Write(this.m_recursiveReceiver);
					break;
				case MemberName.ToggleSender:
					writer.WriteReference(this.m_toggleSender);
					break;
				case MemberName.RecursiveMember:
					writer.WriteReference(this.m_recursiveMember);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(Visibility.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Hidden:
					this.m_hidden = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Toggle:
					this.m_toggle = reader.ReadString();
					break;
				case MemberName.RecursiveReceiver:
					this.m_recursiveReceiver = reader.ReadBoolean();
					break;
				case MemberName.ToggleSender:
					this.m_toggleSender = reader.ReadReference<TextBox>(this);
					break;
				case MemberName.RecursiveMember:
					this.m_recursiveMember = reader.ReadReference<TablixMember>(this);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			List<MemberReference> list = default(List<MemberReference>);
			if (memberReferencesCollection.TryGetValue(Visibility.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item in list)
				{
					switch (item.MemberName)
					{
					case MemberName.ToggleSender:
					{
						IReferenceable referenceable2 = default(IReferenceable);
						if (referenceableItems.TryGetValue(item.RefID, out referenceable2))
						{
							this.m_toggleSender = (referenceable2 as TextBox);
						}
						break;
					}
					case MemberName.RecursiveMember:
					{
						IReferenceable referenceable = default(IReferenceable);
						if (referenceableItems.TryGetValue(item.RefID, out referenceable))
						{
							this.m_recursiveMember = (referenceable as TablixMember);
						}
						break;
					}
					default:
						Global.Tracer.Assert(false);
						break;
					}
				}
			}
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Visibility;
		}

		internal static bool ComputeHidden(IVisibilityOwner visibilityOwner, AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext renderingContext, ToggleCascadeDirection direction, out bool valueIsDeep)
		{
			valueIsDeep = false;
			bool flag = false;
			Visibility visibility = visibilityOwner.Visibility;
			if (visibility != null)
			{
				switch (Visibility.GetSharedHidden(visibility))
				{
				case SharedHiddenState.Always:
					flag = true;
					break;
				case SharedHiddenState.Never:
					flag = false;
					break;
				case SharedHiddenState.Sometimes:
					flag = visibilityOwner.ComputeStartHidden(renderingContext);
					if (visibility.IsToggleReceiver)
					{
						TextBox toggleSender = visibility.ToggleSender;
						Global.Tracer.Assert(toggleSender != null, "Missing Persisted Toggle Receiver -> Sender Link");
						string senderUniqueName = visibilityOwner.SenderUniqueName;
						if (senderUniqueName != null && renderingContext.IsSenderToggled(senderUniqueName))
						{
							flag = !flag;
						}
						if (!flag)
						{
							flag = Visibility.ComputeDeepHidden(flag, visibilityOwner, direction, renderingContext);
						}
						valueIsDeep = true;
					}
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
			return flag;
		}

		internal static bool ComputeDeepHidden(bool hidden, IVisibilityOwner visibilityOwner, ToggleCascadeDirection direction, AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext renderingContext)
		{
			Visibility visibility = visibilityOwner.Visibility;
			if (hidden && (visibility == null || !visibility.IsToggleReceiver))
			{
				hidden = false;
			}
			if (!hidden && visibility != null && visibility.IsToggleReceiver)
			{
				hidden = ((!visibility.RecursiveReceiver || !(visibilityOwner is TablixMember)) ? (hidden | visibility.ToggleSender.ComputeDeepHidden(renderingContext, direction)) : (hidden | ((TablixMember)visibilityOwner).ComputeToggleSenderDeepHidden(renderingContext)));
			}
			if (!hidden && (visibility == null || !visibility.RecursiveReceiver) && visibilityOwner.ContainingDynamicVisibility != null)
			{
				hidden |= visibilityOwner.ContainingDynamicVisibility.ComputeDeepHidden(renderingContext, direction);
			}
			if (!hidden && direction != ToggleCascadeDirection.Column && visibilityOwner.ContainingDynamicRowVisibility != null)
			{
				hidden |= visibilityOwner.ContainingDynamicRowVisibility.ComputeDeepHidden(renderingContext, direction);
			}
			if (!hidden && direction != ToggleCascadeDirection.Row && visibilityOwner.ContainingDynamicColumnVisibility != null)
			{
				hidden |= visibilityOwner.ContainingDynamicColumnVisibility.ComputeDeepHidden(renderingContext, direction);
			}
			return hidden;
		}
	}
}
