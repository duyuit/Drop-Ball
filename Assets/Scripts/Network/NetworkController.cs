using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using System;
using UnityEngine.UI;

public class NetworkController : SocketIOComponent
{
    public InputField ipText;
    private string _id;
    private string _opponentName;
    public string OpponentName { get { return _opponentName; } }


    // Singleton NetworkController.
    public static NetworkController Instance = null;

    public Action<string> onStartGame;
    public Action<string> onUpdateScore;
    public Action<Vector3> onOpponentMove;
    public Action<Vector2> onOpponentVelo;
    public Action<bool, float> onUpdateRotation;
    public Action<Vector3> onBallMove;
    public Action<Vector3> onFireBall;
    public Action<PlayerTag,bool> onReset;

    //public Action<Tile> onUpdateBoard;
    public Action<string> onUpdateTimer;
    public Action<int> onEndGame;
    public Action onOutOfMove;

    public bool isConnected = false;

    private DelayFunctionHelper _delayFunctionHelper;
    public override void Awake()
    {
        //base.Awake();
        // If there is not already an instance of SoundManager, set it to this.
        if (Instance == null)
        {
            Instance = this;
        }
        //If an instance already exists, destroy whatever this object is to enforce the singleton.
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        _delayFunctionHelper = gameObject.AddComponent<DelayFunctionHelper>();
        DontDestroyOnLoad(gameObject);
    }



    // Start enqueue to find another player

    public void StartMatchMaking()
    {
        Debug.Log("Start match making");
        Emit(GameSetting.startMatchMaking);
    }

    public void ConnectToServer()
    {
        if (!isConnected)
        {
            //ip = ipText.text;

            base.Awake();
            base.Connect();
            _delayFunctionHelper.delayFunction(() =>
            {
                if (!isConnected)
                {
                    Debug.Log("Time out!");
                }
            }, 3.0f);
            RegisterEvent();
        }
    }

    public void HasWin()
    {
        Emit(GameSetting.hasWin);
    }

    // Start is called before the first frame update
    public override void Start()
    {
    }

    public void RegisterEvent()
    {
        // Connect success
        On("open", (E) => { isConnected = true; });

        On(GameSetting.register, (E) => Register(E));
        On(GameSetting.startGame, (E) => StartGame(E));
        On(GameSetting.updatePosition, (E) => UpdateOpponentPosition(E));
        On(GameSetting.updateVelo, (E) => UpdateOpponentVelo(E));
        On(GameSetting.fireBall, (E) => OnFireBall(E));
        On(GameSetting.updateBallPosition, (E) => UpdateBallPosition(E));
        On(GameSetting.updateArrowRotation, (E) => UpdateArrowRotation(E));
        On(GameSetting.reset, (E) => OnReset(E));
        //On(GameSetting.updateScore, (E) => UpdateScore(E));
        On(GameSetting.updateTimer, (E) => UpdateTimer(E));
        On(GameSetting.endGame, (E) => EndGame(E));
        On(GameSetting.outOfMove, (E) => OnOutOfMove(E));
    }

    private void OnReset(SocketIOEvent e)
    {
        int winner = (int)e.data["winner"].n;
        PlayerTag winnerTag = PlayerTag.NONE;

        if (winner == 2)
            winnerTag = PlayerTag.PLAYER1;
        else
            winnerTag = PlayerTag.PLAYER2;

        onReset?.Invoke(winnerTag, true);

    }

    private void UpdateArrowRotation(SocketIOEvent e)
    {
        onUpdateRotation(e.data["isLeft"].b, e.data["currentYRotation"].n);
    }

    private void UpdateBallPosition(SocketIOEvent e)
    {
        onBallMove?.Invoke(CreateVector3FromJson(e));
    }

    private void OnFireBall(SocketIOEvent e)
    {
        Debug.Log("Opponent Fire");
        onFireBall?.Invoke(CreateVector3FromJson(e));
    }

    private void UpdateOpponentVelo(SocketIOEvent e)
    {
        Vector3 velo = new Vector3();
        velo.x = e.data["x"].n;
        velo.y = e.data["y"].n;

        onOpponentVelo?.Invoke(velo);
    }

