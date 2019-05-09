using System;
using System.Text;

namespace Linearstar.Core.PkwkReader.Syntax
{
    /// <summary>
    /// 表組みのセルを表します。
    /// </summary>
	public class TableCell
    {
        /// <summary>
        /// 書式設定を取得または設定します。
        /// </summary>
		public TableCellFormat Format { get; set; }

        /// <summary>
        /// 内容となる子要素を取得または設定します。
        /// </summary>
		public WikiExpression Content { get; set; }

        /// <summary>
        /// ヘッダであるかどうかを取得または設定します。
        /// </summary>
		public bool IsHeader { get; set; }

        /// <summary>
        /// 横方向の連結数を取得または設定します。
        /// </summary>
		public int ColumnSpan { get; set; } = 1;

        /// <summary>
        /// 縦方向の連結数を取得または設定します。
        /// </summary>
		public int RowSpan { get; set; } = 1;

        /// <summary>
        /// <see cref="TableCell"/> の新しいインスタンスを初期化します。
        /// </summary>
		public TableCell()
        {
        }

        /// <summary>
        /// 内容となる子要素、およびヘッダであるかどうかを指定して、<see cref="TableCell"/> の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="content">内容となる子要素。</param>
        /// <param name="isHeader">ヘッダであるかどうか。</param>
		public TableCell(WikiExpression content, bool isHeader)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
            IsHeader = isHeader;
        }

        internal static TableCell Parse(ref int columnSpan, TableCell cellAbove, ParseContext context)
        {
            if (context.Current != '|') throw context.CreateTokenExpectedError("|");

            var startIndex = context.Index;
            var isHeader = false;

            context.Skip(1);

            switch (context.Current)
            {
                case '>' when context.PeekNext() == '|':
                    context.Skip(1);
                    columnSpan++;

                    return null;
                case '~' when context.PeekNext() == '|':
                    context.Skip(1);
                    cellAbove.RowSpan++;

                    return null;
                case '~':
                    isHeader = true;
                    context.SkipWhiteSpaces();

                    goto default;
                default:
                    context.SkipWhiteSpaces();

                    var formatString = new StringBuilder();

                    while (context.IsMatchAny(false, "LEFT:", "RIGHT:", "CENTER:", "BGCOLOR(", "COLOR(", "SIZE("))
                    {
                        formatString.Append(context.TakeUntilAny(false, ":", "|"));
                        formatString.Append(":");

                        if (context.Current == ':')
                            context.Skip(1);
                    }

                    var content = WikiExpression.Parse(context, () => context.Current == '|');

                    if (context.Current == '|')
                        return new TableCell(content, isHeader)
                        {
                            Format = formatString.Length == 0
                                ? new TableCellFormat()
                                : new TableCellFormat(formatString.ToString()),
                        };

                    context.Index = startIndex;

                    return null;
            }
        }

        /// <summary>
        /// 変換に使用するコンテキスト、および列の書式設定を指定して、現在の要素を変換します。
        /// </summary>
        /// <param name="context">変換に使用するコンテキスト。</param>
        /// <param name="columnFormat">列の書式設定。</param>
        /// <returns>変換結果を表す文字列。</returns>
		public string Convert(WikiContext context, TableCellFormat columnFormat)
        {
            var sb = new StringBuilder();

            sb.Append("<");
            sb.Append(IsHeader ? "th" : "td");
            sb.Append((columnFormat?.Combine(Format) ?? Format).Convert());

            if (RowSpan > 1)
                sb.Append($" rowspan=\"{RowSpan}\"");

            if (ColumnSpan > 1)
                sb.Append($" colspan=\"{ColumnSpan}\"");

            sb.Append(">");
            sb.Append(Content.Convert(context));
            sb.Append("</");
            sb.Append(IsHeader ? "th" : "td");
            sb.AppendLine(">");

            return sb.ToString();
        }

        /// <summary>
        /// 現在の要素の Wiki 構文表現を取得します。
        /// </summary>
        /// <returns>要素の Wiki 構文表現。</returns>
		public string ToWikiString() =>
            Format?.ToWikiString() + Content.ToWikiString();
    }
}
