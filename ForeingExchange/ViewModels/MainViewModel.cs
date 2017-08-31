using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Windows.Input;
using ForeingExchange.Models;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace ForeingExchange.ViewModels
{

    public class MainViewModel: INotifyPropertyChanged
    {
        #region Attributes
        bool _isRunning;
        string _result;
        bool _isEnabled;
        ObservableCollection<Rate> _rates;
        #endregion

        #region
        public string Amount
        {
            get;
            set;
        }

        public ObservableCollection<Rate> Rates
        {
            get
            {
                return _rates;
            }
            set
            {
                if (_rates != value)
                {
                    _rates = value;
                    PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(nameof(Rates)));
                }
            }
        }

        public Rate SourceRate
        {
            get;
            set;
        }

        public Rate TargetRate
        {
            get;
            set;
        }

        public bool IsRunning
        {
            get
            {
                return _isRunning;
            }
            set
            {
                if (_isRunning !=value)
                {
                    _isRunning = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRunning)));
                }
            }
        }

        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(nameof(IsEnabled)));
                }
            }
        }

        public string Result
        {
            get
            {
                return _result;  
            }
            set
            {
                if (_result != value)
                {
                    _result = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Result)));
                }
            }
        }
        #endregion

        #region Commands
        public ICommand ConvertCommand
        {
            get
            {
                return new RelayCommand(Conver);
            }

        }
       


        async private void Conver()
        {
            if (string.IsNullOrEmpty((Amount)))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Debe seleccionar una taza para convertir idiota!!",
                    "Aceptar");
                return;
            }

            decimal amount = 0;
            if (!decimal.TryParse(Amount, out amount))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Debe ingresar un valor numerico",
                    "Aceptar");
            }

            if (SourceRate == null)
			{
				await Application.Current.MainPage.DisplayAlert(
					"Error",
					"Debe ingresar una taza a convertir origen",
					"Aceptar");
			}

            if (TargetRate == null)
			{
				await Application.Current.MainPage.DisplayAlert(
					"Error",
					"Debe ingresar una taza a convertir destino",
					"Aceptar");
			}

            var amountConverter = amount / (decimal)SourceRate.TaxRate * (decimal)TargetRate.TaxRate;
            Result = string.Format("{0} {1:C} = {2} {3:C}", SourceRate.Code, amount, TargetRate.Code, amountConverter);
        }
		#endregion

		#region Events
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion

		#region Constructors
		public MainViewModel()
        {
            LoadRates();
        }


		#endregion

		#region Methods
        async void LoadRates()
		{
            IsRunning = true;
            Result = "Cargando tazas de cambio...";
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri("http://apiexchangerates.azurewebsites.net");
                var controller = "/api/Rates";
                var response = await client.GetAsync(controller);
                var result = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    IsRunning = false;
                    Result = result;
                }

                var rates = JsonConvert.DeserializeObject<List<Rate>>(result);
                Rates = new ObservableCollection<Rate>(rates);
                IsRunning = false;
                IsEnabled = true; 
                Result = "Listo para convertir";
            }
            catch (Exception ex)
            {
                IsRunning = false;
                Result = ex.Message;
            }
        }
        #endregion
    }
}