using UnityEngine;

public class hold : MonoBehaviour
{
    [SerializeField] private Transform camPosition;
    [SerializeField] private GameObject hands;
    [SerializeField] private LayerMask pickable;
    public GameObject holdingObject;
    public bool isHolding;
    public float timeElapsed = 0;
    public float lerpDuration = 3f;
    public bool isChild = false;
    GameObject hitObject;
    bool pressed = false;
    void FixedUpdate()
    {
        RaycastHit hit;
        if (Input.GetKey(KeyCode.E)) { pressed = true; }
        else { pressed = false; }
        if (((Physics.Raycast(camPosition.position, camPosition.forward, out hit, 2f, pickable) && pressed && hit.transform != null && hands.transform.childCount <= 1) || (isHolding && pressed && hands.transform.childCount <= 1)) )
        {
            hitObject = hit.transform.gameObject;
            hitObject.transform.GetComponent<Rigidbody>().isKinematic = true;
            hitObject.transform.GetComponent<Rigidbody>().useGravity = false;
            hitObject.transform.SetParent(hands.transform);
            isHolding = true;
            hitObject.transform.position = Vector3.Lerp(hitObject.transform.position, hands.transform.position, 0f);
            hitObject.transform.rotation = hands.transform.rotation;
        }
        else if (isHolding)
        {
            isHolding = false;
            hitObject.transform.SetParent(null);
            hitObject.transform.GetComponent<Rigidbody>().isKinematic = false;
            hitObject.transform.GetComponent<Rigidbody>().useGravity = true;
        }

    }
}
