using System;
using System.Collections.Generic;
using System.Text;

namespace Draw.io_Graph_Parser
{
    /// <summary>
    /// Class <c>PathInformation</c> contains information, used by the Dijkstra algorithm, about the path to reach a <see cref="Vertex"/>:
    /// <list type="bullet"><item><description>The minimum cost to reach the <see cref="Vertex"/> from its <see cref="Predecessor"/></description></item><item><description>The <see cref="DijkstraAlgorithm"/> that comes before this one in the path from the source</description></item><item><description>The <see cref="Edge"/> that connects the two <see cref="Vertex"/>-s</description></item></list>
    /// </summary>
    public class PathInformation
    {
        /// <summary>It represents the minimum cost to reach the <see cref="Vertex"/> from its <see cref="Predecessor"/>.</summary>
        public double MinimumCost { get; set; }
        /// <summary>It represents the <see cref="DijkstraVertex"/> that comes before this one in the path from the source.</summary>
        public DijkstraVertex Predecessor { get; set; }
        /// <summary>It represents the <see cref="Edge"/> that connects the two <see cref="Vertex"/>-s.</summary>
        public Edge ConnectingEdge { get; set; }

        /// <summary>This constructor initializes the new <c>PathInformation</c> with the default values.</summary>
        /// <remarks>
        /// Default values are:
        /// <list type="bullet"><item><description><see cref="MinimumCost"/>: <see cref="double.PositiveInfinity"/></description></item><item><description><see cref="Predecessor"/>: <c>null</c></description></item><item><description><see cref="Edge"/>: <c>null</c></description></item></list>
        /// </remarks>
        public PathInformation()
        {
            SetDefaultValues();
        }

        /// <summary>Resets the information about the path to the default values.</summary>
        /// <remarks>
        /// Default values are:
        /// <list type="bullet"><item><description><see cref="MinimumCost"/>: <see cref="double.PositiveInfinity"/></description></item><item><description><see cref="Predecessor"/>: <c>null</c></description></item><item><description><see cref="Edge"/>: <c>null</c></description></item></list>
        /// </remarks>
        public void SetDefaultValues()
        {
            MinimumCost = double.PositiveInfinity;
            Predecessor = null;
            ConnectingEdge = null;
        }
    }

    /// <summary>Class <c>DijkstraVertex</c> is used by <see cref="DijkstraAlgorithm"/> to store information about the path to reach a <see cref="Vertex"/> and the <see cref="Vertex"/> itself.</summary>
    public class DijkstraVertex
    {
        /// <value>Property <c>Vertex</c> represents a <see cref="Vertex"/> in the graph.</value>
        public Vertex Vertex { get; private set; }
        /// <value>Property <c>PathInformation</c> represents the information about the path to reach <see cref="Vertex"/>.</value>
        public PathInformation PathInformation { get; set; }

        /// <summary>This constructor initializes the new <c>DijkstraVertex</c> from the <see cref="Vertex"/> object it represents.</summary>
        /// <param name="vertex">The <see cref="Vertex"/> of which you want to store the path information.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public DijkstraVertex(Vertex vertex)
        {
            Vertex = vertex ?? throw new ArgumentNullException("vertex");
            PathInformation = new PathInformation();
            PathInformation.SetDefaultValues();
        }

        public static bool operator <(DijkstraVertex left, DijkstraVertex right)
        {
            if (left.PathInformation.MinimumCost < right.PathInformation.MinimumCost)
                return true;
            return false;
        }

        public static bool operator >(DijkstraVertex left, DijkstraVertex right)
        {
            if (left.PathInformation.MinimumCost > right.PathInformation.MinimumCost)
                return true;
            return false;
        }
    }

    /// <summary>Dijkstra algorithm is used, in the graph theory, to find the minimum paths from one vertex to all the other vertices of the graph.</summary>
    public class DijkstraAlgorithm
    {
        /// <value>Property <c>Vertices</c> represents all the vertices of the graph.</value>
        public DijkstraVertex[] Vertices { get; private set; }
        /// <value>Property <c>HeapSize</c> represents the size of the heap, a tree-based data structure, used by the algorithm.</value>
        public int HeapSize { get; private set; }

        /// <summary>This constructor initializes the new <c>DijkstraAlgorithm</c> from an array containing all the <see cref="Vertex"/>-s of the <see cref="GraphParser"/>.</summary>
        /// <param name="vertices">The <see cref="Vertex"/>-s of the <see cref="GraphParser"/>.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public DijkstraAlgorithm(Vertex[] vertices)
        {
            if (vertices == null)
                throw new ArgumentNullException("vertices");

            Vertices = new DijkstraVertex[vertices.Length];
            for (int i = 0; i < Vertices.Length; i++)
                Vertices[i] = new DijkstraVertex(vertices[i]);
            HeapSize = Vertices.Length;
        }

        /// <summary>Finds the minimum paths from the <see cref="Vertex"/> with the specified id to all the other <see cref="Vertices"/> of the graph.</summary>
        /// <param name="sourceId">The id of the source <see cref="Vertex"/>.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void FindMinimumPathsFrom(string sourceId)
        {
            if (sourceId == null)
                throw new ArgumentNullException("sourceId");

            foreach (DijkstraVertex vtx in Vertices)
            {
                vtx.PathInformation.SetDefaultValues();
                if (vtx.Vertex.Id == sourceId)
                    vtx.PathInformation.MinimumCost = 0;
            }

            BuildMinHeap();

            while (HeapSize > 0)
            {
                DijkstraVertex min = ExtractMin();
                foreach (NavigableNeighbor nn in min.Vertex.Neighbors)
                    Relax(min, nn.Neighbor, nn.Cost, nn.ConnectingEdge);
            }
        }

        private void MinHeapify(int i)
        {
            int left = 2 * i + 1;
            int right = left + 1;
            int min;

            if (left < HeapSize && Vertices[left] < Vertices[i])
                min = left;
            else
                min = i;
            if (right < HeapSize && Vertices[right] < Vertices[min])
                min = right;

            if (min != i)
            {
                DijkstraVertex swap = Vertices[i];
                Vertices[i] = Vertices[min];
                Vertices[min] = swap;

                MinHeapify(min);
            }
        }

        private void BuildMinHeap()
        {
            HeapSize = Vertices.Length;
            for (int i = HeapSize / 2; i >= 0; i--)
                MinHeapify(i);
        }

        private void Relax(DijkstraVertex source, Vertex target, double cost, Edge connectingEdge)
        {
            DijkstraVertex trg = Array.Find(Vertices, v => v.Vertex.Id == target.Id);

            if (trg.PathInformation.MinimumCost > source.PathInformation.MinimumCost + cost)
            {
                trg.PathInformation.MinimumCost = source.PathInformation.MinimumCost + cost;
                trg.PathInformation.Predecessor = source;
                trg.PathInformation.ConnectingEdge = connectingEdge;
            }
        }

        private DijkstraVertex ExtractMin()
        {
            DijkstraVertex min = Vertices[0];
            Vertices[0] = Vertices[HeapSize - 1];
            Vertices[HeapSize - 1] = min;

            HeapSize--;
            MinHeapify(0);

            return min;
        }
    }
}
