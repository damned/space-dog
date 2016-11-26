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
		ApplyKeyActions(mappingsWhenever);
	}

	static private void ApplyKeyActions(IDictionary<KeyCode, Action> mappings)
	{
		foreach (var mapping in mappings) {
			if (Input.GetKey(mapping.Key)) {
				mapping.Value();
			}
		}
	}

	private float Distance()
	{
		return speed * Time.deltaTime;
	}

	private float Angle()
	{
		return angularSpeed * Time.deltaTime;
	}

	private void Move(Vector3 unitVector)
	{
		transform.Translate(unitVector * Distance(), Space.World);
	}

	private void RotateRightAround(Vector3 axis)
	{
		transform.RotateAround(transform.position, axis, Angle());
	}

	private void Up()
	{
		Move(transform.up);
	}

	private void Down()
	{
		Move(transform.up * -1);
	}

	private void Forward()
	{
		Move(transform.forward);
	}

	private void Back()
	{
		Move(transform.forward * -1);
	}

	private void MoveLeft()
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
