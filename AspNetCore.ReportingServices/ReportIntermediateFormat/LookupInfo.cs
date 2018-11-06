using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal class LookupInfo : IPersistable
	{
		private ExpressionInfo m_resultExpr;

		private string m_dataSetName;

		private ExpressionInfo m_sourceExpr;

		private int m_destinationIndexInCollection;

		private string m_name;

		private int m_exprHostID;

		private int m_dataSetIndexInCollection;

		private LookupType m_lookupType;

		[NonSerialized]
		private LookupExprHost m_exprHost;

		[NonSerialized]
		private LookupDestinationInfo m_destinationInfo;

		[NonSerialized]
		private static readonly Declaration m_Declaration = LookupInfo.GetDeclaration();

		internal ExpressionInfo ResultExpr
		{
			get
			{
				return this.m_resultExpr;
			}
			set
			{
				this.m_resultExpr = value;
			}
		}

		internal ExpressionInfo SourceExpr
		{
			get
			{
				return this.m_sourceExpr;
			}
			set
			{
				this.m_sourceExpr = value;
			}
		}

		internal int DestinationIndexInCollection
		{
			get
			{
				return this.m_destinationIndexInCollection;
			}
			set
			{
				this.m_destinationIndexInCollection = value;
			}
		}

		internal int DataSetIndexInCollection
		{
			get
			{
				return this.m_dataSetIndexInCollection;
			}
			set
			{
				this.m_dataSetIndexInCollection = value;
			}
		}

		internal LookupType LookupType
		{
			get
			{
				return this.m_lookupType;
			}
			set
			{
				this.m_lookupType = value;
			}
		}

		internal string Name
		{
			get
			{
				return this.m_name;
			}
			set
			{
				this.m_name = value;
			}
		}

		internal int ExprHostID
		{
			get
			{
				return this.m_exprHostID;
			}
			set
			{
				this.m_exprHostID = value;
			}
		}

		internal LookupExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal LookupDestinationInfo DestinationInfo
		{
			get
			{
				return this.m_destinationInfo;
			}
			set
			{
				this.m_destinationInfo = value;
			}
		}

		internal LookupInfo()
		{
		}

		internal bool ReturnFirstMatchOnly()
		{
			return this.m_lookupType != LookupType.LookupSet;
		}

		internal string GetAsString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(Enum.GetName(typeof(LookupType), this.m_lookupType));
			stringBuilder.Append("(");
			bool flag = false;
			this.AppendWithSeperator(stringBuilder, this.m_sourceExpr, ref flag);
			this.AppendWithSeperator(stringBuilder, this.m_destinationInfo.DestinationExpr, ref flag);
			this.AppendWithSeperator(stringBuilder, this.m_resultExpr, ref flag);
			if (!string.IsNullOrEmpty(this.m_destinationInfo.Scope))
			{
				if (flag)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append("\"");
				stringBuilder.Append(this.m_destinationInfo.Scope);
				stringBuilder.Append("\"");
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			LookupInfo lookupInfo = (LookupInfo)base.MemberwiseClone();
			lookupInfo.m_name = context.CreateLookupID(this.m_name);
			if (this.m_resultExpr != null)
			{
				lookupInfo.m_resultExpr = (ExpressionInfo)this.m_resultExpr.PublishClone(context);
			}
			if (this.m_sourceExpr != null)
			{
				lookupInfo.m_sourceExpr = (ExpressionInfo)this.m_sourceExpr.PublishClone(context);
			}
			lookupInfo.m_destinationInfo = (LookupDestinationInfo)this.m_destinationInfo.PublishClone(context);
			return lookupInfo;
		}

		private void AppendWithSeperator(StringBuilder sb, ExpressionInfo expr, ref bool appendSeperator)
		{
			if (expr != null)
			{
				if (appendSeperator)
				{
					sb.Append(", ");
				}
				sb.Append(expr.OriginalText);
				appendSeperator = true;
			}
		}

		internal void Initialize(InitializationContext context, string dataSetName, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			context.ExprHostBuilder.LookupStart();
			if (this.m_resultExpr != null)
			{
				this.m_resultExpr.LookupInitialize(dataSetName, objectType, objectName, propertyName, context);
				context.ExprHostBuilder.LookupResultExpr(this.m_resultExpr);
			}
			if (this.m_sourceExpr != null)
			{
				this.m_sourceExpr.Initialize(propertyName, context);
				context.ExprHostBuilder.LookupSourceExpr(this.m_sourceExpr);
			}
			this.ExprHostID = context.ExprHostBuilder.LookupEnd();
		}

		internal void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (this.ExprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null, "(reportExprHost != null && reportObjectModel != null)");
				this.m_exprHost = reportExprHost.LookupExprHostsRemotable[this.ExprHostID];
				this.m_exprHost.SetReportObjectModel(reportObjectModel);
			}
		}

		internal AspNetCore.ReportingServices.RdlExpressions.VariantResult EvaluateSourceExpr(AspNetCore.ReportingServices.RdlExpressions.ReportRuntime runtime)
		{
			return runtime.EvaluateLookupSourceExpression(this);
		}

		internal AspNetCore.ReportingServices.RdlExpressions.VariantResult EvaluateResultExpr(AspNetCore.ReportingServices.RdlExpressions.ReportRuntime runtime)
		{
			return runtime.EvaluateLookupResultExpression(this);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(LookupInfo.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ResultExpr:
					writer.Write(this.m_resultExpr);
					break;
				case MemberName.DataSetName:
					writer.Write(this.m_dataSetName);
					break;
				case MemberName.SourceExpr:
					writer.Write(this.m_sourceExpr);
					break;
				case MemberName.DestinationIndexInCollection:
					writer.Write(this.m_destinationIndexInCollection);
					break;
				case MemberName.Name:
					writer.Write(this.m_name);
					break;
				case MemberName.ExprHostID:
					writer.Write(this.m_exprHostID);
					break;
				case MemberName.DataSetIndexInCollection:
					writer.Write(this.m_dataSetIndexInCollection);
					break;
				case MemberName.LookupType:
					writer.WriteEnum((int)this.m_lookupType);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(LookupInfo.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.ResultExpr:
					this.m_resultExpr = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DataSetName:
					this.m_dataSetName = reader.ReadString();
					break;
				case MemberName.SourceExpr:
					this.m_sourceExpr = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.IsMultiValue:
					if (reader.ReadBoolean())
					{
						this.m_lookupType = LookupType.LookupSet;
					}
					else
					{
						this.m_lookupType = LookupType.Lookup;
					}
					break;
				case MemberName.DestinationIndexInCollection:
					this.m_destinationIndexInCollection = reader.ReadInt32();
					break;
				case MemberName.Name:
					this.m_name = reader.ReadString();
					break;
				case MemberName.ExprHostID:
					this.m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.DataSetIndexInCollection:
					this.m_dataSetIndexInCollection = reader.ReadInt32();
					break;
				case MemberName.LookupType:
					this.m_lookupType = (LookupType)reader.ReadEnum();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupInfo;
		}

		public static Declaration GetDeclaration()
		{
			if (LookupInfo.m_Declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.ResultExpr, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
				list.Add(new MemberInfo(MemberName.DataSetName, Token.String));
				list.Add(new MemberInfo(MemberName.SourceExpr, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
				list.Add(new ReadOnlyMemberInfo(MemberName.IsMultiValue, Token.Boolean));
				list.Add(new MemberInfo(MemberName.DestinationIndexInCollection, Token.Int32));
				list.Add(new MemberInfo(MemberName.Name, Token.String));
				list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
				list.Add(new MemberInfo(MemberName.DataSetIndexInCollection, Token.Int32));
				list.Add(new MemberInfo(MemberName.LookupType, Token.Enum));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupInfo, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return LookupInfo.m_Declaration;
		}
	}
}
