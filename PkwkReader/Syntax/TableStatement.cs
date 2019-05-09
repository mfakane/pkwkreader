using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linearstar.Core.PkwkReader.Syntax
{
    /// <summary>
    /// 表組みを表します。
    /// </summary>
	public class TableStatement : WikiStatement
    {
        /// <summary>
        /// セルの書式設定を取得または設定します。
        /// </summary>
		public TableCellFormat[] Formats { get; set; }

        /// <summary>
        /// ヘッダ行のセルを取得または設定します。
        /// </summary>
        public TableCell[] HeaderRow { get; set; }

        /// <summary>
        /// 各行のセルを取得または設定します。
        /// </summary>
		public IList<TableCell[]> Rows { get; set; }

        /// <summary>
        /// フッタ行のセルを取得または設定します。
        /// </summary>
		public TableCell[] FooterRow { get; set; }

        /// <summary>
        /// 各行のセルを指定して、<see cref="TableStatement"/> の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="rows">各行のセル。</param>
		public TableStatement(IList<TableCell[]> rows) =>
            Rows = rows ?? throw new ArgumentNullException(nameof(rows));

        /// <summary>
        /// 指定したコンテキストから表組みを表す <see cref="TableStatement"/> を読み取ります。
        /// </summary>
        /// <param name="context">読み取りに使用するコンテキスト。</param>
        /// <returns>読み取られた表組みを表す <see cref="TableStatement"/>。</returns>
		public static new TableStatement Parse(ParseContext context)
        {
            if (context.Current != '|') throw context.CreateTokenExpectedError("|");

            TableCellFormat[] formats = null;
            TableCell[] thead = null;
            TableCell[] tfoot = null;
            var rows = new List<TableCell[]>();
            var previousColumnCount = 0;

            while (context.Current == '|')
            {
                var startIndex = context.Index;
                var columns = new List<TableCell>();
                string format = null;
                var columnSpan = 1;

                for (var columnIndex = 0; context.Current == '|' && context.PeekNext() != '\n'; columnIndex++)
                    if (TableCell.Parse(ref columnSpan, rows.Select(i => i[columnIndex]).LastOrDefault(i => i != null), context) is TableCell cell)
                    {
                        cell.ColumnSpan = columnSpan;
                        columns.Add(cell);
                        columnSpan = 1;
                    }
                    else if (context.Current == '|')
                        columns.Add(null);
                    else
                        format = context.Skip(1).TakeUntilAny(false, "\n");

                if (previousColumnCount != 0 && columns.Count != previousColumnCount)
                {
                    context.Index = startIndex;

                    break;
                }

                context.TakeUntilAny("\n");
                previousColumnCount = columns.Count;

                switch (format)
                {
                    case "c":
                        formats = columns.Select(i =>
                        {
                            if (int.TryParse(i.Content.ToString(), out var width))
                                i.Format.Width = width;

                            return i.Format;
                        }).ToArray();

                        break;
                    case "h":
                        thead = columns.ToArray();

                        break;
                    case "f":
                        tfoot = columns.ToArray();

                        break;
                    default:
                        rows.Add(columns.ToArray());

                        break;
                }
            }

            return new TableStatement(rows)
            {
                Formats = formats,
                HeaderRow = thead,
                FooterRow = tfoot,
            };
        }

        static void ConvertColumns(StringBuilder sb, WikiContext context, TableCell[] cells, TableCellFormat[] formats)
        {
            var idx = 0;

            foreach (var i in cells)
            {
                if (i != null)
                    sb.AppendLine(i.Convert(context, formats?[idx]));

                idx++;
            }
        }

        /// <summary>
        /// 指定したコンテキストを使用して、現在の要素を変換します。
        /// </summary>
        /// <param name="context">変換に使用するコンテキスト。</param>
        /// <returns>変換結果を表す文字列。</returns>
        public override string Convert(WikiContext context)
        {
            var sb = new StringBuilder();

            sb.AppendLine("<table>");

            if (HeaderRow != null)
            {
                sb.AppendLine("<thead>");
                ConvertColumns(sb, context, HeaderRow, Formats);
                sb.AppendLine("</thead>");
            }

            foreach (var i in Rows)
            {
                sb.AppendLine("<tr>");
                ConvertColumns(sb, context, i, Formats);
                sb.AppendLine("</tr>");
            }

            if (FooterRow != null)
            {
                sb.AppendLine("<tfoot>");
                ConvertColumns(sb, context, FooterRow, Formats);
                sb.AppendLine("</tfoot>");
            }

            sb.AppendLine("</table>");

            return sb.ToString();
        }

        static void ColumnsToWikiString(StringBuilder sb, TableCell[] cells, int[] cellsToRowSpan)
        {
            var idx = 0;

            foreach (var i in cells)
            {
                if (i != null)
                {
                    for (var j = 1; j < i.ColumnSpan; j++)
                        sb.Append("|>");

                    sb.Append("|");
                    sb.Append(i.ToWikiString());
                }
                else if (cellsToRowSpan[idx] > 1)
                    sb.Append("|~");

                idx++;
            }
        }

        /// <summary>
        /// 現在の要素の Wiki 構文表現を取得します。
        /// </summary>
        /// <returns>要素の Wiki 構文表現。</returns>
        public override string ToWikiString()
        {
            var sb = new StringBuilder();

            if (Formats != null)
            {
                foreach (var i in Formats)
                {
                    sb.Append("|");
                    sb.Append(i.ToWikiString());
                }

                sb.AppendLine("|c");
            }

            if (HeaderRow != null)
            {
                ColumnsToWikiString(sb, HeaderRow, null);
                sb.AppendLine("|h");
            }

            var cellsToRowSpan = Rows[0].Select(i => i.RowSpan).ToArray();

            foreach (var i in Rows)
            {
                ColumnsToWikiString(sb, i, cellsToRowSpan);
                cellsToRowSpan = cellsToRowSpan.Zip(i, (x, y) => Math.Max(x - 1, y?.RowSpan ?? 0)).ToArray();
                sb.AppendLine("|");
            }

            if (FooterRow != null)
            {
                ColumnsToWikiString(sb, FooterRow, null);
                sb.AppendLine("|f");
            }

            return sb.ToString();
        }
    }
}
