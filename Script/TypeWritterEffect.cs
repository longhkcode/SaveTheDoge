using System.Collections;
using UnityEngine;
using TMPro;

public class TypeWriterEffect : MonoBehaviour
{
    [SerializeField] private TMP_Text textComponent; 
    [SerializeField] private float typingSpeed = 0.05f; 

    private string fullText;
    private Coroutine typingCoroutine;

    void Awake()
    {
        if (textComponent == null)
        {
            textComponent = GetComponent<TMP_Text>();
        }

        fullText = textComponent.text; 
        textComponent.text = ""; 
    }

    void OnEnable()
    {
        // Vẫn tự động chạy chữ mặc định nếu không truyền chữ mới
        StartTyping(fullText);
    }

    // Hàm mới: Nhận một chuỗi text bất kỳ từ bên ngoài truyền vào
    public void StartTyping(string customText)
    {
        fullText = customText; // Thay đổi nội dung hiển thị

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeText());
    }

    private IEnumerator TypeText()
    {
        textComponent.text = ""; 

        foreach (char c in fullText.ToCharArray())
        {
            textComponent.text += c; 
            // Thay WaitForSeconds bằng WaitForSecondsRealtime
            yield return new WaitForSecondsRealtime(typingSpeed); 
        }
    }
}