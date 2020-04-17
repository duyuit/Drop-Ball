using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUtilities
{
    public static string RemoveQuote(string text)
    {
        string newText = text;
        newText = newText.Replace("\"", "");
        return newText;
    }
}
