[System.Serializable]
public class SettingsData
{
    public float initialTime;
    public float[] checkpointExtendTimes = new float[6];

    public float fireLifeScale1P = 1.0f;
    public float fireLifeScale2P = 1.0f;

    [System.Serializable]
    public class ResultThresholds
    {
        public float rankS;
        public float rankA;
        public float rankB;
        public float rankC;
    }

    public ResultThresholds resultThresholds = new ResultThresholds();

    // ★ デバッグ用フラグ追加
    public bool debugMode = false;
}
