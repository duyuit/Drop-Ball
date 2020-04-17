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
        //On(GameSetting.updateScore, (E) => UpdateScore(E));
        On(GameSetting.updateTimer, (E) => UpdateTimer(E));
        On(GameSetting.endGame, (E) => EndGame(E));
        On(GameSetting.outOfMove, (E) => OnOutOfMove(E));
    }

    private void UpdateOpponentPosition(SocketIOEvent e)
    {
        Debug.Log("On opponent move");
        Vector3 pos = new Vector3();
        pos.x = e.data["x"].n;
        pos.y = e.data["y"].n;
        pos.z = e.data["z"].n;

        onOpponentMove?.Invoke(pos);
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

        Debug.Log("Opponent Index : " + opponentIndex);
        Debug.Log("Start Game" + opponentName);
        _opponentName = opponentName;

        onStartGame(opponentName);
        Debug.LogFormat("Found opponent {0} ", opponentName);
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

    public void SendUpdatePosition(Vector3 pos)
    {
        JSONObject position = new JSONObject(JSONObject.Type.OBJECT);
        position.AddField("x", pos.x);
        position.AddField("y", pos.y);
        position.AddField("z", pos.z);
        Emit(GameSetting.updatePosition, position);
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

}
