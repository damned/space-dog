using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System;

public class SpaceDog : MonoBehaviour {

	public GameObject thingPrefab;
	public GameObject chainPrefab;
	public GameObject containerPrefab;
	public float containmentSpringForce = 30f;

	private Color color = new Color(1f, 0.5f, 0.5f);
	private IDictionary<string, GameObject> thingsById = new Dictionary<string, GameObject>();

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

	public class Graph
	{
		public List<Node> nodes;
		public List<Edge> edges;
	}


	// Use this for initialization
	void Start () {
		var graph = LoadGraphDefinition();
		foreach (var node in graph.nodes) {
			GameObject thing;
			if (node.type == "container") {
				thing = CreateSomewhere(containerPrefab);
				thing.GetComponentInChildren<TextMesh>().text = node.label;
			}
			else {
				thing = CreateWithColor(thingPrefab, color);
				var labelParts = node.label.Split('_');
				thing.GetComponentInChildren<TextMesh>().text = labelParts[labelParts.Length - 1];
				UseGravity(thing, true);
			}
			thingsById[node.id] = thing;
		}
		foreach (var edge in graph.edges) {
			if (edge.type == "containment") {
				var container = thingsById[edge.source];
				var containee = thingsById[edge.target];
				var containerAttraction = containee.AddComponent<SpringJoint>();
				containerAttraction.connectedBody = container.GetComponent<Rigidbody>();
				containerAttraction.spring = containmentSpringForce;
				var link = containee.AddComponent<LineRenderer>();
				link.material = new Material(Shader.Find("Particles/Additive"));
				link.SetVertexCount(2);
				link.SetColors(color, color); 
				link.SetPosition(0, container.transform.position); 
				link.SetPosition(1, containee.transform.position);
				containee.GetComponent<NodeController>().container = container;
				link.SetWidth(1, 1); 
			}
			else {
				var chain = CreateChain();
				chain.fromObject = thingsById[edge.source];
				chain.toObject = thingsById[edge.target];
				UseGravity(chain.fromObject, false);
			}
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

	static private GameObject CreateSomewhere(GameObject prefab)
	{
		Vector3 position = new Vector3(UnityEngine.Random.Range(-7.0f, 7.0f), 0, UnityEngine.Random.Range(-7.0f, 7.0f));
		return Instantiate(prefab, position, Quaternion.identity) as GameObject;
	}

	static private GameObject CreateWithColor(GameObject prefab, Color color)
	{
		var created = CreateSomewhere(prefab);
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

	static void UseGravity(GameObject thing, bool useGravity)
	{
		thing.GetComponent<Rigidbody>().useGravity = useGravity;
	}

}
