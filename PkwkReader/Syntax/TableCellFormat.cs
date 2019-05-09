using System;
using System.Text;

namespace Linearstar.Core.PkwkReader.Syntax
{
    /// <summary>
    /// 表組みのセルの書式設定を表します。
    /// </summary>
	public class TableCellFormat
    {
        /// <summary>
        /// 内容の配置を取得または設定します。
        /// </summary>
        public ContentAlignment? Alignment { get; set; }

        /// <summary>
        /// 背景色を取得または設定します。
        /// </summary>
        public string BackgroundColor { get; set; }

        /// <summary>
        /// 文字色を取得または設定します。
        /// </summary>
        public string ForegroundColor { get; set; }

        /// <summary>
        /// フォント サイズを取得または設定します。
        /// </summary>
        public int? FontSize { get; set; }

        /// <summary>
        /// セル幅を取得または設定します。
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// <see cref="TableCellFormat"/> の新しいインスタンスを初期化します。
        /// </summary>
        public TableCellFormat()
        {
        }

        /// <summary>
        /// 書式文字列を指定して、<see cref="TableCellFormat"/> の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="formatString">書式文字列。</param>
        public TableCellFormat(string formatString)
        {
            var sl = (formatString ?? throw new ArgumentNullException(nameof(formatString))).Trim().TrimEnd(':').Split(':');

            foreach (var i in sl)
            {
                var hasArg = i.Contains("(") && i.EndsWith(")");
                var kind = hasArg ? i.Substring(0, i.IndexOf('(')) : i;
                var arg = hasArg ? i.Substring(kind.Length + 1, i.IndexOf(')', kind.Length + 1) - kind.Length - 1) : null;

                switch (i.ToUpper())
                {
                    case "LEFT":
                        Alignment = ContentAlignment.Left;

                        break;
                    case "RIGHT":
                        Alignment = ContentAlignment.Right;

                        break;
                    case "CENTER":
                        Alignment = ContentAlignment.Center;

                        break;
                    case "BGCOLOR":
                        BackgroundColor = arg;

                        break;
                    case "COLOR":
                        ForegroundColor = arg;

                        break;
                    case "SIZE":
                        FontSize = int.Parse(arg);

                        break;
                }
            }
        }

        /// <summary>
        /// 指定した書式設定と現在の書式設定を結合します。
        /// </summary>
        /// <param name="localFormat">結合する書式設定。</param>
        /// <returns>指定した書式設定と現在の書式設定の結合された書式設定。</returns>
        public TableCellFormat Combine(TableCellFormat localFormat) =>
            new TableCellFormat
            {
                Alignment = localFormat.Alignment ?? Alignment,
                BackgroundColor = localFormat.BackgroundColor ?? BackgroundColor,
                ForegroundColor = localFormat.ForegroundColor ?? ForegroundColor,
                FontSize = localFormat.FontSize ?? FontSize,
                Width = Width,
            };

        /// <summary>
        /// 現在の要素を変換します。
        /// </summary>
        /// <returns>変換結果を表す文字列。</returns>
        public string Convert()
        {
            var sb = new StringBuilder();

            if (Width > 0)
            {
                sb.Append(" width=\"");
                sb.Append(Width);
                sb.Append("\"");
            }

            if (Alignment != null ||
                BackgroundColor != null ||
                ForegroundColor != null ||
                FontSize != null)
            {
                sb.Append(" style=\"");

                switch (Alignment)
                {
                    case ContentAlignment.Left:
                        sb.Append("text-align: left;");

                        break;
                    case ContentAlignment.Right:
                        sb.Append("text-align: right;");

                        break;
                    case ContentAlignment.Center:
                        sb.Append("text-align: center;");

                        break;
                }

                if (BackgroundColor != null)
                {
                    sb.Append("background-color: ");
                    sb.Append(BackgroundColor);
                    sb.Append(";");
                }

                if (ForegroundColor != null)
                {
                    sb.Append("color: ");
                    sb.Append(ForegroundColor);
                    sb.Append(";");
                }

                if (FontSize != null)
                {
                    sb.Append("font-size: ");
                    sb.Append(FontSize);
                    sb.Append("px;");
                }

                sb.Append("\"");
            }

            return sb.ToString();
        }

        /// <summary>
        /// 現在の要素の Wiki 構文表現を取得します。
        /// </summary>
        /// <returns>要素の Wiki 構文表現。</returns>
        public string ToWikiString()
        {
            var sb = new StringBuilder();

            switch (Alignment)
            {
                case ContentAlignment.Left:
                    sb.Append("LEFT:");

                    break;
                case ContentAlignment.Right:
                    sb.Append("RIGHT:");

                    break;
                case ContentAlignment.Center:
                    sb.Append("CENTER:");

                    break;
            }

            if (BackgroundColor != null)
            {
                sb.Append("BGCOLOR(");
                sb.Append(BackgroundColor);
                sb.Append("):");
            }

            if (ForegroundColor != null)
            {
                sb.Append("COLOR(");
                sb.Append(ForegroundColor);
                sb.Append("):");
            }

            if (FontSize != null)
            {
                sb.Append("SIZE(");
                sb.Append(FontSize);
                sb.Append("):");
            }

            if (Width > 0)
                sb.Append(Width);

            return sb.ToString();
        }
    }
}