using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer
{
	internal sealed class CharacterRun
	{
		private readonly ICharacterRunModel _model;

		public Font Font
		{
			set
			{
				this._model.SetFont(value);
			}
		}

		internal CharacterRun(ICharacterRunModel model)
		{
			this._model = model;
		}

		public override bool Equals(object obj)
		{
			if (obj != null && obj is CharacterRun)
			{
				if (obj == this)
				{
					return true;
				}
				CharacterRun characterRun = (CharacterRun)obj;
				return characterRun._model.Equals(this._model);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return this._model.GetHashCode();
		}
	}
}
