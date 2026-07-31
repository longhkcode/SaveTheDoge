using UnityEngine;

public class TrapController : MonoBehaviour
{
    // Nếu game của bạn là 2D (Hầu hết game Save The Doge là 2D)
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra nếu đối tượng va chạm có Tag là "Player"
        if (other.CompareTag("Player")) 
        {
            // Gọi thẳng qua Singleton Instance, không cần kéo thả biến ở Inspector nữa
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoseGame();
            }
        }
    }
}