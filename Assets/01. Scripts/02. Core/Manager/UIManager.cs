using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private InGameUIController inGameUIController;
    public InGameUIController InGameUI { get { return inGameUIController; } }

    void Awake()
    {
        if (null == Instance)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
