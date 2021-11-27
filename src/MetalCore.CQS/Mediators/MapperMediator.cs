using MetalCore.CQS.Mapper;
using MetalCore.CQS.Validation;
using System;

namespace MetalCore.CQS.Mediators
{
    /// <summary>
    /// This class handles calling the correct mapper automatically.
    /// </summary>
    public class MapperMediator : IMapperMediator
    {
        private readonly Func<Type, dynamic> _getInstanceCallback;

        /// <summary>
        /// Constructor that takes an anonymous function to lookup an instance from the outside.
        /// </summary>
        /// <param name="getInstanceCallback">The callback to get an instance of a dynamic type.</param>
        public MapperMediator(Func<Type, dynamic> getInstanceCallback)
        {
            Guard.IsNotNull(getInstanceCallback, nameof(getInstanceCallback));

            _getInstanceCallback = getInstanceCallback;
        }

        /// <summary>
        /// Returns a mapped type.
        /// </summary>
        /// <typeparam name="TTo">The type of object being returned.</typeparam>
        /// <param name="from">The source data.</param>
        /// <param name="to">The initial data to map to.</param>
        /// <returns>The target data.</returns>
        public TTo Map<TTo>(object from, TTo to = default)
        {
            Guard.IsNotNull(from, nameof(from));

            Type fromType = from.GetType();
            Type type = typeof(IMapper<,>).MakeGenericType(fromType, typeof(TTo));
            dynamic instance = _getInstanceCallback(type);
            if (instance == null)
            {
                throw new TypeLoadException(
                    $"No mapper type found for map {fromType.FullName} -> {typeof(TTo).FullName}");
            }

            dynamic specificFrom = Convert.ChangeType(from, fromType);
            return instance.Map(specificFrom, to);
        }
    }
}
