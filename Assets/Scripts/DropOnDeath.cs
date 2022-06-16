using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropOnDeath : MonoBehaviour
{

    public GameObject[] DropTable;
    public void Drop()
    {
        //Dropping stuff
        float choice = Random.Range(0, 1);
        if (choice < 0.33) { 
            //Drop nothing
        }
        if (choice > 0.33 && choice < 0.66)
        {
            //Option 1
            Instantiate(DropTable[0], transform.position, Quaternion.identity);
        }
        else {
            //Option 2
            Instantiate(DropTable[1], transform.position, Quaternion.identity);
        }

    }
}
