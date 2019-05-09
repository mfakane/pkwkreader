using System.Collections.Generic;
using System.IO;
using System.Linq;
using Linearstar.Core.PkwkReader.Syntax;

namespace Linearstar.Core.PkwkReader
{
    /// <summary>
    /// ページを表します。
    /// </summary>
	public class WikiDocument
    {
        /// <summary>
        /// ファイル名を取得または設定します。
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// ページ名を取得または設定します。
        /// </summary>
        public string PageName { get; set; }

        /// <summary>
        /// タイトルを取得または設定します。
        /// </summary>
		public string Title { get; set; }

        /// <summary>
        /// 読み取りおよび変換に使用する構成を取得します。
        /// </summary>
		public WikiConfiguration Configuration { get; }

        /// <summary>
        /// 内容となるブロック要素を取得または設定します。
        /// </summary>
		public IList<WikiStatement> Content { get; set; } = new List<WikiStatement>();

        /// <summary>
        /// 読み取りおよび変換に使用する構成、および内容となるブロック要素を指定して、<see cref="WikiDocument"/> の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="config">読み取りおよび変換に使用する構成。</param>
        /// <param name="content">内容となるブロック要素の配列。</param>
		public WikiDocument(WikiConfiguration config, params WikiStatement[] content)
        {
            Configuration = config;

            foreach (var i in content)
                Content.Add(i);
        }

        /// <summary>
        /// 文字列から新しい <see cref="WikiDocument"/> を作成します。
        /// </summary>
        /// <param name="content">内容となる Wiki 構文を含む文字列。</param>
        /// <param name="config">読み取りおよび変換に使用する構成。</param>
        /// <returns>読み取られた要素を内容とする <see cref="WikiDocument"/> のインスタンス。</returns>
		public static WikiDocument Parse(string content, WikiConfiguration config)
        {
            var pctx = new ParseContext(config, content);
            var rt = new WikiDocument(config);

            while (WikiStatement.Parse(pctx) is WikiStatement statement)
                rt.Content.Add(statement);

            rt.Title = pctx.Title;

            return rt;
        }

        /// <summary>
        /// ファイルから新しい <see cref="WikiDocument"/> を作成します。
        /// </summary>
        /// <param name="fileName">内容となる Wiki 構文を含むファイル。</param>
        /// <param name="config">読み取りおよび変換に使用する構成。</param>
        /// <returns>読み取られた要素を内容とする <see cref="WikiDocument"/> のインスタンス。</returns>
        public static WikiDocument FromFile(string fileName, WikiConfiguration config)
        {
            var rt = Parse(File.ReadAllText(fileName), config);

            rt.FileName = fileName;
            rt.PageName = WikiContext.GetPageNameFromFileName(fileName);

            if (rt.Title == null) rt.Title = rt.PageName;

            return rt;
        }

        /// <summary>
        /// 内容となる要素が変換された HTML 文字列を取得します。
        /// </summary>
        /// <returns>内容となる要素が変換された HTML 文字列。</returns>
		public string ToHtml()
        {
            var ctx = new WikiContext(Configuration, this);

            return string.Join("\n", Content.Select(i => i.Convert(ctx)));
        }
    }
}
