﻿using System;
using Newtonsoft.Json;
using Shawn.Utils;

namespace _1RM.Model
{
    public class Tag : NotifyPropertyChangedBase
    {
        private readonly Action _onConfigPropertyChanged;
        public Tag(string? name, bool isPinned, int customOrder, Action onConfigPropertyChanged)
        {
            _name = name?.ToLower() ?? ""; // json deserialization will set null to string
            _isPinned = isPinned;
            _customOrder = customOrder;
            this._onConfigPropertyChanged = onConfigPropertyChanged;
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetAndNotifyIfChanged(ref _name, value);
        }


        private int _itemsCount = 0;
        [JsonIgnore]
        public int ItemsCount
        {
            get => _itemsCount;
            set => SetAndNotifyIfChanged(ref _itemsCount, value);
        }

        private bool _isPinned = false;

        public bool IsPinned
        {
            get => _isPinned;
            set
            {
                SetAndNotifyIfChanged(ref _isPinned, value);
                _onConfigPropertyChanged.Invoke();
            }
        }


        private int _customOrder;
        public int CustomOrder
        {
            get => _customOrder;
            set => SetAndNotifyIfChanged(ref _customOrder, value);
        }
    }
}
