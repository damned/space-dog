using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System;

public class SigmaJsGraphFileLoader
{
	public class Graph
	{
		public List<Node> nodes;
		public List<Edge> edges;
	}

	[Serializable]
	public class Node
	{
		public string id;
		public string label;
		public string type;
	}

	[Serializable]
	public class Edge
	{
		public string id;
		public string label;
		public string source;
		public string target;
		public string type;
	}

	private string filename;

	public SigmaJsGraphFileLoader(string filename)
	{
		this.filename = filename;
	}

	public Graph LoadGraphDefinition()
	{
		Graph graph;
		using (StreamReader r = new StreamReader(filename)) {
			string json = r.ReadToEnd();
			graph = JsonUtility.FromJson<Graph>(json);
			Debug.Log("got graph, nodes: " + graph.nodes.Count);
		}
		return graph;
	}
}
