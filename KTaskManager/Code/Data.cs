using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KTaskManager
{
    public enum TaskStatus { YetToStart = 0, Running = 1, Paused = 2, CompletedSuccessfully = 3, Aborted = 4 }
    public delegate bool TaskCondition();
    public delegate void SyncTask();
    public class DelayDescription
    {
        public bool RealtimeDelay = false;
        public float DelayAmount = -1f;
    }

    public class TaskHandle
    {
        public string TaskName { get { return taskName; } }
        public int TaskID { get { return taskID; } }
        public TaskStatus Status { get { return status; } }
        public bool IsItPerFrameTask { get { return perFrameTask; } }
        public bool IsItAsyncTask { get { return asyncTask; } }
        public bool IsTaskValid { get { return isValid; } }
#if UNITY_EDITOR
        public bool dummyField { get; set; }
#endif

        string taskName = "";
        int taskID = -1;
        TaskStatus status = TaskStatus.YetToStart;
        IEnumerator task = null;
        SyncTask syncTask = null;
        TaskRunner runner = null;
        bool perFrameTask = false;
        bool asyncTask = false;
        bool isValid = true;
        TaskCondition pauseCondition = null;
        TaskCondition exitCondition = null;
        Coroutine asyncHandle = null;
        
        void CheckAndUpdate(string taskName, int taskID)
        {
            if (TaskRunner.instance == null)
            {
                var g = new GameObject("_TaskManRunner_");
                TaskRunner.instance = g.AddComponent<TaskRunner>();
            }
            runner = TaskRunner.instance;
            this.taskName = taskName;
            this.taskID = taskID;
            this.status = TaskStatus.YetToStart;
            this.isValid = true;
        }

        private TaskHandle() { }
        internal TaskHandle(string tName, int tID, IEnumerator task)
        {
            CheckAndUpdate(tName, tID);
            this.task = task;
            this.perFrameTask = false;
            this.asyncTask = true;
        }

        internal TaskHandle(string tName, int tID, IEnumerator task, TaskCondition pauseCondition, TaskCondition exitCondition)
        {
            CheckAndUpdate(tName, tID);
            this.task = task;
            this.perFrameTask = true;
            this.pauseCondition = pauseCondition;
            this.exitCondition = exitCondition;
            this.asyncTask = true;
        }

        internal TaskHandle(string tName, int tID, SyncTask task)
        {
            CheckAndUpdate(tName, tID);
            this.syncTask = task;
            this.perFrameTask = false;
            this.asyncTask = false;
        }

        internal TaskHandle(string tName, int tID, SyncTask task, TaskCondition pauseCondition, TaskCondition exitCondition)
        {
            CheckAndUpdate(tName, tID);
            this.syncTask = task;
            this.perFrameTask = true;
            this.pauseCondition = pauseCondition;
            this.exitCondition = exitCondition;
            this.asyncTask = false;
        }

        public void StartTask(DelayDescription delay = null)
        {
            if (isValid == false) { return; }
            if (status == TaskStatus.Paused || status == TaskStatus.Running) { return; }
            status = TaskStatus.Running;
            asyncHandle = runner.StartCoroutine(TaskKernel(delay));
        }

        public void StopTaskIfApplicable()
        {
            if (status == TaskStatus.Aborted || status == TaskStatus.CompletedSuccessfully || status == TaskStatus.YetToStart) { return; }
            status = TaskStatus.Aborted;
            isValid = false;
            if (asyncHandle != null) { runner.StopCoroutine(asyncHandle); }
            asyncHandle = null;

            TaskManager.DelayedCall(3f, () =>
            {
                var tasks = TaskManager.tasks;
                if (tasks != null && tasks.Contains(this)) { tasks.Remove(this); }
            });
        }

        IEnumerator TaskKernel(DelayDescription delay)
        {
            if(delay != null && delay.DelayAmount > 0.0f)
            {
                if (delay.RealtimeDelay)
                {
                    yield return new WaitForSecondsRealtime(delay.DelayAmount);
                }
                else
                {
                    yield return new WaitForSeconds(delay.DelayAmount);
                }
            }

            if (perFrameTask)
            {
                while (true)
                {
                    var pause = false;
                    if (pauseCondition != null)
                    {
                        pause = pauseCondition();
                    }
                    var exit = false;
                    if (exitCondition != null)
                    {
                        exit = exitCondition();
                    }

                    if (exit)
                    {
                        break;
                    }
                    if (pause) { status = TaskStatus.Paused; yield return null; }
                    else
                    {
                        status = TaskStatus.Running;
                        if (asyncTask && task != null)
                        {
                            var cor = runner.StartCoroutine(task);
                            yield return cor;
                        }
                        else if (!asyncTask && syncTask != null)
                        {
                            syncTask();
                            yield return null;
                        }
                    }
                }
            }
            else
            {
                if (asyncTask && task != null)
                {
                    yield return runner.StartCoroutine(task);
                }
                else if (!asyncTask && syncTask != null)
                {
                    syncTask();
                }
            }
            status = TaskStatus.CompletedSuccessfully;
            asyncHandle = null;
            isValid = false;

            TaskManager.DelayedCall(3f, () =>
            {
                var tasks = TaskManager.tasks;
                if (tasks != null && tasks.Contains(this)) { tasks.Remove(this); }
            });
        }
    }
}