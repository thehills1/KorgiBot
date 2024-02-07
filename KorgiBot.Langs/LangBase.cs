using System.Collections.Generic;

namespace KorgiBot.Langs
{
	public abstract class LangBase : ILang
	{
		private readonly Dictionary<string, string> _translations = new();

		public string this[string translationKey]
		{
			set => _translations.Add(translationKey, value);
		}

		public virtual string GetTranslation(string translationKey, params object[] args)
		{
			if (translationKey == null) return null;

			return _translations.GetValueOrDefault(translationKey);
		}
	}
}
