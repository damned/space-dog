
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NodeController : MonoBehaviour {

	private List<Collider> repulsives = new List<Collider>();
	private List<string> repulsiveTags = new List<string>() { "Thing" };

	public float strengthFactor = 1f;
	public float maxStrength = 9f;

	public void Start() {
	}

	void FixedUpdate()
	{
		repulsives.ForEach (repulsive => {
			Vector3 repulsiveVector = repulsive.transform.position - transform.position;
			float distance = Vector3.Distance(transform.position, repulsive.transform.position);
			float strength = Mathf.Max(0f, maxStrength - distance * strengthFactor);
			Rigidbody rd = transform.GetComponent<Rigidbody>();
			rd.AddForce (repulsiveVector * -strength, ForceMode.Force);
		});
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
