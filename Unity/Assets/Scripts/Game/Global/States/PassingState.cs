using UnityEngine;
using System.Collections.Generic;

/**
  * @class PassGameState
  * @brief Etat de la cam�ra lorsque l'on fait une passe
  * @author Sylvain Lafon
  * @see GameState
  */
public class PassingState : GameState {
    public PassingState(StateMachine sm, CameraManager cam, Game game, Unit from, Unit to) : base(sm, cam, game) {
      //  this.from = from;
      //  this.to = to;
    }

    //private Unit from, to;

    public override void OnEnter()
    {
		Debug.Log("In pass state");
        cam.setTarget(cam.game.Ball.transform);
    }

    public override void OnUpdate()
    {
		var p1 = this.game.southTeam.Player;
        var p2 = this.game.northTeam.Player;
 
        if (p1 != null) p1.myUpdate();
        if (p2 != null) p2.myUpdate();
    }
	
	public override bool OnNewOwner(Unit old, Unit current)
    {
        if (current)
        {
			Debug.Log("New Owner");	
            sm.state_change_son(this, new RunningState(sm, cam, game));
            return true;
        }

        return false;
    }
}