    private void UpdateOpponentPosition(SocketIOEvent e)
    {
        onOpponentMove?.Invoke(CreateVector3FromJson(e));
    }

    public void EndGame(SocketIOEvent e)
    {
        Debug.Log("End Game");
        string result = RemoveQuote(e.data["result"].ToString());
        int resultNumber = Convert.ToInt32(result);
        onEndGame(resultNumber);
    }
    public void UpdateTimer(SocketIOEvent e)
    {
        string timer = RemoveQuote(e.data["time"].ToString());
        onUpdateTimer(timer);
    }
    public void StartGame(SocketIOEvent e)
    {
        string opponentName = RemoveQuote(e.data["userName"].ToString());
        int opponentIndex = int.Parse(RemoveQuote(e.data["index"].ToString()));

        GlobalVariable.myIndex = opponentIndex;

        _opponentName = opponentName;

        onStartGame(opponentName);
    }
    public void OnConnectSuccess()
    {
    }
    public void RegisterName(string name)
    {
        Emit(GameSetting.registerName, new JSONObject(new Dictionary<string, string> { ["name"] = name }));// { name: "duykk"});
    }

    public void Register(SocketIOEvent e)
    {
        string id = e.data["id"].ToString();
        _id = RemoveQuote(id);
        OnConnectSuccess();
        Debug.LogFormat("id {0}", _id);
    }

    //public void UpdateScore(SocketIOEvent e)
    //{
    //    string score = GameUtilities.GameUtilities.RemoveQuote(e.data["score"].ToString());
    //    onUpdateScore(score);
    //}
    // Update is called once per frame
    //public override void Update()
    //{
    //    base.Update();
    //}

    public void SendUpdateArrowRotate(bool isLeft, float currentYRotation)
    {
        JSONObject jSONObject = new JSONObject(JSONObject.Type.OBJECT);
        jSONObject.AddField("isLeft", isLeft);
        jSONObject.AddField("currentYRotation", currentYRotation);

        Emit(GameSetting.updateArrowRotation, jSONObject);
    }

    public void SendBallPosition(Vector3 position)
    {
        Emit(GameSetting.updateBallPosition, CreateJsonFromVector3(position));
    }

    public void SendFireBall(Vector3 force)
    {
        Emit(GameSetting.fireBall, CreateJsonFromVector3(force));
    }

    public void SendUpdateVelocity(Vector2 velo)
    {
        JSONObject veloJSon = new JSONObject(JSONObject.Type.OBJECT);
        veloJSon.AddField("x", velo.x);
        veloJSon.AddField("y", velo.y);
        Emit(GameSetting.updateVelo, veloJSon);
    }

    public void SendUpdatePosition(Vector3 pos)
    {
        Emit(GameSetting.updatePosition, CreateJsonFromVector3(pos));
    }

    public void SendUpdateScore(string v)
    {
        Debug.Log("send update score");
        Emit(GameSetting.updateScore, new JSONObject(new Dictionary<string, string> { ["score"] = v }));
    }
    public void SendOutOfMove()
    {
        Debug.Log("Out of move");
        Emit(GameSetting.outOfMove);
    }
    public void ExitRoom()
    {
        Emit(GameSetting.exitRoom);
    }

    public void OnOutOfMove(SocketIOEvent e)
    {
        onOutOfMove();
    }

    public string RemoveQuote(string text)
    {
        string newText = text;
        newText = newText.Replace("\"", "");
        return newText;
    }

    private JSONObject CreateJsonFromVector3(Vector3 vector3)
    {
        JSONObject jSONObject = new JSONObject(JSONObject.Type.OBJECT);
        jSONObject.AddField("x", vector3.x);
        jSONObject.AddField("y", vector3.y);
        jSONObject.AddField("z", vector3.z);

        return jSONObject;
    }

    private Vector3 CreateVector3FromJson(SocketIOEvent e)
    {
        Vector3 pos = new Vector3();
        pos.x = e.data["x"].n;
        pos.y = e.data["y"].n;
        pos.z = e.data["z"].n;

        return pos;
    }

}
