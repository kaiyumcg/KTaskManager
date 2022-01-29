using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace KTaskManager
{
    internal class TaskManagerWindow : EditorWindow
    {
        [MenuItem("Tools/TaskManager/Show Current Tasks")]
        static void Init()
        {
            TaskManagerWindow window = (TaskManagerWindow)EditorWindow.GetWindow(typeof(TaskManagerWindow));
            window.titleContent = new GUIContent("Task Manager");
            window.Show();
        }

        void OnGUI()
        {
            var tasks = TaskManager.tasks;
            if (tasks != null && tasks.Count > 0)
            {
                for (int i = 0; i < tasks.Count; i++)
                {
                    var task = tasks[i];
                    if (task == null) { continue; }
                    task.dummyField = EditorGUILayout.Foldout(task.dummyField, task.TaskName);
                    if (task.dummyField)
                    {
                        GUILayout.Label("Task ID: "+task.TaskID, EditorStyles.boldLabel);
                        GUILayout.Label("Task Status: " + task.Status.ToString(), EditorStyles.boldLabel);
                        GUILayout.Label(task.IsItAsyncTask ? "Async" : "Sync", EditorStyles.boldLabel);
                        GUILayout.Label(task.IsItPerFrameTask ? "Per Frame" : "Onetime", EditorStyles.boldLabel);
                        if (task.IsTaskValid)
                        {
                            if (task.Status == TaskStatus.Running || task.Status == TaskStatus.Paused)
                            {
                                if (GUILayout.Button("Stop"))
                                {
                                    task.StopTaskIfApplicable();
                                }
                            }
                            else if(task.Status == TaskStatus.YetToStart)
                            {
                                if (GUILayout.Button("Start"))
                                {
                                    task.StartTask();
                                }
                            }
                        }
                    }
                }
            }

            if (tasks == null || tasks.Count == 0)
            {
                GUILayout.Label("There is no task executing!", EditorStyles.boldLabel);
            }
        }
    }
}