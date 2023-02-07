using System;
using System.Collections.Generic;
using System.Text;

namespace InterconnectionManagementAPP.Domains
{
    public class SnowflakeManager
    {
        //机器ID
        private static long _workerId;
        private static long twepoch = 687888001020L; //唯一时间，这是一个避免重复的随机量，自行设定不要大于当前时间戳
        private static long sequence = 0L;
        private static long maxWorkerId = -1L ^ -1L << 4; //最大机器ID
        private static int workerIdShift = 10; //机器码数据左移位数，就是后面计数器占用的位数
        private static int timestampLeftShift = 10 + 4; //时间戳左移动位数就是机器码和计数器总字节数
        public static long sequenceMask = -1L ^ -1L << 10; //一微秒内可以产生计数，如果达到该值则等到下一微妙在进行生成
        private static long _lastTimestamp = -1L;
        private static int _sxid = 0;

        public SnowflakeManager(long id)
        {
            if (_workerId > maxWorkerId || _workerId < 0)
            {
                throw new Exception(string.Format("机器 ID 不能大于 {0} 或小于 0", maxWorkerId));
            }
            _workerId = id;
        }

        public long NextId()
        {
            while (System.Threading.Interlocked.Exchange(ref _sxid, 1) != 0)
            {
                _ = 0; _ = 0; _ = 0; _ = 0;
            }
            long timestamp = TimeGen();
            if (_lastTimestamp == timestamp)
            { //同一微妙中生成ID
                sequence = (sequence + 1) & sequenceMask; //用&运算计算该微秒内产生的计数是否已经到达上限
                if (sequence == 0)
                {
                    //一微妙内产生的ID计数已达上限，等待下一微妙
                    timestamp = TillNextMillis(_lastTimestamp);
                }
            }
            else
            { //不同微秒生成ID
                sequence = 0; //计数清0
            }
            _lastTimestamp = timestamp; //把当前时间戳保存为最后生成ID的时间戳
            long snowflake = (timestamp - twepoch << timestampLeftShift) | (_workerId << workerIdShift) | sequence;
            System.Threading.Interlocked.Exchange(ref _sxid, 0);
            return snowflake;
        }

        /// <summary>
        /// 输入时间生产雪花ID
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long GetTimeId(DateTime dateTime) => (((long)(dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local)).TotalMilliseconds) - twepoch << timestampLeftShift) | (_workerId << workerIdShift) | sequence;

        /// <summary>
        /// 获取下一微秒时间戳
        /// </summary>
        /// <param name="lastTimestamp"></param>
        /// <returns></returns>
        private static long TillNextMillis(long lastTimestamp)
        {
            long timestamp = TimeGen();
            while (timestamp <= lastTimestamp)
            {
                timestamp = TimeGen();
            }
            return timestamp;
        }

        /// <summary>
        /// 生成当前时间戳
        /// </summary>
        /// <returns></returns>
        private static long TimeGen() => (long)(DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local)).TotalMilliseconds;
    }
}
