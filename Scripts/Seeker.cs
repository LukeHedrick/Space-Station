using UnityEngine;
using System.Collections;

public class Seeker : Vehicle {

    //-----------------------------------------------------------------------
    // Class Fields
    //-----------------------------------------------------------------------
    public GameObject seekerTarget;

    //Seeker's steering force (will be added to acceleration)
    private Vector3 force;

    //WEIGHTING!!!!
    public float seekWeight = 75.0f;
    public float separationDistance = 3.0f;
    public float queueingDistance = 5.0f;
    public float safeDistance = 10.0f;
    public float avoidWeight = 100.0f;
    public float separationWeight = 10.0f;
    public float alignWeight = 9.0f;
    //-----------------------------------------------------------------------
    // Start - No Update
    //-----------------------------------------------------------------------
	// Call Inherited Start and then do our own
	override public void Start () {
        //call parent's start
		base.Start();

        //initialize
        force = Vector3.zero;
        safeDistance = 10.0f;
        avoidWeight = 10.0f;
	}

    //-----------------------------------------------------------------------
    // Class Methods
    //-----------------------------------------------------------------------

    protected override void CalcSteeringForces() {
        //reset value to (0, 0, 0)
        force = Vector3.zero;

        //got a seeking force
        force += Seek(seekerTarget.transform.position) * seekWeight;

        //limited the seeker's steering force
        force = Vector3.ClampMagnitude(force, maxForce);

        //applied the steering force to this Vehicle's acceleration (ApplyForce)
        ApplyForce(force);
        
        //applies force to avoid obstacles
        for(int i = 0; i < gm.Obstacles.Length; i++) {
            ApplyForce(AvoidObstacle(gm.Obstacles[i], safeDistance)*avoidWeight);
        }

		if (drunk == true) {
			//applies force to align the flock
			ApplyForce (Alignment (gm.flockDirectionD) * alignWeight);

			//applies force to cohese the flock
			ApplyForce (Cohesion (gm.centriodD));
		} else {
			//applies force to align the flock
			ApplyForce (Alignment (gm.flockDirectionS) * alignWeight);
			
			//applies force to cohese the flock
			ApplyForce (Cohesion (gm.centriodS));
		}

        //applies force to separate the flock
        if (Separation(separationDistance).x != 0)
        {
            print("Separating");
        }
        ApplyForce(Separation(separationDistance) * separationWeight);

		//possible queueing 
		Queuing (queueingDistance);
    }
}
