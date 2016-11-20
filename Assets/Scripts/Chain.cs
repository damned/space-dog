using UnityEngine;
using System.Collections;

public class Chain : MonoBehaviour {

	void Start () {
		CountLinks ();	
	}
	
	void Update () {
	}

	void CountLinks ()
	{
		int linkCount = 0;
		foreach (Transform linkTransform in transform) {
			GameObject link = linkTransform.gameObject;
			linkCount++;
			DebugLinkLengths (link);
		}
		Debug.Log ("link count: " + linkCount);
	}

	void DebugLinkLengths (GameObject link)
	{
		var joints = link.GetComponents<SpringJoint> ();
		foreach (SpringJoint joint in joints) {
			
			var otherEnd = joint.connectedBody;
			if (otherEnd == null) {
				Debug.Log ("joint not connected");
			}
			else {
				float length = Vector3.Distance (joint.transform.position, otherEnd.transform.position);
				Debug.Log ("link length: " + length);
			}
		}
	}
}
