using UnityEngine;

public class DiscoController : MonoBehaviour
{
    [SerializeField] private AudioClip[] sonidosFuerteS = new AudioClip[3];
    [SerializeField] private AudioClip[] sonidosNormales = new AudioClip[4];
    [SerializeField] private AudioClip sonidoBajo;
    private AudioSource choque;
    public Vector2 velocidadMax = new Vector2(25f,25f);

    public float duracionDeLaAnimacion = 0.8f; // Duración de la animación en segundos
    private Vector3 escalaFinal = new Vector3(30.0f, 30.0f, 30.0f); // Escala final deseada
    private Vector3 escalaInicial, posInicial = Vector3.zero, posAnclada;
    public float  desaseleracion = 0.9f;
    public float tiempoPasado = 0.0f;
    public bool fueGol, pausado, empezado, recienEmpezado;
    private Rigidbody2D rb;
    public int puntajeLocal = 0, puntajeVisita = 0;

    private void Awake() {
        choque = GetComponent<AudioSource>();
    }
    void Start()
    { 
        fueGol = false;
         rb = GetComponent<Rigidbody2D>();
         transform.position = posInicial;
         escalaInicial = escalaFinal * 4;
    }
    // Llamado en cada fotograma
    void Update()
    { 
        ControlVel();
        if(recienEmpezado || fueGol && !pausado){
            if(tiempoPasado < duracionDeLaAnimacion && empezado){
                Animacion();
            }else{
                // La animación ha terminado
                transform.localScale = escalaFinal; // Asegura que la escala sea exactamente la final
                tiempoPasado = 0.0f;
                recienEmpezado = false;
                fueGol = false;
            }
        }else if (!empezado){
            rb.velocity = Vector2.zero;
            transform.position = posInicial;
            transform.localScale = escalaFinal;   
            }
        if(pausado){
            transform.position = posAnclada;
        }else{
            posAnclada = transform.position;
        }


    }

   
    private void ControlVel(){
         if (velocidadMax.x < rb.velocity.x || velocidadMax.x * -1 > rb.velocity.x){
            rb.velocity = new Vector2(rb.velocity.x > 0 ? velocidadMax.x : velocidadMax.x *-1, rb.velocity.y); 
        } else if (velocidadMax.y < rb.velocity.y || velocidadMax.y * -1 > rb.velocity.y ){
            rb.velocity =new Vector2(rb.velocity.x , rb.velocity.y > 0 ? velocidadMax.y : velocidadMax.y * -1);
        } else if ((velocidadMax.x < rb.velocity.x && velocidadMax.y < rb.velocity.y) || (velocidadMax.x * -1 > rb.velocity.x && velocidadMax.y*-1 > rb.velocity.y)) {
            rb.velocity = rb.velocity.x > 0 && rb.velocity.y > 0 ? velocidadMax : velocidadMax *-1;
        }
    }
    public void Animacion(){
        rb.velocity = Vector2.zero;
        transform.position = posInicial;
        transform.localScale = escalaInicial;
        tiempoPasado += Time.deltaTime*2;
        // Interpola entre la escala inicial y la escala final en función del tiempo
        float t = tiempoPasado / duracionDeLaAnimacion;
        transform.localScale = Vector3.Lerp(escalaInicial, escalaFinal, t);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.transform.tag == "borde"){
            if(rb.velocity.magnitude == velocidadMax.magnitude){
                choque.PlayOneShot(sonidosFuerteS[Mathf.RoundToInt(Random.Range(0,2))]);
            }else if(rb.velocity.magnitude < velocidadMax.magnitude && rb.velocity.magnitude > 2){
                choque.PlayOneShot(sonidosNormales[Mathf.RoundToInt(Random.Range(0,3))]);
            }else{
                choque.PlayOneShot(sonidoBajo);
            }
 
        }
    }
  private void OnCollisionStay2D(Collision2D other) {
    if (other.transform.tag == "borde"){
        Debug.Log("ESTA CHOCANDO");
        rb.AddForce(other.contacts[0].normal, ForceMode2D.Impulse);
       /* if (rb.position.y >0){
            rb.AddForce(new Vector2(0f,-100f));
        }else{
            rb.AddForce(new Vector2(0f,100f));
        }*/
        
    }
  }
}