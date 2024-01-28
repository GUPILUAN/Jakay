using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SeleccionO : MonoBehaviour
{
 private GameObject  boton, boton2, atras, izquierdo, derecho;
    private bool continuar = false;
    private float duracionDeLaAnimacion = 0.5f, otraAnimacion = 0.0f; // Duración de la animación en
    private Vector3 escalaFinal = new Vector3(1.0f, 1.0f, 1.0f); // Escala  Final
    private Vector3 escalaInicial =  Vector3.zero; // Escala Inicial

    public void clickAtras()
    {
        SceneManager.LoadScene("Menu");
    }
   // public void clickOnline()
    //{
      //  SceneManager.LoadScene("Network");
    //}

    

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
        izquierdo = GameObject.Find("izquierda");
        derecho = GameObject.Find("derecha");

        boton.transform.localScale = escalaInicial;
        boton2.transform.localScale = escalaInicial;
        atras.transform.localScale = escalaInicial;
        izquierdo.transform.localScale = escalaInicial;
        derecho.transform.localScale = escalaInicial;
        if (PlayerPrefs.HasKey("unido")){
            izquierdo.SetActive(false);
            derecho.SetActive(false);
        }
    }
    // Update is called once per frame
    void Update()
    {

        if (!continuar){
            boton.GetComponent<Button>().interactable = false;
            boton2.GetComponent<Button>().interactable = false;
            atras.GetComponent<Button>().interactable = false; 
            izquierdo.GetComponent<Button>().interactable = false;
            derecho.GetComponent<Button>().interactable = false;            
        }else{
            boton.GetComponent<Button>().interactable = true;
            boton2.GetComponent<Button>().interactable = true;
            atras.GetComponent<Button>().interactable = true;
            izquierdo.GetComponent<Button>().interactable = true;
            derecho.GetComponent<Button>().interactable = true; 
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
            if (boton.transform.localScale == escalaFinal){continuar = true;}
        }

    }
}
