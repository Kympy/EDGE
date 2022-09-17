using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObj/PrefabData", order = 1)]
public class PrefabData : ScriptableObject
{
    [Header("Character")]
    public GameObject PlayerPrefab = null;

    [Header("Objects")]
    public GameObject Bullet = null;

    [Header ("Effects")]
    public GameObject BulletHole = null;
    public GameObject DirtImpact = null;
    public GameObject SandImpact = null;
}
