using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Snippets.Models
{
    public class Snippet : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string Description { get; set; }
        private int _used;
        public int Used
        {
            get => _used;
            set => SetField(ref _used, value);
        }
        [YamlMember(ScalarStyle = ScalarStyle.Literal)]
        public string Prerequisites { get; set; }
        [YamlMember(ScalarStyle = ScalarStyle.Literal)]
        public string Code { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }

    public class EmptyStringToHiddenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string stringValue = (string)value;
            return stringValue == string.Empty ? Visibility.Hidden : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
