using System;
using TMPro;
using UnityEngine;

public class TiempoTranscurrido : MonoBehaviour
{ static TiempoTranscurrido instance;
    private float tiempoDeCreacion, tiempoTranscurrido;
    TextMeshProUGUI text;
    private String texto;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            tiempoDeCreacion = Time.time;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Update()
    {
        tiempoTranscurrido = Time.time - tiempoDeCreacion;
        TimeSpan tiempoJugado = TimeSpan.FromSeconds(tiempoTranscurrido);

        texto = "Tiempo jugado: " + tiempoJugado.ToString("hh\\:mm\\:ss");

        if (GameObject.Find("tiempoTranscurridoText") != null)
        {
            text = GameObject.Find("tiempoTranscurridoText").GetComponent<TextMeshProUGUI>();
            text.text = texto;
        }

        Debug.Log(texto);
    }
}