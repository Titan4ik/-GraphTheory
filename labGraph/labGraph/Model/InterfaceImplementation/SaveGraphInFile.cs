using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace labGraph.Model.InterfaceImplementation
{

    public class SaveGraphInFile : ISaveGraphInFile
    {
        public void AdjacencyMatrixFile(string fileName, BaseGraph graph)
        {
            List<List<List<int>>> adjacencyMatrix = GetAdjacencyMatrix(graph);
            List<Vertex> vertexs = GetVertexs(graph);
            using (StreamWriter sw = new StreamWriter(fileName, false, System.Text.Encoding.Default))
            {
                sw.WriteLine(vertexs.Count.ToString());
                for (int i = 0; i < vertexs.Count; i++)
                {
                    for (int j = 0; j < vertexs.Count; j++)
                    {
                        string str = "";
                        foreach (var value in adjacencyMatrix[i][j])
                        {
                            str += value.ToString() + ',';
                        }
                        sw.Write(str + ' ');
                    }
                    sw.WriteLine();
                }
            }
        }

        public void IncidenceMatrixFile(string fileName, BaseGraph graph)
        {
            List<List<List<int>>> adjacencyMatrix = GetAdjacencyMatrix(graph);
            var incidenceMatrix = ConverterMatrixFormatGraph.AdjacencyToIncidenceMatrix(adjacencyMatrix);
            using (StreamWriter sw = new StreamWriter(fileName, false, System.Text.Encoding.Default))
            {
                sw.WriteLine(incidenceMatrix.Count.ToString());
                if (incidenceMatrix.Count != 0)
                    sw.WriteLine(incidenceMatrix[0].Count.ToString());
                else
                    sw.WriteLine("0");
                for (int i = 0; i < incidenceMatrix.Count; i++)
                {
                    for (int j = 0; j < incidenceMatrix[i].Count; j++)
                    {
                        string str = "";
                        foreach (var value in incidenceMatrix[i][j])
                        {
                            str += value.ToString() + ',';
                        }
                        sw.Write(str + ' ');
                    }
                    sw.WriteLine();
                }
            }
        }

        public void EdgeFile(string fileName, BaseGraph graph)
        {
            List<List<List<int>>> adjacencyMatrix = GetAdjacencyMatrix(graph);
            List<Vertex> vertexs = GetVertexs(graph);
            using (StreamWriter sw = new StreamWriter(fileName, false, System.Text.Encoding.Default))
            {
                int count = 0;
                for (int i = 0; i < vertexs.Count; i++)
                {
                    for (int j = 0; j < vertexs.Count; j++)
                    {
                        foreach (var value in adjacencyMatrix[i][j])
                        {
                            if (value != 0)
                            {
                                string str = count.ToString() + "(" + value.ToString() + "," + vertexs[i].ToString() + "," + vertexs[j].ToString() + ",1) ";
                                sw.Write(str);
                                count++;
                            }

                        }

                    }
                }

            }
        }

        public void VertexFile(string fileName, BaseGraph graph)
        {
            List<Vertex> vertexs = GetVertexs(graph);
            using (StreamWriter sw = new StreamWriter(fileName, false, System.Text.Encoding.Default))
            {

                foreach (var value in vertexs)
                {
                    string str;
                    str = value.ToString() + "(" + value.Point.X + "," + value.Point.Y + ") ";
                    sw.Write(str);
                }



            }
        }
        public List<List<List<int>>> GetAdjacencyMatrix(BaseGraph graph)
        {
            return graph.AdjacencyMatrix;
        }

        public List<Vertex> GetVertexs(BaseGraph graph)
        {
            return graph.VertexNames;
        }
        public static void SerializableGraphFile(string fileName, Graph graph)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            // получаем поток, куда будем записывать сериализованный объект
            using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, graph);

            }
        }

   
        public static void PictureGraph(string fileName, Canvas canvas)
        {
            if (canvas == null)
                return;

            Rect bounds = VisualTreeHelper.GetDescendantBounds(canvas);

            RenderTargetBitmap rtb = new RenderTargetBitmap((Int32)bounds.Width, (Int32)bounds.Height, 96, 96, PixelFormats.Pbgra32);

            DrawingVisual dv = new DrawingVisual();

            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(canvas);
                dc.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
            }

            rtb.Render(dv);

            PngBitmapEncoder png = new PngBitmapEncoder();

            png.Frames.Add(BitmapFrame.Create(rtb));

            using (Stream stm = File.Create(fileName))
            {
                png.Save(stm);
            }
        }
    }
}
