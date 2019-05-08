using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace labGraph.Model
{
    interface IDrawGraph
    {
        void Draw(Canvas canvas, Vertex selectedVertex1, Vertex selectedVertex2, BaseGraph graph, bool isCleancanvas);
        void DrawRoute(Canvas canvas, List<Vertex> vertexs, Color colorVertex, Brush brushesLine );
        void DrawSelectedVertex(Canvas canvas, Vertex vertex, Color colorVertex);
        List<List<List<int>>> GetAdjacencyMatrix(BaseGraph graph);
        List<Vertex> GetVertexs(BaseGraph graph);
    }
}
