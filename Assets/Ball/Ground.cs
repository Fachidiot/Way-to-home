using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class Ground : MonoBehaviour
{
    void Update()
    {
        float z = (transform.localEulerAngles.z - Input.GetAxis("Horizontal")) * Time.deltaTime;
        transform.localEulerAngles += new Vector3(0, 0, z);

        if (Input.GetMouseButtonDown(0))
        {
            if (Input.mousePosition.x < Screen.width / 2)
                z = 1;
            else
                z = -1;
            transform.localEulerAngles += new Vector3(0, 0, z);
        }
    }
}
