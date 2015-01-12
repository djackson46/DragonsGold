using UnityEngine;
using System.Collections;

public class gliding : MonoBehaviour {

	// movement property references
	public float baseMoveSpeed = 6.0F;
	public float rollSpeed     = 1.5F;
	public float tiltSpeed     = 1.5F;
	public float airMoveSpeed  = 1.0f;
	public float jumpSpeed     = 8.0F;
	public float gravity       = 10.0F;
	public float windSlow      = 0.002f;

	public AnimationCurve inputAttack;
	public AnimationCurve inputDecay;
	
	// our movement vector
	private Vector3 moveVector = Vector3.zero;
	
	//audio
	public AudioSource pickup_sound;
	public AudioSource flap;
	public AudioSource flap1;
	public AudioSource ffire;
	public AudioSource hsw;
	public AudioSource flywind;
	public AudioSource flywind1;
	public AudioSource flywind2;
	bool p_flap1=false;
	float dim_sound=0f;

	float barrelStart           =0f;
	float barrelSpeed           =50f;
	bool barrel_right           =true;
	public float barrelDuration =0.1f;

	float slowStart =0f;
	bool canSlow    =false;

	float boostStart = 0f;
	float boostV0    = 0f;
	float maxBoost   = 25f;
	bool  canBoost   = true;
	
	// initial position
	Vector3 initialPosition;
	Quaternion initialRotation;
	float stunTime = 0f;
	int confRot    = 500;
	
	// state machine:  not using it yet
	private ImmediateStateMachine stateMachine = new ImmediateStateMachine ();

	int shots                 = 0;
	bool canShoot             = true;
	int misses                = 0;
	float flapStart           = 0f;
	public float flapDuration = 1f;
	float flapAmount          = 0f;
	
	Vector3 horizontalVelocity;
	float horizontalSpeed; 
	float verticalSpeed;

	float inputRoll  = 0f;
	float inputTilt  = 0f;
	float inputBoost = 0f;

	float rollAmount  = 0f;
	float tiltAmount  = 0f;
	float moveSpeed   = 0f;
	bool isFlapping   = false;
	bool resettingRot = false;

	GameObject glider_mesh;
	GameObject lwing;
	GameObject rwing;
	GameObject tail0;
	GameObject tail1;
	GameObject wind;
	public Rigidbody fireball;
	

	public static int health                 = 100;
	public static int legendary_object_count = 0;
	//int gold = 0;

	public GUIText health_text;
	public GUIText gold_text;
	public GUIText legendary_object_text;

	public static bool isAlive;
    bool collecting_object = false;

	// Use this for initialization
	void Start () {
		//glider_mesh=GameObject.Find("glider_mesh");
		glider_mesh=GameObject.Find("body");
		lwing=GameObject.Find("lwing");
		rwing=GameObject.Find("rwing");
		tail0=GameObject.Find("tail0");
		tail1=GameObject.Find("tail1");
		wind=GameObject.Find("windpart");
		

		initialPosition = transform.position;
		initialRotation = transform.rotation;

		moveSpeed=baseMoveSpeed;
		
		// so we can go outside the bounds
		inputAttack.preWrapMode = WrapMode.ClampForever;
		inputAttack.postWrapMode = WrapMode.ClampForever;
		inputDecay.preWrapMode = WrapMode.ClampForever;
		inputDecay.postWrapMode = WrapMode.ClampForever;

		health = 100;
		legendary_object_count = 0;
		isAlive = true;

		switchToGlideFSM();
	}
	
	void GameStart() {
		// restart, reposition things
		transform.position = initialPosition;
		transform.rotation = initialRotation;		
		
		horizontalVelocity = Vector3.zero;
		horizontalSpeed = 0f;
		verticalSpeed = 0f;
		
		moveVector = Vector3.zero;
		moveSpeed = baseMoveSpeed;

		switchToGlideFSM();
	}
	
	void switchToGlideFSM() {
		stateMachine.ChangeState(enterGLIDING, updateGLIDING, exitGLIDING);
	}

	void switchToFlapFSM() {
		//stateMachine.ChangeState(enterGLIDING, updateFLAP, exitGLIDING);
	}

