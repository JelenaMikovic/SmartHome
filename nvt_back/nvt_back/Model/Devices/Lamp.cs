﻿using nvt_back.Model.Devices;
using System.ComponentModel.DataAnnotations;

namespace nvt_back.Model.Devices
{
    public class Lamp : Device
    {
        public bool IsOn { get; set; }
        [Required(ErrorMessage = "Color field is required")]
        [Range(0, 100, ErrorMessage = "Brightness should be between 0 and 100")]
        public int BrightnessLevel { get; set; }

        [Required(ErrorMessage = "Color field is required")]
        [Range(0, 100, ErrorMessage = "Brightness should be between 0 and 100")]
        public LampColor Color { get; set; }
    }
}
