using System;
using System.Collections.Generic;

namespace Linearstar.Core.PkwkReader.Syntax
{
    /// <summary>
    /// 段落を表します。
    /// </summary>
	public class ParagraphStatement : WikiStatement
    {
        /// <summary>
        /// 内容の配置を取得または設定します。
        /// </summary>
		public ContentAlignment? Alignment { get; set; }

        /// <summary>
        /// 内容となる子要素を取得または設定します。
        /// </summary>
		public WikiExpression Content { get; set; }

        /// <summary>
        /// 内容となる子要素を指定して、取得または設定します。
        /// </summary>
        /// <param name="content">内容となる子要素。</param>
		public ParagraphStatement(WikiExpression content) =>
            Content = content ?? throw new ArgumentNullException(nameof(content));

        /// <summary>
        /// 指定したコンテキストから段落を表す <see cref="NoteExpression"/> を読み取ります。
        /// </summary>
        /// <param name="context">読み取りに使用するコンテキスト。</param>
        /// <returns>読み取られた段落を表す <see cref="NoteExpression"/>。</returns>
		public static new ParagraphStatement Parse(ParseContext context)
        {
            ContentAlignment? alignment = null;

            if (context.Current == '~')
            {
                context.Take("~");
                context.SkipWhiteSpaces();
            }
            else if (context.IsMatchAny("LEFT:") || context.IsMatchAny("RIGHT:") || context.IsMatchAny("CENTER:"))
                switch (context.TakeUntilAny(":"))
                {
                    case "LEFT":
                        alignment = ContentAlignment.Left;

                        break;
                    case "RIGHT":
                        alignment = ContentAlignment.Right;

                        break;
                    case "CENTER":
                        alignment = ContentAlignment.Center;

                        break;
                }

            var rt = new List<WikiExpression>();

            do
            {
                rt.Add(WikiExpression.Parse(context));
                context.TakeUntilAny("\n");
            }
            while (context.Current != '\n'
                && !IsStatementPrefix(context));

            return new ParagraphStatement(rt.Count == 1 ? rt[0] : new CompositeExpression(rt))
            {
                Alignment = alignment,
            };
        }

        /// <summary>
        /// 指定したコンテキストを使用して、現在の要素を変換します。
        /// </summary>
        /// <param name="context">変換に使用するコンテキスト。</param>
        /// <returns>変換結果を表す文字列。</returns>
		public override string Convert(WikiContext context) =>
            $"<p{(Alignment == null ? null : $" style=\"text-align: {Alignment.ToString().ToLower()};\"")}>{Content.Convert(context)}</p>";

        /// <summary>
        /// 現在の要素の Wiki 構文表現を取得します。
        /// </summary>
        /// <returns>要素の Wiki 構文表現。</returns>
		public override string ToWikiString() =>
            Content.ToWikiString() + "\n";
    }
}