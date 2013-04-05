using UnityEngine;
using System.Collections.Generic;

public delegate void CallBack_transfo(TransformationManager.Result transformed);

/**
  * @class TransformationManager
  * @brief Description.
  * @author Sylvain Lafon
  * @see MonoBehaviour
  */
public class TransformationManager : MonoBehaviour {
	public CallBack_transfo CallBack;
	
	public Gamer gamer {get; set;}
	public Ball ball {get; set;}
	
	public InputTouch touch;
	
	private Quaternion initialRotation;
	
	private float angle = 0;
	public float angleSpeed;
	
	private float power = 0;
	public float powerSpeed;
	
	public Vector3 maxPower;
	public float maxAngle;
	
	public bool infiniteTime = true;
	public float timeAngle = 0;
	public float timePower = 0;
	
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
	
	public GameObject arrow;
	private GameObject myArrow;
		
	public void OnEnable() {
		angle = 0;
		power = 0;
		initialRotation = ball.Owner.transform.rotation;
		
		this.remainingTime = timeAngle;
		this.state = State.ANGLE;
		
		myArrow = GameObject.Instantiate(arrow) as GameObject;
		if(!myArrow)
			throw new UnityException("Erreur : missing arrow");
		
		myArrow.transform.parent = ball.Owner.transform;
		myArrow.transform.localPosition = Vector3.zero;
		myArrow.transform.localRotation = Quaternion.identity;
		
	}
	
	public void OnGUI() {
		GUILayout.Space(300);
		GUILayout.Label ("Transformation");
		GUILayout.Label ("State : " + state);
		GUILayout.Label ("Time : " + (infiniteTime ? "Infinite" : remainingTime.ToString()));
		GUILayout.Label ("Angle : " + angle);
		GUILayout.Label ("Power : " + power);
	}
	
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
				
			if(remainingTime < 0 || Input.GetKeyDown(touch.keyboard) || (gamer.XboxController.IsConnected && gamer.XboxController.GetButtonDown(touch.xbox))) {
				remainingTime = timePower;
				state = State.POWER;	
			}
			
			ball.Owner.transform.rotation = initialRotation * Quaternion.Euler(new Vector3(0, angle, 0));
		}
		
		if(state == State.POWER) {
					
			power += powerSpeed * Time.deltaTime;
			if(power > 1) {
				power = 1;
				powerSpeed *= -1;
			}
			if(power < 0) {
				power = 0;
				powerSpeed *= -1;
			}
			
			if(!infiniteTime) {
				remainingTime -= Time.deltaTime;	
			}
			
			if(remainingTime < 0 || (gamer.XboxController.IsConnected && gamer.XboxController.GetButtonUp(touch.xbox)) || Input.GetKeyUp(touch.keyboard)) {
				Launch();
			}
			
			
		}
		
		if(state == State.WAITING) {
			if(ball.transform.position.y < 0.3f) {
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
	
	public void Launch() {
		
		state = State.WAITING;
        transformed = Result.NONE;
		
		GameObject.Destroy(myArrow);
					
		ball.transform.parent = null;
        ball.rigidbody.useGravity = true;
		ball.rigidbody.isKinematic = false;
        
		ball.Owner.transform.rotation = initialRotation * Quaternion.Euler(new Vector3(0, angle, 0));
			
		Vector3 force = ball.Owner.transform.forward * power * maxPower.x + 
						ball.Owner.transform.right * power * maxPower.z +
						ball.Owner.transform.up * power * maxPower.y;
		
		ball.rigidbody.AddForce(force);
				
		ball.Owner = null;
	}
	
	public void Finish() {
		state = State.FINISHED;
		this.enabled = false;
		if(CallBack != null) CallBack(transformed);
	}
}
