using System.Collections.Generic;

namespace MetalCore.CQS.Sample.DataStore
{
    public interface ICarDataStore
    {
        public ICollection<CarEntity> Cars { get; }
    }

    public class CarDataStore : ICarDataStore
    {
        public CarDataStore()
        {
            SetupInitialData();
        }

        public ICollection<CarEntity> Cars { get; private set; }

        private void SetupInitialData()
        {
            Cars = new List<CarEntity>
            {
                new CarEntity { Make = "Chevy", Model = "Malibu", Year = 2020 },
                new CarEntity { Make = "Chevy", Model = "Equinox", Year = 2020 },
                new CarEntity { Make = "Ford", Model = "F150", Year = 2021 },
                new CarEntity { Make = "Toyota", Model = "Camry", Year = 2016 }
            };
        }
    }
}
