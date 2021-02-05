using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using DemoRent.Commands;
using DemoRent.DataAccess;
using Microsoft.Win32;
using DemoRent.Models;

namespace DemoRent.ViewModels
{
    public class ShellViewModel : Screen
    {
        #region Attributes

        private string _brand;
        private string _model;
        private int _year;
        private int _kms;
        private decimal _pricePerKm;
        private decimal _pricePerMonth;

        private bool _booked = false;
        private bool _bookingConflict;
        private bool _inStore = true;
        private bool _available = true;
        private bool _editing = false;

        private int _contractedKms = 0;
        private int _contractedMonths = 0;

        private DateTime _pickupDate = DateTime.Today;
        private DateTime _returnDate = DateTime.Today;
        private decimal _discount = 1.00M;

        private decimal _customDiscount = 1.15M;
        private bool _customDiscountAvailable = false;

        private decimal _rentalCost = 0;

        private string _searchString;
        private CarModel _selectedCar = new CarModel();
        private CarModel _tempCar = new CarModel();
        private ImageSource _photo;
        private string _rentalNote;

        #endregion

        #region Constructor

        public ShellViewModel()
        {
            this.Cars = new ObservableCollection<CarModel>();
            this.ListCars();

            this.InstantiateCommands();
        }

        #endregion

        #region Binding Properties       

        // Vehicle details

        public CarModel SelectedCar
        {
            get { return _selectedCar; }
            set
            {
                if (value != _selectedCar)
                {
                    _selectedCar = value;
                    if (_selectedCar != null)
                    {
                        this.Brand = _selectedCar.Brand;
                        this.Model = _selectedCar.Model;
                        this.Year = _selectedCar.Year;
                        this.Kms = _selectedCar.Kms;
                        this.PricePerKm = _selectedCar.PricePerKm;
                        this.PricePerMonth = _selectedCar.PricePerMonth;
                        this.InStore = _selectedCar.InStore;
                        this.Booked = _selectedCar.Booked;
                        this.Available = this.InStore;
                        this.Photo = !string.IsNullOrEmpty(_selectedCar.PhotoUrl) ? new BitmapImage(new Uri(Path.GetFullPath(_selectedCar.PhotoUrl))) : null;

                        this.TempCar.CopyDetails(_selectedCar);

                        NotifyOfPropertyChange(()=>TempCar);

                        CheckBookingConflict();
                        CalculateRentalCost();
                    }

                    NotifyOfPropertyChange(() => SelectedCar);
                    NotifyOfPropertyChange(nameof(CanRemoveCar));
                    NotifyOfPropertyChange(nameof(CanAddCar));
                }
            }
        }

        public CarModel TempCar
        {
            get { return _tempCar; }
            set
            {
                if (value != _tempCar)
                {
                    this._tempCar = value;
                    NotifyOfPropertyChange(() => TempCar);
                }
            }
        }

        public string Brand
        {
            get { return _brand; }
            set
            {
                if (_brand != value)
                {
                    _brand = value;
                    NotifyOfPropertyChange(() => Brand);
                };
            }
        }

        public string Model
        {
            get { return _model; }
            set
            {
                if (_model != value)
                {
                    _model = value;
                    NotifyOfPropertyChange(() => Model);
                };
            }
        }

        public int Year
        {
            get { return _year; }
            set
            {
                if (_year != value)
                {
                    _year = value;
                    NotifyOfPropertyChange(() => Year);
                };
            }
        }

        public int Kms
        {
            get { return _kms; }
            set
            {
                if (_kms != value)
                {
                    _kms = value;
                    NotifyOfPropertyChange(() => Kms);
                };
            }
        }

        public decimal PricePerKm
        {
            get { return _pricePerKm; }
            set
            {
                if (_pricePerKm != value)
                {
                    _pricePerKm = value;
                    NotifyOfPropertyChange(() => PricePerKm);
                };
            }
        }

        public decimal PricePerMonth
        {
            get { return _pricePerMonth; }
            set
            {
                if (_pricePerMonth != value)
                {
                    _pricePerMonth = value;
                    NotifyOfPropertyChange(() => PricePerMonth);
                };
            }
        }

        public ImageSource Photo
        {
            get { return _photo; }
            set
            {
                if (value != _photo)
                {
                    _photo = value;
                    NotifyOfPropertyChange(() => Photo);
                }
            }
        }

        public bool Booked
        {
            get { return _booked; }
            set
            {
                if (value != _booked)
                {
                    _booked = value;
                    NotifyOfPropertyChange(() => Booked);
                }
            }
        }

        public bool InStore
        {
            get { return _inStore; }
            set
            {
                if (value != _inStore)
                {
                    _inStore = value;
                    NotifyOfPropertyChange(() => InStore);
                }
            }
        }

        public bool Available
        {
            get { return _available; }
            set
            {
                if (value != _available)
                {
                    _available = value;
                    NotifyOfPropertyChange(() => Available);
                }
            }
        }

        public string SearchString
        {
            get { return _searchString; }
            set
            {
                if (value != _searchString)
                {
                    _searchString = value;
                    this.ListCars(_searchString);
                    NotifyOfPropertyChange(() => SearchString);
                }
            }
        }

        public bool Editing
        {
            get { return _editing; }
            set
            {
                if (value != _editing)
                {
                    _editing = value;
                    this.Available = !_editing;
                    NotifyOfPropertyChange(() => Editing);
                }
            }
        }

        public ObservableCollection<CarModel> Cars { get; set; }

