using UnityEngine;

public class InvisibleWall : MonoBehaviour
{

    //Variables
    public bool hideWall;
    private MeshRenderer meshRenderer;

    //Variables


    void Start()
    {

        meshRenderer = GetComponent<MeshRenderer>();



        if (hideWall) 
        {
            Debug.Log(" Wall Hidden State " + hideWall);

            meshRenderer.enabled = true;


        }
        else
        {
             Debug.Log(" Wall Hidden State " + hideWall);
             meshRenderer.enabled = false; 


        }
    }

    

}
