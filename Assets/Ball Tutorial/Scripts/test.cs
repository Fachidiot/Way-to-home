using UnityEngine;

public class test : MonoBehaviour
{
    [SerializeField] private GameObject cube;
    private void Start()
    {
        Vector3 a = new Vector3(1, 2, 3);
        Vector3 b = new Vector3(4, 5, 6);
        int[] c;

        int d = 128;
        c = new int[d];

        // Distance 구하는 공식 : vector 차의 제곱합의 제곱근.
        float distance = Vector3.Distance(a, b);
        Debug.Log($"{distance} == {Mathf.Sqrt((a - b).sqrMagnitude)}");

        // Vector의 차로 방향 구하기.
        var ga = Instantiate(cube, a, Quaternion.LookRotation(a)); ga.name = "a";
        Debug.DrawLine(Vector3.zero, a, Color.red, 5);

        var gb = Instantiate(cube, b, Quaternion.LookRotation(b)); gb.name = "b";
        Debug.DrawLine(Vector3.zero, b, Color.blue, 5);

        Debug.Log($"a에서 b를 향하는 방향값 : {b - a} / b에서 a를 향하는 방향값 : {a - b}");
        Vector3 aTob = b - a, bToa = a - b;

        Debug.DrawLine(Vector3.zero, (aTob), Color.pink, 5);
        Debug.DrawLine(Vector3.zero, (bToa), Color.skyBlue, 5);

        Debug.DrawLine(a, (aTob + a), Color.darkRed, 7);
        Debug.DrawLine(b, (bToa + b), Color.darkBlue, 7);
    }
}