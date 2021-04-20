using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	private GameObject playerCam;
	private GameObject playerStart;
	private GameObject playerGoal;

	private GameObject camPos;
	private GameObject ballPos;

	public GameObject ball;
	public GameObject pointer;
	private Rigidbody ballRB;

	public Vector3 shotAngleVectorDefault = new Vector3 (-2.0f, 0.15f, 0.0f);
	private Vector3 shotAngleVector;
	private float shotAngle = 0.0f;
	private float mouseMoveCumulativeTotal;
	private float shotForce;

	public float minStopSpeed;
	public float forceRangeMin;
	public float forceRangeMax;
	public float forcePingPongSpeed;
	public float angleMultiplier;

	public bool isShooting = false;
	public bool afterShot = false;
	private int pingPongDir = 1;
	public bool disableInput = false;
	private int shotCount = 0;

	//----- will get dropped if prefab removed from scene

	public Slider powerSlider;
	public Text shotCountText;
	public Text parText;

	public GameObject goalObject;
	public GameObject zKillObject;

	//---------------------------------------------------

	private GameObject b;
	private GameObject p;

	private Vector3 lastPos;

	public enum gameState {waiting,aiming,shooting};
	public gameState currentState = gameState.waiting;

	void Start ()
	{
		// should auto assign UI elements too at some point
		// slider
		// shot count text
		// par text

		playerCam = GameObject.FindGameObjectsWithTag("MainCamera")[0];
		playerStart = GameObject.FindGameObjectsWithTag("Start")[0];
		playerGoal = GameObject.FindGameObjectsWithTag("Goal")[0];

		ballPos = playerStart.transform.GetChild (1).gameObject;		/// this seems super fragile, should probably fix this
		camPos = playerStart.transform.GetChild (2).gameObject;

		playerCam.transform.position = camPos.transform.position;
		playerCam.transform.rotation = camPos.transform.rotation;

		b = Instantiate (ball, ballPos.transform.position, ballPos.transform.rotation) as GameObject;
		p = Instantiate (pointer, b.transform.position, b.transform.rotation) as GameObject;
		p.SetActive (false);

		ballRB = b.GetComponent<Rigidbody> ();

		playerCam.GetComponent<LookAtObject> ().target = b;

		shotForce = forceRangeMin;
		shotCountText.text = "0";
		shotAngleVector = shotAngleVectorDefault;

		parText.text = goalObject.GetComponent<Hole> ().par.ToString();
	}
	
	void Update ()
	{
		shotAngleVector = Quaternion.AngleAxis (shotAngle, Vector3.up) * shotAngleVector;		// calculate this properly next

		if (currentState == gameState.waiting)
		{
			if (disableInput == false)
			{
				if (Input.GetMouseButtonDown (0))
				{
					StartShot ();
					currentState = gameState.aiming;
				}
			}
		}
		else if (currentState == gameState.aiming)
		{
			DoShot ();
			if (Input.GetMouseButtonUp (0))
			{
				EndShot ();
				currentState = gameState.shooting;
			}
		}
		else if (currentState == gameState.shooting)
		{
			CheckBall ();
		}

		CheckHeight ();
	}

	//-------------------------------------------------

	private void StartShot()
	{
		isShooting = true;
		disableInput = true;
		p.SetActive (true);
	}

	private void DoShot()
	{
		if (isShooting == true)
			shotForce += forcePingPongSpeed * pingPongDir;

		if (shotForce >= forceRangeMax)
			pingPongDir = -1;

		if (shotForce <= forceRangeMin)
			pingPongDir = 1;

		powerSlider.value = (shotForce - forceRangeMin) / (forceRangeMax - forceRangeMin);

		mouseMoveCumulativeTotal = (Input.GetAxis ("Mouse X"));
		shotAngle = mouseMoveCumulativeTotal * angleMultiplier;

		p.transform.position = b.transform.position;
		p.transform.rotation = Quaternion.LookRotation (shotAngleVector, Vector3.up);

	}

	private void EndShot()
	{
		lastPos = b.transform.position;
		p.SetActive (false);
		isShooting = false;
		afterShot = true;
		ballRB.AddForce (shotAngleVector * shotForce);	// this is where the ball gets hit
		shotForce = forceRangeMin;
		powerSlider.value = 0.0f;
		shotCount++;
		shotCountText.text = shotCount.ToString();
	}

	//-------------------------------------------------

	private void CheckBall()
	{
		if (ballRB.velocity.magnitude <= minStopSpeed)
		{
			ballRB.velocity = Vector3.zero;
			ballRB.angularVelocity = Vector3.zero;

			if (afterShot == true)
			{
				disableInput = false;
				afterShot = false;
			}
			
			shotAngleVector = shotAngleVectorDefault;
			currentState = gameState.waiting;
		}
	}

	private void CheckHeight()
	{
		if (b.transform.position.y <= zKillObject.transform.position.y)
		{
			b.transform.position = lastPos;
			ballRB.velocity = Vector3.zero;
			ballRB.angularVelocity = Vector3.zero;
		}
	}
}
