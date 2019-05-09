using System.Collections.Generic;
using Linearstar.Core.PkwkReader.Plugins;

namespace Linearstar.Core.PkwkReader
{
    /// <summary>
    /// Wiki の設定を示す構成を表します。
    /// </summary>
    public class WikiConfiguration
    {
        /// <summary>
        /// プラグイン呼び出しで使用されるプラグインを取得または設定します。
        /// </summary>
		public ISet<IWikiPlugin> Plugins { get; set; } = new HashSet<IWikiPlugin>
        {
            new ColorPlugin(),
            new ContentsPlugin(),
            new DefaultPlugin(),
            new IncludePlugin(),
        };

        /// <summary>
        /// 改行文字を改行タグに変換するかどうかを取得または設定します。
        /// </summary>
        public bool ConvertLineBreak { get; set; }

        /// <summary>
        /// ページのソース ファイルが配置されているディレクトリを取得または設定します。
        /// </summary>
        public string SourceRoot { get; set; }

        /// <summary>
        /// <see cref="WikiConfiguration"/> の新しいインスタンスを初期化します。
        /// </summary>
		public WikiConfiguration()
        {
        }
    }
}
