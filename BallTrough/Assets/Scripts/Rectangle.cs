using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultSide
{
    public Vector2 normal;
    public float embutido; //Cuanto se mete la esquina de un rectangulo en otro rectangulo

    public ResultSide(Vector2 _normal, float _embutido)
    {
        normal = _normal;
        embutido = _embutido;
    }
}

public class Rectangle : MonoBehaviour
{
    private float posX;
    private float posY;
    private float width;
    private float height;
    private float angle;
    private float angularVelocity;
    private float angularAcceleration;
    private float forceX;
    private float forceY;
    private float mass = 5f;
    private float gravity = -9.8f;
    private float accelerationX;
    private float accelerationY;
    private float speedX;
    private float speedY;
    private float speedPointX;
    private float speedPointY;
    private float torque;
    private float inertia;
    private float random = 5;
    private float speedYReference;
    private bool detectCollisionWithFloor = false;
    private bool collideWithBall = false;

    private Vector2 a_corner;
    private Vector2 b_corner;
    private Vector2 c_corner;
    private Vector2 d_corner;
    private Vector2 center;
    private Vector2 vector_width;
    private Vector2 vector_height;
    private SpriteRenderer rectSpriteRenderer;
    private Transform rectTransform;
    public Transform floorTransform;
    public GameObject ball;
    public Rectangle[] cans;
    public Player ballRef;
    private bool collided = true;
    private float timeToSleep = 2.5f;
    private float timeLeftToSleep = 0;

    private float projectionHeight;
    private float projectionWidth;

    void Start()
    {
        rectSpriteRenderer = GetComponent<SpriteRenderer>();
        rectTransform = GetComponent<Transform>();
        posX = rectTransform.position.x;
        posY = rectTransform.position.y;
        angle = rectTransform.eulerAngles.z * Mathf.PI / 180;
        width = rectSpriteRenderer.size.x;
        height = rectSpriteRenderer.size.y;
        inertia = ((width * width) + (height * height) * mass) / 20;
        angularAcceleration = 0;
        angularVelocity = 0;
        torque = 0;
    }

    void FixedUpdate()
    {
        center.x = rectTransform.position.x;
        center.y = rectTransform.position.y;
        RBCornersSidesPositions(); //Calcula posiciones de las esquinas y de los lados de rectangulo
        DetectCollisionWithBall(ballRef);
        DetectCollisionWithFloor();

        if (collideWithBall == true && collided)
        {
            CalculateGravity();
            DetectCollisionWithCan();
            ShootsAmount.CansScoreCounter++;
            collided = false;
        }
        else if (collided == false && timeLeftToSleep < timeToSleep)
        {
            timeLeftToSleep += Time.deltaTime;
            CalculateGravity();
            DetectCollisionWithCan();
            CalculatePhysics();
        }

        forceX = 0;
        forceY = 0;
    }

    
    private void DetectCollisionWithCan()
    {
        var tempIndex = 0;

        foreach (var can in cans)
        {
            CalculateCollisionWithCan(tempIndex);
            tempIndex++;
        }
    }

    private void CalculateCollisionWithCan(int _indexCan)
    {
        CollisionWithCorners(_indexCan);
    }

    private Vector2 DirW(float _angle)
    {
        return new Vector2(Mathf.Cos(_angle), Mathf.Sin(_angle));
    }

    private Vector2 DirH(float _angle)
    {
        return new Vector2(Mathf.Sin(_angle), -Mathf.Cos(_angle));
    }

    private ResultSide CalculateSide(float _projectionWidth, float _projectionHeight, float _angle)
    {
        var distanceLeft = Mathf.Abs(_projectionWidth + width);
        var distanceRight = Mathf.Abs(_projectionWidth - width);
        var distanceInf = Mathf.Abs(_projectionHeight + height);
        var distanceSup = Mathf.Abs(_projectionHeight - height);
        var closestSide = Mathf.Min(distanceLeft, distanceRight, distanceSup, distanceInf);
        Vector2 returnValue = new Vector2();

        if (closestSide == distanceRight)
        {
            returnValue = DirW(_angle);
        }

        if (closestSide == distanceSup)
        {
            returnValue = DirH(_angle);
        }

        if (closestSide == distanceLeft)
        {
            Vector2 invertido = DirW(_angle);
            invertido.x = -invertido.x;
            invertido.y = -invertido.y;
            returnValue = invertido;
        }

        if (closestSide == distanceInf)
        {
            Vector2 invertido = DirH(_angle);
            invertido.x = -invertido.x;
            invertido.y = -invertido.y;
            returnValue = invertido;
        }

        return new ResultSide(returnValue, closestSide);
    }

