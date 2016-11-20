using UnityEngine;
using System.Collections;

public class Chain : MonoBehaviour
{

	public GameObject fromObject;

	public GameObject toObject;

	public float maxLinkLength = 3f;
	public float linkAddTimeout = 0.2f;

	private bool addingNewLink = false;

	void Start()
	{
		ForEachLink(link => {
			if (link.name == "ChainLink0") {
				ForEachUnconnectedJoint(link, (joint => {
					joint.connectedBody = fromObject.GetComponent<Rigidbody>();
				}));
			}
			else if (link.name == "ChainLinkN") {
				ForEachUnconnectedJoint(link, (joint => {
					joint.connectedBody = toObject.GetComponent<Rigidbody>();
				}));
			}
		});
	}

	void Update()
	{
		UpdateLinks();
	}

	void UpdateLinks()
	{
		int linkCount = 0;
		float chainLength = 0;
		ForEachLink(link => {
			ForEachConnectedJoint(link, (joint => {
				chainLength += JointLength(joint);
			}));
			linkCount++;
		});
		if (linkCount > 0) {
			float averageLinkLength = chainLength / linkCount;
			if (averageLinkLength > maxLinkLength && !addingNewLink) {
				Debug.Log("average link length for " + name + ": " + averageLinkLength);
				addingNewLink = true;
				CreateNewLink();
				ForEachLink(link => {
					ForEachConnectedJoint(link, joint => {
						Debug.Log("link " + link.name + " joint connected object: " + joint.connectedBody.gameObject.name);
					});
				});
				Invoke("TimeoutLinkAdd", linkAddTimeout);
			}
		}
	}

	void TimeoutLinkAdd()
	{
		addingNewLink = false;
	}

	void CreateNewLink()
	{
		var newIndex = transform.childCount - 1;
		var beforeLink = transform.FindChild("ChainLink" + (newIndex - 1)).gameObject;
		var afterLink = transform.FindChild("ChainLinkN").gameObject;
		var newLink = Instantiate(beforeLink);
		newLink.name = "ChainLink" + newIndex;
		ForEachConnectedJoint(newLink, joint =>  {
			joint.connectedBody = beforeLink.GetComponent<Rigidbody>();
		});
		ForEachConnectedJoint(afterLink, joint =>  {
			if (joint.connectedBody.gameObject.name.StartsWith("ChainLink")) {
				joint.connectedBody = newLink.GetComponent<Rigidbody>();
			}
		});
		newLink.transform.parent = gameObject.transform;
	}

	private delegate void LinkHandler(GameObject link);

	private delegate void JointHandler(SpringJoint joint);

	private void ForEachLink(LinkHandler linkHandler)
	{
		foreach (Transform linkTransform in transform) {
			GameObject link = linkTransform.gameObject;
			linkHandler(link);
		}
	}

	private void ForEachConnectedJoint(GameObject link, JointHandler jointHandler)
	{
		ForEachJoint(link, jointHandler, joint => {});
	}
	private void ForEachUnconnectedJoint(GameObject link, JointHandler jointHandler)
	{
		ForEachJoint(link, joint => {}, jointHandler);
	}
	private void ForEachJoint(GameObject link, JointHandler connectedJointHandler, JointHandler unconnectedJointHandler)
	{
		var joints = link.GetComponents<SpringJoint>();
		foreach (SpringJoint joint in joints) {
			if (joint.connectedBody == null) {
				unconnectedJointHandler(joint);
			} else {
				connectedJointHandler(joint);
			}
		}
	}

	static float JointLength(SpringJoint joint)
	{
		return Vector3.Distance(joint.transform.position, joint.connectedBody.transform.position);
	}
}
