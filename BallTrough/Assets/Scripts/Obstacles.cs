using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacles : MonoBehaviour
{
    public GameObject ball;
    public GameObject obstacle;
    private SpriteRenderer obstacleSprite;
    private float obstacleSpriteWidth;
    private float obstacleSpriteHeight;

    void Start()
    {
        GameObject obstacle = new GameObject();
        obstacleSprite = obstacle.AddComponent<SpriteRenderer>() as SpriteRenderer;
        obstacleSpriteWidth = obstacleSprite.size.x;
        obstacleSpriteHeight = obstacleSprite.size.y;
    }

    void Update()
    {
        DetectCollision();
    }

    private void DetectCollision() 
    {
        //Colision x; y 
        if (ball.transform.position.x >= obstacle.transform.position.x - obstacleSpriteWidth/2 && ball.transform.position.x <= obstacle.transform.position.x + obstacleSpriteWidth/2)
        {
            if (ball.transform.position.y >= obstacle.transform.position.y - obstacleSpriteHeight / 2 && ball.transform.position.y <= obstacle.transform.position.y + obstacleSpriteHeight / 2)
            {
                //print("tocas a " + obstacle);
                Destroy(gameObject);
                ShootsAmount.CansScoreCounter += 1;
            }
        }
    }
}
