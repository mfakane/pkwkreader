using System;

namespace Linearstar.Core.PkwkReader.Syntax
{
    /// <summary>
    /// 水平線を表します。
    /// </summary>
	public class SeparatorStatement : WikiStatement
    {
        int length = 3;

        /// <summary>
        /// 水平線の長さを取得または設定します。
        /// </summary>
		public int Length
        {
            get => length;
            set
            {
                if (length < 1) throw new ArgumentException($"Value of {nameof(Length)} must be greater than zero.", nameof(value));

                length = value;
            }
        }

        /// <summary>
        /// 指定したコンテキストから水平線を表す <see cref="SeparatorStatement"/> を読み取ります。
        /// </summary>
        /// <param name="context">読み取りに使用するコンテキスト。</param>
        /// <returns>読み取られた水平線を表す <see cref="SeparatorStatement"/>。</returns>
        public static new SeparatorStatement Parse(ParseContext context)
        {
            var length = 0;

            while (context.Current == '-')
            {
                context.Take("-");
                length++;
            }

            context.TakeUntilAny("\n");

            return new SeparatorStatement { Length = length };
        }

        /// <summary>
        /// 指定したコンテキストを使用して、現在の要素を変換します。
        /// </summary>
        /// <param name="context">変換に使用するコンテキスト。</param>
        /// <returns>変換結果を表す文字列。</returns>
        public override string Convert(WikiContext context) =>
            "<hr />";

        /// <summary>
        /// 現在の要素の Wiki 構文表現を取得します。
        /// </summary>
        /// <returns>要素の Wiki 構文表現。</returns>
        public override string ToWikiString() =>
            new string('-', Length);
    }
}