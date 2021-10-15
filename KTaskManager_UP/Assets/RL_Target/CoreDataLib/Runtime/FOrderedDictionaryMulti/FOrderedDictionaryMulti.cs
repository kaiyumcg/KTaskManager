using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AttributeLib;

namespace CoreDataLib
{
    [System.Serializable]
    public class FOrderedDictionaryMultiPair<TKey, TValue>
    {
        [SerializeField] TKey m_key;
        [SerializeField] List<TValue> m_values;
        public TKey _Key { get { return m_key; } internal set { m_key = value; } }
        public List<TValue> _Value { get { return m_values; } set { m_values = value; } }
        internal FOrderedDictionaryMultiPair()
        {
            m_key = default;
            m_values = new List<TValue>();
        }
    }

    public enum MultiDictionaryMode { ExclusiveOverKey = 1, ExclusiveOverEntireData = 2, Inclusive = 0 }

    /// <summary>
    /// Ordered Dictionary which can contain multiple item per key. You can configure to store exclusive items across key or entire dataSet.
    /// </summary>
    /// <typeparam name="Key"></typeparam>
    /// <typeparam name="Value"></typeparam>
    [System.Serializable]
    public class FOrderedMultimap<Key, Value> : IEnumerator, IEnumerable
    {
        [CanNotEdit] [SerializeField] List<FOrderedDictionaryMultiPair<Key, Value>> content;
        bool isKeyValueType, isValueValueType;
        MultiDictionaryMode mode = MultiDictionaryMode.Inclusive;
        public MultiDictionaryMode Mode { get { return mode; } }
        public FOrderedMultimap(MultiDictionaryMode mode = MultiDictionaryMode.Inclusive)
        {
            content = new List<FOrderedDictionaryMultiPair<Key, Value>>();
            this.mode = mode;
            this.isKeyValueType = typeof(Key).IsValueType;
            this.isValueValueType = typeof(Value).IsValueType;
        }

        bool ExistOnEntireData(in Value value, bool useOptimization)
        {
            bool result = false;
            if (content != null && content.Count > 0)
            {
                for (int i = 0; i < content.Count; i++)
                {
                    if (result == true) { break; }
                    if (content[i] == null) { continue; }
                    var vals = content[i]._Value;
                    if (vals != null && vals.Count > 0)
                    {
                        for (int j = 0; j < vals.Count; j++)
                        {
                            if (DSUtil.IsEqual(vals[j], value, isValueValueType, useOptimization))
                            {
                                result = true;
                                break;
                            }
                        }
                    }
                }
            }

            return result;
        }

        bool Exist(in List<Value> values, in Value value, bool useOptimization)
        {
            bool exist = false;
            if (values != null && values.Count > 0)
            {
                for (int i = 0; i < values.Count; i++)
                {
                    if (DSUtil.IsEqual(values[i], value, isValueValueType, useOptimization))
                    {
                        exist = true;
                        break;
                    }
                }
            }
            return exist;
        }

        public FOrderedDictionaryMultiPair<Key, Value> Get(int id)
        {
            return content[id];
        }

        /// <summary>
        /// Adds a value against a key. If the key is not present then a new list is constructed and then the value is added
        /// If the key is present then the value is added to the current list the key holds. If the collection is configured to
        /// take key space exclusive value then unique values are added. If the collection is configured to consider exclusivity over whole data,
        /// then the value is searched through the entire dataset and is added only if it is NEW. Check constructor argument or the getter properties.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool AddValue(Key key, Value value, bool useOptimization = false)
        {
            bool success = false;
            var proceed = DSUtil.IsNull(key, isKeyValueType, useOptimization) == false;
            if (proceed)
            {
                bool entryExist = false;
                FOrderedDictionaryMultiPair<Key, Value> pair = null;
                entryExist = TryGetPair(key, ref pair, useOptimization);
                if (entryExist)
                {
                    if (pair._Value == null) { pair._Value = new List<Value>(); }
                    bool willCreateAndAdd = false;
                    if (mode == MultiDictionaryMode.ExclusiveOverEntireData)
                    {
                        willCreateAndAdd = !ExistOnEntireData(in value, useOptimization);
                    }
                    else if (mode == MultiDictionaryMode.ExclusiveOverKey)
                    {
                        var list = pair._Value;
                        willCreateAndAdd = !Exist(in list, in value, useOptimization);
                    }
                    else if (mode == MultiDictionaryMode.Inclusive)
                    {
                        willCreateAndAdd = true;
                    }

                    if (willCreateAndAdd)
                    {
                        pair._Value.Add(value);
                        success = true;
                    }
                }
                else
                {
                    bool willCreateAndAdd = false;
                    if (mode == MultiDictionaryMode.ExclusiveOverEntireData)
                    {
                        willCreateAndAdd = !ExistOnEntireData(in value, useOptimization);
                    }
                    else
                    {
                        willCreateAndAdd = true;
                    }

                    if (willCreateAndAdd)
                    {
                        var new_pair = new FOrderedDictionaryMultiPair<Key, Value>();
                        new_pair._Key = key;
                        new_pair._Value = new List<Value>();
                        new_pair._Value.Add(value);
                        content.Add(new_pair);
                        success = true;
                    }
                }
            }
            return success;
        }

