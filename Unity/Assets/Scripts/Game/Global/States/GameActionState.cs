using UnityEngine;
using System.Collections.Generic;

/**
  * @class GameActionState
  * @brief Description.
  * @author Sylvain Lafon
  * @see MonoBehaviour
  */
public class GameActionState : GameState {

    public GameActionState(StateMachine sm, CameraManager cam, Game game) : base(sm, cam, game) { }

    public override bool OnTouch()
    {
        sm.state_change_son(this, new TouchState(sm, cam, game));
        return true;
    }

    public override bool OnScrum()
    {
        sm.state_change_son(this, new ScrumState(sm, cam, game));
        return true;
    }

    public override bool OnTry(Zone z)
    {
        sm.state_change_son(this, new ConvertingState(sm, cam, game, z));
        return true;
    }

    const int N = 4; // To define somewhere else..

    public override void OnLeave() // A remplacer par OnUnAutreTruc
    {
       // sm.state_change_me(this, new WaitingState(sm, cam, game, N)); // BAD IDEA !!!
    }
}