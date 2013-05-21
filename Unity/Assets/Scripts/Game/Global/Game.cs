using UnityEngine;
using System.Collections;
using XInputDotNetPure;
using System.Threading;

/**
 * @class Game
 * @brief Classe principale du jeu
 * @author Sylvain Lafon
 * @author Guilleminot Florian
 */
[AddComponentMenu("Scripts/Game/Game")]
public class Game : myMonoBehaviour {
	
    public GamePlaySettings settings;
    public GameReferences refs;

    private static Game _instance;
    public static Game instance
    {
        get
        {
            if (_instance == null)
            {
                return _instance = (GameObject.FindObjectOfType(typeof(Game)) as Game);
            }

            return _instance;
        }
    }
    
    public Ball Ball { get { return refs.gameObjects.ball; } }
    private Gamer p1, p2;
	private Team Owner;
	
    private bool _disableIA = false;
    public bool disableIA
    {
        get
        {
            return _disableIA;
        }
        set
        {
            _disableIA = value;
            this.northTeam.OnOwnerChanged();
            this.southTeam.OnOwnerChanged(); 
        }
    }

	public bool tweakMode;
   	
    public Team northTeam { get { return refs.north; } }
    public Team southTeam { get { return refs.south; } }
    public Referee Referee { get { return refs.Referee; } }
	
	public void Start ()
    {
        northTeam.Game = this;
        southTeam.Game = this;
        northTeam.south = false;
        southTeam.south = true;
        northTeam.CreateUnits();
        southTeam.CreateUnits();

        Referee.Game = this;
        Referee.StartPlacement();

        northTeam.opponent = southTeam;
        southTeam.opponent = northTeam;

        this.refs.xboxInputs.Start();

        p1 = new Gamer(refs.south);
        p2 = new Gamer(refs.north);

        this.Owner = p1.Controlled.Team;
        Ball.Game = this;
        Ball.transform.parent = p1.Controlled.BallPlaceHolderRight.transform;
        Ball.transform.localPosition = Vector3.zero;
        Ball.Owner = p1.Controlled;
		
        this.refs.managers.intro.OnFinish = () =>
        {
            this._disableIA = true;                  
            Referee.OnStart();
			this.refs.stateMachine.event_OnStartSignal();
        };

        this.refs.stateMachine.SetFirstState(
            new MainState(this.refs.stateMachine, this.refs.managers.camera, this)
        );

        this.refs.managers.intro.enabled = true;
    }
	
	public void OnGameEnd(){
		this.refs.stateMachine.event_OnEndSignal();
	}
       	
    public void OnScrum()
    {
        Referee.OnScrum();
        //cameraManager.sm.event_Scrum();
        this.refs.stateMachine.event_Scrum();
    }
    
    public void OnDrop()
    {
        this.refs.stateMachine.event_Drop();
		//
		//this.state = State.DROPING;
        //cameraManager.sm.event_Drop();
    }
	
	public void OnTouch()
	{
		this.refs.stateMachine.event_OnTouch();
	}
	
	public void OnEssai(Zone z) {
        this.refs.stateMachine.event_Try(z);
		Referee.OnEssai();
	}

    public void OnPass(Unit from, Unit to)
    {
        this.refs.stateMachine.event_Pass(from, to);
        //cameraManager.sm.event_Pass(from, to);
    }

    public void OnDropTransformed(But but)
    {
        //cameraManager.sm.event_DropTransformed(but);
        Referee.OnDropTransformed(but);
    }

    public void OnOwnerChanged(Unit before, Unit after)
    {
        this.refs.stateMachine.event_NewOwner(before, after);

		if (after != null)
        {
            if (after.Team != Owner)
            {
                Owner = after.Team;				
            }

            // PATCH
            // p1.controlled = after;
            if (after.Team == southTeam)
            {
                p1.Controlled.IndicateSelected(false);
                p1.Controlled = after;
                p1.Controlled.IndicateSelected(true);
            }
            else if (p2 != null)
            {
                p2.Controlled.IndicateSelected(false);
                p2.Controlled = after;
                p2.Controlled.IndicateSelected(true);
            }
        }
        
        this.northTeam.OnOwnerChanged();
        this.southTeam.OnOwnerChanged();       
    }

    public void OnSuper(Team team, SuperList super)
    {
        //cameraManager.sm.event_Super(team, super);
    }

    /**
     * @author Sylvain Lafon
     * @brief Se déclenche quand il y a plaquage
     */
    public void OnTackle(Unit tackler, Unit tackled)
    {
        this.Referee.OnTackle(tackler, tackled);
        //this.cameraManager.sm.event_Tackle();       
    }

    public void BallOnGround(bool onGround)
    {
        //cameraManager.sm.event_BallOnGround(onGround);

        this.refs.stateMachine.event_BallOnGround(onGround);
    }

    public void OnBallOut()
    {
       //cameraManager.sm.event_BallOut();
        Referee.OnBallOut();
    }

    public void Reset()
    {
        SceneReloader.Go();
    }

    public void OnDodge(Unit u)
    {
        this.refs.stateMachine.event_Dodge(u);
    }

    public void OnDodgeFinished(Unit u)
    {
        this.refs.stateMachine.event_DodgeFinished(u);
    }

    public void OnResumeSignal()
    {
        this.refs.stateMachine.event_OnResumeSignal();
    }

    /*public void TimedDisableIA(float time)
    {
        this.disableIA = true;
        Timer.AddTimer(time, () =>
        {
            this.refs.stateMachine.event_OnStartSignal();
            this.disableIA = false;
        });
    }*/
}