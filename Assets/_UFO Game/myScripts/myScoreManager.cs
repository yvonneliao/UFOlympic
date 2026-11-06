using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class myScoreManager : MonoBehaviour
{
    public static myScoreManager _Instance { get; private set; }

    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private GameObject _gameManager;
    [SerializeField] private int _score = 0;

    private void Awake()
    {
        if (_Instance != null && _Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _Instance = this;

        if (_gameManager == null)
        {
            _gameManager = GameObject.Find("Game Manager");
        }

        _UpdateUI();
    }

    public static void _AddScore(int _amount)
    {
        if (_Instance != null)
        {
            _Instance._ModifyScore(_amount);
        }
    }

    public void _ModifyScore(int _amount)
    {
        _score += _amount;
        _UpdateUI();

        if (_score < 0)
        {
            if (_gameManager != null)
            {
                CustomEvent.Trigger(_gameManager, "GameOver", false);
            }
        }
    }

    private void _UpdateUI()
    {
        if (_scoreText != null)
        {
            _scoreText.text = $"SCORE: {_score}";
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            _UpdateUI();
            if (_score < 0)
            {
                if (_gameManager != null)
                {
                    CustomEvent.Trigger(_gameManager, "GameOver", false);
                }
            }
        }
    }

    [ContextMenu("Test Add -50 Points")]
    private void _ContextMenuSubtract50()
    {
        _ModifyScore(-50);
    }
#endif
}
