
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEasyDIDecore<T>
{
    T Decore { get; set; }
    T PrevDecore { get; set; }

    public void AddDecore(T newDecore)
    {
        var oldDecore = Decore as IEasyDIDecore<T>;
        Decore = newDecore;
        (newDecore as IEasyDIDecore<T>).PrevDecore = (T)this;
        (newDecore as IEasyDIDecore<T>).Decore = (T)oldDecore;
        if (oldDecore != null)
            oldDecore.PrevDecore = newDecore;
    }

    void RemoveThisDecore()
    {
        if (PrevDecore != null)
        {
            (PrevDecore as IEasyDIDecore<T>).Decore = Decore;
            PrevDecore = default;
        }

        if (Decore != null)
        {
            (Decore as IEasyDIDecore<T>).PrevDecore = PrevDecore;
        }

    }

    T GetRoot()
    {
        var c = (T)this;
        while (c != null)
        {
            if ((c as IEasyDIDecore<T>).PrevDecore != null)
                c = (c as IEasyDIDecore<T>).PrevDecore;
            if ((c as IEasyDIDecore<T>) == this)
                break;
        }
        return c;
    }


    /// <summary>
    /// Check from root to end.
    /// </summary>
    /// <param name="onPeek"></param>
    void ForeachDecore(Action<T> onPeek)
    {
        var root = GetRoot();
        var nextCheck = root;
        var start = root;
        while (nextCheck != null)
        {
            onPeek?.Invoke(nextCheck);
            nextCheck = (nextCheck as IEasyDIDecore<T>).Decore;
            if (nextCheck.Equals(start))//tranh loop
            {
                EasyDI.EasyDILog.LogError($"Loop decorator detected!");
                break;
            }
        }

    }

    IList<T> ToListDecore()
    {
        IList<T> list = new List<T>();
        ForeachDecore(_ => list.Add(_));
        return list;
    }
}
