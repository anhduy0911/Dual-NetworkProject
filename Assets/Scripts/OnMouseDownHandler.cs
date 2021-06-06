using UnityEngine;

public class OnMouseDownHandler : MonoBehaviour
{
    private AudioSource audioShoot;
    // Start is called before the first frame update
    void Start()
    {
        audioShoot = GetComponent<AudioSource>();
    }

    public void OnMouseDown()
    {
        
    }
}
