using System.Collections.Generic;


namespace labGraph.Model
{
    interface ISaveGraphInFile
    {
        void AdjacencyMatrixFile(string fileName, BaseGraph graph);
        void IncidenceMatrixFile(string fileName, BaseGraph graph);
        void VertexFile(string fileName, BaseGraph graph);
        void EdgeFile(string fileName, BaseGraph graph);

        List<List<List<int>>> GetAdjacencyMatrix(BaseGraph graph);
        List<Vertex> GetVertexs(BaseGraph graph);
    }
}
