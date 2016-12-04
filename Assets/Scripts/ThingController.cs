using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ThingController : MonoBehaviour, IRepulsionConfiguration {

	private Repulsion repulsion;

	public float strengthFactor = 1f;
	public float maxStrength = 5f;
	public GameObject container;

	public float StrengthFactor {
		get { return strengthFactor; } 
	}
	public float MaxStrength {
		get { return maxStrength; } 
	}

	public void Awake() {
		repulsion = new Repulsion(transform, this);
	}

	public void FixedUpdate()
	{
		repulsion.FixedUpdate();

		if (container != null) {
			var link = GetComponent<LineRenderer>();
			link.SetPosition(0, transform.position); 
			link.SetPosition(1, container.transform.position); 
		}

		GetComponentInChildren<TextMesh>().transform.Rotate(transform.up);
	}
		
	public void OnTriggerEnter(Collider other) 
	{
		repulsion.OnTriggerEnter(other);
	}

	public void OnTriggerExit(Collider other) 
	{
		repulsion.OnTriggerExit(other);
	}
}
