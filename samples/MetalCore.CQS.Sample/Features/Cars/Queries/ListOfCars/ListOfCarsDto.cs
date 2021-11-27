namespace MetalCore.CQS.Sample.Features.Cars.Queries.ListOfCars
{
    /// <summary>
    /// This is the data that comes back from the query.
    /// </summary>
    public class ListOfCarsDto
    {
        public string Make { get; set; }
        public string Model { get; set; }

        public override string ToString()
        {
            return $"{Make} - {Model}";
        }
    }
}
