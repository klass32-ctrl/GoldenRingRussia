using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class MapPuzzleManager : MonoBehaviour
{
    [Header("Сокеты")]
    public XRSocketInteractor socket1;
    public XRSocketInteractor socket2;

    private int piecesPlaced = 0;

    void OnEnable()
    {
        if (socket1 != null) socket1.selectEntered.AddListener(OnFirstPiecePlaced);
        if (socket2 != null) socket2.selectEntered.AddListener(OnSecondPiecePlaced);
    }

    void OnDisable()
    {
        if (socket1 != null) socket1.selectEntered.RemoveListener(OnFirstPiecePlaced);
        if (socket2 != null) socket2.selectEntered.RemoveListener(OnSecondPiecePlaced);
    }

    private void OnFirstPiecePlaced(SelectEnterEventArgs args)
    {
        piecesPlaced++;
        Debug.Log("Первая половинка на месте!");

        socket1.enabled = false;

        // Запуск фиксации с задержкой, передавая сам сокет как ориентир
        StartCoroutine(LockObjectRoutine(args.interactableObject.transform.gameObject, socket1));

        if (socket2 != null)
        {
            socket2.gameObject.SetActive(true);
            socket2.enabled = true;
        }

        CheckWinCondition();
    }

    private void OnSecondPiecePlaced(SelectEnterEventArgs args)
    {
        piecesPlaced++;
        Debug.Log("Вторая половинка на месте!");

        socket2.enabled = false;

        StartCoroutine(LockObjectRoutine(args.interactableObject.transform.gameObject, socket2));

        CheckWinCondition();
    }

    // Корутина, которая переждет внутренние процессы Unity XR и жестко поставит объект на место
    private IEnumerator LockObjectRoutine(GameObject item, XRSocketInteractor socket)
    {
        // 1. Ждем конца текущего кадра
        yield return new WaitForEndOfFrame();

        // 2. Удаляем ваш самописный скрипт захвата
        var customGrab = item.GetComponent<GrabbableItem>();
        if (customGrab != null)
        {
            Destroy(customGrab);
        }

        // 3. Отключаем стандартный XRI
        var grabInteractable = item.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.enabled = false;
        }

        // 4. Жестко блокируем физику Rigidbody
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
#if UNITY_2023_1_OR_NEWER
            rb.linearVelocity = Vector3.zero;
#else
        rb.velocity = Vector3.zero;
#endif
            rb.angularVelocity = Vector3.zero;
        }

        // 5. Отключаем твердые коллайдеры на карте
        Collider[] colliders = item.GetComponentsInChildren<Collider>();
        foreach (var col in colliders)
        {
            if (!col.isTrigger) col.enabled = false;
        }

        // --- ОБНОВЛЕННАЯ ЛОГИКА ВЫРАВНИВАНИЯ ---
        // Определяем, куда именно привязывать карту. 
        // Если у сокета настроен Attach Transform, берем его. Если нет — центр сокета.
        Transform targetAttach = socket.attachTransform != null ? socket.attachTransform : socket.transform;

        // Жестко привязываем карту к этой точке в иерархии
        item.transform.SetParent(targetAttach);

        // Сбрасываем координаты в абсолютный ноль ОТНОСИТЕЛЬНО ТОЧКИ ПРИВЯЗКИ
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        Debug.Log($"Объект {item.name} успешно заблокирован в точке привязки сокета {socket.name}.");
    }



    private void CheckWinCondition()
    {
        if (piecesPlaced >= 2)
        {
            Debug.Log("ИГРА ОКОНЧЕНА! Карта полностью собрана.");
        }
    }
}

