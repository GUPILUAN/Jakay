using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class MenuController : MonoBehaviour
{   
    private DateTime perreo;
    private GameObject arriba, abajo, centro, boton, boton2, conf,user;
    private bool bandera = false, continuar = false, cambio = false, internet = false;
    private float duracionDeLaAnimacion = 1.0f, tiempoPasado = 0.0f, discosAnimados = 0.0f, shake = 0.0f; // Duración de la animación en
    private Vector3 escalaFinal = new Vector3(1.0f, 1.0f, 1.0f); // Escala Final
    private Vector3 escalaInicial = Vector3.zero; // Escala Inicial
    private Vector3 posicionInicial = new Vector3(15.0f,1.5f,0);
    private Vector3 posicionFinal = new Vector3(0f,1.5f,0);
    public GameObject mensajeInternet;
    
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
    public void clickLocal()
    {
        SceneManager.LoadScene("SeleccionLocal");
    }
    public void clickUser(){
        SceneManager.LoadScene("Usuario");
    }
    public void clickSettings(){
        SceneManager.LoadScene("Settings");
    }
     public void clickOnline()
    {
        if(!internet){
            Debug.Log("No hay conexión a Internet.");
            mensajeInternet.SetActive(true);
            mensajeInternet.GetComponent<TextMeshProUGUI>().text = "No tienes conexion a internet :(";
        } else{
            SceneManager.LoadScene("SeleccionOnline");
        }
    }
    

  
    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.HasKey("temp")){PlayerPrefs.DeleteKey("temp");}
        perreo = DateTime.UtcNow;
        string hora = "Dia: " + perreo.DayOfYear.ToString() + " " + perreo.Hour.ToString() + ":" + perreo.Minute.ToString() + ":" + perreo.Second.ToString();
        Debug.Log(PlayerPrefs.GetString("Hora de entrada"));
        Debug.Log(PlayerPrefs.GetString("usuario"));
        if (!PlayerPrefs.HasKey("Hora de entrada")){PlayerPrefs.SetString("Hora de entrada", hora);}
        try
        {
            Destroy(GameObject.Find("playerSkin"));
            Destroy(GameObject.Find("playerOtro"));
            Destroy(GameObject.Find("bgSkin"));
        }
        catch (System.Exception)
        {
            throw;
        }
        try
        {
            Destroy(GameObject.Find("OnlineController"));
        }
        catch (System.Exception)
        {
            throw;
        }
        
        Debug.Log("Hola mundo");
        boton = GameObject.Find("Button");
        boton2 = GameObject.Find("Button2");
        conf = GameObject.Find("conf");
        user = GameObject.Find("user");
        arriba = GameObject.Find("arriba");
        abajo = GameObject.Find("abajo");
        centro = GameObject.Find("centro");
        centro.transform.localScale = escalaInicial;
        arriba.transform.position = posicionInicial*-1;
        abajo.transform.position = posicionInicial;
        boton.transform.localScale = escalaInicial;
    }
    // Update is called once per frame
    void Update()
    {
        NetworkReachability reachability = Application.internetReachability;
        switch (reachability)
        {
            case NetworkReachability.NotReachable:
                internet = false;
                break;
            case NetworkReachability.ReachableViaCarrierDataNetwork:
              //  Debug.Log("Conexión a través de datos móviles.");
                if(PlayerPrefs.HasKey("usuario")){
                    mensajeInternet.SetActive(false);
                }
                internet = true;
                break;
            case NetworkReachability.ReachableViaLocalAreaNetwork:
               // Debug.Log("Conexión a través de una red Wi-Fi u otra red local.");
                mensajeInternet.SetActive(false);
                internet = true;
                break;
            
        }
        if (!bandera){
            boton.GetComponent<Button>().interactable = false;
            boton2.GetComponent<Button>().interactable = false;
            conf.GetComponent<Button>().interactable = false;
            user.GetComponent<Button>().interactable = false;
            
        }else{
            boton.GetComponent<Button>().interactable = true;
            boton2.GetComponent<Button>().interactable = true;
            conf.GetComponent<Button>().interactable = true;
            user.GetComponent<Button>().interactable = true;
        }
        if(tiempoPasado<duracionDeLaAnimacion){
            tiempoPasado += Time.deltaTime;
            // Interpola entre la escala inicial y la escala final en función del tiempo
            float t = tiempoPasado / duracionDeLaAnimacion;
            centro.transform.localScale = Vector3.Lerp(escalaInicial, new Vector3(117,117,117), t);
            boton.transform.localScale = Vector3.Lerp(escalaInicial, escalaFinal, t);
            boton2.transform.localScale = Vector3.Lerp(escalaInicial, escalaFinal, t);
            conf.transform.localScale = Vector3.Lerp(escalaInicial, escalaFinal, t);
            user.transform.localScale = Vector3.Lerp(escalaInicial, escalaFinal, t);
            if (centro.transform.localScale == new Vector3(117,117,117)){bandera = true;}
        }
        if (bandera && abajo.transform.position.x > posicionFinal.x && !continuar && arriba.transform.position.x < posicionFinal.x){
            discosAnimados += Time.deltaTime;
            // Interpola entre la posicion inicial y la posicion final en función del tiempo
            float t2 = discosAnimados / (duracionDeLaAnimacion - 0.5f);
            abajo.transform.position = Vector3.Lerp(posicionInicial, posicionFinal, t2);
            arriba.transform.position = Vector3.Lerp(posicionInicial*-1, posicionFinal, t2);
            if (arriba.transform.position == posicionFinal){continuar = true;}
        }
        if(continuar){
            if (!cambio){
                shake += Time.deltaTime;
                float t3 = shake / (duracionDeLaAnimacion+1);
                abajo.transform.position = Vector3.Lerp(posicionFinal, new Vector3(1f,1.5f,0) , t3);
                arriba.transform.position = Vector3.Lerp(posicionFinal, new Vector3(-1f,1.5f,0) , t3);
                if (arriba.transform.position == new Vector3(-1f,1.5f,0)){cambio = true; shake = 0.0f;}
            }else{
                shake += Time.deltaTime;
                float t3 = shake / (duracionDeLaAnimacion+1);
                abajo.transform.position = Vector3.Lerp(new Vector3(1f,1.5f,0), posicionFinal, t3);
                arriba.transform.position = Vector3.Lerp(new Vector3(-1f,1.5f,0), posicionFinal, t3);
                if (arriba.transform.position == posicionFinal){cambio = false; shake=0.0f;}
            }
        }
        
    }
   
}