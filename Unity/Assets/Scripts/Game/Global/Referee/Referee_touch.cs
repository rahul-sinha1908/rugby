using UnityEngine;

public partial class Referee
{
    public void PlacePlayersForTouch()
    {
        Team interceptTeam = game.Ball.Team;
        Team touchTeam = interceptTeam.opponent;

        // Fixe les unit�s			
        if (interceptTeam.Player != null) interceptTeam.Player.stopMove();
        if (touchTeam.Player != null) touchTeam.Player.stopMove();
        interceptTeam.fixUnits = touchTeam.fixUnits = true;

        // Bouttons pour la touche.			
        //interceptTeam[0].buttonIndicator.ApplyTexture("A");
        //interceptTeam[1].buttonIndicator.ApplyTexture("B");
        //interceptTeam[2].buttonIndicator.ApplyTexture("X");
        //
        //touchTeam[1].buttonIndicator.ApplyTexture("A");
        //touchTeam[2].buttonIndicator.ApplyTexture("B");
        //touchTeam[3].buttonIndicator.ApplyTexture("X");
        //
        //interceptTeam[0].buttonIndicator.target.renderer.enabled = true;
        //interceptTeam[1].buttonIndicator.target.renderer.enabled = true;
        //interceptTeam[2].buttonIndicator.target.renderer.enabled = true;
        //
        //touchTeam[1].buttonIndicator.target.renderer.enabled = true;
        //touchTeam[2].buttonIndicator.target.renderer.enabled = true;
        //touchTeam[3].buttonIndicator.target.renderer.enabled = true;

        // Touche � droite ?
        bool right = (this.game.refs.placeHolders.touchPlacement.position.x > 0);

        // Place les unit�s
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

        // Placement dans la sc�ne de la touche.
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

        // R�glage du mini-jeu
        TouchManager tm = this.game.refs.managers.touch;

        // On indique les �quipes
        tm.gamerIntercept = interceptTeam.Player;
        tm.gamerTouch = touchTeam.Player;

        // Fonction � appeller � la fin de la touche
        tm.CallBack = delegate(TouchManager.Result result, int id)
        {
            this.game.northTeam.OnTouchAction();
            this.game.southTeam.OnTouchAction();

            if (result == TouchManager.Result.INTERCEPTION)
            {
                //this.game.Ball.Pass(interceptTeam[id]);
                //super
                this.IncreaseSuper(game.settings.Global.Super.touchInterceptSuperPoints, interceptTeam);
                this.IncreaseSuper(game.settings.Global.Super.touchLooseSuperPoints, touchTeam);
            }
            else
            {
                //this.game.Ball.Pass(touchTeam[id + 1]);
                //super
                this.IncreaseSuper(game.settings.Global.Super.touchWinSuperPoints, touchTeam);
            }            

            Timer.AddTimer(1, () =>
            {
                if (result == TouchManager.Result.INTERCEPTION)
                {
                    this.game.Ball.Owner = (interceptTeam[id]);
                }
                else
                {
                    this.game.Ball.Owner = (touchTeam[id + 1]);
                } 

                game.OnResumeSignal(freezeAfterTouch);
            });
        };

        tm.enabled = true;
    }

    public float freezeAfterTouch = 5;

}