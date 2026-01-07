using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public bool collide = true;
    public bool isMove = true;
    public float force = 1000f;
    public float max = 3.5f;
    public float speed = 0.1f;

    float delta;
    Animator animator;

    private void Start()
    {
        Application.targetFrameRate = 60;
        animator = GetComponent<Animator>();
        delta = speed;
    }

    void Update()
    {
        if (isMove)
        {   // Ball Logic
            transform.localPosition = new Vector3(
                transform.localPosition.x + delta,
                transform.localPosition.y,
                transform.localPosition.z);

            if (transform.localPosition.x < -max)
                delta = speed;
            else if (transform.localPosition.x > max)
                delta = -speed;
        }
    }

    float DistanceTo(string name)
    {
        var distance = Vector3.Distance(GameObject.Find(name).transform.position,
        transform.position);
        Debug.Log($"{name}과의 거리 : {distance}");
        return distance;
    }

    void OnCollisionEnter(Collision collision)
    {   // 공과의 충돌시 반대 방향으로 날라가게 해야함.
        if (!collide)
            return;
        Vector3 direction = collision.transform.position - transform.position;
        Rigidbody rigidbody;
        if (collision.gameObject.TryGetComponent<Rigidbody>(out rigidbody))
            rigidbody.AddForce(direction.normalized * force);
        if (animator)
            animator.SetTrigger("collision");

        Ball.combo++;
    }
}
