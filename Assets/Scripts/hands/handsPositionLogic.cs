using UnityEngine;

public class handsPositionLogic : MonoBehaviour
{
    [SerializeField] private Transform cameraPosition;
    [SerializeField] private Transform idlePos;
    [SerializeField] private float lenght = 1f;
    public LayerMask Default;

    void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraPosition.position, cameraPosition.forward, out hit, 10, Default)) transform.position = hit.point + transform.forward * 0.3f;
        else transform.position = idlePos.position;
    }
}
