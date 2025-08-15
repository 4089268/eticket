using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using eticket.Data;
using eticket.Adapters;
using eticket.ViewModels;
using Microsoft.EntityFrameworkCore;
using AspNetCoreGeneratedDocument;
using System.Threading.Tasks;

namespace eticket.Controllers
{

    [Authorize]
    [Route("/{Controller}")]
    public class UsersController(
        ILogger<UsersController> logger,
        TicketsDBContext context,
        IValidator<UsuarioRequest> validator,
        IValidator<EditarUsuarioRequest> validator2
    ) : Controller
    {
        private readonly ILogger<UsersController> logger = logger;
        private readonly TicketsDBContext ticketsDBContext = context;
        private readonly IValidator<UsuarioRequest> nuevoUsuarioValidator = validator;
        private readonly IValidator<EditarUsuarioRequest> editUsuarioValidator = validator2;


        [HttpGet]
        public IActionResult Index()
        {
            IEnumerable<UsuarioDTO> users = this.ticketsDBContext.SysUsuarios.ToList().Select(u => u.ToUserDTO());
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> StoreUser([FromForm] UsuarioRequest request)
        {
            // Validate the request
            var validationResults = await nuevoUsuarioValidator.ValidateAsync(request);
            if (!validationResults.IsValid)
            {
                return UnprocessableEntity(new
                {
                    errors = validationResults.Errors.Select(e => new
                    {
                        field = e.PropertyName,
                        message = e.ErrorMessage
                    })
                });
            }

            try
            {
                var sysUsuario = request.ToEntity();

                this.ticketsDBContext.SysUsuarios.Add(sysUsuario);
                this.ticketsDBContext.SaveChanges();
                this.logger.LogInformation("Nuevo usuario creado usuario=[{user}] id=[{id}]", sysUsuario.Usuario, sysUsuario.IdUsuario);

                return StatusCode(201, new
                {
                    Message = "Usuario creado con exito",
                    Usuario = sysUsuario.ToUserDTO()
                });
            }
            catch (System.Exception err)
            {
                this.logger.LogError(err, "Error al generar el nuevo usuario: {message}", err.Message);
                return Conflict(new
                {
                    Title = "Error al generar el usuario",
                    Message = err.Message
                });
            }
        }

        [HttpGet("{userId}/edit")]
        public IActionResult EditarUsuario([FromRoute] int userId)
        {
            var user = this.ticketsDBContext.SysUsuarios.FirstOrDefault(usu => usu.IdUsuario == userId);
            if (user == null)
            {
                return NotFound(new
                {
                    Message = "No se encontro el usuario seleccionado."
                });
            }

            EditarUsuarioRequest editarUsuarioRequest = user.ToUserEditRequest();
            return View(editarUsuarioRequest);
        }

        [HttpPost("{userId}/edit")]
        public async Task<IActionResult> ActualizarUsuario([FromRoute] int userId, [FromForm] EditarUsuarioRequest request)
        {
            var validationResults = await this.editUsuarioValidator.ValidateAsync(request);
            if (!validationResults.IsValid)
            {
                foreach (var error in validationResults.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View("EditarUsuario", request);
            }


            // update the user
            var user = this.ticketsDBContext.SysUsuarios.Find(userId);
            if (user == null)
            {
                return NotFound(new
                {
                    Title = "Usuario no encontrado",
                    Message = "El usuario no se encuentra registrado en el sistema"
                });
            }

            user.Usuario = request.Usuario!;
            user.Correo = request.Correo!;
            user.Nombre = request.Nombre!;
            user.Apellido = request.Apellido!;
            if (!string.IsNullOrEmpty(request.Contraseña))
            {
                user.Contraseña = BCrypt.Net.BCrypt.HashPassword(request.Contraseña);
            }
            this.ticketsDBContext.SysUsuarios.Update(user);
            this.ticketsDBContext.SaveChanges();
            this.logger.LogInformation("Usuario {userId} actualizado", user.IdUsuario);

            return RedirectToAction("Index");
        }

        #region Partial Views
        [HttpGet("partial/nuevo-usuario")]
        public ActionResult NuevoUsuarioForm()
        {
            return PartialView("~/Views/Users/Partials/NuevoUsuarioForm.cshtml");
        }
        #endregion

    }
}
