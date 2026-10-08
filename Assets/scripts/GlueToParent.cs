using UnityEngine;

public class GlueToParent : MonoBehaviour
{
    void LateUpdate()
    {
        // Насильно привязываем позицию и поворот модели к родителю
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
}
