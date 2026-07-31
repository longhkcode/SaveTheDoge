using UnityEngine;
using System.Collections.Generic;

public class LineController : MonoBehaviour
{
    private LineRenderer line;
    private EdgeCollider2D edge;
    private Rigidbody2D rb;

    [SerializeField]
    private List<Vector2> localPoints = new();

    private Vector2 oldPoint;

    [Header("System")]
    [SerializeField]
    private List<Script.SpamEnemy> spamEnemyScripts = new();

    bool spawned = false;
    bool drawing = false;

    // Đã vẽ hay chưa
    private bool hasDrawn = false;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        edge = GetComponent<EdgeCollider2D>();
        rb = GetComponent<Rigidbody2D>();

        rb.simulated = false;

        line.useWorldSpace = false;
        line.positionCount = 0;
    }

    void Update()
    {
        DrawLine();

        if (rb.simulated && localPoints.Count > 1)
        {
            edge.SetPoints(localPoints);
        }
    }

    void DrawLine()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Nếu đã vẽ rồi thì không cho vẽ nữa
            if (hasDrawn)
                return;

            drawing = true;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.PauseGameForDrawing();
            }

            rb.simulated = false;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0;

            transform.position = Vector3.zero;

            localPoints.Clear();
            line.positionCount = 0;
            edge.SetPoints(new List<Vector2>());

            oldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (drawing && Input.GetMouseButton(0))
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (Vector2.Distance(worldPoint, oldPoint) < 0.12f)
                return;

            oldPoint = worldPoint;

            Vector2 local = transform.InverseTransformPoint(worldPoint);

            localPoints.Add(local);

            line.positionCount = localPoints.Count;

            for (int i = 0; i < localPoints.Count; i++)
            {
                line.SetPosition(i, localPoints[i]);
            }

            if (localPoints.Count > 1)
                edge.SetPoints(localPoints);
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (!drawing)
                return;

            drawing = false;

            if (localPoints.Count < 2)
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.ResumeGameAfterDrawing();
                }
                return;
            }

            // Đánh dấu đã vẽ xong
            hasDrawn = true;

            rb.simulated = true;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.ResumeGameAfterDrawing();
                GameManager.Instance.StartCountdown();
            }

            if (!spawned)
            {
                spawned = true;

                if (spamEnemyScripts != null && spamEnemyScripts.Count > 0)
                {
                    foreach (var spawner in spamEnemyScripts)
                    {
                        if (spawner != null)
                        {
                            spawner.StartSpawning();
                        }
                    }
                }
            }
        }
    }
}