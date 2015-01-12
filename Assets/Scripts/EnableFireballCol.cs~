using UnityEngine;
using System.Collections;

public class EnableFireballCol : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnCollisionEnter(Collision col){
		if(col.gameObject.CompareTag("projectile")){
	    	transform.parent.GetComponent<Building>().collisions++;
			col.rigidbody.isKinematic=false;
		}
		
	}
}
