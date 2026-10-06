using UnityEngine;
using UnityEngine.InputSystem;

public class HandInteraction : MonoBehaviour
{
    [Header("Ссылки")]
    public Transform fingerTip;
    public Transform holdPoint;

    [Header("Настройки")]
    public float maxDistance = 5f;
    public LayerMask interactionLayers;
    public Key grabKey = Key.G;

    private LineRenderer lineRenderer;
    private GrabbableItem hoveredItem;
    private GrabbableItem grabbedItem;

    // Флаг: было ли начало нажатия в этом кадре
    private bool isGrabTriggeredThisFrame = false;
    private bool wasKeyDownLastFrame = false;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
            lineRenderer = gameObject.AddComponent<LineRenderer>();

        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;
        var mat = new Material(Shader.Find("Unlit/Color")) { color = Color.cyan };
        lineRenderer.material = mat;

        if (holdPoint == null)
        {
            holdPoint = new GameObject("HoldPoint").transform;
            holdPoint.SetParent(transform);
            holdPoint.localPosition = Vector3.zero;
        }
    }

    void Update()
    {
        Vector3 origin = (fingerTip != null) ? fingerTip.position : transform.position;
        Vector3 direction = (fingerTip != null) ? fingerTip.forward : transform.forward;

        RaycastHit hit;
        bool hasHit = Physics.Raycast(origin, direction, out hit, maxDistance, interactionLayers);

        // Рисуем луч
        lineRenderer.SetPosition(0, origin);
        lineRenderer.SetPosition(1, hasHit ? hit.point : origin + direction * maxDistance);

        // Цвет луча
        if (grabbedItem != null)
            lineRenderer.material.color = Color.yellow;
        else if (hasHit && hit.collider.GetComponent<GrabbableItem>() != null)
            lineRenderer.material.color = Color.green;
        else
            lineRenderer.material.color = Color.cyan;

        // Обновляем hoveredItem
        if (hasHit && grabbedItem == null)
            hoveredItem = hit.collider.GetComponent<GrabbableItem>();
        else if (!hasHit)
            hoveredItem = null;

        // --- СТАБИЛЬНЫЙ ЗАХВАТ ПО УДЕРЖАНИЮ ---
        var keyboard = Keyboard.current;
        bool isKeyDown = keyboard != null && keyboard[grabKey].isPressed;

        // Фиксируем момент «начала нажатия» (один кадр)
        if (isKeyDown && !wasKeyDownLastFrame)
            isGrabTriggeredThisFrame = true;

        wasKeyDownLastFrame = isKeyDown;

        // Логика: если было срабатывание и есть предмет — берём
        if (isGrabTriggeredThisFrame && hoveredItem != null && grabbedItem == null)
        {
            hoveredItem.Grab(holdPoint);
            grabbedItem = hoveredItem;
        }

        // Если кнопка отпущена и что-то держали — отпускаем
        if (!isKeyDown && grabbedItem != null)
        {
            grabbedItem.Release();
            grabbedItem = null;
        }

        isGrabTriggeredThisFrame = false; // сбрасываем, чтобы не сработало снова в этом же кадре
    }
}
