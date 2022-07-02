using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_Invicibility : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] private Shader dissolveShader;

    /*bool invisibilityState=false;

    private float targetDissolveValue = 1f;
    private float currentDissolveValue = 1f;*/

    public bool IsDissolving = false; //{ get; private set; }

    private bool dissolve = false;



    // Start is called before the first frame update
    void Start()
    {
        //material = GetComponent<SkinnedMeshRenderer>().material;
        material.SetFloat("InvisibilityIntensity", 0f);
    }

    // Update is called once per frame
    void Update()
    {
       // currentDissolveValue = Mathf.Lerp(currentDissolveValue,targetDissolveValue, 2f *Time.deltaTime);

        if(!IsDissolving)     //dissolve
            {
                if(dissolve)
                {
                    IsDissolving = true;
                }
            }
        if(IsDissolving && !dissolve)
            {
                IsDissolving = false;
            }

         handleInput();
        
    }

    protected void handleInput()
    {
        dissolve = Input.GetKey(KeyCode.Q);

        Debug.Log("inside handl input");
        Debug.Log(dissolve);

        /*if(!IsDissolving)
        {
            material.SetFloat("_InvisibilityIntensity", 1.0f);
        }
        else if(IsDissolving)
        {       
             material.SetFloat("_InvisibilityIntensity", 0f);
        }*/

       /* if(Input.GetKey(KeyCode.Q))
         {
            Debug.Log("key is pressed");
            if(invisibilityState==false) //if visible
            {
                TurnInvisible();
                Debug.Log("invisibility on");
                invisibilityState=true;
            }
            else if(invisibilityState==true)   //if invisible
            {
                TurnVisible();
                Debug.Log("invisibility off");
                invisibilityState=false;
            }    
         }*/

    } 

   /* private void TurnInvisible()
    {
        material.SetFloat("_InvisibilityIntensity", 0f);
    }

    private void TurnVisible()
    {
         material.SetFloat("_InvisibilityIntensity", 1.0f);
    }*/

}
