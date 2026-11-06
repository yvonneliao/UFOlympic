using System.Collections;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class myScoreManager : MonoBehaviour
{
    public static myScoreManager _instance { get; private set; }

    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private GameObject _gameManager;
    [SerializeField] private GameObject _deductionAlert;
    [SerializeField] private int _score = 0;

    private int _deductionCount = 0;
    private Coroutine _alertCoroutine;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        if (_gameManager == null)
        {
            _gameManager = GameObject.Find("Game Manager");
        }

        if (_deductionAlert != null)
        {
            _deductionAlert.SetActive(false);
        }

        _updateUI();
    }

    public static void _addScore(int _amount)
    {
        if (_instance != null)
        {
            _instance._modifyScore(_amount);
        }
    }

    public void _modifyScore(int _amount)
    {
        _score += _amount;
        _updateUI();

        if (_amount < 0 && _deductionCount < 100)
        {
            _deductionCount++;
            if (_deductionAlert != null)
            {
                if (_alertCoroutine != null)
                {
                    StopCoroutine(_alertCoroutine);
                }
                _alertCoroutine = StartCoroutine(_blinkAlertRoutine());
            }
        }

        if (_score < 0)
        {
            if (_gameManager != null)
            {
                CustomEvent.Trigger(_gameManager, "GameOver", false);
            }
        }
    }

    private IEnumerator _blinkAlertRoutine()
    {
        float _blinkInterval = 0.16f;

        // Blink twice quickly
        for (int i = 0; i < 2; i++)
        {
            _deductionAlert.SetActive(true);
            yield return new WaitForSeconds(_blinkInterval);
            _deductionAlert.SetActive(false);
            yield return new WaitForSeconds(_blinkInterval);
        }

        // Hold visible
        _deductionAlert.SetActive(true);
        yield return new WaitForSeconds(3.0f);

        // Disappear
        _deductionAlert.SetActive(false);
    }


    private void _updateUI()
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
            _updateUI();
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
    private void _contextMenuSubtract50()
    {
        _modifyScore(-50);
    }
#endif
}
