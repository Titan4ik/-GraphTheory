using System.Collections.Generic;

namespace labGraph.Model.InterfaceImplementation
{
    public static class ConverterMatrixFormatGraph 
    {
        public static List<List<List<int>>> AdjacencyToIncidenceMatrix(List<List<List<int>>> adjacencyMatrix)
        {
            var incidenceMatrix = new List<List<List<int>>>();
            for (int i = 0; i < adjacencyMatrix.Count; i++)
            {
                incidenceMatrix.Add(new List<List<int>>());
            }
            for (var i = 0; i < adjacencyMatrix.Count; i++)
                for (var j = 0; j < adjacencyMatrix.Count; j++)
                {

                    if (adjacencyMatrix[i][j][0] != 0)
                    {

                        for (var q = 0; q < adjacencyMatrix.Count; q++)
                        {
                            if (i == q || j == q)
                            {
                                incidenceMatrix[q].Add(adjacencyMatrix[i][j]);
                            }
                            else
                                incidenceMatrix[q].Add(new List<int> { 0 });
                        }

                    }

                }



            return incidenceMatrix;
        }

        public static List<List<List<int>>> IncidenceToAdjacencyMatrix(List<List<List<int>>> incidenceMatrix)
        {
            var adjacencyMatrix = new List<List<List<int>>>();
            for (int i = 0; i < incidenceMatrix.Count; i++)
            {
                adjacencyMatrix.Add(new List<List<int>>());
                for (int j = 0; j < incidenceMatrix.Count; j++)
                {
                    adjacencyMatrix[i].Add(new List<int>());
                }
            }

            if (incidenceMatrix.Count > 0)
                for (var j = 0; j < incidenceMatrix[0].Count; j++)
                {
                    List<int> list = new List<int>();
                    int row = -1, column = -1;
                    for (var i = 0; i < incidenceMatrix.Count; i++)
                    {

                        if (incidenceMatrix[i][j][0] != 0)
                        {
                            if (row == -1)
                                row = i;
                            else if (column == -1)
                                column = i;
                            list = incidenceMatrix[i][j];

                        }

                    }
                    if (row != -1 && column != -1)
                    {
                        adjacencyMatrix[row][column] = list;
                        adjacencyMatrix[column][row] = list;
                        row = -1; column = -1;
                    }
                    else if (row != -1)
                    {
                        adjacencyMatrix[row][row] = list;
                        row = -1; column = -1;
                    }
                }

            for (var i = 0; i < adjacencyMatrix.Count; i++)
                for (var j = 0; j < adjacencyMatrix.Count; j++)
                {

                    if (adjacencyMatrix[i][j].Count == 0)
                    {

                        adjacencyMatrix[i][j].Add(0);

                    }

                }

            return adjacencyMatrix;
        }
    }
}
