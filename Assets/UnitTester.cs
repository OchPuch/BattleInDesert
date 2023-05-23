using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTester : MonoBehaviour
{
    public GameObject unit;
    
    public void SpawnUnit()
    {
        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Instantiate(unit, pos, Quaternion.identity);
    }
}