        // Rental details      
        public DateTime PickupDate
        {
            get
            {
                if (_pickupDate == DateTime.MinValue)
                    return DateTime.Today;

                return _pickupDate;
            }
            set
            {
                if (value != _pickupDate)
                {
                    _pickupDate = value;
                    CheckBookingConflict();
                    CalculateRentalCost();
                    NotifyOfPropertyChange(() => PickupDate);
                }
            }
        }

        public DateTime ReturnDate
        {
            get
            {
                if (_returnDate == DateTime.MinValue)
                    return DateTime.Today;

                return _returnDate;
            }
            set
            {
                if (value != _returnDate)
                {
                    _returnDate = value;
                    CheckBookingConflict();
                    CalculateRentalCost();
                    NotifyOfPropertyChange(()=>ReturnDate);
                }
            }
        }

        public int ContractedKms
        {
            get { return _contractedKms; }
            set
            {
                if (value >= 0)
                    _contractedKms = value;

                CalculateRentalCost();
                NotifyOfPropertyChange(() => ContractedKms);
            }
        }

        public int ContractedMonths
        {
            get { return _contractedMonths; }
            set
            {
                _contractedMonths = value;
                NotifyOfPropertyChange(()=>ContractedMonths);
            }
        }

        public decimal Discount
        {
            get { return _discount; }
            set
            {
                if (value != _discount)
                {
                    _discount = value;
                    NotifyOfPropertyChange(()=>Discount);
                }
            }
        }

        public bool CustomDiscountAvailable
        {
            get { return _customDiscountAvailable; }
            set
            {
                if (value != _customDiscountAvailable)
                {
                    _customDiscountAvailable = value;
                    NotifyOfPropertyChange(()=>CustomDiscountAvailable);
                }
            }
        }

        public decimal CustomDiscount
        {
            get { return _customDiscount; }
            set
            {
                if (value != _customDiscount)
                {
                    _customDiscount = value;
                    CalculateRentalCost();
                    NotifyOfPropertyChange(()=>CustomDiscount);
                }
            }
        }

        public decimal RentalCost
        {
            get { return _rentalCost; }
            set
            {
                _rentalCost = value;
                NotifyOfPropertyChange(()=>RentalCost);
            }
        }

        public string RentalNote
        {
            get { return _rentalNote; }
            set
            {
                if (value != _rentalNote)
                {
                    _rentalNote = value;
                    NotifyOfPropertyChange(()=>RentalNote);
                }
            }
        }

        public bool BookingConflict
        {
            get { return _bookingConflict; }
            set
            {
                if (value != _bookingConflict)
                {
                    _bookingConflict = value;
                    NotifyOfPropertyChange(()=>BookingConflict);
                }
            }
        }

        #endregion

        #region Commands

        public BookCarCommand BookCarCommand { get; private set; }
        public EditCarCommand EditCarCommand { get; private set; }
        public SaveCarCommand SaveCarCommand { get; private set; }
        public AddCarCommand AddCarCommand { get; private set; }
        public UploadPhotoCommand UploadPhotoCommand { get; private set; }
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
            TempCar.CopyDetails(_selectedCar);
            Editing = !Editing;

            if (Editing == false)
                ListCars();
        }

        public bool CanAddCar()
        {
            return SelectedCar.Brand == "Seat";

        }

        /// <summary>
        /// Adds a new car to the car collection in the data source and re lists all cars.
        /// </summary>
        public void AddCar()
        {
            SelectedCar = new CarModel();
            Editing = true;
        }

        /// <summary>
        /// Saves the details of a car being edited.
        /// </summary>
        internal void SaveCar()
        {
            SelectedCar.CopyDetails(_tempCar);
            if (!CarDAO.Instance.UpdateCar(SelectedCar))
            {
                // Handle error and log
            };
            Editing = false;

            ListCars();
        }

        public bool CanRemoveCar()
        {
            return false;
        }

        /// <summary>
        /// Removes a car from the list of cars.
        /// </summary>
        public void RemoveCar()
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
                    TempCar.Photo = new BitmapImage(new Uri(_tempCar.PhotoUrl));
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
                if (_pickupDate >= rental.PickupDate && _pickupDate <= rental.ReturnDate ||
                    _returnDate >= rental.PickupDate && _returnDate <= rental.ReturnDate)
                {
                    BookingConflict = true;
                    break;
                }
            }
        }

        #endregion

        #region Private Methods

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

            RentalCost = (_selectedCar.PricePerMonth * ContractedMonths + _selectedCar.PricePerKm * ContractedKms) / Discount;
        }

        /// <summary>
        /// Determines the discount.
        /// If the number of kms per _year is determined as higher than 50 000 km, it activates custom discount.
        /// </summary>
        private void CalculateDiscount()
        {
            if (this._contractedMonths > 0 && this._contractedKms > 0)
            {
                // Assumption: 13 months are considered 2 years. The total kms/_year will take this value into consideration.
                double yearFraction = Math.Ceiling(_contractedMonths / 12d);
                double kmsPerYear = (double)Math.Round(_contractedKms / yearFraction);

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
        /// Instantiates all commands needed by the view _model.
        /// It is called only at the constructor.
        /// </summary>
        private void InstantiateCommands()
        {
            this.BookCarCommand = new BookCarCommand(this);
            this.EditCarCommand = new EditCarCommand(this);
            this.SaveCarCommand = new SaveCarCommand(this);
            this.AddCarCommand = new AddCarCommand(this);
            this.UploadPhotoCommand = new UploadPhotoCommand(this);
            this.RemoveCarCommand = new RemoveCarCommand(this);
        }

        #endregion
    }

}

