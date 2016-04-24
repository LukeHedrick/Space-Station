using UnityEngine;
using System.Collections;

//use the Generic system here to make use of a Flocker list later on
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]

abstract public class Vehicle : MonoBehaviour {

    //-----------------------------------------------------------------------
    // Class Fields
    //-----------------------------------------------------------------------

    //movement
    protected Vector3 acceleration;
    protected Vector3 velocity;
    protected Vector3 desired;
	public bool drunk;

    public Vector3 Velocity {
        get { return velocity; }
    }

    //public for changing in Inspector
    //define movement behaviors
    public float maxSpeed = 6.0f;
    public float maxForce = 12.0f;
    public float mass = 1.0f;
    public float radius = 1.0f;

    //access to Character Controller component
    CharacterController charControl;

    //Access to GameManager script
    protected GameManager gm;


    abstract protected void CalcSteeringForces();


    //-----------------------------------------------------------------------
    // Start and Update
    //-----------------------------------------------------------------------
	virtual public void Start(){
        //acceleration = new Vector3 (0, 0, 0);     
        acceleration = Vector3.zero;
        velocity = transform.forward;
        charControl = GetComponent<CharacterController>();

        gm = GameObject.Find("GameManagerGO").GetComponent<GameManager>(); 
	}

	
	// Update is called once per frame
	protected void Update () {
        //calculate all necessary steering forces
        CalcSteeringForces();

        //add accel to vel
        velocity += acceleration * Time.deltaTime;
        velocity.y = 0; //keeping us on same plane

        //limit vel to max speed
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        //turn googles
        transform.forward = velocity.normalized;

        //move the character based on velocity
        charControl.Move(velocity * Time.deltaTime);

        //reset acceleration to 0
        acceleration = Vector3.zero;
	}


    //-----------------------------------------------------------------------
    // Class Methods
    //-----------------------------------------------------------------------

    protected void ApplyForce(Vector3 steeringForce) {
        acceleration += steeringForce / mass;
    }

    protected Vector3 Seek(Vector3 targetPos) {
        desired = targetPos - transform.position;
        desired = desired.normalized * maxSpeed;
        desired -= velocity;
        desired.y = 0;
        return desired;
    }

	protected Vector3 Arrive(Vector3 targetPos){
		desired = targetPos = transform.position;
		float dist = desired.magnitude;
		desired.Normalize ();
		if (dist < 50) {
			desired = desired * (50 - dist/50) * maxSpeed;
		} else {
			desired = desired * maxSpeed;
		}
		return desired;
	}

	protected GameObject getNeighborAhead(float queueDist){
		//set up vectors for finding if someone is ahead of the current flocker
		Vector3 qa = velocity.normalized * queueDist;
		Vector3 ahead = transform.position + qa;

		if (drunk == true) {
			//go through the list of flockers and find the nearest flockmate
			for (int i = 0; i < gm.numberFlockers; i++) {
				GameObject neighbor = gm.FlockD [i];
				float dist = Vector3.Distance (ahead, neighbor.transform.position);
				print (dist + " " + i);
				if (Vector3.Distance (transform.position, neighbor.transform.position) != 0.0f && dist <= queueDist) {
					return neighbor;
				}
			}
		} else {
			//go through the list of flockers and find the nearest flockmate
			for (int i = 0; i < gm.numberFlockers; i++) {
				GameObject neighbor = gm.FlockS [i];
				float dist = Vector3.Distance (ahead, neighbor.transform.position);
				print (dist + " " + i);
				if (Vector3.Distance (transform.position, neighbor.transform.position) != 0.0f && dist <= queueDist) {
					return neighbor;
				}
			}
		}
		return null;
	}

	protected void Queuing(float queueDist){
		//get the closest neighbor
		GameObject neighbor = getNeighborAhead (queueDist);
        
        if (neighbor != null)
        {
            Debug.DrawLine(transform.position, neighbor.transform.position, Color.green);
        }

        //if there is a neighbor, execute, else, don't
		if (neighbor != null) {
			velocity = velocity * 0.3f;
			print ("Totes too close");
            print("Current Velocity: " + velocity.magnitude);
		}
	}

    protected Vector3 AvoidObstacle(GameObject ob, float safe)
    {

        //reset desired velocity
        desired = Vector3.zero;

        //get radius from obstacle's script
        float obRad = ob.GetComponent<ObstacleScript>().Radius;

        //get vector from vehicle to obstacle
        Vector3 vecToCenter = ob.transform.position - transform.position;

        //zero-out y component (only necessary when working on X-Z plane)
        vecToCenter.y = 0;

        //if object is out of my safe zone, ignore it
        if (vecToCenter.magnitude > safe)
        {
            return Vector3.zero;
        }

        //if object is behind me, ignore it
        if (Vector3.Dot(vecToCenter, transform.forward) < 0)
        {
            return Vector3.zero;
        }

        //if object is not in my forward path, ignore it
        if (Mathf.Abs(Vector3.Dot(vecToCenter, transform.right)) > obRad + radius)
        {
            return Vector3.zero;
        }

        //if we get this far, we will collide with an obstacle!
        //object on left, steer right
        if (Vector3.Dot(vecToCenter, transform.right) < 0)
        {
            desired = transform.right * maxSpeed;

            //debug line to see if the dude is avoiding to the right
            Debug.DrawLine(transform.position, ob.transform.position, Color.red);
        }
        else
        {
            desired = transform.right * -maxSpeed;

            //debug line to see if the dude is avoiding to the left
            Debug.DrawLine(transform.position, ob.transform.position, Color.green);
        }

        return desired;
    }

    public Vector3 Separation(float separationDistance) {
        float[] distance = new float[gm.numberFlockers];
        Vector3[] fleeingForce = new Vector3[gm.numberFlockers];
        Vector3 summedForce = Vector3.zero;
		for (int i = 0; i < gm.numberFlockers; i++) {
			if (Vector3.Distance (transform.position, gm.FlockD [i].transform.position) != 0.0f && Vector3.Distance (transform.position, gm.FlockD [i].transform.position) < separationDistance) {
				distance [i] = Vector3.Distance (transform.position, gm.FlockD [i].transform.position);
			}
			fleeingForce [i] = -1 * Seek (gm.FlockD [i].transform.position);
			fleeingForce [i].Normalize ();
		}
		for (int i = 0; i < gm.numberFlockers; i++) {
			if (Vector3.Distance (transform.position, gm.FlockS [i].transform.position) != 0.0f && Vector3.Distance (transform.position, gm.FlockS [i].transform.position) < separationDistance) {
				distance [i] = Vector3.Distance (transform.position, gm.FlockS [i].transform.position);
			}
			fleeingForce [i] = -1 * Seek (gm.FlockS [i].transform.position);
			fleeingForce [i].Normalize ();
		}
        for (int i = 0; i < gm.numberFlockers; i++)
        {
            if (distance[i] != 0.0f)
            {
                summedForce += fleeingForce[i] / distance[i];
            }
        }
        if (summedForce.magnitude > 0) 
        {
            summedForce.Normalize();
            summedForce = summedForce * maxSpeed;
            summedForce -= velocity;
        }
        return summedForce;
    }

    public Vector3 Alignment(Vector3 alignVector)   {
        Vector3 desired = Vector3.zero;
        desired = alignVector * maxSpeed;
        return desired - velocity;
    }
    public Vector3 Cohesion(Vector3 cohesionVector) {
        return Seek(cohesionVector);
    }
}
