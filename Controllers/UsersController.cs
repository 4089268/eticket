using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using FluentValidation;
using eticket.Data;
using eticket.Adapters;
using eticket.ViewModels;
using eticket.Services;

namespace eticket.Controllers
{

    [Authorize]
    [Route("/{Controller}")]
    public class UsersController(
        ILogger<UsersController> logger,
        TicketsDBContext context,
        UserService us,
        IValidator<UsuarioRequest> validator,
        IValidator<EditarUsuarioRequest> validator2
    ) : Controller
    {
        private readonly ILogger<UsersController> logger = logger;
        private readonly TicketsDBContext ticketsDBContext = context;
        private readonly UserService userService = us;
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

            // * obtener catalogos
            CargarCatalogos();

            EditarUsuarioRequest editarUsuarioRequest = user.ToUserEditRequest();
            editarUsuarioRequest.Oficinas = this.ticketsDBContext.UsuarioOficinas.Where(e => e.IdUsuario == user.IdUsuario).Select(o => o.IdOficina).ToList();
            return View(editarUsuarioRequest);
        }

        [HttpPost("{userId}/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarUsuario([FromRoute] int userId, [FromForm] EditarUsuarioRequest request)
        {
            var validationResults = await this.editUsuarioValidator.ValidateAsync(request);
            if (!validationResults.IsValid)
            {
                foreach (var error in validationResults.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                CargarCatalogos();
                return View("EditarUsuario", request);
            }

            // update the user
            try
            {
                await this.userService.ActualizarUsuario(request);
            }
            catch (KeyNotFoundException kex)
            {
                return NotFound(new
                {
                    Title = "El usuario no se encontro en el sistema.",
                    kex.Message
                });
            }
            catch (System.Exception ex)
            {
                return NotFound(new
                {
                    Title = "Error no controlado al actualizar el usuario",
                    ex.Message
                });
            }

            return RedirectToAction("Index");
        }

        #region Partial Views
        [HttpGet("partial/nuevo-usuario")]
        public ActionResult NuevoUsuarioForm()
        {
            return PartialView("~/Views/Users/Partials/NuevoUsuarioForm.cshtml");
        }
        #endregion


        #region Private Functions
        private void CargarCatalogos()
        {
            // * obtener catalogo niveles-usuario
            var nivelesSelectList = this.ticketsDBContext.CatNivelesUsuarios
                .Select(n => new SelectListItem
                {
                    Text = n.Nombre.ToString(),
                    Value = n.IdNivel.ToString()
                });
            ViewBag.NivelesUsuarioSelectList = nivelesSelectList;

            // * obtener catalogo oficinas
            var oficinasSelectList = this.ticketsDBContext.CatOficinas
                .Select(o => new SelectListItem
                {
                    Text = o.Oficina.ToString(),
                    Value = o.Id.ToString()
                });
            ViewBag.OficinaSelectList = oficinasSelectList;
        }
        #endregion
    }
}
