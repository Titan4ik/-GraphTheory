using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;

namespace labGraph.Model.InterfaceImplementation
{

    public class OpenGraphFromFile : IOpenGraphFromFile
    {
        public bool AdjacencyMatrixFile(string fileName, int sizeX, int sizeY, BaseGraph graph)
        {
            List<List<List<int>>> adjacencyMatrix = GetAdjacencyMatrix(graph);
            List<Vertex> vertexs = GetVertexs(graph);
            try
            {
                using (StreamReader sr = new StreamReader(fileName))
                {

                    string line = "", lineTemp = "";
                    while ((lineTemp = sr.ReadLine()) != null)
                    {
                        if (lineTemp[0] != '%') line += lineTemp + ' ';
                    }
                    char[] delimiterChars = { ' ', '\n', '\0', '\r' };
                    var values = line.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);


                    int count = Int32.Parse(values[0]);

                    if (vertexs.Count < count)
                    {

                        Random rand = new Random();

                        while (count != vertexs.Count)
                        {
                            Vertex vertex;
                            do
                            {
                                string name = rand.Next(100, 10000).ToString();
                                vertex = new Vertex(name, new Point(rand.Next(sizeX), rand.Next(sizeY)));
                            } while (!AddVertex(vertex, graph));


                        }
                    }
                    for (int i = 0; i < count; i++)
                        for (int j = 0; j < count; j++)
                        {
                            List<int> temp = new List<int>();
                            char[] delchar = { ',' };
                            foreach (string str in values[i * count + j + 1].Split(delchar, StringSplitOptions.RemoveEmptyEntries))
                            {
                                temp.Add(Int32.Parse(str));
                            }
                            adjacencyMatrix[i][j] = temp;
                        }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool IncidenceMatrixFile(string fileName, int sizeX, int sizeY, BaseGraph graph)
        {
            List<List<List<int>>> adjacencyMatrix = GetAdjacencyMatrix(graph);
            List<Vertex> vertexs = GetVertexs(graph);

            try
            {
                using (StreamReader sr = new StreamReader(fileName))
                {

                    string line = "", lineTemp = "";
                    while ((lineTemp = sr.ReadLine()) != null)
                    {
                        if (lineTemp[0] != '%') line += lineTemp + ' ';
                    }
                    char[] delimiterChars = { ' ', '\n', '\0', '\r' };
                    var values = line.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);


                    int count1 = Int32.Parse(values[0]);
                    int count2 = Int32.Parse(values[1]);
                    if (vertexs.Count < count1)
                    {

                        Random rand = new Random();

                        while (count1 != vertexs.Count)
                        {
                            Vertex vertex;
                            do
                            {
                                string name = rand.Next(100, 10000).ToString();
                                vertex = new Vertex(name, new Point(rand.Next(sizeX), rand.Next(sizeY)));
                            } while (!AddVertex(vertex, graph));


                        }
                    }


                    //
                    var incidenceMatrix = new List<List<List<int>>>();
                    for (int i = 0; i < count1; i++)
                    {
                        incidenceMatrix.Add(new List<List<int>>());
                        for (int j = 0; j < count2; j++)
                        {
                            incidenceMatrix[i].Add(new List<int>());
                        }
                    }



                    for (var i = 0; i < count1; i++)
                        for (var j = 0; j < count2; j++)
                        {
                            List<int> temp = new List<int>();
                            char[] delchar = { ',' };
                            foreach (string str in values[i * count2 + j + 2].Split(delchar, StringSplitOptions.RemoveEmptyEntries))
                            {

                                temp.Add(Int32.Parse(str));

                            }
                            incidenceMatrix[i][j] = temp;

                        }
                    List<List<List<int>>> adjacencyMatrixTemp;
                    adjacencyMatrixTemp = ConverterMatrixFormatGraph.IncidenceToAdjacencyMatrix(incidenceMatrix);
                    for (int i = 0; i < adjacencyMatrixTemp.Count; i++)
                        for (int j = 0; j < adjacencyMatrixTemp.Count; j++)
                        {
                            adjacencyMatrix[i][j] = adjacencyMatrixTemp[i][j];
                        }


                    //

                }
            }
            catch (Exception)
            {

                return false;
            }

            return true;
        }

        public bool EdgeFile(string fileName, int sizeX, int sizeY, BaseGraph graph)
        {
            try
            {
                using (StreamReader sr = new StreamReader(fileName))
                {

                    string line = "", lineTemp = "";
                    while ((lineTemp = sr.ReadLine()) != null)
                    {
                        if (lineTemp[0] != '%') line += lineTemp + ' ';
                    }
                    char[] delimiterChars = { ' ', '\n', '\0', '\r' };
                    var values = line.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
                    Random rand = new Random();
                    foreach (string ver in values)
                    {
                        char[] delChars = { '(', ',', ')' };
                        var parts = ver.Split(delChars, StringSplitOptions.RemoveEmptyEntries);
                        Vertex vertex1 = FindVertex(parts[2],graph);
                        Vertex vertex2 = FindVertex(parts[3], graph);

                        if (vertex1 == null)
                        {

                            vertex1 = new Vertex(parts[2], new Point(rand.Next(sizeX), rand.Next(sizeY)));
                            AddVertex(vertex1, graph);
                        }
                        if (vertex2 == null)
                        {

                            vertex2 = new Vertex(parts[3], new Point(rand.Next(sizeX), rand.Next(sizeY)));
                            AddVertex(vertex2, graph);
                        }
                        int weight = Int32.Parse(parts[1]);
                        int isOriented = Int32.Parse(parts[4]);
                        AddEdge(vertex1, vertex2, weight, isOriented, graph);
                    }
                }
            }
            catch (Exception)
            {

                return false;
            }
            return true;
        }

        public bool VertexFile(string fileName, BaseGraph graph)
        {
            try
            {
                using (StreamReader sr = new StreamReader(fileName))
                {

                    string line = "", lineTemp = "";
                    while ((lineTemp = sr.ReadLine()) != null)
                    {
                        if (lineTemp[0] != '%') line += lineTemp + ' ';
                    }
                    char[] delimiterChars = { ' ', '\n', '\0', '\r' };
                    var values = line.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string ver in values)
                    {
                        char[] delChars = { '(', ',', ')' };
                        var parts = ver.Split(delChars, StringSplitOptions.RemoveEmptyEntries);
                        AddVertex(new Vertex(parts[0], new Point(Int32.Parse(parts[1]), (Int32.Parse(parts[2])))),graph);
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public List<List<List<int>>> GetAdjacencyMatrix(BaseGraph graph)
        {
            return graph.AdjacencyMatrix;
        }

        public List<Vertex> GetVertexs(BaseGraph graph)
        {
            return graph.VertexNames;
        }

        static public Graph SerializableGraphFile(string fileName)
        {
            Graph gr = null;
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
                {
                    gr = (Graph)formatter.Deserialize(fs);
                }

            }
            catch (Exception)
            {
                return null;

            }
            gr.InitInterface();
            return gr;
        }


        private bool AddVertex(Vertex vertex, BaseGraph graph)
        {
            foreach (var data in graph.VertexNames)
            {
                if (data.ToString() == vertex.ToString())
                    return false;
            }
            graph.VertexNames.Add(vertex);
            graph.AdjacencyMatrix.Add(new List<List<int>>());
            for (int i = 0; i < graph.AdjacencyMatrix.Count - 1; i++)
                graph.AdjacencyMatrix[graph.AdjacencyMatrix.Count - 1].Add(new List<int> { 0 });

            foreach (var t in graph.AdjacencyMatrix)
            {
                t.Add(new List<int> { 0 });
            }

            return true;
        }
        private void AddEdge(Vertex vertex1, Vertex vertex2, int weight, int isOriented, BaseGraph graph)
        {
            if (graph.AdjacencyMatrix[graph.VertexNames.IndexOf(vertex1)][graph.VertexNames.IndexOf(vertex2)][0] == 0)
                graph.AdjacencyMatrix[graph.VertexNames.IndexOf(vertex1)][graph.VertexNames.IndexOf(vertex2)][0] = weight;
            else
                graph.AdjacencyMatrix[graph.VertexNames.IndexOf(vertex1)][graph.VertexNames.IndexOf(vertex2)].Add(weight);
            graph.AdjacencyMatrix[graph.VertexNames.IndexOf(vertex1)][graph.VertexNames.IndexOf(vertex2)].Sort();
            if (isOriented == 0)
            {
                AddEdge(vertex2, vertex1, weight, 1, graph);
            }

        }
        private Vertex FindVertex(string name, BaseGraph graph)
        {
            foreach (var ver in graph.VertexNames)
            {

                if (ver.ToString() == name)
                    return ver;
            }

            return null;
        }
    }
}
