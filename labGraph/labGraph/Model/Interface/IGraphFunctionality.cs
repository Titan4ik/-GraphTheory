using System.Collections.Generic;
using System.Windows.Controls;

namespace labGraph.Model
{
    interface IGraphFunctionality
    {
        List<List<List<int>>> GetAdjacencyMatrix(BaseGraph graph);
        List<Vertex> GetVertexs(BaseGraph graph);    

        List<List<Vertex>> BreadthFirstSearch(Vertex startVertex, BaseGraph graph);

        List<List<Vertex>> BestFirstSearch(Vertex startVertex, BaseGraph graph);

        List<List<Vertex>> DijkstraAlgorithm(Vertex startVertex, BaseGraph graph);

        List<Vertex> AStarAlgorithm(Vertex startVertex, Vertex endVertex, BaseGraph graph);

        List<Vertex> GetVertexWeights(BaseGraph graph);

        int SearchDiameter(BaseGraph graph);

        int SearchRadius(BaseGraph graph);

        List<Vertex> GetDegreeVertexGrapf(BaseGraph graph);

        bool IsIsomorphism(BaseGraph graph1, BaseGraph graph2);

        bool IsFullGraph(BaseGraph graph);

        Graph GetAdditionGraph(BaseGraph graph);

        int FindChromaticNumber(Canvas canvas, BaseGraph graph);

    }
}
