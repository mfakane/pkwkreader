using System;

namespace Linearstar.Core.PkwkReader.Syntax
{
    /// <summary>
    /// ブロック要素を表します。
    /// </summary>
	public abstract class WikiStatement
    {
        /// <summary>
        /// 指定したコンテキストを使用して、現在の要素を変換します。
        /// </summary>
        /// <param name="context">変換に使用するコンテキスト。</param>
        /// <returns>変換結果を表す文字列。</returns>
		public abstract string Convert(WikiContext context);

        /// <summary>
        /// 現在の要素の Wiki 構文表現を取得します。
        /// </summary>
        /// <returns>要素の Wiki 構文表現。</returns>
		public abstract string ToWikiString();

        /// <summary>
        /// 現在の要素を表す文字列表現を取得します。
        /// </summary>
        /// <returns>現在の要素を表す文字列表現。</returns>
		public override string ToString() =>
            $"[{GetType().Name.Replace("Statement", null)}] {ToWikiString()}";

        /// <summary>
        /// 指定した文字列および構成からブロック要素を読み取ります。
        /// </summary>
        /// <param name="str">読み取る文字列。</param>
        /// <param name="config">読み取りに使用する構成。</param>
        /// <returns>読み取られたブロック要素。</returns>
        public static WikiStatement Parse(string str, WikiConfiguration config) =>
            Parse(new ParseContext(config, str));

        /// <summary>
        /// 指定したコンテキストからブロック要素を読み取ります。
        /// </summary>
        /// <param name="context">読み取りに使用するコンテキスト。</param>
        /// <returns>読み取られたブロック要素。</returns>
		public static WikiStatement Parse(ParseContext context)
        {
            while (context.Current == '\n'
                || context.Current == '/' && context.PeekNext() == '/')
                context.TakeUntilAny("\n");

            if (context.Current == '\0')
                return null;

            if (context.IsMatchAny(false, "TITLE:"))
                context.Title = context.Skip(6).TakeUntilAny("\n");

            if (ParseSingle(context) is WikiStatement statement)
                return statement;

            return ParagraphStatement.Parse(context);
        }

        /// <summary>
        /// 指定したコンテキストの現在の文字がブロック要素の開始となりうるかどうかを取得します。
        /// </summary>
        /// <param name="context">読み取りに使用するコンテキスト。</param>
        /// <returns>現在の文字がブロック要素の開始となりうるかどうか。</returns>
		protected static bool IsStatementPrefix(ParseContext context)
        {
            switch (context.Current)
            {
                case '#':
                case '*':
                case '+':
                case '-':
                case ':':
                case ' ':
                case '|':
                case '>':
                case '<':
                case '~':
                    return true;
                default:
                    return false;
            }
        }

        static WikiStatement ParseSingle(ParseContext context)
        {
            switch (context.Current)
            {
                case '#':
                    return PluginStatement.Parse(context);
                case '*':
                    return HeadingStatement.Parse(context);
                case '-' when context.PeekNext() == '-' && context.PeekNext(2) == '-':
                    return SeparatorStatement.Parse(context);
                case '+':
                case '-':
                    return ListStatement.Parse(context);
                case ':':
                    throw new NotImplementedException();
                case ' ':
                    return FormattedTextStatement.Parse(context);
                case '|':
                    return TableStatement.Parse(context);
                case '>':
                    return QuoteStatement.Parse(context);
                case '~':
                    return ParagraphStatement.Parse(context);
                default:
                    return null;
            }
        }
    }
}
