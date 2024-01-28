using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JuegoOnlineController : MonoBehaviourPunCallbacks
{
    private GameObject music;
    private AudioSource aS;
    [SerializeField] private AudioClip MusicaMaestro;
    [SerializeField]private GameObject inicio, jugador, backg, puck, nombre, borde, meta1, segundero, marcadorLocal, marcadorVisita, botonL, botonV, meta2, botonInicio;
    private TextMeshProUGUI marcadorL, marcadorV;
    private bool esNeon, ocurriendo,terminado, alguienEntro;
    [SerializeField]private List<Sprite> backgrounds;
    int jugadoresLocal=0, jugadoresVisita=0;
    [SerializeField]private Sprite bordesNeon, puckneon;
    private float tiempoI;
    private int skinSelected;

    public void InicializarElementos(bool local){
        botonL.SetActive(false);
        botonV.SetActive(false);
        if (PhotonNetwork.IsMasterClient){
            puck = PhotonNetwork.Instantiate("Puck",Vector3.zero, Quaternion.identity);
            PukaQuiereAGaru(puck);   
        }
        float decision = 0;
        if(local && jugadoresLocal == 1){
            decision = 3f;        
        }else if(local && jugadoresLocal<1){
                decision = -3f;
        }
       if(!local && jugadoresVisita == 1){
            decision = 3f;
        }else if(!local && jugadoresVisita<1){
            decision = -3f;
        }
        if(jugador == null){
            jugador = PhotonNetwork.Instantiate("Jugador", new Vector3(local ? 6f:-6f, decision,0), local ? new Quaternion(0,0,0.707106829f,0.707106829f):new Quaternion(0,0,-0.707106829f,0.707106829f));
            if (local){
                jugadoresLocal += 1;
                photonView.RPC("conteoJugadores", RpcTarget.AllBuffered,jugadoresLocal,local);
                jugador.GetComponent<JugadorOnline>().jugadorLocal = local;
            }else{
                jugadoresVisita += 1;
                photonView.RPC("conteoJugadores", RpcTarget.AllBuffered,jugadoresVisita,local);
                jugador.GetComponent<JugadorOnline>().jugadorLocal = local;
            }
            miPlayer(jugador);
        }
        SolicitarMarcador();

    }
    
    
    private void InicializarUI(){
        marcadorL = marcadorLocal.GetComponent<TextMeshProUGUI>();
        marcadorV = marcadorVisita.GetComponent<TextMeshProUGUI>();
        if (esNeon){
            borde.GetComponent<SpriteRenderer>().sprite = bordesNeon;
            segundero.GetComponent<TextMeshProUGUI>().color = Color.white;
            marcadorL.color = Color.white;
            marcadorV.color = Color.white;
            marcadorL.alpha = 0.5f;
            marcadorV.alpha = 0.5f;
            borde.GetComponent<SpriteRenderer>().color = Color.magenta;
        }else{
            borde.GetComponent<SpriteRenderer>().color = Color.black;
        }

    }
    
   private void Awake() {

   music = GameObject.Find("musica");
    aS = music.GetComponent<AudioSource>();
         if(music != null){
            if(aS.isPlaying){
                aS.volume = aS.volume * 0.5f;
                aS.Stop();
            }
        }
     botonInicio = GameObject.Find("inicio");
     botonInicio.SetActive(false);
    
   }
    private void Start() {
        Application.targetFrameRate = 60;

    }
    private void Update() {
        if(PhotonNetwork.InRoom){
            if(music != null){
                if(!aS.isPlaying){
                    aS.PlayOneShot(MusicaMaestro);
                }
            }
        /*    if (puck != null){
                if (jugador != null && puck.GetComponent<PhotonView>().OwnerActorNr == jugador.GetComponent<PhotonView>().OwnerActorNr)
                {
                    Debug.Log("mandando señal" + gameObject.GetComponent<PhotonView>().ViewID + "el perro " + gameObject.GetComponent<PhotonView>().OwnerActorNr);
                    updatePuck(puck);
                }
            }*/
            if(jugadoresLocal >=2){
                botonL.SetActive(false);
            }
            if(jugadoresVisita >= 2){
                botonV.SetActive(false);
            }
            if(PhotonNetwork.IsMasterClient){
                if (alguienEntro){
                    photonView.RPC("setMarcador", RpcTarget.AllBuffered,int.Parse(marcadorL.text),int.Parse(marcadorV.text),ocurriendo);
                    alguienEntro = false;
                }
                tiempoActualizado();
                if (jugadoresLocal>=1 && jugadoresVisita>=1 && !ocurriendo && !terminado){
                    botonInicio.SetActive(true);
                }else if(ocurriendo){
                    if(jugador!=null && puck == null){
                        InicializarElementos(jugador.GetComponent<JugadorOnline>().jugadorLocal);
                        puck.GetComponent<PuckOnline>().empezado = true;
                    }
                    botonInicio.SetActive(false);
                    segundero.GetComponent<SegunderoController>().segundero();
                    jugador.GetComponent<JugadorOnline>().arrastrando  = true;
                    checarGol(1);
                }else{
                    segundero.GetComponent<SegunderoController>().mostrarTiempo();
                    if(segundero.GetComponent<SegunderoController>().duracionDelJuego<tiempoI && segundero.GetComponent<SegunderoController>().duracionDelJuego > 0){
                        ocurriendo = true;
                    }
                }
            }else{
                Debug.Log(tiempoI);
                if(segundero.GetComponent<SegunderoController>().duracionDelJuego<tiempoI && segundero.GetComponent<SegunderoController>().duracionDelJuego>0){
                    ocurriendo = true;
                }
                segundero.GetComponent<SegunderoController>().mostrarTiempo();
                if (ocurriendo){
                    if(puck != null){
                        puck.GetComponent<PuckOnline>().empezado = true;
                    }
                    if(jugador!=null){
                        checarGol(1);
                        jugador.GetComponent<JugadorOnline>().arrastrando  = true;
                        SolicitarMarcador();
                    }
                }
            }

            if(segundero.GetComponent<SegunderoController>().duracionDelJuego <= 0){
                Termino();
            }

        }
       if(!terminado){Debug.Log(jugadoresLocal + "L vs V" + jugadoresVisita);}     
    }
    
    [PunRPC]
    private void RPC_ChangeEnvironmentSkin(int newSkin,bool neon)
    {
        // Este método se ejecutará en todos los jugadores, y aquí puedes cambiar la skin del entorno.
        backg.GetComponent<SpriteRenderer>().sprite = backgrounds[newSkin];
        esNeon = neon;
        skinSelected = newSkin;
        InicializarUI();

    }
    public void ponerFondo(int newSkin)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (newSkin == 2 || newSkin == 3){
                esNeon = true;
            }else{
                esNeon = false;
            }
            // Envía la información de la nueva skin a todos los jugadores en la sala.
            photonView.RPC("RPC_ChangeEnvironmentSkin", RpcTarget.AllBuffered, newSkin, esNeon);
            
        }
        
    }
    public override void OnJoinedRoom()
    {
        botonL.GetComponent<Button>().interactable = true;
        botonV.GetComponent<Button>().interactable = true;
        if(PhotonNetwork.IsMasterClient){
            ponerFondo(PlayerPrefs.GetInt("ultimoO"));
            segundero.GetComponent<SegunderoController>().duracionDelJuego = PlayerPrefs.GetFloat("tiempoJuego");
            tiempoInicial();
        }
        SolicitarMarcador();

    }
    
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if(PhotonNetwork.IsMasterClient){
            foreach (PhotonView photonViewX in FindObjectsOfType<PhotonView>())
            {
            // Verificar si el PhotonView tiene un propietario y si el ID coincide
                if (photonViewX.Owner != null && photonViewX.Owner.ActorNumber == otherPlayer.ActorNumber)
                {
                    Debug.Log("Se encontró un objeto perteneciente al jugador con ID: " + otherPlayer.ActorNumber);
                    if (photonViewX.gameObject.TryGetComponent<JugadorOnline>(out _)){
                        if (photonViewX.gameObject.GetComponent<JugadorOnline>().jugadorLocal && jugadoresLocal>0){
                            jugadoresLocal -= 1;
                            photonView.RPC("conteoJugadores", RpcTarget.AllBuffered,jugadoresLocal,photonViewX.gameObject.GetComponent<JugadorOnline>().jugadorLocal);
                        }else if(!photonViewX.gameObject.GetComponent<JugadorOnline>().jugadorLocal && jugadoresVisita>0){
                            jugadoresVisita -= 1;
                            photonView.RPC("conteoJugadores", RpcTarget.AllBuffered,jugadoresVisita,photonViewX.gameObject.GetComponent<JugadorOnline>().jugadorLocal);
                        }
                    }
                }
            }

        }
        Debug.Log(otherPlayer.NickName + " se acaba de ir :c");
    }

    [PunRPC]
    private void ActualizarTiempo(float segunderoMaster)
    {
       segundero.GetComponent<SegunderoController>().duracionDelJuego = segunderoMaster;
    }
    [PunRPC]
    public void conteoJugadores(int jugadoresAhora, bool local)
    {
       if(local){
            jugadoresLocal = jugadoresAhora;
        }else{
            jugadoresVisita = jugadoresAhora;
        }
    }

    private void SolicitarMarcador(){
        if(!PhotonNetwork.IsMasterClient){
            photonView.RPC("verificar", RpcTarget.MasterClient);
        }
    }
    [PunRPC]
    private void verificar(){
        alguienEntro = true;
    }

    [PunRPC]
    private void setMarcador(int local, int visita, bool ocurriendo)
    {
       if (ocurriendo){
            marcadorL.text = local.ToString();
            marcadorV.text = visita.ToString();
       }
    }
    [PunRPC]
    private void setTiempo(float tiempo)
    {
       tiempoI = tiempo;
    }
    [PunRPC]
    private void PonerPuck(string puckViewID)
    {
        PhotonView puckPhotonView = PhotonView.Find(int.Parse(puckViewID));
        if (puckPhotonView != null)
        {
            puck = puckPhotonView.gameObject;
            GameObject canvas = GameObject.Find("Canvas");
            puck.transform.SetParent(canvas.transform);
            if(!puckPhotonView.GetComponent<PuckOnline>().recienEmpezado){
                puck.transform.localScale = new Vector3(30, 30, 30);
            }
            Physics2D.IgnoreCollision(puck.GetComponent<Collider2D>(),meta1.GetComponent<Collider2D>(),true);
            Physics2D.IgnoreCollision(puck.GetComponent<Collider2D>(),meta2.GetComponent<Collider2D>(),true);
            if (esNeon){
                puck.GetComponent<SpriteRenderer>().sprite = puckneon;
            }
            if (esNeon){
                puck.GetComponent<SpriteRenderer>().sprite = puckneon;
            }
        }else{
            Debug.Log("No puck");
        }
    }

    
    private void PukaQuiereAGaru(GameObject p)
    {
        
        photonView.RPC("PonerPuck", RpcTarget.AllBuffered, p.GetComponent<PhotonView>().ViewID.ToString());
        
    }
    private void tiempoInicial(){
        photonView.RPC("setTiempo", RpcTarget.AllBuffered, segundero.GetComponent<SegunderoController>().duracionDelJuego);
    }
    private void tiempoActualizado(){
        photonView.RPC("ActualizarTiempo", RpcTarget.OthersBuffered, segundero.GetComponent<SegunderoController>().duracionDelJuego);
    }


    [PunRPC]
    private void MeterAlCampo(string jugadorViewID)
    {
      PhotonView jugadorPhotonView = PhotonView.Find(int.Parse(jugadorViewID));
        if (jugadorViewID != null)
        {
            GameObject jugadorX = jugadorPhotonView.gameObject;
            GameObject canvas = GameObject.Find("Canvas");
            jugadorX.transform.SetParent(canvas.transform);
            jugadorX.transform.localScale = new Vector3(30, 30, 30);
        }else{
            Debug.Log("NO");
        }
    }
    private void miPlayer(GameObject player)
    {
        photonView.RPC("MeterAlCampo", RpcTarget.AllBuffered, player.GetComponent<PhotonView>().ViewID.ToString());
    }
    private void checarGol(int potenciador)
    {
        if(puck!=null){
            GameObject porteria = (puck.transform.position.x < 0) ? meta1 : meta2;
            TextMeshProUGUI marcador = (puck.transform.position.x < 0) ? marcadorL : marcadorV;

                if (Mathf.Abs(puck.transform.position.x) >= Mathf.Abs(porteria.transform.position.x)){
                    int score = int.Parse(marcador.text);
                    score += potenciador;
                    marcador.text = score.ToString();
                    puck.GetComponent<PuckOnline>().fueGol = true;
                }
                jugador.GetComponent<JugadorOnline>().ReiniciarPos(puck.GetComponent<PuckOnline>().fueGol);
            }
    }
    [PunRPC]
    private void ActualizarPuck(string puckViewID)
    {
        PhotonView puckPhotonView = PhotonView.Find(int.Parse(puckViewID));
        if (puckPhotonView != null)
        {

            puck.transform.position = puckPhotonView.gameObject.transform.position;
            puck.transform.localScale = puckPhotonView.gameObject.transform.localScale;
            puck.GetComponent<PuckOnline>().recienEmpezado = puckPhotonView.GetComponent<PuckOnline>().recienEmpezado;
            puck.GetComponent<PuckOnline>().empezado = puckPhotonView.GetComponent<PuckOnline>().empezado;
            puck.GetComponent<PuckOnline>().animacionTermino = puckPhotonView.GetComponent<PuckOnline>().animacionTermino;
            jugador.GetComponent<JugadorOnline>().ReiniciarPos(puckPhotonView.GetComponent<PuckOnline>().fueGol);
            Physics2D.IgnoreCollision(puck.GetComponent<Collider2D>(),meta1.GetComponent<Collider2D>(),true);
            Physics2D.IgnoreCollision(puck.GetComponent<Collider2D>(),meta2.GetComponent<Collider2D>(),true);
        }else{
            Debug.Log("No puck");
        }
    }
    private void updatePuck(GameObject p)
    {
        photonView.RPC("ActualizarPuck", RpcTarget.OthersBuffered, p.GetComponent<PhotonView>().ViewID.ToString());
        
    }
    public void Salir(){
        if(PhotonNetwork.InRoom){
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
        }
        Destroy(music);
        GameObject.Find("OnlineController").GetComponent<OnlineMenuController>().CargarEscena("Menu");
    }
    public void Listo(){
        puck.GetComponent<PuckOnline>().recienEmpezado = true;
        puck.GetComponent<PuckOnline>().empezado = true;
        ocurriendo = true;
    }
    private void Termino(){
            ocurriendo = false;
            terminado = true;
            puck.GetComponent<PuckOnline>().empezado = false;
            if(jugador!=null){
            //Dejan de arrastrar
            jugador.GetComponent<JugadorOnline>().arrastrando  = false;
            jugador.GetComponent<JugadorOnline>().jugadorID  = null;
            //Se resetean a su posicion inicial y para cualquier rotacion
            jugador.transform.position = jugador.GetComponent<JugadorOnline>().posInicial;
            jugador.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            jugador.GetComponent<Rigidbody2D>().freezeRotation = true;
            jugador.transform.rotation = jugador.GetComponent<JugadorOnline>().rotadoOriginal;
            }
    }
    public override void OnMasterClientSwitched(Player newMasterClient){
    // Manejar la desconexión del host aquí
    Debug.Log("El host se ha desconectado. Nuevo host: " + newMasterClient.NickName);
        if(PhotonNetwork.IsMasterClient){
            ponerFondo(skinSelected);
        }
    }
}
