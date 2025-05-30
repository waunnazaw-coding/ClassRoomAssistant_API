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
        private const string AllowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private const int ClassCodeLength = 6;
        
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

        public async Task<IEnumerable<ClassDetailsWithEntityId>> GetClassDetailsWithEntityIdAsync(int classId)
        {
            return await _classRepository.GetClassDetailsWithEntityIdAsync(classId);
        }

        public async Task<IEnumerable<UserClassesRawDto>> GetClassesByUserId(int userId)
        {
            return  await _classRepository.GetClassesByUserId(userId);
        }

        public async Task<ClassResponseDto> GetClassByIdAsync(int id)
        {
            var entity = await _classRepository.GetClassByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"Class with ID {id} not found.");

            return MapToResponseDto(entity);
        }

        public async Task<GetClassDetailsResponse> GetClassDetailsAsync(int classId)
        {
            var details = await _classRepository.GetClassDetailsAsync(classId);

            return new GetClassDetailsResponse
            {
                Details = details
            };
        }

        public async Task<bool> ApproveParticipantAsync(int userId, int classId)
        {
            return await _classRepository.ApproveParticipantAsync(userId, classId);
        }

        public async Task<ClassResponseDto> AddClassAsync(ClassRequestDto requestDto, int userId)
        {
            string classCode = await GenerateUniqueClassCodeAsync();
            
            var entity = new Class
            {
                Name = requestDto.Name,
                Section = requestDto.Section,
                Subject = requestDto.Subject,
                Room = requestDto.Room,
                ClassCode = classCode,
                CreatedBy = userId,
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false
            };

            var created = await _classRepository.AddClassAsync(entity);

            // Assign main teacher
            await _participantsRepository.SetMainTeacherAsync(userId, created.Id);

            return MapToResponseDto(created);
        }
        
        public async Task<ClassResponseDto?> GetClassByCodeAsync(string classCode)
        {
            var cls = await _classRepository.GetClassByCodeAsync(classCode);
            if (cls == null) return null;

            return MapToResponseDto(cls);
        }
        
        public async Task<bool> EnrollStudentInClassAsync(string classCode, int studentId)
        {
            var cls = await _classRepository.GetClassByCodeAsync(classCode);
            if (cls == null) return false;

            bool alreadyEnrolled = await _classRepository.StudentExistsInClassAsync(cls.Id, studentId);
            if (alreadyEnrolled) return false;

            var participant = new ClassParticipant
            {
                ClassId = cls.Id,
                UserId = studentId,
                Role = "Student", // or enum
                AddedAt = DateTime.UtcNow
            };

            await _classRepository.AddClassParticipantAsync(participant);
            return true;
        }

        public async Task<ClassResponseDto> UpdateClassAsync(int id, ClassUpdateRequestDto requestDto)
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
        
        
        public async Task<bool> RestoreAsync(int id)
        {
            return await _classRepository.RestoreAsync(id);
        }

        public async Task<bool> ActualDeleteAsync(int id)
        {
            return await _classRepository.ActualDeleteAsync(id);
        }


        private static ClassResponseDto MapToResponseDto(Class entity)
        {
            return new ClassResponseDto
            {
                Id = entity.Id,
                Name = entity.Name,
                ClassCode = entity.ClassCode,
                Section = entity.Section,
                Subject = entity.Subject,
                Room = entity.Room,
                CreatedBy = entity.CreatedBy,
                CreatedDate = entity.CreatedDate
            };
        }
        
        private async Task<string> GenerateUniqueClassCodeAsync()
        {
            string code;
            bool exists;

            do
            {
                code = GenerateRandomCode(ClassCodeLength);
                exists = await _classRepository.ClassCodeExistsAsync(code);
            } 
            while (exists);

            return code;
        }

        private string GenerateRandomCode(int length)
        {
            var random = new Random();
            var chars = new char[length];

            for (int i = 0; i < length; i++)
            {
                chars[i] = AllowedChars[random.Next(AllowedChars.Length)];
            }

            return new string(chars);
        }

    }
}
