using AutoMapper;
using System.Globalization;
using WindTurbine.Domain.Entities;
using WindTurbineApi.Application.DTOs;
using WindTurbineApi.Domain.ValueObjects;

namespace WindTurbineApi.Application.Mappers
{
    public class SensorRecordMappingProfile: Profile
    {
        public SensorRecordMappingProfile()
        {
            CreateMap<SensorRecord, SensorRecordDto>()
                .ForMember(dest => dest.Temperature, opt => opt.MapFrom(src => src.Reading.Value));

            CreateMap<CreateSensorRecordDto, SensorRecord>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.SensorId, opt => opt.Ignore()) // Will be set based on SensorName lookup in service
                .ForMember(dest => dest.Sensor, opt => opt.Ignore())   // Navigation property, will be handled in service
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => DateTime.ParseExact(src.Timestamp, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None).ToUniversalTime()))
                .ForMember(dest => dest.Reading, opt => opt.MapFrom(src => new ReadingValue(src.Temperature, Domain.Enums.ReadingUnit.Celsius))); // Default to celsius for this demo
        }
    }
}