    private void DetectCollisionWithBall(Player _ball)
    {
        //Cada lata va a detectar la colision individual con la pelota
        float diffX;
        float diffY;
        var multiplierForce = 500;

        Vector2 dirHother = DirH(angle);
        Vector2 dirWother = DirW(angle);

        diffX = a_corner.x - _ball.transform.position.x - _ball.radius;
        diffY = a_corner.y - _ball.transform.position.y - _ball.radius;

        projectionWidth = diffX * dirWother.x + diffY * dirWother.y;
        projectionHeight = diffX * dirHother.x + diffY * dirHother.y;

        if (Mathf.Abs(projectionWidth) < this.width && Mathf.Abs(projectionHeight) < this.height)
        {
            collideWithBall = true;
            CalculatePointVelocity(a_corner.x, a_corner.y);
            ResultSide result = CalculateSide(projectionWidth, projectionHeight, angle);
            //ApplyImpulse(result.normal.x * multiplierForce * result.embutido, result.normal.y * multiplierForce * result.embutido);

            ApplyPointForce(result.normal.x * multiplierForce * result.embutido, result.normal.y * multiplierForce * result.embutido, a_corner.x, a_corner.y);
        }

        //ESQUINA D -OK
        diffX = d_corner.x - _ball.transform.position.x + _ball.radius;
        diffY = d_corner.y - _ball.transform.position.y + _ball.radius;
        projectionWidth = diffX * dirWother.x + diffY * dirWother.y;
        projectionHeight = diffX * dirHother.x + diffY * dirHother.y;
        if (Mathf.Abs(projectionWidth) < this.width && Mathf.Abs(projectionHeight) < this.height)
        {
            collideWithBall = true;
            CalculatePointVelocity(d_corner.x, d_corner.y);
            ResultSide result = CalculateSide(projectionWidth, projectionHeight, angle);
            //ApplyImpulse(result.normal.x * multiplierForce * result.embutido, result.normal.y * multiplierForce * result.embutido);

            ApplyPointForce(result.normal.x * multiplierForce * result.embutido, result.normal.y * multiplierForce * result.embutido, d_corner.x, d_corner.y);
        }

        //ESQUINA B -OK
        diffX = b_corner.x - _ball.transform.position.x + _ball.radius;
        diffY = b_corner.y - _ball.transform.position.y + _ball.radius;
        projectionWidth = diffX * dirWother.x + diffY * dirWother.y;
        projectionHeight = diffX * dirHother.x + diffY * dirHother.y;
        if (Mathf.Abs(projectionWidth) < this.width && Mathf.Abs(projectionHeight) < this.height)
        {
            collideWithBall = true;
            CalculatePointVelocity(b_corner.x, b_corner.y);
            ResultSide result = CalculateSide(projectionWidth, projectionHeight, angle);
            //ApplyImpulse(result.normal.x * multiplierForce * result.embutido, result.normal.y * multiplierForce * result.embutido);

            ApplyPointForce(result.normal.x * multiplierForce * result.embutido, result.normal.y * multiplierForce * result.embutido, b_corner.x, b_corner.y);

        }

        //ESQUINA C
        diffX = c_corner.x - _ball.transform.position.x + _ball.radius;
        diffY = c_corner.y - _ball.transform.position.y + _ball.radius;
        projectionWidth = diffX * dirWother.x + diffY * dirWother.y;
        projectionHeight = diffX * dirHother.x + diffY * dirHother.y;
        if (Mathf.Abs(projectionWidth) < this.width && Mathf.Abs(projectionHeight) < this.height)
        {
            collideWithBall = true;
            CalculatePointVelocity(c_corner.x, c_corner.y);
            ResultSide result = CalculateSide(projectionWidth, projectionHeight, angle);
            //ApplyImpulse(result.normal.x * multiplierForce * result.embutido, result.normal.y * multiplierForce * result.embutido);

            ApplyPointForce(result.normal.x * multiplierForce * result.embutido, result.normal.y * multiplierForce * result.embutido, c_corner.x, c_corner.y);
        }
    }

