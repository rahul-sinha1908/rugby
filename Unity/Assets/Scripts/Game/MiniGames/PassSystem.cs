//#define DEBUG

using UnityEngine;
using System.Collections;

/**
 * @class Pass
 * @brief Doing a pass
 * @author Guilleminot Florian
 */
public class PassSystem
{   
	// Variables
	private Unit from;
	private Unit target;
	private Ball ball;
	private float velocityPass;
	private float magnitude;
	private Vector3 relativePosition;
	private Vector3 relativeDirection;
	private float TimeUpdate;
	//private float BetaAngle;
	//private float GammaAngle;
	private Vector3 initialPosition;
	private float angle;
	private float initialVelocity;
	private float fromVelocity;
	private Vector3 butBleu;
	private Vector3 butRouge;
	private Vector3 butBleuRouge;
	private float multiplyDirection = 20f;

	//float timeToShortPass = 1.5f;
	//float timeToLongPass = 3f;

    public passState oPassState = passState.NONE;
   
	public enum passState
	{
		SETUP,
		ONPASS,
		ONGROUND,
		ONTARGET,
		NONE
	}

	// Constructor
	public PassSystem(Vector3 b1, Vector3 b2, Unit from, Unit target, Ball ball)
	{
		Init(b1, b2, from, target, ball);
	}

	private void Init(Vector3 b1, Vector3 b2, Unit from, Unit target, Ball ball)
	{
		this.from = from;
		this.target = target;
		this.ball = ball;
		this.initialPosition = ball.transform.position;
		this.initialPosition.y -= 0.5f;
		this.butBleu = b1;
		this.butRouge = b2;

		
#if UNITY_EDITOR && DEBUG       
        int index = (from.Team == Game.instance.southTeam ? 0 : 1);
            Game.instance.logTeam[index].WriteLine("Pass State : " + oPassState);
            Game.instance.logTeam[index].WriteLine("From : " + from + " " + from.transform.position);
            Game.instance.logTeam[index].WriteLine("To : " + target + " " + target.transform.position);
            Game.instance.logTeam[index].WriteLine("Position initial of Ball : " + ball.transform.position);
            Game.instance.logTeam[index].WriteLine("Position modify of Ball : " + initialPosition);        
#endif
		/*
		Debug.DrawRay(this.target.transform.position, new Vector3(-1f, this.target.transform.position.y, 0), Color.yellow, 100f);
		Debug.DrawRay(this.target.transform.position, new Vector3(1f, this.target.transform.position.y, 0), Color.yellow, 100f);
		Debug.DrawRay(this.target.transform.position, new Vector3(0f, this.target.transform.position.y, 1f), Color.yellow, 100f);
		Debug.DrawRay(this.target.transform.position, new Vector3(0f, this.target.transform.position.y, -1f), Color.yellow, 100f);
		 * */
	}

	// Getter/Setter
	public Unit GetFrom()
	{
		return from;
	}
	public void SetFrom(Unit from)
	{
		this.from = from;
	}
	public Unit GetTarget()
	{
		return target;
	}
	public void SetTarget(Unit target)
	{
		this.target = target;
	}

	// Methods

	public void CalculatePass()
	{
		if (oPassState != passState.NONE)
		{
			return;
		}

		oPassState = passState.SETUP;
		if (from && target && ball)
        {
            velocityPass = this.ball.passSpeed;
            this.magnitude = calculateMagnitude(from.transform.position, target.transform.position);
            //directionFromToTarget();
            calculateRelativePosition();
            calculateRelativeDirection();
#if UNITY_EDITOR && DEBUG
            {
                int index = (from.Team == Game.instance.southTeam ? 0 : 1);
                Game.instance.logTeam[index].WriteLine("Pass State : " + oPassState);
                Game.instance.logTeam[index].WriteLine("velocity Pass : " + velocityPass);
                Game.instance.logTeam[index].WriteLine("magnitude Pass before relative Position: " + magnitude);
                Game.instance.logTeam[index].WriteLine("relative Position before validity check : " + relativePosition);
                Game.instance.logTeam[index].WriteLine("relative Direction before validity check : " + relativeDirection);
            }
#endif
            if (!passValidity())
            {
                //CorrectTrajectory();
                CorrectPosition();
                calculateRelativeDirection();
#if UNITY_EDITOR && DEBUG
                int index = (from.Team == Game.instance.southTeam ? 0 : 1);
                Game.instance.logTeam[index].WriteLine("relative Position after validity check : " + relativePosition);
                Game.instance.logTeam[index].WriteLine("relative Direction after validity check : " + relativeDirection);
#endif
            }

            multiplyRelativeDirection();
#if DEBUG
            Debug.DrawRay(relativePosition, new Vector3(-1f, relativePosition.y, 0), Color.yellow, 100f);
            Debug.DrawRay(relativePosition, new Vector3(1f, relativePosition.y, 0), Color.yellow, 100f);
            Debug.DrawRay(relativePosition, new Vector3(0f, relativePosition.y, 1f), Color.yellow, 100f);
            Debug.DrawRay(relativePosition, new Vector3(0f, relativePosition.y, -1f), Color.yellow, 100f);
#endif

            ball.AttachToRoot();
            //ball.rigidbody.isKinematic = false;
            //ball.rigidbody.useGravity = false;
            ball.Owner = null;

            this.magnitude = calculateMagnitude(from.transform.position, relativePosition);
            angle = Mathf.Deg2Rad * 25.0f;
            target.Order = Order.OrderMove(relativePosition);
#if UNITY_EDITOR && DEBUG
            {
                int index = (from.Team == Game.instance.southTeam ? 0 : 1);
                Game.instance.logTeam[index].WriteLine("magnitude Pass after relative Position : " + magnitude);
                Game.instance.logTeam[index].WriteLine("angle Impulsion Pass : " + angle);
                Game.instance.logTeam[index].WriteLine("order target : " + target.Order.point);
            }
#endif

        }
	}

