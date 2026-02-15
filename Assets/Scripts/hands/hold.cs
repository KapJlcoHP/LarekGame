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
        if ((Physics.Raycast(camPosition.position, camPosition.forward, out hit, 2f, pickable) && pressed) || (isHolding && pressed))
        {

           // hit.transform.position = hands.transform.position;
            hitObject = hit.transform.gameObject;
            //Debug.Log(hitObject.name);
            hitObject.transform.GetComponent<Rigidbody>().isKinematic = true;
            hitObject.transform.SetParent(hands.transform);

            //holdingObject = hit.transform.gameObject;
            isHolding = true;
            //hit.transform.GetChild(0).position = holdingObject.transform.position;


            hitObject.transform.position = Vector3.Lerp(hitObject.transform.position, new Vector3(0,0,0), 0f);
            
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
