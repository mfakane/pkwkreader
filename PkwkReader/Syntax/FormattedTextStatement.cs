using System;
using System.Linq;
using System.Text;

namespace Linearstar.Core.PkwkReader.Syntax
{
    /// <summary>
    /// 整形済みテキストを表します。
    /// </summary>
	public class FormattedTextStatement : WikiStatement
    {
        /// <summary>
        /// 内容となる文字列を取得または設定します。
        /// </summary>
		public string Text { get; set; }

        /// <summary>
        /// 内容となる文字列を指定して、<see cref="FormattedTextStatement"/> の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="text">内容となる文字列。</param>
		public FormattedTextStatement(string text) =>
            Text = text ?? throw new ArgumentNullException(nameof(text));

        /// <summary>
        /// 指定したコンテキストから整形済みテキストを表す <see cref="FormattedTextStatement"/> を読み取ります。
        /// </summary>
        /// <param name="context">読み取りに使用するコンテキスト。</param>
        /// <returns>読み取られた整形済みテキストを表す <see cref="FormattedTextStatement"/>。</returns>
		public static new FormattedTextStatement Parse(ParseContext context)
        {
            var sb = new StringBuilder();

            while (context.Current == ' ')
                sb.AppendLine(context.TakeUntilAny("\n").Substring(1));

            return new FormattedTextStatement(sb.ToString());
        }

        /// <summary>
        /// 指定したコンテキストを使用して、現在の要素を変換します。
        /// </summary>
        /// <param name="context">変換に使用するコンテキスト。</param>
        /// <returns>変換結果を表す文字列。</returns>
		public override string Convert(WikiContext context) =>
            $"<pre>{Text}</pre>";

        /// <summary>
        /// 現在の要素の Wiki 構文表現を取得します。
        /// </summary>
        /// <returns>要素の Wiki 構文表現。</returns>
		public override string ToWikiString() =>
            string.Join("\n", Text.Split('\n').Select(i => " " + i));
    }
}
