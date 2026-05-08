using Application.Common.Pagination;
using Application.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponseDto>> GetAllAsync(ProductQueryParams queryParams);
        Task<ProductResponseDto?> GetByIdAsync(int id);
        Task<ProductResponseDto> CreateAsync(ProductDto dto);
        Task<ProductResponseDto?> UpdateAsync(int id, ProductDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
