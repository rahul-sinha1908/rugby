using UnityEngine;
using System.Collections;
using XInputDotNetPure;

/*
 *@author Maxens Dubois, Lafon Sylvain
 */
[AddComponentMenu("Scripts/Game/UI Manager"), RequireComponent(typeof(Game))]
public class gameUIManager : myMonoBehaviour {
	
	private Game _game;
		
	public Texture2D emptyBar;
	public Texture2D blueBar;
	public Texture2D redBar;
	
	public float quotientMin = 0.5f;
	public float quotientMax = 1.5f;
	
	private float blueProgress;
	private float redProgress;
	
	public GUIStyle superOkTextStyle;
	public GUIStyle gameTimeTextStyle;
	public GUIStyle gameScoreTextStyle;
	public GUIStyle timeBeforeScrumStyle;
	
	public float ScrumBarMaxDelta = 1.5f;
	
	public float timeBoxWidthPercentage   = 10;
	public float timeBoxHeightPercentage  = 5;
	public float timeBoxXPercentage		= 50;
	public float timeBoxYPercentage		= 10;
	
	public float blueGaugeBoxWidthPercentage   = 25;
	public float blueGaugeBoxHeightPercentage  = 10;
	public float blueGaugeBoxXPercentage		= 22.5f;
	public float blueGaugeBoxYPercentage		= 10;
	
	public float redGaugeBoxWidthPercentage   = 25;
	public float redGaugeBoxHeightPercentage  = 10;
	public float redGaugeBoxXPercentage		= 77.5f;
	public float redGaugeBoxYPercentage		= 10;
	
	public float scoreBoxWidthPercentage  = 20;
	public float scoreBoxHeightPercentage = 15;
	public float scoreBoxXPercentage = 50;
	public float scoreBoxYPercentage = 10;
	
	public float scrumBarBoxWidthPercentage = 50;
	public float scrumBarBoxHeightPercentage = 16;
	public float scrumBarBoxXPercentage = 50;
	public float scrumBarBoxYPercentage = 50;
	
	public float scrumSpecialBoxWidthPercentage = 50;
	public float scrumSpecialBoxHeightPercentage = 16;
	public float scrumSpecialBoxXPercentage = 50;
	public float scrumSpecialBoxYPercentage = 66;
	
	public float scrumTimeBoxWidthPercentage = 50;
	public float scrumTimeBoxHeightPercentage = 16;
	public float scrumTimeBoxXPercentage = 50;
	public float scrumTimeBoxYPercentage = 34;
	
	void Start () 
    {
		_game 		= gameObject.GetComponent<Game>();
	  
		blueProgress = 0f;
		redProgress  = 0f;
	}
	
	void Update()
    {
       // if (this._game.state == Game.State.INTRODUCTION)
       // {
       //     return;
       // }

		//GamePadState pad = GamePad.GetState(_game.p1.playerIndex);
        
        Gamer.initGamerId();					
		UpdateSuperProgress();
	}
	
	void UpdateSuperProgress(){

		float blueCurrent = (float)_game.right.SuperGaugeValue;
		float redCurrent  = (float)_game.left.SuperGaugeValue;
		float max		  = (float)_game.settings.super.superGaugeMaximum;
		blueProgress = Mathf.Clamp01(blueCurrent/max);
		redProgress  = Mathf.Clamp01(redCurrent/max);
	}
	
	/**
	  * @brief Fabrique un rectangle en fonction des dimensions de l'écran.
	  * @param x, y
	  * 	Position du rectangle (pourcentage)
	  * @param w, h
	  * 	Taille du rectangle (pourcentage)
	  * @return Rectangle contenant la position et la taille en pixels.
	  * @author Sylvain LAFON
	  */
	public static Rect screenRelativeRect(float x, float y, float w, float h) {
		
		float H = Screen.height / 100f;
		float W = Screen.width / 100f;

		return new Rect(x * W, y * H, w * W, h * H);	
	}

    public static Rect screenRelativeRect(Rect r)
    {
        return screenRelativeRect(r.x, r.y, r.width, r.height);
    }

