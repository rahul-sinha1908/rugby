using System.Collections;
using UnityEngine;

[System.Serializable]
public class GameStatesSettings
{
	public MainStateSettings MainState;
}

[System.Serializable]
public class MainStateSettings
{
	public IntroStateSettings 	IntroState;
	public PlayingStateSettings PlayingState;
	public EndStateSettings 	EndState;
}

[System.Serializable]
public class IntroStateSettings
{
	public float 	timeToSleepAfterIntro    = 3; // Seconds (precision : miliseconds)
	public float 	rotationSpeed		   	  = 10; // Seconds (precision : miliseconds)
	public Vector3	rotationAxis;
}

[System.Serializable]
public class EndStateSettings
{
		
}

[System.Serializable]
public class PlayingStateSettings
{
	public MainGameStateSettings 	MainGameState;
	public WaitingStateSettings  	WaitingState;
	public GameActionStateSettings	GameActionState;
}

[System.Serializable]
public class MainGameStateSettings
{
	public RunningStateSettings 	RunningState;
	public PassingStateSettings 	PassingState;
	public TacklingStateSettings 	TacklingState;
}

[System.Serializable]
public class RunningStateSettings
{
	public BallHandlingStateSettings 	BallHandlingState;
	public BallFreeStateSettings		BallFreeState;
}

[System.Serializable]
public class BallHandlingStateSettings
{
	//public GainGroundingStateSettings 	GainGroundingState;
	public CamSettings					GainingGrounCamSettings;	
	
	public DodgingStateSettings			DodgingState;
}

[System.Serializable]
public class DodgingStateSettings
{
	public float unitDodgeSpeedFactor;
	public float unitDodgeDuration;
	public float unitDodgeCooldown;
	public bool  unitInvincibleDodge;	
}

[System.Serializable]
public class BallFreeStateSettings
{
	//public BallFlyingStateSettings BallFlyingState;
	public float angleDropKick 			= 45f;
	public float angleDropUpAndUnder 	= 70f;
	public CamSettings BallDropCamSettings;	
	public CamSettings GroundBallCamSettings;
}

[System.Serializable]
public class PassingStateSettings
{
	public float maxTimeHoldingPassButton = 3; // Seconds
	public CamSettings PassingCamSettings;
}

[System.Serializable]
public class TacklingStateSettings
{
	public float tackledTime = 3;	
}

[System.Serializable]
public class WaitingStateSettings
{
	public SuperCutsceneStateSettings superCutsceneState;
}

[System.Serializable]
public class SuperCutsceneStateSettings
{
	public Vector3	rotationAxis;
	public float 	duration,
					finalAngle,
					smooth;
}

[System.Serializable]
public class GameActionStateSettings
{
	public ConvertingStateSettings 	ConvertingState;
	public ScrumingStateSettings	ScrumingState;
	public TouchingStateSettings	TouchingSgtate;
	
}

[System.Serializable]
public class ConvertingStateSettings
{
	public bool TransfoRemiseAuCentre = false;
	public AimingConversionStateSettings 	AimingConversion;
	public ConversionFlyStateSettings		ConversionFly;
}

[System.Serializable]
public class AimingConversionStateSettings
{
		
}

[System.Serializable]
public class ConversionFlyStateSettings
{
	public CamSettings ConversionFlyCam;
}

[System.Serializable]
public class ScrumingStateSettings
{
	public float timeToGetOutTackleAreaBeforeScrum = 2;
    public int	 minPlayersEachTeamToTriggerScrum = 3;
	public float FeedSuperPerSmash;    // 0 to 1           (tweak)
    public float FeedSuperPerSecond;   // 0 to 1           (tweak)
    public float MaximumDuration;      // Seconds          (tweak)  
    public float MaximumDistance;      // Unity Distance   (tweak)
    public float SmashValue;           // 0 to 1           (tweak)
    public float SuperMultiplicator;   // Mult             (tweak)
    public float MalusValue;           // Mult            (tweak)
    public float InvincibleCooldown;    // Seconds (tweak)

    public Vector3 offsetCamera = new Vector3(1, 1, 0);
}

[System.Serializable]
public class TouchingStateSettings
{
	public bool ToucheRemiseAuCentre = false;
	
	public CamSettings TouchCamSettings;
}