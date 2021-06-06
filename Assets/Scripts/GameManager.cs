using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameMessage;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public Transform bulletPrefab;

    private float offsetY = 20.0f;

    IDictionary<int, Transform> bulletsList = new Dictionary<int, Transform>();
    List<int> appearBulletID = new List<int>();
    Transform player;


    // Start is called before the first frame update
    void Start()
    {
        player = transform.FindChildWithTag("Player");
        HealthBarHandler.SetHealthBarValue(1.0f);

        Time.fixedDeltaTime = 0.025f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //receive message from server
        while (true)
        {
            int flag = Client.clientInstance.parseReceivedMessage();

            //Debug.Log("client Instance: " + Client.clientInstance.ToString() + ", flag: " + flag.ToString());

            if (flag >= 0)
            {
                //check if the message is status or game state
                switch (flag)
                {
                    case 0:
                        Debug.Log("Receive gameState");
                        updatePosition();
                        break;
                    case 1:
                        Debug.Log("Receive status");
                        checkStatus();
                        break;
                }
            }
            else break;
        }

        //send the direction of player
        OnMouseDrag();
    }

    void updatePosition()
    {
        GameStateMessage state = Client.clientInstance.parseGameState();

        Vector3 newPos;
        if (Client.player_id == 1)
            newPos = new Vector3(state.PlayerStat.X, state.PlayerStat.Y, player.position.z);
        else
            newPos = new Vector3(state.PlayerStat.X, state.PlayerStat.Y + offsetY, player.position.z);
        //Debug.Log("Player new position: " + newPos.x.ToString() + " " + newPos.y.ToString());
        player.position = newPos;
        HealthBarHandler.SetHealthBarValue((float) state.PlayerStat.Health / 100);

        appearBulletID.Clear();
        foreach(Bullet bullet in state.Bullets)
        {
            appearBulletID.Add(bullet.Id);
            if (bulletsList.Keys.Contains(bullet.Id))
            {
                Transform b = bulletsList[bullet.Id];
                Vector3 bPos;
                if (Client.player_id == 0)
                    bPos = new Vector3(bullet.X, bullet.Y + 20, player.position.z);
                else
                    bPos = new Vector3(bullet.X, bullet.Y, player.position.z);
                b.position = bPos;
            }
            else
            {
                Vector3 bPos;
                if (Client.player_id == 0)
                    bPos = new Vector3(bullet.X, bullet.Y + 20, player.position.z);
                else
                    bPos = new Vector3(bullet.X, bullet.Y, player.position.z);
                Transform newBullet = Instantiate(bulletPrefab, bPos, player.rotation, transform);
                bulletsList[bullet.Id] = newBullet;
                if (bullet.Playerid != Client.player_id)
                {
                    newBullet.gameObject.GetComponent<Renderer>().material.color = Color.red;
                    newBullet.rotation = new Quaternion(0, 0, newBullet.rotation.z + 180, newBullet.rotation.w);
                }
            }
        }

        foreach(int id in bulletsList.Keys.ToList())
        {
            if (!appearBulletID.Contains(id))
            {
                Destroy(bulletsList[id].gameObject, 0.0f);
                bulletsList.Remove(id);
            }
        }
    }

    void checkStatus()
    {
        Status status = Client.clientInstance.parseStatus();
        Debug.Log("Status: " + status.ToString());
        switch (status.Status_)
        {
            case 2:
                {
                    Client.winState = 1;
                    //Client.sendAcknowledgeWrapper();
                    IsShot isShoot = new IsShot();
                    Client.sendShoot(isShoot, 1);
                    Debug.Log("Game win!");
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                    break;
                }
            case 3:
                {
                    Client.winState = 0;
                    //Client.sendAcknowledgeWrapper();
                    IsShot isShoot = new IsShot();
                    Client.sendShoot(isShoot, 1);
                    Debug.Log("Game loose!");
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                    break;
                }
            case 4:
                {
                    Client.winState = 2;
                    //Client.sendAcknowledgeWrapper();
                    IsShot isShoot = new IsShot();
                    Client.sendShoot(isShoot, 1);
                    Debug.Log("Game win!");
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                    break;
                }
        }
    }

    void OnMouseDrag()
    {
        //Vector3 screenPoint = Camera.main.WorldToScreenPoint(player.position);
        Vector3 screenPoint = player.position;
        Vector3 curScreenPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3 diff = curScreenPoint - screenPoint;

        float angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        angle -= 90;
        player.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        //Debug.Log("Angle" + player.rotation.ToString());

        Vector2 playerPos = new Vector2(screenPoint.x, screenPoint.y);
        Vector2 mousePos = new Vector2(curScreenPoint.x, curScreenPoint.y);


        Vector2 movingVect = (mousePos - playerPos);
        //Vector2 movingVect = (mousePos - playerPos).normalized;

        //Debug.Log("Player" + screenPoint.ToString());
        //Debug.Log("Mouse" + curScreenPoint.ToString());
        //Debug.Log("moving Vect" + movingVect.ToString());
        MovingDirection mv = new MovingDirection();

        if (Vector2.Distance(playerPos, mousePos) < 0.001f)
        {
            mv.Vx = 0;
            mv.Vy = 0;
        }
        else
        {
            mv.Vx = movingVect.x;
            mv.Vy = movingVect.y;
        }
        
        //Debug.Log("Vx: " + mv.Vx.ToString() + ", Vy: " + mv.Vy.ToString());
        Client.sendDirection(mv);
        //player.position = curPosition;
    }

}
