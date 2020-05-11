using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
[ExecuteInEditMode]
public class TaskManager : MonoBehaviourSingleton<TaskManager> 
{
    public  List<CustomThread> customThreads = new List<CustomThread>();


    private  ConcurrentQueue<Action> taskQueue = new ConcurrentQueue<Action>();
    private  ConcurrentQueue<Action> toMainUnityThread = new ConcurrentQueue<Action>();

    public List<Thread> threads= new List<Thread>();
    
    private void OnEnable()
    {

        if (!_instance)
        {
            _instance = this;
        }
        

        threads = new List<Thread>();
        Thread thread = new Thread(QueueExequteThread);
        threads.Add(thread);
        thread.Start();

    }
    private void OnDisable()
    {
        foreach(var thread in threads)
        {
            thread.Abort();
        }
    }
    void QueueExequteThread()
    {

        Action action;
        while (true)
        {
            try
            {
                if (taskQueue.TryDequeue(out action))
                {
                    action.Invoke();
                    action = null;
                }
                else
                {
                    Thread.Sleep(100);
                }
            } catch(Exception e)
            {
                TaskManager.Instance.ExecuteInMainThread(()=> {

                    Debug.LogError(e.Message);
                });
            }
            
        }
    }

    public void StartTask(Action action)
    {
        GetTask(action).Start();
    }

    public void StartTaskInQueue(Action action)
    {
        taskQueue.Enqueue(action);
    }
    public Task GetTask(Action action) {
        return new Task(()=> {
            action.Invoke();
            OnTaskEnded();
        });
    }

    public void OnTaskEnded()
    {
        
    }

    public void ExecuteInMainThread (Action action)
    {
        toMainUnityThread.Enqueue(action);
    }

    private void Update()
    {
        while (!toMainUnityThread.IsEmpty)
        {
            try
            {
                Action action;
                if (toMainUnityThread.TryDequeue(out action))
                    action.Invoke();
            }
            catch (Exception e)
            {
               
                Debug.LogError(e.Message);
                
            }
          

        }
    }
}
