using UnityEngine;

public static class ObjectChecker
{
    public static void CheckNullity(Object obj, string error)
    {
        if (obj == null)
        {
            Debug.LogError(error);
        }
    }
}
