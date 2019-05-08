using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace labGraph
{
    [Serializable]
    public sealed class Vertex : ICloneable
    {
        private Point _point;
        private readonly string _name;
        private int _sizeX;
        private int _sizeY;
        private int _weight = 0;
        public int Weight { get => _weight; set => _weight = value; }
        private int _degree = 0;
        public int Deegre { get => _degree; set => _degree = value; }
        public Point Point
        {
            get => _point;
            set => _point = value;
        }
        public int SizeX { get => _sizeX; set => _sizeX = value; }
        public int SizeY { get => _sizeY; set => _sizeY = value; }

        public Vertex(string name, Point point)
        {
            _name = name.Replace(' ','_');
            Point = point;
            SizeX = _name.Length * 5 + 30;
            SizeY = 30;
        }
        public Vertex(string name="default")
        {
            _name = name;
            Point = new Point(10,10);

        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            Vertex m = obj as Vertex; 
            if (m as Vertex == null)
                return false;

            return ((Vertex)obj)._name == this._name && ((Vertex)obj).Point == this.Point;
        }
        public override int GetHashCode()
        {
          
            return _name.GetHashCode();
        }
        public void Draw(Canvas canvas, Color colorBrush, Brush colorStroke = null)
        {
            SolidColorBrush colorBrushEllipse = new SolidColorBrush();
            SolidColorBrush colorBrushText = new SolidColorBrush();

            colorBrushEllipse.Color = colorBrush;//Color.FromArgb(215, 215, 215, 215);
            colorBrushText.Color = Color.FromArgb(250, 0, 0, 0);


            Ellipse myEllipse = new Ellipse();
            if (colorStroke != null)
                myEllipse.Stroke = colorStroke;
            myEllipse.Fill = colorBrushEllipse;
            myEllipse.Width = SizeX;
            myEllipse.Height = SizeY;
            myEllipse.Margin = new Thickness(_point.X - SizeX / 2, _point.Y - SizeY / 2, 0, 0);

            TextBlock textBlock = new TextBlock();

            textBlock.Text = this.ToString();
            textBlock.Foreground = colorBrushText;

            Canvas.SetLeft(textBlock, _point.X - SizeX / 4);
            Canvas.SetTop(textBlock, _point.Y - SizeY / 3);

            canvas.Children.Add(myEllipse);
            canvas.Children.Add(textBlock);


        }
        public static bool operator ==(Vertex left, Vertex right)
        {
            return Equals(left, right);
        }
        public static bool operator !=(Vertex left, Vertex right)
        {
            return !Equals(left, right);
        }


        public override string ToString()
        {
            return _name.Length<10?_name:_name.Substring(0,9);
        }

        public object Clone()
        {
            return new Vertex(_name, new Point(_point.X, _point.Y));
        }
    }
}
