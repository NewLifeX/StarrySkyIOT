using InterconnectionManagementAPP.Enums;
using InterconnectionManagementAPP.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using XingLucifer.IBase.Enums;

namespace InterconnectionManagementAPP.Domains
{
    public class TimingManager
    {
        public ObservableCollection<TimingModel> TimingList { get; set; }
        private Action<string> _sendMessage;
        private Dictionary<string, TimerModel> _keyValuePairs = new Dictionary<string, TimerModel>();
        private IEventAggregator _aggregator;
        public TimingManager(Action<string> sendMessage, IEventAggregator aggregator)
        {
            _sendMessage = sendMessage;
            _aggregator = aggregator;
            if (File.Exists(GlobalStatics.TimingList))
            {
                try
                {
                    TimingList = JsonSerializer.Deserialize<ObservableCollection<TimingModel>>(File.ReadAllText(GlobalStatics.TimingList, Encoding.UTF8));
                    foreach (var item in TimingList)
                    {
                        if (item.RunCount > 0)
                        {
                            AddTimer(item.Name, item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    sendMessage?.Invoke("读取定时列表配置异常");
                    TimingList = new ObservableCollection<TimingModel>();
                }
            }
            else
            {
                TimingList = new ObservableCollection<TimingModel>();
            }
            aggregator.GetEvent<MessageEvent>().Subscribe(sender => {
                if (sender.Data is TimingModel timing)
                {

                    var model = TimingList.FirstOrDefault(x => x.Name == timing.Name);
                    if (model == null && AddTimer(timing.Name, timing))
                    {
                        TimingList.Add(timing);
                    }
                    SaveDeviceList();
                }
            }, arg => arg.Command == Enums.EventTypes.添加定时任务);

            aggregator.GetEvent<MessageEvent>().Subscribe(sender => {
                if (sender.Data is TimingModel timing)
                {
                    var model = TimingList.FirstOrDefault(x => x.Name == timing.Name);
                    if (model == null) { return; }
                    TimingList.Remove(model);
                    RemoveTimer(timing.Name);
                    SaveDeviceList();
                }
            }, arg => arg.Command == Enums.EventTypes.删除定时任务);
        }

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="name">任务名</param>
        /// <param name="timingModel">任务模型</param>
        public bool AddTimer(string name, TimingModel timingModel)
        {
            if (_keyValuePairs.ContainsKey(name))
            {
                return true;
            }
            bool bl = DateTime.TryParse($"2016-09-08 {timingModel.Hour}:{timingModel.Minute}:{timingModel.Seconds}", out DateTime dateTime);
            if (!bl)
            {
                _sendMessage?.Invoke($"添加 [{name}] 任务失败");
                return false;
            }
            _keyValuePairs.Add(name, new TimerModel()
            {
                Timing = timingModel,
            });
            TimeSpan timeSpan = dateTime.TimeOfDay;
            System.Threading.Timer timer = new System.Threading.Timer(Timer, name, TimeSpan.FromSeconds(30), TimeSpan.FromMinutes(1));
            timer.Change(
                SubtractTime(timingModel.PeriodType, timingModel.Weeks, timeSpan), //第一个参数为首次启动多少时间后触发
                GetNewTime(timingModel.PeriodType, timeSpan)//第二个参数为每次执行间隔时间
            );
            _keyValuePairs[name].TimerControl = timer;
            _sendMessage?.Invoke($"添加 [{name}] 任务成功");
            return true;
        }

        /// <summary>
        /// 移除任务
        /// </summary>
        /// <param name="name">任务名</param>
        public void RemoveTimer(string name)
        {
            if (_keyValuePairs.ContainsKey(name))
            {
                _keyValuePairs[name].TimerControl.Dispose();
                _keyValuePairs.Remove(name);
                _sendMessage?.Invoke($"移除 [{name}] 任务");
            }
        }

        private async void Timer(object state)
        {
            string name = (string)state;
            if (!_keyValuePairs.ContainsKey(name) || _keyValuePairs[name].Timing.RunCount == 0) { return; }
            if (!(_keyValuePairs[name].Timing.RunCount == 999999))
            {
                _keyValuePairs[name].Timing.RunCount -= 1;
            }
            MessageModel message = new MessageModel
            {
                Command = EventTypes.启动定时任务,
                Data = _keyValuePairs[name].Timing,
            };
            _aggregator.GetEvent<MessageEvent>().Publish(message);
            if (_keyValuePairs[name].Timing.RunCount == 0)
            {
                if (_keyValuePairs[name].Timing.IsDelete)
                {
                    TimingList.Remove(_keyValuePairs[name].Timing);
                }
                _keyValuePairs[name].TimerControl.Dispose();
                _keyValuePairs.Remove(name);
                SaveDeviceList();
                return;
            }
            DateTime.TryParse($"2016-09-08 {_keyValuePairs[name].Timing.Hour}:{_keyValuePairs[name].Timing.Minute}:{_keyValuePairs[name].Timing.Seconds}", out DateTime dateTime);
            TimeSpan timeSpan = dateTime.TimeOfDay;
            _keyValuePairs[name].TimerControl.Change(
                SubtractTime(
                    _keyValuePairs[name].Timing.PeriodType,
                    _keyValuePairs[name].Timing.Weeks,
                    timeSpan),
                GetNewTime(
                    _keyValuePairs[name].Timing.PeriodType,
                    timeSpan)
                    );
        }

        #region 私有方法
        /// <summary>
        /// 计算时间间隔
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="weeks">指定时间，只对年月周有用</param>
        /// <param name="time2">结束时间</param>
        /// <returns></returns>
        private TimeSpan SubtractTime(PeriodTypes type, string weeks, TimeSpan time2)
        {
            DateTime start = DateTime.Now;
            DateTime end = new DateTime();
            //1：周 2：月 3：年 4：天
            switch (type)
            {
                case PeriodTypes.周:
                    int count = weeks != "0" ? int.Parse(weeks) : 7;
                    //当前星期
                    int currentweek = start.DayOfWeek != DayOfWeek.Sunday ? start.DayOfWeek.GetHashCode() : 7;
                    switch (currentweek)
                    {
                        case var d when d > count:
                            end = DateTime.Now.Date.Add(time2).AddDays(8 - count);
                            break;
                        case var d when d == count:
                            end = DateTime.Now.Date.Add(time2);
                            break;
                        case var d when d < count:
                            end = DateTime.Now.Date.Add(time2).AddDays(count - currentweek);
                            break;
                    }
                    break;
                case PeriodTypes.月:
                    end = new DateTime(start.Year, start.Month, int.Parse(weeks), time2.Hours, time2.Minutes, time2.Seconds);
                    break;
                case PeriodTypes.年:
                    string[] date = weeks.Split('-');
                    end = new DateTime(start.Year, int.Parse(date[0]), int.Parse(date[1]), time2.Hours, time2.Minutes, time2.Seconds);
                    break;
                case PeriodTypes.时:
                    end = new DateTime(start.Year, start.Month, start.Day, start.Hour, time2.Minutes, time2.Seconds);
                    break;
                case PeriodTypes.日:
                    end = new DateTime(start.Year, start.Month, start.Day, time2.Hours, time2.Minutes, time2.Seconds);
                    break;
                case PeriodTypes.秒:
                    end = DateTime.Now.Add(time2);
                    break;
            }
            while (start > end)
            {
                switch (type)
                {
                    case PeriodTypes.周:
                        //一周
                        end = end.AddDays(7);
                        break;
                    case PeriodTypes.月:
                        //一月
                        end = end.AddMonths(1);
                        break;
                    case PeriodTypes.年:
                        //一年
                        end = end.AddYears(1);
                        break;
                    case PeriodTypes.日:
                        //一天
                        end = end.AddDays(1);
                        break;
                    case PeriodTypes.时:
                        //一个小时
                        end = end.AddHours(1);
                        break;
                    case PeriodTypes.秒:
                        end = end.Add(time2);
                        break;
                }
            }
            return end.Subtract(start);
        }

        private TimeSpan GetNewTime(PeriodTypes type, TimeSpan timeSpan)
        {
            DateTime start = DateTime.Now.Date.Add(timeSpan);
            DateTime end = DateTime.Now;
            switch (type)
            {
                case PeriodTypes.周:
                    end = start.AddDays(7);
                    break;
                case PeriodTypes.月:
                    end = start.AddMonths(1);
                    break;
                case PeriodTypes.年:
                    end = start.AddYears(1);
                    break;
                case PeriodTypes.日:
                    end = start.AddDays(1);
                    break;
            }
            return end.Subtract(start);
        }
        #endregion
        private void SaveDeviceList() => File.WriteAllText(GlobalStatics.TimingList, JsonSerializer.Serialize(TimingList, GlobalStatics.Options), Encoding.UTF8);
    }
}
