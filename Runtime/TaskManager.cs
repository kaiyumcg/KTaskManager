using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KTaskManager
{
    public static class TaskManager
    {
        internal static List<TaskHandle> tasks = null;
#if UNITY_EDITOR
        public static List<TaskHandle> Tasks { get { return tasks; } }
#endif

        public static TaskHandle CreatePerFrameTask(string taskName, IEnumerator taskRoutine,
            TaskCondition pauseCondition = null, TaskCondition exitCondition = null)
        {
            if (tasks == null) { tasks = new List<TaskHandle>(); }
            var task = new TaskHandle(taskName, tasks.Count + 1, taskRoutine, pauseCondition, exitCondition);
            tasks.Add(task);
            return task;
        }

        public static TaskHandle CreatePerFrameTask(string taskName, SyncTask syncTask,
           TaskCondition pauseCondition = null, TaskCondition exitCondition = null)
        {
            if (tasks == null) { tasks = new List<TaskHandle>(); }
            var task = new TaskHandle(taskName, tasks.Count + 1, syncTask, pauseCondition, exitCondition);
            tasks.Add(task);
            return task;
        }

        public static TaskHandle CreateOneTimeTask(string taskName, IEnumerator taskRoutine)
        {
            if (tasks == null) { tasks = new List<TaskHandle>(); }
            var task = new TaskHandle(taskName, tasks.Count + 1, taskRoutine);
            tasks.Add(task);
            return task;
        }

        public static TaskHandle CreateOneTimeTask(string taskName, SyncTask syncTask)
        {
            if (tasks == null) { tasks = new List<TaskHandle>(); }
            var task = new TaskHandle(taskName, tasks.Count + 1, syncTask);
            tasks.Add(task);
            return task;
        }

        public static TaskHandle GetTaskByName(string taskName)
        {
            TaskHandle handle = null;
            if (string.IsNullOrEmpty(taskName) == false && tasks != null && tasks.Count > 0)
            {
                for (int i = 0; i < tasks.Count; i++)
                {
                    var t = tasks[i];
                    if (t == null) { continue; }
                    if (t.TaskName == taskName)
                    {
                        handle = t;
                        break;
                    }
                }
            }
            return handle;
        }

        public static TaskHandle GetTaskByID(int taskID)
        {
            TaskHandle handle = null;
            if (taskID > 0 && tasks != null && tasks.Count > 0)
            {
                for (int i = 0; i < tasks.Count; i++)
                {
                    var t = tasks[i];
                    if (t == null) { continue; }
                    if (t.TaskID == taskID)
                    {
                        handle = t;
                        break;
                    }
                }
            }
            return handle;
        }

        public static void Abort(string taskName)
        {
            var task = GetTaskByName(taskName);
            if (task != null)
            {
                task.StopTaskIfApplicable();
                if (tasks != null && tasks.Contains(task)) 
                {
                    tasks.Remove(task);
                }
            }
        }

        public static void AbortAll()
        {
            if (tasks != null && tasks.Count > 0)
            {
                for (int i = 0; i < tasks.Count; i++)
                {
                    var task = tasks[i];
                    Abort(task.TaskName);
                }
            }
        }

        public static void Abort(params string[] taskNames)
        {
            if (taskNames != null && taskNames.Length > 0)
            {
                for (int i = 0; i < taskNames.Length; i++)
                {
                    var tName = taskNames[i];
                    if (string.IsNullOrEmpty(tName)) { continue; }
                    Abort(tName);
                }
            }
        }

        public static void Abort(int taskID)
        {
            var task = GetTaskByID(taskID);
            if (task != null)
            {
                task.StopTaskIfApplicable();
                if (tasks != null && tasks.Contains(task))
                {
                    tasks.Remove(task);
                }
            }
        }

        public static void Abort(params int[] taskIDs)
        {
            if (taskIDs != null && taskIDs.Length > 0)
            {
                for (int i = 0; i < taskIDs.Length; i++)
                {
                    var tID = taskIDs[i];
                    if (tID <= 0) { continue; }
                    Abort(tID);
                }
            }
        }

        public static void DelayedCall(float delay, SyncTask task, bool useRealtimeForDelay = false)
        {
            if (TaskRunner.instance != null && TaskRunner.instance.gameObject != null && TaskRunner.instance.gameObject.activeInHierarchy)
            {
                TaskRunner.instance.StartCoroutine(DelayedCallCOR(delay, task));
            }
            
            IEnumerator DelayedCallCOR(float delay, SyncTask task)
            {
                if (useRealtimeForDelay)
                {
                    yield return new WaitForSecondsRealtime(delay);
                }
                else
                {
                    yield return new WaitForSeconds(delay);
                }
                task?.Invoke();
            }
        }

        public static void DelayedCall(float delay, IEnumerator task, bool useRealtimeForDelay = false)
        {
            if (TaskRunner.instance != null && TaskRunner.instance.gameObject != null && TaskRunner.instance.gameObject.activeInHierarchy)
            {
                TaskRunner.instance.StartCoroutine(DelayedCallCOR(delay, task));
            }
            
            IEnumerator DelayedCallCOR(float delay, IEnumerator task)
            {
                if (useRealtimeForDelay)
                {
                    yield return new WaitForSecondsRealtime(delay);
                }
                else
                {
                    yield return new WaitForSeconds(delay);
                }
                yield return TaskRunner.instance.StartCoroutine(task);
            }
        }
    }
}