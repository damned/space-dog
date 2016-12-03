using System;
using System.Collections.Generic;
using UnityEngine;

[IntegrationTest.DynamicTestAttribute("IntegrationTests")]
[IntegrationTest.SucceedWithAssertions]
[IntegrationTest.TimeoutAttribute(1)]
[IntegrationTest.ExpectExceptions(false, typeof(ArgumentException))]
[IntegrationTest.ExcludePlatformAttribute()]
public class ThingRepulsionDynamicTest : MonoBehaviour
{
	public void Start()
	{
		IntegrationTest.Pass(gameObject);
	}
}
