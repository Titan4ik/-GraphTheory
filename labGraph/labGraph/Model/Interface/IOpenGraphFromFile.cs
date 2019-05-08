using System.Collections.Generic;


namespace labGraph.Model
{
    interface IOpenGraphFromFile
    {
        bool AdjacencyMatrixFile(string fileName, int sizeX, int sizeY, BaseGraph graph);

        bool IncidenceMatrixFile(string fileName, int sizeX, int sizeY, BaseGraph graph);

        bool VertexFile(string fileName, BaseGraph graph);

        bool EdgeFile(string fileName, int sizeX, int sizeY, BaseGraph graph);

        List<List<List<int>>> GetAdjacencyMatrix(BaseGraph graph);
        List<Vertex> GetVertexs(BaseGraph graph);

    }
}
