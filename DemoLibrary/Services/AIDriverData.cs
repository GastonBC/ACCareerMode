using DBLink.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DBLink.Services
{
    public static class AIDriverData
    {

        public static AIDriver GenAIDriver(int seed)
        {
            Random rd = new Random(seed);
            return new AIDriver(DriverNames[rd.Next(DriverNames.Count)]);
        }


        internal static List<string> DriverNames = new List<string>
        {
            "Harley Hill",
            "Bobby Henderson",
            "Jonathan Burton",
            "Alexander Morris",
            "Morgan Duncan",
            "Kade Hoffman",
            "Ayaan Henson",
            "Tyree Lyons",
            "Maxwell Chase",
            "Chance Rosales",
            "Nathan Ward",
            "Samuel Palmer",
            "Oscar Young",
            "Samuel Dawson",
            "Harry Elliott",
            "Sawyer Saunders",
            "Blaze Serrano",
            "Jamal Mills",
            "Carlos Potter",
            "Hendrix Mcmahon",
            "Isaac Newman",
            "Blake Robertson",
            "Corey George",
            "Morgan Armstrong",
            "William Mccarthy",
            "Chase Hayes",
            "Pablo Hernandez",
            "Francis Daugherty",
            "Urijah Robbins",
            "Brentley Reeves",
            "Mia Powell",
            "Madeleine Baxter",
            "Maisy Turner",
            "Mya Byrne",
            "Maddison Pearson",
            "Juliana Gilbert",
            "Kristina Gonzalez",
            "Kaylin Serrano",
            "Lucille Blanchard",
            "Jimena Mullen",
            "Natasha Owen",
            "Kate Scott",
            "Eve Hussain",
            "Imogen Baker",
            "Courtney Lawrence",
            "Ariana Tate",
            "Kyra Reese",
            "Eileen Frank",
            "Harley Harrington",
            "Kassandra Lopez",
            "Jasmine Lewis",
            "Matilda Johnston",
            "Sophia Ross",
            "Faith Evans",
            "Chloe Webb",
            "Marianna Long",
            "Beatrice Gray",
            "Karis Barton",
            "Katrina Moreno",
            "Elin Dorsey",

        };
    }
}
