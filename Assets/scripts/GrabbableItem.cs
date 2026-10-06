using UnityEngine;

public class GrabbableItem : MonoBehaviour
{
    [Header("Настройки")]
    public bool isGrabbed = false;
    private Transform grabPoint;      // точка, к которой притягивается предмет
    private Rigidbody rb;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Grab(Transform handHold)
    {
        isGrabbed = true;
        grabPoint = handHold;

        // Отключаем физику, пока предмет в руке
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // Делаем предмет дочерним к руке
        transform.SetParent(grabPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Release()
    {
        isGrabbed = false;

        // Отвязываем от руки
        transform.SetParent(null);

        // Возвращаем физику
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }
}