	void switchToSlowFSM(){
		stateMachine.ChangeState(enterSlow, updateSlow, exitGLIDING);
	}

	void switchToBoostFSM(){
		stateMachine.ChangeState(enterBoost, updateBoost, exitGLIDING);
	}

	void switchToBarrelFSM() {
		stateMachine.ChangeState(enterBarrel, updateBarrel, exitGLIDING);
	}

	void resetWings(){
		rwing.transform.localPosition=new Vector3(1.65f,0f,0f);
		lwing.transform.localPosition=new Vector3(-1.65f,0f,0f);
		rwing.transform.localRotation=Quaternion.AngleAxis(45, transform.up);
		lwing.transform.localRotation=Quaternion.AngleAxis(45, transform.up);
		tail1.transform.localRotation=Quaternion.Euler(0,0,0);
		tail0.transform.localRotation=Quaternion.Euler(0,0,0);
	}

	void enterGLIDING() {
		resetWings();
		shots=0;
	}

	void enterBarrel() {
		barrelStart=Time.time;
		moveVector=(barrel_right)?moveVector+barrelSpeed*transform.right : moveVector-barrelSpeed*transform.right;
	}

	void enterSlow() {
		slowStart=Time.time;
		resetWings();
	}

	void enterBoost() {
		boostStart=Time.time;
		boostV0=moveVector.magnitude;
		rwing.transform.localRotation=Quaternion.AngleAxis(45, transform.up);
		rwing.transform.localRotation=Quaternion.AngleAxis(45, transform.up);
		rwing.transform.localRotation=Quaternion.AngleAxis(45, transform.up);
		resetWings();
	}

	void updateBarrel(){
		float dt=Time.time-barrelStart;
		moveVector=(barrel_right)?moveVector-dt*0.3f*barrelSpeed*transform.right : moveVector+dt*0.3f*barrelSpeed*transform.right;
		if(barrel_right){
			glider_mesh.transform.Rotate(0f, 0f, Time.deltaTime*barrelSpeed*-60f*((barrelDuration-dt)/barrelDuration));
		}else{
			glider_mesh.transform.Rotate(0f, 0f, Time.deltaTime*barrelSpeed*60f*((barrelDuration-dt)/barrelDuration));
		}
		if(dt>barrelDuration){
			switchToGlideFSM();
		}
	}

	void updateSlow() {
		float dt=Time.time-slowStart;
		resetWings();
		rwing.transform.localRotation*=Quaternion.AngleAxis(5+Mathf.Min(-inputBoost, 1f)*45, glider_mesh.transform.right);
		lwing.transform.localRotation*=Quaternion.AngleAxis(5+Mathf.Min(-inputBoost, 1f)*45, glider_mesh.transform.right);
		moveVector=moveVector*Mathf.Max(0.98f,1f-(0.02f*Mathf.Abs(inputBoost)));//negative axis input
		if(moveVector.magnitude<3f){
			//moveVector=Vector3.ClampMagnitude(moveVector,2f);
			moveVector=3f*Vector3.Normalize(moveVector);
		}
		if(Mathf.Abs(inputBoost)<0.2f || dt>1.5f){
			moveSpeed=(moveSpeed/2f<baseMoveSpeed)?baseMoveSpeed:moveSpeed/2f;
			switchToGlideFSM();
		}
		canSlow=false;

		if(inputRoll>0.1 || inputRoll<-0.1){
			rollAmount-=2.2f*inputRoll*(2-(rollAmount/80));
			rollAmount=(rollAmount>50f)? 50f : rollAmount;
			rollAmount=(rollAmount<-50f)? -50f : rollAmount;
		}else{
			rollAmount=(rollAmount>0f)? rollAmount-(3*(rollAmount/50f)) : rollAmount-(3*(rollAmount/50f));
			rollAmount=(rollAmount<1 && rollAmount>-1)?0f : rollAmount;
			rollAmount=(rollAmount>50f)? 50f : rollAmount;
			rollAmount=(rollAmount<-50f)? -50f : rollAmount;
		}
		float ra=Mathf.Abs(rollAmount)/2;
		if(inputTilt>0.1 || inputTilt<-0.1){
			tiltAmount-=inputTilt*(tiltSpeed-(tiltAmount/80f));
			tiltAmount=(tiltAmount>80f-ra)? 80f-ra : tiltAmount;
			tiltAmount=(tiltAmount<-80f+ra)? -80f+ra : tiltAmount;
		}else{
			tiltAmount=(tiltAmount>0f)?tiltAmount-3f : tiltAmount+3f;
			tiltAmount=(tiltAmount<1.5 && tiltAmount>-1.5)?0f : tiltAmount;
			tiltAmount=(tiltAmount>80f-ra)? 80f-ra : tiltAmount;
			tiltAmount=(tiltAmount<-80f+ra)? -80f+ra : tiltAmount;
		}

		transform.Rotate(0f, inputRoll*rollSpeed, 0f);
		glider_mesh.transform.localRotation=Quaternion.Euler(new Vector3(tiltAmount, 0f, 0f));
		glider_mesh.transform.localRotation*=Quaternion.AngleAxis(rollAmount, Vector3.forward);
		moveVector=moveVector.magnitude*Vector3.Normalize(glider_mesh.transform.forward);
		
		if(Input.GetKey(KeyCode.JoystickButton5)){
			if(shots<5){
				fireBall();
				shots+=10;
			}
		}
	}
	public void maintain_circle(){
		rollAmount=45f;
		transform.Rotate(0f, -0.3f*rollSpeed, 0f);
	}
	
