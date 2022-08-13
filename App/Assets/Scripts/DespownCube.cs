using UnityEngine;

public class DespownCube : MonoBehaviour
{
    private SpownCube m_SpownCube = null;
    public void SetSpownCube(SpownCube spownCube) { m_SpownCube = spownCube; }

    private void Update()
    {
        if (transform.position.y < -2.0)
        {
            m_SpownCube.RemoveCube(gameObject.GetInstanceID());
            Destroy(gameObject);
        }
    }
}