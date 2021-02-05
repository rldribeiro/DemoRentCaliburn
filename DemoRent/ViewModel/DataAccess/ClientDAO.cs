using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.DataAccess
{
    public sealed class ClientDAO
    {
        #region Singleton implementation

        private static ClientDAO instance;
        private static readonly object lockObj = new Object();

        private ClientDAO()
        {
        }

        public static ClientDAO Instance
        {
            get
            {
                lock (lockObj)
                {
                    if (null == instance)
                        instance = new ClientDAO();

                    return instance;
                }                
            }
        }

        #endregion

        #region Data Access Methods

        #endregion
    }
}
