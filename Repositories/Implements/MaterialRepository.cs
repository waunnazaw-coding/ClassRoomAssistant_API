using System.Xml;
using ClassRoomClone_App.Server.Models;
using ClassRoomClone_App.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClassRoomClone_App.Server.Repositories.Implements;

public class MaterialRepository : IMaterialRepository
{
    private readonly DbContextClassName _context;

    public MaterialRepository(DbContextClassName context)
    {
        _context = context;
    }

    public async Task<Material?> GetByIdWithAttachmentsAsync(int materialId)
    {
        var entities =  await _context.Materials
            .AsNoTracking()
            .Include(m => m.ClassWork)
            //.Include(m => m.Attachments) 
            //.ThenInclude(cw => cw.Attachments)
            .FirstOrDefaultAsync(m => m.Id == materialId);

        return entities;
    }
    public async Task<Material> AddAsync(Material material)
    {
        await _context.Materials.AddAsync(material);
        await _context.SaveChangesAsync();
        return material;
    }
    
    public async Task<Material?> GetByIdAsync(int materialId)
    {
        return await _context.Materials.FindAsync(materialId);
    }

    public async Task<Material> UpdateAsync(Material material)
    {
        _context.Materials.Update(material);
        await _context.SaveChangesAsync();
        return material;
    }

    public async Task<bool> DeleteAsync(int materialId)
    {
        var material = await _context.Materials.FindAsync(materialId);
        if (material == null) return false;

        _context.Materials.Remove(material);
        await _context.SaveChangesAsync();
        return true;
    }
}
