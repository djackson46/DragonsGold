using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour {
	
	public int collisions=0;
	GameObject fire1, fire2;
	// Use this for initialization
	void Start () {
		fire1=GameObject.Find("Fire1");
		fire2=GameObject.Find("Fire2");
		fire1.active=false;
		fire2.active=false;

	}
	
	// Update is called once per frame
	void Update () {
		if(collisions>0){
			fire1.active=true;
			fire2.active=true;
		}
			if(collisions>3){
					
					GameObject[] sub=GameObject.FindGameObjectsWithTag("destructable");
					foreach(GameObject g in sub){
						//temp hacky way to fix catapult-dragon collision deactivate all arms...
						if(Vector3.Distance(transform.position, g.transform.position)<10){
							g.rigidbody.isKinematic=false;
						}
					}
				
			}
	}
	
	void OnCollisionEnter(Collision col){
		
	}
}
