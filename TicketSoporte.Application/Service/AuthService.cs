
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TicketSoporte.Application.DTOs.Usuarios;
using TicketSoporte.Application.Interface.Repository;
using TicketSoporte.Application.Interface.Service;
using TicketSoporte.Application.Response;
using TicketSoporte.Domain.Entites;

namespace TicketSoporte.Application.Service
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<Usuarios> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly IConfiguration _config;
        private readonly IRefreshTokenRepository _refresRepo;
        private readonly IMapper _mapper;

        public AuthService(UserManager<Usuarios> userManager, RoleManager<IdentityRole<int>> roleManager, IConfiguration config, IRefreshTokenRepository refresRepo, IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
            _refresRepo = refresRepo;
            _mapper = mapper;
        }

        #region Métodos Privados

        // Convierte un ApplicationUser => UsuarioDto
        // Además obtiene el rol desde Identity
        private async Task<UsuariosDto> MapearUsuarioDtoAsync(Usuarios usuario)
        {
            var roles = await _userManager.GetRolesAsync(usuario);

            return new UsuariosDto
            {
                Id = usuario.Id,
                UserName = usuario.UserName!,
                Email = usuario.Email!,
                Rol = roles.FirstOrDefault() ?? "",
                PhoneNumber = usuario.PhoneNumber ?? "",
                NombreCompleto = usuario.NombreCompleto
            };
        }

        // Valida si una operación de Identity fue existosa, en caso de falla:
        // une todos los errores
        // y lanza excepción con un mensaje claro
        private static void ValidarResultado(IdentityResult resultado, string mensajeError)
        {
            if (!resultado.Succeeded)
            {
                var errores = string.Join(" | ", resultado.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"{mensajeError}: '{errores}'");
            }
        }

        private string GenerarToken(Usuarios usuario, string rol, DateTime expiracion)
        {
            // Leer configuración de variables de entorno
            var key = _config["JWT_KEY"]
                ?? throw new Exception("JWT_KEY no configurado");

            var issuer = _config["JWT_ISSUER"]
                ?? throw new Exception("JWT_ISSUER no configurado");

            var audience = _config["JWT_AUDIENCE"]
                ?? throw new Exception("JWT_AUDIENCE no configurado");


            // Convertir la clave secreta a bytes
            // Esto es necesario para crear la firma del token
            var keyBytes = Encoding.ASCII.GetBytes(key!);


            // Crear claims (información que incluirá el token)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.UserName ?? ""),
                new Claim(ClaimTypes.Email, usuario.Email ?? ""),
                new Claim(ClaimTypes.Role, rol)
            };


            // Preparar el descriptor del token
            // Aquí se define la información del token: claims, expiración, issuer, audience y firma
            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),           // Claim que incluirá el token
                Expires = expiracion,                          // El token expira en 15 minutos
                Issuer = issuer,                                // Quién emite el token
                Audience = audience,                            // A quien va dirigido el token
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(keyBytes),         // Clave secreta en bytes
                    SecurityAlgorithms.HmacSha256Signature)     // Algoritmo de firma HMAC SHA256
            };

            // Generar el token
            var handler = new JwtSecurityTokenHandler();
            return handler.WriteToken(handler.CreateToken(descriptor));

            // Devolver el token como string
            //return tokenHandler.WriteToken(token);
        }

        // Genera 64 bots aleatorios, los convierte a string
        private string GenerarRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        #endregion

        public async Task<RespuestaLoginDto> LoginAsync(UsuariosLoginDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "El campo es requerido.");

            var email = dto.Email.Trim().ToLower();

            var usuario = await _userManager.FindByEmailAsync(email);
            if (usuario == null)
                throw new UnauthorizedAccessException("Usuario no registrado.");

            if (!await _userManager.CheckPasswordAsync(usuario, dto.Password))
                throw new UnauthorizedAccessException("Contraseña es incorrecta. Verifique por favor.");

            var roles = await _userManager.GetRolesAsync(usuario);
            var rol = roles.FirstOrDefault() ?? "Usuario";

            var expiracion = DateTime.UtcNow.AddMinutes(15);
            var jwt = GenerarToken(usuario, rol, expiracion);
            var refresh = GenerarRefreshToken();

            var refreshEntity = new RefreshToken
            {
                Token = refresh,
                UsuarioId = usuario.Id,
                Expiracion = DateTime.UtcNow.AddDays(7)
            };

            await _refresRepo.GuardarAsync(refreshEntity);

            return new RespuestaLoginDto
            {
                Usuario = await MapearUsuarioDtoAsync(usuario),
                AccessToken = jwt,
                RefreshToken = refresh,
                ExpiraEn = expiracion
            };
        }

        public async Task<RespuestaLoginDto> RefreshTokenAsync(string refreshToken)
        {
            var tokenDB = await _refresRepo.ObtenerAsync(refreshToken);
            if (tokenDB == null || tokenDB.Expiracion < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Refresh token inválido.");

            var usuario = tokenDB.Usuario;
            var rol = (await _userManager.GetRolesAsync(usuario)).FirstOrDefault() ?? "";

            var expiracion = DateTime.UtcNow.AddMinutes(15);

            var nuevoJwt = GenerarToken(usuario, rol, expiracion);
            var nuevoRefresh = GenerarRefreshToken();

            tokenDB.Token = nuevoRefresh;
            tokenDB.Expiracion = DateTime.UtcNow.AddDays(7);

            await _refresRepo.ActualizarAsync(tokenDB);

            return new RespuestaLoginDto
            {
                Usuario = await MapearUsuarioDtoAsync(usuario),
                AccessToken = nuevoJwt,
                RefreshToken = refreshToken,
                ExpiraEn = expiracion
            };
        }

        public async Task<UsuariosDto> RegistrarUsuarioAsync(UsuariosRegistroDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "El campo es requerido.");

            dto.Email = dto.Email.Trim().ToLower();

            var existeEmail = await _userManager.FindByEmailAsync(dto.Email);
            if (existeEmail != null)
                throw new InvalidOperationException("El email ya se encuentra registrado.");

            var rolExistente = await _roleManager.RoleExistsAsync(dto.Rol);
            if (!rolExistente)
                throw new InvalidOperationException($"El rol '{dto.Rol}' no existe.");

            var usuario = _mapper.Map<Usuarios>(dto);
            usuario.Email = dto.Email;
            usuario.UserName = dto.Email;

            usuario.EmailConfirmed = true;
            usuario.PhoneNumberConfirmed = true;

            // Crear el usuario
            var usuarioCreado = await _userManager.CreateAsync(usuario, dto.Password);
            ValidarResultado(usuarioCreado, "Error al registrar el usuario");

            // Asignar el rol al usuario
            var rolAsignado = await _userManager.AddToRoleAsync(usuario, dto.Rol);
            ValidarResultado(rolAsignado, "Error al asignar el rol");

            return await MapearUsuarioDtoAsync(usuario);
        }
    }
}
