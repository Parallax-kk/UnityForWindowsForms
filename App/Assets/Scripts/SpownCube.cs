using System.Collections.Generic;
using UnityEngine;

public class SpownCube : MonoBehaviour
{
    [SerializeField]
    private GameObject m_CubePrefab;

    [SerializeField]
    private Transform m_Root = null;

    [SerializeField]
    private List<GameObject> m_listCubes = new List<GameObject>();
    public List<GameObject> GetListCubes() { return m_listCubes; }

    private List<SaveData> m_listHistory = new List<SaveData>();

    private int m_CurremtHistory = 0;

    public void Undo()
    {
        if (m_CurremtHistory == 0)
            return;

        RemoveAllCube();

        m_CurremtHistory--;
        SpownFromSaveData(m_listHistory[m_CurremtHistory]);
    }

    public void Redo()
    {
        if (m_CurremtHistory >= m_listHistory.Count-1)
            return;

        RemoveAllCube();

        m_CurremtHistory++;
        SpownFromSaveData(m_listHistory[m_CurremtHistory]);
    }

    private void AddHistory()
    {
        SaveData saveData = new SaveData();

        var listCubes = GetListCubes();
        foreach (var cube in listCubes)
        {
            CubeData data = new CubeData();
            data.m_Position = cube.transform.position;
            data.m_Quaternion = cube.transform.rotation;
            data.m_Velocity = cube.GetComponent<Rigidbody>().velocity;
            data.m_AngularVelocity = cube.GetComponent<Rigidbody>().angularVelocity;
            saveData.m_CubeData.Add(data);
        }

        if(m_CurremtHistory < m_listHistory.Count-1)
            m_listHistory.RemoveRange(m_CurremtHistory + 1, m_listHistory.Count-1);

        m_listHistory.Add(saveData);
        m_CurremtHistory = m_listHistory.Count - 1;
    }

    public void RemoveCube(int uid)
    {
        int index = m_listCubes.FindIndex(x => x.GetInstanceID() == uid);
        m_listCubes.RemoveAt(index);
    }

    public void RemoveAllCube()
    {
        foreach (GameObject cube in m_listCubes)
            Destroy(cube);

        m_listCubes.Clear();
    }

    public void RemoveAllHistory()
    {
        m_listHistory.Clear();
        m_CurremtHistory = 0;
    }

    public void Spown()
    {
        Quaternion quaternion = new Quaternion(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1.0f);
        var cube = Instantiate(m_CubePrefab, transform.position, quaternion, m_Root);
        cube.GetComponent<DespownCube>().SetSpownCube(this);
        m_listCubes.Add(cube);
        AddHistory();
    }

    public void SpownFromSaveData(SaveData saveData)
    {
        foreach (var cubeData in saveData.m_CubeData)
        {
            GameObject cube = Instantiate(m_CubePrefab, cubeData.m_Position, cubeData.m_Quaternion, m_Root);
            cube.GetComponent<Rigidbody>().velocity = cubeData.m_Velocity;
            cube.GetComponent<Rigidbody>().angularVelocity = cubeData.m_AngularVelocity;
            cube.GetComponent<DespownCube>().SetSpownCube(this);
            m_listCubes.Add(cube);
        }
    }
}