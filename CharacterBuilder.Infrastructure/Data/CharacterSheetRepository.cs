﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CharacterBuilder.Core.DTO;
using CharacterBuilder.Core.Model;
using CharacterBuilder.Core.Model.User;
using CharacterBuilder.Infrastructure.Data.Contexts;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CharacterBuilder.Infrastructure.Data
{
    public class CharacterSheetRepository
    {
        private readonly CharacterBuilderDbContext _db;
        private readonly UserManager<ApplicationUser> _manager;

        public CharacterSheetRepository()
        {
            _db = new CharacterBuilderDbContext();
            _manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_db));
        }

        public CharacterSheet CreateNewSheet(string userId)
        {
            var currentUser = _manager.FindById(userId);
            var sheet = new CharacterSheet
            {
                User = currentUser
            };

            _db.CharacterSheets.Add(sheet);

            Save();

            return sheet;
        }

        public CharacterSheet CreateNewSheetWithClass(string userId, int classId)
        {
            var currentUser = _manager.FindById(userId);
            var clsFromDb = _db.Classes
                .Include(s => s.Skills)
                .Include(f => f.Features)
                .Single(c => c.Id == classId);


            var sheet = new CharacterSheet
            {
                User = currentUser,
                ToDo = new ToDo
                {
                    HasSelectedClass = true
                },
                CreatedDate = DateTime.UtcNow,
                Class = clsFromDb,
            };

            _db.CharacterSheets.Add(sheet);

            Save();

            return sheet;
        }

        public SkillDto GetSkillsBySheetId(int sheetId)
        {
            var sheetDb = _db.CharacterSheets
                .Include(s => s.Skills.Select(a => a.AbilityScore))
                .Include(c => c.Class.Skills)
                .Include(b => b.Background.Skills.Select(a=>a.AbilityScore))
                .Single(x => x.Id == sheetId);

            var allSkills = ListAllSkills();
            var pickCount = sheetDb.Class?.SkillPickCount + sheetDb.Background?.Skills.Count;

            var mapped = new SkillDto
            {
                AllSkills = allSkills,
                Skills = sheetDb.Skills.Select(item => item.Id).ToList(),
                SkillProficiencies = sheetDb.Class?.Skills ?? new List<Skill>(),
                BackgroundSkills = sheetDb.Background?.Skills.Select(item => item.Id).ToList() ?? new List<int>(),
                SkillPickCount = pickCount ?? 0
            };

            return mapped;
        }

        public IList<CharacterSheet> GetUserSheets(string userId)
        {
            var currentUser = _manager.FindById(userId);

            return _db.CharacterSheets
                .Include(t => t.ToDo)                
                .Include(c => c.Class.Skills.Select(a => a.AbilityScore))
                .Include(f => f.Class.Features)
                .Include(b => b.Background.Skills.Select(a => a.AbilityScore))
                .Include(r => r.Race)
                .Include(sr => sr.Subrace)
                .Include(i => i.AbilityScoreIncreases.Select(a => a.AbilityScore))
                .Include(l => l.LevelChecklists)
                .Include(cs => cs.Skills)
                .Where(x => x.User.Id == currentUser.Id).ToList();          
        }
        
        public CharacterSheet GetCharacterSheetById(int sheetId)
        {
            return _db.CharacterSheets.Include(t => t.ToDo)
                .Include(t => t.ToDo)
                .Include(c => c.Class.Skills.Select(a => a.AbilityScore))
                .Include(f => f.Class.Features)
                .Include(b => b.Background.Skills.Select(a => a.AbilityScore))
                .Include(r => r.Race)
                .Include(sr => sr.Subrace)
                .Include(i => i.AbilityScoreIncreases.Select(a => a.AbilityScore))
                .Include(l => l.LevelChecklists)
                .Include(cs => cs.Skills)
                .Include(u => u.User)
                .Single(s => s.Id == sheetId);
        }

        public CharacterSheet GetCharacterSheetByIdNoInludes(int sheetId)
        {
            return _db.CharacterSheets.Include(t => t.ToDo)
                .Include(t => t.ToDo)
                .Include(f => f.Class.Features)
                .Include(b => b.Background.Skills.Select(a => a.AbilityScore))
                .Include(r => r.Race)
                .Include(sr => sr.Subrace)
                .Include(i => i.AbilityScoreIncreases.Select(a => a.AbilityScore))
                .Include(l => l.LevelChecklists)
                .Include(cs => cs.Skills)
                .Include(u => u.User)
                .Single(s => s.Id == sheetId);
        }

        public LevelChecklist AddLevelChecklist(int sheetId)
        {
            var sheetFromDb = GetCharacterSheetById(sheetId);
            var chkListToAdd = new LevelChecklist
            {
                CharacterSheet = sheetFromDb,
                Level = sheetFromDb.ClassLevel
            };

            _db.LevelChecklists.Add(chkListToAdd);
            Save();

            return chkListToAdd;
        }

        public IList<ProficiencyBonus> GetProficiencyBonusesByClassId(int classId)
        {
            return classId == 0 ? new List<ProficiencyBonus>() : _db.ProficiencyBonuses.Where(p => p.ClassId == classId).ToList();
        }

        public IList<Skill> ListAllSkills()
        {
            return _db.Skills.ToList();
        }

        public Skill GetSkillById(int skillId)
        {
            return _db.Skills.Single(s => s.Id == skillId);
        }

        public LevelChecklist GetLevelChecklistBySheetId(int sheetId, int sheetLevel)
        {
            return _db.LevelChecklists.SingleOrDefault(l => l.CharacterSheet.Id == sheetId && l.Level == l.CharacterSheet.ClassLevel);
        }

        public void DeleteSheetAndToDoList(int characterSheetId)
        {
            var sheetFromDb = _db.CharacterSheets
                .Include(t => t.ToDo)
                .Single(s => s.Id == characterSheetId);

            var toDoToDelete = sheetFromDb.ToDo;

            _db.CharacterSheets.Remove(sheetFromDb);
            _db.ToDos.Remove(toDoToDelete);

            Save();
        }

        public void Update<T>(T entity) where T : class
        {
            _db.Entry(entity).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public void Save()
        {
            _db.SaveChanges();
        }

    }
}
