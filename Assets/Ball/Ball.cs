using System.Diagnostics;
using UnityEngine;

public class Ball : MonoBehaviour
{
    bool isGrounded;
    float maxSize = 3f;
    float minSize = 0.2f;
    Vector3 start;
    Vector3 prevPosition;

    private void Start()
    {
        start = transform.position;
    }

    void Update()
    {
        Jump();
        // GroundCheck();
        Increase();
        // Decrease();
        Reset();
    }

    void GroundCheck()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        // if(collision.gameObject.tag)
        isGrounded = true;
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            GetComponent<Rigidbody>().AddForce(Vector3.up * 300);

        if (Input.GetKey(KeyCode.Backspace))
            GetComponent<Rigidbody>().AddForce(Vector3.back);
    }

    void Increase()
    {
        Vector3 amount = prevPosition - transform.position;
        if (isGrounded && amount != Vector3.zero && transform.localScale.x < maxSize)
        {   // 움직였을때
            transform.localScale += new Vector3(0.1f, 0.1f, 0.1f) * Time.deltaTime;
        }

        prevPosition = transform.position;
    }

    void Decrease()
    {
        Vector3 amount = prevPosition - transform.position;
        if (amount != Vector3.zero)
        {   // 움직였을때
            transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
        }

        prevPosition = transform.position;
    }

    void Reset()
    {
        float yDistance = Mathf.Abs(transform.position.y - start.y);

        if (yDistance > 20)
        {
            transform.position = start;
            GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        }
    }
}
