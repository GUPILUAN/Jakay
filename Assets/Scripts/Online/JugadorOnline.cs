using System;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class JugadorOnline : MonoBehaviourPunCallbacks
{

    public bool arrastrando;
    public Vector3 posInicial;
    private Rigidbody2D rb;
    public Quaternion rotadoOriginal;
    public GameObject bordes;
    public int? jugadorID;
    public float maxXPos { get; private set; }
    public float minXPos { get; private set; }
    [SerializeField] public bool jugadorLocal;
    [SerializeField] private GameObject nickname;
    private bool pausado;
    private Vector3 posAnclada { get; set; }

    [SerializeField] private List<Sprite> skins;
    public MonoBehaviour[] codigosIgnorados;
    public GameObject botonS;

    private void Awake()
    {
        if (!photonView.IsMine)
        {
            nickname.GetComponent<TextMeshProUGUI>().text = photonView.Owner.NickName;
            nickname.GetComponent<TextMeshProUGUI>().color = new Color(176f/255f,224f/255f,230/255f);
            foreach (var codigo in codigosIgnorados)
            {
                codigo.enabled = false;
            }
        }else{
            nickname.GetComponent<TextMeshProUGUI>().text = PhotonNetwork.NickName;
            nickname.GetComponent<TextMeshProUGUI>().color = new Color(102f/255f,205f/255f,170f/255f);
        }
        
    }
    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = skins[PlayerPrefs.GetInt("skinO")];
        bordes = GameObject.Find("newBgBorder");
        botonS = GameObject.Find("salir");
        arrastrando = false;
        rb = GetComponent<Rigidbody2D>();
        maxXPos = 10f;
        minXPos = 0f;
        if (!jugadorLocal)
        {
            maxXPos *= -1;
            minXPos *= -1;
        }
        rotadoOriginal = transform.rotation;
        posInicial = transform.position;
        if (photonView.IsMine){
            ChangeSkin(PlayerPrefs.GetInt("skinO"));
        }
    }
    private void Update()
    {
        if (arrastrando)
        {
            arrastre();
        }
        fijarPosicionX();

        if(jugadorID != null){
            botonS.GetComponent<Button>().interactable = false;
        }else{
            botonS.GetComponent<Button>().interactable = true;
        }
    }

    public void moverA(Vector2 posicion, bool activacion)
    {

        if (activacion)
        {
            Vector2 clampedTouch = new Vector2(Mathf.Clamp(posicion.x, minXPos, maxXPos), Mathf.Clamp(posicion.y, -24.5f, 24.5f));
            float velocidad = 20.0f; //ajustar esto para controlar la velocidad de interpolación.
            Vector2 nuevaPosicion = Vector2.Lerp(rb.position, clampedTouch, velocidad * Time.deltaTime);
            rb.MovePosition(nuevaPosicion);


        }
        else
        {
            Vector2 clampedTouch = new Vector2(Mathf.Clamp(posicion.x, maxXPos, minXPos), Mathf.Clamp(posicion.y, -24.5f, 24.5f));
            float velocidad = 20.0f; //ajustar esto para controlar la velocidad de interpolación.
            Vector2 nuevaPosicion = Vector2.Lerp(rb.position, clampedTouch, velocidad * Time.deltaTime);
            rb.MovePosition(nuevaPosicion);
        }

    }
    private void arrastre()
    {

        for (int i = 0; i < Input.touchCount; i++)
        {
            Vector2 touchPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position);
            // Touch touch = Input.GetTouch(i);                       
            if (jugadorID == null &&
                    GetComponent<Collider2D>().OverlapPoint(touchPos))
            {
                if (Input.GetTouch(i).phase == TouchPhase.Began)
                    jugadorID = Input.GetTouch(i).fingerId;
                rb.freezeRotation = true;
                transform.rotation = rotadoOriginal;
            }
            else if (jugadorID == Input.GetTouch(i).fingerId && !bordes.GetComponent<Collider2D>().OverlapPoint(touchPos))
            {
                if (Input.GetTouch(i).phase == TouchPhase.Moved)
                {
                    moverA(touchPos, jugadorLocal);
                    rb.freezeRotation = false;
                }

                if (Input.GetTouch(i).phase == TouchPhase.Ended ||
                    Input.GetTouch(i).phase == TouchPhase.Canceled)
                {
                    rb.freezeRotation = false;
                    jugadorID = null;
            
                }
                else
                {
                    rb.velocity = Vector2.zero;
                }
            }
            else if (jugadorID == Input.GetTouch(i).fingerId && bordes.GetComponent<Collider2D>().OverlapPoint(touchPos))
            {
                jugadorID = null;
            }

        }
    }


    private void fijarPosicionX()
    {
        if (jugadorLocal)
        {
            if (transform.position.x > maxXPos)
            {
                transform.position = new Vector3(maxXPos, transform.position.y, transform.position.z);
                rb.velocity = Vector2.zero;
            }
            if (transform.position.x < minXPos)
            {
                transform.position = new Vector3(minXPos, transform.position.y, transform.position.z);
                rb.velocity = Vector2.zero;
            }
        }
        else
        {
            if (transform.position.x < maxXPos)
            {
                transform.position = new Vector3(maxXPos, transform.position.y, transform.position.z);
                rb.velocity = Vector2.zero;
            }
            if (transform.position.x > minXPos)
            {
                transform.position = new Vector3(minXPos, transform.position.y, transform.position.z);
                rb.velocity = Vector2.zero;
            }
        }
    }


    public void ReiniciarPos(bool gol)
    {
        if (gol)
        {
            transform.rotation = rotadoOriginal;
            rb.freezeRotation = true;
            transform.position = posInicial;
            rb.velocity = Vector2.zero;
            jugadorID = null;
        }
    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "puck")
        {
            Debug.Log("se golpeo puck");
        }
        try
        {
            if (other.gameObject.GetComponent<JugadorOnline>().jugadorLocal == gameObject.GetComponent<JugadorOnline>().jugadorLocal)
            {
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), other.collider, true);
            }
        }
        catch (Exception)
        {
            // print(e);
        }
    }


    public void ChangeSkin(int skinE)
    {
        photonView.RPC("RPC_ChangeSkin", RpcTarget.OthersBuffered, GetComponent<PhotonView>().ViewID.ToString(),skinE);
    }
    [PunRPC]
    private void RPC_ChangeSkin(string jugadorViewID, int elec)
    {
        PhotonView jugadorPhotonView = PhotonView.Find(int.Parse(jugadorViewID));
        if (jugadorViewID != null)
        {
            GameObject jugadorX = jugadorPhotonView.gameObject;
            jugadorX.GetComponent<SpriteRenderer>().sprite = skins[elec];
        }else{
            Debug.Log("NO");
        }

    }

}


