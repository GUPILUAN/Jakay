using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AnimacionReset : MonoBehaviour
{   
    private GameObject arriba, abajo, centro, boton, boton2, atras;
    private bool bandera = false, continuar = false;
    private float duracionDeLaAnimacion = 0.5f, tiempoPasado = 0.0f, otraAnimacion = 0.0f; // Duración de la animación en
    private Vector3 escalaFinal = new Vector3(1.0f, 1.0f, 1.0f); // Escala  Final
    private Vector3 escalaInicial =  Vector3.zero; // Escala Inicial
    private Vector3 posicionInicial = new Vector3(0f,1.5f,0);
    private Vector3 posicionFinal =  new Vector3(15.0f,1.5f,0);

    public void clickAtras()
    {
        SceneManager.LoadScene("Menu");
    }
    public void clickModo1()
    {
        if(PlayerPrefs.HasKey("modo")){
            PlayerPrefs.DeleteKey("modo");
        }
         if(PlayerPrefs.HasKey("duo")){
            PlayerPrefs.DeleteKey("duo");
        }
        PlayerPrefs.SetString("modo","pvc");
        SceneManager.LoadScene("SeleccionSkin");
    }
    public void clickModo2()
    {
        if(PlayerPrefs.HasKey("modo")){
            PlayerPrefs.DeleteKey("modo");
        }
        PlayerPrefs.SetInt("duo",1);
        PlayerPrefs.SetString("modo","pvp");
        SceneManager.LoadScene("SeleccionSkin");
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
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Hola mundo");
        boton = GameObject.Find("Button");
        boton2 = GameObject.Find("Button2");
        atras = GameObject.Find("atras");
        arriba = GameObject.Find("arriba");
        abajo = GameObject.Find("abajo");
        centro = GameObject.Find("centro");
        centro.transform.localScale = escalaInicial;
        arriba.transform.position = posicionInicial;
        abajo.transform.position = posicionInicial;
        boton.transform.localScale = escalaInicial;
        boton2.transform.localScale = escalaInicial;
        atras.transform.localScale = escalaInicial;

    }
    // Update is called once per frame
    void Update()
    {
        NetworkReachability reachability = Application.internetReachability;
        switch (reachability)
        {
            case NetworkReachability.NotReachable:
                SceneManager.LoadScene("Menu");
                break;
        }
        if (!bandera && !continuar){
            boton.GetComponent<Button>().interactable = false;
            boton2.GetComponent<Button>().interactable = false;
            atras.GetComponent<Button>().interactable = false;            
        }else{
            boton.GetComponent<Button>().interactable = true;
            boton2.GetComponent<Button>().interactable = true;
            atras.GetComponent<Button>().interactable = true;
        }
        if(tiempoPasado<duracionDeLaAnimacion){
            tiempoPasado += Time.deltaTime;
            // Interpola entre la escala inicial y la escala final en función del tiempo
            float t = tiempoPasado / duracionDeLaAnimacion;
            centro.transform.localScale = Vector3.Lerp(new Vector3(117f,117f,117f), Vector3.zero, t);
            abajo.transform.position = Vector3.Lerp(posicionInicial, posicionFinal, t);
            arriba.transform.position = Vector3.Lerp(posicionInicial, posicionFinal*-1, t);
            if (centro.transform.localScale == Vector3.zero){bandera = true;}
        }
        if (bandera && !continuar){
            otraAnimacion += Time.deltaTime;
            // Interpola entre la posicion inicial y la posicion final en función del tiempo
            float t2 = otraAnimacion / duracionDeLaAnimacion;
            boton.transform.localScale = Vector3.Lerp(escalaInicial, escalaFinal, t2);
            boton2.transform.localScale = Vector3.Lerp(escalaInicial, escalaFinal, t2);
            atras.transform.localScale = Vector3.Lerp(escalaInicial, escalaFinal, t2);
            if (boton.transform.localScale == escalaFinal){continuar = true;}
        }

    }
}