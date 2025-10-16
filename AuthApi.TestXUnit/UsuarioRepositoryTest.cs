using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthApi.Entidades;
using AuthApi.Repositorios;


namespace AuthApi.TestXunit
{
    public class UsuarioRepositoryTest
    {

        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDB_{System.Guid.NewGuid()}").Options;

            var context = new AppDbContext(options);

            context.Roles.Add(
                new Rol { Id = 1, Nombre = "Admin" }
            );

            context.Usuarios.Add(
                new Usuario
                {
                    Id = 2,
                    Nombre = "henry",
                    Email = "henry@test.com",
                    PasswordHash = "123",
                    RolId = 1
                }
            );

            context.SaveChanges();
            return context;
        }

        [Fact]
        public async Task GetByEmailAsync_RetornarUsuarioExistente()
        {
            //Arrange
            var context = GetInMemoryDbContext();
            var repo = new UsuarioRepository(context);

            //Act
            var usuario = await repo.GetByEmailAsync("henry@test.com");

            //Assert
            Assert.NotNull(usuario);
            Assert.Equal("henry", usuario.Nombre);
            Assert.Equal("Admin", usuario.Rol.Nombre);
        }
        [Fact]
        public async Task AddAsync_AgregarUsuario()
        {
            //Arrange
            var context = GetInMemoryDbContext();
            var repo = new UsuarioRepository(context);

            var nuevoUsuario = new Usuario()
            {
                Nombre = "eduardo",
                Email = "eduardo@test.com",
                PasswordHash = "123",
                RolId = 1
            };

            //Act
            await repo.AddAsync(nuevoUsuario);

            //Assert
            var usuarioGuardado = await context.Usuarios.FirstOrDefaultAsync(u => u.Email == "eduardo@test.com");

            Assert.NotNull(usuarioGuardado);
            Assert.Equal("eduardo", usuarioGuardado.Nombre);
        }
        [Fact]

        public async Task GetAllUsuariosAsync_RetornaListaUsuarios()
        {
            //Arrange
            var context = GetInMemoryDbContext();
            var repo = new UsuarioRepository(context);

            //Act
            var lista = await repo.GetAllUsuariosAsync();

            //Assert
            Assert.NotEmpty(lista);
            Assert.Contains(lista, u => u.Email == "henry@test.com");
        }
    }
}
