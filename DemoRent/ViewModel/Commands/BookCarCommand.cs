using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ViewModel.Commands
{
    public class BookCarCommand : ICommand
    {
        public BookCarCommand(CarVM vm)
        {
            VM = vm;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public CarVM VM { get; set; }

        public bool CanExecute(object parameter)
        {
            return VM.SelectedCar != null &&
                   VM.ContractedKms > 0 &&
                   VM.ReturnDate > VM.PickupDate &&
                   !VM.BookingConflict;
        }

        public void Execute(object parameter)
        {
            VM.BookCar();
        }
    }
}
