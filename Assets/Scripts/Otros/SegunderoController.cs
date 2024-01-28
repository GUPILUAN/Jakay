using System;
using TMPro;
using UnityEngine;
public class SegunderoController : MonoBehaviour
{
    public GameObject tiempo;
    private TextMeshProUGUI tiempoTxt;
    public float duracionDelJuego; 
    // Start is called before the first frame update
    void Start()
    {
        duracionDelJuego = PlayerPrefs.GetFloat("tiempoJuego");
        mostrarTiempo();
    }
    public void mostrarTiempo(){
        tiempoTxt = tiempo.GetComponent<TextMeshProUGUI>();
        tiempoTxt.text = Math.Truncate(duracionDelJuego/60) +":"+ (Math.Truncate(duracionDelJuego%60) >= 10 ? Math.Truncate(duracionDelJuego%60).ToString() : "0" + Math.Truncate(duracionDelJuego%60).ToString());
    }
    public void segundero(){
        if (duracionDelJuego > 0 ){
        duracionDelJuego -= Time.deltaTime;
       // PlayerPrefs.SetFloat("tiempoJuego",duracionDelJuego);
        }
        tiempoTxt.text = Math.Truncate(duracionDelJuego/60) +":"+ (Math.Truncate(duracionDelJuego%60) >= 10 ? Math.Truncate(duracionDelJuego%60).ToString() : "0" + Math.Truncate(duracionDelJuego%60).ToString());
    }
}
