using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoRent.Models;
using Newtonsoft.Json;

namespace DemoRent.DataAccess
{
    /// <summary>
    /// This class represents an interface to the data source.
    /// In the current case, the data source is a json file.
    /// In a complete solution, a database should be used.
    /// </summary>
    public sealed class CarDAO
    {
        #region Singleton implementation

        // Implementing the singleton pattern to assure that only one instance exists with access to the data source. 

        private static CarDAO instance;
        private static readonly object lockObj = new Object();

        private readonly string jsonSourcePath = Path.GetFullPath("DataBaseMock/Cars.json");

        private CarDAO()
        {                        
        }

        public static CarDAO Instance
        {
            get
            {
                lock (lockObj)
                {
                    if (null == instance)
                        instance = new CarDAO();

                    return instance;
                }
            }
        }

        #endregion

        #region Data Access Methods               

        /// <summary>
        /// Retrieves a list of cars from the data source.
        /// </summary>
        /// <returns>A list of cars.</returns>
        public List<CarModel> GetCars()
        {            
            try 
            { 
                this.Cars = JsonConvert.DeserializeObject<List<CarModel>>(File.ReadAllText(jsonSourcePath));
                return Cars;
            }
            catch (Exception ex)
            {
                // Log error accessing file to database and return empty list.
                return new List<CarModel>();
            }            
        }        

        /// <summary>
        /// Updates the details of a selected car.
        /// If the car does not exist in the datasource, it is added.
        /// </summary>
        /// <param name="selectedCar">The selected car to update/add.</param>
        /// <returns>True if update was successful</returns>
        public bool UpdateCar(CarModel selectedCar)
        {
            // Check if car already exists. If not, add it to the database.
            if (!Cars.Contains(selectedCar))
                Cars.Add(selectedCar);

            return SaveCars();            
        }

        /// <summary>
        /// Deletes the selected car from the data source.
        /// </summary>
        /// <param name="selectedCar">The selected car.</param>
        /// <returns>True if update was successful</returns>
        public bool DeleteCar(CarModel selectedCar)
        {
            // Remove car from database
            if (Cars.Contains(selectedCar))
                Cars.Remove(selectedCar);

            return SaveCars();            
        }

        #endregion

        #region Data mocks

        public List<CarModel> Cars { get; set; }

        private bool SaveCars()
        {
            try 
            { 
                string json = JsonConvert.SerializeObject(Cars);
                File.WriteAllText(Path.GetFullPath(@"DataBaseMock/Cars.json"), json);
                return true;
            }
            catch (Exception ex)
            {
                // Log error on accessing file/database
                return false;
            }
        }

        #endregion
    }
}
