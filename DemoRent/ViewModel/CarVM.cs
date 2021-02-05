using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using ViewModel.Commands;
using ViewModel.DataAccess;
using Microsoft.Win32;
using System.Windows.Media;

namespace ViewModel
{
    public class CarVM : INotifyPropertyChanged
    {
        #region Attributes

        private string brand;
        private string model;
        private int year;
        private int kms;
        private decimal pricePerKm;
        private decimal pricePerMonth;

        private bool booked = false;
        private bool bookingConflict;
        private bool inStore = true;
        private bool available = true;
        private bool editing = false;

        private int contractedKms = 0;
        private int contractedMonths = 0;

        private DateTime pickupDate = DateTime.Today;
        private DateTime returnDate = DateTime.Today;
        private decimal discount = 1.00M;

        private decimal customDiscount = 1.15M;
        private bool customDiscountAvailable = false;

        private decimal rentalCost = 0;

        private string searchString;
        private Car selectedCar = new Car();
        private Car tempCar = new Car();
        private ImageSource photo;
        private string rentalNote;

        #endregion

        #region Constructor

        public CarVM()
        {
            this.Cars = new ObservableCollection<Car>();
            this.ListCars();

            this.InstantiateCommands();
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Binding Properties       

        // Vehicle details

        public Car SelectedCar
        {
            get { return selectedCar; }
            set
            {
                if (value != selectedCar)
                {
                    selectedCar = value;
                    if (selectedCar != null)
                    {
                        this.Brand = selectedCar.Brand;
                        this.Model = selectedCar.Model;
                        this.Year = selectedCar.Year;
                        this.Kms = selectedCar.Kms;
                        this.PricePerKm = selectedCar.PricePerKm;
                        this.PricePerMonth = selectedCar.PricePerMonth;
                        this.InStore = selectedCar.InStore;
                        this.Booked = selectedCar.Booked;
                        this.Available = this.InStore;
                        this.Photo = !string.IsNullOrEmpty(selectedCar.PhotoUrl) ? new BitmapImage(new Uri(Path.GetFullPath(selectedCar.PhotoUrl))) : null;

                        this.TempCar.CopyDetails(selectedCar);

                        OnPropertyChanged("TempCar");

                        CheckBookingConflict();
                        CalculateRentalCost();
                    }
                    OnPropertyChanged("SelectedCar");
                }
            }
        }

        public Car TempCar
        {
            get { return tempCar; }
            set
            {
                if (value != tempCar)
                {
                    this.tempCar = value;
                    OnPropertyChanged("TempCar");
                }
            }
        }

        public string Brand
        {
            get { return brand; }
            set
            {
                if (brand != value)
                {
                    brand = value;
                    OnPropertyChanged("Brand");
                };
            }
        }

        public string Model
        {
            get { return model; }
            set
            {
                if (model != value)
                {
                    model = value;
                    OnPropertyChanged("Model");
                };
            }
        }

        public int Year
        {
            get { return year; }
            set
            {
                if (year != value)
                {
                    year = value;
                    OnPropertyChanged("Year");
                };
            }
        }

        public int Kms
        {
            get { return kms; }
            set
            {
                if (kms != value)
                {
                    kms = value;
                    OnPropertyChanged("Kms");
                };
            }
        }

        public decimal PricePerKm
        {
            get { return pricePerKm; }
            set
            {
                if (pricePerKm != value)
                {
                    pricePerKm = value;
                    OnPropertyChanged("PricePerKm");
                };
            }
        }

        public decimal PricePerMonth
        {
            get { return pricePerMonth; }
            set
            {
                if (pricePerMonth != value)
                {
                    pricePerMonth = value;
                    OnPropertyChanged("PricePerMonth");
                };
            }
        }

        public ImageSource Photo
        {
            get { return photo; }
            set
            {
                if (value != photo)
                {
                    photo = value;
                    OnPropertyChanged("Photo");
                }
            }
        }

        public bool Booked
        {
            get { return booked; }
            set
            {
                if (value != booked)
                {
                    booked = value;
                    OnPropertyChanged("Booked");
                }
            }
        }

        public bool InStore
        {
            get { return inStore; }
            set
            {
                if (value != inStore)
                {
                    inStore = value;
                    OnPropertyChanged("InStore");
                }
            }
        }

        public bool Available
        {
            get { return available; }
            set
            {
                if (value != available)
                {
                    available = value;
                    OnPropertyChanged("Available");
                }
            }
        }

        public string SearchString
        {
            get { return searchString; }
            set
            {
                if (value != searchString)
                {
                    searchString = value;
                    this.ListCars(searchString);
                    OnPropertyChanged("SearchString");
                }
            }
        }

        public bool Editing
        {
            get { return editing; }
            set
            {
                if (value != editing)
                {
                    editing = value;
                    this.Available = !editing;
                    OnPropertyChanged("Editing");
                }
            }
        }

        public ObservableCollection<Car> Cars { get; set; }

        // Rental details      
        public DateTime PickupDate
        {
            get
            {
                if (pickupDate == DateTime.MinValue)
                    return DateTime.Today;

                return pickupDate;
            }
            set
            {
                if (value != pickupDate)
                {
                    pickupDate = value;
                    CheckBookingConflict();
                    CalculateRentalCost();
                    OnPropertyChanged("PickupDate");
                }
            }
        }

        public DateTime ReturnDate
        {
            get
            {
                if (returnDate == DateTime.MinValue)
                    return DateTime.Today;

                return returnDate;
            }
            set
            {
                if (value != returnDate)
                {
                    returnDate = value;
                    CheckBookingConflict();
                    CalculateRentalCost();
                    OnPropertyChanged("ReturnDate");
                }
            }
        }

        public int ContractedKms
        {
            get { return contractedKms; }
            set
            {
                if (value >= 0)
                    contractedKms = value;

                CalculateRentalCost();
                OnPropertyChanged("ContractedKms");
            }
        }

        public int ContractedMonths
        {
            get { return contractedMonths; }
            set
            {
                contractedMonths = value;
                OnPropertyChanged("ContractedMonths");
            }
        }

        public decimal Discount
        {
            get { return discount; }
            set
            {
                if (value != discount)
                {
                    discount = value;
                    OnPropertyChanged("Discount");
                }
            }
        }

        public bool CustomDiscountAvailable
        {
            get { return customDiscountAvailable; }
            set
            {
                if (value != customDiscountAvailable)
                {
                    customDiscountAvailable = value;
                    OnPropertyChanged("CustomDiscountAvailable");
                }
            }
        }

        public decimal CustomDiscount
        {
            get { return customDiscount; }
            set
            {
                if (value != customDiscount)
                {
                    customDiscount = value;
                    CalculateRentalCost();
                    OnPropertyChanged("CustomDiscount");
                }
            }
        }

        public decimal RentalCost
        {
            get { return rentalCost; }
            set
            {
                rentalCost = value;
                OnPropertyChanged("RentalCost");
            }
        }

        public string RentalNote
        {
            get { return rentalNote; }
            set
            {
                if (value != rentalNote)
                {
                    rentalNote = value;
                    OnPropertyChanged("RentalNote");
                }
            }
        }

        public bool BookingConflict
        {
            get { return bookingConflict; }
            set
            {
                if (value != bookingConflict)
                {
                    bookingConflict = value;
                    OnPropertyChanged("BookingConflict");
                }
            }
        }

        #endregion

        #region Commands

        public BookCarCommand BookCarCommand { get; private set; }
        public EditCarCommand EditCarCommand { get; private set; }
        public SaveCarCommand SaveCarCommand { get; private set; }
        public UploadPhotoCommand UploadPhotoCommand { get; private set; }
        public AddCarCommand AddCarCommand { get; private set; }
        public RemoveCarCommand RemoveCarCommand { get; private set; }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Lists all the cars found in the datasource. If a query is given, it filters the cars by that query. Not case sensitive.
        /// </summary>
        /// <param name="query">The query to filter the cars.</param>
        internal void ListCars(string query = null)
        {
            this.Cars.Clear();

            var cars = CarDAO.Instance.GetCars();

            for (int i = 0; i < cars.Count; i++)
            {
                if (null == query || cars[i].Brand.ToUpper().Contains(query.ToUpper()) || cars[i].Model.ToUpper().Contains(query.ToUpper()))
                    this.Cars.Add(cars[i]);
            }

            this.SelectedCar = cars[0];
        }

        /// <summary>
        /// Creates a rental and adds it to the list of rentals of the current vehicle.
        /// </summary>
        internal void BookCar()
        {
            var rental = new RentalDetails()
            {
                CurrentClient = new Customer(),
                PickupDate = this.PickupDate,
                ReturnDate = this.ReturnDate,
                ContractedKms = this.ContractedKms,
                RentalCost = this.RentalCost,
                Note = this.RentalNote
            };

            SelectedCar.Rentals.Add(rental);
            SelectedCar.Rentals = SelectedCar.Rentals.OrderBy(x => x.ReturnDate).ToList();
            SelectedCar.Kms += this.ContractedKms;

            if (!CarDAO.Instance.UpdateCar(SelectedCar))
            {
                // Handle error and log
            };

            ClearRentalValues();
            ListCars();
        }

        /// <summary>
        /// Toggles the editing of the selected vehicle.
        /// </summary>
        internal void EditCar()
        {
            TempCar.CopyDetails(selectedCar);
            Editing = !Editing;

            if (Editing == false)
                ListCars();
        }

        /// <summary>
        /// Adds a new car to the car collection in the data source and re lists all cars.
        /// </summary>
        internal void AddCar()
        {
            SelectedCar = new Car();
            Editing = true;
        }

        /// <summary>
        /// Saves the details of a car being edited.
        /// </summary>
        internal void SaveCar()
        {
            SelectedCar.CopyDetails(tempCar);
            if (!CarDAO.Instance.UpdateCar(SelectedCar))
            {
                // Handle error and log
            };
            Editing = false;

            ListCars();
        }

        /// <summary>
        /// Removes a car from the list of cars.
        /// </summary>
        internal void RemoveCar()
        {
            if (!CarDAO.Instance.DeleteCar(SelectedCar))
            {
                // Handle error and log
            }
            ListCars();
        }

        /// <summary>
        /// Opens a file dialog. Copies the selected file to the assets folder, with a new name (GUID) and associates it with the selected car.
        /// </summary>
        internal void UploadPhoto()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = "Select a picture";
            fileDialog.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (fileDialog.ShowDialog() == true)
            {
                Guid guid = Guid.NewGuid();
                string extension = Path.GetExtension(fileDialog.FileName);
                string finalPath = Path.GetFullPath($"Assets/{guid}{extension}");

                try
                {
                    File.Copy(fileDialog.FileName, finalPath);

                    TempCar.PhotoUrl = finalPath;
                    TempCar.Photo = new BitmapImage(new Uri(tempCar.PhotoUrl));
                    Photo = TempCar.Photo;
                }
                catch
                {
                    // Handle error and log 
                }
            }
        }

