using UnityEngine;

public class Aimibo : MonoBehaviour
{
    public Character LinkObj;
    private Character character;
    public Progress progress;
    public bool InfoIsOn = false;
    private bool ShowBo = false;
    private CharacterInfoManager _infoManager;
    private bool _head = false;

    void Start()
    {
        // 只获取一次组件引用
        character = GetComponent<Character>();
        progress = GlobalData.Progress;
        if (_infoManager is null && GlobalData.Day == 0)
        {
            _infoManager = GameObject.Find("CharacterInfo")?.GetComponent<CharacterInfoManager>();
        }
    }

    void Update()
    {
        if (LinkObj != null && character != null && progress != null)
        {
            if (progress.day_num == 0 || progress.day_num == 3)
            {
                if (LinkObj.have_talk)
                {
                    character.have_talk = true;
                }
            }
        }

        if (GlobalData.Day != 0)
        {
            return;
        }
        if (InfoIsOn && (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Space))) 
        {
            if(!ShowBo && _head)
            {
                _infoManager.CloseInfo();
                _infoManager.ShowInfo("艾米莉");
                Debug.Log("展示艾米莉");
                ShowBo = true;
            }
            else if (!ShowBo && !_head)
            {
                _infoManager.CloseInfo();
                _infoManager.ShowInfo("博金森");
                Debug.Log("展示博金森");
                ShowBo = true;
            }
            else
            {
                gameObject.GetComponent<Character>().ShowInfo = true;
                InfoIsOn = false;
                Debug.Log("展示无");
            }

        }



    }


    public void ShowInfo(string Name)
    {
        if (Name == "博金森")
        {
            _head = true;
        }
        _infoManager.ShowInfo(Name);
        InfoIsOn = true;
        ShowBo = false;
    }




}
