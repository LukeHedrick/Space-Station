using UnityEngine;
using System.Collections;

//use the Generic system here to make use of a Flocker list later on
using System.Collections.Generic;

public class DrunkPath : MonoBehaviour {
	//set up array
	private Vector3[] pos = new Vector3[5];
	private int stage = 0;
	private bool stepForward = true;

	//Access to GameManager script
	protected GameManager gm;

	// Use this for initialization
	void Start () {
		pos [0] = new Vector3 (115.250f, 4.11f, -3.250f);
		pos [1] = new Vector3 (65.30f, 4.11f, 0.0f);
		pos [2] = new Vector3 (14.30f, 4.11f, 7.70f);
		pos [3] = new Vector3 (8.30f, 4.11f, 7.70f);
		pos [4] = new Vector3 (-18.150f, 4.11f, 6.960f);

		gm = GameObject.Find("GameManagerGO").GetComponent<GameManager>(); 
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < gm.numberFlockers; i++) {
			float dist = Vector3.Distance(transform.position, gm.FlockD[i].transform.position);
			if (dist < 5.0f) {
				if(stepForward == true){
					stage++;
				}
				else{
					stage--;
				}
			}
			if (stage > 4)
			{
				stepForward = false;
			}
		}
		transform.position = pos[stage];
	}
}
