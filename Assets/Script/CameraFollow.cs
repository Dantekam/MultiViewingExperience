using UnityEngine;

public class CanvasFollow : MonoBehaviour
{
    [SerializeField] private Transform head;

    [Header("Follow")]
    [SerializeField] private float distance = 2f;
    [SerializeField] private float heightOffset = -0.15f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotateSpeed = 5f;

    [Header("Lazy Follow")]
    [SerializeField] private float maxAngle = 35f;

    void LateUpdate()
    {
        if (head == null)
            return;

        Vector3 forward = head.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 desiredPosition =
            head.position +
            forward * distance;

        desiredPosition.y += heightOffset;

        Vector3 toCanvas =
            transform.position - head.position;

        float angle =
            Vector3.Angle(forward, toCanvas);

        if (angle > maxAngle)
        {
            transform.position =
                Vector3.Lerp(
                    transform.position,
                    desiredPosition,
                    Time.deltaTime * moveSpeed);

            Quaternion lookRotation =
                Quaternion.LookRotation(
                    transform.position - head.position);

            transform.rotation =
                Quaternion.Slerp(
                    transform.rotation,
                    lookRotation,
                    Time.deltaTime * rotateSpeed);
        }
    }
}