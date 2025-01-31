﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shop.Controllers
{
    [Route("v1/categories")]
    public class CategoryController : ControllerBase
    {

        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        [ResponseCache(VaryByHeader = "User-Agent", Location = ResponseCacheLocation.Any, Duration = 30)]
        public async Task<ActionResult<List<Category>>> Get(
            [FromServices]DataContext context
            )
        {
           var categories = await context.Categories.AsNoTracking().ToListAsync();
           return Ok (categories);
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        [ResponseCache(VaryByHeader = "User-Agent", Location = ResponseCacheLocation.Any, Duration = 30)]
        public async Task<ActionResult<Category>> GetById(
            [FromServices]DataContext context,
            int id)
        {
            var categories = await context.Categories.AsNoTracking().FirstOrDefaultAsync();
            return Ok(categories);
            
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "Desenvolvedor")]
        [ResponseCache(VaryByHeader = "User-Agent", Location = ResponseCacheLocation.Any, Duration = 30)]
        public async Task<ActionResult<List<Category>>> Post(
            [FromBody]Category model,
            [FromServices]DataContext context)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Categories.Add(model);
                await context.SaveChangesAsync();

                return Ok(model);
            }
            catch
            {
                return BadRequest(new { message = "Não foi possível criar a categoria!" });
            }
        }


        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "Desenvolvedor")]
        [ResponseCache(VaryByHeader = "User-Agent", Location = ResponseCacheLocation.Any, Duration = 30)]
        public async Task<ActionResult<List<Category>>> Put(
            int id,
            [FromBody]Category model,
            [FromServices] DataContext context
            )
        {
            if(id!= model.Id)

                //Verifica se o ID informado é o mesmo do modelo
                return NotFound(new { message = "Categoria não encontrada!"});

            //Valida se os dados informados atendem os requisitos do Category.Models
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Entry<Category>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Ok (model);
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest( new {message = "Este registro já foi atualizado!"});
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível atualizar a categoria!" });
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "Desenvolvedor")]
        [ResponseCache(VaryByHeader = "User-Agent", Location = ResponseCacheLocation.Any, Duration = 30)]
        public async Task<ActionResult<List<Category>>> Delete(
            int id,
            [FromServices]DataContext context

            )
        {
                var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
                if (category == null)
                    return NotFound(new { message = "Categoria não encontrada!" });

            try
            {
                context.Categories.Remove(category);
                await context.SaveChangesAsync();
                return Ok(new { message = "Categoria removida com sucesso!" });
               
            }
            catch
            {
                return BadRequest(new { message = "Não foi possível remover a categoria!" });
            }
        }


    }
}
