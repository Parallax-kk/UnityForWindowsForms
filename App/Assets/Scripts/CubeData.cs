using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CubeData
{
    public Vector3 m_Position = new Vector3();
    public Quaternion m_Quaternion = Quaternion.identity;
    public Vector3 m_Velocity = new Vector3();
    public Vector3 m_AngularVelocity = new Vector3();
}

[Serializable]
public class SaveData
{
    public List<CubeData> m_CubeData = new List<CubeData>();
}