        /// <summary>
        /// Removes an entry which have the specified 'Not null' key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool RemoveKeyedPair(Key key, bool useOptimization = false)
        {
            bool success = false;
            
            if (DSUtil.IsNull(key, isKeyValueType, useOptimization) == false && ContainsKey(key, useOptimization))
            {
                bool pairGetSuccess = false;
                FOrderedDictionaryMultiPair<Key, Value> pair = null;
                pairGetSuccess = TryGetPair(key, ref pair, useOptimization);
                if (pairGetSuccess)
                {
                    content.Remove(pair);
                    success = true;
                }
            }
            return success;
        }

        /// <summary>
        /// Check whether a 'Not null' key is present in the collection
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(Key key, bool useOptimization = false)
        {
            bool exist = false;
            if (DSUtil.IsNull(key, isKeyValueType, useOptimization) == false && content != null && content.Count > 0)
            {
                for (int i = 0; i < content.Count; i++)
                {
                    if (DSUtil.IsEqual(content[i]._Key, key, isKeyValueType, useOptimization))
                    {
                        exist = true;
                        break;
                    }
                }
            }
            return exist;
        }

        bool TryGetPair(Key key, ref FOrderedDictionaryMultiPair<Key, Value> pair, bool useOptimization)
        {
            bool exist = false;
            if (DSUtil.IsNull(key, isKeyValueType, useOptimization) == false && content != null && content.Count > 0)
            {
                for (int i = 0; i < content.Count; i++)
                {
                    if (DSUtil.IsEqual(content[i]._Key, key, isKeyValueType, useOptimization))
                    {
                        exist = true;
                        pair = content[i];
                        break;
                    }
                }
            }
            return exist;
        }

        /// <summary>
        /// Output the value of a given 'Not null' key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValues(Key key, out List<Value> value, bool useOptimization = false)
        {
            bool success = false;
            FOrderedDictionaryMultiPair<Key, Value> pair = null;
            success = TryGetPair(key, ref pair, useOptimization);
            value = success ? pair._Value : null;
            return success;
        }

        void SetData(Key key, List<Value> value, bool useOptimization)
        {
#if _DEBUG_MODE
            bool success = false;
#endif
            if (DSUtil.IsNull(key, isKeyValueType, useOptimization) == false && content != null && content.Count > 0)
            {
                for (int i = 0; i < content.Count; i++)
                {
                    if (DSUtil.IsEqual(content[i]._Key, key, isKeyValueType, useOptimization))
                    {
                        content[i]._Value = value;
#if _DEBUG_MODE
                        success = true;
#endif
                    }
                }
            }

#if _DEBUG_MODE
            if (success == false)
            {
                Debug.LogWarning("You were trying to set values against a key in the dictionary but the key is not present!" +
                    " Try adding a value against the key first!");
            }
#endif
        }

        List<Value> GetData(Key key, bool useOptimization)
        {
            List<Value> result = null;
            if (DSUtil.IsNull(key, isKeyValueType, useOptimization) == false)
            {
                bool success = TryGetValues(key, out result, useOptimization);
#if _DEBUG_MODE
                if (success == false)
                {
                    Debug.LogWarning("You were trying to get value against a key in the dictionary but the key is not present!" +
                        " Try adding a value against the key first!");
                }
#endif
            }
            return result;
        }

        /// <summary>
        /// Access the elements of the collection. If we can not get or set values due to many reasons(i.e. key is null), 
        /// Then return value is 'null'
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<Value> this[Key key, bool useOptimization = false]
        {
            get => GetData(key, useOptimization);
            set => SetData(key, value, useOptimization);
        }

        /// <summary>
        /// Element count of the collection
        /// </summary>
        public int Count { get { return content == null ? 0 : content.Count; } }

        object IEnumerator.Current { get { return content[position]; } }
        int position = -1;
        bool IEnumerator.MoveNext()
        {
            position++;
            return (position < content.Count);
        }

        void IEnumerator.Reset()
        {
            position = 0;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)this;
        }
    }
}