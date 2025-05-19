using System.Collections.Generic;
using UnityEngine;

public class SnakeManager : MonoBehaviour
{
    [SerializeField] private float distanceBetween = 0.2f;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float turnInterval = 2f;
    [SerializeField] List<GameObject> bodyParts = new List<GameObject>();
    private List<GameObject> snakeBody = new List<GameObject>();
    private float countUp;
    private float turnTimer;

    private Vector2 currentDirection = Vector2.right;

    private void Start()
    {
        CreateBodyParts();
        turnTimer = turnInterval;
    }

    private void FixedUpdate()
    {
        if (bodyParts.Count > 0)
        {
            CreateBodyParts();
        }

        turnTimer -= Time.deltaTime;
        if (turnTimer <= 0)
        {
            ChangeDirection();
        }

        SnakeMovement();
    }

    private void SnakeMovement()
    {
        snakeBody[0].GetComponent<Rigidbody2D>().linearVelocity = currentDirection * speed;

        if (snakeBody.Count > 1)
        {
            for (int i = 1; i < snakeBody.Count; i++)
            {
                MarkerManager markerM = snakeBody[i - 1].GetComponent<MarkerManager>();
                if (markerM.markerList.Count > 0)
                {
                    snakeBody[i].transform.position = markerM.markerList[0].position;
                    snakeBody[i].transform.rotation = markerM.markerList[0].rotation;
                    markerM.markerList.RemoveAt(0);
                }
            }
        }
    }

    private void CreateBodyParts()
    {
        if (snakeBody.Count == 0)
        {
            GameObject temp1 = Instantiate(bodyParts[0], transform.position, transform.rotation, transform);
            SnakeHead head = temp1.GetComponent<SnakeHead>();
            head.manager = this;
            SetupBodyPart(temp1);
            snakeBody.Add(temp1);
            bodyParts.RemoveAt(0);
        }

        MarkerManager markerM = snakeBody[snakeBody.Count - 1].GetComponent<MarkerManager>();
        if (countUp == 0)
        {
            markerM.ClearMarkerList();
        }

        countUp += Time.deltaTime;
        if (countUp >= distanceBetween && bodyParts.Count > 0 && markerM.markerList.Count > 0)
        {
            GameObject temp = Instantiate(bodyParts[0], markerM.markerList[0].position, markerM.markerList[0].rotation, transform);
            SetupBodyPart(temp);
            snakeBody.Add(temp);
            bodyParts.RemoveAt(0);
            temp.GetComponent<MarkerManager>().ClearMarkerList();
            countUp = 0;
        }
    }

    private void SetupBodyPart(GameObject part)
    {
        if (!part.GetComponent<MarkerManager>())
            part.AddComponent<MarkerManager>();

        Rigidbody2D rb = part.GetComponent<Rigidbody2D>();
        if (!rb)
        {
            rb = part.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
        }
        rb.freezeRotation = true;
    }

    public void ChangeDirection()
    {
        turnTimer = turnInterval;
        if (currentDirection == Vector2.right || currentDirection == Vector2.left)
        {
            currentDirection = (Random.value > 0.5f) ? Vector2.up : Vector2.down;
        }
        else
        {
            currentDirection = (Random.value > 0.5f) ? Vector2.right : Vector2.left;
        }

        float angle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;
        snakeBody[0].transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
