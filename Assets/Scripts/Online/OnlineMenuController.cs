using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class OnlineMenuController : MonoBehaviourPunCallbacks
{
    
    public static OnlineMenuController instancia;
    private GameObject arriba, abajo, centro, boton, boton2, atras;
    public GameObject pingText, salaText;
    private bool bandera = false, continuar = false, Conectado = false;
    private float duracionDeLaAnimacion = 0.5f, tiempoPasado = 0.0f, otraAnimacion = 0.0f; // Duración de la animación en
    private Vector3 escalaFinal = new Vector3(1.0f, 1.0f, 1.0f); // Escala  Final
    private Vector3 escalaInicial =  Vector3.zero; // Escala Inicial
    private Vector3 posicionInicial = new Vector3(0f,1.5f,0);
    private Vector3 posicionFinal =  new Vector3(15.0f,1.5f,0);

    public void clickAtras()
    {
        SceneManager.LoadScene("Menu");
        PhotonNetwork.Disconnect();
    }
    public void clickCrear()
    {
        if(PlayerPrefs.HasKey("temp")){
            PlayerPrefs.DeleteKey("temp");
        }
        if(salaText.GetComponent<TMP_InputField>().text.Length < 4){
            //alerta.SetActive(true);
        }else{
            PlayerPrefs.SetString("creado", salaText.GetComponent<TMP_InputField>().text);
            //CrearRoom(salaText.GetComponent<TMP_InputField>().text);
            setUserName();
            CargarEscena("SeleccionSkinO");
        }

    }
     public void clickUnirse()
    {
        if(PlayerPrefs.HasKey("creado")){
            PlayerPrefs.DeleteKey("creado");
        }
        if(salaText.GetComponent<TMP_InputField>().text.Length < 4){
            //alerta.SetActive(true);
        }else{
            PlayerPrefs.SetString("temp", salaText.GetComponent<TMP_InputField>().text);
            //UnirseRoom(salaText.GetComponent<TMP_InputField>().text);
            setUserName();
            CargarEscena("SeleccionSkinO");
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

    private void setUserName(){
        if(PlayerPrefs.HasKey("usuario")){
            PhotonNetwork.NickName = PlayerPrefs.GetString("usuario");
        }
    }
    private void Awake() {
        if (instancia == null){
            instancia = this;
            DontDestroyOnLoad(gameObject);
        }else{
            Destroy(gameObject);
        }
        
    }
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.SendRate = 120;
        PhotonNetwork.SerializationRate = 120;
        try{
            PlayerPrefs.DeleteKey("UltimaEscena");
        }catch(Exception e){
            Debug.LogException(e);
        }
        //string nombreEscenaActual = SceneManager.GetActiveScene().name;
       // PlayerPrefs.SetString("UltimaEscena",nombreEscenaActual);
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
                PhotonNetwork.Disconnect();
                SceneManager.LoadScene("Menu");
                break;
        }
        if (pingText != null){
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
            }else if(bandera && continuar){
                if (Conectado){
                    pingText.GetComponent<TextMeshProUGUI>().text = "Ping: " + PhotonNetwork.GetPing();
                }
            }
        }

    }
      public override void OnConnectedToMaster()
    {
        Conectado = true;
        Debug.Log("Conectado exitosamente");
        boton.GetComponent<Button>().interactable = true;
        boton2.GetComponent<Button>().interactable = true;
        salaText.SetActive(true);
        
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        Conectado = false;
        Debug.LogWarning($"Se desconecto por {cause}");
        SceneManager.LoadScene("Menu");
    }
   
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log(otherPlayer.NickName + " se acaba de ir :c");
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("Alguien entró al room");
        
    }
    public override void OnLeftRoom()
    {
        CargarEscena("Menu");
        GameObject musica = GameObject.Find("musica");
        if(musica != null){
            Destroy(musica);
        }
        PhotonNetwork.Disconnect();
    }

    private void OnApplicationQuit() {
            if(PhotonNetwork.IsConnectedAndReady){
                if(PhotonNetwork.InRoom){
                    PhotonNetwork.LeaveRoom();
                }
                PhotonNetwork.Disconnect();
            }
        
    }

    private void regresar(){
        CargarEscena("Menu");
            GameObject musica = GameObject.Find("musica");
            if(musica != null){
                Destroy(musica);
            }
            PhotonNetwork.Disconnect();
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        regresar();
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        regresar();
    }
   /* public void JoinOrCreatePrivateRoom()
    {
        String nameEveryFriendKnows = salaText.GetComponent<TMP_InputField>().text;

        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 4,
            IsVisible = false
        };
        PhotonNetwork.JoinOrCreateRoom(nameEveryFriendKnows, roomOptions, null);
    }

   private void QuickMatch()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        CreateRoom();
    } */
    /// <summary>
    ///   
    /// </summary>
    public void CrearRoom(string nombre)
    {
        RoomOptions roomOptions = new RoomOptions(){
            MaxPlayers = 4
        };
        PhotonNetwork.CreateRoom(nombre,roomOptions,null);

    }
    
    public void UnirseRoom(string nombre)
    {
        PhotonNetwork.JoinRoom(nombre);
    }
    [PunRPC]
    public void CargarEscena(string nombreEscena){
        PhotonNetwork.LoadLevel(nombreEscena);
    }


}
