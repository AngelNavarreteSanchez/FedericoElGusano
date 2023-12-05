using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] List<Transform> pathPoints;
    //[SerializeField] private Animator animator;
    float speed = 3;
    float distance = 0.2f;
    byte nextpath = 0;
    private void Start()
    {
        //animator = GetComponent<Animator>();
    }
    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, pathPoints[nextpath].transform.position, speed*Time.deltaTime);
        if (Vector3.Distance(transform.position, pathPoints[nextpath].transform.position) < distance)
        {
            nextpath++;
            if (nextpath >=pathPoints.Count) nextpath = 0;
            
        }
    }

    

   


}
