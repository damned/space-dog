using UnityEngine;
using System.Collections;

public class SpaceDog : MonoBehaviour {

	public GameObject nodePrefab;

	// Use this for initialization
	void Start () {
		var node = Instantiate(nodePrefab) as GameObject;
		node.GetComponent<Renderer>().material.color = new Color(1f, 0.5f, 0.5f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
