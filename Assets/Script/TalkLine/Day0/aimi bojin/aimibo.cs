using UnityEngine;

public class Aimibo : MonoBehaviour
{
    public Character LinkObj;
    private Character character;
    private Progress progress;

    void Start()
    {
        // 只获取一次组件引用
        character = GetComponent<Character>();
        if (character != null && character.end != null)
        {
            progress = character.end.GetComponent<Progress>();
        }
    }

    void Update()
    {
        if (LinkObj != null && character != null && progress != null)
        {
            if (LinkObj.have_talk && (progress.day_num == 0 || progress.day_num == 3))
            {
                character.have_talk = true;
            }
        }
    }
}
