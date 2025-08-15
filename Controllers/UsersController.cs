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
    public class UsersController(ILogger<UsersController> logger, TicketsDBContext context, IValidator<UsuarioRequest> validator) : Controller
    {
        private readonly ILogger<UsersController> logger = logger;
        private readonly TicketsDBContext ticketsDBContext = context;
        private readonly IValidator<UsuarioRequest> nuevoUsuarioValidator = validator;


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

        #region Partial Views
        [HttpGet("partial/nuevo-usuario")]
        public ActionResult NuevoUsuarioForm()
        {
            return PartialView("~/Views/Users/Partials/NuevoUsuarioForm.cshtml");
        }
        #endregion

    }
}
