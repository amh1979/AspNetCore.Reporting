using System;

namespace AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel
{
	public sealed class UserImpl : User
	{
		internal const string Name = "User";

		internal const string FullName = "AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel.User";

		private string m_userID;

		private string m_language;

		private UserProfileState m_allowUserProfileState;

		private UserProfileState m_hasUserProfileState;

		private UserProfileState m_location = UserProfileState.InReport;

		private bool m_indirectQueryReference;

		public override object this[string key]
		{
			get
			{
				this.UpdateUserProfileState();
				switch (key)
				{
				case "UserID":
					return this.m_userID;
				case "Language":
					return this.m_language;
				default:
					throw new ArgumentOutOfRangeException("key");
				}
			}
		}

		public override string UserID
		{
			get
			{
				this.UpdateUserProfileState();
				return this.m_userID;
			}
		}

		public override string Language
		{
			get
			{
				this.UpdateUserProfileState();
				return this.m_language;
			}
		}

		internal UserProfileState UserProfileLocation
		{
			get
			{
				return this.m_location;
			}
			set
			{
				this.m_location = value;
			}
		}

		internal UserProfileState HasUserProfileState
		{
			get
			{
				return this.m_hasUserProfileState;
			}
			set
			{
				this.m_hasUserProfileState = value;
			}
		}

		internal bool IndirectQueryReference
		{
			get
			{
				return this.m_indirectQueryReference;
			}
			set
			{
				this.m_indirectQueryReference = value;
			}
		}

		internal UserImpl(string userID, string language, UserProfileState allowUserProfileState)
		{
			this.m_userID = userID;
			this.m_language = language;
			this.m_allowUserProfileState = allowUserProfileState;
		}

		internal void UpdateUserProfileState()
		{
			this.m_hasUserProfileState |= this.m_location;
			if (this.m_indirectQueryReference)
			{
				this.m_hasUserProfileState |= UserProfileState.InQuery;
				if ((this.m_allowUserProfileState & UserProfileState.InQuery) == UserProfileState.None)
				{
					throw new ReportProcessingException_UserProfilesDependencies();
				}
			}
			if ((this.m_allowUserProfileState & this.m_location) != 0)
			{
				return;
			}
			throw new ReportProcessingException_UserProfilesDependencies();
		}
	}
}
