using LiteDB;
using Rocket.Unturned.Player;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Lafalafa.L4OwnIdentity.Extras
{


    public class DbRP
    {
        private static string path;

        public static void initializeDatabase(string path)
        {
            DbRP.path = path;
            using (LiteDatabase db = new LiteDatabase(path))
            {
                var col = db.GetCollection<Character>("characters");                   
                //Character character = new Character("Lafa", "Fafa", "Hombre", new DateTime(2020, 09 / 11), new CSteamID(76561198280429981));
                //{

                //    Name = "Lafa",
                //    Lastname = "Fafa",
                //    Sex = "Hombre",
                //    Birth = new DateTime(2020, 09 / 11),
                //    _id = 

                //};
                //getTable().Insert(character);

            }
            
        }


    //    public static bool isPlayerRegistered(UnturnedPlayer p)
    //    {
    //        UnturnedPlayer player = p;
            
    //        using (LiteDatabase db = new LiteDatabase(path))
    //        {
                
    //            var col = db.GetCollection<Character>("characters");
               

    //            var r = col.FindOne(x => x._id.Contains(player.CSteamID.m_SteamID.ToString()));
    //            Console.WriteLine(r.Name);
    //            if (col.FindById(player.CSteamID.m_SteamID.ToString()) == null)
    //            {
    //                Console.WriteLine("2");
    //                return false;
    //            }
    //            else
    //            {
    //                Console.WriteLine("1");
    //                return true;
                   
    //            }

    //    }

    //}

    public static void addCharacter(Character character)
        {

            using (LiteDatabase db = new LiteDatabase(path))
            {
            
                 var col = db.GetCollection<Character>("characters");
                
                if (col.FindById(character._id) == null)
                  {
                     
                     col.Insert(character);
                 }
            }

        }

        public static Character getCharacter(UnturnedPlayer player) 
        {

            using (LiteDatabase db = new LiteDatabase(path))
            {

                var col = db.GetCollection<Character>("characters");
                return col.FindById(player.CSteamID.m_SteamID.ToString());
            }
        }

    }
}
