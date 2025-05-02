using ClassRoomClone_App.Server.DTOs;
using ClassRoomClone_App.Server.Models;
using ClassRoomClone_App.Server.Repositories.Interfaces;
using ClassRoomClone_App.Server.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassRoomClone_App.Server.Services.Implements
{
    public class ClassService : IClassService
    {
        private readonly IClassRepository _classRepository;
        private readonly IClassParticipantsRepository _participantsRepository;

        public ClassService(IClassRepository classRepository, IClassParticipantsRepository participantsRepository)
        {
            _classRepository = classRepository;
            _participantsRepository = participantsRepository;
        }

        public async Task<IEnumerable<ClassResponseDto>> GetAllClassesAsync()
        {
            var classes = await _classRepository.GetAllClassesAsync();
            return classes.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<ClassResponseDto>> GetArchivedClassesAsync()
        {
            var classes = await _classRepository.GetArchivedClassesAsync();
            return classes.Select(MapToResponseDto);
        }

        public async Task<ClassResponseDto> GetClassByIdAsync(int id)
        {
            var entity = await _classRepository.GetClassByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"Class with ID {id} not found.");

            return MapToResponseDto(entity);
        }

        public async Task<ClassDetailsResponseDto> GetClassDetailsAsync(int id)
        {
            var entity = await _classRepository.GetClassDetailsAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"Class with ID {id} not found.");

            var participants = entity.ClassParticipants.Select(cp => new ClassParticipantResponseDto
            {
                UserId = cp.UserId,
                UserName = cp.User.Name,
                Email = cp.User.Email,
                Role = cp.Role,
                IsOwner = cp.IsOwner,
                AddedAt = cp.AddedAt
            }).ToList();

            return new ClassDetailsResponseDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Section = entity.Section,
                Subject = entity.Subject,
                Room = entity.Room,
                CreatedBy = entity.CreatedBy,
                CreatedDate = entity.CreatedDate,
                Participants = participants
            };
        }

        public async Task<ClassResponseDto> AddClassAsync(ClassRequestDto requestDto, int userId)
        {
            var entity = new Class
            {
                Name = requestDto.Name,
                Section = requestDto.Section,
                Subject = requestDto.Subject,
                Room = requestDto.Room,
                CreatedBy = userId,
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false
            };

            var created = await _classRepository.AddClassAsync(entity);

            // Assign main teacher
            await _participantsRepository.SetMainTeacherAsync(userId, created.Id);

            return MapToResponseDto(created);
        }

        public async Task<ClassResponseDto> UpdateClassAsync(int id, ClassRequestDto requestDto)
        {
            var entity = new Class
            {
                Id = id,
                Name = requestDto.Name,
                Section = requestDto.Section,
                Subject = requestDto.Subject,
                Room = requestDto.Room
            };

            var updated = await _classRepository.UpdateClassAsync(entity);
            if (updated == null)
                throw new KeyNotFoundException($"Class with ID {id} not found.");

            return MapToResponseDto(updated);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _classRepository.DeleteAsync(id);
        }

        private static ClassResponseDto MapToResponseDto(Class entity)
        {
            return new ClassResponseDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Section = entity.Section,
                Subject = entity.Subject,
                Room = entity.Room,
                CreatedBy = entity.CreatedBy,
                CreatedDate = entity.CreatedDate
            };
        }
    }
}
