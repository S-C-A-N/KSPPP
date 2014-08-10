using System.Collections.Generic;
using System.IO;
using System.Web.Util;
using ColorCode.Common;
using ColorCode.Parsing;
using ColorCode.Styling;

namespace ColorCode.Formatting
{
    public class UnityFormatter : IFormatter
    {
        public void Write(string parsedSourceCode, IList<Scope> scopes, IStyleSheet styleSheet, TextWriter textWriter)
        {
            var styleInsertions = new List<TextInsertion>();

            foreach (Scope scope in scopes)
            {
                GetStyleInsertionsForCapturedStyle(scope, styleInsertions, styleSheet);
            }

            styleInsertions.SortStable((x, y) => x.Index.CompareTo(y.Index));

            int offset = 0;

            foreach (TextInsertion styleInsertion in styleInsertions)
            {
                textWriter.Write(MiniEncode(parsedSourceCode.Substring(offset, styleInsertion.Index - offset)));
                textWriter.Write(styleInsertion.Text);
                offset = styleInsertion.Index;
            }

            textWriter.Write(MiniEncode(parsedSourceCode.Substring(offset)));
        }

        public void WriteFooter(IStyleSheet styleSheet, ILanguage language, TextWriter textWriter)
        {
        }

        public void WriteHeader(IStyleSheet styleSheet, ILanguage language, TextWriter textWriter)
        {
        }

        public string MiniEncode(string txt)
        {
            return txt; //.Replace("<", "&lt;").Replace(">", "&gt;");
        }

        private static void GetStyleInsertionsForCapturedStyle(Scope scope, ICollection<TextInsertion> styleInsertions, IStyleSheet styleSheet)
        {
            Color foreground = Color.Empty;
            Color background = Color.Empty;
            bool italic = false;
            bool bold = false;

            if (styleSheet.Styles.Contains(scope.Name))
            {
                Style style = styleSheet.Styles[scope.Name];

                foreground = style.Foreground;
                background = style.Background;
                italic = style.Italic;
                bold = style.Bold;
            }

            styleInsertions.Add(new TextInsertion { Index = scope.Index, Text = (foreground.IsEmpty ? "" : "<color=\"" + foreground.ToHtmlColor() + "\">") + (italic ? "<i>" : "") + (bold ? "<b>" : "") });


            foreach (Scope childScope in scope.Children)
            {
                GetStyleInsertionsForCapturedStyle(childScope, styleInsertions, styleSheet);
            }

            styleInsertions.Add(new TextInsertion { Index = scope.Index + scope.Length, Text = (bold ? "</b>" : "") + (italic ? "</i>" : "") + (foreground.IsEmpty ? "" : "</color>") });
        }
    }
}