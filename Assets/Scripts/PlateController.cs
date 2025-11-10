using UnityEngine;
using UnityEngine.Events;

public class PlateController : MonoBehaviour
{
    [SerializeField]
    private float pressedThreshold = 0.01f;

    public UnityEvent OnPlateActivated;

    private Vector3 startPos;
    private bool isActivated;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        //one time activation
        if (isActivated) return;

        float displacement = startPos.y - transform.localPosition.y;
        isActivated = displacement > pressedThreshold;

        if (isActivated)
            OnPlateActivated?.Invoke();
    }
}
