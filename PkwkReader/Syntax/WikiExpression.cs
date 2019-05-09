using System;
using System.Collections.Generic;
using System.Text;

namespace Linearstar.Core.PkwkReader.Syntax
{
    /// <summary>
    /// インライン要素を表します。
    /// </summary>
	public abstract class WikiExpression
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
            $"({GetType().Name.Replace("Expression", null)}) {ToWikiString()}";

        /// <summary>
        /// 指定した文字列および構成からインライン要素を読み取ります。
        /// </summary>
        /// <param name="str">読み取る文字列。</param>
        /// <param name="config">読み取りに使用する構成。</param>
        /// <returns>読み取られたインライン要素。</returns>
        public static WikiExpression Parse(string str, WikiConfiguration config) =>
            Parse(new ParseContext(config, str));

        /// <summary>
        /// 指定したコンテキストからインライン要素を読み取ります。
        /// </summary>
        /// <param name="context">読み取りに使用するコンテキスト。</param>
        /// <returns>読み取られたインライン要素。</returns>
		public static WikiExpression Parse(ParseContext context) =>
            Parse(context, null);

        /// <summary>
        /// 指定した条件を満たすまで、指定したコンテキストからインライン要素を読み取ります。
        /// </summary>
        /// <param name="context">読み取りに使用するコンテキスト。</param>
        /// <param name="takeUntil">読み取りを停止する条件、または指定しない場合 null。</param>
        /// <returns>読み取られたインライン要素。</returns>
		public static WikiExpression Parse(ParseContext context, Func<bool> takeUntil)
        {
            if (takeUntil?.Invoke() ?? false)
                return null;

            if (context.Index == 0 || context.PeekNext(-1) == '\n')
                while (context.Current == '/' && context.PeekNext() == '/')
                    context.TakeUntilAny("\n");

            var rt = new List<WikiExpression>();
            StringBuilder currrentText = null;

            do
            {
                var node = ParseSingle(context);

                if (node == null)
                {
                    if (currrentText == null) currrentText = new StringBuilder();

                    currrentText.Append(context.Current);
                    context.MoveNext();
                }
                else
                {
                    if (currrentText != null)
                    {
                        rt.Add(new PlainExpression(currrentText.ToString()));
                        currrentText = null;
                    }

                    rt.Add(node);
                }
            }
            while (context.Current != '\0'
                && context.Current != '\n'
                && !(takeUntil?.Invoke() ?? false));

            if (currrentText != null)
                rt.Add(new PlainExpression(currrentText.ToString()));

            return
                rt.Count == 0 ? null :
                rt.Count == 1 ? rt[0] : new CompositeExpression(rt);
        }

        static WikiExpression ParseSingle(ParseContext context)
        {
            switch (context.Current)
            {
                case '\'' when context.PeekNext() == '\'':
                    return BoldExpression.Parse(context);
                case '\'' when context.PeekNext() == '\'' && context.PeekNext(2) == '\'':
                    return ItalicExpression.Parse(context);
                case '%' when context.PeekNext() == '%':
                    return StrikeExpression.Parse(context);
                case '(' when context.PeekNext() == '(':
                    return NoteExpression.Parse(context);
                case '[' when context.PeekNext() == '[':
                    return LinkExpression.Parse(context);
                case '&' when context.PeekNext() is char c && !char.IsWhiteSpace(c) && !char.IsSymbol(c) && !char.IsControl(c):
                    return PluginExpression.Parse(context);
                default:
                    return null;
            }
        }
    }
}
