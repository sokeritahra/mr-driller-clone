using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxAudioController : MonoBehaviour {


	public AudioSource[] boxSfx;
	public AudioSource engine;
	public float power;
	public float adjustment;

	void Start () {
		
	}
	

	void Update () {
		
		Rigidbody rb = GetComponent<Rigidbody>();
		if(Input.GetKeyDown(KeyCode.Z)){
			rb.AddForce(Vector3.left * power,ForceMode.Impulse);
		}
		if(Input.GetKeyDown(KeyCode.X)){
			rb.AddForce(Vector3.right * power,ForceMode.Impulse);
		}
		
		float speed = (rb.velocity.x / adjustment) + 0.5f;
		engine.pitch = speed;

	}
	




	void OnCollisionEnter (Collision c){
		boxSfx[Random.Range(0,boxSfx.Length)].Play();
	}





}
