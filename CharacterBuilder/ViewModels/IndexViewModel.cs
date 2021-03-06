﻿namespace CharacterBuilder.ViewModels
{
    public class IndexViewModel
    {
        public string SheetId { get; set; }
        public int ClassId { get; set; }
        public bool HasSelectedClass { get; set; }
        public bool HasSelectedBackground { get; set; }
        public bool HasSelectedRace { get; set; }
        public int RaceId { get; set; }
        public int BackgroundId { get; set; }
        public int ClassLevel { get; set; }
        public string UserName { get; set; }
    }
}