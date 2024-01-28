using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class settingsConroller : MonoBehaviour
{   
    public GameObject arriba, abajo, centro, boton, barra, atras, text;
    private bool bandera = false, continuar = false;
    private float duracionDeLaAnimacion = 0.5f, tiempoPasado = 0.0f, otraAnimacion = 0.0f; // Duración de la animación en
    private Vector3 escalaFinal = new Vector3(1.0f, 1.0f, 1.0f); // Escala  Final
    private Vector3 escalaInicial =  Vector3.zero; // Escala Inicial
    private Vector3 posicionInicial = new Vector3(0f,1.5f,0);
    private Vector3 posicionFinal =  new Vector3(15.0f,1.5f,0);

    public void clickAtras()
    {
        GameObject.Find("musica").GetComponent<AudioSource>().volume = PlayerPrefs.HasKey("volumen") ? PlayerPrefs.GetFloat("volumen"):1;
        SceneManager.LoadScene("Menu");
    }

    public void VolumenReal(){
        GameObject.Find("musica").GetComponent<AudioSource>().volume = barra.GetComponent<Slider>().value;
    }
    public void clickContinuar()
    {
        PlayerPrefs.SetFloat("volumen",barra.GetComponent<Slider>().value);
        SceneManager.LoadScene("Menu");
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
        centro.transform.localScale = escalaInicial;
        arriba.transform.position = posicionInicial;
        abajo.transform.position = posicionInicial;
        boton.transform.localScale = escalaInicial;
        barra.transform.localScale = escalaInicial;
        atras.transform.localScale = escalaInicial;
        text.transform.localScale = escalaInicial;
        barra.GetComponent<Slider>().value = GameObject.Find("musica").GetComponent<AudioSource>().volume;

    }
    // Update is called once per frame
    void Update()
    {
        if (!bandera && !continuar){
            boton.GetComponent<Button>().interactable = false;
            atras.GetComponent<Button>().interactable = false;            
        }else if(bandera && continuar){
            boton.GetComponent<Button>().interactable = true;
            
        }else{
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
            barra.transform.localScale = Vector3.Lerp(escalaInicial, escalaFinal*5, t2);
            atras.transform.localScale = Vector3.Lerp(escalaInicial, escalaFinal, t2);
            text.transform.localScale = Vector3.Lerp(escalaInicial, escalaFinal, t2);
            if (boton.transform.localScale == escalaFinal){continuar = true;}
        }

    }
}
