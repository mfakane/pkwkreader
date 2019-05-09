using System.Text;

namespace Linearstar.Core.PkwkReader.Plugins
{
	public class ColorPlugin : IWikiPlugin
	{
		public bool IsMatch(string pluginName) =>
			pluginName == "color";

		public string ConvertBlock(string pluginName, WikiContext context, string[] args, string content) => 
			null;

		public string ConvertInline(string pluginName, WikiContext context, string[] args, string content)
		{
			var sb = new StringBuilder();

			sb.Append($"<span style=\"color: {args[0]};");

			if (args.Length >= 2)
			{
				sb.Append($"background-color: {args[1]};");
			}

			sb.Append("\">");
			sb.Append(content);
			sb.Append("</span>");

			return sb.ToString();
		}
	}
}
