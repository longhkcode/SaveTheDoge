using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Skin Settings")]
    public List<EnemyData> allEnemySkins;

    [Header("Movement Settings")]
    public float speed = 6f;
    public float noiseIntensity = 0.2f; 
    public float acceleration = 40f;

    [Header("Collision Bounce")]
    [SerializeField] private float bounceForceMultiplier = 8f; 

    [Header("180 Degree Raycast Scanning")]
    [SerializeField] private float detectionRange = 4.0f; 
    [SerializeField] private LayerMask obstacleLayer;     
    [SerializeField] private int rayCount = 13;            
    [SerializeField] private float fovAngle = 180f;        

    private Transform dogeTransform;
    private Rigidbody2D rb;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    private Player_Controller[] allPlayers;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        LoadEquippedEnemySkin();
    }

    void Start()
    {
        if (rb != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        // Tắt tính năng Raycast tự trúng Collider của chính nó
        Physics2D.queriesStartInColliders = false;

        UpdatePlayerList();
    }

    public void UpdatePlayerList()
    {
        allPlayers = FindObjectsByType<Player_Controller>(FindObjectsSortMode.None);
    }

    private Transform GetNearestPlayer()
    {
        Player_Controller[] players =
            FindObjectsByType<Player_Controller>(FindObjectsSortMode.None);

        Transform nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (var p in players)
        {
            if (!p.gameObject.activeInHierarchy)
                continue;

            float d = Vector2.Distance(transform.position, p.transform.position);

            if (d < minDistance)
            {
                minDistance = d;
                nearest = p.transform;
            }
        }

        return nearest;
    }

    public void LoadEquippedEnemySkin()
    {
        int equippedEnemyID = PlayerPrefs.GetInt("Equipped_Enemy_Item", 0);
        if (allEnemySkins == null || allEnemySkins.Count == 0) return;

        EnemyData selectedSkin = allEnemySkins.Find(skin => skin != null && skin.ID == equippedEnemyID);
        if (selectedSkin == null) selectedSkin = allEnemySkins[0];

        if (selectedSkin != null)
        {
            if (_animator != null)
            {
                _animator.enabled = false;
                if (selectedSkin.animatorController != null)
                    _animator.runtimeAnimatorController = selectedSkin.animatorController;
            }

            if (_spriteRenderer != null && selectedSkin.enemySprite != null)
            {
                _spriteRenderer.sprite = selectedSkin.enemySprite;
            }

            if (_animator != null)
            {
                _animator.enabled = true;
                _animator.Rebind();
                _animator.Update(0f);
            }
        }
    }

    void FixedUpdate()
    {
        dogeTransform = GetNearestPlayer();
        if (dogeTransform == null) return;

        Vector2 targetDirection = (dogeTransform.position - transform.position).normalized;
        Vector2 bestDirection = FindClearDirectionInArc(targetDirection);

        Vector2 noise = Random.insideUnitCircle * noiseIntensity;
        Vector2 desiredVelocity = (bestDirection + noise).normalized * speed;

        Vector2 velocityError = desiredVelocity - rb.linearVelocity;
        Vector2 movementForce = velocityError * acceleration;
        rb.AddForce(movementForce * rb.mass);

        if (rb.linearVelocity.magnitude > speed * 1.5f)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * (speed * 1.5f);
        }

        if (rb.linearVelocity.x > 0.05f) transform.localScale = new Vector3(-1, 1, 1);
        else if (rb.linearVelocity.x < -0.05f) transform.localScale = new Vector3(1, 1, 1);
    }

    private Vector2 FindClearDirectionInArc(Vector2 targetDir)
    {
        // Quét toàn bộ 360 độ quanh ong (36 tia = mỗi tia cách nhau 10 độ)
        int fullRayCount = 36;
        float angleStep = 360f / fullRayCount;

        Vector2 bestDir = targetDir;
        float highestScore = -9999f;

        for (int i = 0; i < fullRayCount; i++)
        {
            float currentAngle = i * angleStep;
            Vector2 rayDir = Quaternion.Euler(0, 0, currentAngle) * Vector2.right;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDir, detectionRange, obstacleLayer);

            // 1. Tính khoảng trống của tia (Tối đa = 1.0)
            float openDistance = (hit.collider == null) ? detectionRange : hit.distance;
            float distanceRatio = openDistance / detectionRange; 

            // Nếu hướng này bị chặn ngay trước mặt (< 0.5m), bỏ qua luôn
            if (openDistance < 0.4f) continue;

            // 2. Tính độ đồng hướng với Doge (-1.0 đến 1.0)
            float dotToTarget = Vector2.Dot(rayDir, targetDir);

            // 3. Chấm điểm: Hướng trống được cộng điểm lớn, hướng đi về Doge được cộng thêm điểm ưu tiên
            // Bạn có thể chỉnh hệ số 1.5f để ong ưu tiên lách luật hơn hay cắm đầu vào target hơn
            float score = (distanceRatio * 2.0f) + (dotToTarget * 1.5f);

            // Debug vẽ raycast (Xanh = Trống, Đỏ = Vướng tường)
            Color rayColor = (hit.collider == null) ? Color.green : Color.red;
            Debug.DrawRay(transform.position, rayDir * openDistance, rayColor);

            if (score > highestScore)
            {
                highestScore = score;
                bestDir = rayDir;
            }
        }

        return bestDir;
    }

    // Xử lý va chạm chuẩn 100% cho cả Collision và Trigger
    private void OnCollisionStay2D(Collision2D collision)
    {
        CheckDogeCollision(collision.gameObject);
        HandleLineBounce(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        CheckDogeCollision(collision.gameObject);
    }

    private void CheckDogeCollision(GameObject obj)
    {
        Player_Controller player =
            obj.GetComponentInParent<Player_Controller>();

        if(player != null)
        {
            GameManager.Instance.LoseGame();
        }
    }
    private void HandleLineBounce(Collision2D collision)

    {
        if (collision.gameObject.name.Contains("DrawLine") || collision.gameObject.CompareTag("DrawLine"))
        {
            Rigidbody2D lineRb = collision.gameObject.GetComponent<Rigidbody2D>();
            Vector2 contactNormal = collision.GetContact(0).normal; 
        
            float calculatedForce = speed * bounceForceMultiplier;
            rb.AddForce(contactNormal * calculatedForce, ForceMode2D.Impulse);

            if (lineRb != null && lineRb.simulated)
            {
                Vector2 bounceDirection = (contactNormal + Vector2.up * 0.5f).normalized;
                lineRb.AddForce(bounceDirection * (calculatedForce * 0.5f), ForceMode2D.Impulse);
            }
        }
    }
}