	void OnGUI()
    {
        //if (this._game.state == Game.State.INTRODUCTION)
        //{
        //    return;
        //}
		//	
		//if(_game.state != Game.State.END)
        //{
        //    GUIPlaying();			
		//}
        //else
        //{
        //    GUIGameOver();
		//}
	}

    void GUIPlaying()
    {
        //time box
        Rect timeBox = screenRelativeRect(timeBoxXPercentage - timeBoxWidthPercentage / 2,
            timeBoxYPercentage - timeBoxHeightPercentage / 2,
            timeBoxWidthPercentage, timeBoxHeightPercentage);

        //score box
        Rect scoreBox = screenRelativeRect(scoreBoxXPercentage - scoreBoxWidthPercentage / 2,
            scoreBoxYPercentage - scoreBoxHeightPercentage / 2,
            scoreBoxWidthPercentage, scoreBoxHeightPercentage);

        //super gauges
        Rect blueGaugeBox = screenRelativeRect(blueGaugeBoxXPercentage - blueGaugeBoxWidthPercentage / 2,
            blueGaugeBoxYPercentage - blueGaugeBoxHeightPercentage / 2,
            blueGaugeBoxWidthPercentage, blueGaugeBoxHeightPercentage);

        Rect blueProgressGaugeBox = screenRelativeRect(blueGaugeBoxXPercentage - blueGaugeBoxWidthPercentage / 2,
            blueGaugeBoxYPercentage - blueGaugeBoxHeightPercentage / 2,
            blueGaugeBoxWidthPercentage * blueProgress,
            blueGaugeBoxHeightPercentage);

        Rect redGaugeBox = screenRelativeRect(redGaugeBoxXPercentage - redGaugeBoxWidthPercentage / 2,
            redGaugeBoxYPercentage - redGaugeBoxHeightPercentage / 2,
            redGaugeBoxWidthPercentage, redGaugeBoxHeightPercentage);

        Rect redprogressGaugeBox = screenRelativeRect(redGaugeBoxXPercentage - redGaugeBoxWidthPercentage / 2,
            redGaugeBoxYPercentage - redGaugeBoxHeightPercentage / 2,
            redGaugeBoxWidthPercentage * redProgress,
            redGaugeBoxHeightPercentage);

        //superbars
        //blue 
        GUI.DrawTexture(blueGaugeBox, emptyBar);
        GUI.DrawTexture(blueProgressGaugeBox, blueBar);
        if (blueProgress == 1f) GUI.Label(blueGaugeBox, "SUPER READY !", superOkTextStyle);


        //red
        GUI.DrawTexture(redGaugeBox, emptyBar);
        GUI.DrawTexture(redprogressGaugeBox, redBar);
        if (redProgress == 1f) GUI.Label(redGaugeBox, "SUPER READY !", superOkTextStyle);


        //score
        GUI.Label(scoreBox, _game.right.nbPoints + "  -  " + _game.left.nbPoints, gameScoreTextStyle);

        //time
        GUI.Label(timeBox, "Time : " + ((int)_game.settings.score.period_time - (int)_game.arbiter.IngameTime), gameTimeTextStyle);
			
    }

    public Rect ResultRect, ResultButtonRect, ResultScoreRect;
    public GUIStyle ResultStyle, ResultScoreStyle;
    public int btnFontSize;

    void GUIGameOver()
    {
        Rect resultRect = screenRelativeRect(ResultRect);
        Rect resultButtonRect = screenRelativeRect(ResultButtonRect);
        Rect resultScoreRect = screenRelativeRect(ResultScoreRect);

        string result = "";
        if (_game.right.nbPoints < _game.left.nbPoints)
        {
            result = _game.left.Name + " win !";
        }
        else if (_game.left.nbPoints < _game.right.nbPoints)
        {
            result = _game.right.Name + " win !";
        }
        else
        {
            result = "Draw !";
        }

        GUI.Label(resultRect, result, ResultStyle);
        GUI.Label(resultScoreRect, _game.right.nbPoints + "  -  " + _game.left.nbPoints, ResultScoreStyle);

        GUIStyle btnStyle = GUI.skin.button;
        btnStyle.fontSize = btnFontSize;

        if (GUI.Button(resultButtonRect, "restart", btnStyle))
            _game.Reset();
    }
}
