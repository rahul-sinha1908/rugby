using UnityEngine;

public partial class Referee
{
    
    public void PlacePlayersForTransfo()
    {
        if (game.Ball.Owner == null)
        {
            throw new UnityException("Ball.Owner n'est pas r�gl� ? Dafuq ?!");
        }

        game.Ball.transform.position = game.Ball.Owner.BallPlaceHolderTransformation.transform.position;
        //float x = game.Ball.transform.position.x;

        Team t = game.Ball.Owner.Team;
                
        t.placeUnits(this.game.refs.placeHolders.conversionPlacement.FindChild("TeamShoot"), 1, true);
        t.placeUnit(this.game.refs.placeHolders.conversionPlacement.FindChild("ShootPlayer"), 0, true);
        Team.switchPlaces(t[0], game.Ball.Owner);
        t.opponent.placeUnits(this.game.refs.placeHolders.conversionPlacement.FindChild("TeamLook"), true);

        //Team opponent = game.Ball.Owner.Team.opponent;

        // Joueur face au look At
        Transform butPoint = t.opponent.But.transform.FindChild("Transformation LookAt");
        game.Ball.Owner.transform.LookAt(butPoint);
    }

    public void EnableTransformation()
    {
        TransformationManager tm = this.game.refs.managers.conversion;
        tm.enabled = true;
    }

    private void PlaceTransfoPlaceholders()
    {
        Team t = game.Ball.Owner.Team;
        float x = game.Ball.transform.position.x;

        Transform point = t.opponent.But.transformationPoint;
        point.transform.position = new Vector3(x, 0, point.transform.position.z);

        this.game.refs.placeHolders.conversionPlacement.transform.position = point.position;
        this.game.refs.placeHolders.conversionPlacement.transform.rotation = point.rotation;
    }

    public void OnTry()
    {
        Team t = game.Ball.Owner.Team;
        
        t.fixUnits = t.opponent.fixUnits = true;
        if (t.Player != null) t.Player.stopMove();
        if (t.opponent.Player != null) t.opponent.Player.stopMove();


        t.nbPoints += game.settings.Global.Game.points_essai;
        Team opponent = game.Ball.Owner.Team.opponent;

        //super for try
        IncreaseSuper(game.settings.Global.Super.tryWinSuperPoints, t);
        IncreaseSuper(game.settings.Global.Super.tryLooseSuperPoints, opponent);

        TransformationManager tm = this.game.refs.managers.conversion;

        tm.ball = game.Ball;
        tm.gamer = t.Player;
		
		//Debug.Log("Fire Event");
        tm.OnLaunch = this.game.OnConversionShot;

        // After the transformation is done, according to the result :
        tm.CallBack = delegate(TransformationManager.Result transformed)
        {

            if (transformed == TransformationManager.Result.TRANSFORMED)
            {
                //transfo super
                IncreaseSuper(game.settings.Global.Super.conversionWinSuperPoints, t);
            }
            else
            {
                //transfo super
                IncreaseSuper(game.settings.Global.Super.conversionLooseSuperPoints, t);
            }

            IncreaseSuper(game.settings.Global.Super.conversionOpponentSuperPoints, t.opponent);

            //if (game.settings.GameStates.MainState.PlayingState.GameActionState.ConvertingState.TransfoRemiseAuCentre || transformed != TransformationManager.Result.GROUND)
            //{
                UnitToGiveBallTo = opponent[3];
              //  this.StartPlacement();
            //}

            //this.game.OnResumeSignal(FreezeAfterConversion);
        };
                
        PlaceTransfoPlaceholders();
        UnitToGiveBallTo = opponent[3];
    }
	
	public void GivePoints(int _amount, Team _teamConcerned){
		_teamConcerned.nbPoints += _amount;
	}
	
    public float FreezeAfterConversion = 5;

    public void OnDropTransformed(But but)
    {
		Debug.Log("La");
		
        // On donne les points
        but.Owner.opponent.nbPoints += this.game.settings.Global.Game.points_drop;

        // A faire en cam�ra :
        this.StartPlacement();
        this.game.Ball.Owner = but.Owner[2];
		
        IncreaseSuper(game.settings.Global.Super.dropWinSuperPoints, but.Owner.opponent);

        //this.game.TimedDisableIA(3);
    }

    public void OnDropFinished(Ball.DropResult res)
    {
        Team t = this.game.Ball.PreviousOwner.Team;
        SuperSettings settings = game.settings.Global.Super;

        if (res == Ball.DropResult.GROUND)
        {
            IncreaseSuper(settings.dropLooseSuperPoints, t);
        }
        if (res == Ball.DropResult.INTERCEPTED)
        {
            IncreaseSuper(settings.dropLooseSuperPoints, t);
            //IncreaseSuper(settings.?????, t.opponent);
        }
    }
}