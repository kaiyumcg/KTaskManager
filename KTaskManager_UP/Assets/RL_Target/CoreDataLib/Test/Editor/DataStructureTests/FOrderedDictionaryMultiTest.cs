using System.Collections;
using System.Collections.Generic;
using CoreDataLib;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class FOrderedDictionaryMultiTest
{
    [System.Serializable]
    public class FOrderedDictionaryMultiTestData
    {
        public int someVal;
        public string someStr;
    }

    [Test]
    public void CanCreateData()
    {
        Assert.DoesNotThrow(() => { var dic = new FOrderedMultimap<int, int>(); });
        Assert.DoesNotThrow(() => { var dic2 = new FOrderedMultimap<int, Transform>(); });
        Assert.DoesNotThrow(() => { var dic3 = new FOrderedMultimap<Transform, int>(); });
        Assert.DoesNotThrow(() => { var dic4 = new FOrderedMultimap<Transform, Transform>(); });
        Assert.DoesNotThrow(() => { var dic = new FOrderedMultimap<FOrderedDictionaryMultiTestData, FOrderedDictionaryMultiTestData>(); });
        Assert.DoesNotThrow(() => { var dic2 = new FOrderedMultimap<FOrderedDictionaryMultiTestData, Transform>(); });
        Assert.DoesNotThrow(() => { var dic3 = new FOrderedMultimap<Transform, FOrderedDictionaryMultiTestData>(); });
    }

    public class ScoreDesc
    {
        public int score;
        public float time;
    }

    [Test]
    public void Add_CanHaveNull_Inclusive()
    {
        FOrderedMultimap<string, ScoreDesc> dic = null;
        dic = new FOrderedMultimap<string, ScoreDesc>();
        Assert.True(dic.AddValue("Rumman", null));
        var scoreData = new ScoreDesc { score = 10, time = Time.time };
        Assert.True(dic.AddValue("Rumman", scoreData));
        Assert.True(dic.Count == 1);
        Assert.True(dic["Rumman"].Count == 2);
        Assert.True(dic.AddValue("Rumman", scoreData));
        Assert.True(dic.Count == 1);
        Assert.True(dic["Rumman"].Count == 3);
        Assert.True(dic.AddValue("RSX", null));
        Assert.True(dic.Count == 2);
        Assert.True(dic["Rumman"].Count == 3);
        Assert.True(dic["RSX"].Count == 1);
        Assert.False(dic.AddValue(null, null));
        Assert.True(dic.Count == 2);
        Assert.False(dic.AddValue(null, scoreData));
        Assert.True(dic.Count == 2);
        Assert.True(dic["Rumman"].Count == 3);
        Assert.True(dic["RSX"].Count == 1);
        Assert.True(dic.AddValue("RSX", null));
        Assert.True(dic.AddValue("RSX", null));
        Assert.True(dic.AddValue("RSX", null));
        Assert.True(dic.Count == 2);
        Assert.True(dic["Rumman"].Count == 3);
        Assert.True(dic["RSX"].Count == 4);
    }

    [Test]
    public void Add_CanHaveNull_ExclusiveOverKey()
    {
        FOrderedMultimap<string, ScoreDesc> dic = null;
        dic = new FOrderedMultimap<string, ScoreDesc>(MultiDictionaryMode.ExclusiveOverKey);
        Assert.True(dic.AddValue("Rumman", null));
        var scoreData = new ScoreDesc { score = 10, time = Time.time };
        Assert.True(dic.AddValue("Rumman", scoreData));
        Assert.True(dic.Count == 1);
        Assert.True(dic["Rumman"].Count == 2);
        Assert.False(dic.AddValue("Rumman", scoreData));
        Assert.False(dic.AddValue("Rumman", null));
        Assert.True(dic.Count == 1);
        Assert.True(dic["Rumman"].Count == 2);
        Assert.True(dic.AddValue("RSX", null));
        Assert.True(dic.Count == 2);
        Assert.True(dic["Rumman"].Count == 2);
        Assert.True(dic["RSX"].Count == 1);
        Assert.False(dic.AddValue(null, null));
        Assert.True(dic.Count == 2);
        Assert.False(dic.AddValue(null, scoreData));
        Assert.True(dic.Count == 2);
        Assert.True(dic["Rumman"].Count == 2);
        Assert.True(dic["RSX"].Count == 1);
        Assert.False(dic.AddValue("RSX", null));
        Assert.False(dic.AddValue("RSX", null));
        Assert.False(dic.AddValue("RSX", null));
        Assert.True(dic.Count == 2);
        Assert.True(dic["Rumman"].Count == 2);
        Assert.True(dic["RSX"].Count == 1);
    }

    [Test]
    public void RemoveValue()
    {
        FOrderedMultimap<string, ScoreDesc> dic = null;
        dic = new FOrderedMultimap<string, ScoreDesc>();
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
        FOrderedMultimap<string, ScoreDesc> dic = null;
        dic = new FOrderedMultimap<string, ScoreDesc>();
        var scoreData = new ScoreDesc { score = 10, time = Time.time };
        dic.AddValue("Rumman", scoreData);
        dic.AddValue("RSX", null);
        List<ScoreDesc> result = null;
        Assert.True(dic.TryGetValues("Rumman", out result));
        Assert.True(result != null);
        Assert.True(result[0].score == 10);
        Assert.False(dic.TryGetValues(null, out result));
        Assert.True(dic.TryGetValues("RSX", out result));
        Assert.True(result != null);
        dic.RemoveKeyedPair("RSX");
        Assert.False(dic.TryGetValues("RSX", out result));
        Assert.True(result == null);
    }

    [Test]
    public void IndexerTest()
    {
        FOrderedMultimap<string, int> dic = null;
        dic = new FOrderedMultimap<string, int>();
        Assert.True(dic.AddValue("Rumman", -2));
        Assert.True(dic.AddValue("Rumman", 3));
        Assert.True(dic.AddValue("RSX", 77));

        Assert.True(dic["RSX"][0] == 77);
        dic["RSX"][0] = 79;
        Assert.True(dic["RSX"][0] == 79);
        Assert.True(dic.RemoveKeyedPair("RSX"));
        Assert.True(dic["RSX"] == null);
    }
}