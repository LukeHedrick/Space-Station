
using UnityEngine;
using System.Collections;

//add using System.Collections.Generic; to use the generic list format
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    //-----------------------------------------------------------------------
    // Class Fields
    //-----------------------------------------------------------------------
    public GameObject drunk;
    public GameObject targetD;	
	public GameObject sober;
	public GameObject targetS;

    public GameObject drunkPrefab;
	public GameObject soberPrefab;
    public GameObject targetDPrefab;
	public GameObject targetSPrefab;
    public GameObject obstaclePrefab;

    private GameObject[] obstacles;

    //flocker attributes
    public int numberFlockers = 3;
    private List<GameObject> flockD;
    public Vector3 centriodD;
    public Vector3 flockDirectionD;
	private List<GameObject> flockS;
	public Vector3 centriodS;
	public Vector3 flockDirectionS;

    //-----------------------------------------------------------------------
    // Start and Update
    //-----------------------------------------------------------------------
	void Start () {
        //drunk
		//Create the target (noodle)
		Vector3 pos = new Vector3(115.250f, 4.11f, -3.250f);
        targetD = (GameObject)Instantiate(targetDPrefab, pos, Quaternion.identity);

        flockD = new List<GameObject>();

        //Create the GooglyEye Guy at (10, 1, 10)
        pos = new Vector3(122.19f, 1f, -4.91f);
		drunk = (GameObject)Instantiate(drunkPrefab, pos, Quaternion.identity);
		drunk.GetComponent<Seeker> ().drunk = true;

        //set googles's target
		drunk.GetComponent<Seeker>().seekerTarget = targetD;
		flockD.Add(drunk);
		
		//Create obstacles and place them in the obstacles array
        obstacles = GameObject.FindGameObjectsWithTag("Obstacle");

        //create flock
		//flocker 1
		pos = new Vector3(126.56f, 1f, -5.83f);
		drunk = (GameObject)Instantiate(drunkPrefab, pos, Quaternion.identity);
		drunk.GetComponent<Seeker> ().drunk = true;
		drunk.GetComponent<Seeker>().seekerTarget = targetD;
		flockD.Add(drunk);

		//flocker 2
		pos = new Vector3(124.3f, 1f, -8.35f);
		drunk = (GameObject)Instantiate(drunkPrefab, pos, Quaternion.identity);
		drunk.GetComponent<Seeker> ().drunk = true;
		drunk.GetComponent<Seeker>().seekerTarget = targetD;
		flockD.Add(drunk);

		//sober
		//Create the target (noodle)
		pos = new Vector3(115.250f, 4.11f, -3.250f);
		targetS = (GameObject)Instantiate(targetSPrefab, pos, Quaternion.identity);
		
		flockS = new List<GameObject>();
		
		//Create the GooglyEye Guy at (10, 1, 10)
		pos = new Vector3(122.19f, 1f, -4.91f);
		sober = (GameObject)Instantiate(soberPrefab, pos, Quaternion.identity);
		sober.GetComponent<Seeker> ().drunk = false;
		
		//set googles's target
		sober.GetComponent<Seeker>().seekerTarget = targetS;
		flockS.Add(sober);
		
		//Create obstacles and place them in the obstacles array
		/*for (int i = 0; i < 20; i++)
        {
            pos = new Vector3(Random.Range(-30.0f, 30.0f), 1.1f, Random.Range(-30.0f, 30.0f));
            Quaternion rot = Quaternion.Euler(new Vector2(0, Random.Range(0.0f, 180.0f)));
            Instantiate(obstaclePrefab, pos, rot);
        }

        obstacles = GameObject.FindGameObjectsWithTag("Obstacle");*/
		
		//create flock
		//flocker 1
		pos = new Vector3(126.56f, 1f, -5.83f);
		sober = (GameObject)Instantiate(soberPrefab, pos, Quaternion.identity);
		sober.GetComponent<Seeker> ().drunk = false;
		sober.GetComponent<Seeker>().seekerTarget = targetS;
		flockS.Add(sober);
		
		//flocker 2
		pos = new Vector3(124.3f, 1f, -8.35f);
		sober = (GameObject)Instantiate(soberPrefab, pos, Quaternion.identity);
		sober.GetComponent<Seeker> ().drunk = false;
		sober.GetComponent<Seeker>().seekerTarget = targetS;
		flockS.Add(sober);
	}
	

	void Update () {
		//drunk
        CalcCentriodD();
        CalcFlockDirectionD();

		//sober
		CalcCentriodS();
		CalcFlockDirectionS();
	}

    bool NearAnObstacle()
    {
        //iterate through all obstacles and compare the distance between each obstacle and the noodle
        //if the noodle is within a 4 unit distance of the noodle, return true
        for (int i = 0; i < obstacles.Length; i++)
        {
            if (Vector3.Distance(targetD.transform.position, obstacles[i].transform.position) < 5.0f)
            {
                return true;
            }
        }
        //otherwise, the noodle is not near an obstacle
        return false;
    }

	//set up properties for the Obstacles array, Flock list, Centroid vector, and FlockDirection vector
    public GameObject[] Obstacles
    {
        get { return obstacles; }
    }

    public List<GameObject> FlockD
    {
        get { return flockD; }
    }

    public Vector3 CentriodD
    {
        get { return centriodD; }
    }

    public Vector3 FlockDirectionD
    {
        get { return flockDirectionD; }
    }

	public List<GameObject> FlockS
	{
		get { return flockS; }
	}
	
	public Vector3 CentriodS
	{
		get { return centriodS; }
	}
	
	public Vector3 FlockDirectionS
	{
		get { return flockDirectionS; }
	}

    //-----------------------------------------------------------------------
    // Flocking Methods
    //-----------------------------------------------------------------------
    //method to find the centroid
	void CalcCentriodD()
    {
        Vector3 flockSumPos = Vector3.zero;
        
		for (int i = 0; i < numberFlockers; i++ )
        {
            flockSumPos += flockD[i].transform.position;
        }
        
		centriodD = flockSumPos / numberFlockers;
    }

	//method to find average flock direction
    void CalcFlockDirectionD()
    {
        Vector3 flockSumDirection = Vector3.zero;
        
		for (int i = 0; i < numberFlockers; i++)
        {
            flockSumDirection += flockD[i].GetComponent<Seeker>().Velocity.normalized;
        }
        
		flockDirectionD = flockSumDirection / numberFlockers;
    }

	//method to find the centroid
	void CalcCentriodS()
	{
		Vector3 flockSumPos = Vector3.zero;
		
		for (int i = 0; i < numberFlockers; i++ )
		{
			flockSumPos += flockS[i].transform.position;
		}
		
		centriodS = flockSumPos / numberFlockers;
	}
	
	//method to find average flock direction
	void CalcFlockDirectionS()
	{
		Vector3 flockSumDirection = Vector3.zero;
		
		for (int i = 0; i < numberFlockers; i++)
		{
			flockSumDirection += flockS[i].GetComponent<Seeker>().Velocity.normalized;
		}
		
		flockDirectionS = flockSumDirection / numberFlockers;
	}
}
