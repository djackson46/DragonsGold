using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]

public class VillageController : MonoBehaviour {

	public AudioClip projectileAudio;
	public float attackDistance = 75;
	public float attackTimeLimit = 5;
	public GameObject explosion;
	public GameObject fire1;
	public GameObject fire2;
	public GameObject fire3;
	public GameObject projectile;
	public int health = 3;

	private bool isActive = true;
	private float attackTime;
	private GameObject building;
	private int color = 3;
	private Transform glider;

	void Start () {
		glider = GameObject.Find("glider").transform;
		building = gameObject.transform.parent.gameObject;
	}

	void Update () {
		attackTime += Time.deltaTime;
		if (Vector3.Distance(transform.position, glider.position) <= attackDistance && attackTime >= attackTimeLimit && isActive && gliding.isAlive) {
			Instantiate(projectile.transform, transform.position, transform.rotation);
			audio.PlayOneShot(projectileAudio);
			attackTime = 0;
		}
	}
	
	void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.layer == 8) {
			Color buildingColor = building.renderer.material.color;
			building.renderer.material.color = new Color(buildingColor.r - 1.0f/color, buildingColor.g - 1.0f/color, buildingColor.b - 1.0f/color);

			Instantiate(explosion.transform, transform.position, transform.rotation);
			
			if (health > 0) health--;
			if (health == 2) fire1.SetActive(true);
			if (health == 1) fire2.SetActive(true);
			if (health == 0) {
				fire3.SetActive(true);
				isActive = false;
			}
		}
	}
}