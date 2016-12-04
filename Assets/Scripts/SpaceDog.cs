using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System;

public class SpaceDogBuilder
{
	private float containmentSpringForce = 3f;
	private MonoBehaviour instantiator;
	private GameObject thingPrefab;
	private GameObject chainPrefab;
	private GameObject containerPrefab;
	private Color color = new Color(1f, 0.5f, 0.5f);
	private IDictionary<string, GameObject> nodesById = new Dictionary<string, GameObject>();

	public SpaceDogBuilder(GameObject thingPrefab, GameObject chainPrefab, GameObject containerPrefab) {
		this.thingPrefab = thingPrefab;
		this.chainPrefab = chainPrefab;
		this.containerPrefab = containerPrefab;
	}

	public SpaceDogBuilder AddContainer(string nodeId, string label)
	{
		var thing = CreateSomewhere(containerPrefab);
		thing.GetComponentInChildren<TextMesh>().text = label;
		nodesById[nodeId] = thing;
		return this;
	}

	public SpaceDogBuilder AddThing(string nodeId, string label)
	{
		var thing = CreateWithColor(thingPrefab, color);
		thing.GetComponentInChildren<TextMesh>().text = label;
		UseGravity(thing, true);
		nodesById[nodeId] = thing;
		return this;
	}

	public SpaceDogBuilder AddContainmentEdge(string parentId, string childId)
	{
		var parent = nodesById[parentId];
		var child = nodesById[childId];

		CreateContainmentAttraction(parent, child);
		CreateVisibleContainmentLink(child);
		child.GetComponent<ThingController>().container = parent;
		return this;
	}

	public SpaceDogBuilder AddPointerEdge(string fromNodeId, string toNodeId)
	{
		var fromNode = nodesById[fromNodeId];
		var toNode = nodesById[toNodeId];

		var chain = CreateChain();
		chain.fromObject = fromNode;
		chain.toObject = toNode;
		UseGravity(chain.fromObject, false);
		return this;
	}
		
	public SpaceDogBuilder WithContainmentSpringForce(float force)
	{
		this.containmentSpringForce = force;
		return this;
	}

	private void CreateContainmentAttraction(GameObject parent, GameObject child)
	{
		var containerAttraction = child.GetComponent<SpringJoint>();
		containerAttraction.spring = containmentSpringForce;
		containerAttraction.connectedBody = parent.GetComponent<Rigidbody>();
		containerAttraction.anchor = new Vector3(0, 0, 0);
	}

	private void CreateVisibleContainmentLink(GameObject containee)
	{
		var link = containee.AddComponent<LineRenderer>();
		link.material = new Material(Shader.Find("Particles/Additive"));
		link.SetVertexCount(2);
		link.SetColors(color, color);
		link.SetWidth(1, 1);
		link.enabled = false;
	}

	static private GameObject CreateWithColor(GameObject prefab, Color color)
	{
		var created = CreateSomewhere(prefab);
		created.GetComponent<Renderer>().material.color = color;
		return created;
	}

	static private GameObject CreateSomewhere(GameObject prefab)
	{
		Vector3 position = new Vector3(UnityEngine.Random.Range(-7.0f, 7.0f), 0, UnityEngine.Random.Range(-7.0f, 7.0f));
		return MonoBehaviour.Instantiate(prefab, position, Quaternion.identity) as GameObject;
	}

	static void UseGravity(GameObject go, bool useGravity)
	{
		go.GetComponent<Rigidbody>().useGravity = useGravity;
	}

	private Chain CreateChain()
	{
		var chainObject = MonoBehaviour.Instantiate(chainPrefab);
		Chain chain = chainObject.GetComponent<Chain>();
		chain.SetColor(color);
		return chain;
	}
}
	

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

public class SpaceDog : MonoBehaviour {

	public GameObject thingPrefab;
	public GameObject chainPrefab;
	public GameObject containerPrefab;
	public float containmentSpringForce = 3f;
	public string sigmaJsJsonFilename = "Assets/graph.json";
	private Color color = new Color(1f, 0.5f, 0.5f);

	private IDictionary<string, GameObject> thingsById = new Dictionary<string, GameObject>();

	public void Start () {
		BuildGraph();
	}

	private void BuildGraph()
	{
		var loader = new SigmaJsGraphFileLoader(sigmaJsJsonFilename);
		var builder = new SpaceDogBuilder(thingPrefab, chainPrefab, containerPrefab).WithContainmentSpringForce(containmentSpringForce);
		var graph = loader.LoadGraphDefinition();
		BuildNodes(builder, graph);

		BuildEdges(builder, graph);
	}

	private static void BuildNodes(SpaceDogBuilder builder, SigmaJsGraphFileLoader.Graph graph)
	{
		foreach (var node in graph.nodes) {
			if (node.type == "container") {
				builder.AddContainer(node.id, node.label);
			}
			else {
				builder.AddThing(node.id, ShortThingLabel(node.label));
			}
		}
	}

	private static void BuildEdges(SpaceDogBuilder builder, SigmaJsGraphFileLoader.Graph graph)
	{
		foreach (var edge in graph.edges) {
			if (edge.type == "containment") {
				builder.AddContainmentEdge(edge.source, edge.target);
			}
			else {
				builder.AddPointerEdge(edge.source, edge.target);
			}
		}
	}

	static string ShortThingLabel(string label)
	{
		var labelParts = label.Split('_');
		return labelParts[labelParts.Length - 1];
	}

}
