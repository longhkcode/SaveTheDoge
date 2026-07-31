using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    [Header("Skin Database")]
    public List<CharacterData> allCharacterSkins; 

    public float alertDistance = 4.0f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        LoadEquippedSkin();
    }

    private void Start()
    {
        // Ban đầu đóng băng vật lý để chờ người chơi vẽ xong
        SetPhysicsActive(false); 
    }

    // Hàm này giúp GameManager kích hoạt vật lý cho Doge rơi xuống khi bắt đầu tính giờ
    public void SetPhysicsActive(bool active)
    {
        if (_rb == null)
            return;

        _rb.simulated = active;

        if (active)
        {
            _rb.bodyType = RigidbodyType2D.Dynamic;
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0;
            _rb.WakeUp();
        }
    }

    public void LoadEquippedSkin()
    {
        int equippedID = PlayerPrefs.GetInt("Equipped_Item", 0);
        if (allCharacterSkins == null || allCharacterSkins.Count == 0) return;

        CharacterData selectedSkin = allCharacterSkins.Find(skin => skin != null && skin.ID == equippedID);
        if (selectedSkin == null) selectedSkin = allCharacterSkins[0];

        if (selectedSkin != null)
        {
            if (_animator != null)
            {
                _animator.enabled = false;
                if (selectedSkin.animatorController != null)
                    _animator.runtimeAnimatorController = selectedSkin.animatorController;
            }

            if (_spriteRenderer != null && selectedSkin.characterSprite != null)
            {
                _spriteRenderer.sprite = selectedSkin.characterSprite;
            }

            if (_animator != null)
            {
                _animator.enabled = true;
                _animator.Rebind();
                _animator.Update(0f);
            }
        }
    }

    private void Update()
    {
        CheckDistanceToBees();
    }

    void CheckDistanceToBees()
    {
        EnemyAI[] allBees = FindObjectsByType<EnemyAI>(FindObjectsSortMode.None);
        bool isAnyBeeTooClose = false;

        foreach (EnemyAI bee in allBees)
        {
            if (bee != null)
            {
                float distance = Vector2.Distance(transform.position, bee.transform.position);
                if (distance < alertDistance)
                {
                    isAnyBeeTooClose = true;
                    break;
                }
            }
        }

        if (_animator != null)
        {
            _animator.SetBool("SoHai", isAnyBeeTooClose);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "TrapMove")
        {
            GameManager.Instance.LoseGame();
        }
    }
}