using UnityEngine;

public class handsPositionLogic : MonoBehaviour
{
    [SerializeField] private Transform cameraPosition;
    [SerializeField] private Transform idlePos;
    [SerializeField] private int lenght = 1;
    public LayerMask Default;

    void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraPosition.position, cameraPosition.forward, out hit, lenght, Default)) transform.position = hit.point + transform.forward * 0.3f;
        else transform.position = idlePos.position;
    }
}
