﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _1RM.Model.Protocol.Base;
using _1RM.Service.DataSource.DAO;
using _1RM.View;
using com.github.xiangyuecn.rsacsharp;
using JsonKnownTypes;
using Newtonsoft.Json;
using Shawn.Utils;
using Stylet;

namespace _1RM.Service.DataSource.Model
{
    [JsonConverter(typeof(JsonKnownTypesConverter<DataSourceBase>))] // json serialize/deserialize derived types https://stackoverflow.com/a/60296886/8629624
    [JsonKnownType(typeof(MysqlSource), nameof(MysqlSource))]
    [JsonKnownType(typeof(SqliteSource), nameof(SqliteSource))]
    [JsonKnownType(typeof(PgsqlSource), nameof(PgsqlSource))]
    public abstract partial class DataSourceBase : NotifyPropertyChangedBase
    {
        protected DataSourceBase()
        {
            this.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName != nameof(Description))
                {
                    RaisePropertyChanged(nameof(Description));
                }
            };
        }

        public override string ToString()
        {
            return DataSourceName;
        }

        private EnumDatabaseStatus _status = EnumDatabaseStatus.NotConnectedYet;
        [JsonIgnore]
        public EnumDatabaseStatus Status
        {
            get => _status;
            set
            {
                SetAndNotifyIfChanged(ref _status, value);
                ReconnectTime = Status != EnumDatabaseStatus.OK ?
                    DateTime.Now.AddSeconds(IoC.TryGet<ConfigurationService>()?.DatabaseReconnectPeriod ?? 60 * 5)
                    : DateTime.MinValue;
                StatusInfo = Status == EnumDatabaseStatus.OK ? $"{CachedProtocols.Count} servers" : Status.GetErrorInfo();
            }
        }


        private string _statusInfo = "";
        [JsonIgnore]
        public string StatusInfo
        {
            get => _statusInfo;
            set => SetAndNotifyIfChanged(ref _statusInfo, value);
        }

        [JsonIgnore]
        public DateTime ReconnectTime = DateTime.MinValue;
        private string _reconnectInfo = "";
        /// <summary>
        /// to display the reconnect info in the UI, such as: Next update check in 5 seconds...
        /// </summary>
        [JsonIgnore]
        public string ReconnectInfo
        {
            get => _reconnectInfo;
            set => SetAndNotifyIfChanged(ref _reconnectInfo, value);
        }


        private string _dataSourceName = "";
        public string DataSourceName
        {
            get => _dataSourceName;
            set => SetAndNotifyIfChanged(ref _dataSourceName, value);
        }

        public abstract string GetConnectionString(int connectTimeOutSeconds = 5);

        [JsonIgnore]
        public abstract DatabaseType DatabaseType { get; }

        [JsonIgnore]
        public abstract string Description { get; }
    }
}
