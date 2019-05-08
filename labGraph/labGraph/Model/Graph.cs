
using labGraph.Model;
using labGraph.Model.InterfaceImplementation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace labGraph
{
    [Serializable]
    public sealed class Graph : ICloneable
    {
        #region Private field(list<vertex> and matrix adjacency
        private List<Vertex> _vertexNames;
        //1 list row
        //2 list column
        //3 list create for multiGraph
        private List<List<List<int>>> _adjacencyMatrix;
        [NonSerialized]
        private IDrawGraph _drawGraph;
        [NonSerialized]
        private ISaveGraphInFile _saveGraphinFile;
        [NonSerialized]
        private IOpenGraphFromFile _openGraphFromFile;
        [NonSerialized]
        private IGraphFunctionality _perfomLabsTask;
        #endregion


        public Graph()
        {
            _vertexNames = new List<Vertex>();
            _adjacencyMatrix = new List<List<List<int>>>();
            InitInterface();


        }
        public Graph(List<List<List<int>>> adjacencyMatrix, List<Vertex> vertexNames)
        {
            _vertexNames = vertexNames;
            _adjacencyMatrix = adjacencyMatrix;
            InitInterface();
        }
        public void InitInterface()
        {
            _drawGraph = new DrawGraph();
            _saveGraphinFile = new SaveGraphInFile();
            _openGraphFromFile = new OpenGraphFromFile();
            _perfomLabsTask = new GraphFunctionality();
        }

        #region open graph from file
        public bool OpenAdjacencyMatrixFile(string fileName, int sizeX, int sizeY)
        {
            Graph graphBackupp = (Graph)this.Clone();           
            if (!_openGraphFromFile.AdjacencyMatrixFile(fileName, sizeX, sizeY, new BaseGraph(_adjacencyMatrix,_vertexNames)))
            {
                _adjacencyMatrix = graphBackupp._adjacencyMatrix;
                _vertexNames = graphBackupp.VertexNames;
                return false;
            }
            return true;

        }

        public bool OpenIncidenceMatrixFile(string fileName, int sizeX, int sizeY)
        {
            Graph graphBackupp = (Graph)this.Clone();
            if (!_openGraphFromFile.IncidenceMatrixFile(fileName, sizeX, sizeY, new BaseGraph(_adjacencyMatrix, _vertexNames)))
            {
                _adjacencyMatrix = graphBackupp._adjacencyMatrix;
                _vertexNames = graphBackupp.VertexNames;
                return false;
            }
            return true;
        }

        public bool OpenVertexFile(string fileName, int sizeX, int sizeY)
        {
            Graph graphBackupp = (Graph)this.Clone();
            if (!_openGraphFromFile.VertexFile(fileName, new BaseGraph(_adjacencyMatrix, _vertexNames)))
            {
                _adjacencyMatrix = graphBackupp._adjacencyMatrix;
                _vertexNames = graphBackupp.VertexNames;
                return false;
            }
            return true;


        }

        public bool OpenEdgeFile(string fileName, int sizeX, int sizeY)
        {
            Graph graphBackupp = (Graph)this.Clone();
            if (!_openGraphFromFile.EdgeFile(fileName, sizeX, sizeY, new BaseGraph(_adjacencyMatrix, _vertexNames)))
            {
                _adjacencyMatrix = graphBackupp._adjacencyMatrix;
                _vertexNames = graphBackupp.VertexNames;
                return false;
            }
            return true;

        }
        #endregion

        #region save graph in file
        public void SaveAdjacencyMatrixFile(string fileName)
        {
            _saveGraphinFile.AdjacencyMatrixFile(fileName, new BaseGraph(_adjacencyMatrix, _vertexNames));
        }

        public void SaveIncidenceMatrixFile(string fileName)
        {
            _saveGraphinFile.IncidenceMatrixFile(fileName, new BaseGraph(_adjacencyMatrix, _vertexNames));
        }

        public void SaveVertexFile(string fileName)
        {
            _saveGraphinFile.VertexFile(fileName, new BaseGraph(_adjacencyMatrix, _vertexNames));
        }

        public void SaveEdgeFile(string fileName)
        {
            _saveGraphinFile.EdgeFile(fileName, new BaseGraph(_adjacencyMatrix, _vertexNames));
        }
       
        #endregion

       
     
        #region for exchange data with AplicationViewModel
        public DataTable AdjacencyMatrix
        {

            get
            {
                DataTable dataTable = new DataTable();
                foreach (var name in VertexNames)
                {
                    dataTable.Columns.Add(name.ToString(), Type.GetType("System.String"));
                }
                foreach (var data in _adjacencyMatrix)
                {
                    DataRow row = dataTable.NewRow();
                    var dataArray = data.ToArray();
                    object[] arr = new object[dataArray.Length];

                    for (int i = 0; i < dataArray.Length; i++)
                    {
                        string str = "";

                        foreach (var value in dataArray[i])
                        {
                            str += value.ToString() + ",";
                        }

                        arr[i] = str;
                    }

                    row.ItemArray = arr;

                    dataTable.Rows.Add(row);
                }

                return dataTable;
            }


        }

        public List<Vertex> VertexNames { get => _vertexNames; }
        public List<List<List<int>>> AdjacencyMatrix3DimensionalArray { get => _adjacencyMatrix; }
        public string SetValueCell(int inderRow, int indexColumn, string value)
        {
            _adjacencyMatrix[inderRow][indexColumn].Clear();
            var strs = value.Split(',');
            string answer = "";
            foreach (var str in strs)
            {
                if (str == "") continue;
                int val;
                try
                {
                    val = Convert.ToInt32(str);
                }
                catch (FormatException)
                {
                    answer = "При вводе в таблицу смежности были допущены ошибки ввода. Не все значения были распознаны.";
                    continue;
                }
                _adjacencyMatrix[inderRow][indexColumn].Add(val);
            }
            _adjacencyMatrix[inderRow][indexColumn].Sort();
            return answer;

        }
        #endregion


        #region work with vertex and Edge(Attention this code work with miltygraph

        public bool AddVertex(Vertex vertex)
        {
            foreach (var data in VertexNames)
            {
                if (data.ToString() == vertex.ToString())
                    return false;
            }
            VertexNames.Add(vertex);
            _adjacencyMatrix.Add(new List<List<int>>());
            for (int i = 0; i < _adjacencyMatrix.Count - 1; i++)
                _adjacencyMatrix[_adjacencyMatrix.Count - 1].Add(new List<int> { 0 });

            foreach (var t in _adjacencyMatrix)
            {
                t.Add(new List<int> { 0 });
            }

            return true;
        }


        public void AddEdge(Vertex vertex1, Vertex vertex2, int weight, int isOriented)
        {
            if (_adjacencyMatrix[VertexNames.IndexOf(vertex1)][VertexNames.IndexOf(vertex2)][0] == 0)
                _adjacencyMatrix[VertexNames.IndexOf(vertex1)][VertexNames.IndexOf(vertex2)][0] = weight;
            else
                _adjacencyMatrix[VertexNames.IndexOf(vertex1)][VertexNames.IndexOf(vertex2)].Add(weight);
            _adjacencyMatrix[VertexNames.IndexOf(vertex1)][VertexNames.IndexOf(vertex2)].Sort();
            if (isOriented == 0)
            {
                AddEdge(vertex2, vertex1, weight, 1);
            }

        }

        public void DeleteVertex(Vertex vertex)
        {
            var index = VertexNames.IndexOf(vertex);

            VertexNames.RemoveAt(index);
            _adjacencyMatrix.RemoveAt(index);
            foreach (var list in _adjacencyMatrix)
            {
                list.RemoveAt(index);
            }

        }

        public void MoveVertex(Vertex vertex, Point newPoint)
        {
            foreach (var ver in VertexNames)
            {
                if (ver == vertex)
                    ver.Point = newPoint;
            }
        }


        public Vertex FindVertex(Point point)
        {
            foreach (var ver in VertexNames)
            {

                if (ver.Point.X < point.X + 20 && point.X < (ver.Point.X + ver.SizeX / 4))
                    if (ver.Point.Y < point.Y + 20 && point.Y < (ver.Point.Y + ver.SizeY / 3))
                        return ver;
            }

            return null;
        }

        public Vertex FindVertex(string name)
        {
            foreach (var ver in VertexNames)
            {

                if (ver.ToString() == name)
                    return ver;
            }

            return null;
        }
        #endregion

        #region Drawing graph

        public void Draw(Canvas canvas, Vertex selectedVertex1, Vertex selectedVertex2, bool isCleancanvas = true)
        {
            _drawGraph.Draw(canvas, selectedVertex1, selectedVertex2, new BaseGraph(_adjacencyMatrix, _vertexNames), isCleancanvas);
        }

        public void DrawRoute(Canvas canvas, List<Vertex> vertexs,Color colorVertex, Brush brushesLine = null)
        {
            _drawGraph.DrawRoute(canvas, vertexs, colorVertex, brushesLine);
        }

        public void DrawSelectedVertex(Canvas canvas, Vertex vertex, Color colorVertex)
        {
            _drawGraph.DrawSelectedVertex(canvas,vertex,colorVertex);
        }

       
       
        #endregion

        #region Implimentation interface
        public object Clone()
        {
            Graph graphClone = new Graph();

            foreach (var vertex in VertexNames)
            {
                graphClone.VertexNames.Add((Vertex)vertex.Clone());
            }
            for (int i = 0; i < _adjacencyMatrix.Count; i++)
            {
                graphClone._adjacencyMatrix.Add(new List<List<int>>());

                for (int j = 0; j < _adjacencyMatrix[i].Count; j++)
                {
                    graphClone._adjacencyMatrix[i].Add(new List<int>());
                    graphClone._adjacencyMatrix[i][j] = new List<int>(_adjacencyMatrix[i][j]);

                }
            }

            return graphClone;
        }
        #endregion
       
        #region Methods for labs


        public List<List<Vertex>> BreadthFirstSearch(Vertex startVertex)//2 lab
        {
            return _perfomLabsTask.BreadthFirstSearch(startVertex, new BaseGraph(_adjacencyMatrix, _vertexNames));

        }

        public List<List<Vertex>> BestFirstSearch(Vertex startVertex)//3 lab
        {
            return _perfomLabsTask.BestFirstSearch(startVertex, new BaseGraph(_adjacencyMatrix, _vertexNames));
        }

        public List<List<Vertex>> DijkstraAlgorithm(Vertex startVertex)//4 lab
        {
            return _perfomLabsTask.DijkstraAlgorithm(startVertex, new BaseGraph(_adjacencyMatrix, _vertexNames));
        }

        public List<Vertex> AStarAlgorithm(Vertex startVertex, Vertex endVertex)//4 lab
        {
            return _perfomLabsTask.AStarAlgorithm(startVertex, endVertex, new BaseGraph(_adjacencyMatrix, _vertexNames));
        }
        //lab6
        public List<Vertex> GetVertexWeights()
        {
            return _perfomLabsTask.GetVertexWeights(new BaseGraph(_adjacencyMatrix, _vertexNames));
        }

        public int SearchDiameter()
        {
            return _perfomLabsTask.SearchDiameter(new BaseGraph(_adjacencyMatrix, _vertexNames));
        }

        public int SearchRadius()
        {
            return _perfomLabsTask.SearchRadius(new BaseGraph(_adjacencyMatrix, _vertexNames));
        }

        public List<Vertex> GetDegreeVertexGrapf()
        {
            return _perfomLabsTask.GetDegreeVertexGrapf(new BaseGraph(_adjacencyMatrix, _vertexNames));
        }

        public bool IsIsomorphism(Graph gr)
        {
            return _perfomLabsTask.IsIsomorphism(new BaseGraph(_adjacencyMatrix, _vertexNames), new BaseGraph(gr._adjacencyMatrix,gr._vertexNames));
        }

        public bool IsFullGraph()
        {
            return _perfomLabsTask.IsFullGraph(new BaseGraph(_adjacencyMatrix, _vertexNames));
        }

        public Graph GetAdditionGraph()
        {
            return _perfomLabsTask.GetAdditionGraph(new BaseGraph(_adjacencyMatrix, _vertexNames));
        }

        public int FindChromaticNumber(Canvas canvas)
        {
            return _perfomLabsTask.FindChromaticNumber(canvas,new BaseGraph(_adjacencyMatrix, _vertexNames));
        }
        
        #endregion
    }
}

