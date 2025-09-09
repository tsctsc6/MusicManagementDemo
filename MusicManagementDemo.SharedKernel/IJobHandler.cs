namespace MusicManagementDemo.SharedKernel;

public interface IJobHandler
{
    public Task Handle(JobType requestType);
}
