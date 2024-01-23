using System.Threading;

public class AWSTaskProgress {
    public float progress;
    public float bytesTransferred;
    public float bytesTotal;
    public CancellationTokenSource cancellationTokenSource;

    public AWSTaskProgress() {
        this.progress = 0;
        this.cancellationTokenSource = new CancellationTokenSource();
    }
}