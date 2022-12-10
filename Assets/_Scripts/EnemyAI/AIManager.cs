using UnityEngine;

public class AIManager : MonoBehaviour
{
    public Vector3 truePlayerPosition;

    public Vector3 WhereIsPlayer(Vector3 playerPosition)
    {

        bool isEqual = playerPosition.Equals(new Vector3 (0f,0f,0f));  //if equals, then enemy

        if(!isEqual)   //for player
        {
            truePlayerPosition = playerPosition;
            return new Vector3 (0f,0f,0f);
        }
        //for enemy
        
        return truePlayerPosition;         

    }
}
