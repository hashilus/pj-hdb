using UnityEngine;

public class CalibrationPositionHolder : MonoBehaviour
{
    public enum PlayerSelection
    {
        Player1,
        Player2
    }

    public static CalibrationPositionHolder instance;

    public bool IsCalibrated;
    
    Vector3[] positions = new Vector3[2];
    Quaternion[] rotations = new Quaternion[2];

    public static CalibrationPositionHolder Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CalibrationPositionHolder>();
                if (instance == null)
                {
                    GameObject obj = new ("CalibrationPositionHolder");
                    instance = obj.AddComponent<CalibrationPositionHolder>();
                }
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
   }

    public void SetPosition(PlayerSelection player, Vector3 position)
    {
        positions[(int)player] = position;
    }

    public void SetRotation(PlayerSelection player, Quaternion rotation)
    {
        rotations[(int)player] = rotation;
    }

    public Vector3 GetPosition(PlayerSelection player)
    {
        return positions[(int)player];
    }

    public Quaternion GetRotation(PlayerSelection player)
    {
        return rotations[(int)player];
    }
}
