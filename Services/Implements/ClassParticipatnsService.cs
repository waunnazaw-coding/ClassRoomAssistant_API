using ClassRoomClone_App.Server.DTOs;
using ClassRoomClone_App.Server.Models;
using ClassRoomClone_App.Server.Repositories.Interfaces;
using ClassRoomClone_App.Server.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassRoomClone_App.Server.Services.Implements
{
    public class ClassParticipantsService : IClassParticipantsService
    {
        private readonly IClassParticipantsRepository _participantsRepository;
        private readonly DbContextClassName _context;

        public ClassParticipantsService(IClassParticipantsRepository participantsRepository, DbContextClassName context)
        {
            _participantsRepository = participantsRepository;
            _context = context;
        }

        public async Task<IEnumerable<ClassParticipantResponseDto>> GetAllParticipantsAsync(int classId)
        {
            var participants = await _participantsRepository.GetAllParticipantsAsync(classId);
            return participants.Select(MapToDto);
        }

        public async Task<ClassParticipantResponseDto> SetMainTeacherAsync(int userId, int classId)
        {
            var model = await _participantsRepository.SetMainTeacherAsync(userId, classId);
            if (model == null)
                throw new Exception($"Failed to set main teacher with UserId {userId} in ClassId {classId}.");

            return await MapToDtoAsync(model);
        }

        public async Task<ClassParticipantResponseDto> AddSubTeacherAsync(int userId, int classId)
        {
            var model = await _participantsRepository.AddSubTeacherAsync(userId, classId);
            if (model == null)
                throw new Exception($"Failed to add sub-teacher with UserId {userId} to ClassId {classId}.");

            return await MapToDtoAsync(model);
        }

        public async Task<bool> TransferOwnershipAsync(int classId, int currentOwnerId, int newOwnerId)
        {
            return await _participantsRepository.TransferOwnershipAsync(classId, currentOwnerId, newOwnerId);
        }

        public async Task<bool> RemoveSubTeacherAsync(int userId, int classId)
        {
            return await _participantsRepository.RemoveSubTeacherAsync(userId, classId);
        }

        public async Task<bool> RemoveStudentAsync(int userId, int classId)
        {
            return await _participantsRepository.RemoveStudentAsync(userId, classId);
        }

        public async Task<ClassParticipantResponseDto> AddStudentAsync(int userId, int classId)
        {
            var model = await _participantsRepository.AddStudentAsync(userId, classId);
            if (model == null)
                throw new Exception($"Failed to add student with UserId {userId} to ClassId {classId}.");

            return await MapToDtoAsync(model);
        }

        // Map entity to DTO synchronously for collections
        private ClassParticipantResponseDto MapToDto(ClassParticipant model)
        {
            var user = _context.Users.AsNoTracking().SingleOrDefault(u => u.Id == model.UserId);

            return new ClassParticipantResponseDto
            {
                //UserId = model.UserId,
                UserName = user?.Name ?? "Unknown",
                Email = user?.Email ?? "Unknown",
                Role = model.Role,
                //IsOwner = model.IsOwner,
                AddedAt = model.AddedAt
            };
        }

        // Async version for single entity mapping (better for async EF calls)
        private async Task<ClassParticipantResponseDto> MapToDtoAsync(ClassParticipant model)
        {
            var user = await _context.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Id == model.UserId);

            return new ClassParticipantResponseDto
            {
                //UserId = model.UserId,
                UserName = user?.Name ?? "Unknown",
                Email = user?.Email ?? "Unknown",
                Role = model.Role,
                //IsOwner = model.IsOwner,
                AddedAt = model.AddedAt
            };
        }
    }
}
