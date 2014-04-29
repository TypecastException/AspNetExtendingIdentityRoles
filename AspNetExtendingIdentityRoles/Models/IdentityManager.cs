﻿using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using AspNetExtendingIdentityRoles.Models;
using System.Linq;

namespace AspNetExtendingIdentityRoles.Models
{
    public class IdentityManager
    {
        // Swap ApplicationRole for IdentityRole:
        RoleManager<ApplicationRole> _roleManager = new RoleManager<ApplicationRole>(
            new RoleStore<ApplicationRole>(new ApplicationDbContext()));

        UserManager<ApplicationUser> _userManager = new UserManager<ApplicationUser>(
            new UserStore<ApplicationUser>(new ApplicationDbContext()));

        ApplicationDbContext _db = new ApplicationDbContext();


        public bool RoleExists(string name)
        {
            return _roleManager.RoleExists(name);
        }


        public bool CreateRole(string name, string description = "")
        {
            // Swap ApplicationRole for IdentityRole:
            var idResult = _roleManager.Create(new ApplicationRole(name, description));
            return idResult.Succeeded;
        }


        public bool CreateUser(ApplicationUser user, string password)
        {
            var idResult = _userManager.Create(user, password);
            return idResult.Succeeded;
        }


        public bool AddUserToRole(string userId, string roleName)
        {
            var idResult = _userManager.AddToRole(userId, roleName);
            return idResult.Succeeded;
        }


        public void ClearUserRoles(string userId)
        {
            var user = _userManager.FindById(userId);
            var currentRoles = new List<IdentityUserRole>();
            
            var Db = new ApplicationDbContext();

            // Add all available roles to the list of EditorViewModels:
            var allRoles = Db.Roles;

            currentRoles.AddRange(user.Roles);
            foreach (var role in currentRoles)
            {
                var roleName = allRoles.First(r => r.Id == role.RoleId).Name;
                _userManager.RemoveFromRole(userId, roleName);
            }
        }


        public void RemoveFromRole(string userId, string roleName)
        {
            _userManager.RemoveFromRole(userId, roleName);
        }


        public void DeleteRole(string roleId)
        {
            var roleUsers = _db.Users.Where(u => u.Roles.Any(r => r.RoleId == roleId));
            var role = _db.Roles.Find(roleId);

            foreach (var user in roleUsers)
            {
                this.RemoveFromRole(user.Id, role.Name);
            }
            _db.Roles.Remove(role);
            _db.SaveChanges();
        }
    }
}