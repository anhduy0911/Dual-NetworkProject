using System;
using UnityEngine;

public static class Helper
{
    public static Transform FindChildWithTag(this Transform parent, string tag)
    {
        foreach (Transform tr in parent)
        {
            if (tr.tag == tag)
            {
                return tr;
            }
        }
        return null;
    }
}
