using Rocket.API;
using SteelSeries.GameSense.DeviceZone;

namespace Lafalafa.L4OwnIdentity
{
    public class RPCharacterPluginConfig : IRocketPluginConfiguration
    {

        public string Logo { get; set; }
        public string Fondo { get; set; }
        public bool DisableNames{ get; set; }
        public bool ChangeNames { get; set; }
        public ushort EffectRegister{ get; set; }
        public ushort EffectDni { get; set; }
        public void LoadDefaults()
        {

            Logo = "https://imgur.com/BaAHFLo.png";
            Fondo = "https://wallpaperaccess.com/full/1782498.jpg";
            DisableNames = true;
            ChangeNames = false;
            EffectRegister = 11034;
            EffectDni = 11021;
        }


    }
}