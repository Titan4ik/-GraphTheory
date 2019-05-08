using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace labGraph.Model.InterfaceImplementation
{

    public class GraphFunctionality : IGraphFunctionality
    {
        public List<List<List<int>>> GetAdjacencyMatrix(BaseGraph graph)
        {
            return graph.AdjacencyMatrix;
        }

        public List<Vertex> GetVertexs(BaseGraph graph)
        {
             return graph.VertexNames;
        }


        public static string GetVectorRoutes(List<List<Vertex>> routes)
        {
            string answer = "";
            for (int i = 0; i < routes.Count; i++)
            {
                if (routes[i].Count > 0)
                    answer += routes[i][routes[i].Count - 1].Weight.ToString() + " ";
                else
                    answer += "- ";
            }
            return answer;
        }

        public List<List<Vertex>> BreadthFirstSearch(Vertex startVertex, BaseGraph graph)
        {
            List<List<List<int>>> adjacencyMatrix=GetAdjacencyMatrix(graph);
            List<Vertex> vertexs=GetVertexs(graph);
            List<List<Vertex>> path = new List<List<Vertex>>();
            // List<Vertex> path2 = new List<Vertex>();
            Queue<Vertex> queue = new Queue<Vertex>();    //Это очередь, хранящая номера вершин
            bool[] used = new bool[vertexs.Count];
            for (int i = 0; i < used.Length; i++)
                used[i] = false;

            Vertex current;
            Vertex temp = (Vertex)startVertex.Clone();
            temp.Weight = 0;
            queue.Enqueue(temp);
            for (int i = 0; i < used.Length; i++)
                path.Add(new List<Vertex>());






            while (queue.Count != 0)
            {
                current = queue.Peek();
                queue.Dequeue();
                int pos = vertexs.IndexOf(current);
                if (used[pos] == true) continue;
                used[pos] = true;


                path[vertexs.IndexOf(current)].Add(current);

                for (int i = 0; i < adjacencyMatrix.Count; i++)
                {

                    if (Convert.ToBoolean(adjacencyMatrix[pos][i][0]))
                    {
                        temp = (Vertex)vertexs[i].Clone();
                        if (!used[i])
                        {


                            temp.Weight = current.Weight + 1;
                            queue.Enqueue(temp);
                            if (path[vertexs.IndexOf(temp)].Count == 0)
                                path[vertexs.IndexOf(temp)].Add(current);
                        }
                    }
                }

            }


            for (int i = 0; i < path.Count; i++)
            {
                if (path[i].Count == 0) continue;
                if (path[i][0] == startVertex) continue;
                while (path[i][0] != startVertex)
                {
                    int pos = vertexs.IndexOf(path[i][0]);
                    for (int j = path[pos].Count - 2; j >= 0; j--)
                    {
                        path[i].Insert(0, path[pos][j]);
                    }
                }

            }

            return path;


        }

        public List<List<Vertex>> BestFirstSearch(Vertex startVertex, BaseGraph graph)
        {
            List<List<List<int>>> adjacencyMatrix = GetAdjacencyMatrix(graph);
            List<Vertex> vertexs = GetVertexs(graph);

            SortedDictionary<int, List<Vertex>> priorityQueue = new SortedDictionary<int, List<Vertex>>();
            List<List<Vertex>> path = new List<List<Vertex>>();

            bool[] used = new bool[vertexs.Count];
            for (int i = 0; i < used.Length; i++)
                used[i] = false;

            Vertex current;
            Vertex temp = (Vertex)startVertex.Clone();
            temp.Weight = 0;
            priorityQueue.Add(0, new List<Vertex>());
            temp.Weight = 0;
            priorityQueue[0].Add(temp);

            for (int i = 0; i < used.Length; i++)
                path.Add(new List<Vertex>());


            while (priorityQueue.Count != 0)
            {
                current = priorityQueue[priorityQueue.Keys.Min()].First();

                priorityQueue[priorityQueue.Keys.Min()].RemoveAt(0);
                if (priorityQueue[priorityQueue.Keys.Min()].Count == 0)
                    priorityQueue.Remove(priorityQueue.Keys.Min());
                int pos = vertexs.IndexOf(current);
                if (used[pos] == true) continue;
                used[pos] = true;
                path[vertexs.IndexOf(current)].Add(current);

                for (int i = 0; i < adjacencyMatrix.Count; i++)
                {
                    if (Convert.ToBoolean(adjacencyMatrix[pos][i][0]))
                    {
                        temp = (Vertex)vertexs[i].Clone();
                        if (!used[i])
                        {
                            temp.Weight = current.Weight + adjacencyMatrix[pos][i][0];

                            if (!priorityQueue.ContainsKey(adjacencyMatrix[pos][i][0]))
                                priorityQueue.Add(adjacencyMatrix[pos][i][0], new List<Vertex>());
                            priorityQueue[adjacencyMatrix[pos][i][0]].Insert(0, temp);

                            if (path[vertexs.IndexOf(temp)].Count == 0)
                            {
                                path[vertexs.IndexOf(temp)].Add(current);

                            }

                        }
                    }
                }

            }
            //если не найдено вершщины
            for (int i = 0; i < path.Count; i++)
            {
                if (path[i].Count == 0) continue;
                if (path[i][0] == startVertex) continue;
                while (path[i][0] != startVertex)
                {
                    int pos = vertexs.IndexOf(path[i][0]);
                    for (int j = path[pos].Count - 2; j >= 0; j--)
                    {
                        path[i].Insert(0, path[pos][j]);
                    }
                }

            }
            return path;
        }

        public List<List<Vertex>> DijkstraAlgorithm(Vertex startVertex, BaseGraph graph)
        {
            List<List<List<int>>> adjacencyMatrix = GetAdjacencyMatrix(graph);
            List<Vertex> vertexs = GetVertexs(graph);

            List<List<Vertex>> path = new List<List<Vertex>>();
            List<int>[] matrixDijkstra = new List<int>[vertexs.Count];


            for (int i = 0; i < vertexs.Count; i++)
            {

                path.Add(new List<Vertex>());
                matrixDijkstra[i] = new List<int> { Int32.MaxValue };
            }
            matrixDijkstra[vertexs.IndexOf(startVertex)][0] = 0;

            for (int k = 0; k < vertexs.Count; k++)
            {
                //поиск минимального
                int posMin = Int32.MinValue, minValue = Int32.MaxValue;
                for (int i = 0; i < matrixDijkstra.Length; i++)
                {
                    var countVertex = matrixDijkstra[i].Count - 1;
                    if (matrixDijkstra[i][countVertex] != Int32.MinValue && matrixDijkstra[i][countVertex] < minValue)
                    {
                        minValue = matrixDijkstra[i][countVertex];
                        posMin = i;
                    }
                }
                if (posMin == Int32.MinValue) break;
                matrixDijkstra[posMin].Add(Int32.MinValue);
                path[posMin].Add(vertexs[posMin]);
                //прозод по мтарице смежности
                for (int i = 0; i < adjacencyMatrix.Count; i++)
                {
                    if (Convert.ToBoolean(adjacencyMatrix[posMin][i][0]))
                    {
                        int valueMatDij = matrixDijkstra[i][matrixDijkstra[i].Count - 1];
                        if (valueMatDij == Int32.MinValue) continue;
                        int newValue = adjacencyMatrix[posMin][i][0] + matrixDijkstra[posMin][matrixDijkstra[posMin].Count - 2];
                        if (valueMatDij > newValue)
                        {
                            matrixDijkstra[i].Add(newValue);

                            path[i].Clear();
                            path[i].Add(vertexs[posMin]);

                        }

                    }
                }
            }


            for (int i = 0; i < path.Count; i++)
            {
                if (matrixDijkstra[i].Count == 1)
                {
                    path[i].Clear();
                }
                else
                    path[i][path[i].Count - 1].Weight = matrixDijkstra[i][matrixDijkstra[i].Count - 2];
                if (path[i].Count == 0) continue;
                if (path[i][0] == startVertex) continue;
                while (path[i][0] != startVertex)
                {
                    int pos = vertexs.IndexOf(path[i][0]);
                    for (int j = path[pos].Count - 2; j >= 0; j--)
                    {
                        path[i].Insert(0, path[pos][j]);
                    }
                }

            }

            return path;
        }

        private int GetLengthBetweenVertex(Vertex startVertex, Vertex endVertex)
        {
            return (int)Math.Sqrt(Math.Pow(endVertex.Point.X - startVertex.Point.X, 2) + Math.Pow(endVertex.Point.Y - startVertex.Point.Y, 2));
        }
        
      
        public List<Vertex> AStarAlgorithm(Vertex startVertex, Vertex endVertex, BaseGraph graph)
        {
            List<List<List<int>>> adjacencyMatrix = GetAdjacencyMatrix(graph);
            List<Vertex> vertexs = GetVertexs(graph);

            SortedDictionary<int, List<Vertex>> priorityQueue = new SortedDictionary<int, List<Vertex>>();
            List<List<Vertex>> path = new List<List<Vertex>>();

            bool[] used = new bool[vertexs.Count];
            for (int i = 0; i < used.Length; i++)
                used[i] = false;

            Vertex current;
            Vertex temp = (Vertex)startVertex.Clone();
            temp.Weight = 0;
            priorityQueue.Add(0, new List<Vertex>());
            temp.Weight = 0;
            priorityQueue[0].Add(temp);

            for (int i = 0; i < used.Length; i++)
                path.Add(new List<Vertex>());


            while (priorityQueue.Count != 0)
            {
                current = priorityQueue[priorityQueue.Keys.Min()].First();

                priorityQueue[priorityQueue.Keys.Min()].RemoveAt(0);
                if (priorityQueue[priorityQueue.Keys.Min()].Count == 0)
                    priorityQueue.Remove(priorityQueue.Keys.Min());
                int pos = vertexs.IndexOf(current);
                if (used[pos] == true) continue;
                used[pos] = true;
                path[vertexs.IndexOf(current)].Add(current);

                for (int i = 0; i < adjacencyMatrix.Count; i++)
                {
                    if (Convert.ToBoolean(adjacencyMatrix[pos][i][0]))
                    {
                        temp = (Vertex)vertexs[i].Clone();
                        if (!used[i])
                        {
                            temp.Weight = current.Weight + adjacencyMatrix[pos][i][0];
                            int key = GetLengthBetweenVertex(current, temp) + GetLengthBetweenVertex(temp, endVertex);


                            if (!priorityQueue.ContainsKey(key))
                                priorityQueue.Add(key, new List<Vertex>());

                            priorityQueue[key].Insert(0, temp);

                            if (path[vertexs.IndexOf(temp)].Count == 0)
                            {
                                path[vertexs.IndexOf(temp)].Add(current);

                            }

                        }
                    }
                }

            }
            //если не найдено вершщины
            List<Vertex> pathToEndVertex = new List<Vertex>();
            int indexEndVertex = vertexs.IndexOf(endVertex);
            if(path[indexEndVertex].Count>0)   
                while (path[indexEndVertex][0] != startVertex)
                {
                    int pos = vertexs.IndexOf(path[indexEndVertex][0]);
                    for (int j = path[pos].Count - 2; j >= 0; j--)
                    {
                        path[indexEndVertex].Insert(0, path[pos][j]);
                    }
                }

            pathToEndVertex = path[indexEndVertex];
            return pathToEndVertex;
        }

        public List<Vertex> GetVertexWeights(BaseGraph graph)
        {
            List<Vertex> vertexs = GetVertexs(graph);

            List<Vertex> answer = new List<Vertex>();


            foreach (var ver in vertexs)
            {

                List<List<Vertex>> routes;
                routes = DijkstraAlgorithm(ver,graph);

                int maxWeight = -1;
                foreach (var route in routes)
                {
                    if (route.Count > 0)
                        if (route[route.Count - 1].Weight > maxWeight)
                            maxWeight = route[route.Count - 1].Weight;


                }
                var temp = (Vertex)ver.Clone();
                temp.Weight = maxWeight;
                answer.Add(temp);

            }
            return answer;
        }

        public int SearchDiameter(BaseGraph graph)
        {
            List<Vertex> vertexWeights = GetVertexWeights(graph);
            int maxWeight = 0;
            if (vertexWeights.Count == 0)
                return 0;
            foreach (var ver in vertexWeights)
            {
                if (ver.Weight > maxWeight)
                    maxWeight = ver.Weight;
            }
            return maxWeight;
        }

        public int SearchRadius(BaseGraph graph)
        {

            List<Vertex> vertexWeights = GetVertexWeights(graph);
            int minWeight = Int32.MaxValue;
            if (vertexWeights.Count ==0)
                return 0;
            foreach (var ver in vertexWeights)
            {
                if (ver.Weight < minWeight)
                    minWeight = ver.Weight;
            }
            
            return minWeight;
        }

        public List<Vertex> GetDegreeVertexGrapf(BaseGraph graph)
        {
            List<List<List<int>>> adjacencyMatrix = GetAdjacencyMatrix(graph);
            List<Vertex> vertexs = GetVertexs(graph);

            List<Vertex> degreeVertexGrapf = new List<Vertex>();


            for (int i = 0; i < adjacencyMatrix.Count; i++)
            {
                degreeVertexGrapf.Add((Vertex)vertexs[i].Clone());
                degreeVertexGrapf[i].Deegre = 0;
                foreach (var Vertex in adjacencyMatrix[i])
                {
                    if (Convert.ToBoolean(Vertex[0]))
                    {
                        degreeVertexGrapf[i].Deegre += 1;
                    }
                }
            }


            return degreeVertexGrapf;

        }

        public bool IsIsomorphism(BaseGraph graph1, BaseGraph graph2)
        {
            var list1 = GetDegreeVertexGrapf(graph1);
            var list2 = GetDegreeVertexGrapf(graph2);
            List<int> listDegree1 = new List<int>();
            List<int> listDegree2 = new List<int>();
            foreach (var ver in list1)
            {
                listDegree1.Add(ver.Deegre);
            }
            foreach (var ver in list2)
            {
                listDegree2.Add(ver.Deegre);
            }
            foreach (var deg in listDegree1)
            {
                if (listDegree2.Exists(x => x == deg))
                {
                    listDegree2.Remove(deg);
                }
                else
                {
                    return false;
                }
            }
            if (listDegree2.Count > 0)
                return false;
            else
                return true;
        }

        public bool IsFullGraph(BaseGraph graph)
        {
            List<List<List<int>>> adjacencyMatrix = GetAdjacencyMatrix(graph);

            if (adjacencyMatrix.Count == 0)
                return false;

            for (int i = 0; i < adjacencyMatrix.Count; i++)
                for (int j = 0; j < adjacencyMatrix[i].Count; j++)
                {
                    if (i == j) continue;
                    if (!Convert.ToBoolean(adjacencyMatrix[i][j][0]))
                    {
                        return false;
                    }
                }          
            return true;
        }

        public Graph GetAdditionGraph(BaseGraph graph)
        {
            
            List<List<List<int>>> adjacencyMatrix = GetAdjacencyMatrix(graph);
            Random rand = new Random();
            for (int i = 0; i < adjacencyMatrix.Count; i++)
                for (int j = 0; j < adjacencyMatrix[i].Count; j++)
                {
                    if (i == j) continue;
                    if (Convert.ToBoolean(adjacencyMatrix[i][j][0]))
                    {
                        adjacencyMatrix[i][j].Clear();
                        adjacencyMatrix[i][j].Add(0);
                    }
                    else
                    {
                        adjacencyMatrix[i][j].Clear();
                        adjacencyMatrix[i][j].Add(rand.Next(5, 50));
                    }
                }

            return new Graph(GetAdjacencyMatrix(graph), GetVertexs(graph));
        }

        public int FindChromaticNumber(Canvas canvas, BaseGraph graph)
        {
            List<Vertex> vertexs = GetVertexs(graph);
            List<List<List<int>>> adjacencyMatrix = GetAdjacencyMatrix(graph);

            var listDegree = GetDegreeVertexGrapf(graph);
            List<Vertex> ShadedVertex = new List<Vertex>();
            int colorsARGB = 50000;
            
            /*foreach (var v in listDegree)
            {
                Console.WriteLine(v.ToString() + " " + v.Deegre);
            }
           Console.WriteLine("all");*/
            listDegree.Sort(delegate (Vertex v1, Vertex v2)
            {
                return -1*v1.Deegre.CompareTo(v2.Deegre);
            });
            bool flag = false;
            while (listDegree.Count > 0)
            {
                byte[] BytesColor = BitConverter.GetBytes(colorsARGB);
                foreach (var ver in listDegree)
                {
                    for (int i = 0; i < adjacencyMatrix.Count; i++)
                    {
                        if (adjacencyMatrix[vertexs.IndexOf(ver)][i][0] != 0)
                        {
                            if (ShadedVertex.Contains(vertexs[i]))
                            {
                                flag = true;
                                break;
                            }

                        }
                        if (adjacencyMatrix[i][vertexs.IndexOf(ver)][0] != 0)
                        {
                            if (ShadedVertex.Contains(vertexs[i]))
                            {
                                flag = true;
                                break;
                            }

                        }
                    }
                    
                    if (flag == false)
                    {
                        ShadedVertex.Add(ver);
                        
                        
                    }
                    else
                    {
                        flag = false;
                    }

                }
                foreach (var ver in ShadedVertex)
                {
                    ver.Draw(canvas, Color.FromArgb(BytesColor[0], BytesColor[1], BytesColor[2], BytesColor[3]));
                    listDegree.Remove(ver);
                }
                ShadedVertex.Clear();
                colorsARGB += 50000;
            }

            
            return colorsARGB/50000-1;
        }
    }
}
