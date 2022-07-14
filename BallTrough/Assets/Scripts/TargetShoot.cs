using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetShoot : MonoBehaviour
{
    private SpriteRenderer rectSpriteRenderer;
    private Transform rectTransform;
    private float posX;
    private float posY; 
    private float width;
    private float height;

    public Player ball;

    private void Start()
    {
        rectSpriteRenderer = GetComponent<SpriteRenderer>();
        rectTransform = GetComponent<Transform>();
        posX = rectTransform.position.x;
        posY = rectTransform.position.y;
        width = rectSpriteRenderer.size.x;
        height = rectSpriteRenderer.size.y;
    }

    private void Update()
    {
        DetectCollisionWithBall();
    }

    private void DetectCollisionWithBall()
    {
        if(ball.transform.position.x + ball.radius > transform.position.x && ball.transform.position.x - ball.radius < transform.position.x)
        {
            if(ball.transform.position.y + ball.radius > transform.position.y - 1 && ball.transform.position.y - ball.radius < transform.position.y + 1)
            {
                ShootsAmount.TargetsCounter++;
                Destroy(gameObject);
            }

        }
    }
}
