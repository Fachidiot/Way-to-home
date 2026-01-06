using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ball : MonoBehaviour
{
    public LayerMask groundLayer;
    public bool isBall;
    [SerializeField] private bool growable;
    [SerializeField] private Ground ground;
    [SerializeField] private float resetHeight = -11;
    bool isGrounded;
    float maxSize = 3f;
    Vector3 startPos;
    Vector3 prevPosition;
    bool isStart = false;

    private void Start()
    {
        startPos = transform.position;
        GetComponent<Rigidbody>().useGravity = false;
    }

    void Update()
    {
        if (!isStart)
        {
            isStart = Input.anyKey;
            return;
        }
        if (!GetComponent<Rigidbody>().useGravity)
        {
            GetComponent<Rigidbody>().useGravity = true;
        }

        if (isBall)
        {
            Jump();
            // GroundCheck();
            if (growable)
                Increase();
            // Decrease();
        }
        Reset();

        if (WaytoHome)
            SceneManager.LoadScene("Stage1_Scene");
    }

    void OnCollisionEnter(Collision collision)
    {
        // if(collision.gameObject.tag)
        if (0 == collision.gameObject.layer.CompareTo(groundLayer))
            isGrounded = true;
    }

    void OnCollisionExit(Collision collision)
    {
        if (0 == collision.gameObject.layer.CompareTo(groundLayer))
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
        if (transform.localPosition.y < resetHeight)
        {
            transform.position = startPos;
            transform.rotation = Quaternion.identity;

            var rigidbody = GetComponent<Rigidbody>();
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;

            if (Coin.best < Coin.score)
                Coin.best = Coin.score;
            Coin.score = 0;
        }
    }

    bool WaytoHome = false;
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 60, 60));
        GUILayout.TextArea("Best : " + Coin.best.ToString());
        GUILayout.TextArea("Score: " + Coin.score.ToString());
        GUILayout.EndArea();

        GUIStyle buttonstyle = new GUIStyle(GUI.skin.button);
        buttonstyle.alignment = TextAnchor.MiddleCenter;
        buttonstyle.fontStyle = FontStyle.Bold;

        GUILayout.BeginArea(new Rect(Screen.width - 100, Screen.height - 40, 90, 30));
        WaytoHome = GUILayout.Button("WayToHome Demo", buttonstyle);
        GUILayout.EndArea();
    }
}
