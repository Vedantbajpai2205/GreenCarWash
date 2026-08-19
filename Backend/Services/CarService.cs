using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
    public class CarService : ICarService
    {
        private readonly ICarRepository _repository;

        public CarService(ICarRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CarResponseDto>> GetAllCarsAsync(string userId)
        {
            var cars = await _repository.GetAllByUserIdAsync(userId);
            return cars.Select(car => new CarResponseDto
            {
                Id = car.Id,
                Make = car.Make,
                Model = car.Model,
                Year = car.Year,
                IsActive = car.IsActive
            });
        }

        public async Task<CarResponseDto> GetCarByIdAsync(int id, string userId)
        {
            var car = await _repository.GetByIdAndUserIdAsync(id, userId);
            if (car == null) return null;
            return new CarResponseDto
            {
                Id = car.Id,
                Make = car.Make,
                Model = car.Model,
                Year = car.Year,
                IsActive = car.IsActive
            };
        }

        public async Task<CarResponseDto> CreateCarAsync(CarCreateDto createDto, string userId)
        {
            var car = new Car
            {
                Make = createDto.Make,
                Model = createDto.Model,
                Year = createDto.Year,
                IsActive = true,   // default new cars to active
                UserId = userId
            };

            await _repository.AddAsync(car);
            // After AddAsync, car.Id should be set by the DbContext
            return new CarResponseDto
            {
                Id = car.Id,
                Make = car.Make,
                Model = car.Model,
                Year = car.Year,
                IsActive = car.IsActive
            };
        }

        public async Task<bool> UpdateCarAsync(CarUpdateDto updateDto, string userId)
        {
            var car = await _repository.GetByIdAndUserIdAsync(updateDto.Id, userId);
            if (car == null) return false;

            car.Make = updateDto.Make;
            car.Model = updateDto.Model;
            car.Year = updateDto.Year;
            car.IsActive = updateDto.IsActive;

            await _repository.UpdateAsync(car);
            return true;
        }

        public async Task<bool> DeleteCarAsync(int id, string userId)
        {
            var car = await _repository.GetByIdAndUserIdAsync(id, userId);
            if (car == null) return false;

            await _repository.DeleteAsync(car);
            return true;
        }
    }