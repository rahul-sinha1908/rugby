using UnityEngine;
using System.Collections;

public class TackleSystem {
	
	private Unit tackled;
	private Unit tackler;
	private Ball ball;
	
	private float angleOfFOV;
	private float distanceOfTackle;

	public TackleSystem(Unit tackled = null, Unit tackler = null, Ball b = null, float teta = 0.0f)
	{
		this.tackled = tackled;
		this.tackler = tackler;
		this.angleOfFOV = teta;
		this.ball = b;
	}
	
	public void Tackle()
	{
		if (canTackle())
		{
			if (IsInRange())
			{
				if (IsCrit())
				{
					ball.Owner = tackler;
				}
				else
				{
					//TODO : Launch CutScene
					if (Random.Range(0,1) > 0.5f)
					{
						ball.Owner = tackler;
					}
				}
			}
		}
	}
	
	private bool canTackle()
	{
		return tackled == ball.Owner && tackled.Team != tackler.Team;
	}
	
	private bool IsCrit()
	{
		return Vector3.Angle(tackled.transform.position - tackler.transform.position, tackler.transform.forward) <= angleOfFOV;
	}
	
	private bool IsInRange()
	{
		return tackler.NearUnits.Contains(tackled);
	}

}