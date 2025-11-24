using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QteButtonManager : MonoBehaviour
{
    public Image remainingImage;
    public float decreaseSpeed;
    private KeyCode _needKey;
    public TextMeshProUGUI keyText;
    private Image _selfImage;
    private bool Unlocked;

    private void Awake()
    {
        decreaseSpeed = decreaseSpeed / 60;
        _selfImage = GetComponent<Image>();
    }


    private void OnEnable()
    {
        Unlocked = false;
        remainingImage.fillAmount = 1f;
        _selfImage.color = Color.white;
        remainingImage.color = Color.green;
        RandomInput();
    }


    private void FixedUpdate()
    {
        if (remainingImage.fillAmount > 0 && !Unlocked)
        {
            remainingImage.fillAmount -= decreaseSpeed;
            if (remainingImage.fillAmount <= 0)
            {
                _selfImage.color = Color.red;
            }
        }
        
        
    }

    private void Update()
    {
        Unlock();
    }


    private void Unlock()
    {
        if (Input.GetKeyDown(_needKey))
        {
            Unlocked = true;
        }
    }

    void RandomInput()
    {
        int randomCode = UnityEngine.Random.Range(0, 4);
        switch (randomCode)
        {
            case 0:
                _needKey = KeyCode.W;
                keyText.text = "W";
                break;
            case 1:
                _needKey = KeyCode.Q;
                keyText.text = "Q";
                break;
            case 2:
                _needKey = KeyCode.E;
                keyText.text = "E";
                break;
            case 3:
                _needKey = KeyCode.R;
                keyText.text = "R";
                break;
            
        }
        
        
        
    }


}
