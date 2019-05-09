using System;

namespace Linearstar.Core.PkwkReader.Syntax
{
    /// <summary>
    /// 打ち消し表示を表します。
    /// </summary>
	public class StrikeExpression : WikiExpression
    {
        /// <summary>
        /// 打ち消し表示する子要素を取得または設定します。
        /// </summary>
		public WikiExpression Content { get; set; }

        /// <summary>
        /// 打ち消し表示する子要素を指定して、<see cref="StrikeExpression"/> の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="content">打ち消し表示する子要素。</param>
		public StrikeExpression(WikiExpression content) =>
            Content = content ?? throw new ArgumentNullException(nameof(content));

        /// <summary>
        /// 指定したコンテキストから打ち消し表示を表す <see cref="StrikeExpression"/> を読み取ります。
        /// </summary>
        /// <param name="context">読み取りに使用するコンテキスト。</param>
        /// <returns>読み取られた打ち消し表示を表す <see cref="StrikeExpression"/>。</returns>
		public static new StrikeExpression Parse(ParseContext context)
        {
            context.Take("%%");

            var content = Parse(context, () => context.IsMatchAny("%%"));

            context.Take("%%");

            return new StrikeExpression(content);
        }

        /// <summary>
        /// 指定したコンテキストを使用して、現在の要素を変換します。
        /// </summary>
        /// <param name="context">変換に使用するコンテキスト。</param>
        /// <returns>変換結果を表す文字列。</returns>
		public override string Convert(WikiContext context) =>
            $"<s>{Content.Convert(context)}</s>";

        /// <summary>
        /// 現在の要素の Wiki 構文表現を取得します。
        /// </summary>
        /// <returns>要素の Wiki 構文表現。</returns>
		public override string ToWikiString() =>
            $"%%{Content.ToWikiString()}%%";
    }
}
