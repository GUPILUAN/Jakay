using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class SkinOnline : MonoBehaviourPunCallbacks
{
  
    public List<Sprite> skins; //skins de las cuales vamos a elegir.
    public List<Sprite> back;
    private Vector3 escalaFinal = new Vector3(1.0f, 1.0f, 1.0f); // Escala  Final
    private int skinSeleccionada = 0, bgsSeleccionado = 0;
    [SerializeField] private GameObject skinIZQ, skinDER, izquierda, derecha, botonPlay, skin, bg, DW;
     public void Siguiente(SpriteRenderer srX){
        if (PlayerPrefs.HasKey("skinO")){
            skinSeleccionada = PlayerPrefs.GetInt("skinO");
        }
        skinSeleccionada ++;
        if (skinSeleccionada == skins.Count){
            skinSeleccionada = 0;
        } 
        srX.sprite = skins[skinSeleccionada];
        PlayerPrefs.SetInt("skinO",skinSeleccionada);
        
    }
    public void Anterior(SpriteRenderer srX){
        if (PlayerPrefs.HasKey("skinO")){
            skinSeleccionada = PlayerPrefs.GetInt("skinO");
        }
        skinSeleccionada--;
        if (skinSeleccionada < 0){
            skinSeleccionada = skins.Count - 1;
        }
        srX.sprite = skins[skinSeleccionada];
        PlayerPrefs.SetInt("skinO", skinSeleccionada);
    }

     public void Siguientebg(SpriteRenderer srX){
        if (PlayerPrefs.HasKey("ultimoO")){
            bgsSeleccionado = PlayerPrefs.GetInt("ultimoO");        
        }
        bgsSeleccionado++;
        if (bgsSeleccionado == back.Count){
            bgsSeleccionado = 0;
        }
        srX.sprite = back[bgsSeleccionado];
        PlayerPrefs.SetInt("ultimoO",bgsSeleccionado);
    }
     public void Anteriorbg(SpriteRenderer srX){
        if (PlayerPrefs.HasKey("ultimoO")){
            bgsSeleccionado = PlayerPrefs.GetInt("ultimoO");
        }
        bgsSeleccionado --;
        if (bgsSeleccionado < 0){
            bgsSeleccionado = back.Count - 1;
        }
        srX.sprite = back[bgsSeleccionado];
        PlayerPrefs.SetInt("ultimoO",bgsSeleccionado);
    }

    public float EleccionDeltiempo(Dropdown eleccionX)
    {
        if (eleccionX.options[eleccionX.value].text == "2 Min"){
            return 120.0f;
        }else if (eleccionX.options[eleccionX.value].text == "3 Min"){
            return 180.0f;
        }else if (eleccionX.options[eleccionX.value].text == "5 Min"){
            return 300.0f;
        }else{
            return 0;
        }
    }



     public void puchado(GameObject botonX)
    {
        // Se llama cuando se hace clic en el botón
        botonX.transform.localScale = new Vector3(1.1f,1.1f,1.1f);
    }

    public void noPuchado(GameObject botonX)
    {
        // Se llama cuando se libera el clic en el botón
        botonX.transform.localScale = escalaFinal;
    }


    public void clickJugar()
    {
        string nombreSala = PlayerPrefs.HasKey("temp") ? PlayerPrefs.GetString("temp") : PlayerPrefs.GetString("creado");
        if(!PlayerPrefs.HasKey("temp")){
            PlayerPrefs.SetFloat("tiempoJuego",EleccionDeltiempo(DW.GetComponent<Dropdown>()));
            GameObject.Find("OnlineController").GetComponent<OnlineMenuController>().CrearRoom(nombreSala);
            GameObject.Find("OnlineController").GetComponent<OnlineMenuController>().CargarEscena("JuegoOnline");
        }else{
            GameObject.Find("OnlineController").GetComponent<OnlineMenuController>().UnirseRoom(nombreSala);
            Debug.Log(PlayerPrefs.GetString("temp"));
            GameObject.Find("OnlineController").GetComponent<OnlineMenuController>().CargarEscena("JuegoOnline");
        }
        
       
    }
    public void clickAtras()
    {
        GameObject.Find("OnlineController").GetComponent<OnlineMenuController>().CargarEscena("Menu");
        PhotonNetwork.Disconnect();
    }
  

    private void Awake() {
        
        if (PlayerPrefs.HasKey("temp")){
            DW.SetActive(false);
            izquierda.SetActive(false);
            derecha.SetActive(false);
            bg.SetActive(false);
        }
        
    }
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetInt("ultimoO",0);
        PlayerPrefs.SetInt("skinO",0);
        //DontDestroyOnLoad(skin);
        //DontDestroyOnLoad(bg);
        skin.GetComponent<SpriteRenderer>().sprite = skins[0];
        bg.GetComponent<SpriteRenderer>().sprite = back[0];
       // GameObject.Find("OnlineController").GetComponent<OnlineMenuController>();
        Debug.Log("EL USERNAME DE ESTE USUARIO ES: " + PhotonNetwork.NickName);
    }
    
}