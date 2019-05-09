using System;
using System.Linq;
using Linearstar.Core.PkwkReader.Plugins;

namespace Linearstar.Core.PkwkReader.Syntax
{
    /// <summary>
    /// インライン形式のプラグイン呼び出しを表します。
    /// </summary>
	public class PluginExpression : WikiExpression
    {
        /// <summary>
        /// プラグイン名を取得または設定します。
        /// </summary>
		public string Name { get; set; }

        /// <summary>
        /// 引数を取得または設定します。
        /// </summary>
		public string[] Arguments { get; set; }

        /// <summary>
        /// 内容となる子要素を取得または設定します。
        /// </summary>
		public WikiExpression Content { get; set; }

        /// <summary>
        /// プラグイン名を指定して、<see cref="PluginExpression"/> の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="name">プラグイン名。</param>
        public PluginExpression(string name)
            : this(name, null)
        {
        }

        /// <summary>
        /// プラグイン名、および引数を指定して、<see cref="PluginExpression"/> の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="name">プラグイン名。</param>
        /// <param name="args">引数、または存在しない場合 null。</param>
        public PluginExpression(string name, string[] args)
            : this(name, args, null)
        {
        }

        /// <summary>
        /// プラグイン名、引数、および内容となる子要素を指定して、<see cref="PluginExpression"/> の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="name">プラグイン名。</param>
        /// <param name="args">引数、または存在しない場合 null。</param>
        /// <param name="content">内容となる子要素、または存在しない場合 null。</param>
		public PluginExpression(string name, string[] args, WikiExpression content)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Arguments = args;
            Content = content;
        }

        /// <summary>
        /// 指定したコンテキストからインライン形式のプラグイン呼び出しを表す <see cref="PluginExpression"/> を読み取ります。
        /// </summary>
        /// <param name="context">読み取りに使用するコンテキスト。</param>
        /// <returns>読み取られたインライン形式のプラグイン呼び出しを表す <see cref="PluginExpression"/>。</returns>
        public static new PluginExpression Parse(ParseContext context)
        {
            context.Take("&");

            var name = context.TakeUntilAny(false, "(", "{", ";");
            string[] args = null;
            WikiExpression content = null;

            if (context.Current == '(')
                args = context.Skip(1).TakeUntilAny(")").Split(new[] { ',' }).Select(i => i.Trim()).ToArray();

            if (context.Current == '{')
            {
                context.Skip(1);
                content = Parse(context, () => context.Current == '}');
                context.Take("}");
            }

            context.Take(";");

            return new PluginExpression(name, args, content);
        }

        /// <summary>
        /// 指定したコンテキストを使用して、現在の要素を変換します。
        /// </summary>
        /// <param name="context">変換に使用するコンテキスト。</param>
        /// <returns>変換結果を表す文字列。</returns>
        public override string Convert(WikiContext context)
        {
            if (!(context.Configuration.Plugins.FirstOrDefault(i => i.IsMatch(Name)) is IWikiPlugin plugin) ||
                !(plugin.ConvertInline(Name, context, Arguments, Content?.Convert(context)) is string rt))
                throw new Exception($"Plugin {Name} not found.");

            return rt;
        }

        /// <summary>
        /// 現在の要素の Wiki 構文表現を取得します。
        /// </summary>
        /// <returns>要素の Wiki 構文表現。</returns>
        public override string ToWikiString() =>
            $"&{Name}{(Arguments == null ? null : $"({string.Join(",", Arguments)})")}{(Content == null ? null : $"{{{Content.ToWikiString()}}}")};";
    }
}
