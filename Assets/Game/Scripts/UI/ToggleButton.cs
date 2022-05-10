using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleButton : MonoBehaviour
{
    [SerializeField] GameObject itemToToggle = null;

    // Start is called before the first frame update
    void Start()
    {
        itemToToggle.SetActive(false);
    }

    public void Toggle()
    {
        if (itemToToggle.activeSelf)
        {
            itemToToggle.SetActive(false);
        }

        else
        {
            itemToToggle.SetActive(true);
        }
    }
    
}
