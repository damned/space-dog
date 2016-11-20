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
		var color = new Color(Random.Range(0.5f, 1f), Random.Range(0.5f, 1f), Random.Range(0.5f, 1f));
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
			link.GetComponent<Renderer>().material.color = color;
		});
	}

	void Update()
	{
		UpdateLinks();
	}

	int LinkCount() {
		return transform.childCount;
	}

	float CalculateChainLength()
	{
		float chainLength = 0;
		ForEachLink(link =>  {
			ForEachConnectedJoint(link, (joint =>  {
				chainLength += JointLength(joint);
			}));
		});
		return chainLength;
	}

	float AverageLinkLength()
	{
		return CalculateChainLength() / LinkCount();
	}

	void DebugLinkConnections()
	{
		ForEachLink(link =>  {
			ForEachConnectedJoint(link, joint =>  {
				Debug.Log("link " + link.name + " joint connected object: " + joint.connectedBody.gameObject.name);
			});
		});
	}

	bool ShouldAddNewLink()
	{
		return LinkCount() > 0 && (AverageLinkLength() > maxLinkLength && !addingNewLink);
	}

	void AddNewLink()
	{
		Debug.Log("average link length for " + name + ": " + AverageLinkLength());
		addingNewLink = true;
		DoNewLinkCreation();
		DebugLinkConnections();
		Invoke("TimeoutLinkAdd", linkAddTimeout);
	}

	void UpdateLinks()
	{
		if (ShouldAddNewLink()) {
			AddNewLink();
		}
	}

	void TimeoutLinkAdd()
	{
		addingNewLink = false;
	}

	void ConnectUpNewLink(GameObject beforeLink, GameObject afterLink, GameObject newLink)
	{
		ForEachConnectedJoint(newLink, joint =>  {
			joint.connectedBody = beforeLink.GetComponent<Rigidbody>();
		});
		ForEachConnectedJoint(afterLink, joint =>  {
			if (joint.connectedBody.gameObject.name.StartsWith("ChainLink")) {
				joint.connectedBody = newLink.GetComponent<Rigidbody>();
			}
		});
	}

	static GameObject CreateUnlinkedLink(int newIndex, GameObject beforeLink)
	{
		var newLink = Instantiate(beforeLink);
		newLink.name = "ChainLink" + newIndex;
		return newLink;
	}

	void DoNewLinkCreation()
	{
		var newIndex = transform.childCount - 1;
		var beforeLink = transform.FindChild("ChainLink" + (newIndex - 1)).gameObject;
		var afterLink = transform.FindChild("ChainLinkN").gameObject;
		var newLink = CreateUnlinkedLink(newIndex, beforeLink);
		ConnectUpNewLink(beforeLink, afterLink, newLink);
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
