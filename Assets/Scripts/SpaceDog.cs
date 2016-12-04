using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System;

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