        /// <summary>
        /// Iterates through all rentals of the car.
        /// If a rental is found with an interval in which the pickup or the return date is found, it signals a conflict.
        /// </summary>
        internal void CheckBookingConflict()
        {
            BookingConflict = false;

            for (int i = 0; i < SelectedCar.Rentals.Count; i++)
            {
                RentalDetails rental = SelectedCar.Rentals[i];
                if (pickupDate >= rental.PickupDate && pickupDate <= rental.ReturnDate ||
                    returnDate >= rental.PickupDate && returnDate <= rental.ReturnDate)
                {
                    BookingConflict = true;
                    break;
                }
            }
        }

        #endregion

        #region Private Methods

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Auxiliary

        /// <summary>
        /// Calculates the cost of a rental.
        /// This method is called everytime a change to the rental details is made.
        /// </summary>
        private void CalculateRentalCost()
        {
            // Determining the number of months, rounding up: one month and one day = two months.
            ContractedMonths = (ReturnDate.Year * 12 + ReturnDate.Month) -
                               (PickupDate.Year * 12 + PickupDate.Month) +
                               (ReturnDate.Day > PickupDate.Day ? 1 : 0);

            CalculateDiscount();

            RentalCost = (selectedCar.PricePerMonth * ContractedMonths + selectedCar.PricePerKm * ContractedKms) / Discount;
        }

