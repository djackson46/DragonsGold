using UnityEngine;
using System.Collections;

public class GUIManager : MonoBehaviour {
	float startTime;
	bool titleScreen=true;
	GameObject title, subtitle;
	public gliding dragon_script;
	// Use this for initialization
	void Start () {
		title=GameObject.Find("title");
		subtitle=GameObject.Find("subtitle");
		startTime=Time.time;
		//dragon_script=GetComponent<gliding>();
	}
	
	// Update is called once per frame
	void Update () {
		if(titleScreen){
			dragon_script.maintain_circle();
		}
		
		if((Mathf.Abs(Input.GetAxis("Horizontal"))>0.2f || Mathf.Abs(Input.GetAxis("Vertical"))>0.2f) && Time.time>(startTime+2f)){
			title.active=false;
			subtitle.active=false;
			titleScreen=false;
		}
	}
}	
