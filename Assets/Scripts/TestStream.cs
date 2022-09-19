using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class TestStream : MonoBehaviour
{
    private void Start()
    {
        string inData = WriteData();

        ReadData(inData);
    }
    private float totalDamage;
    private void TransferDamage(string value)
    {
        float damage = ReadData(value);

        totalDamage += damage;

    }
    private string WriteData()
    {
        MemoryStream ms = new MemoryStream();
        BinaryWriter bw = new BinaryWriter(ms);
        bw.Write((int)10);
        bw.Write("Data1");

        byte[] inData = ms.GetBuffer();
        string strData = System.Convert.ToBase64String(inData);
        return strData;
    }
    private float ReadData(string inData)
    {
        byte[] byteArray = System.Convert.FromBase64String(inData);

        MemoryStream ms = new MemoryStream(byteArray);
        BinaryReader br = new BinaryReader(ms);
        float fData = br.ReadInt32();
        string sData = br.ReadString();

        Debug.Log("## " + fData);
        Debug.Log("##2 " + sData);

        return fData;
    }
}
