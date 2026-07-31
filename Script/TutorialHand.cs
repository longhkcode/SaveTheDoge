using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialHand : MonoBehaviour
{
    [Header("Cấu hình Hướng dẫn")]
    [Tooltip("Danh sách các điểm (Transforms) ngón tay sẽ di chuyển qua")]
    [SerializeField] private Transform[] waypoints;

    [Tooltip("Nối từ điểm cuối cùng về lại điểm đầu tiên để tạo khung kín")]
    [SerializeField] private bool loopBackToStart = true;

    [Tooltip("Tốc độ di chuyển của ngón tay")]
    [SerializeField] private float moveSpeed = 4f;

    [Tooltip("Thời gian chờ khi ngón tay vẽ xong 1 hình trước khi lặp lại")]
    [SerializeField] private float delayBetweenLoops = 0.8f;

    [Header("Tham chiếu Component")]
    [SerializeField] private LineRenderer hintLine; // LineRenderer tạo đường mờ hướng dẫn

    private bool _isTutorialActive = true;

    void Start()
    {
        if (hintLine == null)
        {
            hintLine = GetComponent<LineRenderer>();
        }

        if (waypoints == null || waypoints.Length < 2)
        {
            Debug.LogWarning("TutorialHand: Cần ít nhất 2 Waypoints để vẽ hướng dẫn!");
            gameObject.SetActive(false);
            return;
        }

        StartCoroutine(FollowWaypointsRoutine());
    }

    void Update()
    {
        // Khi người chơi nhấn chuột trái (bắt đầu vẽ) -> Ẩn ngón tay & đường mờ ngay lập tức
        if (_isTutorialActive && Input.GetMouseButtonDown(0))
        {
            StopTutorial();
        }
    }

    private IEnumerator FollowWaypointsRoutine()
    {
        while (_isTutorialActive)
        {
            // 1. Reset đường vẽ và đặt tay về điểm đầu tiên
            if (hintLine != null)
            {
                hintLine.positionCount = 0;
            }

            transform.position = waypoints[0].position;

            if (hintLine != null)
            {
                hintLine.positionCount = 1;
                hintLine.SetPosition(0, waypoints[0].position);
            }

            // 2. Chạy qua từng điểm waypoint (từ pos1 -> pos8)
            for (int i = 1; i < waypoints.Length; i++)
            {
                yield return StartCoroutine(MoveToTarget(waypoints[i].position));
            }

            // 3. NẾU BẬT KHÉP KÍN: Chạy tiếp 1 đoạn từ điểm cuối về lại điểm đầu (pos0)
            if (loopBackToStart)
            {
                yield return StartCoroutine(MoveToTarget(waypoints[0].position));
            }

            // 4. Nghỉ một chút khi đã vẽ xong khung khép kín rồi mới reset lại
            yield return new WaitForSeconds(delayBetweenLoops);
        }
    }

    // Hàm phụ trách di chuyển bàn tay và kéo theo đường LineRenderer
    private IEnumerator MoveToTarget(Vector3 targetPos)
    {
        if (hintLine != null)
        {
            hintLine.positionCount++;
        }

        while (Vector3.Distance(transform.position, targetPos) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            if (hintLine != null)
            {
                hintLine.SetPosition(hintLine.positionCount - 1, transform.position);
            }

            yield return null;
        }

        transform.position = targetPos;
        if (hintLine != null)
        {
            hintLine.SetPosition(hintLine.positionCount - 1, targetPos);
        }
    }

    private void StopTutorial()
    {
        _isTutorialActive = false;
        StopAllCoroutines();

        if (hintLine != null)
        {
            hintLine.positionCount = 0;
        }

        gameObject.SetActive(false);
    }
}