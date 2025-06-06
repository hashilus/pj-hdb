[System.Serializable]
public class SettingsData
{
    public float initialTime;
    public float[] checkpointExtendTimes = new float[6]; // 6‰ÓŠŒÅ’è
    public float fireLifeScale = 1.0f;

    [System.Serializable]
    public class ResultThresholds
    {
        public float rankS;
        public float rankA;
        public float rankB;
        public float rankC;
    }

    public ResultThresholds resultThresholds = new ResultThresholds();
}