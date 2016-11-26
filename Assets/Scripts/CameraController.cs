using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CameraController : MonoBehaviour {

	public float speed = 3f;
	public float angularSpeed = 30f;

	public IDictionary<KeyCode, Action> mappingsWithControlKey;
	public IDictionary<KeyCode, Action> mappingsWithoutControlKey;
	public IDictionary<KeyCode, Action> mappingsWhenever;

	// Use this for initialization
	void Start () {
		mappingsWithControlKey = new Dictionary<KeyCode, Action>()
		{
			{ KeyCode.RightArrow, RotateRight },
			{ KeyCode.LeftArrow, RotateLeft },
			{ KeyCode.DownArrow, RotateDown },
			{ KeyCode.UpArrow, RotateUp }
		};
		mappingsWithoutControlKey = new Dictionary<KeyCode, Action>()
		{
			{ KeyCode.RightArrow, MoveRight },
			{ KeyCode.LeftArrow, MoveLeft },
			{ KeyCode.DownArrow, Back },
			{ KeyCode.UpArrow, Forward }
		};
		mappingsWhenever = new Dictionary<KeyCode, Action>()
		{
			{ KeyCode.A, Up },
			{ KeyCode.Z, Down }
		};
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKey(KeyCode.LeftControl)) {
			ApplyKeyActions(mappingsWithControlKey);
		}
		else 
		{
			ApplyKeyActions(mappingsWithoutControlKey);
		}

		if(Input.GetKey(KeyCode.A))
		{
			Up();
		}
		if(Input.GetKey(KeyCode.Z))
		{
			Down();
		}
	}

	static void ApplyKeyActions(IDictionary<KeyCode, Action> mappings)
	{
		foreach (var mapping in mappings) {
			if (Input.GetKey(mapping.Key)) {
				mapping.Value();
			}
		}
	}

	float Distance()
	{
		return speed * Time.deltaTime;
	}

	float Angle()
	{
		return angularSpeed * Time.deltaTime;
	}

	void Move(Vector3 unitVector)
	{
		transform.Translate(unitVector * Distance(), Space.World);
	}

	void RotateRightAround(Vector3 axis)
	{
		transform.RotateAround(transform.position, axis, Angle());
	}

	void Up()
	{
		Move(transform.up);
	}

	void Down()
	{
		Move(transform.up * -1);
	}

	void Forward()
	{
		Move(transform.forward);
	}

	void Back()
	{
		Move(transform.forward * -1);
	}

	void MoveLeft()
	{
		Move(transform.right * -1);
	}

	void MoveRight()
	{
		Move(transform.right);
	}

	void RotateUp()
	{
		RotateRightAround(transform.right);
	}

	void RotateDown()
	{
		RotateRightAround(transform.right * -1);
	}

	void RotateLeft()
	{
		RotateRightAround(transform.up * -1);
	}

	void RotateRight()
	{
		RotateRightAround(transform.up);
	}
}
