namespace Another_Mirai_Native
{
    public class Debouncer
    {
        private CancellationTokenSource cancellationTokenSource = new();

        private bool canCancel = true;

        public Debouncer()
        {
            
        }

        public Debouncer(CancellationTokenSource cancel)
        {
            cancellationTokenSource = cancel;
            canCancel = false;
        }

        /// <summary>
        /// 防抖
        /// </summary>
        /// <param name="callback">要执行的方法</param>
        /// <param name="waitTime">等待时间（毫秒）</param>
        public void Debounce(Action callback, int waitTime)
        {
            Debounce(callback, TimeSpan.FromMilliseconds(waitTime));
        }

        /// <summary>
        /// 防抖
        /// </summary>
        /// <param name="callback">要执行的方法</param>
        /// <param name="waitTime">等待时间（毫秒）</param>
        public void Debounce(Action callback, TimeSpan waitTime)
        {
            if (canCancel)
            {
                // 取消之前的延时调用
                cancellationTokenSource.Cancel();
                cancellationTokenSource = new CancellationTokenSource();
            }

            Task.Delay(waitTime, cancellationTokenSource.Token)
                .ContinueWith(task =>
                {
                    if (!task.IsCanceled)
                    {
                        callback();
                    }
                });
        }
    }
}
