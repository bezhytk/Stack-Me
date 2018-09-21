using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveWreck : MonoBehaviour 
{
	private void OnCollisionEnter(Collision col)
	{
		Destroy (col.gameObject);
	}
}
