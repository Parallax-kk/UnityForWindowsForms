using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveDataManager : MonoBehaviour
{
    [SerializeField]
    private SpownCube m_SpownCube = null;

    public string Save(string path)
    {
        SaveData saveData = new SaveData();

        var listCubes = m_SpownCube.GetListCubes();
        foreach (var cube in listCubes)
        {
            CubeData data = new CubeData();
            data.m_Position        = cube.transform.position;
            data.m_Quaternion      = cube.transform.rotation;
            data.m_Velocity        = cube.GetComponent<Rigidbody>().velocity;
            data.m_AngularVelocity = cube.GetComponent<Rigidbody>().angularVelocity;
            saveData.m_CubeData.Add(data);
        }

        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(path, json);
        return json;
    }

    public void Load(string path)
    {
        m_SpownCube.RemoveAllCube();
        m_SpownCube.RemoveAllHistory();

        string json = File.ReadAllText(path);
        SaveData saveData = JsonUtility.FromJson<SaveData>(json);

        m_SpownCube.SpownFromSaveData(saveData);
    }
}
