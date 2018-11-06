namespace AspNetCore.Reporting.Map.WebForms
{
	internal class CallbackManager : MapObject
	{
		private string jsCode = "";

		private string controlUpdates = "";

		private bool disableClientUpdate;

		public bool DisableClientUpdate
		{
			get
			{
				return this.disableClientUpdate;
			}
			set
			{
				this.disableClientUpdate = value;
			}
		}

		public CallbackManager()
			: this(null)
		{
		}

		public CallbackManager(object parent)
			: base(parent)
		{
		}

		public void ExecuteClientScript(string jsSourceCode)
		{
			this.jsCode = this.jsCode + jsSourceCode + "; ";
		}

		internal void Reset()
		{
			this.jsCode = "";
			this.controlUpdates = "";
			this.disableClientUpdate = false;
		}

		internal string GetJavaScript()
		{
			return this.jsCode;
		}

		internal void ResetControlUpdates()
		{
			this.controlUpdates = "";
		}

		internal string GetControlUpdates()
		{
			return this.controlUpdates;
		}
	}
}
