using UnityEngine;
using System.Collections;

public class Destruct : MonoBehaviour {

	public float timeOut = 3.0f;
	public bool detachChildren = false;
	
	void Awake () {
		Invoke("DestroyNow", timeOut);
	}
	
	void DestroyNow () {
		if (detachChildren) {
			transform.DetachChildren();
		}
		DestroyObject(gameObject);
	}
}