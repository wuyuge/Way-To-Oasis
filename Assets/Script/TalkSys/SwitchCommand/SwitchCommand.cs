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
        A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,
        Aa, Ab, Ba, Bb, Ca, Cb, Da, Db, Ea, Eb, Fa, Fb, Ga, Gb, 
        Ha, Hb, Ia, Ib, Ja, Jb, Ka, Kb, La, Lb, Ma, Mb, Na, Nb, 
        Oa, Ob, Pa, Pb, Qa, Qb, Ra, Rb, Sa, Sb, Ta, Tb, Ua, Ub, 
        Va, Vb, Wa, Wb, Xa, Xb, Ya, Yb, Za, Zb
    }
}
