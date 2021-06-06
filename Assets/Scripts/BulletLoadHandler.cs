using UnityEngine;
using GameMessage;

public class BulletLoadHandler : MonoBehaviour
{
    public AudioSource audioShoot;
    private Animation bulletLoadAnim;
    private bool isReloading = false;
    // Start is called before the first frame update
    void Start()
    {
        bulletLoadAnim = GetComponent<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        if (isReloading)
            return;

        Debug.Log("Baam! Clicked");
        Transform child = transform.GetChild(5 - Client.clientInstance.bulletLeft);
        Client.clientInstance.bulletLeft--;
        child.gameObject.SetActive(false);

        if (Client.clientInstance.bulletLeft == 0)
        {
            foreach (Transform ch in transform)
                ch.gameObject.SetActive(true);
            Invoke("ReloadBullet", 0.32f);
            isReloading = true;
            bulletLoadAnim.Play();
        }

        audioShoot.Play();
        Debug.Log("Click ...");
        IsShot isShoot = new IsShot();
        Client.sendShoot(isShoot, 0);
        //send the shoot signal of player
    }

    void ReloadBullet()
    {
        Client.clientInstance.bulletLeft = 5;
        isReloading = false;
    }
}
