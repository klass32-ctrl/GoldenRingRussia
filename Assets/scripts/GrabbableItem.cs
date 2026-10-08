using UnityEngine;

public class GrabbableItem : MonoBehaviour
{
    [Header("Íàñòðîéêè")]
    public bool isGrabbed = false;
    private Transform grabPoint;      // òî÷êà, ê êîòîðîé ïðèòÿãèâàåòñÿ ïðåäìåò
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

        // Îòêëþ÷àåì ôèçèêó, ïîêà ïðåäìåò â ðóêå
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // Äåëàåì ïðåäìåò äî÷åðíèì ê ðóêå
        transform.SetParent(grabPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Release()
    {
        isGrabbed = false;

        // 1. Отвязываем от руки БЕЗ сохранения иерархии (полный сброс в корень сцены)
        transform.SetParent(null);

        // 2. Форсируем включение физики напрямую
        if (rb != null)
        {
            rb.isKinematic = false; // Принудительно выключаем кинематику
            rb.useGravity = true;   // Принудительно включаем гравитацию

            // Сбрасываем скорости, чтобы объект не улетал от старого движения руки
#if UNITY_2023_1_OR_NEWER
            rb.linearVelocity = Vector3.zero;
#else
        rb.velocity = Vector3.zero;
#endif
            rb.angularVelocity = Vector3.zero;

            // Будим физический движок
            rb.WakeUp();
        }

        // 3. Проверяем коллайдер
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = false; // Убеждаемся, что это ТВЕРДОЕ тело, а не призрак
            col.enabled = true;
        }
    }




}
