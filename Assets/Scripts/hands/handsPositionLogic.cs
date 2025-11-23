using UnityEngine;

public class handsPositionLogic : MonoBehaviour
{
    [SerializeField] private Transform cameraPosition;
    [SerializeField] private Transform idlePos;
    private int lenght = 1;
    public LayerMask Default;

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraPosition.position, cameraPosition.forward, out hit, lenght, Default)) transform.position = hit.point;
        else transform.position = idlePos.position;
    }
}
