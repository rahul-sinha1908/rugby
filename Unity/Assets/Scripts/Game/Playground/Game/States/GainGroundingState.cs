using UnityEngine;
/**
  * @class FollowPlayerState
  * @brief Etat de la cam�ra lorsqu'elle doit suivre un joueur.
  * @author Sylvain Lafon
  * @see myMonoBehaviour
  */
public class GainGroundingState : GameState {
    public GainGroundingState(StateMachine sm, CameraManager cam, Game game) : base(sm, cam, game) { }

    public override void OnEnter()
    {
        cam.setTarget(this.game.Ball.Owner.transform);
    }
        
    public override bool OnDodge(Unit u)
    {
        if (u == this.game.Ball.Owner)
        {
            sm.state_change_son(this, new DodgingState(sm, cam, this.game));
            return true;
        }

        return false;
    }

    //  TODO � changer de place ou pas
    public override bool OnTackle()   
    {
		
		
		
        //if(this.target.isTackled) 
        //{
			//MyDebug.Log("On Tackle");
            sm.state_change_son(this, new TackleState(sm, cam, game, this.game.Ball.Owner));
            return true;
        //}

        //return false;
    }
	
	
}
