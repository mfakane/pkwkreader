using System;
using System.Collections.Generic;
using System.Linq;

namespace Linearstar.Core.PkwkReader.Syntax
{
    /// <summary>
    /// 複数の子要素を含むブロック要素を表します。
    /// </summary>
	public class CompositeStatement : WikiStatement
    {
        /// <summary>
        /// 現在の要素に含まれる子要素を取得または設定します。
        /// </summary>
		public IList<WikiStatement> Children { get; set; }

        /// <summary>
        /// 含まれる子要素を指定して、<see cref="CompositeStatement"/> の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="children">含まれる子要素。</param>
        public CompositeStatement(IList<WikiStatement> children) =>
            Children = children ?? throw new ArgumentNullException(nameof(children));

        /// <summary>
        /// 指定したコンテキストを使用して、現在の要素を変換します。
        /// </summary>
        /// <param name="context">変換に使用するコンテキスト。</param>
        /// <returns>変換結果を表す文字列。</returns>
        public override string Convert(WikiContext context) =>
            string.Join(null, Children.Select(i => i.Convert(context)));

        /// <summary>
        /// 現在の要素の Wiki 構文表現を取得します。
        /// </summary>
        /// <returns>要素の Wiki 構文表現。</returns>
        public override string ToWikiString() =>
            string.Join(null, Children.Select(i => i.ToWikiString()));
    }
}
