using UnityEngine;
using System.Collections;

public class ProjectileController : MonoBehaviour {

	public float projectileDistance = 10;
	public float projectileRange = 3;
	public float projectileSpeed = 75;
	public float timeOut = 10;

	private bool pastGlider;
	private Transform target;
	private Transform glider;

	void Start () {
		if (gliding.isAlive) {
			glider = GameObject.Find("glider").transform;
			target = new GameObject("Target").transform;
			target.position = glider.position + new Vector3(Random.Range(-projectileRange, projectileRange), 0, Random.Range(-projectileRange, projectileRange));
			pastGlider = false;
		}
	}

	void Update () {
		if (gliding.isAlive) {
			if (Vector3.Distance(transform.position, glider.position) >= projectileDistance && !pastGlider) {
				transform.LookAt(target);
				pastGlider = true;
			}
			transform.position += transform.forward * projectileSpeed * Time.deltaTime;
		}
	}

	void OnCollisionEnter (Collision collision) {
		if (collision.gameObject.name != "Village" && collision.gameObject.name != "Building")
			DestroyNow();
	}

	void Awake () {
		Invoke("DestroyNow", timeOut);
	}
	
	void DestroyNow () {
		DestroyObject(gameObject);
	}
}