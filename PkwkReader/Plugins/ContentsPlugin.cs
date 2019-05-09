using System.Text;
using Linearstar.Core.PkwkReader.Syntax;

namespace Linearstar.Core.PkwkReader.Plugins
{
	public class ContentsPlugin : IWikiPlugin
	{
		public bool IsMatch(string pluginName) =>
			pluginName == "contents";

		public string ConvertBlock(string pluginName, WikiContext context, string[] args, string content)
		{
			var sb = new StringBuilder();
			var currentLevel = 1;
			var realLevel = 1;
			var isFirst = true;

			sb.AppendLine("<ul>");

			foreach (var i in context.Document.Content)
				if (i is HeadingStatement heading)
				{
					if (heading.Level > currentLevel)
					{
						sb.AppendLine("<ul>");
						realLevel++;
					}
					else if (heading.Level < currentLevel)
					{
						sb.AppendLine("</li>");
						sb.AppendLine("</ul>");
						sb.AppendLine("</li>");
						realLevel--;
					}
					else if (!isFirst)
						sb.AppendLine("</li>");

					isFirst = false;
					sb.AppendLine("<li>");
					sb.Append($"<a href=\"#{heading.Anchor}\">");
					sb.Append(heading.Content.Convert(context));
					sb.AppendLine("</a>");
					currentLevel = heading.Level;
				}

			sb.AppendLine("</li>");

			for (var i = 0; i < realLevel - 1; i++)
			{
				sb.AppendLine("</ul>");
				sb.AppendLine("</li>");
			}

			sb.AppendLine("</ul>");

			return sb.ToString();
		}

		public string ConvertInline(string pluginName, WikiContext context, string[] args, string content) =>
			null;
	}
}
