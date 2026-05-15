using BCrypt.Net;
using iCarus.Src.Model;

namespace iCarus.Src.Db;

// Clase Seeder para poblar la base de datos con datos iniciales.

public class Seeder(ContextDb context)
{


    private readonly ContextDb _context = context;


 
    public async Task Seed()
    {
        // Se verifica si existen algun rol en la base de datos, en caso de que no existan, se crea uno de admin y uno de usuario
        if (!_context.Roles.Any())
        {
            var adminRole = new Role { Name = "Admin" };
            var userRole = new Role { Name = "User" };

            await _context.Roles.AddRangeAsync(adminRole, userRole);
            await _context.SaveChangesAsync();
        }

        // Se verifica si existe algun usuario en la base de datos, en caso de que no exista, se crea un usuario administrador y uno normal

        if (!_context.Users.Any())
        {
            var adminUser = new User
            {
                Name = "Admin",
                LastName = "User",
                Email = "admin@divine.cl",
                Rut = "12345678-9",
                Phone = "123456789",
                Password = BCrypt.Net.BCrypt.HashPassword("admin123"), 
                Status = true,
                RoleId = _context.Roles.First(r => r.Name == "Admin").Id
            };  
            var normalUser = new User
            {
                Name = "Normal",
                LastName = "User",
                Email = "user@divine.cl",
                Rut = "98765432-1",
                Phone = "987654321",
                Password = BCrypt.Net.BCrypt.HashPassword("user123"), // Hasheamos la contraseña para el usuario normal
                Status = true,
                RoleId = _context.Roles.First(r => r.Name == "User").Id
            };

            await _context.Users.AddRangeAsync(adminUser, normalUser);
            await _context.SaveChangesAsync();
        }
    }
}