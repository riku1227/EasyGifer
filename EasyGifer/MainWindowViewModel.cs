using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyGifer
{
    class MainWindowViewModel
    {
        public String InpuPath { get; set; } = "";
        public String OutputPath { get; set; } = "";
        public String GifWidth { get; set; } = "640";
        public String GifFps { get; set; } = "30";
    }
}
