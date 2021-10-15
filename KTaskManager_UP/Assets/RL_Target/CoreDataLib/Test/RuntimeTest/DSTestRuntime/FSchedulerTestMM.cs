using System.Collections;
using System.Collections.Generic;
using CoreDataLib;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class FSchedulerTestMM
{
    //// A Test behaves as an ordinary method
    //[Test]
    //public void FSchedulerTestMMSimplePasses()
    //{
    //    // Use the Assert class to test conditions
    //}

    int myVal;
    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator FSchedulerMegaTest()
    {
        myVal = -1;
        var gObj = new GameObject("_Test_");
        var mono = gObj.AddComponent<TestMono>();
        FSerialTaskScheduler scheduler = null;
        Assert.DoesNotThrow((TestDelegate)(() => { scheduler = new FSerialTaskScheduler(mono); }));
        Assert.DoesNotThrow(() =>
        {
            scheduler.ScheduleTask(() =>
            {
                Debug.Log("this is simple sync task " +
                "that writes a global value");
                myVal = 0;
            });
        });

        Assert.DoesNotThrow(() =>
        {
            scheduler.ScheduleTask(PrintForSeconds("First! press space to exit first task! ", 0.5f, 19),
            () =>
            {
                Debug.Log("First completed!");
                Assert.True(myVal == 19);
                 
            });
        });
        Assert.DoesNotThrow(() =>
        {
            scheduler.ScheduleTask(PrintForSeconds("Second! Similarly abort with space bar!", 0.5f, 55),
                    () => {
                        Debug.Log("Second completed!");
                        Assert.True(myVal == 55);
                    });
        });
        Assert.False(scheduler.IsExecuting);
        Assert.True(myVal == -1);
        Assert.DoesNotThrow(() => { scheduler.PushForExecution(); });
        Assert.True(scheduler.IsExecuting);
        Assert.True(myVal == 0);
        Assert.True(scheduler.IsExecuting);

        Assert.DoesNotThrow(() =>
        {
            scheduler.ScheduleTask(PrintForSeconds("Third! Similarly abort with space bar!", 0.5f, 333),
                    () => {
                        Debug.Log("Third completed!");
                        Assert.True(myVal == 333);
                    });
        });


        Assert.DoesNotThrow(() =>
        {
            scheduler.ScheduleTask(PrintForSeconds("Fourth! Similarly abort with space bar!", 0.5f, 444),
                    () => {

                        Debug.Log("Fourth completed!");
                        Assert.True(myVal == 444);
                    });
        });
        Assert.DoesNotThrow(() => { scheduler.PushForExecution(); });

        yield return new WaitForSeconds(0.7f);
        //Assert.DoesNotThrow(() =>
        //{

        //});
        scheduler.AbortAll();
        Debug.Log("myval: " + myVal);
        //Assert.True(myVal == 19);
        //NEW
        Assert.DoesNotThrow(() =>
        {
            scheduler.ScheduleTask(PrintForSeconds("Fifth! Similarly abort with space bar!", 0.5f, 333),
                    () => {
                        Debug.Log("Fifth completed!");
                        Assert.True(myVal == 333);
                        Assert.DoesNotThrow(() =>
                        {
                            scheduler.ScheduleTask(PrintForSeconds("Sixth! Similarly abort with space bar!", 0.5f, 444),
                                    () => {

                                        Debug.Log("Sixth completed!");
                                        Assert.True(myVal == 444);
                                        Assert.DoesNotThrow(() =>
                                        {
                                            scheduler.ScheduleTask(PrintForSeconds("Seventh! Similarly abort with space bar!", 0.5f, 444),
                                                    () => {

                                                        Debug.Log("Seventh completed!");
                                                        Assert.True(myVal == 444);
                                                        Assert.DoesNotThrow(() => { scheduler.PushForExecution(); });
                                                    });
                                            Assert.DoesNotThrow(() => { scheduler.PushForExecution(); });
                                        });
                                    });
                        });
                    });
        });
        Assert.DoesNotThrow(() => { scheduler.PushForExecution(); });





        while (true)
        {
            if (scheduler.IsExecuting == false)
            {
                break;
            }
            yield return null;
        }
    }

    float timer = 0f;
    IEnumerator PrintForSeconds(string msg, float duration, int val)
    {
        timer = 0f;
        while (true)
        {
            Debug.Log(msg);
            timer += Time.deltaTime;
            if (timer > duration)
            {
                timer = 0f;
                myVal = val;
                yield break;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}
