using UnityEngine;
using UnityEngine.Events;

public class PhysicalButton : MonoBehaviour
{
    [Header("Событие при нажатии")]
    public UnityEvent onButtonTouched;

    // ВАРИАНТ 1: Для VR (Физическое касание контроллером)
    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, что в триггер вошла рука
        if (other.CompareTag("Player") || other.name.Contains("Hand") || other.name.Contains("Controller") || other.name.Contains("VRHandCT"))
        {
            ExecuteClick();
        }
    }

    // ВАРИАНТ 2: Для ПК / Клавиатуры (Клик мышкой прямо по 3D-кубу в редакторе)
    private void OnMouseDown()
    {
        // Этот метод Unity вызывает сама, если кликнуть левой кнопкой мыши по объекту с коллайдером
        ExecuteClick();
    }

    // Общая функция вызова
    private void ExecuteClick()
    {
        if (onButtonTouched != null)
        {
            Debug.Log($"[PhysicalButton] Кнопка {gameObject.name} успешно активирована!");
            onButtonTouched.Invoke();
        }
    }
}




