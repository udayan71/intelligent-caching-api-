using Application.Constants;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Seed
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            await SeedRolesAsync(context);
            await SeedPermissionsAsync(context);
            await SeedRolePermissionsAsync(context);
            await SeedAdminUserAsync(context);
        }

        private static async Task SeedRolesAsync(AppDbContext context)
        {
            if (await context.Roles.AnyAsync())
                return;

            var roles = new List<Role>
            {
                new()
                {
                    Name = Roles.Admin,
                    Description = "Full system access"
                },
                new()
                {
                    Name = Roles.Manager,
                    Description = "Product management access"
                },
                new()
                {
                    Name = Roles.Viewer,
                    Description = "Read-only access"
                }
            };

            await context.Roles.AddRangeAsync(roles);
            await context.SaveChangesAsync();
        }

        private static async Task SeedPermissionsAsync(AppDbContext context)
        {
            if (await context.Permissions.AnyAsync())
                return;

            var permissions = new List<Permission>
            {
                new()
                {
                    Name = "View Product",
                    Code = Permissions.Products.View,
                    Description = "Allows viewing products"
                },
                new()
                {
                    Name = "Create Product",
                    Code = Permissions.Products.Create,
                    Description = "Allows creating products"
                },
                new()
                {
                    Name = "Update Product",
                    Code = Permissions.Products.Update,
                    Description = "Allows updating products"
                },
                new()
                {
                    Name = "Delete Product",
                    Code = Permissions.Products.Delete,
                    Description = "Allows deleting products"
                }
            };

            await context.Permissions.AddRangeAsync(permissions);
            await context.SaveChangesAsync();
        }

        private static async Task SeedRolePermissionsAsync(AppDbContext context)
        {
            if (await context.RolePermissions.AnyAsync())
                return;

            var adminRole = await context.Roles
                .FirstAsync(r => r.Name == Roles.Admin);

            var managerRole = await context.Roles
                .FirstAsync(r => r.Name == Roles.Manager);

            var viewerRole = await context.Roles
                .FirstAsync(r => r.Name == Roles.Viewer);

            var permissions = await context.Permissions.ToListAsync();

            var mappings = new List<RolePermission>();

            // Admin -> All permissions
            mappings.AddRange(
                permissions.Select(permission =>
                    new RolePermission
                    {
                        RoleId = adminRole.Id,
                        PermissionId = permission.Id
                    }));

            // Manager -> View, Create, Update
            mappings.AddRange(
                permissions
                    .Where(p =>
                        p.Code == Permissions.Products.View ||
                        p.Code == Permissions.Products.Create ||
                        p.Code == Permissions.Products.Update)
                    .Select(permission =>
                        new RolePermission
                        {
                            RoleId = managerRole.Id,
                            PermissionId = permission.Id
                        }));

            // Viewer -> View only
            mappings.AddRange(
                permissions
                    .Where(p =>
                        p.Code == Permissions.Products.View)
                    .Select(permission =>
                        new RolePermission
                        {
                            RoleId = viewerRole.Id,
                            PermissionId = permission.Id
                        }));

            await context.RolePermissions.AddRangeAsync(mappings);

            await context.SaveChangesAsync();
        }

        private static async Task SeedAdminUserAsync(
    AppDbContext context)
        {
            if (await context.Users.AnyAsync())
                return;

            var adminRole = await context.Roles
                .FirstAsync(r => r.Name == Roles.Admin);

            var adminUser = new User
            {
                FirstName = "System",
                LastName = "Administrator",
                Email = "admin@intelligentcache.com",
                PasswordHash =
                    BCrypt.Net.BCrypt.HashPassword(
                        "Admin@123"),
                IsActive = true
            };

            await context.Users.AddAsync(adminUser);

            await context.SaveChangesAsync();

            await context.UserRoles.AddAsync(
                new UserRole
                {
                    UserId = adminUser.Id,
                    RoleId = adminRole.Id
                });

            await context.SaveChangesAsync();
        }
    }
}
