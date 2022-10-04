using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Singleton<T> : MonoBehaviourPunCallbacks where T : class
{
    private static T instance;
    private static object _lock = new object();
    public static T Instance
    {
        get
        {
            lock (_lock)
            {
                if (instance == null)
                {
                    instance = FindObjectOfType(typeof(T)) as T;

                    if (instance == null)
                    {
                        GameObject obj = new GameObject(typeof(T).ToString(), typeof(T));
                        instance = obj.GetComponent<T>();
                    }
                }
                return instance;
            }
        }
    }
}
