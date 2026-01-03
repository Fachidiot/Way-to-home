using UnityEngine;

public class test
{
    private void Start()
    {
        int age = 30;
        if (20 > age || age <= 65)
            Debug.Log($"discount");
        else
            Debug.Log($"not discount");

        if (age / 10 == 3)
            Debug.Log($"30");
        else
            Debug.Log($"not 30");
    }
}