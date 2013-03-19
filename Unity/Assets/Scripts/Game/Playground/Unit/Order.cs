using UnityEngine;
using System.Collections;

/**
 * @class Order
 * @brief Un ordre (unite)
 * @author Sylvain Lafon
 * @author Guilleminot Florian
 */
[System.Serializable]
public struct Order  {
    public enum TYPE
    {
        NOTHING,
        MOVE,
        PASS,
        DROP,
        TACKLE,
        FOLLOW,
        TRIANGLE,   // ASSISTER
        LANE,      // PRESSER
        SEARCH,
    }
    public enum TYPE_DEPLACEMENT
    {
        MARCHE,
        COURSE,
        SPRINT
    }

    public TYPE type;
    public TYPE_DEPLACEMENT deplacement;
	public float pressionCapture;
	public Vector3 passDirection;
    public Unit target;
    public float power;
    public Vector3 point;

    public static Order OrderNothing() {
        Order o = new Order();
        o.type = TYPE.NOTHING;
        return o;
    }

    public static Order OrderMove(Vector3 point, TYPE_DEPLACEMENT type)
    {
        Order o = new Order();
        o.type = TYPE.MOVE;
        o.point = point;
        o.deplacement = type;
        return o;
    }

    public static Order OrderFollow(Unit unit, TYPE_DEPLACEMENT type)
    {
        Order o = new Order();
        o.type = TYPE.FOLLOW;
        o.target = unit;
        o.deplacement = type;
        return o;
    }
	
	public static Order OrderDrop(Unit unit)
    {
		return OrderDrop(unit, 1.0f);
	}

    public static Order OrderDrop(Unit unit, float pressionCapture)
    {
        Order o = new Order();
        o.type = TYPE.DROP;
        o.target = unit;
		o.pressionCapture = pressionCapture;
        return o;
    }

	public static Order OrderPass(Unit unit)
	{
		Order o = new Order();
		o.type = TYPE.PASS;
		o.target = unit;
		return o;
	}

    public static Order OrderSupport(Unit unit, Vector3 distance, bool right)
    {
        Order o = new Order();
        o.type = TYPE.TRIANGLE;
        o.target = unit;
        o.point = new Vector3(distance.x, 0, distance.z * (right ? -1 : 1));
        return o;
    }

    public static Order OrderAttack(Unit unit, float distance, bool right)
    {
        Order o = new Order();
        o.type = TYPE.LANE;
        o.target = unit;
        o.power = distance;
        return o;
    }

    public static Order OrderFollowBall()
    {
        Order o = new Order();
        o.type = TYPE.SEARCH;
        return o;
    }

    public static Order OrderPlaquer(Unit u)
    {
        Order o = new Order();
        o.type = TYPE.TACKLE;
        o.target = u;
        return o;
    }
}