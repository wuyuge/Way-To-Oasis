using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CommandCode
{
    [Tooltip("命令名")]
    public string command;
    [SerializeReference]
    [Tooltip("执行的子分支列表")]
    public List<SwitchCommand> actions;
    [Tooltip("执行的对应函数")]
    public List<FunctionCode.Function> functions;
}


public class TalkSysSwitch : MonoBehaviour,ITalkSysCore
{
    private TalkSystem _talkSys;
    private List<Manager> _talkLines;
    private int DayNum => _talkSys?.Daytime ?? 0;
    private int Line => _talkSys.line;
    [SerializeField]
    [Header("命令列表")]
    private List<CommandCode> switchCodes;
    [SerializeField]
    [Header("挂载的子命令分支")]
    private List<SwitchCommand> functionList;

    private void Start()
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            functionList.Add(gameObject.transform.GetChild(i).GetComponent<SwitchCommand>());
        }
    }

    /// <summary>
    /// 初始化TalkSysSwitch对象，设置对话系统和对话行列表，并重置当前行索引。
    /// </summary>
    /// <param name="talkSys">对话系统的实例，包含所有对话数据和配置。</param>
    public void Init(TalkSystem talkSys)
    {
        _talkSys = talkSys;
       _talkLines = talkSys.Talklines;
       /*if (switchCodes != null)
       {
           foreach (var value in switchCodes)
           {
               foreach (var code in value.actions)
               {
                   try
                   {
                       code.Init(_talkSys);
                   }
                   catch (Exception e)
                   {
                       Debug.LogError($"actions脚本为空 错误类型{e}");
                   }
                   
               }
           } 
       }*/

       foreach (var VARIABLE in functionList)
       {
           if (talkSys != null)
           {
               VARIABLE.Init(talkSys);
           }
           
       }
       
    }

    /// <summary>
    /// 执行当前对话行中的命令代码，根据对话文本中的特殊标记进行相应的UI或逻辑处理。
    /// </summary>
    public void DoSwitchCode()
    {
        //命令命名规范  ${命令}
        try
        {
            string curText = _talkLines[DayNum].TxtLine[Line];
            if (curText.Contains("DownTalkBox"))
            {
                _talkSys.showText.historyManager.Refresh();
            }
            curText = curText.Replace("$", "");
            curText = curText.Replace("{", "");
            curText = curText.Replace("}", "");
            Debug.Log(curText);
            bool isExist = false;
            foreach (var codes in switchCodes)
            {
                
                if (codes.command == curText)
                {
                    isExist = true;
                    for (var i = 0; i < codes.actions.Count; i++)
                    {
                        try
                        {
                            codes.actions[i].Execute(codes.functions[i]);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"错误指令{curText}  索引{i} 错误类型{e}");
                            return;
                        }
                    }
                }
  
            }
            if (!isExist)
            {
                Debug.LogError($"标识命令不存在{curText}");
            }
            _talkSys.line++;
            if (_talkSys.line < _talkLines[DayNum].TxtLine.Count && 
                _talkLines[DayNum].TxtLine[_talkSys.line].Contains("$"))
            {
                
                DoSwitchCode();
            }

        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

 

    
    
}
