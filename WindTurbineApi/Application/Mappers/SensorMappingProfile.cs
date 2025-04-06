using AutoMapper;
using WindTurbine.Domain.Entities;
using WindTurbineApi.Application.DTOs;

namespace WindTurbineApi.Application.Mappers
{
    public class SensorMappingProfile: Profile
    {
        public SensorMappingProfile()
        {
            CreateMap<Sensor, SensorDto>();
            CreateMap<CreateSensorDto, Sensor>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
               .ForMember(dest => dest.SensorRecords, opt => opt.Ignore());
        }
    }
}
