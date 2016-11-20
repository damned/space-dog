
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NodeController : MonoBehaviour {

	private List<Collider> repulsives = new List<Collider>();
	private List<string> repulsiveTags = new List<string>() { "Thing" };

	public void Start() {
		Debug.Log ("started");
	}

	void FixedUpdate()
	{
		repulsives.ForEach (repulsive => {
			float strength = -5;
			Vector3 repulsiveVector = repulsive.transform.position - transform.position;
			Rigidbody rd = transform.GetComponent<Rigidbody>();
			rd.AddForce (repulsiveVector * strength, ForceMode.Force);
		});
	}

	void OnTriggerEnter(Collider other) 
	{
		Debug.Log ("entered! " + other.tag);
		if (repulsiveTags.Contains(other.tag)) {
			if (!repulsives.Contains (other)) {
				repulsives.Add (other);
			}
		}
	}
	void OnTriggerExit(Collider other) 
	{
		Debug.Log ("exited! " + other.tag);
		if (repulsiveTags.Contains(other.tag)) {
			if (repulsives.Contains(other)) {
				repulsives.Remove(other);
			}
		}
	}
}
