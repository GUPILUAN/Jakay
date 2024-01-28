using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private GameObject skin1,skin2,bgs;
    public GameObject bg,j1,j2, segundero, marcador, pausaBoton, borde, puck, meta1, meta2, marcadorLocal,MarcadorVisita , umbral,umbral2, panel1, panel2;
    private Sprite backGroud, jugador, jugador2;
    [SerializeField] private Sprite bordeNeon;
    private bool inteligenciaArtificial, pausa, ocurriendo = false, terminado = false, puckEnCancha;
    private JugadorController jc1, jc2;
    private TextMeshProUGUI marcadorV, marcadorL;
    private DiscoController puckC;
    private Rigidbody2D rb1, rb2;
    private Quaternion rotacionG1, rotacionG2;
    public Sprite puckneon, pausaneon, reaunudarB, reanudarNeon;
    private Sprite pausaN;
    private float offset;
    Vector2 posicion0,posAnterior;
    [SerializeField] private AudioClip MusicaMaestro;
    private GameObject music;
    private AudioSource aS;

    private void Awake() {
        segundero.GetComponent<SegunderoController>().duracionDelJuego = PlayerPrefs.GetFloat("tiempoJuego");
        music = GameObject.Find("musica");
        aS = music.GetComponent<AudioSource>();
         if(music != null){
            if(aS.isPlaying){
                aS.volume = aS.volume * 0.5f;
                aS.Stop();
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {   
        Application.targetFrameRate = 60;
        pausa = false;
        InicializarElementos();
        InicializarUI();
        //Destroy(bgs);
         bgs.SetActive(false);
         Destroy(skin1);
         Destroy(skin2);  
    }
    private void InicializarElementos(){
        bgs = GameObject.Find("bgSkin");
        skin1 = GameObject.Find("playerSkin");
        skin2 = GameObject.Find("playerOtro");
        jc1 = j1.GetComponent<JugadorController>();
        jc2 = j2.GetComponent<JugadorController>();
        rb1 = j1.GetComponent<Rigidbody2D>();
        rb2 = j2.GetComponent<Rigidbody2D>();
        puckC = puck.GetComponent<DiscoController>();  
        if (PlayerPrefs.GetString("modo")=="pvp"){
            inteligenciaArtificial = false;
            jc1.jugadorReal=true;
            jc2.jugadorReal=true;
        }else if (PlayerPrefs.GetString("modo")=="pvc"){
            inteligenciaArtificial = true;
        }
        if (inteligenciaArtificial){
            jc2.jugadorReal = false;
            jc1.jugadorReal = true;
        }
    }
    private void InicializarUI(){
        marcadorL = marcadorLocal.GetComponent<TextMeshProUGUI>();
        marcadorV = MarcadorVisita.GetComponent<TextMeshProUGUI>();
        backGroud = bgs.GetComponent<SpriteRenderer>().sprite;
        pausaN = pausaBoton.GetComponent<Image>().sprite;
        if (bgs.tag == "neon"){
            borde.GetComponent<SpriteRenderer>().sprite = bordeNeon;
            segundero.GetComponent<TextMeshProUGUI>().color = Color.white;
            marcadorL.color = Color.white;
            marcadorV.color = Color.white;
            marcadorL.alpha = 0.5f;
            marcadorV.alpha = 0.5f;
            borde.GetComponent<SpriteRenderer>().color = Color.magenta;
            puck.GetComponent<SpriteRenderer>().sprite = puckneon;
            pausaBoton.GetComponent<Image>().sprite = pausaneon;
        }else{
            borde.GetComponent<SpriteRenderer>().color = Color.black;
        }
        bg.GetComponent<SpriteRenderer>().sprite = backGroud;
        jugador = skin1.GetComponent<SpriteRenderer>().sprite;
        j1.GetComponent<SpriteRenderer>().sprite = jugador;
        jugador2 = skin2.GetComponent<SpriteRenderer>().sprite;
        j2.GetComponent<SpriteRenderer>().sprite = jugador2;
        j1.SetActive(false);
        j2.SetActive(false);
        puck.SetActive(false);        
    }
    // Update is called once per frame
    void Update()
    {
        if(music != null){
            if(!aS.isPlaying){
                aS.PlayOneShot(MusicaMaestro);
            }
        }
        if(ocurriendo){
            if (pausa){
                EnPausa();
            }else{
                pasaEltiempo();
                Physics2D.IgnoreCollision(puckC.GetComponent<Collider2D>(),meta1.GetComponent<Collider2D>(),true);
                Physics2D.IgnoreCollision(puckC.GetComponent<Collider2D>(),meta2.GetComponent<Collider2D>(),true);
                //Verifica si el puck se va de largo (es decir, entra en la porteria) y lo regresa a la posicion original.
                //También desactiva el juego por un momento para que empiece la animacion de nuevo, por lo que también regresa a los jugadores.
                checarGol(1);
                jc1.ReiniciarPos(puckC.fueGol);
                jc2.ReiniciarPos(puckC.fueGol);   
                if(!jc2.jugadorReal){
                inteligenciaA(PlayerPrefs.HasKey("dificultad") ? PlayerPrefs.GetFloat("dificultad"):1.5f);
                }
            }
            if (segundero.GetComponent<SegunderoController>().duracionDelJuego <= 0){
                Termino();
            };
            
        }
        if(terminado){
            Termino();
            puck.SetActive(false);
            //enable menu final
            if(!pausa){panel2.SetActive(true);}
            panel2.GetComponent<Image>().color = new Color(0f, 0f, 0f, (float)100/255);
            j1.GetComponent<Renderer>().sortingOrder = -1;
            j2.GetComponent<Renderer>().sortingOrder = -1;
            puck.GetComponent<Renderer>().sortingOrder = -1;
            //MenuBoton.GetComponent<Button>().interactable = true;
           // reiniciarBoton.GetComponent<Button>().interactable = true;
            if (int.Parse(marcadorL.text) >  int.Parse(marcadorV.text)){
                 Debug.Log("Gano local");
            }else if(int.Parse(marcadorL.text) ==  int.Parse(marcadorV.text)){
                Debug.Log("Empate");
            }else{
                Debug.Log("Gano visita");
            }
        }
    }

    public void botonPausa(){
        if (!pausa){
            rotacionG1 = j1.transform.rotation;
            rotacionG2 = j2.transform.rotation;
            pausa = true;
            if (bgs.tag == "neon"){
                pausaBoton.GetComponent<Image>().sprite = reanudarNeon;
            }else{
                pausaBoton.GetComponent<Image>().sprite = reaunudarB;
            }
            panel2.SetActive(true);
            pausaBoton.transform.SetParent(panel2.transform);
            panel2.GetComponent<Image>().color = new Color(1f, 1f, 1f, (float)100/255);
        }else{
            pausaBoton.transform.SetParent(GameObject.Find("Canvas").transform);
            if (bgs.tag == "neon"){
                pausaBoton.GetComponent<Image>().sprite = pausaneon;
            }else{
                pausaBoton.GetComponent<Image>().sprite = pausaN;
            }
            pausa = false;
            panel2.SetActive(false);
            Reaunudar();
        }
    }
    
    public void VolverAEmpezar(){
        if(pausa){
            pausaBoton.transform.SetParent(GameObject.Find("Canvas").transform);
            if (bgs.tag == "neon"){
                pausaBoton.GetComponent<Image>().sprite = pausaneon;
            }else{
                pausaBoton.GetComponent<Image>().sprite = pausaN;
            }
            pausa = false;
            Reaunudar();
            Termino();
            panel2.SetActive(false);
        }
        segundero.GetComponent<SegunderoController>().duracionDelJuego = PlayerPrefs.GetFloat("tiempoJuego");
        marcadorL.text = "0";
        marcadorV.text = "0";
        terminado = false;
        j1.SetActive(false);
        j2.SetActive(false);
        Comenzar();
        panel2.SetActive(false);
        pausaBoton.GetComponent<Button>().interactable = true;
    }
    public void MenuP(){
        SceneManager.LoadScene("Menu");
        PlayerPrefs.DeleteKey("modo");
        Destroy(music);
    }

    private void EnPausa(){
            puckC.pausado = true;
            jc1.pausado = true;
            jc2.pausado = true;
            jc1.arrastrando = false;
            jc2.arrastrando = false; 
            jc1.jugadorID = null;
            jc2.jugadorID = null;
            rb1.freezeRotation = true;
            rb2.freezeRotation = true;
            j1.GetComponent<Renderer>().sortingOrder = -1;
            j2.GetComponent<Renderer>().sortingOrder = -1;
            puck.GetComponent<Renderer>().sortingOrder = -1;
            
    }
    private void Reaunudar(){
            puckC.pausado = false;
            jc1.pausado = false;
            jc2.pausado = false;
            rb1.freezeRotation = false;
            rb2.freezeRotation = false;
            j1.transform.rotation = rotacionG1;
            j2.transform.rotation = rotacionG2;
            jc1.arrastrando = true;
            jc2.arrastrando = true;
            j1.GetComponent<Renderer>().sortingOrder = 0;
            j2.GetComponent<Renderer>().sortingOrder = 0;
            puck.GetComponent<Renderer>().sortingOrder = 0; 
    }
    public void Comenzar(){
            aparecer();
            ocurriendo = true;
            puckC.empezado = true;
            puckC.recienEmpezado = true;
            jc1.arrastrando = true;
            jc2.arrastrando = true;
            rb1.freezeRotation = false;
            rb2.freezeRotation = false;
            j1.transform.rotation = jc1.rotadoOriginal;
            j2.transform.rotation = jc2.rotadoOriginal;
            panel1.SetActive(false);
            j1.GetComponent<Renderer>().sortingOrder = 0;
            j2.GetComponent<Renderer>().sortingOrder = 0;
            puck.GetComponent<Renderer>().sortingOrder = 0;
    }
    private void Termino(){
            terminado=true;
            ocurriendo = false;
            pausaBoton.GetComponent<Button>().interactable = false;
            puckC.empezado = false;
            //Dejan de arrastrar
            jc1.arrastrando = false;
            jc2.arrastrando = false;
            jc1.jugadorID = null;
            jc2.jugadorID = null;
            //Se resetean a su posicion inicial y para cualquier rotacion
            j1.transform.position = jc1.posInicial;
            j2.transform.position = jc2.posInicial;
            rb1.velocity =  Vector2.zero; 
            rb2.velocity = Vector2.zero;
            rb1.freezeRotation = true;
            j1.transform.rotation = jc1.rotadoOriginal;
            j2.transform.rotation = jc2.rotadoOriginal;
            rb2.freezeRotation = true;
    }
    private void pasaEltiempo(){
        segundero.GetComponent<SegunderoController>().segundero();
    }
    private void aparecer(){
        j1.SetActive(true);
        j2.SetActive(true);
        puck.SetActive(true);
    }
    
    private void checarGol(int potenciador)
    {
        GameObject porteria = (puckC.transform.position.x < 0) ? meta1 : meta2;
        TextMeshProUGUI marcador = (puckC.transform.position.x < 0) ? marcadorL : marcadorV;

        if (Mathf.Abs(puckC.transform.position.x) >= Mathf.Abs(porteria.transform.position.x))
        {
            int score = int.Parse(marcador.text);
            score += potenciador;
            marcador.text = score.ToString();
            puckC.fueGol = true;
        }
    }
    private void inteligenciaA(float dificultad){
        float numeroR = Random.Range(1,5);
        Vector3 direccionDeEmpuje = puck.transform.position - j2.transform.position;
        if (puck.transform.position.x <= 0f){
            puckEnCancha = true;
        }else{
            offset = Random.Range(-1, 1);
            jc2.golpeo = false;
            puckEnCancha = false;
        }
        if (puckEnCancha){
            if (rb2.position.x > puck.transform.position.x){
                if(puck.transform.position.y > 0){
                    posicion0 = new Vector2(Mathf.Clamp(jc2.posInicial.x-2, -24.5f, 24.5f),umbral.transform.position.y + offset);
                }else if(puck.transform.position.y < 0){
                    posicion0 = new Vector2(Mathf.Clamp(jc2.posInicial.x-2, -24.5f, 24.5f),umbral2.transform.position.y + offset);
                }
                rb2.MovePosition(Vector2.MoveTowards(rb2.position, posicion0, numeroR * 3.5f * Time.fixedDeltaTime));
            }else{
                if(puck.transform.position.x != 0 || jc1.jugadorID != null ){
                    if(!jc2.golpeo){
                        rb2.MovePosition(Vector2.MoveTowards(rb2.position, puck.transform.position, numeroR  * 3.5f * dificultad  * Time.fixedDeltaTime));
                        if(Vector2.Distance(rb2.position ,puck.transform.position) <  6/dificultad){
                            rb2.velocity = direccionDeEmpuje * puckC.velocidadMax * dificultad * .2f;
                        }
                    }else{
                        if(puck.GetComponent<Rigidbody2D>().velocity.magnitude < puckC.velocidadMax.magnitude * dificultad * .1 || puck.GetComponent<Rigidbody2D>().velocity.x < puck.GetComponent<Rigidbody2D>().velocity.y ){
                            jc2.golpeo = false;
                        }
                        rb2.MovePosition(Vector2.MoveTowards(rb2.position, jc2.posInicial, numeroR * 3.5f * Time.fixedDeltaTime));
                    }
                }
                
            }

        }else{
                posicion0 = new Vector2(Mathf.Clamp(jc2.posInicial.x, -24.5f, 24.5f),puck.transform.position.y);
                rb2.MovePosition(Vector2.MoveTowards(rb2.position, posicion0, numeroR * 3.5f * Time.fixedDeltaTime));
        }
         
    }
   
    
}
    