using System;
using System.Linq;

namespace Linearstar.Core.PkwkReader.Syntax
{
    /// <summary>
    /// リンクを表します。
    /// </summary>
	public class LinkExpression : WikiExpression
    {
        /// <summary>
        /// リンクとして表示される子要素を取得または設定します。
        /// </summary>
		public WikiExpression Content { get; set; }

        /// <summary>
        /// リンク名を取得または設定します。
        /// </summary>
        /// <remarks>
        /// リンク名となる値には URL、メールアドレス、またはページ名が含まれます。
        /// </remarks>
		public string Link { get; set; }

        /// <summary>
        /// アンカー名を取得または設定します。
        /// </summary>
		public string Anchor { get; set; }

        /// <summary>
        /// InterWikiName を取得または設定します。
        /// </summary>
		public string InterWikiPrefix { get; set; }

        /// <summary>
        /// リンク名を指定して、<see cref="LinkExpression"/> の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="link">リンク名。</param>
		public LinkExpression(string link) =>
            Content = new PlainExpression(Link = link ?? throw new ArgumentNullException(nameof(link)));

        /// <summary>
        /// リンクとして表示される子要素、およびリンク名を指定して、<see cref="LinkExpression"/> の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="content">リンクとして表示される子要素。</param>
        /// <param name="link">リンク名。</param>
		public LinkExpression(WikiExpression content, string link)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
            Link = link ?? throw new ArgumentNullException(nameof(link));
        }

        /// <summary>
        /// 指定したコンテキストからリンクを表す <see cref="LinkExpression"/> を読み取ります。
        /// </summary>
        /// <param name="context">読み取りに使用するコンテキスト。</param>
        /// <returns>読み取られたリンクを表す <see cref="BoldExpression"/>。</returns>
		public static new LinkExpression Parse(ParseContext context)
        {
            context.Take("[[");

            var content = Parse(context, () => context.IsMatchAny(">", "]]"));
            var linkString = context.Current == '>' ? context.Take(">").TakeUntilAny(false, "]]") : content.ToWikiString();

            context.Take("]]");

            var interWikiAndLink = linkString.Split(new[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries);
            var interWikiPrefix = interWikiAndLink.Length == 1 ? null : interWikiAndLink[0];
            var linkAndAnchor = interWikiAndLink.Last().Split(new[] { '#' }, 2, StringSplitOptions.RemoveEmptyEntries);
            var link = linkAndAnchor[0];
            var anchor = linkAndAnchor.Length == 1 ? null : linkAndAnchor[1];

            return new LinkExpression(content, link)
            {
                InterWikiPrefix = interWikiPrefix,
                Anchor = anchor,
            };
        }

        /// <summary>
        /// 指定したコンテキストを使用して、現在の要素を変換します。
        /// </summary>
        /// <param name="context">変換に使用するコンテキスト。</param>
        /// <returns>変換結果を表す文字列。</returns>
        public override string Convert(WikiContext context) =>
            $"<a href=\"{Link}{(Anchor == null ? null : $"#{Anchor}")}\">{Content.Convert(context)}</a>";

        /// <summary>
        /// 現在の要素の Wiki 構文表現を取得します。
        /// </summary>
        /// <returns>要素の Wiki 構文表現。</returns>
		public override string ToWikiString() =>
            $"[[{Content.ToWikiString()}>{(InterWikiPrefix == null ? null : InterWikiPrefix + ":")}{Link}{(Anchor == null ? null : "#" + Anchor)}]]";
    }
}
