﻿using System.Collections.Generic;

namespace CharacterBuilder.Core.Model
{
    public class WeaponProperty
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IList<Weapon> Weapons { get; set; }
    }
}
