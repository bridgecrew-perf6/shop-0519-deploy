using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shop.Services;

namespace Shop.Controllers
{
    [Route("v1/users")]
    public class UserControllers : Controller
    {
        [HttpGet]
        [Route("")]
        [Authorize(Roles = "Desenvolvedor")]
        [ResponseCache(VaryByHeader = "User-Agent", Location = ResponseCacheLocation.Any, Duration = 30)]
        public async Task<ActionResult<List<User>>> Get(
            [FromServices] DataContext context
            )
        {
            var user = await context
                .Users
                .AsNoTracking()
                .ToListAsync();
            return Ok(user);
        }

        [HttpPost]
        [Route("")]
        [AllowAnonymous]
        [ResponseCache(VaryByHeader = "User-Agent", Location = ResponseCacheLocation.Any, Duration = 30)]
        public async Task<ActionResult<User>> Post(
            [FromBody] User model,
            [FromServices] DataContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                model.Role = "Desenvolvedor";
                context.Users.Add(model);
                await context.SaveChangesAsync();

                model.Password = "";
                return Ok(model);
            }
            catch
            {
                return BadRequest(new { message = "Não foi possível cadastrar o usuário!" });
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "Desenvolvedor")]
        [ResponseCache(VaryByHeader = "User-Agent", Location = ResponseCacheLocation.Any, Duration = 30)]
        public async Task<ActionResult<User>> Put(
            int id,
            [FromBody] User model,
            [FromServices] DataContext context)
        {
            if (id != model.Id)

                //Verifica se o ID informado é o mesmo do modelo
                return NotFound(new { message = "Uusário não encontrada!" });

            //Valida se os dados informados atendem os requisitos do Category.Models
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Entry<User>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Ok(model);
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "Este Registro já foi atualizado!" });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível atualizar o registro do usuário!" });
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "Desenvolvedor")]
        [ResponseCache(VaryByHeader = "User-Agent", Location = ResponseCacheLocation.Any, Duration = 30)]
        public async Task<ActionResult<User>> Delete(
            int id,
            [FromServices] DataContext context

            )
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
                return NotFound(new { message = "Usuário não encontrada!" });

            try
            {
                context.Users.Remove(user);
                await context.SaveChangesAsync();
                return Ok(new { message = "Usuário removido com sucesso!" });

            }
            catch
            {
                return BadRequest(new { message = "Não foi possível remover o usuário!" });
            }
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        [ResponseCache(VaryByHeader = "User-Agent", Location = ResponseCacheLocation.Any, Duration = 30)]
        public async Task<ActionResult<dynamic>> Authenticate(
            [FromBody] User model,
            [FromServices] DataContext context)
        {
            var user = await context.Users
                .AsNoTracking()
                .Where(x => x.Username == model.Username && x.Password == model.Password)
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound(new { message = "Usuário ou senha inválidos!" });

            var token = TokernService.GenerateToken(user);
            user.Password = "";
            return new
            {
                user = user,
                token = token
            };
        }
    }
}
