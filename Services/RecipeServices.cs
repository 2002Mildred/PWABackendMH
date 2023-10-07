using FireSharp.Interfaces;
using FireSharp;
using System.Collections.Generic;
using System.Threading.Tasks;
using FireSharp.Config;
using App_Xamarin_Firebase.Models;
using System.Linq;
using System;

namespace App_Xamarin_Firebase.Services
{
    public class RecipeService
    {
        private readonly FirebaseClient client;

        public RecipeService()
        {
            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = "yhMvDH9DX93xmp05w2EdKj01C7JOGH23cwWijeQ5",
                BasePath = "https://login-3b71d-default-rtdb.firebaseio.com/"
            };
            client = new FirebaseClient(config);
        }

        public async Task<List<Recipe>> GetAllRecipes()
        {
            var response = await client.GetAsync("recipes");
            var recipes = response.ResultAs<Dictionary<string, Recipe>>();

            return recipes.Select(r => new Recipe
            {
                Id = r.Key,
                Name = r.Value.Name,
                Ingredients = r.Value.Ingredients,
                Category = r.Value.Category,
                PreparationSteps = r.Value.PreparationSteps,
                UidUser = r.Value.UidUser,
                Image = r.Value.Image,
                time = r.Value.time
            }).ToList();
        }

        public async Task<Recipe> CreateRecipe(Recipe recipe)
        {
            var recipeId = Guid.NewGuid().ToString("N");
            await client.SetAsync($"recipes/{recipeId}", recipe);

            recipe.Id = recipeId;
            return recipe;
        }

        public async Task<List<Recipe>> GetRecipesByCategory(string category)
        {
            var response = await client.GetAsync("recipes");
            var recipes = response.ResultAs<Dictionary<string, Recipe>>();

            return recipes.Values.Where(r => r.Category == category).ToList();
        }

        public async Task<List<Recipe>> GetRecipesByIngredients(List<string> ingredients)
        {
            var response = await client.GetAsync("recipes");
            var recipes = response.ResultAs<Dictionary<string, Recipe>>();

            var matchingRecipes = recipes.Values.Where(r => ingredients.Any(i => r.Ingredients.Any(ri => ri.Contains(i, StringComparison.OrdinalIgnoreCase)))).ToList();
            return matchingRecipes;
        }

        public async Task<List<Recipe>> SearchRecipesByName(string recipeName)
        {
            var response = await client.GetAsync("recipes");
            var recipes = response.ResultAs<Dictionary<string, Recipe>>();

            var matchingRecipes = recipes.Values.Where(r => r.Name.Contains(recipeName, StringComparison.OrdinalIgnoreCase)).ToList();
            return matchingRecipes;
        }

        public async Task<List<Recipe>> SearchRecipesByUserId(string uidUser)
        {
            var response = await client.GetAsync("recipes");
            var recipes = response.ResultAs<Dictionary<string, Recipe>>();

            var matchingRecipes = recipes.Values.Where(r => r.UidUser == uidUser).ToList();
            return matchingRecipes;
        }


    }
}
