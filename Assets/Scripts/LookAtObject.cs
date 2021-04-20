using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtObject : MonoBehaviour {

	public GameObject target;

	void Update ()
	{
		transform.LookAt (target.transform);	
	}
}