	void updateBoost() {
		float dt=Time.time-boostStart;
		float rad=Mathf.Sin(dt*5f);
		dt*=1.7f;
		lwing.transform.localPosition=new Vector3(-1.65f+0.5f*Mathf.Asin(rad), -Mathf.Asin(rad), 0f);
		lwing.transform.rotation=Quaternion.AngleAxis(rad*80f,transform.forward);
		lwing.transform.rotation*=Quaternion.AngleAxis(90*dt, transform.right);
			
		rwing.transform.localPosition=new Vector3(1.65f-0.5f*Mathf.Asin(rad), -Mathf.Asin(rad), 0f);
		rwing.transform.rotation=Quaternion.AngleAxis(rad*-80f,transform.forward);
		rwing.transform.rotation*=Quaternion.AngleAxis(90*dt, transform.right);

		if(moveVector.magnitude>boostV0*(1f+0.5f*(1f-Mathf.Abs(tiltAmount)))){
			moveVector=Vector3.ClampMagnitude(moveVector, boostV0*1.5f);	
		}
		if(inputBoost<0.2f || dt>0.7f){
			switchToGlideFSM();
		}else{

			rollAmount=(rollAmount>0f)? rollAmount-(3*(rollAmount/50f)) : rollAmount-(3*(rollAmount/50f));
			rollAmount=(rollAmount<1 && rollAmount>-1)?0f : rollAmount;
			rollAmount=(rollAmount>50f)? 50f : rollAmount;
			rollAmount=(rollAmount<-50f)? -50f : rollAmount;

			float ra=Mathf.Abs(rollAmount)/2;
			tiltAmount=(tiltAmount>0f)?tiltAmount-3f : tiltAmount+3f;
			tiltAmount=(tiltAmount<1.5 && tiltAmount>-1.5)?0f : tiltAmount;
			tiltAmount=(tiltAmount>80f-ra)? 80f-ra : tiltAmount;
			tiltAmount=(tiltAmount<-80f+ra)? -80f+ra : tiltAmount;
			moveVector=moveVector*Mathf.Max(1.0f, 2f*(Mathf.Abs(inputBoost)));//negative axis input
			glider_mesh.transform.localRotation=Quaternion.Euler(new Vector3(tiltAmount, 0f, 0f));
			glider_mesh.transform.localRotation*=Quaternion.AngleAxis(rollAmount, Vector3.forward);
		}
		if(moveVector.magnitude<maxBoost){
			moveSpeed=moveVector.magnitude;
		}
		else{
			moveSpeed=maxBoost;
		}
		canBoost=false;
		moveVector=moveVector.magnitude*Vector3.Normalize(glider_mesh.transform.forward);
	}

