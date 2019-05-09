using System;

namespace Linearstar.Core.PkwkReader.Syntax
{
    /// <summary>
    /// 強調表示を表します。
    /// </summary>
	public class BoldExpression : WikiExpression
    {
        /// <summary>
        /// 強調表示する子要素を取得または設定します。
        /// </summary>
		public WikiExpression Content { get; set; }

        /// <summary>
        /// 強調表示する子要素を指定して、<see cref="BoldExpression"/> の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="content">強調表示する子要素。</param>
		public BoldExpression(WikiExpression content) =>
            Content = content ?? throw new ArgumentNullException(nameof(content));

        /// <summary>
        /// 指定したコンテキストから強調表示を表す <see cref="BoldExpression"/> を読み取ります。
        /// </summary>
        /// <param name="context">読み取りに使用するコンテキスト。</param>
        /// <returns>読み取られた強調表示を表す <see cref="BoldExpression"/>。</returns>
		public static new BoldExpression Parse(ParseContext context)
        {
            context.Take("''");

            var content = Parse(context, () => context.IsMatchAny("''"));

            context.Take("''");

            return new BoldExpression(content);
        }

        /// <summary>
        /// 指定したコンテキストを使用して、現在の要素を変換します。
        /// </summary>
        /// <param name="context">変換に使用するコンテキスト。</param>
        /// <returns>変換結果を表す文字列。</returns>
		public override string Convert(WikiContext context) =>
            $"<b>{Content.Convert(context)}</b>";

        /// <summary>
        /// 現在の要素の Wiki 構文表現を取得します。
        /// </summary>
        /// <returns>要素の Wiki 構文表現。</returns>
		public override string ToWikiString() =>
            $"''{Content.ToWikiString()}''";
    }
}
