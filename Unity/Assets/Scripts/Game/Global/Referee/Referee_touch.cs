using UnityEngine;

public partial class Referee
{
    public bool IsTouchRight()
    {
        return (this.game.refs.placeHolders.touchPlacement.position.x > 0);
    }

    public void PlacePlayersForTouch()
    {
        Team interceptTeam = game.Ball.Team;
        Team touchTeam = interceptTeam.opponent;

        // Fixe les unités			
        if (interceptTeam.Player != null) interceptTeam.Player.stopMove();
        if (touchTeam.Player != null) touchTeam.Player.stopMove();
        interceptTeam.fixUnits = touchTeam.fixUnits = true;

        bool right = this.IsTouchRight();

        // Place les unités
        Transform southTeam, northTeam, rightTeam, leftTeam, interTeam, passTeam;
        rightTeam = this.game.refs.placeHolders.touchPlacement.FindChild("RightTeam");
        leftTeam = this.game.refs.placeHolders.touchPlacement.FindChild("LeftTeam");

        if (right)
        {
            northTeam = rightTeam;
            southTeam = leftTeam;
        }
        else
        {
            northTeam = leftTeam;
            southTeam = rightTeam;
        }
        
        if(interceptTeam.south) 
        {
            interTeam = southTeam;
            passTeam = northTeam;
        }
        else 
        {
            interTeam = northTeam;
            passTeam = southTeam;
        }

        interceptTeam.placeUnits(interTeam, true);        
        touchTeam.placeUnits(passTeam, 1, true);

        Transform passUnitPosition = this.game.refs.placeHolders.touchPlacement.FindChild("TouchPlayer");
        touchTeam.placeUnit(passUnitPosition, 0, true);

        interceptTeam.Player.ChangeControlled(null);
        touchTeam.Player.ChangeControlled(touchTeam[0]);

        game.Ball.Owner = touchTeam[0];
        game.refs.managers.camera.setTarget(null);
    }

    public void OnTouch(Touche t)
    {
        if (t == null || t.a == null || t.b == null)
        {
            throw new UnityException("I need the touch to be configured");
        }

        // Indique que le jeu passe en mode "Touche"			

        // Placement dans la scène de la touche.
        Vector3 pos = Vector3.Project(game.Ball.transform.position - t.a.position, t.b.position - t.a.position) + t.a.position;
        pos.y = 0; // A terre           

        if (this.game.refs.placeHolders.touchPlacement == null)
        {
            throw new UnityException("I need to know how place the players when a touch occurs");
        }

        bool right = (pos.x > 0);

        if (right)
        {
            this.game.refs.placeHolders.touchPlacement.localRotation = Quaternion.Euler(0, -90, 0);
        }
        else
        {
            this.game.refs.placeHolders.touchPlacement.localRotation = Quaternion.Euler(0, 90, 0);
        }

        this.game.refs.placeHolders.touchPlacement.position = pos;

        Team interceptTeam = game.Ball.Team;
        Team touchTeam = interceptTeam.opponent;

        // Règlage du mini-jeu
        TouchManager tm = this.game.refs.managers.touch;

        // On indique les équipes
        tm.gamerIntercept = interceptTeam.Player;
        tm.gamerTouch = touchTeam.Player;

        // Fonction à appeller à la fin de la touche
        tm.CallBack = delegate(TouchManager.Result result, int id)
        {
            game.southTeam.OnTouchAction();
            game.northTeam.OnTouchAction();

            Timer.AddTimer(1, () =>
            {
                // On donne la balle à la bonne personne
                if (result == TouchManager.Result.INTERCEPTION)
                {
                    game.Ball.Owner = interceptTeam[id];

                    //super
                    this.IncreaseSuper(game.settings.Global.Super.touchInterceptSuperPoints, interceptTeam);
                    this.IncreaseSuper(game.settings.Global.Super.touchLooseSuperPoints, touchTeam);
                }
                else
                {
                    game.Ball.Owner = touchTeam[id + 1];
                    interceptTeam.Player.ChangeControlled(interceptTeam[id]);

                    //super
                    this.IncreaseSuper(game.settings.Global.Super.touchWinSuperPoints, touchTeam);
                }

                game.OnResumeSignal(freezeAfterTouch);
            });
        };

        tm.enabled = true;
    }

    public float freezeAfterTouch = 5;

}