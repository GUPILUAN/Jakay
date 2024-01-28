using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SeleccionController : MonoBehaviour
{
    public GameObject j1,j2,bg, slider;
    private GameObject  boton, boton2, atras, botoncopia, boton2copia, izquierdo, derecho;
    private bool continuar = false;
    private float duracionDeLaAnimacion = 0.5f, otraAnimacion = 0.0f; // Duración de la animación en
    private Vector3 escalaFinal = new Vector3(1.0f, 1.0f, 1.0f); // Escala  Final
    private Vector3 escalaFinalObjetos = new Vector3(0.16f, 0.16f, 0.16f); // Escala  Final
    private Vector3 escalaInicial =  Vector3.zero; // Escala Inicial
    public List<Sprite> skins = new List<Sprite>();
    public List<Sprite> bgs = new List<Sprite>();
    private int skinSeleccionada = 0, bgsSeleccionado = 0;
    public GameObject eleccion;
    private bool visita;

    public void botonesVisita(){
        visita = true;
    }
    public void botonesLocal(){
        visita = false;
    }

    public void Siguiente(SpriteRenderer srX){
        string clave = visita ? "ultimaV" : "ultimaL";
        if (PlayerPrefs.HasKey(clave)){
            skinSeleccionada = PlayerPrefs.GetInt(clave);
        }
        skinSeleccionada ++;
        if (skinSeleccionada == skins.Count){
            skinSeleccionada = 0;
        } 
        srX.sprite = skins[skinSeleccionada];
        PlayerPrefs.SetInt(clave,skinSeleccionada);

        
    }
    public void Anterior(SpriteRenderer srX){
        string clave = visita ? "ultimaV" : "ultimaL";
        if (PlayerPrefs.HasKey(clave)){
            skinSeleccionada = PlayerPrefs.GetInt(clave);
        }
        skinSeleccionada--;
        if (skinSeleccionada < 0){
            skinSeleccionada = skins.Count - 1;
        }
        srX.sprite = skins[skinSeleccionada];
        PlayerPrefs.SetInt(clave, skinSeleccionada);
    }

     public void Siguientebg(SpriteRenderer srX){
        if (PlayerPrefs.HasKey("ultimo")){
            bgsSeleccionado = PlayerPrefs.GetInt("ultimo");        
        }
        bgsSeleccionado++;
        if (bgsSeleccionado == bgs.Count){
            bgsSeleccionado = 0;
        }
        srX.sprite = bgs[bgsSeleccionado];
        PlayerPrefs.SetInt("ultimo",bgsSeleccionado);
    }
     public void Anteriorbg(SpriteRenderer srX){
        if (PlayerPrefs.HasKey("ultimo")){
            bgsSeleccionado = PlayerPrefs.GetInt("ultimo");
        }
        bgsSeleccionado --;
        if (bgsSeleccionado < 0){
            bgsSeleccionado = bgs.Count - 1;
        }
        srX.sprite = bgs[bgsSeleccionado];
        PlayerPrefs.SetInt("ultimo",bgsSeleccionado);
    }

    public void clickAtras()
    {
        SceneManager.LoadScene("Menu");
    }
    public void clickJugar()
    {
        PlayerPrefs.SetFloat("tiempoJuego",EleccionDeltiempo(eleccion.GetComponent<Dropdown>()));
       // PrefabUtility.SaveAsPrefabAsset(j1,"Assets/Prefabs/otros/playerSkin.prefab");
       // PrefabUtility.SaveAsPrefabAsset(j2,"Assets/Prefabs/otros/playerOtro.prefab");
        if(PlayerPrefs.GetInt("ultimo") == 2 || PlayerPrefs.GetInt("ultimo") == 3){
            bg.tag = "neon";
        }else{
            bg.tag = "normal";
        }
       // PrefabUtility.SaveAsPrefabAsset(bg,"Assets/Prefabs/otros/bg.prefab");
        SceneManager.LoadScene("JuegoLocal");
        j2.SetActive(true);
    }
    public void seleccionDificultad(){
        PlayerPrefs.SetFloat("dificultad", slider.GetComponent<Slider>().value);
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
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(j1);
        DontDestroyOnLoad(j2);
        DontDestroyOnLoad(bg);
        PlayerPrefs.SetInt("ultimo",0);
        PlayerPrefs.SetInt("ultimaL",0);
        PlayerPrefs.SetInt("ultimaV",0);
        Debug.Log("Hola mundo");
        boton = GameObject.Find("Button");
        boton2 = GameObject.Find("Button2");
        botoncopia = GameObject.Find("Button(1)");
        boton2copia = GameObject.Find("Button2(1)");
        atras = GameObject.Find("atras");
        izquierdo = GameObject.Find("izquierda");
        derecho = GameObject.Find("derecha");
        j1.SetActive(true);
        j2.SetActive(true);
        bg.SetActive(true);
        j1.GetComponent<SpriteRenderer>().sprite = skins[0];
        j2.GetComponent<SpriteRenderer>().sprite = skins[0];
        bg.GetComponent<SpriteRenderer>().sprite = bgs[0];
        slider.GetComponent<Slider>().value = PlayerPrefs.HasKey("dificultad") ? PlayerPrefs.GetFloat("dificultad") :1.5f;
        if(!PlayerPrefs.HasKey("duo")){
            botoncopia.SetActive(false);
            boton2copia.SetActive(false);
            j2.transform.localScale = escalaInicial;
            slider.SetActive(true);
            slider.transform.localScale = escalaInicial;
        }else{
            botoncopia.transform.localScale = escalaInicial;
            boton2copia.transform.localScale = escalaInicial;
            j2.transform.localScale = escalaInicial;
        }
        boton.transform.localScale = escalaInicial;
        boton2.transform.localScale = escalaInicial;
        atras.transform.localScale = escalaInicial;
        izquierdo.transform.localScale = escalaInicial;
        derecho.transform.localScale = escalaInicial;
        j1.transform.localScale = escalaInicial;
        bg.transform.localScale = escalaInicial;
        
    }
    // Update is called once per frame
    void Update()
    {
        if (!continuar&&!PlayerPrefs.HasKey("duo")){
            boton.GetComponent<Button>().interactable = false;
            boton2.GetComponent<Button>().interactable = false;
            atras.GetComponent<Button>().interactable = false; 
            izquierdo.GetComponent<Button>().interactable = false;
            derecho.GetComponent<Button>().interactable = false;  
            slider.GetComponent<Slider>().interactable = false;          
        }else if(!continuar&&PlayerPrefs.HasKey("duo")){
            boton.GetComponent<Button>().interactable = false;
            boton2.GetComponent<Button>().interactable = false;
            atras.GetComponent<Button>().interactable = false;
            botoncopia.GetComponent<Button>().interactable = false;
            boton2copia.GetComponent<Button>().interactable = false;
            izquierdo.GetComponent<Button>().interactable = false;
            derecho.GetComponent<Button>().interactable = false; 
        }else if(continuar&& PlayerPrefs.HasKey("duo")){
            boton.GetComponent<Button>().interactable = true;
            boton2.GetComponent<Button>().interactable = true;
            atras.GetComponent<Button>().interactable = true;
            botoncopia.GetComponent<Button>().interactable = true;
            boton2copia.GetComponent<Button>().interactable = true;
            izquierdo.GetComponent<Button>().interactable = true;
            derecho.GetComponent<Button>().interactable = true; 
        }else{
            boton.GetComponent<Button>().interactable = true;
            boton2.GetComponent<Button>().interactable = true;
            atras.GetComponent<Button>().interactable = true;
            izquierdo.GetComponent<Button>().interactable = true;
            derecho.GetComponent<Button>().interactable = true; 
            slider.GetComponent<Slider>().interactable = true;   
        }
        if (!continuar){
            otraAnimacion += Time.deltaTime;
            // Interpola entre la posicion inicial y la posicion final en función del tiempo
            float t2 = otraAnimacion / duracionDeLaAnimacion;
            boton.transform.localScale = Vector3.Lerp(escalaInicial, escalaFinal, t2);
            boton2.transform.localScale = Vector3.Lerp(escalaInicial, escalaFinal, t2);
            atras.transform.localScale = Vector3.Lerp(escalaInicial, escalaFinal, t2);
            derecho.transform.localScale = Vector3.Lerp(escalaInicial, escalaFinal, t2);
            izquierdo.transform.localScale = Vector3.Lerp(escalaInicial, escalaFinal, t2);
            j1.transform.localScale = Vector3.Lerp(escalaInicial, escalaFinalObjetos, t2);
            bg.transform.localScale = Vector3.Lerp(escalaInicial, escalaFinalObjetos, t2);
            if (PlayerPrefs.HasKey("duo")){
                botoncopia.transform.localScale = Vector3.Lerp(escalaInicial, escalaFinal, t2);
                boton2copia.transform.localScale = Vector3.Lerp(escalaInicial, escalaFinal, t2);
                j2.transform.localScale = Vector3.Lerp(escalaInicial, escalaFinalObjetos, t2);
            }else{
                slider.transform.localScale = Vector3.Lerp(escalaInicial, escalaFinal*5, t2);  
            }
            if (boton.transform.localScale == escalaFinal){continuar = true;}
        }

    }
}