    private void CollisionWithCorners(int _indexCan)
    {
        float diffX;
        float diffY;
        var multiplierForce = 200;

        Vector2 dirHother = DirH(cans[_indexCan].angle);
        Vector2 dirWother = DirW(cans[_indexCan].angle);

        diffX = a_corner.x - cans[_indexCan].center.x;
        diffY = a_corner.y - cans[_indexCan].center.y;
        projectionWidth = diffX * dirWother.x + diffY * dirWother.y;
        projectionHeight = diffX * dirHother.x + diffY * dirHother.y;

        //ESQUINA A -OK
        if (Mathf.Abs(projectionWidth) < this.width && Mathf.Abs(projectionHeight) < this.height)
        {
            CalculatePointVelocity(a_corner.x, a_corner.y);
            ResultSide result = CalculateSide(projectionWidth, projectionHeight, cans[_indexCan].angle);

            ApplyPointForce(result.normal.x * multiplierForce * result.embutido, result.normal.y * multiplierForce * result.embutido, a_corner.x, a_corner.y);
        }

        //ESQUINA D -OK
        diffX = a_corner.x - cans[_indexCan].center.x;
        diffY = a_corner.y - cans[_indexCan].center.y;
        projectionWidth = diffX * dirWother.x + diffY * dirWother.y;
        projectionHeight = diffX * dirHother.x + diffY * dirHother.y;
        if (Mathf.Abs(projectionWidth) < this.width && Mathf.Abs(projectionHeight) < this.height)
        {
            CalculatePointVelocity(d_corner.x, d_corner.y);
            ResultSide result = CalculateSide(projectionWidth, projectionHeight, cans[_indexCan].angle);

            ApplyPointForce(result.normal.x * multiplierForce * result.embutido, result.normal.y * multiplierForce * result.embutido, d_corner.x, d_corner.y);
        }

        //ESQUINA B -OK
        diffX = b_corner.x - cans[_indexCan].center.x;
        diffY = b_corner.y - cans[_indexCan].center.y;
        projectionWidth = diffX * dirWother.x + diffY * dirWother.y;
        projectionHeight = diffX * dirHother.x + diffY * dirHother.y;
        if (Mathf.Abs(projectionWidth) < this.width && Mathf.Abs(projectionHeight) < this.height)
        {
            CalculatePointVelocity(b_corner.x, b_corner.y);
            ResultSide result = CalculateSide(projectionWidth, projectionHeight, cans[_indexCan].angle);

            ApplyPointForce(result.normal.x * multiplierForce * result.embutido, result.normal.y * multiplierForce * result.embutido, b_corner.x, b_corner.y);

        }

        //ESQUINA C
        diffX = c_corner.x - cans[_indexCan].center.x;
        diffY = c_corner.y - cans[_indexCan].center.y;
        projectionWidth = diffX * dirWother.x + diffY * dirWother.y;
        projectionHeight = diffX * dirHother.x + diffY * dirHother.y;
        if (Mathf.Abs(projectionWidth) < this.width && Mathf.Abs(projectionHeight) < this.height)
        {
            CalculatePointVelocity(c_corner.x, c_corner.y);
            ResultSide result = CalculateSide(projectionWidth, projectionHeight, cans[_indexCan].angle);

            ApplyPointForce(result.normal.x * multiplierForce * result.embutido, result.normal.y * multiplierForce * result.embutido, c_corner.x, c_corner.y);
        }
    }

    private void DetectCollisionWithFloor()
    {
        //float auxX = 0;

        if (a_corner.y <= floorTransform.position.y)
        {
            CalculatePointVelocity(a_corner.x, a_corner.y);

            if (speedPointY < 0)
            {
                ApplyImpulsePoint(-speedPointX * 0.5f, -speedPointY, a_corner.x, a_corner.y);
                var diffY = a_corner.y - floorTransform.position.y;
                posY -= diffY;

            }
        }

        if (b_corner.y <= floorTransform.position.y)
        {
            CalculatePointVelocity(b_corner.x, b_corner.y);

            if (speedPointY < 0)
            {
                ApplyImpulsePoint(-speedPointX * 0.5f, -speedPointY, b_corner.x, b_corner.y);
                var diffY = b_corner.y - floorTransform.position.y;
                posY -= diffY;
            }
        }

        if (c_corner.y <= floorTransform.position.y)
        {
            CalculatePointVelocity(c_corner.x, c_corner.y);

            if (speedPointY < 0)
            {
                ApplyImpulsePoint(-speedPointX * 0.5f, -speedPointY, c_corner.x, c_corner.y);
                var diffY = c_corner.y - floorTransform.position.y;
                posY -= diffY;
            }
        }

        if (d_corner.y <= floorTransform.position.y)
        {
            CalculatePointVelocity(d_corner.x, d_corner.y);

            if (speedPointY < 0)
            {
                ApplyImpulsePoint(-speedPointX * 0.5f, -speedPointY, d_corner.x, d_corner.y);
                var diffY = d_corner.y - floorTransform.position.y;
                posY -= diffY;
            }

        }
    }

