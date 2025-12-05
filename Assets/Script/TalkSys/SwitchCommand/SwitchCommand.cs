using System;
using UnityEngine;

public abstract class SwitchCommand:MonoBehaviour
{
    
    public abstract void Execute(FunctionCode.Function function);
    public abstract void Init(TalkSystem talkSys);
    
    
}

public static class FunctionCode
{
    public enum Function
    {
        A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z
    }
}
