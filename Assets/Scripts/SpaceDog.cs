using UnityEngine;
using System.Collections;

public class SpaceDog : MonoBehaviour {

	public GameObject thingPrefab;
	public GameObject chainPrefab;

	// Use this for initialization
	void Start () {
		var color = new Color(1f, 0.5f, 0.5f);
		var chainObject = Instantiate(chainPrefab);
		Chain chain = chainObject.GetComponent<Chain>();
		chain.SetColor(color);
		chain.fromObject = CreateWithColor(thingPrefab, color);
		chain.toObject = CreateWithColor(thingPrefab, color);
	}

	// Update is called once per frame
	void Update () {

	}

	static private GameObject CreateWithColor(GameObject prefab, Color color)
	{
		var created = Instantiate(prefab) as GameObject;
		created.GetComponent<Renderer>().material.color = color;
		return created;
	}
}
