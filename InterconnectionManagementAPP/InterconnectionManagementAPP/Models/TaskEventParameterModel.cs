using System;
using System.Collections.Generic;
using System.Text;

namespace InterconnectionManagementAPP.Models
{
    public class TaskEventParameterModel : Prism.Mvvm.BindableBase
    {
		private bool _switchStatus;
		/// <summary>
		/// 开关状态
		/// </summary>
		public bool SwitchStatus
		{
			get { return _switchStatus; }
			set { SetProperty(ref _switchStatus, value); }
		}

		private bool _beforeSwitchStatus;
		/// <summary>
		/// 前一个开关状态
		/// </summary>
		public bool BeforeSwitchStatus
        {
			get { return _beforeSwitchStatus; }
			set { SetProperty(ref _beforeSwitchStatus, value); }
		}

		private int _intervalTime;
		/// <summary>
		/// 间隔时间
		/// </summary>
		public int IntervalTime
		{
			get { return _intervalTime; }
			set { SetProperty(ref _intervalTime, value); }
		}

		private bool _afterSwitchStatus;
        /// <summary>
        /// 后一个开关状态
        /// </summary>
        public bool AfterSwitchStatus
        {
            get { return _afterSwitchStatus; }
            set { SetProperty(ref _afterSwitchStatus, value); }
        }
    }
}
