using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    
    private Rigidbody rb;
    private Camera mainCam;

    void FixedUpdate()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");
        rb = GetComponent<Rigidbody>();
        mainCam = Camera.main;

        Vector3 inputDir = new Vector3(inputX, 0f, inputZ).normalized;
        if (inputDir.magnitude >= 0.01f)
        {
            Vector3 move = inputDir * moveSpeed;
            rb.MovePosition(rb.position + move * Time.fixedDeltaTime);
        }
        Vector3 worldPos = GetMouseWorldPositionAtPlayerHeight();

        Vector3 direction = worldPos - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime));
        }
    }
    Vector3 GetMouseWorldPositionAtPlayerHeight()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);

        Plane plane = new Plane(Vector3.up, new Vector3(0, transform.position.y, 0));

        if (plane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }

        return transform.position + transform.forward;
    }

}