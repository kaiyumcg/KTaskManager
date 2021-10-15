using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using CoreDataLib;

public class FOrderedDictionaryTest
{
    [System.Serializable]
    public class FOrderedDictionaryTestData
    {
        public int someVal;
        public string someStr;
    }

    [Test]
    public void CanCreateData()
    {
        Assert.DoesNotThrow(() => { var dic = new FOrderedDictionary<int, int>(); });
        Assert.DoesNotThrow(() => { var dic2 = new FOrderedDictionary<int, Transform>(); });
        Assert.DoesNotThrow(() => { var dic3 = new FOrderedDictionary<Transform, int>(); });
        Assert.DoesNotThrow(() => { var dic4 = new FOrderedDictionary<Transform, Transform>(); });
        Assert.DoesNotThrow(() => { var dic = new FOrderedDictionary<FOrderedDictionaryTestData, FOrderedDictionaryTestData>(); });
        Assert.DoesNotThrow(() => { var dic2 = new FOrderedDictionary<FOrderedDictionaryTestData, Transform>(); });
        Assert.DoesNotThrow(() => { var dic3 = new FOrderedDictionary<Transform, FOrderedDictionaryTestData>(); });
    }

    public class ScoreDesc
    {
        public int score;
        public float time;
    }

    [Test]
    public void AddValue()
    {
        FOrderedDictionary<string, ScoreDesc> dic = null;
        dic = new FOrderedDictionary<string, ScoreDesc>();
        Assert.True(dic.AddValue("Rumman", null));
        var scoreData = new ScoreDesc { score = 10, time = Time.time };
        Assert.True(!dic.AddValue("Rumman", scoreData));
        Assert.True(dic.Count == 1);
        Assert.True(!dic.AddValue("Rumman", scoreData));
        Assert.True(dic.Count == 1);
        Assert.True(dic.AddValue("RSX", null));
        Assert.True(dic.Count == 2);
        Assert.True(!dic.AddValue(null, null));
        Assert.True(dic.Count == 2);
        Assert.True(!dic.AddValue(null, scoreData));
        Assert.True(dic.Count == 2);
    }

    [Test]
    public void RemoveValue()
    {
        FOrderedDictionary<string, ScoreDesc> dic = null;
        dic = new FOrderedDictionary<string, ScoreDesc>();
        dic.AddValue("Rumman", null);
        var scoreData = new ScoreDesc { score = 10, time = Time.time };
        dic.AddValue("Rumman", scoreData);
        dic.AddValue("RSX", null);
        Assert.True(dic.ContainsKey("RSX"));
        Assert.True(dic.ContainsKey("Rumman"));
        Assert.True(dic.Count == 2);
        Assert.True(dic.RemoveKeyedPair("Rumman"));
        Assert.True(dic.ContainsKey("RSX"));
        Assert.True(!dic.ContainsKey("Rumman"));
        Assert.True(dic.Count == 1);
    }

    [Test]
    public void TryGetValue()
    {
        FOrderedDictionary<string, ScoreDesc> dic = null;
        dic = new FOrderedDictionary<string, ScoreDesc>();
        var scoreData = new ScoreDesc { score = 10, time = Time.time };
        dic.AddValue("Rumman", scoreData);
        dic.AddValue("RSX", null);
        ScoreDesc result = null;
        Assert.True(dic.TryGetValue("Rumman", out result));
        Assert.True(result != null);
        Assert.True(result.score == 10);
        Assert.False(dic.TryGetValue(null, out result));
        Assert.True(dic.TryGetValue("RSX", out result));
        Assert.True(result == null);
        dic.RemoveKeyedPair("RSX");
        Assert.False(dic.TryGetValue("RSX", out result));
    }

    [Test]
    public void IndexerTest()
    {
        FOrderedDictionary<string, int> dic = null;
        dic = new FOrderedDictionary<string, int>();
        Assert.True(dic.AddValue("Rumman", -2));
        Assert.False(dic.AddValue("Rumman", 3));
        Assert.True(dic.AddValue("RSX", 77));
        Assert.True(dic["RSX"] == 77);
        Assert.True(dic.RemoveKeyedPair("RSX"));
        Assert.False(dic["RSX"] == 77);
        Assert.True(dic["Rumman"] == -2);
        Assert.False(dic["Rumman"] == 3);
        dic["RSX"] = 78;
        Assert.True(dic["RSX"] == default(int));
        dic["Rumman"] = 797;
        Assert.True(dic["Rumman"] == 797);
    }

    /*
    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator FOrderedDictionaryTestWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
    */
}
