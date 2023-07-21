using System;
using System.Collections.Generic;

public static class SimplePoolSweeper
{
    private static List<Action> sweepActionList = new List<Action>();

    public static void Sweep()
    {
        for (int i = 0; i < sweepActionList.Count; i++)
        {
            sweepActionList[i]();
        }
    }

    public static void Register(Action sweepAction)
    {
        sweepActionList.Add(sweepAction);
    }
}