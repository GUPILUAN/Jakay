using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController Instance;

    private void Awake() { 
        if(Instance == null){
            Instance = this;
            gameObject.GetComponent<AudioSource>().volume = PlayerPrefs.HasKey("volumen") ? PlayerPrefs.GetFloat("volumen") : 1;
            DontDestroyOnLoad(gameObject);
        }else{
            Destroy(gameObject);
        }
    

    }
    
}
