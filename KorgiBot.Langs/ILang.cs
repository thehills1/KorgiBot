namespace KorgiBot.Langs
{
	public interface ILang
	{
		string GetTranslation(string translationKey, params object[] args);
	}
}
