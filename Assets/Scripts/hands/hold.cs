using UnityEngine;

public class hold : MonoBehaviour
{
    [SerializeField] private Transform camPosition;
    [SerializeField] private GameObject hands;
    [SerializeField] private LayerMask pickable;
    GameObject holdingObject;
    bool isHolding;

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(camPosition.position, camPosition.forward, out hit, 2f, pickable) && Input.GetKey(KeyCode.E))
        {
            hit.transform.position = hands.transform.position;
            hit.transform.SetParent(hands.transform);
            hit.transform.GetComponent<Rigidbody>().isKinematic = true;
            holdingObject = hit.transform.gameObject;
            isHolding = true;
            holdingObject.transform.rotation = camPosition.rotation;
        }
        else if (isHolding)
        {
            isHolding = false;
            holdingObject.transform.SetParent(null);
            holdingObject.transform.GetComponent<Rigidbody>().isKinematic = false;
        }
    }
}
