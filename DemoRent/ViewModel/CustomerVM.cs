using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    /// <summary>
    /// This class represents a possible extension of the application.
    /// Rentals should save their customer's data.
    /// </summary>
    public class CustomerVM : INotifyPropertyChanged
    {
        #region Attributes

        private string name;
        private string nif;
        private string email;
        private string phoneNumber;

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        #region Binding Properties       

        public string Name
        {
            get { return name; }
            set
            {
                if (value != name)
                {
                    name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public string NIF
        {
            get { return nif; }
            set
            {
                if (value != nif)
                {
                    nif = value;
                    OnPropertyChanged("NIF");
                }
            }
        }

        public string Email
        {
            get { return email; }
            set
            {
                if (value != email)
                {
                    email = value;
                    OnPropertyChanged("Email");
                }
            }
        }

        public string PhoneNumber
        {
            get { return phoneNumber; }
            set
            {
                if (value != phoneNumber)
                {
                    phoneNumber = value;
                    OnPropertyChanged("PhoneNumber");
                }
            }
        }

        #endregion

        #region Private Methods

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}