using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lafalafa.L4OwnIdentity.Extras
{


    public class Character
    {
        public string Name { get; set; }
        public string Lastname { get; set; }
        public bool Sex { get; set; }
        public DateTime Birth { get; set; }
        public string _id { get; set; }


        public Character(string Name, string Lastname, string Sex, DateTime Birth, CSteamID steamID)
        {
            _id = steamID.m_SteamID.ToString();
            this.Name = Name;
            this.Lastname= Lastname;
            this.Sex = false;
            this.Birth = Birth;
        
        }

        public string[] getParameters()
        {
            string sex;
            if (this.Sex == false)
            {
                sex = "Male";
            }
            else {
                sex = "Female";
            }
            string[] parameters = new string[] { this.Name, this.Lastname ,this.Birth.Date.ToString(), sex};

            return parameters;
        }

        public Character()
        { 
        
        
        }
    }
}
