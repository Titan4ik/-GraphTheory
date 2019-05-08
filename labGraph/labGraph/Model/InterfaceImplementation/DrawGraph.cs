using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace labGraph.Model.InterfaceImplementation
{

    public class DrawGraph : IDrawGraph
    {

        public void Draw(Canvas canvas, Vertex selectedVertex1, Vertex selectedVertex2, BaseGraph graph, bool isCleancanvas = true)
        {
            List<List<List<int>>> adjacencyMatrix=GetAdjacencyMatrix(graph);
            List<Vertex> vertexs=GetVertexs(graph);
            if (isCleancanvas)
                canvas.Children.Clear();
            DrawEdge(canvas, adjacencyMatrix, vertexs);
            DrawVertex(canvas, vertexs);
            DrawSelectedVertex(canvas, selectedVertex1, Color.FromArgb(250, 255, 0, 0));
            DrawSelectedVertex(canvas, selectedVertex2, Color.FromArgb(250, 255, 0, 0));
        }


        public void DrawRoute(Canvas canvas, List<Vertex> vertexs, Color colorVertex, Brush brushesLine=null)
        {
            for (int i = 0; i < vertexs.Count - 1; i++)
            {
                Line line = new Line
                {
                    
                    X1 = vertexs[i].Point.X,
                    Y1 = vertexs[i].Point.Y,
                    X2 = vertexs[i + 1].Point.X,
                    Y2 = vertexs[i + 1].Point.Y
                };
                if (brushesLine == null)
                    line.Stroke = Brushes.Red;
                else
                    line.Stroke = brushesLine;
                line.StrokeThickness = 1;
                vertexs[i].Draw(canvas, colorVertex, Brushes.DarkBlue);


                canvas.Children.Add(line);
                
            }
            vertexs[vertexs.Count-1].Draw(canvas, colorVertex, Brushes.DarkBlue);

        }

        public void DrawSelectedVertex(Canvas canvas, Vertex vertex, Color colorVertex)
        {
            if (vertex != null)
                vertex.Draw(canvas, colorVertex, Brushes.DarkBlue);
            //
        }

        public List<List<List<int>>> GetAdjacencyMatrix(BaseGraph graph)
        {
            return graph.AdjacencyMatrix;
        }

        public List<Vertex> GetVertexs(BaseGraph graph)
        {
            return graph.VertexNames;
        }

        private void DrawArrow(Canvas canvas, double x1, double y1, double x2, double y2)
        {
            if (x1 == x2 || y1 == y2)
            {
                return;
            }
            double d = Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));

            double X = x2 - x1;
            double Y = y2 - y1;

            double X3 = x2 - (X / d) * 25;
            double Y3 = y2 - (Y / d) * 25;

            double Xp = y2 - y1;
            double Yp = x1 - x2;

            double X4 = X3 + (Xp / d) * 4;
            double Y4 = Y3 + (Yp / d) * 4;
            double X5 = X3 - (Xp / d) * 4;
            double Y5 = Y3 - (Yp / d) * 4;



            Line line = new Line
            {
                Stroke = Brushes.Green,
                X1 = x2 - (X / d) * 10,
                Y1 = y2 - (Y / d) * 10,
                X2 = X4,
                Y2 = Y4
            };
            line.StrokeThickness = 1;
            canvas.Children.Add(line);

            line = new Line
            {
                Stroke = Brushes.Green,
                X1 = x2 - (X / d) * 10,
                Y1 = y2 - (Y / d) * 10,
                X2 = X5,
                Y2 = Y5
            };
            line.StrokeThickness = 1;
            canvas.Children.Add(line);
        }

        private void DrawEdge(Canvas canvas, List<List<List<int>>> adjacencyMatrix, List<Vertex> vertexs)
        {
            {//i кто j куда
                for (var i = 0; i < adjacencyMatrix.Count; i++)
                    for (var j = 0; j < adjacencyMatrix.Count; j++)
                    {
                        for (int k = 0; k < adjacencyMatrix[i][j].Count; k++)
                        {
                            var value = adjacencyMatrix[i][j][k];
                            if (value != 0)
                            {
                                TextBlock textBlock = new TextBlock();
                                SolidColorBrush colorBrushText = new SolidColorBrush();
                                colorBrushText.Color = Color.FromArgb(250, 255, 0, 0);
                                textBlock.Foreground = colorBrushText;
                                if (i == j)
                                {
                                    textBlock.Text = "☆";
                                    Canvas.SetLeft(textBlock, vertexs[i].Point.X + 20);
                                    Canvas.SetTop(textBlock, vertexs[j].Point.Y - 25);
                                    canvas.Children.Add(textBlock);
                                    continue;
                                }



                                Point p1 = vertexs[i].Point, p3 = vertexs[j].Point, p2 = new Point();
                                //

                                double koef = ((int)k % 2 == 1) ? (16 * k) : (-8 * k);


                              

                                p2.X = p1.X + (p3.X - p1.X) / 2+koef;
                                p2.Y = p1.Y+(p3.Y - p1.Y) / 2 + koef;

                                double sign1 = Math.Sign(p3.X - p1.X);
                                double sign2 = Math.Sign(p3.Y - p1.Y);
                                if (sign1==sign2)
                                {
                                    p2.X = p1.X + (p3.X - p1.X) / 2 - koef;
                                }
                                System.Windows.Shapes.Path path = new System.Windows.Shapes.Path();
                                PathGeometry geometry = new PathGeometry();
                                PathFigure figure = new PathFigure();
                                figure.StartPoint = p1;
                                figure.Segments.Add(new QuadraticBezierSegment()
                                {
                                    Point1 = p2,
                                    Point2 = p3
                                });
                                geometry.Figures.Add(figure);
                                path.Data = geometry;
                                path.Stroke = Brushes.DimGray;
                                path.StrokeThickness = 1;
                                canvas.Children.Add(path);


                                DrawArrow(canvas, p2.X, p2.Y, p3.X - (p3.X - p2.X) / 5, p3.Y - (p3.Y - p2.Y) / 5);

                                textBlock.Text = value.ToString();

                                textBlock.FontSize = 14;

                                Canvas.SetLeft(textBlock, p2.X + (p3.X - p2.X) / 3);
                                Canvas.SetTop(textBlock, p2.Y + (p3.Y - p2.Y) / 3 - 5);

                                canvas.Children.Add(textBlock);
                            }
                        }
                    }


            }
        }
     

        private void DrawVertex(Canvas canvas, List<Vertex> vertexs)
        {
            foreach (var vertex in vertexs)
            {
                vertex.Draw(canvas, Color.FromArgb(215, 215, 215, 215));

            }
        }
    }
}
