using UnityEngine;
using System;
using System.Collections.Generic;

/**
  * @class TransformationManager
  * @brief Description.
  * @author Sylvain Lafon
  * @see myMonoBehaviour
  */
[AddComponentMenu("Scripts/MiniGames/Transformation")]
public class TransformationManager : myMonoBehaviour {
	public System.Action<TransformationManager.Result> CallBack;
	
	public Gamer gamer {get; set;}
	public Ball ball {get; set;}
	
	private Quaternion initialRotation;

    public bool UseNegativeEdge = false;

	private float angle = 0;
	public  float angleSpeed;
	
	private float power = 0;
	public  float powerSpeed;

    public float minPower;
	public float maxPower;
	public float maxAngle;
	
	public bool infiniteTime = true;
	public float timeAngle = 0;
	public float timePower = 0;
	
	private Vector3 pos;
	private Vector3 dir;
	private float timeInAir;
	
	private float remainingTime = 0;
	
	private enum State {
		ANGLE,
		POWER,
		WAITING,
		FINISHED
	}
	
	public enum Result {
		NONE,
		TRANSFORMED,	
		GROUND,
		LIMIT
	}
	
	private Result transformed;	
	private State state;
	
	//public GameObject arrow;
    public MySlider2 arrow;
    private MySlider2 myArrow;
    //private GameObject myArrowPower;
		
	public void OnEnable() {
		angle = 0;
		power = 0;
		initialRotation = ball.Owner.transform.rotation;
		
		this.remainingTime = timeAngle;
		this.state = State.ANGLE;
		
		GameObject myArrowGO = GameObject.Instantiate(arrow.gameObject) as GameObject;
        if (!myArrowGO)
			throw new UnityException("Error : missing arrow");
        myArrow = myArrowGO.GetComponent<MySlider2>();
        if (!myArrowGO)
            throw new UnityException("Error : missing slider comp");
		
		myArrow.transform.parent = ball.Owner.transform;
		myArrow.transform.localPosition = new Vector3(0, 2, 0);
        myArrow.transform.localEulerAngles = Vector3.right * (85 * Mathf.Deg2Rad);

        //Transform jaugePower = myArrow.transform.FindChild("Power");
        //if (!jaugePower)
        //    throw new UnityException("Error : missing arrow -> power");
        //
        //myArrowPower = jaugePower.gameObject;
	}

    public GUIStyle timeStyle;
    public Rect timeRect;

    //public void OnGUI()
    //{
    //    if (this.state == State.ANGLE || this.state == State.POWER)
    //    {
    //        Rect rect = UIManager.screenRelativeRect(timeRect.x, timeRect.y, timeRect.width, timeRect.height);
    //
    //        if (!infiniteTime)
    //            GUI.Label(rect, "Time : " + (int)remainingTime, timeStyle);
    //    }
    //}
	
	public void Update() {
		
		if(state == State.ANGLE) {				
			angle += angleSpeed * Time.deltaTime;
			if(angle > maxAngle) {
				angle = maxAngle;
				angleSpeed *= -1;
			}
			if(angle < -maxAngle) {
				angle = -maxAngle;
				angleSpeed *= -1;
			}
			
			if(!infiniteTime) {
				remainingTime -= Time.deltaTime;	
			}				
				
			if(remainingTime < 0 || Input.GetKeyDown(Game.instance.settings.Inputs.conversionTouch.keyboard(gamer.Team)) || (gamer.XboxController.IsConnected && gamer.XboxController.GetButtonDown(Game.instance.settings.Inputs.conversionTouch.xbox))) {
				remainingTime = timePower;
				state = State.POWER;	
			}

            myArrow.transform.rotation = initialRotation * Quaternion.Euler(new Vector3(0, angle, 0));
		}
		
		else if(state == State.POWER) {
					
			power += powerSpeed * Time.deltaTime;
			if(power > 1) {
				power = 1;
				powerSpeed *= -1;
			}
			if(power < 0) {
				power = 0;
				powerSpeed *= -1;
			}

            //Vector3 scale = myArrowPower.transform.localScale;
            //scale.z = power;
            //myArrowPower.transform.localScale = scale;

            myArrow.percent = power;

			if(!infiniteTime) 
            {
				remainingTime -= Time.deltaTime;	
			}

            if (remainingTime < 0) 
            {
                Launch();
            }
            else if (UseNegativeEdge)
            {
                if ((gamer.XboxController.IsConnected && !gamer.XboxController.GetButton(Game.instance.settings.Inputs.conversionTouch.xbox)) || Input.GetKeyUp(Game.instance.settings.Inputs.conversionTouch.keyboard(gamer.Team)))
                {
                    Launch();
                }
            }
            else
            {
                if (gamer.XboxController.GetButtonDown(Game.instance.settings.Inputs.conversionTouch.xbox) || Input.GetKeyDown(Game.instance.settings.Inputs.conversionTouch.keyboard(gamer.Team)))
                {
                    Launch();
                }
            }						
		}
		
		else if(state == State.WAITING) {
            timeInAir += Time.deltaTime;
            doTransfo(timeInAir);

			if(ball.isOnGround()) {
                transformed = Result.GROUND;
				Finish ();	
			}			
		}
	}
	
	public void OnLimit() {
		transformed = Result.LIMIT;
		Finish ();
	}
	
	public void But() {
		transformed = Result.TRANSFORMED;
		Finish ();
	}
	
	public Action OnLaunch;
	
	private void Launch() {
		
		state = State.WAITING;
        transformed = Result.NONE;
        timeInAir = 0;
		
		GameObject.Destroy(myArrow.gameObject);
					
		ball.AttachToRoot();
        //ball.rigidbody.useGravity = false;
		//ball.rigidbody.isKinematic = false;
        
		ball.Owner.transform.rotation = initialRotation * Quaternion.Euler(new Vector3(0, angle, 0));
				
		pos = ball.Owner.BallPlaceHolderTransformation.transform.position;
		dir = ball.Owner.transform.forward;
		if(OnLaunch != null) OnLaunch();
		ball.Owner = null;
	}
	
	private void doTransfo(float t)
	{
        float X = dir.x * maxPower * t + pos.x;
        float Y = -0.5f * 9.81f * t * t + minPower + ((maxPower - minPower) * Mathf.Sin(Mathf.Deg2Rad * power * 80f)) * t + pos.y;
        float Z = dir.z * maxPower * t + pos.z;

        if (Y < 0)
            Y = 0;

		ball.transform.position = new Vector3(X, Y, Z);
	}
	
	public void Finish() {
		state = State.FINISHED;
		this.enabled = false;
		if(CallBack != null) CallBack(transformed);
	}
}
