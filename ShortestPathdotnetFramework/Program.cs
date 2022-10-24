/*
    Have the function ShortestPath(strArr) take strArr which will be an array of strings which models a non-looping Graph. 
    The structure of the array will be as follows: The first element in the array will be the number of nodes N (points) in the array as a string. 
    The next N elements will be the nodes which can be anything (A, B, C .. Brick Street, Main Street .. etc.). Then after the Nth element, 
    the rest of the elements in the array will be the connections between all of the nodes. They will look like this: (A - B, B - C..Brick Street - Main Street..etc.). 
    Although, there may exist no connections at all.

    An example of strArr may be: ["4","A","B","C","D","A-B","B-D","B-C","C-D"]. 
    Your program should return the shortest path from the first Node to the last Node in the array separated by dashes. 
    So in the example above the output should be A-B-D. Here is another example with strArr being ["7","A","B","C","D","E","F","G","A-B","A-E","B-C","C-D","D-F","E-D","F-G"].
    The output for this array should be A-E-D-F-G. 
    There will only ever be one shortest path for the array. 
    If no path between the first and last node exists, return -1. 
    The array will at minimum have two nodes. Also, the connection A-B for example, means that A can get to B and B can get to A.
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace ShortestPathdotnetFramework
{
    internal class Program
    {
        /*
         * We use a form of BFS algorithm. First we manipulate the input data into the objects/arrays/lists we want - this is the NodeList object.
         * From there, using loops we figure out which are the adjacent nodes for populate the AdjacencyList  object.
         * 
         * We give each node properties (DistanceFromSource and PreviousNodeLabel) - we default these to some max number/string
         * 
         * Assuming all edges are of the same weight (1) we use a version of the BFS algorithm using queues (starting with the 1st node) and 
         * traversing it's adjacent neighbor nodes, if we find a neighbor node we calculate the weight (distance) and if that distance < the currently 
         * recorded distance for the adjacent node we update the PreviousNodeLabel and DistanceFromSource fields in the NodeList, if distance < currently
         * recorded values, we do nothing.
         * 
         * 
         * Last step is to display the actual path for the first and last node which is where we loop over the NodesList and use the PreviousNodeLable
         * starting from the last Node to the First Node. If i path is found (PreviousNodeLable != "XXXXX") we add this to a path array.
         * Finally we print the path in reverse.
         * 
         * Resources used:
         * https://www.youtube.com/watch?v=9jJl29oXT18
         */
        static string lastNode = string.Empty;
        static string firstNode = string.Empty;
        static int numberOfNodes = 0;

        static void Main(string[] args)
        {
            //ShortestPath(new string[]{ "4", "A", "B", "C", "D", "A-B", "B-D", "B-C", "C-D" });
            ShortestPath(new string[] { "7", "A", "B", "C", "D", "E", "F", "G", "A-B", "A-E", "B-C", "C-D", "D-F", "E-D", "F-G" });


            Console.WriteLine("Press any Key to terminate");
            Console.ReadKey();
        }

        static void ShortestPath(string[] strArr)
        {
            if (strArr.Length < 1) return;

            numberOfNodes = Convert.ToInt32(strArr[0]);            

            // manipulate the data the way we want it
            string[] NodeStr = strArr.Skip(1).Take(numberOfNodes).ToArray();
            string[] Connections = strArr.Skip(numberOfNodes + 1).ToArray();

            lastNode = NodeStr[numberOfNodes - 1];
            firstNode = NodeStr[0];

            Console.WriteLine("Input: " + String.Join(", ", strArr));
            Console.WriteLine("Nodes: " + String.Join(", ", NodeStr));
            Console.WriteLine("Connections: " + String.Join(", ", Connections));
            Console.WriteLine("First Node: " + firstNode);
            Console.WriteLine("Last Node: " + lastNode);

            // could for the BFS (altered)
            ShortestPathBFS(numberOfNodes, GetNodeList(numberOfNodes, NodeStr), Connections);
        }

        private static AdjacencyList[] GetAdjacencyList(int numberOfNodes, Node[] NodeList, string[] Connections)
        {
            // Gets the list of Adjacent Node Labels for each Node
            // Maybe could have used a LinkedList?
            AdjacencyList[] adjacencyList = new AdjacencyList[numberOfNodes];

            for (var i = 0; i < NodeList.Length; i++)
            {
                var n = new AdjacencyList();
                n.NodeLabel = NodeList[i].NodeLabel;

                foreach (var conn in Connections)
                {
                    if (conn.StartsWith(NodeList[i].NodeLabel))
                    {
                        n.Adjancent.Add(conn.Split('-')[1]);
                    }
                }
                adjacencyList[i] = n;
            }

            return adjacencyList;
        }

        private static Node[] GetNodeList(int numberOfNodes, string[] NodeStr)
        {
            // Populates the Node[] object for us
            Node[] nodeList = new Node[numberOfNodes];

            for (var i = 0; i < NodeStr.Length; i++)
            {
                var n = new Node();
                n.NodeLabel = NodeStr[i];

                if (i == 0)
                    n.DistanceFromSource = 0;

                nodeList[i] = n;
            }

            return nodeList;
        }

        private static void ShortestPathBFS(int numberOfNodes, Node[] nodeList, string[] connections)
        {
            // The modified BFS algorithm

            // Queue we use to navigate the Nodes
            Queue<Node> queue = new Queue<Node>();

            // Gets the AdjacencyList object
            AdjacencyList[] adjacencyList = GetAdjacencyList(numberOfNodes, nodeList, connections);

            // We add the first Node to the queue
            queue.Enqueue(nodeList[0]);

            // We begin traversing the graph from the first node
            while (queue.Count > 0)
            {
                // current node
                Node currentNode = queue.Dequeue();

                // adjacent nodes for this current node
                var adjacentNodes = adjacencyList.FirstOrDefault(x => x.NodeLabel == currentNode.NodeLabel).Adjancent;
                
                // loop through all adjacent nodes, could be more than 0
                foreach (var adjacentNode in adjacentNodes)
                {
                    var node = nodeList.FirstOrDefault(x => x.NodeLabel == adjacentNode);

                    // here we compare the distance from first node if the distance < currently recorded distance we update our values
                    // and add the current node to the queue to continur processing
                    if (currentNode.DistanceFromSource + 1 < node.DistanceFromSource)
                    {                        
                        node.PreviousNodeLabel = currentNode.NodeLabel;
                        node.DistanceFromSource = currentNode.DistanceFromSource + 1;

                        queue.Enqueue(node);
                    }
                }
            }
            FindShortestPath(nodeList);
        }

        static void FindShortestPath(Node[] nodeList)
        {
            string[] path = new string[nodeList.FirstOrDefault(x => x.NodeLabel == lastNode).DistanceFromSource + 1];

            int count = 0;

            string last = lastNode;
            string first = firstNode;

            path[count] = lastNode;

            while (last != first)
            {
                count++;
                if(nodeList.FirstOrDefault(x => x.NodeLabel == last).PreviousNodeLabel != "XXXXX")
                {
                    path[count] = nodeList.FirstOrDefault(x => x.NodeLabel == last).PreviousNodeLabel;
                    last = nodeList.FirstOrDefault(x => x.NodeLabel == last).PreviousNodeLabel;
                }
                else
                {
                    Console.WriteLine("Shortest Path: -1");
                    return;
                }
                
            }
            count++;

            Console.WriteLine("Shortest Path: " + String.Join("-", path.Reverse()));

            Console.WriteLine();
        }
    }

    public class Node
    {
        public string NodeLabel { get; set; } = "";
        public string PreviousNodeLabel { get; set; } = "XXXXX";
        public int DistanceFromSource { get; set; } = 99999;
    }

    public class AdjacencyList
    {
        public string NodeLabel { get; set; } = "";
        public List<string> Adjancent { get; set; } = new List<string>();
    }
}
