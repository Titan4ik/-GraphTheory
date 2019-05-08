using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace labGraph.Model
{
    public class BaseGraph
    {
        private List<Vertex> _vertexNames;   
        private List<List<List<int>>> _adjacencyMatrix;
        public BaseGraph(List<List<List<int>>> adjacencyMatrix,List<Vertex> vertexNames)
        {
            VertexNames = vertexNames;
            AdjacencyMatrix = adjacencyMatrix;
        }

        public List<Vertex> VertexNames { get => _vertexNames; set => _vertexNames = value; }
        public List<List<List<int>>> AdjacencyMatrix { get => _adjacencyMatrix; set => _adjacencyMatrix = value; }
    }
}
