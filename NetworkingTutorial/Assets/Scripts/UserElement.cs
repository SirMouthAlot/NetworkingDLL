using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserElement : MonoBehaviour
{
    public Text username;
    public Text userstatus;

    public void UpdateUser(string p_username, string p_userstatus)
    {
        username.text = p_username;
        userstatus.text = p_userstatus;
    }
}
