using System.Collections;
using TMPro;
using UnityEngine;

public class AutoObjectNumUpdater : MonoBehaviour
{
    public Manager manager1,manager2;
    public ObjectManager objectManager;
    private TextMeshProUGUI _text;
    public float updateRate;
    private bool ShowSecond => objectManager.progress.start;
    private Coroutine _coroutine;

    private void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _coroutine = StartCoroutine(TextUpdate());
    }

    private IEnumerator TextUpdate()
    {
        if (!ShowSecond)
        {
            _text.text = manager1.Weight.ToString();
        }
        else
        {
            _text.text = manager2.Weight.ToString();
        }

        yield return new WaitForSecondsRealtime(updateRate);
    }
    
}
