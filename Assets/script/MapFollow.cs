using UnityEngine;
using System.Collections;

public class MapFollow : MonoBehaviour {
    // The target we are following
    public Transform target;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if(target != null)
            transform.position = new Vector3(target.position.x, transform.position.y, target.position.z);
	}
}