	void updateGLIDING() {
		if(inputRoll>0.1 || inputRoll<-0.1){
			rollAmount-=2.2f*inputRoll*(2-(rollAmount/80));
			rollAmount=(rollAmount>50f)? 50f : rollAmount;
			rollAmount=(rollAmount<-50f)? -50f : rollAmount;
		}else{
			rollAmount=(rollAmount>0f)? rollAmount-(3*(rollAmount/50f)) : rollAmount-(3*(rollAmount/50f));
			rollAmount=(rollAmount<1 && rollAmount>-1)?0f : rollAmount;
			rollAmount=(rollAmount>50f)? 50f : rollAmount;
			rollAmount=(rollAmount<-50f)? -50f : rollAmount;
		}
		
		float ra=Mathf.Abs(rollAmount)/2;
		if(inputTilt>0.1 || inputTilt<-0.1){
			tiltAmount-=inputTilt*(tiltSpeed-(tiltAmount/80f));
			tiltAmount=(tiltAmount>80f-ra)? 80f-ra : tiltAmount;
			tiltAmount=(tiltAmount<-80f+ra)? -80f+ra : tiltAmount;
		}else{
			tiltAmount=(tiltAmount>0f)?tiltAmount-3f : tiltAmount+3f;
			tiltAmount=(tiltAmount<1.5 && tiltAmount>-1.5)?0f : tiltAmount;
			tiltAmount=(tiltAmount>80f-ra)? 80f-ra : tiltAmount;
			tiltAmount=(tiltAmount<-80f+ra)? -80f+ra : tiltAmount;
		}

		transform.Rotate(0f, inputRoll*rollSpeed, 0f);
		glider_mesh.transform.localRotation=Quaternion.Euler(new Vector3(tiltAmount, 0f, 0f));
		glider_mesh.transform.localRotation*=Quaternion.AngleAxis(rollAmount, Vector3.forward);
		float ttc=(tiltAmount>0)?Mathf.Max(1f+(tiltAmount/80f)*0.3f,1f):1f;
		ttc=(tiltAmount<-50f)?0.5f:ttc;
		tail0.transform.localPosition=new Vector3(0f, Mathf.Sin(15f*Time.time*ttc)*0.3f, -1.1f);
		tail1.transform.localPosition=new Vector3(0f, Mathf.Sin(15f*Time.time*ttc+1)*0.3f, -2.3f);
		//transform.Rotate(tiltAmount * tiltSpeed, 0f, 0f);

		moveVector = glider_mesh.transform.TransformDirection(new Vector3(0f, 0f, 1f) * moveSpeed);

		float g_accel=-moveVector.normalized.y;
		g_accel=(moveVector.y>0)?g_accel*0.95f:g_accel;

		moveSpeed=(moveSpeed>baseMoveSpeed*Mathf.Cos(Mathf.Deg2Rad*tiltAmount))? 0.2f*g_accel+moveSpeed : baseMoveSpeed*Mathf.Cos(Mathf.Deg2Rad*tiltAmount);
		moveSpeed=(moveSpeed>baseMoveSpeed+1)?moveSpeed-windSlow:moveSpeed;
		if(moveSpeed>3f*baseMoveSpeed){
			wind.active=true;
			if(!hsw.isPlaying){
				hsw.Play();
			}
		}else{wind.active=false;}
		//moveVector = transform.TransformDirection(moveVector); //moveVector.y = jumpSpeed;
		if(tiltAmount<30f){
			float r=Random.Range(0f,4f);
			if(!flywind.isPlaying && !flywind1.isPlaying && !flywind2.isPlaying){
				if(r<2.5f){
					flywind.Play();
				}else if(r<3.6){
					flywind1.Play();
				}else{
					flywind2.Play();
				}
			}
		}
		
		if(isFlapping && Time.time-flapStart<flapDuration){
			float dt=Time.time-flapStart;
			float rad=Mathf.Sin(dt*5f);
			lwing.transform.localPosition=new Vector3(-1.65f, 0f, -Mathf.Asin(rad));
			lwing.transform.rotation=Quaternion.AngleAxis(rad*80f,transform.forward);
			lwing.transform.rotation*=Quaternion.AngleAxis(45, transform.up);
			
			rwing.transform.localPosition=new Vector3(1.65f, 0f, -Mathf.Asin(rad));
			rwing.transform.rotation=Quaternion.AngleAxis(rad*-80f,transform.forward);
			rwing.transform.rotation*=Quaternion.AngleAxis(45, transform.up);
				
			if(rad>0.5){
				moveVector=new Vector3(moveVector.x, moveVector.y+rad*10f, moveVector.z);
				if(p_flap1){
					if(!flap1.isPlaying && !flap.isPlaying){
						flap1.Play();
					}else{
						p_flap1=!p_flap1;
					}
				}else{
					if(!flap1.isPlaying && !flap.isPlaying){
						flap.Play();
					}else{
						p_flap1=!p_flap1;
					}
				}
			}
		}else{
			isFlapping=false;
			resetWings();
		}

		if(tiltAmount<-29){
			if(!isFlapping && moveSpeed<=baseMoveSpeed){
				isFlapping=true;
				flapStart=Time.time;
			}
		}

		if(Input.GetKeyDown(KeyCode.Space)){
			if(Input.GetKey(KeyCode.D)){
				barrel_right=true;
				switchToBarrelFSM();
			}else if(Input.GetKey(KeyCode.A)){
				barrel_right=false;
				switchToBarrelFSM();
			}else{
				fireBall();
			}
		}
		
		if(inputBoost>0.2f && canBoost){
			switchToBoostFSM();	
		}
		if(inputBoost<0.2f){
			canBoost=true;
		}
		if(inputBoost>-0.2f){
			canSlow=true;
		}

		if(inputBoost<-0.2f && canSlow){
			switchToSlowFSM();	
			
		}

		if(Input.GetKey(KeyCode.JoystickButton7) || Input.GetKey(KeyCode.E) || Input.GetAxis("BarrelLeft")>0.1){
			barrel_right=true;
			switchToBarrelFSM();
		}
		
		if(Input.GetKey(KeyCode.JoystickButton6) || Input.GetKey(KeyCode.Q)  || Input.GetAxis("BarrelRight")>0.1){
			barrel_right=false;
			switchToBarrelFSM();
		}

		if(Input.GetKey(KeyCode.JoystickButton5) || Input.GetAxis("Fire")>0.1){
		       	if(shots<5){
				fireBall();
				shots+=10;
			}
		}
		
		moveVector=new Vector3(moveVector.x, moveVector.y+flapAmount,moveVector.z);
		flapAmount=(flapAmount>0.2)? flapAmount-gravity*Time.deltaTime:0;
		if(shots>20){
			//switchToFlapFSM();
		}

		if(stunTime>0){
			rotate_parts();
			//moveVector=moveVector+10f*Vector3.up;
		}
		if(stunTime>1f){
			//resettingRot=true;
			transform.rigidbody.velocity=new Vector3(0,0,0);
			transform.rigidbody.angularVelocity=new Vector3(0,0,0);
			resetWings();
			glider_mesh.transform.localRotation=Quaternion.AngleAxis(0, transform.up);

		}

		if(resettingRot){
			transform.rotation=Quaternion.RotateTowards(transform.rotation, initialRotation, 75f*Time.deltaTime);	
			if(transform.rotation==initialRotation){
				transform.rigidbody.velocity=new Vector3(0,0,0);
				resettingRot=false;
				moveSpeed/=2f;
				stunTime=0f;
			}
		}else{
			initialRotation*=Quaternion.AngleAxis(10f, Vector3.up);
		}
		

	}

