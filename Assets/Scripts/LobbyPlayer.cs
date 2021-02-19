using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayer : MonoBehaviour
{
    public Image color;
    public Text name;
    public static int numero = 0;

    private void Start()
    {
        name.text = "Jugador " + numero;
        numero += 1;
    }

    public void SetColor(int i)
    {
        switch (i)
        {
            case 0:
                color.color = Color.red;
                break;
            
            case 1:
                color.color = Color.green;
                break;
            
            case 2:
                color.color = Color.blue;
                break;
        }
    }

    public void SetName(string s)
    {
        name.text = s;
    }
}
