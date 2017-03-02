﻿using System.Collections.Generic;

namespace CharacterBuilder.Core.Model
{
    public class Background
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Gold { get; set; }
        public IList<BackgroundCharacteristic> BackgroundCharacteristic { get; set; }
        public IList<Skill>Skills { get; set; }
        public int LanguageCount { get; set; } //How  Many they get
        public IList<Language> Languages { get; set; }  //From what pool they get them
        public BackgroundVariant BackgroundVariant { get; set; }
    }
}

//public List<List<object>> equipleft { get; set; }
//public List<List<object>> equipright { get; set; }
//public string feature { get; set; }
//public List<object> variant { get; set; }
//public string lifestyle { get; set; }
//public List<string> extra { get; set; }
//public List<string> tools { get; set; }
//public List<object> source { get; set; }
//public string skillstxt { get; set; }