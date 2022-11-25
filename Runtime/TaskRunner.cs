using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KTaskManager
{
    internal class TaskRunner : MonoBehaviour
    {
        internal static TaskRunner instance = null;
        private void Awake()
        {
            instance = this;
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            TaskManager.AbortAll();
            TaskManager.tasks = new List<TaskHandle>();
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
            TaskManager.AbortAll();
            TaskManager.tasks = new List<TaskHandle>();
        }
    }
}