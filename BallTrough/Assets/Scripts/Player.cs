using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    public Transform SpawnPoint;
    public Canvas canvas;
    public GameObject ball;
    public float radius = 0.5f;
    public static float ballAngle = 0.1f;
    public ShootsAmount shootAmountReference;

    private float speedX;
    private float speedY;
    private float accelerationX;
    private float accelerationY;
    private float forceX;
    private float forceY;
    private float mass = 5f;
    private float posX;
    private float posY;
    private float gravity = -9.18f;
    private bool clickedLeft = false;
    private float height;
    private float width;
    private float angle = 45f;
    private float angleInRadians;
    private bool kick = false;
    private bool collisionWithFloor = false;
    private float angularVelocity = 0;
    private float angularAcceleration = 0;
    private float inertialMoment;
    private float rightLimit = 20;
    private float torque;

    private void Start()
    {
        posX = transform.position.x;
        posY = transform.position.y;
        inertialMoment = 0.01f * mass * radius * radius;
        Canvas canvas = new Canvas();
        height = this.canvas.pixelRect.height;
        width = this.canvas.pixelRect.width;
        height *= 0.0092f;
        width *= 0.0092f;
    }

    private void FixedUpdate()
    {
        if (kick)
        {
            Shoot();

            //Formula : x += vx * t + 0.5 * ax * t * t;  MRUV
            posX += speedX * Time.deltaTime + (0.5f * accelerationX * math.pow(Time.deltaTime, 2));
            posY += speedY * Time.deltaTime + (0.5f * accelerationY * math.pow(Time.deltaTime, 2));

            forceX = 0f;
            forceY = 0f;
            accelerationX = 0f;

            CalcGravity();
        }

        CalcPhysics();
        angleInRadians = angle * (math.PI / 180);
        DetectColission();
        RotateBall();
    }

    void Update()
    {
        if (ball.transform.position.x > rightLimit && shootAmountReference.shootsLeftAmountValue > 0)
        {
            shootAmountReference.shootsLeftAmountValue -= 1;
            kick = false;
            Respawn();
        }

        if (Input.GetKey(KeyCode.Mouse0) && !clickedLeft)
        {
            forceX += 1f;
            forceY += 1f;
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            clickedLeft = true;
            kick = true;
        }
    }

    private void CalcForce(float _fx, float _fy)
    {
        forceX += _fx;
        forceY += _fy;
    }

    private void CalcGravity()
    {
        CalcForce(0f, gravity * mass);
    }

    private void CalcPhysics()
    {
        //Formula: 𝐹𝑥 = 𝑚𝑎𝑥; f
        accelerationX = forceX / mass;
        accelerationY = forceY / mass;

        //speedX += accelerationX * Time.deltaTime;
        speedX += accelerationX * Time.deltaTime;
        speedY += accelerationY * Time.deltaTime;
    }

    private void Shoot()
    {
        Vector2 moveBall = new Vector2(posX, posY);
        transform.position = moveBall;
    }

    public void DetectColission()
    {
        if ((ball.transform.position.y - (radius - 0.2f)) <= -(height / 2)) //abajo
        {
            speedY = math.abs(speedY);
            CalculateRotationBall();
            collisionWithFloor = true;
        }
    }

    public void CalculateRotationBall()
    {
        angularVelocity = speedY / radius;

        angularAcceleration = angularVelocity / Time.deltaTime;

        torque = -angularAcceleration * inertialMoment;
    }

    public void RotateBall()
    {
        if (collisionWithFloor == true)
        {
            //print(torque);
            transform.Rotate(new Vector3(0f, 0f, torque));
        }
    }

    private void Respawn()
    {
        transform.position = SpawnPoint.position;
        clickedLeft = false;
        accelerationX = 0f;
        accelerationY = 0f;
        forceX = 0f;
        forceY = 0f;
        speedX = 0f;
        speedY = 0f;
        posX = transform.position.x;
        posY = transform.position.y;
        torque = 0;
        ballAngle = 0f;
        angularVelocity = 0f;
        angularAcceleration = 0f;
        kick = false;
    }
}




