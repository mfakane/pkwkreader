using System;
using System.IO;

namespace Linearstar.Core.PkwkReader.Plugins
{
	public class IncludePlugin : IWikiPlugin
	{
		public bool IsMatch(string pluginName) =>
			pluginName == "include";

		public string ConvertBlock(string pluginName, WikiContext context, string[] args, string content)
		{
            if (context.Document.FileName == null && context.Configuration.SourceRoot == null)
                throw new ArgumentException($"Cannot include from a {nameof(WikiDocument)} with no {nameof(WikiDocument.FileName)}. Specify a valid {nameof(WikiConfiguration.SourceRoot)} or try reading from a existing file with {nameof(WikiDocument.FromFile)}.", nameof(context));

			var fileName = Path.Combine(context.Configuration.SourceRoot ?? Path.GetDirectoryName(context.Document.FileName), WikiContext.GetFileNameFromPageName(args[0]));

			return WikiDocument.FromFile(fileName, context.Configuration).ToHtml();
		}

		public string ConvertInline(string pluginName, WikiContext context, string[] args, string content) =>
			null;
	}
}
