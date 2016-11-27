
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NodeController : MonoBehaviour {

	private List<Collider> repulsives = new List<Collider>();
	private List<string> repulsiveTags = new List<string>() { "Wall", "Thing" };

	public float strengthFactor = 1f;
	public float maxStrength = 9f;
	public GameObject container;

	public void Start() {
	}

	void FixedUpdate()
	{
		repulsives.ForEach (repulsive => {
			Vector3 repulsiveVector = repulsive.transform.position - transform.position;
			float distance = Vector3.Distance(transform.position, repulsive.transform.position);
			float strength = Mathf.Max(0f, maxStrength - distance * strengthFactor);
			if (repulsive.tag == "Wall") {
				strength *= 2;
			}
			Rigidbody rd = transform.GetComponent<Rigidbody>();
			rd.AddForce (repulsiveVector * -strength, ForceMode.Force);
		});

		if (container != null) {
			var link = GetComponent<LineRenderer>();
			link.SetPosition(0, transform.position); 
			link.SetPosition(1, container.transform.position); 
		}
	}
		
	void OnTriggerEnter(Collider other) 
	{
		if (repulsiveTags.Contains(other.tag)) {
			if (!repulsives.Contains (other)) {
				repulsives.Add (other);
			}
		}
	}
	void OnTriggerExit(Collider other) 
	{
		if (repulsiveTags.Contains(other.tag)) {
			if (repulsives.Contains(other)) {
				repulsives.Remove(other);
			}
		}
	}
}
