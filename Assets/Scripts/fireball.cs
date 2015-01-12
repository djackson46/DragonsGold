﻿using UnityEngine;
using System.Collections;

public class fireball : MonoBehaviour {

	public Vector3 initDirection;
	public float velocity=10f;
	public GameObject tFireworks;
	public GameObject Fire1;
	public AudioSource ffire;
	public AudioSource colsnd;

	int bounces=0;
	float dt=-1;
	// Use this for initialization
	void Start () {
		initDirection=new Vector3(0f,0f,0f);
		ffire.Play();
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.position=transform.position+ transform.forward* velocity*Time.deltaTime;
		if(transform.position.x>10000 || transform.position.y>10000 || transform.position.x<-10000 || transform.position.y<-10000){
				Destroy(this.gameObject);
		}
	}
	
	void OnCollisionEnter(Collision col){
		Debug.Log(col.gameObject.tag);
		if(col.gameObject.CompareTag("target")){
			GameObject ps = (GameObject)Instantiate(tFireworks, transform.position, transform.rotation);
			//colsnd.mute=false;
			//colsnd.Play();
			if(dt<0f)dt=Time.time;
			if(Time.time-dt>0.2f){
				Destroy(ps);
				Destroy(this.gameObject);
				ps.active=false;
			}
		}
		if(col.gameObject.CompareTag("destructable")){
			GameObject ps = (GameObject)Instantiate(Fire1, transform.position, transform.rotation);
			//colsnd.mute=false;
			//colsnd.Play();
			if(dt<0f)dt=Time.time;
			if(Time.time-dt>0.2f){
				Destroy(ps);
				Destroy(this.gameObject);
				ps.active=false;
			}
			col.rigidbody.isKinematic=false;
		}

		//if(++bounces>2){
			Destroy(this.gameObject);
		//}

	}
}
