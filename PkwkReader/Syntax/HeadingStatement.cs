using System;

namespace Linearstar.Core.PkwkReader.Syntax
{
    /// <summary>
    /// 見出しを表します。
    /// </summary>
	public class HeadingStatement : WikiStatement
    {
        int level;

        /// <summary>
        /// 見出しの段階を取得または設定します。
        /// </summary>
        /// <remarks>
        /// 見出しの段階は 1～3 の間の値でなくてはなりません。
        /// </remarks>
		public int Level
        {
            get => level;
            set
            {
                if (level < 1 || level > 3) throw new ArgumentException($"Value of {nameof(Level)} must be between 1 and 3.", nameof(value));

                level = value;
            }
        }

        /// <summary>
        /// 見出しとして表示される子要素を取得または設定します。
        /// </summary>
		public WikiExpression Content { get; set; }

        /// <summary>
        /// 見出しのアンカー名を取得または設定します。
        /// </summary>
		public string Anchor { get; set; }

        /// <summary>
        /// 見出しの段階、および表示される子要素を指定して、<see cref="HeadingStatement"/> の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="level">見出しの段階。</param>
        /// <param name="content">表示される子要素。</param>
        public HeadingStatement(int level, WikiExpression content)
            : this(level, content, null)
        {
        }

        /// <summary>
        /// 見出しの段階、表示される子要素、およびアンカー名を指定して、<see cref="HeadingStatement"/> の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="level">見出しの段階。</param>
        /// <param name="content">表示される子要素。</param>
        /// <param name="anchor">アンカー名、または指定しない場合 null。</param>
		public HeadingStatement(int level, WikiExpression content, string anchor)
        {
            if (level < 1 || level > 3) throw new ArgumentException($"Value of {nameof(Level)} must be between 1 and 3.", nameof(level));

            Level = level;
            Content = content ?? throw new ArgumentNullException(nameof(content));
            Anchor = anchor;
        }

        /// <summary>
        /// 指定したコンテキストから見出しを表す <see cref="HeadingStatement"/> を読み取ります。
        /// </summary>
        /// <param name="context">読み取りに使用するコンテキスト。</param>
        /// <returns>読み取られた見出しを表す <see cref="HeadingStatement"/>。</returns>
		public static new HeadingStatement Parse(ParseContext context)
        {
            if (context.Current != '*') throw context.CreateTokenExpectedError("*");

            var level = 1;

            while (context.MoveNext() == '*')
                level++;

            context.SkipWhiteSpaces();

            var content = WikiExpression.Parse(context, () => context.Current == '[' || char.IsWhiteSpace(context.Current) && context.PeekNext() == '[');

            if (char.IsWhiteSpace(context.Current) && context.PeekNext() == '[')
                context.Skip(1);

            var anchor = context.Current == '['
                ? context.Take("[").TakeUntilAny(false, "]", "\n").TrimStart('#')
                : null;

            if (anchor != null)
                context.Take("]");

            context.TakeUntilAny("\n");

            return new HeadingStatement(level, content, anchor);
        }

        /// <summary>
        /// 指定したコンテキストを使用して、現在の要素を変換します。
        /// </summary>
        /// <param name="context">変換に使用するコンテキスト。</param>
        /// <returns>変換結果を表す文字列。</returns>
		public override string Convert(WikiContext context) =>
            $"<h{Level}{(Anchor == null ? null : $" id=\"{Anchor}\"")}>{Content.Convert(context)}</h{Level}>";

        /// <summary>
        /// 現在の要素の Wiki 構文表現を取得します。
        /// </summary>
        /// <returns>要素の Wiki 構文表現。</returns>
		public override string ToWikiString() =>
            $"{new string('*', Level)} {Content.ToWikiString()}{(Anchor == null ? null : $" [#{Anchor}]")}";
    }
}