	void exitGLIDING () {
		// reset the avatar values we mucked with to known values
		rwing.transform.rotation = Quaternion.Euler (0,0,0);
		lwing.transform.rotation = Quaternion.Euler (0,0,0);
		shots=0;
	}

	void fireBall() {
	    Rigidbody rocketClone = (Rigidbody) Instantiate(fireball, transform.position + new Vector3(0f,0f,1.5f), transform.rotation);
	    rocketClone.GetComponent<fireball>().velocity=Mathf.Max(30f, moveSpeed*5f); rocketClone.GetComponent<fireball>().transform.rotation=transform.rotation*glider_mesh.transform.localRotation;
	    ffire.Play();
	}
	void rotate_parts(){
		glider_mesh.transform.localPosition=new Vector3(Mathf.Sin(Time.time*200f)*0.5f, 0.5f*Mathf.Sin(200f*Time.time+1.7f), 0f);
		glider_mesh.transform.localRotation=Quaternion.Euler(confRot*Time.deltaTime, 2f*confRot*Time.deltaTime, -30f*confRot*Time.deltaTime);
		rwing.transform.localRotation=Quaternion.Euler(confRot*Time.deltaTime, 20f*confRot*Time.deltaTime, 300f*confRot*Time.deltaTime);
		lwing.transform.localRotation=Quaternion.Euler(confRot*Time.deltaTime, 20f*confRot*Time.deltaTime, -300f*confRot*Time.deltaTime);
		tail0.transform.localRotation=Quaternion.Euler(confRot*Time.deltaTime, 20f*confRot*Time.deltaTime, 200f*confRot*Time.deltaTime);
		tail1.transform.localRotation=Quaternion.Euler(confRot*Time.deltaTime, 20f*confRot*Time.deltaTime, 59f*confRot*Time.deltaTime);
	}
	
	
	// Update is called once per frame
	void Update() {
		if(shots>0){shots--;}
		// set some globals used by the state machine!
		inputRoll = Input.GetAxis("Horizontal");
		inputTilt = Input.GetAxis("Vertical");
		inputBoost = Input.GetAxis("VerticalBoost");

		//horizontalVelocity = new Vector3(transform.rigidbody.velocity.x, 0, transform.rigidbody.velocity.z);
		horizontalVelocity = transform.InverseTransformDirection(horizontalVelocity);
		horizontalSpeed = horizontalVelocity.z;  // want the plus or minus on speed
		//verticalSpeed = transform.rigidbody.velocity.y;

		stateMachine.Execute();

		transform.position=transform.position+(moveVector * Time.deltaTime);
		if(transform.rigidbody.angularVelocity==new Vector3(0,0,0)){
			initialRotation=transform.rotation;
			stunTime=0f;
		}else{
			stunTime+=Time.deltaTime;
		}

		health_text.text="Health: "+health.ToString();
		//gold_text.text="Gold: "+gold.ToString();
		
		SetLegendaryObjectCountText();
	}
	
