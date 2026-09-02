using System.Diagnostics;

namespace TBotPlatform.Common.BackgroundServices.Base;

public abstract class BackgroundServiceBase<T> : Microsoft.Extensions.Hosting.BackgroundService
{
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        if (Activity.Current != null)
        {
            await base.StartAsync(cancellationToken);
            return;
        }
        using var activity = new Activity(typeof(T).Name);
        activity.SetParentId(Guid.NewGuid().ToString());
        activity.Start();
        try
        {
            await base.StartAsync(cancellationToken);
        }
        finally
        {
            activity.Stop();
        }
    }
}