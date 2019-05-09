using System;
using System.IO;
using System.Linq;

namespace Linearstar.Core.PkwkReader
{
    /// <summary>
    /// 要素の変換に必要な情報を表します。
    /// </summary>
    public class WikiContext
    {
        /// <summary>
        /// 読み取りに使用する構成を取得します。
        /// </summary>
		public WikiConfiguration Configuration { get; }

        /// <summary>
        /// 現在のページを取得します。
        /// </summary>
		public WikiDocument Document { get; }

        /// <summary>
        /// 読み取りに使用する構成、および現在のページを指定して、<see cref="WikiContext"/> の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="config">読み取りに使用する構成。</param>
        /// <param name="document">現在の Wiki ページ。</param>
		public WikiContext(WikiConfiguration config, WikiDocument document)
        {
            Configuration = config;
            Document = document;
        }

        /// <summary>
        /// ページ名からソース ファイル名を取得します。
        /// </summary>
        /// <param name="pageName">ページ名。</param>
        /// <returns>ページ名から求められたソース ファイル名。</returns>
		public static string GetFileNameFromPageName(string pageName) =>
            Uri.EscapeDataString(pageName).Replace("%", null).ToUpper() + ".txt";

        /// <summary>
        /// ソース ファイル名からページ名を取得します。
        /// </summary>
        /// <param name="fileName">ソース ファイル名。</param>
        /// <returns>ソース ファイル名から求められたページ名。</returns>
		public static string GetPageNameFromFileName(string fileName)
        {
            var name = Path.GetFileNameWithoutExtension(fileName);

            return string.IsNullOrEmpty(name)
                ? name :
                "%" + string.Join("%", Enumerable.Range(0, name.Length / 2).Select(i => name[i] + name[i + 1]));
        }
    }
}
