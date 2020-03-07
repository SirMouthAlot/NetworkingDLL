using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextElement : MonoBehaviour
{
    public Text username;
    public Text message;

    public void UpdateText(string p_username, string p_text)
    {
        username.text = p_username;
        message.text = p_text;
    }
}