        /// <summary>
        /// Determines the discount.
        /// If the number of kms per year is determined as higher than 50 000 km, it activates custom discount.
        /// </summary>
        private void CalculateDiscount()
        {
            if (this.contractedMonths > 0 && this.contractedKms > 0)
            {
                // Assumption: 13 months are considered 2 years. The total kms/year will take this value into consideration.
                double yearFraction = Math.Ceiling(contractedMonths / 12d);
                double kmsPerYear = (double)Math.Round(contractedKms / yearFraction);

                CustomDiscountAvailable = false;

                if (kmsPerYear <= 15000)
                {
                    Discount = 1.00M;
                }
                else if (kmsPerYear > 15000 && kmsPerYear <= 20000)
                {
                    Discount = 1.10M;
                }
                else if (kmsPerYear <= 50000)
                {
                    Discount = 1.15M;
                }
                else if (kmsPerYear > 50000)
                {
                    CustomDiscountAvailable = true;
                    Discount = CustomDiscount;
                }
            }
        }

        /// <summary>
        /// Clears all values for the rental details from the form.
        /// </summary>
        private void ClearRentalValues()
        {
            this.ContractedKms = 0;
            this.ContractedMonths = 0;
            this.PickupDate = DateTime.Today;
            this.ReturnDate = DateTime.Today;
            this.Discount = 1.00M;
            this.CustomDiscountAvailable = false;
            this.RentalNote = string.Empty;
            this.RentalCost = 0;
        }

        /// <summary>
        /// Instantiates all commands needed by the view model.
        /// It is called only at the constructor.
        /// </summary>
        private void InstantiateCommands()
        {
            this.BookCarCommand = new BookCarCommand(this);
            this.EditCarCommand = new EditCarCommand(this);
            this.SaveCarCommand = new SaveCarCommand(this);
            this.UploadPhotoCommand = new UploadPhotoCommand(this);
            this.AddCarCommand = new AddCarCommand(this);
            this.RemoveCarCommand = new RemoveCarCommand(this);
        }

        #endregion
    }
}
