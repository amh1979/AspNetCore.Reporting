using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Threading;

namespace AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	public sealed class UserImpl : User
	{
		private struct UserProfileTrackingContext : IDisposable
		{
			private UserImpl m_userImpl;

			private UserProfileState m_oldLocation;

			internal UserProfileTrackingContext(UserImpl userImpl, UserProfileState oldLocation)
			{
				this.m_userImpl = userImpl;
				this.m_oldLocation = oldLocation;
			}

			public void Dispose()
			{
				this.m_userImpl.m_location = this.m_oldLocation;
				Monitor.Exit(this.m_userImpl.m_locationUpdateLock);
			}
		}

		private string m_userID;

		private string m_language;

		private UserProfileState m_allowUserProfileState;

		private UserProfileState m_hasUserProfileState;

		private UserProfileState m_location = UserProfileState.InReport;

		private bool m_indirectQueryReference;

		private object m_locationUpdateLock = new object();

		private OnDemandProcessingContext m_odpContext;

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
					throw new ReportProcessingException_NonExistingUserReference(key);
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
		}

		internal UserProfileState HasUserProfileState
		{
			get
			{
				return this.m_hasUserProfileState;
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

		internal UserImpl(string userID, string language, UserProfileState allowUserProfileState, OnDemandProcessingContext odpContext)
		{
			this.m_userID = userID;
			this.m_language = language;
			this.m_allowUserProfileState = allowUserProfileState;
			this.m_odpContext = odpContext;
		}

		internal UserImpl(UserImpl copy, OnDemandProcessingContext odpContext)
		{
			this.m_userID = copy.m_userID;
			this.m_language = copy.m_language;
			this.m_allowUserProfileState = copy.m_allowUserProfileState;
			this.m_odpContext = odpContext;
		}

		internal IDisposable UpdateUserProfileLocation(UserProfileState newLocation)
		{
			Monitor.Enter(this.m_locationUpdateLock);
			UserProfileTrackingContext userProfileTrackingContext = new UserProfileTrackingContext(this, this.m_location);
			this.m_location = newLocation;
			return (IDisposable)(object)userProfileTrackingContext;
		}

		internal UserProfileState UpdateUserProfileLocationWithoutLocking(UserProfileState newLocation)
		{
			UserProfileState location = this.m_location;
			this.m_location = newLocation;
			return location;
		}

		private void UpdateUserProfileState()
		{
			Exception exceptionToThrow = null;
			UserProfileState userProfileState = this.m_hasUserProfileState | this.m_location;
			if (this.m_indirectQueryReference)
			{
				userProfileState |= UserProfileState.InQuery;
				if ((this.m_allowUserProfileState & UserProfileState.InQuery) == UserProfileState.None)
				{
					exceptionToThrow = new ReportProcessingException_UserProfilesDependencies();
				}
			}
			if (this.m_location != UserProfileState.OnDemandExpressions && (this.m_allowUserProfileState & this.m_location) == UserProfileState.None)
			{
				exceptionToThrow = new ReportProcessingException_UserProfilesDependencies();
			}
			this.UpdateOverallUserProfileState(exceptionToThrow, userProfileState);
		}

		private void UpdateOverallUserProfileState(Exception exceptionToThrow, UserProfileState newState)
		{
			if (newState != this.m_hasUserProfileState)
			{
				this.m_hasUserProfileState = newState;
				if (exceptionToThrow == null || !this.m_odpContext.InSubreport)
				{
					this.m_odpContext.MergeHasUserProfileState(newState);
				}
			}
			if (exceptionToThrow == null)
			{
				return;
			}
			throw exceptionToThrow;
		}

		internal void SetConnectionStringUserProfileDependencyOrThrow()
		{
			Exception exceptionToThrow = null;
			using (this.UpdateUserProfileLocation(UserProfileState.InQuery))
			{
				UserProfileState newState = this.m_hasUserProfileState | this.m_location;
				if ((this.m_allowUserProfileState & UserProfileState.InQuery) == UserProfileState.None)
				{
					exceptionToThrow = new ReportProcessingException_UserProfilesDependencies();
				}
				this.UpdateOverallUserProfileState(exceptionToThrow, newState);
			}
		}
	}
}
