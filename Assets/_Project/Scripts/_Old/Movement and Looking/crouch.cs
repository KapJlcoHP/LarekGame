using UnityEngine;

public class crouch : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    bool isCrouching;
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && !isCrouching)
        {
            playerTransform.localScale -= new Vector3(0, 0.5f, 0);
            playerTransform.position -= new Vector3(0, 0.5f, 0);
            isCrouching = true;
        }
        if(!Input.GetKey(KeyCode.LeftControl) && isCrouching)
        {
            isCrouching = false;
            playerTransform.localScale += new Vector3(0, 0.5f, 0);
            playerTransform.position += new Vector3(0, 1f, 0);
        }
    }
}
