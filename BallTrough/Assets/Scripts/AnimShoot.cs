using System.Collections;
using UnityEngine;

public class AnimShoot : MonoBehaviour
{
    public static bool clicked = false;
    private float rotationSpeed = -30;
    private float acummulatedRotation = 0;
    public static bool goingBackToInitialPosition = false;

    void Update()
    {
        Vector3 maxFootRotation = new Vector3(0, 0, -125);

        if (Input.GetKey(KeyCode.Mouse0) && !clicked && acummulatedRotation >= maxFootRotation.z)//boton izquierdo
        {
            //print("apretaste el boton izquierdo");
            Vector3 footRotation = Vector3.forward * Time.deltaTime * rotationSpeed;
            transform.Rotate(footRotation);
            acummulatedRotation += footRotation.z;
        }

        if ((Input.GetKeyUp(KeyCode.Mouse0) || goingBackToInitialPosition) && acummulatedRotation <= 0)
        {
            goingBackToInitialPosition = true;
            clicked = true;
            Vector3 footRotation = Vector3.forward * Time.deltaTime * rotationSpeed * -20;
            transform.Rotate(footRotation);
            acummulatedRotation += footRotation.z;
            StartCoroutine("Restart");
        }
    }

    IEnumerator Restart()
    {
        yield return new WaitForSeconds(1f);
        Reset();
    } 

    public void Reset()
    {
        clicked = false;
        acummulatedRotation = 0;
        goingBackToInitialPosition = false;
    }
}
