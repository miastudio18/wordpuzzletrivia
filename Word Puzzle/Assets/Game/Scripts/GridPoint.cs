using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPoint : MonoBehaviour {
	
	public bool IsOccupied {
		get;
		set;
	}

	[HideInInspector]
	public GameObject Letter;

}
