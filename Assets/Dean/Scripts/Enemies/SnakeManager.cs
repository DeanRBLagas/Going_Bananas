using System.Collections.Generic;
using UnityEngine;

public class SnakeManager : MonoBehaviour
{
    [SerializeField] private float distanceBetween = 0.2f;
    [SerializeField] private float speed = 280;
    [SerializeField] private float turnSpeed = 18;
    [SerializeField] List<GameObject> bodyParts = new List<GameObject>();
    private List<GameObject> snakeBody = new List<GameObject>();
    private float countUp;

    private void Start()
    {
        CreateBodyParts();
    }

    private void FixedUpdate()
    {
        if (bodyParts.Count > 0)
        {
            CreateBodyParts();
        }
        SnakeMovement();
    }

    private void SnakeMovement()
    {
        snakeBody[0].GetComponent<Rigidbody2D>().linearVelocity = snakeBody[0].transform.right * speed * Time.deltaTime;

        if (snakeBody.Count > 1)
        {
            for (int i = 1; i < snakeBody.Count; i++)
            {
                MarkerManager markerM = snakeBody[i - 1].GetComponent<MarkerManager>();
                snakeBody[i].transform.position = markerM.markerList[0].position;
                snakeBody[i].transform.rotation = markerM.markerList[0].rotation;
                markerM.markerList.RemoveAt(0);
            }
        }
    }

    private void CreateBodyParts()
    {
        if (snakeBody.Count == 0)
        {
            GameObject temp1 = Instantiate(bodyParts[0], transform.position, transform.rotation, transform);
            if (!temp1.GetComponent<MarkerManager>())
            {
                temp1.AddComponent<MarkerManager>();
            }
            if (!temp1.GetComponent<Rigidbody2D>())
            {
                temp1.AddComponent<Rigidbody2D>();
                temp1.GetComponent<Rigidbody2D>().gravityScale = 0;
            }
            snakeBody.Add(temp1);
            bodyParts.RemoveAt(0);
        }
        MarkerManager markerM = snakeBody[snakeBody.Count - 1].GetComponent<MarkerManager>();
        if (countUp == 0)
        {
            markerM.ClearMarkerList();
        }
        countUp += Time.deltaTime;
        if (countUp >= distanceBetween)
        {
            GameObject temp = Instantiate(bodyParts[0], markerM.markerList[0].position, markerM.markerList[0].rotation, transform);
            if (!temp.GetComponent<MarkerManager>())
            {
                temp.AddComponent<MarkerManager>();
            }
            if (!temp.GetComponent<Rigidbody2D>())
            {
                temp.AddComponent<Rigidbody2D>();
                temp.GetComponent<Rigidbody2D>().gravityScale = 0;
            }
            snakeBody.Add(temp);
            bodyParts.RemoveAt(0);
            temp.GetComponent<MarkerManager>().ClearMarkerList();
            countUp = 0;
        }
    }
}
