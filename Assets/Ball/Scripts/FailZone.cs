using UnityEngine;
using UnityEngine.SceneManagement;

public class FailZone : MonoBehaviour
{
    [SerializeField] private LayerMask layerName;
    void OnTriggerEnter(Collider other)
    {
        if (0 != other.gameObject.layer.CompareTo(layerName))
            SceneManager.LoadScene("FailScene");
    }
}