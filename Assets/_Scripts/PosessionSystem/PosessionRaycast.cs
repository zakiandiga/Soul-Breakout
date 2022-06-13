using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosessionRaycast : MonoBehaviour
{
    [SerializeField] private Material highlightMaterial; 
    [SerializeField] private string selectableTag = "Player";
    [SerializeField] private Material defaultMaterial;
    private Transform _selection;

    private void Update()
    {
        if(_selection != null)
        {
            var selectionRenderer = _selection.GetComponent<Renderer>();
            selectionRenderer.material = defaultMaterial;
            _selection = null;
        }

        var ray = Camera.current.ScreenPointToRay(new Vector3(Screen.width/2f, Screen.height/2f, 0f));
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit))
        {
            var selection = hit.transform;
            if(selection.CompareTag(selectableTag))
            {
                 var selectionRenderer = selection.GetComponent<Renderer>();
                if(selectionRenderer != null )
                 {
                    selectionRenderer.material = highlightMaterial;
                 }

                 _selection = selection;
            }     

        }
    }
    
    private void TryPossess()
    {
        //raycast to a target direction

        //get the hit character info
        
        ///character struct consist of:
        ///character's game object (GameObject)
        ///character's properties
        ///character's currentController (enum)
        ///

        if(true)
        {
            TryPossess();
        }
    }

    private void Possess()
    {
        //character's currentController = this
    }
}
