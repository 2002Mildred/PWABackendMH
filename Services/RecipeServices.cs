using FireSharp.Interfaces;
using FireSharp;
using System.Collections.Generic;
using System.Threading.Tasks;
using FireSharp.Config;
using App_Xamarin_Firebase.Models;
using System.Linq;
using System;
using FireSharp.Response;

namespace App_Xamarin_Firebase.Services
{
    public class RecipeService
    {
        private readonly FirebaseClient client;

        public  RecipeService()
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
                id= r.Key,
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

            recipe.id = recipeId;
            return recipe;
        }



        public async Task<List<Recipe>> GetRecipesByCategory(string category)
        {
            var response = await client.GetAsync("recipes");
            var recipes = response.ResultAs<Dictionary<string, Recipe>>();

            var recipesWithKeys = recipes.Where(kv => kv.Value.Category == category)
                                         .Select(kv => new Recipe
                                         {
                                             id = kv.Key,
                                             Name = kv.Value.Name,
                                             time = kv.Value.time,
                                             Image = kv.Value.Image,
                                             Ingredients = kv.Value.Ingredients,
                                             Category = kv.Value.Category,
                                             PreparationSteps = kv.Value.PreparationSteps,
                                             UidUser = kv.Value.UidUser
                                         })
                                         .ToList();

            return recipesWithKeys;
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

        public async Task<Recipe> GetRecipeById(string id)
        {
            try
            {
                FirebaseResponse response = await client.GetAsync($"recipes/{id}");

                if (response.Body != "null")
                {
                    var recipes = response.ResultAs<Dictionary<string, Recipe>>();

                    var matchingRecipes = recipes.Values.ToList();
                    return matchingRecipes.FirstOrDefault();



                }
               
                    // Handle the case where the recipe with the provided ID doesn't exist.
                    return null;
                
            }
            catch (Exception ex)
            {
                // Handle any potential exceptions here.
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
    }


    }
}