	void OnCollisionEnter(Collision col){
		Debug.Log(col.gameObject.tag);
		if(col.gameObject.CompareTag("projectile")){
			//ADD SOUND HERE and SOUND EFFECT PUBLIC REFERENCE ON TOP
			//colsnd.mute=false;
			//colsnd.Play();
			Vector3 vel=moveVector.normalized+col.rigidbody.velocity.normalized;
			transform.rigidbody.velocity=10f*vel;
			health-=10;
			if( health < 0 )
			{
				health = 0;
			}
		}else{
			transform.rigidbody.velocity=Vector3.Reflect(glider_mesh.transform.forward,col.contacts[0].normal);
			health--;
		}
		if(col.gameObject.CompareTag("destructable")){
			GameObject[] sub=GameObject.FindGameObjectsWithTag("destructable");
			foreach(GameObject g in sub){
				//temp hacky way to fix catapult-dragon collision deactivate all arms...
				if(Vector3.Distance(transform.position, g.transform.position)<10){
					g.rigidbody.isKinematic=false;
				}
			}
		}
	}

	void OnTriggerEnter(Collider other) 
	{
		Debug.Log(other);
		Debug.Log(other.gameObject.tag);
		if(other.gameObject.tag == "LegendaryPickUp" && collecting_object == false ) 
		{
            collecting_object = true;
            pickup_sound.Play();
			other.gameObject.SetActive (false);
			legendary_object_count++;
			SetLegendaryObjectCountText();
		}
	}

    void OnTriggerExit( Collider other )
    {
        if( other.gameObject.tag == "LegendaryPickUp" && collecting_object == true ) 
        {
            collecting_object = false;
        }
    }

	void SetLegendaryObjectCountText()
	{
		legendary_object_text.text = "Legendary Armour: " + legendary_object_count.ToString() + "/5";
	}
}
