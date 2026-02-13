using UnityEngine;

public class hold : MonoBehaviour
{
    [SerializeField] private Transform camPosition;
    [SerializeField] private GameObject hands;
    [SerializeField] private LayerMask pickable;
    public GameObject holdingObject;
    public bool isHolding;
    public float timeElapsed = 0;
    public float lerpDuration = 0.1f;
    public bool isChild = false;
    GameObject hitObject;
    void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(camPosition.position, camPosition.forward, out hit, 2f, pickable) && Input.GetKey(KeyCode.E))
        {
            //hit.transform.position = hands.transform.position;
            hitObject = hit.transform.gameObject;
            //Debug.Log(hitObject.name);
            hitObject.transform.GetComponent<Rigidbody>().isKinematic = true;
            hitObject.transform.SetParent(hands.transform);

            //holdingObject = hit.transform.gameObject;
            isHolding = true;
            //hit.transform.GetChild(0).position = holdingObject.transform.position;
            if (timeElapsed > lerpDuration) {
                timeElapsed += Time.deltaTime;
                float t = timeElapsed / lerpDuration;
                hitObject.transform.position = Vector3.Lerp(hit.transform.position, hands.transform.position, t);
        }
            hitObject.transform.rotation = camPosition.rotation;
        }
        else if (isHolding)
        {
            isHolding = false;

            hitObject.transform.SetParent(null);
            hitObject.transform.GetComponent<Rigidbody>().isKinematic = false;
            

            
        }

    }
}
