
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Repulsion {
	public List<Collider> repulsives = new List<Collider>();
	public List<string> repulsiveTags = new List<string>() { "Thing" };

	private Transform repulserTransform;
	private IRepulsionConfiguration configuration;

	public Repulsion(Transform repulserTransform, IRepulsionConfiguration repulsionConfiguration) {
		this.repulserTransform = repulserTransform;
		this.configuration = repulsionConfiguration;
	}

	public void OnTriggerEnter(Collider other) 
	{
		if (ShouldRepluse(other)) {
			Repulse(other);
		}
	}
	public void OnTriggerExit(Collider other) 
	{
		if (ShouldRepluse(other)) {
			DoNotRepulse(other);
		}
	}

	public void FixedUpdate()
	{
		repulsives.ForEach (repulsive => {
			RepulserBody().AddForce(VectorToRepulsive(repulsive) * -CalculateRepulsiveForce(repulsive), ForceMode.Force);
		});
	}

	private float CalculateRepulsiveForce(Collider repulsive)
	{
		float distance = Vector3.Distance(repulserTransform.position, repulsive.transform.position);
		return Mathf.Min(configuration.MaxStrength, configuration.StrengthFactor / distance);
	}

	private bool ShouldRepluse(Collider other)
	{
		return repulsiveTags.Contains(other.tag);
	}

	private Rigidbody RepulserBody()
	{
		return repulserTransform.GetComponent<Rigidbody>();
	}

	private Vector3 VectorToRepulsive(Collider repulsive)
	{
		return repulsive.transform.position - repulserTransform.position;
	}

	private void Repulse(Collider other)
	{
		if (!repulsives.Contains(other)) {
			repulsives.Add(other);
		}
	}

	private void DoNotRepulse(Collider other)
	{
		if (repulsives.Contains(other)) {
			repulsives.Remove(other);
		}
	}
}
