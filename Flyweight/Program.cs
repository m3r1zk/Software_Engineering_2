using System;
using System.Collections.Generic;

namespace Flyweight
{
    public class TextStyle
    {
        public string FontName { get; }
        public int FontSize { get; }
        public bool IsBold { get; }
        public bool IsItalic { get; }
        public string Color { get; }

        public TextStyle(string fontName, int fontSize, bool isBold, bool isItalic, string color)
        {
            FontName = fontName;
            FontSize = fontSize;
            IsBold = isBold;
            IsItalic = isItalic;
            Color = color;
        }
    }

    public class TextStyleFactory
    {
        private Dictionary<string, TextStyle> _styles = new Dictionary<string, TextStyle>();

        public TextStyle GetTextStyle(string fontName, int fontSize, bool isBold, bool isItalic, string color)
        {
            string key = GetKey(fontName, fontSize, isBold, isItalic, color);

            if (!_styles.ContainsKey(key))
            {
                _styles[key] = new TextStyle(fontName, fontSize, isBold, isItalic, color);
                Console.WriteLine($"Создан новый стиль: {key}");
            }
            else
            {
                Console.WriteLine($"Использован существующий стиль: {key}");
            }

            return _styles[key];
        }

        private string GetKey(string fontName, int fontSize, bool isBold, bool isItalic, string color)
        {
            return $"{fontName}_{fontSize}_{isBold}_{isItalic}_{color}";
        }

        public int GetStyleCount()
        {
            return _styles.Count;
        }
    }

    public class TextLine
    {
        private TextStyle _style;
        private string _text;
        private int _positionX;
        private int _positionY;

        public TextLine(string text, int x, int y, TextStyle style)
        {
            _text = text;
            _positionX = x;
            _positionY = y;
            _style = style;
        }
    }

    public class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== Паттерн Flyweight: Оптимизация использования стилей текста ===\n");

            TextStyleFactory styleFactory = new TextStyleFactory();

            List<TextLine> document = new List<TextLine>();

            TextStyle titleStyle = styleFactory.GetTextStyle("Arial", 16, true, false, "Red");
            document.Add(new TextLine("Документ 1", 0, 0, titleStyle));

            TextStyle normalStyle = styleFactory.GetTextStyle("Times New Roman", 12, false, false, "White");
            document.Add(new TextLine("Это основной текст документа.", 0, 1, normalStyle));

            TextStyle boldStyle = styleFactory.GetTextStyle("Times New Roman", 12, true, false, "Yellow");
            document.Add(new TextLine("Это важная информация.", 0, 2, boldStyle));

            TextStyle italicStyle = styleFactory.GetTextStyle("Times New Roman", 12, false, true, "Cyan");
            document.Add(new TextLine("Это цитата из другого источника.", 0, 3, italicStyle));

            TextStyle titleStyle2 = styleFactory.GetTextStyle("Arial", 16, true, false, "Red");
            document.Add(new TextLine("Раздел 2", 0, 5, titleStyle2));

            TextStyle normalStyle2 = styleFactory.GetTextStyle("Times New Roman", 12, false, false, "White");
            document.Add(new TextLine("Продолжение основного текста.", 0, 6, normalStyle2));

            TextStyle largeStyle = styleFactory.GetTextStyle("Arial", 20, true, true, "Green");
            document.Add(new TextLine("ВАЖНОЕ УВЕДОМЛЕНИЕ", 0, 8, largeStyle));

            TextStyle italicStyle2 = styleFactory.GetTextStyle("Times New Roman", 12, false, true, "Cyan");
            document.Add(new TextLine("Еще одна цитата.", 0, 9, italicStyle2));

            Console.WriteLine($"\nВсего создано стилей: {styleFactory.GetStyleCount()}");
            Console.WriteLine($"Всего создано текстовых строк: {document.Count}");
            Console.WriteLine($"Экономия памяти: вместо {document.Count} объектов стилей используется {styleFactory.GetStyleCount()}\n");
        }
    }
}
