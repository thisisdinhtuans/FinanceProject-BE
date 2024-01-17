using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Stock;
using api.Models;
using AutoMapper;

namespace api.Helpers
{
    public class MappingProfiles:Profile
    {
        public MappingProfiles()
        {
            CreateMap<Stock, StockDto>().ReverseMap();;
            // CreateMap<StockDto, Stock>();
        }
    }
}