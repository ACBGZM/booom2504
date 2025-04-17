using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskList : MonoBehaviour
{
    [SerializeField] private GameObject orderTemplatePrefab;
    [SerializeField] private Transform contentPanel;


    void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject orderTemplatePrefab = Instantiate(this.orderTemplatePrefab, contentPanel);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
