using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class SequenceTaskRunner
{
    public delegate void TaskDelegate();
    
    private List<TaskDelegate> taskSequence = new List<TaskDelegate>();
    private List<TaskDelegate> sideTaskSequence = new List<TaskDelegate>();

    void Init(){}

    // Update is called once per frame
    public void Update()
    {
        if (taskSequence.Count != 0)
        {
            if (shootTimes[0] < DateTime.Now)
            {
                taskSequence[0].Invoke();
                taskSequence.Remove(taskSequence[0]);
                shootTimes.Remove(shootTimes[0]);
                return;
            }
        }
        
        if (sideTaskSequence.Count != 0)
        {
            if (sideShootTimes[0] < DateTime.Now)
            { 
                sideTaskSequence[0].Invoke();
                sideTaskSequence.Remove(sideTaskSequence[0]);
                sideShootTimes.Remove(sideShootTimes[0]);
            }
        }
    }

    public void AddTask(TaskDelegate task, DateTime shootTime)
    {
        taskSequence.Add(task);
        shootTimes.Add(shootTime);
    }
    
    public void AddSideTask(TaskDelegate task, DateTime shootTime)
    {
        sideTaskSequence.Add(task);
        sideShootTimes.Add(shootTime);
    }
}
