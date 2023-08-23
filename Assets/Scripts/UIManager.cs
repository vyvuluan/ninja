using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<UIManager>();
            }
            return instance;
        }

    }
    [SerializeField] private TextMeshProUGUI coinText;

    public void SetCoin(int coin)
    {
        coinText.text = coin.ToString();
    }
}
