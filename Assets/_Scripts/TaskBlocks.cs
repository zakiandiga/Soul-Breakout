using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskBlocks : MonoBehaviour
{
    public GameObject taskBlockPrefab;
    [SerializeField] private List<Transform> TBpoints;

    //private List<Transform> visitedTBpoints;

    private int pointNum=0;
    private int selectRange;

    private int blockCode=0;

    [SerializeField] private int numOfChar=5;


    // Start is called before the first frame update
    void Awake()
    {
        selectRange = TBpoints.Count-1;


        for(int i=0; i<numOfChar; i++)
        {
            taskBlockPrefab.GetComponent<TaskFill>().blockCharacterCode = blockCode;

            for(int j=0; j<2; j++)   //2 blocks each character
            {
           
                pointNum = Random.Range(0, selectRange);


                Instantiate(taskBlockPrefab, TBpoints[pointNum].position, Quaternion.identity);  //instantiate the block in a random location from TBpoints


                //switch the used taskblock point to last

                Transform temp = TBpoints[selectRange];    
                TBpoints[selectRange] = TBpoints[pointNum];
                TBpoints[pointNum] = temp;
                

                selectRange--;
                
            }
            blockCode++;
            

        }

        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