	/*
	 * TODO : keep in mind that the curve on Y in the pass depends of the passSpeed
	 */
	public void DoPass(float t)
	{
		//Vector3 oldPos = ball.transform.position;
		ball.transform.position = new Vector3(relativeDirection.x * 1.5f * t + initialPosition.x,
			-0.5f * 9.81f * t * t + velocityPass * Mathf.Sin(angle) * t + initialPosition.y,
			relativeDirection.z * 1.5f * t + initialPosition.z);
#if UNITY_EDITOR && DEBUG
        int index = (from.Team == Game.instance.southTeam ? 0 : 1);
            Game.instance.logTeam[index].WriteLine("Time : " + t + " Ball Position : " + ball.transform.position + "\tTarget : pos : " + target.transform.position + " speed : " + target.nma.velocity.magnitude);
#endif
	}

	/*
	 * Correct the relativeDirection to send at the initial position of the target
	 * TODO : Tweak the passSpeed or the NMA velocity max if this case is too much present;
	 * */
	private void CorrectTrajectory()
	{
		if (from.transform.position.x > target.transform.position.x)
			relativeDirection = -Vector3.Cross(butBleuRouge, Vector3.up).normalized;
		else
			relativeDirection = Vector3.Cross(butBleuRouge, Vector3.up).normalized;
		//Debug.DrawRay(ball.Owner.transform.position, relativeDirection*100f, Color.cyan, 100f);
	}

	private void CorrectPosition()
	{
		relativePosition = new Vector3(target.transform.position.x, from.transform.position.y, from.transform.position.z);
	}

	/*
	 * @brief distance initial between two players
	 */
	private float calculateMagnitude(Vector3 from, Vector3 to)
	{
		return (to - from).magnitude;
	}

	/*
	 * @brief relative position of the target
	 */
	private void calculateRelativePosition()
	{
		if (target.nma.velocity.magnitude == 0)
		{
			Vector3 nmaSpeed = Vector3.zero;

			nmaSpeed.z = target.nma.speed;
			target.nma.velocity = nmaSpeed;
		}
		if (target.Team == target.game.southTeam)
			relativePosition = target.transform.position + Vector3.forward * target.nma.speed * 0.5f * magnitude / velocityPass;
		else
			relativePosition = target.transform.position - Vector3.forward * target.nma.speed * 0.5f * magnitude / velocityPass;
		//Debug.DrawRay(relativePosition, Vector3.up, Color.red, 10f);
	}

	/*
	 * @brief relative direction between the ball and the relative position of the target
	 */
	private void calculateRelativeDirection()
	{
		relativeDirection = (relativePosition - ball.transform.position).normalized;
	}

	private void multiplyRelativeDirection()
	{
		relativeDirection = relativeDirection * multiplyDirection;
	}

	/**
	 * Dot product between my relative direction of the pass and forward to know if the direction calculate is good
	 * return true if the direction is behind the forward else return false
	 */
	private bool passValidity()
	{
		//Debug.DrawRay(ball.Owner.transform.position, relativeDirection * 100f, Color.red, 100f);
		//Debug.DrawRay(target.transform.position, target.transform.forward * 100f, Color.red, 100f);
		if (Game.instance.southTeam == from.Team)
		butBleuRouge = butBleu - butRouge;
		else
			butBleuRouge = -butBleu + butRouge;
		return Vector3.Dot(relativeDirection, butBleuRouge) > 0 ? true : false;
	}
}
