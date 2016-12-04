using System;
using System.Collections.Generic;
using UnityEngine;

[IntegrationTest.DynamicTestAttribute("IntegrationTests")]
[IntegrationTest.SucceedWithAssertions]
[IntegrationTest.TimeoutAttribute(5)]
[IntegrationTest.ExcludePlatformAttribute()]
public class ThingsAndContainersLoadedDynamicTest : MonoBehaviour
{
	private bool firstUpdate = true;
	private List<GameObject> things;
	private List<GameObject> chains;
	private List<GameObject> containers;

	public void Awake() 
	{
		SetupTestScenario();

	}

	private void OnSecondUpdate()
	{
		RunAllTests();
	}

	static void SetupTestScenario()
	{
		var testGraphDefinition = "Assets/Tests/IntegrationTests/Graphs/simple-graph.json";

		var scripts = GameObject.Find("GameScripts") as GameObject;
		var spaceDog = scripts.AddComponent<SpaceDog>();
		spaceDog.sigmaJsJsonFilename = testGraphDefinition;
		spaceDog.chainPrefab = LoadPrefab("Chain");
		spaceDog.thingPrefab = LoadPrefab("Thing");
		spaceDog.containerPrefab = LoadPrefab("ContainerMass");
	}

	private void RunAllTests()
	{
		things = AllTagged("Thing");
		chains = AllTagged("Chain");
		containers = AllTagged("Container");

		TestCorrectObjectsAreInstantiated();
		TestNodesAreWiredTogetherWithChains();

		IntegrationTest.Pass();
	}

	private void TestCorrectObjectsAreInstantiated()
	{
		AssertEqual(things.Count, 3);
		AssertEqual(containers.Count, 1);
		AssertEqual(chains.Count, 3);
	}

	private void TestNodesAreWiredTogetherWithChains()
	{
		var thing0 = FindThingObject("Thing 0"); 
		var thing1 = FindThingObject("Thing 1"); 
		var thing2 = FindThingObject("Thing 2"); 

		AssertEqual(FindChainFrom(thing0).toObject, thing1);
		AssertEqual(FindChainFrom(thing1).toObject, thing2);
		AssertEqual(FindChainFrom(thing2).toObject, thing0);
	}
		
	public void Update()
	{
		if (firstUpdate) {
			firstUpdate = false;
		} else {
			OnSecondUpdate();
		}
	}

	static GameObject LoadPrefab(string prefab)
	{
		Debug.Log("loading prefab: " + prefab);
		var loaded = Resources.Load<GameObject>("Prefabs/" + prefab);
		Debug.Log("loaded prefab: " + loaded);
		return loaded;
	}

	private void AssertEqual(object actual, object expected)
	{
		if (!actual.Equals(expected)) {
			IntegrationTest.Fail("expected [" + actual + "] to equal [" + expected + "]");
		}
	}

	static List<GameObject> AllTagged(string tag)
	{
		return new List<GameObject>(GameObject.FindGameObjectsWithTag(tag) as GameObject[]);
	}

	private GameObject FindThingObject(string name)
	{
		return things.Find(t => t.GetComponentInChildren<TextMesh>().text.Equals(name));
	}

	private Chain FindChainFrom(GameObject fromThing)
	{
		return chains.Find(t => t.GetComponent<Chain>().fromObject == fromThing).GetComponent<Chain>();
	}
}
