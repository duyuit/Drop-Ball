using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetting
{
    public static int readyTime = 1;

    // Network command
    public static string register = "register";
    public static string registerName = "registerName";
    public static string startMatchMaking = "startMatchMaking";
    public static string canceltMatchMaking = "cancelMatchMaking";
    public static string startGame = "startGame";
    public static string updateScore = "updateScore";
    public static string updateBoard = "updateBoard";
    public static string generateTile = "generateTile";
    public static string updateTimer = "updateTimer";
    public static string endGame = "endGame";
    public static string exitRoom = "exitRoom";
    public static string outOfMove = "outOfMove";
    public static string updatePosition = "updatePosition";
    public static string updateVelo = "updateVelo";
    public static string fireBall = "fireBall";
    public static string updateBallPosition = "updateBallPosition";
    public static string updateArrowRotation = "updateArrowRotation";
    public static string hasWin = "hasWin";
    public static string reset = "reset";

}
