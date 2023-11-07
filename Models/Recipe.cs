using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading;

namespace App_Xamarin_Firebase.Models
{

    public class Recipe
    {
       
        public string id { get; set; }
        public string Name { get; set; }
        public string time { get; set; }
        public byte[]? Image { get; set; }
        public List<string> Ingredients { get; set; }
        public string Category { get; set; }
        public List<string> PreparationSteps { get; set; }
        public string UidUser { get; set; }
    }

}
