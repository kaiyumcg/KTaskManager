using System.Collections;
using UnityEngine;
using System;
using AttributeLib;

namespace CoreDataLib
{
    [System.Serializable]
    internal class TaskBase
    {
        [CanNotEdit] [SerializeField] string taskName;
        [CanNotEdit] [SerializeField] int PID;
        [CanNotEdit] [SerializeField] bool executed, executing;
        internal string TaskName { get { return taskName; } set { taskName = value; } }
        internal int ProcessID { get { return PID; } set { PID = value; } }
        internal bool Executed { get { return executed; } set { executed = value; } }
        internal bool IsExecuting { get { return executing; } set { executing = value; } }
    }

    [System.Serializable]
    internal class AsyncTask : TaskBase
    {
        IEnumerator task;
        OnDoAnything onComplete;
        internal IEnumerator Task { get { return task; } set { task = value; } }
        internal OnDoAnything OnComplete { get { return onComplete; } set { onComplete = value; } }
    }

    [System.Serializable]
    internal class SyncTask : TaskBase
    {
        OnDoAnything task;
        internal OnDoAnything Task { get { return task; } set { task = value; } }
    }

    [System.Serializable]
    public class FSerialTaskScheduler
    {
        //DebugViews for CustomInspector
        [CanNotEdit] [SerializeField] FOrderedDictionary<int, AsyncTask> asyncCol;
        [CanNotEdit] [SerializeField] FOrderedDictionary<int, SyncTask> syncCol;
        [CanNotEdit] [SerializeField] FOrderedDictionary<int, AsyncTask> asyncCol_Queued;
        [CanNotEdit] [SerializeField] FOrderedDictionary<int, SyncTask> syncCol_Queued;

        #region MutableStates
        MonoBehaviour mono;
        bool isExecuting;
        Coroutine mainKernelHandle;
        Coroutine currentTaskHandle;
        #endregion

        public bool IsExecuting { get { return isExecuting; } }
        public FSerialTaskScheduler(MonoBehaviour mono)
        {
            this.mono = mono;
            InitCollectionIfNull();
        }
        void InitCollectionIfNull()
        {
            if (asyncCol == null) { asyncCol = new FOrderedDictionary<int, AsyncTask>(); }
            if (syncCol == null) { syncCol = new FOrderedDictionary<int, SyncTask>(); }
            if (asyncCol_Queued == null) { asyncCol_Queued = new FOrderedDictionary<int, AsyncTask>(); }
            if (syncCol_Queued == null) { syncCol_Queued = new FOrderedDictionary<int, SyncTask>(); }
        }
        public void ScheduleTask(IEnumerator coroutine, OnDoAnything OnComplete = null, string processName = "", int PID = 0)
        {
            InitCollectionIfNull();
            if (isExecuting)
            {
                asyncCol_Queued.AddValue(asyncCol_Queued.Count,
                    new AsyncTask
                    {
                        TaskName = processName+"_Async",
                        ProcessID = PID,
                        Executed = false,
                        IsExecuting = false,
                        Task = coroutine,
                        OnComplete = OnComplete
                    });
                syncCol_Queued.AddValue(syncCol_Queued.Count, null);
            }
            else
            {
                asyncCol.AddValue(asyncCol.Count,
                    new AsyncTask
                    {
                        TaskName = processName+"_Async",
                        ProcessID = PID,
                        Executed = false,
                        IsExecuting = false,
                        Task = coroutine,
                        OnComplete = OnComplete
                    });
                syncCol.AddValue(syncCol.Count, null);
            }
        }
        public void ScheduleTask(OnDoAnything action, string processName = "", int PID = 0)
        {
            InitCollectionIfNull();
            if (isExecuting)
            {
                asyncCol_Queued.AddValue(asyncCol_Queued.Count, null);
                syncCol_Queued.AddValue(syncCol_Queued.Count,
                    new SyncTask
                    {
                        TaskName = processName+"_Sync",
                        ProcessID = PID,
                        Executed = false,
                        IsExecuting = false,
                        Task = action
                    });
            }
            else
            {
                asyncCol.AddValue(asyncCol.Count, null);
                syncCol.AddValue(syncCol.Count,
                    new SyncTask
                    {
                        TaskName = processName+"_Sync",
                        ProcessID = PID,
                        Executed = false,
                        IsExecuting = false,
                        Task = action
                    });
            }
        }
        public void AbortAll()
        {
            if (mainKernelHandle != null)
            {
                mono.StopCoroutine(mainKernelHandle);
            }

            if (currentTaskHandle != null)
            {
                mono.StopCoroutine(currentTaskHandle);
            }

            asyncCol_Queued = new FOrderedDictionary<int, AsyncTask>();
            syncCol_Queued = new FOrderedDictionary<int, SyncTask>();
            asyncCol = new FOrderedDictionary<int, AsyncTask>();
            syncCol = new FOrderedDictionary<int, SyncTask>();
            isExecuting = false;
        }
        public void PushForExecution()
        {
            if (isExecuting == false && asyncCol != null && asyncCol.Count > 0
                && syncCol != null && syncCol.Count > 0
                && asyncCol.Count == syncCol.Count)
            {
                isExecuting = true;
                mainKernelHandle = mono.StartCoroutine(Kernel());
            }
        }
        IEnumerator Kernel()
        {
            var count = asyncCol.Count;
            for (int i = 0; i < count; i++)
            {
                var ta = asyncCol[i];
                var t = syncCol[i];

                if (ta == null && t != null)
                {
                    t.IsExecuting = true;
                    t.Task?.Invoke();
                    t.Executed = true;
                    t.IsExecuting = false;
                }
                else if (ta != null && t == null)
                {
                    ta.IsExecuting = true;
                    currentTaskHandle = mono.StartCoroutine(ta.Task);
                    yield return currentTaskHandle;
                    ta.Executed = true;
                    ta.IsExecuting = false;
                    ta.OnComplete?.Invoke();
                }
                else if (ta == null && t == null)
                {
                    throw new Exception("Double dictionary data null sync error! BUG!");
                }
                else
                {
                    if (ta != null && t != null)
                    {
                        if (ta.Task != null && t.Task != null)
                        {
                            throw new Exception("Double dictionary data null sync error! BUG!");
                        }
                    }
                }
            }
            asyncCol = new FOrderedDictionary<int, AsyncTask>();
            syncCol = new FOrderedDictionary<int, SyncTask>();
            if (asyncCol_Queued != null && syncCol_Queued != null
                && asyncCol_Queued.Count > 0 && syncCol_Queued.Count > 0
                && asyncCol_Queued.Count == syncCol_Queued.Count)
            {
                var count_Queued = asyncCol_Queued.Count;
                for (int i = 0; i < count_Queued; i++)
                {
                    asyncCol.AddValue(i, asyncCol_Queued[i]);
                    syncCol.AddValue(i, syncCol_Queued[i]);
                }
            }
            asyncCol_Queued = new FOrderedDictionary<int, AsyncTask>();
            syncCol_Queued = new FOrderedDictionary<int, SyncTask>();
            if (asyncCol != null && syncCol != null
                && asyncCol.Count > 0 && syncCol.Count > 0
                && asyncCol.Count == syncCol.Count)
            {
                mainKernelHandle = mono.StartCoroutine(Kernel());
                yield return mainKernelHandle;
            }
            else
            {
                isExecuting = false;
                yield break;
            }
        }
    }
}