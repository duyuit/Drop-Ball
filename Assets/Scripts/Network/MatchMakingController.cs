using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchMakingController : MonoBehaviour
{
    public GameObject requestNamePanel;
    public GameObject findingPanel;
    public GameObject foundPanel;

    public InputField playerName;
    public Text opponentNameText;
    public Text remainSecondText;

    private int _remainSeconds = 5;
    private DelayFunctionHelper _delayFunctionHelper;
    // Start is called before the first frame update
    void Start()
    {
        NetworkController.Instance.onStartGame += OnStartGame;
        _delayFunctionHelper = gameObject.AddComponent<DelayFunctionHelper>();
    }
    private void OnDestroy()
    {
        NetworkController.Instance.onStartGame -= OnStartGame;
    }

    public void OnStartGame(string opponentName)
    {
        findingPanel.SetActive(false);
        foundPanel.SetActive(true);
        opponentNameText.text = opponentName;

        _delayFunctionHelper.delayFunction(() =>
        {
            foundPanel.SetActive(false);
            GlobalVariable.isOnline = true;
            SceneController.Instance.LoadOnlineScene();
        }, GameSetting.readyTime);

        InvokeRepeating("MinusStartTime", 0.0f, 1f);
    }

    public void StartMatchMaking()
    {
        string name = playerName.text;
        if (name == "")
            name = "Billy  " + Random.Range(1, 100);

        NetworkController.Instance.ConnectToServer();
        //if (HasName())
        //{
        //requestNamePanel.SetActive(false);
        findingPanel.SetActive(true);
        _delayFunctionHelper.delayFunction(() =>
        {
            NetworkController.Instance.RegisterName(name);
            NetworkController.Instance.StartMatchMaking();
        }, 1.0f);
        //}
        //else
        //{
        //    requestEnterName();
        //}
    }
    public void CancelMatchMaking()
    {
        findingPanel.SetActive(false);
        NetworkController.Instance.Emit(GameSetting.canceltMatchMaking);
    }
    private bool HasName()
    {
        string userName = PlayerPrefs.GetString("userName", "");
        return userName != "";
    }
    private void requestEnterName()
    {
        requestNamePanel.SetActive(true);
    }

    private void MinusStartTime()
    {
        _remainSeconds--;
        remainSecondText.text = string.Format("Game start in {0}s . . .", _remainSeconds);
    }

}
