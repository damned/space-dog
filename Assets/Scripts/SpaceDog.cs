using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System;

public class SpaceDog : MonoBehaviour {

	public GameObject thingPrefab;
	public GameObject chainPrefab;
	private Color color = new Color(1f, 0.5f, 0.5f);
	private IDictionary<string, GameObject> thingsById = new Dictionary<string, GameObject>();

	[Serializable]
	public class Node
	{
		public string id;
		public string label;
	}

	[Serializable]
	public class Edge
	{
		public string id;
		public string label;
		public string source;
		public string target;
	}

	public class Graph
	{
		public List<Node> nodes;
		public List<Edge> edges;
	}


	// Use this for initialization
	void Start () {
		var graph = LoadGraphDefinition();
		foreach (var node in graph.nodes) {
			thingsById[node.id] = CreateWithColor(thingPrefab, color);
		}
		foreach (var edge in graph.edges) {
			var chain = CreateChain();
			chain.fromObject = thingsById[edge.source];
			chain.toObject = thingsById[edge.target];
		}
	}

	// Update is called once per frame
	void Update () {

	}

	private Chain CreateChain()
	{
		var chainObject = Instantiate(chainPrefab);
		Chain chain = chainObject.GetComponent<Chain>();
		chain.SetColor(color);
		return chain;
	}

	static private GameObject CreateWithColor(GameObject prefab, Color color)
	{
		Vector3 position = new Vector3(UnityEngine.Random.Range(-7.0f, 7.0f), 0, UnityEngine.Random.Range(-7.0f, 7.0f));
		var created = Instantiate(prefab, position, Quaternion.identity) as GameObject;
		created.GetComponent<Renderer>().material.color = color;
		return created;
	}

	static private Graph LoadGraphDefinition()
	{
		Graph graph;
		using (StreamReader r = new StreamReader("/tmp/graph.json")) {
			string json = r.ReadToEnd();
			graph = JsonUtility.FromJson<Graph>(json);
			Debug.Log("got graph, nodes: " + graph.nodes.Count);
		}
		return graph;
	}

}
