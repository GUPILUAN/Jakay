using System;
using UnityEngine;

public class JugadorController : MonoBehaviour
{
    public bool arrastrando = false;
    public Vector3  posInicial;
    private Rigidbody2D rb;
    public Quaternion rotadoOriginal;
    public GameObject discoController, bordes;
    public bool empujando;
    public int? jugadorID;
    public float maxXPos {get; private set;}
    public float minXPos{get; private set;}
    [SerializeField] private bool jugadorLocal;
    public JugadorController jugador;
    public bool jugadorReal, pausado, golpeo;
    private Vector3 posAnclada { get; set;}


    private void Start()
    {
        posInicial = transform.position;
        rb = GetComponent<Rigidbody2D>();
        maxXPos = 10f;
        minXPos = 0f;
        if (jugador.jugadorLocal){
            jugador.transform.rotation = new Quaternion(0,0,0.707106829f,0.707106829f);
            rotadoOriginal = jugador.transform.rotation;
        }else{
            jugador.transform.rotation = new Quaternion(0,0,-0.707106829f,0.707106829f);
            rotadoOriginal = jugador.transform.rotation;
        }
        if (!jugadorLocal){
            maxXPos *= -1;
            minXPos *= -1;
        }
    }

    private void Update()
    {
        if (arrastrando && jugadorReal){
            arrastre();
        }
        fijarPosicionX();
        if(pausado){
            transform.position = posAnclada;
        }else{
            posAnclada = transform.position;
        }

    }

    public void moverA(Vector2 posicion, bool activacion){
        
        if (activacion){
            Vector2 clampedTouch = new Vector2(Mathf.Clamp(posicion.x,minXPos,maxXPos),Mathf.Clamp(posicion.y, -24.5f, 24.5f));
            float velocidad = 20.0f; //ajustar esto para controlar la velocidad de interpolación.
            Vector2 nuevaPosicion = Vector2.Lerp(rb.position, clampedTouch, velocidad * Time.deltaTime);
            rb.MovePosition(nuevaPosicion);
           

        } else {
            Vector2 clampedTouch = new Vector2(Mathf.Clamp(posicion.x, maxXPos,minXPos),Mathf.Clamp(posicion.y, -24.5f, 24.5f));
            float velocidad = 20.0f; //ajustar esto para controlar la velocidad de interpolación.
            Vector2 nuevaPosicion = Vector2.Lerp(rb.position, clampedTouch, velocidad * Time.deltaTime);
            rb.MovePosition(nuevaPosicion);
        }
        
    }
    private void arrastre()
    {
        for (int i = 0; i < Input.touchCount; i++){
            Vector2 touchPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position); 
           // Touch touch = Input.GetTouch(i);                       
                if (jugador.jugadorID == null &&
                        jugador.GetComponent<Collider2D>().OverlapPoint(touchPos))
                {
                    if (Input.GetTouch(i).phase == TouchPhase.Began)
                        jugador.jugadorID = Input.GetTouch(i).fingerId;
                        rb.freezeRotation = true;
                        transform.rotation = rotadoOriginal;
                      //  posTouchI = Input.GetTouch(i).position;
                        
                }
                else if (jugador.jugadorID == Input.GetTouch(i).fingerId && !bordes.GetComponent<Collider2D>().OverlapPoint(touchPos))
                {
                    if(Input.GetTouch(i).phase == TouchPhase.Moved){
                        jugador.moverA(touchPos,jugador.jugadorLocal);
                        rb.freezeRotation = false;
                        //tiempoX += Time.deltaTime;
                    }

                    if (Input.GetTouch(i).phase == TouchPhase.Ended ||
                        Input.GetTouch(i).phase == TouchPhase.Canceled){
                        rb.freezeRotation = false;
                        jugador.jugadorID = null;
                       // Vector2 swipeVelocity = (Input.GetTouch(i).deltaPosition - posTouchI)/ Time.deltaTime;
                       // Vector2 swipeVelocity = touch.deltaPosition / touch.deltaTime;
                       // rb.velocity = swipeVelocity * .1f;
                    }else{
                        rb.velocity = Vector2.zero;
                    }
                }else if (jugador.jugadorID == Input.GetTouch(i).fingerId && bordes.GetComponent<Collider2D>().OverlapPoint(touchPos)){
                    jugador.jugadorID = null;
                }
                
        }
    }


    private void fijarPosicionX()
    {
        if (jugadorLocal){
            if (transform.position.x > maxXPos){
            transform.position = new Vector3(maxXPos, transform.position.y, transform.position.z);
            rb.velocity = Vector2.zero;
            }
            if (transform.position.x < minXPos){
            transform.position = new Vector3(minXPos, transform.position.y, transform.position.z);
            rb.velocity = Vector2.zero;
            }
        }else{
            if (transform.position.x < maxXPos){
            transform.position = new Vector3(maxXPos, transform.position.y, transform.position.z);
            rb.velocity = Vector2.zero;
            } 
            if (transform.position.x > minXPos){
            transform.position = new Vector3(minXPos, transform.position.y, transform.position.z);
            rb.velocity = Vector2.zero;
            }
        }
    }
    public void ReiniciarPos(bool gol){
        if (gol){
            jugador.transform.rotation = rotadoOriginal;
            jugador.rb.freezeRotation = true;
            jugador.transform.position = posInicial;
            jugador.rb.velocity = Vector2.zero;
            jugador.jugadorID = null;
        }else{
           
            if (!jugador.jugadorReal){
            try
            {
                jugador.rb.freezeRotation = false;
            }
            catch (System.Exception){}
            }
        }

    }
    private void OnCollisionEnter2D(Collision2D other) {
       // Debug.Log(other.gameObject.GetComponent<Collider2D>().bounds);
       if (other.gameObject.tag == "puck" && !jugadorReal){
            Debug.Log("AI golpeo puck");
       }
        try
        {
            if (other.gameObject.GetComponent<JugadorController>().jugadorLocal == gameObject.GetComponent<JugadorController>().jugadorLocal){
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), other.collider, true);
            }
        }
        catch (Exception)
        {
           // print(e);
        } 
    }
    private void OnCollisionStay2D(Collision2D other) {
        if (other.gameObject.tag == "puck" && !jugadorReal){
            Debug.Log("AAAAAAA");
            golpeo = true;
       }
    }
    
}
