using System.Collections.Generic;
using UnityEngine;

public partial class Referee
{
    public void OnTackle(Unit tackler, Unit tackled)
    {

        if (tackler != null && tackled == null)
        {
            tackler.sm.event_Tackle();
            game.OnDodgeSuccess();
            return;
        }

        TackleManager tm = this.game.refs.managers.tackle;
        if (tm == null)
            throw new UnityException("Game needs a TackleManager !");

        if (tackler == null || tackled == null || tackler.Team == tackled.Team)
            throw new UnityException("Error : " + tackler + " cannot tackle " + tackled + " !");

        tm.game = this.game;
        tm.tackler = tackler;
        tm.tackled = tackled;

        // End of a tackle, according to the result
        tm.callback = (TackleManager.RESULT res) =>
        {
            TacklePlaceUnitsAtEnd(tackler, tackled);

            switch (res)
            {
                // Plaquage critique, le plaqueur recup�re la balle, le plaqu� est knockout
                case TackleManager.RESULT.CRITIC:
                    this.game.Ball.Owner = tackler;
                    break;

                // Passe : les deux sont knock-out mais la balle a pu �tre donn�e � un alli�
                case TackleManager.RESULT.PASS:

                    List<Unit> listToCheck = null;
                    Unit unitTo = null;

                    listToCheck = tackled.Team.GetRight(tackled);

                    if (listToCheck.Count == 0)
                    {
                        listToCheck = tackled.Team.GetLeft(tackled);
                    }
                    if (listToCheck.Count > 0)
                    {
                        foreach (Unit u in listToCheck)
                        {
                            if (u.typeOfPlayer == Unit.TYPEOFPLAYER.OFFENSIVE)
                            {
                                if (tackled.Team == game.southTeam)
                                {
                                    if (u.transform.position.z < tackled.transform.position.z && u.canCatchTheBall)
                                    {
                                        //Debug.Log("try to pass to " + u);
                                        unitTo = u;
                                        break;
                                    }
                                    else
                                        unitTo = null;
                                }
                                else if (tackled.Team == game.northTeam)
                                {
                                    if (u.transform.position.z > tackled.transform.position.z && u.canCatchTheBall)
                                    {
                                        //Debug.Log("try to pass to " + u);
                                        unitTo = u;
                                        break;
                                    }
                                    else
                                        unitTo = null;
                                }
                            }
                        }
                    }
                    else
                    {
                        unitTo = null;
                        return;
                    }

                    if (unitTo != null && unitTo != game.Ball.Owner)
                    {
                        if (tackled.unitAnimator)
                        {
                            tackled.unitAnimator.OnTacklePass();
                        }
                        game.Ball.Pass(unitTo);
                    }

                    //Unit target = tackled.GetNearestAlly();
                    //if (tackled.unitAnimator)
                    //{
                    //    tackled.unitAnimator.OnTacklePass();
                    //}
                    //game.Ball.Pass(target);

                    tackled.sm.event_Tackle();
                    tackler.sm.event_Tackle();
                    break;

                // Normal : les deux sont knock-out et la balle est par terre 
                // /!\ M�l�e possible /!\
                case TackleManager.RESULT.NORMAL:

                    //super			
                    game.Ball.TeleportOnGround();
                    IncreaseSuper(game.settings.Global.Super.tackleWinSuperPoints, tackler.Team);
                    tackled.sm.event_Tackle();
                    tackler.sm.event_Tackle();
                    break;
            }

            tackler.Team.Player.UpdateControlled();
            tackled.Team.Player.UpdateControlled();

            LastTackle = Time.time;
        };

        this.TacklePlaceUnitsAtStart(tackler, tackled);
        tm.atUpdate = TacklePlaceUnitsAtUpdate;

        tm.Tackle();
    }

    private void TacklePlaceUnitsAtStart(Unit tackler, Unit tackled)
    {
        tackler.transform.LookAt(tackled.transform, Vector3.up);
    }

    private void TacklePlaceUnitsAtUpdate(Unit tackler, Unit tackled)
    {
        tackler.transform.LookAt(tackled.transform, Vector3.up);
        tackler.transform.position = Vector3.Lerp(tackler.transform.position, tackled.transform.position, Time.deltaTime * 5);
    }

    private void TacklePlaceUnitsAtEnd(Unit tackler, Unit tackled)
    {
        tackler.transform.forward = Vector3.forward;
    }

    public void UpdateTackle()
    {
        if (LastTackle != -1)
        {
            // TODO cte : 2 -> temps pour checker
            if (Time.time - LastTackle > game.settings.GameStates.MainState.PlayingState.GameActionState.ScrumingState.timeToGetOutTackleAreaBeforeScrum)
            {
                LastTackle = -1;
                int right = 0, left = 0;
                for (int i = 0; i < this.game.Ball.scrumFieldUnits.Count; i++)
                {
                    if (this.game.Ball.scrumFieldUnits[i].Team == game.southTeam)
                        right++;
                    else
                        left++;
                }

                // TODO cte : 3 --> nb de joueurs de chaque equipe qui doivent etre dans la zone
                if (right >= game.settings.GameStates.MainState.PlayingState.GameActionState.ScrumingState.minPlayersEachTeamToTriggerScrum &&
                    left >= game.settings.GameStates.MainState.PlayingState.GameActionState.ScrumingState.minPlayersEachTeamToTriggerScrum)
                {
                    game.OnScrum();
                    //goScrum = true;
                    //
                }
            }
        }
    }
}