using KursDollar.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KursDollar.ViewModel
{
    internal class ViewModel1:INotifyPropertyChanged
    {
        private static readonly HttpClient HttpClient = new();
        DateTime _DateStart;
        private bool _isButtonEnabled=true;
        public bool IsButtonEnabled
        {
            get => _isButtonEnabled;
            set
            {
                if (value == _isButtonEnabled) return;
                _isButtonEnabled = value;
                OnPropertyChanged();
            }
        }
        public DateTime DateSTart
        {
            get => _DateStart;
            set
            {
                if (value == _DateStart) return;
                _DateStart = value;
                OnPropertyChanged();
            }
        }
        private string LinkToCbR
        {
            get
            {
                var formattedStartTime = _DateStart.ToString("dd/MM/yyyy").Replace('.', '/');
                var formattedEndTime = _DateEnd.ToString("dd/MM/yyyy").Replace('.', '/');

                return $"http://www.cbr.ru/scripts/XML_dynamic.asp?date_req1={formattedStartTime}&date_req2={formattedEndTime}&VAL_NM_RQ=R01235";
            }
        }
        DateTime _DateEnd;
        public DateTime DateEnd
        {
            get => _DateEnd;
            set
            {
                if (value == _DateEnd) return;
                _DateEnd = value;
                OnPropertyChanged();
            }
        }
        private Command addCommand;
        public Command StartCommand
        {
            get
            {
                return addCommand ??
                  (addCommand = new Command((obj)=>
                      {
                          FillObservableCollection(obj);
                          Progress = 0;
                      }


                  ));
            }
        }
        private int _progress;
        public int Progress
        {
            get => _progress;
            set
            {
                if (value == _progress || value < 0 || value > 100) return;
                _progress = value;
                OnPropertyChanged();
            }
        }
        public ViewModel1()
        {
            _DateStart = DateTime.Now;
            _DateEnd = DateTime.Now;
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        double _average=0;
        public double average 
        {
            get => _average;
            set
            {
                if (value == _average) return;
                _average = value;
                OnPropertyChanged();
            }
        }
        private async void FillObservableCollection(object? obj)
        {
            
            var records = await GetResponse();
            IsButtonEnabled = false;
            for (var i = 0; i < records.Length; i++)
            {
                if (i==0)
                {
                    average = Convert.ToDouble(records[i].Value);
                }
                Progress = (100 / records.Length) * (i + 1);
                average = (average + Convert.ToDouble(records[i].Value))/2;
                await Task.Delay(1000); // dead
                
            }
            IsButtonEnabled = true;
            Progress = 100;    
        }
        
        private async Task<Record[]> GetResponse()
        {
            var byteArrayResponse = await HttpClient.GetByteArrayAsync(LinkToCbR);
            var xmlString = Encoding.UTF8.GetString(byteArrayResponse, 0, byteArrayResponse.Length);

            using var stringReader = new StringReader(xmlString);
            var serializer = new XmlSerializer(typeof(ValCurs));

            return ((ValCurs)serializer.Deserialize(stringReader)!).Records;
        }
    }
}
