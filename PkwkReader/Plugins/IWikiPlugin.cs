namespace Linearstar.Core.PkwkReader.Plugins
{
	public interface IWikiPlugin
	{
		bool IsMatch(string pluginName);
		string ConvertBlock(string pluginName, WikiContext context, string[] args, string content);
		string ConvertInline(string pluginName, WikiContext context, string[] args, string content);
	}
}
