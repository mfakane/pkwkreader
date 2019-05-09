namespace Linearstar.Core.PkwkReader.Plugins
{
	public class DefaultPlugin : IWikiPlugin
	{
		public bool IsMatch(string pluginName)
		{
			switch (pluginName)
			{
				case "norelated":
				case "freeze":
				case "hr":
				case "br":
				case "amp":
				case "heart":
				case "smile":
				case "bigsmile":
				case "huh":
				case "oh":
				case "wink":
				case "sad":
				case "worried":
					return true;
				default:
					return pluginName.StartsWith("#");
			}
		}

		public string ConvertBlock(string pluginName, WikiContext context, string[] args, string content)
		{
			switch (pluginName)
			{
				case "hr":
					return "<hr />";
				case "br":
					return "<br />";
				case "clear":
					return "<span style=\"clear: both;\"></span>";
				case "norelated":
				case "freeze":
					return "";
				default:
					return null;
			}
		}

		public string ConvertInline(string pluginName, WikiContext context, string[] args, string content)
		{
			switch (pluginName)
			{
				case "br":
					return "<br />";
				case "amp":
					return "&amp;";
				case "heart":
				case "smile":
				case "bigsmile":
				case "huh":
				case "oh":
				case "wink":
				case "sad":
				case "worried":
					return "";
				default:
					return pluginName.StartsWith("#") ? $"&{pluginName};" : null;
			}
		}
	}
}
