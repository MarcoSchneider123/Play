﻿using System.Collections;
using UnityEditor;

/**
 * Normally Unity executes coroutines in Edit Mode only when the scene changes.
 * This script adds a Editor callback to explicitly update the coroutines that have been added to the CoroutineManager.
 * This works also in Edit Mode.
 */
[InitializeOnLoad]
public static class EditorCoroutineManager
{
    private static CoroutineManager coroutineManager;
    private static int currentExecute;

    static EditorCoroutineManager()
    {
        EditorApplication.update += ExecuteCoroutines;
    }

    private static void ExecuteCoroutines()
    {
        // This method is executed multiple times per second.
        // Thus, do not perform slow stuff in here, or Unity Editor will have bad performance.
        if (coroutineManager == null)
        {
            coroutineManager = CoroutineManager.Instance;
        }

        if (coroutineManager == null
            || coroutineManager.CoroutinesInProgress.Count <= 0)
        {
            // No coroutines
            return;
        }

        currentExecute = (currentExecute + 1) % coroutineManager.CoroutinesInProgress.Count;

        IEnumerator coroutine = coroutineManager.CoroutinesInProgress[currentExecute];
        bool finish = (coroutine == null) || !coroutine.MoveNext();
        if (finish)
        {
            coroutineManager.CoroutinesInProgress.RemoveAt(currentExecute);
        }
    }
}
