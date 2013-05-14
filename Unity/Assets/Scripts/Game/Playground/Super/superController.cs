using UnityEngine;
using System.Collections;

public enum SuperList{
	//null is the base status
	superNull,
	superTackle,
	superDash,
	superWall
};

[AddComponentMenu("Scripts/Supers/Controller")]
public class superController : myMonoBehaviour {
	
	//public KeyCode OffensiveSuperButton;
	//public KeyCode DefensiveSuperButton;
	
	public SuperList OffensiveSuper;
	public SuperList DefensiveSuper;
	
	private Game _game;
	private Team _team;
	private SuperList currentSuper;
	private Color colorSave;

    public bool SuperActive
    {
        get
        {
            return currentSuper != SuperList.superNull;
        }
    }
	
	//super color
	public Color superTackleColor;
	public Color superDashColor;
	
	//time management for supers
	private float OffensiveSuperTimeAmount;
	private float DefensiveSuperTimeAmount;
	private float SuperTimeLeft;
	
	
	void Start () {
		_game 	        = GameObject.Find("GameDesign").GetComponent<Game>();
		_team			= gameObject.GetComponent<Team>();
		currentSuper    = SuperList.superNull;
		
		OffensiveSuperTimeAmount = _game.settings.super.OffensiveSuperDurationTime;
		DefensiveSuperTimeAmount = _game.settings.super.DefensiveSuperDurationTime;
		
		SuperTimeLeft = 0f;
	}
	
	void Update () {
        if (this._game.state == Game.State.INTRODUCTION)
        {
            return;
        }

		updateSuperValue();
        updateSuperInput();
		updateSuperStatus();
	}
	
	void updateSuperValue(){
		if( (Random.Range(1,20) == 1) && (currentSuper == SuperList.superNull) ){
			_team.increaseSuperGauge(5);
		}
	}
	
	void updateSuperInput(){
		
		InputTouch superOff = _game.settings.inputs.superOff;
		InputTouch superDef = _game.settings.inputs.superDef;
		
		if(_game.state == Game.State.PLAYING) {
		
			//offense
			if(_team.Player.XboxController != null){
				if(Input.GetKeyDown(superOff.keyboard) || _team.Player.XboxController.GetButtonDown(superOff.xbox)){
					if(_team.SuperGaugeValue == _game.settings.super.superGaugeOffensiveLimitBreak){
						MyDebug.Log("Offensive Super attack !");
						launchSuper(OffensiveSuper, OffensiveSuperTimeAmount);
						_team.SuperGaugeValue -= _game.settings.super.superGaugeOffensiveLimitBreak;
                        _game.OnSuper(_team, SuperList.superDash);
					}else{
						MyDebug.Log("Need more Power to lauch the offensive super");
						MyDebug.Log("Current Power : "+_team.SuperGaugeValue);
						MyDebug.Log("Needed  Power : "+_game.settings.super.superGaugeOffensiveLimitBreak);
					}
				}
				
				//defense
				if(Input.GetKeyDown(superDef.keyboard) || _team.Player.XboxController.GetButtonDown(superDef.xbox)){
					if(_team.SuperGaugeValue == _game.settings.super.superGaugeDefensiveLimitBreak){
						MyDebug.Log("Defensive Super attack !");
							launchSuper(DefensiveSuper, DefensiveSuperTimeAmount);
							_team.SuperGaugeValue -= _game.settings.super.superGaugeDefensiveLimitBreak;
                            _game.OnSuper(_team, SuperList.superWall);
					}else{
						MyDebug.Log("Need more Power to lauch the defensive super");
						MyDebug.Log("Current Power : "+_team.SuperGaugeValue);
						MyDebug.Log("Needed  Power : "+_game.settings.super.superGaugeDefensiveLimitBreak);
					}
				}
			}
		}
	}
	
	void updateSuperStatus(){
		if(currentSuper != SuperList.superNull){
			//maj super time
			SuperTimeLeft -= Time.deltaTime;
			//MyDebug.Log("Super Time left  : "+SuperTimeLeft);
			/*if(SuperTimeLeft > 0f){
				switch(currentSuper){
					case SuperList.superDash:{
						break;
					}
					case SuperList.superTackle:{
						break;
					}
					case SuperList.superWall:{
						break;
					}
					default:{
						break;
					}
				}
			}else{
				endSuper();
			}*/

            if (SuperTimeLeft <= 0) {
                endSuper();
            }
		}
	}
	
	void endSuper(){
		MyDebug.Log("Super Over");
		currentSuper = SuperList.superNull;
		_team.speedFactor 	= 1f;
		_team.tackleFactor 	= 1f;
		_team.ChangePlayersColor(colorSave);
		stopDashAttackFeedback();
		stopTackleAttackFeedback();
	}
	
	void launchSuper(SuperList super, float duration){
		colorSave = _team.GetPlayerColor();
		SuperTimeLeft = duration;
		currentSuper  = super;
		switch (super){
			case SuperList.superDash:{
				MyDebug.Log("Dash Super attack !");
				launchDashAttackFeedback();
				_team.speedFactor = _game.settings.super.superSpeedScale;
				//dash
			break;
			}
			case SuperList.superTackle:{
				MyDebug.Log("Tackle Super attack !");
				launchTackleAttackFeedback();
				_team.tackleFactor = _game.settings.super.superTackleBoxScale;
				//tackle
			break;
			}
			case SuperList.superWall:{
				MyDebug.Log("Wall Super attack !");
				//wall
			break;
			}
			default:{
				break;
			}
		}
	}
	
	
	
	void launchDashAttackFeedback(){		
		_team.ChangePlayersColor(superDashColor);
		_team.PlaySuperParticleSystem(SuperList.superDash, true);
	}
	
	void launchTackleAttackFeedback(){
		_team.ChangePlayersColor(superTackleColor);
		_team.PlaySuperParticleSystem(SuperList.superTackle, true);
	}
	
	void stopDashAttackFeedback(){
		_team.ChangePlayersColor(colorSave);
		_team.PlaySuperParticleSystem(SuperList.superDash, false);
	}
	
	void stopTackleAttackFeedback(){
		_team.ChangePlayersColor(colorSave);
		_team.PlaySuperParticleSystem(SuperList.superTackle, false);
	}
}