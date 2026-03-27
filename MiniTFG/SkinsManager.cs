using System;
using System.Collections.Generic;
using System.Text;

namespace MiniTFG
{
    public static class SkinsManager
    {
        public static List<Skin> BannersDisponibles { get; set; } = new List<Skin>
        {
            new Skin { Id = "Skin1", Image = "brbabanner.jpg", Precio = 100, Comprado = false },
            new Skin { Id = "Skin2", Image = "bcsbanner.jpg", Precio = 200, Comprado = false },
            new Skin { Id = "Skin3", Image = "peakybanner.jpg", Precio = 300, Comprado = false },
            new Skin { Id = "Skin4", Image = "opbanner.jpg", Precio = 400, Comprado = false },
            new Skin { Id = "Skin5", Image = "dbbanner.jpg", Precio = 500, Comprado = false }
        };

        public static List<Skin> FotosDisponibles { get; set; } = new List<Skin>
        {
            new Skin { Id = "Foto1", Image = "heisenberg.jpg", Precio = 150, Comprado = false },
            new Skin { Id = "Foto2", Image = "sanji.jpg", Precio = 250, Comprado = false },
            new Skin { Id = "Foto3", Image = "house.jpg", Precio = 350, Comprado = false },
            new Skin { Id = "Foto4", Image = "arguinano.jpg", Precio = 450, Comprado = false },
            new Skin { Id = "Foto5", Image = "chicote.jpg", Precio = 550, Comprado = false }
        };
    }
}
