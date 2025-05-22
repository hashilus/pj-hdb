public class SingleExecutionLock
{
    bool isLocked;

    public bool GetLock()
    {
        if (isLocked)
        {
            return false;
        }
        else
        {
            isLocked = true;
            return true;
        }
    }

    public void Release()
    {
        isLocked = false;
    }
}
