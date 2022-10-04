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
    public GameObject TumbleWeed1 = null;
    public GameObject TumbleWeed2 = null;

    [Header ("Effects")]
    public GameObject BulletHole = null;
    public GameObject DirtImpact = null;
    public GameObject SandImpact = null;

    [Header("Indicator UI")]
    public Sprite Head = null;
    public Sprite Body = null;
    public Sprite ArmL = null;
    public Sprite ArmR = null;
    public Sprite LegL = null;
    public Sprite LegR = null;

    [Header ("Indicator Red UI")]
    public Sprite HeadRed = null;
    public Sprite BodyRed = null;
    public Sprite ArmLRed = null;
    public Sprite ArmRRed = null;
    public Sprite LegLRed = null;
    public Sprite LegRRed = null;
}
