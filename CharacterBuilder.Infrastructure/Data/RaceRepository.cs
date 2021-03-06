﻿using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CharacterBuilder.Core.Model;
using CharacterBuilder.Infrastructure.Data.Contexts;

namespace CharacterBuilder.Infrastructure.Data
{
    public class RaceRepository
    {
        private readonly CharacterBuilderDbContext _db;

        public RaceRepository()
        {
            _db = new CharacterBuilderDbContext();
        }

        public IList<Race> GetAllRaces()
        {
            return _db.Races.Include(r => r.AbilityScoreIncreases).Include(s => s.Subraces).ToList();
        }

        public Race GetRaceById(int raceId)
        {
            return _db.Races
                .Include(a=>a.AbilityScoreIncreases)
                .Single(r => r.Id == raceId);
        }

        public CharacterSheet SaveRaceSelection(int sheetId, int raceId)
        {
            var raceFromDb = _db.Races.Include(a=>a.AbilityScoreIncreases.Select(y=>y.AbilityScore)).Single(r => r.Id == raceId);
            var sheetFromDb = _db.CharacterSheets
                .Include(t=>t.ToDo)
                .Single(s => s.Id == sheetId);

            sheetFromDb.Race = raceFromDb;
            sheetFromDb.ToDo.HasSelectedRace = true;
            foreach (var item in raceFromDb.AbilityScoreIncreases)
            {
                var increaseFromDb = _db.AbilityScoreIncreases.Single(i => i.Id == item.Id);
                sheetFromDb.AbilityScoreIncreases.Add(increaseFromDb);
            }
            
            Save();

            return sheetFromDb;
        }

        public CharacterSheet SaveSubRaceSelection(int sheetId, int subraceId)
        {
            var subraceFromDb = _db.Subraces.Include(a => a.AbilityScoreIncreases.Select(y => y.AbilityScore)).Single(s => s.Id == subraceId);
            var sheetFromDb = _db.CharacterSheets
                .Include(t=>t.ToDo)
                .Single(s => s.Id == sheetId);

            sheetFromDb.Subrace = subraceFromDb;
            sheetFromDb.ToDo.HasSelectedSubRace = true;
            foreach (var item in subraceFromDb.AbilityScoreIncreases)
            {
                var increaseFromDb = _db.AbilityScoreIncreases.Single(i => i.Id == item.Id);
                sheetFromDb.AbilityScoreIncreases.Add(increaseFromDb);
            }

            Save();

            return sheetFromDb;
        }

        public List<AbilityScoreIncrease> GetByRaceId(int raceId)
        {
            return _db.Races.Single(r => r.Id == raceId).AbilityScoreIncreases.ToList();
        }

        public void Save()
        {
            _db.SaveChanges();
        }

    }
}
