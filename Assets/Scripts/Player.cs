using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    List<GameObject> selectedObjects;

    void Select(GameObject go)
    {
        selectedObjects.Add(go);
    }

    void Awake()
    {
        selectedObjects = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "Tile")
                {
                    Debug.Log("hit Tile: " + hit.collider.GetComponent<HexTile>().ToString());
                    Select(hit.collider.gameObject);
                }
                else
                {
                    Debug.Log("This isn't a Tile");
                }
            }
        }
    }
}
