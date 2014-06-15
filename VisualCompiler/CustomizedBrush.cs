using System.Windows.Media;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;

namespace VisualCompiler
{
    internal sealed class CustomizedBrush : HighlightingBrush
    {
        private readonly SolidColorBrush brush;
        public CustomizedBrush(Color color)
        {
            brush = CreateFrozenBrush(color);
        }

        public CustomizedBrush(System.Drawing.Color c)
        {
            var c2 = System.Windows.Media.Color.FromArgb(c.A, c.R, c.G, c.B);
            brush = CreateFrozenBrush(c2);
        }

        public override Brush GetBrush(ITextRunConstructionContext context)
        {
            return brush;
        }

        public override string ToString()
        {
            return brush.ToString();
        }

        private static SolidColorBrush CreateFrozenBrush(Color color)
        {
            SolidColorBrush brush = new SolidColorBrush(color);
            brush.Freeze();
            return brush;
        }
    }
}