    private void RBCornersSidesPositions()
    {
        //Calculo de distancia desde el centro hacia cualquiera de los costados
        vector_width.x = (Mathf.Cos(angle) * width / 2);
        vector_width.y = (Mathf.Sin(angle) * width / 2);

        vector_height.x = -(Mathf.Sin(angle) * height / 2);
        vector_height.y = (Mathf.Cos(angle) * height / 2);

        //Calculo de las posiciones de las esquinas -- Esquina A = Punto superior derecho // Esquina B = Punto inferior derecho // Esquina C = Punto inferior izquierdo // Esquina D = Punto superior izquierdo
        //Calculo de esquina A
        a_corner.x = (center.x + vector_width.x + vector_height.x);
        a_corner.y = (center.y + vector_width.y + vector_height.y);
        //print($"la posicion x,y del corner a es.. {a_corner.x} , {a_corner.y} ");

        //Calculo de esquina B
        b_corner.x = (center.x + vector_width.x - vector_height.x);
        b_corner.y = (center.y + vector_width.y - vector_height.y);
        //print($"la posicion x,y del corner b es.. {b_corner.x} , {b_corner.y} ");


        //Calculo de esquina C
        c_corner.x = (center.x - vector_width.x - vector_height.x);
        c_corner.y = (center.y - vector_width.y - vector_height.y);
        //print($"la posicion x,y del corner c es.. {c_corner.x} , {c_corner.y} ");


        //Calculo de esquina D
        d_corner.x = (center.x - vector_width.x + vector_height.x);
        d_corner.y = (center.y - vector_width.y + vector_height.y);
        //print($"la posicion x,y del corner d es.. {d_corner.x} , {d_corner.y} ");
    }

    private void CalculateForce(float _fx, float _fy)
    {
        forceX += _fx;
        forceY += _fy;
    }

    private void CalculateGravity()
    {
        //F(fuerza neta externa) = m * g
        CalculateForce(0, gravity * mass);
    }

    private void CalculatePhysics()
    {
        //Formula: 𝐹𝑥 = 𝑚𝑎𝑥; f (aceleracion)
        accelerationX = forceX / mass;
        accelerationY = forceY / mass;

        //Formula : x += vx * t + 0.5 * ax * t * t;  MRUV - caida
        posX += speedX * Time.deltaTime + 0.5f * accelerationX * Time.deltaTime * Time.deltaTime;
        posY += speedY * Time.deltaTime + 0.5f * accelerationY * Time.deltaTime * Time.deltaTime;

        //speedX += accelerationX * Time.deltaTime;
        speedX += accelerationX * Time.deltaTime;
        speedY += accelerationY * Time.deltaTime;
        rectTransform.position = new Vector2(posX, posY);

        forceX = 0;
        forceY = 0;

        //Velocidad angular = Δ𝜃 / Δ𝑡
        angularVelocity = angularAcceleration * Time.deltaTime;

        //Aceleracion angular = Δ𝜔 / Δ𝑡
        angularAcceleration = torque / inertia;

        //Angulo
        angle += angularVelocity * Time.deltaTime + 0.5f * angularAcceleration * Time.deltaTime * Time.deltaTime;
        transform.eulerAngles = new Vector3(0, 0, angle * 180 / Mathf.PI);

        torque = 0;
    }

    private void AddTorque(float _torque)
    {
        torque += _torque;
    }

    private void ApplyPointForce(float _forceX, float _forceY, float _posX, float _posY)
    {
        this.forceX += _forceX;
        this.forceY += _forceY;

        float diffX = _posX - this.posX;
        float diffy = _posY - this.posY;

        AddTorque(_forceX * diffy + _forceY * diffX);
    }

    private void ApplyImpulse(float velX, float velY)
    {
        var fx = velX * mass / Time.deltaTime;
        var fy = velY * mass / Time.deltaTime;

        CalculateForce(fx, fy);
    }

    private void ApplyImpulsePoint(float _velX, float _velY, float _posX, float _posY)
    {
        var fx = _velX * mass / Time.deltaTime * 0.1f;
        var fy = _velY * mass / Time.deltaTime * 0.1f;

        ApplyPointForce(fx, fy, _posX, _posY);
    }

    private void CalculatePointVelocity(float _posX, float _posY)
    {
        var diffX = _posX - this.posX;
        var diffY = _posY - this.posY;

        speedPointX = -diffY * angularVelocity + speedX;
        speedPointY = diffX * angularVelocity + speedY;
    }
}
