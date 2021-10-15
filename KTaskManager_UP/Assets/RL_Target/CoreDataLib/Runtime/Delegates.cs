using System.Collections;
using UnityEngine;

namespace CoreDataLib
{
    public delegate void OnAnimation(Animator animator);
    public delegate IEnumerator OnAnimationAsync(Animator animator);
    public delegate void OnDoAnything();
    public delegate IEnumerator OnDoAnythingAsync();

    public delegate void OnDoAnything<T1>(T1 t1);
    public delegate IEnumerator OnDoAnythingAsync<T1>(T1 t1);

    public delegate void OnDoAnythingArray<T1>(T1[] t1s);
    public delegate IEnumerator OnDoAnythingAsyncArray<T1>(T1[] t1s);

    public delegate void OnDoAnything<T1, T2>(T1 t1, T2 t2);
    public delegate IEnumerator OnDoAnythingAsync<T1, T2>(T1 t1, T2 t2);

    public delegate void OnDoAnythingArray<T1, T2>(T1[] t1s, T2[] t2s);
    public delegate IEnumerator OnDoAnythingAsyncArray<T1, T2>(T1[] t1s, T2[] t2s);

    public delegate void OnDoAnything<T1, T2, T3>(T1 t1, T2 t2, T3 t3);
    public delegate IEnumerator OnDoAnythingAsync<T1, T2, T3>(T1 t1, T2 t2, T3 t3);

    public delegate void OnDoAnythingArray<T1, T2, T3>(T1[] t1s, T2[] t2s, T3[] t3s);
    public delegate IEnumerator OnDoAnythingAsyncArray<T1, T2, T3>(T1[] t1s, T2[] t2s, T3[] t3s);

    public delegate void OnDoAnything<T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3, T4 t4);
    public delegate IEnumerator OnDoAnythingAsync<T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3, T4 t4);

    public delegate void OnDoAnythingArray<T1, T2, T3, T4>(T1[] t1s, T2[] t2s, T3[] t3s, T4[] t4s);
    public delegate IEnumerator OnDoAnythingAsyncArray<T1, T2, T3, T4>(T1[] t1s, T2[] t2s, T3[] t3s, T4[] t4s);

    public delegate void OnDoAnything<T1, T2, T3, T4, T5>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);
    public delegate IEnumerator OnDoAnythingAsync<T1, T2, T3, T4, T5>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);

    public delegate void OnDoAnythingArray<T1, T2, T3, T4, T5>(T1[] t1s, T2[] t2s, T3[] t3s, T4[] t4s, T5[] t5s);
    public delegate IEnumerator OnDoAnythingAsyncArray<T1, T2, T3, T4, T5>(T1[] t1s, T2[] t2s, T3[] t3s, T4[] t4s, T5[] t5s);

    public delegate void OnDoAnything<T1, T2, T3, T4, T5, T6>(T1 t1, T2 t2, T3 t3,
        T4 t4, T5 t5, T6 t6);
    public delegate IEnumerator OnDoAnythingAsync<T1, T2, T3, T4, T5, T6>(T1 t1, T2 t2, T3 t3,
        T4 t4, T5 t5, T6 t6);

    public delegate void OnDoAnythingArray<T1, T2, T3, T4, T5, T6>(T1[] t1s, T2[] t2s, T3[] t3s,
        T4[] t4s, T5[] t5s, T6[] t6s);
    public delegate IEnumerator OnDoAnythingAsyncArray<T1, T2, T3, T4, T5, T6>(T1[] t1s, T2[] t2s, T3[] t3s,
        T4[] t4s, T5[] t5s, T6[] t6s);

    public delegate void OnDoAnything<T1, T2, T3, T4, T5, T6, T7>(T1 t1, T2 t2, T3 t3,
        T4 t4, T5 t5, T6 t6, T7 t7);
    public delegate IEnumerator OnDoAnythingAsync<T1, T2, T3, T4, T5, T6, T7>(T1 t1, T2 t2, T3 t3,
        T4 t4, T5 t5, T6 t6, T7 t7);

    public delegate void OnDoAnythingArray<T1, T2, T3, T4, T5, T6, T7>(T1[] t1s, T2[] t2s, T3[] t3s,
        T4[] t4s, T5[] t5s, T6[] t6s, T7[] t7s);
    public delegate IEnumerator OnDoAnythingAsyncArray<T1, T2, T3, T4, T5, T6, T7>(T1[] t1s, T2[] t2s, T3[] t3s,
        T4[] t4s, T5[] t5s, T6[] t6s, T7[] t7s);

    public delegate void OnDoAnything<T1, T2, T3, T4, T5, T6, T7, T8>(T1 t1, T2 t2, T3 t3,
        T4 t4, T5 t5, T6 t6, T7 t7, T8 t8);
    public delegate IEnumerator OnDoAnythingAsync<T1, T2, T3, T4, T5, T6, T7, T8>(T1 t1, T2 t2, T3 t3,
        T4 t4, T5 t5, T6 t6, T7 t7, T8 t8);

    public delegate void OnDoAnythingArray<T1, T2, T3, T4, T5, T6, T7, T8>(T1[] t1s, T2[] t2s, T3[] t3s,
        T4[] t4s, T5[] t5s, T6[] t6s, T7[] t7s, T8[] t8s);
    public delegate IEnumerator OnDoAnythingAsyncArray<T1, T2, T3, T4, T5, T6, T7, T8>(T1[] t1s, T2[] t2s, T3[] t3s,
        T4[] t4s, T5[] t5s, T6[] t6s, T7[] t7s, T8[] t8s);
}