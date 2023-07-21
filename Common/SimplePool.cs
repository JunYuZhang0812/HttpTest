using System;
using System.Collections.Generic;

public class SimplePool<T> where T : class
{
    public static Func<T> CreateInstance;

    public static Action<T> CtorFunc;

    public static Action<T> DtorFunc;

    public static Action<T> ReleaseFunc;

    public static Func<T, int> ElementSizeFunc;

    private static SimplePool<T> _sharedInstance;

    private List<T> m_stack = new List<T>();

    public static SimplePool<T> sharedInstance => _sharedInstance;

    static SimplePool()
    {
        CreateInstance = null;
        CtorFunc = null;
        DtorFunc = null;
        ReleaseFunc = null;
        ElementSizeFunc = null;
        _sharedInstance = null;
        _sharedInstance = new SimplePool<T>();
        SimplePoolSweeper.Register(delegate
        {
            _sharedInstance.Sweep();
        });
    }

    public void Sweep()
    {
        try
        {
            if (ReleaseFunc != null)
            {
                for (int i = 0; i < m_stack.Count; i++)
                {
                    ReleaseFunc(m_stack[i]);
                }
            }
        }
        finally
        {
            m_stack.Clear();
            m_stack.TrimExcess();
        }
    }

    public void TrimExcess()
    {
        m_stack.TrimExcess();
    }

    public T Allocate()
    {
        if (m_stack.Count == 0)
        {
            T val = null;
            if (CreateInstance == null)
            {
                val = Activator.CreateInstance(typeof(T)) as T;
                UDebug.LogError("SimplePool: there is no public parameterless ctor. you should specify a ctor for this pool.");
            }
            else
            {
                val = CreateInstance();
            }

            m_stack.Add(val);
        }

        int index = m_stack.Count - 1;
        T val2 = m_stack[index];
        m_stack.RemoveAt(index);
        if (CtorFunc != null)
        {
            CtorFunc(val2);
        }

        return val2;
    }

    public void Free(ref T o)
    {
        if (o != null)
        {
            if (DtorFunc != null)
            {
                DtorFunc(o);
            }

            m_stack.Add(o);
            o = null;
        }
    }

    public int GetTotalSize()
    {
        int num = 0;
        if (ElementSizeFunc != null)
        {
            for (int i = 0; i < m_stack.Count; i++)
            {
                num += ElementSizeFunc(m_stack[i]);
            }
        }

        return num;
    }

    public void Preload(int count)
    {
        int num = m_stack.Count + count;
        if (num > m_stack.Capacity)
        {
            m_stack.Capacity = num;
        }

        if (CreateInstance != null)
        {
            for (int i = 0; i < count; i++)
            {
                T val = CreateInstance();
                if (val != null)
                {
                    m_stack.Add(val);
                }
            }

            return;
        }

        UDebug.LogError("SimplePool: there is no public parameterless ctor. you should specify a ctor for this pool.");
        for (int j = 0; j < count; j++)
        {
            T val2 = Activator.CreateInstance(typeof(T)) as T;
            if (val2 != null)
            {
                m_stack.Add(val2);
            }
        }
    }
}
