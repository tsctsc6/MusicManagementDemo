namespace MusicManagementDemo.SharedKernel;

public enum JobType
{
    Undefined,
    ScanIncremental,
}

public enum JobStatus
{
    Undefined,
    WaitingStart,
    Running,
    Completed,
}
