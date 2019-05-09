using System;

namespace Linearstar.Core.PkwkReader.Syntax
{
    /// <summary>
    /// 平文を表します。
    /// </summary>
	public class PlainExpression : WikiExpression
    {
        /// <summary>
        /// 内容となる文字列を取得または設定します。
        /// </summary>
		public string Text { get; set; }

        /// <summary>
        /// 内容となる文字列を指定して、<see cref="PlainExpression"/> の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="text">内容となる文字列。</param>
		public PlainExpression(string text) =>
            Text = text ?? throw new ArgumentNullException(nameof(text));

        /// <summary>
        /// 指定したコンテキストを使用して、現在の要素を変換します。
        /// </summary>
        /// <param name="context">変換に使用するコンテキスト。</param>
        /// <returns>変換結果を表す文字列。</returns>
		public override string Convert(WikiContext context) =>
            Text.Replace("<", "&lt;").Replace(">", "&gt;");

        /// <summary>
        /// 現在の要素の Wiki 構文表現を取得します。
        /// </summary>
        /// <returns>要素の Wiki 構文表現。</returns>
		public override string ToWikiString() =>
            Text;
    }
}
