using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductState : MonoBehaviour
{
    [SerializeField]
    private Button select;
    public bool isUnlocked;

    public void Start()
    {
        select = gameObject.GetComponent<Button>();
    }
    public void disableProduct()
    {
        if (select != null)
        {
            select.enabled = false;
        }
    }
    public void enableProduct()
    {
        if (select)
        {
            select.interactable = true;
        }
    }
}
