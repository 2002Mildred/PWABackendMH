using App_Xamarin_Firebase.Models;
using App_Xamarin_Firebase.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Microsoft.Win32;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App_Xamarin_Firebase.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipeController : ControllerBase
    {
        private readonly RecipeService recipeService;

        public RecipeController(RecipeService recipeService)
        {
            this.recipeService = recipeService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Recipe>>> GetAllRecipes()
        {
            var recipes = await recipeService.GetAllRecipes();
            return Ok(recipes);
        }

        [HttpPost]
        public async Task<ActionResult<Recipe>> CreateRecipe(Recipe recipe)
        {
            var createdRecipe = await recipeService.CreateRecipe(recipe);
            return Ok(createdRecipe);
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<List<Recipe>>> GetRecipesByCategory(string category)
        {
            var recipes = await recipeService.GetRecipesByCategory(category);
            return Ok(recipes);
        }

        [HttpGet("ingredients")]
        public async Task<ActionResult<List<Recipe>>> GetRecipesByIngredients([FromQuery] List<string> ingredients)
        {
            var recipes = await recipeService.GetRecipesByIngredients(ingredients);
            if (recipes.Count == 0)
            {
                return NotFound();
            }
            return Ok(recipes);
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<Recipe>>> SearchRecipesByName([FromQuery] string recipeName)
        {
            var recipes = await recipeService.SearchRecipesByName(recipeName);
            if (recipes.Count == 0)
            {
               await GetAllRecipes();
            }
            return Ok(recipes);
        }

        [HttpGet("searchByUserId")]
        public async Task<ActionResult<List<Recipe>>> SearchRecipesByUserId([FromQuery] string uidUser)
        {
            var recipes = await recipeService.SearchRecipesByUserId(uidUser);
            if (recipes.Count == 0)
            {
                return NotFound();
            }
            return Ok(recipes);
        }




    }


}
